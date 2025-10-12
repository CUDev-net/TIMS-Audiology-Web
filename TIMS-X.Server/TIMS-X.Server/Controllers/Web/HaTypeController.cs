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
public class HaTypeController : ControllerBase
{
    private readonly IHaTypeRepository _haTypeRepository;
    private readonly IHaTypeValidator _haTypeValidator;

    public HaTypeController(IHaTypeRepository haTypeRepository,
        IHaTypeValidator haTypeValidator)
    {
        _haTypeRepository = haTypeRepository;
        _haTypeValidator = haTypeValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HaType haType)
    {
        try
        {
            var newHaType = await _haTypeRepository.Add(haType);
            return Ok(newHaType);
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
            _haTypeRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetHaTypes()
    {
        var results = await _haTypeRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var haType = await _haTypeRepository.Get(id);
        if (haType == null)
            return BadRequest(new { message = $"Type with {id} id not found" });

        return Ok(haType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HaType haType)
    {
        try
        {
            var updated = await _haTypeRepository.Update(haType);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HaType haType)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (haType.IsNew())
                validationResults = await _haTypeValidator.AddNew(haType);
            else
                validationResults = await _haTypeValidator.Update(haType);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}