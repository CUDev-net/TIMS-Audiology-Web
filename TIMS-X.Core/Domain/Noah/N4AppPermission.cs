using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain.Noah
{
    public class N4AppPermission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public bool Default { get; set; }
        public bool ReadOnly { get; set; }
        public bool Required { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
    }
}
