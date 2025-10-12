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
    public class PermissionsModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;
        
        public string DeleteError { get; set; }

        public PermissionsModel(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<VendorPermission> VendorPermissions { get; private set; }
        

        public async Task OnGetAsync(int? permissionId)
        {
            VendorPermissions = await _dbContext.VendorPermissions
                .Include(v => v.ApiUrls).ThenInclude(a => a.ApiUrl)
                .Include(v => v.AssociatedDefaultPermissions)
                .Include(v => v.AssociatedCustomers)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var vendorPermission = await _dbContext.VendorPermissions
                .Include(v => v.ApiUrls).ThenInclude(a => a.ApiUrl)
                .Include(v => v.AssociatedDefaultPermissions)
                .Include(v => v.AssociatedCustomers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (vendorPermission == null)
            {
                DeleteError = $"Cannot find vendor permission with ID {id}";
            }
            else
            {
                _dbContext.VendorPermissions.Remove(vendorPermission);
                await _dbContext.SaveChangesAsync();
            }

            VendorPermissions = await _dbContext.VendorPermissions
                .Include(v => v.ApiUrls).ThenInclude(a => a.ApiUrl)
                .Include(v => v.AssociatedDefaultPermissions)
                .Include(v => v.AssociatedCustomers)
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }

    }
}
