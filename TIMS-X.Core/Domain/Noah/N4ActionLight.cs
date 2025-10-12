using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain.Noah
{
    public class N4ActionLight
    {
        public N4ActionLight()
        {
            ActionReferences = new HashSet<N4ActionReference>();
        }

        public int Id { get; set; }
        public int? SessionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        public int ModuleId { get; set; }
        public short DevTypeCode { get; set; }
        public short DataTypeCode { get; set; }
        public short DataFmtCodeStd { get; set; }
        public short DataFmtCodePriv { get; set; }
        public string Description { get; set; }
        public bool Removed { get; set; }
        public bool Hidden { get; set; }
        public DateTime? ActionGroup { get; set; }
        public bool IsArchived { get; set; }
        public bool CompressedPublicBlob { get; set; }
        public bool CompressedPrivateBlob { get; set; }
        public string SpeechData { get; set; }
        public string WordRecognitionData { get; set; }
        public string SetupData { get; set; }
        public string ExtendedData { get; set; }
        public int FastViewDataType { get; set; }
        public int FastViewVersion { get; set; }
        public Guid ActionGuid { get; set; }
        public ICollection<N4ActionReference> ActionReferences { get; set; }
    }
}
