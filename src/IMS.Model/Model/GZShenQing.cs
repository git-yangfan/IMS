using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Model
{
   public class GZShenQing
    {
       /// <summary>
       /// 故障id 设备编号 发生时间 报告时间 报告人姓名 故障现象 故障描述 故障一级二级三级部位 是否可以修改
       /// </summary>
        public int Id { get; set; }
        public string SBBH { get; set; }
        public DateTime fssj { get; set; }
        public DateTime bgsj { get; set; }
        public string bgrxm { get; set; }
        public string gzxianxiang { get; set; }
        public string gzms { get; set; }
        public string GZBWA { get; set; }
        public string GZBWB { get; set; }
        public string GZBWC { get; set; }
        public int sfkyxg { get; set; }
    }
}
