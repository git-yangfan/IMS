using IMS.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.ViewModel
{
  public  class DispatchSheetViewModel
    {
        public string Instruction { get; set; }
        public int DispatcherId { get; set; }
        public DateTime DispatchTime { get; set; }
        public int EngineerId { get; set; }

    }
}
