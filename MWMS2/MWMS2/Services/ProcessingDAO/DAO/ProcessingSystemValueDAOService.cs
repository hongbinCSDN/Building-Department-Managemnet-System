using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingSystemValueDAOService
    {
        public P_S_SYSTEM_VALUE GetSSystemValueByTypeAndCode(String type, String code)
        {
            P_S_SYSTEM_VALUE result = null;
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                result = (from sSystemValue in db.P_S_SYSTEM_VALUE
                          join sSystemType in db.P_S_SYSTEM_TYPE on sSystemValue.SYSTEM_TYPE_ID equals sSystemType.UUID
                          where 1 == 1
                          && sSystemType.TYPE == type
                          && sSystemValue.CODE == code
                          && sSystemValue.IS_ACTIVE == ProcessingConstant.FLAG_Y
                          select sSystemValue).FirstOrDefault();
            }
            return result;
        }

        public List<P_S_SYSTEM_VALUE> GetSSystemValueByType(String type)
        {
            List<P_S_SYSTEM_VALUE> resultList = null;
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                resultList = (from sSystemValue in db.P_S_SYSTEM_VALUE
                              join sSystemType in db.P_S_SYSTEM_TYPE on sSystemValue.SYSTEM_TYPE_ID equals sSystemType.UUID
                              where 1 == 1
                              && sSystemType.TYPE == type
                              && sSystemValue.IS_ACTIVE == ProcessingConstant.FLAG_Y
                              select sSystemValue).ToList();
            }
            return resultList;
        }

        public List<P_S_SYSTEM_VALUE> GetSSystemValueByTypeOrderByOrdering(String type)
        {
            List<P_S_SYSTEM_VALUE> resultList = null;
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                resultList = (from sSystemValue in db.P_S_SYSTEM_VALUE
                              join sSystemType in db.P_S_SYSTEM_TYPE on sSystemValue.SYSTEM_TYPE_ID equals sSystemType.UUID
                              where 1 == 1
                              && sSystemType.TYPE == type
                              && sSystemValue.IS_ACTIVE == ProcessingConstant.FLAG_Y
                              orderby sSystemValue.ORDERING ascending
                              select sSystemValue).ToList();
            }
            return resultList;
        }

        public P_S_SYSTEM_VALUE GetSSystemValueByUuid(String svUuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_S_SYSTEM_VALUE sv = db.P_S_SYSTEM_VALUE.Where(o => o.UUID == svUuid).FirstOrDefault();
                return sv;
            }
        }

        public P_S_SYSTEM_VALUE GetSSystemValueByCode(string code)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_S_SYSTEM_VALUE sv = db.P_S_SYSTEM_VALUE.Where(o => o.CODE == code).FirstOrDefault();
                return sv;
            }
        }

        public P_S_SYSTEM_VALUE GetSSystemValueByCode(string code, EntitiesMWProcessing db)
        {
            return db.P_S_SYSTEM_VALUE.Where(o => o.CODE == code).FirstOrDefault();
        }

        public List<P_S_SYSTEM_VALUE> GetS_SYSTEM_VALUEsByCodeList(string[] Codes, EntitiesMWProcessing db)
        {
            return (from sv in db.P_S_SYSTEM_VALUE
                    where Codes.Contains(sv.CODE)
                    select sv).ToList();
        }
    }
}