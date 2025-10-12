using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class UserSiteModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSiteHours>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.EndTime).HasColumnType("datetime");

            entity.Property(e => e.SiteId).HasColumnName("SiteID");

            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.Property(e => e.UserId).HasColumnName("UID");

            entity.ToTable("TIMSUserSite");
        });
    }
}