using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class MedicationModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Medication>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Inactive).IsRequired();

            entity.Property(e => e.UpdatedUserId)
                .HasColumnName("UpdatedByUserID")
                .IsRequired();

            entity.Property(e => e.Description).HasMaxLength(100);

            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .IsRequired()
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .IsRequired()
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.IsOtotoxic).IsRequired();

            entity.Property(e => e.ForDryMouth).IsRequired();

            entity.Property(e => e.ForPsychologicalIssuesCausingStutter).IsRequired();

            entity.ToTable(nameof(Medication));
        });
    }
}