using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.BLL.Repositories;
using TIMS_X.Core.Utils;

namespace TIMS_X.Server.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
[ApiExplorerSettings(IgnoreApi = true)]
public class PracticeController : ControllerBase
{
    private readonly IPracticeRepository _practiceRepository;

    public PracticeController(IPracticeRepository practiceRepository)
    {
        _practiceRepository = practiceRepository;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var practice = await _practiceRepository.GetPractice();
        return Ok(practice);
    }

    [HttpGet("get-businessrules")]
    public async Task<IActionResult> GetBusinessRules()
    {
        var practiceHours = await _practiceRepository.GetBusinessRules();

        return Ok(practiceHours);
    }

    [HttpGet("get-hours")]
    public async Task<IActionResult> GetHours()
    {
        var practiceHours = await _practiceRepository.GetPracticeHours();

        return Ok(practiceHours);
    }

    [HttpGet("Summary")]
    public async Task<ActionResult> GetSummary()
    {
        var practice = await _practiceRepository.GetPracticeSummary();
        return Ok(practice);
    }
}