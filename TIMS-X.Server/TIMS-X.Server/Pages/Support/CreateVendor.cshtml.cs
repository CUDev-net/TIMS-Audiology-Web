using System.Collections.Generic;
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
    public class CreateVendorModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;
        private readonly AppSettings _appSettings;
        public CreateVendorModel(TimsInternalDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _appSettings = configuration.Get<AppSettings>();
        }

        [BindProperty]
        public Vendor Vendor { get; set; }

        public IList<VendorPermission> VendorPermissions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Vendor = new Vendor();
            VendorPermissions = await _dbContext.VendorPermissions.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Vendor.DefaultPermissions =
                JsonConvert.DeserializeObject<HashSet<DefaultVendorPermission>>(Vendor.DefaultPermissionsJson);
            _dbContext.Vendors.Add(Vendor);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Support/Vendors");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var model = await _dbContext.Vendors.FindAsync(id);

            if (model != null)
            {
                _dbContext.Vendors.Remove(model);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
