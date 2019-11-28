using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ComSubmissionBLService
    {
        private ComSubmissionDAOService _DA;
        protected ComSubmissionDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new ComSubmissionDAOService());
            }
        }

        private P_MW_RECORD_ITEM_DAOService itemService;
        protected P_MW_RECORD_ITEM_DAOService mwRecordItemService
        {
            get { return itemService ?? (itemService = new P_MW_RECORD_ITEM_DAOService()); }
        }

        public Fn03TSK_SSModel GetSubmissionInfoByRecordID(Fn03TSK_SSModel model)
        {
            DA.GetSubmissionInfoByRecordID(model);

            model.FinalP_MW_RECORD_ITEMs = mwRecordItemService.GetFinalP_MW_RECORD_ITEMsByRefNo(model.P_MW_REFERENCE_NO.REFERENCE_NO);

            return model;
        }

        public ServiceResult CheckIsVeri(string r_uuid)
        {
            return DA.CheckIsVeri(r_uuid);
        }

        public Fn03TSK_SSModel SearchWI1Form(Fn03TSK_SSModel model)
        {
            model.Query = @"Select BLOCK_ID,UNIT_ID,NATURE,RECEIVED_DATE,MW_NO,PBP,PRC,WORK_LOCATION
                            ,COMM_DATE,COMP_DATE,PAW,PAW_CONTACT,LETTER_DATE,UMW_NOTICE_NO,BD_REF,V_SUBMISSION_CASE_NO,STATUTORY_NOTICE_NO from P_IMPORT_36_ITEM Where MW_NO = :MW_NO";
            model.QueryParameters.Add("MW_NO", model.P_MW_REFERENCE_NO.REFERENCE_NO);
            model.Search();
            return model;
        }

        public Fn03TSK_SSModel SearchModData(Fn03TSK_SSModel model)
        {
            model.Query = @"Select md.REFERENCE_NO,MD.EMAIL,MD.DSN from P_MW_MODIFICATION_RELATED_MWNO rm inner join P_MW_MODIFICATION md on rm.MODIFICATION_ID = md.UUID 
                            Where rm.MW_NO = :MW_NO";
            model.QueryParameters.Add("MW_NO", model.P_MW_REFERENCE_NO.REFERENCE_NO);
            model.Search();
            return model;

        }

        public Fn03TSK_SSModel SearchFileRefer(Fn03TSK_SSModel model)
        {
            model.Query = @"SELECT fileref_four||'/'||fileref_two AS FILE_REF,blk_id AS BLK,unit_id AS UNIT  FROM P_MW_FILEREF WHERE mw_record_id=:mwno";
            model.QueryParameters.Add("mwno", model.P_MW_REFERENCE_NO.REFERENCE_NO);
            model.Search();
            return model;
        }
    }
}