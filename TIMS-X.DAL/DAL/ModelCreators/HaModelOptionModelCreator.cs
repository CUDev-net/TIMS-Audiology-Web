using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class HaModelOptionModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HaModelOption>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.Property(e => e.HaOptionId).HasColumnName("HAOptionID");

            entity.Property(e => e.HaModelId).HasColumnName("HAModelID");

            entity.HasOne(e => e.HaModel)
                .WithMany()
                .HasForeignKey(e => e.HaModelId);

            entity.ToTable("HAModelOption");
        });
    }
}