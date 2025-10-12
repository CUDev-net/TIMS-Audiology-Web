using Microsoft.EntityFrameworkCore;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class PatientSearchResultModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PatientSearchResult>(entity =>
        {
            entity.Property(e => e.LastName)
                .HasMaxLength(35);
        });
    }
}