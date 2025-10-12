using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IOutsideFacilityRepository
{
    Task<OutsideFacility> Add(OutsideFacility outsideFacility);
    void Delete(int id);
    Task<OutsideFacility> Get(int id);
    Task<List<OutsideFacility>> GetAll(bool includeInactive);
    Task<OutsideFacility> Update(OutsideFacility outsideFacility);
}

public class OutsideFacilityRepository : IOutsideFacilityRepository
{
    private readonly IOutsideFacilityUnitOfWork _outsideFacilityUnitOfWork;

    public OutsideFacilityRepository(IOutsideFacilityUnitOfWork outsideFacilityUnitOfWork)
    {
        _outsideFacilityUnitOfWork = outsideFacilityUnitOfWork;
    }

    public async Task<OutsideFacility> Add(OutsideFacility outsideFacility)
    {
        return await _outsideFacilityUnitOfWork.Add(outsideFacility);
    }

    public void Delete(int id)
    {
        _outsideFacilityUnitOfWork.Delete(id);
    }

    public async Task<OutsideFacility> Get(int id)
    {
        return await _outsideFacilityUnitOfWork.GetOutsideFacility(id);
    }

    public async Task<List<OutsideFacility>> GetAll(bool includeInactive)
    {
        return await _outsideFacilityUnitOfWork.GetOutsideFacilities(x => includeInactive || !x.Inactive);
    }

    public async Task<OutsideFacility> Update(OutsideFacility outsideFacility)
    {
        return await _outsideFacilityUnitOfWork.Update(outsideFacility);
    }
}