using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IScheduleUnitOfWork : IUnitOfWork
{

    void DeleteOccurrence(int id, int occurrenceId, DateTime itemDate);

    Task<List<ScheduleRecurringItemSummary>> GetRecurringScheduleSummaries(DateTime startDate, DateTime endDate,
        User user);

    Task<ScheduleRecurringItemSummary> GetRecurringScheduleSummary(int id);

    Task<Schedule> GetSchedule(long id,
        Func<IQueryable<Schedule>, IIncludableQueryable<Schedule, object>> includes = null);

    IQueryable<Schedule> GetSchedules(Expression<Func<Schedule, bool>> filter = null,
        Func<IQueryable<Schedule>, IOrderedQueryable<Schedule>> orderBy = null,
        Func<IQueryable<Schedule>, IIncludableQueryable<Schedule, object>> includes = null);

    IQueryable<ScheduleItemSummary> GetScheduleSummaries(DateTime startDate, DateTime endDate, User user);
}

public class ScheduleUnitOfWork : UnitOfWorkBase, IScheduleUnitOfWork
{
    public ScheduleUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(Schedule);

    public async Task<Schedule> GetSchedule(long id,
        Func<IQueryable<Schedule>, IIncludableQueryable<Schedule, object>> includes = null)
    {
        return await Single(u => u.Id == id, includes);
    }

    public IQueryable<ScheduleItemSummary> GetScheduleSummaries(DateTime startDate, DateTime endDate, User user)
    {
        object[] sqlParams =
        {
            new SqlParameter("@fromdate", startDate),
            new SqlParameter("@todate", endDate)
        };
        var sql = new StringBuilder(@"select *
		                from vw_web_scheduler_schedules
                            where start_date >= @fromdate and end_date <= @todate ");
        if (!string.IsNullOrEmpty(user.ScheduleProviderFilter) && user.ScheduleProviderFilter != "0")
        {
            var ids = user.ScheduleProviderFilter.Split('-');
            sql.AppendLine($"and provider_id in ({string.Join(',', ids)})");
        }

        if (!string.IsNullOrEmpty(user.ScheduleSiteFilter) && user.ScheduleSiteFilter != "0")
        {
            var ids = user.ScheduleSiteFilter.Split('-');
            sql.AppendLine($"and site_id in ({string.Join(',', ids)})");
        }

        return FromSql<ScheduleItemSummary>(sql.ToString(), sqlParams);
    }

    public async Task<ScheduleRecurringItemSummary> GetRecurringScheduleSummary(int id)
    {
        object[] sqlParams =
        {
            new SqlParameter("@id", $"X-{id}")
        };
        var sql = new StringBuilder(@"
                    select *
		                from vw_web_scheduler_recurring_schedules
                            where id = @id ");
        var recurringItem =
            await FromSql<ScheduleRecurringItemSummary>(sql.ToString(), sqlParams).FirstOrDefaultAsync();

        object[] deletedSqlParams =
        {
            new SqlParameter("@id", id)
        };

        var deletedItems = await FromSql<RecurringIntervalRemoved>(@"
select 
    sch.ID scheduleId,
    rr.ID Id,
    rr.ItemNum,
    rr.ItemDate,
    rr.RecurringInvervalID,
    rr.CreatedDate
 from RecurringIntervalRemoved rr 
	join RecurringInterval r on rr.RecurringInvervalID = r.ID
    join Schedule sch on r.ID = sch.RecurringIntervalId
    where sch.ID = @id
", deletedSqlParams).ToListAsync();

        foreach (var intervalRemoved in deletedItems.Distinct()) recurringItem.RemovedInstances.Add(intervalRemoved);
        return recurringItem;
    }

    public IQueryable<Schedule> GetSchedules(Expression<Func<Schedule, bool>> filter = null,
        Func<IQueryable<Schedule>, IOrderedQueryable<Schedule>> orderBy = null,
        Func<IQueryable<Schedule>, IIncludableQueryable<Schedule, object>> includes = null)
    {
        return Get(filter, orderBy, includes);
    }

    public async Task<List<ScheduleRecurringItemSummary>> GetRecurringScheduleSummaries(DateTime startDate,
        DateTime endDate, User user)
    {
        object[] sqlParams =
        {
            new SqlParameter("@fromdate", startDate),
            new SqlParameter("@todate", endDate)
        };
        var sql = new StringBuilder(@"
                    select *
		                from vw_web_scheduler_recurring_schedules
                            where ( (end_type < 2 
                                and start_date is not null and start_date <= @todate)
                            or recurrence_end_date >= @fromdate ) ");
        if (!string.IsNullOrEmpty(user.ScheduleProviderFilter) && user.ScheduleProviderFilter != "0")
        {
            var ids = user.ScheduleProviderFilter.Split('-');
            sql.AppendLine($"and provider_id in ({string.Join(',', ids)})");
        }

        if (!string.IsNullOrEmpty(user.ScheduleSiteFilter) && user.ScheduleSiteFilter != "0")
        {
            var ids = user.ScheduleSiteFilter.Split('-');
            sql.AppendLine($"and site_id in ({string.Join(',', ids)})");
        }

        var items = await FromSql<ScheduleRecurringItemSummary>(sql.ToString(), sqlParams).ToListAsync();

        object[] deletedSqlParams =
        {
            new SqlParameter("@fromdate", startDate),
            new SqlParameter("@todate", endDate)
        };

        var deletedItems = await FromSql<RecurringIntervalRemoved>(@"
select 
    rr.ID Id,
    rr.ItemNum,
    rr.ItemDate,
    rr.RecurringInvervalID,
    rr.CreatedDate
 from RecurringIntervalRemoved rr 
	join RecurringInterval r on rr.RecurringInvervalID = r.ID
	where ( r.EndType < 2 and r.DtStart < @todate )
	or (r.DtEnd > @fromdate)
    order by rr.ItemNum asc
", deletedSqlParams).ToListAsync();

        foreach (var intervalRemoved in deletedItems.Distinct())
        {
            var scheduleItem =
                items.FirstOrDefault(x => x.recurring_interval_id == intervalRemoved.RecurringIntervalId);
            if (scheduleItem != null) scheduleItem.RemovedInstances.Add(intervalRemoved);
        }

        return items;
    }

    public new void Delete(int id)
    {
        object[] sqlParams =
        {
            new SqlParameter("@id", id),
            new SqlParameter("@userid", id)
        };
        ExecuteSqlCommand("exec sp_delete_and_archive_schedule @id, @userid", sqlParams);
        ExecuteSqlCommand($"DELETE from Schedule  where ID = {id}");
    }

    public void DeleteOccurrence(int id, int occurrenceId, DateTime itemDate)
    {
        object[] sqlParams =
        {
            new SqlParameter("@id", id),
            new SqlParameter("@userid", id)
        };
    }
}