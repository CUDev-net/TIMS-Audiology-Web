using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain.Noah
{
    public class N4UnboundAction : IUpdateAudited
    {
        public N4UnboundAction()
        {
            N4UnboundActionReferences = new HashSet<N4UnboundActionReference>();
        }

        public int Id { get; set; }
        public int VersionNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModuleId { get; set; }
        public short? DevTypeCode { get; set; }
        public short? DataTypeCode { get; set; }
        public short? DataFmtCodeStd { get; set; }
        public short? DataFmtCodePriv { get; set; }
        public string Description { get; set; }
        public byte[] PublicData { get; set; }
        public byte[] PrivateData { get; set; }
        public bool Removed { get; set; }
        public bool Hidden { get; set; }
        public DateTime ActionGroup { get; set; }
        public bool IsArchived { get; set; }
        public bool CompressedPublicBlob { get; set; }
        public bool CompressedPrivateBlob { get; set; }
        public Guid ActionGuid { get; set; }

        public ICollection<N4UnboundActionReference> N4UnboundActionReferences { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
