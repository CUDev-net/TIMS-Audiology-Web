using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IKpiSiteTargetRepository
{
    Task<KpiSiteTarget> Add(KpiSiteTarget kpiSiteTarget);
    void Delete(int id);
    Task<KpiSiteTarget> Get(int id);
    Task<List<KpiSiteTarget>> GetAll(bool includeInactive);
    Task<KpiSiteTarget> Update(KpiSiteTarget kpiSiteTarget);
}

public class KpiSiteTargetRepository : IKpiSiteTargetRepository
{
    private readonly IKpiSiteTargetUnitOfWork _kpiSiteTargetUnitOfWork;

    public KpiSiteTargetRepository(IKpiSiteTargetUnitOfWork kpiSiteTargetUnitOfWork)
    {
        _kpiSiteTargetUnitOfWork = kpiSiteTargetUnitOfWork;
    }

    public async Task<KpiSiteTarget> Add(KpiSiteTarget kpiSiteTarget)
    {
        return await _kpiSiteTargetUnitOfWork.Add(kpiSiteTarget);
    }

    public void Delete(int id)
    {
        _kpiSiteTargetUnitOfWork.Delete(id);
    }

    public async Task<KpiSiteTarget> Get(int id)
    {
        return await _kpiSiteTargetUnitOfWork.GetKpiSiteTarget(id);
    }

    public async Task<List<KpiSiteTarget>> GetAll(bool includeInactive)
    {
        return await _kpiSiteTargetUnitOfWork.GetKpiSiteTargets(x => includeInactive || !x.Inactive);
    }

    public async Task<KpiSiteTarget> Update(KpiSiteTarget kpiSiteTarget)
    {
        return await _kpiSiteTargetUnitOfWork.Update(kpiSiteTarget);
    }
}