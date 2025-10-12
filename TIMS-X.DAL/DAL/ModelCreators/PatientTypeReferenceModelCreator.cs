using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class PatientTypeReferenceModelCreator : IModelCreator
{
	public void CreateModel(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<PatientTypeReference>(entity =>
		{
			entity.HasKey(
				i => new { i.PatientTypeId, i.PatientId });

			entity.Property(e => e.PatientId);

			entity.Property(e => e.PatientTypeId);

			entity.Property(e => e.CreatedDate);

			entity.ToTable(nameof(PatientTypeReference));
		});
	}
}