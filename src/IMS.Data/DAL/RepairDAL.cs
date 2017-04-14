using IMS.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleSugar;
using IMS.Enume;

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
        public static Dictionary<string, string> DeviceShortNameAndNoDic;
        public bool InsertNewApplication(RepairApplication application)
        {
            bool result = false;
            try
            {
                using (var client = DbConfig.GetInstance())
                {
                    //ApplicationM.Id = client.Queryable<RepairApplication>().Max(it => it.Id).ObjToInt() + 1;
                    result = client.Insert<RepairApplication>(application).ObjToBool();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
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
                                   Status = CurrentStatus.方案撤销中.ToString()
                               }, it => it.Id == appId).ObjToBool();
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
                            Status = CurrentStatus.已驳回.ToString()
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
                    client.Update<RepairApplication>(
                        new
                        {
                            Status = CurrentStatus.待维修.ToString(),
                            ReplyTime = DateTime.Now,
                            ReplyMsg = "同意",
                            DispatchSheetID = dispatchId
                        }, it => it.Id == applicationId);
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
                    var diagnoseDispatchId = client.Queryable<DiagnoseDispatch>().Max(it => it.Id).ObjToInt();
                    client.Update<RepairApplication>(
                        new
                        {
                            Status = CurrentStatus.待维修.ToString(),
                            ReplyTime = DateTime.Now,
                            ReplyMsg = "同意",
                            DiagnoseSheetID = diagnoseDispatchId
                        }, it => it.Id == applicationId);
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
        public bool ApproveSelfRepairPlan(int selfRepairPlanId, string msg)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                client.Update<RepairApplication>(
                    new
                    {
                        Status = CurrentStatus.自修方案通过.ToString()
                    }, it => it.SelfRepairPlanID == selfRepairPlanId);
                client.Update<SelfRepairPlan>(
                    new
                    {
                        ReplyTime = DateTime.Now,
                        ReplyMsg = msg,
                        ReplyerId = 9999,
                        IsPass = "是"
                    }, it => it.Id == selfRepairPlanId);
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
        public bool RejectSelfRepairPlan(int selfRepairPlanId, string msg)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                client.Update<RepairApplication>(
                    new
                    {
                        Status = CurrentStatus.自修方案失败.ToString()
                    }, it => it.SelfRepairPlanID == selfRepairPlanId);
                client.Update<SelfRepairPlan>(
                    new
                    {
                        ReplyTime = DateTime.Now,
                        ReplyMsg = msg,
                        ReplyerId = 9999,
                        IsPass = "否"
                    }, it => it.Id == selfRepairPlanId);
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
                    var planId = client.Queryable<SelfRepairPlan>().Max(it => it.Id).ObjToInt();
                    selfPlanId = planId;
                    //更改申请记录的状态 RepairApplication
                    client.Update<RepairApplication>(
                        new
                        {
                            Status = CurrentStatus.自修方案待审.ToString(),
                            SelfRepairPlanID = planId,
                            MethodCategory = MethodCategory.自修.ToString()
                        }, it => it.Id == appId);
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
        public bool UpdateSelfRepairPlan(SelfRepairPlan selfRepairPlan, int planId)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                //更新gzsq中的当前状态
                client.Update<RepairApplication>(
                    new
                    {
                       Status = CurrentStatus.自修方案待审.ToString()
                    }, it => it.SelfRepairPlanID == planId);
                //更新SelfRepairPlan
                client.DisableUpdateColumns = new string[] { "ID" };//id不更新
                client.Update<SelfRepairPlan>(selfRepairPlan, it => it.Id == planId);
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
                    client.Update<RepairApplication>(
                        new
                        {
                            Status = CurrentStatus.撤销成功.ToString(),
                            SelfRepairPlanID = 0,
                            MethodCategory = string.Empty
                        }, it => it.SelfRepairPlanID == categoryId);
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
        public RepairApplication AllInfo(int appId, ref Dispatch dispatchSheet, ref SelfRepairPlan selfRepairPlan, ref string dispatherName, ref string engineerName)
        {
            using (var client = DbConfig.GetInstance())
            {
                var app = client.Queryable<RepairApplication>().SingleOrDefault(it => it.Id == appId);
                var dispatch = client.Queryable<Dispatch>().SingleOrDefault(it => it.Id == app.DispatchSheetID);
                var dispather = client.Queryable<Users>().SingleOrDefault(it => it.Id == dispatch.Dispatcher);
                var engineer = client.Queryable<Users>().SingleOrDefault(it => it.Id == dispatch.Engineer);
                var repairPlan = client.Queryable<SelfRepairPlan>().Single(it => it.Id == app.SelfRepairPlanID);
                selfRepairPlan = repairPlan;
                if (dispather != null)
                {
                    dispatherName = dispather.Name;
                }
                if (engineer != null)
                {
                    engineerName = engineer.Name;
                }
                dispatchSheet = dispatch;
                return app;
            }

        }

        public bool Finish(RepairApplication application, SelfRepairPlan selfRepairPlan)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                client.Update<RepairApplication>(
                    new
                    {
                        FirstLocation = application.FirstLocation,
                        SecondLocation = application.SecondLocation,
                        ThirdLocation = application.ThirdLocation,
                        FailureAppearance = application.FailureAppearance,
                        FailureDescription = application.FailureDescription,
                        Status = CurrentStatus.已总结.ToString()
                    }, it => it.Id == application.Id);
                client.Update<SelfRepairPlan>(
                    new
                    {
                        StartTime = selfRepairPlan.StartTime,
                        TimeCost = selfRepairPlan.TimeCost,
                        IsUseSpareParts = selfRepairPlan.IsUseSpareParts,
                        Step = selfRepairPlan.Step,
                        Tool = selfRepairPlan.Tool,
                        SparePartsInfo = selfRepairPlan.SparePartsInfo,
                    }, it => it.Id == selfRepairPlan.Id);
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
        public SelfRepairPlan SelfRepairPlanByAppId(int selfRepairPlanId)
        {
            using (var client = DbConfig.GetInstance())
            {
                SelfRepairPlan selfRepairPlan = client.Queryable<SelfRepairPlan>().Single(it => it.Id == selfRepairPlanId);
                return selfRepairPlan;
            }
        }

    }
}
