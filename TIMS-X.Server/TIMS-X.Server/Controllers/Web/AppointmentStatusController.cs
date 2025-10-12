using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.BLL.Repositories;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Utils;

namespace TIMS_X.Server.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
public class AppointmentStatusController : ControllerBase
{
    private readonly IAppointmentStatusRepository _appointmentStatusRepository;
    private readonly IAppointmentStatusValidator _appointmentStatusValidator;

    public AppointmentStatusController(IAppointmentStatusRepository appointmentStatusRepository,
        IAppointmentStatusValidator appointmentStatusValidator)
    {
        _appointmentStatusRepository = appointmentStatusRepository;
        _appointmentStatusValidator = appointmentStatusValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(AppointmentStatus appointmentStatus)
    {
        try
        {
            var newAppointmentStatus = await _appointmentStatusRepository.Add(appointmentStatus);
            return Ok(newAppointmentStatus);
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
            _appointmentStatusRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAppointmentStatuses()
    {
        var appointmentStatuses = await _appointmentStatusRepository.GetAll(false);

        return Ok(appointmentStatuses);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var appointmentStatus = await _appointmentStatusRepository.Get(id);
        if (appointmentStatus == null)
            return BadRequest(new { message = $"AppointmentStatus with {id} id not found" });

        return Ok(appointmentStatus);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(AppointmentStatus appointmentStatus)
    {
        try
        {
            var updated = await _appointmentStatusRepository.Update(appointmentStatus);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(AppointmentStatus schedule)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (schedule.IsNew())
                validationResults = await _appointmentStatusValidator.AddNew(schedule);
            else
                validationResults = await _appointmentStatusValidator.Update(schedule);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}