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
public class BatterySizeController : ControllerBase
{
    private readonly IBatterySizeRepository _batterySizeRepository;
    private readonly IBatterySizeValidator _batterySizeValidator;

    public BatterySizeController(IBatterySizeRepository batterySizeRepository,
        IBatterySizeValidator batterySizeValidator)
    {
        _batterySizeRepository = batterySizeRepository;
        _batterySizeValidator = batterySizeValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(BatterySize batterySize)
    {
        try
        {
            var newBatterySize = await _batterySizeRepository.Add(batterySize);
            return Ok(newBatterySize);
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
            _batterySizeRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetBatterySizes()
    {
        var results = await _batterySizeRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var batterySize = await _batterySizeRepository.Get(id);
        if (batterySize == null)
            return BadRequest(new { message = $"BatterySize with {id} id not found" });

        return Ok(batterySize);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(BatterySize batterySize)
    {
        try
        {
            var updated = await _batterySizeRepository.Update(batterySize);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(BatterySize batterySize)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (batterySize.IsNew())
                validationResults = await _batterySizeValidator.AddNew(batterySize);
            else
                validationResults = await _batterySizeValidator.Update(batterySize);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}