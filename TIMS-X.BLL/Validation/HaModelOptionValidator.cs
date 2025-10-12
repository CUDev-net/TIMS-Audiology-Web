using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHaModelOptionValidator
{
    Task<List<ValidationResult>> AddNew(HaModelOption haModelOption);
    Task<List<ValidationResult>> Update(HaModelOption haModelOption);
}

public class HaModelOptionValidator : IHaModelOptionValidator
{
    private readonly IHaModelOptionUnitOfWork _haModelOptionUnitOfWork;

    public HaModelOptionValidator(IHaModelOptionUnitOfWork haModelOptionUnitOfWork)
    {
        _haModelOptionUnitOfWork = haModelOptionUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HaModelOption haModelOption)
    {
        var validationResults = await _ValidateBase(haModelOption);
        if (validationResults.Count == 0)
        {
            var existing = await _haModelOptionUnitOfWork
                .GetHaModelOptions().FirstOrDefaultAsync();
            if (existing != null)
                validationResults.Add(new ValidationResult("ID must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(HaModelOption haModelOption)
    {
        var validationResults = await _ValidateBase(haModelOption);
        return validationResults;
    }

    private async Task<List<ValidationResult>> _ValidateBase(HaModelOption haModelOption)
    {
        var validationResults = new List<ValidationResult>();
        var x = await _haModelOptionUnitOfWork.GetHaModelOption(haModelOption.Id);
        if (x == null)
            validationResults.Add(new ValidationResult("HaModelID must exist", Severity.Error));
        return validationResults;
    }
}