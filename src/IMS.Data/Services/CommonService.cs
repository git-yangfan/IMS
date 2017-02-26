using IMS.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleSugar;

namespace IMS.Data.Services
{
    public static class CommonService
    {
        public static string GetShortName(string sbbh)
        {
            using (var client = DbConfig.GetInstance())
            {
                var single = client.Queryable<SBXX>().Single(c=>c.SBBH==sbbh);
                return single.SBJC;
            }
        }
    }
}
