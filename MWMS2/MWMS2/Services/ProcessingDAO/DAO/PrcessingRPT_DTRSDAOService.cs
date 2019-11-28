using MWMS2.Areas.MWProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class PrcessingRPT_DTRSDAOService
    {
        private const string Search_q = "Select dsn.*,sv.DESCRIPTION from P_MW_DSN dsn inner join P_S_SYSTEM_VALUE sv on dsn.scanned_status_id = sv.UUID ";

        public Fn10RPT_DTRSSearchModel Search(Fn10RPT_DTRSSearchModel model)
        {
            model.Query = Search_q;
            model.Search();
            return model;
        }

        public string IncomingExportExcel(Fn10RPT_DTRSSearchModel model)
        {
            model.Query = Search_q;
            return model.Export("Incoming Document " + DateTime.Now.ToShortTimeString());
        }
        public string OutgoingExportExcel(Fn10RPT_DTRSSearchModel model)
        {
            model.Query = Search_q;
            return model.Export("Outgoing Document " + DateTime.Now.ToShortTimeString());
        }
    }
}