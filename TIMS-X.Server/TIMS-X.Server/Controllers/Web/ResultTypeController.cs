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
public class ResultTypeController : ControllerBase
{
    private readonly IResultTypeRepository _resultTypeRepository;
    private readonly IResultTypeValidator _resultTypeValidator;

    public ResultTypeController(IResultTypeRepository resultTypeRepository,
        IResultTypeValidator resultTypeValidator)
    {
        _resultTypeRepository = resultTypeRepository;
        _resultTypeValidator = resultTypeValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(ResultType resultType)
    {
        try
        {
            var newResultType = await _resultTypeRepository.Add(resultType);
            return Ok(newResultType);
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
            _resultTypeRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetResultTypes()
    {
        var resultTypes = await _resultTypeRepository.GetAll(false);

        return Ok(resultTypes);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var resultType = await _resultTypeRepository.Get(id);
        if (resultType == null)
            return BadRequest(new { message = $"ResultType with {id} id not found" });

        return Ok(resultType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(ResultType resultType)
    {
        try
        {
            var updated = await _resultTypeRepository.Update(resultType);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(ResultType resultType)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (resultType.IsNew())
                validationResults = await _resultTypeValidator.AddNew(resultType);
            else
                validationResults = await _resultTypeValidator.Update(resultType);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message }); 
        }
    }
}