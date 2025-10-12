using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid AreaId { get; set; }
        public byte CountryId { get; set; }
    }
}
