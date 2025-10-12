using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.BLL.Utilities;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public class BaseValidator
{
    protected async Task<List<ValidationResult>> ValidateProviderHours(int providerId, int siteId,
        DateTime starTime, DateTime endTime,
        IProvidersUnitOfWork providersUnitOfWork,
        IUserUnitOfWork timsUser,
        ITimsUserSiteUnitOfWork timsUserSiteUnitOfWork)
    {
        var validationResults = new List<ValidationResult>();
        if (providerId > 0)
        {
            var appointmentProvider = await providersUnitOfWork.GetProvider(providerId);
            if (appointmentProvider != null)
            {
                var user = await timsUser.GetUser(appointmentProvider.UserId);
                if (user != null)
                {
                    var dayNumber = starTime.GetDayOfWeekAsNumber();
                    var siteHours = await timsUserSiteUnitOfWork.GetUserDayAndSite(user.Id, dayNumber, siteId);
                    if (siteHours == null || !siteHours.StartTime.HasValue || !siteHours.EndTime.HasValue ||
                        starTime.TimeOfDay < siteHours.StartTime.Value.TimeOfDay ||
                        endTime.TimeOfDay > siteHours.EndTime.Value.TimeOfDay)
                        validationResults.Add(new ValidationResult("The provider is not scheduled for this site at this time",
                            Severity.Warning));
                }
            }
        }

        return validationResults;
    }

    protected List<ValidationResult> SortResults(List<ValidationResult> validationResults)
    {
        validationResults = validationResults.Distinct().ToList();
        var errors = validationResults.Where(v => v.Severity == Severity.Error).ToList();
        if (errors.Any())
        {
            // Bring errors to front of list
            errors.AddRange(validationResults.Where(v => v.Severity == Severity.Warning));
            return errors;
        }

        return validationResults;
    }
}