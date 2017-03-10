using IMS.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleSugar;
using IMS.Model.ViewModel;

namespace IMS.Data.Services
{
    public static class CommonService
    {

   public     static Dictionary<string, string> StatusDic = new Dictionary<string, string>()
        {
            {"Checking", "审核中"},
            {"Dispatched" , "已派工"},
            {"SelfRepair", "自修中"},
            {"OutRepair" , "外修中"},
            {"Diagnosis" , "诊断中"},
            {"PauseRepair" , "缓修"},
            {"Reject" , "已驳回"}
        };
        public static string GetShortName(string sbbh)
        {
            using (var client = DbConfig.GetInstance())
            {
                var single = client.Queryable<SBXX>().Single(c => c.SBBH == sbbh);
                return single.SBJC;
            }
        }


        public static List<EngineerViewModel> GetName(string type, string teamName)
        {
            List<EngineerViewModel> engineerVMList = new List<EngineerViewModel>();
            using (var client = DbConfig.GetInstance())
            {
                List<Users> usersList = null;
                if (string.Equals(type, "team"))
                {
                    usersList = client.Queryable<Users>().Select("BanZu").OrderBy("BanZu").GroupBy("BanZu").ToList();
                }
                if (string.Equals(type, "Engineer"))
                {
                    usersList = client.Queryable<Users>().Where(c => c.BanZu == teamName).ToList();
                }
                if (usersList.Count > 0)
                {
                    for (int i = 0; i < usersList.Count; i++)
                    {
                        EngineerViewModel engineerVM = new EngineerViewModel(usersList[i]);
                        engineerVMList.Add(engineerVM);
                    }
                }
            }
            return engineerVMList;
        }



        public static List<DeviceViewModel> GetWorkSectionNameList()
        {
            List<DeviceViewModel> deviceVMList = new List<DeviceViewModel>();
            using (var client = DbConfig.GetInstance())
            {
                IEnumerable<SBXX> ListWithAllInfo = client.Queryable<SBXX>().Select("SSGD").OrderBy("SSGD desc").GroupBy("SSGD").ToList();
                foreach (var item in ListWithAllInfo)
                {
                    DeviceViewModel viewModel = new DeviceViewModel();
                    viewModel.WorkSection = item.SSGD;
                    deviceVMList.Add(viewModel);
                }
            }
            return deviceVMList;
        }


        public static List<DeviceViewModel> GetDeviceNamesBySection(string sectionName)
        {
            List<DeviceViewModel> deviceVMList = new List<DeviceViewModel>();
            using (var client = DbConfig.GetInstance())
            {
                IEnumerable<SBXX> ListWithAllInfo = client.Queryable<SBXX>().Where(c => c.SSGD == sectionName).ToList();
                foreach (var item in ListWithAllInfo)
                {
                    DeviceViewModel viewModel = new DeviceViewModel();
                    viewModel.DeviceName = item.SBJC;
                    viewModel.DeviceNo = item.SBBH;
                    deviceVMList.Add(viewModel);
                }
            }
            return deviceVMList;
        }


        public static List<MaintenanceApplicationViewModel> GetAllApplicationsByName(string name, string sectionName, string deviceNo, string beginTime, string endTime)
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
                    sql.In("sbbh",deviceNosofSelectSection);
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

    }



}
