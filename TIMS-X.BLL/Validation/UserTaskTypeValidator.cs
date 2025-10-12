using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IUserTaskTypeValidator
{
    Task<List<ValidationResult>> AddNew(UserTaskType userTaskType);
    Task<List<ValidationResult>> Update(UserTaskType userTaskType);
}

public class UserTaskTypeValidator : IUserTaskTypeValidator
{
    private readonly IUserTaskTypeUnitOfWork _userTaskTypeUnitOfWork;

    public UserTaskTypeValidator(IUserTaskTypeUnitOfWork userTaskTypeUnitOfWork)
    {
        _userTaskTypeUnitOfWork = userTaskTypeUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(UserTaskType userTaskType)
    {
        var validationResults = _ValidateBase(userTaskType);
        if (validationResults.Count == 0)
        {
            var existing = await _userTaskTypeUnitOfWork
                .GetUserTaskTypes(a => a.Name == userTaskType.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(UserTaskType userTaskType)
    {
        var validationResults = _ValidateBase(userTaskType);
        if (validationResults.Count == 0)
        {
            var existing = await _userTaskTypeUnitOfWork
                .GetUserTaskTypes(
                    a => a.Name == userTaskType.Name && a.Id != userTaskType.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(UserTaskType userTaskType)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(userTaskType.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}