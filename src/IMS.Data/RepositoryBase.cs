using Dapper;
using DapperExtensions;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Data
{
    public class RepositoryBase<T> : IDataRepository<T>
        where T : class
    {
        IDbConnection conn;
        IDbTransaction trans;
        public RepositoryBase()
        {
            conn = Utils.DataUtils.GetConn();
            conn.Open();
            trans = conn.BeginTransaction();
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="primaryId"></param>
        /// <returns></returns>
        public T GetById(dynamic primaryId)
        {
            return conn.Get<T>(primaryId as object);
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="primaryId"></param>
        /// <returns></returns>
        public TReturn GetById<TReturn>(dynamic primaryId) where TReturn : class
        {
            return conn.Get<TReturn>(primaryId as object);
        }

        /// <summary>
        /// 根据主键获取多个实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<T> GetByIds(IList<dynamic> ids)
        {
            var tblName = string.Format("dbo.{0}", typeof(T).Name);
            var idsin = string.Join(",", ids.ToArray<dynamic>());
            var sql = "SELECT * FROM @table WHERE Id in (@ids)";
            IEnumerable<T> dataList = conn.Query<T>(sql, new { table = tblName, ids = idsin });
            return dataList;
        }

        /// <summary>
        /// 根据主键获取多个实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<TReturn> GetByIds<TReturn>(IList<dynamic> ids) where TReturn : class
        {
            var tblName = string.Format("dbo.{0}", typeof(TReturn).Name);
            var idsin = string.Join(",", ids.ToArray<dynamic>());
            var sql = "SELECT * FROM @table WHERE Id in (@ids)";
            IEnumerable<TReturn> dataList = conn.Query<TReturn>(sql, new { table = tblName, ids = idsin });
            return dataList;
        }


        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            return conn.GetList<T>();
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <typeparam name="TReturn">实体类型</typeparam>
        /// <returns></returns>
        public IEnumerable<TReturn> GetAll<TReturn>() where TReturn : class
        {
            return conn.GetList<TReturn>();
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int Count(object predicate)
        {
            return conn.Count<T>(predicate);
        }

        /// <summary>
        /// 根据条件获取集合
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        public IEnumerable<T> GetList(object predicate = null, IList<ISort> sort = null, bool buffered = false)
        {
            return conn.GetList<T>(predicate, sort, null, null, buffered);
        }

        /// <summary>
        /// 根据条件获取集合
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        public IEnumerable<TReturn> GetList<TReturn>(object predicate = null, IList<ISort> sort = null, bool buffered = false) where TReturn : class
        {
            return conn.GetList<TReturn>(predicate, sort, null, null, buffered);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="allRowsCount"></param>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        public IEnumerable<T> GetPageList(int pageIndex, int pageSize, out long allRowsCount, object predicate = null, IList<ISort> sort = null, bool buffered = true)
        {
            allRowsCount = 0;
            IEnumerable<T> entityList = conn.GetPage<T>(predicate, sort, pageIndex, pageSize, null, null, buffered);
            allRowsCount = conn.Count<T>(predicate);
            return entityList;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="allRowsCount"></param>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        public IEnumerable<TReturn> GetPageList<TReturn>(int pageIndex, int pageSize, out long allRowsCount, object predicate = null, IList<ISort> sort = null, bool buffered = true) where TReturn : class
        {
            allRowsCount = 0;
            IEnumerable<TReturn> entityList = conn.GetPage<TReturn>(predicate, sort, pageIndex, pageSize, null, null, buffered);
            allRowsCount = conn.Count<TReturn>(predicate);
            return entityList;
        }

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public dynamic Insert(T entity)
        {
            return conn.Insert<T>(entity);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public bool InsertBatch(IEnumerable<T> entityList)
        {
            var isOk = false;
            try
            {
                foreach (var item in entityList)
                {
                    conn.Insert<T>(item,trans);
                }
                trans.Commit();
            }
            catch (Exception)
            {
                isOk = false;
                trans.Rollback();
            }
            return isOk;
        }

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            return conn.Update<T>(entity);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public bool UpdateBatch(IEnumerable<T> entityList)
        {
            var isOk = false;
            try
            {
                foreach (var item in entityList)
                {
                    conn.Update<T>(item, trans);
                }
                trans.Commit();
            }
            catch (Exception)
            {
                isOk = false;
                trans.Rollback();
            }
            return isOk;
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="primaryId"></param>
        /// <returns></returns>
        public bool Delete(dynamic primaryId)
        {
            return conn.Delete<T>(primaryId as object);
        }

        /// <summary>
        /// 根据条件批量删除
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool DeleteList(object predicate)
        {
            return conn.Delete<T>(predicate);
        }

        /// <summary>
        /// 根据主键批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteBatch(IEnumerable<dynamic> ids)
        {
            var isOk = false;
            try
            {
                foreach (var id in ids)
                {
                    conn.Delete<T>(id as object, trans);
                }
                trans.Commit();
            }
            catch (Exception)
            {
                isOk = false;
                trans.Rollback();
            }
            return isOk;
        }
    }
}
