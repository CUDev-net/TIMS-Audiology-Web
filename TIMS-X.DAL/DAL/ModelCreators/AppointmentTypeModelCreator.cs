using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class AppointmentTypeModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentType>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.Description).HasMaxLength(50);

            entity.Property(e => e.InUse).IsRequired();

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Duration).HasDefaultValueSql("((30))");

            entity.Property(e => e.HistoryTypeId).HasColumnName("HistoryTypeID");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.ScheduleBlockId).HasColumnName("ScheduleBlockID");

            entity.HasOne(e => e.ScheduleBlock)
                .WithMany()
                .HasForeignKey(k => k.ScheduleBlockId)
                .IsRequired(false);

            entity.Property(e => e.Slp).HasColumnName("SLP");

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");
            entity.Property(e => e.CreatedDate).HasColumnName("InsertedDate");

            entity.ToTable("AppointmentType");
        });
    }
}