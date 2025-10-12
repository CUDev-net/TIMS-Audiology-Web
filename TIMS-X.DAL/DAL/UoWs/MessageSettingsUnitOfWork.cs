using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IMessageSettingsUnitOfWork : IUnitOfWork
{
	Task<MessageSettings> GetMessageSettings(
		Func<IQueryable<MessageSettings>, IIncludableQueryable<MessageSettings, object>> includes = null);
}

public class MessageSettingsUnitOfWork : UnitOfWorkBase, IMessageSettingsUnitOfWork
{
	public MessageSettingsUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
		httpContextAccessor)
	{
	}

	protected override string TableName => nameof(MessageSettings);

	public async Task<MessageSettings> GetMessageSettings(
		Func<IQueryable<MessageSettings>, IIncludableQueryable<MessageSettings, object>> includes = null)
	{
		return await Single(null, includes);
	}
}