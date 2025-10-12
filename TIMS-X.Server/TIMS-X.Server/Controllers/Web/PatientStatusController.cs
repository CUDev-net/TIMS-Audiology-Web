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
public class PatientStatusController : ControllerBase
{
    private readonly IPatientStatusRepository _patientStatusRepository;
    private readonly IPatientStatusValidator _patientStatusValidator;

    public PatientStatusController(IPatientStatusRepository patientStatusRepository,
        IPatientStatusValidator patientStatusValidator)
    {
        _patientStatusRepository = patientStatusRepository;
        _patientStatusValidator = patientStatusValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(PatientStatus patientStatus)
    {
        try
        {
            var newPatientStatus = await _patientStatusRepository.Add(patientStatus);
            return Ok(newPatientStatus);
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
            _patientStatusRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetPatientStatuses()
    {
        var patientStatuses = await _patientStatusRepository.GetAll(false);

        return Ok(patientStatuses);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var patientStatus = await _patientStatusRepository.Get(id);
        if (patientStatus == null)
            return BadRequest(new { message = $"PatientStatus with {id} id not found" });

        return Ok(patientStatus);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(PatientStatus patientStatus)
    {
        try
        {
            var updated = await _patientStatusRepository.Update(patientStatus);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(PatientStatus patientStatus)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (patientStatus.IsNew())
                validationResults = await _patientStatusValidator.AddNew(patientStatus);
            else
                validationResults = await _patientStatusValidator.Update(patientStatus);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}