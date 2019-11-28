using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using MWMS2.Utility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingLetterModuleDAOService : BaseDAOService
    {
        private const int defaultPage = 1;
        private const int defaultRpp = 10;
        private const string defaultSort = "1";
        private const int defaultSortType = 0;

        private const string SearchAck_q = ""
           + "\r\n\t" + " select * from P_MW_ACK_LETTER "
            + "\r\n\t" + " Where 1=1 ";

        private const string SearchDistMWNo_q = @" SELECT DISTINCT( MW_NO )
                                                    FROM   P_MW_ACK_LETTER
                                                    WHERE  1 = 1
                                                           AND MW_NO IS NOT NULL ";

        private const string SearchFinalRecord_q = @"SELECT PBP_NO,
                                                           PRC_NO,
                                                           ITEM_DISPLAY,
                                                           COMM_DATE,
                                                           COMP_DATE,
                                                           case when fileref_four is not null then fileref_four||'&nbsp;/&nbsp;' else '' end " + "FILEREF_FOUR" + @",
                                                           FILEREF_TWO,
                                                           DSN,
                                                           FORM_NO,
                                                           NATURE,
                                                           RECEIVED_DATE,
                                                           LETTER_DATE,
                                                           ADDRESS,
                                                           UNIT,
                                                           FLOOR,
                                                           BUILDING,
                                                           STREET_NO,
                                                           STREET,
                                                           AUDIT_RELATED
                                                    FROM   P_MW_ACK_LETTER
                                                    WHERE  FORM_NO IN( 'MW01', 'MW02', 'MW03', 'MW04',
                                                                        'MW05', 'MW06', 'MW06_01', 'MW06_02', 'MW06_03' )
                                                           AND MW_NO = :equalMWNo
                                                           AND rownum = 1
                                                    ORDER BY FORM_NO DESC";

        private const string SearchRecordsByMWNo = @"select * from P_MW_ACK_LETTER where MW_NO= :equalMWNo";

        private const string SearchMWDSN = @" select * from p_mw_dsn where 1=1 ";

        private const string SearchSPO = @"SELECT sv.*
                                            FROM   p_s_system_value sv
                                                   INNER JOIN p_s_system_type sType
                                                           ON sType.uuid = sv.system_type_id
                                                              AND sType.type = 'ACK_LETTER_TEMPLETE'
                                            WHERE  code IN ( 'CHT_SPO_NAME', 'CHT_SPO_POSITION', 'ENG_SPO_NAME', 'ENG_SPO_POSITION' ) 
                                            ";
        private const string SearchAL = @"SELECT DISTINCT AL.PO_POST,
                                                            AL.DSN,
                                                            AL.RECEIVED_DATE,
                                                            AL.LETTER_DATE,
                                                            AL.MW_NO,
                                                            AL.FORM_NO,
                                                            AL.PBP_NO,
                                                            AL.PRC_NO,
                                                            AL.ADDRESS,
                                                            AL.MW_ITEM
                                            FROM   P_MW_ACK_LETTER AL
                                                   LEFT JOIN P_S_TO_DETAILS TD
                                                          ON AL.PO_POST = TD.PO_POST
                                            WHERE  AL.IN_AL_LIST = 'Y' ";
        private const string SearchCompletion = @"  SELECT *
                                                    FROM   P_MW_ACK_LETTER ack
                                                           JOIN(SELECT *
                                                                FROM  (SELECT Count(MW_NO) cout,
                                                                              MW_NO
                                                                       FROM   P_MW_ACK_LETTER
                                                                       WHERE  Form_NO IN ( 'MW01', 'MW02', 'MW03', 'MW04' )
                                                                       GROUP  BY MW_NO)
                                                                WHERE  cout = 1) noCom
                                                             ON noCom.MW_NO = ack.MW_NO
                                                    WHERE  Form_NO IN ( 'MW01', 'MW03' )
                                            ";


        #region Acknowledgement
        public ServiceResult CheckAddress(Fn01LM_AckSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var address = db.P_S_ADDRESS_LIST.Where(m => m.STREET == model.P_MW_ACK_LETTER.STREET && m.STREET_NO == model.P_MW_ACK_LETTER.STREET_NO && m.BUILDING == model.P_MW_ACK_LETTER.BUILDING && m.FLOOR == model.P_MW_ACK_LETTER.FLOOR && m.UNIT == model.P_MW_ACK_LETTER.UNIT).FirstOrDefault();
                if (address != null)
                {
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }

                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_FAILURE
                };
            }
        }

        public ServiceResult SaveAckLetter(Fn01LM_AckSearchModel model, EntitiesMWProcessing db)
        {
            //Modify by dive 20191011
            model.P_MW_DSN = db.P_MW_DSN.Where(m => m.DSN == model.SearchDSN).FirstOrDefault();
            db.P_MW_ACK_LETTER.Add(model.P_MW_ACK_LETTER);

            db.SaveChanges();
            //tran.Commit();

            return new ServiceResult()
            {
                Result = ServiceResult.RESULT_SUCCESS
                ,
                Message = new List<string>()
                        {
                            model.P_MW_ACK_LETTER.UUID
                            ,model.P_MW_ACK_LETTER.LETTER_DATE.ToString()
                        }
            };

        }

        public IEnumerable<P_MW_ACK_LETTER> GetAckLetter(string mwNo)
        {
            string sSql = @"SELECT *
                            FROM   P_MW_ACK_LETTER
                            WHERE  MW_NO = :MW_NO
                            ORDER  BY FORM_NO ASC,
                                      MODIFIED_DATE ASC ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":MW_NO",mwNo)
            };

            return GetObjectData<P_MW_ACK_LETTER>(sSql, oracleParameters);
        }

        public void UpdateAckLetter(Fn01LM_AckSearchModel model, EntitiesMWProcessing db)
        {
            //Modify by dive 20191012

            var origModel = db.P_MW_ACK_LETTER.Where(m => m.UUID == model.P_MW_ACK_LETTER.UUID).FirstOrDefault();
            origModel.COUNTER = model.P_MW_ACK_LETTER.COUNTER;
            origModel.NATURE = model.P_MW_ACK_LETTER.NATURE;
            origModel.RECEIVED_DATE = model.P_MW_ACK_LETTER.RECEIVED_DATE;
            origModel.LETTER_DATE = model.P_MW_ACK_LETTER.LETTER_DATE;
            origModel.EFSS_REF_NO = model.P_MW_ACK_LETTER.EFSS_REF_NO;
            origModel.DSN = model.P_MW_ACK_LETTER.DSN;
            origModel.MW_NO = model.P_MW_ACK_LETTER.MW_NO;
            origModel.FORM_NO = model.P_MW_ACK_LETTER.FORM_NO;
            origModel.COMP_DATE = model.P_MW_ACK_LETTER.COMP_DATE;
            origModel.MW_ITEM = model.P_MW_ACK_LETTER.MW_ITEM;
            origModel.ITEM_DISPLAY = model.P_MW_ACK_LETTER.ITEM_DISPLAY;
            //origModel.AUDIT_RELATED = model.P_MW_ACK_LETTER.AUDIT_RELATED;
            //origModel.ORDER_RELATED = model.P_MW_ACK_LETTER.ORDER_RELATED;

            // Special Logic for Order_related
            if (ProcessingConstant.FLAG_Y.Equals(model.P_MW_ACK_LETTER.AUDIT_RELATED))
            {
                origModel.ORDER_RELATED = ProcessingConstant.FLAG_N;
            }
            else
            {
                origModel.ORDER_RELATED = model.P_MW_ACK_LETTER.ORDER_RELATED;
            }

            origModel.SDF_RELATED = model.P_MW_ACK_LETTER.SDF_RELATED;
            origModel.SIGNBOARD_RELATED = model.P_MW_ACK_LETTER.SIGNBOARD_RELATED;
            origModel.SSP = model.P_MW_ACK_LETTER.SSP;
            origModel.PBP_NO = model.P_MW_ACK_LETTER.PBP_NO;
            origModel.PRC_NO = model.P_MW_ACK_LETTER.PRC_NO;
            origModel.ADDRESS = model.P_MW_ACK_LETTER.ADDRESS;
            origModel.BUILDING = model.P_MW_ACK_LETTER.BUILDING;
            origModel.STREET = model.P_MW_ACK_LETTER.STREET;
            origModel.STREET_NO = model.P_MW_ACK_LETTER.STREET_NO;
            origModel.FLOOR = model.P_MW_ACK_LETTER.FLOOR;
            origModel.UNIT = model.P_MW_ACK_LETTER.UNIT;
            origModel.PAW = model.P_MW_ACK_LETTER.PAW;
            origModel.PAW_CONTACT = model.P_MW_ACK_LETTER.PAW_CONTACT;
            origModel.IO_MGT = model.P_MW_ACK_LETTER.IO_MGT;
            origModel.IO_MGT_CONTACT = model.P_MW_ACK_LETTER.IO_MGT_CONTACT;
            origModel.REMARK = model.P_MW_ACK_LETTER.REMARK;
            origModel.LANGUAGE = model.P_MW_ACK_LETTER.LANGUAGE;
            origModel.FILE_TYPE = model.P_MW_ACK_LETTER.FILE_TYPE;
            origModel.REFERRAL_DATE = model.P_MW_ACK_LETTER.REFERRAL_DATE;
            origModel.EMAIL_OF_PBP = model.P_MW_ACK_LETTER.EMAIL_OF_PBP;
            origModel.EMAIL_OF_PRC = model.P_MW_ACK_LETTER.EMAIL_OF_PRC;
            origModel.FILEREF_FOUR = model.P_MW_ACK_LETTER.FILEREF_FOUR;
            origModel.FILEREF_TWO = model.P_MW_ACK_LETTER.FILEREF_TWO;

            db.SaveChanges();

        }
        public ServiceResult DeleteAckLetter(string id)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    P_MW_ACK_LETTER ack = db.P_MW_ACK_LETTER.Where(m => m.UUID == id).FirstOrDefault();
                    try
                    {
                        db.P_MW_ACK_LETTER.Remove(ack);
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }
            }
        }

        public List<P_MW_DW_LETTER_ITEM> createMwDwItems(P_MW_ACK_LETTER ack)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                List<P_S_SYSTEM_VALUE> sysItems = db.P_S_SYSTEM_VALUE.Where(x => x.SYSTEM_TYPE_ID == ProcessingConstant.S_TYPE_ACK_LETTER_CHECKBOX && x.IS_ACTIVE == ProcessingConstant.FLAG_Y).OrderBy(x => x.CODE).ToList();
                List<P_MW_DW_LETTER_ITEM> itemList = new List<P_MW_DW_LETTER_ITEM>();
                foreach (var sysItem in sysItems)
                {
                    P_MW_DW_LETTER_ITEM item = new P_MW_DW_LETTER_ITEM();
                    item.MW_ACK_LETTER_ID = ack.UUID;
                    item.ITEM_ID = sysItem.UUID;
                    item.ITEM_NO = sysItem.CODE;
                    item.ITEM_TEXT = sysItem.DESCRIPTION;
                    item.ITEM_TEXT_E = sysItem.DESCRIPTION_E;
                    item.CHECKED = ProcessingConstant.FLAG_Y;
                    item.LETTER_TYPE = ProcessingConstant.AL_LETTER_ITEM_LETTER_TYPE;

                    itemList.Add(item);
                }
                db.P_MW_DW_LETTER_ITEM.AddRange(itemList);
                db.SaveChanges();

                return itemList;
            }
        }

        public void saveMwDwLetterItemForAckLetter(List<P_MW_DW_LETTER_ITEM> newItems, P_MW_ACK_LETTER ack)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                List<P_MW_DW_LETTER_ITEM> currentItems = db.P_MW_DW_LETTER_ITEM.Where(x => x.MW_ACK_LETTER_ID == ack.UUID).ToList();
                if (currentItems != null && currentItems.Count() > 0)
                {
                    db.P_MW_DW_LETTER_ITEM.RemoveRange(currentItems);
                }
                db.P_MW_DW_LETTER_ITEM.AddRange(newItems);
                db.SaveChanges();
            }
        }

        public Fn01LM_AckSearchModel SearchDSN(Fn01LM_AckSearchModel model)
        {
            model.Query = SearchAck_q;
            model.Search();
            return model;
        }

        public Fn01LM_AckSearchModel SearchReceivedDate(Fn01LM_AckSearchModel model)
        {
            model.Query = SearchAck_q;
            model.Search();
            return model;
        }

        public Fn01LM_AckSearchModel GetAckLetterById(string id)
        {
            Fn01LM_AckSearchModel model = new Fn01LM_AckSearchModel();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                model.P_MW_ACK_LETTER = db.P_MW_ACK_LETTER.Where(ack => ack.UUID == id).FirstOrDefault();
                return model;
            }

        }

        public ServiceResult GetMWNo(string dsn, string type)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                //P_MW_DSN p_MW_DSN = db.P_MW_DSN.Where(m => m.DSN == dsn).FirstOrDefault();
                P_MW_DSN p_MW_DSN = new P_MW_DSN();
                if (type == "DSN")
                {
                    p_MW_DSN = db.P_MW_DSN.Where(m => m.DSN.EndsWith(dsn)).FirstOrDefault();
                }
                else
                {
                    p_MW_DSN = db.P_MW_DSN.Where(m => m.RECORD_ID.EndsWith(dsn)).FirstOrDefault();
                }

                if (p_MW_DSN != null)
                {
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                        ,
                        Message =
                        {
                            p_MW_DSN==null?"":p_MW_DSN.RECORD_ID
                            ,p_MW_DSN==null?"":p_MW_DSN.FORM_CODE
                            ,p_MW_DSN == null?"":p_MW_DSN.DSN
                        }
                    };
                }
                Dictionary<string, List<string>> errorMessage = new Dictionary<string, List<string>>();
                errorMessage.Add("P_MW_ACK_LETTER.DSN", new List<string>() {
                    "DSN NOT FOUND"
                });
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_FAILURE
                    ,
                    ErrorMessages = errorMessage
                };

            }
        }

        public P_MW_ACK_LETTER GetACKLetter(P_MW_ACK_LETTER model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_ACK_LETTER tmpModel = db.P_MW_ACK_LETTER.Where(m => m.DSN == model.DSN).FirstOrDefault();
                if (tmpModel == null)
                {
                    return null;
                }
                model.UUID = tmpModel.UUID;
                model.COUNTER = tmpModel.COUNTER;
                model.NATURE = tmpModel.NATURE;
                model.RECEIVED_DATE = tmpModel.RECEIVED_DATE;
                model.LETTER_DATE = tmpModel.LETTER_DATE;
                model.FILEREF_FOUR = tmpModel.FILEREF_FOUR;
                model.FILEREF_TWO = tmpModel.FILEREF_TWO;
                model.COMP_DATE = tmpModel.COMP_DATE;
                model.MW_ITEM = tmpModel.MW_ITEM;
                model.ORDER_RELATED = tmpModel.ORDER_RELATED;
                model.SSP = tmpModel.SSP;
                model.PBP_NO = tmpModel.PBP_NO;
                model.EMAIL_OF_PBP = tmpModel.EMAIL_OF_PBP;
                model.PRC_NO = tmpModel.PRC_NO;
                model.EMAIL_OF_PRC = tmpModel.EMAIL_OF_PRC;
                model.COMM_DATE = tmpModel.COMM_DATE;
                model.PREVIOUS_RELATED_MW_NO = tmpModel.PREVIOUS_RELATED_MW_NO;
                model.ADDRESS = tmpModel.ADDRESS;
                model.STREET = tmpModel.STREET;
                model.STREET_NO = tmpModel.STREET_NO;
                model.BUILDING = tmpModel.BUILDING;
                model.FLOOR = tmpModel.FLOOR;
                model.UNIT = tmpModel.UNIT;
                model.PAW = tmpModel.PAW;
                model.PAW_CONTACT = tmpModel.PAW_CONTACT;
                model.IO_MGT = tmpModel.IO_MGT;
                model.IO_MGT_CONTACT = tmpModel.IO_MGT_CONTACT;
                model.REMARK = tmpModel.REMARK;
                model.LANGUAGE = tmpModel.LANGUAGE;
                model.FILE_TYPE = tmpModel.FILE_TYPE;
                model.BARCODE = tmpModel.BARCODE;
            }
            return model;
        }

        //利用mwAckLetter.MW_NO, 找到ACK.List
        public List<P_MW_ACK_LETTER> GetListACKLetterByMWNo(P_MW_ACK_LETTER mwAckLetter)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_ACK_LETTER.Where(m => m.MW_NO == mwAckLetter.MW_NO).ToList();
            }
        }

        public Fn01LM_AckPrintModel GetPrintModel(string id)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_ACK_LETTER p_MW_ACK_LETTER = db.P_MW_ACK_LETTER.Where(m => m.UUID == id).FirstOrDefault();
                Fn01LM_AckPrintModel model = new Fn01LM_AckPrintModel()
                {
                    FormOn = p_MW_ACK_LETTER.FORM_NO
                    ,
                    address = p_MW_ACK_LETTER.ADDRESS
                    ,
                    unit = p_MW_ACK_LETTER.UNIT
                    ,
                    floor = p_MW_ACK_LETTER.FLOOR
                    ,
                    building = p_MW_ACK_LETTER.BUILDING
                    ,
                    street = p_MW_ACK_LETTER.STREET
                    ,
                    streetNo = p_MW_ACK_LETTER.STREET_NO
                    ,
                    PBPNo = p_MW_ACK_LETTER.PBP_NO
                    ,
                    pbpName = ""
                    ,
                    pbpContact = ""
                    ,
                    PRCNo = p_MW_ACK_LETTER.PRC_NO
                    ,
                    prcName = ""
                    ,
                    prcContact = ""
                    ,
                    mwno = p_MW_ACK_LETTER.MW_NO
                    ,
                    enqno = p_MW_ACK_LETTER.EFSS_REF_NO
                    ,
                    ryear = p_MW_ACK_LETTER.RECEIVED_DATE != null ? p_MW_ACK_LETTER.RECEIVED_DATE.Value.Year.ToString() : ""
                    ,
                    rmonth = p_MW_ACK_LETTER.RECEIVED_DATE != null ? p_MW_ACK_LETTER.RECEIVED_DATE.Value.Month.ToString() : ""
                    ,
                    rday = p_MW_ACK_LETTER.RECEIVED_DATE != null ? p_MW_ACK_LETTER.RECEIVED_DATE.Value.Day.ToString() : ""
                    ,
                    paw = p_MW_ACK_LETTER.PAW
                    ,
                    pawContact = p_MW_ACK_LETTER.PAW_CONTACT
                    ,
                    lyear = p_MW_ACK_LETTER.LETTER_DATE != null ? p_MW_ACK_LETTER.LETTER_DATE.Value.Year.ToString() : ""
                    ,
                    lmonth = p_MW_ACK_LETTER.LETTER_DATE != null ? p_MW_ACK_LETTER.LETTER_DATE.Value.Month.ToString() : ""
                    ,
                    lday = p_MW_ACK_LETTER.LETTER_DATE != null ? p_MW_ACK_LETTER.LETTER_DATE.Value.Day.ToString() : ""
                    ,
                    Language = p_MW_ACK_LETTER.LANGUAGE
                    ,
                    letterDate = p_MW_ACK_LETTER.LETTER_DATE != null ? Convert.ToDateTime(p_MW_ACK_LETTER.LETTER_DATE).ToString("dd MMMM, yyyy", CultureInfo.CreateSpecificCulture("en-GB")) : "",
                    ReceiveYear = p_MW_ACK_LETTER.RECEIVED_DATE != null ? p_MW_ACK_LETTER.RECEIVED_DATE.Value.ToString("yyyy") : DateTime.Now.ToString("yyyy"),
                    ReceiveMonth = p_MW_ACK_LETTER.RECEIVED_DATE != null ? p_MW_ACK_LETTER.LANGUAGE == ProcessingConstant.LANGUAGE_RADIO_ENGLISH ? p_MW_ACK_LETTER.RECEIVED_DATE.Value.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-GB")) : p_MW_ACK_LETTER.RECEIVED_DATE.Value.ToString("MM") : p_MW_ACK_LETTER.LANGUAGE == ProcessingConstant.LANGUAGE_RADIO_ENGLISH ? DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-GB")) : DateTime.Now.ToString("MM"),
                    ReceiveDay = p_MW_ACK_LETTER.RECEIVED_DATE != null ? p_MW_ACK_LETTER.RECEIVED_DATE.Value.ToString("dd") : DateTime.Now.ToString("dd")
                };
                return model;
            }
        }

        public List<P_S_SYSTEM_VALUE> GeSPO(Fn01LM_AckPrintModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                List<P_S_SYSTEM_VALUE> spo = GetObjectData<P_S_SYSTEM_VALUE>(SearchSPO).ToList();
                return spo;
            }
        }

        public string Excel(Fn01LM_AckSearchModel model)
        {
            model.Query = SearchAck_q;
            return model.Export("Acknowlegdement");
        }

        public SYS_EMAIL_SENDER GetEmailSender(string dsn, string type)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var sender = db.SYS_EMAIL_SENDER.Where(m => m.DSN == dsn && m.EMAIL_TYPE == type).FirstOrDefault();
                return sender;
            }
        }

        public ServiceResult AddEmailSender(string dsn, string email, string type, byte[] letter)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {

                        SYS_EMAIL_SENDER emailSender = new SYS_EMAIL_SENDER()
                        {
                            RECIPIENT = email
                            ,
                            CC = "test1"
                            ,
                            SUBJECT = "test1"
                            ,
                            EMAILCONTENT = "ACK Letter test1"
                            ,
                            STATUS = ProcessingConstant.EMAIL_STATUS_READY
                            ,
                            ATTACHMENT = letter
                            ,
                            DSN = dsn
                            ,
                            EMAIL_TYPE = type
                        };
                        db.SYS_EMAIL_SENDER.Add(emailSender);

                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        AuditLogService.logDebug(ex);
                        tran.Rollback();
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }
        }
        #endregion

        #region Common

        public List<Dictionary<string, object>> SearchMWNo(DisplayGrid model, string queryWhere)
        {
            model.Rpp = model.Rpp <= 0 ? defaultRpp : model.Rpp;
            model.Page = model.Page <= 0 ? defaultPage : model.Page;
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {

                    string sort = "MW_NO";
                    //string query = @"select distinct(MW_NO) from P_MW_ACK_LETTER where 1=1 ";
                    DbDataReader dr = CommonUtil.GetDataReader(conn
                                , "SELECT * FROM (SELECT a.*, rownum r__ FROM (" + SearchDistMWNo_q + "\r\n\r\n" + queryWhere + " ORDER BY " + (sort == null ? "1" : sort) + (defaultSortType == 1 ? " DESC " : " ASC") + " ) a WHERE rownum <" + (model.Rpp * model.Page + 1) + ") WHERE r__ >= " + ((model.Page - 1) * model.Rpp + 1)
                                , model.QueryParameters);
                    List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);

                    return Data;
                }
            }
        }
        public int GetSearchCount(string queryStr, string queryWhere, Dictionary<string, object> queryParams, DbConnection conn)
        {
            return CommonUtil.LoadDbCount(CommonUtil.GetDataReader(conn
                        , "SELECT COUNT(*) FROM (" + queryStr + "\r\n\r\n" + queryWhere + ")"
                        , queryParams));
        }

        #endregion

        #region Minor Works List,Search,Signboard,Order

        public Fn01LM_SearchModel GetACKLettersDividedByMWNo(Fn01LM_SearchModel model, string lettersWhereq)
        {
            model.Query = SearchDistMWNo_q;
            model.Search();

            List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
            OracleParameter[] sqlparas = new OracleParameter[model.QueryParameters.Count() + 1];
            int parameterIndex = 0;
            foreach (var parameter in model.QueryParameters)
            {
                sqlparas[parameterIndex] = new OracleParameter(parameter.Key, parameter.Value);
                parameterIndex++;
            }
            foreach (var item in model.Data)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                string mwno = item["MW_NO"].ToString();


                sqlparas[parameterIndex] = new OracleParameter("equalMWNo", mwno);
                P_MW_ACK_LETTER finalRecord = GetObjectData<P_MW_ACK_LETTER>(SearchFinalRecord_q, sqlparas).FirstOrDefault();
                if (finalRecord != null)
                {
                    finalRecord = GetLetterAdress(finalRecord);
                }

                List<P_MW_ACK_LETTER> ackRecords = GetObjectData<P_MW_ACK_LETTER>(SearchRecordsByMWNo + lettersWhereq, sqlparas).ToList();

                data["MW_NO"] = mwno;
                data["FINAL_RECORD"] = finalRecord;
                data["ACK_RECORDS"] = ackRecords;
                datas.Add(data);
            }
            model.Data = datas;
            return model;
        }
        public P_MW_ACK_LETTER GetLetterAdress(P_MW_ACK_LETTER model)
        {
            if (string.IsNullOrWhiteSpace(model.ADDRESS))
            {
                if (model.LANGUAGE == ProcessingConstant.LANGUAGE_RADIO_ENGLISH)
                {
                    StringBuilder addressFirstComponent = new StringBuilder();
                    StringBuilder addressSecondComponent = new StringBuilder();

                    if (!string.IsNullOrWhiteSpace(model.UNIT))
                    {
                        addressFirstComponent.Append(model.UNIT + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.FLOOR))
                    {
                        addressFirstComponent.Append(model.FLOOR + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.BUILDING))
                    {
                        addressFirstComponent.Append(model.BUILDING + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(addressFirstComponent.ToString()))
                    {
                        addressFirstComponent.Append(", ");
                    }

                    if (!string.IsNullOrWhiteSpace(model.STREET_NO))
                    {
                        addressSecondComponent.Append(model.STREET_NO + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.STREET))
                    {
                        addressSecondComponent.Append(model.STREET);
                    }
                    model.ADDRESS = addressFirstComponent.ToString() + addressSecondComponent.ToString();
                }
                else if (model.LANGUAGE == ProcessingConstant.LANGUAGE_RADIO_CHINESE)
                {
                    StringBuilder fullAddress = new StringBuilder();
                    if (!string.IsNullOrWhiteSpace(model.STREET))
                    {
                        fullAddress.Append(model.STREET + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.STREET_NO))
                    {
                        fullAddress.Append(model.STREET_NO + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.BUILDING))
                    {
                        fullAddress.Append(model.BUILDING + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.FLOOR))
                    {
                        fullAddress.Append(model.FLOOR + " ");
                    }
                    if (!string.IsNullOrWhiteSpace(model.UNIT))
                    {
                        fullAddress.Append(model.UNIT + " ");
                    }
                    model.ADDRESS = fullAddress.ToString();
                }
            }
            else
            {
                model.ADDRESS = model.ADDRESS.Trim();
            }
            return model;
        }

        public Fn01LM_SearchModel GetSearchExcelData(Fn01LM_SearchModel model)
        {
            model.Query = SearchAck_q;
            model.Rpp = -1;
            model.Search();

            return model;
        }

        //public ServiceResult GetSearchDSNMWNO(string type,string str)
        //{
        //    using(EntitiesMWProcessing db = new EntitiesMWProcessing())
        //    {
        //        P_MW_ACK_LETTER model = new P_MW_ACK_LETTER();
        //        if (type == "DSN")
        //        {
        //            model = db.P_MW_ACK_LETTER.Where(m => m.DSN.EndsWith(str)).FirstOrDefault();
        //        }
        //        else
        //        {
        //             model = db.P_MW_ACK_LETTER.Where(m => m.MW_NO.EndsWith(str)).FirstOrDefault();
        //        }
        //        if (model != null)
        //        {
        //            return new ServiceResult()
        //            {
        //                Result = ServiceResult.RESULT_SUCCESS
        //                ,
        //                Message =
        //                    {
        //                        model.DSN
        //                        ,
        //                        model.MW_NO
        //                    }
        //            };
        //        }
        //        Dictionary<string, List<string>> errerMessage = new Dictionary<string, List<string>>();
        //        errerMessage.Add("")
        //    }

        //}


        #endregion

        #region Audit List Management

        public Fn01LM_ALMSearchModel SearchALM(Fn01LM_ALMSearchModel model, string lettersWhereq)
        {
            model.Query = SearchDistMWNo_q + " AND IN_AL_LIST = 'Y' ";
            model.Sort = " MW_NO ";
            model.SortType = 1;
            model.Search();

            List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
            OracleParameter[] sqlparas = new OracleParameter[model.QueryParameters.Count() + 1];
            int parameterIndex = 0;
            foreach (var parameter in model.QueryParameters)
            {
                sqlparas[parameterIndex] = new OracleParameter(parameter.Key, parameter.Value);
                parameterIndex++;
            }
            foreach (var item in model.Data)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                string mwno = item["MW_NO"].ToString();


                sqlparas[parameterIndex] = new OracleParameter("equalMWNo", mwno);
                //P_MW_ACK_LETTER finalRecord = GetObjectData<P_MW_ACK_LETTER>(SearchFinalRecord_q, sqlparas).FirstOrDefault();

                List<P_MW_ACK_LETTER> ackRecords = GetObjectData<P_MW_ACK_LETTER>(SearchRecordsByMWNo + " AND IN_AL_LIST = 'Y' " + lettersWhereq, sqlparas).ToList();

                data["MW_NO"] = mwno;
                //data["FINAL_RECORD"] = finalRecord;
                data["ACK_RECORDS"] = ackRecords;
                datas.Add(data);
            }
            model.Data = datas;

            return model;
        }

        public ServiceResult PickAuditAFC(string id)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_ACK_LETTER ack = db.P_MW_ACK_LETTER.Where(m => m.UUID == id).FirstOrDefault();
                ack.AUDIT_RELATED = "Y";
                try
                {
                    if (db.SaveChanges() > 0)
                    {
                        return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                    }
                    else
                    {
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE };
                    }
                }
                catch (DbEntityValidationException validationError)
                {
                    var errorMessages = validationError.EntityValidationErrors
                    .SelectMany(validationResult => validationResult.ValidationErrors)
                    .Select(m => m.ErrorMessage);
                    Console.WriteLine("Error :" + validationError.Message);
                    return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error :" + ex.Message);
                    return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                }
            }
        }

        public ServiceResult PickAuditSAC(string id)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_ACK_LETTER ack = db.P_MW_ACK_LETTER.Where(m => m.UUID == id).FirstOrDefault();
                ack.SITE_AUDIT_RELATED = "Y";
                try
                {
                    if (db.SaveChanges() > 0)
                    {
                        return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                    }
                    else
                    {
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE };
                    }
                }
                catch (DbEntityValidationException validationError)
                {
                    var errorMessages = validationError.EntityValidationErrors
                    .SelectMany(validationResult => validationResult.ValidationErrors)
                    .Select(m => m.ErrorMessage);
                    Console.WriteLine("Error :" + validationError.Message);
                    return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error :" + ex.Message);
                    return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                }
            }
        }

        public ServiceResult PickAuditPSAC(string id)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_ACK_LETTER ack = db.P_MW_ACK_LETTER.Where(m => m.UUID == id).FirstOrDefault();
                ack.PRE_SITE_AUDIT_RELATED = "Y";
                try
                {
                    if (db.SaveChanges() > 0)
                    {
                        return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                    }
                    else
                    {
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE };
                    }
                }
                catch (DbEntityValidationException validationError)
                {
                    var errorMessages = validationError.EntityValidationErrors
                    .SelectMany(validationResult => validationResult.ValidationErrors)
                    .Select(m => m.ErrorMessage);
                    Console.WriteLine("Error :" + validationError.Message);
                    return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error :" + ex.Message);
                    return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                }
            }
        }

        public ServiceResult PickAudit(Fn01LM_ALMSearchModel model, string lettersWhereq)
        {
            List<P_MW_ACK_LETTER> pickAuditAcks = new List<P_MW_ACK_LETTER>();
            OracleParameter[] sqlparas = new OracleParameter[model.QueryParameters.Count() + 1];
            int parameterIndex = 0;
            foreach (var parameter in model.QueryParameters)
            {
                sqlparas[parameterIndex] = new OracleParameter(parameter.Key, parameter.Value);
                parameterIndex++;
            }
            foreach (var item in model.MWNOs)
            {
                sqlparas[parameterIndex] = new OracleParameter("equalMWNo", item);

                List<P_MW_ACK_LETTER> ackRecords = GetObjectData<P_MW_ACK_LETTER>(SearchRecordsByMWNo + " AND IN_AL_LIST = 'Y' " + lettersWhereq, sqlparas).ToList();

                pickAuditAcks.AddRange(ackRecords);
            }

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in pickAuditAcks)
                        {
                            P_MW_ACK_LETTER ack = db.P_MW_ACK_LETTER.Where(m => m.UUID == item.UUID).FirstOrDefault();
                            ack.AUDIT_RELATED = "Y";
                            ack.SITE_AUDIT_RELATED = "Y";
                            ack.PRE_SITE_AUDIT_RELATED = "Y";
                        }
                        int count = db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        AuditLogService.logDebug(ex);
                        tran.Rollback();
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }
        }

        public Fn01LM_ALMSearchModel Completion(Fn01LM_ALMSearchModel model)
        {
            //model.Query = SearchAck_q;
            model.Query = SearchCompletion;
            model.Search();
            return model;
        }

        public int Validaton_DSN(string dsn)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_ACK_LETTER ack = db.P_MW_ACK_LETTER.Where(x => x.DSN == dsn).FirstOrDefault();
                if (ack != null)
                {
                    List<P_MW_ACK_LETTER> ackList = db.P_MW_ACK_LETTER.Where(x => x.MW_NO == ack.MW_NO).ToList();
                    P_MW_ACK_LETTER nAck = ackList.Where(x => x.AUDIT_RELATED == "Y").FirstOrDefault();
                    P_MW_ACK_LETTER nAckOrderRelatedList = ackList.Where(x => x.ORDER_RELATED == "Y").FirstOrDefault();
                    if (nAck != null)
                    {
                        return -1;
                    }
                    if (nAckOrderRelatedList != null)
                    {
                        return -2;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                    return 0;
            }
        }

        public ServiceResult UpdateAuditRelated(Fn01LM_ALMSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {

                    P_MW_ACK_LETTER ack = db.P_MW_ACK_LETTER.Where(x => x.DSN == model.DocumentSN).FirstOrDefault();
                    List<P_MW_ACK_LETTER> ackList = db.P_MW_ACK_LETTER.Where(x => x.MW_NO == ack.MW_NO).ToList();
                    List<P_MW_RECORD> P_MW_RECORDs = db.P_MW_RECORD.Where(o => o.MW_DSN == model.DocumentSN).ToList();
                    P_MW_DSN dsn = db.P_MW_DSN.Where(o => o.DSN == model.DocumentSN).FirstOrDefault();
                    P_S_SYSTEM_TYPE stype = db.P_S_SYSTEM_TYPE.Where(o => o.TYPE == ProcessingConstant.DSN_STATUS).FirstOrDefault();
                    P_S_SYSTEM_VALUE svalue = db.P_S_SYSTEM_VALUE.Where(o => o.CODE == ProcessingConstant.WILL_SCAN).Where(o => o.SYSTEM_TYPE_ID == stype.UUID).FirstOrDefault();
                    dsn.SCANNED_STATUS_ID = svalue.UUID;
                    //dsn.SCANNED_STATUS_ID
                    for (int i = 0; i < ackList.Count; i++)
                    {
                        P_MW_ACK_LETTER item = ackList[i];

                        item.AUDIT_RELATED = ProcessingConstant.FLAG_Y;
                        if (ProcessingConstant.FORM_01.Equals(item.FORM_NO) || ProcessingConstant.FORM_03.Equals(item.FORM_NO))
                        {
                            if (i == 0)
                            {
                                ProcessingLetterModuleBLService s = new ProcessingLetterModuleBLService();
                                s.randomPickAudit(item, "PSAC");

                                for (int j = 0; j < P_MW_RECORDs.Count; j++)
                                {
                                    P_MW_RECORDs[j].PRE_SITE_AUDIT_RELATED = ackList[0].PRE_SITE_AUDIT_RELATED;
                                    P_MW_RECORDs[j].SITE_AUDIT_RELATED = "Y";
                                    // if (P_MW_RECORDs[j].STATUS_CODE == ProcessingConstant.SECOND_ENTRY_COMPLETED)
                                    //     P_MW_RECORDs[j].STATUS_CODE = ProcessingConstant.WILL_SCAN;
                                }
                            }
                            else
                            {
                                item.PRE_SITE_AUDIT_RELATED = ackList[0].PRE_SITE_AUDIT_RELATED;
                            }
                        }
                        item.REMARK = "This case is manually picked for audit under:" + model.SERemark;
                    }
                    try
                    {
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (DbEntityValidationException validationError)
                    {
                        var errorMessages = validationError.EntityValidationErrors
                        .SelectMany(validationResult => validationResult.ValidationErrors)
                        .Select(m => m.ErrorMessage);
                        Console.WriteLine("Error :" + validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }

            }
        }



        #endregion

        #region Advisory Letters
        public ServiceResult GetPOInfo(string popost)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_S_TO_DETAILS p_S_TO_DETAILS = db.P_S_TO_DETAILS.Where(m => m.UUID == popost).FirstOrDefault();
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_SUCCESS
                    ,
                    Message = new List<string>()
                    {
                        p_S_TO_DETAILS.PO_NAME_CHI
                        ,p_S_TO_DETAILS.PO_CONTACT
                    }
                };
            }
        }

        public Fn01LM_ALSearchModel GetALList(Fn01LM_ALSearchModel model)
        {
            model.Query = SearchAL;
            model.Search();
            return model;
        }

        public string ExportALListExcel(Fn01LM_ALSearchModel model)
        {
            model.Query = SearchAL;
            return model.Export("ExportData");
        }

        public void setInAlList(string ackLetterUuid, string flag)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var ack = db.P_MW_ACK_LETTER.Find(ackLetterUuid);
                ack.IN_AL_LIST = flag;
                db.SaveChanges();
            }
        }

        public int UpdateALInfo(P_MW_ACK_LETTER model, EntitiesMWProcessing db)
        {
            P_MW_ACK_LETTER record = db.P_MW_ACK_LETTER.Where(w => w.UUID == model.UUID).FirstOrDefault();

            record.IN_AL_LIST = model.IN_AL_LIST;
            record.PO_POST = model.PO_POST;

            return db.SaveChanges();

        }
        #endregion

        #region Statistics

        public List<int> GetMWSubmissionData(SearchSubmissionReceivedModel model, string queryString)
        {
            OracleParameter[] oracleParameters = new OracleParameter[]
            {

                 new OracleParameter(":ReceivedDateFrom",Convert.ToDateTime(model.ReceivedDateFrom))
                ,new OracleParameter(":ReceivedDateTo",Convert.ToDateTime(model.ReceivedDateTo))
                ,new OracleParameter(":LetterDateFrom",Convert.ToDateTime(model.LetterDateFrom))
                ,new OracleParameter(":LetterDateTo",Convert.ToDateTime(model.LetterDateTo))
                ,new OracleParameter(":ReferralDateFrom",Convert.ToDateTime(model.ReceivedDateFrom))
                ,new OracleParameter(":ReferralDateTo",Convert.ToDateTime(model.ReceivedDateTo))
            };
            return GetObjectData<int>(queryString, oracleParameters).ToList();
        }

        public List<Fn01LM_StatisticsIncomingModel> GetStatisticsIncomingCalendarData(GetIncomingOutgoingParameterModel model)
        {
            string queryString = string.Format(@"SELECT  
                        TO_DATE(d.DateRange) AS RECEIVED_DATE,
                        CASE WHEN TO_NUMBER(TO_CHAR(TO_DATE(d.DateRange),'D'))-1 = 0 THEN 7 ELSE TO_NUMBER(TO_CHAR(TO_DATE(d.DateRange),'D'))-1 END AS WEEK_DAY,
                        EXTRACT(day from TO_DATE(d.DateRange)) AS DAY,
                        SUM(CASE WHEN ACK.COUNTER = :PCCounter AND ACK.NATURE IN ( {0} ) THEN 1 ELSE 0 END) AS PC_REC, 
                        SUM(CASE WHEN ACK.COUNTER = :PCCounter AND ACK.NATURE IN ( {0} ) AND ACK.LETTER_DATE > P_GET_WORKING_DAY(ACK.RECEIVED_DATE,4) THEN 1 ELSE 0 END) AS PC_OD, 
                        SUM(CASE WHEN ACK.COUNTER = :PCCounter AND ACK.NATURE IN ( {0} ) AND ACK.LETTER_DATE IS NULL THEN 1 ELSE 0 END) AS PC_OS, 
                        SUM(CASE WHEN ACK.COUNTER = :KTCounter AND ACK.NATURE IN ( {0} ) THEN 1 ELSE 0 END) AS KT_REC, 
                        SUM(CASE WHEN ACK.COUNTER = :KTCounter AND ACK.NATURE IN ( {0} ) AND ACK.LETTER_DATE > P_GET_WORKING_DAY(ACK.RECEIVED_DATE,4) THEN 1 ELSE 0 END) AS KT_OD, 
                        SUM(CASE WHEN ACK.COUNTER = :KTCounter AND ACK.NATURE IN ( {0} ) AND ACK.LETTER_DATE IS NULL THEN 1 ELSE 0 END) AS KT_OS, 
                        SUM(CASE WHEN ACK.COUNTER = :WKGOCounter AND ACK.NATURE IN ( {4} ) THEN 1 ELSE 0 END) AS WKG_REC,
                        SUM(CASE WHEN ACK.COUNTER = :WKGOCounter AND ACK.NATURE IN ({4} ) AND ACK.LETTER_DATE > P_GET_WORKING_DAY(ACK.RECEIVED_DATE,4) THEN 1 ELSE 0 END) AS WKG_OD,
                        SUM(CASE WHEN ACK.COUNTER = :WKGOCounter AND ACK.NATURE IN ( {4} ) AND ACK.LETTER_DATE IS NULL THEN 1 ELSE 0 END) AS WKG_OS,
                        SUM(CASE WHEN ACK.COUNTER = :ECounter AND ACK.NATURE IN  ( {0} ) THEN 1 ELSE 0 END) AS ES_REC, 
                        SUM(CASE WHEN ACK.COUNTER = :ECounter AND ACK.NATURE IN  ( {0} ) AND ACK.LETTER_DATE > P_GET_WORKING_DAY(ACK.RECEIVED_DATE,4) THEN 1 ELSE 0 END) AS ES_OD, 
                        SUM(CASE WHEN ACK.COUNTER = :ECounter AND ACK.NATURE IN ( {0} )  AND ACK.LETTER_DATE IS NULL THEN 1 ELSE 0 END) AS ES_OS, 
                        SUM(CASE WHEN ACK.NATURE IN ( {1} ) THEN 1 ELSE 0 END) AS DL_REC, 
                        SUM(CASE WHEN ACK.NATURE IN ( {1} ) AND ACK.LETTER_DATE > P_GET_WORKING_DAY(ACK.RECEIVED_DATE,4 ) THEN 1 ELSE 0 END) AS DL_OD, 
                        SUM(CASE WHEN ACK.NATURE IN ( {1} ) AND ACK.LETTER_DATE IS NULL THEN 1 ELSE 0 END) AS DL_OS, 
                        SUM(CASE WHEN ACK.NATURE IN ( {2} ) AND ACK.ORDER_RELATED='Y' THEN 1 ELSE 0 END) AS OR_REC, 
                        SUM(CASE WHEN ACK.NATURE IN ( {2} ) AND ACK.ORDER_RELATED='Y' AND ACK.REFERRAL_DATE > P_GET_WORKING_DAY(ACK.RECEIVED_DATE,7) THEN 1 ELSE 0 END) AS OR_OD, 
                        SUM(CASE WHEN ACK.ORDER_RELATED='Y' AND ACK.REFERRAL_DATE IS NULL THEN 1 ELSE 0 END) AS OR_OS, 
                        SUM(CASE WHEN  ACK.NATURE IN ( {2} ) 
                        AND ACK.AUDIT_RELATED='Y' THEN 1 ELSE 0 END) AS ADUIT, 
                        SUM(CASE WHEN ACK.NATURE IN ( {3} )  THEN 1 ELSE 0 END) AS ICU
                        , CASE WHEN MAX(CC.COUNT) IS NULL THEN 0 ELSE MAX(CC.COUNT) END AS CR 
                        from ( select (to_date( :YearMonth ,'YYYYMM')-1 + LEVEL) as DateRange from dual 
                        where (to_date( :YearMonth ,'YYYYMM')-1+level) <= last_day(to_date( :YearMonth ,'YYYYMM')) connect by level<=31 
                        ) d 
                        LEFT OUTER join P_MW_ACK_LETTER ACK ON  TO_DATE(ACK.RECEIVED_DATE) = TO_DATE(d.DateRange) 
                        LEFT OUTER join P_S_DAILY_DIRECT_RT_OVER_CNT CC ON  TO_DATE(CC.RECEIVED_DATE) = TO_DATE(d.DateRange) 
                        GROUP BY TO_DATE(d.DateRange) 
                        ORDER BY TO_DATE(d.DateRange)", model.RecNatureList, model.DirectandReviseList, model.CRNatureList, model.ICUNatureList, model.WKGNatureList);


            OracleParameter[] oracleParameters = new OracleParameter[]
           {
                  new OracleParameter(":PCCounter",model.PCCounter)
                  ,new OracleParameter(":KTCounter",model.KTCounter)
                  ,new OracleParameter(":ECounter",model.ECounter)
                  ,new OracleParameter (":WKGOCounter",model.WKGOCounter)
                  ,new OracleParameter(":YearMonth",model.YearMonth)
           };

            return GetObjectData<Fn01LM_StatisticsIncomingModel>(queryString, oracleParameters).ToList();
        }

        public List<TypeReceivedTableModel> GetIncomingOutgoingTypeReceivedTable(GetIncomingOutgoingParameterModel model)
        {
            string queryString = string.Format(@"SELECT 
                                ACK.FORM_NO,  
                                COUNT(ACK.FORM_NO) AS total_count,  
                                SUM( CASE WHEN ACK.AUDIT_RELATED = 'Y' THEN 1 ELSE 0 end) AS audit_count  
                                FROM  P_MW_ACK_LETTER ACK  
                                WHERE TO_CHAR(ACK.RECEIVED_DATE,'YYYYMM') = :YearMonth  
                                AND ACK.FORM_NO IS NOT NULL 
                                AND ACK.NATURE IN ({0}) 
                                GROUP BY FORM_NO 
                                ORDER BY FORM_NO", model.RecNatureList);
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":YearMonth",model.YearMonth)
            };
            return GetObjectData<TypeReceivedTableModel>(queryString, oracleParameters).ToList();
        }

        public int GetAccumulatedTotalSubminssion(GetIncomingOutgoingParameterModel model)
        {
            string queryString = string.Format(@"SELECT COUNT(ACK.DSN) as count 
                            FROM P_MW_ACK_LETTER ACK
                            WHERE ACK.AUDIT_RELATED = 'Y'
                            AND ACK.NATURE IN ({0}) 
                            AND ACK.FORM_NO IN ({1})
                            AND EXTRACT(year FROM RECEIVED_DATE) = :Year", model.CRNatureList, "'MW01','MW02','MW03','MW04','MW05','MW06','MW11','MW12'");
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":Year",model.YearMonth.Substring(0,4))
            };
            return GetObjectData<int>(queryString, oracleParameters).FirstOrDefault();
        }

        public List<Fn01LM_StatisticsOutgoingModel> GetStatisticsOutgoingCalendarData(GetIncomingOutgoingParameterModel model)
        {
            string queryString = string.Format(@"SELECT d.DateRange, 
                                --TO_NUMBER(TO_CHAR(TO_DATE(d.DateRange),'D'))-1 AS WEEK_DAY,
                                CASE WHEN TO_NUMBER(TO_CHAR(TO_DATE(d.DateRange),'D'))-1 = 0 THEN 7 ELSE TO_NUMBER(TO_CHAR(TO_DATE(d.DateRange),'D'))-1 END AS WEEK_DAY,
                                EXTRACT(day from TO_DATE(d.DateRange)) AS DAY,
                                 ( SELECT COUNT(ACK.COUNTER) FROM P_MW_ACK_LETTER ACK 
                                 WHERE ACK.NATURE IN ( {0}) AND 
                                 ACK.RECEIVED_DATE BETWEEN TO_DATE( :YearStartDate ,'yyyy/mm/dd') AND CURRENT_DATE AND 
                                 ACK.COUNTER = :PCCounter AND TO_DATE(ACK.LETTER_DATE) = TO_DATE(d.DateRange) 
                                 AND LETTER_DATE BETWEEN TO_DATE(:letterDateFrom,'yyyy/mm/dd') AND TO_DATE(:letterDateTo,'yyyy/mm/dd')
                                 ) AS PC_COUNTER, 
                                 ( SELECT COUNT(ACK.COUNTER) FROM P_MW_ACK_LETTER ACK 
                                 WHERE ACK.NATURE IN ( {0} ) AND 
                                 ACK.RECEIVED_DATE BETWEEN TO_DATE( :YearStartDate ,'yyyy/mm/dd') AND CURRENT_DATE AND 
                                 ACK.COUNTER = :KTCounter AND TO_DATE(ACK.LETTER_DATE) = TO_DATE(d.DateRange) 
                                 AND LETTER_DATE BETWEEN TO_DATE(:letterDateFrom,'yyyy/mm/dd') AND TO_DATE(:letterDateTo,'yyyy/mm/dd')
                                 ) AS KT_COUNTER, 
                                 (SELECT COUNT(ACK.COUNTER) FROM P_MW_ACK_LETTER ACK
                                 WHERE ACK.NATURE IN ( {0} )  AND
                                 ACK.RECEIVED_DATE BETWEEN TO_DATE( :YearStartDate ,'yyyy/mm/dd') AND CURRENT_DATE AND 
                                 ACK.COUNTER = :WKGOCounter AND TO_DATE(ACK.LETTER_DATE) = TO_DATE(d.DateRange) 
                                 AND LETTER_DATE BETWEEN TO_DATE(:letterDateFrom,'yyyy/mm/dd') AND TO_DATE(:letterDateTo,'yyyy/mm/dd')
                                 ) AS WKGO_COUNTER,
                                 ( SELECT COUNT(ACK.COUNTER) FROM P_MW_ACK_LETTER ACK 
                                 WHERE ACK.NATURE IN ( {0} ) AND 
                                 ACK.RECEIVED_DATE BETWEEN TO_DATE( :YearStartDate ,'yyyy/mm/dd') AND CURRENT_DATE AND 
                                 ACK.COUNTER = :ECounter AND TO_DATE(ACK.LETTER_DATE) = TO_DATE(d.DateRange) 
                                 AND LETTER_DATE BETWEEN TO_DATE(:letterDateFrom,'yyyy/mm/dd') AND TO_DATE(:letterDateTo,'yyyy/mm/dd')
                                 ) AS ES_COUNTER, 
                                 (SELECT count(ACK.DSN) from P_MW_ACK_LETTER ACK WHERE TO_DATE(ACK.LETTER_DATE) = TO_DATE(d.DateRange) 
                                 AND LETTER_DATE BETWEEN TO_DATE(:letterDateFrom,'yyyy/mm/dd') AND TO_DATE(:letterDateTo,'yyyy/mm/dd')
                                 and ACK.nature IN ( {1} ) ) AS D_LET, 
                                 (SELECT count(ACK.DSN) from P_MW_ACK_LETTER ACK WHERE TO_DATE(ACK.REFERRAL_DATE) = TO_DATE(d.DateRange) 
                                 and ACK.nature IN ( {2} ) AND ACK.ORDER_RELATED = 'Y' ) AS O_REL, 
                                 (SELECT count(ACK.DSN) from P_MW_ACK_LETTER ACK WHERE TO_DATE(ACK.RECEIVED_DATE) = TO_DATE(d.DateRange) 
                                 and ACK.nature IN ( {3})) AS ICU, 
                                (SELECT CASE WHEN max(c.COUNT) IS NULL THEN 0 ELSE max(c.COUNT) END  FROM P_S_DAILY_DIRECT_RT_OVER_CNT c WHERE TO_DATE(C.RECEIVED_DATE) = TO_DATE(d.DateRange)) AS  CR 
                                 FROM 
                                 ( select (to_date( :MonthYear ,'MM YYYY')-1 + LEVEL) as DateRange from    dual 
                                 where   (to_date( :MonthYear ,'MM YYYY')-1+level) <= last_day(to_date( :MonthYear ,'MM YYYY')) 
                                 connect by level<=31 
                                 ) d ORDER BY d.DateRange", model.RecNatureList, model.DirectandReviseList, model.CRNatureList, model.ICUNatureList);
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":YearStartDate",model.YearStartDate)
                ,new OracleParameter(":letterDateFrom",model.LetterDateFrom)
                ,new OracleParameter(":letterDateTo",model.LetterDateTo)
                ,new OracleParameter(":PCCounter",model.PCCounter)
                ,new OracleParameter(":KTCounter",model.KTCounter)
                ,new OracleParameter(":WKGOCounter",model.WKGOCounter)
                ,new OracleParameter(":ECounter",model.ECounter)
                ,new OracleParameter(":MonthYear",model.MonthYear)
            };
            return GetObjectData<Fn01LM_StatisticsOutgoingModel>(queryString, oracleParameters).ToList();
        }

        public List<P_S_SYSTEM_VALUE> GetC114ValidationScheme()
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_S_SYSTEM_VALUE.Join(db.P_S_SYSTEM_TYPE, sv => sv.SYSTEM_TYPE_ID, stype => stype.UUID, (sv, stype) => new { sv = sv, stype = stype })
                                                .Where(sty => sty.stype.TYPE == "NO_OF_VALIDATE").Select(sty => sty.sv).ToList();
                //(from sv in db.P_S_SYSTEM_VALUE
                // join stype in db.P_S_SYSTEM_TYPE on sv.UUID equals stype.UUID
                // where
            }
        }

        public Fn01LM_StatisticsSDMParticularItemModel GetSDMParticularItem(SearchSubmissionReceivedModel model)
        {
            string queryString = @"SELECT 
                                 COUNT(DISTINCT ROW1) AS WindowsSubmission 
                                ,COUNT(DISTINCT ROW2) AS RenderingSubmission
                                ,COUNT(DISTINCT ROW3) AS RepairSubmission		
                                ,COUNT(DISTINCT ROW4) AS AbovegroudDrainageSubmission
                                ,COUNT(DISTINCT ROW5) AS AcSupportingFrameSubmission		
                                ,COUNT(DISTINCT ROW6) AS DryingRackSubmission
                                ,COUNT(DISTINCT ROW7) AS CanopySubmission
                                ,COUNT(DISTINCT ROW8) AS SdfSubmission
                                ,COUNT(DISTINCT ROW9) AS SignboardRelatedSubmission
                                FROM (
                                     SELECT 
                                      t0.UUID AS ROW0
                                     ,CASE WHEN t1.ITEM_NO IN ('2.8','2.9','3.6','3.7') THEN t0.UUID ELSE NULL END AS ROW1
                                     ,CASE WHEN t1.ITEM_NO = '2.34' THEN t0.UUID ELSE NULL END AS ROW2
                                     ,CASE WHEN t1.ITEM_NO IN ('1.17','2.17') THEN t0.UUID ELSE NULL END AS ROW3
                                     ,CASE WHEN t1.ITEM_NO IN ('2.30','3.23','3.24') THEN t0.UUID ELSE NULL END AS ROW4
                                     ,CASE WHEN t1.ITEM_NO IN ('1.28','1.29','3.27','3.28') THEN t0.UUID ELSE NULL END AS ROW5
                                     ,CASE WHEN t1.ITEM_NO IN ('3.29','3.30') THEN t0.UUID ELSE NULL END AS ROW6
                                     ,CASE WHEN t1.ITEM_NO IN ('1.27','3.26') THEN t0.UUID ELSE NULL END AS ROW7
                                     ,CASE WHEN t1.ITEM_NO IN ('1.41','1.42','1.43','1.44','3.39','3.40','3.41','3.42') THEN t0.UUID ELSE NULL END AS ROW8
                                     ,CASE WHEN t0.SIGNBOARD_RELATED = 'Y' THEN t0.UUID ELSE NULL END AS ROW9 
                                     FROM P_MW_ACK_LETTER t0 LEFT JOIN 
                                     (SELECT DISTINCT MW_ACK_LETTER_ID, ITEM_NO FROM P_MW_ACK_LETTER_ITEM ) t1 ON t1.MW_ACK_LETTER_ID = t0.UUID 
                                     WHERE 1=1 
                                     AND TO_CHAR(t0.RECEIVED_DATE, 'YYYYMMDD') >= TO_CHAR(:recDate1, 'YYYYMMDD') 
                                     AND TO_CHAR(t0.RECEIVED_DATE, 'YYYYMMDD') <= TO_CHAR(:recDate2, 'YYYYMMDD')
                                ) ";
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
               new OracleParameter(":recDate1",Convert.ToDateTime(model.ReceivedDateFrom))
               ,new OracleParameter(":recDate2",Convert.ToDateTime(model.ReceivedDateTo))
            };
            return GetObjectData<Fn01LM_StatisticsSDMParticularItemModel>(queryString, oracleParameters).FirstOrDefault();

        }

        public List<Fn01LM_StatisticsRectificationNote3Model> GetRectificationNote3Data()
        {
            string queryString = @"
                                SELECT to_char(extract(year from F.RECEIVED_DATE)) PeriodYear,Count(d.mw_Dsn) Count
                                FROM P_MW_RECORD D
                                inner JOIN P_MW_REFERENCE_NO REF_NO ON D.REFERENCE_NUMBER = REF_NO.UUID
                                inner JOIN P_MW_FORM F ON D.UUID = F.MW_RECORD_ID 
                                inner join P_MW_SUMMARY_MW_ITEM_CHECKLIST mws on mws.MW_RECORD_ID = d.uuid
                                where 1=1 
                                and d.STATUS_CODE in ('MW_SECOND_COMPLETE') 
                                and (mws.GROUNDS_OF_REFUSAL = 'Y' or mws.GROUNDS_OF_CONDITIONAL = 'Y')
                                and extract(year from F.RECEIVED_DATE) >= '2019'
                                group by  extract(year from F.RECEIVED_DATE)
                                ";
            return GetObjectData<Fn01LM_StatisticsRectificationNote3Model>(queryString).ToList();
        }

        public int GetTotalCounterByDate(SearchSubmissionReceivedModel model, string counter, string isBarCode, string allNatureList)
        {
            string queryString = string.Format(@"SELECT COUNT(*)
                                    FROM P_MW_ACK_LETTER
                                    WHERE COUNTER IS NOT NULL
                                    AND COUNTER = :Counter
                                    AND BARCODE = :IsBarCode
                                    AND RECEIVED_DATE is not null
                                    AND TO_CHAR(RECEIVED_DATE, 'YYYYMMDD') >= TO_CHAR(:recDate1, 'YYYYMMDD')
                                    AND TO_CHAR(RECEIVED_DATE, 'YYYYMMDD') <= TO_CHAR(:recDate2, 'YYYYMMDD')
                                    AND NATURE IN ( {0} )", allNatureList);
            OracleParameter[] oracleParameters = new OracleParameter[]
           {
               new OracleParameter(":Counter",Convert.ToInt32(counter))
               ,new OracleParameter(":IsBarCode",isBarCode)
               ,new OracleParameter(":recDate1",Convert.ToDateTime(model.ReceivedDateFrom))
               ,new OracleParameter(":recDate2",Convert.ToDateTime(model.ReceivedDateTo))
           };
            return GetObjectData<int>(queryString, oracleParameters).FirstOrDefault();
        }

        #endregion

        #region P_MW_ACK_LETTER Common function
        public P_MW_ACK_LETTER GetP_MW_ACK_LETTER(string DSN)
        {
            string RecordSql = @"SELECT ACK.*
                                FROM P_MW_ACK_LETTER ACK
                                WHERE ACK.DSN = :DSN";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":DSN", DSN)
            };

            return GetObjectData<P_MW_ACK_LETTER>(RecordSql, oracleParameters).FirstOrDefault();
        }
        #endregion

        public int UpdateFinalRecordCompletionDate(P_MW_RECORD model, EntitiesMWProcessing db)
        {
            P_MW_RECORD record = db.P_MW_RECORD.Where(w => w.UUID == model.UUID).FirstOrDefault();
            if (record != null)
            {
                record.COMPLETION_DATE = model.COMPLETION_DATE;
            }

            return db.SaveChanges();
        }
    }
}