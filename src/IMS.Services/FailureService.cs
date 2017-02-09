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
        FailureReportRepository failureRepository = new FailureReportRepository();
        public List<SubSystem> GetSubSystemList(int lev, int pid)
        {
            List<SubSystem> subSystemList = new List<SubSystem>();
            if (lev == 0)
            {
                //subSystemList = subSystemRepository.GetList<SubSystem>(new { Level = 0 }, null, false);
            }
            else if (lev == 1)
            {

            }
            else if (lev == 2)
            {

            }

            return subSystemList;
        }
        public void AddFailureReport(FailureReport failureReport)
        {
            if (failureReport!=null)
            {
                failureRepository.Insert(failureReport);
            }

        }
    }
}
