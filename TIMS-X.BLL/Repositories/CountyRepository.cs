using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ICountyRepository
{
    Task<County> Add(County county);
    void Delete(int id);
    Task<County> Get(int id);
    Task<List<County>> GetAll(bool includeInactive);
    Task<County> Update(County county);
}

public class CountyRepository : ICountyRepository
{
    private readonly ICountyUnitOfWork _countyUnitOfWork;

    public CountyRepository(ICountyUnitOfWork countyUnitOfWork)
    {
        _countyUnitOfWork = countyUnitOfWork;
    }

    public async Task<County> Add(County county)
    {
        return await _countyUnitOfWork.Add(county);
    }

    public async Task<County> Get(int id)
    {
        return await _countyUnitOfWork.GetCounty(id);
    }

    public async Task<List<County>> GetAll(bool includeInactive)
    {
        return await _countyUnitOfWork.GetCounties(x => includeInactive || !x.Inactive);
    }

    public async Task<County> Update(County county)
    {
        return await _countyUnitOfWork.Update(county);
    }

    public void Delete(int id)
    {
        _countyUnitOfWork.Delete(id);
    }
}