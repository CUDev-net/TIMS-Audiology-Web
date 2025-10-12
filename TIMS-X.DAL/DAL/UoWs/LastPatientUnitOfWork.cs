using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface ILastPatientUnitOfWork : IUnitOfWork
{
    Task<LastPatientList> GetLastPatientList(long userId);
}

public class LastPatientUnitOfWork : UnitOfWorkBase, ILastPatientUnitOfWork
{
    public LastPatientUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(LastPatientList);

    public async Task<LastPatientList> GetLastPatientList(long userId)
    {
        return await Single<LastPatientList>(u => u.UserId == userId);
    }
}