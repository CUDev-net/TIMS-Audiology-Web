using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class Manufacturer: Entity, IUpdateAudited
    {
        public int InUse { get; set; }
        public bool Inactive { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Ext { get; set; }
        public string Notes { get; set; }
        public DateTime? AgreementDate { get; set; }
        public string UpdatedUserName { get; set; }
        public string AccountNumber { get; set; }
        public int? QbAccountId { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
