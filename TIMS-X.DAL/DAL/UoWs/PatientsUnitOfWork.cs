using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IPatientsUnitOfWork : IUnitOfWork
{
	Task<List<PatientSummary>> FindPatients(PatientSearchCriteriaDto searchCriteriaDto);
	Task<Patient> GetPatient(long id, Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>> includes = null);
	IQueryable<CommunicationRestriction> GetPatientRestrictions(long patientId);

	IQueryable<Patient> GetPatients(Expression<Func<Patient, bool>> filter = null,
		Func<IQueryable<Patient>, IOrderedQueryable<Patient>> orderBy = null,
		Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>> includes = null);

	Task<PatientSummary> GetPatientSummary(long id);

	Task<QBPatientBalance> GetQBPatientBalance(string qbid,
		Func<IQueryable<QBPatientBalance>, IIncludableQueryable<QBPatientBalance, object>> includes = null);

	IQueryable<PatientSummary> Search(PatientSearchCriteriaDto searchCriteriaDto);
}

public class PatientsUnitOfWork : UnitOfWorkBase, IPatientsUnitOfWork
{
	private readonly string _patientSummaryCoreSelect = @"select p.ID,
	p.DtCreated as CreatedDate,
	p.DtUpdated as UpdatedDate,
	p.FName as FirstName, 
	p.LName as LastName, 
	p.Initial, 
	p.Addr1 as Address1, 
	p.Addr2 as Address2,
	p.City,
	p.State,
	p.ZipCode,
	p.PrimaryPhone,
	p.HomePhone,
	p.WorkPhone,
	p.OtherPhone,
	p.MobilePhone,
	p.Email,
	p.Contact,
    p.SiteID as SiteId,
	p.DtOfBirth as BirthDate,
	p.Sex as Gender,
	p.Deceased,
	p.DtOfDeath as DeathDate,
    p.OtStatus as OtStatusId,
    p.OtStatusDescriptionId as OtStatusDescriptionId,
	p.Inactive,
    ";

