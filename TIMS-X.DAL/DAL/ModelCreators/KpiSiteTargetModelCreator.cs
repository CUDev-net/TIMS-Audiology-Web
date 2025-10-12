using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class KpiSiteTargetModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KpiSiteTarget>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime");

            entity.Property(e => e.StartDate)
                .HasColumnType("datetime");

            entity.Property(e => e.SiteId).HasColumnName("SiteID");

            entity.HasOne(e => e.Site)
                .WithMany()
                .HasForeignKey(e => e.SiteId);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UpdatedByUserID");

            entity.ToTable("KPISiteTarget");
        });
    }
}