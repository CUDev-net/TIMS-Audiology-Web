using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface ICommunicationRestrictionValidator
{
    Task<List<ValidationResult>> AddNew(CommunicationRestriction communicationRestriction);
    Task<List<ValidationResult>> Update(CommunicationRestriction communicationRestriction);
}

public class CommunicationRestrictionValidator : ICommunicationRestrictionValidator
{
    private readonly ICommunicationRestrictionUnitOfWork _communicationRestrictionUnitOfWork;

    public CommunicationRestrictionValidator(ICommunicationRestrictionUnitOfWork communicationRestrictionUnitOfWork)
    {
        _communicationRestrictionUnitOfWork = communicationRestrictionUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(CommunicationRestriction communicationRestriction)
    {
        var validationResults = _ValidateBase(communicationRestriction);
        if (validationResults.Count == 0)
        {
            var existing = await _communicationRestrictionUnitOfWork
                .GetCommunicationRestrictions(a => a.Name == communicationRestriction.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(CommunicationRestriction communicationRestriction)
    {
        var validationResults = _ValidateBase(communicationRestriction);
        if (validationResults.Count == 0)
        {
            var existing = await _communicationRestrictionUnitOfWork
                .GetCommunicationRestrictions(
                    a => a.Name == communicationRestriction.Name && a.Id != communicationRestriction.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(CommunicationRestriction communicationRestriction)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(communicationRestriction.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}