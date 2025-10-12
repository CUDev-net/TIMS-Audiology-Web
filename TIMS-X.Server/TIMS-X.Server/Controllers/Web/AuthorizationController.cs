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
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
    Roles = StringConstants.Customer)]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationRepository _authorizationRepository;
    private readonly IAuthorizationValidator _authorizationValidator;

    public AuthorizationController(IAuthorizationRepository authorizationRepository,
        IAuthorizationValidator authorizationValidator)
    {
        _authorizationRepository = authorizationRepository;
        _authorizationValidator = authorizationValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Authorization authorization)
    {
        try
        {
            var newAuthorization = await _authorizationRepository.Add(authorization);
            return Ok(newAuthorization);
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
            _authorizationRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAuthorizations()
    {
        var historyTypes = await _authorizationRepository.GetAll(false);

        return Ok(historyTypes);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var historyType = await _authorizationRepository.Get(id);
        if (historyType == null)
            return BadRequest(new { message = $"AppointmentType with {id} id not found" });

        return Ok(historyType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Authorization authorization)
    {
        try
        {
            var updated = await _authorizationRepository.Update(authorization);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Authorization authorization)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (authorization.IsNew())
                validationResults = await _authorizationValidator.AddNew(authorization);
            else
                validationResults = await _authorizationValidator.Update(authorization);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}