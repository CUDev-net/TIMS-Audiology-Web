using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IBatterySizeRepository
{
    Task<BatterySize> Add(BatterySize batterySize);
    void Delete(int id);
    Task<BatterySize> Get(int id);
    Task<List<BatterySize>> GetAll(bool includeInactive);
    Task<BatterySize> Update(BatterySize batterySize);
}

public class BatterySizeRepository : IBatterySizeRepository
{
    private readonly IBatterySizeUnitOfWork _batterySizeUnitOfWork;

    public BatterySizeRepository(IBatterySizeUnitOfWork batterySizeUnitOfWork)
    {
        _batterySizeUnitOfWork = batterySizeUnitOfWork;
    }

    public async Task<BatterySize> Add(BatterySize batterySize)
    {
        return await _batterySizeUnitOfWork.Add(batterySize);
    }

    public void Delete(int id)
    {
        _batterySizeUnitOfWork.Delete(id);
    }

    public async Task<BatterySize> Get(int id)
    {
        return await _batterySizeUnitOfWork.GetBatterySize(id);
    }

    public async Task<List<BatterySize>> GetAll(bool includeInactive)
    {
        return await _batterySizeUnitOfWork.GetBatterySizes(x => includeInactive || !x.Inactive);
    }

    public async Task<BatterySize> Update(BatterySize batterySize)
    {
        return await _batterySizeUnitOfWork.Update(batterySize);
    }
}