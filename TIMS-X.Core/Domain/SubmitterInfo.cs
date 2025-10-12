using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class SubmitterInfo : Entity, IUpdateDateAudited, ICreateDateAudited
    {
        public bool Inactive { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string TaxId { get; set; }
        public string Notes { get; set; }
        public string Description { get; set; }
        public string NationalProviderId { get; set; }
        public int UpdatedUserId { get; set; }
        public string TaxIdType { get; set; }
        public int CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}