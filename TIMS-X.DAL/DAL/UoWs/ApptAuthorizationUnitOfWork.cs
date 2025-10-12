using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IApptAuthorizationUnitOfWork : IUnitOfWork
{
	Task<ApptAuthorization> GetApptAuthorization(int id);

	Task<List<ApptAuthorization>> GetApptAuthorizations(Expression<Func<ApptAuthorization, bool>> filter = null,
		Func<IQueryable<ApptAuthorization>, IOrderedQueryable<ApptAuthorization>> orderBy = null,
		Func<IQueryable<ApptAuthorization>, IIncludableQueryable<ApptAuthorization, object>> includes = null);
}

public class ApptAuthorizationUnitOfWork : UnitOfWorkBase, IApptAuthorizationUnitOfWork
{
	public ApptAuthorizationUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
		httpContextAccessor)
	{
	}

	protected override string TableName => nameof(ApptAuthorization);

	public async Task<ApptAuthorization> GetApptAuthorization(int id)
	{
		return await Single<ApptAuthorization>(u => u.Id == id);
	}

	public async Task<List<ApptAuthorization>> GetApptAuthorizations(
		Expression<Func<ApptAuthorization, bool>> filter = null,
		Func<IQueryable<ApptAuthorization>, IOrderedQueryable<ApptAuthorization>> orderBy = null,
		Func<IQueryable<ApptAuthorization>, IIncludableQueryable<ApptAuthorization, object>> includes = null)
	{
		return await Get(filter, orderBy, includes).ToListAsync();
	}
}