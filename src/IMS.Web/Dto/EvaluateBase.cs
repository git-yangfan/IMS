using IMS.Web.Areas.Evaluate.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Dto
{
    public class EvaluateBase
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string DisplayName { get; set; }
        public List<Curve> Curves { get; set; }
        public double MTBF { get; set; }
        public double Alph { get; set; }
        public double Beta { get; set; }

    }
}