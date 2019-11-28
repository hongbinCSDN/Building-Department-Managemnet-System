using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class DirectReturnRosterBLService
    {
        private DirectReturnRosterDAOService _daoService;
        protected DirectReturnRosterDAOService daoService
        {
            get { return _daoService ?? (_daoService = new DirectReturnRosterDAOService()); }
        }

        public string Search(DirectReturnRosterModel model)
        {
            daoService.Search(model);

            return JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        public string ExportExcel(DirectReturnRosterModel model)
        {
            return daoService.ExportExcel(model);
        }

        public ServiceResult Add(DirectReturnRosterModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    ServiceResult serviceResult = new ServiceResult();
                    try
                    {
                        daoService.Add(model.P_S_DIRECT_RETURN_ROSTER, db);

                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        serviceResult.Message = new List<string> { e.Message };
                        AuditLogService.logDebug(e);
                    }

                    return serviceResult;
                }
            }
        }

        public ServiceResult Update(DirectReturnRosterModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    ServiceResult serviceResult = new ServiceResult();
                    try
                    {
                        daoService.Update(model.P_S_DIRECT_RETURN_ROSTER, db);

                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        serviceResult.Message = new List<string> { e.Message };
                        AuditLogService.logDebug(e);
                    }

                    return serviceResult;
                }
            }
        }

        public ServiceResult Delete(DirectReturnRosterModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    ServiceResult serviceResult = new ServiceResult();
                    try
                    {
                        daoService.Delete(model.P_S_DIRECT_RETURN_ROSTER, db);

                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        serviceResult.Message = new List<string> { e.Message };
                        AuditLogService.logDebug(e);
                    }

                    return serviceResult;
                }
            }
        }

        public ServiceResult GetRosterInfoByDate(DateTime? onDutyDate)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                if (onDutyDate != null)
                {
                    serviceResult.Data = daoService.GetRosterInfoByDate(onDutyDate.Value);
                }
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string> { e.Message };
                AuditLogService.logDebug(e);
            }


            return serviceResult;
        }
    }
}