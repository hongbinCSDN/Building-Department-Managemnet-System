using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class LetterTemplateDAOService
    {
        public List<List<object>> getLetterTemplateListWithPara(string formCode, string letterType, string result)
        {
            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();
            List<List<object>> resultList = new List<List<object>>();

            string queryStr = 
                " SELECT a.uuid, a.letter_Name, a.form_code, a.letter_type, a.result, a.file_name, " +
                " b.description " +
                " FROM B_S_LETTER_TEMPLATE a, B_S_SYSTEM_VALUE b " +
                " where 1=1 " +
                " and a.LETTER_TYPE = b.CODE ";

            if (formCode != null && !formCode.Equals(""))
            {
                queryStr = queryStr + "and a.form_code = :formCode ";

            }
            if (letterType != null && !letterType.Equals(""))
            {
                queryStr = queryStr + "and a.letter_type = :letterType ";

            }
            if (result != null && !result.Equals(""))
            {
                queryStr = queryStr + "and a.result = :result ";

            }
            if (formCode != null && !formCode.Equals(""))
            {
                QueryParameters.Add("formCode", formCode);

            }
            if (letterType != null && !letterType.Equals(""))
            {
                QueryParameters.Add("letterType", letterType);

            }
            if (result != null && !result.Equals(""))
            {
                QueryParameters.Add("result", result);

            }
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, QueryParameters);
                    resultList = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return resultList;
        }
        public B_S_LETTER_TEMPLATE GetLetterbyID(string id)
        {
            List<B_S_LETTER_TEMPLATE> resultList = new List<B_S_LETTER_TEMPLATE>();
            B_S_LETTER_TEMPLATE result = new B_S_LETTER_TEMPLATE();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                result = db.B_S_LETTER_TEMPLATE.Where
                       (o => o.UUID == id).FirstOrDefault();
            }
            return result;
        }
    }
}