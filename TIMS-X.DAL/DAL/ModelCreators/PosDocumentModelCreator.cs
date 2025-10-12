using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class PosDocumentModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PosDocument>(entity =>
        {
            entity.HasIndex(e => e.ApplyToId);

            entity.HasIndex(e => e.AppointmentId)
                .HasDatabaseName("IX_POSDocument_AppointmentId");

            entity.HasIndex(e => e.PatientId);

            entity.HasIndex(e => new
                    { e.Id, e.Final, e.Void, e.QbInvoice, e.QbTransactionId, e.QbUpdateDate, e.ProviderId, e.SiteId })
                .HasDatabaseName("POSDoc_QBSync");

            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.PatientId).HasColumnName("PatID");

            entity.Property(e => e.ApplyToId).HasColumnName("ApplyToID");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");

            entity.Property(e => e.ArVoid).HasColumnName("ARVoid");

            entity.Property(e => e.BillTo)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValueSql("('Not Provided')");

            entity.Property(e => e.BillToAddr1)
                .IsRequired()
                .HasMaxLength(41)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.BillToAddr2)
                .IsRequired()
                .HasMaxLength(41)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.BillToAddr3)
                .IsRequired()
                .HasMaxLength(41)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.BillToAddr4)
                .IsRequired()
                .HasMaxLength(41)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.BillToCity)
                .IsRequired()
                .HasMaxLength(31)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.BillToCountry)
                .IsRequired()
                .HasMaxLength(31)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.BillToPostalCode)
                .IsRequired()
                .HasMaxLength(13)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.BillToState)
                .IsRequired()
                .HasMaxLength(21)
                .HasDefaultValueSql("('')");

            entity.Property(e => e.CopayAmount).HasColumnType("money");

            entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

            entity.Property(e => e.CustomerMessageId).HasColumnName("CustomerMessageID");

            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.ArUpdateDate)
                .HasColumnName("DtARUpdate")
                .HasColumnType("datetime");

            entity.Property(e => e.DocumentDate)
                .HasColumnName("DtDocument")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.FormClaimNumber).HasMaxLength(25);

            entity.Property(e => e.InsurancePosItem).HasColumnName("InsurancePOSItem");

            entity.Property(e => e.MarketingId).HasColumnName("MarketingID");

            entity.Property(e => e.Memo).HasMaxLength(1000);

            entity.Property(e => e.Notes).HasMaxLength(250);

            entity.Property(e => e.PdfClaimData).HasColumnName("PDFClaimData");

            entity.Property(e => e.PaymentAmount)
                .HasColumnName("PmtAmount")
                .HasColumnType("money");

            entity.Property(e => e.PaymentMethodId).HasColumnName("PmtMethodID");

            entity.Property(e => e.PaymentReference)
                .HasColumnName("PmtRef")
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.PoNumber)
                .HasColumnName("PONumber")
                .HasMaxLength(25);

            entity.Property(e => e.DocumentType)
                .HasColumnName("DocType");

            entity.Property(e => e.PosDepositId).HasColumnName("POSDepositID");

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.Property(e => e.QbInvoice)
                .HasColumnName("QBInvoice")
                .HasMaxLength(50);

            entity.Property(e => e.QbTransactionId)
                .HasColumnName("QBTxnID")
                .HasMaxLength(50);

            entity.Property(e => e.QbUpdateDate)
                .HasColumnName("QBUpdate")
                .HasColumnType("datetime");

            entity.Property(e => e.RowVersion)
                .IsRequired()
                .HasColumnName("rowVersion")
                .IsRowVersion();

            entity.Property(e => e.SiteId).HasColumnName("SiteID");

            entity.Property(e => e.TaxGroupId).HasColumnName("TaxGroupID");

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.Property(e => e.UpdatedBySiteId).HasColumnName("UpdatedBySiteID");

            entity.HasMany(e => e.PosLines)
                .WithOne().HasForeignKey(p => p.POSDocID);

            entity.ToTable("POSDocument");
        });
    }
}