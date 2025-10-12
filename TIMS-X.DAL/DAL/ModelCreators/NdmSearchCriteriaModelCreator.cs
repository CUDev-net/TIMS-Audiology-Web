using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class NdmSearchCriteriaModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NdmSearchCriteria>(entity =>
        {
            entity.HasKey(e => e.Id)
                .IsClustered(false);

            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);

            entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.Property(e => e.IsBaha).HasColumnName("IsBAHA");

            entity.Property(e => e.IsBc).HasColumnName("IsBC");

            entity.Property(e => e.IsMcl).HasColumnName("IsMCL");

            entity.Property(e => e.IsUcl).HasColumnName("IsUCL");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.SeverityId).HasColumnName("SeverityID");

            entity.Property(e => e.TypeofLossId).HasColumnName("TypeofLossID");

            entity.Property(e => e.UpdatedByUserId).HasColumnName("UpdatedByUserID");

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.Property(e => e.UsedForOpportunityTracking).HasColumnName("UsedForOT");

            entity.ToTable("NDMSearchCriteria");
        });
    }
}