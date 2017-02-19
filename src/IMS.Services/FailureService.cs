using IMS.Data;
using IMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Services
{
    public class FailureService
    {
        SubSystemRepository subSystemRepository = new SubSystemRepository();
        GZShenQingRepository failureRepository = new GZShenQingRepository();
        public IEnumerable<SubSystem> GetSubSystemList(int lev, int pid)
        {
            IEnumerable<SubSystem> subSystemList=null;
            if (lev == 0)
            {
                subSystemList = subSystemRepository.GetAll().Where(c=>c.Lev==0);
            }
            else if (lev == 1)
            {

            }
            else if (lev == 2)
            {

            }

            return subSystemList;
        }
        public void AddFailureReport(GZShenQing failureReport)
        {
            if (failureReport!=null)
            {
                failureRepository.Insert(failureReport);
            }

        }
    }
}
