using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class InsurancePayerModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InsurancePayer>(entity =>
        {
            entity.ToTable("InsuranceCarrier");

            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);


            entity.Property(e => e.Address1).HasColumnName("Addr1").HasMaxLength(30);

            entity.Property(e => e.Address2).HasColumnName("Addr2").HasMaxLength(30);

            entity.Property(e => e.CarrierCode).HasMaxLength(128);

            entity.Property(e => e.City).HasMaxLength(20);

            entity.Property(e => e.ClaimOfficeNum)
                .HasMaxLength(128)
                .IsUnicode(false);

            entity.Property(e => e.Contact).HasMaxLength(30);

            entity.Property(e => e.DtQbModified)
                .HasColumnType("datetime")
                .HasColumnName("DtQBModified");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime");

            entity.Property(e => e.Email).HasMaxLength(99);

            entity.Property(e => e.EmcProviderId)
                .HasMaxLength(15)
                .HasColumnName("EMCProviderID");

            entity.Property(e => e.Extension).HasMaxLength(30);

            entity.Property(e => e.Fax).HasMaxLength(30);

            entity.Property(e => e.InsuranceType).HasMaxLength(15);

            entity.Property(e => e.IsIcd10).HasColumnName("IsICD10");

            entity.Property(e => e.MipsRequired).HasColumnName("MIPSRequired");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Notes).HasMaxLength(255);

            entity.Property(e => e.Npi)
                .HasMaxLength(15)
                .HasColumnName("NPI");

            entity.Property(e => e.OrganizationId)
                .HasMaxLength(5)
                .HasColumnName("OrganizationID");

            entity.Property(e => e.Phone).HasMaxLength(30);

            entity.Property(e => e.ProviderId)
                .HasMaxLength(15)
                .HasColumnName("ProviderID");

            entity.Property(e => e.QbId)
                .HasMaxLength(50)
                .HasColumnName("QBID");

            entity.Property(e => e.SecondaryId)
                .HasMaxLength(15)
                .HasColumnName("SecondaryID");

            entity.Property(e => e.SecondaryIdQualifier)
                .HasMaxLength(2)
                .HasColumnName("SecondaryIDQualifier");

            entity.Property(e => e.State).HasMaxLength(3);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.Property(e => e.ZipCode).HasMaxLength(10);
        });
    }
}