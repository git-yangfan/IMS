using AutoMapper;
using IMS.Data.DAL;
using IMS.Model.Entity;
using IMS.Web.Dto;
using System.Collections.Generic;
using System.Web.Mvc;

namespace IMS.Web.Areas.Repair.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /Partial/

        public ActionResult SubSystem()
        {
            return PartialView();
        }
        public ActionResult EngineerSelect()
        {
            return PartialView();
        }

        public ActionResult SearchBar()
        {
            return PartialView();
        }
        public JsonResult GetSubSystemList(int lev, int parentId)
        {
            var res = new JsonResult<List<SubSystemDto>>();
            if (lev != -1)
            {
                var subsystemEntityList = CommonDAL.GetSubSystemList(lev, parentId);
                var list = Mapper.Map<List<SubSystem>, List<SubSystemDto>>(subsystemEntityList);
                if (list != null)
                {
                    res.flag = true;
                    res.data = list;
                }
            }
            return Json(res);
        }
        public ActionResult GetTeamOrEngrName(string type, string teamName)
        {
            var res = new JsonResult<List<EngineerDto>>();
            List<Users> userList = CommonDAL.GetTeamOrEngrName(type, teamName);
            var list = Mapper.Map<List<Users>, List<EngineerDto>>(userList);
            if (list != null)
            {
                res.flag = true;
                res.data = list;
            }
            return Json(res);
        }

        public JsonResult GetWorkSectionNameList()
        {
            var res = new JsonResult<List<DeviceDto>>();
            List<Device> deviceList = CommonDAL.GetWorkSectionNameList();
            var list = Mapper.Map<List<Device>, List<DeviceDto>>(deviceList);
            if (list != null)
            {
                res.flag = true;
                res.data = list;
            }
            return Json(res);
        }

        public JsonResult GetDeviceNamesBySection(string sectionName)
        {
            var res = new JsonResult<List<DeviceDto>>();
            List<Device> deviceList = CommonDAL.GetDevicesBySection(sectionName);
            var list = Mapper.Map<List<Device>, List<DeviceDto>>(deviceList);
            if (list != null)
            {
                res.flag = true;
                res.data = list;
            }
            return Json(res);
        }
    }
}