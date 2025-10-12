using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IKpiSiteTargetValidator
{
    Task<List<ValidationResult>> AddNew(KpiSiteTarget kpiSiteTarget);
    Task<List<ValidationResult>> Update(KpiSiteTarget kpiSiteTarget);
}

public class KpiSiteTargetValidator : IKpiSiteTargetValidator
{
    private readonly IKpiSiteTargetUnitOfWork _kpiSiteTargetUnitOfWork;
    private readonly ISiteUnitOfWork _siteUnitOfWork;

    public KpiSiteTargetValidator(ISiteUnitOfWork siteUnitOfWork,
        IKpiSiteTargetUnitOfWork kpiSiteTargetUnitOfWork)
    {
        _siteUnitOfWork = siteUnitOfWork;
        _kpiSiteTargetUnitOfWork = kpiSiteTargetUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(KpiSiteTarget kpiSiteTarget)
    {
        var validationResults = await _ValidateBase(kpiSiteTarget);

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(KpiSiteTarget kpiSiteTarget)
    {
        var validationResults = await _ValidateBase(kpiSiteTarget);

        return validationResults;
    }

    private async Task<List<ValidationResult>> _ValidateBase(KpiSiteTarget kpiSiteTarget)
    {
        var validationResults = new List<ValidationResult>();
        var existing = await _siteUnitOfWork.GetSite(kpiSiteTarget.SiteId);
        if (existing == null)
            validationResults.Add(new ValidationResult("Site must exist", Severity.Error));
        if (_kpiSiteTargetUnitOfWork.GetKpiSiteTarget(kpiSiteTarget.Id).Result.StartDate <= DateTime.MinValue)
            validationResults.Add(new ValidationResult("Invalid StartDate", Severity.Error));

        return validationResults;
    }
}