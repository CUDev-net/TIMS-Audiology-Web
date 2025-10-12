using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IApptAuthorizationRepository
{
	Task<ApptAuthorization> Add(ApptAuthorization appointmentType);
	void Delete(int id);
	Task<ApptAuthorization> Get(int id);
	Task<List<ApptAuthorization>> GetAll(bool includeInactive, int patientId, int authorizationId);
	Task<ApptAuthorization> Update(ApptAuthorization appointmentType);
}

public class ApptAuthorizationRepository : IApptAuthorizationRepository
{
	private readonly IApptAuthorizationUnitOfWork _apptAuthorizationUnitOfWork;
	private readonly IAppointmentStatusUnitOfWork _appointmentStatusUnitOfWork;
	private readonly IAppointmentsUnitOfWork _appointmentsUnitOfWork;

	public ApptAuthorizationRepository(IApptAuthorizationUnitOfWork apptAuthorizationUnitOfWork, 
		IAppointmentStatusUnitOfWork appointmentStatusUnitOfWork,
		IAppointmentsUnitOfWork appointmentsUnitOfWork)
	{
		_apptAuthorizationUnitOfWork = apptAuthorizationUnitOfWork;
		_appointmentStatusUnitOfWork = appointmentStatusUnitOfWork;
		_appointmentsUnitOfWork = appointmentsUnitOfWork;
	}

	public async Task<ApptAuthorization> Add(ApptAuthorization apptAuthorization)
	{
		return await _apptAuthorizationUnitOfWork.Add(apptAuthorization);
	}

	public async Task<ApptAuthorization> Get(int id)
	{
		return await _apptAuthorizationUnitOfWork.GetApptAuthorization(id);
	}

	public async Task<List<ApptAuthorization>> GetAll(bool includeInactive, int patientId, int authorizationId)
	{
		var noShowStatuses = await 
			_appointmentStatusUnitOfWork.GetAppointmentStatuses(x =>
			x.Name.ToLower().Contains("no show") || 
			x.Name.ToLower().Contains("cancel") || 
			x.Name.ToLower().Contains("reschedule"));
		
		var noShowIds = noShowStatuses.Select(x => x.Id).ToList();

		var authorizations = await _apptAuthorizationUnitOfWork.GetApptAuthorizations(x =>
			(includeInactive || !x.Inactive) && x.PatientId == patientId);

		foreach (var authorization in authorizations)
		{
			authorization.NumberUsed = await 
				_appointmentsUnitOfWork.GetAppointments(
					a => a.PatientId == patientId && a.AuthorizationId == authorization.Id
					&& !noShowIds.Contains( a.AppointmentStatusId)).CountAsync();
			authorization.DisplayString = $"{authorization.Name} ({authorization.NumberUsed}/{authorization.Authorizations})";
			if (authorization.Expires.HasValue)
			{
				authorization.DisplayString += $" - Exp. {authorization.Expires.Value.ToShortDateString()}";
				if (authorization.Expires.Value.Date < DateTime.Now.Date)
				{
					authorization.Inactive = true;
				}
			}

			if (authorization.NumberUsed >= authorization.Authorizations)
				authorization.Inactive = true;
		}
		
		var authorizationsToReturn =  includeInactive ? authorizations : 
			authorizations.Where(x => !x.Inactive).ToList();

		if (authorizationId > 0 && authorizationsToReturn.All(a => a.Id != authorizationId))
		{
			var a = authorizations.FirstOrDefault(x => x.Id == authorizationId);
			if(a != null)
			{
				authorizationsToReturn.Add(a);
			}
		}
		return authorizationsToReturn;
	}

	public async Task<ApptAuthorization> Update(ApptAuthorization apptAuthorization)
	{
		return await _apptAuthorizationUnitOfWork.Update(apptAuthorization);
	}

	public void Delete(int id)
	{
		_apptAuthorizationUnitOfWork.Delete(id);
	}
}