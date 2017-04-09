using AutoMapper;
using IMS.Data.DAL;
using IMS.Data.Services;
using IMS.Model.Entity;
using IMS.Web.Dic;
using IMS.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using IMS.Json;
using System.Web.Mvc;

namespace IMS.Web.Areas.Repair.Controllers
{
    public class RepairController : Controller
    {
        // GET: Repair
        RepairDAL RepairService = new RepairDAL();
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
            applicationDto.ApplicantId = "报告人A";
            applicationDto.ApplicationTime = DateTime.Now;
            applicationDto.Status = "待审核";
            var application = Mapper.Map<ApplicationDto, RepairApplication>(applicationDto);
            RepairService.InsertNewApplication(application);//这个地方应该有结果判定，但是这个页面不重要不做了
        }
        public ActionResult AllApplications()
        {
            return View();
        }
        //[HttpPost]
        public ActionResult GetAllApplications(int limit, int offset, string sectionName, string deviceNo, string beginTime, string endTime, string ordername, string order)
        {
            //角色 role 从当前登录的用户信息里获取
            string _sortName = string.Empty;
            if (!string.IsNullOrEmpty(ordername))
            {
                _sortName = CommonDic.AppSortDic[ordername];
            }

            var applicationList = RepairService.ApplicationsByRole(Role.Manager.ToString(), sectionName, deviceNo, beginTime, endTime, ordername, order, limit, offset);
            List<ApplicationDto> ApplicationDtoList =Mapper.Map<List<RepairApplication>,List<ApplicationDto>>(applicationList);
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
            bool result = RepairService.UpdateApplication(id, appearance, description, firstLocation, secondLocation, thirdLocation);
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




        //#region 维修申请处理---管理员
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
                dispatch = Mapper.Map<DispatchDto,Dispatch>(dispatchDto);
                result = RepairService.RepairDispatch(dispatch, applicationId);
            }
            if (String.Equals(workType, "故障诊断"))
            {
                diagnoseDispatch = Mapper.Map<DispatchDto,DiagnoseDispatch>(dispatchDto);
                result = RepairService.DiagnoseDispatch(diagnoseDispatch, applicationId);
            }
            if (result)
            {
                return Content(new { msg = "成功", status = "success", phase = RepairDAL.StatusDic["Repairing"] }.ToJsonString());
            }
            else
            {
                return Content(new { msg = "失败", status = "failed" }.ToJsonString());
            }
        }
        //[HttpPost]
        //public ActionResult Reject(int applicationId, string rejectReason)
        //{
        //    var result = RepairService.UpdateApplication(applicationId, rejectReason);
        //    if (result)
        //    {
        //        var data = new { msg = "已经成功驳回", status = "success", phase = RepairService.StatusDic["Reject"] };
        //        return Content(data.ToJsonString());
        //    }
        //    else
        //    {
        //        return Content(new { msg = "驳回失败", status = "failed" }.ToJsonString());
        //    }
        //}

        //[HttpPost]
        //public ActionResult CheckSelfRepairPlanByMngr(int selfRepairPlanID, string type, string msg)
        //{
        //    bool result = false;
        //    if (string.Equals(type, "approve"))
        //    {
        //        result = RepairService.ApproveSelfRepairPlan(selfRepairPlanID, msg);
        //    }
        //    if (string.Equals(type, "reject"))
        //    {
        //        result = RepairService.RejectSelfRepairPlan(selfRepairPlanID, msg);
        //    }
        //    if (result)
        //    {
        //        string _phase = string.Empty;
        //        if (string.Equals(type, "approve"))
        //        { _phase = RepairService.StatusDic["SelfRepairPass"]; }
        //        if (string.Equals(type, "reject"))
        //        { _phase = RepairService.StatusDic["SelfRepairFail"]; }
        //        return Content(new { msg = "处理成功", status = "success", phase = _phase }.ToJsonString());
        //    }
        //    else
        //    {
        //        return Content(new { msg = "处理失败", status = "failed" }.ToJsonString());
        //    }
        //}





        //#endregion



        //#region 维修申请处理---维修工程师
        //public ActionResult Disposing()
        //{
        //    return View();
        //}

        //public ActionResult NewRepairPlan()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult CreatOrUpdateSelfRepairPlanByEngr(int appId, int selfRepairPlanID, string steps, string tools, double timecost, string isspare, string spareparts, string type)
        //{
        //    bool result = false;
        //    SelfRepairPlanDto selfRepairPlanVM = new SelfRepairPlanDto();
        //    selfRepairPlanVM.Steps = steps;
        //    selfRepairPlanVM.Tools = tools;
        //    selfRepairPlanVM.TimeCost = timecost;
        //    selfRepairPlanVM.IsUseSpareParts = isspare;
        //    selfRepairPlanVM.SparePartsInfo = spareparts;
        //    ZXFA selfRepairPlanM = new ZXFA(selfRepairPlanVM);
        //    if (string.Equals(type, "Create"))
        //    {
        //        result = RepairService.InsertNewSelfRepairPlan(selfRepairPlanM, appId);
        //    }
        //    if (string.Equals(type, "modify"))
        //    {
        //        result = RepairService.UpdateSelfRepairPlan(selfRepairPlanM, selfRepairPlanID);
        //    }
        //    if (result)
        //        return Content(new { msg = "成功", status = "success", phase = RepairService.StatusDic["SelfRepairChecking"], methodCategory = RepairService.MethodCategoryDic["Self"] }.ToJsonString());
        //    else
        //        return Content(new { msg = "失败", status = "failed" }.ToJsonString());
        //}

        //[HttpPost]
        //public ActionResult MarkCancelingProcedure(int appId)
        //{
        //    bool result = RepairService.UpDateApplication(appId);
        //    if (result)
        //    {
        //        return Content(new { msg = "处理成功", status = "success", phase = RepairService.StatusDic["Canceling"] }.ToJsonString());
        //    }
        //    else
        //    {
        //        return Content(new { msg = "处理失败", status = "failed" }.ToJsonString());
        //    }
        //}
        //[HttpPost]
        //public ActionResult CancelProcedure(string methodCategory, int categoryId)
        //{
        //    bool result = RepairService.CancelProcedure(methodCategory, categoryId);
        //    if (result)
        //    {
        //        return Content(new { msg = "处理成功", status = "success", phase = RepairService.StatusDic["CancelOK"] }.ToJsonString());
        //    }
        //    else
        //    {
        //        return Content(new { msg = "处理失败", status = "failed" }.ToJsonString());
        //    }
        //}

        //#endregion







        //public ActionResult ShowAllInOne(int appId, string type)
        //{
        //    SummarizeViewModel SummarizeVM = new SummarizeViewModel();
        //    if (String.Equals(type, "自修"))
        //    {
        //        Dispatch dispatchSheetM = new Dispatch();
        //        ZXFA selfRepairPlanM = new ZXFA();
        //        string dispatherName = string.Empty;
        //        string engineerName = string.Empty;
        //        WXShenQing applicationM = RepairService.AllInfo(appId, ref dispatchSheetM, ref selfRepairPlanM, ref dispatherName, ref engineerName);
        //        SummarizeVM.Instruction = dispatchSheetM.ZSSX;
        //        SummarizeVM.DispatchTime = dispatchSheetM.PGSJ;
        //        SummarizeVM.Dispatcher = dispatherName;
        //        SummarizeVM.Engineer = engineerName;
        //        SummarizeVM.ApplicationVM = new ApplicationsViewModel(applicationM);
        //        SummarizeVM.ApplicationVM.DeviceShortName = RepairService.DeviceShortNameAndNoDic[applicationM.SBBH];
        //        SummarizeVM.SelfRepairVM = new SelfRepairPlanDto(selfRepairPlanM);
        //    }
        //    return View(SummarizeVM);
        //}
        //public ActionResult Summarize()
        //{
        //    SummarizeViewModel summarizeVM = new SummarizeViewModel();
        //    summarizeVM.ApplicationVM.Id = Convert.ToInt32(Request.Params["appID"]);
        //    string beginTime = Request.Params["beginTime"];
        //    if (!string.IsNullOrEmpty(beginTime))
        //    {
        //        summarizeVM.SelfRepairVM.StartTime = Convert.ToDateTime(beginTime);
        //    }
        //    summarizeVM.SelfRepairVM.ID = Convert.ToInt32(Request.Params["selfRepairPlanId"]);
        //    summarizeVM.SelfRepairVM.TimeCost = Convert.ToDouble(Request.Params["timeCost"]);
        //    summarizeVM.SelfRepairVM.Steps = Request.Params["steps"];
        //    summarizeVM.SelfRepairVM.Tools = Request.Params["tools"];
        //    var partsInfo = Request.Params["partsInfo"];
        //    summarizeVM.SelfRepairVM.IsUseSpareParts = partsInfo == string.Empty ? "否" : "是";
        //    summarizeVM.SelfRepairVM.SparePartsInfo = partsInfo;
        //    summarizeVM.ApplicationVM.FailureDescription = Request.Params["description"];
        //    summarizeVM.ApplicationVM.FailureAppearance = Request.Params["appearance"];
        //    summarizeVM.ApplicationVM.FstLevFailureLocation = Request.Params["fstLocation"];
        //    summarizeVM.ApplicationVM.SecLevFailureLocation = Request.Params["secLocation"];
        //    summarizeVM.ApplicationVM.ThiLevFailureLocation = Request.Params["thiLocation"];
        //    WXShenQing applicationM = new WXShenQing(summarizeVM.ApplicationVM);
        //    ZXFA selfRepairPlanM = new ZXFA(summarizeVM.SelfRepairVM);
        //    bool result = RepairService.Finish(applicationM, selfRepairPlanM);
        //    if (result)
        //    {
        //        return Content(new { msg = "处理成功", status = "success" }.ToJsonString());
        //    }
        //    else
        //    {
        //        return Content(new { msg = "处理失败", status = "failed" }.ToJsonString());
        //    }

        //}





        //[HttpPost]
        //public ActionResult GetSelfRepairPlanByAppId(int selfRepairPlanID)
        //{
        //    ZXFA resultM = RepairService.SelfRepairPlanByAppId(selfRepairPlanID);
        //    SelfRepairPlanDto resultVM = new SelfRepairPlanDto(resultM);
        //    if (resultVM != null)
        //    {
        //        return Content(new { msg = "找到方案", status = "success", data = resultVM }.ToJsonString());
        //    }
        //    else
        //    {
        //        return Content(new { msg = "未找到方案", status = "failed" }.ToJsonString());
        //    }
        //}

        ///// <summary>
        ///// 测试功能的
        ///// </summary>
        ///// <returns></returns>
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

    }
}