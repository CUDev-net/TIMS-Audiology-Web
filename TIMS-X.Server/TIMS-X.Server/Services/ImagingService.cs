using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BCrypt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Domain.Imaging;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Middleware;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Services;

public class ImagingService
{
	public static Guid PhotoDocumentType = new(StringConstants.PhotoDocumentTypeId);
	private static readonly string VideoBasePath = "C:\\ProgramData\\Tims\\Videos";
	private readonly AppSettings _appSettings;
	private readonly ContextHelper _contextHelper;
	private readonly CustomerQuery _customerQuery;
	private readonly ImagingDbContext _dbContext;

	public ImagingService(ImagingDbContext dbContext, IConfiguration configuration, CustomerQuery customerQuery,
		ContextHelper contextHelper)
	{
		_dbContext = dbContext;
		_appSettings = configuration.Get<AppSettings>();
		_contextHelper = contextHelper;
		_customerQuery = customerQuery;
	}

	private Tuple<int, int> _CalculateThumbnailSize(int imageWidth, int imageHeight, int width, int height)
	{
		if (imageWidth < width) width = imageWidth;

		var newHeight = imageHeight * width / imageWidth;
		if (newHeight > height)
		{
			// Resize with height instead
			width = imageWidth * height / imageHeight;
			newHeight = height;
		}

		return new Tuple<int, int>(width, newHeight);
	}

	/// <summary>
	///     Deletes videos files that have expired (the video is still in the database) to save hard drive space
	/// </summary>
	/// <returns></returns>
	private async Task _CleanupExpiredVideoFilesAsync()
	{
		var expiredIds = await _dbContext.PatientLetterArchives
			.Where(x => x.ExpirationDate.HasValue && x.ExpirationDate.Value <= DateTime.Now)
			.Select(x => x.Id)
			.ToListAsync();

		foreach (var id in expiredIds)
		{
			var filename = Path.Combine(VideoBasePath, $"{_contextHelper.OfficeCode}-{id}.mp4".ToLower());
			if (File.Exists(filename)) File.Delete(filename);
		}
	}


	private byte[] _CompressAndEncrypt(byte[] imageBytes)
	{
		imageBytes = CompressionHelper.CompressAsync(imageBytes);
		return CryptographyHelper.Encrypt(imageBytes, _appSettings.Keys.ImagingKey);
	}

