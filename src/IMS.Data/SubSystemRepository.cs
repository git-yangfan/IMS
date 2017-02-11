using IMS.Model;
using System;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;

namespace IMS.Data
{
    public class SubSystemRepository : RepositoryBase<SubSystem>
    {
        public IEnumerable<SubSystem> GetSubSystemList(int lev, int pid)
        {
            IEnumerable<SubSystem> subSystemList = null;
            if (lev == 0)
            {
                subSystemList = this.GetList(new { Lev = 0 });
            }
            else if (lev == 1)
            {

            }
            else if (lev == 2)
            {

            }

            return subSystemList;
        }
    }
}
