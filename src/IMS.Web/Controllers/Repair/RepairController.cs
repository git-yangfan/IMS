using IMS.Data.Services;
using IMS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMS.Json;

namespace IMS.Web.Controllers.Repair
{
    public class RepairController : Controller
    {
        // GET: Repair




        RepairService RepairService = new RepairService();
        [HttpPost]
        public ActionResult GetAllApplications(int limit, int offset, string sectionName, string deviceNo, string beginTime, string endTime, string ordername)
        {
            //角色 role 从当前登录的用户信息里获取
            var AllApplicationsList = RepairService.GetAllApplicationsByRole(RepairService.Role.Manager, sectionName, deviceNo, beginTime, endTime);
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
            ApplicationsViewModel repairApplicationVM = new ApplicationsViewModel();
            repairApplicationVM.DeviceNo = Request.Params["deviceNo"];
            repairApplicationVM.BeginTime = Convert.ToDateTime(Request.Params["beginTime"]);
            repairApplicationVM.FailureAppearance = Request.Params["failureAppearance"];
            repairApplicationVM.FailureDescription = Request.Params["failureDescription"];
            repairApplicationVM.FstLevFailureLocation = Request.Params["fstLevFailureLocation"];
            repairApplicationVM.SecLevFailureLocation = Request.Params["secLevFailureLocation"];
            repairApplicationVM.ThiLevFailureLocation = Request.Params["thiLevFailureLocation"];
            repairApplicationVM.ApplicantId = "报告人A";
            repairApplicationVM.ApplicationTime = DateTime.Now;
            repairApplicationVM.Status = "待审核";
            repairApplicationVM.Modifiable = 0;
            RepairService.AddNewFailure(repairApplicationVM);
        }
        [HttpPost]
        public ActionResult UpDateApplication()
        {
            ApplicationsViewModel repairApplicationVM = new ApplicationsViewModel();
            repairApplicationVM.Id = Convert.ToInt32(Request.Params["applicationId"]);
            repairApplicationVM.FailureAppearance = Request.Params["failureAppearance"];
            repairApplicationVM.FailureDescription = Request.Params["failureDescription"];
            repairApplicationVM.FstLevFailureLocation = Request.Params["fstLevFailureLocation"];
            repairApplicationVM.SecLevFailureLocation = Request.Params["secLevFailureLocation"];
            repairApplicationVM.ThiLevFailureLocation = Request.Params["thiLevFailureLocation"];
            repairApplicationVM.ApplicationTime = DateTime.Now;
            bool result = RepairService.UpDateApplication(repairApplicationVM);
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
        public ActionResult DeleteApplication(int applicationId)
        {
            var result = RepairService.DeleteApplicationById(applicationId);
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
        public ActionResult Dispatch(int applicationId, string workType, int engineerId, string instruction)
        {
            DispatchSheetViewModel dispatchSheetVM = new DispatchSheetViewModel();
            dispatchSheetVM.Instruction = instruction;
            dispatchSheetVM.EngineerId = engineerId;
            dispatchSheetVM.DispatcherId = 9999;
            dispatchSheetVM.DispatchTime = DateTime.Now;
            bool result = RepairService.Dispatch(dispatchSheetVM, workType, applicationId);
            if (result)
            {
                return Content(new { msg = "成功", status = "success", phase = RepairService.StatusDic["Repairing"] }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "失败", status = "failed" }.ToJsonString());
            }

        }
        [HttpPost]
        public ActionResult Reject(int applicationId, string rejectReason)
        {
            var result = RepairService.Reject(applicationId, rejectReason);
            if (result)
            {
                var data = new { msg = "已经成功驳回", status = "success", phase = RepairService.StatusDic["Reject"] };
                return Content(data.ToJsonString());
            }
            else
            {
                return Content(new { msg = "驳回失败", status = "failed" }.ToJsonString());
            }
        }

        [HttpPost]
        public ActionResult UpdateSelfRepairPlanByMngr(int selfRepairPlanID, string type, string msg)
        {
            bool result = RepairService.UpdateSelfRepairPlan(selfRepairPlanID, type, msg);
            if (result)
            {
                return Content(new { msg = "处理成功", status = "success", phase = RepairService.StatusDic["SelfRepairPass"] }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "处理失败", status = "failed", phase = RepairService.StatusDic["SelfRepairFail"] }.ToJsonString());
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
        public ActionResult CreatOrUpdateSelfRepairPlanByEngr(int appId, int selfRepairPlanID, string steps, string tools, int timecost, string isspare, string spareparts, string type)
        {
            bool result = false;
            SelfRepairPlanViewModel selfRepairPlanVM = new SelfRepairPlanViewModel();
            selfRepairPlanVM.Steps = steps;
            selfRepairPlanVM.Tools = tools;
            selfRepairPlanVM.TimeCost = timecost;
            selfRepairPlanVM.IsUseSpareParts = isspare;
            selfRepairPlanVM.SparePartsInfo = spareparts;
            if (string.Equals(type, "Create"))
            {
                result = RepairService.CreatNewSelfRepairPlan(selfRepairPlanVM, appId);
            }
            if (string.Equals(type, "modify"))
            {
                result = RepairService.UpdateSelfRepairPlan(selfRepairPlanVM, selfRepairPlanID);
            }
            if (result)
                return Content(new { msg = "成功", status = "success", phase = RepairService.StatusDic["SelfRepairChecking"] }.ToJsonString());
            else
                return Content(new { msg = "失败", status = "failed" }.ToJsonString());
        }

        [HttpPost]
        public ActionResult MarkProcedure(int appId)
        {
            bool result = RepairService.UpDateApplication(appId);
            if (result)
            {
                return Content(new { msg = "处理成功", status = "success", phase = RepairService.StatusDic["Canceling"] }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "处理失败", status = "failed" }.ToJsonString());
            }
        }
        [HttpPost]
        public ActionResult CancelProcedure(string procedure,int id) 
        {
            bool result = RepairService.UpDateApplication(procedure, id);
            if (result)
            {
                return Content(new { msg = "处理成功", status = "success", phase = RepairService.StatusDic["CancelOK"] }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "处理失败", status = "failed" }.ToJsonString());
            }
        }

        #endregion














        [HttpPost]
        public ActionResult GetSelfRepairPlanByAppId(int selfRepairPlanID)
        {
            SelfRepairPlanViewModel ResultVM = RepairService.SelfRepairPlanByAppId(selfRepairPlanID);
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