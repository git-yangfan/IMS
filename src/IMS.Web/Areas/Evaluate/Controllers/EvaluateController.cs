using IMS.Data.DAL;
using IMS.Web.Areas.Evaluate.Algorithms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMS.Json;
using IMS.Web.Dto;
using IMS.Model.Entity;
using System.Linq.Expressions;
using AutoMapper;
using System.Text;

namespace IMS.Web.Areas.Evaluate.Controllers
{
    public class EvaluateController : Controller
    {
        EvaluateDAL evaluateDAL = new EvaluateDAL();
        public ActionResult Index()
        {
            return View();
        }

        #region 单机评价
        public ActionResult DeviceEvaluate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EvaluateResult()
        {
            string startTime = "2011-04-02", endTime = "2017-5-2", deviceNo = "13113113";//测试用
            var res = new JsonResult<DeviceEvaluateDto>();
            res.flag = true;
            res.data = SingleDeviceEvaluate(deviceNo, startTime, endTime);
            return Content(res.ToJsonString());
        }
        private DeviceEvaluateDto SingleDeviceEvaluate(string deviceNo, string startTime, string endTime)
        {
            DeviceEvaluateDto deviceEvaluateDto = new DeviceEvaluateDto();
            string recordsSql = "select app.DeviceNo,app.FailureType,app.BeginTime,app.FirstLocation,app.SecondLocation,app.ThirdLocation, round((" +
                                "(case when app.CHECKTIME is null then to_date('" + endTime + "', 'yyyy-mm-dd hh24:mi:ss') else app.CHECKTIME end)-" +
                                "(app.BEGINTIME)) * 24, 2) AS pausetime FROM REPAIRAPPLICATION app " +
                                "WHERE (app.BEGINTIME > to_date('" + startTime + "', 'yyyy-mm-dd hh24:mi:ss') AND app.BEGINTIME < to_date('" + endTime + "', 'yyyy-mm-dd hh24:mi:ss')) ";
            if (!String.IsNullOrEmpty(deviceNo))
            {
                recordsSql += " and app.DeviceNo='" + deviceNo + "'";
            }
            recordsSql += " order by app.begintime";
            DataTable sourceTable = evaluateDAL.GetFailureRecords(recordsSql);
            List<string> groupNames = new List<string>() { "FailureType", "SecondLocation" };//按故障类别，二级故障部位统计，统计故障次数和故障时间
            List<DataUnit> groupedData = GroupByColName(sourceTable, groupNames);
            deviceEvaluateDto.Curves = CreatCurves(groupedData);
            deviceEvaluateDto.FailureStas = Statistics(groupedData);//故障数据统计
            MMDD_SBLY dncData = evaluateDAL.GetDncData(deviceNo, startTime, endTime);
            DncRelated dncRelateReliability = Reliability.DncRelateReliability(dncData);
            deviceEvaluateDto.dncRelateReliability = dncRelateReliability;

            List<double> intervar = new List<double>();
            //for (int i = 0; i < sourceTable.Rows.Count-1; i++)
            //{
            //    intervar.Add(Convert.ToDouble((Convert.ToDateTime(sourceTable.Rows[i + 1]["BeginTime"]) -  Convert.ToDateTime(sourceTable.Rows[i ]["BeginTime"])).TotalHours));
            //}真实故障间隔数据
            double alph, beta;
            string mtbfSql = "select gzjgsj as jgsj from csmtbfsj";
            intervar = evaluateDAL.Interval(mtbfSql);//测试的时候用的
            deviceEvaluateDto.MTBF = Reliability.MTBF(intervar, out alph, out beta);
            return deviceEvaluateDto;
        } 
        #endregion

