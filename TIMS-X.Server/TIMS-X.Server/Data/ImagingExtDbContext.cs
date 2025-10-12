using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core;
using TIMS_X.Core.Domain.Imaging;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Data;

public class ImagingExtDbContext : DbContext
{
	private readonly ContextHelper _contextHelper;

	public ImagingExtDbContext(DbContextOptions<ImagingExtDbContext> options, ContextHelper contextHelper) :
		base(options)
	{
		_contextHelper = contextHelper;
	}

	public DbSet<TimsArchive> TimsArchives { get; set; }

	public DbSet<TimsImage> TimsImages { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ModelConfigurator.Configure(modelBuilder, typeof(TimsImage));
		ModelConfigurator.Configure(modelBuilder, typeof(TimsArchive));
	}

	public override int SaveChanges()
	{
		ChangeTracker.DetectChanges();
		var added = ChangeTracker.Entries()
			.Where(t => t.State == EntityState.Added)
			.Select(t => t.Entity)
			.ToArray();

		foreach (var entity in added)
		{
			if (entity is TimsImage img)
			{
				if (img.Id == Guid.Empty) img.Id = Guid.NewGuid();
				img.DateCreated = DateTime.Now;
				img.UpdatedUserId = _contextHelper.CurrentUser?.Id ?? 0;
			}

			if (entity is TimsArchive archive)
			{
				archive.CreatedDate = DateTime.Now;
				archive.UpdatedUserId = _contextHelper.CurrentUser?.Id ?? 0;
			}
		}

		return base.SaveChanges();
	}
}