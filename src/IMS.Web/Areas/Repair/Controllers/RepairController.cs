using AutoMapper;
using IMS.Data.DAL;
using IMS.Model.Entity;
using IMS.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using IMS.Json;
using System.Web.Mvc;
using IMS.Enume;
using IMS.Extensions;

namespace IMS.Web.Areas.Repair.Controllers
{
    public class RepairController : Controller
    {
        // GET: Repair
        RepairDAL repairDAL = new RepairDAL();
        public ActionResult AllApplications()
        {
            return View();
        }
        //[HttpPost]
        public ActionResult GetAllApplications(int limit, int offset, string sectionName, string deviceNo, string beginTime, string endTime, string ordername, string order)
        {
            //角色 role 从当前登录的用户信息里获取
            string role = string.Empty;
            string orderName = ordername == "DeviceShortName" ? "DeviceNo" : ordername;
            Expression<Func<RepairApplication, bool>> exp = item => !item.IsDeleted;
            if (!String.IsNullOrEmpty(deviceNo) && !String.Equals(deviceNo, "-1"))
            {
                exp = exp.And(g => g.DeviceNo == deviceNo);
            }
            if (!String.IsNullOrEmpty(beginTime))
            {
                DateTime _beginTime = Convert.ToDateTime(beginTime);
                exp = exp.And(g => g.BeginTime > _beginTime);
            }
            if (String.Equals(role, "Worker"))
            {
                exp = exp.And(g => g.Status == CurrentStatus.待审核.ToString())
                         .Or(g => g.Status == CurrentStatus.已驳回.ToString());
            }
            if (String.Equals(role, "Engineer"))
            {
                exp = exp.And(g => g.Status != CurrentStatus.待审核.ToString())
                         .And(g => g.Status != CurrentStatus.已驳回.ToString())
                         .And(g => g.Status != CurrentStatus.已总结.ToString());
            }
            if (!String.IsNullOrEmpty(endTime))
            {
                DateTime _endTime = Convert.ToDateTime(endTime);
                exp = exp.And(g => g.BeginTime < _endTime);
            }
            var applicationList = repairDAL.ApplicationsByRole(sectionName, orderName, order, exp); 
            List<ApplicationDto> ApplicationDtoList = Mapper.Map<List<RepairApplication>, List<ApplicationDto>>(applicationList);
            if (ApplicationDtoList != null)
            {
                var total = ApplicationDtoList.Count;
                var rows = ApplicationDtoList.Skip(offset).Take(limit).ToList();
                var result = new { total = total, rows = rows };
                return Content(result.ToJsonString());
            }
            else
                return Json("");
        }

        #region 操作工
        public ActionResult NewApplication()
        {
            return View();
        }
        [HttpPost]
        public void CreatApplication()
        {
            ApplicationDto applicationDto = new ApplicationDto();
            applicationDto.DeviceNo = Request.Params["deviceNo"];
            applicationDto.BeginTime = Convert.ToDateTime(Request.Params["beginTime"]);
            applicationDto.FailureAppearance = Request.Params["failureAppearance"];
            applicationDto.FailureDescription = Request.Params["failureDescription"];
            applicationDto.FirstLocation = Request.Params["fstLevFailureLocation"];
            applicationDto.SecondLocation = Request.Params["secLevFailureLocation"];
            applicationDto.ThirdLocation = Request.Params["thiLevFailureLocation"];
            applicationDto.FailureType = Request.Params["failureType"];
            applicationDto.ApplicantId = "报告人A";
            applicationDto.ApplicationTime = DateTime.Now;
            applicationDto.Status = "待审核";
            var application = Mapper.Map<ApplicationDto, RepairApplication>(applicationDto);
            repairDAL.InsertNewApplication(application);//这个地方应该有结果判定，但是这个页面不重要不做了
        }
        [HttpPost]
        public ActionResult ModifyApplication()
        {
            ApplicationDto repairApplicationVM = new ApplicationDto();
            int id = Convert.ToInt32(Request.Params["applicationId"]);
            string appearance = Request.Params["failureAppearance"];
            string description = Request.Params["failureDescription"];
            string firstLocation = Request.Params["fstLevFailureLocation"];
            string secondLocation = Request.Params["secLevFailureLocation"];
            string thirdLocation = Request.Params["thiLevFailureLocation"];
            string failureType = Request.Params["failureType"];
            bool result = repairDAL.UpdateApplication(id, appearance, description, firstLocation, secondLocation, thirdLocation, failureType);
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
            var result = repairDAL.DeleteApplicationById(applicationId);
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
            bool result = false;
            DispatchDto dispatchDto = new DispatchDto();
            dispatchDto.Instruction = instruction;
            dispatchDto.Engineer = engineerId;
            dispatchDto.Dispatcher = 9999;
            dispatchDto.CreatTime = DateTime.Now;
            Dispatch dispatch = null;
            DiagnoseDispatch diagnoseDispatch = null;
            if (String.Equals(workType, "维修"))
            {
                dispatch = Mapper.Map<DispatchDto, Dispatch>(dispatchDto);
                result = repairDAL.RepairDispatch(dispatch, applicationId);
            }
            if (String.Equals(workType, "故障诊断"))
            {
                diagnoseDispatch = Mapper.Map<DispatchDto, DiagnoseDispatch>(dispatchDto);
                result = repairDAL.DiagnoseDispatch(diagnoseDispatch, applicationId);
            }
            if (result)
            {
                return Content(new { msg = "成功", status = "success", phase = CurrentStatus.待维修.ToString() }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "失败", status = "failed" }.ToJsonString());
            }
        }
        [HttpPost]
        public ActionResult Reject(int applicationId, string rejectReason)
        {
            var result = repairDAL.UpdateApplication(applicationId, rejectReason);
            if (result)
            {
                var data = new { msg = "已经成功驳回", status = "success", phase = CurrentStatus.已驳回.ToString() };
                return Content(data.ToJsonString());
            }
            else
            {
                return Content(new { msg = "驳回失败", status = "failed" }.ToJsonString());
            }
        }

