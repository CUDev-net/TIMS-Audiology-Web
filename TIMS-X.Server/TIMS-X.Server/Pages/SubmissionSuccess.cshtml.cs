using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Pages;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
public class SubmissionSuccessModel : PageModel
{
	#region Constructors

	public SubmissionSuccessModel(PatientService patientService, ImagingService imagingService)
	{
		_patientService = patientService;
	}

	#endregion Constructors

	#region PatientModel Members

	[BindProperty] public Patient Patient { get; set; }


	public async Task<IActionResult> OnGetAsync(int id)
	{
		Patient = await _patientService.GetAsync(id);

		if (Patient == null)
		{
			Response.StatusCode = 404;
			Patient = new Patient();
		}

		return Page();
	}

	#endregion PatientModel Members

	#region Fields

	private readonly PatientService _patientService;

	#endregion Fields

	#region Private Members

	#endregion Private Members
}