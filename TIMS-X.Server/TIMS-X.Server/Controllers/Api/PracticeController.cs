using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PracticeController : ControllerBase
    {
        private readonly PracticeQuery _practiceQuery;

        public PracticeController(PracticeQuery practiceQuery)
        {
            _practiceQuery = practiceQuery;
        }
        
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var practice = await _practiceQuery.GetPracticeAsync();
            return Ok(practice);
        }
        
        [HttpGet("Name")]
        [AllowAnonymous]
        public async Task<ActionResult> GetNameAsync()
        {
            var practiceName = await _practiceQuery.GetValueAsync(practice => practice.Name);
            return Ok(practiceName);
        }
        
        [HttpGet("Sites")]
        [AllowAnonymous]
        public async Task<ActionResult> GetSitesAsync(bool includeInactive)
        {
            var sites = await _practiceQuery.GetSitesAsync(includeInactive);
            return Ok(sites);
        }

        [HttpGet("TestConnection")]
        [AllowAnonymous]
        public async Task<IActionResult> TestConnectionAsync()
        {
            var connected = await _practiceQuery.TestConnectionAsync();
            return Ok(connected);
        }

        [HttpGet("UsesAdAuthentication")]
        [AllowAnonymous]
        public async Task<IActionResult> UsesAdAuthenticationAsync()
        {
            var connected = await _practiceQuery.UsesAdAuthenticationAsync();
            return Ok(connected);
        }

        [HttpGet("GetVersion")]
        public IActionResult GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return Ok(version);
        }
    }
}
