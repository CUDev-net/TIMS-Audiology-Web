using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Services;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Hidden")]
    public class DocumentModel : PageModel
    {

        private readonly ImagingService _imagingService;

        public DocumentModel(ImagingService imagingService)
        {
            _imagingService = imagingService;
        }
        

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }

        [TempData]
        public Guid DocumentId { get; set; }
        [TempData]
        public string DocumentPassword { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id, string officeCode)
        {
            DocumentId = id;
            var doc = await _imagingService.GetImageAsync(id, false);

            if (doc == null)
            {
                ErrorMessage = "Error: The requested document does not exist.";
            }
            else if (!doc.WebAccess)
            {
                ErrorMessage = "Error: The requested document does not allow web access.";
            }
            else
            {
                if (doc.DtExpires.HasValue && doc.DtExpires.Value <= DateTime.Now)
                {
                    ErrorMessage = $"Error: Access to the requested document expired on {doc.DtExpires.Value:MM/dd/yyyy a\\t h:mm tt}.";
                }
                else if(string.IsNullOrWhiteSpace(doc.Password))
                {
                    doc = await _imagingService.GetImageAsync(id, true);
                    return new FileContentResult(doc.Image, "application/pdf");
                }
                else
                {
                    DocumentPassword = doc.Password;
                }
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("Password", "Password is required");
            }

            

            if (!BCrypt.BCryptHelper.CheckPassword(Password, DocumentPassword))
            {
                ModelState.AddModelError("Password", "Invalid password");
            }


            if (ModelState.IsValid)
            {
                var doc = await _imagingService.GetImageAsync(DocumentId, true);
                if (doc.Image == null || doc.Image.Length == 0)
                {
                    return new EmptyResult();
                }
                return new FileContentResult(doc.Image, "application/pdf");
            }

            TempData.Keep(nameof(DocumentId));
            TempData.Keep(nameof(DocumentPassword));
            return Page();
        }
    }
}