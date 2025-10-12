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
public class InsurancePayerController : ControllerBase
{
    private readonly IInsurancePayerRepository _insurancePayerRepository;
    private readonly IInsurancePayerValidator _insurancePayerValidator;

    public InsurancePayerController(IInsurancePayerRepository insurancePayerRepository,
        IInsurancePayerValidator insurancePayerValidator)
    {
        _insurancePayerRepository = insurancePayerRepository;
        _insurancePayerValidator = insurancePayerValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(InsurancePayer insurancePayer)
    {
        try
        {
            var newInsurancePayer = await _insurancePayerRepository.Add(insurancePayer);
            return Ok(newInsurancePayer);
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
            _insurancePayerRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetInsurancePayers()
    {
        var insurancePayers = await _insurancePayerRepository.GetAll(false);

        return Ok(insurancePayers);
    }

	[HttpGet("get")]
	public async Task<IActionResult> GetById(int id)
    {
        var insurancePayer = await _insurancePayerRepository.Get(id);
        if (insurancePayer == null)
            return BadRequest(new { message = $"InsurancePayer with {id} id not found" });

        return Ok(insurancePayer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(InsurancePayer insurancePayer)
    {
        try
        {
            var updated = await _insurancePayerRepository.Update(insurancePayer);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(InsurancePayer insurancePayer)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (insurancePayer.IsNew())
                validationResults = await _insurancePayerValidator.AddNew(insurancePayer);
            else
                validationResults = await _insurancePayerValidator.Update(insurancePayer);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}