using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Domain
{
    public class PosDocument : Entity, IUpdateAudited
    {
        public int PatientId { get; set; }
        public DateTime DocumentDate { get; set; }
        public int TaxGroupId { get; set; }
        public PosDocumentType DocumentType { get; set; }
        public int SiteId { get; set; }
        public int ProviderId { get; set; }
        public int CustomerMessageId { get; set; }
        public bool Void { get; set; }
        public bool Final { get; set; }
        public string BillTo { get; set; }
        public decimal PaymentAmount { get; set; }
        public int PaymentMethodId { get; set; }
        public string Memo { get; set; }
        public DateTime? QbUpdateDate { get; set; }
        public string QbInvoice { get; set; }
        public string QbTransactionId { get; set; }
        public string PaymentReference { get; set; }
        public bool IsCopay { get; set; }
        public bool CopayApplied { get; set; }
        public decimal CopayAmount { get; set; }
        public int AppointmentId { get; set; }
        public string Notes { get; set; }
        public string PoNumber { get; set; }
        public int ApplyToId { get; set; }
        public int MarketingId { get; set; }
        public int PaymentType { get; set; }
        public string BillToAddr1 { get; set; }
        public string BillToAddr2 { get; set; }
        public string BillToAddr3 { get; set; }
        public string BillToAddr4 { get; set; }
        public string BillToCity { get; set; }
        public string BillToState { get; set; }
        public string BillToPostalCode { get; set; }
        public string BillToCountry { get; set; }
        public byte[] RowVersion { get; set; }
        public int InsurancePosItem { get; set; }
        public bool InsuranceSelected { get; set; }
        public bool IsTaxIncluded { get; set; }
        public string PdfClaimData { get; set; }
        public string FormClaimNumber { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public int UpdatedBySiteId { get; set; }
        public int? Modifier1 { get; set; }
        public int? Modifier2 { get; set; }
        public int? Modifier3 { get; set; }
        public int? Modifier4 { get; set; }
        public int? Diagnosis1 { get; set; }
        public int? Diagnosis2 { get; set; }
        public int? Diagnosis3 { get; set; }
        public int? Diagnosis4 { get; set; }
        public int PosDepositId { get; set; }
        public DateTime? ArUpdateDate { get; set; }
        public bool ArVoid { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<PosLineItem> PosLines { get; set; }
    }
}
