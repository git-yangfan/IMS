using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleSugar;
using IMS.Model.Entity;
using System.Linq.Expressions;

namespace IMS.Data.DAL
{
    /// <summary>
    /// 员工信息
    /// </summary>
    public class EmployeeDAL
    {
        SqlSugarClient client;
        public EmployeeDAL()
        {
            client = DbConfig.GetInstance();
        }

        /// <summary>
        /// 根据条件查询员工信息
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public List<Employee> GetAllEmpl(Expression<Func<Employee, bool>> exp)
        {
            return client.Queryable<Employee>().Where(exp).ToList();
        }

        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool AddEmpl(Employee e)
        {
            return (bool)client.Insert<Employee>(e);
        }

        /// <summary>
        /// 修改员工
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool UpdateEmpl(Employee e)
        {
            return client.Update<Employee>(e);
        }

        /// <summary>
        /// 查询工号是否已存在
        /// </summary>
        /// <param name="empl_No"></param>
        /// <returns></returns>
        public bool EmployeeNoExists(string empl_No)
        {
            return client.Queryable<Employee>().Any(e => e.Empl_No == empl_No);
        }
    }
}
