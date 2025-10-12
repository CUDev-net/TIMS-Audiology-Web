using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IDescriptionUnitOfWork : IUnitOfWork
{
	Task<Description> GetDescriptions();
}

public class DescriptionUnitOfWork : UnitOfWorkBase, IDescriptionUnitOfWork
{
	public DescriptionUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
		httpContextAccessor)
	{
	}

	protected override string TableName => "Description";

	public async Task<Description> GetDescriptions()
	{
		return await Single<Description>();
	}
}