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
public class UserTaskTypeController : ControllerBase
{
    private readonly IUserTaskTypeRepository _userTaskTypeRepository;
    private readonly IUserTaskTypeValidator _userTaskTypeValidator;

    public UserTaskTypeController(IUserTaskTypeRepository userTaskTypeRepository,
        IUserTaskTypeValidator userTaskTypeValidator)
    {
        _userTaskTypeRepository = userTaskTypeRepository;
        _userTaskTypeValidator = userTaskTypeValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserTaskType userTaskType)
    {
        try
        {
            var newUserTaskType = await _userTaskTypeRepository.Add(userTaskType);
            return Ok(newUserTaskType);
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
            _userTaskTypeRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetUserTaskTypes()
    {
        var userTaskTypes = await _userTaskTypeRepository.GetAll(false);

        return Ok(userTaskTypes);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var userTaskType = await _userTaskTypeRepository.Get(id);
        if (userTaskType == null)
            return BadRequest(new { message = $"CommunicationRestriction with {id} id not found" });

        return Ok(userTaskType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(UserTaskType userTaskType)
    {
        try
        {
            var updated = await _userTaskTypeRepository.Update(userTaskType);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(UserTaskType userTaskType)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (userTaskType.IsNew())
                validationResults = await _userTaskTypeValidator.AddNew(userTaskType);
            else
                validationResults = await _userTaskTypeValidator.Update(userTaskType);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}