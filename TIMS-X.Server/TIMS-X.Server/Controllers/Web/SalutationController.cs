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
public class SalutationController : ControllerBase
{
    private readonly ISalutationRepository _salutationRepository;
    private readonly ISalutationValidator _salutationValidator;

    public SalutationController(ISalutationRepository salutationRepository,
        ISalutationValidator salutationValidator)
    {
        _salutationRepository = salutationRepository;
        _salutationValidator = salutationValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Salutation salutation)
    {
        try
        {
            var newSalutation = await _salutationRepository.Add(salutation);
            return Ok(newSalutation);
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
            _salutationRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetSalutations()
    {
        var salutations = await _salutationRepository.GetAll(false);

        return Ok(salutations);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var salutation = await _salutationRepository.Get(id);
        if (salutation == null)
            return BadRequest(new { message = $"Salutation with {id} id not found" });

        return Ok(salutation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Salutation salutation)
    {
        try
        {
            var updated = await _salutationRepository.Update(salutation);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Salutation salutation)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (salutation.IsNew())
                validationResults = await _salutationValidator.AddNew(salutation);
            else
                validationResults = await _salutationValidator.Update(salutation);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}