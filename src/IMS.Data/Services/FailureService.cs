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
        public bool AddNewFailure(MaintenanceApplicationViewModel maintenanceApplicationVM)
        {
            if (maintenanceApplicationVM != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {
                        GZShenQing gZShenQing = MaintenanceApplicationViewModelToModel(maintenanceApplicationVM);
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
        private GZShenQing MaintenanceApplicationViewModelToModel(MaintenanceApplicationViewModel viewModel)
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
            gZShenQing.DQZT = viewModel.Status;

            return gZShenQing;
        }
        public bool UpDateApplication(int id, MaintenanceApplicationViewModel maintenanceApplicationVM)
        {
            bool isUpdateSuccess=false;
            if (maintenanceApplicationVM != null)
            {
                try
                {
                    using (var client = DbConfig.GetInstance())
                    {
                       isUpdateSuccess = client.Update<GZShenQing>(
                              new
                              {
                                  gzms = maintenanceApplicationVM.FailureDescription,
                                  gzxianxiang = maintenanceApplicationVM.FailureAppearance,
                                  gzbwa = maintenanceApplicationVM.FstLevFailureLocation,
                                  gzbwb = maintenanceApplicationVM.SecLevFailureLocation,
                                  gzbwc = maintenanceApplicationVM.ThiLevFailureLocation
                              },
                              it => it.Id == id).ObjToBool();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return isUpdateSuccess;
        }
        public bool DeleteApplicationById(int id)
        {
            using (var client = DbConfig.GetInstance())
            {
                return client.Delete<GZShenQing>(it => it.Id == id).ObjToBool();
            }
        }

    }
}
