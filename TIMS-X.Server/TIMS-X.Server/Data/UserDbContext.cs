using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Data;

public class UserDbContext : DbContext
{
	public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
	{
	}

	public DbSet<Alert> Alerts { get; set; }

	public DbSet<LastPatientList> LastPatientLists { get; set; }
	public DbSet<Patient> Patients { get; set; }
	public DbSet<Site> Sites { get; set; }
	public DbSet<UserSiteHours> TIMSUserSites { get; set; }
	public DbSet<UserGroupAppSetting> UserGroupAppSettings { get; set; }
	public DbSet<UserGroupReference> UserGroupReferences { get; set; }
	public DbSet<UserGroup> UserGroups { get; set; }


	public DbSet<User> Users { get; set; }
	public DbSet<UserTask> UserTasks { get; set; }
	public DbSet<UserTaskUserReference> UserTasksUserReferences { get; set; }

	public DbSet<UserTaskType> UserTaskTypes { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Ignore<HoursOfOperationModel>();
		ModelConfigurator.Configure(modelBuilder, typeof(User));
		ModelConfigurator.Configure(modelBuilder, typeof(UserGroup));
		ModelConfigurator.Configure(modelBuilder, typeof(UserGroupAppSetting));
		ModelConfigurator.Configure(modelBuilder, typeof(UserGroupReference));
		ModelConfigurator.Configure(modelBuilder, typeof(Patient));
		ModelConfigurator.Configure(modelBuilder, typeof(Site));
		ModelConfigurator.Configure(modelBuilder, typeof(LastPatientList));
		ModelConfigurator.Configure(modelBuilder, typeof(UserSiteHours));
		ModelConfigurator.Configure(modelBuilder, typeof(UserTask));
		ModelConfigurator.Configure(modelBuilder, typeof(UserTaskType));
		ModelConfigurator.Configure(modelBuilder, typeof(UserTaskUserReference));
		ModelConfigurator.Configure(modelBuilder, typeof(Alert));
	}
}