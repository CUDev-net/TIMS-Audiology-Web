using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IScheduleValidator
{
    Task<List<ValidationResult>> AddNew(Schedule schedule);
    List<ValidationResult> Delete(Schedule schedule);
    Task<List<ValidationResult>> Update(Schedule schedule);
}

public class ScheduleValidator : BaseValidator, IScheduleValidator
{
    private readonly IProvidersUnitOfWork _providerRepository;
    private readonly IUserUnitOfWork _timsUserRepository;
    private readonly ITimsUserSiteUnitOfWork _timsUserSiteRepository;

    public ScheduleValidator(IProvidersUnitOfWork providerRepository,
        IUserUnitOfWork timsUserRepository,
        ITimsUserSiteUnitOfWork timsUserSiteRepository)
    {
        _providerRepository = providerRepository;
        _timsUserRepository = timsUserRepository;
        _timsUserSiteRepository = timsUserSiteRepository;
    }

    public async Task<List<ValidationResult>> AddNew(Schedule schedule)
    {
        var validationResults = _ValidateBase(schedule);

        foreach (var scheduleProviderId in schedule.ProviderIds)
        {
            foreach (var scheduleSiteId in schedule.SiteIds)
            {
                var providerHoursValidation = await ValidateProviderHours(scheduleProviderId, scheduleSiteId,
                    schedule.StartsAt, schedule.EndsAt, _providerRepository, _timsUserRepository,
                    _timsUserSiteRepository);
                validationResults.AddRange(providerHoursValidation);
            }
        }

        return SortResults(validationResults);
    }

    public async Task<List<ValidationResult>> Update(Schedule schedule)
    {
        var validationResults = _ValidateBase(schedule);

        var providerHoursValidation = await ValidateProviderHours(schedule.ProviderId, schedule.SiteId,
            schedule.StartsAt, schedule.EndsAt, _providerRepository, _timsUserRepository,
            _timsUserSiteRepository);
        validationResults.AddRange(providerHoursValidation);

        return SortResults(validationResults);
    }

    public List<ValidationResult> Delete(Schedule schedule)
    {
        var result = new List<ValidationResult>();

        return result;
    }

    private List<ValidationResult> _ValidateBase(Schedule schedule)
    {
        var validationResults = new List<ValidationResult>();
        if (schedule.EndsAt <= schedule.StartsAt)
            validationResults.Add(new ValidationResult("End must be after Start", Severity.Error));
        if (string.IsNullOrWhiteSpace(schedule.Title))
            validationResults.Add(new ValidationResult("Title is required", Severity.Error));
        if (schedule.Id == 0)
        {
            if (schedule.ProviderIds == null || schedule.ProviderIds.Length == 0)
            {
                validationResults.Add(new ValidationResult("Provider is required", Severity.Error));
            }
            if (schedule.SiteIds == null || schedule.SiteIds.Length == 0)
            {
                validationResults.Add(new ValidationResult("Site is required", Severity.Error));
            }
        }
        else
        {
            if (schedule.ProviderId == 0)
                validationResults.Add(new ValidationResult("Provider is required", Severity.Error));
            if (schedule.SiteId == 0)
                validationResults.Add(new ValidationResult("Site is required", Severity.Error));
        }
        if (schedule.StartsAt <= DateTime.Now)
            validationResults.Add(new ValidationResult("Schedule is being made in the past", Severity.Warning));

        return validationResults;
    }
}