using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.ViewModel
{
  public  class ApplicationProcessViewModel
    {
      public ApplicationProcessViewModel() 
      {
          this.MaintenanceApplicationViewModel = new MaintenanceApplicationViewModel();
          this.EngineerViewModel = new EngineerViewModel();
      }
        public MaintenanceApplicationViewModel MaintenanceApplicationViewModel { get; set; }
        public EngineerViewModel EngineerViewModel { get; set; }
        public string Instruction { get; set; }

    }
}
