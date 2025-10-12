using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Tims.Dal.Models;
using TIMS_X.Core;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IAppointmentsUnitOfWork : IUnitOfWork
{

    Task<Appointment> GetAppointment(int id,
        Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>> includes = null);

    IQueryable<Appointment> GetAppointments(Expression<Func<Appointment, bool>> filter = null,
        Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>> orderBy = null,
        Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>> includes = null);

    IQueryable<AppointmentItemSummary> GetAppointmentSummaries(DateTime startDate, DateTime endDate, User user);

    Task<Appointment> GetLastAppointmentForPatient(long patientId, DateTime dateTime);
    Task<Appointment> GetNextAppointmentForPatient(long patientId, DateTime dateTime);

    IQueryable<Appointment> GetPatientAppointments(int patientId, int appointmentId, DateTime startDate,
        DateTime endDate);

    IQueryable<AppointmentSummary> GetPatientAppointmentSummaries(int patientId);

    bool HasClaimData(int id);

    bool HasPosDocument(int id);
}

public class AppointmentsUnitOfWork : UnitOfWorkBase, IAppointmentsUnitOfWork
{
    private readonly ContextHelper _contextHelper;

    public AppointmentsUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor,
        ContextHelper contextHelper) :
        base(context, httpContextAccessor)
    {
        _contextHelper = contextHelper;
    }

    protected override string TableName => nameof(Appointment);

    public async Task<Appointment> GetAppointment(int id,
        Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>> includes = null)
    {
        return await Single(u => u.Id == id, includes);
    }

    public IQueryable<Appointment> GetAppointments(Expression<Func<Appointment, bool>> filter = null,
        Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>> orderBy = null,
        Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>> includes = null)
    {
        var appointments = Get(filter, orderBy, includes);
        return appointments;
    }

    public IQueryable<AppointmentSummary> GetPatientAppointmentSummaries(int patientId)
    {
        var sql = @"select 
a.ID,
a.PatID PatientId,
stat.Name AppointmentStatus,
atype.Name AppointmentType,
s.Name Site,
p.FName ProviderFirstName,
p.LName ProviderLastName,
a.ApptStart StartsAt,
a.ApptEnd EndsAt,
a.DtUpdated UpdatedDate
from Appointment a
left outer join AppointmentStatus stat on a.ApptStatusID = stat.ID
left outer join AppointmentType atype on a.ApptTypeID = atype.ID
left outer join Site s on a.SiteID = s.ID
left outer join Provider p on a.ProviderID = p.ID
where a.PatID = @patientId and stat.Show = 1 
-- Do not get parent recurring for appointments
and a.RecurringIntervalID is null
order by ApptStart desc
";

        var sqlParams = new object[]
        {
            new SqlParameter("@patientId", patientId)
        };

        return FromSql<AppointmentSummary>(sql, sqlParams);
    }

    public new void Delete(int id)
    {
        object[] sqlParams =
        {
            new SqlParameter("@id", id),
            new SqlParameter("@userid", id)
        };
        ExecuteSqlCommand("exec sp_delete_and_archive_appointment @id, @userid", sqlParams);
    }

    public bool HasPosDocument(int id)
    {
        var sqlParams = new object[]
        {
            new SqlParameter("@id", id)
        };
        var sql = "select count(*) as Value from POSDocument where AppointmentID = @id";
        var count = FromSql<IntReturn>(sql, sqlParams).AsEnumerable().First().Value;
        return count > 0;
    }

    public bool HasClaimData(int id)
    {
        var sqlParams = new object[]
        {
            new SqlParameter("@id", id)
        };
        var sql = "select count(*) as Value from ClaimTransaction where AppointmentID = @id";
        var count = FromSql<IntReturn>(sql, sqlParams).AsEnumerable().First().Value;
        return count > 0;
    }

    public IQueryable<AppointmentItemSummary> GetAppointmentSummaries(DateTime startDate, DateTime endDate, User user)
    {
        object[] sqlParams =
        {
            new SqlParameter("@fromdate", startDate),
            new SqlParameter("@todate", endDate)
        };
        var sql = new StringBuilder(@"select *
                  from vw_web_scheduler_appointments 
                        where status_show=1 and start_date >= @fromdate and end_date <= @todate ");
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
        if (!string.IsNullOrEmpty(user.ScheduleResourceFilter) && user.ScheduleResourceFilter != "0")
        {
            var ids = user.ScheduleResourceFilter.Split('-');
            sql.AppendLine($"and resource_id in ({string.Join(',', ids)})");
        }

        if (!string.IsNullOrEmpty(user.ScheduleSpecialtyFilter) && user.ScheduleSpecialtyFilter != "0")
        {
            if (user.ScheduleSpecialtyFilter == "1")
            {
                sql.AppendLine(" and appointment_type_slp != 2");
            }
            else if (user.ScheduleSpecialtyFilter == "2")
            {
                sql.AppendLine(" and appointment_type_slp = 2");
            }
        }

        return FromSql<AppointmentItemSummary>(sql.ToString(), sqlParams);
    }

    public async Task<Appointment> GetLastAppointmentForPatient(long patientId, DateTime dateTime)
    {
        return await Get<Appointment>(p => p.PatientId == patientId && p.StartsAt < dateTime &&
                                           p.AppointmentStatus.Show, null,
	        a => a.Include( x => x.AppointmentStatus))
            .OrderByDescending(a => a.StartsAt).FirstOrDefaultAsync();
    }

    public async Task<Appointment> GetNextAppointmentForPatient(long patientId, DateTime dateTime)
    {
        return await Get<Appointment>(p => p.PatientId == patientId && p.StartsAt > dateTime &&
                                           p.AppointmentStatus.Show &&
                                           (p.AppointmentStatus.Name == "New" ||
                                            p.AppointmentStatus.Name.Contains("Confirmed")), null,
				a => a.Include(x => x.AppointmentStatus))
            .OrderBy(a => a.StartsAt).FirstOrDefaultAsync();
    }

    public IQueryable<Appointment> GetPatientAppointments(int patientId, int appointmentId, DateTime startDate,
        DateTime endDate)
    {
        IQueryable<Appointment> query = _context.Set<Appointment>();
        return query.Where(s =>
            s.PatientId == patientId &&
            s.Id != appointmentId &&
            s.StartsAt >= startDate.Date &&
            s.EndsAt <= endDate.Date
        );
    }
}