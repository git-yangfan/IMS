using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Web.Dto
{
   public class SelfRepairPlanDto
    {
        public int ID { get; set; }
        public string Step { get; set; }
        public string Tool { get; set; }
        public string IsUseSpareParts { get; set; }
        public string SparePartsInfo { get; set; }
        public double TimeCost { get; set; }
        public DateTime ReplyTime { get; set; }
        public int ReplyerId { get; set; }
        public string ReplyMsg { get; set; }
        public string IsPass { get; set; }
        public DateTime  StartTime { get; set; }

       
    }
}
