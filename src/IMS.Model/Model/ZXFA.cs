using IMS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Model
{
    public class ZXFA
    {
        public int ID { get; set; }
        public int GZSHENQINGID { get; set; }
        public string BUZOU { get; set; }
        public string GONGJU { get; set; }
        public string SFSYBJ { get; set; }
        public string BJXX { get; set; }
        public int YJYS { get; set; }//预计用时
        public DateTime HFSJ { get; set; }
        public int HFRID { get; set; }
        public string HFXX { get; set; }
        public string SFTG { get; set; }


        public ZXFA(SelfRepairPlanViewModel VM)
        {
            this.GZSHENQINGID = VM.RepairAppId;
            this.BUZOU = VM.Steps;
            this.GONGJU = VM.Tools;
            this.SFSYBJ = VM.IsUseSpareParts;
            this.BJXX = VM.SparePartsInfo;
            this.YJYS = VM.TimeCost;
        }
        public ZXFA() { }
    }
}
