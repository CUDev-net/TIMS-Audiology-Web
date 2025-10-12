using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHistoryTypeRepository
{
    Task<HistoryType> Add(HistoryType historyType);
    void Delete(int id);
    Task<HistoryType> Get(int id);
    Task<List<HistoryType>> GetAll(bool includeInactive);
    Task<HistoryType> Update(HistoryType historyType);
}

public class HistoryTypeRepository : IHistoryTypeRepository
{
    private readonly IHistoryTypeUnitOfWork _historyTypeUnitOfWork;

    public HistoryTypeRepository(IHistoryTypeUnitOfWork historyTypeUnitOfWork)
    {
        _historyTypeUnitOfWork = historyTypeUnitOfWork;
    }

    public async Task<HistoryType> Add(HistoryType historyType)
    {
        return await _historyTypeUnitOfWork.Add(historyType);
    }

    public void Delete(int id)
    {
        _historyTypeUnitOfWork.Delete(id);
    }

    public async Task<HistoryType> Get(int id)
    {
        return await _historyTypeUnitOfWork.GetHistoryType(id);
    }

    public async Task<List<HistoryType>> GetAll(bool includeInactive)
    {
        return await _historyTypeUnitOfWork.GetHistoryTypes(x => includeInactive || !x.Inactive); 
    }

    public async Task<HistoryType> Update(HistoryType historyType)
    {
        return await _historyTypeUnitOfWork.Update(historyType);
    }
}