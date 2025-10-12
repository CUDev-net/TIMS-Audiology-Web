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
public class ResourceController : ControllerBase
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IResourceValidator _resourceValidator;

    public ResourceController(IResourceRepository resourceRepository,
        IResourceValidator resourceValidator)
    {
        _resourceRepository = resourceRepository;
        _resourceValidator = resourceValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Resource resource)
    {
        try
        {
            var newResource = await _resourceRepository.Add(resource);
            return Ok(newResource);
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
            _resourceRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetResources()
    {
        var resources = await _resourceRepository.GetAll(false);

        return Ok(resources);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var resource = await _resourceRepository.Get(id);
        if (resource == null)
            return BadRequest(new { message = $"Resource with {id} id not found" });

        return Ok(resource);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Resource resource)
    {
        try
        {
            var updated = await _resourceRepository.Update(resource);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(Resource resource)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (resource.IsNew())
                validationResults = await _resourceValidator.AddNew(resource);
            else
                validationResults = await _resourceValidator.Update(resource);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}