using System;
using System.Collections.Generic;
using TIMS_X.Core.Attributes;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class Provider : Entity, IUpdateAudited
    {
        public Provider()
        {
            BlockSchedules = new HashSet<ProviderBlockSchedule>();
        }
        public bool Inactive { get; set; }
        public string LastName { get; set; }
        public bool UseForPatientScheduling { get; set; }
		public bool SignatureOnFile { get; set; }
        public bool UsePracticeIds { get; set; }
        public string FirstName { get; set; }
        public bool NotBillable { get; set; }
        public int BillToId { get; set; }
        public string Initial { get; set; }
        public string Degree { get; set; }
        public DateTime? SignatureDate { get; set; }
        public string Upin { get; set; }
        public string TaxId { get; set; }
        public string TaxIdType { get; set; }
        public string GroupNum { get; set; }
        public bool Deleted { get; set; }
        public string Npi { get; set; }
        public int? Color { get; set; }
        public string SecondaryIdNum { get; set; }
        public string SecondaryIdQualifier { get; set; }
        public string SpecialtyCode { get; set; }
        public string SpecialtyLicNum { get; set; }
        public string MedicareNum { get; set; }
        public string MedicaidNum { get; set; }
        public string BlueShieldNum { get; set; }
        public string StateLicNum { get; set; }
        public string AnesthesiaLicNum { get; set; }
        public string Qbid { get; set; }
        public DateTime? QbModifiedDate { get; set; }
        public string Qbid2 { get; set; }
        public string Taxonomy { get; set; }
        //public byte[] RowVersion { get; set; }
        //public byte[] Signature { get; set; }
        public int DisplayOrder { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string FullName =>
            $"{LastName}, {FirstName}{(string.IsNullOrWhiteSpace(Initial) ? string.Empty : " " + Initial + ".")}";

        public int UserId { get; set; }
        [TimsObject]
        public User User { get; set; }

        public ICollection<ProviderBlockSchedule> BlockSchedules { get; set; }
        public List<HoursOfOperationModel> Hours { get; set; }
        public List<UserSiteHours> SiteHours { get; set; }
        public string WebColor { get; set; }
    }
}
