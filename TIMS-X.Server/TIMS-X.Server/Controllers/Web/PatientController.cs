using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.BLL.Repositories;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Utils;
using TIMS_X.Core;
using TIMS_X.DAL.Dtos;
using TIMS_X.Server.Services;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
public class PatientController : Controller
{
    private readonly ContextHelper _contextHelper;
    private readonly IPatientRepository _patientRepository;
    private readonly PatientService _patientService;
    private readonly IUserRepository _userRepository;

    public PatientController(PatientService patientService,
        IPatientRepository patientRepository,
        ContextHelper contextHelper,
        IUserRepository userRepository)
    {
        _patientService = patientService;
        _patientRepository = patientRepository;
        _contextHelper = contextHelper;
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Patient patient)
    {
        try
        {
            var created = await _patientRepository.Add(patient);

            var userId = ClaimHelper.GetUserIdFromClaim(User);
            await _userRepository.UpdateRecentPatientList(userId, patient.Id);

            return Ok(created);
        }
        catch (Exception ex)
        {
            //Log.Error(ex, "Error Updating student");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetById(int id)
    {
        var patient = await _patientRepository.Get(id);
        if (patient == null)
            return BadRequest(new { message = $"Patient with {id} id not found" });

        var userId = ClaimHelper.GetUserIdFromClaim(User);
        await _userRepository.UpdateRecentPatientList(userId, patient.Id);

        return Ok(patient);
    }

    [HttpGet("IntakeLink")]
    public async Task<IActionResult> GetIntakeLinkAsync()
    {
        var patientIdString = Request.Headers.Where(x => x.Key.ToLower() == "patientid").Select(x => x.Value)
            .FirstOrDefault()
            .ToString();
        if (string.IsNullOrWhiteSpace(patientIdString))
        {
            Response.StatusCode = 400;
            return new JsonResult("Invalid patient id");
        }

        var (success, data) = await _patientService.GetFormLinkAsync(PatientFormTypeEnum.Intake,
            int.Parse(patientIdString), _contextHelper.CurrentUser.Id);
        if (success)
        {
            return Ok(data);
        }

        Response.StatusCode = 400;
        return new JsonResult(data);
    }

    [HttpGet("Summary")]
    public async Task<IActionResult> GetPatientSummary(int id)
    {
        var patient = await _patientRepository.GetSummary(id);

        var userId = ClaimHelper.GetUserIdFromClaim(User);
        await _userRepository.UpdateRecentPatientList(userId, patient.Id);

        return Ok(patient);
    }

    [HttpPost("Search")]
    public async Task<IActionResult> SearchPatients(PatientSearchCriteriaDto criteriaDto)
    {
        var patient = await _patientRepository.Search(criteriaDto);
        return Ok(patient);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Patient patient)
    {
        try
        {
            var updatedPatient = await _patientRepository.Update(patient);

            var userId = ClaimHelper.GetUserIdFromClaim(User);
            await _userRepository.UpdateRecentPatientList(userId, patient.Id);

            return Ok(updatedPatient);
        }
        catch (Exception ex)
        {
#if TEST
            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                sb.AppendLine(ex.InnerException.Message);
                sb.AppendLine(ex.InnerException.StackTrace);

            }
            return BadRequest(new { message = sb.ToString() });
#else
            return BadRequest(new { message = ex.Message });
#endif
        }
    }
}