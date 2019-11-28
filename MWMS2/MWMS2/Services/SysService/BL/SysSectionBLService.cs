using MWMS2.Areas.Admin.Models;
using MWMS2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class SysSectionBLService
    {
        //SysSectionDAOService
        private static volatile SysSectionDAOService _DA;
        private static readonly object locker = new object();
        private static SysSectionDAOService DA { get { if (_DA == null) lock (locker) if (_DA == null) _DA = new SysSectionDAOService(); return _DA; } }



        public ActionResult Search(Sys_SectionModel model)
        {
            DA.Search(model);
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                ContentType = "application/json"
            };
        }

        public string Excel(Sys_SectionModel model)
        {
            return DA.Excel(model);
        }

        public JsonResult Create(Sys_SectionModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                serviceResult.Result = DA.Create(model.SYS_SECTION)>0? ServiceResult.RESULT_SUCCESS: ServiceResult.RESULT_FAILURE;

            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message = new List<string>() { e.Message };
            }

            return new JsonResult() { Data = serviceResult };

        }

        public void GetRecord(Sys_SectionModel model)
        {
            model.SYS_SECTION = DA.GetRecord(model.SYS_SECTION.UUID);
        }

        public JsonResult Update(Sys_SectionModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                serviceResult.Result = DA.Update(model.SYS_SECTION) > 0 ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;
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