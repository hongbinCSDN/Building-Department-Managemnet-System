using MWMS2.Entity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SvRecordItemDAOService : BaseDAOService
    {
        public List<B_SV_RECORD_ITEM> getSvRecordItemBySVRecord(string UUID)
        {
            List<B_SV_RECORD_ITEM> svRecordItemList = null;
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                svRecordItemList = (from svri in db.B_SV_RECORD_ITEM
                                where svri.UUID == UUID
                                select svri).ToList();
            }
            return svRecordItemList;
        }
        public List<B_SV_RECORD_ITEM> findByProperty(B_SV_RECORD value)
        {
            List<B_SV_RECORD_ITEM> result = new List<B_SV_RECORD_ITEM>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                result = db.B_SV_RECORD_ITEM.Where
                       (o => o.SV_RECORD_ID == value.UUID).ToList();
            }
            return result;
        }

        public List<B_SV_RECORD_ITEM> FindByProperty(string propertyName, Object value)
        {
            List<B_SV_AUDIT_RECORD> resultList = new List<B_SV_AUDIT_RECORD>();
            string queryString = @"SELECT * FROM B_SV_RECORD_ITEM WHERE :PROPERTY_NAME = :VALUE";
            queryString = queryString.Replace(":PROPERTY_NAME", propertyName);
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
            new OracleParameter("VALUE", value)
            };

            return GetObjectData<B_SV_RECORD_ITEM>(queryString, oracleParameters).ToList();
        }
        public B_SV_RECORD_ITEM getSvRecordItemByUuid(string UUID)
        {
            B_SV_RECORD_ITEM svRecordItem = null;
            List<B_SV_RECORD_ITEM> arrayList = new List<B_SV_RECORD_ITEM>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                arrayList = (from svri in db.B_SV_RECORD_ITEM
                                where svri.UUID == UUID
                                select svri).ToList();
            }
            if (arrayList.Count()!=0){
                svRecordItem = arrayList[0];
            }
            return svRecordItem;
        }
    }
}