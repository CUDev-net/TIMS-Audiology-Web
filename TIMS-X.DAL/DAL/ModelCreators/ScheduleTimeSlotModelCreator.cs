using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class ScheduleTimeSlotModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduleTimeSlot>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.CreatedDate)
                .HasColumnName("DateCreated")
                .HasColumnType("datetime");

            entity.Property(e => e.EndTime).HasColumnType("datetime");

            entity.Property(e => e.RowVersion)
                .IsRequired()
                .HasColumnName("rowVersion")
                .IsRowVersion();

            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.ToTable(nameof(ScheduleTimeSlot));
        });
    }
}