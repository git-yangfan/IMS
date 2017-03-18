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


        public static  IEnumerable<SubSystem> GetSubSystemList(int lev, int pid)
        {
            IEnumerable<SubSystem> subSystemList = null;
            using (var client = DbConfig.GetInstance())
            {
                subSystemList = client.Queryable<SubSystem>().Where(s => s.Lev == lev).Where(s => s.Pid == pid).ToList();
            }
            return subSystemList;


        }
        public static List<EngineerViewModel> GetTeamOrEngrName(string type, string teamName)
        {
            List<EngineerViewModel> engineerVMList = new List<EngineerViewModel>();
            using (var client = DbConfig.GetInstance())
            {
                List<Users> resultList = null;
                if (string.Equals(type, "team"))
                {
                    resultList = client.Queryable<Users>().Select("BanZu").OrderBy("BanZu").GroupBy("BanZu").ToList();
                }
                if (string.Equals(type, "Engineer"))
                {
                    resultList = client.Queryable<Users>().Where(c => c.BanZu == teamName).ToList();
                }
                if (resultList.Count > 0)
                {
                    for (int i = 0; i < resultList.Count; i++)
                    {
                        EngineerViewModel engineerVM = new EngineerViewModel(resultList[i]);
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
     
    }



}
