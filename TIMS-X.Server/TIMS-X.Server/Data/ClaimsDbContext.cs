using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Data;

public class ClaimsDbContext : DbContext
{
	public ClaimsDbContext(DbContextOptions<ClaimsDbContext> options) : base(options)
	{
	}

	public DbSet<ClaimTransaction> ClaimTransactions { get; set; }

	public DbSet<PosDocument> PosDocuments { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ModelConfigurator.Configure(modelBuilder, typeof(PosDocument));
		ModelConfigurator.Configure(modelBuilder, typeof(ClaimTransaction));
	}
}