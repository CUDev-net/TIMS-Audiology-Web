using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Serilog;
using TIMS_X.Core.Attributes;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Utils;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IUnitOfWork
{
	Task<T> Add<T>(T item) where T : Entity;
	void AttachAndSetItemModifiedState<T>(T item) where T : Entity;
	Task Delete<T>(T item) where T : Entity;
	void Delete(int id);
	int ExecuteSqlCommand(string sql, params object[] parameters);
	IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class;
	public void SetItemDeletedState<T>(T item) where T : Entity;
	Task<T> Update<T>(T item) where T : Entity;
}

public abstract class UnitOfWorkBase : IDisposable
{
	protected readonly IHttpContextAccessor _httpContextAccessor;
	protected Action<Entity, DateTime> _auditingFunction;
	protected DbContext _context;

	protected UnitOfWorkBase(DbContext context, IHttpContextAccessor httpContextAccessor)
	{
		_context = context;
		_httpContextAccessor = httpContextAccessor;
	}

	protected abstract string TableName { get; }

	public void Dispose()
	{
		_context?.Dispose();
	}

	private void _SetChildEntitesModifiedState<T>(T entity, DbContext context) where T : Entity
	{
	}

	private void _SetInsertedData(Entity entity, DateTime date)
	{
		// 0 is Support User
		var userId = int.Parse(_httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == StringConstants.User)
			.Select(x => x.Value).FirstOrDefault() ?? "0");

		if (entity is ICreateByUserAudited createAudited) createAudited.CreatedUserId = userId;

		if (entity is ICreateDateAudited dateAudited) dateAudited.CreatedDate = date;
		if (entity is IUpdateDateAudited updateDateAudited) updateDateAudited.UpdatedDate = date;

		if (entity is IUpdateAudited updateAudited)
		{
			updateAudited.UpdatedDate = date;
			updateAudited.UpdatedUserId = userId;
		}

		_auditingFunction = _SetInsertedData;
		SetEntityTrackingData(entity, date);
	}

	private void _ValidateEntity<T>(T entity) where T : Entity
	{
	}

	public async Task<T> Add<T>(T item) where T : Entity
	{
		var now = DateTime.Now;
		_ValidateEntity(item);
		_SetInsertedData(item, now);

		await using var transaction = await _context.Database.BeginTransactionAsync();
		try
		{
			await _context.Set<T>().AddAsync(item);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}
		catch (Exception e)
		{
			var message = $"Error Adding Entity {item.GetType()}, Error: {e}";
			Trace.TraceError(message);
			Log.Error(e, message);
			await transaction.RollbackAsync();
			throw;
		}

		return item;
	}

	public void AttachAndSetItemModifiedState<T>(T item) where T : Entity
	{
		_context.Set<T>().Attach(item);
		_context.Entry(item).State = EntityState.Modified;
	}

	public void Delete(int id)
	{
		object[] sqlParams =
		{
			new SqlParameter("@id", id)
		};
		ExecuteSqlCommand($"delete from [{TableName}] where ID = @id", sqlParams);
	}

	public async Task Delete<T>(T item) where T : Entity
	{
		await using var transaction = await _context.Database.BeginTransactionAsync();
		try
		{
			_context.Set<T>().Attach(item);
			_context.Set<T>().Remove(item);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}
		catch (Exception e)
		{
			var message = $"Error Deleting Entity {item.GetType()}, Error: {e}";
			Trace.TraceError(message);
			Log.Error(e, message);
			await transaction.RollbackAsync();
			throw;
		}
	}

	public int ExecuteSqlCommand(string sql, params object[] parameters)
	{
		var results = _context.Database.ExecuteSqlRaw(sql, parameters);
		_context.SaveChanges();
		return results;
	}

	public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class
	{
		var results = _context.Set<TEntity>().FromSqlRaw(sql, parameters);
		return results;
	}

	public IQueryable<T> Get<T>(Expression<Func<T, bool>> filter = null,
		Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
		Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null
	) where T : Entity
	{
		var dbSet = _context.Set<T>();
		IQueryable<T> query = dbSet;

		// https://stackoverflow.com/questions/46374252/how-to-write-repository-method-for-theninclude-in-ef-core-2
		if (include != null) query = include(query);
		if (filter != null) query = query.Where(filter);
		if (orderBy != null) query = orderBy(query);
		return query;
	}

	private void SetEntityTrackingData(Entity entity, DateTime date)
	{
		if (_auditingFunction == null)
			throw new NullReferenceException(
				"RepositoryBase: SetEntityTrackingDates - _auditingFunction cannot be null");
		var propertyInfos = entity.GetType().GetProperties();
		foreach (var pi in propertyInfos)
		{
			var attributes = pi.GetCustomAttributes(false);
			foreach (var attribute in attributes)
			{
				var entityPropertyInfo = entity.GetType().GetProperty(pi.Name);
				if (entityPropertyInfo == null)
					continue;
				if (attribute is TimsObjectAttribute)
				{
					if (entityPropertyInfo.GetValue(entity, null) is AuditableEntity child &&
					    !child.HasBeenAudited)
						_auditingFunction(child, date);
				}
				else if (attribute is TimsCollectionAttribute)
				{
					if (entityPropertyInfo.GetValue(entity, null) is IEnumerable<Entity>
					    childCollection)
						foreach (var c in childCollection)
							if (c is AuditableEntity { HasBeenAudited: false } a)
								_auditingFunction(a, date);
				}
			}
		}
	}

	public void SetItemDeletedState<T>(T item) where T : Entity
	{
		_context.Entry(item).State = EntityState.Deleted;
	}

	protected void SetUpdatedData(Entity entity, DateTime date)
	{
		var userId = int.Parse(_httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == StringConstants.User)
			.Select(x => x.Value).First());

		if (entity is IUpdateAudited updateAudited) updateAudited.UpdatedUserId = userId;
		if (entity is IUpdateDateAudited updateDateAudited) updateDateAudited.UpdatedDate = date;
		if (entity.Id <= 0 && entity is ICreateByUserAudited createAudited)
		{
			if (entity is ICreateDateAudited dateAudited) dateAudited.CreatedDate = date;
			createAudited.CreatedUserId = userId;
			entity.Id = 0;
		}

		_auditingFunction = SetUpdatedData;
		SetEntityTrackingData(entity, date);
	}

	public async Task<T> Single<T>(Expression<Func<T, bool>> filter = null,
		Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null) where T : Entity
	{
		var dbSet = _context.Set<T>();
		IQueryable<T> query = dbSet;

		// https://stackoverflow.com/questions/46374252/how-to-write-repository-method-for-theninclude-in-ef-core-2
		if (include != null) query = include(query);

		//foreach (var include in includes) query = query.Include(include);
		if (filter != null) query = query.Where(filter);
		return await query.FirstOrDefaultAsync();
	}

	public async Task<T> Update<T>(T item) where T : Entity
	{
		var now = DateTime.Now;
		_ValidateEntity(item);
		SetUpdatedData(item, now);

		await using var transaction = await _context.Database.BeginTransactionAsync();
		try
		{
			_context.Set<T>().Attach(item);
			_context.Entry(item).State = EntityState.Modified;
			_SetChildEntitesModifiedState(item, _context);
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
		}
		catch (Exception e)
		{
			var message = $"Error Updating Entity {item.GetType()}, Error: {e}";
			Trace.TraceError(message);
			Log.Error(e, message);
			await transaction.RollbackAsync();
			throw;
		}

		return item;
	}
}