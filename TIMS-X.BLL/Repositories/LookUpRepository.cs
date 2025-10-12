using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ILookUpRepository
{
	Task<CityStateLookup> GetCityAndStateFromZipCode(string zipCode);
	Task<Description> GetDescriptions();
	Task<List<EmplStatus>> GetEmplStatuses();
	Task<List<Ethnicity>> GetEthnicities();
	Task<List<Sex>> GetGenders();
	Task<List<Language>> GetLanguages();
	Task<List<MaritalStatus>> GetMaritalStatuses();
	List<Race> GetRaces();
	Task<List<StudentStatus>> GetStudentStatuses();
}

public class CityStateLookup
{
	public string City { get; set; }
	public string Country { get; set; }
	public string State { get; set; }
}

public class LookUpRepository : ILookUpRepository
{
	private readonly IDescriptionUnitOfWork _descriptionUnitOfWork;
	private readonly IEmplStatusUnitOfWork _emplStatusUnitOfWork;
	private readonly IGenderUnitOfWork _genderUnitOfWork;
	private readonly IMaritalStatusUnitOfWork _maritalStatusUnitOfWork;
	private readonly IPracticeUnitOfWork _practiceUnitOfWork;
	private readonly IStudentStatusUnitOfWork _studentStatusUnitOfWork;

	public LookUpRepository(IGenderUnitOfWork genderUnitOfWork,
		IPracticeUnitOfWork practiceUnitOfWork,
		IEmplStatusUnitOfWork emplStatusUnitOfWork,
		IMaritalStatusUnitOfWork maritalStatusUnitOfWork,
		IStudentStatusUnitOfWork studentStatusUnitOfWork,
		IDescriptionUnitOfWork descriptionUnitOfWork)
	{
		_genderUnitOfWork = genderUnitOfWork;
		_practiceUnitOfWork = practiceUnitOfWork;
		_emplStatusUnitOfWork = emplStatusUnitOfWork;
		_maritalStatusUnitOfWork = maritalStatusUnitOfWork;
		_studentStatusUnitOfWork = studentStatusUnitOfWork;
		_descriptionUnitOfWork = descriptionUnitOfWork;
	}

	public async Task<CityStateLookup> GetCityAndStateFromZipCode(string zipCode)
	{
		var cityStateLookup = new CityStateLookup();
		if (string.IsNullOrEmpty(zipCode)) return cityStateLookup;

		var url = "http://ziptasticapi.com/" + zipCode;
		using var client = new HttpClient();
		using var response = await client.GetAsync(url);

		if (response.IsSuccessStatusCode)
		{
			var content = await response.Content.ReadAsStringAsync();
			cityStateLookup = JsonConvert.DeserializeObject<CityStateLookup>(content);
		}

		return cityStateLookup;
	}

	public async Task<List<Ethnicity>> GetEthnicities()
	{
		var practice = await _practiceUnitOfWork.GetPracticeSummary();
		return Ethnicity.LoadAll(practice.Locale);
	}

	public async Task<List<Sex>> GetGenders()
	{
		return await _genderUnitOfWork.GetGenders().ToListAsync();
	}

	public async Task<List<EmplStatus>> GetEmplStatuses()
	{
		return await _emplStatusUnitOfWork.GetEmplStatuses();
	}

	public async Task<List<MaritalStatus>> GetMaritalStatuses()
	{
		return await _maritalStatusUnitOfWork.GetMaritalStatuses();
	}

	public async Task<List<StudentStatus>> GetStudentStatuses()
	{
		return await _studentStatusUnitOfWork.GetStudentStatuses();
	}

	public async Task<List<Language>> GetLanguages()
	{
		var practice = await _practiceUnitOfWork.GetPracticeSummary();
		return Language.LoadAll(practice.Locale);
	}

	public List<Race> GetRaces()
	{
		return Race.LoadAll();
	}

	public async Task<Description> GetDescriptions()
	{
		return await _descriptionUnitOfWork.GetDescriptions();
	}
}