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
public class MarketingCategoryController : ControllerBase
{
    private readonly IMarketingCategoryRepository _marketingCategoryRepository;
    private readonly IMarketingCategoryValidator _marketingCategoryValidator;

    public MarketingCategoryController(IMarketingCategoryRepository marketingCategoryRepository,
        IMarketingCategoryValidator marketingCategoryValidator)
    {
        _marketingCategoryRepository = marketingCategoryRepository;
        _marketingCategoryValidator = marketingCategoryValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(MarketingReferenceCategory marketingCategory)
    {
        try
        {
            var newMarketingCategory = await _marketingCategoryRepository.Add(marketingCategory);
            return Ok(newMarketingCategory);
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
            _marketingCategoryRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAppointmentTypes()
    {
        var marketingCategories = await _marketingCategoryRepository.GetAll(false);

        return Ok(marketingCategories);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var marketingCategory = await _marketingCategoryRepository.Get(id);
        if (marketingCategory == null)
            return BadRequest(new { message = $"Marketing Category with {id} id not found" });

        return Ok(marketingCategory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(MarketingReferenceCategory marketingCategory)
    {
        try
        {
            var updated = await _marketingCategoryRepository.Update(marketingCategory);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(MarketingReferenceCategory marketingCategory)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (marketingCategory.IsNew())
                validationResults = await _marketingCategoryValidator.AddNew(marketingCategory);
            else
                validationResults = await _marketingCategoryValidator.Update(marketingCategory);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}