using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Models
{
    public class AuthenticationForm
    {
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public string AdDomain { get; set; }
        public string AdUsername { get; set; }
        public string Password { get; set; }

        public string TimsToken { get; set; }
        
    }
}
