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

public interface ICustomerMessageUnitOfWork : IUnitOfWork
{
    Task<CustomerMessage> GetCustomerMessage(int id);

    Task<List<CustomerMessage>> GetCustomerMessages(Expression<Func<CustomerMessage, bool>> filter = null,
        Func<IQueryable<CustomerMessage>, IOrderedQueryable<CustomerMessage>> orderBy = null,
        Func<IQueryable<CustomerMessage>, IIncludableQueryable<CustomerMessage, object>> includes = null);
}

public class CustomerMessageUnitOfWork : UnitOfWorkBase, ICustomerMessageUnitOfWork
{
    public CustomerMessageUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "POSCustomerMessage";

    public async Task<CustomerMessage> GetCustomerMessage(int id)
    {
        return await Single<CustomerMessage>(u => u.Id == id);
    }

    public async Task<List<CustomerMessage>> GetCustomerMessages(Expression<Func<CustomerMessage, bool>> filter = null,
        Func<IQueryable<CustomerMessage>, IOrderedQueryable<CustomerMessage>> orderBy = null,
        Func<IQueryable<CustomerMessage>, IIncludableQueryable<CustomerMessage, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}