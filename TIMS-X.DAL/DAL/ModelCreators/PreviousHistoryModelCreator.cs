using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class PreviousHistoryModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PreviousHistory>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.Protected).IsRequired();

            entity.Property(e => e.PatientId).HasColumnName("PatID");

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId);

            entity.HasOne(e => e.Condition)
                .WithMany()
                .HasForeignKey(e => e.ConditionId);

            entity.ToTable(nameof(PreviousHistory));
        });
    }
}