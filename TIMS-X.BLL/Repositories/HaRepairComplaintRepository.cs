using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHaRepairComplaintRepository
{
    Task<HaRepairComplaint> Add(HaRepairComplaint repairComplaint);
    void Delete(int id);
    Task<HaRepairComplaint> Get(int id);
    Task<List<HaRepairComplaint>> GetAll(bool includeInactive);
    Task<HaRepairComplaint> Update(HaRepairComplaint repairComplaint);
}

public class HaRepairComplaintRepository : IHaRepairComplaintRepository
{
    private readonly IHaRepairComplaintUnitOfWork _haRepairComplaintUnitOfWork;

    public HaRepairComplaintRepository(IHaRepairComplaintUnitOfWork haHaRepairComplaintUnitOfWork)
    {
        _haRepairComplaintUnitOfWork = haHaRepairComplaintUnitOfWork;
    }

    public async Task<HaRepairComplaint> Add(HaRepairComplaint repairComplaint)
    {
        return await _haRepairComplaintUnitOfWork.Add(repairComplaint);
    }

    public void Delete(int id)
    {
        _haRepairComplaintUnitOfWork.Delete(id);
    }

    public async Task<HaRepairComplaint> Get(int id)
    {
        return await _haRepairComplaintUnitOfWork.GetHaRepairComplaint(id);
    }

    public async Task<List<HaRepairComplaint>> GetAll(bool includeInactive)
    {
        return await _haRepairComplaintUnitOfWork.GetHaRepairComplaints(x => includeInactive || !x.Inactive);
    }

    public async Task<HaRepairComplaint> Update(HaRepairComplaint haRepairComplaint)
    {
        return await _haRepairComplaintUnitOfWork.Update(haRepairComplaint);
    }
}