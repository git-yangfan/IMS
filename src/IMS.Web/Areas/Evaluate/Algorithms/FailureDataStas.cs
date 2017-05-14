using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Evaluate.Algorithms
{
    /// <summary>
    /// 故障数据统计
    /// 统计总次数，总停工时间，常发故障等
    /// </summary>
    public class FailureDataStas
    {
        public int TotalCount { get; set; }
        public double TotalPauseTime { get; set; }
        public string FrequentType { get; set; }
        public int FrequentTypeCount { get; set; }
        public double  FrequentTypePauseTime { get; set; }
        public string FrequentLoc { get; set; }
        public int FrequentLocCount { get; set; }
        public double FrequentLocPauseTime { get; set; }
    }
}