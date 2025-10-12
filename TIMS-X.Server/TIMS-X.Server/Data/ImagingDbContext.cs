using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Data;

public class ImagingDbContext : DbContext
{
	private readonly ContextHelper _contextHelper;

	public ImagingDbContext(DbContextOptions<ImagingDbContext> options, ContextHelper contextHelper) : base(options)
	{
		_contextHelper = contextHelper;
	}

	public DbSet<ImageDocumentType> DocumentTypes { get; set; }
	public DbSet<ImageServer> ImageServers { get; set; }

	public DbSet<PatientImage> PatientImages { get; set; }
	public DbSet<PatientLetterArchive> PatientLetterArchives { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ModelConfigurator.Configure(modelBuilder, typeof(PatientImage));
		ModelConfigurator.Configure(modelBuilder, typeof(ImageServer));
		ModelConfigurator.Configure(modelBuilder, typeof(ImageDocumentType));
		ModelConfigurator.Configure(modelBuilder, typeof(PatientLetterArchive));
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
			if (entity is PatientImage img)
			{
				if (img.Id == Guid.Empty) img.Id = Guid.NewGuid();
				img.CreatedDate = DateTime.Now;
				img.UpdatedUserId = _contextHelper.CurrentUser?.Id ?? 0;
				if (img.Notes == null) img.Notes = string.Empty;
			}

			if (entity is ImageDocumentType docType)
			{
				if (docType.Id == Guid.Empty) docType.Id = Guid.NewGuid();
				docType.CreatedDate = DateTime.Now;
				docType.UpdatedUserId = _contextHelper.CurrentUser?.Id ?? 0;
			}
		}

		return base.SaveChanges();
	}
}