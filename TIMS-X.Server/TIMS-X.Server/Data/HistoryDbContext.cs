using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Data;

public class HistoryDbContext : DbContext
{
	public HistoryDbContext(DbContextOptions<HistoryDbContext> options) : base(options)
	{
	}

	public DbSet<History> Histories { get; set; }
	public DbSet<Patient> Patients { get; set; }
	public DbSet<Practice> Practices { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ModelConfigurator.Configure(modelBuilder, typeof(History));
		ModelConfigurator.Configure(modelBuilder, typeof(Practice));
		ModelConfigurator.Configure(modelBuilder, typeof(Patient));
	}
}