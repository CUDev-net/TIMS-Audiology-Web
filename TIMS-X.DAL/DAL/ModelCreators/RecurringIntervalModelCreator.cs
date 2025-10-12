using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class RecurringIntervalModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecurringInterval>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);
            entity.Ignore(i => i.HasBeenAudited);

            entity.Property(e => e.EndDate)
                .HasColumnName("DtEnd")
                .HasColumnType("datetime");

            entity.Property(e => e.StartDate)
                .HasColumnName("DtStart")
                .HasColumnType("datetime");

            entity.Property(e => e.DayInterval).HasColumnName("T1Days");

            entity.Property(e => e.IsFridaySet).HasColumnName("T2Friday");

            entity.Property(e => e.IsMondaySet).HasColumnName("T2Monday");

            entity.Property(e => e.IsSaturdaySet).HasColumnName("T2Saturday");

            entity.Property(e => e.IsSundaySet).HasColumnName("T2Sunday");

            entity.Property(e => e.IsThursdaySet).HasColumnName("T2Thursday");

            entity.Property(e => e.IsTuesdaySet).HasColumnName("T2Tuesday");

            entity.Property(e => e.IsWednesdaySet).HasColumnName("T2Wednesday");

            entity.Property(e => e.WeekInterval).HasColumnName("T2WeekCnt");

            entity.Property(e => e.DayOfMonth).HasColumnName("T34DayNum");

            entity.Property(e => e.DayQualifier).HasColumnName("T34DayQualID");

            entity.Property(e => e.DayOfWeek).HasColumnName("T34DayTypeID");

            entity.Property(e => e.MonthInterval).HasColumnName("T3MonthCnt");

            entity.Property(e => e.Month).HasColumnName("T4MonthID");

            entity.HasMany(e => e.DeletedOccurrences)
                .WithOne()
                .HasForeignKey(e => e.RecurringIntervalId);

            entity.ToTable(nameof(RecurringInterval));
        });
    }
}