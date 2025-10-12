using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class AppointmentModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);
            entity.Ignore(i => i.UpdatedByUserName);
            entity.Ignore(i => i.OpportunityDescription);

            entity.HasIndex(e => e.ProviderId);

            entity.HasIndex(e => e.SiteId);

            entity.HasIndex(e => new { e.Id, e.SiteId, e.PatientId, e.ProviderId })
                .HasDatabaseName("Appt_QBSync");

            entity.Property(e => e.EndsAt)
                .HasColumnName("ApptEnd")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.StartsAt)
                .HasColumnName("ApptStart")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.AppointmentStatusId).HasColumnName("ApptStatusID");

            entity.Property(e => e.AppointmentTypeId).HasColumnName("ApptTypeID");

            entity.Property(e => e.CreatedUserId).HasColumnName("CreatedByUserID");

            entity.Property(e => e.CreatedDate)
                .HasColumnName("DtCreated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.NextContactDate).HasColumnName("DtNextContact").HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.MarketingId).HasColumnName("MarketingID");

            entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");

            entity.Property(e => e.OtStatus).HasColumnName("OTStatus");

            entity.Property(e => e.PatientId).HasColumnName("PatID");

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.Property(e => e.RecurringIntervalId).HasColumnName("RecurringIntervalID");

            entity.Property(e => e.RecurringParentId).HasColumnName("RecurringParentID");

            entity.Property(e => e.ReferringPhysicianId).HasColumnName("ReferralSourceID");

            entity.Property(e => e.ResourceId).HasColumnName("ResourceID");

            entity.Property(e => e.SiteId).HasColumnName("SiteID");

            entity.Property(e => e.SyncSiteId).HasColumnName("SyncSiteID");

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.Property(e => e.UpdatedSiteId).HasColumnName("UpdatedSiteID");

            entity.HasOne(e => e.AppointmentType)
                .WithMany()
                .HasForeignKey(e => e.AppointmentTypeId);

            entity.HasOne(e => e.AppointmentStatus)
                .WithMany()
                .HasForeignKey(e => e.AppointmentStatusId);

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId);

            entity.HasOne(e => e.Provider)
                .WithMany()
                .HasForeignKey(e => e.ProviderId);

            entity.HasOne(e => e.Site)
                .WithMany()
                .HasForeignKey(e => e.SiteId);

            entity.HasMany(e => e.PosDocuments)
                .WithOne()
                .HasForeignKey(e => e.AppointmentId);

            entity.HasOne(e => e.RecurringInterval)
                .WithMany()
                .HasForeignKey(e => e.RecurringIntervalId)
                .IsRequired(false);

            entity.HasOne(e => e.OtStatusDescription)
                .WithMany()
                .HasForeignKey(e => e.OtStatusDescriptionId)
                .IsRequired(false);
            
            entity.ToTable("Appointment");
        });
    }
}