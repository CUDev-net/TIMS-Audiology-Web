using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Models;

namespace TIMS_X.Server.Queries;

public class VendorQuery
{
	private readonly AppSettings _appSettings;
	private readonly TimsInternalDbContext _dbContext;

	public VendorQuery(TimsInternalDbContext dbContext, IConfiguration configuration)
	{
		_dbContext = dbContext;
		_appSettings = configuration.Get<AppSettings>();
	}

	public async Task<IEnumerable<Vendor>> GetVendorsAsync()
	{
		try
		{
			var vendors = await _dbContext.Vendors
				.AsNoTracking()
				.Include(x => x.CustomerPermissions)
				.ThenInclude(p => p.Customer)
				.Include(x => x.CustomerPermissions)
				.ThenInclude(p => p.Permission)
				.ThenInclude(p => p.ApiUrls)
				.ThenInclude(p => p.ApiUrl).OrderBy(o => o.ApiKey)
				.Where(x => !x.Inactive)
				.ToListAsync();
			return vendors;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}
}