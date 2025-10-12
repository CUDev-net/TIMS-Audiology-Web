using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class SubmitterInfoModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubmitterInfo>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.Name).HasMaxLength(128).IsRequired();

            entity.Property(e => e.Address1).HasMaxLength(35);

            entity.Property(e => e.Address2).HasMaxLength(35);

            entity.Property(e => e.City).HasMaxLength(35);

            entity.Property(e => e.State).HasMaxLength(3);

            entity.Property(e => e.ZipCode).HasMaxLength(12);

            entity.Property(e => e.Description).HasMaxLength(128);

            entity.Property(e => e.Phone).HasMaxLength(25);

            entity.Property(e => e.TaxId).HasColumnName("TaxID").HasMaxLength(15);

            entity.Property(e => e.TaxIdType).HasColumnName("TaxIDType").HasMaxLength(1);

            entity.Property(e => e.NationalProviderId).HasColumnName("NPI").HasMaxLength(15);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime");

            entity.Property(e => e.Notes).HasMaxLength(255);

            entity.Property(e => e.CreatedUserId).HasColumnName("CreatedByUserID");

            entity.Property(e => e.UpdatedUserId).HasColumnName("UpdatedByUserID");

            entity.ToTable(nameof(SubmitterInfo));
        });
    }
}