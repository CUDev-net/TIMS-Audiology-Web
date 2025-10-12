using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators
{
    public class ProviderModelCreator : IModelCreator
    {
        public void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Provider>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.HasKey(i => i.Id);
                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);

                entity.Ignore(i => i.Hours);
                entity.Ignore(i => i.SiteHours);
                entity.Ignore(i => i.WebColor);

                entity.Property(e => e.AnesthesiaLicNum).HasMaxLength(15);

                entity.Property(e => e.BillToId).HasColumnName("BillToID");

                entity.Property(e => e.BlueShieldNum).HasMaxLength(15);

                entity.Property(e => e.Degree).HasMaxLength(15);

                entity.Property(e => e.QbModifiedDate)
                    .HasColumnName("DtQBModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.SignatureDate).HasColumnName("DtSignature").HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("DtUpdated")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("FName")
                    .HasMaxLength(12)
                    .HasDefaultValueSql("(N'NA')");

                entity.Property(e => e.GroupNum).HasMaxLength(15);

                entity.Property(e => e.Initial).HasMaxLength(1);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("LName")
                    .HasMaxLength(20)
                    .HasDefaultValueSql("('NoneGiven')");

                entity.Property(e => e.MedicaidNum).HasMaxLength(15);

                entity.Property(e => e.MedicareNum).HasMaxLength(15);

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.Npi)
                    .HasColumnName("NPI")
                    .HasMaxLength(15);

                entity.Property(e => e.Qbid)
                    .HasColumnName("QBID")
                    .HasMaxLength(35);

                entity.Property(e => e.Qbid2)
                    .HasColumnName("QBID2")
                    .HasMaxLength(35);

                entity.Property(e => e.SecondaryIdNum)
                    .HasColumnName("SecondaryIDNum")
                    .HasMaxLength(15);

                entity.Property(e => e.SecondaryIdQualifier)
                    .HasColumnName("SecondaryIDQualifier")
                    .HasMaxLength(2);

                entity.Property(e => e.SpecialtyCode).HasMaxLength(50);

                entity.Property(e => e.SpecialtyLicNum).HasMaxLength(15);

                entity.Property(e => e.StateLicNum).HasMaxLength(15);

                entity.Property(e => e.TaxId)
                    .HasColumnName("TaxID")
                    .HasMaxLength(15);

                entity.Property(e => e.TaxIdType)
                    .HasColumnName("TaxIDType")
                    .HasMaxLength(10);

                entity.Property(e => e.Taxonomy).HasMaxLength(10);

                entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

                entity.Property(e => e.Upin)
                    .HasColumnName("UPINNum")
                    .HasMaxLength(6);

                entity.Property(e => e.UsePracticeIds).HasColumnName("UsePracticeIDs");

                entity.Property(e => e.UserId).HasColumnName("UserIDIs");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId);

                entity.Property(e => e.UseForPatientScheduling).HasColumnName("InUse").HasConversion<int>();

				entity.ToTable(nameof(Provider));
            });
        }
    }
}