        #region 整机对比
        public ActionResult Compare()
        {
            string deviceNos = Request.QueryString["devs"];
            string startTime = Convert.ToDateTime(Request.QueryString["starttime"]).ToShortDateString();
            string endTime = Convert.ToDateTime(Request.QueryString["endtime"]).ToShortDateString();
            ViewData["devices"] = deviceNos;
            ViewData["startTime"] = startTime;
            ViewData["endTime"] = endTime;
            return View();
        }
        public ActionResult CompareDevice(List<string> devices, string startTime, string endTime)
        {
            var res = new JsonResult<List<CompareDeviceDto>>();
            List<CompareDeviceDto> compareResult = new List<CompareDeviceDto>();
            foreach (var dev in devices)
            {
                CompareDeviceDto dto = new CompareDeviceDto();
                Expression<Func<Device, bool>> exp = item => item.DeviceNo == dev;
                DeviceDto devDto = Mapper.Map<Device, DeviceDto>(evaluateDAL.Single(exp));
                dto.Device = devDto;
                MMDD_SBLY dncData = evaluateDAL.GetDncData(dev, startTime, endTime);
                DncRelated dncRelateReliability = Reliability.DncRelateReliability(dncData);
                dto.DncReliability = dncRelateReliability;
                //string mtbfSql = "select gzjgsj as jgsj from csmtbfsj"; //测试的时候用的，实际调用时应该给定sql语句
                string recordsSql = "select app.DeviceNo,app.FailureType,app.BeginTime,app.FirstLocation,app.SecondLocation,app.ThirdLocation, round((" +
                                    "(case when app.CHECKTIME is null then to_date('" + endTime + "', 'yyyy-mm-dd hh24:mi:ss') else app.CHECKTIME end)-" +
                                    "(app.BEGINTIME)) * 24, 2) AS pausetime FROM REPAIRAPPLICATION app " +
                                    "WHERE (app.BEGINTIME > to_date('" + startTime + "', 'yyyy-mm-dd hh24:mi:ss') AND app.BEGINTIME < to_date('" + endTime + "', 'yyyy-mm-dd hh24:mi:ss'))";
                if (!String.IsNullOrEmpty(dev))
                {
                    recordsSql += " and app.DeviceNo='" + dev + "'";
                }
                recordsSql += " order by app.begintime";

                DataTable sourceTable = evaluateDAL.GetFailureRecords(recordsSql);
                List<double> intervar = new List<double>();
                for (int i = 0; i < sourceTable.Rows.Count - 1; i++)
                {
                    intervar.Add(Convert.ToDouble((Convert.ToDateTime(sourceTable.Rows[i + 1]["BeginTime"]) - Convert.ToDateTime(sourceTable.Rows[i]["BeginTime"])).TotalHours));
                }//真实故障间隔数据
                //intervar = evaluateDAL.Interval(mtbfSql);
                double alph, beta;
                dto.MTBF = Reliability.MTBF(intervar, out alph, out beta);
                dto.Alph = alph; dto.Beta = beta;

                string[] t = new string[2000];
                double[] f = new double[2000];
                Curve curve = new Curve();
                for (int i = 1; i < 2000; i++)
                {
                    t[i] = i.ToString();
                    f[i] = Math.Exp(-Math.Pow((i / alph), beta));
                }
                curve.XValues = t;
                curve.YTimeValues = f;
                List<Curve> curves = new List<Curve>();
                curves.Add(curve);
                dto.Curves = curves;
                compareResult.Add(dto);
            }
            res.data = compareResult;
            res.flag = true;
            return Content(res.ToJsonString());
        }
        public ActionResult DeviceList()
        {
            return View();
        }
        public ActionResult DevicesBySection(string sectionName)
        {
            var devs = CommonDAL.GetDevicesBySection(sectionName);
            List<DeviceDto> devsDto = Mapper.Map<List<Device>, List<DeviceDto>>(devs);
            var res = new JsonResult<List<DeviceDto>>();
            if (devsDto.Count > 0)
            {
                res.data = devsDto;
                res.flag = true;
            }
            return Content(res.ToJsonString());
        } 
        #endregion

        #region 按机床种类和品牌对比

        public ActionResult CompareByBrand()
        {
            var machineTypes = evaluateDAL.MachineType();
            ViewData["machineTypes"] = machineTypes;
            return View();
        }
        [HttpPost]
        public ActionResult BrandByMachineType(string machineType)
        {
            var res = new JsonResult<List<string>>();
            var brands = evaluateDAL.GetBrandsByType(machineType);
            if (brands.Count > 0)
            {
                res.flag = true;
                res.data = brands;
            }
            return Content(res.ToJsonString());
        }
        [HttpPost]
        public ActionResult CompareByBrand(string type, List<string> brands, string startTime, string endTime)
        {
            CompareBrandDto compareResultDto = new CompareBrandDto();
            compareResultDto.MachineType = type;
            compareResultDto.DisplayName = type + "对比结果";
            compareResultDto.StartTime = startTime; compareResultDto.EndTime = endTime;
            compareResultDto.BrandList = new List<BrandEvaluateDto>();
            var res = new JsonResult<CompareBrandDto>();
            if (brands.Count > 0)
            {
                foreach (var brand in brands)
                {
                    String Sql = "select app.DeviceNo,app.FailureType,app.BeginTime,app.FirstLocation,app.SecondLocation,app.ThirdLocation, round((" +
                          "(case when app.CHECKTIME is null then to_date('" + endTime + "', 'yyyy-mm-dd hh24:mi:ss') else app.CHECKTIME end)-" +
                          "(app.BEGINTIME)) * 24, 2) AS pausetime FROM REPAIRAPPLICATION app " +
                          "WHERE (app.BEGINTIME > to_date('" + startTime + "', 'yyyy-mm-dd hh24:mi:ss') AND app.BEGINTIME < to_date('" + endTime + "', 'yyyy-mm-dd hh24:mi:ss')) " +
                          "and deviceno in (select deviceno from device where type='" + type + "' and brand='" + brand + "' ) order by app.begintime";
                    DataTable sourceTable = evaluateDAL.GetFailureRecords(Sql);
                    if (sourceTable.Rows.Count > 0)
                    {
                        res.flag = true;
                        BrandEvaluateDto brandDto = new BrandEvaluateDto();
                        List<string> groupNames = new List<string>() { "FailureType", "SecondLocation" };//按故障类别，二级故障部位统计，统计故障次数和故障时间
                        List<DataUnit> groupedData = GroupByColName(sourceTable, groupNames);
                        brandDto.Curves = CreatCurves(groupedData);
                        brandDto.Brand = brand;
                        List<double> intervar = new List<double>();
                        for (int i = 0; i < sourceTable.Rows.Count - 1; i++)
                        {
                            intervar.Add(Convert.ToDouble((Convert.ToDateTime(sourceTable.Rows[i + 1]["BeginTime"]) - Convert.ToDateTime(sourceTable.Rows[i]["BeginTime"])).TotalHours));
                        }//真实故障间隔数据
                        double alph, beta;
                        double mtbf = Reliability.MTBF(intervar, out alph, out beta);
                        brandDto.MTBF = mtbf;
                        brandDto.Alph = alph; brandDto.Beta = beta;
                        compareResultDto.BrandList.Add(brandDto);
                    }
                }
                res.data = compareResultDto;
            }
            return Content(res.ToJsonString());
        }
        
