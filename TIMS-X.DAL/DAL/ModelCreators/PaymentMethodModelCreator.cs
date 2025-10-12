using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class PaymentMethodModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.DateQuickBookModified)
                .HasColumnName("DtQBModified")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime");

            entity.Property(e => e.QuickBookId)
                .HasMaxLength(50)
                .HasColumnName("QBID");

            entity.Property(e => e.PaymentGroupId).HasColumnName("PaymentGroupID");

            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();

            entity.Property(e => e.Description).HasMaxLength(50);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.ToTable(nameof(PaymentMethod));
        });
    }
}