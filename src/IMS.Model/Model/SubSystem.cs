using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Model
{
  public class SubSystem
    {
        public int Id { get; set; }
        public int Lev { get; set; }
        public int Pid { get; set; }
        public string Name { get; set; }
        public string Src { get; set; }//图片地址
        public string Map { get; set; }//图片
        public int Ratio { get; set; }//比例，系数，权重

    }
}
