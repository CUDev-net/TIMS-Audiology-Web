using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IRepairComplaintRepository
{
    Task<RepairComplaint> Add(RepairComplaint repairComplaint);
    void Delete(int id);
    Task<RepairComplaint> Get(int id);
    Task<List<RepairComplaint>> GetAll(bool includeInactive);
    Task<RepairComplaint> Update(RepairComplaint repairComplaint);
}

public class RepairComplaintRepository : IRepairComplaintRepository
{
    private readonly IRepairComplaintUnitOfWork _repairComplaintUnitOfWork;

    public RepairComplaintRepository(IRepairComplaintUnitOfWork repairComplaintUnitOfWork)
    {
        _repairComplaintUnitOfWork = repairComplaintUnitOfWork;
    }

    public async Task<RepairComplaint> Add(RepairComplaint repairComplaint)
    {
        return await _repairComplaintUnitOfWork.Add(repairComplaint);
    }

    public void Delete(int id)
    {
        _repairComplaintUnitOfWork.Delete(id);
    }

    public async Task<RepairComplaint> Get(int id)
    {
        return await _repairComplaintUnitOfWork.GetRepairComplaint(id);
    }

    public async Task<List<RepairComplaint>> GetAll(bool includeInactive)
    {
        return await _repairComplaintUnitOfWork.GetRepairComplaints(x => includeInactive || !x.Inactive);
    }

    public async Task<RepairComplaint> Update(RepairComplaint repairComplaint)
    {
        return await _repairComplaintUnitOfWork.Update(repairComplaint);
    }
}