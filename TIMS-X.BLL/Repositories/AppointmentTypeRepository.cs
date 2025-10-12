using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IAppointmentTypeRepository
{
    Task<AppointmentType> Add(AppointmentType appointmentType);
    void Delete(int id);
    Task<AppointmentType> Get(int id);
    Task<List<AppointmentType>> GetAll(bool includeInactive);
    Task<List<AppointmentType>> GetAllWithScheduleBlocks(bool includeInactive);
    Task<AppointmentType> Update(AppointmentType appointmentType);
}

public class AppointmentTypeRepository : IAppointmentTypeRepository
{
    private readonly IAppointmentTypeUnitOfWork _appointmentTypeUnitOfWork;

    public AppointmentTypeRepository(IAppointmentTypeUnitOfWork appointmentTypeUnitOfWork)
    {
        _appointmentTypeUnitOfWork = appointmentTypeUnitOfWork;
    }

    public async Task<AppointmentType> Add(AppointmentType appointmentType)
    {
        return await _appointmentTypeUnitOfWork.Add(appointmentType);
    }

    public async Task<AppointmentType> Get(int id)
    {
        return await _appointmentTypeUnitOfWork.GetAppointmentType(id);
    }

    public async Task<List<AppointmentType>> GetAll(bool includeInactive)
    {
        return await _appointmentTypeUnitOfWork.GetAppointmentTypes(x => includeInactive || !x.Inactive);
    }

    public async Task<List<AppointmentType>> GetAllWithScheduleBlocks(bool includeInactive)
    {
        var appointmentTypes = await _appointmentTypeUnitOfWork.GetAppointmentTypes(
            x => includeInactive || !x.Inactive,
            null,
            a => a.Include(b => b.ScheduleBlock)
                .ThenInclude(c => c.ProviderBlockSchedules)
                .ThenInclude(t => t.ScheduleTimeSlot)
        );

        return appointmentTypes;
    }

    public async Task<AppointmentType> Update(AppointmentType appointmentType)
    {
        return await _appointmentTypeUnitOfWork.Update(appointmentType);
    }

    public void Delete(int id)
    {
        _appointmentTypeUnitOfWork.Delete(id);
    }
}