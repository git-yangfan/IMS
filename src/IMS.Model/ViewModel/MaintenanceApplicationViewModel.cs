using IMS.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IMS.Model.ViewModel
{
    public class MaintenanceApplicationViewModel
    {
        public MaintenanceApplicationViewModel()
        { }
        public MaintenanceApplicationViewModel(GZShenQing model)
        {
            this.Id = model.Id;
            this.DeviceNo = model.SBBH;
            this.BeginTime = model.FSSJ;
            this.ReportTime = model.BGSJ;
            this.ReporterId = model.BGRId;
            this.FailureAppearance = model.GZXianXiang;
            this.FailureDescription = model.GZMS;
            this.FstLevFailureLocation = model.GZBWA;
            this.SecLevFailureLocation = model.GZBWB;
            this.ThiLevFailureLocation = model.GZBWC;
            this.Modifiable = model.SFKYXG;
            this.ReplyTime = model.HFSJ;
            this.ReplyMsg = model.HFXX;
            this.Status = model.DQZT;
            this.StrLocation = model.GZBWA + (model.GZBWB == "请选择" ? "" : "/" + model.GZBWB) + (model.GZBWC == "请选择" ? "" : "/" + model.GZBWC);

        }
        public int Order { get; set; }
        public int Id { get; set; }
        public string DeviceShortName { get; set; }
        public string DeviceNo { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime ReportTime { get; set; }
        public string ReporterId { get; set; }
        public string FailureAppearance { get; set; }
        public string FailureDescription { get; set; }
        public string FstLevFailureLocation { get; set; }
        public string SecLevFailureLocation { get; set; }
        public string ThiLevFailureLocation { get; set; }
        public int Modifiable { get; set; }
        public string StrLocation { get; set; }
        public DateTime ReplyTime { set; get; }
        public string ReplyMsg { get; set; }

        public string Status { set; get; }
    }
}
