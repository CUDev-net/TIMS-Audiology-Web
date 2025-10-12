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
public class ManufacturerController : ControllerBase
{
    private readonly IManufacturerRepository _manufacturerRepository;
    private readonly IManufacturerValidator _manufacturerValidator;

    public ManufacturerController(IManufacturerRepository manufacturerRepository,
        IManufacturerValidator manufacturerValidator)
    {
        _manufacturerRepository = manufacturerRepository;
        _manufacturerValidator = manufacturerValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Manufacturer manufacturer)
    {
        try
        {
            var newManufacturer = await _manufacturerRepository.Add(manufacturer);
            return Ok(newManufacturer);
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
            _manufacturerRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetManufacturers()
    {
        var results = await _manufacturerRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var manufacturer = await _manufacturerRepository.Get(id);
        if (manufacturer == null)
            return BadRequest(new { message = $"Manufacturer with {id} id not found" });

        return Ok(manufacturer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Manufacturer manufacturer)
    {
        try
        {
            var updated = await _manufacturerRepository.Update(manufacturer);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Manufacturer manufacturer)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (manufacturer.IsNew())
                validationResults = await _manufacturerValidator.AddNew(manufacturer);
            else
                validationResults = await _manufacturerValidator.Update(manufacturer);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}