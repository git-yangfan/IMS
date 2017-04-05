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
        public string BUZHOU { get; set; }
        public string GONGJU { get; set; }
        public string SFSYBJ { get; set; }
        public string BJXX { get; set; }
        public double WXYS { get; set; }//预计用时
        public DateTime HFSJ { get; set; }
        public int HFRID { get; set; }
        public string HFXX { get; set; }
        public string SFTG { get; set; }
        public DateTime KSSJ { get; set; }


        public ZXFA(SelfRepairPlanViewModel VM)
        {
            this.BUZHOU = VM.Steps;
            this.GONGJU = VM.Tools;
            this.SFSYBJ = VM.IsUseSpareParts;
            this.BJXX = VM.SparePartsInfo;
            this.WXYS = VM.TimeCost;
            this.KSSJ = VM.StartTime;
            if (!int.Equals(VM.ID,null))
            {
               this.ID = VM.ID; 
            }
            
        }
        public ZXFA() { }
    }
}
