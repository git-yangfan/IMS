using IMS.Model;
using System;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Data
{
    public class SubSystemRepository : RepositoryBase<SubSystem>
    {

        private List<SubSystem> GetFirstLevelSubSys(dynamic lev) 
        {
            List<SubSystem> fistLevelSubSystemList = new List<SubSystem>();
            //var tblName = string.Format("dbo.{0}", typeof(SubSystem).Name);
            //var sql = "SELECT * FROM @table WHERE LEV=@lev";
            //IEnumerable<SubSystem> dataList = conn.Query<SubSystem>(sql, new { table = tblName, ids = idsin });
            return fistLevelSubSystemList;
        }

    }
}
