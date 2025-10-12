using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.BLL.VendorSync.Audigy;
using TIMS_X.BLL.VendorSync.Common;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.DAL.UoWs;
using Patient = TIMS_X.BLL.VendorSync.Audigy.Patient;
using PatientInsurance = TIMS_X.BLL.VendorSync.Audigy.PatientInsurance;
using TIMSPatient = TIMS_X.Core.Domain.Patient;

namespace TIMS_X.BLL.VendorSync.Repositories;

public interface IVendorPatientRepository
{
    Task<Patient> GetPatient(int id);

    Task<List<Patient>> GetPatients(DateTime? fromDate, DateTime? toDate);
}

public class VendorPatientRepository : IVendorPatientRepository
{
    private readonly IGenderUnitOfWork _genderUnitOfWork;
    private readonly IPatientsUnitOfWork _patientsUnitOfWork;
    private readonly IAppointmentsUnitOfWork _appointmentsUnitOfWork;
    private readonly IPracticeUnitOfWork _practiceUnitOfWork;

    public VendorPatientRepository(IPatientsUnitOfWork patientsUnitOfWork, IGenderUnitOfWork genderUnitOfWork, 
        IPracticeUnitOfWork practiceUnitOfWork, IAppointmentsUnitOfWork appointmentsUnitOfWork)
    {
        _patientsUnitOfWork = patientsUnitOfWork;
        _genderUnitOfWork = genderUnitOfWork;
        _practiceUnitOfWork = practiceUnitOfWork;
        _appointmentsUnitOfWork = appointmentsUnitOfWork;
    }

    public async Task<Patient> GetPatient(int id)
    {
        var practiceId = await _practiceUnitOfWork.GetPracticeAudigyId();
        var timsPatient = await _patientsUnitOfWork.GetPatient(id, _Includes);

        if (timsPatient == null) return null;
        var genders = await _genderUnitOfWork.GetGenders().ToListAsync();
        string qbBalance = string.Empty;
        var qbObject = await _patientsUnitOfWork.GetQBPatientBalance(timsPatient.QBID);
        if (qbObject != null)
        {
            qbBalance = qbObject.Balance.ToString("F");
        }
        var firstAppt = await _appointmentsUnitOfWork.GetAppointments(a => a.PatientId == timsPatient.Id,
                q => q.OrderBy(a => a.StartsAt), null).FirstOrDefaultAsync();
        var firstVisit = firstAppt == null ? string.Empty : firstAppt.StartsAt.ToString();
        return _MapPatientToPatient(timsPatient, genders, practiceId, qbBalance, firstVisit);
    }


    public async Task<List<Patient>> GetPatients(DateTime? fromDate, DateTime? toDate)
    {
        var practiceId = await _practiceUnitOfWork.GetPracticeAudigyId();
        IQueryable<TIMSPatient> rawItems;
        if (fromDate.HasValue && toDate.HasValue)
            rawItems = _patientsUnitOfWork.GetPatients(p => p.UpdatedDate >= fromDate && p.UpdatedDate <= toDate.Value,
                null, _Includes);
        else if (fromDate.HasValue)
            rawItems = _patientsUnitOfWork.GetPatients(p => p.UpdatedDate >= fromDate,
                null, _Includes);
        else if (toDate.HasValue)
            rawItems = _patientsUnitOfWork.GetPatients(p => p.UpdatedDate <= toDate,
                null, _Includes);
        else
            rawItems = _patientsUnitOfWork.GetPatients(null, null, _Includes);

        var genders = await _genderUnitOfWork.GetGenders().ToListAsync();
        var patientList = new List<Patient>();
        foreach (var patient in await rawItems.ToListAsync())
        {
            string qbBalance = string.Empty;
            var qbObject = await _patientsUnitOfWork.GetQBPatientBalance(patient.QBID);
            if (qbObject != null)
            {
                qbBalance = qbObject.Balance.ToString("F");
            }

            var firstAppt = await _appointmentsUnitOfWork.GetAppointments(a => a.PatientId == patient.Id,
                q => q.OrderBy(a => a.StartsAt), null).FirstOrDefaultAsync();
            var firstVisit = firstAppt == null ? string.Empty : firstAppt.StartsAt.ToString();

            patientList.Add(_MapPatientToPatient(patient, genders, practiceId, qbBalance, firstVisit));
        }
        
        return patientList;
    }

