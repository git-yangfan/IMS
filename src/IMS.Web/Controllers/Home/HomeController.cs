using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers.Home
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult HomePage()
        {
            return View();
        }
	}
}