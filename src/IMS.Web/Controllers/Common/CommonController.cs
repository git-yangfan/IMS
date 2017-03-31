using IMS.Data.Services;
using IMS.Model.Model;
using IMS.Model.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace IMS.Web.Controllers.Partial
{
    public class CommonController : Controller
    {
        //
        // GET: /Partial/
      
        public ActionResult SubSystem()
        {
            return PartialView();
        }
        public ActionResult EngineerSelect()
        {
            return PartialView();
        }

        public ActionResult SearchBar() 
        {
            return PartialView();
        }
        public JsonResult GetSubSystemList(int lev, int parentId)
        {
            if (lev != -1)
            {
                IEnumerable<SubSystem> subSystemList = CommonService.GetSubSystemList(lev, parentId);
                if (subSystemList != null)
                {
                    return Json(subSystemList);
                }
            }
            return Json("");
        }
        public ActionResult GetTeamOrEngrName(string type, string teamName)
        {
            List<EngineerViewModel> engineerVMList = CommonService.GetTeamOrEngrName(type, teamName);
            if (engineerVMList != null)
            {
                return Json(engineerVMList);
            }
            else
                return Json("");

        }

        public JsonResult GetWorkSectionNameList() 
        {
            List<DeviceViewModel> deviceVMList =CommonService.GetWorkSectionNameList();
            if (deviceVMList != null)
            {
                return Json(deviceVMList);
            }
            else
            {
                return Json("");
            }
        }

        public JsonResult GetDeviceNamesBySection(string sectionName) 
        {
            List<DeviceViewModel> deviceVMList = CommonService.GetDeviceNamesBySection(sectionName);
            if (deviceVMList != null)
            {
                return Json(deviceVMList);
            }
            else
            {
                return Json("");
            }
        }

       
    }
}