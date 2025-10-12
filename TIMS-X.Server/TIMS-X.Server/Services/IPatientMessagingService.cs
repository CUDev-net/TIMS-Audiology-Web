using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Server.Models;
using Task = System.Threading.Tasks.Task;

namespace TIMS_X.Server.Services;

public interface IPatientMessagingService
{
	Task<List<Conversation>> GetAllConversationsAsync();
	Task<int> GetAllConversationsCountAsync();
	Task<List<Conversation>> GetAllConversationsPagedAsync(int page, int pageSize);

	Task<CallScriptTokens> GetCallScriptTokensAsync(
		string officeCode,
		string key,
		string callSid);

	Task<List<Message>> GetConversationHistoryAsync(int userId, int patientId, DateTime? cutoffDate);
	Task<List<Message>> GetConversationHistoryAsync(int userId, string phoneNumber, DateTime? cutoffDate);
	Task<List<Tuple<int, int>>> GetConversationHistoryMessageCountAllAsync(int patientId);
	Task<int> GetConversationHistoryMessageCountAsync(int userId, int patientId);
	Task<MessageTemplate> GetMessageTemplateAsync(int id);

	Task<MessageTemplate> GetMessageTemplateFromCallSidAsync(string callSid);
	Task<MessageTemplate> GetMessageTemplateFromEmailLogAsync(int logId);
	Task<MessageTemplate> GetMessageTemplateFromSmsLogAsync(string phoneNumber);

	Task HandleAwsSmsResponseAsync(AwsSmsNotification smsNotification);

	Task<Tuple<MessageTemplate, int, bool, int>> HandlePatientEmailNotificationResponseAsync(
		PatientNotificationResponse response, int logId);

	Task<Tuple<PatientNotificationResponse, MessageTemplate, int, int>> HandlePatientSmsNotificationResponseAsync(
		AwsSmsNotification smsResponse);

	Task<Tuple<PatientNotificationResponse, MessageTemplate, int, int>> HandlePatientSmsNotificationResponseAsync(
		TwilioSmsResponse smsResponse);

	Task<Tuple<PatientNotificationResponse, MessageTemplate>> HandlePatientVoiceNotificationResponseAsync(
		TwilioCallResponse callResponse);

	Task<bool> IsPreviewFinishedAsync(int appointmentId);

	Task<Dictionary<int, string>> PreviewPatientNotificationAsync(MessageDeliveryMethod deliveryMethod, int templateId,
		int siteId, string contact);

	Task<Dictionary<int, string>> SendConfirmationMessageAsync(int appointmentId, MessageDeliveryMethod deliveryMethod);

	Task SendPatientMessagesAsync(PatientMessageModel model);
	Task SendSmsReplyAsync(string to, string message, int patientId, int userId, int templateId, int appointmentId);
	Task<Dictionary<int, string>> SendVerificationMessageAsync(int appointmentId, MessageDeliveryMethod deliveryMethod);
	Task UpdateCallStatusAsync(string callSid, string callStatus, string digitsPressed);
	Task UpdateSmsStatusAsync(TwilioSmsStatus status);
}