using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Entity
{
    public class MMDD_SBLY
    {
        public string DeviceNo { get; set; }
        public string MMDD { get; set; }
        public double RunTime { get; set; }
        public double RunNull { get; set; }
        public double RunDelay { get; set; }
        public double PauseTime { get; set; }
        public double RepairTime { get; set; }
        public double DelayTime { get; set; }
        public double Schepuleddowntime { get; set; }
        public double ShutdownTime { get; set; }
        public double KBSJ { get; set; }
        public double RunTime_KB { get; set; }
    }
}
