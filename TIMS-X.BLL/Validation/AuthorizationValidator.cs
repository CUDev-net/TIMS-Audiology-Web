using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IAuthorizationValidator
{
    Task<List<ValidationResult>> AddNew(Authorization authorization);
    Task<List<ValidationResult>> Update(Authorization authorization);
}

public class AuthorizationValidator : IAuthorizationValidator
{
    private readonly IAuthorizationUnitOfWork _authorizationUnitOfWork;

    public AuthorizationValidator(IAuthorizationUnitOfWork authorizationUnitOfWork)
    {
        _authorizationUnitOfWork = authorizationUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(Authorization authorization)
    {
        var validationResults = _ValidateBase(authorization);
        if (validationResults.Count == 0)
        {
            var existing = await _authorizationUnitOfWork
                .GetAuthorizations(a => a.Name == authorization.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(Authorization authorization)
    {
        var validationResults = _ValidateBase(authorization);
        if (validationResults.Count == 0)
        {
            var existing = await _authorizationUnitOfWork
                .GetAuthorizations(
                    a => a.Name == authorization.Name && a.Id != authorization.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(Authorization authorization)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(authorization.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}