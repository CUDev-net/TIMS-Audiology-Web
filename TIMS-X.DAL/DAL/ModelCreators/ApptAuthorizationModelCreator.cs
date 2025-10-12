using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class ApptAuthorizationModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApptAuthorization>(entity =>
        {
	        entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
	        entity.HasKey(i => i.Id);
	        entity.Ignore(i => i.PendingDelete);
	        entity.Ignore(i => i.HasStateBeenSet);
	        entity.Ignore(i => i.DisplayString);
	        entity.Ignore(i => i.NumberUsed);
	        entity.Ignore(i => i.IsDeleted);

			entity.Property(e => e.PatientId).HasColumnName("PatID");

			entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

			entity.Property(e => e.UpdatedDate).HasColumnName("DtUpdated");

			entity.ToTable(nameof(ApptAuthorization));
        });
    }
}