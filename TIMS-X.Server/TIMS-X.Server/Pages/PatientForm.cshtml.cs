using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.DAL;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Hubs;
using TIMS_X.Server.Middleware;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Pages
{
    [AllowAnonymous]
    public class PatientFormModel : PageModel
    {
        private readonly CustomerQuery _customerQuery;
        private readonly IHubContext<SmsHub> _hub;
        private PracticeQuery _practiceQuery;
        private readonly ContextHelper _contextHelper;

        public PatientFormModel(CustomerQuery customerQuery, IHubContext<SmsHub> hub, ContextHelper contextHelper)
        {
            _customerQuery = customerQuery;
            _hub = hub;
            _contextHelper = contextHelper;
        }
        
        public string FormTypeDesc { get; set; }
        public async Task<IActionResult> OnGetAsync(string p)
        {
            if(string.IsNullOrEmpty(p))
                return new BadRequestResult();

            Url = p;

            try
            {
                var formLink = await _customerQuery.GetFormLinkAsync(p);
                if (formLink == null)
                {
                    return RedirectToPage("/FormSubmitted");
                }
                await _LoadFormAsync(formLink);
            }
            catch (Exception)
            {
                return new BadRequestResult();
            }
            
            return Page();
        }


        // Read-only objects
        public DigitalForm Form { get; set; }

        [BindProperty]
        public Site Site { get; set; }

        public Patient Patient { get; set; }

        public List<Area> StateList { get; set; }
        public List<MaritalStatus> MaritalStatuses { get; set; }
        public List<EmplStatus> EmploymentStatuses { get; set; }
        public List<Sex> Sexes { get; set; }
        public List<Language> Languages { get; set; }
        public List<Race> Races { get; set; }
        public List<Ethnicity> Ethnicities { get; set; }


        // Writeable objects or things we need to preserve for the post handler
        [BindProperty]
        public int RequestingUserId { get; set; }

        [BindProperty]
        public string OfficeCode { get; set; }

        [BindProperty]
        public PatientFormTypeEnum FormType { get; set; }

        [BindProperty]
#pragma warning disable CS0108, CS0114
        public string Url { get; set; }
#pragma warning restore CS0108, CS0114


        [BindProperty]
        public DigitalPatientIntakeData IntakeData { get; set; }



        private async Task _LoadFormAsync(FormLink formLink)
        {
            FormType = formLink.FormType;
            FormTypeDesc = EnumUtilities.GetDescriptionFromEnum(formLink.FormType);
            RequestingUserId = formLink.UserId;
            OfficeCode = formLink.Customer.OfficeCode;
            // Connect to customer database to grab digital form
            var connInfo = await _customerQuery.GetConnectionInfoAsync(OfficeCode);
            if(connInfo == null)
            {
                throw new Exception("Invalid data.");
            }
            var practiceDbContextOptions = new DbContextOptionsBuilder<PracticeDbContext>();
            ConnectionStringBuilder.SetConnectionString(practiceDbContextOptions, connInfo.Server, connInfo.Database, connInfo.User, connInfo.Password);
            var practiceDbContext = new PracticeDbContext(practiceDbContextOptions.Options);
            _practiceQuery = new PracticeQuery(practiceDbContext, _contextHelper);
            Form = await _practiceQuery.GetDigitalFormAsync(formLink.FormType);

            if(Form == null)
            {
                throw new Exception("No digital form found for form type " + FormType);
            }

            var patientDbContextOptions = new DbContextOptionsBuilder<PatientDbContext>();
            ConnectionStringBuilder.SetConnectionString(patientDbContextOptions, connInfo.Server, connInfo.Database, connInfo.User, connInfo.Password);
            var patientDbContext = new PatientDbContext(patientDbContextOptions.Options);
            var patientQuery = new PatientQuery(patientDbContext);

            Patient = await patientQuery.GetFullPatientAsync(formLink.PatientId);
            
            if(Patient == null)
            {
                throw new Exception("Invalid patient id.");
            }

            if (formLink.FormType == PatientFormTypeEnum.Intake)
            {
                IntakeData = new DigitalPatientIntakeData { PatientId = formLink.PatientId };
            }

            
            
            if(Patient.SiteId.HasValue && Patient.SiteId.Value > 0)
            {
                Site = await _practiceQuery.GetSiteAsync(Patient.SiteId.Value);
            } else
            {
                Site = await _practiceQuery.GetFirstSiteAsync();
            }

            if(Site == null)
            {
                throw new Exception("No active site.");
            }

            // Always use practice name instead of site name
            Site.Name = await _practiceQuery.GetValueAsync(p => p.Name);

            // Apply rule "Use Site Address For Reports". Use practice address/phone if set to false.
            var useSiteAddress = await _practiceQuery.GetValueAsync(p => p.UseSiteAddressForReports);
            if(!useSiteAddress)
            {
                (Site.Address1, Site.Address2, Site.City, Site.State, Site.Zip, Site.Phone) =
                    await _practiceQuery.GetValueAsync(p => new Tuple<string, string, string, string, string, string>(
                        p.BillingAddress1, p.BillingAddress2, p.BillingCity, p.BillingState, p.BillingZipCode, p.BillingPhoneNumber));
            }

            StateList = await _practiceQuery.GetStateListAsync();
            MaritalStatuses = await _practiceQuery.GetMaritalStatusesAsync();
            EmploymentStatuses = await _practiceQuery.GetEmploymentStatusesAsync();
            try
            {
                Sexes = await _practiceQuery.GetSexesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

            var qbLocale = await _practiceQuery.GetValueAsync(practice => practice.QbLocale);

            Languages = Language.LoadAll(qbLocale);
            Races = Race.LoadAll();
            Ethnicities = Ethnicity.LoadAll(qbLocale);

            PrimaryInsurance = await patientQuery.GetPatientInsuranceAsync(Patient.Id, PayerLevel.Primary);
            SecondaryInsurance = await patientQuery.GetPatientInsuranceAsync(Patient.Id, PayerLevel.Secondary);


        }

        public PatientInsurance PrimaryInsurance { get; set; }
        public PatientInsurance SecondaryInsurance { get; set; }

        [BindProperty]
        public IFormFile FormScreenshot { get; set; }


        private async Task _ProcessFormScreenshotAsync()
        {
            byte[] formScreenshot = null;
            if (FormScreenshot != null && FormScreenshot.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await FormScreenshot.CopyToAsync(stream);
                    formScreenshot = stream.ToArray();
                }
            }
            IntakeData.FormScreenshot = formScreenshot;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _ProcessFormScreenshotAsync();

            if (await SaveFormDataAsync())
            {
                return RedirectToPage("/FormSubmitted");
            }
            else
            {

            }

            return Page();
        }

        public async Task<IActionResult> OnGetSiteLogo(string officeCode, int patientId)
        {
            byte[] bytes = null;

            var connInfo = await _customerQuery.GetConnectionInfoAsync(officeCode);
            if (connInfo == null)
            {
                throw new Exception("Invalid data.");
            }
            var practiceDbContextOptions = new DbContextOptionsBuilder<PracticeDbContext>();
            ConnectionStringBuilder.SetConnectionString(practiceDbContextOptions, connInfo.Server, connInfo.Database, connInfo.User, connInfo.Password);
            var practiceDbContext = new PracticeDbContext(practiceDbContextOptions.Options);
            _practiceQuery = new PracticeQuery(practiceDbContext, _contextHelper);


            var patientDbContextOptions = new DbContextOptionsBuilder<PatientDbContext>();
            ConnectionStringBuilder.SetConnectionString(patientDbContextOptions, connInfo.Server, connInfo.Database, connInfo.User, connInfo.Password);
            var patientDbContext = new PatientDbContext(patientDbContextOptions.Options);
            var patientQuery = new PatientQuery(patientDbContext);

            var patient = await patientQuery.GetFullPatientAsync(patientId);
            
            if (patient != null)
            {
                if (patient.SiteId.HasValue && patient.SiteId.Value > 0)
                {
                    bytes = await _practiceQuery.GetSiteLogoAsync(patient.SiteId.Value);
                }
                else
                {
                    bytes = await _practiceQuery.GetFirstSiteLogoAsync();
                }
            }

            return File(bytes, "application/octet-stream", "1.jpg");

        }

        public async Task<bool> SaveFormDataAsync()
        {
            if (FormType == PatientFormTypeEnum.Intake )
            {
                // get db conn info
                var connInfo = await _customerQuery.GetConnectionInfoAsync(OfficeCode);


                var userId = $"{OfficeCode}-{RequestingUserId}".ToLower();
                if (SmsHub.IsUserConnected(userId))
                {
                    await _hub.Clients.User(userId).SendAsync("ReceiveIntakeSheet", IntakeData);
                }
                else
                {
                    var alert = new Alert
                    {
                        CreatedUserId = 0,
                        CreatedDate = DateTime.Now,
                        DueDate = DateTime.Now,
                        Name = EnumUtilities.GetDescriptionFromEnum(AlertTypeEnum.IntakeSheetReceived),
                        Description = JsonConvert.SerializeObject(IntakeData),
                        AlertUserId = RequestingUserId,
                        AlertObjectId = IntakeData.PatientId,
                        AlertType = AlertTypeEnum.IntakeSheetReceived,
                    };
                    
                    try
                    {
                        var practiceDbContextOptions = new DbContextOptionsBuilder<PracticeDbContext>();
                        ConnectionStringBuilder.SetConnectionString(practiceDbContextOptions, connInfo.Server, connInfo.Database, connInfo.User, connInfo.Password);
                        var practiceDbContext = new PracticeDbContext(practiceDbContextOptions.Options);
                        _practiceQuery = new PracticeQuery(practiceDbContext, _contextHelper);
                        await _practiceQuery.PutAlertAsync(alert);

                        var patientDbContextOptions = new DbContextOptionsBuilder<PatientDbContext>();
                        ConnectionStringBuilder.SetConnectionString(patientDbContextOptions, connInfo.Server, connInfo.Database, connInfo.User, connInfo.Password);
                        var patientDbContext = new PatientDbContext(patientDbContextOptions.Options);
                        var patientQuery = new PatientQuery(patientDbContext);
                        var patient = await patientQuery.GetFullPatientAsync(IntakeData.PatientId);
                        patient.HasIntakeData = true;
                        await patientQuery.PutPatientAsync(patient);
                        

                    } catch(Exception)
                    {
                        return false;
                    }
                    
                }

                await _customerQuery.DeleteFormLinkAsync(Url);


            }
            
            

            


            return true;
        }
        
    }
}
