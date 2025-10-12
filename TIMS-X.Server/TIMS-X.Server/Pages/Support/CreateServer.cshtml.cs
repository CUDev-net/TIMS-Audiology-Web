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
    public class CreateServerModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;
        private readonly AppSettings _appSettings;
        public CreateServerModel(TimsInternalDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _appSettings = configuration.Get<AppSettings>();
        }

        [BindProperty]
        public TimsServer Server { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            _dbContext.TimsServers.Add(Server);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Support/Servers");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var server = await _dbContext.TimsServers.FindAsync(id);

            if (server != null)
            {
                _dbContext.TimsServers.Remove(server);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
