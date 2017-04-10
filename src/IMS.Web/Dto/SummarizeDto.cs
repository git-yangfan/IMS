using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Web.Dto
{
   public class SummarizeDto
    {
        public ApplicationDto ApplicationDto { get; set; }
        public SelfRepairPlanDto SelfRepairDto { get; set; }
        public string Dispatcher { get; set; }
        public string Engineer { get; set; }
        public DateTime DispatchTime { get; set; }
        public string Instruction { get; set; }


    }
}
