using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class OutsideFacilityModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutsideFacility>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.Name).HasMaxLength(33).IsRequired();

            entity.Property(e => e.Address1).HasColumnName("Addr1").HasMaxLength(30);

            entity.Property(e => e.Address2).HasColumnName("Addr2").HasMaxLength(30);

            entity.Property(e => e.City).HasMaxLength(20);

            entity.Property(e => e.State).HasMaxLength(3);

            entity.Property(e => e.ZipCode).HasMaxLength(10);

            entity.Property(e => e.Phone).HasMaxLength(15);

            entity.Property(e => e.TaxId).HasColumnName("TaxID").HasMaxLength(15);

            entity.Property(e => e.Fax).HasMaxLength(50);

            entity.Property(e => e.TaxIdType).HasColumnName("TaxIDType").HasMaxLength(1);

            entity.Property(e => e.FacilityType).HasMaxLength(2);

            entity.Property(e => e.SecondaryIdNumber).HasColumnName("SecondaryIDNum").HasMaxLength(15);

            entity.Property(e => e.SecondaryIdQualifier).HasColumnName("SecondaryIDQualifier").HasMaxLength(2);

            entity.Property(e => e.NationalProviderId).HasColumnName("NPI").HasMaxLength(15);

            entity.Property(e => e.AgreementDate)
                .HasColumnName("DtAgreement")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime");

            entity.Property(e => e.Notes).HasMaxLength(255);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.ToTable(nameof(OutsideFacility));
        });
    }
}