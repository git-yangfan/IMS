using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Data
{
    public interface IDataRepository<T>
        where T : class
    {
        /// <summary>
        /// 根据主键获得实体对象
        /// </summary>
        /// <param name="primaryId"></param>
        /// <returns></returns>
        T GetById(dynamic primaryId);


        /// <summary>
        /// 根据主键获得实体对象
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="primaryId"></param>
        /// <returns></returns>
        TReturn GetById<TReturn>(dynamic primaryId) where TReturn : class;


        /// <summary>
        /// 根据主键列表 获得实体对象集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<T> GetByIds(IList<dynamic> ids);

        /// <summary>
        /// 根据主键列表 获得实体对象集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<TReturn> GetByIds<TReturn>(IList<dynamic> ids) where TReturn : class;

        /// <summary>
        /// 活动所有数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// 活动所有数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<TReturn> GetAll<TReturn>() where TReturn : class;

        /// <summary>
        /// 返回数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        int Count(object predicate);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        IEnumerable<T> GetList(object predicate = null, IList<ISort> sort = null, bool buffered = false);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        IEnumerable<TReturn> GetList<TReturn>(object predicate = null, IList<ISort> sort = null, bool buffered = false) where TReturn : class;

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="allRowsCount"></param>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        IEnumerable<T> GetPageList(int pageIndex, int pageSize, out long allRowsCount, object predicate = null, IList<ISort> sort = null, bool buffered = true);


        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="allRowsCount"></param>
        /// <param name="predicate"></param>
        /// <param name="sort"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        IEnumerable<TReturn> GetPageList<TReturn>(int pageIndex, int pageSize, out long allRowsCount, object predicate = null, IList<ISort> sort = null, bool buffered = true) where TReturn : class;


        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        dynamic Insert(T entity);

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool InsertBatch(IEnumerable<T> entityList);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Update(T entity);

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool UpdateBatch(IEnumerable<T> entityList);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="primaryId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Delete(dynamic primaryId);

        /// <summary>
        ///根据条件 批量删除数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool DeleteList(object predicate);

        /// <summary>
        /// 根据主键列表 批量删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool DeleteBatch(IEnumerable<dynamic> ids);
    }
}
