using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class NdmSearchPointModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NdmSearchPoint>(entity =>
        {
            entity.HasKey(e => e.Id)
                .IsClustered(false);

            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);

            entity.Property(e => e.SearchCriteriaId).HasColumnName("NDMSearchCriteriaID");

            entity.HasOne(d => d.SearchCriteria)
                .WithMany(p => p.SearchPoints)
                .HasForeignKey(d => d.SearchCriteriaId);

            entity.ToTable("NDMSearchPoint");
        });
    }
}