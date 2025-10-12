using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHaModelRepository
{
    Task<HaModel> Add(HaModel haModel);
    void Delete(int id);
    Task<HaModel> Get(int id);
    Task<List<HaModel>> GetAll(bool includeInactive);
    Task<HaModel> Update(HaModel haModel);
}

public class HaModelRepository : IHaModelRepository
{
    private readonly IHaModelUnitOfWork _haModelUnitOfWork;

    public HaModelRepository(IHaModelUnitOfWork haModelUnitOfWork)
    {
        _haModelUnitOfWork = haModelUnitOfWork;
    }

    public async Task<HaModel> Add(HaModel haModel)
    {
        return await _haModelUnitOfWork.Add(haModel);
    }

    public void Delete(int id)
    {
        _haModelUnitOfWork.Delete(id);
    }

    public async Task<HaModel> Get(int id)
    {
        return await _haModelUnitOfWork.GetHaModel(id);
    }

    public async Task<List<HaModel>> GetAll(bool includeInactive)
    {
        return await _haModelUnitOfWork.GetHaModels(x => includeInactive || !x.Inactive);
    }

    public async Task<HaModel> Update(HaModel haModel)
    {
        return await _haModelUnitOfWork.Update(haModel);
    }
}