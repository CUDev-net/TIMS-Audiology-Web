using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Server.Data;

namespace TIMS_X.Server.Queries;

public class ProviderQuery
{
	private readonly ContextHelper _contextHelper;
	private readonly ProviderDbContext _dbContext;

	public ProviderQuery(ProviderDbContext dbContext, ContextHelper contextHelper)
	{
		_dbContext = dbContext;
		_contextHelper = contextHelper;
	}

	public async Task<MessageTemplate> GetMessageTemplateAsync(int providerId, MessageTemplateType templateType,
		LanguageEnum language)
	{
		MessageTemplate template = null;

		// Language of 0 is None. 1 is English.
		// English is the default language, so use it in case of None
		if (language == LanguageEnum.None)
			language = LanguageEnum.English;

		template = await _dbContext.MessageTemplates.FirstOrDefaultAsync(
			t => t.ProviderId == providerId && t.MessageType == templateType.ToString() && t.Language == language &&
			     !t.Inactive);

		// If template does not exist for this language, default to American English template if it exists.
		if (template == null)
		{
			language = LanguageEnum.English;
			template = await _dbContext.MessageTemplates.FirstOrDefaultAsync(
				t => t.ProviderId == providerId && t.MessageType == templateType.ToString() && t.Language == language &&
				     !t.Inactive);
		}

		if (template != null)
			template.TemplateType = Enum.Parse<MessageTemplateType>(template.MessageType);
		return template;
	}

	public async Task<MessageTemplate> GetMessageTemplateAsync(int id)
	{
		var template = await _dbContext.MessageTemplates.FirstOrDefaultAsync(
			t => t.Id == id);
		if (template != null)
			template.TemplateType = Enum.Parse<MessageTemplateType>(template.MessageType);
		return template;
	}

	public async Task<MessageTemplate> GetMessageTemplateFromCallLogAsync(string callSid)
	{
		var log = await _dbContext.VoiceCallLogs.FirstOrDefaultAsync(x => x.Identifier == callSid);
		if (log == null)
			return null;

		var template = await _dbContext.MessageTemplates.FirstOrDefaultAsync(
			t => t.Id == log.MessageTemplateId);
		template.TemplateType = Enum.Parse<MessageTemplateType>(template.MessageType);
		return template;
	}


	public async Task<MessageTemplate> GetMessageTemplateFromEmailLogAsync(int logId)
	{
		var templateId = await _dbContext.EmailLogs.Where(x => x.Id == logId).Select(x => x.MessageTemplateId)
			.FirstOrDefaultAsync();
		if (templateId == 0)
			return null;

		var template = await _dbContext.MessageTemplates.FirstOrDefaultAsync(
			t => t.Id == templateId);
		template.TemplateType = Enum.Parse<MessageTemplateType>(template.MessageType);
		return template;
	}

	public async Task<MessageTemplate> GetMessageTemplateFromSmsLogAsync(string phoneNumber)
	{
		var log = await _dbContext.SmsLogs.OrderBy(x => x.CreatedDate).LastOrDefaultAsync(x => x.To == phoneNumber);
		if (log == null)
			return null;

		var template = await _dbContext.MessageTemplates.FirstOrDefaultAsync(
			t => t.Id == log.MessageTemplateId);
		template.TemplateType = Enum.Parse<MessageTemplateType>(template.MessageType);
		return template;
	}

	public async Task<Provider> GetProviderAsync(int providerId)
	{
		var provider = await _dbContext.Providers
			.Include(x => x.User)
			.ThenInclude(x => x.SiteHours)
			.Include(x => x.BlockSchedules)
			.ThenInclude(x => x.ScheduleBlock)
			//.ThenInclude(x => x.AppointmentType)
			.Include(x => x.BlockSchedules)
			.ThenInclude(x => x.ScheduleTimeSlot)
			.Where(x => x.Id == providerId).FirstOrDefaultAsync();

		return provider;
	}

	public async Task<ProviderItem> GetProviderFromUserIdAsync(int userId)
	{
		var query = _dbContext.Providers
			.Where(prov => prov.UserId == userId)
			.Select(prov => new ProviderItem
				{ Id = prov.Id, Inactive = prov.Inactive, FirstName = prov.FirstName, LastName = prov.LastName });
		var result = await query.FirstOrDefaultAsync();

		return result;
	}

	public async Task<IEnumerable<ProviderItem>> GetProvidersAsync(bool includeInactive)
	{
		var query = _dbContext.Providers
			.Where(prov => (includeInactive || !prov.Inactive) && !prov.Deleted)
			.Select(prov => new ProviderItem
			{
				Id = prov.Id, UserId = prov.UserId, Inactive = prov.Inactive, FirstName = prov.FirstName,
				LastName = prov.LastName
			});
		List<ProviderItem> results = null;
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

	public async Task<IEnumerable<ProviderItem>> GetProvidersAsync(int siteId, bool includeInactive)
	{
		var query = _dbContext.Providers
			.Include(x => x.User)
			.ThenInclude(x => x.SiteHours)
			.Include(x => x.BlockSchedules)
			.ThenInclude(x => x.ScheduleBlock)
			//.ThenInclude(x => x.AppointmentType)
			.Include(x => x.BlockSchedules)
			.ThenInclude(x => x.ScheduleTimeSlot)
			.Where(prov => (includeInactive || !prov.Inactive) && !prov.Deleted)
			.Select(prov => new ProviderItem(prov));
		List<ProviderItem> results = null;
		try
		{
			results = await query.ToListAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}

		results = results.Where(x => x.SiteSchedules.ContainsKey(siteId)).ToList();


		return results;
	}

	public async Task<int> GetUserIdFromProviderAsync(int providerId)
	{
		var query = _dbContext.Providers
			.Where(prov => prov.Id == providerId)
			.Select(prov => prov.UserId);
		var result = await query.FirstOrDefaultAsync();

		return result;
	}

	public async Task PutMessageTemplateAsync(MessageTemplate messageTemplate)
	{
		try
		{
			if (messageTemplate.Id == 0)
			{
				await _dbContext.MessageTemplates.AddAsync(messageTemplate);
			}
			else
			{
				messageTemplate.UpdatedDate = DateTime.Now;
				messageTemplate.UpdatedUserId = _contextHelper.CurrentUser.Id;
				_dbContext.MessageTemplates.Attach(messageTemplate).State = EntityState.Modified;
			}

			await _dbContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task<bool> ValidateProviderAsync(int providerId)
	{
		return await _dbContext.Providers.AnyAsync(x => x.Id == providerId && !x.Inactive);
	}
}