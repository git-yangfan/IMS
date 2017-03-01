using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMS.Json;
using IMS.Model.ViewModel;

namespace IMS.Web.Controllers.Failure
{
    public class FailureProcessController : Controller
    {
        // GET: FailureProcess
        public ActionResult Dispatch()
        {
            return View();
        }

        public JsonResult Dispatch2(string applicationId, string engineerId) 
        {
            var a = engineerId;
            return Json("");

        }
    }
}