    private IIncludableQueryable<TIMSPatient, object> _Includes(IQueryable<TIMSPatient> patient)
    {
        return patient
           .Include(p => p.Marketing)
           .Include(p => p.PrimaryCarePhysician);
        //return patient.Include(p => p.PatientInsurances)
        //    .ThenInclude(i => i.InsurancePayer)
        //    .Include(p => p.Marketing)
        //    .Include(p => p.PrimaryCarePhysician);
    }

    private static Patient _MapPatientToPatient(TIMSPatient patient, List<Sex> genders, string practiceId, string qbBalance, string firstVisit)
    {
        var audigyPatient = new Patient
        {
            PracticeID = practiceId,
            PatientID = patient.Id,
            AccountID = patient.AccountNo,
            FirstName = patient.FirstName,
            MiddleInitial = patient.Initial,
            LastName = patient.LastName,
            Salutation = patient.Salutation?.Name,
            DateOfBirth = patient.BirthDate?.ToShortDateString(),
            Address1 = patient.Address1,
            Address2 = patient.Address2,
            City = patient.City,
            State = patient.State,
            ZipCode = patient.ZipCode,
            Phone = patient.HomePhone.StripNonNumeric(),
            WorkPhone = patient.WorkPhone.StripNonNumeric(),
            MobilePhone = patient.MobilePhone.StripNonNumeric(),
            Email = patient.Email,
            FirstVisit = firstVisit,
            AudiologistID = patient.ProviderId ?? 0,
            AccountBalance = qbBalance,
            ReferringPhysician = patient.ReferringPhysician?.Name,
            MarketingSource = patient.Marketing?.Name,
            Status = patient.Inactive ? "Inactive" : "Active",
            LastUpdatedDate = patient.UpdatedDate,
        };



        if (genders.Exists(g => g.Name == patient.Sex))
            audigyPatient.Gender = genders.Single(x => x.Name == patient.Sex).Description;

        //var patientRelationShips = Helpers.GetPatientRelationTable();
        //foreach (var patientInsurance in patient.PatientInsurances)
        //{
        //    var audigyPatientInsurance = new PatientInsurance
        //    {
        //        Id = patientInsurance.Id,
        //        GroupNumber = patientInsurance.PolicyGroupNum,
        //        PlanName = patientInsurance.PolicyGroupName,
        //        PolicyHolder = $"{patientInsurance.FirstName} {patientInsurance.LastName}"
        //    };
        //    var relationship =
        //        patientRelationShips.FirstOrDefault(x => x.Name == patientInsurance.RelationtoInsured);
        //    if (relationship != null) audigyPatientInsurance.RelationshipToPatient = relationship.Description;
        //    if (patientInsurance.InsurancePayer != null)
        //        audigyPatientInsurance.InsuranceCompany = new InsuranceCompany
        //        {
        //            Id = patientInsurance.InsurancePayer.Id,
        //            Name = patientInsurance.InsurancePayer.Name,
        //            //OrganizationId = patientInsurance.InsurancePayer.OrganizationId, verify this is what they actually want
        //            Phone = patientInsurance.InsurancePayer.Phone,
        //            Fax = patientInsurance.InsurancePayer.Fax,
        //            Address = patientInsurance.InsurancePayer.Address1,
        //            City = patientInsurance.InsurancePayer.City,
        //            State = patientInsurance.InsurancePayer.State,
        //            ZipCode = patientInsurance.InsurancePayer.ZipCode
        //        };
        //    audigyPatient.PatientInsurances.Add(audigyPatientInsurance);
        //}

        return audigyPatient;
    }
}