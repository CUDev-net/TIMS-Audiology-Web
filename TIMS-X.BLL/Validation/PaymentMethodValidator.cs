using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IPaymentMethodValidator
{
    Task<List<ValidationResult>> AddNew(PaymentMethod paymentMethod);
    Task<List<ValidationResult>> Update(PaymentMethod paymentMethod);
}

public class PaymentMethodValidator : IPaymentMethodValidator
{
    private readonly IPaymentMethodUnitOfWork _paymentMethodUnitOfWork;

    public PaymentMethodValidator(IPaymentMethodUnitOfWork paymentMethodUnitOfWork)
    {
        _paymentMethodUnitOfWork = paymentMethodUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(PaymentMethod paymentMethod)
    {
        var validationResults = _ValidateBase(paymentMethod);
        if (validationResults.Count == 0)
        {
            var existing = await _paymentMethodUnitOfWork
                .GetPaymentMethods(a => a.Name == paymentMethod.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(PaymentMethod paymentMethod)
    {
        var validationResults = _ValidateBase(paymentMethod);
        if (validationResults.Count == 0)
        {
            var existing = await _paymentMethodUnitOfWork
                .GetPaymentMethods(
                    a => a.Name == paymentMethod.Name && a.Id != paymentMethod.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(PaymentMethod paymentMethod)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(paymentMethod.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}