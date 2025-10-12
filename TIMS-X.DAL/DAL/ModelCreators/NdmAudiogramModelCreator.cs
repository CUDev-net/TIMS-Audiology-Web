using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class NdmAudiogramModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NdmAudiogram>(entity =>
        {
            entity.HasKey(e => e.Id)
                .IsClustered(false);

            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);

            entity.Property(e => e.ActionId).HasColumnName("NDMActionID");

            entity.Property(e => e.MeasurementConditionId).HasColumnName("NDMMeasurementConditionID");

            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.HasOne(d => d.MeasurementCondition)
                .WithMany()
                .HasForeignKey(d => d.MeasurementConditionId);

            entity.HasOne(d => d.Action)
                .WithMany(p => p.Audiograms)
                .HasForeignKey(d => d.ActionId);

            entity.ToTable("NDMAudiogram");
        });
    }
}