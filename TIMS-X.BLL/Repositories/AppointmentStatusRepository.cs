using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IAppointmentStatusRepository
{
    Task<AppointmentStatus> Add(AppointmentStatus site);
    void Delete(int id);
    Task<AppointmentStatus> Get(int id);
    Task<List<AppointmentStatus>> GetAll(bool includeInactive);
    Task<AppointmentStatus> Update(AppointmentStatus appointmentStatus);
}

public class AppointmentStatusRepository : IAppointmentStatusRepository
{
    private readonly IAppointmentStatusUnitOfWork _appointmentStatusUnitOfWork;

    public AppointmentStatusRepository(IAppointmentStatusUnitOfWork appointmentStatusUnitOfWork)
    {
        _appointmentStatusUnitOfWork = appointmentStatusUnitOfWork;
    }

    public async Task<AppointmentStatus> Add(AppointmentStatus appointmentStatus)
    {
        return await _appointmentStatusUnitOfWork.Add(appointmentStatus);
    }

    public async Task<AppointmentStatus> Get(int id)
    {
        return await _appointmentStatusUnitOfWork.GetAppointmentStatus(id);
    }

    public async Task<List<AppointmentStatus>> GetAll(bool includeInactive)
    {
        return await _appointmentStatusUnitOfWork.GetAppointmentStatuses(x => includeInactive || !x.Inactive);
    }

    public async Task<AppointmentStatus> Update(AppointmentStatus appointmentStatus)
    {
        return await _appointmentStatusUnitOfWork.Update(appointmentStatus);
    }

    public void Delete(int id)
    {
        _appointmentStatusUnitOfWork.Delete(id);
    }
}