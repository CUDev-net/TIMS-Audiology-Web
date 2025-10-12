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
public class HistoryTypeController : ControllerBase
{
    private readonly IHistoryTypeRepository _historyTypeRepository;
    private readonly IHistoryTypeValidator _historyTypeValidator;

    public HistoryTypeController(IHistoryTypeRepository historyTypeRepository,
        IHistoryTypeValidator historyTypeValidator)
    {
        _historyTypeRepository = historyTypeRepository;
        _historyTypeValidator = historyTypeValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HistoryType historyType)
    {
        try
        {
            var newHistoryType = await _historyTypeRepository.Add(historyType);
            return Ok(newHistoryType);
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
            _historyTypeRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetHistoryTypes()
    {
        var historyTypes = await _historyTypeRepository.GetAll(false);

        return Ok(historyTypes);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var historyType = await _historyTypeRepository.Get(id);
        if (historyType == null)
            return BadRequest(new { message = $"HistoryType with {id} id not found" });

        return Ok(historyType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HistoryType historyType)
    {
        try
        {
            var updated = await _historyTypeRepository.Update(historyType);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HistoryType historyType)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (historyType.IsNew())
                validationResults = await _historyTypeValidator.AddNew(historyType);
            else
                validationResults = await _historyTypeValidator.Update(historyType);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}