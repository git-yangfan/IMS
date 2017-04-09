using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Dic
{
    public static class CommonDic
    {
        public static Dictionary<string, string> AppSortDic = new Dictionary<string, string>() 
        {
            {"Id","Id"},
            {"DeviceNo","SBBH"},
            {"DeviceShortName","SBBH"},
            {"BeginTime","FSSJ"},
            {"ApplicationTime","SQSJ"},
            {"ApplicantId","SQRId"},
            {"FailureAppearance","GZXianXiang"},
            {"FailureDescription","GZMS"},
            {"FstLevFailureLocation","GZBWA"},
            {"SecLevFailureLocation","GZBWB"},
            {"ThiLevFailureLocation","GZBWC"},
            {"Modifiable","SFKYXG"},
            {"ReplyTime","HFSJ"},
            {"ReplyMsg","HFXX"},
            {"Status","DQZT"},
            {"DispatchSheetID","PGDID"},
            {"SelfRepairPlanID","ZXFAID"},
            {"OutRepairSheetID","WXDID"},
            {"PauseSheetID","HXDID"},
            {"DiagnoseSheetID","ZDDID"},
            {"EvaluateSheetID","PingGuDID"},
            {"ReplyerId","HFRID"},
            {"MethodCategory","WXFFLB"},
        };
    }
}