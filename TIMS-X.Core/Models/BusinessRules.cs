using TIMS_X.Core.Attributes;

namespace TIMS_X.Core.Models
{
	public class BusinessRules
	{
		public static readonly string XML_TAG_THRESHOLD_VALUES = "ThresholdValues";
		public static readonly string XML_TAG_OPPORTUNITY_TRACKING = "OpportunityTracking";
		public static readonly string XML_ONLINEAPPOINTMENTPATIENTID = "OnlineAppointmentPatientId";
		public static readonly string XML_HASHKEYONLINEAPPOINTMENT = "HashKeyOnlineAppointment";
		public static readonly string XML_USEINSURANCEAUTHORIZATIONS = "UseApptAuthorizations";
		public static readonly string XML_ATTRIBUTE_NAME = "Name";
		public static readonly string XML_ATTRIBUTE_VALUE = "Value";
		public static readonly string XML_TAG_BR = "BR";
		public static readonly string XML_TAG_BUSINESS_RULE = "BusinessRule";
		public static readonly string XML_VALUE_TRUE = "true";
		public static readonly string XML_VALUE_FALSE = "false";

		public BusinessRules()
		{
			OpportunityTracking = new OpportunityTrackingRules();
			RequireAppointmentType = true;
			SendsNotificationsFromUSNumber = true;
		}

		[XmlTag("DupPatNM")]
        public bool AllowDuplicatePatientNames { get; set; }

		[XmlTag(nameof(ForceMarketingSourcePatientAppointment))]
		public bool ForceMarketingSourcePatientAppointment { get; set; }

		[XmlTag("UseApptAuthorizations")]
        public bool UseApptAuthorizations { get; set; }
		
		[XmlTag("HashKeyOnlineAppointment")]
		public string HashKeyOnlineAppointment { get; set; }
		
		[XmlTag("OnlineAppointmentPatientId")]
        public long OnlineAppointmentPatientId { get; set; }

		[XmlTag("OnlineApptMsg")]
		public string OnlinePatientAppointmentMessage { get; set; }

		public OpportunityTrackingRules OpportunityTracking { get; set; }

		[XmlTag("RqApTp")] 
        public bool RequireAppointmentType { get; set; }

		[XmlTag("SendsNotificationsFromUSNumber")]
		public bool SendsNotificationsFromUSNumber { get; set; }

		[XmlTag("UsesCalendarResources")] 
        public bool UsesCalendarResources { get; set; }

		[XmlTag("AmazonMessaging")] 
        public bool UseAmazonMessaging { get; set; }

		[XmlTag("UsesSlp")]
        public bool UsesSlp { get; set; }
	}
}