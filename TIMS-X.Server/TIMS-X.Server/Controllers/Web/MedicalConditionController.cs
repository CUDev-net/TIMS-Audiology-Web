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
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
    Roles = StringConstants.Customer)]
public class MedicalConditionController : ControllerBase
{
    private readonly IMedicalConditionRepository _medicalConditionRepository;
    private readonly IMedicalConditionValidator _medicalConditionValidator;

    public MedicalConditionController(IMedicalConditionRepository medicalConditionRepository,
        IMedicalConditionValidator medicalConditionValidator)
    {
        _medicalConditionRepository = medicalConditionRepository;
        _medicalConditionValidator = medicalConditionValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(MedicalCondition medicalCondition)
    {
        try
        {
            var newMedicalCondition = await _medicalConditionRepository.Add(medicalCondition);
            return Ok(newMedicalCondition);
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
            _medicalConditionRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetMedicalConditions()
    {
        var medicalConditions = await _medicalConditionRepository.GetAll(false);

        return Ok(medicalConditions);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var medicalCondition = await _medicalConditionRepository.Get(id);
        if (medicalCondition == null)
            return BadRequest(new { message = $"MedicalCondition with {id} id not found" });

        return Ok(medicalCondition);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(MedicalCondition medicalCondition)
    {
        try
        {
            var updated = await _medicalConditionRepository.Update(medicalCondition);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(MedicalCondition medicalCondition)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (medicalCondition.IsNew())
                validationResults = await _medicalConditionValidator.AddNew(medicalCondition);
            else
                validationResults = await _medicalConditionValidator.Update(medicalCondition);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}