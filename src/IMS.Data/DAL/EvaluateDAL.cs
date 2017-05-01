using IMS.Model.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OracleSugar;

namespace IMS.Data.DAL
{
    public class EvaluateDAL
    {
        public Device Single(Expression<Func<Device, bool>> exp)
        {
            using (var client = DbConfig.GetInstance())
            {
                return client.Queryable<Device>().SingleOrDefault(exp);
            }
        }
        public DataTable GetDataTable(string sql)
        {
            DataTable resultTable = null;
            using (var client = DbConfig.GetInstance())
            {
                resultTable = client.GetDataTable(sql);
            }
            return resultTable;
        }
        public DataTable GetPauseTimeData(string deviceNo, string startTime, string endTime)
        {
            string sql1 = "select app.*, round((" +
                        "(case when app.CHECKTIME is null then to_date('" + endTime + "', 'yyyy-mm-dd hh24:mi:ss') else app.CHECKTIME end)-" +
                        "(case WHEN app.BEGINTIME > to_date('" + startTime + "', 'yyyy-mm-dd hh24:mi:ss') THEN app.BEGINTIME else to_date('" + startTime + "', 'yyyy-mm-dd hh24:mi:ss') end)" +
                        ") * 24, 2) AS pausetime " +
                        "FROM REPAIRAPPLICATION app " +
                        "WHERE app.CHECKTIME > to_date('" + startTime + "', 'yyyy-mm-dd hh24:mi:ss') " +
                        "AND app.CHECKTIME < to_date('" + endTime + "', 'yyyy-mm-dd hh24:mi:ss') " +
                        "OR app.CHECKTIME is null";
            if (!String.IsNullOrEmpty(deviceNo))
            {
                sql1 += " and app.sbbh='" + deviceNo + "'";
            }
            return GetDataTable(sql1);
        }

        public MMDD_SBLY GetDncData(string deviceNo, string startDate, string endDate)
        {
            StringBuilder strBuilder = new StringBuilder("SELECT sum(kbsj)*24 AS kbsj,sum(runtime_kb)*24 AS runtime_kb,sum(runtime)*24 AS runtime,sum(runnull)*24 AS runnull,sum(rundelay)*24 AS rundelay,");
            strBuilder.Append("sum(pausetime)*24 AS pausetime,sum(repairtime)*24 AS repairtime,sum(delaytime)*24 AS delaytime FROM mmdd_sbly where 1=1 ");

            if (!String.IsNullOrEmpty(deviceNo))
                strBuilder.Append("and sbbh='" + deviceNo+"'");
            if (!String.IsNullOrEmpty(startDate))
                strBuilder.Append(" and to_date(mmdd,'yyyy-mm-dd') >to_date('" + startDate + "','yyyy-mm-dd ')");
            if (!String.IsNullOrEmpty(endDate))
                strBuilder.Append(" and to_date(mmdd,'yyyy-mm-dd') <to_date('" + endDate + "','yyyy-mm-dd ')");
            string sql = strBuilder.ToString();
            using (var client = DbConfig.GetInstance())
            {
                var MMDD = client.SqlQuery<MMDD_SBLY>(sql).FirstOrDefault();
                MMDD.DeviceNo = deviceNo;
                return MMDD;
            }
        }

    }
}
