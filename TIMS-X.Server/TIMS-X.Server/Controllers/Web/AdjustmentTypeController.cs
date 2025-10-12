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
public class AdjustmentTypeController : ControllerBase
{
    private readonly IAdjustmentTypeRepository _adjustmentTypeRepository;
    private readonly IAdjustmentTypeValidator _adjustmentTypeValidator;

    public AdjustmentTypeController(IAdjustmentTypeRepository adjustmentTypeRepository,
        IAdjustmentTypeValidator adjustmentTypeValidator)
    {
        _adjustmentTypeRepository = adjustmentTypeRepository;
        _adjustmentTypeValidator = adjustmentTypeValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdjustmentType adjustmentType)
    {
        try
        {
            var newAdjustmentType = await _adjustmentTypeRepository.Add(adjustmentType);
            return Ok(newAdjustmentType);
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
            _adjustmentTypeRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAdjustmentTypes()
    {
        var adjustmentTypes = await _adjustmentTypeRepository.GetAll(false);

        return Ok(adjustmentTypes);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var adjustmentType = await _adjustmentTypeRepository.Get(id);
        if (adjustmentType == null)
            return BadRequest(new { message = $"AdjustmentType with {id} id not found" });

        return Ok(adjustmentType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(AdjustmentType adjustmentType)
    {
        try
        {
            var updated = await _adjustmentTypeRepository.Update(adjustmentType);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(AdjustmentType adjustmentType)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (adjustmentType.IsNew())
                validationResults = await _adjustmentTypeValidator.AddNew(adjustmentType);
            else
                validationResults = await _adjustmentTypeValidator.Update(adjustmentType);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}