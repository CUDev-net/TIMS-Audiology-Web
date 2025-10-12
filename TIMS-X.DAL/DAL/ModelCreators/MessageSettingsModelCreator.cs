using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class MessageSettingsModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MessageSettings>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

			entity.Property(e => e.AccountSid)
				.HasColumnName("AccountSID")
				.HasMaxLength(128);

			entity.Property(e => e.AuthorizationToken).HasMaxLength(128);

			entity.Property(e => e.UpdatedDate).HasColumnName("DtUpdated").HasColumnType("datetime");

			entity.Property(e => e.FromEmailAddress).HasMaxLength(100);

			entity.Property(e => e.FromPhoneNumber).HasMaxLength(20);

			entity.Property(e => e.FromSmsNumber)
				.HasColumnName("FromSMSNumber")
				.HasMaxLength(20);

			entity.Property(e => e.InitiateVoiceCallAt).HasColumnType("datetime");

			entity.Property(e => e.IsVoiceEnabled).HasDefaultValueSql("((0))");

			entity.Property(e => e.SendEmailAt).HasColumnType("datetime");

			entity.Property(e => e.SendSmsAt).HasColumnType("datetime");

			entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

			entity.ToTable(nameof(MessageSettings));
		});
    }
}