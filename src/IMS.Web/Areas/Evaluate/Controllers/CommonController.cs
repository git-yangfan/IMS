using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Evaluate.Controllers
{
    public class CommonController : Controller
    {
        // GET: Evaluate/Common
        public ActionResult SearchBar()
        {
            return PartialView();
        }
        public ActionResult Index() { return View(); }
    }
}