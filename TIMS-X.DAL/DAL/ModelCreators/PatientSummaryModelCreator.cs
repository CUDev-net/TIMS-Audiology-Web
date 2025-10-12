using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class PatientSummaryModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PatientSummary>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.HasNoKey();
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Ignore(i => i.PhoneToDisplay);
            entity.Ignore(i => i.NextAppointmentDate);
            entity.Ignore(i => i.LastAppointmentDate);
            entity.Ignore(i => i.LastAppointmentStatus);
			entity.Ignore(i => i.CurrentLeftHearingAid);
            entity.Ignore(i => i.CurrentRightHearingAid);
            entity.Ignore(i => i.NextAppointmentStatus);

			entity.Property(e => e.LastName)
                .HasMaxLength(35);
        });
    }
}