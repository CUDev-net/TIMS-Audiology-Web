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
public class HaHistoryController : ControllerBase
{
    private readonly IHaHistoryRepository _haHistoryRepository;
    private readonly IHaHistoryValidator _haHistoryValidator;

    public HaHistoryController(IHaHistoryRepository haHistoryRepository,
        IHaHistoryValidator haHistoryValidator)
    {
        _haHistoryRepository = haHistoryRepository;
        _haHistoryValidator = haHistoryValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HaHistory haHistory)
    {
        try
        {
            var newHaHistory = await _haHistoryRepository.Add(haHistory);
            return Ok(newHaHistory);
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
            _haHistoryRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetHaHistories()
    {
        var haHistories = await _haHistoryRepository.GetAll(false);

        return Ok(haHistories);
    }

    [HttpGet("get-all-by-patientId")]
    public async Task<IActionResult> GetByPatientId(int patientId)
    {
        var haHistories = await _haHistoryRepository.GetAllByPatientId(patientId);
        if (haHistories == null)
            return BadRequest(new { message = $"HaHistory with {patientId} patientId not found" });

        return Ok(haHistories);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var haHistory = await _haHistoryRepository.Get(id);
        if (haHistory == null)
            return BadRequest(new { message = $"HaHistory with {id} id not found" });

        return Ok(haHistory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HaHistory haHistory)
    {
        try
        {
            var updated = await _haHistoryRepository.Update(haHistory);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HaHistory schedule)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (schedule.IsNew())
                validationResults = await _haHistoryValidator.AddNew(schedule);
            else
                validationResults = await _haHistoryValidator.Update(schedule);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}