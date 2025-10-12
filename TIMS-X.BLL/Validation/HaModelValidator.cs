using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHaModelValidator
{
    Task<List<ValidationResult>> AddNew(HaModel haModel);
    Task<List<ValidationResult>> Update(HaModel haModel);
}

public class HaModelValidator : IHaModelValidator
{
    private readonly IHaModelUnitOfWork _haModelUnitOfWork;

    public HaModelValidator(IHaModelUnitOfWork haModelUnitOfWork)
    {
        _haModelUnitOfWork = haModelUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HaModel haModel)
    {
        var validationResults = _ValidateBase(haModel);
        if (validationResults.Count == 0)
        {
            var existing = await _haModelUnitOfWork
                .GetHaModels(a => a.Name == haModel.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(HaModel haModel)
    {
        var validationResults = _ValidateBase(haModel);
        if (validationResults.Count == 0)
        {
            var existing = await _haModelUnitOfWork
                .GetHaModels(
                    a => a.Name == haModel.Name && a.Id != haModel.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(HaModel haModel)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(haModel.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}