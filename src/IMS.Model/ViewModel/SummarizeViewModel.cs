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

        public SummarizeViewModel() 
        {
            this.ApplicationVM = new ApplicationsViewModel();
            this.SelfRepairVM = new SelfRepairPlanViewModel();
        }
       

    }
}
