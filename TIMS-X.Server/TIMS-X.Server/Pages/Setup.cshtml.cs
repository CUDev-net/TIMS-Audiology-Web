using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TIMS_X.Core.Models;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
    public class SetupModel : PageModel
    {

        private readonly CustomerQuery _customerQuery;
        private readonly UserQuery _userQuery;

        public SetupModel(CustomerQuery customerQuery, UserQuery userQuery)
        {
            _customerQuery = customerQuery;
            _userQuery = userQuery;
        }

        [BindProperty]
        public ConnectionDetails ConnectionDetails { get; set; }

        [BindProperty]
        public bool CanUserModifySqlCredentials { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == StringConstants.User).Select(x => x.Value).First());
            CanUserModifySqlCredentials = await _userQuery.DoesUserHaveSettingAsync(userId, Core.Enums.SettingEnum.CanModifySqlCredentials);

            
            var officeCode = HttpContext.User.Claims.Where(x => x.Type == StringConstants.OfficeCode).Select(x => x.Value).First();
            var connectionInfo = await _customerQuery.GetConnectionInfoAsync(officeCode);
            if(connectionInfo != null)
            {
                ConnectionDetails = new ConnectionDetails
                {
                    Server = connectionInfo.Server,
                    Database = connectionInfo.Database,
                    User = connectionInfo.User,
                    Password = connectionInfo.Password
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string saveSqlUser)
        {
            if(!string.IsNullOrEmpty(saveSqlUser))
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var connInfo = new ConnectionInfo
                {
                    Database = ConnectionDetails.Database,
                    Server = ConnectionDetails.Server,
                    User = ConnectionDetails.User,
                    Password = ConnectionDetails.Password
                };
                var officeCode = HttpContext.User.Claims.Where(x => x.Type == StringConstants.OfficeCode).Select(x => x.Value).First();
                await _customerQuery.UpdateConnectionInfoAsync(officeCode, connInfo);

            }
            
            return RedirectToPage("Setup");
        }
    }
}