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
public class HaReturnReasonController : ControllerBase
{
    private readonly IHaReturnReasonRepository _haReturnReasonRepository;
    private readonly IHaReturnReasonValidator _haReturnReasonValidator;

    public HaReturnReasonController(IHaReturnReasonRepository haReturnReasonRepository,
        IHaReturnReasonValidator haReturnReasonValidator)
    {
        _haReturnReasonRepository = haReturnReasonRepository;
        _haReturnReasonValidator = haReturnReasonValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HaReturnReason haReturnReason)
    {
        try
        {
            var newHaReturnReason = await _haReturnReasonRepository.Add(haReturnReason);
            return Ok(newHaReturnReason);
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
            _haReturnReasonRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetHaReturnReasons()
    {
        var results = await _haReturnReasonRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var haReturnReason = await _haReturnReasonRepository.Get(id);
        if (haReturnReason == null)
            return BadRequest(new { message = $"ReturnReason with {id} id not found" });

        return Ok(haReturnReason);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HaReturnReason haReturnReason)
    {
        try
        {
            var updated = await _haReturnReasonRepository.Update(haReturnReason);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HaReturnReason haReturnReason)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (haReturnReason.IsNew())
                validationResults = await _haReturnReasonValidator.AddNew(haReturnReason);
            else
                validationResults = await _haReturnReasonValidator.Update(haReturnReason);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}