using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Controllers.Common;
using TIMS_X.Server.Services;
using Azure.Storage.Blobs;
using System.Net.Http;
using System.Net;

namespace TIMS_X.Server.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TimsUpdateController : ControllerBase
    {
        
        private readonly TimsUpdateService _updateService;
        
        public TimsUpdateController(TimsUpdateService updateService)
        {
            _updateService = updateService;
        }

        [HttpGet("DownloadExists")]
        public async Task<IActionResult> DownloadExistsAsync(string version)
        {
            var exists = await _updateService.DownloadExistsAsync(version);
            return Ok(exists);
        }

        [HttpGet("Download")]
        public async Task<IActionResult> DownloadAsync(string version)
        {
            var stream = await _updateService.GetFileStreamAsync(version);
            return File(stream, "application/octet-stream");
        }

    }
}
