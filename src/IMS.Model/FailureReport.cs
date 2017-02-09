using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model
{
   public class FailureReport
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public DateTime OccurTime { get; set; }
        public int SubSystemId { get; set; }
        public DateTime ReportTime { get; set; }
        public string ReporterName { get; set; }
        public string FaultSymptom { get; set; }
        public string FaultDescription { get; set; }
    }
}
