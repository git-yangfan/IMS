using IMS.Model.Model;
using System;
using System.Collections.Generic;
using OracleSugar;
using IMS.Model.ViewModel;

namespace IMS.Data.Services
{
    public class FailureService
    {
        public IEnumerable<SubSystem> GetSubSystemList(int lev, int pid)
        {
            IEnumerable<SubSystem> subSystemList = null;
            using (var client = DbConfig.GetInstance())
            {
                subSystemList = client.Queryable<SubSystem>().Where(s => s.Lev == lev).Where(s => s.Pid == pid).ToList();
            }
            return subSystemList;


        }
        public List<MaintenanceApplicationViewModel> GetAllApplicationsByName(string name, int limit, int offset, string sbbh, string fssj)
        {

            using (var client = DbConfig.GetInstance())
            {
                List<GZShenQing> listWithSBBH = new List<GZShenQing>();
                if (sbbh!=""&&!(String.Equals(sbbh,"-1")))
                {
                    listWithSBBH = client.Queryable<GZShenQing>().Where(c => c.BGRId == name).Where(g => g.SBBH == sbbh).OrderBy(d => d.FSSJ).Skip(offset).Take(limit).ToList();
                }
                else if (sbbh != "" && !(String.Equals(sbbh, "-1")) && !(String.Equals(fssj, string.Empty)))
                {
                    DateTime beginTime = Convert.ToDateTime(fssj);
                    listWithSBBH = client.Queryable<GZShenQing>().Where(c => c.BGRId == name).Where(g => g.SBBH == sbbh).Where(s => s.FSSJ == beginTime).OrderBy(d => d.FSSJ).Skip(offset).Take(limit).ToList();
                }
                else if (sbbh == "" || (String.Equals(sbbh, "-1")) && !(String.Equals(fssj, string.Empty)))
                {
                    DateTime beginTime = Convert.ToDateTime(fssj);
                    listWithSBBH = client.Queryable<GZShenQing>().Where(c => c.BGRId == name).Where(s => s.FSSJ == beginTime).OrderBy(d => d.FSSJ).Skip(offset).Take(limit).ToList();
                 
                }
                else
                {
                    listWithSBBH = client.Queryable<GZShenQing>().Where(c => c.BGRId == name).OrderBy(d=>d.FSSJ).Skip(offset).Take(limit).ToList();
                }
                List<MaintenanceApplicationViewModel> gZShenQingViewModelList = new List<MaintenanceApplicationViewModel>();
                for (int i = 0; i < listWithSBBH.Count; i++)
                {
                    var item=listWithSBBH[i];
                    MaintenanceApplicationViewModel gZShenQingViewModel = new MaintenanceApplicationViewModel(item);
                    gZShenQingViewModel.Order = i+1;
                    gZShenQingViewModel.DeviceShortName = CommonService.GetShortName(item.SBBH);
                    gZShenQingViewModelList.Add(gZShenQingViewModel);
                }
                return gZShenQingViewModelList;
            }

        }
    
        public bool AddNewFailure(MaintenanceApplicationViewModel gZShenQingViewModel)
        {
            if (gZShenQingViewModel != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {
                        GZShenQing gZShenQing = GZShenQingViewModelToModel(gZShenQingViewModel);
                        int id = client.Queryable<GZShenQing>().Max(it => it.Id).ObjToInt() + 1;
                        gZShenQing.Id = id;
                        return client.Insert<GZShenQing>(gZShenQing).ObjToBool();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
                return false;

        }
        private GZShenQing GZShenQingViewModelToModel(MaintenanceApplicationViewModel viewModel)
        {
            GZShenQing gZShenQing = new GZShenQing();
            gZShenQing.BGRId = viewModel.ReporterId;
            gZShenQing.BGSJ = viewModel.ReportTime;
            gZShenQing.FSSJ = viewModel.BeginTime;
            gZShenQing.GZBWA = viewModel.FstLevFailureLocation;
            gZShenQing.GZBWB = viewModel.SecLevFailureLocation;
            gZShenQing.GZBWC = viewModel.ThiLevFailureLocation;
            gZShenQing.GZXianXiang = viewModel.FailureAppearance;
            gZShenQing.GZMS = viewModel.FailureDescription;
            gZShenQing.Id = viewModel.Id;
            gZShenQing.SFKYXG = viewModel.Modifiable;
            gZShenQing.SBBH = viewModel.DeviceNo;
            return gZShenQing;
        }
        public bool UpDateFailureInfo(int id, MaintenanceApplicationViewModel GZShenQingViewModel)
        {
            if (GZShenQingViewModel != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {

                        bool result = client.Update<GZShenQing>(
                              new
                              {
                                  fssj = GZShenQingViewModel.BeginTime,
                                  gzms = GZShenQingViewModel.FailureDescription,
                                  gzxianxiang = GZShenQingViewModel.FailureAppearance,
                                  gzbwa = GZShenQingViewModel.FstLevFailureLocation,
                                  gzbwb = GZShenQingViewModel.SecLevFailureLocation,
                                  gzbwc = GZShenQingViewModel.ThiLevFailureLocation
                              },
                              it => it.Id == id).ObjToBool();
                        return result;
                    }


                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
                return false;
        }
        public bool DeleteFailureByID(int id)
        {
            using (var client = DbConfig.GetInstance())
            {
                return client.Delete<GZShenQing>(it => it.Id == id).ObjToBool();
            }
        }

    }
}
