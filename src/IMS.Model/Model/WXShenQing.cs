using IMS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Model
{
   public class WXShenQing
    {
       /// <summary>
       /// 故障id 设备编号 发生时间 报告时间 报告人姓名 故障现象 故障描述 故障一级二级三级部位 是否可以修改
       /// </summary>
        public int Id { get; set; }
        public string SBBH { get; set; }
        public DateTime FSSJ { get; set; }
        public DateTime SQSJ { get; set; }
        public string SQRId { get; set; }
        public string GZXianXiang { get; set; }
        public string GZMS { get; set; }
        public string GZBWA { get; set; }
        public string GZBWB { get; set; }
        public string GZBWC { get; set; }
        public int SFKYXG { get; set; }
        public DateTime HFSJ { set; get; }
        public string DQZT { get; set; }
        public string HFXX { get; set; }
        public int HFRID { get; set; }
        public int PGDID { get; set; }
        public int ZXFAID { get; set; }
        public int WXDID { get; set; }
        public int HXDID { get; set; }
        public int ZDDID { get; set; }
        public int PingGuDID { get; set; }
        public string WXFFLB { get; set; }


        public WXShenQing() { }
        public WXShenQing(ApplicationsViewModel viewModel) 
        {
            this.SQRId = viewModel.ApplicantId;
            this.SQSJ = viewModel.ApplicationTime;
            this.FSSJ = viewModel.BeginTime;
            this.GZBWA = viewModel.FstLevFailureLocation;
            this.GZBWB = viewModel.SecLevFailureLocation;
            this.GZBWC = viewModel.ThiLevFailureLocation;
            this.GZXianXiang = viewModel.FailureAppearance;
            this.GZMS = viewModel.FailureDescription;
            this.HFSJ = viewModel.ReplyTime;
            this.HFXX = viewModel.ReplyMsg;
            this.HFRID = viewModel.ReplyerId;
            this.SFKYXG = viewModel.Modifiable;
            this.SBBH = viewModel.DeviceNo;
            this.DQZT = viewModel.Status;
            this.PGDID = viewModel.DispatchSheetID;
            this.PingGuDID = viewModel.DiagnoseSheetID;
            this.ZXFAID = viewModel.SelfRepairPlanID;
            this.HXDID = viewModel.PauseSheetID;
            this.ZDDID = viewModel.DiagnoseSheetID;
            this.WXFFLB = viewModel.MethodCategory;
            if (!int.Equals(viewModel.Id,null))
            {
                this.Id = viewModel.Id;
            }
        }
    }
}
