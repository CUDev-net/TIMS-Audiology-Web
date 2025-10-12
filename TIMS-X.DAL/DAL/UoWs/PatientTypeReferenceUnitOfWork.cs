using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IPatientTypeReferenceUnitOfWork
{
	Task<List<PatientTypeReference>> GetPatientTypeReferences(int patientId);
	Task<List<PatientTypeReference>> UpdatePatientTypeReferences(int patientId, List<int> ids);
}

public class PatientTypeReferenceUnitOfWork : UnitOfWorkBase, IPatientTypeReferenceUnitOfWork
{
	public PatientTypeReferenceUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(
		context,
		httpContextAccessor)
	{
	}

	protected override string TableName => nameof(PatientTypeReference);
	
	public async Task<List<PatientTypeReference>> GetPatientTypeReferences(int patientId)
	{
		var sqlParams = new object[]
		{
			new SqlParameter("patientId", patientId)
		};

		var patientTypeReferences = FromSql<PatientTypeReference>(@"
        SELECT 
			PatientID PatientId,
			PatientTypeID PatientTypeId,
			DateAdded CreatedDate
			FROM PatientTypeReference WHERE PatientID = @patientId", sqlParams).ToListAsync();
		return await patientTypeReferences;
	}

	public async Task<List<PatientTypeReference>> UpdatePatientTypeReferences(int patientId, List<int> ids)
	{
		var sqlParams = new object[]
		{
			new SqlParameter("patientId", patientId)
		};

		var current = await GetPatientTypeReferences(patientId);
		var currentIds = current.Select(x => x.PatientTypeId).ToList();
		var toAdd = ids.Where(i => !currentIds.Contains(i)).ToList();
		if (toAdd.Any())
		{
			var addQuery = $@"INSERT INTO PatientTypeReference (PatientID, PatientTypeID, DateAdded)
			SELECT @patientId, ID, GETDATE() FROM [PatientType] WHERE ID IN (
				{string.Join(",", toAdd)})";
			var added = ExecuteSqlCommand(addQuery, sqlParams);
		}

		var toDelete = currentIds.Where(i => !ids.Contains(i)).ToList();
		if (toDelete.Any())
		{
			var deleteQuery = $@"DELETE FROM PatientTypeReference 
				WHERE PatientID = @patientId AND PatientTypeID IN ({string.Join(",", toDelete)})";
			var deleted = ExecuteSqlCommand(deleteQuery, sqlParams);
		}

		return await GetPatientTypeReferences(patientId);
	}
}