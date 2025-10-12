using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class Version
    {
        public int Id { get; set; }
        public int? FileSeq { get; set; }
        public string VersionNumber { get; set; }
        public DateTime? DtApplied { get; set; }
        public DateTime? DtDownload { get; set; }
        public byte[] FileImage { get; set; }
        public bool ApplyUpdate { get; set; }
        public byte[] RowVersion { get; set; }
        public Guid VersionGuid { get; set; }

        public DateTime DateApplied => DtApplied ?? DtDownload.Value;

        public string VersionDisplay
        {
            get
            {
                if(DtApplied.HasValue)
                {
                    return VersionNumber.Substring(0, VersionNumber.LastIndexOf('.'));
                }
                return VersionNumber;
            }
        }

        public string Type => DtApplied.HasValue ? "DB Patch" : "TIMS Update";

        public string Background => DtApplied.HasValue ? "rgb(220, 220, 220)" : "white";
    }
}
