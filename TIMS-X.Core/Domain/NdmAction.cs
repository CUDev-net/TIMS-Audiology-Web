using System;
using System.Collections.Generic;

namespace TIMS_X.Core.Domain
{
    public class NdmAction
    {
        public NdmAction()
        {
            Audiograms = new HashSet<NdmAudiogram>();
        }

        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime AudiogramDate { get; set; }
        public int ActionId { get; set; }

        public ICollection<NdmAudiogram> Audiograms { get; set; }
    }
}