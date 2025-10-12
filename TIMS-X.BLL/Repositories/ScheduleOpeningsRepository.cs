using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.DAL.UoWs;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.BLL.Repositories;

public interface IScheduleOpeningsRepository
{
	Task<IEnumerable<ScheduleOpeningModel>> FetchScheduleOpenings(ScheduleOpeningsSearchModel model);
}

public class ScheduleOpeningsRepository : IScheduleOpeningsRepository
{
	private readonly IAppointmentsUnitOfWork _appointmentsUnitOfWork;
	private readonly IAppointmentTypeUnitOfWork _appointmentTypeUnitOfWork;
	private readonly IPracticeUnitOfWork _practiceUnitOfWork;
	private readonly IProviderBlockScheduleUnitOfWork _providerBlockScheduleUnitOfWork;
	private readonly IProviderRepository _providerRepository;
	private readonly IResourceUnitOfWork _resourceUnitOfWork;
	private readonly IScheduleBlockUnitOfWork _scheduleBlockUnitOfWork;
	private readonly IScheduleUnitOfWork _scheduleUnitOfWork;
	private IList<Resource> _resources;
	private bool _useResources;

	public ScheduleOpeningsRepository(IResourceUnitOfWork resourcesUnitOfWork,
		IPracticeUnitOfWork practiceUnitOfWork,
		IAppointmentTypeUnitOfWork appointmentTypeUnitOfWork,
		IProviderBlockScheduleUnitOfWork providerBlockScheduleUnitOfWork,
		IProviderRepository providerRepository,
		IAppointmentsUnitOfWork appointmentsUnitOfWork,
		IScheduleUnitOfWork scheduleUnitOfWork,
		IScheduleBlockUnitOfWork scheduleBlockUnitOfWork)
	{
		_resourceUnitOfWork = resourcesUnitOfWork;
		_practiceUnitOfWork = practiceUnitOfWork;
		_appointmentTypeUnitOfWork = appointmentTypeUnitOfWork;
		_providerBlockScheduleUnitOfWork = providerBlockScheduleUnitOfWork;
		_providerRepository = providerRepository;
		_appointmentsUnitOfWork = appointmentsUnitOfWork;
		_scheduleUnitOfWork = scheduleUnitOfWork;
		_scheduleBlockUnitOfWork = scheduleBlockUnitOfWork;
	}

	public async Task<IEnumerable<ScheduleOpeningModel>> FetchScheduleOpenings(ScheduleOpeningsSearchModel model)
	{
		var defaultResults = Enumerable.Empty<ScheduleOpeningModel>();
		DateTime now;
		// Fix the start and end dates.
		if (model.StartDate.HasValue && model.StartDate.Value.Date != DateTime.Now.Date)
			now = DateTime.Now.Date;
		else
			now = DateTime.Now.Date.AddHours(DateTime.Now.Hour);
		var startDate = model.StartDate.GetValueOrDefault(now).SetTime(now);
		var endDate = model.EndDate.GetValueOrDefault(startDate.AddDays(30)).ToEndOfDay();

		// Preconditions:
		if (model.DurationTotalMinutes <= 0) return defaultResults;
		// Set up access to resources if we are searching by any.
		await _SetupAccessToResources(startDate, endDate, model);

		var openings = await _FetchMasterListOfOpenings(startDate, endDate, model);

		// Exclude times in the past.
		_ExcludeTimesInThePast(openings, startDate);

		// Exclude the times relating to schedule blocks.
		_ExcludeTimesRelatedToScheduleBlocks(openings, startDate, endDate, model).GetAwaiter().GetResult();

		// Exclude the times relating to existing appointments.
		_ExcludeTimesRelatedToExistingAppointments(openings, startDate, endDate, model).GetAwaiter().GetResult();

		// Exclude the times relating to existing schedules.
		_ExcludeTimesRelatedToExistingSchedules(openings, startDate, endDate, model).GetAwaiter().GetResult();

		openings = openings.Where(o => o.DurationTotalMinutes >= model.DurationTotalMinutes)
			.OrderBy(o => o.StartsAt)
			.ThenBy(o => o.ProviderLastName)
			.ThenBy(o => o.ProviderFirstName).ToList();

		return openings;
	}

