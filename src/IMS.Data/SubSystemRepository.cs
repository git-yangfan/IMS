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
            subSystemList = this.GetList(new { Lev = lev, Pid = pid });
            return subSystemList;
        }
    }
}
