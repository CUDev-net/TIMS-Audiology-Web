using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IProvidersUnitOfWork : IUnitOfWork
{
	Task<Provider> GetProvider(int id,
		Func<IQueryable<Provider>, IIncludableQueryable<Provider, object>> includes = null);

	Task<IDictionary<int, IEnumerable<HoursOfOperationModel>>> GetProviderHours();

	IQueryable<ProviderSummary> GetProviderSummaries(Expression<Func<Provider, bool>> filter = null,
		Func<IQueryable<Provider>, IOrderedQueryable<Provider>> orderBy = null,
		Func<IQueryable<Provider>, IIncludableQueryable<Provider, object>> includes = null);


	Task<ProviderSummary> GetProviderSummary(int id,
		Func<IQueryable<Provider>, IIncludableQueryable<Provider, object>> includes = null);

	Task<IEnumerable<HoursOfOperationModel>> GetSingleProviderHours(int providerId, int siteId, int timsDayOfWeek);
}

public class ProvidersUnitOfWork : UnitOfWorkBase, IProvidersUnitOfWork
{
	public ProvidersUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
		httpContextAccessor)
	{
	}

	protected override string TableName => nameof(Provider);

	public async Task<Provider> GetProvider(int id,
		Func<IQueryable<Provider>, IIncludableQueryable<Provider, object>> includes = null)
	{
		return await Single(u => u.Id == id, includes);
	}

	public async Task<ProviderSummary> GetProviderSummary(int id,
		Func<IQueryable<Provider>, IIncludableQueryable<Provider, object>> includes = null)
	{
		var p = await Single(u => u.Id == id, includes);
		return new ProviderSummary
		{
			Id = p.Id,
			Inactive = p.Inactive,
			LastName = p.LastName,
			FirstName = p.FirstName,
			Initial = p.Initial,
			UsePracticeIds = p.UsePracticeIds,
			Degree = p.Degree,
			Deleted = p.Deleted,
			Color = p.Color,
			Npi = p.Npi,
			DisplayOrder = p.DisplayOrder,
			UserId = p.UserId
		};
	}

	public async Task<IEnumerable<HoursOfOperationModel>> GetSingleProviderHours(int providerId, int siteId,
		int timsDayOfWeek)
	{
		// MS Day of week is 0 based, Mon = 0, Sun = 6
		// TIMS is 1 based, Mon = 1, Sun = 7
		var sqlParams = new object[]
		{
			new SqlParameter("@providerId", providerId),
			new SqlParameter("@siteId", siteId),
			new SqlParameter("@dayOfWeek", timsDayOfWeek)
		};

		var hoursOfOperation = await FromSql<HoursOfOperation>(@"
select p.id ProviderID, s.id SiteID, us.DayNum Day, us.StartTime, us.EndTime from Provider p 
join TIMSUser u on u.ID = p.UserIDIs
join TIMSUSerSite us on u.id = us.UID
join site s on s.ID = us.SiteID
where p.Inactive = 0 and s.Inactive = 0
and p.id = @providerId
and s.id = @siteId
and us.DayNum = @dayOfWeek
order by s.id, DayNum, StartTime
", sqlParams).ToListAsync();

		return hoursOfOperation.Select(h => new HoursOfOperationModel
		{
			SiteId = h.SiteId,
			Day = (DayOfWeek)(h.Day % 7),
			StartTime = h.StartTime,
			EndTime = h.EndTime
		});
	}

	public async Task<IDictionary<int, IEnumerable<HoursOfOperationModel>>> GetProviderHours()
	{
		var hoursOfOperation = FromSql<HoursOfOperation>(@"
select p.id ProviderID, s.id SiteID, us.DayNum Day, us.StartTime, us.EndTime from Provider p 
join TIMSUser u on u.ID = p.UserIDIs
join TIMSUSerSite us on u.id = us.UID
join site s on s.ID = us.SiteID
where p.Inactive = 0 and s.Inactive = 0
order by s.id, p.id, DayNum, StartTime
").ToListAsync();

		var providerSiteHours = (from us in await hoursOfOperation
			group us by us.ProviderID
			into grouping
			select new
			{
				ProviderID = grouping.Key,
				SiteData = grouping
			}).ToDictionary(
			x => x.ProviderID,
			x => x.SiteData.Select(sd => new HoursOfOperationModel
			{
				SiteId = sd.SiteId,
				Day = (DayOfWeek)(sd.Day % 7),
				StartTime = sd.StartTime,
				EndTime = sd.EndTime
			}));
		return providerSiteHours;
	}


	public IQueryable<ProviderSummary> GetProviderSummaries(Expression<Func<Provider, bool>> filter = null,
		Func<IQueryable<Provider>, IOrderedQueryable<Provider>> orderBy = null,
		Func<IQueryable<Provider>, IIncludableQueryable<Provider, object>> includes = null)
	{
		return Get(filter, orderBy, includes).Select(p => new ProviderSummary
		{
			Id = p.Id,
			Inactive = p.Inactive,
			LastName = p.LastName,
			FirstName = p.FirstName,
			Initial = p.Initial,
			UsePracticeIds = p.UsePracticeIds,
			Degree = p.Degree,
			Deleted = p.Deleted,
			Color = p.Color,
			Npi = p.Npi,
			DisplayOrder = p.DisplayOrder,
			UserId = p.UserId
		});
	}

	[Keyless]
	public class HoursOfOperation
	{
		public int Day { get; set; }
		public DateTime? EndTime { get; set; }
		public int ProviderID { get; set; }
		public int? SiteId { get; set; }
		public DateTime? StartTime { get; set; }
	}
}