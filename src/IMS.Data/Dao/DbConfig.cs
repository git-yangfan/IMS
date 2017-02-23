using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleSugar;

namespace IMS.Data
{
    public class DbConfig
    {
        private DbConfig()
        {

        }

        private static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["IMS_Oracle"].ConnectionString; }
        }

        public static SqlSugarClient GetInstance()
        {
            var db = new SqlSugarClient(ConnectionString);
            return db;
        }

    }
}