	private void _ApplyScheduleExclusions(ICollection<ScheduleOpeningModel> openings,
		IEnumerable<ScheduleExclusion> scheduleExclusions)
	{
		var scheduleExclusionsByDay = scheduleExclusions.ToLookup(x => x.StartsAt.Date);
		_ApplyScheduleExclusions(openings, scheduleExclusionsByDay);
	}

	private void _ApplyScheduleExclusions(ICollection<ScheduleOpeningModel> openings,
		ILookup<DateTime, ScheduleExclusion> scheduleExclusionsByDay)
	{
		// Going to track new and removed openings. 
		// New openings need to have this same logic applied -- recursively -- until no more new openings are created.
		// Openings that get cut to pieces will be removed from the collection.
		var newOpenings = new List<ScheduleOpeningModel>();
		var closedOpenings = new List<ScheduleOpeningModel>();

		// Run through the openings, and compare against exclusions for the day.
		var openingsByDay = openings.GroupBy(x => x.StartsAt.Date, x => x).ToList();

		foreach (var openingsForDay in openingsByDay)
		{
			// Get the exclusions for the day. 
			// If none, then continue since following loop is dependent on such exclusions.
			var exclusionsForDay = scheduleExclusionsByDay[openingsForDay.Key].ToList();
			if (!exclusionsForDay.Any()) continue;

			foreach (var opening in openingsForDay)
			{
				// Get exclusions that affect this opening.
				var applicableExclusions = (from x in exclusionsForDay
											where x.ProviderId == opening.ProviderId
												  || (x.ResourceId.HasValue && x.ResourceId == opening.ResourceId)
											select x).ToList();

				// Make adjustments to opening times as necessary.
				foreach (var exclusion in applicableExclusions)
					// Check for donut hole
					if (exclusion.StartsAt >= opening.StartsAt && exclusion.EndsAt <= opening.EndsAt)
					{
						// Clone the current opening into a new opening starting when the exclusion ends
						var newOpening = opening.CloneJson();
						newOpening.StartsAt = exclusion.EndsAt;
						newOpening.EndsAt = opening.EndsAt;
						newOpenings.Add(newOpening);

						// Adjust the existing opening to end at the beginning of the hole.
						opening.EndsAt = exclusion.StartsAt;
					}
					else // No donut hole; possible overlap.
					{
						// Adjust overlaps with opening start time
						if (exclusion.StartsAt < opening.StartsAt && exclusion.EndsAt > opening.StartsAt)
							opening.StartsAt = exclusion.EndsAt;

						// Adjust overlaps with opening end time
						if (exclusion.StartsAt < opening.EndsAt && exclusion.EndsAt > opening.EndsAt)
							opening.EndsAt = exclusion.StartsAt;
					}

				// If the opening has no duration, schedule it for removal from the list.
				if (opening.StartsAt >= opening.EndsAt) closedOpenings.Add(opening);

				// Next opening....
			}

			// Next set of openings by day....
		}

		// Remove openings.
		closedOpenings.ForEach(o => openings.Remove(o));

		// If there are no new openings, we're done.
		if (!newOpenings.Any()) return;

		// Otherwise, run this logic on the new openings and add them to the rest.
		_ApplyScheduleExclusions(newOpenings, scheduleExclusionsByDay);
		newOpenings.ForEach(openings.Add);
	}

	private async Task<bool> _BlockSchedulingApplies(ScheduleOpeningsSearchModel model)
	{
		var practice = await _practiceUnitOfWork.GetPracticeSummary();
		if (!practice.UseBlockScheduling) return false;
		if (!model.AppointmentTypeId.HasValue) return false;
		var appointmentTypes = await _appointmentTypeUnitOfWork.GetAppointmentTypes(
			a => a.Id == model.AppointmentTypeId.Value, null,
			x => x.Include(a => a.ScheduleBlock)
				.ThenInclude(a => a.ProviderBlockSchedules));
		var appointmentType = appointmentTypes.FirstOrDefault();
		return appointmentType?.ScheduleBlock != null;
	}

