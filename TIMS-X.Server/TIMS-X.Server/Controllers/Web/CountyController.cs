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
public class CountyController : ControllerBase
{
    private readonly ICountyRepository _countyRepository;
    private readonly ICountyValidator _countyValidator;

    public CountyController(ICountyRepository countyRepository,
        ICountyValidator countyValidator)
    {
        _countyRepository = countyRepository;
        _countyValidator = countyValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(County county)
    {
        try
        {
            var newCounty = await _countyRepository.Add(county);
            return Ok(newCounty);
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
            _countyRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetCounties()
    {
        var counties = await _countyRepository.GetAll(false);

        return Ok(counties);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var county = await _countyRepository.Get(id);
        if (county == null)
            return BadRequest(new { message = $"County with {id} id not found" });

        return Ok(county);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(County county)
    {
        try
        {
            var updated = await _countyRepository.Update(county);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(County county)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (county.IsNew())
                validationResults = await _countyValidator.AddNew(county);
            else
                validationResults = await _countyValidator.Update(county);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}