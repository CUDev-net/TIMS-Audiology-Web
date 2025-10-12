using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Models;

namespace TIMS_X.Server.Pages.Support
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Support)]
    public class CreatePermissionModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;
        private readonly AppSettings _appSettings;
        public CreatePermissionModel(TimsInternalDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _appSettings = configuration.Get<AppSettings>();
        }

        [BindProperty]
        public VendorPermission VendorPermission { get; set; }

        public IList<ApiUrl> ApiUrls { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ApiUrls = await _dbContext.ApiUrls.Where(x => !x.Inactive).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ApiUrls = await _dbContext.ApiUrls.Where(x => !x.Inactive).ToListAsync();
                return Page();
            }
            // must deserialize api urls
            VendorPermission.ApiUrls = JsonConvert.DeserializeObject<HashSet<VendorPermissionApiUrl>>(VendorPermission.UrlsJson);
            _dbContext.VendorPermissions.Add(VendorPermission);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Support/Permissions");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var permission = await _dbContext.VendorPermissions.FindAsync(id);

            if (permission != null)
            {
                _dbContext.VendorPermissions.Remove(permission);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
