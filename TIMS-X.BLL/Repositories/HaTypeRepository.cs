using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHaTypeRepository
{
    Task<HaType> Add(HaType haType);
    void Delete(int id);
    Task<HaType> Get(int id);
    Task<List<HaType>> GetAll(bool includeInactive);
    Task<HaType> Update(HaType haType);
}

public class HaTypeRepository : IHaTypeRepository
{
    private readonly IHaTypeUnitOfWork _haTypeUnitOfWork;

    public HaTypeRepository(IHaTypeUnitOfWork haTypeUnitOfWork)
    {
        _haTypeUnitOfWork = haTypeUnitOfWork;
    }

    public async Task<HaType> Add(HaType haType)
    {
        return await _haTypeUnitOfWork.Add(haType);
    }

    public void Delete(int id)
    {
        _haTypeUnitOfWork.Delete(id);
    }

    public async Task<HaType> Get(int id)
    {
        return await _haTypeUnitOfWork.GetHaType(id);
    }

    public async Task<List<HaType>> GetAll(bool includeInactive)
    {
        return await _haTypeUnitOfWork.GetHaTypes(x => includeInactive || !x.Inactive);
    }

    public async Task<HaType> Update(HaType haType)
    {
        return await _haTypeUnitOfWork.Update(haType);
    }
}