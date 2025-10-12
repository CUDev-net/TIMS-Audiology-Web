using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class BatterySizeModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BatterySize>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.Description).HasMaxLength(50);

            entity.Property(e => e.QbModifiedDate)
                .HasColumnName("DtQBModified")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

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
            entity.ToTable(nameof(BatterySize));
        });
    }
}