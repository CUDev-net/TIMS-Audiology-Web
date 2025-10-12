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
public class CustomerMessageController : ControllerBase
{
    private readonly ICustomerMessageRepository _customerMessageRepository;
    private readonly ICustomerMessageValidator _customerMessageValidator;

    public CustomerMessageController(ICustomerMessageRepository customerMessageRepository,
        ICustomerMessageValidator customerMessageValidator)
    {
        _customerMessageRepository = customerMessageRepository;
        _customerMessageValidator = customerMessageValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CustomerMessage customerMessage)
    {
        try
        {
            var newCustomerMessage = await _customerMessageRepository.Add(customerMessage);
            return Ok(newCustomerMessage);
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
            _customerMessageRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetCustomerMessages()
    {
        var customerMessages = await _customerMessageRepository.GetAll(false);

        return Ok(customerMessages);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var customerMessage = await _customerMessageRepository.Get(id);
        if (customerMessage == null)
            return BadRequest(new { message = $"CustomerMessage with {id} id not found" });

        return Ok(customerMessage);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(CustomerMessage customerMessage)
    {
        try
        {
            var updated = await _customerMessageRepository.Update(customerMessage);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(CustomerMessage customerMessage)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (customerMessage.IsNew())
                validationResults = await _customerMessageValidator.AddNew(customerMessage);
            else
                validationResults = await _customerMessageValidator.Update(customerMessage);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}