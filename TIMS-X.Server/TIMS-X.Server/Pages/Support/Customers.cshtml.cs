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
    public class CustomersModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;

        public string ServerName { get; set; }

        public CustomersModel(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<Customer> Customers { get; private set; }

        public async Task OnGetAsync(int? serverId)
        {
            if (serverId.HasValue)
            {
                Customers = await _dbContext.Customers
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.Server)
                    .AsNoTracking()
                    .Where(x => x.ServerId == serverId)
                    .OrderBy(x => x.Name)
                    .ToListAsync();
                if (Customers.Any())
                {
                    ServerName = Customers.First().Server.Name;
                }
            }
            else
            {
                Customers = await _dbContext.Customers
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.Server)
                    .AsNoTracking()
                    .OrderBy(x => x.Name)
                    .ToListAsync();
            }
            
        }
        


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer != null)
            {
                _dbContext.Customers.Remove(customer);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToPage("/Support/Customers");
        }
    }
}
