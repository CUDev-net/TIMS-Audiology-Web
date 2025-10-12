using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.BLL.Repositories;
using TIMS_X.Core.Utils;

namespace TIMS_X.Server.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
public class ProviderController : ControllerBase
{
    private readonly IProviderRepository _providerRepository;

    public ProviderController(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    [HttpGet("get-summaries")]
    public async Task<IActionResult> GetSummaries(bool includeInactive)
    {
        var providers = await _providerRepository.GetSummaries(includeInactive);
        return Ok(providers);
    }

    [HttpGet("get-summary")]
    public async Task<IActionResult> GetSummary(int id)
    {
        var providers = await _providerRepository.GetSummary(id);
        return Ok(providers);
    }

    [HttpGet("get-with-hours")]
    public async Task<IActionResult> GetWithHours()
    {
        var providers = await _providerRepository.GetWithHours();

        return Ok(providers.OrderBy(p => p.FirstName));
    }
}