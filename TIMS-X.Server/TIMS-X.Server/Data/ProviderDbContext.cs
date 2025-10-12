using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Data;

public class ProviderDbContext : DbContext
{
	public ProviderDbContext(DbContextOptions<ProviderDbContext> options) : base(options)
	{
	}

	public DbSet<AppointmentType> AppointmentTypes { get; set; }
	public DbSet<EmailLog> EmailLogs { get; set; }
	public DbSet<MessageTemplate> MessageTemplates { get; set; }

	public DbSet<ProviderBlockSchedule> ProviderBlockSchedules { get; set; }
	public DbSet<Provider> Providers { get; set; }
	public DbSet<ScheduleBlock> ScheduleBlocks { get; set; }
	public DbSet<ScheduleTimeSlot> ScheduleTimeSlots { get; set; }
	public DbSet<SmsLog> SmsLogs { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<UserSiteHours> UserSiteHoursTable { get; set; }
	public DbSet<VoiceCallLog> VoiceCallLogs { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ModelConfigurator.Configure(modelBuilder, typeof(Provider));
		ModelConfigurator.Configure(modelBuilder, typeof(User));
		ModelConfigurator.Configure(modelBuilder, typeof(UserSiteHours));
		ModelConfigurator.Configure(modelBuilder, typeof(ProviderBlockSchedule));
		ModelConfigurator.Configure(modelBuilder, typeof(ScheduleBlock));
		ModelConfigurator.Configure(modelBuilder, typeof(ScheduleTimeSlot));
		ModelConfigurator.Configure(modelBuilder, typeof(AppointmentType));
		ModelConfigurator.Configure(modelBuilder, typeof(MessageTemplate));
		ModelConfigurator.Configure(modelBuilder, typeof(VoiceCallLog));
		ModelConfigurator.Configure(modelBuilder, typeof(EmailLog));
		ModelConfigurator.Configure(modelBuilder, typeof(SmsLog));
	}
}