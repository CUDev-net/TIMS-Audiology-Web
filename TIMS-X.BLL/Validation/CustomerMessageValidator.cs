using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface ICustomerMessageValidator
{
    Task<List<ValidationResult>> AddNew(CustomerMessage customerMessage);
    Task<List<ValidationResult>> Update(CustomerMessage customerMessage);
}

public class CustomerMessageValidator : ICustomerMessageValidator
{
    private readonly ICustomerMessageUnitOfWork _customerMessageUnitOfWork;

    public CustomerMessageValidator(ICustomerMessageUnitOfWork customerMessageUnitOfWork)
    {
        _customerMessageUnitOfWork = customerMessageUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(CustomerMessage customerMessage)
    {
        var validationResults = _ValidateBase(customerMessage);
        if (validationResults.Count == 0)
        {
            var existing = await _customerMessageUnitOfWork
                .GetCustomerMessages(a => a.Name == customerMessage.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(CustomerMessage customerMessage)
    {
        var validationResults = _ValidateBase(customerMessage);
        if (validationResults.Count == 0)
        {
            var existing = await _customerMessageUnitOfWork
                .GetCustomerMessages(
                    a => a.Name == customerMessage.Name && a.Id != customerMessage.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(CustomerMessage customerMessage)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(customerMessage.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}