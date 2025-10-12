using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class QBPatientBalanceModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<QBPatientBalance>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.HasIndex(e => e.QBID, "IX_QBPatientBalance_QBID");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Balance).HasColumnType("money");

            entity.Property(e => e.DtAcquired)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.QBID)
                .IsRequired()
                .HasMaxLength(50);

            entity.ToTable(nameof(QBPatientBalance));
        });
    }
}