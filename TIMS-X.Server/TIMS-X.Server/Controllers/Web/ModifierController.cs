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
public class ModifierController : ControllerBase
{
    private readonly IModifierRepository _modifierRepository;
    private readonly IModifierValidator _modifierValidator;

    public ModifierController(IModifierRepository modifierRepository,
        IModifierValidator modifierValidator)
    {
        _modifierRepository = modifierRepository;
        _modifierValidator = modifierValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Modifier modifier)
    {
        try
        {
            var newModifier = await _modifierRepository.Add(modifier);
            return Ok(newModifier);
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
            _modifierRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetModifiers()
    {
        var modifiers = await _modifierRepository.GetAll(false);

        return Ok(modifiers);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var modifier = await _modifierRepository.Get(id);
        if (modifier == null)
            return BadRequest(new { message = $"Modifier with {id} id not found" });

        return Ok(modifier);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Modifier modifier)
    {
        try
        {
            var updated = await _modifierRepository.Update(modifier);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Modifier modifier)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (modifier.IsNew())
                validationResults = await _modifierValidator.AddNew(modifier);
            else
                validationResults = await _modifierValidator.Update(modifier);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}