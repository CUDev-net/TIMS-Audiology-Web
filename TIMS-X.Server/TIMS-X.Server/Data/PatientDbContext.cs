using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Data;

public class PatientDbContext : DbContext
{
	public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
	{
	}

	public DbSet<CommunicationRestriction> CommunicationRestrictions { get; set; }
	public DbSet<InsurancePayer> InsurancePayers { get; set; }
	public DbSet<MedicalCondition> MedicalConditions { get; set; }
	public DbSet<PatientInsurance> PatientInsurances { get; set; }
	public DbSet<PatientRestriction> PatientRestrictions { get; set; }

	public DbSet<Patient> Patients { get; set; }
	public DbSet<PreviousHistory> PreviousHistories { get; set; }
	public DbSet<MarketingReference> ReferringPhysicians { get; set; }
	public DbSet<SmsLog> SmsLogs { get; set; }
	public DbSet<SmsReply> SmsReplies { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Ignore<HoursOfOperationModel>();
		ModelConfigurator.Configure(modelBuilder, typeof(Patient));
		ModelConfigurator.Configure(modelBuilder, typeof(CommunicationRestriction));
		ModelConfigurator.Configure(modelBuilder, typeof(PatientRestriction));
		ModelConfigurator.Configure(modelBuilder, typeof(MedicalCondition));
		ModelConfigurator.Configure(modelBuilder, typeof(PreviousHistory));
		ModelConfigurator.Configure(modelBuilder, typeof(SmsLog));
		ModelConfigurator.Configure(modelBuilder, typeof(SmsReply));
		ModelConfigurator.Configure(modelBuilder, typeof(PatientInsurance));
		ModelConfigurator.Configure(modelBuilder, typeof(InsurancePayer));
		ModelConfigurator.Configure(modelBuilder, typeof(MarketingReference));
	}
}