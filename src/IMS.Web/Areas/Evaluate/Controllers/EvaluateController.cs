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

namespace IMS.Web.Areas.Evaluate.Controllers
{
    public class EvaluateController : Controller
    {
        // GET: Evaluate/Evaluate
        EvaluateDAL evaluateDAL = new EvaluateDAL();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DeviceEvaluate()
        {

            return View();
        }
        [HttpPost]
        public ActionResult EvaluateResult()
        {
            string startTime = "2015-04-02", endTime = "2017-5-2", deviceNo = "";
            var res = new JsonResult<DeviceEvaluateDto>();
            res.data = SingleDeviceEvaluate(deviceNo, startTime, endTime);
            res.flag = true;
            return Content(res.ToJsonString());
        }
        private DeviceEvaluateDto SingleDeviceEvaluate(string deviceNo, string startTime, string endTime)
        {
            DeviceEvaluateDto deviceEvaluateDto = new DeviceEvaluateDto();
            FailureDataStas failureStas;
            DataTable sourceTable = evaluateDAL.GetPauseTimeData(deviceNo, startTime, endTime);
            List<string> groupNames = new List<string>() { "FailureType", "SecondLocation" };//按故障类别，二级故障部位统计，统计故障次数和故障时间
            List<Curve> curves = CreatCurves(sourceTable, groupNames, out failureStas);
            if (curves != null)
                deviceEvaluateDto.Curves = curves;
            deviceEvaluateDto.FailureStas = failureStas;
            MMDD_SBLY dncData = evaluateDAL.GetDncData(deviceNo, startTime, endTime);
            DncRelated dncRelateReliability = Reliability.DncRelateReliability(dncData);
            deviceEvaluateDto.dncRelateReliability = dncRelateReliability;

            string mtbfSql = "select gzjgsj as jgsj from csmtbfsj"; //测试的时候用的，实际调用时应该给定sql语句
            //string sql = "select FSSJ,Round((fssj-lag(fssj,1,null)over(order by fssj asc))*24,1) as JGSJ from sbgzxx  where sbbh='设备1'";
            DataTable data = evaluateDAL.GetDataTable(mtbfSql);
            List<double> intervar = new List<double>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                intervar.Add(Convert.ToDouble(data.Rows[i]["jgsj"]));
            }
            double alph, beta;
            deviceEvaluateDto.MTBF = Reliability.MTBF(intervar, out alph, out beta);
            return deviceEvaluateDto;
        }

        private List<Curve> CreatCurves(DataTable sourceTable, List<string> groupNames, out FailureDataStas stas)
        {
            List<Curve> curves = new List<Curve>();
            stas = new FailureDataStas();
            foreach (var item in groupNames)
            {
                string curveName = string.Empty;
                var dataUnitList = CalculateTimeAndCount(sourceTable, item);
                stas.TotalCount = dataUnitList.Sum(d => d.Count);
                stas.TotalPauseTime = dataUnitList.Sum(d => d.PauseTime);
                switch (item)
                {
                    case "FailureType":
                        curveName = "故障类别";
                        var freqType = dataUnitList.Find(d => d.Count == dataUnitList.Max<DataPoint>(c => c.Count));
                        stas.FrequentType = freqType.XValueStr;
                        stas.FrequentTypeCount = freqType.Count;
                        stas.FrequentTypePauseTime = freqType.PauseTime;
                        break;
                    case "SecondLocation":
                        curveName = "故障部位";
                        var freqLoc = dataUnitList.Find(d => d.Count == dataUnitList.Max<DataPoint>(c => c.Count));
                        stas.FrequentLoc = freqLoc.XValueStr;
                        stas.FrequentLocCount = freqLoc.Count;
                        stas.FrequentLocPauseTime = freqLoc.PauseTime;
                        break;
                    default:
                        break;
                }

                Curve singleCurve = new Curve(curveName, dataUnitList);
                curves.Add(singleCurve);
            }
            return curves;
        }

        private List<DataPoint> CalculateTimeAndCount(DataTable sourceTable, string colName)
        {
            if (sourceTable.Columns.Contains(colName))
            {
                //计算相同 故障部位或故障类别 的 总次数 总停工时间
                string strKey = string.Empty;
                List<DataPoint> pointCollection = new List<DataPoint>();

                for (int i = 0; i < sourceTable.Rows.Count; i++)
                {
                    strKey = sourceTable.Rows[i][colName].ToString();
                    if (pointCollection.Any(t => t.XValueStr == strKey))
                    {
                        pointCollection.Find(t => t.XValueStr == strKey).Count++;
                        pointCollection.Find(t => t.XValueStr == strKey).PauseTime += Convert.ToDouble(sourceTable.Rows[i]["pausetime"]);
                    }
                    else
                    {
                        DataPoint dataPoint = new DataPoint()
                        {
                            XValueStr = strKey,
                            Count = 1,
                            PauseTime = Convert.ToDouble(sourceTable.Rows[i]["pausetime"])
                        };
                        pointCollection.Add(dataPoint);
                    }
                }
                return pointCollection;

            }
            else return null;
        }

        public ActionResult Compare()
        {
            string deviceNos = Request.QueryString["devs"];
            ViewData["devices"] = deviceNos;
            return View();
        }
        public ActionResult CompareDevice(List<string> devices, string starttime, string endtime)
        {
            var res = new JsonResult<List<CompareDto>>();
            List<CompareDto> compareResult = new List<CompareDto>();
            foreach (var dev in devices)
            {
                CompareDto dto = new CompareDto();
                Expression<Func<Device, bool>> exp = item => item.DeviceNo == dev;
                DeviceDto devDto = Mapper.Map<Device, DeviceDto>(evaluateDAL.Single(exp));
                dto.Device = devDto;
                MMDD_SBLY dncData = evaluateDAL.GetDncData(dev, starttime, endtime);
                DncRelated dncRelateReliability = Reliability.DncRelateReliability(dncData);
                dto.DncReliability = dncRelateReliability;
                string mtbfSql = "select gzjgsj as jgsj from csmtbfsj"; //测试的时候用的，实际调用时应该给定sql语句
                //string sql = "select FSSJ,Round((fssj-lag(fssj,1,null)over(order by fssj asc))*24,1) as JGSJ from sbgzxx  where sbbh='设备1'";
                DataTable data = evaluateDAL.GetDataTable(mtbfSql);
                List<double> intervar = new List<double>();
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    intervar.Add(Convert.ToDouble(data.Rows[i]["jgsj"]));
                }
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



    }
}