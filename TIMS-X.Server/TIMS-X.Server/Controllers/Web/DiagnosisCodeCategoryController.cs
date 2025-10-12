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
public class DiagnosisCodeCategoryController : ControllerBase
{
    private readonly IDiagnosisCodeCategoryRepository _diagnosisCodeCategoryRepository;
    private readonly IDiagnosisCodeCategoryValidator _diagnosisCodeCategoryValidator;

    public DiagnosisCodeCategoryController(IDiagnosisCodeCategoryRepository diagnosisCodeCategoryRepository,
        IDiagnosisCodeCategoryValidator diagnosisCodeCategoryValidator)
    {
        _diagnosisCodeCategoryRepository = diagnosisCodeCategoryRepository;
        _diagnosisCodeCategoryValidator = diagnosisCodeCategoryValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(DiagnosisCodeCategory diagnosisCodeCategory)
    {
        try
        {
            var newDiagnosisCodeCategory = await _diagnosisCodeCategoryRepository.Add(diagnosisCodeCategory);
            return Ok(newDiagnosisCodeCategory);
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
            _diagnosisCodeCategoryRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetDiagnosisCodeCategories()
    {
        var diagnosisCodeCategories = await _diagnosisCodeCategoryRepository.GetAll(false);

        return Ok(diagnosisCodeCategories);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var diagnosisCodeCategory = await _diagnosisCodeCategoryRepository.Get(id);
        if (diagnosisCodeCategory == null)
            return BadRequest(new { message = $"DiagnosisCodeCategory with {id} id not found" });

        return Ok(diagnosisCodeCategory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(DiagnosisCodeCategory diagnosisCodeCategory)
    {
        try
        {
            var updated = await _diagnosisCodeCategoryRepository.Update(diagnosisCodeCategory);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(DiagnosisCodeCategory diagnosisCodeCategory)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (diagnosisCodeCategory.IsNew())
                validationResults = await _diagnosisCodeCategoryValidator.AddNew(diagnosisCodeCategory);
            else
                validationResults = await _diagnosisCodeCategoryValidator.Update(diagnosisCodeCategory);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}