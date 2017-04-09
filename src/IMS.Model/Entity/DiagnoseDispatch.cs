using IMS.Model.Base;
using IMS.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Entity
{
    public class DiagnoseDispatch : BaseEntity
    {
        public string Instruction { get; set; }
        public int Dispatcher { get; set; }
        public DateTime CreatTime { get; set; }
        public int Engineer { get; set; }

    }
}
