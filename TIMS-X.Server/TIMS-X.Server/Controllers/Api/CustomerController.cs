using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Controllers.Api
{
    /// <summary>
    /// The customer controller provides API endpoints for querying the internal TIMS database for customers and vendor permissions
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomerController : ControllerBase
    {
#region Constructors

        public CustomerController(CustomerQuery customerQuery)
        {
            _customerQuery = customerQuery;
        }

        #endregion Constructors

        #region CustomerController Members


        /// <summary>
        /// Retrieves the customer server IP address and database name
        /// </summary>
        /// <returns>{IpAddress, Server}</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("ConnectionInfo")]
        public async Task<IActionResult> GetConnectionInfoAsync()
        {
            var officeCode = Request.Headers.GetValue(StringConstants.OfficeCode).ToString();
            if (string.IsNullOrWhiteSpace(officeCode))
            {
                return Ok(new ConnectionInfo());
            }

            var connectionInfo = await _customerQuery.GetConnectionInfoAsync(officeCode);

            return Ok(connectionInfo);
        }


        /// <summary>
        /// Allows client to ensure it can reach this server.
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        [HttpGet("Ping")]
        public IActionResult Ping()
        {
            return Ok(true);
        }

        /// <summary>
        /// Tests a customer database connection. Returns true if it can connect, false otherwise.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("ValidateCustomer")]
        public async Task<IActionResult> ValidateCustomerAsync()
        {
            var officeCode = Request.Headers.GetValue(StringConstants.OfficeCode).ToString();
            if (string.IsNullOrWhiteSpace(officeCode))
            {
                return Ok(false);
            }

            var connectionTestResult = await _customerQuery.ValidateCustomerAsync(officeCode);

            return Ok(connectionTestResult);
        }

#endregion CustomerController Members

#region Fields

        private readonly CustomerQuery _customerQuery;

#endregion Fields

    }

}