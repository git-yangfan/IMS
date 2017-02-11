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

        SubSystemRepository subSystemRepository;
        public ActionResult Index()
        {
            return View();

        }
        
        public ActionResult SubSystem()
        {
            return PartialView();
        }
        public JsonResult GetSubSystemList(int lev, int parentId)
        {
            subSystemRepository = new SubSystemRepository();           
            IEnumerable<SubSystem> subSystemList = subSystemRepository.GetSubSystemList(lev, parentId);
            if (subSystemList != null)
            {
                return Json(subSystemList);
            }
            return Json(subSystemList);
        }
    }
}