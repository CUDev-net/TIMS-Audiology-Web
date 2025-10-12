using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

public class UserTaskModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DateUpdated")
                .HasColumnType("datetime");

            entity.Property(e => e.CreatedDate)
                .IsRequired()
                .HasColumnName("DtCreated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.CompletedDate)
                .HasColumnType("datetime")
                .HasColumnName("DtComplete");

            entity.Property(e => e.DueDate)
                .HasColumnType("datetime")
                .HasColumnName("DtDue");

            entity.Property(e => e.Task).IsRequired();

            entity.Property(e => e.UserTaskTypeId).HasColumnName("TIMSUserTaskTypeID");

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.HasOne(e => e.UserTaskType)
                .WithMany()
                .HasForeignKey(e => e.UserTaskTypeId);

            entity.ToTable("TIMSUserTask");
        });
    }
}