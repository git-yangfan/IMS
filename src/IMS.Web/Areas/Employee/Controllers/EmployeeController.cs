using IMS.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMS.Json;

namespace IMS.Web.Areas.Employee.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee/Employee
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EmployeeList() 
        {
            var res = new List<EmployeeDto>();
            for (int i = 0; i < 15; i++)
            {
                var temp = new EmployeeDto() 
                {
                    Empl_Name="wang"+i.ToString(),
                    Empl_No=i.ToString(),
                    Role_Name = "管理人员",
                    Add_TIME=DateTime.Now.AddDays(-i),
                    IS_Deleted="否"
                };
                res.Add(temp);
            }
            if (res != null)
            {
                var total = res.Count;
                var rows = res;
                var result = new { total = total, rows = rows };
                return Content(result.ToJsonString());
            }
            else
            {
                return Json("");
            }
        }


        [HttpPost]
        public ActionResult Add(int moudleId, int menuId, int btnId, EmployeeDto dto)
        {
            return RedirectToAction("Index", RouteData.Values);
        }
    }
}