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
public class CptCodeCategoryController : ControllerBase
{
    private readonly ICptCodeCategoryRepository _cptCodeCategoryRepository;
    private readonly ICptCodeCategoryValidator _cptCodeCategoryValidator;

    public CptCodeCategoryController(ICptCodeCategoryRepository cptCodeCategoryRepository,
        ICptCodeCategoryValidator cptCodeCategoryValidator)
    {
        _cptCodeCategoryRepository = cptCodeCategoryRepository;
        _cptCodeCategoryValidator = cptCodeCategoryValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CptCodeCategory cptCodeCategory)
    {
        try
        {
            var newCptCodeCategory = await _cptCodeCategoryRepository.Add(cptCodeCategory);
            return Ok(newCptCodeCategory);
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
            _cptCodeCategoryRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetCptCodeCategories()
    {
        var cptCodeCategories = await _cptCodeCategoryRepository.GetAll(false);

        return Ok(cptCodeCategories);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var cptCodeCategory = await _cptCodeCategoryRepository.Get(id);
        if (cptCodeCategory == null)
            return BadRequest(new { message = $"CptCodeCategory with {id} id not found" });

        return Ok(cptCodeCategory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(CptCodeCategory cptCodeCategory)
    {
        try
        {
            var updated = await _cptCodeCategoryRepository.Update(cptCodeCategory);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(CptCodeCategory cptCodeCategory)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (cptCodeCategory.IsNew())
                validationResults = await _cptCodeCategoryValidator.AddNew(cptCodeCategory);
            else
                validationResults = await _cptCodeCategoryValidator.Update(cptCodeCategory);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}