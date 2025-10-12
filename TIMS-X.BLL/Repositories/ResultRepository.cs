using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IResultRepository
{
    Task<Result> Add(Result result);
    void Delete(int id);
    Task<Result> Get(int id);
    Task<List<Result>> GetAll(bool includeInactive);
    Task<Result> Update(Result result);
}

public class ResultRepository : IResultRepository
{
    private readonly IResultUnitOfWork _resultUnitOfWork;

    public ResultRepository(IResultUnitOfWork resultUnitOfWork)
    {
        _resultUnitOfWork = resultUnitOfWork;
    }

    public async Task<Result> Add(Result result)
    {
        return await _resultUnitOfWork.Add(result);
    }

    public void Delete(int id)
    {
        _resultUnitOfWork.Delete(id);
    }

    public async Task<Result> Get(int id)
    {
        return await _resultUnitOfWork.GetResult(id);
    }

    public async Task<List<Result>> GetAll(bool includeInactive)
    {
        return await _resultUnitOfWork.GetResults(x => includeInactive || !x.Inactive);
    }

    public async Task<Result> Update(Result result)
    {
        return await _resultUnitOfWork.Update(result);
    }
}