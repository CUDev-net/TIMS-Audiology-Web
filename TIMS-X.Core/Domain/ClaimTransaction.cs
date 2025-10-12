using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class ClaimTransaction: ICreateByUserAudited, IUpdateAudited, ICreateDateAudited
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public bool Selected { get; set; }
        public bool Deleted { get; set; }
        public int ClaimId { get; set; }
        public int Action { get; set; }
        public int Place { get; set; }
        public int Type { get; set; }
        public int TransactionType { get; set; }
        public int? SequenceNum { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? CptCode { get; set; }
        public int? Modifier1 { get; set; }
        public int? Modifier2 { get; set; }
        public int? Modifier3 { get; set; }
        public int? Modifier4 { get; set; }
        public string Diagnosis1 { get; set; }
        public string Diagnosis2 { get; set; }
        public string Diagnosis3 { get; set; }
        public string Diagnosis4 { get; set; }
        public decimal? Charges { get; set; }
        public decimal? Units { get; set; }
        public int? DiagPointer1 { get; set; }
        public int? DiagPointer2 { get; set; }
        public int? DiagPointer3 { get; set; }
        public int? DiagPointer4 { get; set; }
        public string ReviewByCode { get; set; }
        public decimal? ObligatedToAcceptAmount { get; set; }
        public bool? PurchaseService { get; set; }
        public decimal? PurchaseServiceCharge { get; set; }
        public string SpecialPricingIndicator { get; set; }
        public int? CopayStatus { get; set; }
        public string NarrativeData { get; set; }
        public decimal? PrimaryAmountPaid { get; set; }
        public decimal? DisallowedCostCont { get; set; }
        public decimal? DisallowedOther { get; set; }
        public decimal? PrimaryAllowedAmount { get; set; }
        public bool? Resubmit { get; set; }
        public decimal? SecondaryAmountPaid { get; set; }
        public decimal? SecondaryAmountDisallowed { get; set; }
        public decimal? PatientAmountPaid { get; set; }
        public decimal? Adjustment { get; set; }
        public string Notes { get; set; }
        public int? OriginalId { get; set; }
        public DateTime? PatientPaidDate { get; set; }
        public DateTime? PrimaryPaidDate { get; set; }
        public DateTime? SecondaryPaidDate { get; set; }
        public int? ResubmittalId { get; set; }
        public DateTime? QbUpdatedDate { get; set; }
        public string QbInvoice { get; set; }
        public string PrimaryPaymentReference { get; set; }
        public string SecondaryPaymentReference { get; set; }
        public DateTime? QbPrimaryUpdateDate { get; set; }
        public DateTime? QbSecondaryUpdateDate { get; set; }
        public DateTime? AdjustmentDate { get; set; }
        public int? AdjustmentType { get; set; }
        public string PatientPaymentReference { get; set; }
        public DateTime? QbPatientUpdateDate { get; set; }
        public DateTime? QbAdjudicationUpdateDate { get; set; }
        public string QbInvoiceTransactionId { get; set; }
        public string Status { get; set; }
        public decimal Deductible { get; set; }
        public decimal CoInsurance { get; set; }
        public decimal? PatientResponsibilityAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public int MarketingId { get; set; }
        public string OrderNumber { get; set; }
        public int PosLineItemId { get; set; }
        public int PosDocumentId { get; set; }
        public int? ProviderId { get; set; }
        public int? ServiceId { get; set; }
        public int? SupervisingId { get; set; }
        public int? OrderingId { get; set; }
        public int? ReferralId { get; set; }
        public int? FacilityId { get; set; }
        public bool IsInstitutional { get; set; }
        public int RevenueCode { get; set; }
        public string OverrideAccountListId { get; set; }
        public int UpdatedSiteId { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? ArUpdateDate { get; set; }
        public bool ArVoid { get; set; }
    }
}
