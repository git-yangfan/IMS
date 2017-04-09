using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;using System.ComponentModel;

namespace IMS.Web.Dto
{
    public class BaseDto
    {
        public BaseDto()
        {
            IsDeleted = false;
        }
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

      
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}