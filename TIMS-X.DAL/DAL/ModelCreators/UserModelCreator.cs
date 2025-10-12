using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators
{
    public class UserModelCreator : IModelCreator
    {
        public void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.HasKey(i => i.Id);
                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);

                entity.Property(e => e.ScheduleProviderFilter)
                    .HasColumnName("ApptProvider")
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ScheduleResourceFilter)
                    .HasColumnName("ApptResource")
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ScheduleSiteFilter)
                    .HasColumnName("ApptSite")
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ScheduleSpecialtyFilter)
                    .HasColumnName("ApptSpecialty")
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('0')");
                entity.Property(e => e.PasswordChangedDate).HasColumnName("DatePasswordLastChanged")
                    .HasColumnType("datetime");
                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("DtUpdated")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Initials)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('UNK')");

                entity.Property(e => e.MobilePhone)
                    .HasMaxLength(15);

                entity.Property(e => e.Email)
                    .HasMaxLength(99);

                entity.Property(e => e.RequireMFA);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('Unknown')");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("('password')");

                entity.Property(e => e.SiteId).HasColumnName("SiteID");
                entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

                entity.Property(e => e.UserSettings).IsUnicode(false);

                entity.Property(e => e.AdDomain).HasColumnName("ADDomain");

                entity.Property(e => e.AdUsername).HasColumnName("ADUsername");
                entity.Property(e => e.Deleted).HasColumnName("deleted");
                entity.Property(e => e.CalendarInterval).HasColumnName("CalInterval");

                entity.Ignore(e => e.Jwt);
                entity.HasMany(e => e.SiteHours)
                    .WithOne()
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.UserReferences)
                    .WithOne()
                    .HasForeignKey(e => e.UserId);

                entity.ToTable("TIMSUser");
            });
        }
    }
}