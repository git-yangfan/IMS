﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Model
{
    //派工单
  public  class ZDPGD
    {
        public int ID { get; set; }
        public int PGRID { get; set; }
        public int ZDRID { get; set; }
        public DateTime PGSJ { get; set; }
        public string ZSSX { get; set; }
        public int GZSHENQINGID { get; set; }
    }
}
