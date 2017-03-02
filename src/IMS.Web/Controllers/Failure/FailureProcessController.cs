using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMS.Json;
using IMS.Model.ViewModel;
using IMS.Data.Services;

namespace IMS.Web.Controllers.Failure
{
    public class FailureProcessController : Controller
    {
        FailureProcessService FailureProcessService = new FailureProcessService();

        // GET: FailureProcess
        public ActionResult Dispatch()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Dispatch(string applicationId, string engineerId, string instruction)
        {
            var a = engineerId;
            FailureProcessViewModel failureProcessVM = new FailureProcessViewModel();
            failureProcessVM.Instruction = instruction;
            failureProcessVM.MaintenanceApplicationViewModel.Id = Convert.ToInt32(applicationId);
            failureProcessVM.EngineerViewModel.EngineerId = Convert.ToInt32(engineerId);
            bool result = FailureProcessService.Dispatch(failureProcessVM);
            if (result)
            {
                return Json("ok");
            }
            else
            {
                return Json("");
            }

        }
    }
}