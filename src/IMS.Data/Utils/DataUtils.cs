using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Data.Utils
{
    public class DataUtils
    {
        public static IDbConnection GetConn()
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["IMS_Oracle"];
            string strConn = connectionStringSettings.ConnectionString;
            string providerName = connectionStringSettings.ProviderName;


            if (string.IsNullOrEmpty(providerName))
            {
                throw new Exception("IMS_Oralce连接字符串未定义 ProviderName");
            }
            DataBaseType dbType = GetDBTypeByConnKey(providerName);
            switch (dbType)
            {
                case DataBaseType.SqlServer:
                    return new System.Data.SqlClient.SqlConnection(strConn);
                case DataBaseType.Oracle:
                    return new Oracle.ManagedDataAccess.Client.OracleConnection(strConn);
                case DataBaseType.MySql:
                    return null;
                case DataBaseType.Sqlite:
                    return null;
                case DataBaseType.Aceess:
                    return null;
                default:
                    return new System.Data.SqlClient.SqlConnection(strConn);
            }
        }

        public static DataBaseType GetDBTypeByConnKey(string providerName)
        {
            DataBaseType dbType = DataBaseType.SqlServer;
            if (providerName == "System.Data.SqlClient")
            {
                dbType = DataBaseType.SqlServer;
            }
            else if (providerName == "Oracle.ManagedDataAccess.Client")
            {
                dbType = DataBaseType.Oracle;
            }
            else if (providerName == "System.Data.OracleClient")
            {
                dbType = DataBaseType.Oracle;
            }
            else if (providerName == "MySql.Data.MySqlClient")
            {
                dbType = DataBaseType.MySql;
            }
            else if (providerName == "System.Data.OleDb")
            {
                dbType = DataBaseType.Aceess;
            }
            else
            {
                throw new Exception("连接字符串未识别 ProviderName");
            }
            return dbType;
        }
    }

    public enum DataBaseType
    {
        SqlServer,
        Oracle,
        MySql,
        Sqlite,
        Aceess
    }
}
