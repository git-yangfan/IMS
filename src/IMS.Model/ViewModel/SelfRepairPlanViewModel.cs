﻿using IMS.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.ViewModel
{
   public class SelfRepairPlanViewModel
    {
        public int ID { get; set; }
        public string Steps { get; set; }
        public string Tools { get; set; }
        public string IsUseSpareParts { get; set; }
        public string SparePartsInfo { get; set; }
        public int TimeCost { get; set; }
        public DateTime ReplyTime { get; set; }
        public int ReplyerId { get; set; }
        public string ReplyMsg { get; set; }
        public string IsPass { get; set; }

        public SelfRepairPlanViewModel() { }
        public SelfRepairPlanViewModel(ZXFA model) 
        {
            this.ID = model.ID;
            this.Steps = model.BUZHOU;
            this.Tools = model.GONGJU;
            this.IsUseSpareParts = model.SFSYBJ;
            this.SparePartsInfo = model.BJXX;
            this.TimeCost = model.YJYS;
            this.ReplyTime = model.HFSJ;
            this.ReplyerId = model.HFRID;
            this.ReplyMsg = model.HFXX;
            this.IsPass = model.SFTG;
        }
    }
}
