using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Middleware;

/// <summary>
///     This class gets inserted into the request pipeline and validates all requests that require an api key.
/// </summary>
public class ApiKeyMiddleware
{
	public static readonly string GetPracticesApi = "/api/v1/GetPractices";
	private readonly AppSettings _appSettings;
	private readonly RequestDelegate _next;
	private readonly VendorQuery _vendorQuery;

	public ApiKeyMiddleware(IConfiguration configuration, VendorQuery vendorQuery, RequestDelegate next)
	{
		_appSettings = configuration.Get<AppSettings>();
		_vendorQuery = vendorQuery;
		_next = next;
	}

	/// <summary>
	///     Invoke the middleware on a request to require api key authentication
	/// </summary>
	/// <param name="context"></param>
	/// <returns></returns>
	public async Task Invoke(HttpContext context)
	{
		string apiKey = null, officeCode = null;
		var keyRequired = false;
		var systemOnly = false;
		if (context.Request.Path == "/api/User/AuthenticateAd" ||
		    context.Request.Path.StartsWithSegments("/api/v1") ||
		    context.Request.Path.StartsWithSegments("/datasync"))
		{
			keyRequired = true;
			if (context.Request.Path.Value.Contains("SqlConnectionDetails")) systemOnly = true;
			apiKey = context.Request.Headers.GetValue("Authorization").ToString();
			officeCode = context.Request.Headers.GetValue(StringConstants.OfficeCode).ToString();
		}
		// webhooks pass these as query parameters
		else if (context.Request.Path.StartsWithSegments(DbContextResolver.WebHooksApi))
		{
			keyRequired = true;
			if (context.Request.Query.TryGetValue("key", out var key)) apiKey = key.ToString();
			if (context.Request.Query.TryGetValue("officeCode", out var code)) officeCode = code.ToString();
		}
		else if (context.Request.Path.StartsWithSegments(DbContextResolver.PatientSchedulerApi))
		{
			var callerIp = context.Connection.RemoteIpAddress.ToString();
			// TODO: IP Address filtering
		}

		if (context.Request.Path.StartsWithSegments("/patient-schedule") ||
		    context.Request.Path.StartsWithSegments("/patient-schedule-complete"))
		{
			if (context.Request.Query.TryGetValue("officeCode", out var code))
			{
				officeCode = code.ToString();
				await _next.Invoke(context);
				return;
			}

			context.Response.StatusCode = 401; // Unauthorized
			return;
		}

		if (context.Request.Path == "/scheduler" && context is DefaultHttpContext httpContext)
		{
			var request = httpContext.Request;

			if (request.Cookies.Count > 0)
			{
				var cookie = request.Cookies.FirstOrDefault(x => x.Key == ".AspNetCore.Cookies");
				if (!string.IsNullOrEmpty(cookie.Key))
				{
					await _next.Invoke(httpContext);
					return;
				}
			}

			context.Response.StatusCode = 401; // Unauthorized
			return;
		}

		if (keyRequired)
			await Validate(context, systemOnly, apiKey, officeCode);
		else
			await _next.Invoke(context);
	}

	public void LogRequest(string requestPath, bool accessGranted, string vendorName, string officeCode,
		string callerIp)
	{
		Log.Information(
			$"{(accessGranted ? "Accepted" : "Denied")} request to {requestPath}. Vendor: {vendorName}, Customer: {officeCode}, Caller Ip: {callerIp}");
	}

	public async Task Validate(HttpContext context, bool systemOnly, string apiKey, string officeCode)
	{
		// Ensure api key exists
		if (string.IsNullOrWhiteSpace(apiKey))
		{
			context.Response.StatusCode = 400; //Bad Request                
			await context.Response.WriteAsync("Api Key is missing");
			return;
		}

		// Ensure office code exists
		if (string.IsNullOrWhiteSpace(officeCode) &&
		    context.Request.Path != "/api/Internal/ValidateDatabaseConnection" &&
		    context.Request.Path != GetPracticesApi)
		{
			context.Response.StatusCode = 400; //Bad Request                
			await context.Response.WriteAsync("Office Code is missing");
			return;
		}

		var callerIp = context.Connection.RemoteIpAddress.ToString();
		// Grant access to a internal api calls
		if (apiKey == _appSettings.Keys.ApiKey)
		{
			// Log successful api request
			LogRequest(context.Request.Path, true, "Tims", officeCode, callerIp);
			await _next.Invoke(context);
			return;
		}

		// Find vendor with matching key
		var vendorList = await _vendorQuery.GetVendorsAsync();
		var vendor = vendorList.FirstOrDefault(x => x.ApiKey == apiKey);

		// Ensure vendor exists
		if (vendor == null)
		{
			LogRequest(context.Request.Path, false, "Unknown", officeCode, callerIp);
			context.Response.StatusCode = 401; // Unauthorized
			await context.Response.WriteAsync("Invalid Api Key");
			return;
		}

		// only tims api key allowed

		if (systemOnly)
		{
			LogRequest(context.Request.Path, false, vendor.Name, officeCode, callerIp);
			return;
		}

		// Make sure vendor has permission to use this api endpoint with this customer
		var hasPermission = false;
		var requestPath = context.Request.Path.ToString().ToLower();


		if (context.Request.Path == GetPracticesApi)
		{
			hasPermission = true;
			var audigyCustomers = AudigyCustomers.GetCustomers();
			foreach (var audigyCustomerOfficeCode in audigyCustomers.Keys)
			{
				var hasPermissionForThisCustomer = false;
				// Loop through permission bindings on vendor's record
				foreach (var customerVendorPermission in vendor.CustomerPermissions)
					// Only check permission bindings to the customer with matching office code
					if (string.Equals(customerVendorPermission.Customer.OfficeCode, audigyCustomerOfficeCode,
						    StringComparison.CurrentCultureIgnoreCase))
					{
						// grab api endpoints from the permission object
						var urls = customerVendorPermission.Permission.ApiUrls.Select(x => x.ApiUrl.Url.ToLower())
							.ToList();

						// check if the request path is one of the allowed paths
						if (urls.Contains(requestPath))
						{
							hasPermissionForThisCustomer = true;
							break;
						}
					}

				if (!hasPermissionForThisCustomer)
				{
					hasPermission = false;
					break;
				}
			}
		}
		else
		{
			// Loop through permission bindings on vendor's record
			foreach (var customerVendorPermission in vendor.CustomerPermissions)
				// Only check permission bindings to the customer with matching office code
				if (string.Equals(customerVendorPermission.Customer.OfficeCode, officeCode,
					    StringComparison.CurrentCultureIgnoreCase))
				{
					// grab api endpoints from the permission object
					var urls = customerVendorPermission.Permission.ApiUrls.Select(x => x.ApiUrl.Url.ToLower())
						.ToList();

					// check if the request path is one of the allowed paths
					if (urls.Contains(requestPath))
					{
						hasPermission = true;
						break;
					}
				}
		}


		// Ensure vendor has permission
		if (!hasPermission)
		{
			LogRequest(context.Request.Path, false, vendor.Name, officeCode, callerIp);
			context.Response.StatusCode = 403; // Forbidden
			await context.Response.WriteAsync(
				$"Access denied to endpoint '{context.Request.Path}' for customer '{officeCode}'");
			return;
		}

		// Log successful api request
		LogRequest(context.Request.Path, true, vendor.Name, officeCode, callerIp);

		await _next.Invoke(context);
	}
}