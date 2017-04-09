using IMS.Model.Model;
//using IMS.Model.ViewModel;
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
            {"Reject" , "已驳回"},
            {"End","已总结"}
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
        public static Dictionary<string, string> DeviceShortNameAndNoDic;
        public bool InsertNewApplication(WXShenQing ApplicationM)
        {
            bool result = false;
            try
            {
                using (var client = DbConfig.GetInstance())
                {
                    //WXShenQing repariApplicationM = new WXShenQing(repairApplicationVM);
                    ApplicationM.Id = client.Queryable<WXShenQing>().Max(it => it.Id).ObjToInt() + 1;
                    result = client.Insert<WXShenQing>(ApplicationM).ObjToBool();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;

        }
        public List<WXShenQing> GetApplicationsByRole(Role role, string sectionName, string deviceNo, string beginTime, string endTime, string ordername, string order)
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
                    sql.Where("DQZT!=@Checking and DQZT!=@Reject and DQZT!=@End", StatusDic);
                }
                if (!string.IsNullOrEmpty(ordername) && !string.IsNullOrEmpty(order))
                {
                    sql.OrderBy(ordername + " " + order);
                }
                try
                {
                    listWithDeviceNo = sql.ToList();
                }
                catch (Exception)
                {
                    throw;
                }
                return listWithDeviceNo;
            }

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
        public bool UpdateApplication(int applicationId, string rejectReason)
        {
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    bool result = client.Update<WXShenQing>(
                        new
                        {
                            SFKYXG = 0,
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
        public bool UpdateApplication(int id, string app, string des, string firstLoc, string secondLoc, string thirdLoc)
        {
            bool isUpdateSuccess = false;
            try
            {
                using (var client = DbConfig.GetInstance())
                {
                    isUpdateSuccess = client.Update<WXShenQing>(
                           new
                           {
                               gzms = des,
                               gzxianxiang = app,
                               gzbwa = firstLoc,
                               gzbwb = secondLoc,
                               gzbwc = thirdLoc
                           }, it => it.Id == id).ObjToBool();
                }
            }
            catch (Exception)
            {
                throw;
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
        public bool RepairDispatch(PGD dispatchSheetM, int applicationId)
        {
            bool isDispatchSuccess = false;
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    client.BeginTran();
                    client.CommandTimeOut = 30000;
                    dispatchSheetM.ID = client.Queryable<PGD>().Max(it => it.ID).ObjToInt() + 1;
                    client.Insert<PGD>(dispatchSheetM);
                    client.Update<WXShenQing>(new { SFKYXG = 1, DQZT = StatusDic["Repairing"], HFSJ = DateTime.Now, HFXX = "同意", PGDID = dispatchSheetM.ID }, it => it.Id == applicationId);
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
        public bool DiagnoseDispatch(ZDPGD dispatchSheetM, int applicationId)
        {
            bool isDispatchSuccess = false;
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    client.BeginTran();
                    client.CommandTimeOut = 30000;
                    dispatchSheetM.ID = client.Queryable<ZDPGD>().Max(it => it.ID).ObjToInt() + 1;
                    client.Insert<ZDPGD>(dispatchSheetM);
                    client.Update<WXShenQing>(new { SFKYXG = 1, DQZT = StatusDic["Repairing"], HFSJ = DateTime.Now, HFXX = "同意", ZDPGDID = dispatchSheetM.ID }, it => it.Id == applicationId);
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
        public bool ApproveSelfRepairPlan(int selfRepairPlanID, string msg)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                client.Update<WXShenQing>(new { DQZT = StatusDic["SelfRepairPass"] }, it => it.ZXFAID == selfRepairPlanID);
                client.Update<ZXFA>(new { HFSJ = DateTime.Now, HFXX = msg, HFRID = 9999, SFTG = "是" }, it => it.ID == selfRepairPlanID);
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
        public bool RejectSelfRepairPlan(int selfRepairPlanID, string msg)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                client.Update<WXShenQing>(new { DQZT = StatusDic["SelfRepairFail"] }, it => it.ZXFAID == selfRepairPlanID);
                client.Update<ZXFA>(new { HFSJ = DateTime.Now, HFXX = msg, HFRID = 9999, SFTG = "否" }, it => it.ID == selfRepairPlanID);
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
        public bool InsertNewSelfRepairPlan(ZXFA selfRepairPlanM, int appId)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    client.BeginTran();
                    client.CommandTimeOut = 30000;
                    //插入新的自修方案 ZXFA
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
        public bool UpdateSelfRepairPlan(ZXFA selfRepairPlanM, int planId)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                //更新gzsq中的当前状态
                client.Update<WXShenQing>(new { DQZT = StatusDic["SelfRepairChecking"] }, it => it.ZXFAID == planId);
                //更新zxfa
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
        public WXShenQing AllInfo(int appId,  ref PGD dispatchSheetM, ref ZXFA selfRepairPlanM, ref string dispatherName, ref string engineerName)
        {
            using (var client = DbConfig.GetInstance())
            {
                var appM = client.Queryable<WXShenQing>().SingleOrDefault(it => it.Id == appId);
                var pGDM = client.Queryable<PGD>().SingleOrDefault(it => it.ID == appM.PGDID);
                var dispather = client.Queryable<Users>().SingleOrDefault(it => it.Id == pGDM.PGRID);
                var engineer = client.Queryable<Users>().SingleOrDefault(it => it.Id == pGDM.WXRID);
                var ZXFA = client.Queryable<ZXFA>().Single(it => it.ID == appM.ZXFAID);
                selfRepairPlanM = ZXFA;
                if (dispather != null)
                {
                    dispatherName = dispather.Name;
                }
                if (engineer != null)
                {
                    engineerName = engineer.Name;
                }
                dispatchSheetM = pGDM;
                return appM;
            }

        }

        public bool Finish(WXShenQing applicationM, ZXFA selfRepairPlanM)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                client.Update<WXShenQing>(
                    new
                    {
                        GZBWA = applicationM.GZBWA,
                        GZBWB = applicationM.GZBWB,
                        GZBWC = applicationM.GZBWC,
                        GZXIANXIANG = applicationM.GZXianXiang,
                        GZMS = applicationM.GZMS,
                        DQZT = StatusDic["End"]
                    }, it => it.Id == applicationM.Id);
                client.Update<ZXFA>(
                    new
                    {
                        KSSJ = selfRepairPlanM.KSSJ,
                        WXYS = selfRepairPlanM.WXYS,
                        SFSYBJ = selfRepairPlanM.SFSYBJ,
                        BUZHOU = selfRepairPlanM.BUZHOU,
                        GONGJU = selfRepairPlanM.GONGJU,
                        BJXX = selfRepairPlanM.BJXX,
                    }, it => it.ID == selfRepairPlanM.ID);
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
        public ZXFA SelfRepairPlanByAppId(int selfRepairPlanID)
        {
            using (var client = DbConfig.GetInstance())
            {
                ZXFA selfRepairPlanM = client.Queryable<ZXFA>().Single(it => it.ID == selfRepairPlanID);
                return selfRepairPlanM;
            }
        }




    }
}
