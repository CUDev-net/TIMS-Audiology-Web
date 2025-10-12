using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHaModelOptionRepository
{
    Task<HaModelOption> Add(HaModelOption haModelOption);
    void Delete(int id);
    Task<HaModelOption> Get(int id);
    Task<List<HaModelOption>> GetAll(bool includeInactive);
    Task<HaModelOption> Update(HaModelOption haModelOption);
}

public class HaModelOptionRepository : IHaModelOptionRepository
{
    private readonly IHaModelOptionUnitOfWork _haModelOptionUnitOfWork;

    public HaModelOptionRepository(IHaModelOptionUnitOfWork haModelOptionUnitOfWork)
    {
        _haModelOptionUnitOfWork = haModelOptionUnitOfWork;
    }

    public async Task<HaModelOption> Add(HaModelOption haModelOption)
    {
        return await _haModelOptionUnitOfWork.Add(haModelOption);
    }

    public void Delete(int id)
    {
        _haModelOptionUnitOfWork.Delete(id);
    }

    public async Task<HaModelOption> Get(int id)
    {
        return await _haModelOptionUnitOfWork.GetHaModelOption(id);
    }

    public async Task<List<HaModelOption>> GetAll(bool includeInactive)
    {
        return await _haModelOptionUnitOfWork.GetHaModelOptions().ToListAsync();
    }

    public async Task<HaModelOption> Update(HaModelOption haModelOption)
    {
        return await _haModelOptionUnitOfWork.Update(haModelOption);
    }
}