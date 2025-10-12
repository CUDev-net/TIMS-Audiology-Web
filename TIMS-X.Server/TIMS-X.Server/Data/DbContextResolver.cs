using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Middleware;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Utils;
using ConnectionInfo = TIMS_X.Core.Models.ConnectionInfo;

namespace TIMS_X.Server.Data;

public class DbContextResolver
{
	public static readonly string PingEndpoint = "/api/Customer/Ping";
	public static readonly string VendorApi = "/api/v1";
	public static readonly string VendorApi2 = "/datasync";
	public static readonly string ValidationApi = "/web/Validation";
	public static readonly string WebHooksApi = "/api/WebHooks";
	public static readonly string AwsWebHooksApi = "/api/AwsWebHooks";
	public static readonly string EmailResponseApi = "/api/EmailResponse";
	public static readonly string GetPracticesApi = "/api/v1/GetPractices";
	public static readonly string PatientSchedulerApi = "/api/PatientScheduler";
	public static readonly string DocumentRequest = "/Document";
	public static readonly string PatientSchedule = "/web/patientscheduling";


	public static readonly List<string> PublicEndpoints = new()
	{
		"/api/User/Authenticate",
		"/api/User/LoginUsers",
		"/api/Practice/Name",
		"/api/Practice/Sites",
		"/api/Practice/UsesAdAuthentication",
		"/api/Noah/ValidateLogin",
		"/api/Customer/ValidateCustomer",
		"/api/Customer/Ping"
	};

	protected readonly AppSettings _appSettings;
	private readonly ContextHelper _contextHelper;
	protected readonly CustomerQuery _customerQuery;
	protected HttpContext _httpContext;

	public DbContextResolver(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
		CustomerQuery customerQuery, ContextHelper contextHelper)
	{
		_appSettings = configuration.Get<AppSettings>();
		_httpContext = httpContextAccessor.HttpContext;
		_customerQuery = customerQuery;
		_contextHelper = contextHelper;
	}

	public void ResolveConnection(DbContextOptionsBuilder options)
	{
		options.EnableSensitiveDataLogging();
		string officeCode = null;
		if (_httpContext.Request.Path == PingEndpoint ||
		    _httpContext.Request.Path.StartsWithSegments(ValidationApi))
			return;

		if (_httpContext.Request.Path == "/web/User/SignOut") return;

		ConnectionInfo connInfo = null;

		if (_httpContext.Request.Path == "/Account/SignIn")
		{
			if (_httpContext.Request.Query.TryGetValue("Input.OfficeCode", out var oc))
			{
				officeCode = oc;
				if (!string.IsNullOrWhiteSpace(officeCode))
				{
					if (string.Equals(officeCode, StringConstants.Support, StringComparison.CurrentCultureIgnoreCase))
						return;
					connInfo = _customerQuery.GetConnectionInfoAsync(officeCode).Result;
					if (connInfo == null)
						return;
				}
			}
			else
			{
				return;
			}
		}

		else if (_httpContext.Request.Path.StartsWithSegments(WebHooksApi) ||
		         _httpContext.Request.Path.StartsWithSegments(AwsWebHooksApi) ||
		         _httpContext.Request.Path.StartsWithSegments(DocumentRequest) ||
		         _httpContext.Request.Path.StartsWithSegments(EmailResponseApi) ||
		         _httpContext.Request.Path.StartsWithSegments(GetPracticesApi) ||
		         _httpContext.Request.Path.StartsWithSegments("/ConfirmAppointment") ||
		         //ConfirmAppointment
		         _httpContext.Request.Path.StartsWithSegments(PatientSchedulerApi))
		{
			if (_httpContext.Request.Query.TryGetValue("officeCode", out var oc))
			{
				officeCode = oc;
				if (!string.IsNullOrWhiteSpace(officeCode))
					connInfo = _customerQuery.GetConnectionInfoAsync(officeCode).Result;
			}
			else
			{
				return;
			}
		}

		// Check if accessing public endpoint and if so require office code in header as well
		else if ((PublicEndpoints.Contains(_httpContext.Request.Path) || _httpContext.Request.Path.StartsWithSegments(
			                                                              VendorApi)
		                                                              || _httpContext.Request.Path.StartsWithSegments(
			                                                              VendorApi2)
		                                                              || _httpContext.Request.Path.StartsWithSegments(
			                                                              "/api/User/AuthenticateAd")) &&
		         _httpContext.Request.Headers.ContainsKey(StringConstants.OfficeCode))
		{
			officeCode = _httpContext.Request.Headers[StringConstants.OfficeCode].ToString();
			if (string.IsNullOrWhiteSpace(officeCode))
			{
				_httpContext.Response.StatusCode = 401; //Unauthorized
				_httpContext.Response.WriteAsync("Office code required");
				return;
			}

			connInfo = _customerQuery.GetConnectionInfoAsync(officeCode).Result;
		}
		// office code will be parsed out later
		else if (_httpContext.Request.Path.StartsWithSegments("/video") ||
		         _httpContext.Request.Path.StartsWithSegments("/PlayVideo"))
		{
			return;
		}

		else if (_httpContext.Request.Path.StartsWithSegments(PatientSchedule))
		{
			var encrypted = _httpContext.Request.Query["officeCode"];
			if (string.IsNullOrWhiteSpace(encrypted))
			{
				_httpContext.Response.StatusCode = 401; //Unauthorized
				_httpContext.Response.WriteAsync("Office code required");
				return;
			}

			var e = encrypted.ToString();
			var index = e.IndexOf(":");
			var key = e.Substring(0, index);
			var base64 = e.Substring(index + 1);
			var bytes = Convert.FromBase64String(base64);
			var t = Encoding.UTF8.GetString(bytes);
			officeCode = CryptographyHelper.Decrypt(t, key);
			connInfo = _customerQuery.GetConnectionInfoAsync(officeCode).Result;
		}

		// For every other scenario, extract the connection details from the JWT token or the cookie.
		else
		{
			officeCode = _httpContext.User.Claims
				.Where(c => c.Type == StringConstants.OfficeCode).Select(c => c.Value).FirstOrDefault();
			connInfo = _customerQuery.GetConnectionInfoAsync(officeCode).Result;
		}

		if (connInfo == null) throw new InvalidOperationException("Office code not found/invalid.");

		ConnectionStringBuilder.SetConnectionString(options, connInfo);

		_contextHelper.OfficeCode = officeCode;

		// -1 used as default because 0 is valid Support Id
		var userId = _httpContext.User.Claims.Where(c => c.Type == StringConstants.User).Select(c => int.Parse(c.Value))
			.DefaultIfEmpty(-1).First();
		var siteId = _httpContext.User.Claims.Where(c => c.Type == StringConstants.Site).Select(c => int.Parse(c.Value))
			.FirstOrDefault();

		if (userId != -1)
		{
			var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
			ConnectionStringBuilder.SetConnectionString(optionsBuilder, connInfo);
			var dbContext = new UserDbContext(optionsBuilder.Options);
			var queryInterface = new UserQuery(dbContext);
			_contextHelper.CurrentUser = queryInterface.Get(userId);
			_contextHelper.CurrentSite = queryInterface.GetSite(siteId);
		}
	}
}