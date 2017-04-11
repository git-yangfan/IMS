using IMS.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleSugar;

namespace IMS.Data.DAL
{
    public class RepairDAL
    {
        public RepairDAL()
        {
            DeviceShortNameAndNoDic = new Dictionary<string, string>();
            using (var client = DbConfig.GetInstance())
            {
                var deviceShortNameAndNoList = client.Queryable<Device>().ToList();
                if (deviceShortNameAndNoList != null)
                {
                    foreach (var item in deviceShortNameAndNoList)
                    {
                        DeviceShortNameAndNoDic.Add(item.DeviceNo, item.ShortName);
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

        public static Dictionary<string, string> DeviceShortNameAndNoDic;
        public bool InsertNewApplication(RepairApplication ApplicationM)
        {
            bool result = false;
            try
            {
                using (var client = DbConfig.GetInstance())
                {
                    //ApplicationM.Id = client.Queryable<RepairApplication>().Max(it => it.Id).ObjToInt() + 1;
                    result = client.Insert<RepairApplication>(ApplicationM).ObjToBool();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;

        }
        public List<RepairApplication> ApplicationsByRole1(string role, string sectionName, string deviceNo, string beginTime, string endTime, string ordername, string order, int limit, int offset)
        {
            using (var client = DbConfig.GetInstance())
            {
                List<RepairApplication> listWithDeviceNo = new List<RepairApplication>();
                var sql = client.Queryable<RepairApplication>();
                if (!String.IsNullOrEmpty(sectionName) && !String.Equals(sectionName, "请选择"))
                {
                    var deviceofSelectSection = client.Queryable<Device>().Where(c => c.WorkSection == sectionName).ToList();
                    List<string> deviceNosofSelectSection = new List<string>();
                    foreach (var item in deviceofSelectSection)
                    {
                        deviceNosofSelectSection.Add(item.DeviceNo);
                    }
                    sql.In("DeviceNo", deviceNosofSelectSection);
                }
                if (!String.IsNullOrEmpty(deviceNo) && !String.Equals(deviceNo, "-1"))
                {
                    sql.Where(g => g.DeviceNo == deviceNo);
                }
                if (!String.IsNullOrEmpty(beginTime))
                {
                    DateTime _beginTime = Convert.ToDateTime(beginTime);
                    sql.Where(g => g.BeginTime > _beginTime);
                }
                if (!String.IsNullOrEmpty(endTime))
                {
                    DateTime _endTime = Convert.ToDateTime(endTime);
                    sql.Where(g => g.BeginTime < _endTime);
                }
                if (Enum.Equals(role, "Worker"))
                {
                    sql.Where("Status=@Checking or Status=@Reject", StatusDic);
                    var b = sql.ToSql();
                }
                if (Enum.Equals(role, "Engineer"))
                {
                    sql.Where("Status!=@Checking and Status!=@Reject and Status!=@End", StatusDic);
                }
                if (!string.IsNullOrEmpty(ordername) && !string.IsNullOrEmpty(order))
                {
                    sql.OrderBy(ordername + " " + order);
                }
                try
                {
                    //listWithDeviceNo = sql.Skip(offset).Take(limit).ToList();
                    listWithDeviceNo = sql.ToList();
                }
                catch (Exception)
                {
                    throw;
                }
                return listWithDeviceNo;
            }

        }

        public List<RepairApplication> ApplicationsByRole(string sectionName, string ordername, string order, System.Linq.Expressions.Expression<Func<RepairApplication, bool>> exp, int limit, int offset)
        {
            using (var client = DbConfig.GetInstance())
            {
                List<RepairApplication> listWithDeviceNo = new List<RepairApplication>();
                var sql = client.Queryable<RepairApplication>();
                if (!String.IsNullOrEmpty(sectionName) && !String.Equals(sectionName, "请选择"))
                {
                    var deviceofSelectSection = client.Queryable<Device>().Where(c => c.WorkSection == sectionName).ToList();
                    List<string> deviceNosofSelectSection = new List<string>();
                    foreach (var item in deviceofSelectSection)
                    {
                        deviceNosofSelectSection.Add(item.DeviceNo);
                    }
                    sql.In("DeviceNo", deviceNosofSelectSection);
                }
                sql.Where(exp);
                if (!string.IsNullOrEmpty(ordername) && !string.IsNullOrEmpty(order))
                {
                    sql.OrderBy(ordername + " " + order);
                }
                else
                {
                    sql.OrderBy(it => it.BeginTime, OrderByType.Desc);
                }
                try
                {
                    listWithDeviceNo = sql.Skip(offset).Take(limit).ToList();
                    //listWithDeviceNo = sql.ToList();
                }
                catch (Exception)
                {
                    throw;
                }
                return listWithDeviceNo;
            }

        }

        /// <summary>
        /// 维修工程师撤销某个维修方案，只标记维修申请表中的 Status为 撤销中
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool UpDateApplication(int appId)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                result = client.Update<RepairApplication>(
                               new
                               {
                                   Status = StatusDic["Canceling"]
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
                    bool result = client.Update<RepairApplication>(
                        new
                        {
                            ReplyTime = DateTime.Now,
                            ReplyMsg = rejectReason,
                            Status = StatusDic["Reject"]
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
                    isUpdateSuccess = client.Update<RepairApplication>(
                           new
                           {
                               FailureDescription = des,
                               FailureAppearance = app,
                               FirstLocation = firstLoc,
                               SecondLocation = secondLoc,
                               ThirdLocation = thirdLoc
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
                return client.Delete<RepairApplication>(it => it.Id == id).ObjToBool();
            }
        }
        public bool RepairDispatch(Dispatch dispatch, int applicationId)
        {
            bool isDispatchSuccess = false;
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    client.BeginTran();
                    client.CommandTimeOut = 30000;   
                    client.Insert<Dispatch>(dispatch).ObjToInt();
                    var dispatchId = client.Queryable<Dispatch>().Max(it => it.Id).ObjToInt();                 
                    client.Update<RepairApplication>(new { Status = StatusDic["Repairing"], ReplyTime = DateTime.Now, ReplyMsg = "同意", DispatchSheetID = dispatchId }, it => it.Id == applicationId);
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
        public bool DiagnoseDispatch(DiagnoseDispatch diagnoseDispatch, int applicationId)
        {
            bool isDispatchSuccess = false;
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    client.BeginTran();
                    client.CommandTimeOut = 30000; 
                    client.Insert<DiagnoseDispatch>(diagnoseDispatch).ObjToInt();
                    var diagnoseDispatchId = client.Queryable<DiagnoseDispatch>().Max(it => it.Id).ObjToInt() ;
                    client.Update<RepairApplication>(new { Status = StatusDic["Repairing"], ReplyTime = DateTime.Now, ReplyMsg = "同意", DiagnoseSheetID = diagnoseDispatchId }, it => it.Id == applicationId);
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
                client.Update<RepairApplication>(new { Status = StatusDic["SelfRepairPass"] }, it => it.SelfRepairPlanID == selfRepairPlanID);
                client.Update<SelfRepairPlan>(new { ReplyTime = DateTime.Now, ReplyMsg = msg, ReplyerId = 9999, IsPass = "是" }, it => it.Id == selfRepairPlanID);
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
                client.Update<RepairApplication>(new { Status = StatusDic["SelfRepairFail"] }, it => it.SelfRepairPlanID == selfRepairPlanID);
                client.Update<SelfRepairPlan>(new { ReplyTime = DateTime.Now, ReplyMsg = msg, ReplyerId = 9999, IsPass = "否" }, it => it.Id == selfRepairPlanID);
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
        public bool InsertNewSelfRepairPlan(SelfRepairPlan selfRepairPlan, int appId, out int selfPlanId)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    client.BeginTran();
                    client.CommandTimeOut = 30000;
                    //插入新的自修方案 SelfRepairPlan
                    client.Insert<SelfRepairPlan>(selfRepairPlan);
                    var  planId = client.Queryable<SelfRepairPlan>().Max(it => it.Id).ObjToInt();
                    selfPlanId = planId;
                    //更改申请记录的状态 RepairApplication
                    client.Update<RepairApplication>(new { Status = StatusDic["SelfRepairChecking"], SelfRepairPlanID = planId, MethodCategory = MethodCategoryDic["Self"] }, it => it.Id == appId);
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
        public bool UpdateSelfRepairPlan(SelfRepairPlan selfRepairPlanM, int planId)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                //更新gzsq中的当前状态
                client.Update<RepairApplication>(new { Status = StatusDic["SelfRepairChecking"] }, it => it.SelfRepairPlanID == planId);
                //更新SelfRepairPlan
                client.DisableUpdateColumns = new string[] { "ID" };//id不更新
                client.Update<SelfRepairPlan>(selfRepairPlanM, it => it.Id == planId);
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
        /// 管理员撤销维修方案，更新RepairApplication中的Status和维修方案的id，删除对应的方案记录
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
                    client.Update<RepairApplication>(new { Status = StatusDic["CancelOK"], SelfRepairPlanID = 0, MethodCategory = string.Empty }, it => it.SelfRepairPlanID == categoryId);
                    client.Delete<SelfRepairPlan>(it => it.Id == categoryId);
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
        public RepairApplication AllInfo(int appId, ref Dispatch dispatchSheetM, ref SelfRepairPlan selfRepairPlanM, ref string dispatherName, ref string engineerName)
        {
            using (var client = DbConfig.GetInstance())
            {
                var appM = client.Queryable<RepairApplication>().SingleOrDefault(it => it.Id == appId);
                var DispatchM = client.Queryable<Dispatch>().SingleOrDefault(it => it.Id== appM.DispatchSheetID);
                var dispather = client.Queryable<Users>().SingleOrDefault(it => it.Id == DispatchM.Dispatcher);
                var engineer = client.Queryable<Users>().SingleOrDefault(it => it.Id == DispatchM.Engineer);
                var SelfRepairPlan = client.Queryable<SelfRepairPlan>().Single(it => it.Id == appM.SelfRepairPlanID);
                selfRepairPlanM = SelfRepairPlan;
                if (dispather != null)
                {
                    dispatherName = dispather.Name;
                }
                if (engineer != null)
                {
                    engineerName = engineer.Name;
                }
                dispatchSheetM = DispatchM;
                return appM;
            }

        }

        public bool Finish(RepairApplication applicationM, SelfRepairPlan selfRepairPlanM)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                client.Update<RepairApplication>(
                    new
                    {
                        FirstLocation = applicationM.FirstLocation,
                        SecondLocation = applicationM.SecondLocation,
                        ThirdLocation = applicationM.ThirdLocation,
                        FailureAppearance = applicationM.FailureAppearance,
                        FailureDescription = applicationM.FailureDescription,
                        Status = StatusDic["End"]
                    }, it => it.Id == applicationM.Id);
                client.Update<SelfRepairPlan>(
                    new
                    {
                        StartTime = selfRepairPlanM.StartTime,
                        TimeCost = selfRepairPlanM.TimeCost,
                        IsUseSpareParts = selfRepairPlanM.IsUseSpareParts,
                        Step = selfRepairPlanM.Step,
                        Tool = selfRepairPlanM.Tool,
                        SparePartsInfo = selfRepairPlanM.SparePartsInfo,
                    }, it => it.Id == selfRepairPlanM.Id);
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
        public SelfRepairPlan SelfRepairPlanByAppId(int selfRepairPlanID)
        {
            using (var client = DbConfig.GetInstance())
            {
                SelfRepairPlan selfRepairPlanM = client.Queryable<SelfRepairPlan>().Single(it => it.Id == selfRepairPlanID);
                return selfRepairPlanM;
            }
        }




    }
}
