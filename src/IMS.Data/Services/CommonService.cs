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
       

        public static List<EngineerViewModel> GetName(string type,string teamName)
        {
            List<EngineerViewModel> engineerVMList = new List<EngineerViewModel>();
            using (var client = DbConfig.GetInstance())
            {
                List<Users> usersList = null;
                if (string.Equals(type,"team"))
                {
                    usersList = client.Queryable<Users>().Select("BanZu").OrderBy("BanZu").GroupBy("BanZu").ToList();
                }
                if (string.Equals(type,"Engineer"))
                {
                      usersList = client.Queryable<Users>().Where(c=>c.BanZu==teamName).ToList();
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
                IEnumerable<SBXX> ListWithAllInfo = client.Queryable<SBXX>().Where(c=>c.SSGD==sectionName).ToList();
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

    }



}