	private async Task<Guid> _CreateTimsImageAsync(byte[] imageBytes, Guid docTypeId, Guid serverId)
	{
		try
		{
			var timsImage = new TimsImage
			{
				DocumentTypeId = docTypeId,
				Image = _CompressAndEncrypt(imageBytes),
				IsCompressed = true,
				IsEncrypted = true
			};
			var connInfo = await _GetImagingConnectionInfo(serverId);
			var dbContext = _GetImagingDatabaseConnection(connInfo);
			dbContext.TimsImages.Add(timsImage);
			dbContext.SaveChanges();
			return timsImage.Id;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}


	/// <summary>
	///     Decompresses the binary image data
	/// </summary>
	/// <param name="image"></param>
	/// <returns></returns>
	private void _Decompress(TimsImage image)
	{
		if (image.IsCompressed)
		{
			image.Image = CompressionHelper.Decompress(image.Image);
			image.IsCompressed = false;
		}
	}

	/// <summary>
	///     Decrypts the binary image data
	/// </summary>
	/// <param name="image"></param>
	/// <returns></returns>
	private void _Decrypt(TimsImage image)
	{
		if (image.IsEncrypted)
		{
			image.Image = CryptographyHelper.Decrypt(image.Image.ToArray(), _appSettings.Keys.ImagingKey);
			image.IsEncrypted = false;
		}
	}

	/*
	 * public async Task CreateAppointmentAsync(AppointmentCreateModel createModel)
	{
	    var appointment = new Appointment(createModel);

	    (appointment.OtStatus, _) = await _opportunityTrackingService.GetAppointmentOpportunityStatusAsync(appointment.PatientId);
	    // Validation needs provider object
	    appointment.Provider = await _providerQuery.GetProviderAsync(appointment.ProviderId);
	    await _ValidateAppointmentAsync(appointment);
	    appointment.Provider = null;

	    appointment.UpdatedSiteId = appointment.SyncSiteId = appointment.SiteId;
	    appointment.UpdatedUserId = _contextHelper.CurrentUser.Id;
	    appointment.CreatedUserId = appointment.UpdatedUserId;
	    await _schedulerQuery.PutAppointmentAsync(appointment);
	    if (appointment.Id > 0)
	    {
	        createModel.Id = appointment.Id;
	        await _historyQuery.CreateHistoryForAppointmentAsync(appointment);
	    }
	}
	 */

	/*



	 */

	private async Task<ImageServer> _GetActiveImageServerAsync()
	{
		return await _dbContext.ImageServers
			.OrderByDescending(x => x.DateCreated)
			.FirstOrDefaultAsync(x => x.IsActive);
	}

	//private string _VideoToPath(PatientLetterArchive video)
	//{

	//}

	private async Task<ConnectionInfo> _GetImagingConnectionInfo(Guid serverId)
	{
		var imageServer = await GetImageServerAsync(serverId);
		var connInfo = await _customerQuery.GetConnectionInfoAsync(_contextHelper.OfficeCode);
		connInfo.Database = imageServer?.DatabaseName ?? connInfo.Database;
		return connInfo;
	}

	private ImagingExtDbContext _GetImagingDatabaseConnection(ConnectionInfo connInfo)
	{
		var options = new DbContextOptionsBuilder<ImagingExtDbContext>();
		ConnectionStringBuilder.SetConnectionString(options, connInfo.Server, connInfo.Database,
			connInfo.User, connInfo.Password);
		return new ImagingExtDbContext(options.Options, _contextHelper);
	}


	private string _GetImagingDatabaseConnectionString(ConnectionInfo connInfo, string imageDatabase)
	{
		return ConnectionStringBuilder.GetConnectionString(connInfo.Server, imageDatabase, connInfo.User,
			connInfo.Password);
	}


	private async Task<PatientLetterArchive> _GetVideoMetadataAsync(ConnectionInfo conn, int videoId)
	{
		PatientLetterArchive video = null;
		// find customer in TimsInternal database

		//  Build Db Context
		var options = new DbContextOptionsBuilder<ImagingDbContext>();
		ConnectionStringBuilder.SetConnectionString(options, conn.Server, conn.Database, conn.User, conn.Password);
		var dbContext = new ImagingDbContext(options.Options, _contextHelper);

		video = await dbContext.PatientLetterArchives.FirstOrDefaultAsync(x => x.Id == videoId);

		return video;
	}

	private async Task _PrepareVideoFileAsync(PatientLetterArchive video, ConnectionInfo connInfo)
	{
		var filename = Path.Combine(VideoBasePath, $"{_contextHelper.OfficeCode}-{video.Id}.mp4".ToLower());
		if (video.WebAccess)
		{
			if (!File.Exists(filename))
			{
				var connectionString = _GetImagingDatabaseConnectionString(connInfo, connInfo.Database);

				using (var connection = new SqlConnection(connectionString))
				{
					await connection.OpenAsync();
					using (var command = new SqlCommand("SELECT [ArchiveImage] FROM [TIMSArchive] WHERE [id]=@id",
						       connection))
					{
						command.Parameters.AddWithValue("id", video.ArchiveId);

						// The reader needs to be executed with the SequentialAccess behavior to enable network streaming
						// Otherwise ReadAsync will buffer the entire BLOB into memory which can cause scalability issues or even OutOfMemoryExceptions
						using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
						{
							if (await reader.ReadAsync())
								if (!await reader.IsDBNullAsync(0))
									using (var file = new FileStream(filename, FileMode.Create, FileAccess.Write))
									{
										using (var data = reader.GetStream(0))
										{
											// Asynchronously copy the stream from the server to the file we just created
											await data.CopyToAsync(file);
										}
									}
						}
					}
				}
			}
		}
		else
		{
			// web access revoked. Delete file if exists
			if (File.Exists(filename)) File.Delete(filename);
		}
	}

	public async Task CreateDocumentTypeAsync(ImageDocumentType docType)
	{
		var hasExistingDocTypes = await _dbContext.DocumentTypes.Where(x => x.Name == docType.Name).AnyAsync();
		if (hasExistingDocTypes) throw new ValidationException($"Document type named '{docType.Name}' already exists");

		_dbContext.DocumentTypes.Add(docType);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<bool> DoesVideoExistAsync(int videoId)
	{
		var exists = await _dbContext.PatientLetterArchives.AnyAsync(x => x.Id == videoId && !x.DeletedDate.HasValue);
		return exists;
	}

	public async Task<ImageDocumentType> GetApiUploadDocumentTypeAsync()
	{
		var documentTypes = await GetDocumentTypesAsync(true);
		var docType = documentTypes.FirstOrDefault(x => x.Name == "pdf file");
		if (docType == null)
		{
			docType = new ImageDocumentType
			{
				Id = Guid.NewGuid(),
				Name = "pdf file",
				Description = "PDF Files Uploaded through API",
				DocumentQuality = 2,
				IsActive = true,
				CreatedDate = DateTime.Now,
				UpdatedDate = DateTime.Now,
				UpdatedUserId = 0
			};
			await CreateDocumentTypeAsync(docType);
		}
		else if (!docType.IsActive)
		{
			docType.IsActive = true;
			await UpdateDocumentTypeAsync(docType);
		}

		return docType;
	}

	public async Task<ImageDocumentType> GetDigitalFormDocumentTypeAsync(PatientFormTypeEnum formType)
	{
		var description = EnumUtilities.GetDescriptionFromEnum(formType);
		var documentTypes = await GetDocumentTypesAsync(true);
		var docType = documentTypes.FirstOrDefault(x => x.Name == description);
		if (docType == null)
		{
			docType = new ImageDocumentType
			{
				Name = description,
				DocumentQuality = 2,
				IsActive = true
			};
			await CreateDocumentTypeAsync(docType);
		}
		else if (!docType.IsActive)
		{
			docType.IsActive = true;
			await UpdateDocumentTypeAsync(docType);
		}

		return docType;
	}

	public async Task<List<ImageDocumentType>> GetDocumentTypesAsync(bool includeInactive = false)
	{
		var docTypes = await _dbContext.DocumentTypes.AsNoTracking().Where(x => includeInactive || x.IsActive)
			.ToListAsync();
		docTypes.Add(new ImageDocumentType
		{
			Id = PhotoDocumentType,
			Name = "Patient Photo",
			IsActive = true
		});
		return docTypes.OrderBy(x => x.Name).ToList();
	}


	public async Task<PatientImage> GetImageAsync(Guid documentId, bool includeImageData)
	{
		var patientImage = await _dbContext.PatientImages.SingleOrDefaultAsync(p => p.Id == documentId);

		if (patientImage != null && includeImageData)
		{
			var timsImage = await GetImageAsync(patientImage.ImageServerId, patientImage.ImageId);
			if (timsImage != null) patientImage.Image = timsImage.Image;
		}

		return patientImage;
	}


	public async Task<TimsImage> GetImageAsync(Guid serverId, Guid imageId)
	{
		TimsImage image = null;
		try
		{
			var connInfo = await _GetImagingConnectionInfo(serverId);
			var dbContext = _GetImagingDatabaseConnection(connInfo);
			image = await dbContext.TimsImages.SingleOrDefaultAsync(i => i.Id == imageId);
			if (image != null)
			{
				_Decrypt(image);
				_Decompress(image);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return image;
	}

	public async Task<ImageServer> GetImageServerAsync(Guid serverId)
	{
		return await _dbContext.ImageServers.SingleOrDefaultAsync(x => x.Id == serverId);
	}


	public async Task<PatientImage> GetInsuranceCardAsync(Guid patientGuid, bool front)
	{
		var insuranceCardDocType = await GetInsuranceCardDocumentTypeAsync();
		var description = front ? "front" : "back";
		var patientImage = await _dbContext.PatientImages
			.Where(p => p.PatientGuid == patientGuid &&
			            p.DocumentTypeId == insuranceCardDocType.Id &&
			            p.IsActive &&
			            p.Description != null && p.Description.ToLower() == description)
			.OrderByDescending(p => p.CreatedDate)
			.FirstOrDefaultAsync();

		if (patientImage != null)
		{
			var image = await GetImageAsync(patientImage.ImageServerId, patientImage.ImageId);
			if (image != null) patientImage.Image = image.Image;
		}

		return patientImage;
	}

	public async Task<PatientImage> GetInsuranceCardAsync(Guid patientGuid, InsuranceSlot slot, bool front)
	{
		var insuranceCardDocType = await GetInsuranceCardDocumentTypeAsync();
		var description = front ? $"front ({slot})" : $"back ({slot})";
		var patientImage = await _dbContext.PatientImages
			.Where(p => p.PatientGuid == patientGuid &&
			            p.DocumentTypeId == insuranceCardDocType.Id &&
			            p.IsActive &&
			            p.Description != null && p.Description.ToLower() == description)
			.OrderByDescending(p => p.CreatedDate)
			.FirstOrDefaultAsync();

		// Check old format if null
		if (slot == InsuranceSlot.Primary && patientImage == null)
		{
			description = front ? "front" : "back";
			patientImage = await _dbContext.PatientImages
				.Where(p => p.PatientGuid == patientGuid &&
				            p.DocumentTypeId == insuranceCardDocType.Id &&
				            p.IsActive &&
				            p.Description != null && p.Description.ToLower() == description)
				.OrderByDescending(p => p.CreatedDate)
				.FirstOrDefaultAsync();
		}

		if (patientImage != null)
		{
			var image = await GetImageAsync(patientImage.ImageServerId, patientImage.ImageId);
			if (image != null) patientImage.Image = image.Image;
		}

		return patientImage;
	}


	public async Task<DateTime?> GetInsuranceCardDateAsync(Guid patientGuid, bool front)
	{
		var insuranceCardDocType = await GetInsuranceCardDocumentTypeAsync();
		var description = front ? "front" : "back";
		var patientImage = await _dbContext.PatientImages
			.Where(p => p.PatientGuid == patientGuid &&
			            p.DocumentTypeId == insuranceCardDocType.Id &&
			            p.IsActive &&
			            p.Description != null && p.Description.ToLower() == description)
			.OrderByDescending(p => p.CreatedDate)
			.FirstOrDefaultAsync();

		if (patientImage == null) return null;
		return patientImage.CreatedDate;
	}

	public async Task<ImageDocumentType> GetInsuranceCardDocumentTypeAsync()
	{
		var documentTypes = await GetDocumentTypesAsync(true);
		var insuranceDocType = documentTypes.FirstOrDefault(x => x.Name == "Insurance Card");
		if (insuranceDocType == null)
		{
			insuranceDocType = new ImageDocumentType
			{
				Name = "Insurance Card",
				DocumentQuality = 2,
				IsActive = true
			};
			await CreateDocumentTypeAsync(insuranceDocType);
		}
		else if (!insuranceDocType.IsActive)
		{
			insuranceDocType.IsActive = true;
			await UpdateDocumentTypeAsync(insuranceDocType);
		}

		return insuranceDocType;
	}

	public async Task<PatientImage> GetPatientPhotoAsync(Guid patientGuid)
	{
		var patientImage = await _dbContext.PatientImages
			.Where(p => p.PatientGuid == patientGuid &&
			            p.DocumentTypeId == PhotoDocumentType &&
			            p.IsActive)
			.OrderByDescending(p => p.CreatedDate)
			.FirstOrDefaultAsync();

		if (patientImage != null)
		{
			var timsImage = await GetImageAsync(patientImage.ImageServerId, patientImage.ImageId);
			if (timsImage != null)
				using (var stream = new MemoryStream(timsImage.Image.ToArray()))
				{
					using (var image = Image.Load(stream))
					{
						if (image.Width > 320 || image.Height > 240)
						{
							var newSize = _CalculateThumbnailSize(image.Width, image.Height, 320, 240);
							image.Mutate(x => x.Resize(newSize.Item1, newSize.Item2));
							using (var resizedImage = new MemoryStream())
							{
								image.SaveAsJpeg(resizedImage);
								patientImage.Image = resizedImage.ToArray();
							}
						}
						else
						{
							patientImage.Image = timsImage.Image;
						}
					}
				}
		}

		return patientImage;
	}

	public async Task<ShareVideoModel> GetShareVideoAsync(int videoId)
	{
		var video = await _dbContext.PatientLetterArchives.FirstOrDefaultAsync(x => x.Id == videoId);
		var result = new ShareVideoModel { VideoId = videoId };
		if (video != null)
		{
			result.AccessEnabled = video.WebAccess;
			result.ExpirationDate = video.ExpirationDate;
		}

		return result;
	}

	public async Task<Tuple<string, string>> GetVideoFilePathAsync(string officeCode, int videoId, string password)
	{
		var connectionInfo = await _customerQuery.GetConnectionInfoAsync(officeCode);
		if (connectionInfo == null) return new Tuple<string, string>(null, "Invalid video id");

		var metadata = await _GetVideoMetadataAsync(connectionInfo, videoId);
		if (metadata != null)
		{
			// check web access enabled
			if (!metadata.WebAccess) return new Tuple<string, string>(null, "Web Access Not Enabled");

			// check expiration
			if (metadata.ExpirationDate.HasValue && metadata.ExpirationDate.Value <= DateTime.Now)
				return new Tuple<string, string>(null, "Access Expired");

			// check password
			if (!BCryptHelper.CheckPassword(password, metadata.Password))
				return new Tuple<string, string>(null, "Invalid Password");

			var filename = Path.Combine(VideoBasePath, $"{officeCode}-{videoId}.mp4".ToLower());

			if (File.Exists(filename)) return new Tuple<string, string>(filename, null);
		}

		return new Tuple<string, string>(null, "Video Not Found");
	}

	public async Task UpdateDocumentTypeAsync(ImageDocumentType docType)
	{
		var existingDocType = await _dbContext.DocumentTypes.SingleOrDefaultAsync(x => x.Id == docType.Id);
		if (existingDocType != null)
		{
			existingDocType.Name = docType.Name;
			existingDocType.Description = docType.Description;
			existingDocType.IsActive = docType.IsActive;
			await _dbContext.SaveChangesAsync();
		}
	}


	public async Task UpdatePatientPhotoAsync(Guid patientGuid, string description, byte[] imageBytes)
	{
		var activeImageServer = await _GetActiveImageServerAsync();
		if (activeImageServer == null) throw new Exception("No active image server to put patient photo");

		// deactivate all existing patient photos
		var patientImages = await _dbContext.PatientImages
			.Where(p => p.PatientGuid == patientGuid &&
			            p.DocumentTypeId == PhotoDocumentType &&
			            p.IsActive)
			.ToListAsync();

		foreach (var image in patientImages) image.IsActive = false;

		// create new patient photo
		var patientImage = new PatientImage
		{
			Description = description,
			DocumentTypeId = PhotoDocumentType,
			ImageServerId = activeImageServer.Id,
			PatientGuid = patientGuid,
			IsActive = true,
			ImageId = await _CreateTimsImageAsync(imageBytes, PhotoDocumentType, activeImageServer.Id)
		};

		_dbContext.PatientImages.Add(patientImage);
		_dbContext.SaveChanges();
	}

	public async Task UpdateShareVideoAsync(ShareVideoModel model)
	{
		var video = await _dbContext.PatientLetterArchives.FirstOrDefaultAsync(x => x.Id == model.VideoId);
		if (video != null)
		{
			video.WebAccess = model.AccessEnabled;
			video.ExpirationDate = model.ExpirationDate;
			video.Password = string.IsNullOrWhiteSpace(model.Password)
				? model.Password
				: BCryptHelper.HashPassword(model.Password, BCryptHelper.GenerateSalt());
			await _dbContext.SaveChangesAsync();

			var connInfo = await _GetImagingConnectionInfo(video.ArchiveServerId);

			await _PrepareVideoFileAsync(video, connInfo);
		}

		await _CleanupExpiredVideoFilesAsync();
	}

	public async Task UploadImagesAsync(Guid patientGuid, List<PatientImageModel> documents)
	{
		var activeImageServer = await _GetActiveImageServerAsync();
		if (activeImageServer == null) throw new Exception("No active image server to put patient photo");

		foreach (var document in documents)
			if (document.DocumentTypeId == PhotoDocumentType)
			{
				await UpdatePatientPhotoAsync(patientGuid, document.Description, document.Image);
			}
			else
			{
				// create new patient photo
				var patientImage = new PatientImage
				{
					Id = Guid.NewGuid(),
					Description = document.Description,
					DocumentTypeId = document.DocumentTypeId,
					ImageServerId = activeImageServer.Id,
					PatientGuid = patientGuid,
					IsActive = true,
					ImageId = await _CreateTimsImageAsync(document.Image, document.DocumentTypeId, activeImageServer.Id)
				};

				_dbContext.PatientImages.Add(patientImage);
			}

		_dbContext.SaveChanges();
	}

	public async Task UploadPatientDocumentAsync(Guid patientGuid, PatientImageModel document)
	{
		var activeImageServer = await _GetActiveImageServerAsync();
		if (activeImageServer == null) throw new Exception("No active image server to put patient photo");

		if (document.DocumentTypeId == PhotoDocumentType)
		{
			await UpdatePatientPhotoAsync(patientGuid, document.Description, document.Image);
		}
		else
		{
			// create new patient photo
			var patientImage = new PatientImage
			{
				Id = Guid.NewGuid(),
				Description = document.Description,
				DocumentTypeId = document.DocumentTypeId,
				ImageServerId = activeImageServer.Id,
				PatientGuid = patientGuid,
				IsActive = true,
				ImageId = await _CreateTimsImageAsync(document.Image, document.DocumentTypeId, activeImageServer.Id)
			};

			_dbContext.PatientImages.Add(patientImage);
		}

		_dbContext.SaveChanges();
	}
}