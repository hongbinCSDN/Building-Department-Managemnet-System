using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class SginboardDataExportService : BaseCommonService
    {
        public Fn04RPT_SVSBHSearchModel SVSBHIndex(Fn04RPT_SVSBHSearchModel model)
        {
            model.SVSBBYPeriodFromDate = new DateTime(2013, 9, 02);
            model.SVSBBYPeriodToDate = DateTime.Now;
            model.PeriodFromDate = new DateTime(2013, 9, 02);
            model.PeriodToDate = DateTime.Now;
            return new Fn04RPT_SVSBHSearchModel
            {
                SVSBBYPeriodFromDate = model.SVSBBYPeriodFromDate,
                SVSBBYPeriodToDate = model.SVSBBYPeriodToDate,
                PeriodFromDate = model.PeriodFromDate,
                PeriodToDate = model.PeriodToDate
            };
        }
        public static List<DateTime> getYearList(DateTime fromDate, DateTime toDate)
        {
            List<DateTime> getYearList = new List<DateTime>();
            DateTime target = fromDate;
            while (target < toDate)
            {
                getYearList.Add(target);
                target = target.AddYears(1);
            }
            //getYearList.Add(target);

            return getYearList;
        }
        public static List<DateTime> getFormDateList(List<DateTime> getDateList)
        {
            List<DateTime> FormDateResult = new List<DateTime>();
            DateTime formdate;
            for (int i=0;i < getDateList.Count();i++)
            {
                formdate = new DateTime(getDateList[i].Year, 1,1);
                FormDateResult.Add(formdate);
            }
            return FormDateResult;
        }
        public static List<DateTime> getToDateList(List<DateTime> getDateList)
        {
            List<DateTime> ToDateResult = new List<DateTime>();
            DateTime Todate;
            for (int i = 0; i < getDateList.Count(); i++)
            {
                Todate = new DateTime(getDateList[i].Year, 12, 31);
                ToDateResult.Add(Todate);
            }
            return ToDateResult;
        }

        public FileStreamResult ExportSVSYearReport(Fn04RPT_SVSBHSearchModel model)
        {
            List<List<object>> data = new List<List<object>>();
            List<DateTime> YearOfHeaderList = new List<DateTime>();
            List<List<object>> dataList = new List<List<object>>();
            List<List<object>> dataResult = new List<List<object>>();
            List<string> headerList = new List<string>();
            List<DateTime> FromDateResult = new List<DateTime>();
            List<DateTime> ToDateResult = new List<DateTime>();

            string toYear = "";
            string fromYear = "";
            string todate = "";
            if (model.SVSBBYPeriodToDate != null)
            {
                toYear = model.SVSBBYPeriodToDate.Value.Year.ToString();
                todate = model.SVSBBYPeriodToDate.ToString();
            }
            else
            {
                model.SVSBBYPeriodToDate = DateTime.Now;
            }
            if (model.SVSBBYPeriodFromDate != null)
            {
                fromYear = model.SVSBBYPeriodFromDate.Value.Year.ToString();
            }

            YearOfHeaderList = getYearList(model.SVSBBYPeriodFromDate.Value, model.SVSBBYPeriodToDate.Value);

            headerList.Add("Year");
            for (int i = 0; i < YearOfHeaderList.Count(); i++)
            {
                headerList.Add(YearOfHeaderList[i].Year.ToString());
            }

            headerList.Add(toYear + "(as at " + todate + ")");
            headerList.Add(fromYear + " to " + toYear + "(as at" + todate + ")");
            //last dat result
            DateTime lastYear = new DateTime(model.SVSBBYPeriodToDate.Value.Year, 1, 1);
            FromDateResult = getFormDateList(YearOfHeaderList);
            FromDateResult.Add(lastYear);
            ToDateResult = getToDateList(YearOfHeaderList);
            ToDateResult.Add(model.SVSBBYPeriodToDate.Value);
            //Last col result
            DateTime LastfromYear = new DateTime(model.SVSBBYPeriodFromDate.Value.Year, 1, 1);
            FromDateResult.Add(LastfromYear);
            ToDateResult.Add(model.SVSBBYPeriodToDate.Value);
            List<List<object>> ROWHEADER = new List<List<object>>();
            ROWHEADER.AddRange(SVSROWHEADER());
            for (int k = 0; k < YearOfHeaderList.Count()+2; k++)
            {
                ROWHEADER.AddRange(SVSDataCount(FromDateResult[k], ToDateResult[k]));
            }
            for (int yy = 0; yy < 9; yy++)
            {
                List<object> row = new List<object>();
                for (int xx = 0; xx < YearOfHeaderList.Count()+3; xx++)
                {
                    //data.Add(a[0 + xx * 9]);
                    var temp = ROWHEADER[yy + xx * 10][0];
                    row.Add(temp);
                }
                data.Add(row);
            }
            
            return this.exportExcelFile("Summary of validation submission being handled (by year)", headerList, data);
        }
        private List<List<object>> SVSROWHEADER()
        {
            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            string queryStr = ""
                + "\r\n" + "\t" + "select VFORM from (                                        "
                + "\r\n" + "\t" + "  select tb1.ordering,tb1.VForm from (                     "
                + "\r\n" + "\t" + "      select                                               "
                + "\r\n" + "\t" + "      1 as ordering,                                       "
                + "\r\n" + "\t" + "      'No. of validation forms received' as VForm          "
                + "\r\n" + "\t" + "      from dual                                            "
                + "\r\n" + "\t" + "      Union                                                "
                + "\r\n" + "\t" + "      select                                               "
                + "\r\n" + "\t" + "      2 as ordering,                                       "
                + "\r\n" + "\t" + "      'No. of validation forms acknowledged'  as VForm     "
                + "\r\n" + "\t" + "      from dual                                            "
                + "\r\n" + "\t" + "      Union                                                "
                + "\r\n" + "\t" + "      select                                               "
                + "\r\n" + "\t" + "      3 as ordering,                                       "
                + "\r\n" + "\t" + "      'No. of validation forms rejected'  as VForm         "
                + "\r\n" + "\t" + "      from dual                                            "
                + "\r\n" + "\t" + "      Union                                                "
                + "\r\n" + "\t" + "      select                                               "
                + "\r\n" + "\t" + "      4 as ordering,                                       "
                + "\r\n" + "\t" + "      'No. of validation forms withdrawn'  as VForm        "
                + "\r\n" + "\t" + "      from dual                                            "
                + "\r\n" + "\t" + "     Union                                                 "
                + "\r\n" + "\t" + "      select                                               "
                + "\r\n" + "\t" + "      5 as ordering,                                       "
                + "\r\n" + "\t" + "      'No. of validation forms being processed'  as VForm  "
                + "\r\n" + "\t" + "      from dual                                            "
                + "\r\n" + "\t" + "      Union                                                "
                + "\r\n" + "\t" + "      select                                               "
                + "\r\n" + "\t" + "      6 as ordering,                                       "
                + "\r\n" + "\t" + "      'No. of signboard Expired'  as VForm                 "
                + "\r\n" + "\t" + "      from dual                                            "
                + "\r\n" + "\t" + "      Union                                                "
                + "\r\n" + "\t" + "      select                                               "
                + "\r\n" + "\t" + "      7 as ordering,                                       "
                + "\r\n" + "\t" + "      'No. of signboard validated'  as VForm               "
                + "\r\n" + "\t" + "      from dual                                            "
                + "\r\n" + "\t" + "      Union                                                "
                + "\r\n" + "\t" + "      select                                               "
                + "\r\n" + "\t" + "      8 as ordering,                                       "
                + "\r\n" + "\t" + "      'No. of signboard rejected'  as VForm                "
                + "\r\n" + "\t" + "      from dual                                            "
                + "\r\n" + "\t" + "      Union                                                "
                + "\r\n" + "\t" + "      select                                               "
                + "\r\n" + "\t" + "      9 as ordering,                                       "
                + "\r\n" + "\t" + "      'No. of signboard withdrawn'  as VForm               "
                + "\r\n" + "\t" + "      from dual                                            "
                + "\r\n" + "\t" + "      Union                                                "
                + "\r\n" + "\t" + "      select                                               "
                + "\r\n" + "\t" + "      10 as ordering,                                       "
                + "\r\n" + "\t" + "      'No. of signboard being prorocessed'  as VForm       "
                + "\r\n" + "\t" + "      from dual                                            "
                + "\r\n" + "\t" + "  ) tb1                                                    "
                + "\r\n" + "\t" + "  order by tb1.ordering							          "
                + "\r\n" + "\t" + ")                                                          "
                ;
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }
        private List<List<object>> SVSDataCount(DateTime FormDate, DateTime ToDate)
        {
            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            string whereQ = "";
            string ValwhereQ = "";
            if (FormDate != null)
            {
                whereQ += "\r\n" + "\t" + "and sv.RECEIVED_DATE >=:FormDate";
                queryParameters.Add("FormDate", FormDate);
            }
            if (ToDate != null)
            {
                whereQ += "\r\n" + "\t" + "and sv.RECEIVED_DATE >=:ToDate";
                queryParameters.Add("ToDate", ToDate);
            }
            if (FormDate != null)
            {
                ValwhereQ += "\r\n" + "\t" + "and CREATED_DATE >=:ValFormDate";
                queryParameters.Add("ValFormDate", FormDate);
            }
            if (ToDate != null)
            {
                ValwhereQ += "\r\n" + "\t" + "and CREATED_DATE >=:valToDate";
                queryParameters.Add("valToDate", ToDate);
            }
            string queryStr = ""
                    + "\r\n" + "\t" + "select FORMCOUNT from (                                    		  "
                    + "\r\n" + "\t" + "  select tb1.ordering,tb1.VForm,tb1.FormCount from (               "
                    + "\r\n" + "\t" + "      select                                                       "
                    + "\r\n" + "\t" + "      1 as ordering,                                               "
                    + "\r\n" + "\t" + "      'No. of validation forms received' as VForm,                 "
                    + "\r\n" + "\t" + "      count(*) as FormCount                                        "
                    + "\r\n" + "\t" + "      from b_sv_record sv                                          "
                    + "\r\n" + "\t" + "      where 1=1                                                    "
                    + "\r\n" + "\t" + whereQ
                    + "\r\n" + "\t" + "      Union                                                        "
                    + "\r\n" + "\t" + "      select                                                       "
                    + "\r\n" + "\t" + "      2 as ordering,                                               "
                    + "\r\n" + "\t" + "      'No. of validation forms acknowledged'  as VForm,            "
                    + "\r\n" + "\t" + "      count(*) as FormCount                                        "
                    + "\r\n" + "\t" + "      from b_sv_validation                                         "
                    + "\r\n" + "\t" + "      where 1=1                                                    "
                    + "\r\n" + "\t" + "      and VALIDATION_RESULT = 'A'                                  "
                    + "\r\n" + "\t" + ValwhereQ
                    + "\r\n" + "\t" + "      Union                                                        "
                    + "\r\n" + "\t" + "      select                                                       "
                    + "\r\n" + "\t" + "      3 as ordering,                                               "
                    + "\r\n" + "\t" + "      'No. of validation forms rejected'  as VForm,                "
                    + "\r\n" + "\t" + "      count(*) as FormCount                                        "
                    + "\r\n" + "\t" + "      from b_sv_validation                                         "
                    + "\r\n" + "\t" + "      where 1=1                                                    "
                    + "\r\n" + "\t" + "      AND VALIDATION_RESULT = 'R'                                  "
                    + "\r\n" + "\t" + ValwhereQ
                    + "\r\n" + "\t" + "      Union                                                        "
                    + "\r\n" + "\t" + "      select                                                       "
                    + "\r\n" + "\t" + "      4 as ordering,                                               "
                    + "\r\n" + "\t" + "      'No. of validation forms withdrawn'  as VForm,               "
                    + "\r\n" + "\t" + "      count(*) as FormCount                                        "
                    + "\r\n" + "\t" + "      from b_sv_validation                                         "
                    + "\r\n" + "\t" + "      where 1=1                                                    "
                    + "\r\n" + "\t" + "      AND VALIDATION_RESULT = 'W'                                  "
                    + "\r\n" + "\t" + ValwhereQ
                    + "\r\n" + "\t" + "     Union                                                         "
                    + "\r\n" + "\t" + "      select                                                       "
                    + "\r\n" + "\t" + "      5 as ordering,                                               "
                    + "\r\n" + "\t" + "      'No. of validation forms being processed'  as VForm,         "
                    + "\r\n" + "\t" + "      count(*) as FormCount                                        "
                    + "\r\n" + "\t" + "      from b_sv_validation                                         "
                    + "\r\n" + "\t" + "      where 1=1                                                    "
                    + "\r\n" + "\t" + "      AND VALIDATION_RESULT is null                                "
                    + "\r\n" + "\t" + ValwhereQ
                    + "\r\n" + "\t" + "      Union                                                        "
                    + "\r\n" + "\t" + "      select                                                       "
                    + "\r\n" + "\t" + "      6 as ordering,                                               "
                    + "\r\n" + "\t" + "      'No. of signboard validated'  as VForm,                      "
                    + "\r\n" + "\t" + "      count(*) as FormCount                                        "
                    + "\r\n" + "\t" + "      from b_sv_record  sv                                         "
                    + "\r\n" + "\t" + "      where 1=1                                                    "
                    + "\r\n" + "\t" + "      AND NO_OF_SIGNBOARD_VALIDATED is not null                    "
                    + "\r\n" + "\t" + whereQ
                    + "\r\n" + "\t" + "      Union                                                        "
                    + "\r\n" + "\t" + "      select                                                       "
                    + "\r\n" + "\t" + "      7 as ordering,                                               "
                    + "\r\n" + "\t" + "      'No. of signboard Expired'  as VForm,                        "
                    + "\r\n" + "\t" + "      count(*) as FormCount                                        "
                    + "\r\n" + "\t" + "      from b_sv_record sv                                          "
                    + "\r\n" + "\t" + "      where 1=1                                                    "
                    + "\r\n" + "\t" + "      AND (RECEIVED_DATE > RECEIVED_DATE+1825)                     "
                    + "\r\n" + "\t" + whereQ
                    + "\r\n" + "\t" + "      Union                                                        "
                    + "\r\n" + "\t" + "      select                                                       "
                    + "\r\n" + "\t" + "      8 as ordering,                                               "
                    + "\r\n" + "\t" + "      'No. of signboard rejected'  as VForm,                       "
                    + "\r\n" + "\t" + "      count(*) as FormCount                                        "
                    + "\r\n" + "\t" + "      from b_sv_record sv                                          "
                    + "\r\n" + "\t" + "	     inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid "
                    + "\r\n" + "\t" + "      where 1=1                                                    "
                    + "\r\n" + "\t" + "	     AND val.VALIDATION_RESULT = 'R'                              "
                    + "\r\n" + "\t" + whereQ
                    + "\r\n" + "\t" + "      Union                                                        "
                    + "\r\n" + "\t" + "      select                                                       "
                    + "\r\n" + "\t" + "      9 as ordering,                                               "
                    + "\r\n" + "\t" + "      'No. of signboard withdrawn'  as VForm,                      "
                    + "\r\n" + "\t" + "      count(*) as FormCount                                        "
                    + "\r\n" + "\t" + "      from b_sv_record sv                                          "
                    + "\r\n" + "\t" + "      inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid "
                    + "\r\n" + "\t" + "      where 1=1                                                    "
                    + "\r\n" + "\t" + "      AND val.VALIDATION_RESULT = 'W'                              "
                    + "\r\n" + "\t" + whereQ
                    + "\r\n" + "\t" + "      Union                                                        "
                    + "\r\n" + "\t" + "      select                                                       "
                    + "\r\n" + "\t" + "      10 as ordering,                                              "
                    + "\r\n" + "\t" + "      'No. of signboard being prorocessed'  as VForm,              "
                    + "\r\n" + "\t" + "      count(*) as FormCount                                        "
                    + "\r\n" + "\t" + "      from b_sv_record sv                                          "
                    + "\r\n" + "\t" + "      inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid "
                    + "\r\n" + "\t" + "      where 1=1                                                    "
                    + "\r\n" + "\t" + "      AND val.VALIDATION_RESULT = null                             "
                    + "\r\n" + "\t" + whereQ
                    + "\r\n" + "\t" + "  ) tb1                                                            "
                    + "\r\n" + "\t" + "  order by tb1.ordering											  "
                    + "\r\n" + "\t" + ")                                                                  "
                    ;
            //List<List<object>> data = new List<List<object>>();
            //Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            //string whereQ = "";
            //if (FormDate != null)
            //{
            //    whereQ += "\r\n" + "\t" + "and RECEIVED_DATE >=:FormDate";
            //    queryParameters.Add("FormDate", FormDate);
            //}
            //if(ToDate != null)
            //{
            //    whereQ += "\r\n" + "\t" + "and RECEIVED_DATE >=:ToDate";
            //    queryParameters.Add("ToDate", ToDate);
            //}
            //string queryStr = ""
            //        + "\r\n" + "\t" + "select FORMCOUNT from (                                    "
            //        + "\r\n" + "\t" + "  select tb1.ordering,tb1.VForm,tb1.FormCount from (               "
            //        + "\r\n" + "\t" + "      select                                                       "
            //        + "\r\n" + "\t" + "      1 as ordering,                                               "
            //        + "\r\n" + "\t" + "      'No. of validation forms received' as VForm,                 "
            //        + "\r\n" + "\t" + "      count(*) as FormCount                                        "
            //        + "\r\n" + "\t" + "      from b_sv_record                                             "
            //        + "\r\n" + "\t" + "      where 1=1                                                    "
            //        + "\r\n" + "\t" + whereQ
            //        + "\r\n" + "\t" + "      Union                                                        "
            //        + "\r\n" + "\t" + "      select                                                       "
            //        + "\r\n" + "\t" + "      2 as ordering,                                               "
            //        + "\r\n" + "\t" + "      'No. of validation forms acknowledged'  as VForm,            "
            //        + "\r\n" + "\t" + "      count(*) as FormCount                                        "
            //        + "\r\n" + "\t" + "      from b_sv_record sv                                          "
            //        + "\r\n" + "\t" + "      inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid "
            //        + "\r\n" + "\t" + "      where 1=1                                                    "
            //        + "\r\n" + "\t" + whereQ
            //        + "\r\n" + "\t" + "      and val.VALIDATION_RESULT = 'A'                              "
            //        + "\r\n" + "\t" + "      Union                                                        "
            //        + "\r\n" + "\t" + "      select                                                       "
            //        + "\r\n" + "\t" + "      3 as ordering,                                               "
            //        + "\r\n" + "\t" + "      'No. of validation forms rejected'  as VForm,                "
            //        + "\r\n" + "\t" + "      count(*) as FormCount                                        "
            //        + "\r\n" + "\t" + "      from b_sv_record sv                                          "
            //        + "\r\n" + "\t" + "      inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid "
            //        + "\r\n" + "\t" + "      where 1=1                                                    "
            //        + "\r\n" + "\t" + "      AND val.VALIDATION_RESULT = 'R'                              "
            //        + "\r\n" + "\t" + whereQ
            //        + "\r\n" + "\t" + "      Union                                                        "
            //        + "\r\n" + "\t" + "      select                                                       "
            //        + "\r\n" + "\t" + "      4 as ordering,                                               "
            //        + "\r\n" + "\t" + "      'No. of validation forms withdrawn'  as VForm,               "
            //        + "\r\n" + "\t" + "      count(*) as FormCount                                        "
            //        + "\r\n" + "\t" + "      from b_sv_record sv                                          "
            //        + "\r\n" + "\t" + "      inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid "
            //        + "\r\n" + "\t" + "      where 1=1                                                    "
            //        + "\r\n" + "\t" + "      AND val.VALIDATION_RESULT = 'W'                              "
            //        + "\r\n" + "\t" + whereQ
            //        + "\r\n" + "\t" + "     Union                                                        "
            //        + "\r\n" + "\t" + "      select                                                       "
            //        + "\r\n" + "\t" + "      5 as ordering,                                               "
            //        + "\r\n" + "\t" + "      'No. of validation forms being processed'  as VForm,         "
            //        + "\r\n" + "\t" + "      count(*) as FormCount                                        "
            //        + "\r\n" + "\t" + "      from b_sv_record sv                                          "
            //        + "\r\n" + "\t" + "      inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid "
            //        + "\r\n" + "\t" + "      where 1=1                                                    "
            //        + "\r\n" + "\t" + "      AND val.VALIDATION_RESULT = 'P'                              "
            //        + "\r\n" + "\t" + whereQ
            //        + "\r\n" + "\t" + "      Union                                                        "
            //        + "\r\n" + "\t" + "      select                                                       "
            //        + "\r\n" + "\t" + "      6 as ordering,                                               "
            //        + "\r\n" + "\t" + "      'No. of signboard validated'  as VForm,                      "
            //        + "\r\n" + "\t" + "      count(*) as FormCount                                        "
            //        + "\r\n" + "\t" + "      from b_sv_record sv                                          "
            //        + "\r\n" + "\t" + "      inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid "
            //        + "\r\n" + "\t" + "      where 1=1                                                    "
            //        + "\r\n" + "\t" + "      AND val.VALIDATION_RESULT = 'V'                              "
            //        + "\r\n" + "\t" + whereQ
            //        + "\r\n" + "\t" + "      Union                                                        "
            //        + "\r\n" + "\t" + "      select                                                       "
            //        + "\r\n" + "\t" + "      7 as ordering,                                               "
            //        + "\r\n" + "\t" + "      'No. of signboard rejected'  as VForm,                       "
            //        + "\r\n" + "\t" + "      count(*) as FormCount                                        "
            //        + "\r\n" + "\t" + "      from b_sv_record sv                                          "
            //        + "\r\n" + "\t" + "      inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid "
            //        + "\r\n" + "\t" + "      where 1=1                                                    "
            //        + "\r\n" + "\t" + "      AND val.VALIDATION_RESULT = 'R'                              "
            //        + "\r\n" + "\t" + whereQ
            //        + "\r\n" + "\t" + "      Union                                                        "
            //        + "\r\n" + "\t" + "      select                                                       "
            //        + "\r\n" + "\t" + "      8 as ordering,                                               "
            //        + "\r\n" + "\t" + "      'No. of signboard withdrawn'  as VForm,                      "
            //        + "\r\n" + "\t" + "      count(*) as FormCount                                        "
            //        + "\r\n" + "\t" + "      from b_sv_record sv                                          "
            //        + "\r\n" + "\t" + "      inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid "
            //        + "\r\n" + "\t" + "      where 1=1                                                    "
            //        + "\r\n" + "\t" + "      AND val.VALIDATION_RESULT = 'W'                              "
            //        + "\r\n" + "\t" + whereQ
            //        + "\r\n" + "\t" + "      Union                                                        "
            //        + "\r\n" + "\t" + "      select                                                       "
            //        + "\r\n" + "\t" + "      9 as ordering,                                               "
            //        + "\r\n" + "\t" + "      'No. of signboard being prorocessed'  as VForm,              "
            //        + "\r\n" + "\t" + "      count(*) as FormCount                                        "
            //        + "\r\n" + "\t" + "      from b_sv_record sv                                          "
            //        + "\r\n" + "\t" + "      inner join b_sv_validation val on val.SV_RECORD_ID = sv.uuid "
            //        + "\r\n" + "\t" + "      where 1=1                                                    "
            //        + "\r\n" + "\t" + "      AND val.VALIDATION_RESULT = 'P'                              "
            //        + "\r\n" + "\t" + whereQ
            //        + "\r\n" + "\t" + "  ) tb1                                                            "
            //        + "\r\n" + "\t" + "  order by tb1.ordering											 "
            //        + "\r\n" + "\t" + ")                                                                  "
            //        ;

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }

        public FileStreamResult ExportSVSUnitReport(Fn04RPT_SVSBHSearchModel model)
        {
            //header from db
            List<string> headerList = new List<string>() {
                "As at "+model.PeriodToDate.ToString(),
                "BS/SC1","BS/SC2", "BS/SC3", "SESC8","SE/SC9","PSOSC1","ABSSC3","ICU","Sub_total"
                ,"BS/SC4","SE/SC2","SE/SC4","PTO/SC1","PSO/SC2","Sub_total"
                ,"SE/SC1","SE/SC3","SE/SC6","CTO/SC","PTO/SC2","Sub_total"
                ,"Total"
             

            };
            List<List<object>> data = new List<List<object>>();
            List<string> SBS_SC_SubList = new List<string>() { "BSSC1", "BSSC2", "BSSC3", "SESC8", "SESC9", "PSOSC1", "ABSSC3","ICU"
                                                             , "BSSC4", "SESC2", "SESC4", "PTOSC1", "PSOSC2"
                                                             , "SESC1", "SESC3", "SESC6","CTOSC","PTOSC2" };
            var AcknowledgedList = SVSUNITDataCount(model.PeriodFromDate.Value, model.PeriodToDate.Value, "Acknowledged", SBS_SC_SubList, "A");
          
            var RejectedList = SVSUNITDataCount(model.PeriodFromDate.Value, model.PeriodToDate.Value, "Rejected", SBS_SC_SubList, "R");
           
            var WithdrawnList = SVSUNITDataCount(model.PeriodFromDate.Value, model.PeriodToDate.Value, "Withdrawn", SBS_SC_SubList, "W");
            
            var BeingProcessedList = SVSUNITDataCount(model.PeriodFromDate.Value, model.PeriodToDate.Value, "BeingProcessed", SBS_SC_SubList, null);
         
            var BeingProcessed45DaysList = SVSUNITDataCount(model.PeriodFromDate.Value, model.PeriodToDate.Value, "BeingProcessed45Days_SBS_SC", SBS_SC_SubList, null);

            data.Add(AddSVSUnitRow("Acknowledged", AcknowledgedList));
            data.Add(AddSVSUnitRow("Rejected", RejectedList));
            data.Add(AddSVSUnitRow("Withdrawn", WithdrawnList));
            data.Add(AddSVSUnitRow("Being processed", BeingProcessedList));
            data.Add(AddSVSUnitRow("Being processed (>45 days)", BeingProcessed45DaysList));

            var TotalRecivedList = new List<List<object>>();
            for (int i=0; i< AcknowledgedList.Count; i++)
            {
                TotalRecivedList.Add(new List<object>() { Convert.ToInt32(AcknowledgedList[i][0].ToString()) + Convert.ToInt32(RejectedList[i][0].ToString())
                           + Convert.ToInt32(WithdrawnList[i][0].ToString()) + Convert.ToInt32(BeingProcessedList[i][0].ToString())
                           + Convert.ToInt32(BeingProcessed45DaysList[i][0].ToString()) });
            }

            data.Add(AddSVSUnitRow("Total Received", TotalRecivedList));

            return this.exportExcelFile("Summary of validation submission being handled (by Unit)", headerList, data);
        }


        public List<object> AddSVSUnitRow(string leftHeader, List<List<object>> resultList)
        {
            return new List<object> { leftHeader, resultList[0][0], resultList[1][0] , resultList[2][0], resultList[3][0]
                                       ,resultList[4][0],resultList[5][0],resultList[6][0],resultList[7][0], CalculateSubTotal(resultList.Take(8))
                                       ,resultList[8][0],resultList[9][0],resultList[10][0],resultList[11][0],resultList[12][0],CalculateSubTotal(resultList.Skip(8).Take(5))
                                       ,resultList[13][0],resultList[14][0],resultList[15][0],resultList[16][0],resultList[17][0],CalculateSubTotal(resultList.Skip(13).Take(5))

                                       ,CalculateSubTotal(resultList.Take(18))
            };
        }

        private List<List<object>> SVSUNITDataCount(DateTime FormDate, DateTime ToDate,string RowName, List<string> modifyByList, string ValidationResult)
        {
            StringBuilder queryAllStr = new StringBuilder();

            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();

            int i = 0;
            foreach(var item in modifyByList)
            {
                StringBuilder queryStr = new StringBuilder(@"select count(*) as " + RowName + @"
                                        from b_sv_validation svv
                                        inner join sys_post sp on sp.code = svv.MODIFIED_BY
                                        inner join b_sv_record sre on sre.uuid = svv.sv_record_id
                                        where 1 = 1 and svv.MODIFIED_BY= '" + item + "'");

                if (!string.IsNullOrEmpty(ValidationResult))
                {
                    queryStr.Append(" And  svv.VALIDATION_RESULT = '" + ValidationResult + "'");
                }
                else
                    queryStr.Append(" And  svv.VALIDATION_RESULT  is null ");

                if (RowName == "BeingProcessed45days")
                {
                    queryStr.Append(" And svv.CREATED_DATE+45 > sysdate ");
                }

                if (!string.IsNullOrEmpty(FormDate.ToString()))
                {
                    queryStr.Append("  and sre.received_date >= :UintfromDate ");
                }
                if (!string.IsNullOrEmpty(ToDate.ToString()))
                {
                    queryStr.Append("  and sre.received_date <= :UnittoDate ");

                }
                if (i + 1 == modifyByList.Count)
                    queryAllStr.Append(queryStr);
                else
                    queryAllStr.Append(queryStr + " union all ");

                i++;
            }

            if(!string.IsNullOrEmpty(FormDate.ToString()))
                queryParameters.Add("UintfromDate", FormDate);
            if (!string.IsNullOrEmpty(ToDate.ToString()))
                queryParameters.Add("UnittoDate", ToDate);


            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryAllStr.ToString(), queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }

        private int CalculateSubTotal(IEnumerable<List<object>> list)
        {
            int total = 0;
            foreach(var item in list)
            {
                total += Convert.ToInt32(item[0]);
            }
            return total;
        }


        private List<List<object>> SVSUNItDataCount()
        {
            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();

            string queryStr = ""
                    + "\r\n" + "\t" + "select VForm from (					       "
                    + "\r\n" + "\t" + "    select tb1.ordering,tb1.VForm from (      "
                    + "\r\n" + "\t" + "        select                                "
                    + "\r\n" + "\t" + "        1 as ordering,                        "
                    + "\r\n" + "\t" + "        'Acknowledged' as VForm               "
                    + "\r\n" + "\t" + "        from dual                             "
                    + "\r\n" + "\t" + "        Union                                 "
                    + "\r\n" + "\t" + "        select                                "
                    + "\r\n" + "\t" + "        2 as ordering,                        "
                    + "\r\n" + "\t" + "        'Rejected'  as VForm                  "
                    + "\r\n" + "\t" + "        from dual                             "
                    + "\r\n" + "\t" + "        Union                                 "
                    + "\r\n" + "\t" + "        select                                "
                    + "\r\n" + "\t" + "        3 as ordering,                        "
                    + "\r\n" + "\t" + "        'Withdrawn'  as VForm                 "
                    + "\r\n" + "\t" + "        from dual                             "
                    + "\r\n" + "\t" + "        Union                                 "
                    + "\r\n" + "\t" + "        select                                "
                    + "\r\n" + "\t" + "        4 as ordering,                        "
                    + "\r\n" + "\t" + "        'Being processed'  as VForm           "
                    + "\r\n" + "\t" + "        from dual                             "
                    + "\r\n" + "\t" + "        Union                                 "
                    + "\r\n" + "\t" + "        select                                "
                    + "\r\n" + "\t" + "        5 as ordering,                        "
                    + "\r\n" + "\t" + "        'Being processed (>45 days)'  as VForm"
                    + "\r\n" + "\t" + "        from dual                             "
                    + "\r\n" + "\t" + "        Union                                 "
                    + "\r\n" + "\t" + "        select                                "
                    + "\r\n" + "\t" + "        6 as ordering,                        "
                    + "\r\n" + "\t" + "        'Total Received'  as VForm            "
                    + "\r\n" + "\t" + "        from dual                             "
                    + "\r\n" + "\t" + "    ) tb1                                     "
                    + "\r\n" + "\t" + "    order by tb1.ordering                     "
                    + "\r\n" + "\t" + ")                                             ";

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }

        public FileStreamResult ExportDataResult(Fn04RPT_DESearchModel model,string UUID)
        {
            List<string> headerList = new List<string>()
            {
                "Submission No.", "Form Code",
                "Received Date", "Expirty Date",
                "Validation Result", "Remarks", "SPO Endorsement Date",
                "Letter Reason", "Letter Remarks",
                "Signboard Address", "Location of Signboard",
                "Depscription", "Facade", "Type",
                "Bottom fixing at Floor", "Top fixing at Floor",
                "Display Area", "Projection",
                "Heigh of Signboard", "Clearance above ground",
                "LED/TV", "Building Portion",
                "RVD No.", "RV Block ID",
                "BCIS Block ID", "BCIS District", "BD ref. (42)",
                "Owner Chinese Name",
                "Owner English Name", "Owner Address",
                "Owner Email", "Owner Contact No",
                "Owner Fax NO", "PAW Chinese Name",
                "PAW English Name", "PAW Address",
                "PAW Email", "PAW Contact No", "PAW Fax NO",
                "IO Chinese Name", "IO English Name", "IO Address",
                "IO Email", "IO Contact No",
                "IO Fax NO", "IO PBP Name",
                "IO PBP Contact No", "IO PRC Name",
                "IO PRC Contact No", "AP Certification No",
                "AP Chinese Name", "AP English Name",
                "AP EN_ADDRESS_LINE1",
                "AP EN_ADDRESS_LINE2", "AP EN_ADDRESS_LINE3",
                "AP EN_ADDRESS_LINE4", "AP EN_ADDRESS_LINE5",
                "AP CN_ADDRESS_LINE1", "AP CN_ADDRESS_LINE2",
                "AP CN_ADDRESS_LINE3", "AP CN_ADDRESS_LINE4",
                "AP CN_ADDRESS_LINE5",
                "AP Contact No", "AP Fax No",
                "RSE Certification No", "RSE Chinese Name",
                "RSE English Name",
                "RSE EN_ADDRESS_LINE1",
                "RSE EN_ADDRESS_LINE2", "RSE EN_ADDRESS_LINE3",
                "RSE EN_ADDRESS_LINE4",
                "RSE EN_ADDRESS_LINE5", "RSE CN_ADDRESS_LINE1",
                "RSE CN_ADDRESS_LINE2",
                "RSE CN_ADDRESS_LINE3", "RSE CN_ADDRESS_LINE4",
                "RSE CN_ADDRESS_LINE5",
                "RSE Contact No", "RSE Fax No",
                "RGE Certification No", "RGE Chinese Name",
                "RGE English Name",
                "RGE EN_ADDRESS_LINE1",
                "RGE EN_ADDRESS_LINE2", "RGE EN_ADDRESS_LINE3",
                "RGE EN_ADDRESS_LINE4",
                "RGE EN_ADDRESS_LINE5", "RGE CN_ADDRESS_LINE1",
                "RGE CN_ADDRESS_LINE2",
                "RGE CN_ADDRESS_LINE3", "RGE CN_ADDRESS_LINE4",
                "RGE CN_ADDRESS_LINE5",
                "RGE Contact No", "RGE Fax No",
                "PRC Certification No", "PRC Chinese Name",
                "PRC English Name","PRC EN_ADDRESS_LINE1",
                "PRC EN_ADDRESS_LINE2", "PRC EN_ADDRESS_LINE3",
                "PRC EN_ADDRESS_LINE4",
                "PRC EN_ADDRESS_LINE5", "PRC CN_ADDRESS_LINE1",
                "PRC CN_ADDRESS_LINE2",
                "PRC CN_ADDRESS_LINE3", "PRC CN_ADDRESS_LINE4",
                "PRC CN_ADDRESS_LINE5",
                "PRC Contact No", "PRC Fax No",
                "PRC AS Chinese Name", "PRC AS English Name"
                };

            List<List<object>> data = new List<List<object>>();
            List<List<object>> dataResult = new List<List<object>>();
            data = ExportVilData(model,UUID);
            dataResult = ExportResult("Validation",data);
            return this.exportExcelFile("Validation", headerList, dataResult);
        }
        public List<List<object>> ExportResult(string type, List<List<object>> data)
        {
            List<List<object>> dataList = new List<List<object>>();
            List<List<object>> CrmPbPPrc = new List<List<object>>();

            if (type == "Validation") {
                for (int i = 0; i < data.Count(); i++)
                {
                    var d = new List<object>();
                    d.Add(data[i][0]);
                    for (int j = 1; j < 4; j++)
                    {
                        d.Add(data[i][j]);//i ==0  
                    }
                    d.Add(data[i][4]);   
                    d.Add(data[i][53]);
                    for (int j = 5; j < 9; j++)
                    {
                        d.Add(data[i][j]);
                    }
                    for (int j = 54; j < 71; j++)
                    {
                        d.Add(data[i][j]);
                    }
                    for (int j = 9; j < 34; j++)
                    {
                        d.Add(data[i][j]);
                    }
                    CrmPbPPrc = getCrmPbpPrc(data[i][31].ToString());

                    if (CrmPbPPrc != null && CrmPbPPrc.Count() > 0)
                    {
                        d.Add(CrmPbPPrc[0][0]);
                        for (int j = 1; j < 10; j++)
                        {
                            d.Add(CrmPbPPrc[0][j]);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            d.Add("");
                        }
                    }
                    d.Add(data[i][34]);
                    d.Add(data[i][35]);
                    d.Add(data[i][36]);
                    d.Add(data[i][37]);
                    d.Add(data[i][38]);
                    CrmPbPPrc = getCrmPbpPrc(data[i][36].ToString());
                    if (CrmPbPPrc != null && CrmPbPPrc.Count() > 0)
                    {
                        d.Add(CrmPbPPrc[0][0]);
                        for (int j = 1; j < 10; j++)
                        {
                            d.Add(CrmPbPPrc[0][j]);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            d.Add("");
                        }
                    }
                    d.Add(data[i][39]);
                    d.Add(data[i][40]);
                    d.Add(data[i][41]);
                    d.Add(data[i][42]);
                    d.Add(data[i][43]);
                    CrmPbPPrc = getCrmPbpPrc(data[i][41].ToString());
                    if (CrmPbPPrc != null && CrmPbPPrc.Count() > 0)
                    {
                        d.Add(CrmPbPPrc[0][0]);
                        for (int j = 1; j < 10; j++)
                        {
                            d.Add(CrmPbPPrc[0][j]);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            d.Add("");
                        }
                    }
                    d.Add(data[i][44]);
                    d.Add(data[i][45]);
                    d.Add(data[i][46]);
                    d.Add(data[i][47]);
                    d.Add(data[i][48]);

                    CrmPbPPrc = getCrmPbpPrc(data[i][46].ToString());
                    if (CrmPbPPrc != null && CrmPbPPrc.Count() > 0)
                    {
                        d.Add(CrmPbPPrc[0][0]);
                        for (int j = 1; j < 10; j++)
                        {
                            d.Add(CrmPbPPrc[0][j]);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            d.Add("");
                        }
                    }
                    d.Add(data[i][49]);
                    d.Add(data[i][50]);
                    d.Add(data[i][51]);
                    d.Add(data[i][52]);
                    dataList.Add(d);
                }
            }
            return dataList;
        }
        private List<List<object>> ExportVilData(Fn04RPT_DESearchModel model,string UUID)
        {
            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            string queryStr = ""
                    + "\r\n" + "\t" + " select "
                    + "\r\n" + "\t" + " svr.reference_no, svr.form_code, svr.received_date, svv.signboard_expiry_date, "
                    + "\r\n" + "\t" + " decode(svv.validation_result, "
                    + "\r\n" + "\t" + " 'A', '" + SignboardConstant.RECOMMEND_ACK_STR + "', "
                    + "\r\n" + "\t" + " 'R', '" + SignboardConstant.RECOMMEND_REF_STR + "', "
                    + "\r\n" + "\t" + " 'C', '" + SignboardConstant.RECOMMEND_COND_STR + "', "
                    + "\r\n" + "\t" + " svv.validation_result) as validation_result, "
                    + "\r\n" + "\t" + " svv.spo_endorsement_date, "
                    + "\r\n" + "\t" + " svv.letter_reason, svv.letter_remarks, "
                    + "\r\n" + "\t" + " sba.full_address, "
                    + "\r\n" + "\t" + " owner.name_chinese as ownerCnaame, owner.name_english as ownerEname, ownera.full_address as owneraddress, owner.email as owneremail, owner.contact_no as ownercontact, owner.fax_no as ownerfaxno, "
                    + "\r\n" + "\t" + " paw.name_chinese as pawCnaame, paw.name_english as pawEname, pawa.full_address as pawaddress, paw.email as pawemail,paw.contact_no as pawcontact, paw.fax_no as pawfaxno, "
                    + "\r\n" + "\t" + " io.name_chinese as ioCnaame, io.name_english as ioEname, ioa.full_address as ioaddress, io.email as ioemail, io.contact_no as iocontact, io.fax_no as iofaxno, io.pbp_name, io.pbp_contact_no, io.prc_name, io.prc_contact_no, "
                    + "\r\n" + "\t" + " ap.certification_no as apcert, ap.chinese_name as apCname, ap.english_name as apEname, ap.contact_no as apcontact, ap.fax_no as apfax, "
                    + "\r\n" + "\t" + " rse.certification_no as rsecert , rse.chinese_name as rseCname, rse.english_name as rseEname, rse.contact_no as rsecontact, rse.fax_no as rsefax, "
                    + "\r\n" + "\t" + " rge.certification_no as rgecert, rge.chinese_name as rgeCname, rge.english_name as rgeEname, rge.contact_no as rgecontact, rge.fax_no as rgefax, "
                    + "\r\n" + "\t" + " prc.certification_no as prccert, prc.chinese_name as prcCname, prc.english_name as prcEname, prc.contact_no as prccontact, prc.fax_no as prcfax, prc.as_chinese_name, prc.as_english_name, svv.remarks, "
                    + "\r\n" + "\t" + " svs.location_of_signboard, svs.description, svs.facade, svs.type, svs.btm_floor, svs.top_floor, svs.a_m2, svs.p_m, svs.h_m, svs.h2_m, svs.led, svs.building_portion, svs.rvd_no, sba.rv_block_id, sba.bcis_block_id, sba.bcis_district, sba.file_reference_no "
                    + "\r\n" + "\t" + " from "
                    + "\r\n" + "\t" + " b_sv_record svr, b_sv_validation svv, b_sv_address sba, b_sv_signboard svs, "
                    + "\r\n" + "\t" + " b_sv_person_contact owner, b_sv_address ownera, "
                    + "\r\n" + "\t" + " b_sv_person_contact paw, b_sv_address pawa, "
                    + "\r\n" + "\t" + " b_sv_person_contact io, b_sv_address ioa, "
                    + "\r\n" + "\t" + " b_sv_appointed_professional ap, b_sv_appointed_professional rse, "
                    + "\r\n" + "\t" + " b_sv_appointed_professional rge, b_sv_appointed_professional prc "
                    + "\r\n" + "\t" + " where 1=1 "
                    + "\r\n" + "\t" + " and svv.uuid in ('" + UUID + "') "
                    + "\r\n" + "\t" + " and svr.uuid = svv.sv_record_id "
                    + "\r\n" + "\t" + " and svs.uuid = svr.sv_signboard_id "
                    + "\r\n" + "\t" + " and sba.uuid = svs.location_address_id "
                    + "\r\n" + "\t" + " and owner.uuid = svs.owner_id and ownera.uuid = owner.sv_address_id "
                    + "\r\n" + "\t" + " and paw.uuid = svr.paw_id and pawa.uuid = paw.sv_address_id "
                    + "\r\n" + "\t" + " and io.uuid = svr.oi_id and ioa.uuid = io.sv_address_id "
                    + "\r\n" + "\t" + " and ap.sv_record_id = svr.uuid  and ap.identify_flag='" + SignboardConstant.PBP_CODE_AP + "'"
                    + "\r\n" + "\t" + " and rse.sv_record_id = svr.uuid  and rse.identify_flag='" + SignboardConstant.PBP_CODE_RSE + "'"
                    + "\r\n" + "\t" + " and rge.sv_record_id = svr.uuid  and rge.identify_flag='" + SignboardConstant.PBP_CODE_RGE + "'"
                    + "\r\n" + "\t" + " and prc.sv_record_id = svr.uuid  and prc.identify_flag='" + SignboardConstant.PRC_CODE + "'";

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }
        public FileStreamResult ExportDataExportByExpiryDate(Fn04RPT_DESearchModel model)
        {
            List<List<object>> data = new List<List<object>>();
            List<List<object>> dataResult = new List<List<object>>();
            List<string> headerList = new List<string>()
            {
                "Submission No.", "Form Code",
                "Received Date", "Expirty Date",
                "Validation Result", "Remarks", "SPO Endorsement Date",
                "Letter Reason", "Letter Remarks",
                "Signboard Address", "Location of Signboard",
                "Depscription", "Facade", "Type",
                "Bottom fixing at Floor", "Top fixing at Floor",
                "Display Area", "Projection",
                "Heigh of Signboard", "Clearance above ground",
                "LED/TV", "Building Portion",
                "RVD No.", "RV Block ID",
                "BCIS Block ID", "BCIS District", "BD ref. (42)",
                "Owner Chinese Name",
                "Owner English Name", "Owner Address",
                "Owner Email", "Owner Contact No",
                "Owner Fax NO", "PAW Chinese Name",
                "PAW English Name", "PAW Address",
                "PAW Email", "PAW Contact No", "PAW Fax NO",
                "IO Chinese Name", "IO English Name", "IO Address",
                "IO Email", "IO Contact No",
                "IO Fax NO", "IO PBP Name",
                "IO PBP Contact No", "IO PRC Name",
                "IO PRC Contact No", "AP Certification No",
                "AP Chinese Name", "AP English Name",
                "AP EN_ADDRESS_LINE1",
                "AP EN_ADDRESS_LINE2", "AP EN_ADDRESS_LINE3",
                "AP EN_ADDRESS_LINE4", "AP EN_ADDRESS_LINE5",
                "AP CN_ADDRESS_LINE1", "AP CN_ADDRESS_LINE2",
                "AP CN_ADDRESS_LINE3", "AP CN_ADDRESS_LINE4",
                "AP CN_ADDRESS_LINE5",
                "AP Contact No", "AP Fax No",
                "RSE Certification No", "RSE Chinese Name",
                "RSE English Name",
                "RSE EN_ADDRESS_LINE1",
                "RSE EN_ADDRESS_LINE2", "RSE EN_ADDRESS_LINE3",
                "RSE EN_ADDRESS_LINE4",
                "RSE EN_ADDRESS_LINE5", "RSE CN_ADDRESS_LINE1",
                "RSE CN_ADDRESS_LINE2",
                "RSE CN_ADDRESS_LINE3", "RSE CN_ADDRESS_LINE4",
                "RSE CN_ADDRESS_LINE5",
                "RSE Contact No", "RSE Fax No",
                "RGE Certification No", "RGE Chinese Name",
                "RGE English Name",
                "RGE EN_ADDRESS_LINE1",
                "RGE EN_ADDRESS_LINE2", "RGE EN_ADDRESS_LINE3",
                "RGE EN_ADDRESS_LINE4",
                "RGE EN_ADDRESS_LINE5", "RGE CN_ADDRESS_LINE1",
                "RGE CN_ADDRESS_LINE2",
                "RGE CN_ADDRESS_LINE3", "RGE CN_ADDRESS_LINE4",
                "RGE CN_ADDRESS_LINE5",
                "RGE Contact No", "RGE Fax No",
                "PRC Certification No", "PRC Chinese Name",
                "PRC English Name","PRC EN_ADDRESS_LINE1",
                "PRC EN_ADDRESS_LINE2", "PRC EN_ADDRESS_LINE3",
                "PRC EN_ADDRESS_LINE4",
                "PRC EN_ADDRESS_LINE5", "PRC CN_ADDRESS_LINE1",
                "PRC CN_ADDRESS_LINE2",
                "PRC CN_ADDRESS_LINE3", "PRC CN_ADDRESS_LINE4",
                "PRC CN_ADDRESS_LINE5",
                "PRC Contact No", "PRC Fax No",
                "PRC AS Chinese Name", "PRC AS English Name"
                };

            data = ExportData(model);
            dataResult = ExportResult("Validation", data);
            return this.exportExcelFile("Validation", headerList, dataResult);
        }
        private List<List<object>> ExportData(Fn04RPT_DESearchModel model)
        {
            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            string whereClause = "";
            if (model.ExpDateFrom!=null)
            {
                whereClause += "\r\n" + "\t" + " and svv.signboard_expiry_date >= :expiryDateFrom ";
                queryParameters.Add("expiryDateFrom", model.ExpDateFrom);
            }
            if (model.ExpDateTo!=null)
            {
                whereClause += "\r\n" + "\t" + " and svv.signboard_expiry_date <= :expiryDateTo ";
                queryParameters.Add("expiryDateTo", model.ExpDateTo);
            }
            string queryStr = ""
                    //select value
                    + "\r\n" + "\t" + " select svr.reference_no, svr.form_code, svr.received_date, svv.signboard_expiry_date, "
                    //+ "\r\n" + "\t" + " svv.validation_result," +
                    + "\r\n" + "\t" + " decode(svv.validation_result, "
                    + "\r\n" + "\t" + " 'A', '" + SignboardConstant.RECOMMEND_ACK_STR + "', "
                    + "\r\n" + "\t" + " 'R', '" + SignboardConstant.RECOMMEND_REF_STR + "', "
                    + "\r\n" + "\t" + " 'C', '" + SignboardConstant.RECOMMEND_COND_STR + "', "
                    + "\r\n" + "\t" + " svv.validation_result) as validation_result, "
                    + "\r\n" + "\t" + " svv.spo_endorsement_date, svv.letter_reason, svv.letter_remarks, sba.full_address, "
                    + "\r\n" + "\t" + " owner.name_chinese as ownerCname, owner.name_english as ownerEname, ownera.full_address as owneraddress, owner.email as owneremail, owner.contact_no as ownercontact, owner.fax_no as ownerfaxno, "
                    + "\r\n" + "\t" + " paw.name_chinese as pawCname, paw.name_english as pawEname, pawa.full_address as pawaddress, paw.email as pawemail,paw.contact_no as pawcontact, paw.fax_no as pawfaxno, "
                    + "\r\n" + "\t" + " io.name_chinese as ioCname, io.name_english as ioEname, ioa.full_address as ioaddress, io.email as ioemail, io.contact_no as iocontact, io.fax_no as iofaxno, io.pbp_name, io.pbp_contact_no, io.prc_name, io.prc_contact_no, "
                    + "\r\n" + "\t" + " ap.certification_no as apcert, ap.chinese_name as apCname, ap.english_name as apEname, ap.contact_no as apcontact, ap.fax_no as apfax, "
                    + "\r\n" + "\t" + " rse.certification_no as rsecert , rse.chinese_name as rseCname, rse.english_name as rseEname, rse.contact_no as rsecontact, rse.fax_no as rsefax, "
                    + "\r\n" + "\t" + " rge.certification_no as rgecert, rge.chinese_name as rgeCname, rge.english_name as rgeEname, rge.contact_no as rgecontact, rge.fax_no as rgefax, "
                    + "\r\n" + "\t" + " prc.certification_no as prccert, prc.chinese_name as prcCname, prc.english_name as prcEname, prc.contact_no as prccontact, prc.fax_no as prcfax, prc.as_chinese_name, prc.as_english_name, svv.remarks,"
                    + "\r\n" + "\t" + " svs.location_of_signboard, svs.description, svs.facade, svs.type, svs.btm_floor, svs.top_floor, svs.a_m2, svs.p_m, svs.h_m, svs.h2_m, svs.led, svs.building_portion, svs.rvd_no, sba.rv_block_id, sba.bcis_block_id, sba.bcis_district, sba.file_reference_no "
                    //From table
                    + "\r\n" + "\t" + " from "
                    + "\r\n" + "\t" + " b_SV_SUBMISSION SUB, b_sv_record svr, b_sv_validation svv, "
                    + "\r\n" + "\t" + " b_sv_address sba, b_sv_signboard svs, "
                    + "\r\n" + "\t" + " b_sv_person_contact owner, b_sv_address ownera, "
                    + "\r\n" + "\t" + " b_sv_person_contact paw, b_sv_address pawa, "
                    + "\r\n" + "\t" + " b_sv_person_contact io, b_sv_address ioa, "
                    + "\r\n" + "\t" + " b_sv_appointed_professional ap, b_sv_appointed_professional rse, "
                    + "\r\n" + "\t" + " b_sv_appointed_professional rge, b_sv_appointed_professional prc "
                    //whereClause 
                    + "\r\n" + "\t" + " where 1=1 "
                    + "\r\n" + "\t" + " and SUB.uuid = svr.SV_SUBMISSION_ID "
                    + "\r\n" + "\t" + " and svr.uuid = svv.sv_record_id "
                    + "\r\n" + "\t" + " and svs.uuid = svr.sv_signboard_id "
                    + "\r\n" + "\t" + " and sba.uuid = svs.location_address_id "
                    + "\r\n" + "\t" + " and owner.uuid = svs.owner_id and ownera.uuid = owner.sv_address_id "
                    + "\r\n" + "\t" + " and paw.uuid = svr.paw_id and pawa.uuid = paw.sv_address_id "
                    + "\r\n" + "\t" + " and io.uuid = svr.oi_id and ioa.uuid = io.sv_address_id "
                    + "\r\n" + "\t" + " and ap.sv_record_id = svr.uuid  and ap.identify_flag=  '" + SignboardConstant.PBP_CODE_AP + "'"
                    + "\r\n" + "\t" + " and rse.sv_record_id = svr.uuid  and rse.identify_flag='" + SignboardConstant.PBP_CODE_RSE + "'"
                    + "\r\n" + "\t" + " and rge.sv_record_id = svr.uuid  and rge.identify_flag='" + SignboardConstant.PBP_CODE_RGE + "'"
                    + "\r\n" + "\t" + " and prc.sv_record_id = svr.uuid  and prc.identify_flag='" + SignboardConstant.PRC_CODE + "'"
                    + "\r\n" + "\t" + " and svv.signboard_expiry_date is not null "
                    + "\r\n" + "\t" + whereClause;

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }



        public FileStreamResult ExportValidationDataForSelection(Fn04RPT_DESearchModel model)
        {
             
            List <List<object>> data = new List<List<object>>();
            List<List<object>> dataResult = new List<List<object>>();
            List<string> headerList = new List<string>()
            {
                "Submission No.", "Form Code",
                "Received Date", "Expirty Date",
                "Validation Result", "Remarks", "SPO Endorsement Date",
                "Letter Reason", "Letter Remarks",
                "Signboard Address", "Location of Signboard",
                "Depscription", "Facade", "Type",
                "Bottom fixing at Floor", "Top fixing at Floor",
                "Display Area", "Projection",
                "Heigh of Signboard", "Clearance above ground",
                "LED/TV", "Building Portion",
                "RVD No.", "RV Block ID",
                "BCIS Block ID", "BCIS District", "BD ref. (42)",
                "Owner Chinese Name",
                "Owner English Name", "Owner Address",
                "Owner Email", "Owner Contact No",
                "Owner Fax NO", "PAW Chinese Name",
                "PAW English Name", "PAW Address",
                "PAW Email", "PAW Contact No", "PAW Fax NO",
                "IO Chinese Name", "IO English Name", "IO Address",
                "IO Email", "IO Contact No",
                "IO Fax NO", "IO PBP Name",
                "IO PBP Contact No", "IO PRC Name",
                "IO PRC Contact No", "AP Certification No",
                "AP Chinese Name", "AP English Name",
                "AP EN_ADDRESS_LINE1",
                "AP EN_ADDRESS_LINE2", "AP EN_ADDRESS_LINE3",
                "AP EN_ADDRESS_LINE4", "AP EN_ADDRESS_LINE5",
                "AP CN_ADDRESS_LINE1", "AP CN_ADDRESS_LINE2",
                "AP CN_ADDRESS_LINE3", "AP CN_ADDRESS_LINE4",
                "AP CN_ADDRESS_LINE5",
                "AP Contact No", "AP Fax No",
                "RSE Certification No", "RSE Chinese Name",
                "RSE English Name",
                "RSE EN_ADDRESS_LINE1",
                "RSE EN_ADDRESS_LINE2", "RSE EN_ADDRESS_LINE3",
                "RSE EN_ADDRESS_LINE4",
                "RSE EN_ADDRESS_LINE5", "RSE CN_ADDRESS_LINE1",
                "RSE CN_ADDRESS_LINE2",
                "RSE CN_ADDRESS_LINE3", "RSE CN_ADDRESS_LINE4",
                "RSE CN_ADDRESS_LINE5",
                "RSE Contact No", "RSE Fax No",
                "RGE Certification No", "RGE Chinese Name",
                "RGE English Name",
                "RGE EN_ADDRESS_LINE1",
                "RGE EN_ADDRESS_LINE2", "RGE EN_ADDRESS_LINE3",
                "RGE EN_ADDRESS_LINE4",
                "RGE EN_ADDRESS_LINE5", "RGE CN_ADDRESS_LINE1",
                "RGE CN_ADDRESS_LINE2",
                "RGE CN_ADDRESS_LINE3", "RGE CN_ADDRESS_LINE4",
                "RGE CN_ADDRESS_LINE5",
                "RGE Contact No", "RGE Fax No",
                "PRC Certification No", "PRC Chinese Name",
                "PRC English Name","PRC EN_ADDRESS_LINE1",
                "PRC EN_ADDRESS_LINE2", "PRC EN_ADDRESS_LINE3",
                "PRC EN_ADDRESS_LINE4",
                "PRC EN_ADDRESS_LINE5", "PRC CN_ADDRESS_LINE1",
                "PRC CN_ADDRESS_LINE2",
                "PRC CN_ADDRESS_LINE3", "PRC CN_ADDRESS_LINE4",
                "PRC CN_ADDRESS_LINE5",
                "PRC Contact No", "PRC Fax No",
                "PRC AS Chinese Name", "PRC AS English Name"
                };
            data = ExportValidationData(model);
            dataResult = ExportResult("Validation", data);
            return this.exportExcelFile("Validation", headerList, dataResult);
        }
        private List<List<object>> ExportValidationData(Fn04RPT_DESearchModel model)
        {
            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            string whereClause = "";
            if (!string.IsNullOrWhiteSpace(model.SearchFileRefNo))
            {
                whereClause +="\r\n" + "\t" + "and upper(svr.reference_no) like :searchFileRefNo ";
                queryParameters.Add("searchFileRefNo","%"+ model.SearchFileRefNo.ToUpper()+"%");
            }
            if (!string.IsNullOrWhiteSpace(model.SearchBatchNumber))
            {
                whereClause += "\r\n" + "\t" + "and upper(SUB.BATCH_NO) like :searchBatchNumber ";
                queryParameters.Add("searchBatchNumber", "%" + model.SearchBatchNumber + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SearchStatus))
            {
                whereClause += "\r\n" + "\t" + "and svv.VALIDATION_RESULT = :searchStatus ";
                queryParameters.Add("searchStatus", model.SearchStatus);
            }
            if (!string.IsNullOrWhiteSpace(model.EndorsedBy))
            {
                whereClause += "\r\n" + "\t" + "and svv.ENDORSED_BY = :endorsedBy ";
                queryParameters.Add("endorsedBy", model.EndorsedBy);
            }
            if (!string.IsNullOrWhiteSpace(model.SearchFormCode))
            {
                whereClause += "\r\n" + "\t" + "and svr.form_code = :searchFormCode ";
                queryParameters.Add("searchFormCode", model.SearchFormCode);
            }
            if (!string.IsNullOrWhiteSpace(model.HandlingOfficer))
            {
                whereClause += "\r\n" + "\t" + "and (svr.to_user_id = :handlingOfficer or svr.po_user_id = :handlingOfficer or svr.spo_user_id = :handlingOfficer) ";
                queryParameters.Add("handlingOfficer", model.HandlingOfficer);
            }
            whereClause += "\r\n" + "\t" + "order by svr.reference_no ";

            string queryStr = ""
                    //select value
                    + "\r\n" + "\t" + " select "
                    + "\r\n" + "\t" + " svr.reference_no, svr.form_code, svr.received_date, svv.signboard_expiry_date, "
                    //+ "\r\n" + "\t" + " svv.validation_result,"
                    + "\r\n" + "\t" + " decode(svv.validation_result, "
                    + "\r\n" + "\t" + " 'A', '" + SignboardConstant.RECOMMEND_ACK_STR + "', "
                    + "\r\n" + "\t" + " 'R', '" + SignboardConstant.RECOMMEND_REF_STR + "', "
                    + "\r\n" + "\t" + " 'C', '" + SignboardConstant.RECOMMEND_COND_STR + "', "
                    + "\r\n" + "\t" + " svv.validation_result) as validation_result, "
                    + "\r\n" + "\t" + " svv.spo_endorsement_date, "
                    + "\r\n" + "\t" + " svv.letter_reason, svv.letter_remarks, "
                    + "\r\n" + "\t" + " sba.full_address, "
                    + "\r\n" + "\t" + " owner.name_chinese as ownerCnaame, owner.name_english as ownerEname, ownera.full_address as owneraddress, owner.email as owneremail, owner.contact_no as ownercontact, owner.fax_no as ownerfaxno, "
                    + "\r\n" + "\t" + " paw.name_chinese as pawCnaame, paw.name_english as pawEname, pawa.full_address as pawaddress, paw.email as pawemail,paw.contact_no as pawcontact, paw.fax_no as pawfaxno, "
                    + "\r\n" + "\t" + " io.name_chinese as ioCnaame, io.name_english as ioEname, ioa.full_address as ioaddress, io.email as ioemail, io.contact_no as iocontact, io.fax_no as iofaxno, io.pbp_name, io.pbp_contact_no, io.prc_name, io.prc_contact_no, "
                    + "\r\n" + "\t" + " ap.certification_no as apcert, ap.chinese_name as apCname, ap.english_name as apEname, ap.contact_no as apcontact, ap.fax_no as apfax, "
                    + "\r\n" + "\t" + " rse.certification_no as rsecert , rse.chinese_name as rseCname, rse.english_name as rseEname, rse.contact_no as rsecontact, rse.fax_no as rsefax, "
                    + "\r\n" + "\t" + " rge.certification_no as rgecert, rge.chinese_name as rgeCname, rge.english_name as rgeEname, rge.contact_no as rgecontact, rge.fax_no as rgefax, "
                    + "\r\n" + "\t" + " prc.certification_no as prccert, prc.chinese_name as prcCname, prc.english_name as prcEname, prc.contact_no as prccontact, prc.fax_no as prcfax, prc.as_chinese_name, prc.as_english_name, svv.remarks, "
                    + "\r\n" + "\t" + " svs.location_of_signboard, svs.description, svs.facade, svs.type, svs.btm_floor, svs.top_floor, svs.a_m2, svs.p_m, svs.h_m, svs.h2_m, svs.led, svs.building_portion, svs.rvd_no, sba.rv_block_id, sba.bcis_block_id, sba.bcis_district, sba.file_reference_no "
                    //From table
                    + "\r\n" + "\t" +" from b_SV_SUBMISSION SUB, b_sv_record svr, b_sv_validation svv, b_sv_address sba, b_sv_signboard svs, "
                    + "\r\n" + "\t" +" b_sv_person_contact owner, b_sv_address ownera, "
                    + "\r\n" + "\t" +" b_sv_person_contact paw, b_sv_address pawa, "
                    + "\r\n" + "\t" +" b_sv_person_contact io, b_sv_address ioa, "
                    + "\r\n" + "\t" +" b_sv_appointed_professional ap, b_sv_appointed_professional rse, "
                    + "\r\n" + "\t" +" b_sv_appointed_professional rge, b_sv_appointed_professional prc "
                    //whereClause 
                    + "\r\n" + "\t" +" where 1=1 "
                    + "\r\n" + "\t" +" and SUB.uuid = svr.SV_SUBMISSION_ID "
                    + "\r\n" + "\t" +" and svr.uuid = svv.sv_record_id "
                    + "\r\n" + "\t" +" and svs.uuid = svr.sv_signboard_id "
                    + "\r\n" + "\t" +" and sba.uuid = svs.location_address_id "
                    + "\r\n" + "\t" +" and owner.uuid = svs.owner_id and ownera.uuid = owner.sv_address_id "
                    + "\r\n" + "\t" +" and paw.uuid = svr.paw_id and pawa.uuid = paw.sv_address_id "
                    + "\r\n" + "\t" +" and io.uuid = svr.oi_id and ioa.uuid = io.sv_address_id "
                    + "\r\n" + "\t" +" and ap.sv_record_id = svr.uuid  and ap.identify_flag='" + SignboardConstant.PBP_CODE_AP +"'"
                    + "\r\n" + "\t" +" and rse.sv_record_id = svr.uuid  and rse.identify_flag='"+SignboardConstant.PBP_CODE_RSE+"'"
                    + "\r\n" + "\t" +" and rge.sv_record_id = svr.uuid  and rge.identify_flag='" + SignboardConstant.PBP_CODE_RGE + "'"
                    + "\r\n" + "\t" +" and prc.sv_record_id = svr.uuid  and prc.identify_flag='" + SignboardConstant.PRC_CODE + "'"
                    + "\r\n" + "\t" + whereClause;

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }
        private List<List<object>> getCrmPbpPrc(string certificationNo)
        {
            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            string queryStr = ""
                + "\r\n" + "\t" + " select en_address_line1, en_address_line2, en_address_line3, en_address_line4, en_address_line5,"
                + "\r\n" + "\t" + " cn_address_line1, cn_address_line2, cn_address_line3, cn_address_line4, cn_address_line5 from b_crm_pbp_prc "
                + "\r\n" + "\t" + " WHERE CERTIFICATION_NO = :certificationNo";

            queryParameters.Add("certificationNo",certificationNo);

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }
        //public FileStreamResult ExportValidationDataExcelFile(string fileName, List<string> Columns, List<List<object>> Data)
        //{
        //    MemoryStream stream = new MemoryStream();
        //    ExcelPackage ep = new ExcelPackage();
        //    ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");
        //    for (int i = 0; i < Columns.Count; i++)
        //    {
        //        sheet.Cells[1, i + 1].LoadFromText(Columns[i].ToString());
        //        sheet.Cells[1, i + 1].Style.Font.Bold = true;
        //        sheet.Cells[1, i + 1].Style.Font.Size = 14;
        //        sheet.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
        //    }
        //    if (Data != null)
        //    {
        //        for (int i = 0; i < Data.Count; i++)
        //        {
        //            List<object> eachRow = Data[i];
        //            for (int j = 0; j < Columns.Count; j++)
        //            {
        //                sheet.Cells[i + 2, j + 1].LoadFromText(eachRow[j].ToString());
        //            }
        //        }
        //    }
        //    sheet.Cells.AutoFitColumns();
        //    ep.SaveAs(stream);
        //    stream.Position = 0;
        //    FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        //    fsr.FileDownloadName = fileName + ".xlsx";
        //    return fsr;
        //}
    }
}