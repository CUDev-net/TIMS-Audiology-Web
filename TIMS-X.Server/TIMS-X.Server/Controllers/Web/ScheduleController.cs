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

namespace TIMS_X.Server.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
[ApiExplorerSettings(IgnoreApi = true)]
public class ScheduleController : ControllerBase
{
    private readonly ContextHelper _contextHelper;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IHubContext<SchedulerHub, ISchedulerHub> _schedulerHub;
    private readonly IScheduleValidator _scheduleValidator;

    public ScheduleController(IScheduleRepository scheduleRepository, IScheduleValidator scheduleValidator,
        IHubContext<SchedulerHub, ISchedulerHub> schedulerHub, ContextHelper contextHelper)
    {
        _scheduleRepository = scheduleRepository;
        _scheduleValidator = scheduleValidator;
        _schedulerHub = schedulerHub;
        _contextHelper = contextHelper;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(Schedule schedule)
    {
        try
        {
            var newSchedules = new List<ScheduleDto>();
            foreach (var scheduleProviderId in schedule.ProviderIds)
            {
                foreach (var scheduleSiteId in schedule.SiteIds)
                {
                    var clone = (Schedule)schedule.Clone();
                    clone.ProviderId = scheduleProviderId;
                    clone.SiteId = scheduleSiteId;
                    var created = await _scheduleRepository.Add(clone);
                    await _schedulerHub.Clients.Group(_contextHelper.SignalrConnectionName).OnScheduleCreated(created);
                    newSchedules.Add(created);
                }
            }
            return Ok(newSchedules);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("delete-recurring-instance")]
    public async Task<IActionResult> CreateRecurringDeletedInstance(RecurringIntervalRemoved recurringIntervalRemoved)
    {
        try
        {
            var (deleted, scheduleDto) = await _scheduleRepository.DeleteOccurrence(recurringIntervalRemoved);
            await _schedulerHub.Clients.Group(_contextHelper.SignalrConnectionName)
                .OnScheduleUpdated(scheduleDto, recurringIntervalRemoved);
            return Ok(deleted);
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
            _scheduleRepository.Delete(id);
            await _schedulerHub.Clients.Group(_contextHelper.SignalrConnectionName).OnScheduleDeleted(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetById(int id)
    {
        var schedule = await _scheduleRepository.Get(id);
        if (schedule == null)
            return BadRequest(new { message = $"Schedule with {id} id not found" });

        return Ok(schedule);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Schedule schedule)
    {
        try
        {
            var updated = await _scheduleRepository.Update(schedule);
            await _schedulerHub.Clients.Group(_contextHelper.SignalrConnectionName).OnScheduleUpdated(updated, null);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Schedule schedule)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (schedule.Id <= 0)
                validationResults = await _scheduleValidator.AddNew(schedule);
            else
                validationResults = await _scheduleValidator.Update(schedule);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}