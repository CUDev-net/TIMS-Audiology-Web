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
public class TaxItemController : ControllerBase
{
    private readonly ITaxItemRepository _taxItemRepository;
    private readonly ITaxItemValidator _taxItemValidator;

    public TaxItemController(ITaxItemRepository taxItemRepository,
        ITaxItemValidator taxItemValidator)
    {
        _taxItemRepository = taxItemRepository;
        _taxItemValidator = taxItemValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaxItem taxItem)
    {
        try
        {
            var newTaxItem = await _taxItemRepository.Add(taxItem);
            return Ok(newTaxItem);
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
            _taxItemRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetTaxItems()
    {
        var taxItems = await _taxItemRepository.GetAll(false);
        return Ok(taxItems);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var taxItem = await _taxItemRepository.Get(id);
        if (taxItem == null)
            return BadRequest(new { message = $"TaxItem with {id} id not found" });

        return Ok(taxItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(TaxItem taxItem)
    {
        try
        {
            var updated = await _taxItemRepository.Update(taxItem);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(TaxItem taxItem)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (taxItem.IsNew())
                validationResults = await _taxItemValidator.AddNew(taxItem);
            else
                validationResults = await _taxItemValidator.Update(taxItem);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}