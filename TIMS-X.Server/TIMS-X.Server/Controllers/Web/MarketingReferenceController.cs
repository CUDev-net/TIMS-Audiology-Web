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
public class MarketingReferenceController : ControllerBase
{
    private readonly IMarketingReferenceRepository _marketingReferenceRepository;
    private readonly IMarketingReferenceValidator _marketingReferenceValidator;

    public MarketingReferenceController(IMarketingReferenceRepository marketingReferenceRepository,
        IMarketingReferenceValidator marketingReferenceValidator)
    {
        _marketingReferenceRepository = marketingReferenceRepository;
        _marketingReferenceValidator = marketingReferenceValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(MarketingReference marketingReference)
    {
        try
        {
            var newMarketingReference = await _marketingReferenceRepository.Add(marketingReference);
            return Ok(newMarketingReference);
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
            _marketingReferenceRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetMarketingReferences(int marketingCategoryId)
    {
        var marketingReferences = await _marketingReferenceRepository.GetAll(marketingCategoryId, false);

        return Ok(marketingReferences);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var marketingReference = await _marketingReferenceRepository.Get(id);
        if (marketingReference == null)
            return BadRequest(new { message = $"Marketing Reference with {id} id not found" });

        return Ok(marketingReference);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(MarketingReference marketingReference)
    {
        try
        {
            var updated = await _marketingReferenceRepository.Update(marketingReference);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(MarketingReference marketingReference)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (marketingReference.IsNew())
                validationResults = await _marketingReferenceValidator.AddNew(marketingReference);
            else
                validationResults = await _marketingReferenceValidator.Update(marketingReference);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}