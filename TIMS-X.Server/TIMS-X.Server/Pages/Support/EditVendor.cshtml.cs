using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Server.Data;
using TIMS_X.Server.Models;
using Newtonsoft.Json;
using TIMS_X.Core.Utils;

namespace TIMS_X.Server.Pages.Support
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Support)]
    public class EditVendorModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;

        public EditVendorModel(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public Vendor Vendor { get; set; }
        public List<VendorPermission> VendorPermissions { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            VendorPermissions = await _dbContext.VendorPermissions.ToListAsync();
            Vendor = await _dbContext.Vendors
                .Include(x => x.DefaultPermissions).ThenInclude(p => p.Permission)
                .Include(x => x.CustomerPermissions).ThenInclude(p => p.Customer)
                .Include(x => x.CustomerPermissions).ThenInclude(p => p.Permission)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (Vendor == null)
            {
                return RedirectToPage("/Support/Vendors");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                VendorPermissions = await _dbContext.VendorPermissions.ToListAsync();
                return Page();
            }
            var dbVendor = await _dbContext.Vendors
                .Include(x => x.DefaultPermissions).ThenInclude(p => p.Permission)
                .Include(x => x.CustomerPermissions).ThenInclude(p => p.Customer)
                .Include(x => x.CustomerPermissions).ThenInclude(p => p.Permission)
                .FirstOrDefaultAsync(x => x.Id == Vendor.Id);

            if (dbVendor == null)
            {
                throw new Exception($"Vendor {Vendor.Id} not found!");
            }

            dbVendor.Name = Vendor.Name;
            dbVendor.ApiKey = Vendor.ApiKey;
            dbVendor.Inactive = Vendor.Inactive;
            dbVendor.DefaultPermissions = JsonConvert.DeserializeObject<HashSet<DefaultVendorPermission>>(Vendor.DefaultPermissionsJson);

            _dbContext.Attach(dbVendor).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception($"Vendor {Vendor.Id} not found!");
            }

            return RedirectToPage("/Support/Vendors");
        }

        
    }
}