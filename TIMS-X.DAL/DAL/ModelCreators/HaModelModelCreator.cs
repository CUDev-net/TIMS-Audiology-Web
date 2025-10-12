using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class HaModelModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HaModel>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.AvailableForSale)
                .IsRequired()
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.BatterySizeId).HasColumnName("BatterySizeID");

            entity.Property(e => e.CatalogModelId).HasColumnName("CatalogModelID");

            entity.Property(e => e.Cost).HasColumnType("money");

            entity.Property(e => e.Description).HasMaxLength(50);

            entity.Property(e => e.QbModifiedDate)
                .HasColumnName("DtQBModified")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.HaStyleId).HasColumnName("HAStyleID");

            entity.Property(e => e.HaTypeId).HasColumnName("HATypeID");

            entity.Property(e => e.IsAccessory).HasDefaultValueSql("((0))");

            entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(24);

            entity.Property(e => e.PosItemTypeId).HasColumnName("POSItemTypeID");

            entity.Property(e => e.Price).HasColumnType("money");

            entity.Property(e => e.QbAcctId)
                .HasColumnName("QBAcctID")
                .HasMaxLength(50);

            entity.Property(e => e.QbId)
                .HasColumnName("QBID")
                .HasMaxLength(50);

            entity.Property(e => e.QbTypeId).HasColumnName("QBTypeID");

            entity.Property(e => e.TaxGroupId).HasColumnName("TaxGroupID");

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.HasOne(e => e.BatterySize)
                .WithMany()
                .HasForeignKey(e => e.BatterySizeId)
                .IsRequired(false);

            entity.HasOne(e => e.Manufacturer)
                .WithMany()
                .HasForeignKey(e => e.ManufacturerId)
                .IsRequired(false);

            entity.HasOne(e => e.Style)
                .WithMany()
                .HasForeignKey(e => e.HaStyleId)
                .IsRequired(false);

            entity.HasOne(e => e.HaType)
                .WithMany()
                .HasForeignKey(e => e.HaTypeId)
                .IsRequired(false);

            entity.ToTable("HAModel");
        });
    }
}