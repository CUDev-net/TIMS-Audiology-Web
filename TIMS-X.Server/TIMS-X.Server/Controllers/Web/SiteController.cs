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
public class SiteController : ControllerBase
{
    private readonly ISiteRepository _siteRepository;
    private readonly ISiteValidator _siteValidator;

    public SiteController(ISiteRepository siteRepository,
        ISiteValidator siteValidator)
    {
        _siteRepository = siteRepository;
        _siteValidator = siteValidator;
    }

    [HttpGet("get-summaries")]
    public async Task<IActionResult> GetSummaries(bool includeInactive)
    {
        var sites = await _siteRepository.GetSummaries(includeInactive);
        return Ok(sites);
    }

    [HttpGet("get-summary")]
    public async Task<IActionResult> GetSummary(int id)
    {
        var sites = await _siteRepository.GetSummary(id);
        return Ok(sites);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Site site)
    {
        try
        {
            var newSite = await _siteRepository.Add(site);
            return Ok(newSite);
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
            _siteRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetSites()
    {
        var sites = await _siteRepository.GetSummaries(false);

        return Ok(sites);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var site = await _siteRepository.Get(id);
        if (site == null)
            return BadRequest(new { message = $"Site with {id} id not found" });

        return Ok(site);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Site site)
    {
        try
        {
            var updated = await _siteRepository.Update(site);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Site site)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (site.IsNew())
                validationResults = await _siteValidator.AddNew(site);
            else
                validationResults = await _siteValidator.Update(site);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}