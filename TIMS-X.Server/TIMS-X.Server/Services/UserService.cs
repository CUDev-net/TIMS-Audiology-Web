using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BCrypt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Models;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Services;

public class UserService
{
	private static readonly string XML_VALUE = "Value";
	private readonly AppSettings _appSettings;
	private readonly UserDbContext _dbContext;
	private readonly UserQuery _userQuery;

	public UserService(IOptions<AppSettings> appSettings, UserQuery userQuery, UserDbContext dbContext)
	{
		_appSettings = appSettings.Value;

		_userQuery = userQuery;
		_dbContext = dbContext;
	}


	private string _GenerateToken(int userId, int siteId, string officeCode)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_appSettings.Keys.JwtSecret);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
			{
				new Claim(StringConstants.User, userId.ToString()),
				new Claim(StringConstants.Site, siteId.ToString()),
				new Claim(ClaimTypes.Role, StringConstants.Customer),
				new Claim(StringConstants.OfficeCode, officeCode)
			}),

			Expires = DateTime.UtcNow.AddDays(7),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature)
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public async Task AddRecentPatientAsync(int userId, int patientId)
	{
		var listContainer = await _userQuery.GetLastPatientListAsync(userId);
		var patientIds = new List<int>();
		if (listContainer != null && !string.IsNullOrWhiteSpace(listContainer.PatientListXml))
		{
			var xSetting = XDocument.Parse(listContainer.PatientListXml).Root;
			if (xSetting != null)
			{
				var xValue = (from p in xSetting.Attributes()
					where p.Name == XML_VALUE
					select p).FirstOrDefault();
				if (xValue != null)
				{
					var temp = xValue.Value;
					var items = temp.Split(',');
					foreach (var item in items)
						if (int.TryParse(item, out var existingId))
							patientIds.Add(existingId);

					var existing = patientIds.FirstOrDefault(x => x == patientId);
					if (existing != 0) patientIds.Remove(existing);
					patientIds.Insert(0, patientId);
					if (patientIds.Count() > 20) patientIds = patientIds.GetRange(0, 20);
					xValue.Value = string.Join(',', patientIds);

					listContainer.PatientListXml = xSetting.ToString();

					await _userQuery.PutLastPatientListAsync(listContainer);
				}
			}
		}
	}

	public async Task<User> AuthenticateApiUserAdAsync(AuthenticationForm form, string officeCode)
	{
		// Handle Support Login
		if (form.UserId == 0) return await AuthenticateApiUserAsync(form, officeCode);

		var user = await _userQuery.GetAdUserAsync(form.AdDomain, form.AdUsername);

		if (user == null) return null;

		var site = await _userQuery.GetSiteAsync(form.SiteId);

		if (site == null) throw new ValidationException($"Site with id {form.SiteId} is invalid.");

		if (form.TimsToken != _appSettings.Keys.TimsToken) throw new ValidationException("Invalid token.");

		var connectionInfo = _dbContext.Database.GetDbConnection();
		var server = connectionInfo.DataSource;
		var database = connectionInfo.Database;

		user.Jwt = _GenerateToken(user.Id, form.SiteId, officeCode);
		user.Password = null;
		return user;
	}

	public async Task<User> AuthenticateApiUserAsync(AuthenticationForm form, string officeCode)
	{
		var isSupportLogin = form.Password == _appSettings.Keys.SupportPassword;

		if (isSupportLogin)
			form.UserId = 0;

		var user = await _userQuery.GetAsync(form.UserId);

		if (user == null) return null;

		var site = await _userQuery.GetSiteAsync(form.SiteId);


		if (site == null) site = await _userQuery.GetFirstSiteAsync();

		if (site == null) throw new ValidationException($"Site with id {form.SiteId} is invalid.");

		var connectionInfo = _dbContext.Database.GetDbConnection();
		var server = connectionInfo.DataSource;
		var database = connectionInfo.Database;


		if (isSupportLogin || BCryptHelper.CheckPassword(form.Password, user.Password))
		{
			user.Jwt = _GenerateToken(user.Id, site.Id, officeCode);
			user.Password = null;
			return user;
		}

		return null;
	}

	public async Task<User> AuthenticateWebUserAsync(AuthenticationForm form)
	{
		var isSupportLogin = form.Password == _appSettings.Keys.SupportPassword;

		if (isSupportLogin)
			form.UserId = 0;

		var user = await _userQuery.GetAsync(form.UserId);

		if (user == null) return null;


		if (isSupportLogin || BCryptHelper.CheckPassword(form.Password, user.Password))
		{
			user.Password = null;
			return user;
		}

		return null;
	}

	public async Task<IEnumerable<User>> GetAllAsync(bool includeInactive)
	{
		return await _userQuery.GetAllAsync(includeInactive);
	}


	public async Task<User> GetAsync(int userId)
	{
		return await _userQuery.GetAsync(userId);
	}

	public async Task<int> GetFirstUserIdAsync()
	{
		return await _userQuery.GetFirstUserIdAsync();
	}

	public async Task<IEnumerable<UserItem>> GetLoginUsersAsync(bool includeInactive)
	{
		return await _userQuery.GetLoginUsersAsync(includeInactive);
	}

	public async Task<List<PatientItem>> GetRecentPatientsListAsync(int userId)
	{
		var result = new List<PatientItem>();
		var listContainer = await _userQuery.GetLastPatientListAsync(userId);

		if (listContainer != null && !string.IsNullOrWhiteSpace(listContainer.PatientListXml))
		{
			var xSetting = XDocument.Parse(listContainer.PatientListXml).Root;
			if (xSetting != null)
			{
				var xValue = (from p in xSetting.Attributes()
					where p.Name == XML_VALUE
					select p).FirstOrDefault();
				if (xValue != null)
				{
					var temp = xValue.Value;
					var items = temp.Split(',');
					foreach (var item in items)
						if (int.TryParse(item, out var patientId))
						{
							var patient = await _userQuery.GetPatientAsync(patientId);
							if (patient != null) result.Add(patient);
						}
				}
			}
		}

		if (result.Count < 20) await _userQuery.GetExtraRecentPatientsAsync(result, userId);

		return result;
	}

	public int GetUserId(bool ad, string username)
	{
		return _userQuery.GetUserId(ad, username);
	}

	public async Task<bool> IsUserAtSiteAsync(int userId, int siteId, DateTime apptStart, DateTime apptEnd)
	{
		var dayNumber = (int)apptStart.Date.DayOfWeek;
		var timsUserSite = await _userQuery.GetTimsUserSiteAsync(userId, dayNumber, siteId);
		return timsUserSite != null && timsUserSite.StartTime.HasValue && timsUserSite.EndTime.HasValue &&
		       timsUserSite.StartTime.Value.TimeOfDay <= apptStart.TimeOfDay &&
		       timsUserSite.EndTime.Value.TimeOfDay >= apptEnd.TimeOfDay;
	}
}