using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHistoryValidator
{
    Task<List<ValidationResult>> AddNew(History history);
    Task<List<ValidationResult>> Update(History history);
}

public class HistoryValidator : IHistoryValidator
{
    private readonly IAppointmentsUnitOfWork _appointmentsUnitOfWork;
    private readonly IHistoryTypeUnitOfWork _historyTypeUnitOfWork;
    private readonly IMarketingReferenceUnitOfWork _marketingReferenceUnitOfWork;
    private readonly IPatientsUnitOfWork _patientsUnitOfWork;
    private readonly IProvidersUnitOfWork _providersUnitOfWork;
    private readonly ISiteUnitOfWork _siteUnitOfWork;

    public HistoryValidator(IPatientsUnitOfWork patientsUnitOfWork,
        IProvidersUnitOfWork providersUnitOfWork, IHistoryTypeUnitOfWork historyTypeUnitOfWork,
        ISiteUnitOfWork siteUnitOfWork, IAppointmentsUnitOfWork appointmentsUnitOfWork,
        IMarketingReferenceUnitOfWork marketingReferenceUnitOfWork)
    {
        _appointmentsUnitOfWork = appointmentsUnitOfWork;
        _historyTypeUnitOfWork = historyTypeUnitOfWork;
        _patientsUnitOfWork = patientsUnitOfWork;
        _providersUnitOfWork = providersUnitOfWork;
        _siteUnitOfWork = siteUnitOfWork;
        _marketingReferenceUnitOfWork = marketingReferenceUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(History history)
    {
        var validationResults = _ValidateBase(history);
        return await validationResults;
    }

    public async Task<List<ValidationResult>> Update(History history)
    {
        var validationResults = _ValidateBase(history);
        return await validationResults;
    }

    private async Task<List<ValidationResult>> _ValidateBase(History history)
    {
        var validationResults = new List<ValidationResult>();
        var referralSource = await _marketingReferenceUnitOfWork.GetMarketingReference(history.ReferralSourceId);
        if (await _patientsUnitOfWork.GetPatient(history.PatientId) == null)
            validationResults.Add(new ValidationResult("Patient must exist", Severity.Error));
        if (history.ProviderId > 0)
            if (await _providersUnitOfWork.GetProvider(history.ProviderId) == null)
                validationResults.Add(new ValidationResult("Provider must exist", Severity.Error));
        if (await _historyTypeUnitOfWork.GetHistoryType(history.HistoryTypeId) == null)
            validationResults.Add(new ValidationResult("HistoryType must exist", Severity.Error));
        if (history.AppointmentId > 0)
            if (await _appointmentsUnitOfWork.GetAppointment(history.AppointmentId) == null)
                validationResults.Add(new ValidationResult("Appointment must exist", Severity.Error));
        if (history.ReferralSourceId > 0)
            if (referralSource == null || history.MarketingReference.CategoryId != 7)
                validationResults.Add(new ValidationResult("ReferralSource must be valid", Severity.Error));
        if (history.SyncSiteId > 0)
            if (await _siteUnitOfWork.GetSite(history.SyncSiteId) == null)
                validationResults.Add(new ValidationResult("Site must exist", Severity.Error));

        return validationResults;
    }
}