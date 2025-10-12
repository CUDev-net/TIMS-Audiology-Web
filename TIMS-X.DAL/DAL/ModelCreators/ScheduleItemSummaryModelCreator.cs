using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class ScheduleItemSummaryModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduleItemSummary>(entity =>
        {
            entity.HasKey(i => i.id);

            entity.Ignore(i => i.color_web);
            entity.Ignore(i => i.provider_web_color);
            entity.Ignore(i => i.site_web_color);

            entity.ToTable("vw_web_scheduler_schedules");
        });
    }
}