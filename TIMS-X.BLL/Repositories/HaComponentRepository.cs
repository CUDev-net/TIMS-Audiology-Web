using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHaComponentRepository
{
    Task<HaComponent> Add(HaComponent haComponent);
    void Delete(int id);
    Task<HaComponent> Get(int id);
    Task<List<HaComponent>> GetAll(bool includeInactive);
    Task<HaComponent> Update(HaComponent haComponent);
}

public class HaComponentRepository : IHaComponentRepository
{
    private readonly IHaComponentUnitOfWork _haComponentUnitOfWork;

    public HaComponentRepository(IHaComponentUnitOfWork haComponentUnitOfWork)
    {
        _haComponentUnitOfWork = haComponentUnitOfWork;
    }

    public async Task<HaComponent> Add(HaComponent haComponent)
    {
        return await _haComponentUnitOfWork.Add(haComponent);
    }

    public void Delete(int id)
    {
        _haComponentUnitOfWork.Delete(id);
    }

    public async Task<HaComponent> Get(int id)
    {
        return await _haComponentUnitOfWork.GetHaComponent(id);
    }

    public async Task<List<HaComponent>> GetAll(bool includeInactive)
    {
        return await _haComponentUnitOfWork.GetHaComponents(x => includeInactive || !x.Inactive);
    }

    public async Task<HaComponent> Update(HaComponent haComponent)
    {
        return await _haComponentUnitOfWork.Update(haComponent);
    }
}