using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Filter;
using MWMS2.Services.ProcessingDAO.DAO;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingTSKSSBLService
    {
        private ProcessingTSKSSDAService _DA;
        protected ProcessingTSKSSDAService DA
        {
            get
            {
                return _DA ?? (_DA = new ProcessingTSKSSDAService());
            }
        }

        public Fn03TSK_SSSearchModel Search(Fn03TSK_SSSearchModel model)
        {
            try { 
                return DA.Search(model);
            }catch(Exception e)
            {
                AuditLogService.logDebug(e);
            }
            return null;
        }

        public string Excel(Fn03TSK_SSSearchModel model)
        {
            return DA.Excel(model);
        }
    }
}