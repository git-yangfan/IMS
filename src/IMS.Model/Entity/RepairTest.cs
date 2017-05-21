using IMS.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Entity
{
   public class RepairTest
    {
       public int Id { get; set; }
        public string DeviceNo { get; set; }
        public DateTime? BeginTime { get; set; }
        public string FailureAppearance { get; set; }
        //public string FailureDescription { get; set; }
        public string FirstLocation { get; set; }
        public string SecondLocation { get; set; }
        public string ThirdLocation { get; set; }
    }
}
