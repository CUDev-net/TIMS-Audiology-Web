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
public class HaStyleController : ControllerBase
{
    private readonly IHaStyleRepository _haStyleRepository;
    private readonly IHaStyleValidator _haStyleValidator;

    public HaStyleController(IHaStyleRepository haStyleRepository,
        IHaStyleValidator haStyleValidator)
    {
        _haStyleRepository = haStyleRepository;
        _haStyleValidator = haStyleValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HaStyle haStyle)
    {
        try
        {
            var newHaStyle = await _haStyleRepository.Add(haStyle);
            return Ok(newHaStyle);
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
            _haStyleRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetHaStyles()
    {
        var results = await _haStyleRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var haStyle = await _haStyleRepository.Get(id);
        if (haStyle == null)
            return BadRequest(new { message = $"Style with {id} id not found" });

        return Ok(haStyle);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HaStyle haStyle)
    {
        try
        {
            var updated = await _haStyleRepository.Update(haStyle);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HaStyle haStyle)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (haStyle.IsNew())
                validationResults = await _haStyleValidator.AddNew(haStyle);
            else
                validationResults = await _haStyleValidator.Update(haStyle);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}