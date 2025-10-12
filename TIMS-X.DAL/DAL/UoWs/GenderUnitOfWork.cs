using System.Linq;
using Microsoft.AspNetCore.Http;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IGenderUnitOfWork : IUnitOfWork
{
    IQueryable<Sex> GetGenders();
}

public class GenderUnitOfWork : UnitOfWorkBase, IGenderUnitOfWork
{
    public GenderUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(Sex);

    public IQueryable<Sex> GetGenders()
    {
        return Get<Sex>();
    }
}