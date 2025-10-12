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
public class HaComponentController : ControllerBase
{
    private readonly IHaComponentRepository _haComponentRepository;
    private readonly IHaComponentValidator _haComponentValidator;

    public HaComponentController(IHaComponentRepository haComponentRepository,
        IHaComponentValidator haComponentValidator)
    {
        _haComponentRepository = haComponentRepository;
        _haComponentValidator = haComponentValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HaComponent haComponent)
    {
        try
        {
            var newHaComponent = await _haComponentRepository.Add(haComponent);
            return Ok(newHaComponent);
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
            _haComponentRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetHaComponents()
    {
        var results = await _haComponentRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var haComponent = await _haComponentRepository.Get(id);
        if (haComponent == null)
            return BadRequest(new { message = $"Component with {id} id not found" });

        return Ok(haComponent);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HaComponent haComponent)
    {
        try
        {
            var updated = await _haComponentRepository.Update(haComponent);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HaComponent haComponent)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (haComponent.IsNew())
                validationResults = await _haComponentValidator.AddNew(haComponent);
            else
                validationResults = await _haComponentValidator.Update(haComponent);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}