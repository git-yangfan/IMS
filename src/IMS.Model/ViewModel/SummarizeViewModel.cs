using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.ViewModel
{
   public class SummarizeViewModel
    {
        public ApplicationsViewModel ApplicationVM { get; set; }
        public SelfRepairPlanViewModel SelfRepairVM { get; set; }
        public string Dispatcher { get; set; }
        public string Engineer { get; set; }
        public DateTime DispatchTime { get; set; }
        public string Instruction { get; set; }

        public string FailureAppearance { get; set; }
        public string FailureDescription { get; set; }
        public string FstLevFailureLocation { get; set; }
        public string SecLevFailureLocation { get; set; }
        public string ThiLevFailureLocation { get; set; }
        public DateTime BeginTime { get; set; }
        public string Steps { get; set; }
        public string Tools { get; set; }
        public string SparePartsInfo { get; set; }
        public int TimeCost { get; set; }
        public int ApplicationID { get; set; }

    }
}
