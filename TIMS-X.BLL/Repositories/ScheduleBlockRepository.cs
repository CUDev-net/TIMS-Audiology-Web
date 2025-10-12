using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.BLL.Utilities;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IScheduleBlockRepository
{
    Task<ScheduleBlock> Add(ScheduleBlock scheduleBlock);
    void Delete(int id);
    Task<ScheduleBlock> Get(int id);
    Task<List<ScheduleBlock>> GetAll();
    Task<ScheduleBlock> Update(ScheduleBlock scheduleBlock);
}

public class ScheduleBlockRepository : IScheduleBlockRepository
{
    private readonly IAppointmentTypeUnitOfWork _appointmentTypeUnitOfWork;
    private readonly IScheduleBlockUnitOfWork _scheduleBlockUnitOfWork;

    public ScheduleBlockRepository(IScheduleBlockUnitOfWork scheduleBlockUnitOfWork,
        IAppointmentTypeUnitOfWork appointmentTypeUnitOfWork)
    {
        _scheduleBlockUnitOfWork = scheduleBlockUnitOfWork;
        _appointmentTypeUnitOfWork = appointmentTypeUnitOfWork;
    }

    public async Task<ScheduleBlock> Add(ScheduleBlock scheduleBlock)
    {
        return await _scheduleBlockUnitOfWork.Add(scheduleBlock);
    }

    public async Task<ScheduleBlock> Get(int id)
    {
        return await _scheduleBlockUnitOfWork.GetScheduleBlock(id);
    }

    public async Task<List<ScheduleBlock>> GetAll()
    {
        var blocks = await _scheduleBlockUnitOfWork
            .GetScheduleBlocks(x => x.Inactive == false,
                i => i.Include(x => x.ProviderBlockSchedules).ThenInclude(x => x.ScheduleTimeSlot));
        var returnedBlocks = new List<ScheduleBlock>();
        foreach (var scheduleBlock in blocks)
            if ((!scheduleBlock.StartDate.HasValue || scheduleBlock.StartDate.Value <= DateTime.Now.Date) &&
                (!scheduleBlock.EndDate.HasValue || scheduleBlock.EndDate.Value > DateTime.Now.Date))
            {
                _HydrateMetaData(scheduleBlock);
                var appointmentTypes = await
                    _appointmentTypeUnitOfWork.GetAppointmentTypes(x => x.ScheduleBlockId == scheduleBlock.Id);
                if (appointmentTypes.Any())
                    foreach (var appointmentType in appointmentTypes.OrderBy(a => a.Name))
                        scheduleBlock.AppointmentTypes.Add(appointmentType.Name);
                // Hack so EF/ASP.net doesn't see a circular reference
                foreach (var scheduleBlockProviderBlockSchedule in scheduleBlock.ProviderBlockSchedules)
                    scheduleBlockProviderBlockSchedule.ScheduleBlock = null;
                returnedBlocks.Add(scheduleBlock);
            }

        return returnedBlocks;
    }

    public async Task<ScheduleBlock> Update(ScheduleBlock scheduleBlock)
    {
        return await _scheduleBlockUnitOfWork.Update(scheduleBlock);
    }

    public void Delete(int id)
    {
        _scheduleBlockUnitOfWork.Delete(id);
    }

    private void _HydrateMetaData(ScheduleBlock item)
    {
        item.color_web = ColorHelper.GetHexColor(item.Color);
    }
}