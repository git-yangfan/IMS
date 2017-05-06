using IMS.Web.Areas.Evaluate.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Dto
{
    public class DeviceEvaluateDto:EvaluateBase
    {
        //public string DisplayName { get; set; }
        //public DateTime StartTime { get; set; }
        //public DateTime  EndTime { get; set; }
        public string Status { get; set; }
        public FailureDataStas FailureStas { get; set; }
        //public List<Curve> Curves{ get; set; }
        public DncRelated dncRelateReliability { get; set; }
        //public double  MTBF { get; set; }
    }
}