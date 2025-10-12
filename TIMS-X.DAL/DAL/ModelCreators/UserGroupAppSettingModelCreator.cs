using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators
{
    internal class UserGroupAppSettingModelCreator : IModelCreator
    {
        public void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroupAppSetting>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.HasKey(i => i.Id);
                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);
            });
        }
    }
}
