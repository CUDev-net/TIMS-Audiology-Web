using Microsoft.AspNetCore.Http;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IRecurringIntervalRemovedUnitOfWork : IUnitOfWork
{
}

internal class RecurringIntervalRemovedUnitOfWork : UnitOfWorkBase, IRecurringIntervalRemovedUnitOfWork
{
    public RecurringIntervalRemovedUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor)
        : base(context, httpContextAccessor)
    {
    }

    protected override string TableName => nameof(RecurringIntervalRemoved);
}