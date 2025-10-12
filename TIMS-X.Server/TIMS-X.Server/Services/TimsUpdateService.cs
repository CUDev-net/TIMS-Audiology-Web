using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Middleware;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using Version = TIMS_X.Core.Domain.Version;

namespace TIMS_X.Server.Services;

public class TimsUpdateService
{
	private readonly AppSettings _appSettings;
	private readonly BlobServiceClient _blobServiceClient;
	private readonly CustomerQuery _customerQuery;

	public TimsUpdateService(CustomerQuery customerQuery, IConfiguration configuration,
		BlobServiceClient blobServiceClient)
	{
		_customerQuery = customerQuery;
		_appSettings = configuration.Get<AppSettings>();
		_blobServiceClient = blobServiceClient;
	}

	public async Task<bool> DownloadExistsAsync(string versionNumber)
	{
		var installerName = VersionToPath(versionNumber);
		var blobContainer = _blobServiceClient.GetBlobContainerClient(StringConstants.InstallerContainerName);
		var blobClient = blobContainer.GetBlobClient(installerName);
		return await blobClient.ExistsAsync();
	}

	public List<TimsUpdate> GetAvailableUpdates(string currentVersion)
	{
		var updates = new List<TimsUpdate>();

		var blobContainer = _blobServiceClient.GetBlobContainerClient(StringConstants.InstallerContainerName);
		var blobs = blobContainer.GetBlobs();

		if (!string.IsNullOrWhiteSpace(currentVersion))
		{
			var currentVersionNumbers = currentVersion.Split('.');
			if (currentVersionNumbers.Length == 4 && int.TryParse(currentVersionNumbers[3], out var minorVersion))
				foreach (var blob in blobs)
				{
					var updateParts = blob.Name.Split("_");
					if (updateParts.Length == 5)
					{
						updateParts[4] = updateParts[4].Split(".")[0];
						// same major version and bigger minor version?
						if (updateParts[3] == currentVersionNumbers[2] &&
						    int.TryParse(updateParts[4], out var updateMinorVersion) &&
						    updateMinorVersion > minorVersion)
						{
							var versionStr = $"{updateParts[1]}.{updateParts[2]}.{updateParts[3]}.{updateParts[4]}";

							updates.Add(new TimsUpdate
							{
								ReleaseDate = blob.Properties.CreatedOn?.LocalDateTime ?? DateTime.Now,
								Version = versionStr,
								BlobName = blob.Name
							});
						}
					}
				}
		}

		return updates;
	}

	public async Task<string> GetCurrentVersionAsync(string officeCode)
	{
		var version = "(Error retrieving version)";
		try
		{
			var connectionInfo = await _customerQuery.GetConnectionInfoAsync(officeCode);
			var options = new DbContextOptionsBuilder<TimsUpdateDbContext>();
			ConnectionStringBuilder.SetConnectionString(options, connectionInfo.Server, connectionInfo.Database,
				_appSettings.Keys.DbUsername, _appSettings.Keys.DbPassword);
			var dbContext = new TimsUpdateDbContext(options.Options);
			var updateQuery = new TimsUpdateQuery(dbContext);
			version = await updateQuery.GetCurrentVersionAsync();
		}
		catch
		{
			// do nothing
		}

		return version;
	}

	public async Task<Tuple<int, int>> GetCurrentVersionIntAsync(string officeCode)
	{
		string versionStr = null;
		var versionMajor = 0;
		var versionMinor = 0;
		try
		{
			var connectionInfo = await _customerQuery.GetConnectionInfoAsync(officeCode);
			var options = new DbContextOptionsBuilder<TimsUpdateDbContext>();
			ConnectionStringBuilder.SetConnectionString(options, connectionInfo.Server, connectionInfo.Database,
				_appSettings.Keys.DbUsername, _appSettings.Keys.DbPassword);
			var dbContext = new TimsUpdateDbContext(options.Options);
			var updateQuery = new TimsUpdateQuery(dbContext);
			versionStr = await updateQuery.GetCurrentVersionAsync();
		}
		catch
		{
			// do nothing
		}

		if (!string.IsNullOrEmpty(versionStr))
		{
			var components = versionStr.Split(".");
			if (components.Length == 4)
			{
				_ = int.TryParse(components[2], out versionMajor);
				_ = int.TryParse(components[3], out versionMinor);
			}
		}

		return new Tuple<int, int>(versionMajor, versionMinor);
	}


	public async Task<Stream> GetFileStreamAsync(string versionNumber)
	{
		var installerName = VersionToPath(versionNumber);
		var blobContainer = _blobServiceClient.GetBlobContainerClient(StringConstants.InstallerContainerName);
		var blobClient = blobContainer.GetBlobClient(installerName);
		return await blobClient.OpenReadAsync();
	}

	public async Task<List<Version>> GetVersionHistoryAsync(string officeCode)
	{
		List<Version> result = null;
		try
		{
			var connectionInfo = await _customerQuery.GetConnectionInfoAsync(officeCode);
			var options = new DbContextOptionsBuilder<TimsUpdateDbContext>();
			ConnectionStringBuilder.SetConnectionString(options, connectionInfo.Server, connectionInfo.Database,
				_appSettings.Keys.DbUsername, _appSettings.Keys.DbPassword);
			var dbContext = new TimsUpdateDbContext(options.Options);
			var updateQuery = new TimsUpdateQuery(dbContext);
			result = await updateQuery.GetVersionHistoryAsync();
		}
		catch
		{
			// do nothing
		}

		return result;
	}

	public async Task InstallNewVersionAsync(string officeCode, string version)
	{
		try
		{
			var connectionInfo = await _customerQuery.GetConnectionInfoAsync(officeCode);
			var options = new DbContextOptionsBuilder<TimsUpdateDbContext>();
			ConnectionStringBuilder.SetConnectionString(options, connectionInfo.Server, connectionInfo.Database,
				_appSettings.Keys.DbUsername, _appSettings.Keys.DbPassword);
			var dbContext = new TimsUpdateDbContext(options.Options);
			var updateQuery = new TimsUpdateQuery(dbContext);
			await updateQuery.InstallNewVersionAsync(version);
		}
		catch
		{
			// do nothing
		}
	}

	public string VersionToPath(string version)
	{
		//https://timsaudiology.blob.core.windows.net/installercontainer/TIMSAudiology_6_0_7_31603.exe
		if (string.IsNullOrWhiteSpace(version)) return null;

		var parts = version.Split('.');
		if (parts.Length != 4) return null;
		return $"TIMSAudiology_{parts[0]}_{parts[1]}_{parts[2]}_{parts[3]}.exe";
	}
}