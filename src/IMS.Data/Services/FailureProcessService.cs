using IMS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleSugar;
using IMS.Model.Model;

namespace IMS.Data.Services
{
    public class FailureProcessService
    {
        public bool Dispatch(FailureProcessViewModel failureProcessVM)
        {
            using (var client = DbConfig.GetInstance())
            {
                PGD paiGongDan = new PGD();
                GZXX gzxx = new GZXX();
                paiGongDan.ID = client.Queryable<PGD>().Max(it => it.ID).ObjToInt() + 1;
                paiGongDan.PGRID = 0;
                paiGongDan.GZSHENQINGID = failureProcessVM.MaintenanceApplicationViewModel.Id;
                paiGongDan.PGSJ = DateTime.Now;
                paiGongDan.WXRID = failureProcessVM.EngineerViewModel.EngineerId;
                paiGongDan.ZSSX = failureProcessVM.Instruction;

                gzxx.ID = client.Queryable<GZXX>().Max(it => it.ID).ObjToInt() + 1;
                gzxx.GZSHENQINGID = failureProcessVM.MaintenanceApplicationViewModel.Id;
                gzxx.PGDID = paiGongDan.ID;

                client.CommandTimeOut = 30000;
                try
                {
                    client.BeginTran();
                    client.Insert<PGD>(paiGongDan);
                    client.Insert<GZXX>(gzxx);
                    client.Update<GZShenQing>(new { SFKYXG = 1, DQZT = CommonService.StatusDic["Dispatched"], HFSJ = DateTime.Now, HFXX = "同意" }, it => it.Id == failureProcessVM.MaintenanceApplicationViewModel.Id);
                    client.CommitTran();
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        public bool Reject(string applicationId,string rejectReason) 
        {
            using (var client=DbConfig.GetInstance())
            {
                try
                {
                    bool result = client.Update<GZShenQing>(
                        new
                        {
                            HFSJ = DateTime.Now,
                            HFXX = rejectReason,
                            DQZT=CommonService.StatusDic["Reject"]
                        }, it => it.Id ==Convert.ToInt32(applicationId)).ObjToBool();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    
    }
}
