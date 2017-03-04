
using IMS.Data.Services;
using IMS.Json;
using IMS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace IMS.Web.Controllers.Failure
{
    public class FailureController : Controller
    {
        //
        // GET: /Failure/

        private FailureService failureService = new FailureService();
        public ActionResult NewApplication()
        {
            return View();
        }

        public ActionResult AllApplications()
        {
            return View();
        }
     

        public void AddNewFailure()
        {
            MaintenanceApplicationViewModel MaintenanceApplicationVM = new MaintenanceApplicationViewModel();
            MaintenanceApplicationVM.DeviceNo = Request.Params["deviceNo"];
            MaintenanceApplicationVM.BeginTime = Convert.ToDateTime(Request.Params["beginTime"]);
            MaintenanceApplicationVM.FailureAppearance = Request.Params["failureAppearance"];
            MaintenanceApplicationVM.FailureDescription = Request.Params["failureDescription"];
            MaintenanceApplicationVM.FstLevFailureLocation = Request.Params["fstLevFailureLocation"];
            MaintenanceApplicationVM.SecLevFailureLocation = Request.Params["secLevFailureLocation"];
            MaintenanceApplicationVM.ThiLevFailureLocation = Request.Params["thiLevFailureLocation"];
            MaintenanceApplicationVM.ReporterId = "报告人A";
            MaintenanceApplicationVM.ReportTime = DateTime.Now;
            MaintenanceApplicationVM.Status = "审核中";
            MaintenanceApplicationVM.Modifiable = 0;
            failureService.AddNewFailure(MaintenanceApplicationVM);

        }

        public void UpDateFailureInfo() 
        {
            MaintenanceApplicationViewModel MaintenanceApplicationVM = new MaintenanceApplicationViewModel();
            int id = Convert.ToInt32(Request.Params["applicationId"]);
            MaintenanceApplicationVM.FailureAppearance = Request.Params["failureAppearance"];
            MaintenanceApplicationVM.FailureDescription = Request.Params["failureDescription"];
            MaintenanceApplicationVM.FstLevFailureLocation = Request.Params["fstLevFailureLocation"];
            MaintenanceApplicationVM.SecLevFailureLocation = Request.Params["secLevFailureLocation"];
            MaintenanceApplicationVM.ThiLevFailureLocation = Request.Params["thiLevFailureLocation"];
            MaintenanceApplicationVM.ReportTime = DateTime.Now;
            failureService.UpDateFailureInfo(id,MaintenanceApplicationVM);
        }

        public void DeleteFailure() 
        {
            int id = Convert.ToInt32(Request.Params["applicationId"]);
            failureService.DeleteFailureByID(id);
        }
      
       
        public ActionResult GetAllApplicationsByUserName(string name, int limit, int offset,string sectionName,string deviceNo,string beginTime,string endTime)
        {
            var AllApplicationsList = CommonService.GetAllApplicationsByName(name,limit,offset,sectionName,deviceNo,beginTime,endTime);
            if (AllApplicationsList != null)
            {
                var total = AllApplicationsList.Count;
                var rows = AllApplicationsList.Skip(offset).Take(limit).ToList();
                var d = new { total = total, rows = rows };
                return Content(d.ToJsonString());
            }
            else
                return Json("");
        }








        /// <summary>
        /// 测试功能的
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDepartment()
        {
            return View();
        }


        public JsonResult GetDepartment1()
        {
            var lstRes = new List<DeviceViewModel>();
            for (var i = 0; i < 50; i++)
            {
                var oModel = new DeviceViewModel();
                oModel.DeviceNo = i.ToString();
                oModel.DeviceName = "销售部" + i;
                lstRes.Add(oModel);
            }

            var total = lstRes.Count;
            var rows = lstRes;
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }


    }
}