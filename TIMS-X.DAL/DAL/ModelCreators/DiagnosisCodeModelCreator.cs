using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class DiagnosisCodeModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiagnosisCode>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime");

            entity.Property(e => e.Name).HasMaxLength(10).IsRequired();

            entity.Property(e => e.Description);

            entity.Property(e => e.Icd10Status).HasColumnName("ICD10Status");

            entity.Property(e => e.IsIcd10).HasColumnName("IsICD10");

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(k => k.CategoryId);

            entity.ToTable(nameof(DiagnosisCode));
        });
    }
}