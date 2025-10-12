using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class ScriptedNoteModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScriptedNote>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime");

            entity.Property(e => e.InsertedDate).HasColumnName("InsertDate");

            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(k => k.CategoryId);

            entity.ToTable(nameof(ScriptedNote));
        });
    }
}