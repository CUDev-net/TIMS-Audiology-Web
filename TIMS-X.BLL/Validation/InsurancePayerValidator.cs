using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IInsurancePayerValidator
{
    Task<List<ValidationResult>> AddNew(InsurancePayer insurancePayer);
    Task<List<ValidationResult>> Update(InsurancePayer insurancePayer);
}

public class InsurancePayerValidator : IInsurancePayerValidator
{
    private readonly IInsurancePayerUnitOfWork _insurancePayerUnitOfWork;

    public InsurancePayerValidator(IInsurancePayerUnitOfWork insurancePayerUnitOfWork)
    {
        _insurancePayerUnitOfWork = insurancePayerUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(InsurancePayer insurancePayer)
    {
        var validationResults = _ValidateBase(insurancePayer);
        if (validationResults.Count == 0)
        {
            var existing = await _insurancePayerUnitOfWork
                .GetInsurancePayers(a => a.Name == insurancePayer.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(InsurancePayer insurancePayer)
    {
        var validationResults = _ValidateBase(insurancePayer);
        if (validationResults.Count == 0)
        {
            var existing = await _insurancePayerUnitOfWork
                .GetInsurancePayers(
                    a => a.Name == insurancePayer.Name && a.Id != insurancePayer.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(InsurancePayer insurancePayer)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(insurancePayer.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}