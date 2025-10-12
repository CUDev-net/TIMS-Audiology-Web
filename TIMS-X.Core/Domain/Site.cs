using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class Site : Entity, IUpdateAudited
    {
        public Site()
        {
            Resources = new HashSet<Resource>();
        }

        public bool Inactive { get; set; }
        public int PracticeId { get; set; }
        public string Name { get; set; }
        public int DefaultTaxGroupId { get; set; }
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string CityStateZip => $"{City}, {State} {Zip}";
        public string Phone { get; set; }
        public string Qbid { get; set; }
        public DateTime? QbModifiedDate { get; set; }
        public DateTime? MonStart { get; set; }
        public DateTime? MonEnd { get; set; }
        public DateTime? TuesStart { get; set; }
        public DateTime? TuesEnd { get; set; }
        public DateTime? WedStart { get; set; }
        public DateTime? WedEnd { get; set; }
        public DateTime? ThurStart { get; set; }
        public DateTime? ThurEnd { get; set; }
        public DateTime? FriStart { get; set; }
        public DateTime? FriEnd { get; set; }
        public DateTime? SatStart { get; set; }
        public DateTime? SatEnd { get; set; }
        public DateTime? SunStart { get; set; }
        public DateTime? SunEnd { get; set; }
        public int? Color { get; set; }
        public Guid? SiteSettingId { get; set; }
        public Guid? EcheckPaymentId { get; set; }
        public Guid? EcreditCardPaymentId { get; set; }
        public int RegionId { get; set; }
        public string Npi { get; set; }
        public string SecondaryIdqualifier { get; set; }
        public string SecondaryIdnum { get; set; }
        public Guid? EcareCreditPaymentId { get; set; }
        public byte[] Logo { get; set; }
        public string CareCreditPassword { get; set; }
        public string CareCreditMerchantNumber { get; set; }
        public string CareCreditPracticeCode { get; set; }
        public string AllWellId { get; set; }
        public string FaxNumber { get; set; }
        public string CcpromoCode { get; set; }
        public string CustomText1 { get; set; }
        public string CustomText2 { get; set; }
        public string CustomText3 { get; set; }
        public string TransnationalUsername { get; set; }
        public string TransnationalPassword { get; set; }
        public string TransnationalAuthKey { get; set; }
        public string OutreachEducator { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<Resource> Resources { get; set; }
        public string WebColor { get; set; }
        public bool UseForPatientScheduling { get; set; }
	}
}
