
using IMS.Data.Services;
using IMS.Json;
using IMS.Model.Model;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

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


        public ActionResult GetShortNameList()
        {
            IEnumerable<SBXX> ShortNameList = failureService.GetShortNameList();
            if (ShortNameList != null)
            {
                return Content(ShortNameList.ToJsonString());
            }
            else
                return Json("");
        }

        public void AddNewFailure()
        {
            GZShenQing GZShenQing = new GZShenQing();
            GZShenQing.SBBH = Request.Params["SBBH"];
            GZShenQing.fssj = Convert.ToDateTime(Request.Params["Fssj"]);
            GZShenQing.gzxianxiang = Request.Params["GZXianXiang"];
            GZShenQing.gzms = Request.Params["GZMS"];
            GZShenQing.GZBWA = Request.Params["GZBWA"];
            GZShenQing.GZBWB = Request.Params["GZBWB"];
            GZShenQing.GZBWC = Request.Params["GZBWC"];
            GZShenQing.bgrxm = "报告人A";
            GZShenQing.bgsj = DateTime.Now;
            GZShenQing.sfkyxg = 0;
            failureService.AddNewFailure(GZShenQing);

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

    }
}