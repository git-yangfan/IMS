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
        #region 维修申请---操作工人
        public bool AddNewFailure(MaintenanceApplicationViewModel maintenanceApplicationVM)
        {
            if (maintenanceApplicationVM != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {
                        GZShenQing gZShenQing = MaintenanceApplicationViewModelToModel(maintenanceApplicationVM);
                        int id = client.Queryable<GZShenQing>().Max(it => it.Id).ObjToInt() + 1;
                        gZShenQing.Id = id;
                        return client.Insert<GZShenQing>(gZShenQing).ObjToBool();
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
        public bool Dispatch(FailureProcessViewModel failureProcessVM, string workType)
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
                    dispatchSheet.GZSHENQINGID = failureProcessVM.MaintenanceApplicationViewModel.Id;
                    dispatchSheet.PGSJ = DateTime.Now;
                    dispatchSheet.WXRID = failureProcessVM.EngineerViewModel.EngineerId;
                    dispatchSheet.ZSSX = failureProcessVM.Instruction;
                    failureInfo.PGDID = dispatchSheet.ID;
                    client.Insert<PGD>(dispatchSheet);
                }
                if (String.Equals(workType, "故障诊断"))
                {
                    diagnosticSheet = new ZDPGD();
                    diagnosticSheet.ID = client.Queryable<ZDPGD>().Max(it => it.ID).ObjToInt() + 1;
                    diagnosticSheet.PGRID = 0;
                    diagnosticSheet.GZSHENQINGID = failureProcessVM.MaintenanceApplicationViewModel.Id;
                    diagnosticSheet.PGSJ = DateTime.Now;
                    diagnosticSheet.ZDRID = failureProcessVM.EngineerViewModel.EngineerId;
                    diagnosticSheet.ZSSX = failureProcessVM.Instruction;
                    failureInfo.ZDPGDID = diagnosticSheet.ID;
                    client.Insert<ZDPGD>(diagnosticSheet);
                }
                failureInfo.ID = client.Queryable<GZXX>().Max(it => it.ID).ObjToInt() + 1;
                failureInfo.GZSHENQINGID = failureProcessVM.MaintenanceApplicationViewModel.Id;
                try
                {
                    client.Insert<GZXX>(failureInfo);
                    client.Update<GZShenQing>(new { SFKYXG = 1, DQZT = CommonService.StatusDic["Dispatched"], HFSJ = DateTime.Now, HFXX = "同意" }, it => it.Id == failureProcessVM.MaintenanceApplicationViewModel.Id);
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
                            DQZT = CommonService.StatusDic["Reject"]
                        }, it => it.Id == Convert.ToInt32(applicationId)).ObjToBool();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        } 
        #endregion



        #region 维修申请处理---维修工程师
        #endregion



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
                    maintenanceApplicationVM.DeviceShortName = CommonService.GetShortName(item.SBBH);
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
