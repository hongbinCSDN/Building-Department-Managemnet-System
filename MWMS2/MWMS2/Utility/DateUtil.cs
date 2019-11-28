using MWMS2.App_Start;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Utility
{
    public class DateUtil
    {
        public static DateTime getNow()
        {
            return DateTime.Now;
        }

        public static string getEnglishDateDisplayFormat(Nullable<System.DateTime> date, string displayFormat)
        {
            try
            {   if (date == null)
                {
                    return "";
                }
                String EngCurrentDate = ((DateTime)date).ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                return EngCurrentDate;
            }
            catch(Exception e)
            {
                AuditLogService.logDebug("DateUtil.getEnglishDateDisplayFormat()"+ e);
                return "ERROR";
                
            }

        }

        public static String getCurrentDate()
        {
            DateTime date = DateTime.Now;
            String CurrentDate = date.ToString();
            return CurrentDate;
        }

        public static String getEnglishCurrentDate()
        {
            DateTime date = DateTime.Now;
            String EngCurrentDate = date.ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            return EngCurrentDate;
        }

        public static String getEnglishCurrentDateWithoutDay()
        {
            DateTime date = DateTime.Now;
            String EngCurrentDate = date.ToString("MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            return EngCurrentDate;
        }

        public static String getChineseCurrentDate()
        {
            DateTime date = DateTime.Now;
            String year = date.Year.ToString();
            String month = date.Month.ToString();
            String day = date.Day.ToString();
            return year + "年" + month + "月" + day + "日";
        }

        public static String getEnglishFormatDate(DateTime? inputDate)
        {
            if (!inputDate.Equals(null))
            {
                String EngCurrentDate = inputDate.Value.ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                return EngCurrentDate;
            }
            return "";
        }

        public static String getChineseFormatDate(DateTime? inputDate)
        {
            if (!inputDate.Equals(null))
            {
                String year = inputDate.Value.Year.ToString();
                String month = inputDate.Value.Month.ToString();
                String day = inputDate.Value.Day.ToString();
                return year + "年" + month + "月" + day + "日";
            }
            return "";
        }


        public static String getEnglishMMMMYYYYFormat(DateTime inputDate){
            if (!inputDate.Equals(null)){
                return inputDate.ToString("MMMM-yyyy");
            }
            return "";
        }


        public static String getYYYYMMFormat(DateTime? inputDate)
        {
            if (!inputDate.Equals(null)){
                if(inputDate.Value.Month < 10)
                {   return inputDate.Value.Year.ToString() + "0" + inputDate.Value.Month;
                }
                else
                {   return inputDate.Value.Year.ToString() + "" + inputDate.Value.Month;
                }
            }
            return "";
        }

        public static String getEnglishDateWithoutDay(DateTime? inputDate)
        {
            if (!inputDate.Equals(null))
            {
                String EngDateWithoutDay = inputDate.Value.ToString("MMMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                return EngDateWithoutDay;
            }
            return "";
        }

        public static string getDisplay(object v)
        {

            if (v == null)
            {
                return "";
            }

            return string.IsNullOrWhiteSpace(v.ToString()) ? "" : v.ToString();
        }

        public static string getDisplay(string v) { return string.IsNullOrWhiteSpace(v) ? "" : v; }

        public const int MILLISECS_PER_DAY = 24 * 60 * 60 * 1000;
        public static DateTime D20040831 = new DateTime(1093881600000L);

        //common
        public static DateTime? Ticks2Datetime(string ticks)
        {
            long lticks = -1;
            long.TryParse(ticks, out lticks);
            DateTime? r = null;
            if (lticks != -1)
            {
                r = new DateTime(lticks);
            }
            return r;
        }

        public static int getDayDiff(DateTime? start, DateTime? end)
        {
            if ((start == null) || (end == null))
            {
                return 0;
            }
            long starttime = start.Value.Ticks;
            long endtime = end.Value.Ticks;
            int result = (int)(((starttime - endtime)) / MILLISECS_PER_DAY);
            return result;
        }
        public static int CallStoredProc(DbConnection conn, string q, Dictionary<string, object> parameters)
        {
            DbCommand comm = conn.CreateCommand();
            int result = -1;
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> kv in parameters)
                {
                    if (kv.Value is ICollection)
                    {
                        ICollection iColl = (ICollection)kv.Value;
                        string changeQ = "";
                        for (int i = 0; i < iColl.Count; i++)
                        {
                            if (i != 0) changeQ += ", ";
                            changeQ += ":" + kv.Key + i;
                        }
                        q = q.Replace(":" + kv.Key, changeQ);
                        IEnumerator iEnu = iColl.GetEnumerator();
                        int j = 0;
                        while (iEnu.MoveNext())
                        {
                            DbParameter p = comm.CreateParameter();
                            p.ParameterName = kv.Key + j;
                            p.Value = iEnu.Current;
                            p.Direction = ParameterDirection.Input;
                            comm.Parameters.Add(p);
                        }
                    }
                    else
                    {
                        DbParameter p = comm.CreateParameter();
                        p.ParameterName = kv.Key;
                        p.Value = kv.Value;
                        p.Direction = ParameterDirection.Input;
                        comm.Parameters.Add(p);
                    }
                }
            }
            comm.CommandText = q;
            conn.Open();
            result = comm.ExecuteNonQuery();
            conn.Close();

            return result;
        }

        public static DbDataReader GetDataReader(DbConnection conn, string q, Dictionary<string, object> parameters)
        {
            DbCommand comm = conn.CreateCommand();
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> kv in parameters)
                {
                    if (kv.Value is ICollection)
                    {
                        ICollection iColl = (ICollection)kv.Value;
                        string changeQ = "";
                        for (int i = 0; i < iColl.Count; i++)
                        {
                            if (i != 0) changeQ += ", ";
                            changeQ += ":" + kv.Key + i;
                        }
                        q = q.Replace(":" + kv.Key, changeQ);
                        IEnumerator iEnu = iColl.GetEnumerator();
                        int j = 0;
                        while (iEnu.MoveNext())
                        {
                            DbParameter p = comm.CreateParameter();
                            p.ParameterName = kv.Key + j;
                            p.Value = iEnu.Current;
                            p.Direction = ParameterDirection.Input;
                            comm.Parameters.Add(p);
                        }
                    }
                    else
                    {
                        DbParameter p = comm.CreateParameter();
                        p.ParameterName = kv.Key;
                        p.Value = kv.Value;
                        //p.Direction = ParameterDirection.Input;
                        comm.Parameters.Add(p);
                    }
                }
            }
            comm.CommandText = q;

            //if(conn.State != ConnectionState.Open)
            //{
            conn.Open();
            // }

            return comm.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public static List<Dictionary<string, object>> LoadDbData(DbDataReader dr)
        {
            List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
            while (dr.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    row[dr.GetName(i)] = dr.GetValue(i);
                    if (dr.GetValue(i) is DateTime)
                    {
                        row[dr.GetName(i) + ApplicationConstant.DISPLAY_DATE_COLUMN_SUFFIX] = ((DateTime)dr.GetValue(i)).ToString(ApplicationConstant.DISPLAY_DATE_FORMAT);
                    }
                }
                datas.Add(row);
            }
            dr.Close();
            return datas;
        }

        public static List<List<object>> convertToList(DbDataReader dr)
        {
            List<List<object>> datas = new List<List<object>>();
            while (dr.Read())
            {
                List<object> row = new List<object>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    row.Add(dr.GetValue(i));
                }
                datas.Add(row);
            }
            dr.Close();
            return datas;
        }

        public static int LoadDbCount(DbDataReader dr)
        {
            dr.Read();
            decimal v = dr.GetDecimal(0);
            dr.Close();
            return decimal.ToInt32(v);
        }
        public static string NewUuid()
        {
            string stringuuid = Guid.NewGuid().ToString().Replace("-", "");
            return stringuuid;
        }
        public static bool isWeekDay(DateTime date)
        {
            if ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday))
            {
                return false;
            }
            return true;
        }

        public static int getCurrentYear()
        {
            return DateTime.Now.Year;
        }

        // Add by Chester

        public static IList<T> DataReaderToList<T>(IDataReader dr)
        {

            List<T> list = new List<T>();
            Type type = typeof(T);
            while (dr.Read())
            {
                T model = Activator.CreateInstance<T>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (!IsNullOrDbNull(dr[i]))
                    {
                        PropertyInfo propertyInfo = type.GetProperty(dr.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (propertyInfo != null)
                        {
                            propertyInfo.SetValue(model, CheckType(dr[i], propertyInfo.PropertyType), null);
                        }
                    }
                }
                list.Add(model);
            }
            dr.Close();
            return list;

        }

        private static object CheckType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(value, conversionType);
        }

        private static bool IsNullOrDbNull(object obj)
        {
            return (obj == null || obj is DBNull) ? true : false;
        }
        public static string getDisplaySetZero(string data)
        {
            if (data == "" || data == null)
            {
                return "0";
            }
            else
            {
                return data;
            }
        }
        public static string getWFStatusDisplay(string wfStatusCodeObject)
        {

            string wfStatusCode = wfStatusCodeObject;

            if (SignboardConstant.WF_MAP_VALIDATION_TO.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_VALIDATION_TO;
            }
            if (SignboardConstant.WF_MAP_VALIDATION_PO.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_VALIDATION_PO;
            }
            if (SignboardConstant.WF_MAP_VALIDATION_SPO.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_VALIDATION_SPO;
            }
            if (SignboardConstant.WF_MAP_AUDIT_TO.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_AUDIT_TO;
            }
            if (SignboardConstant.WF_MAP_AUDIT_PO.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_AUDIT_PO;
            }
            if (SignboardConstant.WF_MAP_AUDIT_SPO.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_AUDIT_SPO;
            }

            if (SignboardConstant.WF_MAP_ASSIGING.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_ASSIGING;
            }
            if (SignboardConstant.WF_MAP_VALIDATION_END.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_VALIDATION_END;
            }

            if (SignboardConstant.WF_MAP_S24_SPO_APPROVE.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_S24_SPO_APPROVE;
            }
            if (SignboardConstant.WF_MAP_S24_PO.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_S24_PO;
            }
            //if (SignboardConstant.WF_MAP_S24_PO.Equals(wfStatusCode))
            //{
            //    return SignboardConstant.DISPLAY_WF_MAP_S24_PO;
            //}
            if (SignboardConstant.WF_MAP_S24_SPO_COMPILE.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_S24_SPO_COMPILE;
            }
            if (SignboardConstant.WF_MAP_S24_END.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_S24_END;
            }

            if (SignboardConstant.WF_MAP_GC_SPO_APPROVE.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_GC_SPO_APPROVE;
            }
            if (SignboardConstant.WF_MAP_GC_PO.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_GC_PO;
            }
            if (SignboardConstant.WF_MAP_GC_SPO_COMPILE.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_GC_SPO_COMPILE;
            }
            if (SignboardConstant.WF_MAP_GC_PO_COMPLI.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_GC_PO_COMPLI;
            }
            if (SignboardConstant.WF_MAP_GC_END.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_GC_END;
            }
            if (SignboardConstant.WF_MAP_END.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_END;
            }

            if (SignboardConstant.WF_MAP_VALIDATION_ISSUE_LETTER_TO.Equals(wfStatusCode))
            {
                return SignboardConstant.DISPLAY_WF_MAP_VALIDATION_ISSUE_LETTER_TO;
            }

            return wfStatusCode;
        }
        public static string getValidationResultDisplay(string validationResult)
        {
            if (SignboardConstant.VALIDATION_RESULT_ACKNOWLEDGED.Equals(validationResult))
            {
                return SignboardConstant.RECOMMEND_ACK_STR;
            }
            if (SignboardConstant.VALIDATION_RESULT_REFUSED.Equals(validationResult))
            {
                return SignboardConstant.RECOMMEND_REF_STR;
            }
            if (SignboardConstant.VALIDATION_RESULT_CONDITIONAL.Equals(validationResult))
            {
                return SignboardConstant.RECOMMEND_COND_STR;
            }
            if (SignboardConstant.VALIDATION_RESULT_WITHDRAW.Equals(validationResult))
            {
                return SignboardConstant.RECOMMEND_WITH_STR;
            }
            return validationResult;
        }
        // End add by Chester

        //public static List<string> merge(List<List<string>> arrList)
        //{
        //    int size = 0;
        //    for (int i = 0; i < arrList.Count(); i++)
        //    {
        //        List<string> a = arrList.get(i);
        //        size += a.length;
        //    }

        //    String[] res = new String[size];
        //    int destPos = 0;
        //    for (int i = 0; i < arrList.size(); i++)
        //    {
        //        String[] a = arrList.get(i);

        //        if (i > 0)
        //        {
        //            String[] b = arrList.get(i - 1);
        //            destPos += b.length;
        //        }
        //        int length = a.length;
        //        System.arraycopy(a, 0, res, destPos, length);
        //    }

        //    return res;
        //}
        public static List<string> getMonth(DateTime fromDate, DateTime toDate)
        {
            List<string> mlist = new List<string>();
            for (int i = fromDate.Month; i <= toDate.Month; i++)
            { }


            return mlist;

        }
        public static List<DateTime> getMonthList(DateTime fromDate, DateTime toDate)
        {

            List<DateTime> getMonthList = new List<DateTime>();
            DateTime target = fromDate;
            while (target < toDate)
            {
                getMonthList.Add(target);
                target = target.AddMonths(1);
            }
            getMonthList.Add(toDate);
            return getMonthList;
        }

        public static List<DateTime> getDateList(DateTime fromDate, DateTime toDate)
        {
            List<DateTime> dateList = new List<DateTime>();
            bool goNext = true;
            DateTime start = getDateFirstMoment(fromDate);
            int endMonth = toDate.Month;
            int endYear = toDate.Year;

            while (goNext)
            {
                dateList.Add(start);

                int startMonth = start.Month;
                int startYear = start.Year;

                if ((startMonth == endMonth) && (startYear == endYear))
                {
                    dateList.Add(getDateLastMoment(toDate));
                    goNext = false;
                }
                else
                {
                    //start = new DateTime(startYear, startMonth+1, 1);
                    start = start.AddMonths(1);
                    start = getDateFirstMoment(start);
                    dateList.Add(getDateLastMoment(getEndDateOfMonth(start)));
                }
            }

            return dateList;
        }
        public static DateTime getDateFirstMoment(DateTime date)
        {
            DateTime firstMoment = new DateTime();

            if (date != null)
            {
                firstMoment = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            }
            return firstMoment;
        }
        public static DateTime getDateLastMoment(DateTime date)
        {
            DateTime lastMoment = new DateTime();

            if (date != null)
            {
                lastMoment = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            }
            return lastMoment;
        }
        public static DateTime getEndDateOfMonth(DateTime date)
        {
            date.AddMonths(1);

            return date;
        }
        public static string padZero(string input, int j)
        {
            if (input == null)
            {
                input = "";
            }
            string number_spaces = "";
            if (input.Count() < j)
                for (int i = 0; i < (j - input.Count()); i++)
                {
                    number_spaces = number_spaces + "0";
                }
            input = number_spaces + input;
            return input;
        }

        public static int daysOfTwo(DateTime? toDate, DateTime? fromDate)
        {
            if (toDate != null && fromDate != null)
            {
                // End date - Start date
                return getDayDiff(toDate, fromDate);
            }
            return 0;
        }

        public static DateTime? getDisplayDateToDBDate(String displayDate)
        {

            if (displayDate == null)
            {
                return null;
            }

            displayDate = displayDate.Trim().Replace("[.]", "/").Replace("[-]", "/");

            if (!displayDate.Equals(""))
            {
                //DateFormat dateformat = new SimpleDateFormat(DATE_DISPLAY_FORMAT);
                DateTime? result = null;
                try
                {
                    //result = dateformat.parse(displayDate);
                    result = Convert.ToDateTime(displayDate);
                }
                catch (Exception e)
                {
                    return result;
                }
                return result;
            }
            else
            {
                return null;
            }
        }


        // Convert from DateTime object to string with AM/PM
        public static string ConvertToDateTimeDisplay(DateTime? dateTime)
        {
            string dtStr = "";
            if (dateTime != null)
            {
                dtStr = dateTime.Value.ToString("dd/MM/yyyy hh:mm:ss tt");
            }
            return dtStr;
        }

        public static int compareDate(DateTime d1, DateTime d2)
        {
            short vl = 1;
            GregorianCalendar gc = new GregorianCalendar();

            int year = gc.GetYear(d1);
            int month = gc.GetMonth(d1);
            int day = gc.GetDayOfMonth(d1);

            int tempYear = gc.GetYear(d2);
            int tempMonth = gc.GetMonth(d2);
            int tempDay = gc.GetDayOfMonth(d2);


            if (year != tempYear)
            {
                if (year > tempYear)
                    vl = 2;
                else
                    vl = 0;
            }
            else
            {
                if (month != tempMonth)
                {
                    if (month > tempMonth)
                        vl = 2;
                    else
                        vl = 0;
                }
                else
                {
                    if (day != tempDay)
                    {
                        if (day > tempDay)
                            vl = 2;
                        else
                            vl = 0;
                    }
                }
            }
            return vl;
        }

    }
}