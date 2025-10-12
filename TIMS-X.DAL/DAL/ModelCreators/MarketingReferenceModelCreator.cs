using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class MarketingReferenceModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MarketingReference>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.Cost).HasColumnType("money");

            entity.Property(e => e.Description).HasMaxLength(50);

            entity.Property(e => e.EndDate).HasColumnName("DtEnd").HasColumnType("datetime");

            entity.Property(e => e.ReviewDate).HasColumnName("DtReview").HasColumnType("datetime");

            entity.Property(e => e.StartDate).HasColumnName("DtStart").HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime");

            entity.Property(e => e.CategoryId).HasColumnName("MktCategoryID");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Notes).HasMaxLength(4000);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.HasMany(e => e.Sites)
                .WithOne()
                .HasForeignKey(e => e.SiteId);

            entity.ToTable("MktReference");
        });
    }
}