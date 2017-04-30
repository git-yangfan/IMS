using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Evaluate.Algorithms
{
    public class DataPoint
    {
        public int  Count { get; set; }
        public double  PauseTime { get; set; }
        public float XValueF { get; set; }//可能在可靠性的时候会用得到
        public string XValueStr { get; set; }
    }
}