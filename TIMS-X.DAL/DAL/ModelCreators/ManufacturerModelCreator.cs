using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class ManufacturerModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.AccountNumber).HasMaxLength(100);

            entity.Property(e => e.Address1).HasColumnName("Addr1").HasMaxLength(50);

            entity.Property(e => e.Address2).HasColumnName("Addr2").HasMaxLength(50);

            entity.Property(e => e.City).HasMaxLength(50);

            entity.Property(e => e.Contact).HasMaxLength(50);

            entity.Property(e => e.AgreementDate).HasColumnName("DtAgreement").HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Ext).HasMaxLength(50);

            entity.Property(e => e.Fax).HasMaxLength(50);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Notes).HasMaxLength(255);

            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.Property(e => e.QbAccountId).HasColumnName("QBAccountID");

            entity.Property(e => e.State).HasMaxLength(3);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.Property(e => e.UpdatedUserName)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("('System')");

            entity.Property(e => e.ZipCode).HasMaxLength(50);

            entity.ToTable(nameof(Manufacturer));
        });
    }
}