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
public class RepairComplaintController : ControllerBase
{
    private readonly IRepairComplaintRepository _repairComplaintRepository;
    private readonly IRepairComplaintValidator _repairComplaintValidator;

    public RepairComplaintController(IRepairComplaintRepository repairComplaintRepository,
        IRepairComplaintValidator repairComplaintValidator)
    {
        _repairComplaintRepository = repairComplaintRepository;
        _repairComplaintValidator = repairComplaintValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(RepairComplaint repairComplaint)
    {
        try
        {
            var newRepairComplaint = await _repairComplaintRepository.Add(repairComplaint);
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
            _repairComplaintRepository.Delete(id);
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
        var results = await _repairComplaintRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var repairComplaint = await _repairComplaintRepository.Get(id);
        if (repairComplaint == null)
            return BadRequest(new { message = $"RepairComplaint with {id} id not found" });

        return Ok(repairComplaint);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(RepairComplaint repairComplaint)
    {
        try
        {
            var updated = await _repairComplaintRepository.Update(repairComplaint);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(RepairComplaint repairComplaint)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (repairComplaint.IsNew())
                validationResults = await _repairComplaintValidator.AddNew(repairComplaint);
            else
                validationResults = await _repairComplaintValidator.Update(repairComplaint);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}