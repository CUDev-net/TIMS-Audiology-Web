using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class NdmActionModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NdmAction>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);

            entity.Property(e => e.ActionId).HasColumnName("ActionID");

            entity.Property(e => e.AudiogramDate).HasColumnType("datetime");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.ToTable("NDMAction");
        });
    }
}