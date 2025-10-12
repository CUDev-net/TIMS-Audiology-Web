using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core;
using TIMS_X.Core.Attributes;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Server.Data;
using TIMS_X.Server.Extensions;
using TIMS_X.Server.Models;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Queries;

public class PracticeQuery
{
	private readonly ContextHelper _contextHelper;
	private readonly PracticeDbContext _dbContext;

	public PracticeQuery(PracticeDbContext dbContext, ContextHelper contextHelper)
	{
		_dbContext = dbContext;
		_contextHelper = contextHelper;
	}

	public async Task<bool> AlertExistsAsync(int targetUserId, AlertTypeEnum alertType, int objectId,
		string phoneNumber = null)
	{
		var exists = await _dbContext.Alerts.AnyAsync(x =>
			(x.AlertUserId == targetUserId &&
			 x.AlertType == alertType &&
			 x.AlertObjectId == objectId &&
			 phoneNumber == null) || x.Description.Contains(phoneNumber));
		return exists;
	}

	public async Task<bool> DigitalFormExistsAsync(PatientFormTypeEnum formType)
	{
		if (formType == PatientFormTypeEnum.Intake)
			return await _dbContext.PatientReportTemplates.AnyAsync(x =>
				x.ReportCategory == PatientReportCategoryEnum.DigitalIntakeSheet);
		return false;
	}

	public async Task<BusinessRules> GetBusinessRulesAsync()
	{
		var practice = await GetPracticeAsync();
		var xDocument = XDocument.Parse(practice.BusinessRules);
		if (xDocument.Root == null) return null;

		var xRules = xDocument.Root.Elements()
			.Where(p => p.Name == BusinessRules.XML_TAG_BR || p.Name == BusinessRules.XML_TAG_BUSINESS_RULE)
			.ToList();

		var businessRules = new BusinessRules();
		var classProperties = typeof(BusinessRules).GetProperties();

		var propertyMap = new Dictionary<string, PropertyInfo>();

		foreach (var property in classProperties)
		{
			string xmlTag = null;
			var xmlTagAttribute = (XmlTagAttribute)Attribute.GetCustomAttribute(property, typeof(XmlTagAttribute));
			if (xmlTagAttribute != null) xmlTag = xmlTagAttribute.Name;

			if (!string.IsNullOrWhiteSpace(xmlTag)) propertyMap[xmlTag] = property;
		}


		foreach (var xElement in xRules)
		{
			var xName = xElement.Attributes().FirstOrDefault(x => x.Name == BusinessRules.XML_ATTRIBUTE_NAME);
			if (xName != null)
			{
				var name = xName.Value;
				if (name == BusinessRules.XML_TAG_OPPORTUNITY_TRACKING)
				{
					var xThresholdMonths = xElement.Descendants().First(x =>
						x.Name == nameof(businessRules.OpportunityTracking.ThresholdMonths));

					var xValue = xThresholdMonths.Attributes().First(x => x.Name == BusinessRules.XML_ATTRIBUTE_VALUE);

					if (int.TryParse(xValue.Value, out var value))
						businessRules.OpportunityTracking.ThresholdMonths = value;

					var otProperties = typeof(OpportunityTrackingRules).GetProperties();
					var xThresholdValues = xElement.Descendants()
						.FirstOrDefault(x => x.Name == BusinessRules.XML_TAG_THRESHOLD_VALUES);
					if (xThresholdValues != null)
						foreach (var xAttribute in xThresholdValues.Attributes())
							if (decimal.TryParse(xAttribute.Value, out var threshold))
							{
								var property = otProperties.FirstOrDefault(x => x.Name == xAttribute.Name);
								if (property != null) property.SetValue(businessRules.OpportunityTracking, threshold);
							}
				}
				else if (propertyMap.ContainsKey(name))
				{
					var property = propertyMap[name];

					var strValue = xElement.Attributes()
						.FirstOrDefault(x => x.Name == BusinessRules.XML_ATTRIBUTE_VALUE)?.Value;

					if (!string.IsNullOrWhiteSpace(strValue))
					{
						if (property.PropertyType == typeof(bool))
						{
							var value = string.Equals(strValue, BusinessRules.XML_VALUE_TRUE,
								StringComparison.CurrentCultureIgnoreCase);

							property.SetValue(businessRules, value);
						}
						else if (property.PropertyType == typeof(int))
						{
							if (int.TryParse(strValue, out var value)) property.SetValue(businessRules, value);
						}
					}
				}
			}
		}

		return businessRules;
	}

