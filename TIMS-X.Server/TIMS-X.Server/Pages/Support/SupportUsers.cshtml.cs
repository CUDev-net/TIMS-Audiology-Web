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
    public class SupportUsersModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;
        public string ErrorMessage { get; set; }

        public SupportUsersModel(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<SupportUser> SupportUsers { get; private set; }

        public async Task OnGetAsync()
        {
            SupportUsers = await _dbContext.SupportUsers
                .Include(x => x.CustomersUpdated)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await _dbContext.SupportUsers.FindAsync(id);
            if (user != null)
            {
                // check for existing references in the Customer table
                if (_dbContext.Customers.Any(x => x.UpdatedBy == user.Id))
                {
                    ErrorMessage = $"Delete {user.Name} failed. First remove customer references.";
                }
                else
                {
                    _dbContext.SupportUsers.Remove(user);
                    await _dbContext.SaveChangesAsync();
                }
            }

            SupportUsers = await _dbContext.SupportUsers
                .Include(x => x.CustomersUpdated)
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }



    }
}
