using MWMS2.App_Start;
using MWMS2.Constant;
using MWMS2.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Utility
{
    public class CommonUtil
    {
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
                            j++;
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
            bool[] isDate = new bool[dr.FieldCount];
            for (int i=  0;i < dr.FieldCount; i++)
            {
                isDate[i]="System.DateTime" == dr.GetFieldType(i).FullName;
            }
            while (dr.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (isDate[i])
                    {
                         try {
                     
                            object v = dr.GetValue(i);
                            row[dr.GetName(i)] =v == System.DBNull.Value ? null : v;
                            row[dr.GetName(i) + ApplicationConstant.DISPLAY_DATE_COLUMN_SUFFIX] = row[dr.GetName(i)] == null ? null : ((DateTime)dr.GetValue(i)).ToString(ApplicationConstant.DISPLAY_DATE_FORMAT);
                        } catch {
                            row[dr.GetName(i)] = "DateTime out of Range (Error Data)";
                            row[dr.GetName(i) + ApplicationConstant.DISPLAY_DATE_COLUMN_SUFFIX] = "";
                        }
                    }
                    else
                    {
                        row[dr.GetName(i)] = dr.GetValue(i);
                    }
                    //row[dr.GetName(i)] = dr.GetValue(i);
                    /*if (dr.GetValue(i) is DateTime)
                    {
                        row[dr.GetName(i) + ApplicationConstant.DISPLAY_DATE_COLUMN_SUFFIX] = ((DateTime)dr.GetValue(i)).ToString(ApplicationConstant.DISPLAY_DATE_FORMAT);
                    }*/
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
        public static int getCurrentMonth()
        {
            return DateTime.Now.Month;
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


        public static bool isCompletedForm(string formCode)
        {
            for (int i = 0; i < SignboardConstant.COMPLETION_FORM_CODES.Count(); i++)
            {
                if (SignboardConstant.COMPLETION_FORM_CODES[i].Equals(formCode))
                {
                    return true;
                }
            }
            return false;
        }
    }
}