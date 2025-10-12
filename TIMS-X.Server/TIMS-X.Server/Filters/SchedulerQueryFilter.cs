using System;
using System.Collections.Generic;
using System.Linq;
using TIMS_X.Core.Domain;

namespace TIMS_X.Server.Filters;

public class SchedulerQueryFilter
{
	public List<int> ApptTypeIds { get; set; }
	public DateTime From { get; set; }

	public bool IncludeHiddenAppointments { get; set; }
	public bool IncludeNonPatientAppointments { get; set; } = true;

	public bool IncludePatientAppointments { get; set; } = true;
	public bool IncludeRecurringAppointments { get; set; } = true;
	public List<int> ProviderIds { get; set; }
	public List<int> ResourceIds { get; set; }
	public List<int> SiteIds { get; set; }
	public List<int> SpecialtyIds { get; set; }
	public DateTime To { get; set; }

	public IQueryable<Appointment> ApplyTo(IQueryable<Appointment> query)
	{
		query = query.Where(appt => appt.StartsAt >= From && appt.StartsAt <= To);

		if (ProviderIds != null && ProviderIds.Any())
			query = query.Where(appt => ProviderIds.Contains(appt.ProviderId));
		if (SiteIds != null && SiteIds.Any()) query = query.Where(appt => SiteIds.Contains(appt.SiteId));
		if (ResourceIds != null && ResourceIds.Any())
			query = query.Where(appt => ResourceIds.Contains(appt.ResourceId ?? 0));
		if (SpecialtyIds != null && SpecialtyIds.Any())
		{
			var includeAud = SpecialtyIds.Contains(0);
			var includeSlp = SpecialtyIds.Contains(1);
			// If include both, there's no need to modify query
			if (includeAud ^ includeSlp)
				query = query.Where(appt => (appt.Slp && includeSlp) || (!appt.Slp && includeAud));
		}

		/*if (ApptTypeIds != null && ApptTypeIds.Any())
		{
		    query = query.Where(appt => ApptTypeIds.Contains(appt.AppointmentTypeId));
		}*/

		// Unless explicitly requested, omit results that are not visible on the schedule.
		if (!IncludeHiddenAppointments)
			query = query.Where(appt => appt.AppointmentStatus == null || appt.AppointmentStatus.Show);

		return query;
	}

	public IQueryable<Schedule> ApplyTo(IQueryable<Schedule> query)
	{
		query = query.Where(appt => appt.StartsAt >= From && appt.StartsAt <= To);

		if (ProviderIds != null && ProviderIds.Any())
			query = query.Where(appt => ProviderIds.Contains(appt.ProviderId));
		if (SiteIds != null && SiteIds.Any()) query = query.Where(appt => SiteIds.Contains(appt.SiteId));

		return query;
	}
}