using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.BLL;
using TIMS_X.BLL.Repositories;
using TIMS_X.Core.Utils;

namespace TIMS_X.Server.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
public class LookUpController : ControllerBase
{
	private readonly IAppointmentStatusRepository _appointmentStatusRepository;
	private readonly IAppointmentTypeRepository _appointmentTypeRepository;
	private readonly ILookUpRepository _lookUpRepository;
	private readonly IMarketingCategoryRepository _marketingCategoryRepository;
	private readonly IMarketingReferenceRepository _marketingReferenceRepository;
	private readonly IPatientRequiredFieldRepository _patientRequiredFieldRepository;
	private readonly IScheduleBlockRepository _scheduleBlockRepository;

	public LookUpController(ILookUpRepository lookUpRepository,
		IMarketingCategoryRepository marketingCategoryRepository,
		IMarketingReferenceRepository marketingReferenceRepository,
		IAppointmentStatusRepository appointmentStatusRepository,
		IAppointmentTypeRepository appointmentTypeRepository,
		IScheduleBlockRepository scheduleBlockRepository,
		IPatientRequiredFieldRepository patientRequiredFieldRepository)
	{
		_lookUpRepository = lookUpRepository;
		_marketingCategoryRepository = marketingCategoryRepository;
		_marketingReferenceRepository = marketingReferenceRepository;
		_appointmentStatusRepository = appointmentStatusRepository;
		_appointmentTypeRepository = appointmentTypeRepository;
		_scheduleBlockRepository = scheduleBlockRepository;
		_patientRequiredFieldRepository = patientRequiredFieldRepository;
	}

	[HttpGet("get-appointment-statuses")]
	public async Task<IActionResult> GetAppointmentStatuses()
	{
		var appointmentStatuses = await _appointmentStatusRepository.GetAll(false);

		return Ok(appointmentStatuses);
	}

	[HttpGet("get-appointment-types")]
	public async Task<IActionResult> GetAppointmentTypes()
	{
		var appointmentTypes = await _appointmentTypeRepository.GetAll(false);

		return Ok(appointmentTypes);
	}

	[HttpGet("get-descriptions")]
	public async Task<IActionResult> GetDescriptions()
	{
		var descriptions = await _lookUpRepository.GetDescriptions();

		return Ok(descriptions);
	}

	[HttpGet("get-empl-statuses")]
	public async Task<IActionResult> GetEmplStatuses()
	{
		var races = await _lookUpRepository.GetEmplStatuses();

		return Ok(races);
	}

	[HttpGet("get-marital-statuses")]
	public async Task<IActionResult> GetMaritalStatuses()
	{
		var maritalStatuses = await _lookUpRepository.GetMaritalStatuses();
		return Ok(maritalStatuses);
	}

	[HttpGet("get-marketing-categories")]
	public async Task<IActionResult> GetMarketingCategories()
	{
		var marketingCategories = await _marketingCategoryRepository.GetAll(false);

		return Ok(marketingCategories);
	}

	[HttpGet("get-marketing-reference")]
	public async Task<IActionResult> GetMarketingReference(int id)
	{
		var marketingCategories = await _marketingReferenceRepository.Get(id);

		return Ok(marketingCategories);
	}

	[HttpGet("get-marketing-references")]
	public async Task<IActionResult> GetMarketingReferences(int marketingCategoryId)
	{
		var marketingCategories = await _marketingReferenceRepository.GetAll(marketingCategoryId, false);

		return Ok(marketingCategories);
	}

	[HttpGet("get-medicare-secondary-codes")]
	public IActionResult GetMedicareSecondaryCodes()
	{
		var relationShips = Helpers.GetMedicareSecondaryCodes();

		return Ok(relationShips);
	}

	[HttpGet("get-patient-ethnicities")]
	public async Task<IActionResult> GetPatientEtnicities()
	{
		var ethnicities = await _lookUpRepository.GetEthnicities();

		return Ok(ethnicities);
	}

	[HttpGet("get-patient-genders")]
	public async Task<IActionResult> GetPatientGenders()
	{
		var genders = await _lookUpRepository.GetGenders();

		return Ok(genders);
	}

	[HttpGet("get-patient-languages")]
	public async Task<IActionResult> GetPatientLanguages()
	{
		var languages = await _lookUpRepository.GetLanguages();

		return Ok(languages.Where(l => !string.IsNullOrEmpty(l.Name)));
	}

	[HttpGet("get-patient-races")]
	public IActionResult GetPatientRaces()
	{
		var races = _lookUpRepository.GetRaces();

		return Ok(races);
	}

	[HttpGet("get-patient-relationships")]
	public IActionResult GetPatientRelationships()
	{
		var relationShips = Helpers.GetPatientRelationTable();

		return Ok(relationShips);
	}

	[HttpGet("get-patient-required-fields")]
	public async Task<IActionResult> GetPatientRequiredFields()
	{
		var races = await _patientRequiredFieldRepository.GetRequired();

		return Ok(races);
	}

	[HttpGet("get-schedule-blocks")]
	public async Task<IActionResult> GetScheduleBlocks()
	{
		var blocks = await _scheduleBlockRepository.GetAll();

		return Ok(blocks);
	}

	[HttpGet("get-student-statuses")]
	public async Task<IActionResult> GetStudentStatuses()
	{
		var studentStatuses = await _lookUpRepository.GetStudentStatuses();
		return Ok(studentStatuses);
	}

	[HttpGet("lookup-city-state")]
	public async Task<IActionResult> LookupCityAndStateFromZipcode(string zipcode)
	{
		var cityAndState = await _lookUpRepository.GetCityAndStateFromZipCode(zipcode);

		return Ok(cityAndState);
	}
}