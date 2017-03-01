
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
        public ActionResult FailureReport()
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
            MaintenanceApplicationVM.DeviceNo = Request.Params["sbbh"];
            MaintenanceApplicationVM.BeginTime = Convert.ToDateTime(Request.Params["fssj"]);
            MaintenanceApplicationVM.FailureAppearance = Request.Params["gzxianxiang"];
            MaintenanceApplicationVM.FailureDescription = Request.Params["gzms"];
            MaintenanceApplicationVM.FstLevFailureLocation = Request.Params["gzbwa"];
            MaintenanceApplicationVM.SecLevFailureLocation = Request.Params["gzbwb"];
            MaintenanceApplicationVM.ThiLevFailureLocation = Request.Params["gzbwc"];
            MaintenanceApplicationVM.ReporterId = "报告人A";
            MaintenanceApplicationVM.ReportTime = DateTime.Now;
            MaintenanceApplicationVM.State = "审核中";
            MaintenanceApplicationVM.Modifiable = 0;
            failureService.AddNewFailure(MaintenanceApplicationVM);

        }

        public void UpDateFailureInfo() 
        {
            MaintenanceApplicationViewModel MaintenanceApplicationVM = new MaintenanceApplicationViewModel();
            int id = Convert.ToInt32(Request.Params["gzshenqingid"]);
            MaintenanceApplicationVM.BeginTime = Convert.ToDateTime(Request.Params["fssj"]);
            MaintenanceApplicationVM.FailureAppearance = Request.Params["gzxianxiang"];
            MaintenanceApplicationVM.FailureDescription = Request.Params["gzms"];
            MaintenanceApplicationVM.FstLevFailureLocation = Request.Params["gzbwa"];
            MaintenanceApplicationVM.SecLevFailureLocation = Request.Params["gzbwb"];
            MaintenanceApplicationVM.ThiLevFailureLocation = Request.Params["gzbwc"];
            MaintenanceApplicationVM.ReportTime = DateTime.Now;
            failureService.UpDateFailureInfo(id,MaintenanceApplicationVM);
        }

        public void DeleteFailure() 
        {
            int id = Convert.ToInt32(Request.Params["gzshenqingid"]);
            failureService.DeleteFailureByID(id);
        }
      
       
        public ActionResult GetAllApplicationsByUserName(string name, int limit, int offset,string sbbh,string fssj)
        {
            var AllApplicationsList = failureService.GetAllApplicationsByName(name,limit,offset,sbbh,fssj);
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