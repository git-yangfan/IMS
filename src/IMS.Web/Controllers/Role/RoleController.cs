using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers.Role
{
    public class RoleController : Controller
    {
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetRole(int limit, int offset, string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                var oJson = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto_Role>(filter);
            }
            var lstRole = new List<Dto_Role>();
            for (var i = 0; i < 20; i++)
            {
                var oModel = new Dto_Role();
                oModel.ROLE_ID = Guid.NewGuid().ToString();
                oModel.ROLE_NAME = "模块管理员" + i;
                oModel.DESCRIPTION = "某一个模块的管理员" + i;
                oModel.CREATETIME = DateTime.Now.ToString();
                oModel.MODIFYTIME = DateTime.Now.ToString();
                oModel.ROLE_DEFAULTURL = "/Home/Index";
                lstRole.Add(oModel);
            }
            var total = lstRole.Count;
            var rows = lstRole.Skip(offset).Take(limit).ToList();
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get(int limit, int offset)
        {
            var lstRole = new List<Dto_Role>();
            for (var i = 0; i < 20; i++)
            {
                var oModel = new Dto_Role();
                oModel.ROLE_ID = Guid.NewGuid().ToString();
                oModel.ROLE_NAME = "模块管理员" + i;
                oModel.DESCRIPTION = "某一个模块的管理员" + i;
                oModel.CREATETIME = DateTime.Now.ToString();
                oModel.MODIFYTIME = DateTime.Now.ToString();
                oModel.ROLE_DEFAULTURL = "/Home/Index";
                lstRole.Add(oModel);
            }

            return Json(lstRole, JsonRequestBehavior.AllowGet);
        }
    }


    public class Dto_Role
    {
        /// <summary>
        /// 角色ID
        /// </summary>
       
        public string ROLE_ID { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
      
        public string ROLE_NAME { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>  [DataMember]
        public string DESCRIPTION { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        
        public string CREATETIME { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>  [DataMember]
        public string MODIFYTIME { get; set; }
        /// <summary>
        /// 默认页面
        /// </summary>
       
        public string ROLE_DEFAULTURL { get; set; }

       
        public string ROLE_DEFAULTURL_WEB { get; set; }
    }
}