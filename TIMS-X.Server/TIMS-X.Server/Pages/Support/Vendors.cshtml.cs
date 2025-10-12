using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Data;
using TIMS_X.Server.Models;

namespace TIMS_X.Server.Pages.Support
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Support)]
    public class VendorsModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;
        
        public string DeleteError { get; set; }

        public VendorsModel(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<Vendor> Vendors { get; private set; }

        public async Task OnGetAsync(int? vendorId)
        {
            
            Vendors = await _dbContext.Vendors
                .Include(x => x.DefaultPermissions)
                .Include(x => x.CustomerPermissions)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var vendor = await _dbContext.Vendors
                .Include(x => x.DefaultPermissions)
                .Include(x => x.CustomerPermissions)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (vendor == null)
            {
                DeleteError = $"Cannot find server with ID {id}";
            }
            else
            {
                _dbContext.Vendors.Remove(vendor);
                await _dbContext.SaveChangesAsync();
            }


            Vendors = await _dbContext.Vendors
                .Include(x => x.DefaultPermissions)
                .Include(x => x.CustomerPermissions)
                .AsNoTracking()
                .ToListAsync();
            return Page();
        }

    }
}
