using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class PEM_UnitBLService
    {
        //PEM_UnitDAOService
        private PEM_UnitDAOService DAService;
        protected PEM_UnitDAOService DA
        {
            get { return DAService ?? (DAService = new PEM_UnitDAOService()); }
        }

        public ActionResult Search(PEM_UnitModel model)
        {
            DA.Search(model);
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        public JsonResult Create(PEM_UnitModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                //Get Section UUID 
                string sSql = @"SELECT *
                                FROM   SYS_SECTION
                                WHERE  CODE = :CODE ";

                OracleParameter[] oracleParams = new OracleParameter[]
                {
                    new OracleParameter(":CODE",model.SectionCode)
                };

                SYS_SECTION SectionRecord =  DA.GetObjectData<SYS_SECTION>(sSql, oracleParams);

                if(SectionRecord == null)
                {
                    Dictionary<string, List<string>> errorMessage = new Dictionary<string, List<string>>();
                    // Error Info
                    List<string> listString = new List<string>();
                    listString.Add("Section code does not exists , please check it");
                    errorMessage.Add("SectionCode", listString);
                    serviceResult.ErrorMessages = errorMessage;
                    return new JsonResult() { Data = serviceResult };
                }

                model.SYS_UNIT.SYS_SECTION_ID = SectionRecord.UUID;

                serviceResult.Result = DA.Create(model.SYS_UNIT) > 0 ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;

            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };

        }

        public void GetRecord(PEM_UnitModel model)
        {
            model.SYS_UNIT = DA.GetRecord(model.SYS_UNIT.UUID);
            //Get Section UUID 
            string sSql = @"SELECT *
                                FROM   SYS_SECTION
                                WHERE  UUID = :UUID ";

            OracleParameter[] oracleParams = new OracleParameter[]
            {
                    new OracleParameter(":UUID",model.SYS_UNIT.SYS_SECTION_ID)
            };
            SYS_SECTION SectionRecord = DA.GetObjectData<SYS_SECTION>(sSql, oracleParams);

            model.SectionCode = SectionRecord.CODE;
        }

        public string Export(PEM_UnitModel model)
        {
            return DA.Export(model);
        }

        public JsonResult Update(PEM_UnitModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                //Get Section UUID 
                string sSql = @"SELECT *
                                FROM   SYS_SECTION
                                WHERE  CODE = :CODE ";

                OracleParameter[] oracleParams = new OracleParameter[]
                {
                    new OracleParameter(":CODE",model.SectionCode)
                };

                SYS_SECTION SectionRecord = DA.GetObjectData<SYS_SECTION>(sSql, oracleParams);

                if (SectionRecord == null)
                {
                    Dictionary<string, List<string>> errorMessage = new Dictionary<string, List<string>>();
                    // Error Info
                    List<string> listString = new List<string>();
                    listString.Add("Section code does not exists , please check it");
                    errorMessage.Add("SectionCode", listString);
                    serviceResult.ErrorMessages = errorMessage;
                    return new JsonResult() { Data = serviceResult };
                }

                model.SYS_UNIT.SYS_SECTION_ID = SectionRecord.UUID;

                serviceResult.Result = DA.Update(model.SYS_UNIT) > 0 ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };

        }
    }
}