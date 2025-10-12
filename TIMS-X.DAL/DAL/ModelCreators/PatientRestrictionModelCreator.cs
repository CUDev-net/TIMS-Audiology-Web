using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class PatientRestrictionModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PatientRestriction>(entity =>
        {
            entity.HasIndex(e => e.PatientId)
                .HasDatabaseName("IX_PatientRestriction_PatID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);


            entity.Property(e => e.PatientId).HasColumnName("PatID");

            entity.Property(e => e.RestrictionId).HasColumnName("RestrictionID");

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Restrictions)
                .HasForeignKey(e => e.PatientId);

            entity.HasOne(e => e.CommunicationRestriction)
                .WithMany()
                .HasForeignKey(e => e.RestrictionId);

            entity.ToTable(nameof(PatientRestriction));
        });
    }
}