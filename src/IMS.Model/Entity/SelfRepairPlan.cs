using IMS.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Entity
{
    public class SelfRepairPlan : BaseEntity
    {
        public string Step { get; set; }
        public string Tool { get; set; }
        public string IsUseSpareParts { get; set; }
        public string SparePartsInfo { get; set; }
        public double TimeCost { get; set; }
        public DateTime ReplyTime { get; set; }
        public int ReplyerId { get; set; }
        public string ReplyMsg { get; set; }
        public string IsPass { get; set; }
        public DateTime  StartTime { get; set; }

        //public SelfRepairPlanEntity() { }
        //public SelfRepairPlanEntity(ZXFA model) 
        //{
        //    this.ID = model.ID;
        //    this.Steps = model.BUZHOU;
        //    this.Tools = model.GONGJU;
        //    this.IsUseSpareParts = model.SFSYBJ;
        //    this.SparePartsInfo = model.BJXX;
        //    this.TimeCost = model.WXYS;
        //    this.ReplyTime = model.HFSJ;
        //    this.ReplyerId = model.HFRID;
        //    this.ReplyMsg = model.HFXX;
        //    this.IsPass = model.SFTG;
        //    this.StartTime = model.KSSJ;
        //}
    }
}
