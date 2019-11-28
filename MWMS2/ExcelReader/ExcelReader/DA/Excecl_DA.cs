using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ExcelReader.DA
{
    public class Excecl_DA
    {

        private DBHelper _DB;
        protected DBHelper DB
        {
            get { return _DB ?? (_DB = new DBHelper()); }
        }


        public void InsertExcelDataToDB(DataTable dt,string sheetName)
        {

            string sql = @"Insert into MW_SUBMISSION_NO 
                           (
                             UUID
                            ,SCREENED_NATURE
                            ,DSN
                            ,DATE_RECEIVED
                            ,DOC_TYPE
                            ,REFERENCE_NO
                            ,RECEIPT_POINT
                            ,REG_NO_OF_AP
                            ,REG_NO_OF_PRC
                            ,WORKS_LOCATION
                            ,STREET
                            ,STREET_NO
                            ,BUILDING_BLOCK
                            ,FLOOR
                            ,UNIT
                            ,MW_ITEMS
                            ,SELECTED_FOR_AUDIT
                            ,SSP
                            ,DESCRIPTION
                            ,COMMENCEMENT_DATE
                            ,COMPLETION_DATE
                            ,ITEM_RELATED_TO_SIGNBOARD
                            ,ORDER_RELATED
                            ,REFERRAL_DATE
                            ,PAW
                            ,CONTACT_OF_PAW
                            ,IO_PROPERTY_MANAGEMENT
                            ,CONTACT_IO_PROPERTY_MANAGEMENT
                            ,REMARK
                            ,REPLY_LETTER_DATE
                            ,SDF
                            ,PREVIOUSLY_RELATED_MW_NO
                            ,BARCODE
                            ,SUBMISSION_RESULT
                            ,SHEETNAME
                            ) 
                            VALUES 
                            (
                             :UUID
                            ,:SCREENED_NATURE
                            ,:DSN
                            ,:DATE_RECEIVED
                            ,:DOC_TYPE
                            ,:REFERENCE_NO
                            ,:RECEIPT_POINT
                            ,:REG_NO_OF_AP
                            ,:REG_NO_OF_PRC
                            ,:WORKS_LOCATION
                            ,:STREET
                            ,:STREET_NO
                            ,:BUILDING_BLOCK
                            ,:FLOOR
                            ,:UNIT
                            ,:MW_ITEMS
                            ,:SELECTED_FOR_AUDIT
                            ,:SSP
                            ,:DESCRIPTION
                            ,:COMMENCEMENT_DATE
                            ,:COMPLETION_DATE
                            ,:ITEM_RELATED_TO_SIGNBOARD
                            ,:ORDER_RELATED
                            ,:REFERRAL_DATE
                            ,:PAW
                            ,:CONTACT_OF_PAW
                            ,:IO_PROPERTY_MANAGEMENT
                            ,:CONTACT_IO_PROPERTY_MANAGEMENT
                            ,:REMARK
                            ,:REPLY_LETTER_DATE
                            ,:SDF
                            ,:PREVIOUSLY_RELATED_MW_NO
                            ,:BARCODE
                            ,:SUBMISSION_RESULT
                            ,:SHEETNAME
                            )";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                OracleParameter[] paramList = new OracleParameter[] {
                     new OracleParameter(":UUID",Guid.NewGuid().ToString("N"))
                    ,new OracleParameter(":SCREENED_NATURE",dt.Rows[i][0])
                    ,new OracleParameter(":DSN",dt.Rows[i][1])
                    ,new OracleParameter(":DATE_RECEIVED",dt.Rows[i][2])
                    ,new OracleParameter(":DOC_TYPE",dt.Rows[i][3])
                    ,new OracleParameter(":REFERENCE_NO",dt.Rows[i][4])
                    ,new OracleParameter(":RECEIPT_POINT",dt.Rows[i][5])
                    ,new OracleParameter(":REG_NO_OF_AP",dt.Rows[i][6])
                    ,new OracleParameter(":REG_NO_OF_PRC",dt.Rows[i][7])
                    ,new OracleParameter(":WORKS_LOCATION",dt.Rows[i][8].ToString().Replace(",", " ").Replace("'", "''"))
                    ,new OracleParameter(":STREET",dt.Rows[i][9])
                    ,new OracleParameter(":STREET_NO",dt.Rows[i][10])
                    ,new OracleParameter(":BUILDING_BLOCK",dt.Rows[i][11])
                    ,new OracleParameter(":FLOOR",dt.Rows[i][12])
                    ,new OracleParameter(":UNIT",dt.Rows[i][13])
                    ,new OracleParameter(":MW_ITEMS",dt.Rows[i][14])
                    ,new OracleParameter(":SELECTED_FOR_AUDIT",dt.Rows[i][15])
                    ,new OracleParameter(":SSP",dt.Rows[i][16])
                    ,new OracleParameter(":DESCRIPTION",dt.Rows[i][17])
                    ,new OracleParameter(":COMMENCEMENT_DATE",dt.Rows[i][18])
                    ,new OracleParameter(":COMPLETION_DATE",dt.Rows[i][19])
                    ,new OracleParameter(":ITEM_RELATED_TO_SIGNBOARD",dt.Rows[i][20])
                    ,new OracleParameter(":ORDER_RELATED",dt.Rows[i][21])
                    ,new OracleParameter(":REFERRAL_DATE",dt.Rows[i][22])
                    ,new OracleParameter(":PAW",dt.Rows[i][23])
                    ,new OracleParameter(":CONTACT_OF_PAW",dt.Rows[i][24])
                    ,new OracleParameter(":IO_PROPERTY_MANAGEMENT",dt.Rows[i][25])
                    ,new OracleParameter(":CONTACT_IO_PROPERTY_MANAGEMENT",dt.Rows[i][26])
                    ,new OracleParameter(":REMARK",dt.Rows[i][27])
                    ,new OracleParameter(":REPLY_LETTER_DATE",dt.Rows[i][28])
                    ,new OracleParameter(":SDF",dt.Rows[i][29])
                    ,new OracleParameter(":PREVIOUSLY_RELATED_MW_NO",dt.Rows[i][30])
                    ,new OracleParameter(":BARCODE",dt.Rows[i][31])
                    ,new OracleParameter(":SUBMISSION_RESULT",dt.Rows[i][32])
                    ,new OracleParameter(":SHEETNAME",sheetName)
                };
                DB.ExceuteNonQuery(sql, CommandType.Text, paramList);
            }
        }

        public DataSet GetSysFunc()
        {
            string sql = "Select * from Sys_Func";
            DataSet ds = DB.query(sql);
            return ds;
        }
    }
}