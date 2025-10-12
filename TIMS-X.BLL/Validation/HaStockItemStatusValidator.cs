using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHaStockItemStatusValidator
{
    Task<List<ValidationResult>> AddNew(HaStockItemStatus haStockItemStatus);
    Task<List<ValidationResult>> Update(HaStockItemStatus haStockItemStatus);
}

public class HaStockItemStatusValidator : IHaStockItemStatusValidator
{
    private readonly IHaStockItemStatusUnitOfWork _haStockItemStatusUnitOfWork;

    public HaStockItemStatusValidator(IHaStockItemStatusUnitOfWork haStockItemStatusUnitOfWork)
    {
        _haStockItemStatusUnitOfWork = haStockItemStatusUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HaStockItemStatus haStockItemStatus)
    {
        var validationResults = _ValidateBase(haStockItemStatus);
        if (validationResults.Count == 0)
        {
            var existing = await _haStockItemStatusUnitOfWork
                .GetHaStockItemStatuses(a => a.Name == haStockItemStatus.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(HaStockItemStatus haStockItemStatus)
    {
        var validationResults = _ValidateBase(haStockItemStatus);
        if (validationResults.Count == 0)
        {
            var existing = await _haStockItemStatusUnitOfWork
                .GetHaStockItemStatuses(
                    a => a.Name == haStockItemStatus.Name && a.Id != haStockItemStatus.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(HaStockItemStatus haStockItemStatus)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(haStockItemStatus.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}