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
public class HaStockItemStatusController : ControllerBase
{
    private readonly IHaStockItemStatusRepository _haStockItemStatusRepository;
    private readonly IHaStockItemStatusValidator _haStockItemStatusValidator;

    public HaStockItemStatusController(IHaStockItemStatusRepository haStockItemStatusRepository,
        IHaStockItemStatusValidator haStockItemStatusValidator)
    {
        _haStockItemStatusRepository = haStockItemStatusRepository;
        _haStockItemStatusValidator = haStockItemStatusValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HaStockItemStatus haStockItemStatus)
    {
        try
        {
            var newHaStockItemStatus = await _haStockItemStatusRepository.Add(haStockItemStatus);
            return Ok(newHaStockItemStatus);
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
            _haStockItemStatusRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetHaStockItemStatuses()
    {
        var results = await _haStockItemStatusRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var haStockItemStatus = await _haStockItemStatusRepository.Get(id);
        if (haStockItemStatus == null)
            return BadRequest(new { message = $"StockItemStatus with {id} id not found" });

        return Ok(haStockItemStatus);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HaStockItemStatus haStockItemStatus)
    {
        try
        {
            var updated = await _haStockItemStatusRepository.Update(haStockItemStatus);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HaStockItemStatus haStockItemStatus)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (haStockItemStatus.IsNew())
                validationResults = await _haStockItemStatusValidator.AddNew(haStockItemStatus);
            else
                validationResults = await _haStockItemStatusValidator.Update(haStockItemStatus);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}