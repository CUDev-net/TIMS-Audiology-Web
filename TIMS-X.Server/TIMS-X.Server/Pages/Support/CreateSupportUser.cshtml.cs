using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Models;

namespace TIMS_X.Server.Pages.Support
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Support)]
    public class CreateSupportUserModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;
        private readonly AppSettings _appSettings;
        public CreateSupportUserModel(TimsInternalDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _appSettings = configuration.Get<AppSettings>();
        }

        [BindProperty]
        public SupportUser SupportUser { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            SupportUser.Password = BCrypt.BCryptHelper.HashPassword(_appSettings.Keys.DefaultPassword,
                BCrypt.BCryptHelper.GenerateSalt());

            _dbContext.SupportUsers.Add(SupportUser);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Support/SupportUsers");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var supportUser = await _dbContext.SupportUsers.FindAsync(id);

            if (supportUser != null)
            {
                _dbContext.SupportUsers.Remove(supportUser);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
