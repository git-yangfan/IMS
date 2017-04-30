using IMS.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleSugar;
using System.Data;

namespace IMS.Data.DAL
{
    public static class CommonDAL
    {
        public static List<SubSystem> GetSubSystemList(int lev, int pid)
        {
            List<SubSystem> subSystemList = null;
            using (var client = DbConfig.GetInstance())
            {
                subSystemList = client.Queryable<SubSystem>().Where(s => s.Lev == lev).Where(s => s.Pid == pid).ToList();
            }
            return subSystemList;
        }

        public static List<Users> GetTeamOrEngrName(string type, string teamName)
        {
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

                return resultList;
            }

        }

        public static List<Device> GetWorkSectionNameList()
        {
            using (var client = DbConfig.GetInstance())
            {
                List<Device> deviceList = client.Queryable<Device>().ToList();
                return deviceList;
            }

        }
        public static List<Device> GetDevicesBySection(string sectionName)
        {
            using (var client = DbConfig.GetInstance())
            {
                List<Device> deviceList = new List<Device>();
                if (!String.IsNullOrEmpty(sectionName))
                    deviceList = client.Queryable<Device>().Where(c => c.WorkSection == sectionName).ToList();
                else
                    deviceList = client.Queryable<Device>().ToList();
                return deviceList;
            }

        }

    }
}
