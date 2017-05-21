using IMS.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Data.DAL
{
    public class DataExchangeDAL
    {
        public bool InsertItem<T>(T t) where T : class
        {
            using (var client = DbConfig.GetInstance())
            {
                return true;// client.Insert<T>(t);
            }
        }
        public bool InsertRange<T>(List<T> entities) where T : class
        {
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    client.BeginTran();
                    client.CommandTimeOut = 30000;
                    client.ExecuteCommand("alter table  " + typeof(T).Name + " disable all triggers");
                    client.InsertRange<T>(entities);
                    client.CommitTran();
                    client.ExecuteCommand("alter table " + typeof(T).Name + " enable all triggers");

                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public bool UpdateRange<T>(List<T> entities) where T : class
        {
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    client.BeginTran();
                    client.CommandTimeOut = 30000;
                    client.SqlBulkReplace(entities);
                    client.CommitTran();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                    throw;
                }
            }
        }
        public bool UpdateApplicationRange(List<RepairApplication> entities)
        {
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    for (int i = 0; i < entities.Count; i++)
                    {
                        client.Update<RepairApplication>(entities[i], it => it.Id == entities[i].Id);
                    }
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public List<int> GetExistIdsByEntity<T>() where T : class
        {
            using (var client = DbConfig.GetInstance())
            {
                return client.SqlQuery<int>("select distinct id from " + typeof(T).Name + "").ToList();
            }
        }
    }
}
