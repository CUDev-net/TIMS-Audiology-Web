using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators
{
    public class PracticeModelCreator : IModelCreator
    {
        public void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Practice>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.HasKey(i => i.Id);
                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);

                entity.Property(e => e.BusinessRules).IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("DtUpdated")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedUserId)
                    .HasColumnName("UID");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Fax).HasMaxLength(50);

                entity.Property(e => e.InactivityTimeout).HasDefaultValueSql("((60))");

                entity.Property(e => e.Logo).HasColumnType("image");

                entity.Property(e => e.UseSiteAddressForReports).HasColumnName("ReportAddr");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("('Not Provided')");

                entity.Property(e => e.Npi)
                    .HasColumnName("NPI")
                    .HasMaxLength(15);

                entity.Property(e => e.TaxId)
                    .HasColumnName("TaxID")
                    .HasMaxLength(15);

                entity.Property(e => e.LastName)
                    .HasColumnName("LName")
                    .HasMaxLength(20);
                entity.Property(e => e.FirstName)
                    .HasColumnName("FName")
                    .HasMaxLength(12);

                entity.Property(e => e.BillingAddress1).HasColumnName("PayToAddr1").HasMaxLength(30);

                entity.Property(e => e.BillingAddress2).HasColumnName("PayToAddr2").HasMaxLength(30);

                entity.Property(e => e.BillingCity).HasColumnName("PayToCity").HasMaxLength(20);

                entity.Property(e => e.BillingPhoneNumber).HasColumnName("PayToPhone").HasMaxLength(50);

                entity.Property(e => e.BillingState).HasColumnName("PayToState").HasMaxLength(3);

                entity.Property(e => e.BillingZipCode).HasColumnName("PayToZipCode").HasMaxLength(10);

                entity.Property(e => e.LinkAppointmentHistory).HasColumnName("LinkApptHist");

                entity.Property(e => e.UsesAdAuthentication).HasColumnName("UseADAuthentication");

                entity.Ignore(e => e.EmailDisclaimer);
                entity.Ignore(e => e.Locale);

                entity.Property(e => e.QbLocale)
                    .IsRequired()
                    .HasColumnName("QBLocale")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('US')");

                entity.Property(e => e.OfficeCode)
                    .HasColumnName("WebOfficeCode")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.WebServer)
                    .HasColumnName("WebApptServer")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.TimsServer)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.ToTable("Practice");
            });
        }
    }
}