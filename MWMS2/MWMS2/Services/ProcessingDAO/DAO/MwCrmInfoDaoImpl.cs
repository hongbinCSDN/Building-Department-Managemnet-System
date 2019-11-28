using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;
using Oracle.ManagedDataAccess.Client;

namespace MWMS2.Services
{
    public class MwCrmInfoDaoImpl:BaseDAOService
    {

        public V_CRM_INFO findByCertNo(string cerNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                V_CRM_INFO crmInfo = db.V_CRM_INFO.Where(m => m.CERTIFICATION_NO == cerNo).OrderBy(o => o.ROLE_CODE).FirstOrDefault();

                return crmInfo;
            }
        }

        public List<V_CRM_INFO> GetV_CRM_INFOs(string certNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.V_CRM_INFO.Where(m => m.CERTIFICATION_NO == certNo).ToList();

            }
        }

        public List<V_CRM_INFO> findListByCertNo(string certNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.V_CRM_INFO.Where(m => m.CERTIFICATION_NO == certNo).OrderBy(o => o.ROLE_CODE).ToList();

            }
        }

        public List<V_CRM_INFO> findListDistinctByCertNo(string certNo)
        {
            string sSql = @"SELECT DISTINCT CODE,
                                            CERTIFICATION_NO,
                                            SURNAME,
                                            GIVEN_NAME,
                                            CHINESE_NAME,
                                            AS_SURNAME,
                                            AS_GIVEN_NAME,
                                            AS_CHINESE_NAME,
                                            EXPIRY_DATE
                            FROM   V_CRM_INFO
                            WHERE  CERTIFICATION_NO = :certNo ";

            OracleParameter[] oracleParameter = new OracleParameter[]
            {
                new OracleParameter(":certNo",certNo)
            };

            return GetObjectData<V_CRM_INFO>(sSql, oracleParameter).ToList();

        }

        public List<V_CRM_INFO> findListByCertNoEnglishNameChineseName(String certNo, String asSurName, String asGivenName, String asChineseName)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                //return db.V_CRM_INFO.Where(m => m.CERTIFICATION_NO == certNo).OrderBy(o => o.ROLE_CODE).ToList();

                var resultList = db.V_CRM_INFO.Where(m => m.CERTIFICATION_NO == certNo);

                if(!string.IsNullOrEmpty(asSurName) && !string.IsNullOrEmpty(asGivenName))
                {
                    resultList = resultList.Where(w => w.AS_SURNAME == asSurName && w.AS_GIVEN_NAME == asGivenName);
                }

                if (!string.IsNullOrEmpty(asChineseName))
                {
                    resultList = resultList.Where(w => w.AS_CHINESE_NAME == asChineseName);
                }

                return resultList.ToList();

            }
        }

        //getItemNosByCertNo
        public List<String> getItemNosByCertNo(string certNo)
        {
            string sSql = @"SELECT Replace(ITEM_CODE, 'Item ', '')
                            FROM   V_CRM_INFO
                            WHERE  CERTIFICATION_NO = :certNo
                                   AND ITEM_CODE IS NOT NULL
                            ORDER  BY ITEM_CODE ";
            OracleParameter[] oracleParameter = new OracleParameter[]
            {
                new OracleParameter(":certNo",certNo)
            };

            return GetObjectData<String>(sSql, oracleParameter).ToList();
        }

        //findActiveListByCertNo
        public List<V_CRM_INFO> findActiveListByCertNo(string certNo)
        {
            string sSql = @"SELECT *
                            FROM   V_CRM_INFO
                            WHERE  CERTIFICATION_NO = :certNo
                                   AND ( ( Code IN ( 'MWC', 'SC(GI)', 'RC', 'GBC',
                                                     'SC(D)', 'SC(F)', 'SC(SF)', 'MWC(P)' )
                                           AND ( Status = 'Active'
                                                 AND ( As_Expiry_Date IS NULL
                                                        OR As_Expiry_Date < SysDate ) ) )
                                          OR Code NOT IN ( 'MWC', 'SC(GI)', 'RC', 'GBC',
                                                           'SC(D)', 'SC(F)', 'SC(SF)', 'MWC(P)' ) ) ";
            OracleParameter[] oracleParameter = new OracleParameter[]
            {
                new OracleParameter(":certNo",certNo)
            };

            return GetObjectData<V_CRM_INFO>(sSql, oracleParameter).ToList();
        }
    }
}