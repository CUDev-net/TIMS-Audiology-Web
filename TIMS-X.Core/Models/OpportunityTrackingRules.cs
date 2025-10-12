using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Models
{
    public class OpportunityTrackingRules
    {
        public int ThresholdMonths { get; set; }
        public decimal Threshold1000 { get; set; }
        public decimal Threshold125 { get; set; }
        public decimal Threshold1500 { get; set; }
        public decimal Threshold2000 { get; set; }
        public decimal Threshold250 { get; set; }
        public decimal Threshold3000 { get; set; }
        public decimal Threshold4000 { get; set; }
        public decimal Threshold500 { get; set; }
        public decimal Threshold6000 { get; set; }
        public decimal Threshold750 { get; set; }
        public decimal Threshold8000 { get; set; }
    }
}
