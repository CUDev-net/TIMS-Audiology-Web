using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ISexRepository
{
    Task<Sex> Add(Sex sex);
    void Delete(int id);
    Task<Sex> Get(int id);
    Task<List<Sex>> GetAll(bool includeInactive);
    Task<Sex> Update(Sex sex);
}

public class SexRepository : ISexRepository
{
    private readonly ISexUnitOfWork _sexUnitOfWork;

    public SexRepository(ISexUnitOfWork sexUnitOfWork)
    {
        _sexUnitOfWork = sexUnitOfWork;
    }

    public async Task<Sex> Add(Sex sex)
    {
        return await _sexUnitOfWork.Add(sex);
    }

    public async Task<Sex> Get(int id)
    {
        return await _sexUnitOfWork.GetSex(id);
    }

    public async Task<List<Sex>> GetAll(bool includeInactive)
    {
        return await _sexUnitOfWork.GetSexes(x => includeInactive || !x.Inactive);
    }

    public async Task<Sex> Update(Sex sex)
    {
        return await _sexUnitOfWork.Update(sex);
    }

    public void Delete(int id)
    {
        _sexUnitOfWork.Delete(id);
    }
}