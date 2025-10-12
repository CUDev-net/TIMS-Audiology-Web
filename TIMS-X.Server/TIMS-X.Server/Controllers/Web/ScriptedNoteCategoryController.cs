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
public class ScriptedNoteCategoryController : ControllerBase
{
    private readonly IScriptedNoteCategoryRepository _scriptedNoteCategoryRepository;
    private readonly IScriptedNoteCategoryValidator _scriptedNoteCategoryValidator;

    public ScriptedNoteCategoryController(IScriptedNoteCategoryRepository scriptedNoteCategoryRepository,
        IScriptedNoteCategoryValidator scriptedNoteCategoryValidator)
    {
        _scriptedNoteCategoryRepository = scriptedNoteCategoryRepository;
        _scriptedNoteCategoryValidator = scriptedNoteCategoryValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(ScriptedNoteCategory scriptedNoteCategory)
    {
        try
        {
            var newScriptedNoteCategory = await _scriptedNoteCategoryRepository.Add(scriptedNoteCategory);
            return Ok(newScriptedNoteCategory);
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
            _scriptedNoteCategoryRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetScriptedNoteCategories()
    {
        var scriptedNoteCategories = await _scriptedNoteCategoryRepository.GetAll(false);

        return Ok(scriptedNoteCategories);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var scriptedNoteCategory = await _scriptedNoteCategoryRepository.Get(id);
        if (scriptedNoteCategory == null)
            return BadRequest(new { message = $"ScriptedNoteCategory with {id} id not found" });

        return Ok(scriptedNoteCategory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(ScriptedNoteCategory scriptedNoteCategory)
    {
        try
        {
            var updated = await _scriptedNoteCategoryRepository.Update(scriptedNoteCategory);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(ScriptedNoteCategory scriptedNoteCategory)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (scriptedNoteCategory.IsNew())
                validationResults = await _scriptedNoteCategoryValidator.AddNew(scriptedNoteCategory);
            else
                validationResults = await _scriptedNoteCategoryValidator.Update(scriptedNoteCategory);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}