using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers.Failure
{
    public class FailureProcessController : Controller
    {
        // GET: FailureProcess
        public ActionResult Dispatch()
        {
            return View();
        }
    }
}