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
public class SexController : ControllerBase
{
    private readonly ISexRepository _sexRepository;
    private readonly ISexValidator _sexValidator;

    public SexController(ISexRepository sexRepository,
        ISexValidator sexValidator)
    {
        _sexRepository = sexRepository;
        _sexValidator = sexValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Sex sex)
    {
        try
        {
            var newSex = await _sexRepository.Add(sex);
            return Ok(newSex);
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
            _sexRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetSexes()
    {
        var sexes = await _sexRepository.GetAll(false);

        return Ok(sexes);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var sex = await _sexRepository.Get(id);
        if (sex == null)
            return BadRequest(new { message = $"Sex with {id} id not found" });

        return Ok(sex);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Sex sex)
    {
        try
        {
            var updated = await _sexRepository.Update(sex);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Sex sex)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (sex.IsNew())
                validationResults = await _sexValidator.AddNew(sex);
            else
                validationResults = await _sexValidator.Update(sex);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}