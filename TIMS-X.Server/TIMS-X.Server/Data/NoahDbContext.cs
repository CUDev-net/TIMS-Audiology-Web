using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Domain.Noah;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Data;

public class NoahDbContext : DbContext
{
	public NoahDbContext(DbContextOptions<NoahDbContext> options) : base(options)
	{
	}

	public DbSet<InsurancePayer> InsurancePayers { get; set; }
	public DbSet<MarketingReference> MarketingReferences { get; set; }
	public DbSet<N4ActionArchive> N4ActionArchives { get; set; }
	public DbSet<N4ActionReference> N4ActionReferences { get; set; }
	public DbSet<N4Action> N4Actions { get; set; }
	public DbSet<N4AppPermission> N4AppPermissions { get; set; }
	public DbSet<N4DashboardAlertArchive> N4DashboardAlertArchives { get; set; }
	public DbSet<N4DashboardAlert> N4DashboardAlerts { get; set; }
	public DbSet<N4ManualTymp> N4ManualTymps { get; set; }

	public DbSet<N4ManufacturerSetup> N4ManufacturerSetups { get; set; }
	public DbSet<N4MobileAppPermission> N4MobileAppPermissions { get; set; }
	public DbSet<N4MobileApp> N4MobileApps { get; set; }
	public DbSet<N4PatientIdentification> N4PatientIdentifications { get; set; }
	public DbSet<N4PatientSetup> N4PatientSetups { get; set; }
	public DbSet<N4Preference> N4Preferences { get; set; }
	public DbSet<N4Session> N4Sessions { get; set; }
	public DbSet<N4UnboundActionArchive> N4UnboundActionArchives { get; set; }
	public DbSet<N4UnboundActionReference> N4UnboundActionReferences { get; set; }
	public DbSet<N4UnboundAction> N4UnboundActions { get; set; }
	public DbSet<N4UserSetup> N4UserSetups { get; set; }

	public DbSet<NdmAction> NdmActions { get; set; }
	public DbSet<NdmAudiogram> NdmAudiograms { get; set; }
	public DbSet<NdmMeasurementCondition> NdmMeasurementConditions { get; set; }
	public DbSet<NdmSearchCriteria> NdmSearchCriterias { get; set; }
	public DbSet<NdmSearchPoint> NdmSearchPoints { get; set; }
	public DbSet<NdmTonePoint> NdmTonePoints { get; set; }
	public DbSet<Patient> Patients { get; set; }
	public DbSet<Practice> Practices { get; set; }
	public DbSet<Provider> Providers { get; set; }
	public DbSet<Salutation> Salutations { get; set; }
	public DbSet<User> Users { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ModelConfigurator.Configure(modelBuilder, typeof(Practice));
		ModelConfigurator.Configure(modelBuilder, typeof(N4Action));
		ModelConfigurator.Configure(modelBuilder, typeof(N4ActionArchive));
		ModelConfigurator.Configure(modelBuilder, typeof(N4ActionReference));
		ModelConfigurator.Configure(modelBuilder, typeof(N4AppPermission));
		ModelConfigurator.Configure(modelBuilder, typeof(N4ManualTymp));
		ModelConfigurator.Configure(modelBuilder, typeof(N4ManufacturerSetup));
		ModelConfigurator.Configure(modelBuilder, typeof(N4MobileApp));
		ModelConfigurator.Configure(modelBuilder, typeof(N4MobileAppPermission));
		ModelConfigurator.Configure(modelBuilder, typeof(N4PatientIdentification));
		ModelConfigurator.Configure(modelBuilder, typeof(N4PatientSetup));
		ModelConfigurator.Configure(modelBuilder, typeof(N4Preference));
		ModelConfigurator.Configure(modelBuilder, typeof(N4Session));
		ModelConfigurator.Configure(modelBuilder, typeof(N4UnboundAction));
		ModelConfigurator.Configure(modelBuilder, typeof(N4UnboundActionArchive));
		ModelConfigurator.Configure(modelBuilder, typeof(N4UnboundActionReference));
		ModelConfigurator.Configure(modelBuilder, typeof(N4UserSetup));
		ModelConfigurator.Configure(modelBuilder, typeof(N4DashboardAlert));
		ModelConfigurator.Configure(modelBuilder, typeof(N4DashboardAlertArchive));
		ModelConfigurator.Configure(modelBuilder, typeof(Patient));
		ModelConfigurator.Configure(modelBuilder, typeof(Salutation));
		ModelConfigurator.Configure(modelBuilder, typeof(InsurancePayer));
		ModelConfigurator.Configure(modelBuilder, typeof(MarketingReference));
		ModelConfigurator.Configure(modelBuilder, typeof(User));
		ModelConfigurator.Configure(modelBuilder, typeof(Provider));
		ModelConfigurator.Configure(modelBuilder, typeof(NdmAction));
		ModelConfigurator.Configure(modelBuilder, typeof(NdmAudiogram));
		ModelConfigurator.Configure(modelBuilder, typeof(NdmMeasurementCondition));
		ModelConfigurator.Configure(modelBuilder, typeof(NdmSearchCriteria));
		ModelConfigurator.Configure(modelBuilder, typeof(NdmSearchPoint));
		ModelConfigurator.Configure(modelBuilder, typeof(NdmTonePoint));
	}
}