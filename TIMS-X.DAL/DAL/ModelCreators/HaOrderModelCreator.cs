using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class HaOrderModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<HaOrder>(entity =>
        {
            entity.HasIndex(e => e.PatientId);

            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.Cost).HasColumnType("money");

            entity.Property(e => e.CreatedUserId).HasColumnName("CreatedByUserID");

            entity.Property(e => e.CreatedDate)
                .HasColumnName("DateCreated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.ExpectedInDate)
                .HasColumnName("DtExpectedIn")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.OrderDate)
                .HasColumnName("DtOrdered")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.ReceivedDate)
                .HasColumnName("DtReceived")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.HaHistoryId).HasColumnName("HAHistoryID");

            entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");

            entity.Property(e => e.Notes).HasMaxLength(255);

            entity.Property(e => e.OrderNumber).HasMaxLength(50);

            entity.Property(e => e.PatientId).HasColumnName("PatID");

            entity.Property(e => e.PdfOrderData).HasColumnName("PDFOrderData");

            entity.Property(e => e.PdfOrderNumber)
                .HasColumnName("PDFOrderNumber")
                .HasMaxLength(25);

            entity.Property(e => e.Price).HasColumnType("money");

            entity.Property(e => e.SyncSiteId).HasColumnName("SyncSiteID");

            entity.Property(e => e.TrackingNumber).HasMaxLength(50);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.Property(e => e.UpdatedSiteId).HasColumnName("UpdatedBySiteID");

            entity.ToTable("HAOrder");
        });
    }
}