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
public class HaWarrantyTypeController : ControllerBase
{
    private readonly IHaWarrantyTypeRepository _haWarrantyTypeRepository;
    private readonly IHaWarrantyTypeValidator _haWarrantyTypeValidator;

    public HaWarrantyTypeController(IHaWarrantyTypeRepository haWarrantyTypeRepository,
        IHaWarrantyTypeValidator haWarrantyTypeValidator)
    {
        _haWarrantyTypeRepository = haWarrantyTypeRepository;
        _haWarrantyTypeValidator = haWarrantyTypeValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HaWarrantyType haWarrantyType)
    {
        try
        {
            var newHaWarrantyType = await _haWarrantyTypeRepository.Add(haWarrantyType);
            return Ok(newHaWarrantyType);
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
            _haWarrantyTypeRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetHaWarrantyTypes()
    {
        var results = await _haWarrantyTypeRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var haWarrantyType = await _haWarrantyTypeRepository.Get(id);
        if (haWarrantyType == null)
            return BadRequest(new { message = $"WarrantyType with {id} id not found" });

        return Ok(haWarrantyType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HaWarrantyType haWarrantyType)
    {
        try
        {
            var updated = await _haWarrantyTypeRepository.Update(haWarrantyType);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HaWarrantyType haWarrantyType)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (haWarrantyType.IsNew())
                validationResults = await _haWarrantyTypeValidator.AddNew(haWarrantyType);
            else
                validationResults = await _haWarrantyTypeValidator.Update(haWarrantyType);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}