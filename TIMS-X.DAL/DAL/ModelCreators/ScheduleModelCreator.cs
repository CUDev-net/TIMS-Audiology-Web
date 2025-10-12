using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class ScheduleModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasIndex(e => e.ProviderId);

            entity.HasIndex(e => e.SiteId);

            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);
            entity.Ignore(i => i.Web_Color);
            entity.Ignore(i => i.UpdatedByUserName);
            entity.Ignore(i => i.ProviderIds);
            entity.Ignore(i => i.SiteIds);

            entity.Property(e => e.EndsAt)
                .HasColumnName("ApptEnd")
                .HasColumnType("datetime");

            entity.Property(e => e.StartsAt)
                .HasColumnName("ApptStart")
                .HasColumnType("datetime");

            entity.Property(e => e.CreatedUserId).HasColumnName("CreatedByUserID");

            entity.Property(e => e.UpdatedDate).HasColumnName("DateUpdated").HasColumnType("datetime");

            entity.Property(e => e.CreatedDate)
                .HasColumnName("DtCreated")
                .HasColumnType("datetime");

            entity.Property(e => e.Location).HasMaxLength(35);

            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.Property(e => e.RecurringIntervalId).HasColumnName("RecurringIntervalID");

            entity.Property(e => e.RowVersion)
                .IsRequired()
                .HasColumnName("rowVersion")
                .IsRowVersion();

            entity.Property(e => e.SiteId).HasColumnName("SiteID");

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(35);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UpdatedByUserID");
            entity.Property(e => e.UpdatedSiteId).HasColumnName("UpdatedSiteID");

            entity.HasOne(e => e.RecurringInterval)
                .WithMany()
                .HasForeignKey(e => e.RecurringIntervalId);

            entity.ToTable("Schedule");
        });
    }
}