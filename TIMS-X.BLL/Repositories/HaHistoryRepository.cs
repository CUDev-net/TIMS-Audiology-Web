using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHaHistoryRepository
{
    Task<HaHistory> Add(HaHistory site);
    void Delete(int id);
    Task<HaHistory> Get(int id);
    Task<List<HaHistory>> GetAll(bool includeInactive);
    Task<List<HaHistory>> GetAllByPatientId(int patientId);
    Task<HaHistory> Update(HaHistory haHistory);
}

public class HaHistoryRepository : IHaHistoryRepository
{
    private readonly IHaHistoryUnitOfWork _haHistoryUnitOfWork;

    public HaHistoryRepository(IHaHistoryUnitOfWork haHistoryUnitOfWork)
    {
        _haHistoryUnitOfWork = haHistoryUnitOfWork;
    }

    public async Task<HaHistory> Add(HaHistory haHistory)
    {
        return await _haHistoryUnitOfWork.Add(haHistory);
    }

    public async Task<HaHistory> Get(int id)
    {
        return await _haHistoryUnitOfWork.GetHaHistory(id);
    }

    public async Task<List<HaHistory>> GetAll(bool includeInactive)
    {
        return await _haHistoryUnitOfWork.GetHaHistories(x => includeInactive);
    }

    public async Task<List<HaHistory>> GetAllByPatientId(int patientId)
    {
        return await _haHistoryUnitOfWork.GetHaHistories(x =>
            x.PatientId == patientId);
    }

    public async Task<HaHistory> Update(HaHistory haHistory)
    {
        return await _haHistoryUnitOfWork.Update(haHistory);
    }

    public void Delete(int id)
    {
        _haHistoryUnitOfWork.Delete(id);
    }
}