using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ICptCodeCategoryRepository
{
    Task<CptCodeCategory> Add(CptCodeCategory cptCodeCategory);
    void Delete(int id);
    Task<CptCodeCategory> Get(int id);
    Task<List<CptCodeCategory>> GetAll(bool includeInactive);
    Task<CptCodeCategory> Update(CptCodeCategory cptCodeCategory);
}

public class CptCodeCategoryRepository : ICptCodeCategoryRepository
{
    private readonly ICptCodeCategoryUnitOfWork _cptCodeCategoryUnitOfWork;

    public CptCodeCategoryRepository(ICptCodeCategoryUnitOfWork cptCodeCategoryUnitOfWork)
    {
        _cptCodeCategoryUnitOfWork = cptCodeCategoryUnitOfWork;
    }

    public async Task<CptCodeCategory> Add(CptCodeCategory cptCodeCategory)
    {
        return await _cptCodeCategoryUnitOfWork.Add(cptCodeCategory);
    }

    public void Delete(int id)
    {
        _cptCodeCategoryUnitOfWork.Delete(id);
    }

    public async Task<CptCodeCategory> Get(int id)
    {
        return await _cptCodeCategoryUnitOfWork.GetCptCodeCategory(id);
    }

    public async Task<List<CptCodeCategory>> GetAll(bool includeInactive)
    {
        return await _cptCodeCategoryUnitOfWork.GetCptCodeCategories(x => includeInactive || !x.Inactive);
    }

    public async Task<CptCodeCategory> Update(CptCodeCategory cptCodeCategory)
    {
        return await _cptCodeCategoryUnitOfWork.Update(cptCodeCategory);
    }
}