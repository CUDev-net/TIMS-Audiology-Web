using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core.Models;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Extensions;
using TIMS_X.Server.Models;

namespace TIMS_X.Server.Queries
{
    public class VendorQuery
    {
        private readonly TimsInternalDbContext _dbContext;
        public VendorQuery(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Vendor>> GetVendorsAsync()
        {
            try
            {
                var vendors = await _dbContext.Vendors
                    .Include(x => x.CustomerPermissions)
                        .ThenInclude(p => p.Customer)
                    .Include(x => x.CustomerPermissions)
                        .ThenInclude(p => p.Permission)
                            .ThenInclude(p => p.ApiUrls)
                                .ThenInclude(p => p.ApiUrl).OrderBy( o => o.ApiKey)
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

        public async Task<Vendor> GetVendorAsyncQuery(string apikey)
        {
	        var vendor = await _dbContext.Vendors
			        .Include(x => x.CustomerPermissions)
			        .ThenInclude(p => p.Customer)
			        .Include(x => x.CustomerPermissions)
			        .ThenInclude(p => p.Permission)
			        .ThenInclude(p => p.ApiUrls).ThenInclude(p => p.ApiUrl)
			        .Where( v => v.ApiKey == apikey)
			        .FirstOrDefaultAsync(x => !x.Inactive);
		        return vendor;
        }

		public async Task<Vendor> GetVendorFromApiKeyAsync(string vendorApiKey)
        {
            var vendor = await _dbContext.Vendors.FirstOrDefaultAsync(v => v.ApiKey == vendorApiKey);
            return vendor;
        }
        
    }
}
