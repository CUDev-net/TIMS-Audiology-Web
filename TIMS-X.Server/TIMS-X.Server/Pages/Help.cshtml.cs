using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Pages;

public class HelpModel : PageModel
{
	#region Fields

	private readonly AppSettings _appSettings;

	#endregion Fields

	#region Constructors

	public HelpModel(IConfiguration configuration)
	{
		_appSettings = configuration.Get<AppSettings>();
	}

	#endregion Constructors

	#region HelpModel Members

	[BindProperty]
	[DataType(DataType.Password)]
	public string HelpPassword { get; set; }

	public IActionResult OnGet(string p)
	{
		if (!string.IsNullOrEmpty(p))
		{
			HelpPassword = p;
			return _RedirectToHelp();
		}

		return Page();
	}

	private IActionResult _RedirectToHelp()
	{
		if (HelpPassword != _appSettings.Keys.HelpPassword6_8)
		{
			ModelState.AddModelError("HelpPassword", "Invalid password");
			return Page();
		}

		var cookieOptions = new CookieOptions { IsEssential = true, Secure = true, Expires = DateTime.Now.AddYears(5) };
		Response.Cookies.Append(_appSettings.Keys.HelpCookie, HelpPassword, cookieOptions);
		Response.Redirect("/6.8/Help/WhatsNewinVersion608.html", true);
		return null;
	}

	/// <summary>
	///     User is submitting password change
	/// </summary>
	public IActionResult OnPost()
	{
		if (!ModelState.IsValid) return Page();

		return _RedirectToHelp();
	}

	#endregion ChangePasswordModel Members
}