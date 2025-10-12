using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class NdmMeasurementConditionModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NdmMeasurementCondition>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);

            entity.Property(e => e.HearingInstrument1Condition).HasColumnName("HearingInstrument_1_Condition");

            entity.Property(e => e.HearingInstrument2Condition).HasColumnName("HearingInstrument_2_Condition");

            entity.ToTable("NDMMeasurementCondition");
        });
    }
}