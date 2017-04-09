using IMS.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Entity
{
    public class Device : BaseEntity
    {
        public string DeviceNo { get; set; }
        public string ShortName { get; set; }
        public string WorkSection { get; set; }
    }
}
