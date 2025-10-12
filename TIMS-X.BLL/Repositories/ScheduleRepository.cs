using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.BLL.Utilities;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.BLL.Repositories;

public interface IScheduleRepository
{
    Task<ScheduleDto> Add(Schedule schedule);
    void Delete(int id);
    Task<(RecurringIntervalRemoved, ScheduleDto)> DeleteOccurrence(RecurringIntervalRemoved recurringIntervalRemoved);
    Task<Schedule> Get(int id);

    Task<List<ScheduleRecurringItemSummary>> GetRecurringScheduleSummaries(DateTime startDate, DateTime endDate,
        int userId);

    Task<List<ScheduleRecurringItemSummary>> GetRecurringScheduleSummariesForDay(DateTime startDate,
        int userId);

    Task<List<ScheduleItemSummary>> GetScheduleSummaries(DateTime startDate, DateTime endDate, int userId);
    Task<ScheduleDto> Update(Schedule schedule);
}

public class ScheduleRepository : IScheduleRepository
{
    private readonly IProvidersUnitOfWork _providersUnitOfWork;
    private readonly IRecurringIntervalRemovedUnitOfWork _recurringIntervalRemovedUnitOfWork;
    private readonly IScheduleUnitOfWork _scheduleUnitOfWork;
    private readonly ISiteUnitOfWork _siteUnitOfWork;
    private readonly IUserUnitOfWork _userUnitOfWork;

    public ScheduleRepository(IScheduleUnitOfWork scheduleUnitOfWork, IProvidersUnitOfWork providers,
        ISiteUnitOfWork siteUnitOfWork, IUserUnitOfWork userUnitOfWork,
        IRecurringIntervalRemovedUnitOfWork recurringIntervalRemovedUnitOfWork)
    {
        _scheduleUnitOfWork = scheduleUnitOfWork;
        _providersUnitOfWork = providers;
        _siteUnitOfWork = siteUnitOfWork;
        _userUnitOfWork = userUnitOfWork;
        _recurringIntervalRemovedUnitOfWork = recurringIntervalRemovedUnitOfWork;
    }

    public async Task<ScheduleDto> Add(Schedule schedule)
    {
        schedule.RecurringIntervalId ??= 0;
        _SaveMetaData(schedule);
        if (schedule.RecurringInterval != null) schedule.RecurringInterval.CalculateDateRange(schedule);
        var updatedSchedule = new ScheduleDto(await _scheduleUnitOfWork.Add(schedule));
        await _SetMetaData(updatedSchedule);
        if (schedule.RecurringInterval != null)
        {
            updatedSchedule.RecurringItemSummary = await _scheduleUnitOfWork.GetRecurringScheduleSummary(schedule.Id);
            _HydrateRecurringData(updatedSchedule.RecurringItemSummary);
            _HydrateMetaData(updatedSchedule.RecurringItemSummary);
        }

        return updatedSchedule;
    }

    public void Delete(int id)
    {
        _scheduleUnitOfWork.Delete(id);
    }

    public async Task<(RecurringIntervalRemoved, ScheduleDto)> DeleteOccurrence(
        RecurringIntervalRemoved recurringIntervalRemoved)
    {
        var deletedOccurrence = await _recurringIntervalRemovedUnitOfWork.Add(recurringIntervalRemoved);
        var schedule = new ScheduleDto(await _scheduleUnitOfWork.GetSchedule(recurringIntervalRemoved.ScheduleId));
        schedule.RecurringItemSummary = await _scheduleUnitOfWork.GetRecurringScheduleSummary(schedule.Schedule.Id);
        _HydrateRecurringData(schedule.RecurringItemSummary);
        _HydrateMetaData(schedule.RecurringItemSummary);
        if (schedule.Schedule.Color.HasValue)
            schedule.Schedule.Web_Color = ColorHelper.GetHexColor(schedule.Schedule.Color.Value);

        return (deletedOccurrence, schedule);
    }

    public async Task<Schedule> Get(int id)
    {
        var schedule = await _scheduleUnitOfWork.GetSchedule(id, s => s.Include(x => x.RecurringInterval));
        if (schedule.UpdatedUserId.HasValue)
        {
            var user = await _userUnitOfWork.GetUser(schedule.UpdatedUserId.Value);
            if (user != null) schedule.UpdatedByUserName = user.Name;
        }

        if (schedule.Color.HasValue) schedule.Web_Color = ColorHelper.GetHexColor(schedule.Color.Value);
        return schedule;
    }

    public async Task<List<ScheduleRecurringItemSummary>> GetRecurringScheduleSummariesForDay(DateTime startDate,
        int userId)
    {
        var user = await _userUnitOfWork.GetUser(userId);
        // Get all recurring schedules for the day
        var instances = new List<ScheduleRecurringItemSummary>();
        var items = await _scheduleUnitOfWork.GetRecurringScheduleSummaries(startDate, startDate, user);
        foreach (var item in items)
        {
            var id = int.Parse(item.id.Substring(2));
            var recurringItem = await _scheduleUnitOfWork.GetSchedule(id, x => x.Include(s => s.RecurringInterval));
            var occurrences =
                recurringItem.RecurringInterval.GetOccurrences(startDate, startDate.AddDays(1), startDate);
            if (occurrences.Any())
            {
                _HydrateMetaData(item);
                instances.Add(item);
            }
        }

        return instances;
    }