	public static int ConvertMStoTIMSDayOfWeek(DayOfWeek dayOfWeek)
	{
		// MS Day of week is 0 based, Mon = 0, Sun = 6
		// TIMS is 1 based, Mon = 1, Sun = 7
		switch (dayOfWeek)
		{
			case DayOfWeek.Monday:
				return 1;
			case DayOfWeek.Tuesday:
				return 2;
			case DayOfWeek.Wednesday:
				return 3;
			case DayOfWeek.Thursday:
				return 4;
			case DayOfWeek.Friday:
				return 5;
			case DayOfWeek.Saturday:
				return 6;
			default:
				return 7;
		}
	}

	private void _ExcludeTimesInThePast(ICollection<ScheduleOpeningModel> openings, DateTime startDate)
	{
		// Going to exclude past times for all providers and sites included in the openings.
		var providerIds = openings.Select(x => x.ProviderId).Distinct();
		var siteIds = openings.Select(x => x.SiteId).Distinct();

		var exclusions = from providerId in providerIds
						 from siteId in siteIds
						 select new ScheduleExclusion
						 {
							 EndsAt = startDate,
							 ProviderId = providerId,
							 StartsAt = startDate.Date
						 };

		_ApplyScheduleExclusions(openings, exclusions);
	}

	private async Task _ExcludeTimesRelatedToExistingAppointments(ICollection<ScheduleOpeningModel> openings,
		DateTime startDate, DateTime endDate, ScheduleOpeningsSearchModel model)
	{
		var appointmentsQuery = _appointmentsUnitOfWork.GetAppointments(
			a => a.StartsAt >= startDate && a.EndsAt <= endDate &&
				 a.AppointmentStatus.Name != "Cancelled" && a.AppointmentStatus.Name != "Rescheduled");

		var masterConflictList = new List<List<Appointment>>(); //super awesome

		// Filter appointments by provider, site, and resource.
		if (model.ResourceIds.Any())
		{
			var resourceConflicts = await appointmentsQuery
				.Where(a => a.ResourceId.HasValue && model.ResourceIds.Contains(a.ResourceId.Value)).ToListAsync();
			if (resourceConflicts.Any()) masterConflictList.Add(resourceConflicts);
		}

		if (model.ProviderIds.Any())
		{
			var providerConflicts =
				await appointmentsQuery.Where(a => model.ProviderIds.Contains(a.ProviderId)).ToListAsync();
			if (providerConflicts.Any()) masterConflictList.Add(providerConflicts);
		}

		if (model.SiteIds.Any())
		{
			var siteConflicts = await appointmentsQuery.Where(a => model.SiteIds.Contains(a.SiteId)).ToListAsync();
			if (siteConflicts.Any()) masterConflictList.Add(siteConflicts);
		}

		var exclusions = new List<ScheduleExclusion>();
		foreach (var list in masterConflictList)
			foreach (var appointment in list)
				// Non-recurring
				exclusions.Add(
					new ScheduleExclusion
					{
						EndsAt = appointment.EndsAt,
						ProviderId = appointment.ProviderId,
						ResourceId = appointment.ResourceId,
						StartsAt = appointment.StartsAt
					});
		// Adjust the openings.
		_ApplyScheduleExclusions(openings, exclusions);
	}

