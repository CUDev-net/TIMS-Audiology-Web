using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IScheduleBlockValidator
{
    Task<List<ValidationResult>> AddNew(ScheduleBlock scheduleBlock);
    Task<List<ValidationResult>> Update(ScheduleBlock scheduleBlock);
}

public class ScheduleBlockValidator : IScheduleBlockValidator
{
    private readonly IScheduleBlockUnitOfWork _scheduleBlockUnitOfWork;

    public ScheduleBlockValidator(IScheduleBlockUnitOfWork scheduleBlockUnitOfWork)
    {
        _scheduleBlockUnitOfWork = scheduleBlockUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(ScheduleBlock scheduleBlock)
    {
        var validationResults = _ValidateBase(scheduleBlock);
        if (validationResults.Count == 0)
        {
            var existing = await _scheduleBlockUnitOfWork
                .GetScheduleBlocks(a => a.Name == scheduleBlock.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(ScheduleBlock scheduleBlock)
    {
        var validationResults = _ValidateBase(scheduleBlock);
        if (validationResults.Count == 0)
        {
            var existing = await _scheduleBlockUnitOfWork
                .GetScheduleBlocks(
                    a => a.Name == scheduleBlock.Name && a.Id != scheduleBlock.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(ScheduleBlock scheduleBlock)
    {
        var validationResults = new List<ValidationResult>();
        if (scheduleBlock.EndDate.HasValue && scheduleBlock.StartDate.HasValue)
        {
            if (scheduleBlock.EndDate <= scheduleBlock.StartDate)
                validationResults.Add(new ValidationResult("End date must be after start date", Severity.Error));
            if (scheduleBlock.StartDate <= DateTime.Now)
                validationResults.Add(
                    new ValidationResult("Schedule Block is being made in the past", Severity.Warning));
        }

        return validationResults;
    }
}