using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIMS_X.Core.Models
{
    public static class AudigyCustomers
    {
        public static Dictionary<string, string> GetCustomers()
        {
#if DEBUG
            return new Dictionary<string, string>
            {
                {"DSDB", "12345"},
            };
#elif TEST
            return new Dictionary<string, string>
            {
                {"SALES", "12345"},
            };
#else
            return new Dictionary<string, string>
            {
                {"HCR", "40527"},
                {"GSH", "40489"},
                {"HVA", "40512"},
                {"OCI", "40760"},
                {"RHC", "40860"},
                {"DSY", "40453"},
            };    
#endif
        }
    }
}
