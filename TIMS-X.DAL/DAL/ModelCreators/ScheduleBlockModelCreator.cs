using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class ScheduleBlockModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduleBlock>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);
            entity.Ignore(i => i.color_web);
            entity.Ignore(i => i.AppointmentTypes);

            entity.Property(e => e.CreatedDate)
                .HasColumnName("DateCreated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DateModified")
                .HasColumnType("datetime");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.RowVersion)
                .IsRequired()
                .HasColumnName("rowVersion")
                .IsRowVersion();

            entity.Property(e => e.UpdatedUserId).HasColumnName("UpdatedByUserID");

            entity.HasMany(e => e.ProviderBlockSchedules)
                .WithOne(e => e.ScheduleBlock)
                .HasForeignKey(e => e.ScheduleBlockId);

            entity.ToTable(nameof(ScheduleBlock));
        });
    }
}