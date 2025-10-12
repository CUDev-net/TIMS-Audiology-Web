using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Data;

namespace TIMS_X.Server.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class IndexModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;

        public IndexModel(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public int ActiveCustomers { get; set; }
        public int ActiveUsers { get; set; }
        
        public async Task OnGetAsync()
        {
            ActiveCustomers = await _dbContext.Customers.CountAsync(x => !x.Inactive);
            ActiveUsers = await _dbContext.SupportUsers.CountAsync(x => !x.Inactive);

            if (User.HasClaim(ClaimTypes.Role, StringConstants.Customer))
                Response.Redirect("/scheduler");
        }
    }
}