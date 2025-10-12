using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Server.Data;
using Version = TIMS_X.Core.Domain.Version;

namespace TIMS_X.Server.Queries;

public class TimsUpdateQuery
{
	private readonly TimsUpdateDbContext _dbContext;


	public TimsUpdateQuery(TimsUpdateDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<string> GetCurrentVersionAsync()
	{
		//string result = null;
		var version = await _dbContext.Versions
			.Where(v => v.ApplyUpdate)
			.OrderByDescending(v => v.VersionNumber)
			.Select(v => v.VersionNumber)
			.FirstOrDefaultAsync();
		return version;
	}

	public async Task<List<Version>> GetVersionHistoryAsync()
	{
		var versions = await _dbContext.Versions.Where(v => v.ApplyUpdate).ToListAsync();
		return versions;
	}

	public async Task InstallNewVersionAsync(string versionNumber)
	{
		var version = new Version
		{
			FileSeq = 0,
			VersionNumber = versionNumber,
			DtApplied = null,
			DtDownload = DateTime.Now,
			FileImage = null,
			ApplyUpdate = true
		};
		await _dbContext.Versions.AddAsync(version);
		await _dbContext.SaveChangesAsync();
	}
}