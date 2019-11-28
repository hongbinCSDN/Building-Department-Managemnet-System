using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingAckOtherBLService
    {
        private ProcessingAckOtherDAOService _DA;
        protected ProcessingAckOtherDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new ProcessingAckOtherDAOService());
            }
        }

        public ServiceResult UpdateDROverCounter(Fn01LM_OtherDROverCounterModel model)
        {
            return DA.UpdateDROverCounter(model);
        }

        public ServiceResult UpdateAckOrderRelatedStatus(Fn01LM_OtherChangeORS model)
        {
            return DA.UpdateAckOrderRelatedStatus(model);
        }
        public ServiceResult GetOrderRelated (string dsn)
        {
            return DA.GetOrderRelated(dsn);
        }

        public ServiceResult UpdateAckFileReferrNo(Fn01LM_OtherUpdateFRN model)
        {
            return DA.UpdateAckFileReferrNo(model);
        }

        public ServiceResult UpdateAckReferrDate(Fn01LM_OtherBatchUpdateRD model)
        {
            return DA.UpdateAckReferrDate(model);
        }

    }
}