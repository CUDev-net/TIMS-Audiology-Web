using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Server.Data;

namespace TIMS_X.Server.Queries;

public class UserQuery
{
	private readonly UserDbContext _dbContext;

	public UserQuery(UserDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<bool> DoesUserHaveSettingAsync(int userId, SettingEnum setting)
	{
		if (userId == 0)
			return true;
		var groups = _dbContext.UserGroups
			.Include(x => x.Settings)
			.Include(x => x.UserReferences)
			.Where(x => x.Settings.Any(s => s.PermissionType == setting) &&
			            x.UserReferences.Any(s => s.UserId == userId));

		return await groups.AnyAsync();
	}

	public User Get(int userId)
	{
		var query = _dbContext.Users.Where(x => x.Id == userId);

		User user = null;
		try
		{
			user = query.FirstOrDefault();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}


		return user;
	}

	public async Task<User> GetAdUserAsync(string domain, string username)
	{
		if (string.IsNullOrWhiteSpace(domain) && string.IsNullOrWhiteSpace(username)) return null;

		var query = _dbContext.Users.Where(u => u.AdDomain == domain && u.AdUsername == username);

		User user = null;
		try
		{
			user = await query.FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}


		return user;
	}

	public async Task<IEnumerable<User>> GetAllAsync(bool includeInactive)
	{
		var query = _dbContext.Users
			.Where(x => (includeInactive || !x.Inactive) && x.Id > 0 && !x.Deleted)
			.OrderBy(x => x.Name);

		List<User> users;
		try
		{
			users = await query.ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		foreach (var user in users) user.Password = null;

		return users;
	}

	public async Task<User> GetAsync(int userId)
	{
		var query = _dbContext.Users.Where(x => x.Id == userId);

		User user = null;
		try
		{
			user = await query.FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}


		return user;
	}

	public async Task<int> GetDefaultMessageReceiverAsync()
	{
		var id = await _dbContext.Users.Where(u => u.DefaultMessageReceiver).Select(u => u.Id).FirstOrDefaultAsync();
		return id;
	}

	public async Task GetExtraRecentPatientsAsync(List<PatientItem> patientList,
		int userId)
	{
		if (patientList.Count >= 20) return;
		var patIds = patientList.Select(x => x.Id).ToList();

		var patients = await _dbContext.Patients.Where(x =>
				!patIds.Contains(x.Id) && !x.Inactive && x.UpdatedUserId.HasValue &&
				x.UpdatedUserId.Value == userId)
			.OrderByDescending(p => p.UpdatedDate)
			.Take(20 - patientList.Count)
			.Select(x => new PatientItem
			{
				Id = x.Id,
				SiteId = x.SiteId,
				Inactive = x.Inactive,
				FirstName = x.FirstName,
				LastName = x.LastName,
				Initial = x.Initial,
				Gender = x.Sex == "M" ? Gender.Male : x.Sex == "F" ? Gender.Female : Gender.Unknown,
				BirthDate = x.BirthDate
			})
			.ToListAsync();

		patientList.AddRange(patients);
	}

	public async Task<Site> GetFirstSiteAsync()
	{
		var query = _dbContext.Sites
			.Where(x => x.Id > 0 && !x.Inactive);

		Site site = null;
		try
		{
			site = await query.FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return site;
	}

	public async Task<int> GetFirstUserIdAsync()
	{
		var query = _dbContext.Users
			.Where(x => x.Id > 0 && !x.Inactive)
			.Select(x => x.Id);

		var userId = 0;
		try
		{
			userId = await query.FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}


		return userId;
	}

	public async Task<LastPatientList> GetLastPatientListAsync(int userId)
	{
		var query = _dbContext.LastPatientLists
			.Where(x => x.UserId == userId);

		return await query.FirstOrDefaultAsync();
	}

	public async Task<IEnumerable<UserItem>> GetLoginUsersAsync(bool includeInactive)
	{
		var query = _dbContext.Users
			.Where(x => (includeInactive || !x.Inactive) && x.Id > 0 && !x.Deleted)
			.OrderBy(x => x.Name)
			.Select(x => new UserItem
			{
				Id = x.Id,
				Inactive = x.Inactive,
				Name = x.Name,
				AdDomain = x.AdDomain,
				AdUsername = x.AdUsername,
				IsWebUser = x.IsWebUser,
				SiteId = x.SiteId
			});

		IEnumerable<UserItem> users = null;
		try
		{
			users = await query.ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}


		return users;
	}

	public async Task<UserTaskType> GetOrCreateUserTaskTypeAsync(string name)
	{
		var existingType = await _dbContext.UserTaskTypes.FirstOrDefaultAsync(x => x.Name == name);
		if (existingType == null)
		{
			existingType = new UserTaskType
			{
				Name = name
			};
			await _dbContext.UserTaskTypes.AddAsync(existingType);
			await _dbContext.SaveChangesAsync();
		}

		return existingType;
	}

	public async Task<PatientItem> GetPatientAsync(int patientId)
	{
		var query = _dbContext.Patients
			.Where(x => x.Id == patientId)
			.Select(x => new PatientItem
			{
				Id = x.Id,
				SiteId = x.SiteId,
				Inactive = x.Inactive,
				FirstName = x.FirstName,
				LastName = x.LastName,
				Initial = x.Initial,
				Gender = x.Sex == "M" ? Gender.Male : x.Sex == "F" ? Gender.Female : Gender.Unknown,
				BirthDate = x.BirthDate
			});


		PatientItem result = null;
		try
		{
			result = await query.FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}


		return result;
	}

	public Site GetSite(int siteId, bool allowInactive = false)
	{
		var site = _dbContext.Sites.FirstOrDefault(x => x.Id == siteId &&
		                                                (allowInactive || !x.Inactive));
		return site;
	}

	public async Task<Site> GetSiteAsync(int siteId, bool allowInactive = false)
	{
		var site = await _dbContext.Sites.FirstOrDefaultAsync(x => x.Id == siteId &&
		                                                           (allowInactive || !x.Inactive));
		return site;
	}

	public async Task<UserSiteHours> GetTimsUserSiteAsync(int userId, int dayNumber, int siteId)
	{
		var timsUserSite = await _dbContext.TIMSUserSites.FirstOrDefaultAsync(x =>
			x.UserId == userId && x.DayNum == dayNumber && x.SiteId == siteId);
		return timsUserSite;
	}


	public int GetUserId(bool ad, string username)
	{
		User user;
		if (ad)
			user = _dbContext.Users
				.Where(u => !u.Inactive && u.AdUsername == username)
				.FirstOrDefault();
		else
			user = _dbContext.Users
				.Where(u => !u.Inactive && u.Name == username)
				.FirstOrDefault();

		return user == null ? -1 : user.Id;
	}

	public async Task PutLastPatientListAsync(LastPatientList lastPatientList)
	{
		var patientList = await _dbContext.LastPatientLists.FindAsync(lastPatientList.Id);
		if (patientList == null)
			await _dbContext.LastPatientLists.AddAsync(lastPatientList);
		else
			patientList.PatientListXml = lastPatientList.PatientListXml;

		await _dbContext.SaveChangesAsync();
	}

	public async Task PutUserTaskAsync(UserTask userTask, int forUserId, string alertTitle, string alertMessage)
	{
		await _dbContext.UserTasks.AddAsync(userTask);
		await _dbContext.SaveChangesAsync();
		var userTaskUserReference = new UserTaskUserReference
		{
			UserId = forUserId,
			UserTaskId = userTask.Id
		};
		await _dbContext.UserTasksUserReferences.AddAsync(userTaskUserReference);
		await _dbContext.SaveChangesAsync();

		var alert = new Alert
		{
			AlertObjectId = userTask.Id,
			AlertType = AlertTypeEnum.Task,
			Name = alertTitle,
			Description = alertMessage,
			DueDate = DateTime.Now,
			AlertUserId = forUserId,
			CreatedUserId = 0,
			CreatedDate = DateTime.Now
		};
		await _dbContext.Alerts.AddAsync(alert);
		await _dbContext.SaveChangesAsync();
	}
}