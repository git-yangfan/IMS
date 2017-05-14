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
        public List<double> Interval(string sql) 
        {
            List<double> resultList = new List<double>();
            using (var client = DbConfig.GetInstance())
            {
                resultList = client.SqlQuery<double>(sql).ToList<double>();
            }
            return resultList;
        }
        public List<double> Interval(string deviceNo,string startTime,string endTime)
        {
            List<double> resultList = new List<double>();
            using (var client = DbConfig.GetInstance())
            {
                string sql = "select Round((BEGINTIME-lag(BEGINTIME,1,null)over(order by BEGINTIME asc))*24,1) as intervaltime from REPAIRAPPLICATION  where 1=1";
                if (!String.IsNullOrEmpty(deviceNo))
                    sql += " and DEVICENO='"+deviceNo+"'";
                if (!String.IsNullOrEmpty(startTime))
                    sql += " and CHECKTIME> to_date('" + startTime + "', 'yyyy-mm-dd hh24:mi:ss') ";
                if (!String.IsNullOrEmpty(endTime))
                    sql += " and CHECKTIME< to_date('" + endTime + "', 'yyyy-mm-dd hh24:mi:ss') ";
                resultList = Interval(sql);
            }
            return resultList;
        }
        public DataTable GetFailureRecords(string sql)
        {
            return GetDataTable(sql);
        }
        public DataTable GetFailureRecords(string deviceNo, string startTime, string endTime)
        {
            string sql1 = "select app.DeviceNo,app.FailureType,app.BeginTime,app.FirstLocation,app.SecondLocation,app.ThirdLocation, round((" +
                        "(case when app.CHECKTIME is null then to_date('" + endTime + "', 'yyyy-mm-dd hh24:mi:ss') else app.CHECKTIME end)-" +
                        "(case WHEN app.BEGINTIME > to_date('" + startTime + "', 'yyyy-mm-dd hh24:mi:ss') THEN app.BEGINTIME else to_date('" + startTime + "', 'yyyy-mm-dd hh24:mi:ss') end)" +
                        ") * 24, 2) AS pausetime " +
                        "FROM REPAIRAPPLICATION app " +
                        "WHERE (app.CHECKTIME > to_date('" + startTime + "', 'yyyy-mm-dd hh24:mi:ss') " +
                        "AND app.CHECKTIME < to_date('" + endTime + "', 'yyyy-mm-dd hh24:mi:ss') " +
                        "OR app.CHECKTIME is null) ";
            if (!String.IsNullOrEmpty(deviceNo))
            {
                sql1 += " and app.DeviceNo='" + deviceNo + "'";
            }
            sql1 += " order by app.begintime";
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




        public List<string> MachineType() 
        {
            List<string> res = new List<string>();
            using (var client=DbConfig.GetInstance())
            {
                res = client.SqlQuery<string>("select distinct type from device where type is not null").ToList();
            }
            return res;
        }
        public List<string> GetBrandsByType(string type) 
        {
            using (var client=DbConfig.GetInstance())
            {
                return client.SqlQuery<string>("select distinct brand from device where brand is not null and type='" + type + "'").ToList();
            }
        }
        public DataTable GetPauseTimeByBrand()
        {
            return null;
        }
    
    
    
    
    
    
    
    
    }
}
