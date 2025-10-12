using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TIMS_X.Server.Pages
{
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        

        public int ErrorStatusCode { get; set; }
        public string ErrorMessage { get; set; } = "An error occurred while processing your request.";

        public void OnGet(int code)
        {
            
            ErrorStatusCode = code;

            if(code == 404)
            {
                ErrorMessage = "The requested resource cannot be found.";
            }
            else if(code == 403)
            {
                ErrorMessage = "Access Denied";
            }

        }
    }
}