	private async Task _ExcludeTimesRelatedToExistingSchedules(ICollection<ScheduleOpeningModel> openings,
		DateTime startDate, DateTime endDate, ScheduleOpeningsSearchModel model)
	{
		// Get schedules; both recurring and non.
		var dummyUser = new User();
		var schedules = await _scheduleUnitOfWork.GetScheduleSummaries(startDate, endDate, dummyUser).ToListAsync();
		var recurringSchedules = await _scheduleUnitOfWork.GetRecurringScheduleSummaries(startDate, endDate, dummyUser);


		// Filter schedules by provider and site.
		if (model.ProviderIds.Any())
		{
			schedules = schedules
				.Where(a => a.provider_id.HasValue && model.ProviderIds.Contains(a.provider_id.Value)).ToList();
			recurringSchedules = recurringSchedules
				.Where(a => a.provider_id.HasValue && model.ProviderIds.Contains(a.provider_id.Value)).ToList();
		}

		if (model.SiteIds.Any())
		{
			schedules = schedules.Where(a => a.site_id.HasValue && model.SiteIds.Contains(a.site_id.Value))
				.ToList();
			recurringSchedules = recurringSchedules
				.Where(a => a.site_id.HasValue && model.SiteIds.Contains(a.site_id.Value)).ToList();
		}

		// Build up a list of schedule exclusions from both the recurring and non recurring schedules.
		var exclusions = new List<ScheduleExclusion>();
		foreach (var schedule in schedules)
			exclusions.Add(new ScheduleExclusion
			{
				EndsAt = schedule.end_date,
				ProviderId = schedule.provider_id.Value,
				StartsAt = schedule.start_date
			});
		foreach (var scheduleSummary in recurringSchedules)
		{
			var id = int.Parse(scheduleSummary.id.Substring(2));
			var recurringSchedule = await _scheduleUnitOfWork.GetSchedule(id, s => s.Include(x => x.RecurringInterval));
			var applicableOccurrences = recurringSchedule.RecurringInterval.GetOccurrences(startDate, endDate);
			foreach (var instance in applicableOccurrences)
				exclusions.Add(new ScheduleExclusion
				{
					EndsAt = instance.EndsAt,
					ProviderId = recurringSchedule.ProviderId,
					StartsAt = instance.StartsAt
				});
		}

		// Adjust the openings.
		_ApplyScheduleExclusions(openings, exclusions);
	}

	private async Task _ExcludeTimesRelatedToScheduleBlocks(ICollection<ScheduleOpeningModel> openings,
		DateTime startDate, DateTime endDate, ScheduleOpeningsSearchModel model)
	{
		if (await _BlockSchedulingApplies(model))
			// This logic is applied when finding original list of openings.
			return;

		// Going to construct a list of exclusions.
		var exclusions = new List<ScheduleExclusion>();

		// Ignore times for providers with blocks associated with them.
		var scheduleBlockLookup = (await _providerBlockScheduleUnitOfWork.GetProviderBlockSchedules().Select(x => new
		{
			x.ProviderId,
			x.ScheduleTimeSlot.DayOfWeek,
			x.ScheduleTimeSlot.EndTime,
			x.ScheduleTimeSlot.StartTime
		}).ToListAsync()).ToLookup(d => d.DayOfWeek);

		var currentDate = startDate.Date;
		while (currentDate <= endDate)
		{
			// Find schedule block data related to the current date.
			var dayOfWeek = (int)DaysOfWeek.FromDayOfWeek(currentDate.DayOfWeek);

			// Create an exclusion for each block time.
			exclusions.AddRange(from sb in scheduleBlockLookup[dayOfWeek]
								select new ScheduleExclusion
								{
									EndsAt = currentDate.SetTime(sb.EndTime),
									ProviderId = sb.ProviderId,
									StartsAt = currentDate.SetTime(sb.StartTime)
								});

			currentDate = currentDate.AddDays(1);
		}

		// Apply the exclusions.
		_ApplyScheduleExclusions(openings, exclusions);
	}

	private async Task<IList<ScheduleOpeningModel>> _FetchMasterListOfOpenings(DateTime startDate, DateTime endDate,
		ScheduleOpeningsSearchModel model)
	{
		var blockSchedulingApplies = await _BlockSchedulingApplies(model);
		if (blockSchedulingApplies) return await _FetchScheduleBlocksAsOpenings(startDate, endDate, model);

		return await _FetchProviderHoursAsOpenings(startDate, endDate, model);
	}

