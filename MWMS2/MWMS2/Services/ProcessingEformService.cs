using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingEformService : BaseCommonService
    {
        public ProcessingCommonService pc = new ProcessingCommonService();

        public string SEPARATOR = "/";
        public string SPACE = " ";

        public P_EFSS_FORM_MASTER getEfssRecordByDsn(string DSN)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var map = db.P_EFSS_SUBMISSION_MAP.Where(x => x.DSN == DSN).FirstOrDefault();

                //Start modify by dive 20191011
                if (map == null) { return null; }
                //End modify by dive 20191011
                var master = db.P_EFSS_FORM_MASTER.Find(map.EFSS_ID);

                return master;
            }
        }

        public P_EFSS_SUBMISSION_MAP getEfssMapByDsn(string DSN)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var map = db.P_EFSS_SUBMISSION_MAP.Where(x => x.DSN == DSN).FirstOrDefault();

                return map;
            }
        }

        public string SearchMWUR_EA_q = " SELECT * FROM ("
            + " \r\n\t SELECT master.ID AS EFSS_ID, master.SUBMISSIONNO AS EFSS_SUBMISSIONNO, master.FOURPLUSTWO AS EFSS_FOURPLUSTWO,"
            + " \r\n\t map.MW_SUBMISSION AS MAP_MW_SUBMISSION, map.DSN AS MAP_DSN,"
            + " \r\n\t CASE WHEN MW01.ID IS NOT NULL THEN 'MW01'"
            + " \r\n\t WHEN MW02.ID IS NOT NULL THEN 'MW02'"
            + " \r\n\t WHEN MW03.ID IS NOT NULL THEN 'MW03'"
            + " \r\n\t WHEN MW04.ID IS NOT NULL THEN 'MW04'"
            + " \r\n\t WHEN MW05.ID IS NOT NULL THEN 'MW05'"
            + " \r\n\t WHEN MW06.ID IS NOT NULL THEN 'MW06'"
            + " \r\n\t WHEN MW07.ID IS NOT NULL THEN 'MW07'"
            + " \r\n\t WHEN MW08.ID IS NOT NULL THEN 'MW08'"
            + " \r\n\t WHEN MW09.ID IS NOT NULL THEN 'MW09'"
            + " \r\n\t WHEN MW10.ID IS NOT NULL THEN 'MW10'"
            + " \r\n\t WHEN MW11.ID IS NOT NULL THEN 'MW11'"
            + " \r\n\t WHEN MW12.ID IS NOT NULL THEN 'MW12'"
            + " \r\n\t WHEN MW31.ID IS NOT NULL THEN 'MW31'"
            + " \r\n\t WHEN MW32.ID IS NOT NULL THEN 'MW32'"
            + " \r\n\t WHEN MW33.ID IS NOT NULL THEN 'MW33'"
            + " \r\n\t END AS FORM_CODE, master.STATUS AS EFSS_STATUS, map.STATUS AS MAP_STATUS"
            + " \r\n FROM P_EFSS_FORM_MASTER master"
            + " \r\n\t LEFT JOIN P_EFSS_SUBMISSION_MAP map ON master.ID = map.EFSS_ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW01 MW01 ON master.FORMCONTENTID = MW01.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW02 MW02 ON master.FORMCONTENTID = MW02.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW03 MW03 ON master.FORMCONTENTID = MW03.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW04 MW04 ON master.FORMCONTENTID = MW04.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW05 MW05 ON master.FORMCONTENTID = MW05.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW06 MW06 ON master.FORMCONTENTID = MW06.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW07 MW07 ON master.FORMCONTENTID = MW07.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW08 MW08 ON master.FORMCONTENTID = MW08.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW09 MW09 ON master.FORMCONTENTID = MW09.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW10 MW10 ON master.FORMCONTENTID = MW10.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW11 MW11 ON master.FORMCONTENTID = MW11.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW12 MW12 ON master.FORMCONTENTID = MW12.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW31 MW31 ON master.FORMCONTENTID = MW31.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW32 MW32 ON master.FORMCONTENTID = MW32.ID"
            + " \r\n\t LEFT JOIN P_EFMWU_TBL_MW33 MW33 ON master.FORMCONTENTID = MW33.ID"
            + " \r\n ) WHERE 1=1";


        public Fn02MWUR_EASearchModel SearchMWUR_EA(Fn02MWUR_EASearchModel model)
        {
            model.Query = SearchMWUR_EA_q;
            model.QueryWhere = SearchMWUR_EA_whereQ(model);

            model.Search();
            return model;
        }

        private string SearchMWUR_EA_whereQ(Fn02MWUR_EASearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.EfssNo))
            {
                whereQ += "\r\n\t AND upper(EFSS_SUBMISSIONNO) like :EFSS_SUBMISSIONNO ";
                model.QueryParameters.Add("EFSS_SUBMISSIONNO", "%" + model.EfssNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.DSN))
            {
                whereQ += "\r\n\t AND upper(MAP_DSN) like :MAP_DSN";
                model.QueryParameters.Add("MAP_DSN", "%" + model.DSN.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.MwNo))
            {
                whereQ += "\r\n\t AND upper(MAP_MW_SUBMISSION) like :MAP_MW_SUBMISSION";
                model.QueryParameters.Add("MAP_MW_SUBMISSION", "%" + model.MwNo.Trim().ToUpper() + "%");
            }
            if (model.Status.Equals("0")) // all
            {

            }
            else if (model.Status.Equals("1")) // submitted
            {
                whereQ += "\r\n\t AND MAP_DSN IS NOT NULL";
            }
            else if (model.Status.Equals("2")) // not yet submitted
            {
                whereQ += "\r\n\t AND MAP_DSN IS NULL";
            }
            if (model.EfssStatus.Equals("0")) // all
            {

            }
            else if (model.EfssStatus.Equals("ACK")) // ACK
            {
                whereQ += "\r\n\t AND EFSS_STATUS = 'ACK'";
            }
            else if (model.EfssStatus.Equals("R")) // not yet DR
            {
                whereQ += "\r\n\t AND EFSS_STATUS = 'R'";
            }


            return whereQ;
        }

        public P_EFSS_SUBMISSION_MAP createEfssSubmissionMap(string EFSS_ID, DisplaySubmissionObj obj, string STATUS)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_EFSS_SUBMISSION_MAP map = new P_EFSS_SUBMISSION_MAP();
                map.EFSS_ID = EFSS_ID;
                if (STATUS.Equals(ProcessingConstant.EFSS_STATUS_ACK))
                {
                    map.MW_SUBMISSION = obj.RefNo;
                    map.DSN = obj.Dsn;
                    map.STATUS = ProcessingConstant.EFSS_SUBMISSION_MAP_STATUS_ACK;
                }
                else if (STATUS.Equals(ProcessingConstant.EFSS_SUBMISSION_MAP_STATUS_DIRECT_RETURN))
                {
                    map.STATUS = ProcessingConstant.EFSS_SUBMISSION_MAP_STATUS_DIRECT_RETURN;
                }

                db.P_EFSS_SUBMISSION_MAP.Add(map);
                db.SaveChanges();

                return map;
            }
        }

        //public void create_ACK(EntitiesMWProcessing db, Fn02MWUR_MWURC_Model mwur, P_EFSS_FORM_MASTER master, P_EFSS_SUBMISSION_MAP map)
        //{
        //    //string separator = "/";
        //    //string space = " ";

        //    //// assign variables
        //    //string COUNTER = "3";
        //    //string NATURE = ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION;
        //    //string EFSS_REF_NO;
        //    //string DSN;
        //    //string MW_NO;
        //    //string FORM_NO;
        //    //DateTime COMP_DATE;
        //    //DateTime COMM_DATE;
        //    //string PREVIOUS_RELATED_MW_NO;
        //    //string MW_ITEM = "";
        //    //string ORDER_RELATED = ProcessingConstant.FLAG_Y;
        //    //string SDF_RELATED;
        //    //string SIGNBOARD_RELATED;
        //    //string SSP;
        //    //string PBP_NO;
        //    //string PRC_NO;
        //    //string ADDRESS;
        //    //string BUILDING;
        //    //string STREET;
        //    //string STREET_NO;
        //    //string FLOOR;
        //    //string UNIT;
        //    //string PAW;
        //    //string PAW_CONTACT;
        //    //string IO_MGT;
        //    //string IO_MGT_CONTACT;
        //    //string REMARK;
        //    //string LANGUAGE;
        //    //string FILE_TYPE;
        //    //string REFERRAL_DATE;
        //    //string EMAIL_OF_PBP;
        //    //string EMAIL_OF_PRC;

        //    //#region Class I MW: 01, 02, 11
        //    //if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW01))
        //    //{
        //    //    P_EFMWU_TBL_MW01 detail = db.P_EFMWU_TBL_MW01.Where(x => x.ID == master.FORMCONTENTID).FirstOrDefault();
        //    //    List<P_EFMWU_TBL_MW01_ITEM> items = db.P_EFMWU_TBL_MW01_ITEM.Where(x => x.MW1ID == detail.ID).ToList();

        //    //    EFSS_REF_NO = master.SUBMISSIONNO;
        //    //    DSN = map.DSN;
        //    //    MW_NO = map.MW_SUBMISSION;
        //    //    FORM_NO = mwur.FormNo;
        //    //    COMP_DATE;
        //    //    COMM_DATE;
        //    //    PREVIOUS_RELATED_MW_NO;


        //    //    foreach(var item in items)
        //    //    {
        //    //        MW_ITEM += item + separator;
        //    //    }

        //    //    ORDER_RELATED = ProcessingConstant.FLAG_Y;
        //    //    SDF_RELATED;
        //    //    SIGNBOARD_RELATED;
        //    //    SSP;
        //    //    PBP_NO;
        //    //    PRC_NO;
        //    //    ADDRESS = detail.FLATROOM + space 
        //    //        + detail.FLOOR + space 
        //    //        + detail.BUILDING + space 
        //    //        + detail.STREETNO + space 
        //    //        + detail.STREETROADVILLAGE + space 
        //    //        + detail.DISTRICT + space + getRegionFromOption(detail.REGINSPECOPTION.ToString());
        //    //    BUILDING = detail.BUILDING;
        //    //    STREET = detail.STREETROADVILLAGE;
        //    //    STREET_NO = detail.STREETNO;
        //    //    FLOOR = detail.FLOOR;
        //    //    UNIT = detail.FLATROOM;
        //    //    PAW;
        //    //    PAW_CONTACT;
        //    //    IO_MGT;
        //    //    IO_MGT_CONTACT;
        //    //    REMARK;
        //    //    LANGUAGE;
        //    //    FILE_TYPE;
        //    //    REFERRAL_DATE;
        //    //    EMAIL_OF_PBP = 
        //    //    EMAIL_OF_PRC;
        //    //}
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW02))
        //    //{

        //    //}
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW11))
        //    //{

        //    //}
        //    //#endregion

        //    //#region Class II MW: 03, 04, 12
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW03))
        //    //{

        //    //}
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW04))
        //    //{

        //    //}
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW12))
        //    //{

        //    //}
        //    //#endregion

        //    //#region Class III MW: 05, 32
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW05))
        //    //{

        //    //}
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW32))
        //    //{

        //    //}
        //    //#endregion

        //    //#region Chamge/Cessation of Appointment of PBP/PRC: MW07, 08, 09, 10, 11, 31
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW07))
        //    //{

        //    //}
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW08))
        //    //{

        //    //}
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW09))
        //    //{

        //    //}
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW10))
        //    //{

        //    //}
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW31))
        //    //{

        //    //}
        //    //#endregion

        //    //#region Additional Documents: MW33
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW33))
        //    //{

        //    //}
        //    //#endregion

        //    //#region Household MW Validation Scheme: MW06
        //    //else if (mwur.FormNo.Equals(ProcessingConstant.FORM_MW06))
        //    //{

        //    //}
        //    //#endregion

        //    //// save to db
        //    //P_MW_ACK_LETTER ack = new P_MW_ACK_LETTER();
        //    //ack.RECEIVED_DATE = DateTime.Now;
        //    //ack.LETTER_DATE = DateTime.Now;
        //    //ack.COUNTER = COUNTER;
        //    //ack.NATURE = NATURE
        //    //ack.EFSS_REF_NO = EFSS_REF_NO;
        //    //ack.DSN = DSN;
        //    //ack.MW_NO = MW_NO;
        //    //ack.FORM_NO = FORM_NO;
        //    //ack.COMP_DATE = COMP_DATE;
        //    //ack.COMM_DATE = COMM_DATE;
        //    //ack.PREVIOUS_RELATED_MW_NO = PREVIOUS_RELATED_MW_NO;
        //    //ack.MW_ITEM = MW_ITEM;
        //    //ack.ORDER_RELATED = ORDER_RELATED;
        //    //ack.SDF_RELATED = SDF_RELATED;
        //    //ack.SIGNBOARD_RELATED = SIGNBOARD_RELATED;
        //    //ack.SSP = SSP;
        //    //ack.PBP_NO = PBP_NO;
        //    //ack.PRC_NO = PRC_NO;
        //    //ack.ADDRESS = ADDRESS;
        //    //ack.BUILDING = BUILDING;
        //    //ack.STREET = STREET;
        //    //ack.STREET_NO = STREET_NO;
        //    //ack.FLOOR = FLOOR;
        //    //ack.UNIT = UNIT;
        //    //ack.PAW = PAW;
        //    //ack.PAW_CONTACT = PAW_CONTACT;
        //    //ack.IO_MGT = IO_MGT;
        //    //ack.IO_MGT_CONTACT = IO_MGT_CONTACT;
        //    //ack.REMARK = REMARK;
        //    //ack.LANGUAGE = LANGUAGE;
        //    //ack.FILE_TYPE = FILE_TYPE;
        //    //ack.REFERRAL_DATE = REFERRAL_DATE;
        //    //ack.EMAIL_OF_PBP = EMAIL_OF_PBP;
        //    //ack.EMAIL_OF_PRC = EMAIL_OF_PRC;

        //    //db.P_MW_ACK_LETTER.Add(ack);
        //    //db.SaveChanges();

        //}

        public Fn01LM_AckSearchModel createAckModel(string EFSS_ID, string FORM_CODE, string DSN, string MW_NO)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_EFSS_FORM_MASTER master = db.P_EFSS_FORM_MASTER.Find(EFSS_ID);

                Fn02MWUR_EAAckLetterModel ackModel = getAckLetterData(FORM_CODE, master.FORMCONTENTID);
                if (ackModel.PBP_NO != null)
                {
                    ackModel.EMAIL_OF_PBP = getEmailByCrmRegNo(ackModel.PBP_NO);
                }
                if (ackModel.PRC_NO != null)
                {
                    ackModel.EMAIL_OF_PRC = getEmailByCrmRegNo(ackModel.PRC_NO);
                }

                Fn01LM_AckSearchModel model = new Fn01LM_AckSearchModel();
                model.P_MW_ACK_LETTER = new P_MW_ACK_LETTER();

                model.P_MW_ACK_LETTER.COUNTER = "3";
                model.P_MW_ACK_LETTER.NATURE = ProcessingConstant.SUBMISSION_VALUE_NATURE_ESUBMISSION;
                model.P_MW_ACK_LETTER.RECEIVED_DATE = DateTime.Now;
                model.P_MW_ACK_LETTER.LETTER_DATE = DateTime.Now;
                model.P_MW_ACK_LETTER.DSN = DSN;
                model.P_MW_ACK_LETTER.MW_NO = MW_NO;
                if (!string.IsNullOrWhiteSpace(master.FOURPLUSTWO))
                {
                    model.P_MW_ACK_LETTER.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    model.P_MW_ACK_LETTER.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                }
                model.P_MW_ACK_LETTER.FORM_NO = FORM_CODE;
                model.P_MW_ACK_LETTER.COMP_DATE = ackModel.COMP_DATE; // 02, 04,... form
                model.P_MW_ACK_LETTER.MW_ITEM = ackModel.MW_ITEM;
                model.P_MW_ACK_LETTER.ORDER_RELATED = ProcessingConstant.FLAG_N; // default
                model.P_MW_ACK_LETTER.SSP = ProcessingConstant.FLAG_N; // default
                model.P_MW_ACK_LETTER.PBP_NO = ackModel.PBP_NO;
                model.P_MW_ACK_LETTER.EMAIL_OF_PBP = ackModel.EMAIL_OF_PBP;
                model.P_MW_ACK_LETTER.PRC_NO = ackModel.PRC_NO;
                model.P_MW_ACK_LETTER.EMAIL_OF_PRC = ackModel.EMAIL_OF_PRC;
                model.P_MW_ACK_LETTER.COMM_DATE = ackModel.COMM_DATE; // 01, 03, 05, 06 form
                model.P_MW_ACK_LETTER.ADDRESS = ackModel.ADDRESS;
                model.P_MW_ACK_LETTER.STREET = ackModel.STREET;
                model.P_MW_ACK_LETTER.STREET_NO = ackModel.STREET_NO;
                model.P_MW_ACK_LETTER.BUILDING = ackModel.BUILDING;
                model.P_MW_ACK_LETTER.FLOOR = ackModel.FLOOR;
                model.P_MW_ACK_LETTER.UNIT = ackModel.UNIT;
                model.P_MW_ACK_LETTER.PAW = ackModel.PAW;
                model.P_MW_ACK_LETTER.PAW_CONTACT = ackModel.PAW_CONTACT;
                //model.P_MW_ACK_LETTER.IO_MGT // null
                //model.P_MW_ACK_LETTER.IO_MGT_CONTACT // null
                //model.P_MW_ACK_LETTER.REMARK // null
                model.P_MW_ACK_LETTER.LANGUAGE = ProcessingConstant.LANG_ENGLISH; // default
                model.P_MW_ACK_LETTER.FILE_TYPE = ProcessingConstant.DOC_TYPE_PDF; //default
                model.P_MW_ACK_LETTER.BARCODE = ProcessingConstant.FLAG_N; // default

                return model;
            }
        }

        public Fn02MWUR_EAAckLetterModel getAckLetterData(string FORM_CODE, string EFSS_DETAIL_FORM_ID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                Fn02MWUR_EAAckLetterModel model = new Fn02MWUR_EAAckLetterModel();

                #region Class I: 01*, 02, 11
                if (FORM_CODE.Equals(ProcessingConstant.FORM_MW01))
                {
                    P_EFMWU_TBL_MW01 detail = db.P_EFMWU_TBL_MW01.Find(EFSS_DETAIL_FORM_ID);
                    List<P_EFMWU_TBL_MW01_ITEM> items = db.P_EFMWU_TBL_MW01_ITEM.Where(x => x.MW1ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    if (items != null && items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }

                    model.PBP_NO = detail.APRICERTNUM1 + model.Separator + detail.APRICERTNUM2;
                    model.PRC_NO = detail.RCCFMCRNUM1 + model.Separator + detail.RCCFMCRNUM2;

                    model.COMM_DATE = detail.COMMDATE;

                    model.ADDRESS = detail.FLATROOM + model.Space
                        + detail.FLOOR + model.Space
                        + detail.BUILDING + model.Space
                        + detail.STREETNO + model.Space
                        + detail.STREETROADVILLAGE + model.Space
                        + detail.DISTRICT + model.Space
                        + pc.getRegionFromOption(detail.REGINSPECOPTION.ToString(), ProcessingConstant.LANG_ENGLISH);

                    model.UNIT = detail.FLATROOM;
                    model.FLOOR = detail.FLOOR;
                    model.BUILDING = detail.BUILDING;
                    model.STREET_NO = detail.STREETNO;
                    model.STREET = detail.STREETROADVILLAGE;


                    model.PAW = detail.MWENGNAME1 + model.Space + detail.MWENGNAME2;
                    model.PAW_CONTACT = detail.ARRTEL;
                }
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW02))
                {
                    P_EFMWU_TBL_MW02 detail = db.P_EFMWU_TBL_MW02.Find(EFSS_DETAIL_FORM_ID);
                    List<P_EFMWU_TBL_MW02_ITEM1> items1 = db.P_EFMWU_TBL_MW02_ITEM1.Where(x => x.MW2ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    List<P_EFMWU_TBL_MW02_ITEM2> items2 = db.P_EFMWU_TBL_MW02_ITEM2.Where(x => x.MW2ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    List<P_EFMWU_TBL_MW02_ITEM3> items3 = db.P_EFMWU_TBL_MW02_ITEM3.Where(x => x.MW2ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    if (items1 != null && items1.Count() > 0)
                    {
                        foreach (var item in items1)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }
                    if (items2 != null && items2.Count() > 0)
                    {
                        foreach (var item in items2)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }
                    if (items3 != null && items3.Count() > 0)
                    {
                        foreach (var item in items3)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }
                    model.PBP_NO = detail.APCRNUM1 + model.Separator + detail.APCRNUM2;
                    model.PRC_NO = detail.RCCRNUM1 + model.Separator + detail.RCCRNUM2;
                }
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW11))
                {
                    P_EFMWU_TBL_MW11 detail = db.P_EFMWU_TBL_MW11.Find(EFSS_DETAIL_FORM_ID);
                    List<P_EFMWU_TBL_MW11_ITEM> items = db.P_EFMWU_TBL_MW11_ITEM.Where(x => x.MW11ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    //model.COMP_DATE
                    if (items != null && items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }

                    model.PBP_NO = detail.AUTHCRNUM1 + model.Separator + detail.AUTHCRNUM2;
                    //model.EMAIL_OF_PBP;
                    model.PRC_NO = detail.RCCRNUM1 + model.Separator + detail.RCCRNUM2;
                    //model.EMAIL_OF_PRC;

                    model.COMM_DATE = detail.COMMENDATE;

                    model.PAW = detail.APPENGNAME1 + model.Space + detail.APPENGNAME2;
                    //model.PAW_CONTACT;
                }
                #endregion

                #region Class II: 03*, 04, 12
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW03))
                {
                    P_EFMWU_TBL_MW03 detail = db.P_EFMWU_TBL_MW03.Find(EFSS_DETAIL_FORM_ID);
                    List<P_EFMWU_TBL_MW03_ITEM> items = db.P_EFMWU_TBL_MW03_ITEM.Where(x => x.MW3ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    //model.COMP_DATE
                    if (items != null && items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }

                    //model.PBP_NO;
                    //model.EMAIL_OF_PBP;
                    model.PRC_NO = detail.RCCRNUM1 + model.Separator + detail.RCCRNUM2;
                    //model.EMAIL_OF_PRC;

                    model.COMM_DATE = detail.COMMENDATE;

                    model.ADDRESS = detail.FLAT + model.Space
                        + detail.FLOOR + model.Space
                        + detail.BUILDING + model.Space
                        + detail.STREETNO + model.Space
                        + detail.STREETROADVILLAGE + model.Space
                        + detail.DISTRICT + model.Space
                        + pc.getRegionFromOption(detail.DISTRICTOPTION.ToString(), ProcessingConstant.LANG_ENGLISH);

                    model.STREET = detail.STREETROADVILLAGE;
                    model.STREET_NO = detail.STREETNO;
                    model.BUILDING = detail.BUILDING;
                    model.FLOOR = detail.FLOOR;
                    model.UNIT = detail.FLAT;

                    model.PAW = detail.MWENGNAME1 + model.Space + detail.MWENGNAME2;
                    model.PAW_CONTACT = detail.MWTEL;
                }
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW04))
                {
                    P_EFMWU_TBL_MW04 detail = db.P_EFMWU_TBL_MW04.Find(EFSS_DETAIL_FORM_ID);
                    List<P_EFMWU_TBL_MW04_ITEM2> items2 = db.P_EFMWU_TBL_MW04_ITEM2.Where(x => x.MW4ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    List<P_EFMWU_TBL_MW04_ITEM3> items3 = db.P_EFMWU_TBL_MW04_ITEM3.Where(x => x.MW4ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    model.COMP_DATE = detail.COMPLETDATE;
                    if (items2 != null && items2.Count() > 0)
                    {
                        foreach (var item in items2)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }
                    if (items3 != null && items3.Count() > 0)
                    {
                        foreach (var item in items3)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }

                    //model.PBP_NO;
                    //model.EMAIL_OF_PBP;
                    model.PRC_NO = detail.RCCRNUM1 + model.Separator + detail.RCCRNUM2;
                    model.EMAIL_OF_PRC = detail.EMAIL;

                    //model.COMM_DATE = detail.COMMENDATE;

                    model.PAW = detail.ARCENGNAME1 + model.Space + detail.ARCENGNAME2;
                    //model.PAW_CONTACT = detail.MWTEL;
                }
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW12))
                {
                    P_EFMWU_TBL_MW12 detail = db.P_EFMWU_TBL_MW12.Find(EFSS_DETAIL_FORM_ID);
                    List<P_EFMWU_TBL_MW12_ITEM> items = db.P_EFMWU_TBL_MW12_ITEM.Where(x => x.MW12ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    //model.COMP_DATE
                    if (items != null && items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }

                    //model.PBP_NO;
                    //model.EMAIL_OF_PBP;
                    model.PRC_NO = detail.RCCRNUM1 + model.Separator + detail.RCCRNUM2;
                    //model.EMAIL_OF_PRC;

                    model.COMM_DATE = detail.COMMENDATE;

                    model.PAW = detail.ARRENGNAME1 + model.Space + detail.ARRENGNAME2;
                    //model.PAW_CONTACT = detail.MWTEL;
                }
                #endregion

                #region Class III: 05*, 32*
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW05))
                {
                    P_EFMWU_TBL_MW05 detail = db.P_EFMWU_TBL_MW05.Find(EFSS_DETAIL_FORM_ID);
                    List<P_EFMWU_TBL_MW05_ITEM> items = db.P_EFMWU_TBL_MW05_ITEM.Where(x => x.MW5ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    //model.COMP_DATE
                    if (items != null && items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }

                    //model.PBP_NO;
                    //model.EMAIL_OF_PBP;
                    model.PRC_NO = detail.RCCRNUM1 + model.Separator + detail.RCCRNUM2;
                    //model.EMAIL_OF_PRC;

                    model.COMM_DATE = detail.COMMENDATE;

                    model.ADDRESS = detail.FLAT + model.Space
                        + detail.FLOOR + model.Space
                        + detail.BUILDING + model.Space
                        + detail.STREETNO + model.Space
                        + detail.STREETROADVILLAGE + model.Space
                        + detail.DISTRICT + model.Space
                        + pc.getRegionFromOption(detail.DISTRICTOPTION.ToString(), ProcessingConstant.LANG_ENGLISH);

                    model.STREET = detail.STREETROADVILLAGE;
                    model.STREET_NO = detail.STREETNO;
                    model.BUILDING = detail.BUILDING;
                    model.FLOOR = detail.FLOOR;
                    model.UNIT = detail.FLAT;

                    model.PAW = detail.MWENGNAME1 + model.Space + detail.MWENGNAME2;
                    model.PAW_CONTACT = detail.MWTEL;
                }
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW32))
                {
                    P_EFMWU_TBL_MW32 detail = db.P_EFMWU_TBL_MW32.Find(EFSS_DETAIL_FORM_ID);
                    List<P_EFMWU_TBL_MW32_ITEM> items = db.P_EFMWU_TBL_MW32_ITEM.Where(x => x.MW32ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    model.COMP_DATE = detail.EXPECTEDCOMPLETEDATE;
                    if (items != null && items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }

                    //model.PBP_NO;
                    //model.EMAIL_OF_PBP;
                    model.PRC_NO = detail.RCCRNUM1 + model.Separator + detail.RCCRNUM2;
                    //model.EMAIL_OF_PRC;

                    model.COMM_DATE = detail.EXPECTEDCOMMENCEDATE;

                    model.ADDRESS = detail.FLAT + model.Space
                        + detail.FLOOR + model.Space
                        + detail.BUILDING + model.Space
                        + detail.STREETNO + model.Space
                        + detail.STREETROADVILLAGE + model.Space
                        + detail.DISTRICT + model.Space
                        + pc.getRegionFromOption(detail.DISTRICTOPTION.ToString(), ProcessingConstant.LANG_ENGLISH);

                    model.STREET = detail.STREETROADVILLAGE;
                    model.STREET_NO = detail.STREETNO;
                    model.BUILDING = detail.BUILDING;
                    model.FLOOR = detail.FLOOR;
                    model.UNIT = detail.FLAT;

                    model.PAW = detail.ARRENGNAME1 + model.Space + detail.ARRENGNAME2;
                    //model.PAW_CONTACT = detail.MWTEL;
                }
                #endregion

                #region Class III: 06*
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW06))
                {
                    P_EFMWU_TBL_MW06 detail = db.P_EFMWU_TBL_MW06.Find(EFSS_DETAIL_FORM_ID);
                    List<P_EFMWU_TBL_MW06_ITEM> items = db.P_EFMWU_TBL_MW06_ITEM.Where(x => x.MW6ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    List<P_EFMWU_TBL_MW06_ITEMAS> itemsAS = db.P_EFMWU_TBL_MW06_ITEMAS.Where(x => x.MW6ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    model.COMP_DATE = detail.COMPLETEDATE;

                    if (items != null && items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }
                    if (itemsAS != null && itemsAS.Count() > 0)
                    {
                        foreach (var item in itemsAS)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }

                    model.PBP_NO = detail.APCRNUM1 + model.Separator + detail.APCRNUM2;
                    //model.EMAIL_OF_PBP;
                    model.PRC_NO = detail.ARCCRNUM1 + model.Separator + detail.ARCCRNUM2;
                    //model.EMAIL_OF_PRC;

                    model.COMM_DATE = detail.COMMENDATE;

                    model.ADDRESS = detail.FLAT + model.Space
                        + detail.FLOOR + model.Space
                        + detail.BUILDING + model.Space
                        + detail.STREETNO + model.Space
                        + detail.STREETROADVILLAGE + model.Space
                        + detail.DISTRICT + model.Space
                        + pc.getRegionFromOption(detail.DISTRICTOPTION.ToString(), ProcessingConstant.LANG_ENGLISH);

                    model.STREET = detail.STREETROADVILLAGE;
                    model.STREET_NO = detail.STREETNO;
                    model.BUILDING = detail.BUILDING;
                    model.FLOOR = detail.FLOOR;
                    model.UNIT = detail.FLAT;

                    //model.PAW = detail.MWENGNAME1 + model.Space + detail.MWENGNAME2;
                    //model.PAW_CONTACT = detail.MWTEL;
                }
                #endregion

                #region 07, 08, 09, 10, 31
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW07))
                {
                    P_EFMWU_TBL_MW07 detail = db.P_EFMWU_TBL_MW07.Find(EFSS_DETAIL_FORM_ID);

                    model.PBP_NO = detail.APRICRNUM1 + model.Separator + detail.APRICRNUM2;
                    if (getTrueFalseFromOption(detail.RCOPTION.ToString()))
                    {
                        model.PRC_NO = detail.RCCRNUM1 + model.Separator + detail.RCCRNUM2;
                        model.EMAIL_OF_PRC = detail.EMAIL;
                    }

                    model.PAW = detail.APENGNAME1 + model.Space + detail.APENGNAME2;
                    //model.PAW_CONTACT = detail.MWTEL;
                }
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW08))
                {
                    P_EFMWU_TBL_MW08 detail = db.P_EFMWU_TBL_MW08.Find(EFSS_DETAIL_FORM_ID);

                    model.PBP_NO = detail.APRICRNUM1 + model.Separator + detail.APRICRNUM2;
                    model.PAW = detail.ARRENGNAME1 + model.Space + detail.ARRENGNAME2;
                    //model.PAW_CONTACT = detail.EMAIL;
                }
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW09))
                {
                    P_EFMWU_TBL_MW09 detail = db.P_EFMWU_TBL_MW09.Find(EFSS_DETAIL_FORM_ID);

                    model.PBP_NO = detail.NOMINEECRNUM1 + model.Separator + detail.NOMINEECRNUM2;
                }
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW10))
                {
                    P_EFMWU_TBL_MW10 detail = db.P_EFMWU_TBL_MW10.Find(EFSS_DETAIL_FORM_ID);
                    List<P_EFMWU_TBL_MW10_ITEM> items = db.P_EFMWU_TBL_MW10_ITEM.Where(x => x.MW10ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    if (items != null && items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }

                    model.PBP_NO = detail.APRICRNUM1 + model.Separator + detail.APRICRNUM2;
                    //model.EMAIL_OF_PBP;
                    model.PRC_NO = detail.ACRCCRNUM1 + model.Separator + detail.ACRCCRNUM2;
                    //model.EMAIL_OF_PRC;

                    //model.PAW = detail.MWENGNAME1 + model.Space + detail.MWENGNAME2;
                    //model.PAW_CONTACT = detail.MWTEL;
                }
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW31))
                {
                    P_EFMWU_TBL_MW31 detail = db.P_EFMWU_TBL_MW31.Find(EFSS_DETAIL_FORM_ID);
                    List<P_EFMWU_TBL_MW31_ITEM> items = db.P_EFMWU_TBL_MW31_ITEM.Where(x => x.MW31ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    //model.COMP_DATE
                    if (items != null && items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            model.MW_ITEM += item.MWITEM + model.Separator;
                        }
                    }

                    model.PBP_NO = detail.ACRCCRNUM1 + model.Separator + detail.ACRCCRNUM2;
                    //model.PBP_NO;
                    model.PRC_NO = detail.ACRCCRNUM1 + model.Separator + detail.ACRCCRNUM2;
                    //model.EMAIL_OF_PRC;
                }
                #endregion

                #region 33
                else if (FORM_CODE.Equals(ProcessingConstant.FORM_MW33))
                {
                    P_EFMWU_TBL_MW33 detail = db.P_EFMWU_TBL_MW33.Find(EFSS_DETAIL_FORM_ID);


                    model.PBP_NO = detail.APCRNUM1 + model.Separator + detail.APCRNUM2;
                    //model.EMAIL_OF_PBP;
                    if (getTrueFalseFromOption(detail.PRCOPTION.ToString()))
                    {
                        model.PRC_NO = detail.APCRNUM1 + model.Separator + detail.APCRNUM2;
                        //model.EMAIL_OF_PRC;
                    }

                    //model.COMM_DATE = detail.COMMENDATE;

                    //model.ADDRESS = detail.FLAT + model.Space
                    //    + detail.FLOOR + model.Space
                    //    + detail.BUILDING + model.Space
                    //    + detail.STREETNO + model.Space
                    //    + detail.STREETROADVILLAGE + model.Space
                    //    + detail.DISTRICT + model.Space
                    //    + getRegionFromOption(detail.DISTRICTOPTION.ToString());

                    //model.STREET = detail.STREETROADVILLAGE;
                    //model.STREET_NO = detail.STREETNO;
                    //model.BUILDING = detail.BUILDING;
                    //model.FLOOR = detail.FLOOR;
                    //model.UNIT = detail.FLAT;

                    //model.PAW = detail.MWENGNAME1 + model.Space + detail.MWENGNAME2;
                    //model.PAW_CONTACT = detail.MWTEL;
                }
                #endregion

                return model;
            }
        }

        public void directReturn(string EFSS_ID, string FORM_CODE)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        P_EFSS_FORM_MASTER master = db.P_EFSS_FORM_MASTER.Find(EFSS_ID);
                        List<P_EFSS_REJECT_REASONS> reasons = db.P_EFSS_REJECT_REASONS.Where(x => x.RECVFORMID == master.ID).ToList();

                        ProcessingSystemValueService PsvService = new ProcessingSystemValueService();
                        List<P_S_SYSTEM_VALUE> irregularitiesList = PsvService.GetSystemListByType("Irregularities");


                        P_MW_DIRECT_RETURN dr = new P_MW_DIRECT_RETURN();
                        //dr.DSN
                        dr.FORM_TYPE = FORM_CODE;
                        //dr.CONTRACTOR_REG_NO = getDirectReturnRegNo(EFSS_ID, master.FORMCONTENTID);
                        dr.RECEIVED_DATE = DateTime.Now;
                        dr.HANDING_STAFF_1 = SessionUtil.LoginPost.CODE; // current logged in user
                                                                         //dr.HANDING_STAFF_2
                                                                         //dr.HANDING_STAFF_3
                        dr.LANGUAGE = ProcessingConstant.LANG_ENGLISH;
                        db.P_MW_DIRECT_RETURN.Add(dr);
                        db.SaveChanges();

                        // reject reason
                        foreach (var irregularities in irregularitiesList)
                        {
                            P_MW_DIRECT_RETURN_IRREGULARITIES irr = new P_MW_DIRECT_RETURN_IRREGULARITIES();
                            irr.MASTER_ID = dr.UUID;
                            irr.SV_IRREGULARITIES_ID = irregularities.UUID;
                            foreach (var reason in reasons)
                            {
                                if (reason.REJECTREASONCODE.Equals(irregularities.CODE))
                                {
                                    irr.IS_CHECKED = "True";
                                }
                                else
                                {
                                    irr.IS_CHECKED = "False";
                                }
                            }
                            db.P_MW_DIRECT_RETURN_IRREGULARITIES.Add(irr);
                            db.SaveChanges();

                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                    }
                }
            }
        }

        public void saveDocument(EntitiesMWProcessing db, P_EFSS_FORM_MASTER master, P_EFSS_SUBMISSION_MAP map)
        {

        }

        public bool getTrueFalseFromOption(string option)
        {
            bool flag = false;
            if (option.Equals("1")) // 1: true, 0: false
            {
                flag = true;
            }
            return flag;
        }

        public string getEmailByCrmRegNo(string RegNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var result = db.V_CRM_INFO.Where(x => x.CERTIFICATION_NO == RegNo).FirstOrDefault();
                return result != null ? result.EMAIL_ADDRESS : null;
            }
        }

        public string getClassCode(string itemNo)
        {
            itemNo = itemNo.Trim();

            if (itemNo[0] == '1')
            {
                return ProcessingConstant.DB_CLASS_I;
            }
            if (itemNo[0] == '2')
            {
                return ProcessingConstant.DB_CLASS_II;
            }
            if (itemNo[0] == '3')
            {
                return ProcessingConstant.DB_CLASS_III;
            }
            return itemNo;
        }

        public string uploadScannedDocumentRelativePath(string dsnSub)
        {
            string fileSeparator = Char.ToString(ApplicationConstant.FileSeparator);
            DateTime ct = DateTime.Now;

            string path = "";
            //path += fileSeparator + "pem_scan";
            path += fileSeparator + ct.Year.ToString();
            path += fileSeparator + ct.Month.ToString();
            path += fileSeparator + dsnSub + fileSeparator;

            return path;
        }

        public string uploadScannedDocumentFullPath(string dsnSub)
        {
            string fileSeparator = Char.ToString(ApplicationConstant.FileSeparator);
            string path = "";
            path = ApplicationConstant.PEMFilePath;
            path += "pem_scan";
            path += uploadScannedDocumentRelativePath(dsnSub);

            return path;
        }

        public string getDsnSubByDsn(string dsn, string docType)
        {
            string dsnSub = dsn;
            if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_FORM))
            {
                dsnSub += "A";
            }
            else if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_PHOTO))
            {
                dsnSub += "B";
            }
            else if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_PLAN))
            {
                dsnSub += "C";
            }
            else if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_OTHER))
            {
                dsnSub += "D";
            }
            else if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_SSP))
            {
                dsnSub += "E";
            }
            else if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_LETTER))
            {
                dsnSub += "F";
            }
            return dsnSub;
        }

        public string getFolderTypeByDocType(string docType)
        {
            string folderType = "";
            if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_FORM))
            {
                folderType = ProcessingConstant.DSN_FOLDER_TYPE_PRIVATE;
            }
            else if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_PHOTO))
            {
                folderType = ProcessingConstant.DSN_FOLDER_TYPE_PUBLIC;
            }
            else if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_PLAN))
            {
                folderType = ProcessingConstant.DSN_FOLDER_TYPE_PUBLIC;
            }
            else if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_OTHER))
            {
                folderType = ProcessingConstant.DSN_FOLDER_TYPE_PRIVATE;
            }
            else if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_SSP))
            {
                folderType = ProcessingConstant.DSN_FOLDER_TYPE_PRIVATE;
            }
            else if (docType.Equals(ProcessingConstant.DSN_DOCUMENT_TYPE_LETTER))
            {
                folderType = ProcessingConstant.DSN_FOLDER_TYPE_PRIVATE;
            }
            return folderType;
        }

        public bool isEfss(Fn01LM_AckSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_EFSS_SUBMISSION_MAP efss = db.P_EFSS_SUBMISSION_MAP.Where(x => x.DSN == model.P_MW_ACK_LETTER.DSN).FirstOrDefault();
                return efss == null ? false : true;
            }
        }
    }
}