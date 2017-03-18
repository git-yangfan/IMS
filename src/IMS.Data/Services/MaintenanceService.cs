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
    public class MaintenanceService
    {
        public MaintenanceService()
        {
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
            {"SelfRepairReject","自修方案失败"},
            {"OutRepairPass" , "外修申请通过"},
            {"OutRepairReject" , "外修申请失败"},
            {"PauseRepairPass" , "缓修申请通过"},
            {"PauseRepairReject" , "缓修申请失败"},
            {"Reject" , "已驳回"}
        };

        private Dictionary<string, string> DeviceShortNameAndNoDic = new Dictionary<string, string>();




        #region 维修申请---操作工人
        public bool AddNewFailure(MaintenanceApplicationViewModel maintenanceApplicationVM)
        {
            if (maintenanceApplicationVM != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {
                        GZShenQing repariApplication = MaintenanceApplicationViewModelToModel(maintenanceApplicationVM);
                        int id = client.Queryable<GZShenQing>().Max(it => it.Id).ObjToInt() + 1;
                        repariApplication.Id = id;
                        return client.Insert<GZShenQing>(repariApplication).ObjToBool();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
                return false;

        }
        public bool UpDateApplication(int id, MaintenanceApplicationViewModel maintenanceApplicationVM)
        {
            bool isUpdateSuccess = false;
            if (maintenanceApplicationVM != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {
                        isUpdateSuccess = client.Update<GZShenQing>(
                               new
                               {
                                   gzms = maintenanceApplicationVM.FailureDescription,
                                   gzxianxiang = maintenanceApplicationVM.FailureAppearance,
                                   gzbwa = maintenanceApplicationVM.FstLevFailureLocation,
                                   gzbwb = maintenanceApplicationVM.SecLevFailureLocation,
                                   gzbwc = maintenanceApplicationVM.ThiLevFailureLocation
                               },
                               it => it.Id == id).ObjToBool();
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
                return client.Delete<GZShenQing>(it => it.Id == id).ObjToBool();
            }
        }
        #endregion


        #region 维修申请处理---管理员
        public bool Dispatch(ApplicationProcessViewModel applicationProcessVM, string workType)
        {

            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                GZXX failureInfo = new GZXX();
                PGD dispatchSheet = null;
                ZDPGD diagnosticSheet = null;
                if (String.Equals(workType, "维修"))
                {
                    dispatchSheet = new PGD();
                    dispatchSheet.ID = client.Queryable<PGD>().Max(it => it.ID).ObjToInt() + 1;
                    dispatchSheet.PGRID = 0;
                    dispatchSheet.GZSHENQINGID = applicationProcessVM.MaintenanceApplicationViewModel.Id;
                    dispatchSheet.PGSJ = DateTime.Now;
                    dispatchSheet.WXRID = applicationProcessVM.EngineerViewModel.EngineerId;
                    dispatchSheet.ZSSX = applicationProcessVM.Instruction;
                    failureInfo.PGDID = dispatchSheet.ID;
                    client.Insert<PGD>(dispatchSheet);
                }
                if (String.Equals(workType, "故障诊断"))
                {
                    diagnosticSheet = new ZDPGD();
                    diagnosticSheet.ID = client.Queryable<ZDPGD>().Max(it => it.ID).ObjToInt() + 1;
                    diagnosticSheet.PGRID = 0;
                    diagnosticSheet.GZSHENQINGID = applicationProcessVM.MaintenanceApplicationViewModel.Id;
                    diagnosticSheet.PGSJ = DateTime.Now;
                    diagnosticSheet.ZDRID = applicationProcessVM.EngineerViewModel.EngineerId;
                    diagnosticSheet.ZSSX = applicationProcessVM.Instruction;
                    failureInfo.ZDPGDID = diagnosticSheet.ID;
                    client.Insert<ZDPGD>(diagnosticSheet);
                }
                failureInfo.ID = client.Queryable<GZXX>().Max(it => it.ID).ObjToInt() + 1;
                failureInfo.GZSHENQINGID = applicationProcessVM.MaintenanceApplicationViewModel.Id;
                try
                {
                    client.Insert<GZXX>(failureInfo);
                    client.Update<GZShenQing>(new { SFKYXG = 1, DQZT = StatusDic["Repairing"], HFSJ = DateTime.Now, HFXX = "同意" }, it => it.Id == applicationProcessVM.MaintenanceApplicationViewModel.Id);
                    client.CommitTran();
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public bool Reject(string applicationId, string rejectReason)
        {
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    bool result = client.Update<GZShenQing>(
                        new
                        {
                            HFSJ = DateTime.Now,
                            HFXX = rejectReason,
                            DQZT = StatusDic["Reject"]
                        }, it => it.Id == Convert.ToInt32(applicationId)).ObjToBool();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public bool UpdateSelfRepairPlanByAdmin(int planId, int appId, string type, string msg)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                if (string.Equals(type, "approve"))
                {
                    client.Update<GZShenQing>(new { DQZT = StatusDic["SelfRepairPass"] }, it => it.Id == appId);
                    client.Update<ZXFA>(new { HFSJ = DateTime.Now, HFXX = msg, HFRID = 9999, SFTG = "是" }, it => it.ID == planId);
                }
                if (string.Equals(type, "reject"))
                {
                    client.Update<GZShenQing>(new { DQZT = StatusDic["SelfRepairReject"] }, it => it.Id == appId);
                    client.Update<ZXFA>(new { HFSJ = DateTime.Now, HFXX = msg, HFRID = 9999, SFTG = "否" }, it => it.ID == planId);
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

        #endregion



        #region 维修申请处理---维修工程师
        public bool InsertNewSelfRepairPlan(SelfRepairPlanViewModel selfRepairPlanVM)
        {
            bool result = false;
            //插入新的自修方案

            //更改故障申请记录的状态
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    client.BeginTran();
                    client.CommandTimeOut = 30000;
                    ZXFA selfRepairPlan = new ZXFA(selfRepairPlanVM);
                    selfRepairPlan.ID = client.Queryable<ZXFA>().Max(it => it.ID).ObjToInt() + 1;
                    client.Insert<ZXFA>(selfRepairPlan);
                    client.Update<GZShenQing>(new { DQZT = StatusDic["SelfRepairChecking"] }, it => it.Id == selfRepairPlanVM.RepairAppId);
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


        public SelfRepairPlanViewModel SelfRepairPlanByAppId(int appId)
        {
            using (var client = DbConfig.GetInstance())
            {
                ZXFA selfRepairPlanM = client.Queryable<ZXFA>().Single(it => it.GZSHENQINGID == appId);
                SelfRepairPlanViewModel resultVM = new SelfRepairPlanViewModel(selfRepairPlanM);
                return resultVM;
            }
        }

        public bool UpdateSelfRepairPlanByEngr(SelfRepairPlanViewModel selfRepairPlanVM, int appId) 
        {
            bool result = false;
            //更新gzsq中的当前状态
            //更新zxfa
            using (var client=DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                client.Update<GZShenQing>(new { DQZT = StatusDic["SelfRepairChecking"] }, it => it.Id == appId);
                ZXFA selfRepairPlanModel = new ZXFA(selfRepairPlanVM);
                client.DisableUpdateColumns = new string[] {"ID"};
                client.Update<ZXFA>(selfRepairPlanModel, it => it.GZSHENQINGID==appId);
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
      
        public List<MaintenanceApplicationViewModel> GetAllApplicationsByName(string name, string sectionName, string deviceNo, string beginTime, string endTime)
        {
            using (var client = DbConfig.GetInstance())
            {

                List<GZShenQing> listWithDeviceNo = new List<GZShenQing>();
                var sql = client.Queryable<GZShenQing>();
                //按工段查询有问题
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
                var b = sql.ToSql();
                try
                {
                    listWithDeviceNo = sql.OrderBy(g => g.FSSJ).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
                List<MaintenanceApplicationViewModel> maintenanceApplicationVMList = new List<MaintenanceApplicationViewModel>();
                for (int i = 0; i < listWithDeviceNo.Count; i++)
                {
                    var item = listWithDeviceNo[i];
                    MaintenanceApplicationViewModel maintenanceApplicationVM = new MaintenanceApplicationViewModel(item);
                    maintenanceApplicationVM.Order = i + 1;
                    maintenanceApplicationVM.DeviceShortName = DeviceShortNameAndNoDic[item.SBBH];
                    maintenanceApplicationVMList.Add(maintenanceApplicationVM);
                }
                return maintenanceApplicationVMList;
            }

        }

        private GZShenQing MaintenanceApplicationViewModelToModel(MaintenanceApplicationViewModel viewModel)
        {
            GZShenQing gZShenQing = new GZShenQing();
            gZShenQing.BGRId = viewModel.ReporterId;
            gZShenQing.BGSJ = viewModel.ReportTime;
            gZShenQing.FSSJ = viewModel.BeginTime;
            gZShenQing.GZBWA = viewModel.FstLevFailureLocation;
            gZShenQing.GZBWB = viewModel.SecLevFailureLocation;
            gZShenQing.GZBWC = viewModel.ThiLevFailureLocation;
            gZShenQing.GZXianXiang = viewModel.FailureAppearance;
            gZShenQing.GZMS = viewModel.FailureDescription;
            gZShenQing.Id = viewModel.Id;
            gZShenQing.SFKYXG = viewModel.Modifiable;
            gZShenQing.SBBH = viewModel.DeviceNo;
            gZShenQing.DQZT = viewModel.Status;

            return gZShenQing;
        }
    }
}
