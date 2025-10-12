using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.BLL.Repositories;

public interface IPracticeRepository
{
    Task<BusinessRuleDto> GetBusinessRules();
    Task<Practice> GetPractice();
    Task<IEnumerable<HoursOfOperationModel>> GetPracticeHours();
    Task<PracticeSummary> GetPracticeSummary();
}

public class PracticeRepository : IPracticeRepository
{
    private readonly IPracticeUnitOfWork _practiceUnitOfWork;
    private readonly ISiteUnitOfWork _siteUnitOfWork;

    public PracticeRepository(IPracticeUnitOfWork practiceUnitOfWork, ISiteUnitOfWork siteUnitOfWork)
    {
        _practiceUnitOfWork = practiceUnitOfWork;
        _siteUnitOfWork = siteUnitOfWork;
    }

    public async Task<Practice> GetPractice()
    {
        return await _practiceUnitOfWork.GetPractice();
    }

    public async Task<BusinessRuleDto> GetBusinessRules()
    {
        var br = await _practiceUnitOfWork.GetPracticeBusinessRules();
        return new BusinessRuleDto(br);
    }

    public async Task<PracticeSummary> GetPracticeSummary()
    {
        var practice = await _practiceUnitOfWork.GetPracticeSummary();
        return practice;
    }

    public async Task<IEnumerable<HoursOfOperationModel>> GetPracticeHours()
    {
        var hours = new List<HoursOfOperationModel>();
        var sites = await _siteUnitOfWork.GetSites();
        hours.Add(_GetHours(DayOfWeek.Sunday, sites));
        hours.Add(_GetHours(DayOfWeek.Monday, sites));
        hours.Add(_GetHours(DayOfWeek.Tuesday, sites));
        hours.Add(_GetHours(DayOfWeek.Wednesday, sites));
        hours.Add(_GetHours(DayOfWeek.Thursday, sites));
        hours.Add(_GetHours(DayOfWeek.Friday, sites));
        hours.Add(_GetHours(DayOfWeek.Saturday, sites));

        return hours;
    }

    private static DateTime? _GetEndTime(Site site, DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Sunday:
                return site.SunEnd;
            case DayOfWeek.Monday:
                return site.MonEnd;
            case DayOfWeek.Tuesday:
                return site.TuesEnd;
            case DayOfWeek.Wednesday:
                return site.WedEnd;
            case DayOfWeek.Thursday:
                return site.ThurEnd;
            case DayOfWeek.Friday:
                return site.FriEnd;
            case DayOfWeek.Saturday:
                return site.SatEnd;
            default:
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null);
        }
    }

    private static HoursOfOperationModel _GetHours(DayOfWeek day, IEnumerable<Site> sites)
    {
        return new HoursOfOperationModel
        {
            Day = day,
            StartTime = GetSiteDayStartTime(day, sites),
            EndTime = GetSiteDayEndTime(day, sites)
        };
    }

    private static DateTime? _GetStartTime(Site site, DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Sunday:
                return site.SunStart;
            case DayOfWeek.Monday:
                return site.MonStart;
            case DayOfWeek.Tuesday:
                return site.TuesStart;
            case DayOfWeek.Wednesday:
                return site.WedStart;
            case DayOfWeek.Thursday:
                return site.ThurStart;
            case DayOfWeek.Friday:
                return site.FriStart;
            case DayOfWeek.Saturday:
                return site.SatStart;
            default:
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null);
        }
    }

    public static DateTime? GetSiteDayEndTime(DayOfWeek dayOfWeek, IEnumerable<Site> sites)
    {
        DateTime? workTime = null;
        var now = DateTime.Now;

        foreach (var site in sites)
        {
            var startTime = _GetEndTime(site, dayOfWeek);
            if (startTime.HasValue)
            {
                if (!workTime.HasValue)
                {
                    workTime = startTime;
                }
                else
                {
                    if (workTime.Value.TimeOfDay.CompareTo(startTime.Value.TimeOfDay) == 1) workTime = startTime;
                }
            }
        }

        if (!workTime.HasValue) workTime = new DateTime(now.Year, now.Month, now.Day, 0, 2, 0);

        return workTime;
    }

    public static DateTime? GetSiteDayStartTime(DayOfWeek dayOfWeek, IEnumerable<Site> sites)
    {
        DateTime? workTime = null;
        var now = DateTime.Now;

        foreach (var site in sites)
        {
            var startTime = _GetStartTime(site, dayOfWeek);
            if (startTime.HasValue)
            {
                if (!workTime.HasValue)
                {
                    workTime = startTime;
                }
                else
                {
                    if (workTime.Value.TimeOfDay.CompareTo(startTime.Value.TimeOfDay) == -1) workTime = startTime;
                }
            }
        }

        if (!workTime.HasValue) workTime = new DateTime(now.Year, now.Month, now.Day, 0, 2, 0);
        return workTime;
    }
}