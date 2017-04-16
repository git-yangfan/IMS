using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Dto
{
    public class EmployeeDto
    {
        /// <summary>
        /// 工号
        /// </summary>
        public string Empl_No { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Empl_Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role_Name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Add_TIME { get; set;}

        /// <summary>
        /// 是否删除
        /// </summary>
        public string IS_Deleted { get; set; }
    }
}