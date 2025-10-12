using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface ITimsUserSiteUnitOfWork : IUnitOfWork
{
    Task<UserSiteHours> GetUserDayAndSite(int userId, int dayNumber, int siteId);

    IEnumerable<UserSiteHours> GetUserSites(List<int> userIds,
        Func<IQueryable<UserSiteHours>, IOrderedQueryable<UserSiteHours>> orderBy = null,
        Func<IQueryable<UserSiteHours>, IIncludableQueryable<UserSiteHours, object>> includes = null);
}

public class TimsUserSiteUnitOfWork : UnitOfWorkBase, ITimsUserSiteUnitOfWork
{
    public TimsUserSiteUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "TIMSUserSite";

    public IEnumerable<UserSiteHours> GetUserSites(List<int> userIds,
        Func<IQueryable<UserSiteHours>, IOrderedQueryable<UserSiteHours>> orderBy = null,
        Func<IQueryable<UserSiteHours>, IIncludableQueryable<UserSiteHours, object>> includes = null)
    {
        return Get(p => userIds.Contains(p.UserId), orderBy, includes);
    }

    public async Task<UserSiteHours> GetUserDayAndSite(int userId, int dayNumber, int siteId)
    {
        return await Single<UserSiteHours>(p => p.UserId == userId && p.SiteId == siteId && p.DayNum == dayNumber);
    }
}