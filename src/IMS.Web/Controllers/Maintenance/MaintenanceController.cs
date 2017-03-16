using IMS.Data.Services;
using IMS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMS.Json;

namespace IMS.Web.Controllers.Maintenance
{
    public class MaintenanceController : Controller
    {
        // GET: Maintenance

        MaintenanceService maintenanceService = new MaintenanceService();
        [HttpPost]
        public ActionResult GetAllApplicationsByUserName(string name, int limit, int offset, string sectionName, string deviceNo, string beginTime, string endTime, string ordername)
        {
            var AllApplicationsList = maintenanceService.GetAllApplicationsByName(name, sectionName, deviceNo, beginTime, endTime);
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
        #region 维修申请---操作工人
        public ActionResult NewApplication()
        {
            return View();
        }
        public ActionResult AllApplications()
        {
            return View();
        }
        [HttpPost]
        public void AddNewApplication()
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
            maintenanceService.AddNewFailure(MaintenanceApplicationVM);

        }
        [HttpPost]
        public ActionResult UpDateApplication()
        {
            MaintenanceApplicationViewModel MaintenanceApplicationVM = new MaintenanceApplicationViewModel();
            int id = Convert.ToInt32(Request.Params["applicationId"]);
            MaintenanceApplicationVM.FailureAppearance = Request.Params["failureAppearance"];
            MaintenanceApplicationVM.FailureDescription = Request.Params["failureDescription"];
            MaintenanceApplicationVM.FstLevFailureLocation = Request.Params["fstLevFailureLocation"];
            MaintenanceApplicationVM.SecLevFailureLocation = Request.Params["secLevFailureLocation"];
            MaintenanceApplicationVM.ThiLevFailureLocation = Request.Params["thiLevFailureLocation"];
            MaintenanceApplicationVM.ReportTime = DateTime.Now;
            bool result = maintenanceService.UpDateApplication(id, MaintenanceApplicationVM);
            if (result)
            {
                return Content(new { msg = "成功", status = "success" }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "失败", status = "failed" }.ToJsonString());
            }

        }
        [HttpPost]
        public ActionResult DeleteApplication()
        {
            int id = Convert.ToInt32(Request.Params["applicationId"]);
            var result = maintenanceService.DeleteApplicationById(id);
            if (result)
            {
                return Content(new { msg = "删除成功", status = "success" }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "删除失败", status = "failed" }.ToJsonString());
            }
        }
        #endregion



        #region 维修申请处理---管理员
        public ActionResult Dispatch()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Dispatch(string applicationId, string workType, string engineerId, string instruction)
        {
            FailureProcessViewModel failureProcessVM = new FailureProcessViewModel();
            failureProcessVM.Instruction = instruction;
            failureProcessVM.MaintenanceApplicationViewModel.Id = Convert.ToInt32(applicationId);
            failureProcessVM.EngineerViewModel.EngineerId = Convert.ToInt32(engineerId);
            bool result = maintenanceService.Dispatch(failureProcessVM, workType);
            if (result)
            {
                return Json("ok");
            }
            else
            {
                return Json("");
            }

        }
        [HttpPost]
        public ActionResult Reject(string applicationId, string rejectReason)
        {
            var result = maintenanceService.Reject(applicationId, rejectReason);
            if (result)
            {
                var data = new { msg = "已经成功驳回", status = "success" };
                return Content(data.ToJsonString());
            }
            else
            {
                return Content(new { msg = "驳回失败", status = "failed" }.ToJsonString());
            }
        }
        #endregion



        #region 维修申请处理---维修工程师
        public ActionResult Disposing()
        {
            return View();
        }

        public ActionResult NewRepairPlan()
        {
            return View();
        } 
        #endregion

















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
                oModel.DeviceName = Guid.NewGuid().ToString();
                lstRes.Add(oModel);
            }

            var total = lstRes.Count;
            var rows = lstRes;
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }

    }
}