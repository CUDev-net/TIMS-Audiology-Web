using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.BLL.VendorSync.Common;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;
using Appointment = TIMS_X.BLL.VendorSync.Audigy.Appointment;
using TIMSAppointment = TIMS_X.Core.Domain.Appointment;

namespace TIMS_X.BLL.VendorSync.Repositories;

public interface IVendorAppointmentRepository
{
    Task<Appointment> GetAppointment(int id);
    Task<List<Appointment>> GetAppointments(DateTime? fromDate, DateTime? toDate);
}

public class VendorAppointmentRepository : IVendorAppointmentRepository
{
    private readonly IAppointmentsUnitOfWork _appointmentsUnitOfWork;
    private readonly IHaHistoryUnitOfWork _haHistoryUnitOfWork;
    private readonly IHaModelUnitOfWork _haModelUnitOfWork;
    private readonly IPracticeUnitOfWork _practiceUnitOfWork;

    public VendorAppointmentRepository(IAppointmentsUnitOfWork appointmentsUnitOfWork,
        IPracticeUnitOfWork practiceUnitOfWork, IHaModelUnitOfWork haModelUnitOfWork,
        IHaHistoryUnitOfWork haHistoryUnitOfWork)
    {
        _appointmentsUnitOfWork = appointmentsUnitOfWork;
        _practiceUnitOfWork = practiceUnitOfWork;
        _haModelUnitOfWork = haModelUnitOfWork;
        _haHistoryUnitOfWork = haHistoryUnitOfWork;
    }

    public async Task<Appointment> GetAppointment(int id)
    {
        var practiceId = await _practiceUnitOfWork.GetPracticeAudigyId();
        var timsAppointment = await _appointmentsUnitOfWork.GetAppointment(id, _Includes);

        if (timsAppointment == null) return null;

        return await _MapAppointmentToAppointmentAsync(timsAppointment, practiceId);
    }

    public async Task<List<Appointment>> GetAppointments(DateTime? fromDate, DateTime? toDate)
    {
        var practiceId = await _practiceUnitOfWork.GetPracticeAudigyId();

        IQueryable<TIMSAppointment> rawItems;
        if (fromDate.HasValue && toDate.HasValue)
            rawItems = _appointmentsUnitOfWork.GetAppointments(
                a => a.UpdatedDate >= fromDate && a.UpdatedDate <= toDate, null, _Includes);
        else if (fromDate.HasValue)
            rawItems = _appointmentsUnitOfWork.GetAppointments(a => a.UpdatedDate >= fromDate, null, _Includes);
        else if (toDate.HasValue)
            rawItems = _appointmentsUnitOfWork.GetAppointments(a => a.UpdatedDate <= toDate, null, _Includes);
        else
            rawItems = _appointmentsUnitOfWork.GetAppointments(null, null, _Includes);
        var queryResult = await rawItems.ToListAsync();
        var appointmentList = new List<Appointment>(queryResult.Count);
        foreach (var appointment in queryResult)
        {
            var appt = await _MapAppointmentToAppointmentAsync(appointment, practiceId);
            appointmentList.Add(appt);
        }
        return appointmentList;
    }

    private IIncludableQueryable<TIMSAppointment, object> _Includes(IQueryable<TIMSAppointment> a)
    {
        return a.Include(x => x.AppointmentType)
            .Include(x => x.AppointmentStatus)
            .Include(o => o.OtStatusDescription)
            .Include(p => p.PosDocuments).ThenInclude(l => l.PosLines);
    }

    private IIncludableQueryable<HaModel, object> _Includes(IQueryable<HaModel> h)
    {
        return h.Include(x => x.Style)
            .Include(x => x.HaType);
    }

    private IIncludableQueryable<HaHistory, object> _Includes(IQueryable<HaHistory> h)
    {
        return h.Include(x => x.Status);
    }

    private async Task<Appointment> _MapAppointmentToAppointmentAsync(TIMSAppointment timsAppointment,
        string practiceId)
    {
        decimal earsFit = 0;
        var technologySold = string.Empty;
        var totalAmount = 0m;
        var userType = string.Empty;
        var invoices = timsAppointment.PosDocuments.ToList();
        foreach (var posDocument in invoices)
        {
            var lines = posDocument.PosLines.Where(x => x.TableID == 170);
            foreach (var posLineItem in lines)
            {
                earsFit += posLineItem.Qty;
                totalAmount += posLineItem.Amount;
                if (string.IsNullOrEmpty(technologySold))
                {
                    var haModel = await _haModelUnitOfWork.GetHaModel(posLineItem.ItemID, _Includes);
                    if (haModel != null)
                    {
                        var sb = new StringBuilder();
                        sb.Append(haModel.Name + " ");
                        if (haModel.Style != null) sb.Append(haModel.Style.Name + " ");
                        if (haModel.HaType != null) sb.Append(haModel.HaType.Name);
                        technologySold = sb.ToString().TrimEnd();
                    }
                }
            }
        }

        //New User, Existing 1-4 years, Existing 4+ years
        var haHistories = await _haHistoryUnitOfWork.GetHaHistories(h => h.PatientId == timsAppointment.PatientId &&
                                                                             h.Status != null &&
                                                                             h.Status.Description ==
                                                                             Constants.AUDIGY_IN_USE &&
                                                                             h.PurchaseDate.HasValue,
                h => h.OrderBy(p => p.PurchaseDate), _Includes);
        var oldestHistory = haHistories.FirstOrDefault();
        if (oldestHistory is { PurchaseDate: not null })
        {
            var ageSpan = DateTime.Now - oldestHistory.PurchaseDate.Value;
            if (ageSpan.Days < 365)
                userType = "New User";
            else if (ageSpan.Days <= 1461)
                userType = "Existing 1-4 years";
            else
                userType = "Existing 4+ years";
        }


        var thirdParty = timsAppointment.AppointmentStatus.Name.ToLower() == "attended companion";

        var audigyAppointment = new Appointment
        {
            AppointmentID = timsAppointment.Id,
            PracticeID = practiceId,
            PatientID = timsAppointment.PatientId,
            DateTime = timsAppointment.StartsAt,
            Length = (int)(timsAppointment.EndsAt - timsAppointment.StartsAt).TotalMinutes,
            AudiologistID = timsAppointment.ProviderId,
            AppointmentType = timsAppointment.AppointmentType?.Name,
            AppointmentStatus = timsAppointment.AppointmentStatus?.Name,
            DateCreated = timsAppointment.CreatedDate,
            EarsFit = (int)earsFit,
            TotalAmount = totalAmount,
            TechnologySold = technologySold,
            HAUserType = userType,
            Has3rdParty = thirdParty,
            Notes = timsAppointment.Notes,
            LastUpdatedDate = timsAppointment.UpdatedDate,
            LocationID = timsAppointment.SiteId,
            InsertDateTime = timsAppointment.CreatedDate,
            AppointmentTypeID = timsAppointment.AppointmentTypeId ?? 0
        };

        return audigyAppointment;
    }
}