using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.DAL.DAL.UoWs;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.BLL.Repositories;

public interface IPatientRepository
{
	Task<Patient> Add(Patient patient);
	Task<Patient> Get(int id);
	Task<PatientSummary> GetSummary(int id);
	Task<List<PatientSummary>> Search(PatientSearchCriteriaDto search);
	Task<Patient> Update(Patient patient);
}

public class PatientRepository : IPatientRepository
{
	private readonly IAppointmentsUnitOfWork _appointmentsUnitOfWork;
	private readonly IAuthorizationReferenceUnitOfWork _authorizationReferenceUnitOfWork;
	private readonly ContextHelper _contextHelper;
	private readonly IPatientsUnitOfWork _patientsUnitOfWork;
	private readonly IPatientTypeReferenceUnitOfWork _patientTypeReferenceUnitOfWork;
	private readonly IUserRepository _userRepository;

	public PatientRepository(IPatientsUnitOfWork patientsUnitOfWork,
		IAppointmentsUnitOfWork appointmentsUnitOfWork,
		IUserRepository userRepository,
		IAuthorizationReferenceUnitOfWork authorizationReferenceUnitOfWork,
		IPatientTypeReferenceUnitOfWork patientTypeReferenceUnitOfWork,
		ContextHelper contextHelper)
	{
		_patientsUnitOfWork = patientsUnitOfWork;
		_appointmentsUnitOfWork = appointmentsUnitOfWork;
		_userRepository = userRepository;
		_contextHelper = contextHelper;
		_authorizationReferenceUnitOfWork = authorizationReferenceUnitOfWork;
		_patientTypeReferenceUnitOfWork = patientTypeReferenceUnitOfWork;
	}

	public async Task<Patient> Add(Patient patient)
	{
		_FormatForPersist(patient);
		patient.OtStatusId = (int)OpportunityStatusEnum.TwoEarsAidable;
		patient.OtStatusDescriptionId =
			33; // Patient is an opportunity, has no hearing aids and no hearing test for both ears

		//HACK
		patient.UpdatedSiteId = 0;
		var newPatient = await _patientsUnitOfWork.Add(patient);
		foreach (var id in patient.RestrictionIds)
			patient.Restrictions.Add(new PatientRestriction
			{
				PatientId = newPatient.Id,
				RestrictionId = id
			});

		patient.PatientTypeReferences =
			await _patientTypeReferenceUnitOfWork.UpdatePatientTypeReferences(newPatient.Id,
				patient.PatientTypeIds.ToList());

		patient.AuthorizationReferences =
			await _authorizationReferenceUnitOfWork.UpdateAuthorizationReferences(patient.Id,
				patient.AuthorizationIds.ToList());

		return newPatient;
	}

	public async Task<Patient> Get(int id)
	{
		var patient = await _patientsUnitOfWork.GetPatient(id,
			i => i.Include(x => x.Restrictions)
		);

		if (patient != null)
		{
			patient.AuthorizationReferences = await _authorizationReferenceUnitOfWork
				.GetAuthorizationReferences(patient.Id);
			patient.PatientTypeReferences = await _patientTypeReferenceUnitOfWork
				.GetPatientTypeReferences(patient.Id);
		}

		return patient;
	}

