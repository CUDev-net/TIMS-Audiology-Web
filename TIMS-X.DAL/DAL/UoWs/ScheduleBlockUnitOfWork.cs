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

public interface IScheduleBlockUnitOfWork : IUnitOfWork
{
    Task<List<ProviderBlockOpening>> GetBlockOpenings(int appointmentTypeId);
    Task<ScheduleBlock> GetScheduleBlock(int id);

    Task<List<ScheduleBlock>> GetScheduleBlocks(Expression<Func<ScheduleBlock, bool>> filter = null,
        Func<IQueryable<ScheduleBlock>, IIncludableQueryable<ScheduleBlock, object>> includes = null);
}

public class ScheduleBlockUnitOfWork : UnitOfWorkBase, IScheduleBlockUnitOfWork
{
    public ScheduleBlockUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(ScheduleBlock);

    public async Task<ScheduleBlock> GetScheduleBlock(int id)
    {
        return await Single<ScheduleBlock>(u => u.Id == id);
    }

    public async Task<List<ScheduleBlock>> GetScheduleBlocks(Expression<Func<ScheduleBlock, bool>> filter = null,
        Func<IQueryable<ScheduleBlock>, IIncludableQueryable<ScheduleBlock, object>> includes = null)
    {
        return await Get(filter, null, includes).ToListAsync();
    }

    public async Task<List<ProviderBlockOpening>> GetBlockOpenings(int appointmentTypeId)
    {
        var sqlParams = new object[]
        {
            new SqlParameter("@appointmentTypeId", appointmentTypeId)
        };

        var blockOpenings = FromSql<ProviderBlockOpening>(@"
SELECT [MAINVIEW].*,
    [STS].[DayOfWeek] AS [DayOfWeek], 
    [STS].[EndTime] AS [EndTime], 
    [STS].[StartTime] AS [StartTime]
    FROM   (SELECT [PBR].[BlockID] AS [BlockId], [PBR].[TimeSlotID] AS [TimeSlotId], [Join1].[ProviderId], [Join1].[Initial], 
        [Join1].[FirstName], [Join1].[LastName], SITE.ID as SiteId, SITE.NAME as SiteName, [Join1].StartTime1 as SiteStartTime, [Join1].EndTime1 as SiteEndTime
        FROM    [dbo].[ProviderBlockReference] AS [PBR]
        INNER JOIN  (SELECT [PROVIDER].[ID] AS [ProviderId], [PROVIDER].[FName] AS [FirstName], [PROVIDER].[LName] AS [LastName], [PROVIDER].[Initial] AS [Initial], [PROVIDER].[UserIDIs] AS [UserIDIs], [TUS].[ID] AS [ID3], [TUS].[DayNum] AS [DayNum], [TUS].[EndTime] AS [EndTime1], [TUS].[SiteID] AS [SiteID], [TUS].[StartTime] AS [StartTime1], [TUS].[UID] AS [UID3]
            FROM  [dbo].[PROVIDER]
            INNER JOIN [dbo].[TIMSUSerSite] AS [TUS] ON [PROVIDER].[UserIDIs] = [TUS].[UID] ) AS [Join1] ON [PBR].[ProviderID] = [Join1].[ProviderId]
        INNER JOIN [dbo].[ScheduleTimeSlot] AS [Extent4] ON ([Join1].[DayNum] = [Extent4].[DayOfWeek]) AND ([PBR].[TimeSlotID] = [Extent4].[ID])
        INNER JOIN [dbo].[Site] AS [SITE] ON [Join1].[SiteID] = [SITE].[ID]
		WHERE ([Join1].[StartTime1] IS NOT NULL) AND ([Join1].[EndTime1] IS NOT NULL) ) AS [MAINVIEW]
    LEFT OUTER JOIN [dbo].[ScheduleTimeSlot] AS [STS] ON [MAINVIEW].[TimeSlotID] = [STS].[ID]
    WHERE  EXISTS (SELECT 
        1 AS [C1]
        FROM [dbo].[AppointmentType] AS [Extent7]
        WHERE ([Extent7].[ScheduleBlockID] IS NOT NULL) AND 
([MAINVIEW].[BlockId] = [Extent7].[ScheduleBlockID]) AND ([Extent7].[ID] = @appointmentTypeId))", sqlParams)
            .ToListAsync();

        return await blockOpenings;
    }
}

[Keyless]
public class ProviderBlockOpening
{
    public int ProviderId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Initial { get; set; }
    public int BlockId { get; set; }
    public int TimeSlotId { get; set; }
    public int SiteId { get; set; }
    public string SiteName { get; set; }
    public int DayOfWeek { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime? SiteStartTime { get; set; }
    public DateTime? SiteEndTime { get; set; }


}