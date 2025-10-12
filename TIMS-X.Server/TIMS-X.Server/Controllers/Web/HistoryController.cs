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
public class HistoryController : ControllerBase
{
    private readonly IHistoryRepository _historyRepository;
    private readonly IHistoryValidator _historyValidator;

    public HistoryController(IHistoryRepository historyRepository,
        IHistoryValidator historyValidator)
    {
        _historyRepository = historyRepository;
        _historyValidator = historyValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(History history)
    {
        try
        {
            var newHistory = await _historyRepository.Add(history);
            return Ok(newHistory);
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
            _historyRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetHistories()
    {
        var histories = await _historyRepository.GetAll(false);

        return Ok(histories);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var history = await _historyRepository.Get(id);
        if (history == null)
            return BadRequest(new { message = $"History with {id} id not found" });

        return Ok(history);
    }

    [HttpGet("get-all-by-patientId")]
    public async Task<IActionResult> GetAllByPatientId(int patientId, DateTime? minimumDateTime)
    {
        var histories = await _historyRepository.GetAllByPatientId(patientId, minimumDateTime);
        if (histories == null)
            return BadRequest(new { message = $"History with {patientId} patientId not found" });

        return Ok(histories);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(History history)
    {
        try
        {
            var updated = await _historyRepository.Update(history);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(History history)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (history.IsNew())
                validationResults = await _historyValidator.AddNew(history);
            else
                validationResults = await _historyValidator.Update(history);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}