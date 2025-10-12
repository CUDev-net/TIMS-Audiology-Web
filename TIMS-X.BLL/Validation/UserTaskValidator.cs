using System.Collections.Generic;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IUserTaskValidator
{
    List<ValidationResult> AddNew(UserTask userTask);
    List<ValidationResult> Update(UserTask userTask);
}

public class UserTaskValidator : IUserTaskValidator
{
    private readonly IUserTaskUnitOfWork _userTaskUnitOfWork;

    public UserTaskValidator(IUserTaskUnitOfWork userTaskUnitOfWork)
    {
        _userTaskUnitOfWork = userTaskUnitOfWork;
    }

    public List<ValidationResult> AddNew(UserTask userTask)
    {
        var validationResults = _ValidateBase(userTask);
        return validationResults;
    }

    public List<ValidationResult> Update(UserTask userTask)
    {
        var validationResults = _ValidateBase(userTask);
        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(UserTask userTask)
    {
        var validationResults = new List<ValidationResult>();
        return validationResults;
    }
}