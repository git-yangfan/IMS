using IMS.Model.Model;
using IMS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleSugar;

namespace IMS.Data.Services
{
    public class RepairService
    {
        public RepairService()
        {
            DeviceShortNameAndNoDic = new Dictionary<string, string>();
            using (var client = DbConfig.GetInstance())
            {
                var deviceShortNameAndNoList = client.Queryable<SBXX>().ToList();
                if (deviceShortNameAndNoList != null)
                {
                    foreach (var item in deviceShortNameAndNoList)
                    {
                        DeviceShortNameAndNoDic.Add(item.SBBH, item.SBJC);
                    }
                }
            }
        }

        public static Dictionary<string, string> StatusDic = new Dictionary<string, string>()
        {
            {"Checking", "待审核"},
            {"Repairing" , "待维修"},
            {"SelfRepairChecking", "自修方案待审"},
            {"OutRepairChecking" , "外修申请待审"},
            {"Diagnosing" , "待诊断"},
            {"PauseRepairChecking" , "缓修申请待审"},
            {"SelfRepairPass","自修方案通过"},
            {"SelfRepairFail","自修方案失败"},
            {"OutRepairPass" , "外修申请通过"},
            {"OutRepairFail" , "外修申请失败"},
            {"PauseRepairPass" , "缓修申请通过"},
            {"PauseRepairFail" , "缓修申请失败"},
            {"Canceling" , "方案撤销中"},
            {"CancelOK" , "撤销成功"},
            {"CancelFail" , "撤销失败"},
            {"Reject" , "已驳回"}
        };
        public static Dictionary<string, string> MethodCategoryDic = new Dictionary<string, string>() 
        {
            {"Self","自修"},{"Out","外修"},{"Pause","缓修"},{"Diagnose","诊断"},
        };
        public enum Role
        {
            Worker,
            Engineer,
            Manager,
        }

        private static Dictionary<string, string> DeviceShortNameAndNoDic;




