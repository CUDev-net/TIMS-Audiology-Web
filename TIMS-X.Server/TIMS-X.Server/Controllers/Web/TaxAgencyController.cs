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
public class TaxAgencyController : ControllerBase
{
    private readonly ITaxAgencyRepository _taxAgencyRepository;
    private readonly ITaxAgencyValidator _taxAgencyValidator;

    public TaxAgencyController(ITaxAgencyRepository taxAgencyRepository,
        ITaxAgencyValidator taxAgencyValidator)
    {
        _taxAgencyRepository = taxAgencyRepository;
        _taxAgencyValidator = taxAgencyValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaxAgency taxAgency)
    {
        try
        {
            var newTaxAgency = await _taxAgencyRepository.Add(taxAgency);
            return Ok(newTaxAgency);
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
            _taxAgencyRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetTaxAgencies()
    {
        var taxAgencies = await _taxAgencyRepository.GetAll(false);

        return Ok(taxAgencies);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var taxAgency = await _taxAgencyRepository.Get(id);
        if (taxAgency == null)
            return BadRequest(new { message = $"TaxAgency with {id} id not found" });

        return Ok(taxAgency);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(TaxAgency taxAgency)
    {
        try
        {
            var updated = await _taxAgencyRepository.Update(taxAgency);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(TaxAgency taxAgency)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (taxAgency.IsNew())
                validationResults = await _taxAgencyValidator.AddNew(taxAgency);
            else
                validationResults = await _taxAgencyValidator.Update(taxAgency);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}