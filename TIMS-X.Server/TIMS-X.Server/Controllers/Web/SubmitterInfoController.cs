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
public class SubmitterInfoController : ControllerBase
{
    private readonly ISubmitterInfoRepository _submitterInfoRepository;
    private readonly ISubmitterInfoValidator _submitterInfoValidator;

    public SubmitterInfoController(ISubmitterInfoRepository submitterInfoRepository,
        ISubmitterInfoValidator submitterInfoValidator)
    {
        _submitterInfoRepository = submitterInfoRepository;
        _submitterInfoValidator = submitterInfoValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(SubmitterInfo submitterInfo)
    {
        try
        {
            var newSubmitterInfo = await _submitterInfoRepository.Add(submitterInfo);
            return Ok(newSubmitterInfo);
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
            _submitterInfoRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetSubmitterInfo()
    {
        var submitterInfos = await _submitterInfoRepository.GetAll(false);

        return Ok(submitterInfos);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var submitterInfo = await _submitterInfoRepository.Get(id);
        if (submitterInfo == null)
            return BadRequest(new { message = $"SubmitterInfo with {id} id not found" });

        return Ok(submitterInfo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(SubmitterInfo submitterInfo)
    {
        try
        {
            var updated = await _submitterInfoRepository.Update(submitterInfo);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(SubmitterInfo submitterInfo)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (submitterInfo.IsNew())
                validationResults = await _submitterInfoValidator.AddNew(submitterInfo);
            else
                validationResults = await _submitterInfoValidator.Update(submitterInfo);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}