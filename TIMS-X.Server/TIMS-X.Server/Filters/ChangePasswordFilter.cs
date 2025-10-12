using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TIMS_X.Core.Utils;

namespace TIMS_X.Server.Filters;

/// <summary>
///     This filter gets injected into the Http request pipeline and every request runs through it.
///     It won't let the user continue until they change their password.
/// </summary>
public class ChangePasswordFilter : IAuthorizationFilter
{
	/// <summary>
	///     Checks if the user cookie contains claim "CP" (Change Password).
	///     If true, forces user to change password page.
	/// </summary>
	/// <param name="context"></param>
	public void OnAuthorization(AuthorizationFilterContext context)
	{
		// Do not redirect user if they are changing password or logging out.
		if (context.HttpContext.Request.Path == "/Account/ChangePassword" ||
		    context.HttpContext.Request.Path == "/web/User/SignOut")
			return;

		// If user has "CP" claim, they must be redirected to Change Password screen.
		if (context.HttpContext.User.HasClaim(StringConstants.ChangePassword, "1"))
			context.Result = new RedirectToPageResult("/Account/ChangePassword");
	}
}