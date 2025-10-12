using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHaWarrantyTypeRepository
{
    Task<HaWarrantyType> Add(HaWarrantyType haWarrantyType);
    void Delete(int id);
    Task<HaWarrantyType> Get(int id);
    Task<List<HaWarrantyType>> GetAll(bool includeInactive);
    Task<HaWarrantyType> Update(HaWarrantyType haWarrantyType);
}

public class HaWarrantyTypeRepository : IHaWarrantyTypeRepository
{
    private readonly IHaWarrantyTypeUnitOfWork _haWarrantyTypeUnitOfWork;

    public HaWarrantyTypeRepository(IHaWarrantyTypeUnitOfWork haWarrantyTypeUnitOfWork)
    {
        _haWarrantyTypeUnitOfWork = haWarrantyTypeUnitOfWork;
    }

    public async Task<HaWarrantyType> Add(HaWarrantyType haWarrantyType)
    {
        return await _haWarrantyTypeUnitOfWork.Add(haWarrantyType);
    }

    public void Delete(int id)
    {
        _haWarrantyTypeUnitOfWork.Delete(id);
    }

    public async Task<HaWarrantyType> Get(int id)
    {
        return await _haWarrantyTypeUnitOfWork.GetHaWarrantyType(id);
    }

    public async Task<List<HaWarrantyType>> GetAll(bool includeInactive)
    {
        return await _haWarrantyTypeUnitOfWork.GetHaWarrantyTypes(x => includeInactive || !x.Inactive);
    }

    public async Task<HaWarrantyType> Update(HaWarrantyType haWarrantyType)
    {
        return await _haWarrantyTypeUnitOfWork.Update(haWarrantyType);
    }
}