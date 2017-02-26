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
        public List<GZShenQingViewModel> GetAllApplicationsByName(string name)
        {

            using (var client = DbConfig.GetInstance())
            {
                List<GZShenQing> listWithSBBH = client.Queryable<GZShenQing>().Where(c => c.bgrxm == name).ToList();
                List<GZShenQingViewModel> gZShenQingViewModelList = new List<GZShenQingViewModel>();
                foreach (var item in listWithSBBH)
                {
                    GZShenQingViewModel gZSQ = new GZShenQingViewModel();
                    gZSQ.SBJC = CommonService.GetShortName(item.SBBH);
                    gZSQ.SBBH = item.SBBH;
                    gZSQ.bgrxm = item.bgrxm;
                    gZSQ.bgsj = item.bgsj;
                    gZSQ.fssj = item.fssj;
                    gZSQ.GZBWA = item.GZBWA;
                    gZSQ.GZBWB = item.GZBWB;
                    gZSQ.GZBWC = item.GZBWC;
                    gZSQ.gzms = item.gzms;
                    gZSQ.gzxianxiang = item.gzxianxiang;
                    gZSQ.Id = item.Id;
                    gZSQ.sfkyxg = item.sfkyxg;
                    gZSQ.GZBWForAll = item.GZBWA + (item.GZBWB == "请选择" ? "" : "/" + item.GZBWB)+(item.GZBWC=="请选择"? "":"/"+item.GZBWC);
                    gZShenQingViewModelList.Add(gZSQ);
                }
                return gZShenQingViewModelList;
            }

        }
        public List<SBXXViewModel> GetShortNameList()
        {
            List<SBXXViewModel> shortNameList = new List<SBXXViewModel>();
            using (var client = DbConfig.GetInstance())
            {
              IEnumerable<SBXX>  ListWithAllInfo = client.Queryable<SBXX>().ToList();
              foreach (var item in ListWithAllInfo)
              {
                  SBXXViewModel viewModel = new SBXXViewModel();
                  viewModel.SBBH = item.SBBH;
                  viewModel.SBJC = item.SBJC;
                  shortNameList.Add(viewModel);
              }
            }

            return shortNameList;
        }
        public bool AddNewFailure(GZShenQingViewModel gZShenQingViewModel)
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
        private GZShenQing GZShenQingViewModelToModel(GZShenQingViewModel viewModel) 
        {
            GZShenQing gZShenQing = new GZShenQing();
            gZShenQing.bgrxm = viewModel.bgrxm;
            gZShenQing.bgsj = viewModel.bgsj;
            gZShenQing.fssj = viewModel.fssj;
            gZShenQing.GZBWA = viewModel.GZBWA;
            gZShenQing.GZBWB = viewModel.GZBWB;
            gZShenQing.GZBWC = viewModel.GZBWC;
            gZShenQing.gzxianxiang = viewModel.gzxianxiang;
            gZShenQing.gzms = viewModel.gzms;
            gZShenQing.Id = viewModel.Id;
            gZShenQing.sfkyxg = viewModel.sfkyxg;
            gZShenQing.SBBH = viewModel.SBBH;
            return gZShenQing;
        }
        public bool UpDateFailureInfo(int id, GZShenQingViewModel GZShenQingViewModel) 
        {
            if (GZShenQingViewModel != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {
                        
                      bool result = client.Update<GZShenQing>(
                            new {
                                fssj=GZShenQingViewModel.fssj,
                                gzms=GZShenQingViewModel.gzms,
                                gzxianxiang=GZShenQingViewModel.gzxianxiang,
                                gzbwa=GZShenQingViewModel.GZBWA,
                                gzbwb=GZShenQingViewModel.GZBWB,
                                gzbwc=GZShenQingViewModel.GZBWC
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
            using (var client=DbConfig.GetInstance())
            {
              return  client.Delete<GZShenQing>(it => it.Id == id).ObjToBool();
            }
        }

    }
}
