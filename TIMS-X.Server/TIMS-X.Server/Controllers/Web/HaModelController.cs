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
public class HaModelController : ControllerBase
{
    private readonly IHaModelRepository _haModelRepository;
    private readonly IHaModelValidator _haModelValidator;

    public HaModelController(IHaModelRepository haModelRepository,
        IHaModelValidator haModelValidator)
    {
        _haModelRepository = haModelRepository;
        _haModelValidator = haModelValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(HaModel haModel)
    {
        try
        {
            var newHaModel = await _haModelRepository.Add(haModel);
            return Ok(newHaModel);
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
            _haModelRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetHaModels()
    {
        var results = await _haModelRepository.GetAll(false);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var haModel = await _haModelRepository.Get(id);
        if (haModel == null)
            return BadRequest(new { message = $"HaModel with {id} id not found" });

        return Ok(haModel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(HaModel haModel)
    {
        try
        {
            var updated = await _haModelRepository.Update(haModel);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(HaModel haModel)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (haModel.IsNew())
                validationResults = await _haModelValidator.AddNew(haModel);
            else
                validationResults = await _haModelValidator.Update(haModel);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}