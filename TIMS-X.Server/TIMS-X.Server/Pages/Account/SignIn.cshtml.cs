using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BCrypt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Models;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Pages.Account;

public class SignInModel : PageModel
{
    private readonly AppSettings _appSettings;
    private readonly TimsInternalDbContext _dbContext;
    private readonly PracticeQuery _practiceQuery;
    private readonly UserService _userService;

    public SignInModel(TimsInternalDbContext dbContext, IConfiguration configuration, UserService userService,
        PracticeQuery practiceQuery)
    {
        _dbContext = dbContext;
        _appSettings = configuration.Get<AppSettings>();
        _userService = userService;
        _practiceQuery = practiceQuery;
    }

    [TempData] public string ErrorMessage { get; set; }

    public string Header { get; set; }

    [BindProperty(SupportsGet = true)] public InputModel Input { get; set; }

    public bool IsSupportLogin => string.Equals(Input.OfficeCode, StringConstants.Support,
        StringComparison.CurrentCultureIgnoreCase);

    public SignInUser SelectedUser { get; set; }

    public List<SignInUser> Users { get; set; }

    public bool IsOfficeCodeValid { get; set; }

    private async Task _ConfigurePage()
    {
        if (string.IsNullOrEmpty(Input.OfficeCode))
        {
            Header = "Enter Office Code to Continue";
            Users = new List<SignInUser>();
        }
        else
        {
            if (IsSupportLogin)
            {
                IsOfficeCodeValid = true;
                Header = "Sign In as TIMS Support";
                Users = await _dbContext.SupportUsers
                    .AsNoTracking()
                    .Where(x => !x.Inactive)
                    .Select(x => new SignInUser { Id = x.Id, Name = x.Name })
                    .ToListAsync();
            }
            else
            {
                Practice practice;
                try
                {
                    practice = await _practiceQuery.GetPracticeAsync();
                }
                catch (InvalidOperationException)
                {
                    ModelState.AddModelError("Input.OfficeCode", "Invalid Office Code");
                    return;
                }

                IsOfficeCodeValid = true;

                if (practice != null) Header = $"Sign In to {practice.Name}";

                var users = await _userService.GetLoginUsersAsync(false);
                Users = users.Where(x => x.IsWebUser)
                    .Select(x => new SignInUser { Id = x.Id, Name = x.Name })
                    .ToList();
            }
        }
    }

    private static DateTimeOffset _GetExpirationDate()
    {
        var expirationDate = DateTime.UtcNow.AddHours(1);
#if DEBUG
        expirationDate = DateTime.UtcNow.AddDays(30);
#endif
        return expirationDate;
    }

    private async Task<IActionResult> _SignInCustomer()
    {
        //var ad = await _practiceQuery.GetValueAsync(x => x.UsesAdAuthentication);
        var userId = _userService.GetUserId(false, Input.Username);
        if (userId == -1)
        {
            ModelState.AddModelError("Input", "Invalid username/password.");
            await _ConfigurePage();
            return Page();
        }

        var user = await _userService.AuthenticateWebUserAsync(new AuthenticationForm
            { UserId = userId, Password = Input.Password });
        if (user == null)
        {
            ModelState.AddModelError("Input", "Invalid username/password.");
            await _ConfigurePage();
            return Page();
        }

        // todo: check if password needs to change
        var requirePasswordChange = false;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(StringConstants.User, user.Id.ToString()),
            new(StringConstants.OfficeCode, Input.OfficeCode),
            new(ClaimTypes.Role, StringConstants.Customer),
            new(StringConstants.ChangePassword, requirePasswordChange ? "1" : "0")
        };

        // Issue the new cookie
        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                ExpiresUtc = _GetExpirationDate(),
                IsPersistent = true
            });

        return LocalRedirect(string.IsNullOrEmpty(Input.ReturnUrl) ? "/Index" : Input.ReturnUrl);
    }

    private async Task<IActionResult> _SignInTimsSupport()
    {
        var supportUser = await _dbContext.SupportUsers
            .AsNoTracking()
            .Where(x => !x.Inactive && x.Name == Input.Username)
            .FirstOrDefaultAsync();
        if (supportUser == null)
        {
            ModelState.AddModelError("Input", "Invalid username/password.");
            await _ConfigurePage();
            return Page();
        }

        if (!BCryptHelper.CheckPassword(Input.Password, supportUser.Password))
        {
            ModelState.AddModelError("Input", "Invalid username/password.");
            await _ConfigurePage();
            return Page();
        }

        // require user to change password if their password is still the default
        var requirePasswordChange = Input.Password == _appSettings.Keys.DefaultPassword;
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, supportUser.Name),
            new(StringConstants.User, supportUser.Id.ToString()),
            new(StringConstants.OfficeCode, StringConstants.Support),
            new(ClaimTypes.Role, StringConstants.Support),
            new(StringConstants.ChangePassword, requirePasswordChange ? "1" : "0")
        };

        // Issue the new cookie
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                ExpiresUtc = _GetExpirationDate(),
                IsPersistent = true
            });

        return LocalRedirect(string.IsNullOrEmpty(Input.ReturnUrl) ? "/Index" : Input.ReturnUrl);
    }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
            ModelState.AddModelError(string.Empty, ErrorMessage);
        else
            ModelState.Clear();


        if (string.IsNullOrEmpty(Input.ReturnUrl)) Input.ReturnUrl = "/Index";

        // Clear the existing external cookie
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await _ConfigurePage();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Input.OfficeCode))
            ModelState.AddModelError("Input.OfficeCode", "Office Code is required");
        if (string.IsNullOrWhiteSpace(Input.Username)) ModelState.AddModelError("Input.Username", "User is required");
        if (string.IsNullOrEmpty(Input.Password)) ModelState.AddModelError("Input.Password", "Password is required");

        if (ModelState.IsValid)
        {
            if (string.Equals(Input.OfficeCode, StringConstants.Support, StringComparison.CurrentCultureIgnoreCase))
                return await _SignInTimsSupport();
            return await _SignInCustomer();
        }

        // Something failed. Redisplay the form.
        return Page();
    }

    public class InputModel
    {
        [Required] public string OfficeCode { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required] public string Username { get; set; }

        [Required] public string ReturnUrl { get; set; }
    }
}