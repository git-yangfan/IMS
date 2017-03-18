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
            MaintenanceApplicationVM.Status = "待审核";
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
        public ActionResult Dispatch(string applicationId, string workType, string engineerId, string instruction)
        {
            ApplicationProcessViewModel applicationProcessVM = new ApplicationProcessViewModel();
            applicationProcessVM.Instruction = instruction;
            applicationProcessVM.MaintenanceApplicationViewModel.Id = Convert.ToInt32(applicationId);
            applicationProcessVM.EngineerViewModel.EngineerId = Convert.ToInt32(engineerId);
            bool result = maintenanceService.Dispatch(applicationProcessVM, workType);
            if (result)
            {
                return Content(new { msg = "成功", status = "success", phase = MaintenanceService.StatusDic["Repairing"] }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "失败", status = "failed" }.ToJsonString());
            }

        }
        [HttpPost]
        public ActionResult Reject(string applicationId, string rejectReason)
        {
            var result = maintenanceService.Reject(applicationId, rejectReason);
            if (result)
            {
                var data = new { msg = "已经成功驳回", status = "success", phase = MaintenanceService.StatusDic["Reject"] };
                return Content(data.ToJsonString());
            }
            else
            {
                return Content(new { msg = "驳回失败", status = "failed" }.ToJsonString());
            }
        }

        [HttpPost]
        public ActionResult UpdateSelfRepairPlanById(int planId, int appId, string type, string msg)
        {
            bool result = maintenanceService.UpdateSelfRepairPlanByAdmin(planId, appId, type, msg);
            if (result)
            {
                return Content(new { msg = "处理成功", status = "success", phase = MaintenanceService.StatusDic["SelfRepairPass"] }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "处理失败", status = "failed", phase = MaintenanceService.StatusDic["SelfRepairReject"] }.ToJsonString());
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
        [HttpPost]
        public ActionResult CreatOrModifyRepairPlan(int appId, string steps, string tools, int timecost, string isspare, string spareparts, string type)
        {
            bool result = false;
            SelfRepairPlanViewModel selfRepairPlanVM = new SelfRepairPlanViewModel();
            selfRepairPlanVM.RepairAppId = appId;
            selfRepairPlanVM.Steps = steps;
            selfRepairPlanVM.Tools = tools;
            selfRepairPlanVM.TimeCost = timecost;
            selfRepairPlanVM.IsUseSpareParts = isspare;
            selfRepairPlanVM.SparePartsInfo = spareparts;
            if (string.Equals(type, "Create"))
            {
                result = maintenanceService.InsertNewSelfRepairPlan(selfRepairPlanVM);
            }
            if (string.Equals(type, "modify"))
            {
                result = maintenanceService.UpdateSelfRepairPlanByEngr(selfRepairPlanVM, appId);
            }
            if (result)
                return Content(new { msg = "成功", status = "success", phase = MaintenanceService.StatusDic["SelfRepairChecking"] }.ToJsonString());
            else
                return Content(new { msg = "失败", status = "failed" }.ToJsonString());
        }
        #endregion














        [HttpPost]
        public ActionResult GetSelfRepairPlanByAppId(int appId)
        {
            SelfRepairPlanViewModel ResultVM = maintenanceService.SelfRepairPlanByAppId(appId);
            if (ResultVM != null)
            {
                return Content(new { msg = "找到方案", status = "success", data = ResultVM }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "未找到方案", status = "failed" }.ToJsonString());
            }
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
                oModel.DeviceName = Guid.NewGuid().ToString();
                lstRes.Add(oModel);
            }

            var total = lstRes.Count;
            var rows = lstRes;
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }

    }
}