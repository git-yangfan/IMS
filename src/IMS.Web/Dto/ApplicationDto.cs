﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace IMS.Web.Dto
{
    public class ApplicationDto : BaseDto
    {
          [DisplayName("设备简称")]
        public string DeviceShortName { get; set; }
          [DisplayName("设备编号")]
        public string DeviceNo { get; set; }
          [DisplayName("发生时间")]
        public DateTime BeginTime { get; set; }
          [DisplayName("申请时间")]
        public DateTime ApplicationTime { get; set; }
        public string ApplicantId { get; set; }
        public string FailureAppearance { get; set; }
        public string FailureDescription { get; set; }
        public string FirstLocation { get; set; }
        public string SecondLocation { get; set; }
        public string ThirdLocation { get; set; }
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
        public string FailureType { get; set; }
        public string FailureReason { get; set; }
    }
}