using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Dto
{
    public class CompareBrandDto:CompareBase
    {
        public string MachineType { get; set; }
        public List<BrandEvaluateDto> BrandList { get; set; }
    }
}