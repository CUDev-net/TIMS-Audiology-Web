using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class Description : Entity, IUpdateAudited
    {
	    public DateTime UpdatedDate { get; set; }
	    public int? UpdatedUserId { get; set; }
        public string CustomDate1Label { get; set; }
        public string CustomDate2Label { get; set; }
        public string CustomText1Label { get; set; }
        public string CustomText2Label { get; set; }
	}
}
