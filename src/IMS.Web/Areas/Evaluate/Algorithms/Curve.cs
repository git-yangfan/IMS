using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Evaluate.Algorithms
{
    public class Curve
    {
        public string DisplayName { get; set; }
        public string[] XValues { get; set; }
        public double[] YTimeValues { get; set; }
        public int[] YCountValues { get; set; }

        public Curve() { }
        public Curve(string name, List<DataPoint> data) 
        {
            this.DisplayName = name;
            if (data.Count>0)
            {
                List<string> x=new List<string>();
                List<int> yc = new List<int>();
                List<double> yt = new List<double>();
                foreach (var item in data)
                {
                    x.Add(item.XValueStr);
                    yc.Add(item.Count);
                    yt.Add(item.PauseTime);
                }
                this.XValues = x.ToArray();
                this.YCountValues = yc.ToArray();
                this.YTimeValues = yt.ToArray();
            }
        }
       

    }
}