using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Data;

namespace TIMS_X.Server.Queries;

public class HistoryQuery
{
	private readonly ContextHelper _contextHelper;
	private readonly HistoryDbContext _dbContext;

	public HistoryQuery(HistoryDbContext dbContext, ContextHelper contextHelper)
	{
		_dbContext = dbContext;
		_contextHelper = contextHelper;
	}

	public async Task CreateHistoryForAppointmentAsync(Appointment appointment)
	{
		var practice = await _dbContext.Practices.FirstOrDefaultAsync();
		if (practice == null || !practice.LinkAppointmentHistory)
			return;

		var exists = await _dbContext.Histories.AnyAsync(x => x.AppointmentId == appointment.Id);
		if (exists)
			return;

		var referringPhysicianId = await _dbContext.Patients
			.Where(x => x.Id == appointment.PatientId)
			.Select(x => x.ReferringPhysicianId)
			.FirstOrDefaultAsync();


		var history = new History
		{
			AppointmentId = appointment.Id,
			PatientId = appointment.PatientId,
			ProviderId = appointment.ProviderId,
			SyncSiteId = appointment.SyncSiteId,
			ReferralSourceId = referringPhysicianId ?? 0,
			AvailableDate = appointment.StartsAt,
			HistoryDate = appointment.StartsAt,
			CreatedDate = DateTime.Now,
			UpdatedDate = DateTime.Now
		};

		if (_contextHelper.CurrentUser != null)
			history.UpdatedUserId = _contextHelper.CurrentUser.Id;
		else
			history.UpdatedUserId = appointment.UpdatedUserId;

		if (_contextHelper.CurrentSite != null)
			history.UpdatedSiteId = _contextHelper.CurrentSite.Id;
		else
			history.UpdatedSiteId = appointment.UpdatedSiteId;

		_dbContext.Histories.Add(history);
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

	public async Task DeleteHistoryForPendingAppointmentAsync(Appointment appointment)
	{
		if (appointment.StartsAt < DateTime.Now) return;

		var histories = await _dbContext.Histories
			.Where(x => x.AppointmentId == appointment.Id)
			.ToListAsync();

		foreach (var history in histories) _dbContext.Remove(history);

		await _dbContext.SaveChangesAsync();
	}
}