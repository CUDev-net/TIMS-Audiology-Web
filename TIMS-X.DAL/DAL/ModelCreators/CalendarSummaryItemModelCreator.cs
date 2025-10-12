using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class CalendarSummaryItemModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalendarSummaryItem>(entity =>
        {
            entity.HasKey(i => i.ItemId);
            entity.Ignore(i => i.RemovedInstances);

            entity.Ignore(i => i.AppointmentTypeWebColor);
            entity.Ignore(i => i.ProviderWebColor);
            entity.Ignore(i => i.SiteWebColor);

            entity.ToTable(nameof(CalendarSummaryItem));
        });
    }
}