	public PatientsUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
		httpContextAccessor)
	{
	}

	protected override string TableName => nameof(Patient);

	public async Task<Patient> GetPatient(long id,
		Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>> includes = null)
	{
		return await Single(u => u.Id == id, includes);
	}

	public async Task<QBPatientBalance> GetQBPatientBalance(string qbid,
		Func<IQueryable<QBPatientBalance>, IIncludableQueryable<QBPatientBalance, object>> includes = null)
	{
		return await Single(u => u.QBID == qbid, includes);
	}

	public IQueryable<Patient> GetPatients(Expression<Func<Patient, bool>> filter = null,
		Func<IQueryable<Patient>, IOrderedQueryable<Patient>> orderBy = null,
		Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>> includes = null)
	{
		return Get(filter, orderBy, includes);
	}

	public async Task<List<PatientSummary>> FindPatients(PatientSearchCriteriaDto searchCriteriaDto)
	{
		object[] sqlParams =
		{
			new SqlParameter("@lastname", searchCriteriaDto.LastName),
			new SqlParameter("@firstname", searchCriteriaDto.FirstName),
			new SqlParameter("@dob", searchCriteriaDto.DateOfBirth.Value.Date)
		};
		var sql = @$"{_patientSummaryCoreSelect}
			OpportunityDescription = '',
			PatientStatus = '',
			MaritalStatus  = '',
			PatientType  = '',
		    UpdatedByUserName  = ''
				from patient p 
			where p.LName = @lastname and 
p.FName like @firstname and 
CAST(p.DtOfBirth as DATE) = @dob 
";

		var candiates = await FromSql<PatientSummary>(sql, sqlParams).ToListAsync();
		if (string.IsNullOrWhiteSpace(searchCriteriaDto.PhoneNumber)) return candiates;

		var patients = new List<PatientSummary>();
		// Strip all non-numeric characters from the phone number
		var phone = Regex.Replace(searchCriteriaDto.PhoneNumber, @"\D", "");
		foreach (var candidate in candiates)
		{
			if (candidate.Email == searchCriteriaDto.Email)
				patients.Add(candidate);
			else if (!string.IsNullOrWhiteSpace(candidate.HomePhone) &&
			    phone == Regex.Replace(candidate.HomePhone, @"\D", ""))
				patients.Add(candidate);
			else if (!string.IsNullOrWhiteSpace(candidate.WorkPhone) &&
			    phone == Regex.Replace(candidate.WorkPhone, @"\D", ""))
				patients.Add(candidate);
			else if (!string.IsNullOrWhiteSpace(candidate.OtherPhone) &&
			    phone == Regex.Replace(candidate.OtherPhone, @"\D", ""))
				patients.Add(candidate);
			else if (!string.IsNullOrWhiteSpace(candidate.MobilePhone) &&
			    phone == Regex.Replace(candidate.MobilePhone, @"\D", ""))
				patients.Add(candidate);
		}

		return patients;
	}

	public IQueryable<PatientSummary> Search(PatientSearchCriteriaDto searchCriteriaDto)
	{
		object[] sqlParams;
		var sql = @$"{_patientSummaryCoreSelect}
			osd.Description as OpportunityDescription,
			ps.Name PatientStatus,
			m.Description MaritalStatus,
			pt.Name PatientType,
		    tu.Name UpdatedByUserName
				from patient p
	left outer join PatientStatus ps on p.PatientStatusID = ps.ID
	left outer join PatientType pt on p.PatientTypeID = pt.ID
	left outer join MaritalStatus m on p.MaritalStatusID = m.Name
    left outer join TIMSUser tu on tu.ID = p.UID
    left outer join OtStatusDescription osd on osd.ID = p.OtStatusDescriptionId
";
		if (!string.IsNullOrEmpty(searchCriteriaDto.LastName))
		{
			sql += "where p.LName like @lastname";
			sqlParams = new object[]
			{
				new SqlParameter("@lastname", searchCriteriaDto.LastName + '%')
			};
		}
		else if (!string.IsNullOrEmpty(searchCriteriaDto.FirstName))
		{
			sql += "where p.FName like @firstname";
			sqlParams = new object[]
			{
				new SqlParameter("@firstname", searchCriteriaDto.FirstName + '%')
			};
		}
		else if (searchCriteriaDto.DateOfBirth.HasValue)
		{
			sql += "where CAST(p.DtOfBirth as DATE) = @dob";
			sqlParams = new object[]
			{
				new SqlParameter("@dob", searchCriteriaDto.DateOfBirth.Value.Date)
			};
		}
		else if (!string.IsNullOrEmpty(searchCriteriaDto.PhoneNumber))
		{
			sql += @"where
                    p.HomePhone = @phone or
                    p.OtherPhone = @phone or
                    p.WorkPhone = @phone or
                    p.MobilePhone = @phone";
			sqlParams = new object[]
			{
				new SqlParameter("@phone", searchCriteriaDto.PhoneNumber)
			};
		}
		else
		{
			throw new ArgumentException("Invalid search criteria");
		}

		if (!searchCriteriaDto.IncludeInactive) sql += " and p.Inactive = 0";

		return FromSql<PatientSummary>(sql, sqlParams);
	}

	public IQueryable<CommunicationRestriction> GetPatientRestrictions(long patientId)
	{
		var sqlParams = new object[]
		{
			new SqlParameter("@patientId", patientId)
		};

		var communicationRestrictions = FromSql<CommunicationRestriction>(@"
select r.*
from PatientRestriction pr
join Restriction r
on r.ID = pr.RestrictionID
where pr.PatID = @patientId", sqlParams);

		return communicationRestrictions;
	}

	public Task<PatientSummary> GetPatientSummary(long id)
	{
		var sqlParams = new object[]
		{
			new SqlParameter("@id", id)
		};

		var patientSummaries = FromSql<PatientSummary>(@"
select p.ID,
	p.DtCreated as CreatedDate,
	p.DtUpdated as UpdatedDate,
	p.FName as FirstName, 
	p.LName as LastName, 
	p.Initial, 
	p.Addr1 as Address1, 
	p.Addr2 as Address2,
	p.City,
	p.State,
	p.ZipCode,
	p.PrimaryPhone,
	p.HomePhone,
	p.WorkPhone,
	p.OtherPhone,
	p.MobilePhone,
	p.Email,
	p.Contact,
    p.SiteID as SiteId,
	p.DtOfBirth as BirthDate,
	p.Sex as Gender,
	p.Deceased,
	p.DtOfDeath as DeathDate,
    p.OtStatus as OtStatusId,
    p.OtStatusDescriptionId as OtStatusDescriptionId,
    osd.Description as OpportunityDescription,
	p.Inactive,
	ps.Name PatientStatus,
	m.Description MaritalStatus,
	pt.Name PatientType,
    tu.Name UpdatedByUserName
	from patient p 
	left outer join PatientStatus ps on p.PatientStatusID = ps.ID
	left outer join PatientType pt on p.PatientTypeID = pt.ID
	left outer join MaritalStatus m on p.MaritalStatusID = m.Name
    left outer join TIMSUser tu on tu.ID = p.UID
    left outer join OtStatusDescription osd on osd.ID = p.OtStatusDescriptionId
    where p.ID = @id", sqlParams);

		var patient = patientSummaries.FirstOrDefaultAsync();
		return patient;
	}

	public Task<string> GetPatientMarketingAsync(long marketingId)
	{
		var sqlParams = new object[]
		{
			new SqlParameter("@id", marketingId)
		};
		return FromSql<string>(@"select mr.Name from MktReference mr where mr.ID = @id", sqlParams)
			.FirstOrDefaultAsync();
	}
}