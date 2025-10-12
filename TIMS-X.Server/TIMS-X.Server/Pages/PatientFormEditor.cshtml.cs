using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
    public class PatientFormEditorModel : PageModel
    {
        private readonly PracticeQuery _practiceQuery;
        public PatientFormEditorModel(PracticeQuery practiceQuery)
        {
            _practiceQuery = practiceQuery;
        }

        [BindProperty]
        public string FormTypeLabel { get; set; }

        [BindProperty]
        public DigitalForm Form { get; set; }


        public async Task<IActionResult> OnGetSiteLogo()
        {
            var bytes = await _practiceQuery.GetFirstSiteLogoAsync();
            return File(bytes, "application/octet-stream", "1.jpg");
        }

        public async Task<IActionResult> OnGetAsync(int formType)
        {
            if(Enum.IsDefined(typeof(PatientFormTypeEnum), formType))
            {
                var formEnum = (PatientFormTypeEnum)formType;
                Form = await _practiceQuery.GetDigitalFormAsync(formEnum);

                if(Form == null)
                {
                    Form = new DigitalForm
                    {
                        FormType = formEnum
                    };
                }

                
                FormTypeLabel = EnumUtilities.GetDescriptionFromEnum(Form.FormType);
            }
            
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            await _practiceQuery.PutDigitalFormAsync(Form);
            return Page();
        }
    }
}
