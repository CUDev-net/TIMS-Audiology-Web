using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.DAL;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Extensions;
using TIMS_X.Server.Middleware;

namespace TIMS_X.Server.Controllers.Web
{
    /// <summary>
    /// This controller provides helper API endpoints for the Customer Portal
    /// </summary>
    [Route("web/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ValidationController : ControllerBase
    {
        #region Constructors

        public ValidationController(TimsInternalDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _appSettings = configuration.Get<AppSettings>();
        }

        #endregion Constructors

        #region InternalController Members

        

        /// <summary>
        /// Remote validation endpoint. Ensures api url is unique
        /// </summary>
        [HttpGet("ValidateApiUrl")]
        public IActionResult ValidateApiUrl([Bind(Prefix = "ApiUrl.Id")]int id, [Bind(Prefix = "ApiUrl.Url")]string url)
        {
            if (_dbContext.ApiUrls.Any(x => x.Id != id && string.Equals(x.Url, url, StringComparison.CurrentCultureIgnoreCase)))
            {
                return new JsonResult($"{url} already exists");
            }
            return new JsonResult(true);
        }

        /// <summary>
        /// Remote validation endpoint. Ensures customer server/database combination is unique
        /// </summary>
        [HttpGet("ValidateDatabaseConnection")]
        public async Task<IActionResult> ValidateDatabaseConnectionAsync(int serverId, string database)
        {
            var connectionTestResult = false;
            var server = await _dbContext.TimsServers.FirstOrDefaultAsync(x => x.Id == serverId);
            if (server != null)
            {
                // 3. Build Db Context
                var options = new DbContextOptionsBuilder<PracticeDbContext>();
                ConnectionStringBuilder.SetConnectionString(options, server.Address, database, _appSettings.Keys.DbUsername, _appSettings.Keys.DbPassword);
                var dbContext = new PracticeDbContext(options.Options);

                // 4. Test Connection
                connectionTestResult = await dbContext.Database.ExistsAsync();

            }

            return Ok(connectionTestResult);
        }

        /// <summary>
        /// Remote validation endpoint. Ensures customer server/database combination is unique
        /// </summary>
        [HttpGet("ValidateCustomerDatabase")]
        public IActionResult ValidateCustomerDatabase([Bind(Prefix = "Customer.Id")]int id, [Bind(Prefix = "Customer.ServerId")]int serverId, [Bind(Prefix = "Customer.Database")]string database)
        {
            var customer = _dbContext.Customers
                 .Include(x => x.Server)
                 .FirstOrDefault(x => x.Id != id && x.ServerId == serverId && x.Database == database);

            if (customer != null)
            {
                return new JsonResult($"{database} already exists on {customer.Server.Name}. Customer: {customer.Name}");

            }
            return new JsonResult(true);
        }

        /// <summary>
        /// Remote validation endpoint. Ensures customer office code is unique
        /// </summary>
        [HttpGet("ValidateCustomerOfficeCode")]
        public IActionResult ValidateCustomerOfficeCode([Bind(Prefix = "Customer.Id")]int id, [Bind(Prefix = "Customer.OfficeCode")]string officeCode)
        {
            if (_dbContext.Customers.Any(x => x.Id != id && x.OfficeCode == officeCode))
            {
                return new JsonResult($"{officeCode} already exists");
            }
            return new JsonResult(true);
        }

        /// <summary>
        /// Remote validation endpoint. Ensures server ip address is unique
        /// </summary>
        [HttpGet("ValidateServerAddress")]
        public IActionResult ValidateServerAddress([Bind(Prefix = "Server.Id")]int id, [Bind(Prefix = "Server.Address")]string address)
        {
            if (_dbContext.TimsServers.Any(x => x.Id != id && x.Address == address))
            {
                return new JsonResult($"{address} already exists");
            }
            return new JsonResult(true);
        }

        /// <summary>
        /// Remote validation endpoint. Ensures server name is unique
        /// </summary>
        [HttpGet("ValidateServerName")]
        public IActionResult ValidateServerName([Bind(Prefix = "Server.Id")]int id, [Bind(Prefix = "Server.Name")]string name)
        {
            if (_dbContext.TimsServers.Any(x => x.Id != id && x.Name == name))
            {
                return new JsonResult($"{name} already exists");
            }
            return new JsonResult(true);
        }

        /// <summary>
        /// Remote validation endpoint. Ensures support user email is unique
        /// </summary>
        [HttpGet("ValidateSupportUserEmail")]
        public IActionResult ValidateSupportUserEmail([Bind(Prefix = "SupportUser.Id")]int id, [Bind(Prefix = "SupportUser.Email")]string email)
        {
            if (_dbContext.SupportUsers.Any(x => x.Id != id && x.Email == email))
            {
                return new JsonResult($"User with email '{email}' already exists");
            }
            return new JsonResult(true);
        }

        /// <summary>
        /// Remote validation endpoint. Ensures support user name is unique
        /// </summary>
        [HttpGet("ValidateSupportUserName")]
        public IActionResult ValidateSupportUserName([Bind(Prefix = "SupportUser.Id")]int id, [Bind(Prefix = "SupportUser.Name")]string name)
        {
            if (_dbContext.SupportUsers.Any(x => x.Id != id && x.Name == name))
            {
                return new JsonResult($"User with name '{name}' already exists");
            }
            return new JsonResult(true);
        }

        /// <summary>
        /// Remote validation endpoint. Ensures vendor api key is unique
        /// </summary>
        [HttpGet("ValidateVendorApiKey")]
        public IActionResult ValidateVendorApiKey([Bind(Prefix = "Vendor.Id")]int id, [Bind(Prefix = "Vendor.ApiKey")]string apiKey)
        {
            if (_dbContext.Vendors.Any(x => x.Id != id && x.ApiKey == apiKey))
            {
                return new JsonResult($"{apiKey} already exists");
            }
            return new JsonResult(true);
        }

        /// <summary>
        /// Remote validation endpoint. Ensures vendor name is unique
        /// </summary>
        [HttpGet("ValidateVendorName")]
        public IActionResult ValidateVendorName([Bind(Prefix = "Vendor.Id")]int id, [Bind(Prefix = "Vendor.Name")]string name)
        {
            if (_dbContext.Vendors.Any(x => x.Id != id && x.Name == name))
            {

                return new JsonResult($"{name} already exists");

            }
            return new JsonResult(true);
        }

        /// <summary>
        /// Remote validation endpoint. Ensures vendor permission name is unique
        /// </summary>
        [HttpGet("ValidateVendorPermissionName")]
        public IActionResult ValidateVendorPermissionName([Bind(Prefix = "VendorPermission.Id")]int id, [Bind(Prefix = "VendorPermission.Name")]string name)
        {
            if (_dbContext.VendorPermissions.Any(x => x.Id != id && x.Name == name))
            {
                return new JsonResult($"{name} already exists");
            }
            return new JsonResult(true);
        }

        #endregion InternalController Members

        #region Fields

        private readonly AppSettings _appSettings;
        private readonly TimsInternalDbContext _dbContext;

        #endregion Fields

    }
}