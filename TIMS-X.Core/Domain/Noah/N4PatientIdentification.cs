using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain.Noah
{
    public class N4PatientIdentification
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int ManufacturerId { get; set; }
        public string IdentificationData { get; set; }
    }
}
