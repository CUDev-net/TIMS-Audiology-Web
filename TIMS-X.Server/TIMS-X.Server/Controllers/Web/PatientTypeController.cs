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
public class PatientTypeController : ControllerBase
{
    private readonly IPatientTypeRepository _patientTypeRepository;
    private readonly IPatientTypeValidator _patientTypeValidator;

    public PatientTypeController(IPatientTypeRepository patientTypeRepository,
        IPatientTypeValidator patientTypeValidator)
    {
        _patientTypeRepository = patientTypeRepository;
        _patientTypeValidator = patientTypeValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(PatientType patientType)
    {
        try
        {
            var newPatientType = await _patientTypeRepository.Add(patientType);
            return Ok(newPatientType);
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
            _patientTypeRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetPatientTypes()
    {
        var patientTypes = await _patientTypeRepository.GetAll(false);

        return Ok(patientTypes);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var patientType = await _patientTypeRepository.Get(id);
        if (patientType == null)
            return BadRequest(new { message = $"PatientType with {id} id not found" });

        return Ok(patientType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(PatientType patientType)
    {
        try
        {
            var updated = await _patientTypeRepository.Update(patientType);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(PatientType patientType)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (patientType.IsNew())
                validationResults = await _patientTypeValidator.AddNew(patientType);
            else
                validationResults = await _patientTypeValidator.Update(patientType);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}