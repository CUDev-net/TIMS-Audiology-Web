using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Remotion.Linq.Clauses;
using Serilog;
using TIMS_X.Core;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.DAL;
using TIMS_X.Server.Data;
using TIMS_X.Server.Integrations;
using TIMS_X.Server.Middleware;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Pages.Support
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Support)]
    public class EditCustomerModel : PageModel
    {
        private readonly TimsInternalDbContext _dbContext;
        private Customer _customer;
        private readonly TimsUpdateService _updateService;
        private readonly CustomerQuery _customerQuery;
        private readonly ContextHelper _contextHelper;
        public EditCustomerModel(TimsInternalDbContext dbContext, TimsUpdateService updateService, CustomerQuery customerQuery, ContextHelper contextHelper)
        {
            _dbContext = dbContext;
            _updateService = updateService;
            _customerQuery = customerQuery;
            _contextHelper = contextHelper;
        }

        [BindProperty]
        public Customer Customer
        {
            get => _customer;
            set { _customer = value; }
        }

        public List<VendorPermission> VendorPermissions { get; set; }

        public List<TimsServer> Servers { get; set; }
        public Dictionary<string, List<TimsTimeZoneModel>> TimeZones { get; set; }
        public List<Vendor> Vendors { get; set; }
        public List<TimsUpdate> Updates { get; set; }
        public List<Core.Domain.Version> VersionHistory { get; set; }
        public bool HasUpdates => Updates != null && Updates.Any();
        public string CurrentVersion { get; set; }
        
        [BindProperty]
        public string SelectedUpdate { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            await _LoadCoreDataAsync(id, true);

            if (Customer == null)
            {
                return RedirectToPage("/Support/Customers");
            }
            if(!string.IsNullOrWhiteSpace(Customer.OfficeCode))
            {
                VersionHistory = await _updateService.GetVersionHistoryAsync(Customer.OfficeCode);
                CurrentVersion = await _updateService.GetCurrentVersionAsync(Customer.OfficeCode);
                if(!string.IsNullOrWhiteSpace(CurrentVersion))
                {
                    try
                    {
                        Updates = _updateService.GetAvailableUpdates(CurrentVersion);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(Serilog.Events.LogEventLevel.Error, $"Error connecting to blob service: {ex.Message}");
                    }
                    
                }
            }

            return Page();
        }



        public List<string> TwilioRecords { get; set; }

        private async Task _LoadTwilioUsageAsync()
        {
            var connInfo = await _customerQuery.GetConnectionInfoAsync(Customer.OfficeCode);
            if (connInfo == null)
            {
                throw new Exception("Invalid data.");
            }
            var practiceDbContextOptions = new DbContextOptionsBuilder<PracticeDbContext>();
            ConnectionStringBuilder.SetConnectionString(practiceDbContextOptions, connInfo.Server, connInfo.Database, connInfo.User, connInfo.Password);
            var practiceDbContext = new PracticeDbContext(practiceDbContextOptions.Options);
            var practiceQuery = new PracticeQuery(practiceDbContext, _contextHelper);
            var twilioMessenger = new TwilioMessenger(practiceQuery);

            TwilioRecords = twilioMessenger.GetMonthlyUsage();


        }

        private async Task _LoadCoreDataAsync(int id, bool reloadCustomer)
        {
            Servers = await _dbContext.TimsServers.AsNoTracking().ToListAsync();
            Vendors = await _dbContext.Vendors.AsNoTracking().ToListAsync();
            VendorPermissions = await _dbContext.VendorPermissions.AsNoTracking().ToListAsync();
            if(reloadCustomer)
            {
                Customer = await _dbContext.Customers
                                        .Include(x => x.UpdatedByUser)
                                        .Include(x => x.VendorPermissions).ThenInclude(p => p.Permission)
                                        .Include(x => x.VendorPermissions).ThenInclude(p => p.Vendor)
                                        .Include(x => x.Server)
                                        .FirstOrDefaultAsync(x => x.Id == id);
            }

            if (!string.IsNullOrWhiteSpace(Customer.OfficeCode))
            {
                VersionHistory = await _updateService.GetVersionHistoryAsync(Customer.OfficeCode);
                CurrentVersion = await _updateService.GetCurrentVersionAsync(Customer.OfficeCode);
                if (!string.IsNullOrWhiteSpace(CurrentVersion))
                {
                    Updates = _updateService.GetAvailableUpdates(CurrentVersion);
                }
            }

            TimeZones = TimsTimeZoneInfo.GetTimeZonesByCountry();

        }

        public async Task<IActionResult> OnPostAsync(string installButton, string saveButton)
        {
            if (!string.IsNullOrEmpty(installButton))
            {
                await _updateService.InstallNewVersionAsync(Customer.OfficeCode, SelectedUpdate);
                await _LoadCoreDataAsync(Customer.Id, true);
                return Page();
            }
            else if(!string.IsNullOrEmpty(saveButton))
            {
                if (!ModelState.IsValid)
                {
                    await _LoadCoreDataAsync(Customer.Id, false);
                    return Page();
                }
                var dbCustomer = await _dbContext.Customers
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.VendorPermissions).ThenInclude(p => p.Permission)
                    .Include(x => x.VendorPermissions).ThenInclude(p => p.Vendor)
                    .Include(x => x.Server)
                    .FirstOrDefaultAsync(x => x.Id == Customer.Id);

                dbCustomer.Name = Customer.Name;
                dbCustomer.OfficeCode = Customer.OfficeCode;
                dbCustomer.ServerId = Customer.ServerId;
                dbCustomer.Database = Customer.Database;
                dbCustomer.Inactive = Customer.Inactive;
                dbCustomer.Notes = Customer.Notes;
                dbCustomer.TimeZoneId = Customer.TimeZoneId;
                dbCustomer.DateUpdated = DateTime.Now;
                dbCustomer.VendorPermissions = JsonConvert.DeserializeObject<HashSet<CustomerVendorPermission>>(Customer.PermissionList);

                _dbContext.Attach(dbCustomer).State = EntityState.Modified;


                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new Exception($"Customer {Customer.Id} not found!");
                }

                return RedirectToPage("/Support/Customers");
            }

            throw new Exception("Invalid Submittal!");

        }

        public bool ConnectionSucceeded { get; set; }
        public string TestConnectionMessage { get; set; }

    }
}