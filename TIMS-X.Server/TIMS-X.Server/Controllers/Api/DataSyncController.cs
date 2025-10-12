using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.Core.Models;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Models;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Controllers.Api;

[Route("datasync")]
[ApiController]
[AllowAnonymous]
public class DataSyncController : ControllerBase
{
	private readonly VendorService _vendorService;

	public DataSyncController(VendorService vendorService)
	{
		_vendorService = vendorService;
	}

	/// <summary>
	///     Get Practice
	/// </summary>
	/// <remarks>
	///     Returns the details of the practice.<br />
	///     <fieldset>
	///         <legend>Example</legend>
	///         <code>curl -X GET "https<span>:</span>https://us.timsaudiology.com/api/v1/GetPractice"<br />
	/// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code>
	///     </fieldset>
	/// </remarks>
	/// <response code="200">If exists, the practice details are returned, otherwise null</response>
	/// <response code="500">An error occurred</response>
	[HttpGet("GetPractice")]
	[ProducesResponseType(typeof(DetailedPatientItem), 200)]
	[ProducesResponseType(typeof(ErrorResponse), 500)]
	public async Task<ActionResult> GetPracticeAsync()
	{
		var fullPractice = await _vendorService.GetPracticeAsync();

		// Return subset of practice to match old audigy sync


		/* Old return example:
		 *
		 <?xml version="1.0" encoding="UTF-8"?>
		<VendorExport>
		<Practice>
		<PracticeID>40860</PracticeID>
		<PracticeCode>RHC40860</PracticeCode>
		<Name>Rehder Hearing Clinic</Name>
		<Address1>1101 N 27th Street, Suite E</Address1>
		<Address2 />
		<City>Billings</City>
		<State>MT</State>
		<ZipCode>59101-0100</ZipCode>
		<Phone>406-245-6893</Phone>
		<Fax>406-245-9954</Fax>
		<TaxID>863386027</TaxID>
		</Practice>
		</VendorExport>
		*/

		var practice = new
		{
			PracticeCode = StringUtilities.StripNonNumeric(fullPractice.OfficeCode),
			fullPractice.Name,
			Address1 = fullPractice.BillingAddress1,
			Address2 = fullPractice.BillingAddress2,
			City = fullPractice.BillingCity,
			State = fullPractice.BillingState,
			ZipCode = fullPractice.BillingZipCode,
			Phone = fullPractice.BillingPhoneNumber,
			fullPractice.Fax
			//TaxID = fullPractice.TaxID
		};

		return Ok(practice);
	}
}