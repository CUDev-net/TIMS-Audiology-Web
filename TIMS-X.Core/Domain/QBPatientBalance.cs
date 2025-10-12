using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class QBPatientBalance : Entity
    {
        public decimal Balance
        {
            get;
            set;
        }

        public DateTime DtAcquired
        {
            get;
            set;
        }

        public string QBID
        {
            get;
            set;
        }

    }
    
}
