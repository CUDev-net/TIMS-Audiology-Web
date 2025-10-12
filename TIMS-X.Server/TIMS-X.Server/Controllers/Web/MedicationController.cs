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
public class MedicationController : ControllerBase
{
    private readonly IMedicationRepository _medicationRepository;
    private readonly IMedicationValidator _medicationValidator;

    public MedicationController(IMedicationRepository medicationRepository,
        IMedicationValidator medicationValidator)
    {
        _medicationRepository = medicationRepository;
        _medicationValidator = medicationValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Medication medication)
    {
        try
        {
            var newMedication = await _medicationRepository.Add(medication);
            return Ok(newMedication);
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
            _medicationRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetMedications()
    {
        var medications = await _medicationRepository.GetAll(false);

        return Ok(medications);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var medication = await _medicationRepository.Get(id);
        if (medication == null)
            return BadRequest(new { message = $"Medication with {id} id not found" });

        return Ok(medication);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Medication medication)
    {
        try
        {
            var updated = await _medicationRepository.Update(medication);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Medication medication)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (medication.IsNew())
                validationResults = await _medicationValidator.AddNew(medication);
            else
                validationResults = await _medicationValidator.Update(medication);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}