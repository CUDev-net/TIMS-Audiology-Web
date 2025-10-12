using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class UserGroupReferenceModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserGroupReference>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.GroupId).HasColumnName("GroupID");

            entity.HasOne(e => e.UserGroup)
                .WithMany( e => e.UserReferences)
                .HasForeignKey(e => e.GroupId);
        });

    }
}