using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Server.Data;
using TIMS_X.Server.Filters;

namespace TIMS_X.Server.Queries;

public class SchedulerQuery
{
	private readonly SchedulerDbContext _dbContext;

	public SchedulerQuery(SchedulerDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	private Func<DateTime, DateTime> _BuildIntervalDateIterator(RecurringIntervalBase interval)
	{
		switch ((IntervalTypeEnum)interval.IntervalType)
		{
			/********************************************
			 * Daily
			 */
			case IntervalTypeEnum.Daily:
			{
				return currentDate =>
				{
					switch ((DailyIntervalSubTypeEnum)interval.SubType)
					{
						case DailyIntervalSubTypeEnum.NthDay:
						{
							// Add N days to the current date.
							return currentDate.AddDays(interval.DayInterval);
						}
						case DailyIntervalSubTypeEnum.Weekday:
						{
							// Advance by the number of days necessary to get to the next weekday.
							var dayOfWeek = currentDate.DayOfWeek;
							if (dayOfWeek >= DayOfWeek.Friday)
								return currentDate.AddDays(2 + DayOfWeek.Saturday - dayOfWeek);
							return currentDate.AddDays(1);
						}
						case DailyIntervalSubTypeEnum.TuesThurs:
						{
							// Advance by the number of days necessary to get to the next TuTh.
							var dayOfWeek = currentDate.DayOfWeek;
							if (dayOfWeek < DayOfWeek.Tuesday)
								return currentDate.AddDays(DayOfWeek.Tuesday - dayOfWeek);
							if (dayOfWeek < DayOfWeek.Thursday)
								return currentDate.AddDays(DayOfWeek.Thursday - dayOfWeek);
							return
								currentDate.AddDays(1 + DayOfWeek.Saturday - dayOfWeek +
								                    (int)DayOfWeek.Tuesday);
						}
						case DailyIntervalSubTypeEnum.MonWedFri:
						{
							// Advance by the number of days necessary to get to the next MWF.
							var dayOfWeek = currentDate.DayOfWeek;
							if (dayOfWeek < DayOfWeek.Monday) return currentDate.AddDays(DayOfWeek.Monday - dayOfWeek);
							if (dayOfWeek < DayOfWeek.Wednesday)
								return currentDate.AddDays(DayOfWeek.Wednesday - dayOfWeek);
							if (dayOfWeek < DayOfWeek.Friday) return currentDate.AddDays(DayOfWeek.Friday - dayOfWeek);
							return
								currentDate.AddDays(1 + DayOfWeek.Saturday - dayOfWeek +
								                    (int)DayOfWeek.Monday);
						}
						default:
						{
							return DateTime.MaxValue;
						}
					}
				};
			}

			/********************************************
			 * Weekly
			 */
			case IntervalTypeEnum.Weekly:
			{
				// Prepare a lookup to simplify finding the next day of week for occurrences.
				var selectedDaysOfWeek = new[]
				{
					interval.IsSundaySet,
					interval.IsMondaySet,
					interval.IsTuesdaySet,
					interval.IsWednesdaySet,
					interval.IsThursdaySet,
					interval.IsFridaySet,
					interval.IsSaturdaySet
				};

				return currentDate =>
				{
					// Determine the value of the next day of the week. 
					var currentDayOfWeek = (int)currentDate.DayOfWeek;
					var nextDayOfWeek = 0;
					for (var i = 1; i <= 7; i++)
					{
						var day = (currentDayOfWeek + i) % 7;
						if (selectedDaysOfWeek[day])
						{
							nextDayOfWeek = day;
							break;
						}
					}

					// Calculate the number of days to advance.
					int daysToAdd;
					if (currentDayOfWeek == nextDayOfWeek)
						daysToAdd = 7 * interval.WeekInterval;
					else if (currentDayOfWeek > nextDayOfWeek)
						// Advance to the next day in the next week, then apply the week interval.
						// Have to offset the interval by 1 since we will already be in the next week.
						daysToAdd = 7 - currentDayOfWeek + nextDayOfWeek
						                                 + 7 * (interval.WeekInterval - 1);
					else // currentDayOfWeek < nextDayOfWeek
						daysToAdd = nextDayOfWeek - currentDayOfWeek;

					return currentDate.AddDays(daysToAdd);
				};
			}

			/********************************************
			 * Monthly
			 */
			case IntervalTypeEnum.Monthly:
			{
				return currentDate =>
				{
					switch ((MonthlyIntervalSubTypeEnum)interval.SubType)
					{
						case MonthlyIntervalSubTypeEnum.DayOfMonth:
						{
							// Move forward N months.
							var nextDate = currentDate.AddMonths(interval.MonthInterval);
							var i = 1;

							// If the day of the next date is not correct (will happen with 31/30/29 days),
							// then keep moving forward along the set interval until the day is correct.
							while (currentDate.Day != nextDate.Day)
								nextDate = currentDate.AddMonths(interval.MonthInterval * i++);

							return nextDate;
						}
						case MonthlyIntervalSubTypeEnum.DayOfWeek:
						{
							// Calculate the day of week the next month starts on.
							var nextMonth = currentDate.AddMonths(interval.MonthInterval);
							var firstDayOfNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
							var firstDayOfNextMonthDayOfWeek = (int)firstDayOfNextMonth.DayOfWeek;
							var intervalDayOfWeek = interval.DayOfWeek % 7;

							// Get the first relevant day of week in the next month.
							// Branch on whether the first interval day for the month is in the second week.
							var firstIntervalDayInNextMonth
								= firstDayOfNextMonthDayOfWeek > intervalDayOfWeek
									? firstDayOfNextMonth.AddDays(7 - firstDayOfNextMonthDayOfWeek + intervalDayOfWeek)
									: firstDayOfNextMonth.AddDays(intervalDayOfWeek - firstDayOfNextMonthDayOfWeek);

							// Advance the date to the Nth day of week.
							// If we overshoot, then rewind by a week.
							var nextDate = firstIntervalDayInNextMonth.AddDays(7 * (interval.DayQualifier - 1));
							if (nextDate.Month != nextMonth.Month) nextDate = nextDate.AddDays(-7);

							return nextDate;
						}
						default:
						{
							return DateTime.MaxValue;
						}
					}
				};
			}

			/********************************************
			 * Yearly
			 */
			case IntervalTypeEnum.Yearly:
			{
				return currentDate =>
				{
					// Advance a year, and test that the month and day exist in the year.
					// There could be a booboo if the interval is to repeat on a leap year.
					var nextDate = currentDate.AddYears(1);
					if (interval.Month == 2 && interval.DayOfMonth == 29)
						while (nextDate.Month != interval.Month || nextDate.Day != interval.DayOfMonth)
							nextDate = currentDate.AddYears(1);
					return nextDate;
				};
			}

			/********************************************
			 * Just in case something goes wrong....
			 */
			default:
			{
				return date => DateTime.MaxValue;
			}
		}
	}


	private List<RecurringIntervalOccurrence> _GenerateRecurringInstances(RecurringIntervalBase interval,
		DateTime fromDate,
		DateTime toDate)
	{
		var occurrences = new List<RecurringIntervalOccurrence>();
		if (interval != null)
		{
			// We're going to stop yielding occurrences by the given end date if this interval has 
			// an unset or never-ending end type. If the interval ends by a specific date
			// (either cause it's an "end after" or "by end date" type), then use the earliest end date.
			DateTime maxDate;
			var maxOccurrenceNumber = int.MaxValue;
			switch ((RecurrenceEndType)interval.EndType)
			{
				case RecurrenceEndType.EndAfterOccurrences:
					maxDate = toDate;
					maxOccurrenceNumber = interval.EndOccurs;
					break;

				case RecurrenceEndType.EndByDate:
					maxDate = toDate < interval.EndDate ? toDate : interval.EndDate;
					break;

				default: /* NotSet and NoEndDate */
					maxDate = toDate;
					break;
			}

			// Iterate from the start of the interval up to the max end date or occurrence number.

			var currentDate = interval.StartDate;
			var getNextDate = _BuildIntervalDateIterator(interval);
			var iOccurrenceNumber = 1;
			while (currentDate <= maxDate && iOccurrenceNumber <= maxOccurrenceNumber)
			{
				if (currentDate >= fromDate)
					occurrences.Add(new RecurringIntervalOccurrence
					{
						StartsAt = currentDate.Date + interval.StartDate.TimeOfDay,
						EndsAt = currentDate.Date + interval.EndDate.TimeOfDay,
						OccurrenceNumber = iOccurrenceNumber
					});

				iOccurrenceNumber = iOccurrenceNumber + 1;
				currentDate = getNextDate(currentDate);
			}

			interval.ProcessDeletedOccurrences(occurrences);

			// Return all occurrences after the given start date.
		}

		return occurrences.ToList();
	}

	public async Task AddNotificationStatusAsync(int appointmentId, NotificationStatus notificationStatus)
	{
		var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(appt => appt.Id == appointmentId);
		if (appointment != null)
		{
			appointment.NotificationStatus |= notificationStatus;
			await _dbContext.SaveChangesAsync();
		}
	}

	public void DeleteAppointment(Appointment appointment)
	{
		_dbContext.Remove(appointment);
		_dbContext.SaveChanges();
	}

	public async Task DeleteCallLogAsync(string callId)
	{
		var callLog = await _dbContext.VoiceCallLogs
			.FirstOrDefaultAsync(x => x.Identifier == callId);
		if (callLog != null)
		{
			_dbContext.VoiceCallLogs.Remove(callLog);
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task<IEnumerable<ScheduleItem>> GenerateRecurringAppointmentsAsync(SchedulerQueryFilter queryFilter)
	{
		var scheduleItems = new List<ScheduleItem>();
		try
		{
			var scheds = _dbContext.Schedules
				.Include(x => x.RecurringInterval)
				.ThenInclude(x => x.DeletedOccurrences)
				.Where(x => x.RecurringIntervalId.HasValue)
				.AsQueryable();


			if (queryFilter.ProviderIds != null && queryFilter.ProviderIds.Any())
				scheds = scheds.Where(appt => queryFilter.ProviderIds.Contains(appt.ProviderId));
			if (queryFilter.SiteIds != null && queryFilter.SiteIds.Any())
				scheds = scheds.Where(appt => queryFilter.SiteIds.Contains(appt.SiteId));

			var schedules = await scheds.ToListAsync();

			foreach (var sched in schedules)
			{
				var occurrences =
					_GenerateRecurringInstances(sched.RecurringInterval, queryFilter.From, queryFilter.To);
				scheduleItems.AddRange(occurrences.Select(x => new ScheduleItem(sched, x)));
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return scheduleItems;
	}

	public async Task<Appointment> GetAppointmentAsync(int appointmentId, bool includeReferences)
	{
		Appointment result = null;
		if (includeReferences)
			result = await _dbContext.Appointments
				.Include(appt => appt.AppointmentType)
				.Include(appt => appt.AppointmentStatus)
				.Include(appt => appt.Patient)
				.Include(appt => appt.Provider)
				.Include(appt => appt.Site)
				.FirstOrDefaultAsync(appt => appt.Id == appointmentId);
		else
			result = await _dbContext.Appointments
				.FirstOrDefaultAsync(appt => appt.Id == appointmentId);

		return result;
	}

	public async Task<List<Appointment>> GetAppointmentsCustomQueryAsync(
		Expression<Func<Appointment, bool>> predicate)
	{
		var result = await _dbContext.Appointments
			.Include(appt => appt.AppointmentType)
			.Include(appt => appt.AppointmentStatus)
			.Include(appt => appt.Patient)
			.Include(appt => appt.Provider)
			.Include(appt => appt.Site)
			.Where(predicate)
			.ToListAsync();

		return result;
	}

	public async Task<VoiceCallLog> GetCallLogAsync(string callId)
	{
		var callLog = await _dbContext.VoiceCallLogs
			.Include(log => log.MessageTemplate)
			.FirstOrDefaultAsync(x => x.Identifier == callId);
		if (callLog?.MessageTemplate != null)
			callLog.MessageTemplate.TemplateType = Enum.Parse<MessageTemplateType>(callLog.MessageTemplate.MessageType);

		return callLog;
	}

	public async Task<VoiceCallLog> GetCallLogAsync(int templateId, int appointmentId)
	{
		var callLog = await _dbContext.VoiceCallLogs
			.Include(log => log.MessageTemplate)
			.OrderBy(log => log.CreatedDate)
			.LastOrDefaultAsync(x => x.MessageTemplateId == templateId && x.AppointmentId == appointmentId);
		return callLog;
	}

	public async Task<IEnumerable<int>> GetDaysScheduledAsync(SchedulerQueryFilter queryFilter)
	{
		var apptsQuery = _dbContext.Appointments
			.AsQueryable();
		apptsQuery = queryFilter.ApplyTo(apptsQuery);
		List<int> results = null;
		try
		{
			results = await apptsQuery.Select(appt => appt.StartsAt.Day).Distinct().ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return results;
	}

	public async Task<EmailLog> GetEmailLogAsync(int templateId, int appointmentId)
	{
		var emailLog = await _dbContext.EmailLogs
			.Include(log => log.MessageTemplate)
			.OrderBy(log => log.CreatedDate)
			.LastOrDefaultAsync(x => x.MessageTemplateId == templateId && x.AppointmentId == appointmentId);
		return emailLog;
	}

	public async Task<EmailLog> GetEmailLogAsync(int logId)
	{
		var emailLog = await _dbContext.EmailLogs
			.Include(log => log.MessageTemplate)
			.FirstOrDefaultAsync(x => x.Id == logId);
		if (emailLog?.MessageTemplate != null)
			emailLog.MessageTemplate.TemplateType =
				Enum.Parse<MessageTemplateType>(emailLog.MessageTemplate.MessageType);
		return emailLog;
	}

	public async Task<NotificationResult> GetNotificationResultAsync(MessageTemplateType templateType,
		MessageDeliveryMethod deliveryMethod, int appointmentId)
	{
		var result = new NotificationResult { AppointmentId = appointmentId, DeliveryMethod = deliveryMethod };
		var templateTypeString = templateType.ToString();
		switch (deliveryMethod)
		{
			case MessageDeliveryMethod.Email:
				var emailLog = await _dbContext.EmailLogs
					.Include(x => x.MessageTemplate)
					.OrderBy(x => x.CreatedDate)
					.LastOrDefaultAsync(x =>
						x.AppointmentId == appointmentId && x.MessageTemplate.MessageType == templateTypeString);
				if (emailLog != null)
				{
					result.SentDate = emailLog.CreatedDate;
					result.ErrorMessage = emailLog.ExceptionMessage;
				}

				break;
			case MessageDeliveryMethod.Sms:
				var smsLog = await _dbContext.SmsLogs
					.Include(x => x.MessageTemplate)
					.OrderBy(x => x.CreatedDate)
					.LastOrDefaultAsync(x =>
						x.AppointmentId == appointmentId && x.MessageTemplate.MessageType == templateTypeString);
				if (smsLog != null)
				{
					result.SentDate = smsLog.CreatedDate;
					result.ErrorMessage = smsLog.ExceptionMessage;
				}

				break;
			case MessageDeliveryMethod.Voice:
				var voiceCallLog = await _dbContext.VoiceCallLogs
					.Include(x => x.MessageTemplate)
					.OrderBy(x => x.CreatedDate)
					.LastOrDefaultAsync(x =>
						x.AppointmentId == appointmentId && x.MessageTemplate.MessageType == templateTypeString);
				if (voiceCallLog != null)
				{
					result.SentDate = voiceCallLog.CreatedDate;
					result.ErrorMessage = voiceCallLog.ExceptionMessage;
				}

				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(deliveryMethod), deliveryMethod, null);
		}

		return result;
	}

	public async Task<List<NotificationResult>> GetNotificationResultsAsync(MessageTemplateType templateType,
		MessageDeliveryMethod[] deliveryMethods, int[] appointmentIds)
	{
		var results = new List<NotificationResult>();
		for (var i = 0; i < appointmentIds.Count(); ++i)
			results.Add(await GetNotificationResultAsync(templateType, deliveryMethods[i], appointmentIds[i]));

		return results;
	}

	public async Task<NotificationStatus> GetNotificationStatusAsync(int appointmentId)
	{
		var notificationStatus = await _dbContext.Appointments
			.Where(appt => appt.Id == appointmentId)
			.Select(appt => appt.NotificationStatus)
			.FirstOrDefaultAsync();
		return notificationStatus;
	}

	public async Task<IEnumerable<PatientItem>> GetPatientsScheduledAsync(DateTime fromDate, DateTime toDate,
		int[] siteIds)
	{
		var patIdQuery = _dbContext.Appointments
			.Where(appt => appt.StartsAt >= fromDate && appt.StartsAt <= toDate)
			.AsQueryable();

		if (siteIds != null && siteIds.Any()) patIdQuery = patIdQuery.Where(appt => siteIds.Contains(appt.SiteId));


		List<PatientItem> result = null;
		try
		{
			var patIds = await patIdQuery.Select(appt => appt.PatientId).Distinct().ToListAsync();
			result = await _dbContext.Patients
				.Where(x => patIds.Contains(x.Id))
				.Select(x => new PatientItem
				{
					Id = x.Id,
					SiteId = x.SiteId,
					Inactive = x.Inactive,
					FirstName = x.FirstName,
					LastName = x.LastName,
					Initial = x.Initial,
					Gender = x.Sex == "M" ? Gender.Male : x.Sex == "F" ? Gender.Female : Gender.Unknown,
					BirthDate = x.BirthDate
				})
				.ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<Dictionary<int, List<ScheduledPatientItem>>> GetPatientsScheduledTodayAsync(
		IEnumerable<int> providerIds)
	{
		var todaysAppointmentsQuery = _dbContext.Appointments
			.Include(appt => appt.Patient)
			//.Include(appt => appt.Provider)
			.Include(appt => appt.AppointmentType)
			.Include(appt => appt.AppointmentStatus)
			.Where(appt => appt.StartsAt >= DateTime.Today &&
			               appt.StartsAt <= DateTime.Today.AddDays(1) &&
			               providerIds.Contains(appt.ProviderId))
			.OrderBy(appt => appt.StartsAt);

		var result = new Dictionary<int, List<ScheduledPatientItem>>();
		try
		{
			var providerList = await _dbContext.Providers.ToListAsync();
			var apptList = await todaysAppointmentsQuery.ToListAsync();
			var todaysAppointments = apptList.GroupBy(x => x.ProviderId).ToDictionary(k => k.Key, v => v);
			foreach (var providerAppts in todaysAppointments)
			{
				result[providerAppts.Key] = new List<ScheduledPatientItem>();
				foreach (var appointment in providerAppts.Value)
				{
					var time = appointment.StartsAt.ToString("hh:mm tt", CultureInfo.InvariantCulture);

					var providerModel = providerList.FirstOrDefault(x => x.Id == appointment.ProviderId);
					var providerName = string.Empty;
					if (providerModel != null) providerName = providerModel.FullName;

					//var provider = appointment.Provider?.FullName ?? string.Empty;
					var apptStatus = appointment.AppointmentStatus?.Name ?? string.Empty;
					var apptType = appointment.AppointmentType?.Name ?? string.Empty;
					var durationMinutes = (int)(appointment.EndsAt - appointment.StartsAt).TotalMinutes;
					var duration = $"{durationMinutes} mins";

					result[providerAppts.Key].Add(new ScheduledPatientItem
					{
						Id = appointment.Patient.Id,
						SiteId = appointment.Patient.SiteId,
						Inactive = appointment.Patient.Inactive,
						FirstName = appointment.Patient.FirstName,
						LastName = appointment.Patient.LastName,
						Initial = appointment.Patient.Initial,
						Gender = appointment.Patient.Sex == "M" ? Gender.Male :
							appointment.Patient.Sex == "F" ? Gender.Female : Gender.Unknown,
						BirthDate = appointment.Patient.BirthDate,
						AppointmentTime = time,
						AppointmentDuration = duration,
						AppointmentType = apptType,
						AppointmentStatus = apptStatus,
						ProviderName = providerName
					});
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}

		return result;
	}


	public async Task<IEnumerable<Resource>> GetResourcesAsync(bool includeInactive)
	{
		var query = _dbContext.Resources.Where(res => includeInactive || !res.Inactive);
		List<Resource> results = null;
		try
		{
			results = await query.ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return results;
	}

	public async Task<int> GetSiteIdForAppointmentAsync(int appointmentId)
	{
		var siteId = await _dbContext.Appointments
			.Where(x => x.Id == appointmentId)
			.Select(x => x.SiteId)
			.FirstOrDefaultAsync();
		return siteId;
	}

	public async Task<SmsLog> GetSmsLogAsync(int templateId, int appointmentId)
	{
		var smsLog = await _dbContext.SmsLogs
			.Include(log => log.MessageTemplate)
			.OrderBy(log => log.CreatedDate)
			.LastOrDefaultAsync(x => x.MessageTemplateId == templateId && x.AppointmentId == appointmentId);
		return smsLog;
	}

	public async Task<SmsLog> GetSmsLogAsync(string phoneNumber)
	{
		try
		{
			var smsLog = await _dbContext.SmsLogs
				.Include(log => log.MessageTemplate)
				.OrderByDescending(log => log.CreatedDate)
				.FirstOrDefaultAsync(x => x.To == phoneNumber);
			if (smsLog?.MessageTemplate != null)
				smsLog.MessageTemplate.TemplateType =
					Enum.Parse<MessageTemplateType>(smsLog.MessageTemplate.MessageType);
			return smsLog;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task<SmsLog> GetSmsLogByIdentifierAsync(string identifier)
	{
		var smsLog = await _dbContext.SmsLogs
			.Include(log => log.MessageTemplate)
			.OrderBy(log => log.CreatedDate)
			.LastOrDefaultAsync(x => x.Identifier == identifier);
		if (smsLog?.MessageTemplate != null)
			smsLog.MessageTemplate.TemplateType = Enum.Parse<MessageTemplateType>(smsLog.MessageTemplate.MessageType);
		return smsLog;
	}

	public async Task<AppointmentStatus> LookupAppointmentStatusAsync(string name)
	{
		var appointmentStatus = await _dbContext.AppointmentStatuses
			.Where(x => x.Name == name)
			.FirstOrDefaultAsync();

		return appointmentStatus;
	}

	public async Task<bool> PreviewLogExistsAsync(int templateId)
	{
		return await _dbContext.VoiceCallLogs.AnyAsync(x => x.MessageTemplateId == templateId && x.IsPreview);
	}

	public async Task PutAppointmentAsync(Appointment appointment)
	{
		try
		{
			if (appointment.Id == 0)
				await _dbContext.Appointments.AddAsync(appointment);
			else
				_dbContext.Appointments.Attach(appointment).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutAppointmentStatusAsync(AppointmentStatus appointmentStatus)
	{
		try
		{
			if (appointmentStatus.Id == 0)
				await _dbContext.AppointmentStatuses.AddAsync(appointmentStatus);
			else
				_dbContext.AppointmentStatuses.Attach(appointmentStatus).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutCallLogAsync(VoiceCallLog callLog)
	{
		if (callLog.Id == 0)
			await _dbContext.VoiceCallLogs.AddAsync(callLog);
		else
			_dbContext.VoiceCallLogs.Attach(callLog).State = EntityState.Modified;

		await _dbContext.SaveChangesAsync();
	}

	public async Task PutCallTrackingAsync(VoiceCallTracking tracking)
	{
		await _dbContext.VoiceCallTrackings.AddAsync(tracking);
		await _dbContext.SaveChangesAsync();
	}

	public async Task PutEmailLogAsync(EmailLog emailLog)
	{
		// BodyText does not allow nulls.
		if (emailLog.BodyText == null) emailLog.BodyText = string.Empty;

		if (emailLog.Id == 0)
			await _dbContext.EmailLogs.AddAsync(emailLog);
		else
			//await _dbContext.EmailLogs.AddAsync(emailLog);
			_dbContext.EmailLogs.Attach(emailLog).State = EntityState.Modified;

		await _dbContext.SaveChangesAsync();
	}

	public async Task PutEmailTrackingAsync(EmailTracking tracking)
	{
		await _dbContext.EmailTrackings.AddAsync(tracking);
		try
		{
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutSmsLogAsync(SmsLog smsLog)
	{
		if (smsLog.Id == 0)
			await _dbContext.SmsLogs.AddAsync(smsLog);
		else
			_dbContext.SmsLogs.Attach(smsLog).State = EntityState.Modified;

		try
		{
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task PutSmsTrackingAsync(SmsTracking tracking)
	{
		await _dbContext.SmsTrackings.AddAsync(tracking);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<ScheduleItem>> SearchAppointmentsAsync(SchedulerQueryFilter queryFilter)
	{
		var patientApptsQuery = _dbContext.Appointments
			.Include(appt => appt.AppointmentType)
			.Include(appt => appt.AppointmentStatus)
			.Include(appt => appt.Patient)
			.AsQueryable();

		patientApptsQuery = queryFilter.ApplyTo(patientApptsQuery);
		var nonPatientApptsQuery = queryFilter.ApplyTo(_dbContext.Schedules.AsQueryable());
		nonPatientApptsQuery = nonPatientApptsQuery.Where(x => x.RecurringIntervalId == 0);
		var scheduleItems = new List<ScheduleItem>();

		try
		{
			if (queryFilter.IncludePatientAppointments)
			{
				var appts = await patientApptsQuery.ToListAsync();
				scheduleItems.AddRange(appts.Select(appt => new ScheduleItem(appt)));
			}

			if (queryFilter.IncludeNonPatientAppointments)
			{
				var appts = await nonPatientApptsQuery.ToListAsync();
				scheduleItems.AddRange(appts.Select(appt => new ScheduleItem(appt)));

				if (queryFilter.IncludeRecurringAppointments)
				{
					var recurringAppts = await GenerateRecurringAppointmentsAsync(queryFilter);
					scheduleItems.AddRange(recurringAppts);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return scheduleItems;
	}

	public async Task<bool> ValidateAppointmentTypeAsync(int appointmentTypeId)
	{
		return await _dbContext.AppointmentTypes.AnyAsync(x => x.Id == appointmentTypeId && !x.Inactive);
	}
}