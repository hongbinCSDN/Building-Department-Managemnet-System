using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingSystemValueService
    {

        public List<P_S_SYSTEM_VALUE> GetSystemListByType(string Type)
        {
            List<P_S_SYSTEM_VALUE> list = new List<P_S_SYSTEM_VALUE>();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {

                list = (from st in db.P_S_SYSTEM_TYPE
                        join sv in db.P_S_SYSTEM_VALUE on st.UUID equals sv.SYSTEM_TYPE_ID
                        where st.TYPE == Type
                        select sv).ToList();
                return list;
            }
        }

        public static List<P_S_SYSTEM_VALUE> GetSystemListByCode(string code)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_S_SYSTEM_VALUE.Where(d => d.CODE == code).ToList();
            }
        }

        // Andy add on 2019/10/24
        public static P_S_SYSTEM_VALUE GetSystemValueByTypeAndCode(String type, string code)
        {
            List<P_S_SYSTEM_VALUE> list = new List<P_S_SYSTEM_VALUE>();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                //list = db.P_S_SYSTEM_VALUE.Where(x=>x.CODE==code &)
                list = (from sv in db.P_S_SYSTEM_VALUE
                        join st in db.P_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                        where st.TYPE == type && sv.CODE == code
                        select sv).ToList();

                P_S_SYSTEM_VALUE P_S_SYSTEM_VALUE = null;
                if (list != null && list.Count > 0)
                {
                    P_S_SYSTEM_VALUE = list.ElementAt(0);
                }

                return P_S_SYSTEM_VALUE;
            }
        }

        public static P_S_SYSTEM_VALUE GetSystemValueByUUID(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_S_SYSTEM_VALUE.Where(d => d.UUID == uuid).FirstOrDefault();
            }
        }
    }
}