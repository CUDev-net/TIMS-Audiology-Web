using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain.Noah
{
    public class N4ActionReference
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public int Reference { get; set; }

        public N4Action Action { get; set; }
    }
}
