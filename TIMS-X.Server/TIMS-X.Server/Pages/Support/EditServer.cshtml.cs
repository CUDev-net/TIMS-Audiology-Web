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
    public class EditServerModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;

        public EditServerModel(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public TimsServer Server { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Server = await _dbContext.TimsServers
                .Include(x => x.Customers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (Server == null)
            {
                return RedirectToPage("/Support/Servers");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            _dbContext.Attach(Server).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception($"Server {Server.Id} not found!");
            }

            return RedirectToPage("/Support/Servers");
        }

        
    }
}