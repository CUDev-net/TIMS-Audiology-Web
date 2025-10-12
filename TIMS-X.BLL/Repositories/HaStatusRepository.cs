using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHaStatusRepository
{
    Task<HaStatus> Add(HaStatus haStatus);
    void Delete(int id);
    Task<HaStatus> Get(int id);
    Task<List<HaStatus>> GetAll(bool includeInactive);
    Task<HaStatus> Update(HaStatus haStatus);
}

public class HaStatusRepository : IHaStatusRepository
{
    private readonly IHaStatusUnitOfWork _haStatusUnitOfWork;

    public HaStatusRepository(IHaStatusUnitOfWork haHaStatusUnitOfWork)
    {
        _haStatusUnitOfWork = haHaStatusUnitOfWork;
    }

    public async Task<HaStatus> Add(HaStatus haStatus)
    {
        return await _haStatusUnitOfWork.Add(haStatus);
    }

    public void Delete(int id)
    {
        _haStatusUnitOfWork.Delete(id);
    }

    public async Task<HaStatus> Get(int id)
    {
        return await _haStatusUnitOfWork.GetHaStatus(id);
    }

    public async Task<List<HaStatus>> GetAll(bool includeInactive)
    {
        return await _haStatusUnitOfWork.GetHaStatuses(x => includeInactive || !x.Inactive);
    }

    public async Task<HaStatus> Update(HaStatus haStatus)
    {
        return await _haStatusUnitOfWork.Update(haStatus);
    }
}