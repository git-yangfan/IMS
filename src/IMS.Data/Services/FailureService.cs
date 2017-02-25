using IMS.Model.Model;
using System;
using System.Collections.Generic;
using OracleSugar;

namespace IMS.Data.Services
{
    public class FailureService
    {

        public IEnumerable<SubSystem> GetSubSystemList(int lev, int pid)
        {
            IEnumerable<SubSystem> subSystemList = null;
            using (var client = DbConfig.GetInstance())
            {
                subSystemList = client.Queryable<SubSystem>().Where(s => s.Lev == lev).Where(s => s.Pid == pid).ToList();
            }
            return subSystemList;


        }

        public IEnumerable<GZShenQing> GetAllApplicationsByName(string name)
        {

            IEnumerable<GZShenQing> allApplicationsList = null;
            using (var client = DbConfig.GetInstance())
            {
                allApplicationsList = client.Queryable<GZShenQing>().Where(c => c.bgrxm == name).ToList();
            }
            return allApplicationsList;

        }
        public IEnumerable<SBXX> GetShortNameList()
        {
            IEnumerable<SBXX> shortNameList = null;
            using (var client = DbConfig.GetInstance())
            {
                shortNameList = client.Queryable<SBXX>().ToList();
            }

            return shortNameList;
        }
        public bool AddNewFailure(GZShenQing gZShenQing)
        {
            if (gZShenQing != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {
                        int id = client.Queryable<GZShenQing>().Max(it => it.Id).ObjToInt() + 1;
                        gZShenQing.Id = id;
                        int result = client.Insert<GZShenQing>(gZShenQing).ObjToInt();
                        if (result >= 0)
                            return true;
                        else
                            return false;
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
    }
}
