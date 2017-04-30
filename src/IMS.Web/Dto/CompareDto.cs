using IMS.Web.Areas.Evaluate.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Dto
{
    public class CompareDto
    {
        public DeviceDto Device { get; set; }
        public DncRelated DncReliability { get; set; }
        public double MTBF { get; set; }
        public double Alph { get; set; }
        public double  Beta { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime  EndTime { get; set; }
        public List<Curve> Curves{ get; set; }
        public FailureDataStas FailureStat { get; set; }

    }
}