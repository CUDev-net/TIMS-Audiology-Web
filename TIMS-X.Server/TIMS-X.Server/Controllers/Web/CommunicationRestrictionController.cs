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
public class CommunicationRestrictionController : ControllerBase
{
    private readonly ICommunicationRestrictionRepository _communicationRestrictionRepository;
    private readonly ICommunicationRestrictionValidator _communicationRestrictionValidator;

    public CommunicationRestrictionController(ICommunicationRestrictionRepository communicationRestrictionRepository,
        ICommunicationRestrictionValidator communicationRestrictionValidator)
    {
        _communicationRestrictionRepository = communicationRestrictionRepository;
        _communicationRestrictionValidator = communicationRestrictionValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CommunicationRestriction communicationRestriction)
    {
        try
        {
            var newCommunicationRestriction = await _communicationRestrictionRepository.Add(communicationRestriction);
            return Ok(newCommunicationRestriction);
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
            _communicationRestrictionRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetCommunicationRestrictions()
    {
        var communicationRestrictions = await _communicationRestrictionRepository.GetAll(false);

        return Ok(communicationRestrictions);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var communicationRestriction = await _communicationRestrictionRepository.Get(id);
        if (communicationRestriction == null)
            return BadRequest(new { message = $"CommunicationRestriction with {id} id not found" });

        return Ok(communicationRestriction);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(CommunicationRestriction communicationRestriction)
    {
        try
        {
            var updated = await _communicationRestrictionRepository.Update(communicationRestriction);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(CommunicationRestriction communicationRestriction)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (communicationRestriction.IsNew())
                validationResults = await _communicationRestrictionValidator.AddNew(communicationRestriction);
            else
                validationResults = await _communicationRestrictionValidator.Update(communicationRestriction);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}