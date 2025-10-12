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
public class ScheduleBlockController : ControllerBase
{
    private readonly IScheduleBlockRepository _scheduleBlockRepository;
    private readonly IScheduleBlockValidator _scheduleBlockValidator;

    public ScheduleBlockController(IScheduleBlockRepository scheduleBlockRepository,
        IScheduleBlockValidator scheduleBlockValidator)
    {
        _scheduleBlockRepository = scheduleBlockRepository;
        _scheduleBlockValidator = scheduleBlockValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(ScheduleBlock scheduleBlock)
    {
        try
        {
            var newScheduleBlock = await _scheduleBlockRepository.Add(scheduleBlock);
            return Ok(newScheduleBlock);
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
            _scheduleBlockRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetScheduleBlocks()
    {
        var scheduleBlocks = await _scheduleBlockRepository.GetAll();

        return Ok(scheduleBlocks);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var scheduleBlock = await _scheduleBlockRepository.Get(id);
        if (scheduleBlock == null)
            return BadRequest(new { message = $"Schedule Block with {id} id not found" });

        return Ok(scheduleBlock);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(ScheduleBlock scheduleBlock)
    {
        try
        {
            var updated = await _scheduleBlockRepository.Update(scheduleBlock);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(ScheduleBlock scheduleBlock)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (scheduleBlock.IsNew())
                validationResults = await _scheduleBlockValidator.AddNew(scheduleBlock);
            else
                validationResults = await _scheduleBlockValidator.Update(scheduleBlock);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}