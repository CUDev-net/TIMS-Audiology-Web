using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHistoryRepository
{
    Task<History> Add(History history);
    void Delete(int id);
    Task<History> Get(int id);
    Task<History> Update(History history);
    Task<List<History>> GetAll(bool includeInactive);
    Task<List<History>> GetAllByPatientId(int patientId, DateTime? minimumDate);
}

public class HistoryRepository : IHistoryRepository
{
    private readonly IHistoryUnitOfWork _historyUnitOfWork;

    public HistoryRepository(IHistoryUnitOfWork historyUnitOfWork)
    {
        _historyUnitOfWork = historyUnitOfWork;
    }

    public async Task<History> Add(History history)
    {
        return await _historyUnitOfWork.Add(history);
    }

    public void Delete(int id)
    {
        _historyUnitOfWork.Delete(id);
    }

    public async Task<History> Get(int id)
    {
        return await _historyUnitOfWork.GetHistory(id);
    }

    public async Task<List<History>> GetAllByPatientId(int patientId, DateTime? minimumDate = null)
    {
        if (minimumDate.HasValue)
            return await _historyUnitOfWork.GetHistories(x =>
                x.PatientId == patientId && x.AvailableDate > minimumDate);
        return await _historyUnitOfWork.GetHistories(x => x.PatientId == patientId);
    }

    public async Task<History> Update(History history)
    {
        return await _historyUnitOfWork.Update(history);
    }

    public async Task<List<History>> GetAll(bool includeInactive)
    {
        return await _historyUnitOfWork.GetHistories(x => includeInactive);
    }
}