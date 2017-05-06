using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Evaluate.Algorithms
{
    //用于统计故障部位 故障类别 ，或者计算mtbf
    public class FailureRecord
    {
        public string DeviceNo { get; set; }
        public double PauseTime { get; set; }
        public string FirstLocation { get; set; }
        public string SecondLocation { get; set; }
        public string ThirdLocation { get; set; }
    }
}