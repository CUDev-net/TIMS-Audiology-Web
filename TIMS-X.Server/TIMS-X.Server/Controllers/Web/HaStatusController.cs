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
public class HaStatusController : ControllerBase
{
    private readonly IHaStatusRepository _haStatusRepository;
    private readonly IHaStatusValidator _haStatusValidator;

    public HaStatusController(IHaStatusRepository haStatusRepository,
        IHaStatusValidator haStatusValidator)
    {
        _haStatusRepository = haStatusRepository;
        _haStatusValidator = haStatusValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HaStatus haStatus)
    {
        try
        {
            var newHaStatus = await _haStatusRepository.Add(haStatus);
            return Ok(newHaStatus);
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
            _haStatusRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetHaStatuses()
    {
        var results = await _haStatusRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var haStatus = await _haStatusRepository.Get(id);
        if (haStatus == null)
            return BadRequest(new { message = $"Status with {id} id not found" });

        return Ok(haStatus);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HaStatus haStatus)
    {
        try
        {
            var updated = await _haStatusRepository.Update(haStatus);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HaStatus haStatus)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (haStatus.IsNew())
                validationResults = await _haStatusValidator.AddNew(haStatus);
            else
                validationResults = await _haStatusValidator.Update(haStatus);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}