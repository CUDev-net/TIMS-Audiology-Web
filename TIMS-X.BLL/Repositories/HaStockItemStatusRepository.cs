using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHaStockItemStatusRepository
{
    Task<HaStockItemStatus> Add(HaStockItemStatus haStockItemStatus);
    void Delete(int id);
    Task<HaStockItemStatus> Get(int id);
    Task<List<HaStockItemStatus>> GetAll(bool includeInactive);
    Task<HaStockItemStatus> Update(HaStockItemStatus haStockItemStatus);
}

public class HaStockItemStatusRepository : IHaStockItemStatusRepository
{
    private readonly IHaStockItemStatusUnitOfWork _haStockItemStatusUnitOfWork;

    public HaStockItemStatusRepository(IHaStockItemStatusUnitOfWork haStockItemStatusUnitOfWork)
    {
        _haStockItemStatusUnitOfWork = haStockItemStatusUnitOfWork;
    }

    public async Task<HaStockItemStatus> Add(HaStockItemStatus haStockItemStatus)
    {
        return await _haStockItemStatusUnitOfWork.Add(haStockItemStatus);
    }

    public void Delete(int id)
    {
        _haStockItemStatusUnitOfWork.Delete(id);
    }

    public async Task<HaStockItemStatus> Get(int id)
    {
        return await _haStockItemStatusUnitOfWork.GetHaStockItemStatus(id);
    }

    public async Task<List<HaStockItemStatus>> GetAll(bool includeInactive)
    {
        return await _haStockItemStatusUnitOfWork.GetHaStockItemStatuses(x => includeInactive || !x.Inactive);
    }

    public async Task<HaStockItemStatus> Update(HaStockItemStatus haStockItemStatus)
    {
        return await _haStockItemStatusUnitOfWork.Update(haStockItemStatus);
    }
}