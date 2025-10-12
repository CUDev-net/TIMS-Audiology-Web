using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators
{
    public class SiteModelCreator : IModelCreator
    {
        public void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Site>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.HasKey(i => i.Id);
                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);
                entity.Ignore(i => i.WebColor);

                entity.Property(e => e.Address1).HasColumnName("Addr1").HasMaxLength(30);

                entity.Property(e => e.Address2).HasColumnName("Addr2").HasMaxLength(30);

                entity.Property(e => e.AllWellId)
                    .HasColumnName("AllWellID")
                    .HasMaxLength(128);

                entity.Property(e => e.CareCreditMerchantNumber).HasMaxLength(64);

                entity.Property(e => e.CareCreditPassword).HasMaxLength(64);

                entity.Property(e => e.CareCreditPracticeCode).HasMaxLength(64);

                entity.Property(e => e.CcpromoCode).HasColumnName("CCPromoCode");

                entity.Property(e => e.City).HasMaxLength(20);

                entity.Property(e => e.DefaultTaxGroupId).HasColumnName("DefaultTaxGroupID");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.QbModifiedDate)
                    .HasColumnName("DtQBModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("DtUpdated")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EcareCreditPaymentId).HasColumnName("ECareCreditPaymentID");

                entity.Property(e => e.EcheckPaymentId).HasColumnName("ECheckPaymentID");

                entity.Property(e => e.EcreditCardPaymentId).HasColumnName("ECreditCardPaymentID");

                entity.Property(e => e.FaxNumber).HasMaxLength(50);

                entity.Property(e => e.FriEnd).HasColumnType("datetime");

                entity.Property(e => e.FriStart).HasColumnType("datetime");

                entity.Property(e => e.MonEnd).HasColumnType("datetime");

                entity.Property(e => e.MonStart).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Npi)
                    .HasColumnName("NPI")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.OutreachEducator).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(15);

                entity.Property(e => e.PracticeId).HasColumnName("PracticeID");

                entity.Property(e => e.Qbid)
                    .HasColumnName("QBID")
                    .HasMaxLength(50);

                entity.Property(e => e.RegionId).HasColumnName("RegionID");

                entity.Property(e => e.SatEnd).HasColumnType("datetime");

                entity.Property(e => e.SatStart).HasColumnType("datetime");

                entity.Property(e => e.SecondaryIdnum)
                    .HasColumnName("SecondaryIDNum")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.SecondaryIdqualifier)
                    .HasColumnName("SecondaryIDQualifier")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SiteSettingId).HasColumnName("SiteSettingID");

                entity.Property(e => e.State).HasMaxLength(3);

                entity.Property(e => e.SunEnd).HasColumnType("datetime");

                entity.Property(e => e.SunStart).HasColumnType("datetime");

                entity.Property(e => e.ThurEnd).HasColumnType("datetime");

                entity.Property(e => e.ThurStart).HasColumnType("datetime");

                entity.Property(e => e.TransnationalAuthKey)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransnationalPassword).HasMaxLength(50);

                entity.Property(e => e.TransnationalUsername).HasMaxLength(50);

                entity.Property(e => e.TuesEnd).HasColumnType("datetime");

                entity.Property(e => e.TuesStart).HasColumnType("datetime");

                entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

                entity.Property(e => e.WedEnd).HasColumnType("datetime");

                entity.Property(e => e.WedStart).HasColumnType("datetime");

                entity.Property(e => e.Zip).HasMaxLength(10);

                entity.HasMany(e => e.Resources)
                    .WithOne()
                    .HasForeignKey(e => e.SiteId);

                entity.Property(e => e.UseForPatientScheduling).HasColumnName("InUse").HasConversion<int>(); ;

				entity.ToTable("Site");
            });
        }
    }
}