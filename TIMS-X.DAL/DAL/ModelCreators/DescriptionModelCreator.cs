using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class DescriptionModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Description>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.UpdatedUserId)
	            .HasColumnName("UID");
            entity.Property(e => e.UpdatedDate)
	            .HasColumnName("DtUpdated");

			entity.Property(e => e.CustomDate1Label)
                .HasColumnName("MiscDtLegend1");
            entity.Property(e => e.CustomDate2Label)
	            .HasColumnName("MiscDtLegend2");

            entity.Property(e => e.CustomText1Label)
	            .HasColumnName("MiscTextLegend1");
            entity.Property(e => e.CustomText2Label)
	            .HasColumnName("MiscTextLegend2");

			entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime");

           entity.ToTable(nameof(Description));
        });
    }
}