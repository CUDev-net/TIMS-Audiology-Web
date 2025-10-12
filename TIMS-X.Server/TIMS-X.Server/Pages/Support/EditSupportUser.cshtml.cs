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
    public class EditSupportUserModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;

        public EditSupportUserModel(TimsInternalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public SupportUser SupportUser { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            SupportUser = await _dbContext.SupportUsers.FindAsync(id);

            if (SupportUser == null)
            {
                return RedirectToPage("/Support/SupportUsers");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            _dbContext.Attach(SupportUser).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception($"User {SupportUser.Id} not found!");
            }

            return RedirectToPage("/Support/SupportUsers");
        }
    }
}