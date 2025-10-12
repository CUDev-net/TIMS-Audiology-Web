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

public interface IEmplStatusUnitOfWork : IUnitOfWork
{
	Task<EmplStatus> GetEmplStatus(int id);

	Task<List<EmplStatus>> GetEmplStatuses(Expression<Func<EmplStatus, bool>> filter = null,
		Func<IQueryable<EmplStatus>, IOrderedQueryable<EmplStatus>> orderBy = null,
		Func<IQueryable<EmplStatus>, IIncludableQueryable<EmplStatus, object>> includes = null);
}

public class EmplStatusUnitOfWork : UnitOfWorkBase, IEmplStatusUnitOfWork
{
	public EmplStatusUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
		httpContextAccessor)
	{
	}

	protected override string TableName => nameof(EmplStatus);

	public async Task<EmplStatus> GetEmplStatus(int id)
	{
		return await Single<EmplStatus>(u => u.Id == id);
	}

	public async Task<List<EmplStatus>> GetEmplStatuses(Expression<Func<EmplStatus, bool>> filter = null,
		Func<IQueryable<EmplStatus>, IOrderedQueryable<EmplStatus>> orderBy = null,
		Func<IQueryable<EmplStatus>, IIncludableQueryable<EmplStatus, object>> includes = null)
	{
		return await Get(filter, orderBy, includes).ToListAsync();
	}
}