using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IHaReturnReasonRepository
{
    Task<HaReturnReason> Add(HaReturnReason haReturnReason);
    void Delete(int id);
    Task<HaReturnReason> Get(int id);
    Task<List<HaReturnReason>> GetAll(bool includeInactive);
    Task<HaReturnReason> Update(HaReturnReason haReturnReason);
}

public class HaReturnReasonRepository : IHaReturnReasonRepository
{
    private readonly IHaReturnReasonUnitOfWork _haReturnReasonUnitOfWork;

    public HaReturnReasonRepository(IHaReturnReasonUnitOfWork haReturnReasonUnitOfWork)
    {
        _haReturnReasonUnitOfWork = haReturnReasonUnitOfWork;
    }

    public async Task<HaReturnReason> Add(HaReturnReason haReturnReason)
    {
        return await _haReturnReasonUnitOfWork.Add(haReturnReason);
    }

    public void Delete(int id)
    {
        _haReturnReasonUnitOfWork.Delete(id);
    }

    public async Task<HaReturnReason> Get(int id)
    {
        return await _haReturnReasonUnitOfWork.GetReturnReason(id);
    }

    public async Task<List<HaReturnReason>> GetAll(bool includeInactive)
    {
        return await _haReturnReasonUnitOfWork.GetReturnReasons(x => includeInactive || !x.Inactive);
    }

    public async Task<HaReturnReason> Update(HaReturnReason haReturnReason)
    {
        return await _haReturnReasonUnitOfWork.Update(haReturnReason);
    }
}