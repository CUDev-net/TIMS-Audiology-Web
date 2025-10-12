using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class PatientTypeModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PatientType>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.Description).HasMaxLength(50);

            entity.Property(e => e.QuickBooksID).HasColumnName("QBID").HasMaxLength(50);

            entity.Property(e => e.DateQuickBooksModified).HasColumnName("DtQBModified");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Protected).IsRequired();

            entity.Property(e => e.InUse).IsRequired();

            entity.Property(e => e.Inactive).IsRequired();

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");
            entity.ToTable("PatientType");
        });
    }
}