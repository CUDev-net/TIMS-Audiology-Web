using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Data;
using TIMS_X.Server.Models;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Pages.Account
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ChangePasswordModel : PageModel
    {
        #region Constructors

        public ChangePasswordModel(TimsInternalDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _appSettings = configuration.Get<AppSettings>();
        }

        #endregion Constructors

        #region ChangePasswordModel Members

        [BindProperty]
        public SupportUser SupportUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get the user ID from the cookie
            var userId = User.Claims.Where(x => x.Type == StringConstants.User).Select(x => int.Parse(x.Value)).FirstOrDefault();
            if (userId == 0)
            {
                await HttpContext.SignOutAsync();
                return RedirectToPage("/Index");
            }

            // Query the user from the database
            SupportUser = await _dbContext.SupportUsers.FindAsync(userId);

            if (SupportUser == null)
            {
                return RedirectToPage("/SupportUsers");
            }

            // No need to sling the password around
            SupportUser.Password = null;

            return Page();
        }

        /// <summary>
        /// User is submitting password change
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Custom validation - don't allow user to change password to the default password
            if (SupportUser.Password == _appSettings.Keys.DefaultPassword)
            {
                ModelState.AddModelError("SupportUser.Password", "Default password forbidden");
                return Page();
            }

            // encrypt it
            var encryptedPassword = BCrypt.BCryptHelper.HashPassword(SupportUser.Password,
                BCrypt.BCryptHelper.GenerateSalt());

            // store it
            SupportUser.Password = encryptedPassword;
            _dbContext.Attach(SupportUser).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception($"Support User {SupportUser.Id} not found!");
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, SupportUser.Name),
                new Claim(StringConstants.User, SupportUser.Id.ToString()),
                new Claim(StringConstants.OfficeCode, StringConstants.Support),
                new Claim(ClaimTypes.Role, StringConstants.Support)
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // issue new cookie that doesn't force password change
            // to do this we call SignOutAsync() followed by SignInAsync()
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            // Let the user know it worked
            return RedirectToPage("/Account/PasswordChanged");
        }

        #endregion ChangePasswordModel Members

        #region Fields

        private readonly AppSettings _appSettings;
        private readonly TimsInternalDbContext _dbContext;

        #endregion Fields

    }
}