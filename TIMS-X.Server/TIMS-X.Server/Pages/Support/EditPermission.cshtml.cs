using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Data;
using TIMS_X.Server.Models;

namespace TIMS_X.Server.Pages.Support
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Support)]
    public class EditPermissionModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;

        public EditPermissionModel(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public VendorPermission VendorPermission { get; set; }

        public List<ApiUrl> ApiUrls { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ApiUrls = await _dbContext.ApiUrls.OrderBy(o => o.Url).ToListAsync();

            VendorPermission = await _dbContext.VendorPermissions
                .Include(p => p.ApiUrls).ThenInclude(u => u.ApiUrl)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (VendorPermission == null)
            {
                return RedirectToPage("/Support/Permissions");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ApiUrls = await _dbContext.ApiUrls.OrderBy(o => o.Url).ToListAsync();
                return Page();
            }

            var dbPermission = await _dbContext.VendorPermissions
                .Include(p => p.ApiUrls).ThenInclude(u => u.ApiUrl)
                .FirstOrDefaultAsync(x => x.Id == VendorPermission.Id);
            
            dbPermission.Name = VendorPermission.Name;
            dbPermission.Description = VendorPermission.Description;
            dbPermission.Inactive = VendorPermission.Inactive;
            dbPermission.ApiUrls = JsonConvert.DeserializeObject<HashSet<VendorPermissionApiUrl>>(VendorPermission.UrlsJson);
            
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception($"Permission {VendorPermission.Id} not found!");
            }

            return RedirectToPage("/Support/Permissions");
        }

        
    }
}