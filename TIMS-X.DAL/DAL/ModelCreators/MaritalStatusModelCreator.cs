using Microsoft.EntityFrameworkCore;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class MaritalStatusModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Core.Domain.MaritalStatus>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.ToTable("MaritalStatus");
        });
    }
}