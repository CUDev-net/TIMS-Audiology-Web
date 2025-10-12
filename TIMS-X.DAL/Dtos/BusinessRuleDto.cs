using TIMS_X.Core.Models;

namespace TIMS_X.DAL.Dtos;

public class BusinessRuleDto
{
	public BusinessRuleDto(BusinessRules businessRules)
	{
		RequireMarketingSourceForPatientAppointment = businessRules.ForceMarketingSourcePatientAppointment;
		RequireAppointmentTypeForAppointment = businessRules.RequireAppointmentType;
		UsesCalendarResources = businessRules.UsesCalendarResources;
		UsesSlp = businessRules.UsesSlp;
		OnlinePatientAppointmentMessage = businessRules.OnlinePatientAppointmentMessage;
		OnlineAppointmentPatientId = (int)businessRules.OnlineAppointmentPatientId;
		UseApptAuthorizations = businessRules.UseApptAuthorizations;
	}

	public int OnlineAppointmentPatientId { get; set; }

	public string OnlinePatientAppointmentMessage { get; set; }
	public bool RequireAppointmentTypeForAppointment { get; set; }
	public bool RequireMarketingSourceForPatientAppointment { get; set; }
	public bool UseApptAuthorizations { get; set; }
	public bool UsesCalendarResources { get; set; }
	public bool UsesSlp { get; set; }
}