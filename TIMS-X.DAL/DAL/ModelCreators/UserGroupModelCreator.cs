using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators
{
    internal class UserGroupModelCreator : IModelCreator
    {
        public void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.HasKey(i => i.Id);
                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);
                
                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.Property(e => e.UpdatedUserId).HasColumnName("UpdatedByUserID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasMany(e => e.Settings)
                    .WithOne()
                    .HasForeignKey(e => e.GroupId);

                entity.HasMany(e => e.UserReferences)
                    .WithOne( e => e.UserGroup)
                    .HasForeignKey(e => e.GroupId);

                entity.ToTable(nameof(UserGroup));
            });
        }
    }
}
