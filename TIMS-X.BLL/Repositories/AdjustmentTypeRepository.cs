using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IAdjustmentTypeRepository
{
    Task<AdjustmentType> Add(AdjustmentType adjustmentType);
    void Delete(int id);
    Task<AdjustmentType> Get(int id);
    Task<List<AdjustmentType>> GetAll(bool includeInactive);
    Task<AdjustmentType> Update(AdjustmentType adjustmentType);
}

public class AdjustmentTypeRepository : IAdjustmentTypeRepository
{
    private readonly IAdjustmentTypeUnitOfWork _adjustmentTypeUnitOfWork;

    public AdjustmentTypeRepository(IAdjustmentTypeUnitOfWork adjustmentTypeUnitOfWork)
    {
        _adjustmentTypeUnitOfWork = adjustmentTypeUnitOfWork;
    }

    public async Task<AdjustmentType> Add(AdjustmentType adjustmentType)
    {
        return await _adjustmentTypeUnitOfWork.Add(adjustmentType);
    }

    public void Delete(int id)
    {
        _adjustmentTypeUnitOfWork.Delete(id);
    }

    public async Task<AdjustmentType> Get(int id)
    {
        return await _adjustmentTypeUnitOfWork.GetAdjustmentType(id);
    }

    public async Task<List<AdjustmentType>> GetAll(bool includeInactive)
    {
        return await _adjustmentTypeUnitOfWork.GetAdjustmentTypes(x => includeInactive || !x.Inactive);
    }

    public async Task<AdjustmentType> Update(AdjustmentType adjustmentType)
    {
        return await _adjustmentTypeUnitOfWork.Update(adjustmentType);
    }
}