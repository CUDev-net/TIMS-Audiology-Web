using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class ProviderBlockScheduleModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderBlockSchedule>(entity =>
        {
            entity.HasKey(e => e.Id)
                .IsClustered(false);

            entity.HasIndex(e => new { e.ProviderId, e.ScheduleBlockId, e.ScheduleTimeSlotId })
                .IsClustered();

            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.ScheduleBlockId).HasColumnName("BlockID");

            entity.Property(e => e.CreatedDate)
                .HasColumnName("DateCreated")
                .HasColumnType("datetime");

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.Property(e => e.RowVersion)
                .IsRequired()
                .HasColumnName("rowVersion")
                .IsRowVersion();

            entity.Property(e => e.ScheduleTimeSlotId).HasColumnName("TimeSlotID");

            entity.HasOne(e => e.ScheduleBlock)
                .WithMany()
                .HasForeignKey(e => e.ScheduleBlockId);

            entity.HasOne(e => e.ScheduleTimeSlot)
                .WithMany()
                .HasForeignKey(e => e.ScheduleTimeSlotId);

            entity.ToTable("ProviderBlockReference");
        });

        // Always return the time slot
        modelBuilder.Entity<ProviderBlockSchedule>().Navigation(e => e.ScheduleTimeSlot).AutoInclude();
    }
}