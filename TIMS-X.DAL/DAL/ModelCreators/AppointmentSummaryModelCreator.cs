using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators
{
    internal class AppointmentSummaryModelCreator : IModelCreator
    {
        public void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppointmentSummary>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.HasKey(i => i.Id);
                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);

                entity.Property(p => p.PatientId);
                entity.Property(p => p.AppointmentStatus);
                entity.Property(p => p.AppointmentType);
                entity.Property(p => p.Site);

            });
        }
    }
}
