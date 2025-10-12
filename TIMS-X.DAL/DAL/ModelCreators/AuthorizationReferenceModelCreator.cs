using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class AuthorizationReferenceModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthorizationReference>(entity =>
        {
			entity.HasKey(
				i => new {i.AuthorizationId, i.PatientId});

			entity.Property(e => e.PatientId);

			entity.Property(e => e.AuthorizationId);

			entity.Property(e => e.CreatedDate);

			entity.ToTable(nameof(AuthorizationReference));
        });
    }
}