using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Models.Noah
{
    public class N4LoginResult
    {
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public string JwtToken { get; set; }
    }
}
