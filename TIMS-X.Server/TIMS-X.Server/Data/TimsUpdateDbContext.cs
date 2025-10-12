using Microsoft.EntityFrameworkCore;
using TIMS_X.Server.Config;
using Version = TIMS_X.Core.Domain.Version;

namespace TIMS_X.Server.Data;

public class TimsUpdateDbContext : DbContext
{
	public TimsUpdateDbContext(DbContextOptions<TimsUpdateDbContext> options) : base(options)
	{
	}

	public DbSet<Version> Versions { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ModelConfigurator.Configure(modelBuilder, typeof(Version));
	}
}