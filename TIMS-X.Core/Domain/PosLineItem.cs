using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class PosLineItem : Entity
    {
        public int POSDocID { get; set; }
        public int ItemID { get; set; }
        public int TableID { get; set; }
        public int ItemType { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public bool Taxable { get; set; }
        public bool IsPercent { get; set; }
        public decimal Amount { get; set; }
        public int AATableID { get; set; }
        public int AAItemID { get; set; }
        public decimal TaxAmt { get; set; }
        public int Seq { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int ApplyToID { get; set; }
        public int TaxGroupID { get; set; }
        public string OrderNumber { get; set; }
        public int PaymentType { get; set; }
        public string PaymentReference { get; set; }
        public bool InsuranceSelected { get; set; }
        public int InsurancePOSItem { get; set; }
        public bool IsTaxIncluded { get; set; }
        public string OverrideAccountListID { get; set; }
        public int? SuperBillLineID { get; set; }
        public decimal? InsuranceAmount { get; set; }
        public decimal UnitCost { get; set; }
        public int? Modifier1 { get; set; }
        public int? Modifier2 { get; set; }
        public int? Modifier3 { get; set; }
        public int? Modifier4 { get; set; }
        public int? Diagnosis1 { get; set; }
        public int? Diagnosis2 { get; set; }
        public int? Diagnosis3 { get; set; }
        public int? Diagnosis4 { get; set; }
        public DateTime? DtARUpdate { get; set; }
        public bool ARVoid { get; set; }
        public int POSDepositID { get; set; }
        public bool deleted { get; set; }
        public HaHistory HaHistory { get; set; }
    }
}