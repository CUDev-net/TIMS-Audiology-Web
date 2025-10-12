using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class LastPatientListModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LastPatientList>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.UserId).HasColumnName("UID");
            entity.Property(e => e.PatientListXml).HasColumnName("Patients").HasMaxLength(200);
            entity.ToTable(nameof(LastPatientList));
        });
    }
}