        [HttpPost]
        public ActionResult CheckSelfRepairPlanByMngr(int selfRepairPlanId, string type, string msg)
        {
            bool result = false;
            if (string.Equals(type, "approve"))
            {
                result = repairDAL.ApproveSelfRepairPlan(selfRepairPlanId, msg);
            }
            if (string.Equals(type, "reject"))
            {
                result = repairDAL.RejectSelfRepairPlan(selfRepairPlanId, msg);
            }
            if (result)
            {
                string _phase = string.Empty;
                if (string.Equals(type, "approve"))
                { _phase = CurrentStatus.自修方案通过.ToString(); }
                if (string.Equals(type, "reject"))
                { _phase = CurrentStatus.自修方案失败.ToString(); }
                return Content(new { msg = "处理成功", status = "success", phase = _phase }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "处理失败", status = "failed" }.ToJsonString());
            }
        }

        [HttpPost]
        public ActionResult CancelProcedure(string methodCategory, int categoryId)
        {
            bool result = repairDAL.CancelProcedure(methodCategory, categoryId);
            if (result)
            {
                return Content(new { msg = "处理成功", status = "success", phase = CurrentStatus.撤销成功.ToString() }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "处理失败", status = "failed" }.ToJsonString());
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
        public ActionResult CreatOrUpdateSelfRepairPlanByEngr(int appId, int selfRepairPlanId, string steps, string tools, double timecost, string isspare, string spareparts, string type)
        {
            bool result = false;
            SelfRepairPlanDto selfRepairPlanDto = new SelfRepairPlanDto();
            selfRepairPlanDto.Step = steps;
            selfRepairPlanDto.Tool = tools;
            selfRepairPlanDto.TimeCost = timecost;
            selfRepairPlanDto.IsUseSpareParts = isspare;
            selfRepairPlanDto.SparePartsInfo = spareparts;
            var selfRepairPlan = Mapper.Map<SelfRepairPlanDto, SelfRepairPlan>(selfRepairPlanDto);
            int selfPlanId = 0;
            if (string.Equals(type, "Create"))
            {
                result = repairDAL.InsertNewSelfRepairPlan(selfRepairPlan, appId, out selfPlanId);
            }
            if (string.Equals(type, "modify"))
            {
                result = repairDAL.UpdateSelfRepairPlan(selfRepairPlan, selfRepairPlanId);
            }
            if (result)
                return Content(new { msg = "成功", status = "success", phase = CurrentStatus.自修方案待审.ToString(), methodCategory = MethodCategory.自修.ToString(), SelfRepairPlanID = selfPlanId }.ToJsonString());
            else
                return Content(new { msg = "失败", status = "failed" }.ToJsonString());
        }

        [HttpPost]
        public ActionResult MarkCancelingProcedure(int appId)
        {
            bool result = repairDAL.UpDateApplication(appId);
            if (result)
            {
                return Content(new { msg = "处理成功", status = "success", phase = CurrentStatus.方案撤销中.ToString() }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "处理失败", status = "failed" }.ToJsonString());
            }
        }
        public ActionResult ShowAllInOne(int appId, string type)
        {
            SummarizeDto SummarizeVM = new SummarizeDto();
            if (String.Equals(type, "自修"))
            {
                Dispatch dispatchSheet = new Dispatch();
                SelfRepairPlan selfRepairPlan = new SelfRepairPlan();
                string dispatherName = string.Empty;
                string engineerName = string.Empty;
                RepairApplication application = repairDAL.AllInfo(appId, ref dispatchSheet, ref selfRepairPlan, ref dispatherName, ref engineerName);
                SummarizeVM.Instruction = dispatchSheet.Instruction;
                SummarizeVM.DispatchTime = dispatchSheet.CreatTime;
                SummarizeVM.Dispatcher = dispatherName;
                SummarizeVM.Engineer = engineerName;
                SummarizeVM.ApplicationDto = Mapper.Map<RepairApplication, ApplicationDto>(application);
                SummarizeVM.SelfRepairDto = Mapper.Map<SelfRepairPlan, SelfRepairPlanDto>(selfRepairPlan);
            }
            return View(SummarizeVM);
        }
        public ActionResult Summarize()
        {
            SummarizeDto summarizeDto = new SummarizeDto();
            summarizeDto.ApplicationDto = new ApplicationDto();
            summarizeDto.SelfRepairDto = new SelfRepairPlanDto();
            summarizeDto.ApplicationDto.Id = Convert.ToInt32(Request.Params["appID"]);
            string beginTime = Request.Params["beginTime"];
            if (!string.IsNullOrEmpty(beginTime))
            {
                summarizeDto.SelfRepairDto.StartTime = Convert.ToDateTime(beginTime);
            }
            summarizeDto.SelfRepairDto.ID = Convert.ToInt32(Request.Params["selfRepairPlanId"]);
            summarizeDto.SelfRepairDto.TimeCost = Convert.ToDouble(Request.Params["timeCost"]);
            summarizeDto.SelfRepairDto.Step = Request.Params["steps"];
            summarizeDto.SelfRepairDto.Tool = Request.Params["tools"];
            var partsInfo = Request.Params["partsInfo"];
            summarizeDto.SelfRepairDto.IsUseSpareParts = partsInfo == string.Empty ? "否" : "是";
            summarizeDto.SelfRepairDto.SparePartsInfo = partsInfo;
            summarizeDto.ApplicationDto.FailureDescription = Request.Params["description"];
            summarizeDto.ApplicationDto.FailureAppearance = Request.Params["appearance"];
            summarizeDto.ApplicationDto.FirstLocation = Request.Params["fstLocation"];
            summarizeDto.ApplicationDto.SecondLocation = Request.Params["secLocation"];
            summarizeDto.ApplicationDto.ThirdLocation = Request.Params["thiLocation"];
            var application = Mapper.Map<ApplicationDto, RepairApplication>(summarizeDto.ApplicationDto);
            var selfRepairPlan = Mapper.Map<SelfRepairPlanDto, SelfRepairPlan>(summarizeDto.SelfRepairDto);
            bool result = repairDAL.Finish(application, selfRepairPlan);
            if (result)
            {
                return Content(new { msg = "处理成功", status = "success" }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "处理失败", status = "failed" }.ToJsonString());
            }

        }

        [HttpPost]
        public ActionResult GetSelfRepairPlanByAppId(int selfRepairPlanId)
        {
            SelfRepairPlan resultM = repairDAL.SelfRepairPlanByAppId(selfRepairPlanId);
            SelfRepairPlanDto resultDto = Mapper.Map<SelfRepairPlan, SelfRepairPlanDto>(resultM);
            if (resultDto != null)
            {
                return Content(new { msg = "找到方案", status = "success", data = resultDto }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "未找到方案", status = "failed" }.ToJsonString());
            }
        }

        #endregion

        #region 测试功能的
          //public ActionResult GetDepartment()
        //{
        //    return View();
        //}
        //public JsonResult GetDepartment1()
        //{
        //    var lstRes = new List<DeviceViewModel>();
        //    for (var i = 0; i < 50; i++)
        //    {
        //        var oModel = new DeviceViewModel();
        //        oModel.DeviceNo = i.ToString();
        //        oModel.DeviceName = "设备" + i;
        //        lstRes.Add(oModel);
        //    }
        //    var total = lstRes.Count;
        //    var rows = lstRes;
        //    //return Json(new {rows = rows }, JsonRequestBehavior.AllowGet);
        //    return Json(lstRes, JsonRequestBehavior.AllowGet);

        //}
        //public ActionResult Index()
        //{
        //    return View();
        //}
        #endregion
      

    }
}