	public async Task<DigitalForm> GetDigitalFormAsync(PatientFormTypeEnum formType)
	{
		PatientReportTemplate reportTemplate = null;
		switch (formType)
		{
			case PatientFormTypeEnum.None:
				break;
			case PatientFormTypeEnum.Intake:
				reportTemplate = await _dbContext.PatientReportTemplates.FirstOrDefaultAsync(x =>
					x.ReportCategory == PatientReportCategoryEnum.DigitalIntakeSheet);
				break;
		}

		if (reportTemplate == null) return null;

		var decompressedForm = CompressionHelper.DecompressAsync(reportTemplate.ReportTemplate);
		var jsonText = Encoding.UTF8.GetString(decompressedForm);
		var digitalForm = JsonSerializer.Deserialize<DigitalForm>(jsonText);

		digitalForm.UpdatedDate = reportTemplate.UpdatedDate;
		digitalForm.UpdatedUser = await _dbContext.Users.Where(x => x.Id == reportTemplate.UpdatedUserId)
			.Select(x => x.Name).FirstOrDefaultAsync();

		return digitalForm;
	}

	public async Task<List<EmplStatus>> GetEmploymentStatusesAsync()
	{
		var statuses = await _dbContext.EmploymentStatuses.ToListAsync();
		return statuses;
	}

	public async Task<Site> GetFirstSiteAsync(bool includeLogo = false)
	{
		Site site = null;
		if (includeLogo)
		{
			site = await _dbContext.Sites.FirstOrDefaultAsync(x => !x.Inactive);
		}
		else
		{
			var data = await _dbContext.Sites.Where(x => !x.Inactive).Select(x => new
			{
				x.Address1,
				x.Address2,
				x.AllWellId,
				x.CareCreditMerchantNumber,
				x.CareCreditPassword,
				x.CareCreditPracticeCode,
				x.CcpromoCode,
				x.City,
				x.Color,
				x.CustomText1,
				x.CustomText2,
				x.CustomText3,
				x.DefaultTaxGroupId,
				x.Description,
				x.EcareCreditPaymentId,
				x.EcheckPaymentId,
				x.EcreditCardPaymentId,
				x.FaxNumber,
				x.FriEnd,
				x.FriStart,
				x.Id,
				x.Inactive,
				x.MonEnd,
				x.MonStart,
				x.Name,
				x.Npi,
				x.OutreachEducator,
				x.Phone,
				x.PracticeId,
				x.Qbid,
				x.QbModifiedDate,
				x.RegionId,
				x.Resources,
				x.SatEnd,
				x.SatStart,
				x.SecondaryIdnum,
				x.SecondaryIdqualifier,
				x.SiteSettingId,
				x.State,
				x.SunEnd,
				x.SunStart,
				x.ThurEnd,
				x.ThurStart,
				x.TransnationalAuthKey,
				x.TransnationalPassword,
				x.TransnationalUsername,
				x.TuesEnd,
				x.TuesStart,
				x.UpdatedDate,
				x.UpdatedUserId,
				x.WedEnd,
				x.WedStart,
				x.Zip
			}).FirstOrDefaultAsync();

			if (data != null)
				site = new Site
				{
					Address1 = data.Address1,
					Address2 = data.Address2,
					AllWellId = data.AllWellId,
					CareCreditMerchantNumber = data.CareCreditMerchantNumber,
					CareCreditPassword = data.CareCreditPassword,
					CareCreditPracticeCode = data.CareCreditPracticeCode,
					CcpromoCode = data.CcpromoCode,
					City = data.City,
					Color = data.Color,
					CustomText1 = data.CustomText1,
					CustomText2 = data.CustomText2,
					CustomText3 = data.CustomText3,
					DefaultTaxGroupId = data.DefaultTaxGroupId,
					Description = data.Description,
					EcareCreditPaymentId = data.EcareCreditPaymentId,
					EcheckPaymentId = data.EcheckPaymentId,
					EcreditCardPaymentId = data.EcreditCardPaymentId,
					FaxNumber = data.FaxNumber,
					FriEnd = data.FriEnd,
					FriStart = data.FriStart,
					Id = data.Id,
					Inactive = data.Inactive,
					MonEnd = data.MonEnd,
					MonStart = data.MonStart,
					Name = data.Name,
					Npi = data.Npi,
					OutreachEducator = data.OutreachEducator,
					Phone = data.Phone,
					PracticeId = data.PracticeId,
					Qbid = data.Qbid,
					QbModifiedDate = data.QbModifiedDate,
					RegionId = data.RegionId,
					Resources = data.Resources,
					SatEnd = data.SatEnd,
					SatStart = data.SatStart,
					SecondaryIdnum = data.SecondaryIdnum,
					SecondaryIdqualifier = data.SecondaryIdqualifier,
					SiteSettingId = data.SiteSettingId,
					State = data.State,
					SunEnd = data.SunEnd,
					SunStart = data.SunStart,
					ThurEnd = data.ThurEnd,
					ThurStart = data.ThurStart,
					TransnationalAuthKey = data.TransnationalAuthKey,
					TransnationalPassword = data.TransnationalPassword,
					TransnationalUsername = data.TransnationalUsername,
					TuesEnd = data.TuesEnd,
					TuesStart = data.TuesStart,
					UpdatedDate = data.UpdatedDate,
					UpdatedUserId = data.UpdatedUserId,
					WedEnd = data.WedEnd,
					WedStart = data.WedStart,
					Zip = data.Zip
				};
		}

		return site;
	}

