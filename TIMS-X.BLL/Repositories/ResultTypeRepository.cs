using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IResultTypeRepository
{
    Task<ResultType> Add(ResultType resultType);
    void Delete(int id);
    Task<ResultType> Get(int id);
    Task<List<ResultType>> GetAll(bool includeInactive);
    Task<ResultType> Update(ResultType resultType);
}

public class ResultTypeRepository : IResultTypeRepository
{
    private readonly IResultTypeUnitOfWork _resultTypeUnitOfWork;

    public ResultTypeRepository(IResultTypeUnitOfWork resultTypeUnitOfWork)
    {
        _resultTypeUnitOfWork = resultTypeUnitOfWork;
    }

    public async Task<ResultType> Add(ResultType resultType)
    {
        return await _resultTypeUnitOfWork.Add(resultType);
    }

    public async Task<ResultType> Get(int id)
    {
        return await _resultTypeUnitOfWork.GetResultType(id);
    }

    public async Task<List<ResultType>> GetAll(bool includeInactive)
    {
        return await _resultTypeUnitOfWork.GetResultTypes(x => includeInactive || !x.Inactive);
    }

    public async Task<ResultType> Update(ResultType resultType)
    {
        return await _resultTypeUnitOfWork.Update(resultType);
    }

    public void Delete(int id)
    {
        _resultTypeUnitOfWork.Delete(id);
    }
}