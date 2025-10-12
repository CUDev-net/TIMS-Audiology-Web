using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IAuthorizationReferenceUnitOfWork
{
	Task<List<AuthorizationReference>> GetAuthorizationReferences(int patientId);
	Task<List<AuthorizationReference>> UpdateAuthorizationReferences(int patientId, List<int> ids);
}

public class AuthorizationReferenceUnitOfWork : UnitOfWorkBase, IAuthorizationReferenceUnitOfWork
{
	public AuthorizationReferenceUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(
		context,
		httpContextAccessor)
	{
	}

	protected override string TableName => nameof(AuthorizationReference);
	
	public async Task<List<AuthorizationReference>> GetAuthorizationReferences(int patientId)
	{
		var sqlParams = new object[]
		{
			new SqlParameter("patientId", patientId)
		};

		var authorizationReferences = FromSql<AuthorizationReference>(@"
        SELECT 
			PatientID PatientId,
			AuthorizationID AuthorizationId,
			DateAdded CreatedDate
			FROM AuthorizationReference WHERE PatientID = @patientId", sqlParams).ToListAsync();
		return await authorizationReferences;
	}

	public async Task<List<AuthorizationReference>> UpdateAuthorizationReferences(int patientId, List<int> ids)
	{
		var sqlParams = new object[]
		{
			new SqlParameter("patientId", patientId)
		};

		var current = await GetAuthorizationReferences(patientId);
		var currentIds = current.Select(x => x.AuthorizationId).ToList();
		var toAdd = ids.Where(i => !currentIds.Contains(i)).ToList();
		if (toAdd.Any())
		{
			var addQuery = $@"INSERT INTO AuthorizationReference (PatientID, AuthorizationID, DateAdded)
			SELECT @patientId, ID, GETDATE() FROM [Authorization] WHERE ID IN (
				{string.Join(",", toAdd)})";
			var added = ExecuteSqlCommand(addQuery, sqlParams);
		}

		var toDelete = currentIds.Where(i => !ids.Contains(i)).ToList();
		if (toDelete.Any())
		{
			var deleteQuery = $@"DELETE FROM AuthorizationReference 
				WHERE PatientID = @patientId AND AuthorizationID IN ({string.Join(",", toDelete)})";
			var deleted = ExecuteSqlCommand(deleteQuery, sqlParams);
		}

		return await GetAuthorizationReferences(patientId);
	}
}