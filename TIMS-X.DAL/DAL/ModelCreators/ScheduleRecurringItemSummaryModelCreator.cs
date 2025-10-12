using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class ScheduleRecurringItemSummaryModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduleRecurringItemSummary>(entity =>
        {
            entity.HasKey(i => i.id);
            entity.Ignore(i => i.RemovedInstances);
            entity.Ignore(i => i.DeletedOccurrences);

            entity.Ignore(i => i.color_web);
            entity.Ignore(i => i.provider_web_color);
            entity.Ignore(i => i.site_web_color);

            entity.Ignore(i => i.rec_type);
            entity.Ignore(i => i.event_length);
            entity.Ignore(i => i.event_pid);

            entity.ToTable("vw_web_scheduler_recurring_schedules");
        });
    }
}