using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Middleware;

/// <summary>
///     This class gets inserted into the request pipeline and validates all requests that require an api key.
/// </summary>
public class ChangePasswordMiddleware
{
	private readonly AppSettings _appSettings;
	private readonly RequestDelegate _next;
	private readonly VendorQuery _vendorQuery;

	public ChangePasswordMiddleware(IConfiguration configuration, VendorQuery vendorQuery, RequestDelegate next)
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
		// Do not redirect user if they are changing password or logging out.
		if (context.Request.Path == "/Account/ChangePassword" || context.Request.Path == "/web/User/SignOut")
			return;

		// If user has "CP" claim, they must be redirected to Change Password screen.
		if (context.User.HasClaim(StringConstants.ChangePassword, "1"))
			//context.Result = new RedirectToPageResult("/Account/ChangePassword");
			Console.WriteLine("BAM");

		await _next.Invoke(context);
	}
}