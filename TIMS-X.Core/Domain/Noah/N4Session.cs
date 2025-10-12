using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain.Noah
{
    public class N4Session
    {
        public N4Session()
        {
            Actions = new HashSet<N4Action>();
        }

        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime? CreateDate { get; set; }

        public Patient Patient { get; set; }
        public ICollection<N4Action> Actions { get; set; }
    }
}