	public async Task<byte[]> GetFirstSiteLogoAsync()
	{
		var logo = await _dbContext.Sites.Where(x => !x.Inactive).Select(x => x.Logo).FirstOrDefaultAsync();
		return logo;
	}

	public async Task<List<MaritalStatus>> GetMaritalStatusesAsync()
	{
		var statuses = await _dbContext.MaritalStatuses.ToListAsync();
		return statuses;
	}

	public MessageSettings GetMessageSettings()
	{
		var messageSettings = _dbContext.MessageSettingsTable.FirstOrDefault();
		return messageSettings;
	}

	public async Task<MessageSettings> GetMessageSettingsAsync()
	{
		var messageSettings = await _dbContext.MessageSettingsTable.FirstOrDefaultAsync();
		return messageSettings;
	}

	public async Task<Practice> GetPracticeAsync()
	{
		Practice result = null;
		try
		{
			result = await _dbContext.Practices.FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return result;
	}

	public SiteItem GetRandomSite()
	{
		return _dbContext.Sites.Select(x => new SiteItem(x)).FirstOrDefault();
	}

	public async Task<List<Sex>> GetSexesAsync()
	{
		var sexes = await _dbContext.Sexes.ToListAsync();
		return sexes;
	}

	public async Task<Site> GetSiteAsync(int siteId, bool includeLogo = false)
	{
		Site site = null;
		if (includeLogo)
		{
			site = await _dbContext.Sites.FirstOrDefaultAsync(x => x.Id == siteId);
		}
		else
		{
			var data = await _dbContext.Sites.Where(x => x.Id == siteId).Select(x => new
			{
				x.Address1,
				x.Address2,
				x.AllWellId,
				x.CareCreditMerchantNumber,
				x.CareCreditPassword,
				x.CareCreditPracticeCode,
				x.CcpromoCode,
				x.City,
				x.Color,
				x.CustomText1,
				x.CustomText2,
				x.CustomText3,
				x.DefaultTaxGroupId,
				x.Description,
				x.EcareCreditPaymentId,
				x.EcheckPaymentId,
				x.EcreditCardPaymentId,
				x.FaxNumber,
				x.FriEnd,
				x.FriStart,
				x.Id,
				x.Inactive,
				x.MonEnd,
				x.MonStart,
				x.Name,
				x.Npi,
				x.OutreachEducator,
				x.Phone,
				x.PracticeId,
				x.Qbid,
				x.QbModifiedDate,
				x.RegionId,
				x.Resources,
				x.SatEnd,
				x.SatStart,
				x.SecondaryIdnum,
				x.SecondaryIdqualifier,
				x.SiteSettingId,
				x.State,
				x.SunEnd,
				x.SunStart,
				x.ThurEnd,
				x.ThurStart,
				x.TransnationalAuthKey,
				x.TransnationalPassword,
				x.TransnationalUsername,
				x.TuesEnd,
				x.TuesStart,
				x.UpdatedDate,
				x.UpdatedUserId,
				x.WedEnd,
				x.WedStart,
				x.Zip
			}).FirstOrDefaultAsync();

			if (data != null)
				site = new Site
				{
					Address1 = data.Address1,
					Address2 = data.Address2,
					AllWellId = data.AllWellId,
					CareCreditMerchantNumber = data.CareCreditMerchantNumber,
					CareCreditPassword = data.CareCreditPassword,
					CareCreditPracticeCode = data.CareCreditPracticeCode,
					CcpromoCode = data.CcpromoCode,
					City = data.City,
					Color = data.Color,
					CustomText1 = data.CustomText1,
					CustomText2 = data.CustomText2,
					CustomText3 = data.CustomText3,
					DefaultTaxGroupId = data.DefaultTaxGroupId,
					Description = data.Description,
					EcareCreditPaymentId = data.EcareCreditPaymentId,
					EcheckPaymentId = data.EcheckPaymentId,
					EcreditCardPaymentId = data.EcreditCardPaymentId,
					FaxNumber = data.FaxNumber,
					FriEnd = data.FriEnd,
					FriStart = data.FriStart,
					Id = data.Id,
					Inactive = data.Inactive,
					MonEnd = data.MonEnd,
					MonStart = data.MonStart,
					Name = data.Name,
					Npi = data.Npi,
					OutreachEducator = data.OutreachEducator,
					Phone = data.Phone,
					PracticeId = data.PracticeId,
					Qbid = data.Qbid,
					QbModifiedDate = data.QbModifiedDate,
					RegionId = data.RegionId,
					Resources = data.Resources,
					SatEnd = data.SatEnd,
					SatStart = data.SatStart,
					SecondaryIdnum = data.SecondaryIdnum,
					SecondaryIdqualifier = data.SecondaryIdqualifier,
					SiteSettingId = data.SiteSettingId,
					State = data.State,
					SunEnd = data.SunEnd,
					SunStart = data.SunStart,
					ThurEnd = data.ThurEnd,
					ThurStart = data.ThurStart,
					TransnationalAuthKey = data.TransnationalAuthKey,
					TransnationalPassword = data.TransnationalPassword,
					TransnationalUsername = data.TransnationalUsername,
					TuesEnd = data.TuesEnd,
					TuesStart = data.TuesStart,
					UpdatedDate = data.UpdatedDate,
					UpdatedUserId = data.UpdatedUserId,
					WedEnd = data.WedEnd,
					WedStart = data.WedStart,
					Zip = data.Zip
				};
		}

		return site;
	}

	public async Task<Tuple<DateTime?, DateTime?>> GetSiteHoursAsync(int siteId, DayOfWeek day)
	{
		Site site = null;
		try
		{
			site = await _dbContext.Sites.FirstOrDefaultAsync(x => x.Id == siteId);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		Tuple<DateTime?, DateTime?> result = null;

		if (site != null)
			switch (day)
			{
				case DayOfWeek.Sunday:
					result = new Tuple<DateTime?, DateTime?>(site.SunStart, site.SunEnd);
					break;
				case DayOfWeek.Monday:
					result = new Tuple<DateTime?, DateTime?>(site.MonStart, site.MonEnd);
					break;
				case DayOfWeek.Tuesday:
					result = new Tuple<DateTime?, DateTime?>(site.TuesStart, site.TuesEnd);
					break;
				case DayOfWeek.Wednesday:
					result = new Tuple<DateTime?, DateTime?>(site.WedStart, site.WedEnd);
					break;
				case DayOfWeek.Thursday:
					result = new Tuple<DateTime?, DateTime?>(site.ThurStart, site.ThurEnd);
					break;
				case DayOfWeek.Friday:
					result = new Tuple<DateTime?, DateTime?>(site.FriStart, site.FriEnd);
					break;
				case DayOfWeek.Saturday:
					result = new Tuple<DateTime?, DateTime?>(site.SatStart, site.SatEnd);
					break;
			}

		return result;
	}

	public async Task<byte[]> GetSiteLogoAsync(int siteId)
	{
		var logo = await _dbContext.Sites.Where(x => x.Id == siteId).Select(x => x.Logo).FirstOrDefaultAsync();
		return logo;
	}

	public async Task<List<SiteItem>> GetSitesAsync(bool includeInactive)
	{
		var query = _dbContext.Sites
			.Include(site => site.Resources)
			.Where(site => includeInactive || !site.Inactive)
			.Select(site => new SiteItem(site));
		List<SiteItem> results = null;
		try
		{
			results = await query.ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		return results;
	}

	public async Task<List<Area>> GetStateListAsync()
	{
		var qbLocale = await GetValueAsync(practice => practice.QbLocale);
		var localeId = 0; // Default US

		if (!string.IsNullOrEmpty(qbLocale))
		{
			qbLocale = qbLocale.ToLower();
			if (qbLocale == "ca")
				localeId = 1;
			else if (qbLocale == "au")
				localeId = 2;
			else if (qbLocale == "nz")
				localeId = 3;
			else if (qbLocale == "uk")
				localeId = 4;
		}


		List<Area> areas = null;

		// For US and Canada, return both sets of states

		if (localeId == 0 || localeId == 1)
			areas = await _dbContext.Areas.Where(x => x.CountryId == 0 || x.CountryId == 1)
				.Select(x => x)
				.OrderBy(x => x.Name)
				.ToListAsync();
		else
			areas = await _dbContext.Areas.Where(x => x.CountryId == localeId)
				.Select(x => x)
				.OrderBy(x => x.Name)
				.ToListAsync();

		return areas;
	}

	public async Task<T> GetValueAsync<T>(Expression<Func<Practice, T>> selector)
	{
		return await _dbContext.Practices.Select(selector).FirstOrDefaultAsync();
	}


	public async Task PutAlertAsync(Alert alert)
	{
		await _dbContext.Alerts.AddAsync(alert);
		await _dbContext.SaveChangesAsync();
	}


	public async Task PutDigitalFormAsync(DigitalForm form)
	{
		var validCategory = false;
		var reportCategory = PatientReportCategoryEnum.DigitalIntakeSheet;
		switch (form.FormType)
		{
			case PatientFormTypeEnum.None:
				break;
			case PatientFormTypeEnum.Intake:
				validCategory = true;
				reportCategory = PatientReportCategoryEnum.DigitalIntakeSheet;
				break;
		}

		if (validCategory)
		{
			var reportTemplate =
				await _dbContext.PatientReportTemplates.FirstOrDefaultAsync(x => x.ReportCategory == reportCategory);
			if (reportTemplate == null)
			{
				reportTemplate = new PatientReportTemplate
				{
					Name = "Digital Intake Sheet",
					ReportCategory = reportCategory,
					CreatedDate = DateTime.Now
				};
				await _dbContext.PatientReportTemplates.AddAsync(reportTemplate);
			}

			reportTemplate.UpdatedUserId = _contextHelper.CurrentUser.Id;
			reportTemplate.UpdatedDate = DateTime.Now;

			var jsonForm = JsonSerializer.Serialize(form);
			var formBytes = Encoding.UTF8.GetBytes(jsonForm);
			var compressedForm = CompressionHelper.CompressAsync(formBytes);
			reportTemplate.ReportTemplate = compressedForm;


			try
			{
				await _dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}
	}

	public async Task<bool> TestConnectionAsync()
	{
		var result = await _dbContext.Database.ExistsAsync();
		return result;
	}

	public async Task<bool> UsesAdAuthenticationAsync()
	{
		return await _dbContext.Practices.Select(x => x.UsesAdAuthentication).FirstOrDefaultAsync();
	}

	public async Task<bool> ValidateSiteAsync(int siteId)
	{
		return await _dbContext.Sites.AnyAsync(x => x.Id == siteId && !x.Inactive);
	}
}