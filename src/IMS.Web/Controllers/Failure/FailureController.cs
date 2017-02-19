
using IMS.Data;
using IMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers.Failure
{
    public class FailureController : Controller
    {
        //
        // GET: /Failure/

        private GZShenQingRepository GZShenQingRepository = new GZShenQingRepository();
        public ActionResult FailureReport()
        {
            return View();

        }

        public JsonResult GetShortNameList()
        {
            IEnumerable<SBXX> ShortNameList = GZShenQingRepository.GetShortNameList();
            if (ShortNameList != null)
            {
                return Json(ShortNameList);
            }
            else
                return Json("");
        }

        public void AddNewFailure()
        {
            GZShenQing GZTiJiao = new GZShenQing();
            GZTiJiao.SBBH = Request.Params["SBBH"];
            GZTiJiao.fssj = Convert.ToDateTime(Request.Params["Fssj"]);
            GZTiJiao.gzxianxiang = Request.Params["GZXianXiang"];
            GZTiJiao.gzms = Request.Params["GZMS"];
            GZTiJiao.GZBWA = Request.Params["GZBWA"];
            GZTiJiao.GZBWB = Request.Params["GZBWB"];
            GZTiJiao.GZBWC = Request.Params["GZBWC"];
            GZTiJiao.bgrxm = "CESHI";
            GZTiJiao.bgsj = DateTime.Now;
            GZTiJiao.sfkyxg = 0;
            GZShenQingRepository.AddNewFailure(GZTiJiao);

        }

        public JsonResult GetAllApplicationsByName(string name) 
        {
            IEnumerable<GZShenQing> AllApplicationsList = GZShenQingRepository.GetAllApplicationsByName(name);
            if (AllApplicationsList != null)
            {
                return Json(AllApplicationsList);
            }
            else
                return Json("");
        }

    }
}