        #endregion

        #region 公用的
        private List<DataUnit> GroupByColName(DataTable sourceData, List<string> groupNames)
        {
            List<DataUnit> groupedData = new List<DataUnit>();
            foreach (var colName in groupNames)
            {
                string displayName = string.Empty;
                switch (colName)
                {
                    case "FailureType":
                        displayName = "故障类别";
                        break;
                    case "SecondLocation":
                        displayName = "故障部位";
                        break;
                    default:
                        break;
                }
                if (sourceData.Columns.Contains(colName))
                {
                    //计算相同 故障部位或故障类别 的 总次数 总停工时间
                    for (int i = 0; i < sourceData.Rows.Count; i++)
                    {
                        string strKey = sourceData.Rows[i][colName].ToString();
                        if (groupedData.Any(t => t.XValueStr == strKey))
                        {
                            groupedData.Find(t => t.XValueStr == strKey).Count++;
                            groupedData.Find(t => t.XValueStr == strKey).PauseTime += Math.Round(Convert.ToDouble(sourceData.Rows[i]["pausetime"]), 2);
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(strKey))
                            {
                                DataUnit dataPoint = new DataUnit()
                                {
                                    XValueStr = strKey,
                                    Count = 1,
                                    PauseTime = Math.Round(Convert.ToDouble(sourceData.Rows[i]["pausetime"]), 2),
                                    DisplayName = displayName
                                };
                                groupedData.Add(dataPoint);
                            }
                        }
                    }
                }
            }
            return groupedData;
        }
        private FailureDataStas Statistics(List<DataUnit> dataUnitList)
        {
            List<Curve> curves = new List<Curve>();
            FailureDataStas statisticsRes = new FailureDataStas();
            statisticsRes.TotalCount = dataUnitList.Sum(d => d.Count);
            statisticsRes.TotalPauseTime = dataUnitList.Sum(d => d.PauseTime);
            var freqType = dataUnitList.FindAll(c => c.DisplayName == "故障类别").Find(d => d.Count == dataUnitList.Max<DataUnit>(c => c.Count));
            if (freqType != null)
            {
                statisticsRes.FrequentType = freqType.XValueStr;
                statisticsRes.FrequentTypeCount = freqType.Count;
                statisticsRes.FrequentTypePauseTime = Math.Round(freqType.PauseTime, 2);
            }
            var freqLoc = dataUnitList.FindAll(c => c.DisplayName == "故障部位").Find(d => d.Count == dataUnitList.Max<DataUnit>(c => c.Count));
            if (freqLoc != null)
            {
                statisticsRes.FrequentLoc = freqLoc.XValueStr;
                statisticsRes.FrequentLocCount = freqLoc.Count;
                statisticsRes.FrequentLocPauseTime = Math.Round(freqLoc.PauseTime, 2);
            }
            return statisticsRes;
        }
        private List<Curve> CreatCurves(List<DataUnit> dataUnitList)
        {
            List<Curve> curves = new List<Curve>();
            var curveKinds = dataUnitList.GroupBy(it => it.DisplayName);
            foreach (var item in curveKinds)
            {
                Curve singleCurve = new Curve(item.Key.ToString(), dataUnitList.FindAll(it => it.DisplayName == item.Key.ToString()));
                curves.Add(singleCurve);
            }
            return curves;
        } 
        #endregion


    }
}