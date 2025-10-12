using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHaStyleRepository
{
    Task<HaStyle> Add(HaStyle haStyle);
    void Delete(int id);
    Task<HaStyle> Get(int id);
    Task<List<HaStyle>> GetAll(bool includeInactive);
    Task<HaStyle> Update(HaStyle haStyle);
}

public class HaStyleRepository : IHaStyleRepository
{
    private readonly IHaStyleUnitOfWork _haStyleUnitOfWork;

    public HaStyleRepository(IHaStyleUnitOfWork haHaStyleUnitOfWork)
    {
        _haStyleUnitOfWork = haHaStyleUnitOfWork;
    }

    public async Task<HaStyle> Add(HaStyle haStyle)
    {
        return await _haStyleUnitOfWork.Add(haStyle);
    }

    public void Delete(int id)
    {
        _haStyleUnitOfWork.Delete(id);
    }

    public async Task<HaStyle> Get(int id)
    {
        return await _haStyleUnitOfWork.GetHaStyle(id);
    }

    public async Task<List<HaStyle>> GetAll(bool includeInactive)
    {
        return await _haStyleUnitOfWork.GetHaStyles(x => includeInactive || !x.Inactive);
    }

    public async Task<HaStyle> Update(HaStyle haStyle)
    {
        return await _haStyleUnitOfWork.Update(haStyle);
    }
}