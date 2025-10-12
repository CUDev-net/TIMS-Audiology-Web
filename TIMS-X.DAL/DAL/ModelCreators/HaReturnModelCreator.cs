using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class HaReturnModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HaReturn>(entity =>
        {
            entity.HasIndex(e => e.HaHistoryId);

            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.ReturnDate)
                .HasColumnName("DtReturn")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.ReturnedToManufacturerDate).HasColumnName("DtReturnedToManu").HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.HaHistoryId).HasColumnName("HAHistoryID");

            entity.Property(e => e.HaReturnReasonId).HasColumnName("HAReturnReasonID");

            entity.Property(e => e.Notes).HasMaxLength(255);

            entity.Property(e => e.PatientId).HasColumnName("PatID");

            entity.Property(e => e.SyncSiteId).HasColumnName("SyncSiteID");

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.HasOne(e => e.ReturnReason)
               .WithMany()
               .HasForeignKey(e => e.HaReturnReasonId);

            entity.ToTable("HAReturn");
        });
    }
}