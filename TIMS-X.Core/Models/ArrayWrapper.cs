using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Models
{
    // used for transferring arrays of objects in put method body
    public class ArrayWrapper<T>
    {
        public T[] Values { get; set; }
    }
}
