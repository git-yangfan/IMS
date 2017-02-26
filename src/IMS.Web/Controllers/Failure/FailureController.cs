
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
        public ActionResult GetShortNameList()
        {
            List<SBXXViewModel> ShortNameList = failureService.GetShortNameList();
            Result<SBXXViewModel> jsonResult=null;
            if (ShortNameList != null)
            {
                jsonResult = new Result<SBXXViewModel>(ResultStatus.OK,  ShortNameList);
               
            }
            else 
            {
                jsonResult = new Result<SBXXViewModel>(ResultStatus.Failed,"Failed For Some Reasons",null);
            }
                 return Content(jsonResult.ToJsonString());
        }

        public void AddNewFailure()
        {
            GZShenQingViewModel GZShenQingViewModl = new GZShenQingViewModel();
            GZShenQingViewModl.SBBH = Request.Params["SBBH"];
            GZShenQingViewModl.fssj = Convert.ToDateTime(Request.Params["Fssj"]);
            GZShenQingViewModl.gzxianxiang = Request.Params["GZXianXiang"];
            GZShenQingViewModl.gzms = Request.Params["GZMS"];
            GZShenQingViewModl.GZBWA = Request.Params["GZBWA"];
            GZShenQingViewModl.GZBWB = Request.Params["GZBWB"];
            GZShenQingViewModl.GZBWC = Request.Params["GZBWC"];
            GZShenQingViewModl.bgrxm = "报告人A";
            GZShenQingViewModl.bgsj = DateTime.Now;
            GZShenQingViewModl.sfkyxg = 0;
            failureService.AddNewFailure(GZShenQingViewModl);

        }

        public void UpDateFailureInfo() 
        {
            GZShenQingViewModel GZShenQingViewModel = new GZShenQingViewModel();
            int id =Convert.ToInt32( Request.Params["GZShenQingId"]);
            GZShenQingViewModel.fssj = Convert.ToDateTime(Request.Params["Fssj"]);
            GZShenQingViewModel.gzxianxiang = Request.Params["GZXianXiang"];
            GZShenQingViewModel.gzms = Request.Params["GZMS"];
            GZShenQingViewModel.GZBWA = Request.Params["GZBWA"];
            GZShenQingViewModel.GZBWB = Request.Params["GZBWB"];
            GZShenQingViewModel.GZBWC = Request.Params["GZBWC"];
            GZShenQingViewModel.bgsj = DateTime.Now;
            failureService.UpDateFailureInfo(id,GZShenQingViewModel);
        }

        public void DeleteFailure() 
        {
            int id = Convert.ToInt32(Request.Params["GZShenQingId"]);
            failureService.DeleteFailureByID(id);
        }
      
        public ActionResult GetAllApplicationsByUserName(string name)
        {
            var AllApplicationsList = failureService.GetAllApplicationsByName(name);
            if (AllApplicationsList != null)
            {
                return Content(AllApplicationsList.ToJsonString());
            }
            else
                return Json("");
        }
        public ActionResult GetAllApplicationsByUserName1(string name, int limit, int offset)
        {
            var AllApplicationsList = failureService.GetAllApplicationsByName(name);
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
            var lstRes = new List<SBXXViewModel>();
            for (var i = 0; i < 50; i++)
            {
                var oModel = new SBXXViewModel();
                oModel.SBBH = Guid.NewGuid().ToString();
                oModel.SBJC = "销售部" + i;
                lstRes.Add(oModel);
            }

            var total = lstRes.Count;
            var rows = lstRes;
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }


    }
}