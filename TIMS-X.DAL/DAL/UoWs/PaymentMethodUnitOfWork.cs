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

public interface IPaymentMethodUnitOfWork : IUnitOfWork
{
    Task<PaymentMethod> GetPaymentMethod(int id);

    Task<List<PaymentMethod>> GetPaymentMethods(Expression<Func<PaymentMethod, bool>> filter = null,
        Func<IQueryable<PaymentMethod>, IOrderedQueryable<PaymentMethod>> orderBy = null,
        Func<IQueryable<PaymentMethod>, IIncludableQueryable<PaymentMethod, object>> includes = null);
}

public class PaymentMethodUnitOfWork : UnitOfWorkBase, IPaymentMethodUnitOfWork
{
    public PaymentMethodUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(PaymentMethod);

    public async Task<PaymentMethod> GetPaymentMethod(int id)
    {
        return await Single<PaymentMethod>(u => u.Id == id);
    }

    public async Task<List<PaymentMethod>> GetPaymentMethods(Expression<Func<PaymentMethod, bool>> filter = null,
        Func<IQueryable<PaymentMethod>, IOrderedQueryable<PaymentMethod>> orderBy = null,
        Func<IQueryable<PaymentMethod>, IIncludableQueryable<PaymentMethod, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}