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
                IEnumerable<SBXX> ListWithAllInfo = client.Queryable<SBXX>().OrderBy(d => d.SSGD, OrderByType.Asc).ToList();
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


        public static List<MaintenanceApplicationViewModel> GetAllApplicationsByName(string name, int limit, int offset, string sectionName, string deviceNo, string beginTime, string endTime)
        {

            using (var client = DbConfig.GetInstance())
            {
                List<GZShenQing> listWithDeviceNo = new List<GZShenQing>();
                var sql = client.Queryable<GZShenQing>();
                //按工段查询有问题
                //if (!String.Equals(sectionName, String.Empty) && !String.Equals(sectionName, "请选择"))
                //{
                //    sql.JoinTable<SBXX>((g, s) => g.SBBH == s.SBBH).Where<SBXX>((g, s) => s.SSGD == sectionName);
                //}
                if (!String.Equals(deviceNo, String.Empty) && !String.Equals(deviceNo, "-1"))
                {
                    sql.Where(g => g.SBBH == deviceNo);
                }
                if (!String.Equals(beginTime, String.Empty))
                {
                    DateTime _beginTime = Convert.ToDateTime(beginTime);
                    sql.Where(g => g.FSSJ > _beginTime);
                }
                if (!String.Equals(endTime, String.Empty))
                {
                    DateTime _endTime = Convert.ToDateTime(endTime);
                    sql.Where(g => g.FSSJ < _endTime);

                }
                var a = sql.OrderBy(g => g.FSSJ).Skip(offset).Take(limit).ToSql();
                try
                {
                    listWithDeviceNo = sql.OrderBy(g => g.FSSJ).Skip(offset).Take(limit).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
                List<MaintenanceApplicationViewModel> gZShenQingViewModelList = new List<MaintenanceApplicationViewModel>();
                for (int i = 0; i < listWithDeviceNo.Count; i++)
                {
                    var item = listWithDeviceNo[i];
                    MaintenanceApplicationViewModel gZShenQingViewModel = new MaintenanceApplicationViewModel(item);
                    gZShenQingViewModel.Order = i + 1;
                    gZShenQingViewModel.DeviceShortName = CommonService.GetShortName(item.SBBH);
                    gZShenQingViewModelList.Add(gZShenQingViewModel);
                }
                return gZShenQingViewModelList;
            }

        }

    }



}