    public async Task<List<ScheduleRecurringItemSummary>> GetRecurringScheduleSummaries(DateTime startDate,
        DateTime endDate, int userId)
    {
        var user = await _userUnitOfWork.GetUser(userId);
        var items = await _scheduleUnitOfWork.GetRecurringScheduleSummaries(startDate, endDate, user);
        foreach (var item in items)
        {
            _HydrateRecurringData(item);
            foreach (var itemSummaryRemovedInstance in item.RemovedInstances)
                item.DeletedOccurrences.Add(itemSummaryRemovedInstance.ItemNumber);
            _HydrateMetaData(item);
        }

        return items;
    }

    public async Task<ScheduleDto> Update(Schedule schedule)
    {
        var recurring = schedule.RecurringInterval;
        _SaveMetaData(schedule);
        // Don't update recurring data
        if (schedule.RecurringInterval != null)
            schedule.RecurringInterval.HasBeenAudited = true;
        var updatedSchedule = new ScheduleDto(await _scheduleUnitOfWork.Update(schedule));
        await _SetMetaData(updatedSchedule);
        if (recurring != null)
        {
            updatedSchedule.Schedule.RecurringInterval = recurring;
            updatedSchedule.RecurringItemSummary = await _scheduleUnitOfWork.GetRecurringScheduleSummary(schedule.Id);
            _HydrateRecurringData(updatedSchedule.RecurringItemSummary);
        }

        return updatedSchedule;
    }

    public async Task<List<ScheduleItemSummary>> GetScheduleSummaries(DateTime startDate, DateTime endDate, int userId)
    {
        var user = await _userUnitOfWork.GetUser(userId);
        var items = await _scheduleUnitOfWork.GetScheduleSummaries(startDate, endDate, user).ToListAsync();
        foreach (var item in items) _HydrateMetaData(item);

        return items;
    }

    private void _HydrateMetaData(IScheduleItemSummary item)
    {
        item.color_web = ColorHelper.GetHexColor(item.color);
        item.site_web_color =
            ColorHelper.GetHexColor(item.site_color_value);
        item.provider_web_color =
            ColorHelper.GetHexColor(item.provider_color_value);
    }

    private void _HydrateRecurringData(ScheduleRecurringItemSummary itemSummary)
    {
        var length = itemSummary.end_date - itemSummary.start_date;
        itemSummary.event_length = length.TotalSeconds.ToString();
        itemSummary.start_date = itemSummary.recurrence_start_date;
        switch (itemSummary.end_type)
        {
            case 0:
            case 1:
                itemSummary.end_date = DateTime.MaxValue;
                break;
            case 2:
            case 3:
                itemSummary.end_date = itemSummary.recurrence_end_date;
                break;
        }

        var sb = new StringBuilder();
        itemSummary.event_pid = "0";
        switch (itemSummary.interval_type)
        {
            case 1:
                switch (itemSummary.interval_sub_type)
                {
                    case 1:
                        sb.Append($"day_{itemSummary.day_interval}___");
                        break;
                    case 2:
                        sb.Append("week_1___1,2,3,4,5");
                        break;
                    case 3:
                        sb.Append("week_1___1,3,5");
                        break;
                    case 4:
                        sb.Append("week_1___2,4");
                        break;
                }

                break;
            case 2:
                sb.Append($"week_{itemSummary.week_interval}___");
                var dayOfWeek = new StringBuilder();
                if (itemSummary.is_monday_set)
                    dayOfWeek.Append("1,");
                if (itemSummary.is_tuesday_set)
                    dayOfWeek.Append("2,");
                if (itemSummary.is_wednesday_set)
                    dayOfWeek.Append("3,");
                if (itemSummary.is_thursday_set)
                    dayOfWeek.Append("4,");
                if (itemSummary.is_friday_set)
                    dayOfWeek.Append("5,");
                if (itemSummary.is_saturday_set)
                    dayOfWeek.Append("6,");
                if (itemSummary.is_sunday_set)
                    dayOfWeek.Append("7,");
                sb.Append(dayOfWeek.ToString().TrimEnd(','));
                break;
            case 3:
                switch (itemSummary.interval_sub_type)
                {
                    case 1:
                        sb.Append($"month_{itemSummary.month_interval}___");
                        break;
                    case 2:
                        sb.Append(
                            $"month_{itemSummary.month_interval}_{itemSummary.day_of_week}_{itemSummary.day_qualifier}_");
                        break;
                }

                break;
            case 4:
                sb.Append("year_1___");
                break;
        }

        switch (itemSummary.end_type)
        {
            case 0:
            case 1:
                sb.Append("#no");
                break;
            case 2:
                sb.Append($"#{itemSummary.end_after}");
                break;
        }

        itemSummary.rec_type = sb.ToString();
    }

    private void _SaveMetaData(Schedule schedule)
    {
        if (string.IsNullOrEmpty(schedule.Web_Color))
            schedule.Color = null;
        else
            schedule.Color = ColorHelper.GetWindowsColor(schedule.Web_Color);
    }

    private async Task _SetMetaData(ScheduleDto schedule)
    {
        var site = await _siteUnitOfWork.GetSite(schedule.Schedule.SiteId);
        schedule.SiteName = site.Name;
        schedule.Site_Color =
            ColorHelper.GetHexColor(site.Color);

        var provider = await _providersUnitOfWork.GetProvider(schedule.Schedule.ProviderId);
        schedule.ProviderName = $"{provider.FirstName} {provider.LastName}";
        schedule.Provider_Color =
            ColorHelper.GetHexColor(provider.Color);
    }
}