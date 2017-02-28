using IMS.Data.Services;
using IMS.Model.Model;
using IMS.Model.ViewModel;
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
        public ActionResult EngineerSelect() 
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

     
        public ActionResult GetName(string type,string teamName) 
        {
            List<EngineerViewModel> engineerVMList = CommonService.GetName(type,teamName);
            if (engineerVMList != null)
            {
                return Json(engineerVMList);
            }
            else
                return Json("");
            
        }
    }
}