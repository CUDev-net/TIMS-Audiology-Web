using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Core.Models.Legacy;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Services;

public class PatientService
{
	private readonly ContextHelper _contextHelper;
	private readonly CustomerQuery _customerQuery;
	private readonly PatientQuery _patientQuery;
	private readonly PracticeQuery _practiceQuery;

	public PatientService(PatientQuery patientQuery, PracticeQuery practiceQuery, CustomerQuery customerQuery,
		ContextHelper contextHelper)
	{
		_patientQuery = patientQuery;
		_practiceQuery = practiceQuery;
		_customerQuery = customerQuery;
		_contextHelper = contextHelper;
	}

	private async Task<string> _GenerateLinkAsync(PatientFormTypeEnum formType, int patientId, int userId)
	{
		var customerId = await _customerQuery.GetCustomerIdAsync(_contextHelper.OfficeCode);

		FormLink formLink = null;
		formLink = await _customerQuery.GetFormLinkAsync(customerId, userId, patientId, formType);
		var success = false;
		if (formLink == null)
		{
			formLink = new FormLink
			{
				DateCreated = DateTime.Now,
				CustomerId = customerId,
				UserId = userId,
				FormType = formType,
				PatientId = patientId
			};
			formLink.Url = await _GenerateUrlAsync();
			success = await _customerQuery.PutFormLinkAsync(formLink);
		}
		else
		{
			success = true;
		}

		if (success)
		{
			var baseUrl = await _practiceQuery.GetValueAsync(x => x.TimsServer) ?? "https://us.timsaudiology.com";
			return baseUrl + "/PatientForm?p=" + formLink.Url;
		}

		return null;
	}

	private async Task<string> _GenerateUrlAsync()
	{
		var urlsafe = string.Empty;
		Enumerable.Range(48, 75)
			.Where(i => i < 58 || (i > 64 && i < 91) || i > 96)
			.OrderBy(o => new Random().Next())
			.ToList()
			.ForEach(i => urlsafe += Convert.ToChar(i)); // Store each char into urlsafe

		var url = urlsafe.Substring(new Random().Next(0, urlsafe.Length - 6), 6).ToUpper();
		while (await _customerQuery.FormLinkUrlExistsAsync(url))
			url = urlsafe.Substring(new Random().Next(0, urlsafe.Length), new Random().Next(6, 6)).ToUpper();
		return url;
	}

	private void _MapPatientUpdateModelToPatient(PatientUpdateModel updateModel,
		Patient patient)
	{
		patient.AccountNo = updateModel.AccountNumber;
		patient.Address1 = updateModel.AddressLine1;
		patient.Address2 = updateModel.AddressLine2;
		patient.BirthDate = updateModel.BirthDate;
		patient.City = updateModel.City;
		patient.ProviderId = updateModel.DefaultProviderID;
		patient.Email = updateModel.Email;
		patient.FirstName = updateModel.FirstName;
		patient.HomePhone = updateModel.HomePhone;
		patient.LastName = updateModel.LastName;
		patient.MarketingId = updateModel.MarketingReferenceId;
		patient.Initial = updateModel.MiddleInitial;
		patient.MobilePhone = updateModel.MobilePhone;
		patient.ReferringPhysicianId = updateModel.ReferralSourceId;
		patient.Sex = updateModel.Sex;
		patient.SiteId = updateModel.SiteId;
		patient.Ssn = updateModel.SocialSecurityNumber;
		patient.State = updateModel.State;
		patient.WorkPhone = updateModel.WorkPhone;
		patient.ZipCode = updateModel.ZipCode;
	}

	private async Task _ValidatePatientAsync(Patient patient)
	{
		if (string.IsNullOrWhiteSpace(patient.FirstName)) throw new ValidationException("First name is required.");
		if (string.IsNullOrWhiteSpace(patient.LastName)) throw new ValidationException("Last name is required.");

		if (!patient.SiteId.HasValue) throw new ValidationException("Site is required.");
		if (!await _practiceQuery.ValidateSiteAsync(patient.SiteId ?? 0))
			throw new ValidationException($"Site with ID {patient.SiteId.Value} is inactive or doesn't exist.");

		var businessRules = await _practiceQuery.GetBusinessRulesAsync();
		// is new patient?
		if (patient.Id == 0 && businessRules.ForceMarketingSourcePatientAppointment)
			if (patient.MarketingId == 0)
				throw new ValidationException("Marketing source is required.");

		if (!businessRules.AllowDuplicatePatientNames)
		{
			var nameExists =
				await _patientQuery.NameExistsAsync(patient.Id, patient.FirstName, patient.LastName, patient.Initial);

			if (nameExists) throw new ValidationException("Name already exists. Duplicate names are not allowed.");
		}
	}

	public async Task<bool> ExistsAsync(int id)
	{
		return await _patientQuery.ExistsAsync(id);
	}

	public async Task<Patient> GetAsync(int id)
	{
		return await _patientQuery.GetFullPatientAsync(id);
	}

	public async Task<Patient> GetAsync(Guid patientGuid)
	{
		return await _patientQuery.GetFullPatientAsync(patientGuid);
	}

	public async Task<Tuple<bool, string>> GetFormLinkAsync(PatientFormTypeEnum formType, int patientId, int userId)
	{
		var patient = await _patientQuery.GetFullPatientAsync(patientId);

		if (patient == null)
			return new Tuple<bool, string>(false, "Patient does not exist");
		if (patient.HasIntakeData)
			return new Tuple<bool, string>(false, "Existing intake data needs review");
		var formExists = await _practiceQuery.DigitalFormExistsAsync(formType);
		if (!formExists)
			return new Tuple<bool, string>(false, "Digital form does not exist");

		var link = await _GenerateLinkAsync(formType, patientId, userId);

		return new Tuple<bool, string>(true, link);
	}

	public async Task<IEnumerable<PatientItem>> SearchAsync(string query, SearchType searchType, bool includeInactive)
	{
		return await _patientQuery.SearchAsync(query, searchType, includeInactive);
	}

	public async Task UpdatePatientAsync(int id, PatientUpdateModel updatePatient)
	{
		var patient = await _patientQuery.GetDomainPatientAsync(id);
		if (patient != null)
		{
			_MapPatientUpdateModelToPatient(updatePatient, patient);

			await _ValidatePatientAsync(patient);

			patient.UpdatedSiteId = _contextHelper.CurrentSite.Id;
			patient.UpdatedUserId = _contextHelper.CurrentUser.Id;
			patient.UpdatedDate = DateTime.Now;

			await _patientQuery.PutPatientAsync(patient);
		}
	}
}