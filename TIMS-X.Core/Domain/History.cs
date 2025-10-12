using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class History : Entity, IUpdateAudited
    {
        public int UpdatedSiteId { get; set; }
        public int PatientId { get; set; }
        public DateTime HistoryDate { get; set; }
        public HistoryType HistoryType { get; set; }
        public Patient Patient { get; set; }
        public Provider Provider { get; set; }
        public Site Site { get; set; }
        public Appointment Appointment { get; set; }
        public MarketingReference MarketingReference { get; set; }
        public int SyncSiteId { get; set; }
        public int ProviderId { get; set; }
        public int ReferralSourceId { get; set; }
        public int SeverityLeft { get; set; }
        public int SeverityRight { get; set; }
        public int SeverityBilateral { get; set; }
        public int TypeofLossLeft { get; set; }
        public int TypeofLossRight { get; set; }
        public int TypeofLossBilateral { get; set; }
        public int Results1Right { get; set; }
        public int Results1Left { get; set; }
        public int Results2Right { get; set; }
        public int Results2Left { get; set; }
        public int Results3Right { get; set; }
        public int Results3Left { get; set; }
        public int Results4Right { get; set; }
        public int Results4Left { get; set; }
        public int Results5Right { get; set; }
        public int Results5Left { get; set; }
        public int Results6Right { get; set; }
        public int Results6Left { get; set; }
        public string Diagnosis { get; set; }
        public string Recommendation { get; set; }
        public string Notes { get; set; }
        public int HistoryTypeId { get; set; }
        public int AppointmentId { get; set; }
        public DateTime? AvailableDate { get; set; }
        public byte[] RowVersion { get; set; }
        public Guid HistoryGuid { get; set; }
        public int ActionId { get; set; }
        public DateTime? LockDate { get; set; }
        public int? LockedByUser { get; set; }
        public int? ParentId { get; set; }
        public int? PatientInteractionId { get; set; }
        public DateTime? ExportDate { get; set; }
        public string OfficeNotes { get; set; }
        public string CustomText1 { get; set; }
        public int ConfigurationRight { get; set; }
        public int ConfigurationLeft { get; set; }
        public string SlpFluencyVoiceNotes { get; set; }
        public int SlpFluency2 { get; set; }
        public int SlpVoice2 { get; set; }
        public int SlpPragmatics { get; set; }
        public int SlpVoice { get; set; }
        public int SlpFluency { get; set; }
        public int SlpArticulation { get; set; }
        public int SlpExpressiveLanguage { get; set; }
        public int SlpReceptiveLanguage { get; set; }
        public int SlpAttendingSkills { get; set; }
        public int SlpResponseRate { get; set; }
        public int SlpLevelOfActivity { get; set; }
        public string SlpGoals { get; set; }
        public int SlpCooperation { get; set; }
        public int SlpSocialInteractions { get; set; }
        public int SlpCommunicativeIntent { get; set; }
        public int SlpAwarenessOfOthers { get; set; }
        public int SlpReliabilityOfScores { get; set; }
        public int SlpEnvironmentalAwareness { get; set; }
        public int SlpPrognosis { get; set; }
        public string SlpProgressNotes { get; set; }
        public string SlpDiagnosis { get; set; }
        public string SlpRecommendationNotes { get; set; }
        public int SlpGoalsStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}