        #region 维修申请---操作工人
        public bool AddNewFailure(ApplicationsViewModel repairApplicationVM)
        {
            bool result = false;
            if (repairApplicationVM != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {
                        WXShenQing repariApplicationM = new WXShenQing(repairApplicationVM);
                        repariApplicationM.Id = client.Queryable<WXShenQing>().Max(it => it.Id).ObjToInt() + 1;
                        result = client.Insert<WXShenQing>(repariApplicationM).ObjToBool();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;

        }
        public bool UpDateApplication(ApplicationsViewModel repairApplicationVM)
        {
            bool isUpdateSuccess = false;
            if (repairApplicationVM != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {
                        isUpdateSuccess = client.Update<WXShenQing>(
                               new
                               {
                                   gzms = repairApplicationVM.FailureDescription,
                                   gzxianxiang = repairApplicationVM.FailureAppearance,
                                   gzbwa = repairApplicationVM.FstLevFailureLocation,
                                   gzbwb = repairApplicationVM.SecLevFailureLocation,
                                   gzbwc = repairApplicationVM.ThiLevFailureLocation
                               },
                               it => it.Id == repairApplicationVM.Id).ObjToBool();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return isUpdateSuccess;
        }
        public bool DeleteApplicationById(int id)
        {
            using (var client = DbConfig.GetInstance())
            {
                return client.Delete<WXShenQing>(it => it.Id == id).ObjToBool();
            }
        }
        #endregion


        #region 维修申请处理---管理员
        public bool Dispatch(DispatchSheetViewModel dispatchSheetVM, string workType, int applicationId)
        {
            bool isDispatchSuccess = false;
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    client.BeginTran();
                    client.CommandTimeOut = 30000;
                    PGD dispatchSheet = null;
                    ZDPGD diagnosticSheet = null;
                    if (String.Equals(workType, "维修"))
                    {
                        dispatchSheet = new PGD(dispatchSheetVM);
                        dispatchSheet.ID = client.Queryable<PGD>().Max(it => it.ID).ObjToInt() + 1;
                        client.Insert<PGD>(dispatchSheet);
                        client.Update<WXShenQing>(new { SFKYXG = 1, DQZT = StatusDic["Repairing"], HFSJ = DateTime.Now, HFXX = "同意", PGDID = dispatchSheet.ID }, it => it.Id == applicationId);
                    }
                    if (String.Equals(workType, "故障诊断"))
                    {
                        diagnosticSheet = new ZDPGD(dispatchSheetVM);
                        diagnosticSheet.ID = client.Queryable<ZDPGD>().Max(it => it.ID).ObjToInt() + 1;
                        client.Insert<ZDPGD>(diagnosticSheet);
                        client.Update<WXShenQing>(new { SFKYXG = 1, DQZT = StatusDic["Repairing"], HFSJ = DateTime.Now, HFXX = "同意", ZDPGDID = diagnosticSheet.ID }, it => it.Id == applicationId);
                    }
                    client.CommitTran();
                    isDispatchSuccess = true;

                }
                catch (Exception)
                {
                    throw;
                }
                return isDispatchSuccess;
            }
        }
        public bool Reject(int applicationId, string rejectReason)
        {
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    bool result = client.Update<WXShenQing>(
                        new
                        {
                            SFKYXG = 1,
                            HFSJ = DateTime.Now,
                            HFXX = rejectReason,
                            DQZT = StatusDic["Reject"]
                        }, it => it.Id == applicationId).ObjToBool();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public bool UpdateSelfRepairPlan(int selfRepairPlanID, string type, string msg)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                if (string.Equals(type, "approve"))
                {
                    client.Update<WXShenQing>(new { DQZT = StatusDic["SelfRepairPass"] }, it => it.ZXFAID == selfRepairPlanID);
                    client.Update<ZXFA>(new { HFSJ = DateTime.Now, HFXX = msg, HFRID = 9999, SFTG = "是" }, it => it.ID == selfRepairPlanID);
                }
                if (string.Equals(type, "reject"))
                {
                    client.Update<WXShenQing>(new { DQZT = StatusDic["SelfRepairFail"] }, it => it.ZXFAID == selfRepairPlanID);
                    client.Update<ZXFA>(new { HFSJ = DateTime.Now, HFXX = msg, HFRID = 9999, SFTG = "否" }, it => it.ID == selfRepairPlanID);
                }
                try
                {
                    client.CommitTran();
                    result = true;
                }
                catch (Exception)
                {
                    throw;
                }
                return result;
            }
        }
        /// <summary>
        /// 管理员撤销维修方案，更新wxshenqing中的dqzt和维修方案的id，删除对应的方案记录
        /// </summary>
        /// <param name="methodCategory"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool CancelProcedure(string methodCategory, int categoryId)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                if (String.Equals(methodCategory, "自修"))
                {
                    client.Update<WXShenQing>(new { DQZT = StatusDic["CancelOK"], ZXFAID = 0, WXFFLB = string.Empty }, it => it.ZXFAID == categoryId);
                    client.Delete<ZXFA>(it => it.ID == categoryId);
                }
                try
                {
                    client.CommitTran();
                    result = true;
                }
                catch (Exception)
                {

                    throw;
                }

            }
            return result;
        }
        #endregion



        #region 维修申请处理---维修工程师
        public bool CreatNewSelfRepairPlan(SelfRepairPlanViewModel selfRepairPlanVM, int appId)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    client.BeginTran();
                    client.CommandTimeOut = 30000;
                    //插入新的自修方案 ZXFA
                    ZXFA selfRepairPlanM = new ZXFA(selfRepairPlanVM);
                    selfRepairPlanM.ID = client.Queryable<ZXFA>().Max(it => it.ID).ObjToInt() + 1;
                    client.Insert<ZXFA>(selfRepairPlanM);
                    //更改申请记录的状态 WXShenQing
                    client.Update<WXShenQing>(new { DQZT = StatusDic["SelfRepairChecking"], ZXFAID = selfRepairPlanM.ID, WXFFLB = MethodCategoryDic["Self"] }, it => it.Id == appId);

