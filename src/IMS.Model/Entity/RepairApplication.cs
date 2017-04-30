using IMS.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Entity
{
    public class RepairApplication : BaseEntity
    {
        public string DeviceNo { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? ApplicationTime { get; set; }
        public string ApplicantId { get; set; }
        public string FailureAppearance { get; set; }
        public string FailureDescription { get; set; }
        public string FirstLocation { get; set; }
        public string SecondLocation { get; set; }
        public string ThirdLocation { get; set; }
        public DateTime? ReplyTime { set; get; }
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
        public DateTime? SummarizeTime { get; set; }
        public DateTime? CheckTime { get; set; }
        public string FailureType { get; set; }
        public string FailureReason { get; set; }


    }
}
