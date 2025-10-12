using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ISubmitterInfoRepository
{
    Task<SubmitterInfo> Add(SubmitterInfo submitterInfo);
    void Delete(int id);
    Task<SubmitterInfo> Get(int id);
    Task<List<SubmitterInfo>> GetAll(bool includeInactive);
    Task<SubmitterInfo> Update(SubmitterInfo submitterInfo);
}

public class SubmitterInfoRepository : ISubmitterInfoRepository
{
    private readonly ISubmitterInfoUnitOfWork _submitterInfoUnitOfWork;

    public SubmitterInfoRepository(ISubmitterInfoUnitOfWork submitterInfoUnitOfWork)
    {
        _submitterInfoUnitOfWork = submitterInfoUnitOfWork;
    }

    public async Task<SubmitterInfo> Add(SubmitterInfo submitterInfo)
    {
        return await _submitterInfoUnitOfWork.Add(submitterInfo);
    }

    public void Delete(int id)
    {
        _submitterInfoUnitOfWork.Delete(id);
    }

    public async Task<SubmitterInfo> Get(int id)
    {
        return await _submitterInfoUnitOfWork.GetSubmitterInfo(id);
    }

    public async Task<List<SubmitterInfo>> GetAll(bool includeInactive)
    {
        return await _submitterInfoUnitOfWork.GetSubmitterInfos(x => includeInactive || !x.Inactive);
    }

    public async Task<SubmitterInfo> Update(SubmitterInfo submitterInfo)
    {
        return await _submitterInfoUnitOfWork.Update(submitterInfo);
    }
}