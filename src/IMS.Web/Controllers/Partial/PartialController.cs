using IMS.Data.Services;
using IMS.Model.Model;
using System.Collections.Generic;
using System.Web.Mvc;

namespace IMS.Web.Controllers.Partial
{
    public class PartialController : Controller
    {
        //
        // GET: /Partial/

        FailureService failureService=new FailureService();
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
            if (lev != -1)
            {
                IEnumerable<SubSystem> subSystemList = failureService.GetSubSystemList(lev, parentId);
                if (subSystemList != null)
                {
                    return Json(subSystemList);
                }
            }
            return Json("");
        }
    }
}