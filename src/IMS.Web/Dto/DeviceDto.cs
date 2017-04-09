using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace IMS.Web.Dto
{
   public  class DeviceDto:BaseDto
    {
       [DisplayName("设备编号")]
        public string DeviceNo { get; set; }
       [DisplayName("设备名称")]
        public string ShortName { get; set; }
       [DisplayName("所属工段")]
        public string WorkSection { get; set; }
    }
}
