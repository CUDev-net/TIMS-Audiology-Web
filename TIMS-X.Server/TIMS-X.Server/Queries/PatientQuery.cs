using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Server.Data;
using TIMS_X.Server.Extensions;
using TIMS_X.Server.Filters;
using TIMS_X.Server.Models;

namespace TIMS_X.Server.Queries;

public class PatientQuery
{
	private readonly PatientDbContext _dbContext;

	public PatientQuery(PatientDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <summary>
	///     Given a batch size n, this routine will return a list of Patient ID Pairs.
	///     Each ID Pair has n records between them, including themselves.
	/// </summary>
	/// <param name="batchSize"></param>
	/// <returns></returns>
	public async Task<IEnumerable<Tuple<int, int>>> CalculateBatchesAsync(int batchSize)
	{
		var result = new List<Tuple<int, int>>();
		try
		{
			var patientCount = await _dbContext.Patients.CountAsync();

			var from = 0;
			var to = Math.Min(batchSize, patientCount);

			while (true)
			{
				// Use Skip function to retrieve correct record
				var fromPatient = await _dbContext.Patients.Skip(from).FirstOrDefaultAsync();
				var toPatient = await _dbContext.Patients.Skip(to - 1).FirstOrDefaultAsync();

				// Store Ids from this index
				result.Add(new Tuple<int, int>(fromPatient?.Id ?? 0, toPatient?.Id ?? int.MaxValue));


				if (to == patientCount)
					break;

				from = to;
				to = Math.Min(from + batchSize, patientCount);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw;
		}

		return result;
	}

	public async Task<DetailedPatientItem> CreatePatientAsync(PatientQueryFilter queryFilter, int userId)
	{
		Patient patient;
		try
		{
			var query = _dbContext.Patients
				.Include(pat => pat.Restrictions).ThenInclude(x => x.CommunicationRestriction)
				.AsQueryable();
			query = queryFilter.ApplyTo(query);
			patient = await query.FirstOrDefaultAsync();

			if (patient == null)
			{
				patient = new Patient
				{
					FirstName = queryFilter.FirstName,
					LastName = queryFilter.LastName,
					Email = queryFilter.Email,
					HomePhone = queryFilter.HomePhone,
					MobilePhone = queryFilter.MobilePhone,
					BirthDate = queryFilter.BirthDate,
					UpdatedUserId = userId,
					CreatedUserId = userId,
					InsuredInsurancePayerId = 0,
					//OtherInsurancePayerId = 0,
					MarketingId = 0,
					PatientStatusId = 0,
					PatientTypeId = 0,
					PrimaryCareId = 0,
					ProviderId = 0,
					ReferringPhysicianId = 0,
					SalutationId = 0,
					SiteId = 0,
					UpdatedSiteId = 0
				};

				_dbContext.Patients.Add(patient);
				await _dbContext.SaveChangesAsync();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw;
		}

		return patient == null ? null : new DetailedPatientItem(patient);
	}

	public async Task<bool> ExistsAsync(int id)
	{
		return await _dbContext.Patients.AnyAsync(p => p.Id == id);
	}

	public async Task<DetailedPatientItem> FindPatientAsync(PatientQueryFilter queryFilter, int userId)
	{
		Patient patient;
		try
		{
			var query = _dbContext.Patients
				.Include(pat => pat.Restrictions).ThenInclude(x => x.CommunicationRestriction)
				.AsQueryable();
			query = queryFilter.ApplyTo(query);
			patient = await query.FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw;
		}

		return patient == null ? null : new DetailedPatientItem(patient);
	}

	public async Task<List<Conversation>> GetAllConversationsAsync()
	{
		var logEntries = await _dbContext.SmsLogs
			.Where(x => x.AppointmentId == 0)
			.OrderBy(x => x.CreatedDate)
			.ToListAsync();

		var conversations = new List<Conversation>();
		foreach (var logEntry in logEntries)
		{
			var digits = logEntry.To.Digits();
			var replies = await _dbContext.SmsReplies.Where(x => x.SmsLogId == logEntry.Id).OrderBy(x => x.CreatedDate)
				.ToListAsync();
			var repliesCount = replies.Count();

			DateTime? dateLastReceived = null;
			if (repliesCount > 0)
			{
				var lastReply = replies.Last();
				dateLastReceived = lastReply.CreatedDate;
			}


			var existingConvo = conversations.FirstOrDefault(x =>
				x.UserId == logEntry.CreatedUserId.Value && x.PatientId == logEntry.PatientId &&
				x.PhoneNumber.Digits() == digits);

			if (existingConvo == null)
				conversations.Add(new Conversation
				{
					DateCreated = logEntry.CreatedDate,
					DateLastReceived = dateLastReceived,
					PatientId = logEntry.PatientId,
					PhoneNumber = logEntry.To,
					MessageCount = logEntry.Body == null ? repliesCount : repliesCount + 1,
					UserId = logEntry.CreatedUserId.Value
				});
			else
				existingConvo.MessageCount += logEntry.Body == null ? repliesCount : repliesCount + 1;
		}

		return conversations;
	}

	public async Task<int> GetAllConversationsCountAsync()
	{
		var count = await _dbContext.SmsLogs
			.Where(x => x.AppointmentId == 0)
			.CountAsync();
		return count;
	}

	public async Task<List<Conversation>> GetAllConversationsPagedAsync(int page, int pageSize)
	{
		var query = _dbContext.SmsLogs
			.Where(x => x.AppointmentId == 0)
			.OrderByDescending(x => x.CreatedDate).AsQueryable();

		if (page > 1) query = query.Skip((page - 1) * pageSize);
		query = query.Take(pageSize);

		var logEntries = query.ToList();

		var conversations = new List<Conversation>();
		foreach (var logEntry in logEntries)
		{
			var digits = logEntry.To.Digits();
			var replies = await _dbContext.SmsReplies.Where(x => x.SmsLogId == logEntry.Id).OrderBy(x => x.CreatedDate)
				.ToListAsync();
			var repliesCount = replies.Count();

			DateTime? dateLastReceived = null;
			if (repliesCount > 0)
			{
				var lastReply = replies.Last();
				dateLastReceived = lastReply.CreatedDate;
			}


			var existingConvo = conversations.FirstOrDefault(x =>
				x.UserId == logEntry.CreatedUserId.Value && x.PatientId == logEntry.PatientId &&
				x.PhoneNumber.Digits() == digits);

			if (existingConvo == null)
				conversations.Add(new Conversation
				{
					DateCreated = logEntry.CreatedDate,
					DateLastReceived = dateLastReceived,
					PatientId = logEntry.PatientId,
					PhoneNumber = logEntry.To,
					MessageCount = logEntry.Body == null ? repliesCount : repliesCount + 1,
					UserId = logEntry.CreatedUserId.Value
				});
			else
				existingConvo.MessageCount += logEntry.Body == null ? repliesCount : repliesCount + 1;
		}

		return conversations.OrderByDescending(x => x.ComparableDate).ToList();
	}

	public async Task<List<Message>> GetConversationHistoryAsync(int userId, string phoneNumber, DateTime? cutoffDate)
	{
		var digits = phoneNumber.Digits();

		var logEntriesQuery = _dbContext.SmsLogs
			.Where(x => x.CreatedUserId == userId && x.To.EndsWith(digits) && x.AppointmentId == 0)
			.AsQueryable();

		if (cutoffDate.HasValue) logEntriesQuery = logEntriesQuery.Where(x => x.CreatedDate >= cutoffDate.Value);

		var logEntries = await logEntriesQuery
			.OrderBy(x => x.CreatedDate)
			.ToListAsync();

		var messages = new List<Message>();
		foreach (var logEntry in logEntries)
		{
			if (!string.IsNullOrEmpty(logEntry.Body))
				messages.Add(new Message
				{
					DateSent = logEntry.CreatedDate,
					FromPatient = false,
					Text = logEntry.Body
				});
			var replies = await _dbContext.SmsReplies.Where(x => x.SmsLogId == logEntry.Id).OrderBy(x => x.CreatedDate)
				.ToListAsync();
			messages.AddRange(replies.Select(x => new Message
				{ DateSent = x.CreatedDate, FromPatient = true, Text = x.Body }));
		}

		return messages;
	}

	public async Task<List<Message>> GetConversationHistoryAsync(int userId, int patientId, DateTime? cutoffDate)
	{
		var logEntriesQuery = _dbContext.SmsLogs
			.Where(x => x.PatientId == patientId && x.AppointmentId == 0)
			.AsQueryable();


		if (userId > 0) logEntriesQuery = logEntriesQuery.Where(x => x.CreatedUserId == userId);

		if (cutoffDate.HasValue) logEntriesQuery = logEntriesQuery.Where(x => x.CreatedDate >= cutoffDate.Value);

		var logEntries = await logEntriesQuery
			.OrderBy(x => x.CreatedDate)
			.ToListAsync();


		var messages = new List<Message>();
		foreach (var logEntry in logEntries)
		{
			if (logEntry.Body != null)
				messages.Add(new Message
				{
					DateSent = logEntry.CreatedDate,
					FromPatient = false,
					UserId = logEntry.CreatedUserId ?? 0,
					Text = logEntry.Body
				});

			var replies = await _dbContext.SmsReplies.Where(x => x.SmsLogId == logEntry.Id).OrderBy(x => x.CreatedDate)
				.ToListAsync();
			messages.AddRange(replies.Select(x => new Message
				{ DateSent = x.CreatedDate, FromPatient = true, Text = x.Body }));
		}

		return messages;
	}

	public async Task<List<Tuple<int, int>>> GetConversationHistoryMessageCountAllAsync(int patientId)
	{
		var logEntries = await _dbContext.SmsLogs
			.Where(x => x.PatientId == patientId && x.AppointmentId == 0)
			.Select(x => new { UserId = x.CreatedUserId.Value, LogId = x.Id })
			.ToListAsync();

		//var logEntries = _dbContext.SmsLogs.GroupBy(x => new { x.CreatedUserId, x.PatientId });

		var result = new List<Tuple<int, int>>();
		foreach (var entry in logEntries)
		{
			var count = 1 + await _dbContext.SmsReplies.Where(x => x.SmsLogId == entry.LogId).CountAsync();
			var existing = result.FirstOrDefault(x => x.Item1 == entry.UserId);
			if (existing != null)
			{
				count += existing.Item2;
				result.Remove(existing);
			}

			result.Add(new Tuple<int, int>(entry.UserId, count));
		}

		return result;
	}

	public async Task<int> GetConversationHistoryMessageCountAsync(int userId, int patientId)
	{
		var logEntries = await _dbContext.SmsLogs
			.Where(x => x.CreatedUserId == userId && x.PatientId == patientId && x.AppointmentId == 0)
			.Select(x => x.Id)
			.ToListAsync();

		var count = logEntries.Count;
		foreach (var logEntryId in logEntries)
			count += await _dbContext.SmsReplies.Where(x => x.SmsLogId == logEntryId).CountAsync();
		return count;
	}

	public async Task<Patient> GetDomainPatientAsync(int patientId)
	{
		Patient patient;
		try
		{
			patient = await _dbContext.Patients
				.Include(pat => pat.Restrictions).ThenInclude(x => x.CommunicationRestriction)
				.Include(pat => pat.MedicalConditions).ThenInclude(x => x.Condition)
				.FirstOrDefaultAsync(pat => pat.Id == patientId);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw;
		}

		return patient;
	}

	public async Task<Patient> GetFullPatientAsync(int patientId)
	{
		Patient patient;
		try
		{
			patient = await _dbContext.Patients
				.Include(p => p.Restrictions)
				.ThenInclude(r => r.CommunicationRestriction)
				.Include(p => p.ReferringPhysician)
				.FirstOrDefaultAsync(pat => pat.Id == patientId);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw;
		}

		return patient;
	}

	public async Task<Patient> GetFullPatientAsync(Guid patienGuid)
	{
		Patient patient;
		try
		{
			patient = await _dbContext.Patients.FirstOrDefaultAsync(pat => pat.Guid == patienGuid);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw;
		}

		return patient;
	}

	public async Task<PatientItem> GetPatientAsync(int patientId)
	{
		Patient patient;
		try
		{
			patient = await _dbContext.Patients.FirstOrDefaultAsync(pat => pat.Id == patientId);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw;
		}

		return new PatientItem(patient);
	}

	public async Task<IEnumerable<DetailedPatientItem>> GetPatientBatchAsync(int fromId, int toId, bool includeInactive)
	{
		var patients = _dbContext.Patients
			.Include(pat => pat.Restrictions).ThenInclude(pat => pat.CommunicationRestriction)
			.Where(pat => pat.Id >= fromId && pat.Id <= toId && (includeInactive || !pat.Inactive));

		List<DetailedPatientItem> result;

		try
		{
			result = await patients.Select(patient => new DetailedPatientItem(patient)).ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<PatientInsurance> GetPatientInsuranceAsync(int patientId, PayerLevel payerLevel)
	{
		PatientInsurance patientInsurance;
		try
		{
			patientInsurance = await _dbContext.PatientInsurances
				.Include(pi => pi.InsurancePayer)
				.FirstOrDefaultAsync(patIns => patIns.PatientId == patientId && patIns.PayerLevel == payerLevel);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw;
		}

		return patientInsurance;
	}

	public async Task<IEnumerable<DetailedPatientItem>> GetUpdatedPatientsAsync(DateTime cutoffDate)
	{
		var patients = _dbContext.Patients
			.Include(pat => pat.Restrictions).ThenInclude(pat => pat.CommunicationRestriction)
			.Where(pat => pat.UpdatedDate >= cutoffDate);

		List<DetailedPatientItem> result;

		try
		{
			result = await patients.Select(patient => new DetailedPatientItem(patient)).ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task<MedicalCondition> LookupMedicalConditionAsync(string name)
	{
		var condition = await _dbContext.MedicalConditions
			.FirstOrDefaultAsync(x => string.Equals(name, x.Name, StringComparison.CurrentCultureIgnoreCase));
		return condition;
	}

	public async Task<Patient> LookupPatientAsync(string mobileNumber)
	{
		var digits = mobileNumber.Digits();
		var patients = await _dbContext.Patients.Where(x => !x.Inactive && !x.Deceased).ToListAsync();
		var match = patients.Where(x => x.MobilePhone != null && x.MobilePhone.Digits() == digits).FirstOrDefault();
		return match;
	}

	public async Task<CommunicationRestriction> LookupRestrictionAsync(string name)
	{
		var communicationRestriction = await _dbContext.CommunicationRestrictions
			.Where(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase))
			.FirstOrDefaultAsync();
		return communicationRestriction;
	}

	public async Task<bool> NameExistsAsync(int id, string firstName, string lastName, string middleInitial)
	{
		var first = firstName.Trim().ToLower();
		var last = lastName.Trim().ToLower();
		var initial = string.IsNullOrEmpty(middleInitial) ? string.Empty : middleInitial.Trim().ToLower();

		return await _dbContext.Patients.AnyAsync(p =>
			p.Id != id &&
			p.FirstName.Trim().ToLower() == first &&
			p.LastName.Trim().ToLower() == last &&
			(p.Initial == null || p.Initial == initial));
	}


	public async Task PutPatientAsync(Patient patient)
	{
		try
		{
			if (patient.Id == 0) await _dbContext.Patients.AddAsync(patient);
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

	public async Task PutSmsReplyAsync(SmsReply smsReply)
	{
		if (smsReply.Id == 0)
			await _dbContext.SmsReplies.AddAsync(smsReply);
		else
			_dbContext.SmsReplies.Attach(smsReply).State = EntityState.Modified;

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


	public async Task<IEnumerable<PatientItem>> SearchAsync(string query, SearchType searchType, bool includeInactive)
	{
		List<PatientItem> result = null;
		try
		{
			switch (searchType)
			{
				case SearchType.LastName:
					result = await _dbContext.Patients
						.Where(pat =>
							(includeInactive || !pat.Inactive) &&
							pat.LastName != null && pat.LastName.StartsWith(query))
						.Select(patient => new PatientItem(patient)).ToListAsync();
					break;
				case SearchType.FirstName:
					result = await _dbContext.Patients
						.Where(pat =>
							(includeInactive || !pat.Inactive) &&
							pat.FirstName != null && pat.FirstName.StartsWith(query))
						.Select(patient => new PatientItem(patient)).ToListAsync();
					break;
				case SearchType.DateOfBirth:
					if (DateTime.TryParse(query, out var date))
						result = await _dbContext.Patients
							.Where(pat =>
								(includeInactive || !pat.Inactive) &&
								pat.BirthDate.HasValue && pat.BirthDate.Value == date)
							.Select(patient => new PatientItem(patient)).ToListAsync();
					break;
				case SearchType.AccountNo:
					result = await _dbContext.Patients
						.Where(pat =>
							(includeInactive || !pat.Inactive) &&
							pat.AccountNo != null && pat.AccountNo.StartsWith(query))
						.Select(patient => new PatientItem(patient)).ToListAsync();
					break;
				default:
					return null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public async Task UpdateUnknownLogEntriesAsync(string phoneNumber, int patientId)
	{
		var digits = phoneNumber.Digits();
		var logs = await _dbContext.SmsLogs
			.Where(x => x.PatientId == 0 && x.AppointmentId == 0 && x.To.Digits() == digits).ToListAsync();
		foreach (var log in logs) log.PatientId = patientId;

		await _dbContext.SaveChangesAsync();
	}
}