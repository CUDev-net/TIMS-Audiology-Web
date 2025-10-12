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
public class TaxGroupController : ControllerBase
{
    private readonly ITaxGroupRepository _taxGroupRepository;
    private readonly ITaxGroupValidator _taxGroupValidator;

    public TaxGroupController(ITaxGroupRepository taxGroupRepository,
        ITaxGroupValidator taxGroupValidator)
    {
        _taxGroupRepository = taxGroupRepository;
        _taxGroupValidator = taxGroupValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaxGroup taxGroup)
    {
        try
        {
            var newTaxGroup = await _taxGroupRepository.Add(taxGroup);
            return Ok(newTaxGroup);
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
            _taxGroupRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetTaxGroups()
    {
        var taxGroups = await _taxGroupRepository.GetAll(false);

        return Ok(taxGroups);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var taxGroup = await _taxGroupRepository.Get(id);
        if (taxGroup == null)
            return BadRequest(new { message = $"TaxGroup with {id} id not found" });
        foreach (var taxGroupTaxItem in taxGroup.TaxItems)
        {
            taxGroupTaxItem.TaxGroups = new List<TaxGroup>();
        }

        return Ok(taxGroup);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(TaxGroup taxGroup)
    {
        try
        {
            var updated = await _taxGroupRepository.Update(taxGroup);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(TaxGroup taxGroup)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (taxGroup.IsNew())
                validationResults = await _taxGroupValidator.AddNew(taxGroup);
            else
                validationResults = await _taxGroupValidator.Update(taxGroup);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}