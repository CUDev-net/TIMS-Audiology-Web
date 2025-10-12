using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Extensions;
using TIMS_X.Server.Middleware;
using TIMS_X.Server.Models;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Queries;

public class CustomerQuery
{
	private readonly AppSettings _appSettings;
	private readonly TimsInternalDbContext _dbContext;

	public CustomerQuery(TimsInternalDbContext dbContext, IConfiguration configuration)
	{
		_dbContext = dbContext;
		_appSettings = configuration.Get<AppSettings>();
	}


	public async Task<bool> DeleteFormLinkAsync(string url)
	{
		try
		{
			var formLink = await _dbContext.FormLinks.FirstAsync(x => x.Url == url);
			_dbContext.FormLinks.Remove(formLink);
			await _dbContext.SaveChangesAsync();
			return true;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return false;
		}
	}

	public async Task<bool> FormLinkUrlExistsAsync(string url)
	{
		return await _dbContext.FormLinks.Where(x => x.Url == url).AnyAsync();
	}

	public async Task<ConnectionInfo> GetConnectionInfoAsync(string officeCode)
	{
		try
		{
			var connectionInfo = await _dbContext.Customers
				.Include(x => x.Server)
				.AsNoTracking()
				.Where(x => x.OfficeCode == officeCode)
				.Select(x => new ConnectionInfo
				{
					Server = x.Server.Address,
					Database = x.Database,
					User = x.SqlUser,
					Password = x.SqlPassword
				})
				.FirstOrDefaultAsync();


			if (connectionInfo != null)
			{
				if (string.IsNullOrEmpty(connectionInfo.Password))
					connectionInfo.Password = _appSettings.Keys.DbPassword;
				else
					connectionInfo.Password =
						CryptographyHelper.Decrypt(connectionInfo.Password, _appSettings.Keys.ImagingKey);
			}

			return connectionInfo;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task<int> GetCustomerIdAsync(string officeCode)
	{
		var customerId = 0;
		try
		{
			customerId = await _dbContext.Customers.Where(x => x.OfficeCode == officeCode).Select(x => x.Id)
				.FirstOrDefaultAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}

		return customerId;
	}

	public async Task<FormLink> GetFormLinkAsync(string url)
	{
		FormLink formLink = null;
		try
		{
			formLink = await _dbContext.FormLinks.Include(x => x.Customer).FirstOrDefaultAsync(x => x.Url == url);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}

		return formLink;
	}

	public async Task<FormLink> GetFormLinkAsync(int customerId, int userId, int patientId,
		PatientFormTypeEnum formType)
	{
		FormLink formLink = null;
		try
		{
			formLink = await _dbContext.FormLinks.FirstOrDefaultAsync(x => x.CustomerId == customerId &&
			                                                               x.UserId == userId &&
			                                                               x.PatientId == patientId &&
			                                                               x.FormType == formType);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}

		return formLink;
	}

	public async Task<TimsTimeZone?> GetTimeZoneAsync(string officeCode)
	{
		TimsTimeZone? timeZone = null;
		try
		{
			timeZone = await _dbContext.Customers
				.Where(x => x.OfficeCode == officeCode)
				.Select(x => x.TimeZoneId)
				.FirstOrDefaultAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}

		return timeZone;
	}

	public async Task<bool> PutFormLinkAsync(FormLink formLink)
	{
		try
		{
			await _dbContext.FormLinks.AddAsync(formLink);
			await _dbContext.SaveChangesAsync();
			return true;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return false;
		}
	}

	public async Task UpdateConnectionInfoAsync(string officeCode, ConnectionInfo connInfo)
	{
		var dbCustomer = await _dbContext.Customers
			.FirstOrDefaultAsync(x => x.OfficeCode.ToLower() == officeCode.ToLower());

		dbCustomer.DateUpdated = DateTime.Now;
		dbCustomer.SqlUser = connInfo.User;
		dbCustomer.SqlPassword = CryptographyHelper.Encrypt(connInfo.Password, _appSettings.Keys.ImagingKey);
		_dbContext.Attach(dbCustomer).State = EntityState.Modified;
		await _dbContext.SaveChangesAsync();
	}

	public async Task<bool> ValidateCustomerAsync(string officeCode)
	{
		var connectionTestResult = false;
		// find customer in TimsInternal database
		var customer = await _dbContext.Customers
			.Include(x => x.Server)
			.Where(x => x.OfficeCode == officeCode)
			.FirstOrDefaultAsync();

		// 1. Ensure server/db is not null
		if (customer != null)
		{
			customer.SqlPassword = CryptographyHelper.Decrypt(customer.SqlPassword, _appSettings.Keys.ImagingKey);
			// 3. Build Db Context
			var options = new DbContextOptionsBuilder<PracticeDbContext>();
			ConnectionStringBuilder.SetConnectionString(options, customer.Server.Address, customer.Database,
				customer.SqlUser, customer.SqlPassword);
			var dbContext = new PracticeDbContext(options.Options);

			// 4. Test Connection
			connectionTestResult = await dbContext.Database.ExistsAsync();
		}

		return connectionTestResult;
	}
}