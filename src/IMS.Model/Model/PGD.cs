using IMS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Model
{
    //派工单
  public  class PGD
    {
      //3.21确认和数据库一致
        public int ID { get; set; }
        public int PGRID { get; set; }
        public int WXRID { get; set; }
        public DateTime PGSJ { get; set; }
        public string ZSSX { get; set; }

        public PGD(DispatchSheetViewModel viewModel) 
        {
            this.PGRID = viewModel.DispatcherId;
            this.WXRID = viewModel.EngineerId;
            this.PGSJ = viewModel.DispatchTime;
            this.ZSSX = viewModel.Instruction;
        }
        public PGD() { }
    }
}
