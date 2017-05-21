using IMS.Data.DAL;
using IMS.Model.Entity;
using IMS.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IMS.Json;
using System.IO;


namespace IMS.Web.Areas.DataExchange.Controllers
{
    public class DataExchangeController : AsyncController
    {
        DataExchangeDAL exchangeDAL = new DataExchangeDAL();
        List<RepairApplication> appEntityList;
        List<Device> devEntityList;

        // GET: DataExchange/DataExchange
        public ActionResult ImportFromExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportApplications()
        {
            var res = new JsonResult<string>();
            HttpPostedFile file = System.Web.HttpContext.Current.Request.Files[0];
            var isLoad = FileToEntity(file, "app");
            if (!isLoad)
                res.msg = "上传文件失败";
            var isSave = EntityToDb("app");
            if (!isSave)
                res.msg = "导入失败";
            if (isLoad & isSave)
            {
                res.flag = true;
                res.msg = "导入成功";
            }
            return Content(res.ToJsonString());
        }
        public ActionResult ImportDevices()
        {
            var res = new JsonResult<string>();
            HttpPostedFile file = System.Web.HttpContext.Current.Request.Files[0];
            var isLoad = FileToEntity(file, "dev");
            var isSave = EntityToDb("dev");
            if (!isLoad)
                res.msg = "上传文件失败";
            if (!isSave)
                res.msg = "导入失败";
            if (isLoad & isSave)
            {
                res.flag = true;
                res.msg = "导入成功";
            }
            return Content(res.ToJsonString());
        }
        public bool FileToEntity(HttpPostedFile file, string modelType)
        {
            bool isSuccess = true;
            if (string.Empty.Equals(file.FileName) || ".xlsx" != Path.GetExtension(file.FileName))
            {
                isSuccess = false;
                throw new ArgumentException("当前文件格式不正确,请确保正确的Excel文件格式!");
            }
            var severPath = this.Server.MapPath("/files/"); //获取当前虚拟文件路径
            var savePath = Path.Combine(severPath, file.FileName); //拼接保存文件路径
            try
            {
                file.SaveAs(savePath);
                if (string.Equals(modelType, "app"))
                {
                    appEntityList = ExcelExtension.LoadFromExcel<RepairApplication>(savePath).ToList();
                }
                if (string.Equals(modelType, "dev"))
                {
                    devEntityList = ExcelExtension.LoadFromExcel<Device>(savePath).ToList();
                }
            }
            catch { isSuccess = false; }
            finally
            {
                System.IO.File.Delete(savePath);//每次上传完毕删除文件
            }
            return isSuccess;
        }
        public bool EntityToDb(string modelType)
        {
            bool isSuccess = false;
            if (string.Equals(modelType, "app"))
            {
                if (appEntityList.Count > 0)
                {
                    var existId = exchangeDAL.GetExistIdsByEntity<RepairApplication>();
                    List<RepairApplication> toUpdateList = new List<RepairApplication>();
                    List<RepairApplication> toInsertList = new List<RepairApplication>();
                    foreach (var item in appEntityList)
                    {
                        if (existId.Contains(item.Id))
                            toUpdateList.Add(item);
                        else
                            toInsertList.Add(item);
                    }
                    if (toUpdateList.Count > 0)
                    {
                        isSuccess = Task.Factory.StartNew(() => { return exchangeDAL.UpdateRange<RepairApplication>(toUpdateList); }).Result;

                    }
                    if (toInsertList.Count > 0)
                    {
                        isSuccess = Task.Factory.StartNew(() => { return exchangeDAL.InsertRange<RepairApplication>(toInsertList); }).Result;
                    }
                }
            }
            if (string.Equals(modelType, "dev"))
            {
                if (devEntityList.Count > 0)
                {
                    var existId = exchangeDAL.GetExistIdsByEntity<Device>();
                    List<Device> toUpdateList = new List<Device>();
                    List<Device> toInsertList = new List<Device>();
                    foreach (var item in devEntityList)
                    {
                        if (existId.Contains(item.Id))
                            toUpdateList.Add(item);
                        else
                            toInsertList.Add(item);
                    }
                    if (toUpdateList.Count > 0)
                    {
                        isSuccess = Task.Factory.StartNew(() => { return exchangeDAL.UpdateRange<Device>(toUpdateList); }).Result;
                    }
                    if (toInsertList.Count > 0)
                    {
                        isSuccess = Task.Factory.StartNew(() => { return exchangeDAL.InsertRange<Device>(toInsertList); }).Result;
                    }
                }
            }
            return isSuccess;
        }
    }
}