                    client.CommitTran();
                    result = true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }
        public bool UpdateSelfRepairPlan(SelfRepairPlanViewModel selfRepairPlanVM, int planId)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                //更新gzsq中的当前状态
                client.Update<WXShenQing>(new { DQZT = StatusDic["SelfRepairChecking"] }, it => it.ZXFAID == planId);
                //更新zxfa
                ZXFA selfRepairPlanM = new ZXFA(selfRepairPlanVM);
                client.DisableUpdateColumns = new string[] { "ID" };//id不更新
                client.Update<ZXFA>(selfRepairPlanM, it => it.ID == planId);
                try
                {
                    client.CommitTran();
                    result = true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// 维修工程师撤销某个维修方案，只标记维修申请表中的 dqzt为 撤销中
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool UpDateApplication(int appId)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                result = client.Update<WXShenQing>(
                               new
                               {
                                   DQZT = StatusDic["Canceling"]
                               },
                               it => it.Id == appId).ObjToBool();
            }
            return result;
        }
        #endregion


        public SelfRepairPlanViewModel SelfRepairPlanByAppId(int selfRepairPlanID)
        {
            using (var client = DbConfig.GetInstance())
            {
                ZXFA selfRepairPlanM = client.Queryable<ZXFA>().Single(it => it.ID == selfRepairPlanID);
                SelfRepairPlanViewModel resultVM = new SelfRepairPlanViewModel(selfRepairPlanM);
                return resultVM;
            }
        }


        public List<ApplicationsViewModel> GetAllApplicationsByRole(Role role, string sectionName, string deviceNo, string beginTime, string endTime)
        {
            using (var client = DbConfig.GetInstance())
            {
                List<WXShenQing> listWithDeviceNo = new List<WXShenQing>();
                var sql = client.Queryable<WXShenQing>();
                if (!String.IsNullOrEmpty(sectionName) && !String.Equals(sectionName, "请选择"))
                {
                    var deviceofSelectSection = client.Queryable<SBXX>().Where(c => c.SSGD == sectionName).ToList();
                    List<string> deviceNosofSelectSection = new List<string>();
                    foreach (var item in deviceofSelectSection)
                    {
                        deviceNosofSelectSection.Add(item.SBBH);
                    }
                    sql.In("sbbh", deviceNosofSelectSection);
                }
                if (!String.IsNullOrEmpty(deviceNo) && !String.Equals(deviceNo, "-1"))
                {
                    sql.Where(g => g.SBBH == deviceNo);
                }
                if (!String.IsNullOrEmpty(beginTime))
                {
                    DateTime _beginTime = Convert.ToDateTime(beginTime);
                    sql.Where(g => g.FSSJ > _beginTime);
                }
                if (!String.IsNullOrEmpty(endTime))
                {
                    DateTime _endTime = Convert.ToDateTime(endTime);
                    sql.Where(g => g.FSSJ < _endTime);
                }
                if (Enum.Equals(role, Role.Worker))
                {
                    sql.Where("DQZT=@Checking or DQZT=@Reject", StatusDic);
                    var b = sql.ToSql();
                }
                if (Enum.Equals(role, Role.Engineer))
                {
                    sql.Where("DQZT!=@Checking or DQZT!=@Reject", StatusDic);
                }
                try
                {
                    listWithDeviceNo = sql.OrderBy(g => g.FSSJ).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
                List<ApplicationsViewModel> repairApplicationVMList = new List<ApplicationsViewModel>();
                for (int i = 0; i < listWithDeviceNo.Count; i++)
                {
                    var item = listWithDeviceNo[i];
                    ApplicationsViewModel repairApplicationVM = new ApplicationsViewModel(item);
                    repairApplicationVM.Order = i + 1;
                    repairApplicationVM.DeviceShortName = DeviceShortNameAndNoDic[item.SBBH];
                    repairApplicationVMList.Add(repairApplicationVM);
                }
                return repairApplicationVMList;
            }

        }


    }
}
