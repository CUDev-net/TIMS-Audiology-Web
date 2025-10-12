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
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
    Roles = StringConstants.Customer)]
public class AppointmentTypeController : ControllerBase
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IAppointmentTypeValidator _appointmentTypeValidator;

    public AppointmentTypeController(IAppointmentTypeRepository appointmentTypeRepository,
        IAppointmentTypeValidator appointmentTypeValidator)
    {
        _appointmentTypeRepository = appointmentTypeRepository;
        _appointmentTypeValidator = appointmentTypeValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(AppointmentType appointmentType)
    {
        try
        {
            var newAppointmentType = await _appointmentTypeRepository.Add(appointmentType);
            return Ok(newAppointmentType);
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
            _appointmentTypeRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAppointmentTypes()
    {
        var appointmentTypes = await _appointmentTypeRepository.GetAll(false);

        return Ok(appointmentTypes);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var appointmentType = await _appointmentTypeRepository.Get(id);
        if (appointmentType == null)
            return BadRequest(new { message = $"AppointmentType with {id} id not found" });

        return Ok(appointmentType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(AppointmentType appointmentType)
    {
        try
        {
            var updated = await _appointmentTypeRepository.Update(appointmentType);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(AppointmentType schedule)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (schedule.IsNew())
                validationResults = await _appointmentTypeValidator.AddNew(schedule);
            else
                validationResults = await _appointmentTypeValidator.Update(schedule);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}