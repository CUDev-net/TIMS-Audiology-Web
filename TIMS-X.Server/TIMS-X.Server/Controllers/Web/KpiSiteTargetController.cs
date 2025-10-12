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
public class KpiSiteTargetController : ControllerBase
{
    private readonly IKpiSiteTargetRepository _kpiSiteTargetRepository;
    private readonly IKpiSiteTargetValidator _kpiSiteTargetValidator;

    public KpiSiteTargetController(IKpiSiteTargetRepository kpiSiteTargetRepository,
        IKpiSiteTargetValidator kpiSiteTargetValidator)
    {
        _kpiSiteTargetRepository = kpiSiteTargetRepository;
        _kpiSiteTargetValidator = kpiSiteTargetValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(KpiSiteTarget kpiSiteTarget)
    {
        try
        {
            var newKpiSiteTarget = await _kpiSiteTargetRepository.Add(kpiSiteTarget);
            return Ok(newKpiSiteTarget);
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
            _kpiSiteTargetRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetKpiSiteTargets()
    {
        var kpiSiteTargets = await _kpiSiteTargetRepository.GetAll(false);

        return Ok(kpiSiteTargets);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var kpiSiteTarget = await _kpiSiteTargetRepository.Get(id);
        if (kpiSiteTarget == null)
            return BadRequest(new { message = $"KpiSiteTarget with {id} id not found" });

        return Ok(kpiSiteTarget);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(KpiSiteTarget kpiSiteTarget)
    {
        try
        {
            var updated = await _kpiSiteTargetRepository.Update(kpiSiteTarget);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(KpiSiteTarget kpiSiteTarget)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (kpiSiteTarget.IsNew())
                validationResults = await _kpiSiteTargetValidator.AddNew(kpiSiteTarget);
            else
                validationResults = await _kpiSiteTargetValidator.Update(kpiSiteTarget);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}