using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TIMS_X.BLL.Repositories;
using TIMS_X.BLL.Validation;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.Dtos;
using TIMS_X.Server.Hubs;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
[ApiExplorerSettings(IgnoreApi = true)]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentValidator _appointmentValidator;
    private readonly ContextHelper _contextHelper;
    private readonly IScheduleOpeningsRepository _scheduleOpeningsRepository;
    private readonly IHubContext<SchedulerHub, ISchedulerHub> _schedulerHub;

    public AppointmentController(IAppointmentRepository appointmentRepository,
        IAppointmentValidator appointmentValidator,
        IHubContext<SchedulerHub, ISchedulerHub> schedulerHub,
        IScheduleOpeningsRepository scheduleOpeningsRepository,
        ContextHelper contextHelper)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentValidator = appointmentValidator;
        _schedulerHub = schedulerHub;
        _contextHelper = contextHelper;
        _scheduleOpeningsRepository = scheduleOpeningsRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Appointment appointment)
    {
        try
        {
            var created = await _appointmentRepository.Add(appointment);
            foreach (var appointmentDto in created)
                await _schedulerHub.Clients.Group(_contextHelper.SignalrConnectionName)
                    .OnAppointmentCreated(appointmentDto);
            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _appointmentRepository.Delete(id);
            await _schedulerHub.Clients.Group(_contextHelper.SignalrConnectionName).OnAppointmentDeleted(id);
            return Ok();
        }
        catch (Exception ex)
        {
            //Log.Error(ex, $"Error Deleting Appointment Id {id}");
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("end-series-{id}")]
    public async Task<IActionResult> EndRecurringSeries(int id)
    {
        try
        {
            var deletedIds = await _appointmentRepository.EndSeries(id);
            foreach (var deletedId in deletedIds)
                await _schedulerHub.Clients.Group(_contextHelper.SignalrConnectionName).OnAppointmentDeleted(deletedId);
            return Ok(deletedIds);
        }
        catch (Exception ex)
        {
            //Log.Error(ex, $"Error Deleting Appointment Id {id}");
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetById(int id)
    {
        var appointment = await _appointmentRepository.Get(id);
        if (appointment == null)
            return BadRequest(new { message = $"Patient with {id} id not found" });

        return Ok(appointment);
    }

    [HttpPost("schedule-openings")]
    public async Task<IActionResult> GetScheduleOpenings(ScheduleOpeningsSearchModel model)
    {
        try
        {
            var openings = await _scheduleOpeningsRepository.FetchScheduleOpenings(model);
            return Ok(openings);
        }
        catch (Exception ex)
        {
#if TEST
	        return BadRequest(new { message = ex.ToString() });
#else
	        return BadRequest(new { message = ex.Message });
#endif
		}
	}

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Appointment appointment)
    {
        try
        {
            var updated = await _appointmentRepository.Update(appointment);
            await _schedulerHub.Clients.Group(_contextHelper.SignalrConnectionName).OnAppointmentUpdated(updated);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Appointment appointment)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (appointment.IsNew())
                validationResults = await _appointmentValidator.AddNew(appointment);
            else
                validationResults = await _appointmentValidator.Update(appointment);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("validate-delete")]
    public IActionResult ValidateDelete(int id)
    {
        try
        {
            var validationResults = _appointmentValidator.Delete(id);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }
}