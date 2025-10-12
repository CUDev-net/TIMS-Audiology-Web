using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHaHistoryValidator
{
    Task<List<ValidationResult>> AddNew(HaHistory haHistory);
    Task<List<ValidationResult>> Update(HaHistory haHistory);
}

public class HaHistoryValidator : IHaHistoryValidator
{
    private readonly IAppointmentsUnitOfWork _appointmentsUnitOfWork;
    private readonly IBatterySizeUnitOfWork _batterySizeUnitOfWork;
    private readonly IHaModelUnitOfWork _haModelUnitOfWork;
    private readonly IHaStatusUnitOfWork _haStatusUnitOfWork;
    private readonly IHaStyleUnitOfWork _haStyleUnitOfWork;
    private readonly IHaWarrantyTypeUnitOfWork _haWarrantyTypeUnitOfWork;
    private readonly IPatientsUnitOfWork _patientsUnitOfWork;
    private readonly IProvidersUnitOfWork _providersUnitOfWork;
    private readonly ISiteUnitOfWork _siteUnitOfWork;

    public HaHistoryValidator(IBatterySizeUnitOfWork batterySizeUnitOfWork,
        IPatientsUnitOfWork patientsUnitOfWork,
        IProvidersUnitOfWork providersUnitOfWork, ISiteUnitOfWork siteUnitOfWork, IHaModelUnitOfWork haModelUnitOfWork,
        IHaWarrantyTypeUnitOfWork haWarrantyTypeUnitOfWork, IAppointmentsUnitOfWork appointmentsUnitOfWork,
        IHaStyleUnitOfWork haStyleUnitOfWork, IHaStatusUnitOfWork haStatusUnitOfWork)
    {
        _batterySizeUnitOfWork = batterySizeUnitOfWork;
        _patientsUnitOfWork = patientsUnitOfWork;
        _providersUnitOfWork = providersUnitOfWork;
        _siteUnitOfWork = siteUnitOfWork;
        _haModelUnitOfWork = haModelUnitOfWork;
        _haWarrantyTypeUnitOfWork = haWarrantyTypeUnitOfWork;
        _appointmentsUnitOfWork = appointmentsUnitOfWork;
        _haStyleUnitOfWork = haStyleUnitOfWork;
        _haStatusUnitOfWork = haStatusUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HaHistory haHistory)
    {
        var validationResults = _ValidateBase(haHistory);
        return await validationResults;
    }

    public async Task<List<ValidationResult>> Update(HaHistory haHistory)
    {
        var validationResults = _ValidateBase(haHistory);
        return await validationResults;
    }

    private async Task<List<ValidationResult>> _ValidateBase(HaHistory haHistory)
    {
        var validationResults = new List<ValidationResult>();
        if (haHistory.BatterySizeId > 0)
            if (await _batterySizeUnitOfWork.GetBatterySize(haHistory.BatterySizeId) == null)
                validationResults.Add(new ValidationResult("BatterySize must exist", Severity.Error));
        if (haHistory.AppointmentId > 0)
            if (await _appointmentsUnitOfWork.GetAppointment(haHistory.AppointmentId) == null)
                validationResults.Add(new ValidationResult("Appointment must exist", Severity.Error));
        if (await _patientsUnitOfWork.GetPatient(haHistory.PatientId) == null)
            validationResults.Add(new ValidationResult("Patient must exist", Severity.Error));
        if (haHistory.ProviderId > 0)
            if (await _providersUnitOfWork.GetProvider(haHistory.ProviderId) == null)
                validationResults.Add(new ValidationResult("Provider must exist", Severity.Error));
        if (haHistory.SyncSiteId > 0)
            if (await _siteUnitOfWork.GetSite(haHistory.SyncSiteId) == null)
                validationResults.Add(new ValidationResult("Site must exist", Severity.Error));
        if (await _haModelUnitOfWork.GetHaModel(haHistory.HaModelId) == null)
            validationResults.Add(new ValidationResult("HaModel must exist", Severity.Error));
        if (haHistory.WarrantyTypeId > 0)
            if (await _haWarrantyTypeUnitOfWork.GetHaWarrantyType(haHistory.WarrantyTypeId) == null)
                validationResults.Add(new ValidationResult("WarrantyType must exist", Severity.Error));
        if (haHistory.HaStyleId > 0)
            if (await _haStyleUnitOfWork.GetHaStyle(haHistory.HaStyleId) == null)
                validationResults.Add(new ValidationResult("HaStyle must exist", Severity.Error));
        if (haHistory.HaStatusId > 0)
            if (await _haStatusUnitOfWork.GetHaStatus(haHistory.HaStatusId) == null)
                validationResults.Add(new ValidationResult("HaStatus must exist", Severity.Error));

        return validationResults;
    }
}