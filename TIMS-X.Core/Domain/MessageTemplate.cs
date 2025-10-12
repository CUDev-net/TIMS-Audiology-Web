using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Domain
{
    public class MessageTemplate : IUpdateAudited
    {
        private string _voice;
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string MessageType { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBodyText { get; set; }
        public string EmailBodyHtml { get; set; }
        public string SmsBody { get; set; }
        
        public bool IsEmailEnabled { get; set; }
        public bool IsSmsEnabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public string VoiceCallScript { get; set; }
        public bool IsVoiceEnabled { get; set; }
        public LanguageEnum Language { get; set; }
        public bool Inactive { get; set; }

        public string Voice
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_voice))
                {
                    if (!string.IsNullOrWhiteSpace(VoiceCallScript) && VoiceCallScript.Length >= 3)
                    {
                        var encodedVoice = VoiceCallScript.Substring(0, 3);
                        if (encodedVoice[0] == '<' && encodedVoice[2] == '>')
                        {
                            int voiceInt = int.Parse(encodedVoice[1].ToString());
                            TwilioVoiceEnum voiceEnum = (TwilioVoiceEnum) voiceInt;
                            _voice = voiceEnum.ToString().ToLower();
                        }
                    }
                }
                return _voice;
            }
            set => _voice = value;
        }

        public bool AllowSmsConfirm { get; set; }
        public bool AllowSmsCancel { get; set; }
        public bool AllowSmsReschedule { get; set; }
        public bool AllowVoiceConfirm { get; set; }
        public bool AllowVoiceCancel { get; set; }
        public bool AllowVoiceReschedule { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string VoicePromptText { get; set; }
        public string SmsPromptText { get; set; }
        public string VoiceConfirmText { get; set; }
        public string VoiceRescheduleText { get; set; }
        public string VoiceCancelText { get; set; }
        public string VoiceMisunderstoodText { get; set; }
        public string EmailMisunderstoodText { get; set; }
        public string SmsMisunderstoodText { get; set; }
        public string EmailConfirmText { get; set; }
        public string EmailRescheduleText { get; set; }
        public string EmailCancelText { get; set; }
        public string SmsConfirmText { get; set; }
        public string SmsRescheduleText { get; set; }
        public string SmsCancelText { get; set; }
        public string ConfirmationLinkText { get; set; }
        public string CallToRescheduleLinkText { get; set; }
        public string CancelLinkText { get; set; }

        public MessageTemplateType TemplateType { get; set; }

        public string GetSmsBody(MessageTemplateType templateType)
        {
            return templateType == MessageTemplateType.AppointmentConfirmation
                ? SmsBody + Environment.NewLine + SmsPromptText
                : SmsBody;
        }
    }
}
