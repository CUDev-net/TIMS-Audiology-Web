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

public interface IStudentStatusUnitOfWork : IUnitOfWork
{
	Task<StudentStatus> GetStudentStatus(int id);

	Task<List<StudentStatus>> GetStudentStatuses(Expression<Func<StudentStatus, bool>> filter = null,
		Func<IQueryable<StudentStatus>, IOrderedQueryable<StudentStatus>> orderBy = null,
		Func<IQueryable<StudentStatus>, IIncludableQueryable<StudentStatus, object>> includes = null);
}

public class StudentStatusUnitOfWork : UnitOfWorkBase, IStudentStatusUnitOfWork
{
	public StudentStatusUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
		httpContextAccessor)
	{
	}

	protected override string TableName => nameof(StudentStatus);

	public async Task<StudentStatus> GetStudentStatus(int id)
	{
		return await Single<StudentStatus>(u => u.Id == id);
	}

	public async Task<List<StudentStatus>> GetStudentStatuses(Expression<Func<StudentStatus, bool>> filter = null,
		Func<IQueryable<StudentStatus>, IOrderedQueryable<StudentStatus>> orderBy = null,
		Func<IQueryable<StudentStatus>, IIncludableQueryable<StudentStatus, object>> includes = null)
	{
		return await Get(filter, orderBy, includes).ToListAsync();
	}
}