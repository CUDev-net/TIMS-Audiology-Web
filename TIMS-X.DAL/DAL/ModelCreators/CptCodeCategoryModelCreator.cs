using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class CptCodeCategoryModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CptCodeCategory>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.CreatedDate)
                .HasColumnName("DateCreated")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DateModified")
                .HasColumnType("datetime");

            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();

            entity.Property(e => e.Description).HasMaxLength(255);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UpdatedByUserID");

            entity.ToTable("CPTCodeCategory");
        });
    }
}