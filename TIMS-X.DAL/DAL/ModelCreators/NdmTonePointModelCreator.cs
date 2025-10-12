using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class NdmTonePointModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NdmTonePoint>(entity =>
        {
            entity.HasKey(e => e.Id)
                .IsClustered(false);

            entity.HasIndex(e => e.AudiogramId)
                .IsClustered();

            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);

            entity.Property(e => e.AudiogramId).HasColumnName("NDMAudiogramID");

            entity.HasOne(d => d.Audiogram)
                .WithMany(p => p.TonePoints)
                .HasForeignKey(d => d.AudiogramId);

            entity.ToTable("NDMTonePoint");
        });
    }
}