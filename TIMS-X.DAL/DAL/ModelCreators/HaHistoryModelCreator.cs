using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class HaHistoryModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HaHistory>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.HasIndex(e => e.PatientId);
            entity.Property(e => e.PatientId).HasColumnName("PatID");

            entity.HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey(p => p.PatientId);

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");

            entity.HasOne(a => a.Appointment)
                .WithMany()
                .HasForeignKey(a => a.AppointmentId);

            entity.Property(e => e.BatterySizeId).HasColumnName("BatterySizeID");

            entity.Property(e => e.Cost).HasColumnType("money");

            entity.Property(e => e.CrMemoId).HasColumnName("CrMemoID");

            entity.Property(e => e.CreatedUserId).HasColumnName("CreatedByUserID");

            entity.Property(e => e.Discount).HasColumnType("money");

            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.IsEarmold).HasColumnName("Earmold");

            entity.Property(e => e.CreatedDate)
                .HasColumnName("DtCreated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.FitDate).HasColumnName("DtFit").HasColumnType("datetime");

            entity.Property(e => e.LastPurchaseDate).HasColumnName("DtLastPurchase").HasColumnType("datetime");

            entity.Property(e => e.LossDamageDate).HasColumnName("DtLossDamage").HasColumnType("datetime");

            entity.Property(e => e.OrigWarrantyDate).HasColumnName("DtOrigWarranty").HasColumnType("datetime");

            entity.Property(e => e.PurchaseDate).HasColumnName("DtPurchase").HasColumnType("datetime");

            entity.Property(e => e.QbUpdateDate)
                .HasColumnName("DtQBUpdate")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.WarrantyDate).HasColumnName("DtWarranty").HasColumnType("datetime");

            entity.Property(e => e.HaModelId).HasColumnName("HAModelID");

            entity.Property(e => e.HaOrderId).HasColumnName("HAOrderID");

            entity.HasOne(o => o.HaOrder)
                .WithMany()
                .HasForeignKey(o => o.HaOrderId);

            entity.Property(e => e.HaStatusId).HasColumnName("HAStatusID");

            entity.Property(e => e.HaStockItemId).HasColumnName("HAStockItemID");

            entity.HasOne(s => s.HaStockItem)
                .WithMany()
                .HasForeignKey(s => s.HaStockItemId);

            entity.Property(e => e.HaStyleId).HasColumnName("HAStyleID");

            entity.HasOne(s => s.HaStyle)
                .WithMany()
                .HasForeignKey(s => s.HaStyleId);

            entity.Property(e => e.Invoice).HasMaxLength(50);

            entity.Property(e => e.IsCros).HasColumnName("IsCROS");

            entity.Property(e => e.LdWarrantyTypeId).HasColumnName("LDWarrantyTypeID");

            entity.Property(e => e.Notes).HasMaxLength(4000);

            entity.Property(e => e.OriginalId).HasColumnName("OriginalID");

            entity.Property(e => e.OtherSideId).HasColumnName("OtherSideID");

            entity.Property(e => e.PosDocId).HasColumnName("POSDocID");

            entity.Property(e => e.Price).HasColumnType("money");

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.HasOne(p => p.Provider)
                .WithMany()
                .HasForeignKey(p => p.ProviderId);

            entity.Property(e => e.QbInvoice)
                .HasColumnName("QBInvoice")
                .HasMaxLength(50);

            entity.Property(e => e.SerialNumber).HasMaxLength(50);

            entity.Property(e => e.SyncSiteId).HasColumnName("SyncSiteID");

            entity.HasOne(s => s.Site)
                .WithMany()
                .HasForeignKey(s => s.SyncSiteId);

            entity.Property(e => e.UdiNumber)
                .HasColumnName("UDINumber")
                .HasMaxLength(50);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.Property(e => e.UpdatedSiteId).HasColumnName("UpdatedSiteID");

            entity.Property(e => e.WarrantyTypeId).HasColumnName("WarrantyTypeID");

            entity.HasOne(b => b.BatterySize)
                .WithMany()
                .HasForeignKey(b => b.BatterySizeId);

            entity.HasOne(w => w.WarrantyType)
                .WithMany()
                .HasForeignKey(w => w.WarrantyTypeId);

            entity.HasOne(e => e.Status)
                .WithMany()
                .HasForeignKey(e => e.HaStatusId)
                .IsRequired(false);

            entity.HasOne(e => e.Model)
                .WithMany()
                .HasForeignKey(e => e.HaModelId)
                .IsRequired(false);

            entity.HasOne(e => e.HaOrder)
                .WithMany()
                .HasForeignKey(e => e.HaOrderId)
                .IsRequired(false);

            entity.ToTable("HAHistory");
        });
    }
}