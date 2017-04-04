using IMS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.Model
{
  public  class WXZongJie
    {
        public int ID { get; set; }
        public int WXSQID { get; set; }
        public string GZBWA { get; set; }
        public string GZBWB { get; set; }
        public string GZBWC { get; set; }
        public string GZXIANXIANG { get; set; }
        public string GZMS { get; set; }
        public string WXBUZHOU { get; set; }
        public string WXGJ { get; set; }
        public string BJXX { get; set; }
        public DateTime KSSJ { get; set; }
        public int WXYS { get; set; }

        public WXZongJie() { }
        //public WXZongJie(SummarizeViewModel viewModel) 
        //{
        //    this.WXSQID = viewModel.ApplicationID;
        //    this.GZBWA = viewModel.FstLevFailureLocation;
        //    this.GZBWB = viewModel.SecLevFailureLocation;
        //    this.GZBWC = viewModel.ThiLevFailureLocation;
        //    this.GZXIANXIANG = viewModel.FailureAppearance;
        //    this.GZMS = viewModel.FailureDescription;
        //    this.WXBUZHOU = viewModel.Steps;
        //    this.WXGJ = viewModel.Tools;
        //    this.KSSJ = viewModel.BeginTime;
        //    this.BJXX = viewModel.SparePartsInfo;
        //    this.WXYS = viewModel.TimeCost;
        //}

    }
}
