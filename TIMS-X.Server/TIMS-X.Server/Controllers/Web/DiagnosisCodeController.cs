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
public class DiagnosisCodeController : ControllerBase
{
    private readonly IDiagnosisCodeRepository _diagnosisCodeRepository;
    private readonly IDiagnosisCodeValidator _diagnosisCodeValidator;

    public DiagnosisCodeController(IDiagnosisCodeRepository diagnosisCodeRepository,
        IDiagnosisCodeValidator diagnosisCodeValidator)
    {
        _diagnosisCodeRepository = diagnosisCodeRepository;
        _diagnosisCodeValidator = diagnosisCodeValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(DiagnosisCode diagnosisCode)
    {
        try
        {
            var newDiagnosisCode = await _diagnosisCodeRepository.Add(diagnosisCode);
            return Ok(newDiagnosisCode);
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
            _diagnosisCodeRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetDiagnosisCodes()
    {
        var diagnosisCodes = await _diagnosisCodeRepository.GetAll(false);

        return Ok(diagnosisCodes);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var diagnosisCode = await _diagnosisCodeRepository.Get(id);
        if (diagnosisCode == null)
            return BadRequest(new { message = $"DiagnosisCode with {id} id not found" });

        return Ok(diagnosisCode);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(DiagnosisCode diagnosisCode)
    {
        try
        {
            var updated = await _diagnosisCodeRepository.Update(diagnosisCode);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(DiagnosisCode diagnosisCode)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (diagnosisCode.IsNew())
                validationResults = await _diagnosisCodeValidator.AddNew(diagnosisCode);
            else
                validationResults = await _diagnosisCodeValidator.Update(diagnosisCode);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}