	private async Task<IList<ScheduleOpeningModel>> _FetchProviderHoursAsOpenings(DateTime startDate, DateTime endDate,
		ScheduleOpeningsSearchModel model)
	{
		var providers = await _providerRepository.GetWithHours();
		if (model.ProviderIds.Any()) providers = providers.Where(p => model.ProviderIds.Contains(p.Id)).ToList();
		if (model.SiteIds.Any())
			foreach (var p in providers)
				p.SiteHours = p.SiteHours.Where(ush => model.SiteIds.Contains(ush.SiteId)).ToList();

		var providerHoursByDay = new Dictionary<int, List<Tuple<ProviderSummary, UserSiteHours>>>();
		foreach (var providerSummary in providers)
		foreach (var siteHour in providerSummary.SiteHours)
		{
			// 1 based day of week, Mon = 1, Sun = 7
			var day = siteHour.DayNum;
			if (!providerHoursByDay.ContainsKey(day))
				providerHoursByDay.Add(day, new List<Tuple<ProviderSummary, UserSiteHours>>());

			providerHoursByDay[day]
				.Add(new Tuple<ProviderSummary, UserSiteHours>(providerSummary, siteHour));
		}

		var openings = new List<ScheduleOpeningModel>();
		var currentDate = startDate;
		while (currentDate <= endDate)
		{
			// Get the provider/site data for the current day of the week.
			var dayOfWeek = ConvertMStoTIMSDayOfWeek(currentDate.DayOfWeek);
			if (providerHoursByDay.ContainsKey(dayOfWeek))
			{
				foreach (var hours in providerHoursByDay[dayOfWeek])
				{
					var provider = hours.Item1;
					var siteHours = hours.Item2;
					var opening = new ScheduleOpeningModel
					{
						EndsAt = currentDate.Date.SetTime(siteHours.EndTime),
						ProviderFirstName = provider.FirstName,
						ProviderId = provider.Id,
						ProviderLastName = provider.LastName,
						ProviderMiddleInitial = provider.Initial,
						SiteId = siteHours.SiteId,
						SiteName = siteHours.Site.Name,
						StartsAt = currentDate.Date.SetTime(siteHours.StartTime)
					};

					openings.Add(opening);
				}
			}

			// Next...
			currentDate = currentDate.AddDays(1);
		}

		return openings;
	}

	private async Task<IList<ScheduleOpeningModel>> _FetchScheduleBlocksAsOpenings(DateTime startDate,
		DateTime endDate,
		ScheduleOpeningsSearchModel model)
	{
		var blocks = await _scheduleBlockUnitOfWork.GetBlockOpenings(model.AppointmentTypeId.Value);

		if (model.ProviderIds.Any()) blocks = blocks.Where(p => model.ProviderIds.Contains(p.ProviderId)).ToList();
		if (model.SiteIds.Any()) blocks = blocks.Where(p => model.SiteIds.Contains(p.SiteId)).ToList();

		var blocksByDay = blocks.ToLookup(x => x.DayOfWeek);

		var openings = new List<ScheduleOpeningModel>();
		var currentDate = startDate.Date;
		while (currentDate <= endDate)
		{
			var dayOfWeek = ConvertMStoTIMSDayOfWeek(currentDate.DayOfWeek);
			foreach (var block in blocksByDay[dayOfWeek])
			{
				if (block.StartTime.TimeOfDay < block.SiteStartTime.GetValueOrDefault().TimeOfDay
					|| block.EndTime.TimeOfDay > block.SiteEndTime.GetValueOrDefault().TimeOfDay)
					continue;

				var opening = new ScheduleOpeningModel
				{
					EndsAt = currentDate.SetTime(block.EndTime),
					ProviderFirstName = block.FirstName,
					ProviderId = block.ProviderId,
					ProviderLastName = block.LastName,
					ProviderMiddleInitial = block.Initial,
					SiteId = block.SiteId,
					SiteName = block.SiteName,
					StartsAt = currentDate.SetTime(block.StartTime)
				};

				openings.Add(opening);
			}

			currentDate = currentDate.AddDays(1);
		}

		return openings;
	}

	private async Task _SetupAccessToResources(DateTime startDate, DateTime endDate, ScheduleOpeningsSearchModel model)
	{
		_useResources = model.SiteIds.Count() == 1 && model.ResourceIds.Any();
		_resources = null;
		if (_useResources)
			_resources = await _resourceUnitOfWork.GetResources(r => model.ResourceIds.Contains(r.Id));
	}

	private class ScheduleExclusion
	{
		public DateTime EndsAt { get; set; }
		public int ProviderId { get; set; }
		public int? ResourceId { get; set; }
		public DateTime StartsAt { get; set; }
	}
}