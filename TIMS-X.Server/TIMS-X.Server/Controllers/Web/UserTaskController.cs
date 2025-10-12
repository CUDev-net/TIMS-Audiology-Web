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
public class UserTaskController : ControllerBase
{
    private readonly IUserTaskRepository _userTaskRepository;
    private readonly IUserTaskValidator _userTaskValidator;

    public UserTaskController(IUserTaskRepository userTaskRepository,
        IUserTaskValidator userTaskValidator)
    {
        _userTaskRepository = userTaskRepository;
        _userTaskValidator = userTaskValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserTask userTask)
    {
        try
        {
            var newUserTask = await _userTaskRepository.Add(userTask);
            return Ok(newUserTask);
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
            _userTaskRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetUserTasks()
    {
        var userTasks = await _userTaskRepository.GetAll(false);

        return Ok(userTasks);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var userTask = await _userTaskRepository.Get(id);
        if (userTask == null)
            return BadRequest(new { message = $"UserTask with {id} id not found" });

        return Ok(userTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(UserTask userTask)
    {
        try
        {
            var updated = await _userTaskRepository.Update(userTask);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public IActionResult Validate(UserTask userTask)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (userTask.IsNew())
                validationResults = _userTaskValidator.AddNew(userTask);
            else
                validationResults = _userTaskValidator.Update(userTask);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}