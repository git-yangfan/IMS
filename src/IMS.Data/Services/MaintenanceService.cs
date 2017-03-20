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
            {"SelfRepairReject","自修方案失败"},
            {"OutRepairPass" , "外修申请通过"},
            {"OutRepairReject" , "外修申请失败"},
            {"PauseRepairPass" , "缓修申请通过"},
            {"PauseRepairReject" , "缓修申请失败"},
            {"Reject" , "已驳回"}
        };
        public enum Role 
        { 
            Worker,
            Engineer,
            Manager,
        }

        private static Dictionary<string, string> DeviceShortNameAndNoDic ;




        #region 维修申请---操作工人
        public bool AddNewFailure(MaintenanceApplicationViewModel maintenanceApplicationVM)
        {
            bool result = false;
            if (maintenanceApplicationVM != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {
                        GZShenQing repariApplicationM = MaintenanceApplicationViewModelToModel(maintenanceApplicationVM);
                        repariApplicationM.Id  = client.Queryable<GZShenQing>().Max(it => it.Id).ObjToInt() + 1;
                        result= client.Insert<GZShenQing>(repariApplicationM).ObjToBool();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;

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
        public bool UpdateSelfRepairPlan(int appId, string type, string msg)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                if (string.Equals(type, "approve"))
                {
                    client.Update<GZShenQing>(new { DQZT = StatusDic["SelfRepairPass"] }, it => it.Id == appId);
                    client.Update<ZXFA>(new { HFSJ = DateTime.Now, HFXX = msg, HFRID = 9999, SFTG = "是" }, it => it.GZSHENQINGID == appId);
                }
                if (string.Equals(type, "reject"))
                {
                    client.Update<GZShenQing>(new { DQZT = StatusDic["SelfRepairReject"] }, it => it.Id == appId);
                    client.Update<ZXFA>(new { HFSJ = DateTime.Now, HFXX = msg, HFRID = 9999, SFTG = "否" }, it => it.GZSHENQINGID == appId);
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
        public bool CreatNewSelfRepairPlan(SelfRepairPlanViewModel selfRepairPlanVM)
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
                    //更改故障申请记录的状态 GZShenQing
                    client.Update<GZShenQing>(new { DQZT = StatusDic["SelfRepairChecking"] }, it => it.Id == selfRepairPlanVM.RepairAppId);
                    //插入新的故障信息   GZXX
                    GZXX applicationInfo = new GZXX();
                    applicationInfo.ID = client.Queryable<GZXX>().Max(it => it.ID).ObjToInt() + 1;
                    applicationInfo.GZSHENQINGID = selfRepairPlanVM.RepairAppId;
                    applicationInfo.ZXID = selfRepairPlanM.ID;
                    client.Insert<GZXX>(applicationInfo);
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
        public bool UpdateSelfRepairPlan(SelfRepairPlanViewModel selfRepairPlanVM, int appId)
        {
            bool result = false;
            using (var client = DbConfig.GetInstance())
            {
                client.BeginTran();
                client.CommandTimeOut = 30000;
                //更新gzsq中的当前状态
                client.Update<GZShenQing>(new { DQZT = StatusDic["SelfRepairChecking"] }, it => it.Id == appId);
                 //更新zxfa
                ZXFA selfRepairPlanM = new ZXFA(selfRepairPlanVM);
                client.DisableUpdateColumns = new string[] { "ID" };//id不更新
                client.Update<ZXFA>(selfRepairPlanM, it => it.GZSHENQINGID == appId);
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


        public SelfRepairPlanViewModel SelfRepairPlanByAppId(int appId)
        {
            using (var client = DbConfig.GetInstance())
            {
                ZXFA selfRepairPlanM = client.Queryable<ZXFA>().Single(it => it.GZSHENQINGID == appId);
                SelfRepairPlanViewModel resultVM = new SelfRepairPlanViewModel(selfRepairPlanM);
                return resultVM;
            }
        }

      
        public List<MaintenanceApplicationViewModel> GetAllApplicationsByRole(Role role, string sectionName, string deviceNo, string beginTime, string endTime)
        {
            using (var client = DbConfig.GetInstance())
            {
                List<GZShenQing> listWithDeviceNo = new List<GZShenQing>();
                var sql = client.Queryable<GZShenQing>();
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
                if ( Enum.Equals(role,Role.Worker))
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