	public async Task<PatientSummary> GetSummary(int id)
	{
		var patientSummary = await _patientsUnitOfWork.GetPatientSummary(id);

		if (patientSummary != null)
		{
			// Get Restrictions
			patientSummary.CommunicationRestrictions =
				await _patientsUnitOfWork.GetPatientRestrictions(id).ToListAsync();

			var lastAppointment = await _appointmentsUnitOfWork.GetLastAppointmentForPatient(id, DateTime.Now);
			if (lastAppointment != null)
			{
				patientSummary.LastAppointmentDate = lastAppointment.StartsAt;
				patientSummary.LastAppointmentStatus = lastAppointment.AppointmentStatus.Name;
			}

			var nextAppointment = await _appointmentsUnitOfWork.GetNextAppointmentForPatient(id, DateTime.Now);
			if (nextAppointment != null)
			{
				patientSummary.NextAppointmentDate = nextAppointment.StartsAt;
				patientSummary.NextAppointmentStatus = nextAppointment.AppointmentStatus.Name;
			}

			var appointmentHistory =
				_appointmentsUnitOfWork.GetPatientAppointmentSummaries(id);

			patientSummary.Appointments = await appointmentHistory.ToListAsync();

			// TODO Get Last History

			if (patientSummary.PrimaryPhone == 0 || patientSummary.PrimaryPhone == (int)PrimaryPhoneEnum.Home)
				patientSummary.PhoneToDisplay = patientSummary.HomePhone;
			else if (patientSummary.PrimaryPhone == (int)PrimaryPhoneEnum.Mobile)
				patientSummary.PhoneToDisplay = patientSummary.MobilePhone;
			else if (patientSummary.PrimaryPhone == (int)PrimaryPhoneEnum.Other)
				patientSummary.PhoneToDisplay = patientSummary.OtherPhone;
			else
				patientSummary.PhoneToDisplay = patientSummary.WorkPhone;

			if (!string.IsNullOrWhiteSpace(patientSummary.PhoneToDisplay))
				patientSummary.PhoneToDisplay = patientSummary.PhoneToDisplay.FormatPhone();
		}

		return patientSummary;
	}

	public async Task<List<PatientSummary>> Search(PatientSearchCriteriaDto search)
	{
		return await _patientsUnitOfWork.Search(search).ToListAsync();
	}

	public async Task<Patient> Update(Patient patient)
	{
		_FormatForPersist(patient);
		if (_contextHelper.CurrentSite == null)
			_contextHelper.CurrentSite = await _userRepository.GetUserSite(_contextHelper.CurrentUser.Id);
		if (_contextHelper.CurrentSite != null)
			patient.UpdatedSiteId = _contextHelper.CurrentSite.Id;

		// Restrictions
		foreach (var newId in patient.RestrictionIds)
		{
			var e = patient.Restrictions.FirstOrDefault(x => x.RestrictionId == newId);
			if (e == null)
				patient.Restrictions.Add(new PatientRestriction
				{
					PatientId = patient.Id,
					RestrictionId = newId
				});
		}

		if (patient.Restrictions != null)
			foreach (var restriction in patient.Restrictions)
				if (!patient.RestrictionIds.Contains(restriction.RestrictionId))
					_patientsUnitOfWork.SetItemDeletedState(restriction);
		// Authorizations
		patient = await _patientsUnitOfWork.Update(patient);
		patient.AuthorizationReferences =
			await _authorizationReferenceUnitOfWork.UpdateAuthorizationReferences(patient.Id,
				patient.AuthorizationIds.ToList());

		// PatientTypes
		patient.PatientTypeReferences =
			await _patientTypeReferenceUnitOfWork.UpdatePatientTypeReferences(patient.Id,
				patient.PatientTypeIds.ToList());

		return patient;
	}

	private void _FormatForPersist(Patient patient)
	{
		patient.SalutationId ??= 0;
		patient.MarketingId ??= 0;
		patient.InsuredInsurancePayerId ??= 0;
		patient.PrimaryCareId ??= 0;
		patient.PatientStatusId ??= 0;
		patient.PatientTypeId ??= 0;
		patient.ProviderId ??= 0;
		patient.ReferringPhysicianId ??= 0;

		if (patient.BirthDate.HasValue) patient.BirthDate = patient.BirthDate.Value.Date;
	}

	private IIncludableQueryable<Patient, object> _Includes(IQueryable<Patient> patient)
	{
		return null;
		//return patient.Include(p => p.PatientInsurances)
		//    .ThenInclude(i => i.InsurancePayer);
	}
}