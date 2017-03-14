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
        public bool Dispatch(FailureProcessViewModel failureProcessVM, string workType)
        {

            using (var client = DbConfig.GetInstance())
            { 
                client.BeginTran(); 
                client.CommandTimeOut = 30000;
                GZXX failureInfo = new GZXX();
                PGD dispatchSheet = null;
                ZDPGD diagnosticSheet = null;
                if (String.Equals(workType, "维修"))
                {
                    dispatchSheet = new PGD();
                    dispatchSheet.ID = client.Queryable<PGD>().Max(it => it.ID).ObjToInt() + 1;
                    dispatchSheet.PGRID = 0;
                    dispatchSheet.GZSHENQINGID = failureProcessVM.MaintenanceApplicationViewModel.Id;
                    dispatchSheet.PGSJ = DateTime.Now;
                    dispatchSheet.WXRID = failureProcessVM.EngineerViewModel.EngineerId;
                    dispatchSheet.ZSSX = failureProcessVM.Instruction;
                    failureInfo.PGDID = dispatchSheet.ID;
                    client.Insert<PGD>(dispatchSheet);
                }
                if (String.Equals(workType,"故障诊断"))
                {
                    diagnosticSheet = new ZDPGD();
                    diagnosticSheet.ID = client.Queryable<ZDPGD>().Max(it => it.ID).ObjToInt() + 1;
                    diagnosticSheet.PGRID = 0;
                    diagnosticSheet.GZSHENQINGID = failureProcessVM.MaintenanceApplicationViewModel.Id;
                    diagnosticSheet.PGSJ = DateTime.Now;
                    diagnosticSheet.ZDRID = failureProcessVM.EngineerViewModel.EngineerId;
                    diagnosticSheet.ZSSX = failureProcessVM.Instruction;
                    failureInfo.ZDPGDID = diagnosticSheet.ID;
                    client.Insert<ZDPGD>(diagnosticSheet);
                }
                failureInfo.ID = client.Queryable<GZXX>().Max(it => it.ID).ObjToInt() + 1;
                failureInfo.GZSHENQINGID = failureProcessVM.MaintenanceApplicationViewModel.Id;
                try
                {
                    client.Insert<GZXX>(failureInfo);
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


        public bool Reject(string applicationId, string rejectReason)
        {
            using (var client = DbConfig.GetInstance())
            {
                try
                {
                    bool result = client.Update<GZShenQing>(
                        new
                        {
                            HFSJ = DateTime.Now,
                            HFXX = rejectReason,
                            DQZT = CommonService.StatusDic["Reject"]
                        }, it => it.Id == Convert.ToInt32(applicationId)).ObjToBool();
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
