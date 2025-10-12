using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Data;

public class SchedulerDbContext : DbContext
{
	public SchedulerDbContext(DbContextOptions<SchedulerDbContext> options) : base(options)
	{
	}

	public DbSet<Appointment> Appointments { get; set; }
	public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
	public DbSet<AppointmentType> AppointmentTypes { get; set; }
	public DbSet<EmailLog> EmailLogs { get; set; }
	public DbSet<EmailTracking> EmailTrackings { get; set; }
	public DbSet<MessageTemplate> MessageTemplates { get; set; }
	public DbSet<Patient> Patients { get; set; }
	public DbSet<Provider> Providers { get; set; }
	public DbSet<RecurringDayQualifier> RecurringDayQualifiers { get; set; }
	public DbSet<RecurringDayType> RecurringDayTypes { get; set; }
	public DbSet<RecurringInterval> RecurringIntervals { get; set; }
	public DbSet<RecurringIntervalRemoved> RecurringIntervalsRemoved { get; set; }
	public DbSet<RecurringMonth> RecurringMonths { get; set; }
	public DbSet<Resource> Resources { get; set; }
	public DbSet<Schedule> Schedules { get; set; }
	public DbSet<Site> Sites { get; set; }
	public DbSet<SmsLog> SmsLogs { get; set; }
	public DbSet<SmsTracking> SmsTrackings { get; set; }
	public DbSet<VoiceCallLog> VoiceCallLogs { get; set; }
	public DbSet<VoiceCallTracking> VoiceCallTrackings { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Ignore<HoursOfOperationModel>();
		ModelConfigurator.Configure(modelBuilder, typeof(Appointment));
		ModelConfigurator.Configure(modelBuilder, typeof(AppointmentType));
		ModelConfigurator.Configure(modelBuilder, typeof(AppointmentStatus));
		ModelConfigurator.Configure(modelBuilder, typeof(Schedule));
		ModelConfigurator.Configure(modelBuilder, typeof(Patient));
		ModelConfigurator.Configure(modelBuilder, typeof(Resource));
		ModelConfigurator.Configure(modelBuilder, typeof(Site));
		ModelConfigurator.Configure(modelBuilder, typeof(Provider));
		ModelConfigurator.Configure(modelBuilder, typeof(RecurringDayType));
		ModelConfigurator.Configure(modelBuilder, typeof(RecurringDayQualifier));
		ModelConfigurator.Configure(modelBuilder, typeof(RecurringInterval));
		ModelConfigurator.Configure(modelBuilder, typeof(RecurringIntervalRemoved));
		ModelConfigurator.Configure(modelBuilder, typeof(ApptRecurringInterval));
		ModelConfigurator.Configure(modelBuilder, typeof(ApptRecurringIntervalRemoved));
		ModelConfigurator.Configure(modelBuilder, typeof(RecurringMonth));
		ModelConfigurator.Configure(modelBuilder, typeof(VoiceCallLog));
		ModelConfigurator.Configure(modelBuilder, typeof(EmailLog));
		ModelConfigurator.Configure(modelBuilder, typeof(SmsLog));
		ModelConfigurator.Configure(modelBuilder, typeof(VoiceCallTracking));
		ModelConfigurator.Configure(modelBuilder, typeof(SmsTracking));
		ModelConfigurator.Configure(modelBuilder, typeof(EmailTracking));
		ModelConfigurator.Configure(modelBuilder, typeof(MessageTemplate));
	}
}