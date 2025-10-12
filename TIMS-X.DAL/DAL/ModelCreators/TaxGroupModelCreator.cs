using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class TaxGroupModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaxGroup>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.DateQuickBookModified)
                .HasColumnName("DtQBModified");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated");

            entity.Property(e => e.QuickBookId)
                .HasColumnName("QBID");

            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();

            entity.Property(e => e.Description).HasMaxLength(50);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.HasMany(x => x.TaxItems)
                .WithMany(x => x.TaxGroups)
                .UsingEntity<TaxGroupAssignment>(
                    x => x.HasOne(ti => ti.TaxItem)
                        .WithMany().HasForeignKey("ItemID"),
                    x => x.HasOne(tg => tg.TaxGroup)
                        .WithMany().HasForeignKey("GroupID"));

            entity.ToTable("POSTaxGroup");
        });
    }
}