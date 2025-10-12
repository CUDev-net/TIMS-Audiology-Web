using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Models.Noah
{
    public class N4FastView
    {
        public int Version { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public byte[] Data { get; set; }
        public int Format { get; set; }
    }
}
