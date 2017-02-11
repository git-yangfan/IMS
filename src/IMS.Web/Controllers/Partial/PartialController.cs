using IMS.Data;
using IMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers.Partial
{
    public class PartialController : Controller
    {
        //
        // GET: /Partial/
        private SubSystemRepository subSystemRepository = new SubSystemRepository();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SubSystem() 
        {
            return PartialView();
        }
        public ActionResult GetSubSystemList(int lev, int parentId) 
        {
            subSystemRepository.GetList(new { Lev = 0 });
            IEnumerable<SubSystem> subSystemList = subSystemRepository.GetSubSystemList(lev,parentId);
            if (subSystemList!=null)
            {
                return Json(subSystemList);
            }
            return Json(subSystemList);
        }
	}
}