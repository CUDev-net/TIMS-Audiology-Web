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
public class OutsideFacilityController : ControllerBase
{
    private readonly IOutsideFacilityRepository _outsideFacilityRepository;
    private readonly IOutsideFacilityValidator _outsideFacilityValidator;

    public OutsideFacilityController(IOutsideFacilityRepository outsideFacilityRepository,
        IOutsideFacilityValidator outsideFacilityValidator)
    {
        _outsideFacilityRepository = outsideFacilityRepository;
        _outsideFacilityValidator = outsideFacilityValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(OutsideFacility outsideFacility)
    {
        try
        {
            var newOutsideFacility = await _outsideFacilityRepository.Add(outsideFacility);
            return Ok(newOutsideFacility);
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
            _outsideFacilityRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetOutsideFacilities()
    {
        var outsideFacilities = await _outsideFacilityRepository.GetAll(false);

        return Ok(outsideFacilities);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var outsideFacility = await _outsideFacilityRepository.Get(id);
        if (outsideFacility == null)
            return BadRequest(new { message = $"OutsideFacility with {id} id not found" });

        return Ok(outsideFacility);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(OutsideFacility outsideFacility)
    {
        try
        {
            var updated = await _outsideFacilityRepository.Update(outsideFacility);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(OutsideFacility outsideFacility)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (outsideFacility.IsNew())
                validationResults = await _outsideFacilityValidator.AddNew(outsideFacility);
            else
                validationResults = await _outsideFacilityValidator.Update(outsideFacility);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}