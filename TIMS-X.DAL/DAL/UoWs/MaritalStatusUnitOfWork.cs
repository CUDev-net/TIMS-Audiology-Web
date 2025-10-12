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

public interface IMaritalStatusUnitOfWork : IUnitOfWork
{
	Task<MaritalStatus> GetMaritalStatus(int id);

	Task<List<MaritalStatus>> GetMaritalStatuses(Expression<Func<MaritalStatus, bool>> filter = null,
		Func<IQueryable<MaritalStatus>, IOrderedQueryable<MaritalStatus>> orderBy = null,
		Func<IQueryable<MaritalStatus>, IIncludableQueryable<MaritalStatus, object>> includes = null);
}

public class MaritalStatusUnitOfWork : UnitOfWorkBase, IMaritalStatusUnitOfWork
{
	public MaritalStatusUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
		httpContextAccessor)
	{
	}

	protected override string TableName => nameof(MaritalStatus);

	public async Task<MaritalStatus> GetMaritalStatus(int id)
	{
		return await Single<MaritalStatus>(u => u.Id == id);
	}

	public async Task<List<MaritalStatus>> GetMaritalStatuses(Expression<Func<MaritalStatus, bool>> filter = null,
		Func<IQueryable<MaritalStatus>, IOrderedQueryable<MaritalStatus>> orderBy = null,
		Func<IQueryable<MaritalStatus>, IIncludableQueryable<MaritalStatus, object>> includes = null)
	{
		return await Get(filter, orderBy, includes).ToListAsync();
	}
}