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
public class ApptAuthorizationController : ControllerBase
{
	private readonly IApptAuthorizationRepository _appointmentAuthorizationRepository;

	public ApptAuthorizationController(IApptAuthorizationRepository appointmentAuthorizationRepository)
	{
		_appointmentAuthorizationRepository = appointmentAuthorizationRepository;
	}

	[HttpPost]
	public async Task<IActionResult> Create(ApptAuthorization appointmentStatus)
	{
		try
		{
			var newApptAuthorization = await _appointmentAuthorizationRepository.Add(appointmentStatus);
			return Ok(newApptAuthorization);
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
			_appointmentAuthorizationRepository.Delete(id);
			return Ok();
		}
		catch (Exception ex)
		{
			// return error message if there was an exception
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpGet("get-for-patient")]
	public async Task<IActionResult> GetApptAuthorizations(int patientId, bool includeInactive, int authorizationId)
	{
		var appointmentStatuses = await _appointmentAuthorizationRepository.GetAll(includeInactive, patientId, authorizationId);

		return Ok(appointmentStatuses);
	}

	[HttpGet]
	public async Task<IActionResult> GetById(int id)
	{
		var appointmentStatus = await _appointmentAuthorizationRepository.Get(id);
		if (appointmentStatus == null)
			return BadRequest(new { message = $"ApptAuthorization with {id} id not found" });

		return Ok(appointmentStatus);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(ApptAuthorization appointmentStatus)
	{
		try
		{
			var updated = await _appointmentAuthorizationRepository.Update(appointmentStatus);
			return Ok(updated);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}
}