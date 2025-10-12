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

public interface ISiteUnitOfWork : IUnitOfWork
{
    Task<Site> GetSite(int id);

    Task<List<Site>> GetSites(Expression<Func<Site, bool>> filter = null,
        Func<IQueryable<Site>, IOrderedQueryable<Site>> orderBy = null,
        Func<IQueryable<Site>, IIncludableQueryable<Site, object>> includes = null);

    Task<List<Site>> GetSiteSummaries(Expression<Func<Site, bool>> filter = null,
        Func<IQueryable<Site>, IOrderedQueryable<Site>> orderBy = null);
}

public class SiteUnitOfWork : UnitOfWorkBase, ISiteUnitOfWork
{
    public SiteUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(Site);

    public async Task<Site> GetSite(int id)
    {
        return await Single<Site>(u => u.Id == id);
    }

    public async Task<List<Site>> GetSites(Expression<Func<Site, bool>> filter = null,
        Func<IQueryable<Site>, IOrderedQueryable<Site>> orderBy = null,
        Func<IQueryable<Site>, IIncludableQueryable<Site, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }

    public async Task<List<Site>> GetSiteSummaries(Expression<Func<Site, bool>> filter = null,
        Func<IQueryable<Site>, IOrderedQueryable<Site>> orderBy = null)
    {
        var siteSummaries = Get(filter, orderBy).Select(x => new Site
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Inactive = x.Inactive,
            Address1 = x.Address1,
            Address2 = x.Address2,
            Color = x.Color,
            City = x.City,
            State = x.State,
            Zip = x.Zip,
            Phone = x.Phone,
            FaxNumber = x.FaxNumber,
            MonStart = x.MonStart,
            MonEnd = x.MonEnd,
            TuesStart = x.TuesStart,
            TuesEnd = x.TuesEnd,
            WedStart = x.WedStart,
            WedEnd = x.WedEnd,
            ThurStart = x.ThurStart,
            ThurEnd = x.ThurEnd,
            FriStart = x.FriStart,
            FriEnd = x.FriEnd,
            SatStart = x.SatStart,
            SatEnd = x.SatEnd,
            SunStart = x.SunStart,
            SunEnd = x.SunEnd
        });
        return await siteSummaries.ToListAsync();
    }
}