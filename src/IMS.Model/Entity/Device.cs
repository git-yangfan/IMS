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

        public string Manufacturer { get; set; }
        public DateTime ManufacTime { get; set; }
        public DateTime BuyTime { get; set; }
        public DateTime BeginUseTime { get; set; }
        public string Spec { get; set; }
        public string Type { get; set; }

        public string UseDepart { get; set; }
        public string  Status { get; set; }
        public string  Src { get; set; }
    }
}
