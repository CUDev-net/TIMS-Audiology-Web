using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class RecurringIntervalRemovedModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecurringIntervalRemoved>(entity =>
        {
            entity.HasIndex(e => e.RecurringIntervalId);

            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);
            entity.Ignore(i => i.ScheduleId);

            entity.Property(i => i.ItemNumber).HasColumnName("ItemNum");
            entity.Property(e => e.RecurringIntervalId).HasColumnName("RecurringInvervalID");

            entity.ToTable(nameof(RecurringIntervalRemoved));
        });
    }
}