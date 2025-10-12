using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain.Noah
{
    public class N4UserSetup
    {
        public int UserId { get; set; }
        public int ModuleId { get; set; }
        public byte[] SetupData { get; set; }
    }
}
