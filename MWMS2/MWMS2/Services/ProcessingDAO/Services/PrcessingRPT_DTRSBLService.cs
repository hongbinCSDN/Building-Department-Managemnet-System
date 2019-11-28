using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Services.ProcessingDAO.DAO;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class PrcessingRPT_DTRSBLService
    {
        private PrcessingRPT_DTRSDAOService _DA;
        protected PrcessingRPT_DTRSDAOService DA
        {
            get { return _DA ?? (_DA = new PrcessingRPT_DTRSDAOService()); }
        }

        public string QueryWhereString(Fn10RPT_DTRSSearchModel model,string type)
        {
            StringBuilder queryString = new StringBuilder();
            if (model.DocumentType == ProcessingConstant.INCOMING || type == ProcessingConstant.INCOMING)
            {
                queryString.Append(" Where sv.Code in ('RD_DELIVERED','REGISTRY_RECEIPT_COUNTED','REGISTRY_RECEIVED','DSN_RD_RE_SENT','DSN_RD_MISSING','TBC','RD_DELIVER_COUNTED') ");
            }
            else if (model.DocumentType == ProcessingConstant.OUTGOING || type == ProcessingConstant.OUTGOING)
            {
                queryString.Append(" Where sv.Code in ('REGISTRY_DELIVERED','DSN_REGISTRY_DELIVERED_COUNTED','RD_RECEIPT_COUNTED','RD_RECEIVED','DSN_REGISTRY_RE_SENT','DSN_REGISTRY_MISSING','REGISTRY_NEW') ");
            }
            if (!string.IsNullOrEmpty(model.DSN))
            {
                queryString.Append(" And regexp_like(dsn.DSN,:DSN,'i') ");
                model.QueryParameters.Add("DSN", model.DSN);
            }
            if (!string.IsNullOrEmpty(model.RefNo))
            {
                queryString.Append(" And regexp_like(dsn.RECORD_ID,:RefNo,'i')");
                model.QueryParameters.Add("RefNo", model.RefNo);
            }
            if (!string.IsNullOrEmpty(model.PeriodFromDate))
            {
                queryString.Append(" And dsn.MWU_RECEIVED_DATE >= :PeriodFromDate ");
                model.QueryParameters.Add("PeriodFromDate", Convert.ToDateTime(model.PeriodFromDate));
            }
            if (!string.IsNullOrEmpty(model.PeriodToDate))
            {
                queryString.Append(" And dsn.MWU_RECEIVED_DATE <= :PeriodToDate ");
                model.QueryParameters.Add("PeriodToDate", Convert.ToDateTime(model.PeriodToDate));
            }
            return queryString.ToString();
        }

        public Fn10RPT_DTRSSearchModel Search(Fn10RPT_DTRSSearchModel model,string type)
        {
           
            model.QueryWhere = QueryWhereString(model,type);
            model.Sort = "dsn.DSN";
            return DA.Search(model);
        }

        public string IncomingExportExcel(Fn10RPT_DTRSSearchModel model, string type)
        {
            model.QueryWhere = QueryWhereString(model, type);
            return DA.IncomingExportExcel(model);
        }

        public string OutgoingExportExcel(Fn10RPT_DTRSSearchModel model, string type)
        {
            model.QueryWhere = QueryWhereString(model, type);
            return DA.OutgoingExportExcel(model);
        }
    }
}