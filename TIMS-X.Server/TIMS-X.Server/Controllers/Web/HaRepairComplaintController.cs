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
public class HaRepairComplaintController : ControllerBase
{
    private readonly IHaRepairComplaintRepository _haRepairComplaintRepository;
    private readonly IHaRepairComplaintValidator _haRepairComplaintValidator;

    public HaRepairComplaintController(IHaRepairComplaintRepository haRepairComplaintRepository,
        IHaRepairComplaintValidator haRepairComplaintValidator)
    {
        _haRepairComplaintRepository = haRepairComplaintRepository;
        _haRepairComplaintValidator = haRepairComplaintValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HaRepairComplaint repairComplaint)
    {
        try
        {
            var newRepairComplaint = await _haRepairComplaintRepository.Add(repairComplaint);
            return Ok(newRepairComplaint);
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
            _haRepairComplaintRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetRepairComplaints()
    {
        var results = await _haRepairComplaintRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var repairComplaint = await _haRepairComplaintRepository.Get(id);
        if (repairComplaint == null)
            return BadRequest(new { message = $"RepairComplaint with {id} id not found" });

        return Ok(repairComplaint);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HaRepairComplaint haRepairComplaint)
    {
        try
        {
            var updated = await _haRepairComplaintRepository.Update(haRepairComplaint);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HaRepairComplaint haRepairComplaint)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (haRepairComplaint.IsNew())
                validationResults = await _haRepairComplaintValidator.AddNew(haRepairComplaint);
            else
                validationResults = await _haRepairComplaintValidator.Update(haRepairComplaint);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}