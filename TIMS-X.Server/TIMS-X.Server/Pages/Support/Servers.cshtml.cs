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
    public class ServersModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;
        
        public string DeleteError { get; set; }

        public ServersModel(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<TimsServer> Servers { get; private set; }

        public async Task OnGetAsync()
        {
            Servers = await _dbContext.TimsServers
                .Include(x => x.Customers)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var server = await _dbContext.TimsServers
                .Include(x => x.Customers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (server == null)
            {
                DeleteError = $"Cannot find server with ID {id}";
            }
            else if (server.CanDelete)
            {
                _dbContext.TimsServers.Remove(server);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                string reference = server.Customers.Count() == 1 ? "references" : "reference";
                DeleteError = $"Delete {server.Name} failed. First remove the {server.Customers.Count()} {reference}.";
            }

            Servers = await _dbContext.TimsServers
                .Include(x => x.Customers)
                .AsNoTracking()
                .ToListAsync();
            return Page();
        }

    }
}
