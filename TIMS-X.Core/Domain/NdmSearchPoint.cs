using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class NdmSearchPoint
    {
        public int Id { get; set; }
        public int SearchCriteriaId { get; set; }
        public int Frequency { get; set; }
        public int? LevelLowerBound { get; set; }
        public int? LevelUpperBound { get; set; }

        public NdmSearchCriteria SearchCriteria { get; set; }
    }
}
