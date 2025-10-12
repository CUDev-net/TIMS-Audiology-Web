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
    public class CreateCustomerModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;

        public CreateCustomerModel(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public Customer Customer { get; set; }

        public List<TimsServer> Servers { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Servers = await _dbContext.TimsServers.AsNoTracking()
                .Where(x => !x.Inactive)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Servers = await _dbContext.TimsServers.AsNoTracking()
                    .Where(x => !x.Inactive)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
                return Page();
            }
            Customer.DateUpdated = DateTime.Now;
            // grab user id from cookie and set updated user with it
            var userId = User.Claims.First(x => x.Type == StringConstants.User);
            Customer.UpdatedBy = int.Parse(userId.Value);
            Customer.SqlUser = "";
            Customer.SqlPassword = "";
            // Save changes
            _dbContext.Customers.Add(Customer);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Support/Customers");
        }
    }
}
