using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class ResultTypeModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResultType>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Protected).IsRequired();

            entity.Property(e => e.Inactive).IsRequired();

            entity.Property(e => e.InUse).IsRequired();

            entity.Property(e => e.UpdatedUserId)
                .HasColumnName("UID")
                .IsRequired();

            entity.Property(e => e.Description).HasMaxLength(50);

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.ToTable(nameof(ResultType));
        });
    }
}