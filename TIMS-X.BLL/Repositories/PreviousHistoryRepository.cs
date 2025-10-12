using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IPreviousHistoryRepository
{
    Task<PreviousHistory> Add(PreviousHistory previousHistory);
    void Delete(int id);
    Task<PreviousHistory> Get(int id);
    Task<List<PreviousHistory>> GetAll(bool includeInactive);
    Task<PreviousHistory> Update(PreviousHistory previousHistory);
}

public class PreviousHistoryRepository : IPreviousHistoryRepository
{
    private readonly IPreviousHistoryUnitOfWork _previousHistoryUnitOfWork;

    public PreviousHistoryRepository(IPreviousHistoryUnitOfWork previousHistoryUnitOfWork)
    {
        _previousHistoryUnitOfWork = previousHistoryUnitOfWork;
    }

    public async Task<PreviousHistory> Add(PreviousHistory previousHistory)
    {
        return await _previousHistoryUnitOfWork.Add(previousHistory);
    }

    public async Task<PreviousHistory> Get(int id)
    {
        return await _previousHistoryUnitOfWork.GetPreviousHistory(id);
    }

    public async Task<List<PreviousHistory>> GetAll(bool includeInactive)
    {
        return await _previousHistoryUnitOfWork.GetPreviousHistories();
    }

    public async Task<PreviousHistory> Update(PreviousHistory previousHistory)
    {
        return await _previousHistoryUnitOfWork.Update(previousHistory);
    }

    public void Delete(int id)
    {
        _previousHistoryUnitOfWork.Delete(id);
    }
}