using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.BLL.Repositories;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Utils;

namespace TIMS_X.Server.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
public class PatientInsuranceController : ControllerBase
{
	private readonly IPatientInsuranceRepository _patientInsuranceRepository;

	public PatientInsuranceController(IPatientInsuranceRepository patientInsuranceRepository
	)
	{
		_patientInsuranceRepository = patientInsuranceRepository;
	}

	[HttpPost]
	public async Task<IActionResult> Create(PatientInsurance patientInsurance)
	{
		try
		{
			var newPatientInsurance = await _patientInsuranceRepository.Add(patientInsurance);
			return Ok(newPatientInsurance);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpDelete("{id}")]
	public IActionResult Delete(int id)
	{
		try
		{
			_patientInsuranceRepository.Delete(id);
			return Ok();
		}
		catch (Exception ex)
		{
			// return error message if there was an exception
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpGet]
	public async Task<IActionResult> GetById(int id)
	{
		var PatientInsurance = await _patientInsuranceRepository.Get(id);
		if (PatientInsurance == null)
			return BadRequest(new { message = $"patientInsurance with {id} id not found" });

		return Ok(PatientInsurance);
	}

	[HttpGet("get-for-patient")]
	public async Task<IActionResult> GetPatientInsurances(int patientId)
	{
		var results = await _patientInsuranceRepository.GetAllForPatient(patientId);

		return Ok(results);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(PatientInsurance patientInsurance)
	{
		try
		{
			var updated = await _patientInsuranceRepository.Update(patientInsurance);
			return Ok(updated);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}
}