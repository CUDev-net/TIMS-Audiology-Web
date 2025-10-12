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
public class HaModelOptionController : ControllerBase
{
    private readonly IHaModelOptionRepository _haModelOptionRepository;
    private readonly IHaModelOptionValidator _haModelOptionValidator;

    public HaModelOptionController(IHaModelOptionRepository haModelOptionRepository,
        IHaModelOptionValidator haModelOptionValidator)
    {
        _haModelOptionRepository = haModelOptionRepository;
        _haModelOptionValidator = haModelOptionValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HaModelOption haModelOption)
    {
        try
        {
            var newHaModelOption = await _haModelOptionRepository.Add(haModelOption);
            return Ok(newHaModelOption);
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
            _haModelOptionRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetModelOptions()
    {
        var results = await _haModelOptionRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var haModelOption = await _haModelOptionRepository.Get(id);
        if (haModelOption == null)
            return BadRequest(new { message = $"HaModelOption with {id} id not found" });

        return Ok(haModelOption);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HaModelOption haModelOption)
    {
        try
        {
            var updated = await _haModelOptionRepository.Update(haModelOption);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HaModelOption haModelOption)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (haModelOption.IsNew())
                validationResults = await _haModelOptionValidator.AddNew(haModelOption);
            else
                validationResults = await _haModelOptionValidator.Update(haModelOption);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}