using IMS.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IMS.Model.ViewModel
{
    public class ApplicationsViewModel
    {
        public ApplicationsViewModel()
        { }
        public ApplicationsViewModel(WXShenQing model)
        {
            this.Id = model.Id;
            this.DeviceNo = model.SBBH;
            this.BeginTime = model.FSSJ;
            this.ApplicationTime = model.SQSJ;
            this.ApplicantId = model.SQRId;
            this.FailureAppearance = model.GZXianXiang;
            this.FailureDescription = model.GZMS;
            this.FstLevFailureLocation = model.GZBWA;
            this.SecLevFailureLocation = model.GZBWB;
            this.ThiLevFailureLocation = model.GZBWC;
            this.Modifiable = model.SFKYXG;
            this.ReplyTime = model.HFSJ;
            this.ReplyMsg = model.HFXX;
            this.Status = model.DQZT;
            this.FullLocation = model.GZBWA + (model.GZBWB == "请选择" ? "" : "/" + model.GZBWB) + (model.GZBWC == "请选择" ? "" : "/" + model.GZBWC);
            this.DispatchSheetID = model.PGDID;
            this.SelfRepairPlanID = model.ZXFAID;
            this.OutRepairSheetID = model.WXDID;
            this.PauseSheetID = model.HXDID;
            this.DiagnoseSheetID= model.ZDDID;
            this.EvaluateSheetID = model.PingGuDID;
            this.ReplyerId = model.HFRID;
            this.MethodCategory = model.WXFFLB;
            
        }
        public int Order { get; set; }
        public int Id { get; set; }
        public string DeviceShortName { get; set; }
        public string DeviceNo { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime ApplicationTime { get; set; }
        public string ApplicantId { get; set; }
        public string FailureAppearance { get; set; }
        public string FailureDescription { get; set; }
        public string FstLevFailureLocation { get; set; }
        public string SecLevFailureLocation { get; set; }
        public string ThiLevFailureLocation { get; set; }
        public int Modifiable { get; set; }
        public string FullLocation { get; set; }
        public DateTime ReplyTime { set; get; }
        public string ReplyMsg { get; set; }
        public int ReplyerId { get; set; }
        public string Status { set; get; }
        public int DispatchSheetID { get; set; }
        public int SelfRepairPlanID { get; set; }
        public int OutRepairSheetID { get; set; }
        public int PauseSheetID { get; set; }
        public int DiagnoseSheetID { get; set; }
        public int EvaluateSheetID { get; set; }
        public string MethodCategory { get; set; }
    }
}
