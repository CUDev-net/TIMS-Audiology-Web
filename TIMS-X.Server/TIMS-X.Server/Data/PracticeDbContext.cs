using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Data;

public class PracticeDbContext : DbContext
{
	public PracticeDbContext(DbContextOptions<PracticeDbContext> options) : base(options)
	{
	}

	public DbSet<Alert> Alerts { get; set; }
	public DbSet<Area> Areas { get; set; }
	public DbSet<EmplStatus> EmploymentStatuses { get; set; }
	public DbSet<MaritalStatus> MaritalStatuses { get; set; }
	public DbSet<MessageSettings> MessageSettingsTable { get; set; }

	public DbSet<PatientReportTemplate> PatientReportTemplates { get; set; }
	public DbSet<Practice> Practices { get; set; }
	public DbSet<Resource> Resources { get; set; }
	public DbSet<Sex> Sexes { get; set; }

	public DbSet<Site> Sites { get; set; }

	public DbSet<User> Users { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Ignore<HoursOfOperationModel>();
		ModelConfigurator.Configure(modelBuilder, typeof(Practice));
		ModelConfigurator.Configure(modelBuilder, typeof(Site));
		ModelConfigurator.Configure(modelBuilder, typeof(MessageSettings));
		ModelConfigurator.Configure(modelBuilder, typeof(Resource));
		ModelConfigurator.Configure(modelBuilder, typeof(Alert));
		ModelConfigurator.Configure(modelBuilder, typeof(Area));
		ModelConfigurator.Configure(modelBuilder, typeof(PatientReportTemplate));
		ModelConfigurator.Configure(modelBuilder, typeof(User));
		ModelConfigurator.Configure(modelBuilder, typeof(MaritalStatus));
		ModelConfigurator.Configure(modelBuilder, typeof(EmplStatus));
		ModelConfigurator.Configure(modelBuilder, typeof(Sex));
	}
}