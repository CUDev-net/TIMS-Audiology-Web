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
public class ScriptedNoteController : ControllerBase
{
    private readonly IScriptedNoteRepository _scriptedNoteRepository;
    private readonly IScriptedNoteValidator _scriptedNoteValidator;

    public ScriptedNoteController(IScriptedNoteRepository scriptedNoteRepository,
        IScriptedNoteValidator scriptedNoteValidator)
    {
        _scriptedNoteRepository = scriptedNoteRepository;
        _scriptedNoteValidator = scriptedNoteValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(ScriptedNote scriptedNote)
    {
        try
        {
            var newScriptedNote = await _scriptedNoteRepository.Add(scriptedNote);
            return Ok(newScriptedNote);
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
            _scriptedNoteRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetScriptedNotes()
    {
        var scriptedNotees = await _scriptedNoteRepository.GetAll(false);

        return Ok(scriptedNotees);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var scriptedNote = await _scriptedNoteRepository.Get(id);
        if (scriptedNote == null)
            return BadRequest(new { message = $"ScriptedNote with {id} id not found" });

        return Ok(scriptedNote);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(ScriptedNote scriptedNote)
    {
        try
        {
            var updated = await _scriptedNoteRepository.Update(scriptedNote);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(ScriptedNote scriptedNote)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (scriptedNote.IsNew())
                validationResults = await _scriptedNoteValidator.AddNew(scriptedNote);
            else
                validationResults = await _scriptedNoteValidator.Update(scriptedNote);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}