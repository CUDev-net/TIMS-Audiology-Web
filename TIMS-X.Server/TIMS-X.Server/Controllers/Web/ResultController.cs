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
public class ResultController : ControllerBase
{
    private readonly IResultRepository _resultRepository;
    private readonly IResultValidator _resultValidator;

    public ResultController(IResultRepository resultRepository,
        IResultValidator resultValidator)
    {
        _resultRepository = resultRepository;
        _resultValidator = resultValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Result result)
    {
        try
        {
            var newResult = await _resultRepository.Add(result);
            return Ok(newResult);
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
            _resultRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetResults()
    {
        var results = await _resultRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _resultRepository.Get(id);
        if (result == null)
            return BadRequest(new { message = $"Result with {id} id not found" });

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Result result)
    {
        try
        {
            var updated = await _resultRepository.Update(result);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Result result)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (result.IsNew())
                validationResults = await _resultValidator.AddNew(result);
            else
                validationResults = await _resultValidator.Update(result);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}