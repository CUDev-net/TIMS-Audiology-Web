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
public class PaymentMethodController : ControllerBase
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IPaymentMethodValidator _paymentMethodValidator;

    public PaymentMethodController(IPaymentMethodRepository paymentMethodRepository,
        IPaymentMethodValidator paymentMethodValidator)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _paymentMethodValidator = paymentMethodValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(PaymentMethod paymentMethod)
    {
        try
        {
            var newPaymentMethod = await _paymentMethodRepository.Add(paymentMethod);
            return Ok(newPaymentMethod);
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
            _paymentMethodRepository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        var paymentMethods = await _paymentMethodRepository.GetAll(false);

        return Ok(paymentMethods);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var paymentMethod = await _paymentMethodRepository.Get(id);
        if (paymentMethod == null)
            return BadRequest(new { message = $"PaymentMethod with {id} id not found" });

        return Ok(paymentMethod);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(PaymentMethod paymentMethod)
    {
        try
        {
            var updated = await _paymentMethodRepository.Update(paymentMethod);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(PaymentMethod paymentMethod)
    {
        try
        {
            List<ValidationResult> validationResults;
            if (paymentMethod.IsNew())
                validationResults = await _paymentMethodValidator.AddNew(paymentMethod);
            else
                validationResults = await _paymentMethodValidator.Update(paymentMethod);
            return Ok(validationResults);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}