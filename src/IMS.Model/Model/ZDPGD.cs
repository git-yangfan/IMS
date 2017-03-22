using IMS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Model
{
    //派工单
  public  class ZDPGD
    {
      //3.21与数据库一致
        public int ID { get; set; }
        public int PGRID { get; set; }
        public int ZDRID { get; set; }
        public DateTime PGSJ { get; set; }
        public string ZSSX { get; set; }
        public ZDPGD(DispatchSheetViewModel viewModel) 
        {
            this.PGRID = viewModel.DispatcherId;
            this.ZDRID = viewModel.EngineerId;
            this.PGSJ = viewModel.DispatchTime;
            this.ZSSX = viewModel.Instruction;
        }
        public ZDPGD() { }
    }
}
