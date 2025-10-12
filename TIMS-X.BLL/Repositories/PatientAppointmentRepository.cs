using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.DAL.DAL.UoWs;
using TIMS_X.DAL.Dtos;
using Appointment = TIMS_X.Core.Domain.Appointment;

namespace TIMS_X.BLL.Repositories;

public interface IPatientAppointmentRepository
{
	Task<List<PatientCandidateDto>> GetPatientAppointmentCandidates(string firstname,
		string lastname, DateTime dateOfBirth, string email, string phone);

	Task<List<PatientAppointmentItemDto>> GetPatientAppointments();

	Task<List<PatientCandidateDto>> GetPatientAppointmentSearch(string name);

	Task<Appointment> LinkPatientAppointment(PatientLinkDto patientLinkDto);
}

public class PatientAppointmentRepository(
	IAppointmentTypeUnitOfWork _appointmentTypeUnitUnitOfWork,
	IAppointmentsUnitOfWork _appointmentsUnitOfWork,
	IProvidersUnitOfWork _providersUnitOfWork,
	ISiteUnitOfWork _siteUnitOfWork,
	IPatientsUnitOfWork _patientsUnitOfWork,
	IPracticeUnitOfWork _practiceUnitOfWork) : IPatientAppointmentRepository
{
	public async Task<List<PatientCandidateDto>> GetPatientAppointmentCandidates(string firstname,
		string lastname, DateTime dateOfBirth, string email, string phone)
	{
		var initial = await _patientsUnitOfWork.FindPatients(new PatientSearchCriteriaDto
			{ LastName = lastname, FirstName = "%", DateOfBirth = dateOfBirth });

		var candidates = new List<PatientCandidateDto>();
		var names = initial.Where(x => x.FirstName == firstname);
		foreach (var patientSummary in names) candidates.Add(_CreateCandidate(patientSummary));

		var cleanPhone = Regex.Replace(phone, @"\D", "");
		foreach (var candidate in initial)
			if (candidate.Email == email && candidates.All(x => x.PatientId != candidate.Id))
				candidates.Add(_CreateCandidate(candidate));
			else if (!string.IsNullOrWhiteSpace(candidate.HomePhone) &&
			         candidates.All(x => x.PatientId != candidate.Id) &&
			         cleanPhone == Regex.Replace(candidate.HomePhone, @"\D", ""))
				candidates.Add(_CreateCandidate(candidate));
			else if (!string.IsNullOrWhiteSpace(candidate.WorkPhone) &&
			         candidates.All(x => x.PatientId != candidate.Id) &&
			         cleanPhone == Regex.Replace(candidate.WorkPhone, @"\D", ""))
				candidates.Add(_CreateCandidate(candidate));
			else if (!string.IsNullOrWhiteSpace(candidate.OtherPhone) &&
			         candidates.All(x => x.PatientId != candidate.Id) &&
			         cleanPhone == Regex.Replace(candidate.OtherPhone, @"\D", ""))
				candidates.Add(_CreateCandidate(candidate));
			else if (!string.IsNullOrWhiteSpace(candidate.MobilePhone) &&
			         candidates.All(x => x.PatientId != candidate.Id) &&
			         cleanPhone == Regex.Replace(candidate.MobilePhone, @"\D", ""))
				candidates.Add(_CreateCandidate(candidate));

		return candidates;
	}

	public async Task<List<PatientCandidateDto>> GetPatientAppointmentSearch(string name)
	{
		var candidates = new List<PatientCandidateDto>();
		var lastnames = _patientsUnitOfWork.Search(new PatientSearchCriteriaDto
			{ LastName = name });
		foreach (var patientSummary in await lastnames.ToListAsync()) candidates.Add(_CreateCandidate(patientSummary));

		var firstnames = _patientsUnitOfWork.Search(new PatientSearchCriteriaDto
			{ FirstName = name });
		foreach (var patientSummary in await firstnames.ToListAsync())
			if (candidates.All(x => x.PatientId != patientSummary.Id))
				candidates.Add(_CreateCandidate(patientSummary));
		return candidates;
	}

	public async Task<Appointment> LinkPatientAppointment(PatientLinkDto patientLinkDto)
	{
		var patient = await _patientsUnitOfWork.GetPatient(patientLinkDto.PatientId);

		var appointment = await _appointmentsUnitOfWork.GetAppointment(patientLinkDto.AppointmentId);
		appointment.PatientId = (int)patientLinkDto.PatientId;
		appointment = await _appointmentsUnitOfWork.Update(appointment);

		return appointment;
	}

	public async Task<List<PatientAppointmentItemDto>> GetPatientAppointments()
	{
        var patientAppointments = new List<PatientAppointmentItemDto>();
        var onlineAt = (await _appointmentTypeUnitUnitOfWork.GetAppointmentTypes(x => x.Name.ToLower() == "online"))
            .FirstOrDefault();
        if (onlineAt == null)
            return patientAppointments;
        var businessRules = await _practiceUnitOfWork.GetPracticeBusinessRules();
		var onlinePatientId = businessRules.OnlineAppointmentPatientId;
		
		var providers = await _providersUnitOfWork.GetProviderSummaries(x => !x.Inactive).ToListAsync();
		var sites = await _siteUnitOfWork.GetSiteSummaries(x => !x.Inactive);

		var appointments = _appointmentsUnitOfWork
			.GetAppointments(x => x.PatientId == onlinePatientId || x.AppointmentTypeId == onlineAt.Id)
			.OrderBy(a => a.StartsAt);
		
		foreach (var appointment in appointments)
		{
			if (string.IsNullOrEmpty(appointment.Notes)) continue;
			var provider = providers.FirstOrDefault(x => x.Id == appointment.ProviderId);
			var site = sites.FirstOrDefault(x => x.Id == appointment.SiteId);
			var a = new PatientAppointmentItemDto(appointment);
			var s = sites.FirstOrDefault(x => x.Id == appointment.SiteId);
			if (s != null) a.Site = s.Name;
			var p = providers.FirstOrDefault(x => x.Id == appointment.ProviderId);
			if (p != null) a.Provider = $"{provider.FirstName} {provider.LastName}";
			patientAppointments.Add(a);
		}

		return patientAppointments;
	}

	private PatientCandidateDto _CreateCandidate(PatientSummary patientSummary)
	{
		var candidate = new PatientCandidateDto
		{
			DateOfBirth = patientSummary.BirthDate,
			Email = patientSummary.Email,
			FirstName = patientSummary.FirstName,
			Initial = patientSummary.Initial,
			LastName = patientSummary.LastName,
			PatientId = patientSummary.Id
		};
		switch ((PrimaryPhoneEnum)patientSummary.PrimaryPhone)
		{
			case PrimaryPhoneEnum.Home:
				candidate.Phone = patientSummary.HomePhone;
				break;
			case PrimaryPhoneEnum.Work:
				candidate.Phone = patientSummary.WorkPhone;
				break;
			case PrimaryPhoneEnum.Other:
				candidate.Phone = patientSummary.OtherPhone;
				break;
			default:
				candidate.Phone = patientSummary.MobilePhone;
				break;
		}

		return candidate;
	}
}