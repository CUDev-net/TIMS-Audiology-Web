using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.BLL.Repositories;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.Dtos;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = $"{StringConstants.Customer},{StringConstants.Support}")]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("get-current-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = ClaimHelper.GetUserIdFromClaim(User);
        UserDto user;
        try
        {
            user = await _userRepository.GetUserAndPatients(userId);
        }
        catch (Exception e)
        {
            Console.Write(e);
            throw new Exception($"get-current-user:GetCurrentUser Error: UserID-{userId} ", e);
        }

        if (user == null)
            return new NotFoundResult();

        return Ok(user);
    }

    [HttpGet("get-patient-list")]
    public async Task<IActionResult> GetPatientList()
    {
        List<PatientSummary> patientList;
        var userId = ClaimHelper.GetUserIdFromClaim(User);
        try
        {
            patientList = await _userRepository.GetUserPatients(userId);
        }
        catch (Exception e)
        {
            Console.Write(e);
            throw new Exception($"get-patient-list:GetPatientList Error: UserID-{userId}", e);
        }

        if (patientList == null)
            return new NotFoundResult();

        return Ok(patientList);
    }

    [HttpGet("get-permissions")]
    public async Task<IActionResult> GetUserPermissions(int userId)
    {
        try
        {
            var permissions = await _userRepository.GetUserPermissions(userId);
            return Ok(permissions);
        }
        catch (Exception e)
        {
            Console.Write(e);
            throw new Exception($"get-permissions Error: UserID-{userId} ", e);
        }
    }

    [HttpGet("SignOut")]
    public async Task<IActionResult> Signout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Account/SignIn");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(UserDto userDto)
    {
        var user = await _userRepository.Update(userDto.User);
        return Ok(user);
    }
}