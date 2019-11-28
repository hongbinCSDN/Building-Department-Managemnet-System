using MWMS2.Areas.Signboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;
using System.Data.Entity;
using MWMS2.Constant;
using MWMS2.Services.Signborad.WorkFlow;
using MWMS2.Utility;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using MWMS2.Models;
using System.Data.Common;
using System.IO;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using MWMS2.Services.Signborad.SignboardServices;
using System.Globalization;
using System.Data.Entity.Validation;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SignboardTDLDetailDAOService : BaseCommonService
    {
        #region IssueLetter
        public IssueLetterDisplayModel ViewIssueLetter(string id)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                IssueLetterDisplayModel model = new IssueLetterDisplayModel();
                //model.SBCommonDisplayModel = new SignboardRecordCommonDisplayModel();
                model.B_SV_VALIDATION = db.B_SV_VALIDATION.Where(x => x.UUID == id)
                    .Include(x => x.B_SV_RECORD)
                    .Include(x => x.B_SV_RECORD.B_SV_RECORD_ITEM)
                    .Include(x => x.B_SV_RECORD.B_SV_APPOINTED_PROFESSIONAL)
                    .Include(x => x.B_SV_RECORD.B_SV_PERSON_CONTACT)
                    .Include(x => x.B_SV_RECORD.B_SV_PERSON_CONTACT.B_SV_ADDRESS)
                    .Include(x => x.B_SV_RECORD.B_SV_PERSON_CONTACT.B_SV_ADDRESS.B_SV_RECORD_ADDRESS_INFO)
                    .Include(x => x.B_SV_RECORD.B_SV_SIGNBOARD)
                    .Include(x => x.B_SV_RECORD.B_SV_SIGNBOARD.B_SV_PHOTO_LIBRARY)
                    .FirstOrDefault();

                model.MODIFIED_DATE = model.B_SV_VALIDATION.MODIFIED_DATE.Value.ToString("dd/MM/yy hh:mm:ss");
                model.B_SV_RECORD = model.B_SV_VALIDATION.B_SV_RECORD;

                model.SDList = db.B_SV_SCANNED_DOCUMENT.Where(x => x.RECORD_ID == model.B_SV_VALIDATION.B_SV_RECORD.REFERENCE_NO).OrderByDescending(x => x.CREATED_DATE).ToList();
                model.AP_AP = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_APPOINTED_PROFESSIONAL.Where(x => x.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_AP).FirstOrDefault();
                model.AP_RSE = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_APPOINTED_PROFESSIONAL.Where(x => x.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_RSE).FirstOrDefault();
                model.AP_RGE = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_APPOINTED_PROFESSIONAL.Where(x => x.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_RGE).FirstOrDefault();
                model.AP_PRC = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_APPOINTED_PROFESSIONAL.Where(x => x.IDENTIFY_FLAG == SignboardConstant.PRC_CODE).FirstOrDefault();

                model.SV_PL_List = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_SIGNBOARD.B_SV_PHOTO_LIBRARY.ToList();
                model.ExportLetterFlag = "N"; // default

                return model;
            }
        }

        public IssueLetterDisplayModel getLetter(IssueLetterDisplayModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var Vquery = db.B_SV_RECORD.Find(model.B_SV_VALIDATION.B_SV_RECORD.UUID);
                List<B_S_LETTER_TEMPLATE> query = new List<B_S_LETTER_TEMPLATE>();
                if(SignboardConstant.LETTER_TYPE_ADUIT_FORM_CODE.Equals(model.SelectedLetterType)/* || SignboardConstant.LETTER_TYPE_ADVISORYLETTER_CODE.Equals(model.SelectedLetterType)*/)
                {
                    query = db.B_S_LETTER_TEMPLATE.Where(x => x.LETTER_TYPE == model.SelectedLetterType && x.RESULT == "Accept").ToList();
                }
                else
                {
                    query = db.B_S_LETTER_TEMPLATE.Where(x => x.FORM_CODE == Vquery.FORM_CODE && x.RESULT == "Accept" && x.LETTER_TYPE == model.SelectedLetterType).ToList();
                }
                model.B_S_LETTER_TEMPLATE_List = query;
                return model;
            }

        }
        public void SaveIssueLetter(IssueLetterDisplayModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var query = db.B_SV_RECORD.Find(model.B_SV_VALIDATION.B_SV_RECORD.UUID);

                query.S_CHK_VS_NO = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_VS_NO;
                query.S_CHK_INSP_DATE = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_INSP_DATE;
                query.S_CHK_WORK_DATE = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_WORK_DATE;
                query.S_CHK_SIGNBOARD = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_SIGNBOARD;
                query.S_CHK_SIG = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_SIG;
                query.S_CHK_SIG_DATE = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_SIG_DATE;
                query.S_CHK_MW_ITEM_NO = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_MW_ITEM_NO;
                query.S_CHK_SUPPORT_DOC = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_SUPPORT_DOC;
                query.S_CHK_SBO_PWA_AP = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_SBO_PWA_AP;
                query.S_CHK_OTHERS = model.B_SV_VALIDATION.B_SV_RECORD.S_CHK_OTHERS;

                query.P_CHK_APP_AP_MW_ITEM = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_APP_AP_MW_ITEM;
                query.P_CHK_VAL_AP = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_VAL_AP;
                query.P_CHK_SIG_AP = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_SIG_AP;
                query.P_CHK_VAL_RSE = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_VAL_RSE;
                query.P_CHK_SIG_RSE = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_SIG_RSE;
                query.P_CHK_SIG_RSE = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_SIG_RSE;
                query.P_CHK_VAL_RI = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_VAL_RI;
                query.P_CHK_SIG_RI = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_SIG_RI;
                query.P_CHK_VAL_PRC = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_VAL_PRC;
                query.P_CHK_SIG_AS = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_SIG_AS;
                query.P_CHK_CAP_AS_MW_ITEM = model.B_SV_VALIDATION.B_SV_RECORD.P_CHK_CAP_AS_MW_ITEM;

                query.TO_OFFICER = model.B_SV_VALIDATION.B_SV_RECORD.TO_OFFICER;

                if(model.ExportLetterFlag.Equals("Y"))
                {
                    query.ACK_LETTERISS_DATE = DateTime.Now;
                }
                else
                {
                    query.ACK_LETTERISS_DATE = model.B_SV_RECORD.ACK_LETTERISS_DATE;
                }

                query.IO_ADDRESS = model.B_SV_VALIDATION.B_SV_RECORD.IO_ADDRESS;


                /* Validation Information */
                query.VALIDITY_AP = model.B_SV_RECORD.VALIDITY_AP;
                query.SIGNATURE_AP = model.B_SV_RECORD.SIGNATURE_AP;
                query.ITEM_STATED = model.B_SV_RECORD.ITEM_STATED;
                query.SIGNATURE_AS = model.B_SV_RECORD.SIGNATURE_AS;
                query.INFO_SIGNBOARD_OWNER_PROVIDED = model.B_SV_RECORD.INFO_SIGNBOARD_OWNER_PROVIDED;
                query.OTHER_IRREGULARITIES = model.B_SV_RECORD.OTHER_IRREGULARITIES;
                query.RECOMMENDATION = model.B_SV_RECORD.RECOMMENDATION;

                db.SaveChanges();

            }


        }


        public void SubmitIssueLetter(IssueLetterDisplayModel model)
        {

            WorkFlowManagementService wfs = new WorkFlowManagementService();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    wfs.goNextWorkFlow(db, SignboardConstant.WF_TYPE_VALIDATION,
                                                          model.B_SV_VALIDATION.B_SV_RECORD.UUID, null);
                    db.SaveChanges();
                    transaction.Commit();
                }
            }

        

        }
        #endregion


        #region Validation
        public ValidationDisplayModel ViewValidation(string id, ValidationDisplayModel model)
        {
            //ValidationDisplayModel model = new ValidationDisplayModel();

            model.TargetValidationUUID = id;
           // model.TaskType = SignboardConstant.WF_MAP_VALIDATION_TO;
            model.NextCheckingType =
                SignboardConstant.CHECKING_TYPE_SUBMISSION_INFO_CHECKING;
           
            model = Load(model);


            return model;

        }

        public ValidationDisplayModel Load(ValidationDisplayModel model)
        {

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                model.B_SV_VALIDATION = db.B_SV_VALIDATION.Where(x => x.UUID == model.TargetValidationUUID)
                                                .Include(x => x.B_SV_RECORD.B_SV_SIGNBOARD.B_SV_PHOTO_LIBRARY)
                                        .Include(x => x.B_SV_RECORD.B_SV_SIGNBOARD.B_SV_ADDRESS)
                                              .Include(x => x.B_SV_RECORD.B_SV_RECORD_ITEM.Select(y => y.B_SV_RECORD_ITEM_CHECKLIST.Select(z => z.B_SV_RECORD_ITEM_CHECKLIST_ITEM.Select(zz => zz.B_S_MW_ITEM_CHECKLIST_ITEM))))
                                        .Include(x => x.B_SV_RECORD.B_SV_RECORD_VALIDATION_ITEM)
                                        .Include(x => x.B_SV_RECORD_FORM_CHECKLIST)
                                        .Include(x => x.B_SV_RECORD.B_SV_APPOINTED_PROFESSIONAL)
                                        .FirstOrDefault();

                model.MODIFIED_DATE = model.B_SV_VALIDATION.MODIFIED_DATE.Value.ToString("dd/MM/yy hh:mm:ss");
                model.SubmittedDocList = db.B_SV_SCANNED_DOCUMENT.Where(x => x.RECORD_ID == model.B_SV_VALIDATION.B_SV_RECORD.REFERENCE_NO).Include(x => x.B_SV_SUBMISSION).OrderByDescending(x => x.CREATED_DATE).ToList();


                model.ValidationItemList = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_RECORD_VALIDATION_ITEM.ToList();

                model.SVRecordItemList = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_RECORD_ITEM.ToList();

                model.SVPLList = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_SIGNBOARD.B_SV_PHOTO_LIBRARY.ToList();

                model.advisoryLetterList = new List<SelectListItem>();
                LetterTemplateDAOService ssd = new LetterTemplateDAOService();
                string formCode = model.B_SV_VALIDATION.B_SV_RECORD.FORM_CODE;
                string letterType = SignboardConstant.LETTER_TYPE_ADVISORYLETTER_CODE;
                string result = null;
                List<List<object>> selectedLetterList = ssd.getLetterTemplateListWithPara(formCode, letterType, result);
                for(int i = 0; i < selectedLetterList.Count; i++)
                {
                    SelectListItem select = new SelectListItem { Text = (string)selectedLetterList[i][1], Value = (string)selectedLetterList[i][0] };
                    model.advisoryLetterList.Add(select);
                }

                using (EntitiesAuth db2 = new EntitiesAuth())
                {
                    string to = model.B_SV_VALIDATION.B_SV_RECORD.TO_USER_ID;
                    string po = model.B_SV_VALIDATION.B_SV_RECORD.PO_USER_ID;
                    string spo = model.B_SV_VALIDATION.B_SV_RECORD.SPO_USER_ID;
                    model.TO_POST = db2.SYS_POST.Where(x => x.UUID == to).FirstOrDefault().CODE;
                    model.PO_POST = db2.SYS_POST.Where(x => x.UUID == po).FirstOrDefault().CODE;
                    model.SPO_POST = db2.SYS_POST.Where(x => x.UUID == spo).FirstOrDefault().CODE;
                }
                   
                List < B_SV_SIGNBOARD_RELATION > svsr1 = db.B_SV_SIGNBOARD_RELATION.Where(x => x.SIGNBOARD_NO_1 == model.B_SV_VALIDATION.B_SV_RECORD.REFERENCE_NO).ToList();
                List<B_SV_SIGNBOARD_RELATION> svsr2 = db.B_SV_SIGNBOARD_RELATION.Where(x => x.SIGNBOARD_NO_2 == model.B_SV_VALIDATION.B_SV_RECORD.REFERENCE_NO).ToList();

                model.SignboardRelationList = new List<B_SV_SIGNBOARD_RELATION>();

                model.SignboardRelationList.AddRange(svsr1);
                foreach (var item in svsr2)
                {
                    if (!model.SignboardRelationList.Contains(item))
                    {
                        model.SignboardRelationList.Add(item);
                    }
                }

                model.SignboardRelationList.OrderBy(x => x.SIGNBOARD_NO_1);




                List<B_SV_PHOTO_LIBRARY> draftPhotoLib = SessionUtil.DraftList<B_SV_PHOTO_LIBRARY>(ApplicationConstant.DRAFT_KEY_PHOTOLIB);

                draftPhotoLib.AddRange(model.SVPLList);



                //draft
                List<B_SV_RECORD_ITEM> draftMWItem = SessionUtil.DraftList<B_SV_RECORD_ITEM>(ApplicationConstant.DRAFT_KEY_SMMMWITEM);

                draftMWItem.AddRange(model.SVRecordItemList);
                
                foreach (B_SV_RECORD_ITEM item in draftMWItem)
                {
                   
                    if (!item.B_SV_RECORD_ITEM_CHECKLIST.Any())
                    {
                         //item.B_SV_RECORD = model.B_SV_VALIDATION.B_SV_RECORD;
                        B_SV_RECORD_ITEM_CHECKLIST l = new B_SV_RECORD_ITEM_CHECKLIST();
                        item.B_SV_RECORD_ITEM_CHECKLIST.Add(l);

                        List<B_S_MW_ITEM_CHECKLIST_ITEM> d = db.B_S_MW_ITEM_CHECKLIST_ITEM.Where(x => x.MW_ITEM_NO == item.MW_ITEM_CODE).OrderBy(x => x.ORDERING).ToList()
                            .Select(o => new B_S_MW_ITEM_CHECKLIST_ITEM() { UUID = o.UUID, CHECKLIST_NO = o.CHECKLIST_NO, DESCRIPTION = o.DESCRIPTION, CHECKLIST_TYPE = o.CHECKLIST_TYPE, ORDERING = o.ORDERING, }).ToList();
                        foreach (B_S_MW_ITEM_CHECKLIST_ITEM itemDesc in d)
                        {
                            B_SV_RECORD_ITEM_CHECKLIST_ITEM x = new B_SV_RECORD_ITEM_CHECKLIST_ITEM();
                            x.B_S_MW_ITEM_CHECKLIST_ITEM = itemDesc;
                            l.B_SV_RECORD_ITEM_CHECKLIST_ITEM.Add(x);
                        }

                    }
                    else
                    {
                        List<B_S_MW_ITEM_CHECKLIST_ITEM> d = db.B_S_MW_ITEM_CHECKLIST_ITEM.Where(x => x.MW_ITEM_NO == item.MW_ITEM_CODE && (x.CHECKLIST_TYPE == "HEADER_A" || x.CHECKLIST_TYPE== "HEADER_B")).OrderBy(x => x.ORDERING).ToList()
                              .Select(o => new B_S_MW_ITEM_CHECKLIST_ITEM() { UUID = o.UUID, CHECKLIST_NO = o.CHECKLIST_NO, DESCRIPTION = o.DESCRIPTION, CHECKLIST_TYPE = o.CHECKLIST_TYPE, ORDERING = o.ORDERING, }).ToList();
                        foreach (B_S_MW_ITEM_CHECKLIST_ITEM itemDesc in d)
                        {
                            B_SV_RECORD_ITEM_CHECKLIST_ITEM x = new B_SV_RECORD_ITEM_CHECKLIST_ITEM();
                            x.B_S_MW_ITEM_CHECKLIST_ITEM = itemDesc;
                            item.B_SV_RECORD_ITEM_CHECKLIST.ElementAt(0).B_SV_RECORD_ITEM_CHECKLIST_ITEM.Add(x);
                        }

                        item.B_SV_RECORD_ITEM_CHECKLIST.ElementAt(0).B_SV_RECORD_ITEM_CHECKLIST_ITEM = item.B_SV_RECORD_ITEM_CHECKLIST.ElementAt(0).B_SV_RECORD_ITEM_CHECKLIST_ITEM.OrderBy(x => x.B_S_MW_ITEM_CHECKLIST_ITEM.ORDERING).OrderByDescending(x => x.B_S_MW_ITEM_CHECKLIST_ITEM.ORDERING.HasValue).ToList();
                    }
                }


                //Dictionary<string, List<B_SV_RECORD_ITEM_CHECKLIST_ITEM>> DraftsvRecordChecklistItemList
                //     = SessionUtil.DraftList<Dictionary<string, List<B_SV_RECORD_ITEM_CHECKLIST_ITEM>>>(ApplicationConstant.DRAFT_KEY_SMMMWITEMDETAIL);


                model.AP_AP = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_APPOINTED_PROFESSIONAL.Where(x => x.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_AP).FirstOrDefault();
                model.AP_RSE = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_APPOINTED_PROFESSIONAL.Where(x => x.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_RSE).FirstOrDefault();
                model.AP_RGE = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_APPOINTED_PROFESSIONAL.Where(x => x.IDENTIFY_FLAG == SignboardConstant.PBP_CODE_RGE).FirstOrDefault();
                model.AP_PRC = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_APPOINTED_PROFESSIONAL.Where(x => x.IDENTIFY_FLAG == SignboardConstant.PRC_CODE).FirstOrDefault();



                model.B_SV_RECORD_FORM_CHECKLIST = model.B_SV_VALIDATION.B_SV_RECORD_FORM_CHECKLIST.FirstOrDefault();

                if (model.AP_AP.CERTIFICATION_NO == null)
                {
                    model.PBP_AP_RESULT = SignboardConstant.NA;
                }
                else if (model.B_SV_VALIDATION.B_SV_RECORD_FORM_CHECKLIST.Any())
                {
                    if (model.B_SV_VALIDATION.B_SV_RECORD_FORM_CHECKLIST.FirstOrDefault().PBP_AP_RESULT == "Y")
                        model.PBP_AP_RESULT = SignboardConstant.OK;
                    else
                        model.PBP_AP_RESULT = SignboardConstant.NOT_OK;
                }
                else
                {
                    model.PBP_AP_RESULT = SignboardConstant.NOT_OK;
                }

                if (model.AP_RSE.CERTIFICATION_NO == null)
                {
                    model.PBP_RSE_RESULT = SignboardConstant.NA;
                }
                else if (model.B_SV_VALIDATION.B_SV_RECORD_FORM_CHECKLIST.Any())
                {
                    if (model.B_SV_VALIDATION.B_SV_RECORD_FORM_CHECKLIST.FirstOrDefault().PBP_RSE_RESULT == "Y")
                        model.PBP_RSE_RESULT = SignboardConstant.OK;
                    else
                        model.PBP_RSE_RESULT = SignboardConstant.NOT_OK;

                }
                else
                {
                    model.PBP_RSE_RESULT = SignboardConstant.NOT_OK;
                }



                if (model.AP_RGE == null || model.AP_RGE.CERTIFICATION_NO == null)
                {
                    model.PBP_RGE_RESULT = SignboardConstant.NA;
                }
                else if (model.B_SV_VALIDATION.B_SV_RECORD_FORM_CHECKLIST.Any())
                {
                    if (model.B_SV_VALIDATION.B_SV_RECORD_FORM_CHECKLIST.FirstOrDefault().PBP_RGE_RESULT == "Y")
                        model.PBP_RGE_RESULT = SignboardConstant.OK;
                    else
                        model.PBP_RGE_RESULT = SignboardConstant.NOT_OK;

                }
                else
                {
                    model.PBP_RGE_RESULT = SignboardConstant.NOT_OK;
                }

                if (model.AP_PRC.CERTIFICATION_NO == null)
                {
                    model.PRC_AS_RESULT = model.PRC_RESULT = SignboardConstant.NA;
                }
                else if (model.B_SV_VALIDATION.B_SV_RECORD_FORM_CHECKLIST.Any())
                {

                    if (model.B_SV_VALIDATION.B_SV_RECORD_FORM_CHECKLIST.FirstOrDefault().PRC_AS_RESULT == "Y")
                        model.PRC_AS_RESULT = SignboardConstant.OK;
                    else
                        model.PRC_AS_RESULT = SignboardConstant.NOT_OK;


                    if (model.B_SV_VALIDATION.B_SV_RECORD_FORM_CHECKLIST.FirstOrDefault().PRC_RESULT == "Y")
                        model.PRC_RESULT = SignboardConstant.OK;
                    else
                        model.PRC_RESULT = SignboardConstant.NOT_OK;

                }
                else
                {
                    model.PRC_RESULT = model.PRC_AS_RESULT = SignboardConstant.NOT_OK;
                }
                if(model.B_SV_RECORD_FORM_CHECKLIST != null)
                     model.PBP_PRC_RESULT_TO = model.B_SV_RECORD_FORM_CHECKLIST.PBP_PRC_RESULT_TO;

                model.b_S_MW_ITEM_CHECKLIST_ITEMsList = new Dictionary<string, List<B_S_MW_ITEM_CHECKLIST_ITEM>>();

                //if(model.SVRecordItemList.Count != 0)
                //     model.SelectedValidationMWItem = model.SVRecordItemList[0].MW_ITEM_CODE;

                if (model.SVRecordItemList.Count != 0)
                    model.SelectedValidationMWItem = model.SVRecordItemList[0].UUID;

                foreach (var item in model.SVRecordItemList)
                {
                    var q = db.B_S_MW_ITEM_CHECKLIST_ITEM.Where(x => x.MW_ITEM_NO == item.MW_ITEM_CODE).OrderBy(x => x.ORDERING);

                    model.b_S_MW_ITEM_CHECKLIST_ITEMsList.Add(item.UUID, q.ToList());
                    //model.b_S_MW_ITEM_CHECKLIST_ITEMsList.Add(item.MW_ITEM_CODE, q.ToList());
                }

                model.EditMode = SignboardConstant.VIEW_MODE;
                // set EditMode as "edit"
                var wfInfo = db.B_WF_INFO.Where(x => x.RECORD_ID == model.B_SV_VALIDATION.SV_RECORD_ID).FirstOrDefault();
                if (SignboardConstant.WF_STATUS_OPEN.Equals(wfInfo.CURRENT_STATUS))
                {
                    WorkFlowDAOService wf = new WorkFlowDAOService();
                    var wf_task_users = wf.getWFTaskUserStatusOpen(SignboardConstant.WF_TYPE_VALIDATION, model.B_SV_VALIDATION.B_SV_RECORD.UUID, model.TaskType);
                    if (wf_task_users != null && wf_task_users.Count() > 0)
                    {
                        for (int i = 0; i < wf_task_users.Count(); i++)
                        {
                            if (SessionUtil.LoginPost.UUID.Equals(wf_task_users[i][0]))
                            {
                                model.EditMode = SignboardConstant.EDIT_MODE;
                            }
                        }
                    }
                }

            }

            



            return model;
        }


        public B_SV_RECORD_ITEM_CHECKLIST loadSvRecordItemChecklist(EntitiesSignboard db, B_SV_RECORD_ITEM svRecordItem, B_SV_VALIDATION svValidation)
        {
            B_SV_RECORD_ITEM_CHECKLIST svRecordItemChecklist = null;
            List<B_SV_RECORD_ITEM_CHECKLIST> svRecordItemChecklists2 = null;
            
            svRecordItemChecklists2 = svValidation.B_SV_RECORD_ITEM_CHECKLIST.Where(x=>x.SV_RECORD_ITEM_ID ==svRecordItem.UUID).ToList();



            /**
             * Create record item checklist
             */
            if (svRecordItemChecklists2 == null ||
                    svRecordItemChecklists2.Count() == 0)
            {
                svRecordItemChecklist = new B_SV_RECORD_ITEM_CHECKLIST();
                svRecordItemChecklist.SV_RECORD_ITEM_ID = svRecordItem.UUID;
                svRecordItemChecklist.SV_VALIDATION_ID = (svValidation.UUID);
               // db.B_SV_RECORD_ITEM_CHECKLIST.Add(svRecordItemChecklist);
               // db.SaveChanges();
               // svRecordItemChecklist.RESULT = SignboardConstant.DB_CHECKED;
               // svRecordItemChecklist.RESULT_TO = SignboardConstant.DB_CHECKED;
            }
            else
            {
                svRecordItemChecklist = svRecordItemChecklists2[0];
            }
               

            return svRecordItemChecklist;
        }
        private B_SV_RECORD_FORM_CHECKLIST loadSvRecordFormChecklist(
          EntitiesSignboard db,  B_SV_VALIDATION svValidation)
        {
            //    String taskType = form.getTaskType();


            List<B_SV_RECORD_FORM_CHECKLIST> svRecordFormChecklists =
                db.B_SV_RECORD_FORM_CHECKLIST.Where(x => x.SV_VALIDATION_ID == svValidation.UUID).ToList();


            B_SV_RECORD_FORM_CHECKLIST svRecordFormChecklist = null;
            if (svRecordFormChecklists != null && svRecordFormChecklists.Count() > 0)
                svRecordFormChecklist = svRecordFormChecklists[0];
            else
            {
                svRecordFormChecklist = new B_SV_RECORD_FORM_CHECKLIST();
            }
            return svRecordFormChecklist;
        }
        public ServiceResult post(ValidationDisplayModel model)
        {
            string direction = null;
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {

                    B_SV_VALIDATION svValidation = db.B_SV_VALIDATION.Where(x => x.UUID == model.B_SV_VALIDATION.UUID)
                                .Include(x => x.B_SV_RECORD.B_SV_SIGNBOARD.B_SV_PHOTO_LIBRARY)
                                        .Include(x => x.B_SV_RECORD.B_SV_SIGNBOARD.B_SV_ADDRESS)
                                              .Include(x => x.B_SV_RECORD.B_SV_RECORD_ITEM.Select(y => y.B_SV_RECORD_ITEM_CHECKLIST.Select(z => z.B_SV_RECORD_ITEM_CHECKLIST_ITEM.Select(zz => zz.B_S_MW_ITEM_CHECKLIST_ITEM))))
                                        .Include(x => x.B_SV_RECORD.B_SV_RECORD_VALIDATION_ITEM)
                                        .Include(x => x.B_SV_RECORD_FORM_CHECKLIST)
                                        .Include(x => x.B_SV_RECORD.B_SV_APPOINTED_PROFESSIONAL)
                               .FirstOrDefault();
                    B_SV_RECORD svRecord = svValidation.B_SV_RECORD;
                    List<B_SV_RECORD_ITEM> svRecordItems = svValidation.B_SV_RECORD.B_SV_RECORD_ITEM.ToList();

                    B_SV_RECORD_FORM_CHECKLIST svRecordFormChecklist = null;

                    List<B_SV_RECORD_ITEM_CHECKLIST> svRecordItemChecklists =
                                                new List<B_SV_RECORD_ITEM_CHECKLIST>();

                    List<B_SV_RECORD_ITEM_CHECKLIST_ITEM> svRecordItemChecklistItems =
                                                    new List<B_SV_RECORD_ITEM_CHECKLIST_ITEM>();

                    List<B_SV_PHOTO_LIBRARY> PLList = svValidation.B_SV_RECORD.B_SV_SIGNBOARD.B_SV_PHOTO_LIBRARY.ToList();
                    List<B_SV_PHOTO_LIBRARY> draftLibList = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_PHOTOLIB] as List<B_SV_PHOTO_LIBRARY>;

                    int dummy;
                    var newPLList = draftLibList.Where(x => int.TryParse(x.UUID, out dummy));
                    foreach (var item in newPLList)
                    {
                        item.UUID = null;
                        item.SV_SIGNBOARD_ID = svValidation.B_SV_RECORD.B_SV_SIGNBOARD.UUID;
                        db.B_SV_PHOTO_LIBRARY.Add(item);
                        db.SaveChanges();
                    }
                    var editPLList = draftLibList.Where(x => !int.TryParse(x.UUID, out dummy));

                    foreach (var item in PLList)
                    {
                        foreach (var x in editPLList)
                        {
                            if (item.UUID == x.UUID)
                            {
                                item.URL = x.URL;
                                item.DESCRIPTION = x.DESCRIPTION;
                                db.SaveChanges();
                            }
                        }

                    }

                    // update BCIS code & ref no.
                    B_SV_ADDRESS B_SV_ADDRESS = db.B_SV_ADDRESS.Find(model.B_SV_VALIDATION.B_SV_RECORD.B_SV_SIGNBOARD.B_SV_ADDRESS.UUID);
                    B_SV_ADDRESS.BCIS_BLOCK_ID = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_SIGNBOARD.B_SV_ADDRESS.BCIS_BLOCK_ID;
                    B_SV_ADDRESS.FILE_REFERENCE_NO = model.B_SV_VALIDATION.B_SV_RECORD.B_SV_SIGNBOARD.B_SV_ADDRESS.FILE_REFERENCE_NO;
                    db.SaveChanges();

                    #region 
                    if (model.NewRelatedSignboard != null)
                    {
                        foreach (var item in model.NewRelatedSignboard)
                        {
                            B_SV_SIGNBOARD_RELATION Nsr = new B_SV_SIGNBOARD_RELATION();

                            Nsr.SIGNBOARD_NO_2 = svRecord.REFERENCE_NO;
                            Nsr.SIGNBOARD_NO_1 = item;

                            db.B_SV_SIGNBOARD_RELATION.Add(Nsr);
                        }
                        db.SaveChanges();
                    }
                    if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_SBRELATION))
                    {
                        List<string> delRSList = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_SBRELATION] as List<string>;
                        if (delRSList.Count != 0)
                        {
                            foreach (var item in delRSList)
                                db.B_SV_SIGNBOARD_RELATION.RemoveRange(db.B_SV_SIGNBOARD_RELATION.Where(x => x.UUID == item));
                        }
                        db.SaveChanges();
                    }
    
                    #endregion



                    string wfStatus = svRecord.WF_STATUS;
                    string saveMode =  model.SaveMode;
                    if (wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_END))
                        return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };

                    if (wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_TO) ||
                                    wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_PO))
                    {
                        int recordItemFormsSize = 0;


                        // if (model.SVRecordItemList != null)
                        //    recordItemFormsSize = model.SVRecordItemList.Count();
                        List<B_SV_RECORD_ITEM> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_SMMMWITEM] as List<B_SV_RECORD_ITEM>;
                        if (v.Count() != 0)
                            recordItemFormsSize = v.Count();


                        for (int a = 0; a < recordItemFormsSize; a++)
                        {
                            var recordItemForm = v[a];
                            // var recordItemForm = model.SVRecordItemList[a];
                            B_SV_RECORD_ITEM svRecordItem = null;

                            if (recordItemForm.MW_ITEM_CODE == null)
                            {
                                continue;
                            }




                            /**
                             * Create record item
                             */
                            int dummy2;
                            if (string.IsNullOrWhiteSpace(recordItemForm.UUID) || int.TryParse(recordItemForm.UUID, out dummy2))
                            {
                                svRecordItem = new B_SV_RECORD_ITEM();

                                svRecordItem.SV_RECORD_ID = svRecord.UUID;
                                svRecordItem.ORDERING = ((long)a);
                                svRecordItems.Add(svRecordItem);
                            }
                            else
                            {
                                svRecordItem = db.B_SV_RECORD_ITEM.Find(recordItemForm.UUID);
                            }

                            svRecordItem.MW_ITEM_CODE = recordItemForm.MW_ITEM_CODE;


                            switch (recordItemForm.MW_ITEM_CODE.Substring(0, 1))
                            {
                                case "1":
                                    svRecordItem.CLASS_CODE = SignboardConstant.CLASS_I;
                                    break;
                                case "2":
                                    svRecordItem.CLASS_CODE = SignboardConstant.CLASS_II;
                                    break;
                                case "3":
                                    svRecordItem.CLASS_CODE = SignboardConstant.CLASS_III;
                                    break;

                            }
                            //svRecordItem.CLASS_CODE = recordItemForm.CLASS_CODE;
                            svRecordItem.LOCATION_DESCRIPTION = recordItemForm.LOCATION_DESCRIPTION;
                            svRecordItem.REVISION = recordItemForm.REVISION;


                        }
                        /**
                          * Save record item checklist
                         */
                        if (wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_TO) ||
                       wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_PO))
                        {
                            foreach (var item in svRecordItems)
                            {
                                if (item.UUID != null)
                                    db.Entry(item).State = EntityState.Modified;
                                else
                                    db.B_SV_RECORD_ITEM.Add(item);

                            }
                            db.SaveChanges();
                        }
             
                        int svRecordItemSize = svRecordItems.Count();
                        for (int a = 0; a < svRecordItemSize; a++)
                        {
                            B_SV_RECORD_ITEM svRecordItem = svRecordItems[a];

                            //   ValidationTaskForm.RecordItemCheckingForm
                            //recordItemCheckingForm = form.getRecordItemCheckingForms().get(a);

                            //!!!!!B_SV_RECORD_ITEM_CHECKLIST svRecordItemChecklist
                            //              = loadSvRecordItemChecklist(db, svRecordItem, svValidation);

                            string TempCurrentMWItem = "";
                            bool NextMWItem = false;
                            bool isNew = false;
                            string TempsvricUUID = "";
                            List<B_SV_RECORD_ITEM_CHECKLIST> ItemChecklistQ = new List<B_SV_RECORD_ITEM_CHECKLIST>();

                            if (wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_TO))
                            {

                                if(model.TargetMWItem !=null)
                                foreach (var itemDic in model.TargetMWItem)
                                {
                                    if (TempCurrentMWItem != itemDic["TargetSVRecordItemUUID"])
                                    {
                                        NextMWItem = true;
                                        TempCurrentMWItem = itemDic["TargetSVRecordItemUUID"];
                                        ItemChecklistQ = db.B_SV_RECORD_ITEM_CHECKLIST.Where(x => x.SV_RECORD_ITEM_ID == TempCurrentMWItem).ToList();
                                        if (ItemChecklistQ.Count() != 0)
                                        {
                                            TempsvricUUID = ItemChecklistQ.FirstOrDefault().UUID;

                                            foreach (var item in model.TargetMWItemResult)
                                            {
                                                if (ItemChecklistQ.FirstOrDefault().SV_RECORD_ITEM_ID == item.Key)
                                                {

                                                    ItemChecklistQ.FirstOrDefault().RESULT_TO = item.Value;
                                                }
                                            }
                                        }
                                        isNew = ItemChecklistQ.Count() == 0 ? true : false;
                                    }


                                    if (isNew == true)
                                    {

                                        if (NextMWItem == true)
                                        {
                                            int dummy3;
                                            B_SV_RECORD_ITEM_CHECKLIST svric = new B_SV_RECORD_ITEM_CHECKLIST();
                                           
                                            if (int.TryParse(itemDic["TargetSVRecordItemUUID"], out dummy3))
                                                svric.SV_RECORD_ITEM_ID = svRecordItem.UUID;
                                            else
                                                svric.SV_RECORD_ITEM_ID = itemDic["TargetSVRecordItemUUID"];



                                            svric.SV_VALIDATION_ID = model.B_SV_VALIDATION.UUID;
                                            //svric.RESULT = "Y";
                                            foreach (var item in model.TargetMWItemResult)
                                            {
                                                if (svric.SV_RECORD_ITEM_ID == item.Key)
                                                {

                                                    svric.RESULT_TO = item.Value;
                                                }
                                            }
                                            // svric.RESULT_TO = "Y";


                                            db.B_SV_RECORD_ITEM_CHECKLIST.Add(svric);
                                            db.SaveChanges();
                                            TempsvricUUID = svric.UUID;
                                            NextMWItem = false;
                                        }
                                        B_SV_RECORD_ITEM_CHECKLIST_ITEM svrici = new B_SV_RECORD_ITEM_CHECKLIST_ITEM();
                                        svrici.S_MW_ITEM_CHECKLIST_ITEM_ID = itemDic["TagertSMWUUID"];
                                        svrici.SV_RECORD_ITEM_CHECKLIST_ID = TempsvricUUID;
                                        svrici.REMARKS = itemDic["TagertMWItemRemarks"];
                                        svrici.ITEM_DETAILS = itemDic["TagertMWItemItemDetails"];

                                        db.B_SV_RECORD_ITEM_CHECKLIST_ITEM.Add(svrici);
                                        db.SaveChanges();
                                    }
                                    else
                                    {

                                        if (NextMWItem == true)
                                        {
                                            db.B_SV_RECORD_ITEM_CHECKLIST_ITEM.RemoveRange(db.B_SV_RECORD_ITEM_CHECKLIST_ITEM.Where(x => x.SV_RECORD_ITEM_CHECKLIST_ID == TempsvricUUID));
                                            NextMWItem = false;
                                        }

                                        B_SV_RECORD_ITEM_CHECKLIST_ITEM svrici = new B_SV_RECORD_ITEM_CHECKLIST_ITEM();
                                        svrici.S_MW_ITEM_CHECKLIST_ITEM_ID = itemDic["TagertSMWUUID"];
                                        svrici.SV_RECORD_ITEM_CHECKLIST_ID = TempsvricUUID;
                                        svrici.REMARKS = itemDic["TagertMWItemRemarks"];
                                        svrici.ITEM_DETAILS = itemDic["TagertMWItemItemDetails"];

                                        db.B_SV_RECORD_ITEM_CHECKLIST_ITEM.Add(svrici);
                                        db.SaveChanges();


                                    }                                                                        
                                }                 

                                SignboardSDDaoService ss = new SignboardSDDaoService();
                                ss.postSubmittedDocs(model.SubmittedDocListFolderType);
                            }
                            else if (wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_PO))
                            {
                                foreach (var itemDic in model.TargetMWItem)
                                {
                                if (TempCurrentMWItem != itemDic["TargetSVRecordItemUUID"])
                                {
                                    NextMWItem = true;
                                    TempCurrentMWItem = itemDic["TargetSVRecordItemUUID"];
                                    ItemChecklistQ = db.B_SV_RECORD_ITEM_CHECKLIST.Where(x => x.SV_RECORD_ITEM_ID == TempCurrentMWItem).ToList();
                                    if (ItemChecklistQ.Count() != 0)
                                    {
                                        TempsvricUUID = ItemChecklistQ.FirstOrDefault().UUID;

                                        foreach (var item in model.TargetMWItemPOResult)
                                        {
                                            if (ItemChecklistQ.FirstOrDefault().SV_RECORD_ITEM_ID == item.Key)
                                            {

                                                ItemChecklistQ.FirstOrDefault().RESULT = item.Value;
                                            }
                                        }
                                    }
                                   
                                }

                                    if (NextMWItem == true)
                                    {
                                        db.B_SV_RECORD_ITEM_CHECKLIST_ITEM.RemoveRange(db.B_SV_RECORD_ITEM_CHECKLIST_ITEM.Where(x => x.SV_RECORD_ITEM_CHECKLIST_ID == TempsvricUUID));
                                        NextMWItem = false;
                                    }

                                    B_SV_RECORD_ITEM_CHECKLIST_ITEM svrici = new B_SV_RECORD_ITEM_CHECKLIST_ITEM();
                                    svrici.S_MW_ITEM_CHECKLIST_ITEM_ID = itemDic["TagertSMWUUID"];
                                    svrici.SV_RECORD_ITEM_CHECKLIST_ID = TempsvricUUID;
                                    svrici.REMARKS = itemDic["TagertMWItemRemarks"];
                                    svrici.ITEM_DETAILS = itemDic["TagertMWItemItemDetails"];

                                    db.B_SV_RECORD_ITEM_CHECKLIST_ITEM.Add(svrici);
                                    db.SaveChanges();

                                }
                            }
                        }

        

                        /**
                  * Save record form checklist
                  */

                        svRecordFormChecklist = loadSvRecordFormChecklist(db,
                                          svValidation);
                        if (string.IsNullOrWhiteSpace(svRecordFormChecklist.UUID))
                            svRecordFormChecklist.SV_VALIDATION_ID = svValidation.UUID;

                        svRecordFormChecklist.PBP_AP_RESULT = model.PBP_AP_RESULT == SignboardConstant.NA ? null : model.PBP_AP_RESULT;
                        svRecordFormChecklist.PBP_RSE_RESULT = model.PBP_RSE_RESULT == SignboardConstant.NA ? null : model.PBP_RSE_RESULT;
                        svRecordFormChecklist.PBP_RGE_RESULT = model.PBP_RGE_RESULT == SignboardConstant.NA ? null : model.PBP_RGE_RESULT;

                        svRecordFormChecklist.PRC_RESULT = model.PRC_RESULT == SignboardConstant.NA ? null : model.PRC_RESULT;
                        svRecordFormChecklist.PRC_AS_RESULT = model.PRC_AS_RESULT == SignboardConstant.NA ? null : model.PRC_AS_RESULT;

                        svRecordFormChecklist.PBP_AP_VALID_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PBP_AP_VALID_RMK;
                        svRecordFormChecklist.PBP_AP_SIGNATURE_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PBP_AP_SIGNATURE_RMK;
                        svRecordFormChecklist.PBP_AP_SIGNATURE_DATE_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PBP_AP_SIGNATURE_DATE_RMK;

                        svRecordFormChecklist.PBP_RSE_VALID_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PBP_RSE_VALID_RMK;
                        svRecordFormChecklist.PBP_RSE_SIGNATURE_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PBP_RSE_SIGNATURE_RMK;
                        svRecordFormChecklist.PBP_RSE_SIGNATURE_DATE_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PBP_RSE_SIGNATURE_DATE_RMK;

                        svRecordFormChecklist.PBP_RGE_VALID_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PBP_RGE_VALID_RMK;
                        svRecordFormChecklist.PBP_RGE_SIGNATURE_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PBP_RGE_SIGNATURE_RMK;
                        svRecordFormChecklist.PBP_RGE_SIGNATURE_DATE_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PBP_RGE_SIGNATURE_DATE_RMK;



                        svRecordFormChecklist.PRC_VALID_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PRC_VALID_RMK;
                        svRecordFormChecklist.PRC_CAP_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PRC_CAP_RMK;
                        svRecordFormChecklist.PRC_AS_VALID_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PRC_AS_VALID_RMK;
                        svRecordFormChecklist.PRC_AS_SIGNATURE_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PRC_AS_SIGNATURE_RMK;
                        svRecordFormChecklist.PRC_SIGNATURE_DATE_RMK = model.B_SV_RECORD_FORM_CHECKLIST.PRC_SIGNATURE_DATE_RMK;

                        if (wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_TO))
                            svRecordFormChecklist.PBP_PRC_RESULT_TO = model.PBP_PRC_RESULT_TO;
                        else
                            svRecordFormChecklist.PBP_PRC_RESULT = model.PBP_PRC_RESULT_TO;
                    }



                    if (wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_TO) ||
                              wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_PO))
                    {
                        //foreach (var item in svRecordItems)
                        //{
                        //    if (item.UUID != null)
                        //        db.Entry(item).State = EntityState.Modified;
                        //    else
                        //        db.B_SV_RECORD_ITEM.Add(item);

                        //}
                        db.B_SV_RECORD_FORM_CHECKLIST.AddOrUpdate(svRecordFormChecklist);
                        //for (SvRecordItemChecklist svRecordItemChecklist : svRecordItemChecklists)
                        //    svRecordItemChecklistDAO.saveOrMerge(svRecordItemChecklist,
                        //        getLoginName(request), DateUtil.getNow());
                        //for (SvRecordItemChecklistItem svRecordItemChecklistItem : svRecordItemChecklistItems)
                        //    svRecordItemChecklistItemDAO.saveOrMerge(svRecordItemChecklistItem,
                        //        getLoginName(request), DateUtil.getNow());
                    }


                    svValidation.VALIDATION_RESULT = model.B_SV_VALIDATION.VALIDATION_RESULT;
                    svValidation.REMARKS = model.B_SV_VALIDATION.REMARKS;
                    svValidation.LETTER_REASON = model.B_SV_VALIDATION.LETTER_REASON;
                    svValidation.LETTER_REMARKS = model.B_SV_VALIDATION.LETTER_REMARKS;
                    svValidation.RECORD_ITEM_RESULT_TO = model.B_SV_VALIDATION.RECORD_ITEM_RESULT_TO;
                    svValidation.RECORD_ITEM_RESULT = model.B_SV_VALIDATION.RECORD_ITEM_RESULT;
                    svValidation.SITE_AUDIT_RESULT = model.TargetMWItemSAResult;
                    svValidation.STRUCT_CALC_CHECK = model.TargetMWItemSCCResult;
                    db.Entry(svValidation).State = EntityState.Modified;
                    db.Entry(svRecord).State = EntityState.Modified;


                    db.SaveChanges();


                    #region WF

                    if (saveMode.Equals(ApplicationConstant.SUBMIT_MODE))
                    {

                //        if (!StringUtil.containsStr(new String[]{
                //    ApplicationConstants.VALIDATION_RESULT_ACKNOWLEDGED,
                //    ApplicationConstants.VALIDATION_RESULT_REFUSED,
                //    ApplicationConstants.VALIDATION_RESULT_CONDITIONAL,
                //}, form.getRecommendResult()))
                //            return;

                        if (wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_TO))
                        {
                            svRecord.WF_STATUS = SignboardConstant.WF_MAP_VALIDATION_PO;
                        }

                        direction = SignboardConstant.WF_GO_NEXT;

                    }
                    else if (saveMode.Equals(ApplicationConstant.PASS_MODE))
                    {
                
                        if (wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_PO))
                        {
                            svRecord.WF_STATUS =  SignboardConstant.WF_MAP_VALIDATION_SPO;
                        }
                
                        direction = SignboardConstant.WF_GO_SPO;
                
                    }
                   else if (saveMode.Equals(ApplicationConstant.ROLLBACK_MODE))
                   {
                
                       if (wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_SPO))
                       {
                           svRecord.WF_STATUS = SignboardConstant.WF_MAP_VALIDATION_PO;
                       }
                       else if (wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_PO))
                       {
                           svRecord.WF_STATUS  =  SignboardConstant.WF_MAP_VALIDATION_TO;
                       }

                       direction = SignboardConstant.WF_GO_BACK;
                  }

                    /**
                     * Commit the changes
                     */
                //    trx = hbmSession.beginTransaction();

                //    postSubmittedDocs(hbmSession, form, wfStatus);

                    String nextStage = "";
                    if (saveMode.Equals(ApplicationConstant.SUBMIT_MODE) ||
                        saveMode.Equals(ApplicationConstant.PASS_MODE) ||
                        saveMode.Equals(ApplicationConstant.ROLLBACK_MODE)
                        )
                    {
                        WorkFlowManagementService wfs = new WorkFlowManagementService();
                           nextStage = wfs.goNextWorkFlow(db,
                                SignboardConstant.WF_TYPE_VALIDATION, svRecord.UUID,
                                 direction);
                        db.SaveChanges();
                        svRecord.WF_STATUS = nextStage;

                    }


                    if (nextStage.Equals(SignboardConstant.WF_MAP_AUDIT_TO))
                    {
                        B_SV_AUDIT_RECORD svAuditRecord = new B_SV_AUDIT_RECORD();
                        svAuditRecord.SV_RECORD_ID = svRecord.UUID;
                        svAuditRecord.WF_STATUS = SignboardConstant.WF_MAP_AUDIT_TO;

                        db.B_SV_AUDIT_RECORD.Add(svAuditRecord);
                      
                    }




                    if (nextStage.Equals(SignboardConstant.WF_MAP_AUDIT_TO) ||
                            nextStage.Equals(SignboardConstant.WF_MAP_VALIDATION_END))
                    {
                        if (svValidation.SPO_ENDORSEMENT_DATE == null && wfStatus.Equals(SignboardConstant.WF_MAP_VALIDATION_SPO))
                        {
                           
                            svValidation.SPO_ENDORSEMENT_DATE = DateTime.Now;
                            svValidation.ENDORSED_BY = SessionUtil.LoginPost.BD_PORTAL_LOGIN;
                        }


                        svValidation.VALIDATION_STATUS = 
                            ApplicationConstant.VALIDATION_STATUS_CLOSE;
                        svRecord.WF_STATUS = SignboardConstant.WF_MAP_VALIDATION_END;

                        if ("A".Equals(svValidation.VALIDATION_RESULT))
                        {
                            svValidation.SIGNBOARD_EXPIRY_DATE = svRecord.VALIDATION_EXPIRY_DATE ;
                        }
                        else
                        {
                            svValidation.SIGNBOARD_EXPIRY_DATE = null;
                        }

                        //if (ApplicationConstant.VALIDATION_RESULT_ACKNOWLEDGED.Equals(
                        //            svValidation.VALIDATION_RESULT) ||
                        //    ApplicationConstant.VALIDATION_RESULT_CONDITIONAL.Equals(
                        //                   svValidation.VALIDATION_RESULT)
                        //)
                        //{
                        //    if (ApplicationConstant.FORM_CODE_COMPLETE.Equals(svRecord.FORM_CODE) ||
                        //       ApplicationConstant.FORM_CODE_VALIDATION.Equals(svRecord.FORM_CODE))
                        //    {
                        //      ///  updateToRRM = true;
                        //    }
                        //}


                    }



                    db.SaveChanges();
                    #endregion

                    





















                    tran.Commit();
                }

            }

            #endregion
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }


        public void AjaxSavePhotoLib( string desc, string url, string uuid)
        {
            List<B_SV_PHOTO_LIBRARY> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_PHOTOLIB] as List<B_SV_PHOTO_LIBRARY>;
            foreach (var item in v)
            {
                if (item.UUID == uuid)
                {
                   
                    item.DESCRIPTION = desc;
                    item.URL = url;
                }
            }
        }

        public void AjaxAddPhotoLib(string desc, string url)
        {
            List<B_SV_PHOTO_LIBRARY> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_PHOTOLIB] as List<B_SV_PHOTO_LIBRARY>;
            B_SV_PHOTO_LIBRARY draft = new B_SV_PHOTO_LIBRARY();
            draft.UUID = SessionUtil.DRAFT_NEXT_ID;
            draft.URL = url;
            draft.DESCRIPTION = desc;

            v.Add(draft);
        }


        public void AjaxSaveMWItem(string mwitem, string desc, string rev, string uuid)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    B_SV_RECORD_ITEM record = db.B_SV_RECORD_ITEM.Where(x => x.UUID == uuid).FirstOrDefault();
                    if (record != null)
                    {
                        record.MW_ITEM_CODE = mwitem;
                        record.LOCATION_DESCRIPTION = desc;
                        record.REVISION = rev;
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
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                    }
                }
            }

            List<B_SV_RECORD_ITEM> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_SMMMWITEM] as List<B_SV_RECORD_ITEM>;
            foreach (var item in v)
            {
                if (item.UUID == uuid)
                {
                    item.MW_ITEM_CODE = mwitem;
                    item.LOCATION_DESCRIPTION = desc;
                    item.REVISION = rev;
                }
            }
            
        }
        public void AjaxAddMWItem(string mwitem,string desc, string rev)
        {
             List<B_SV_RECORD_ITEM> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_SMMMWITEM] as List<B_SV_RECORD_ITEM>;


            B_SV_RECORD_ITEM newMWItem = new B_SV_RECORD_ITEM();

            newMWItem.UUID = SessionUtil.DRAFT_NEXT_ID;
            newMWItem.MW_ITEM_CODE = mwitem;
            newMWItem.LOCATION_DESCRIPTION = desc;
            newMWItem.REVISION = rev;
         
          
            B_SV_RECORD_ITEM_CHECKLIST l = new B_SV_RECORD_ITEM_CHECKLIST();
             newMWItem.B_SV_RECORD_ITEM_CHECKLIST.Add(l);
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                List<B_S_MW_ITEM_CHECKLIST_ITEM> d = db.B_S_MW_ITEM_CHECKLIST_ITEM.Where(x => x.MW_ITEM_NO == mwitem).OrderBy(x => x.ORDERING).ToList()
                     .Select(o => new B_S_MW_ITEM_CHECKLIST_ITEM() { UUID = o.UUID, CHECKLIST_NO = o.CHECKLIST_NO, DESCRIPTION = o.DESCRIPTION, CHECKLIST_TYPE = o.CHECKLIST_TYPE, ORDERING = o.ORDERING, }).ToList();
                foreach (B_S_MW_ITEM_CHECKLIST_ITEM itemDesc in d)
                {
                    B_SV_RECORD_ITEM_CHECKLIST_ITEM x = new B_SV_RECORD_ITEM_CHECKLIST_ITEM();
                    x.B_S_MW_ITEM_CHECKLIST_ITEM = itemDesc;
                    l.B_SV_RECORD_ITEM_CHECKLIST_ITEM.Add(x);
                }
            }
         

           
            v.Add(newMWItem);


         }


        public void AjaxdeleteSignboardRelation(string uuid)
        {
            List<string> v = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_SBRELATION);

            v.Add(uuid);

        }
        public dynamic MWItemSearch(ValidationDisplayModel model)
        {
                //model.Query = "Select * from B_SV_RECORD_ITEM";
                //model.QueryWhere = " Where SV_RECORD_ID = :SV_RECORD_ID ";
                //model.QueryParameters.Add("SV_RECORD_ID", model.B_SV_VALIDATION.SV_RECORD_ID);
                //model.Search();
                //return model;
           
                var result = SessionUtil.DraftList<B_SV_RECORD_ITEM>(ApplicationConstant.DRAFT_KEY_SMMMWITEM).Select(x => new { x.UUID, x.MW_ITEM_CODE, x.LOCATION_DESCRIPTION, x.REVISION, sItem = x.B_SV_RECORD_ITEM_CHECKLIST, STRUCT_CALC_CHECK = x.B_SV_RECORD == null ? "" : x.B_SV_RECORD.B_SV_VALIDATION.FirstOrDefault().STRUCT_CALC_CHECK, SITE_AUDIT_RESULT = x.B_SV_RECORD == null ? "" : x.B_SV_RECORD.B_SV_VALIDATION.FirstOrDefault().SITE_AUDIT_RESULT, x.ORDERING }).OrderBy(x => x.ORDERING);
                return result;
        
            //            return SessionUtil.DraftList<B_SV_RECORD_ITEM>(ApplicationConstant.DRAFT_KEY_SMMMWITEM).Select(x=>new {x.UUID,  x.MW_ITEM_CODE,  x.LOCATION_DESCRIPTION,x.REVISION ,sItem = x.B_SV_RECORD_ITEM_CHECKLIST,  x.B_SV_RECORD.B_SV_VALIDATION.FirstOrDefault().SITE_AUDIT_RESULT ,x.ORDERING  }).OrderBy(x=>x.ORDERING); 
            // List<B_SV_RECORD_ITEM> draftMWItem = SessionUtil.DraftList<B_SV_RECORD_ITEM>(ApplicationConstant.DRAFT_KEY_SMMMWITEM);

        }

        public dynamic PhotoLibSearch()
        {
            return SessionUtil.DraftList<B_SV_PHOTO_LIBRARY>(ApplicationConstant.DRAFT_KEY_PHOTOLIB).Select(x => new { x.UUID, x.URL, x.DESCRIPTION});

        }

        public ValidationDisplayModel loadAddress(string uuid) // B_SV_ADDRESS: UUID
        {
            ValidationDisplayModel model = new ValidationDisplayModel();
            using(EntitiesSignboard db = new EntitiesSignboard())
            {
                model.B_SV_ADDRESS = db.B_SV_ADDRESS.Find(uuid);

                return model.B_SV_ADDRESS != null ? model : null;
            }
        }

        public FileStreamResult exportValidationData(string uuid) // B_SV_VALIDATION: UUID
        {
            List<string> headerList = new List<string>()
            {
                "Submission No.", "Form Code", "Received Date", "Expirty Date","Validation Result", "Remarks", "SPO Endorsement Date",
                "Letter Reason", "Letter Remarks", "Signboard Address", "Location of Signboard","Depscription", "Facade", "Type",
                "Bottom fixing at Floor", "Top fixing at Floor", "Display Area", "Projection", "Heigh of Signboard", "Clearance above ground",
                "LED/TV", "Building Portion", "RVD No.", "RV Block ID", "BCIS Block ID", "BCIS District", "BD ref. (4+2)",
                "Owner Chinese Name", "Owner English Name", "Owner Address", "Owner Email", "Owner Contact No", "Owner Fax NO", "PAW Chinese Name",
                "PAW English Name", "PAW Address", "PAW Email", "PAW Contact No", "PAW Fax NO", "IO Chinese Name", "IO English Name", "IO Address",
                "IO Email", "IO Contact No", "IO Fax NO", "IO PBP Name", "IO PBP Contact No", "IO PRC Name", "IO PRC Contact No", "AP Certification No",
                "AP Chinese Name", "AP English Name", "AP EN_ADDRESS_LINE1", "AP EN_ADDRESS_LINE2", "AP EN_ADDRESS_LINE3", "AP EN_ADDRESS_LINE4",
                "AP EN_ADDRESS_LINE5", "AP CN_ADDRESS_LINE1", "AP CN_ADDRESS_LINE2", "AP CN_ADDRESS_LINE3", "AP CN_ADDRESS_LINE4", "AP CN_ADDRESS_LINE5",
                "AP Contact No", "AP Fax No", "RSE Certification No", "RSE Chinese Name", "RSE English Name", "RSE EN_ADDRESS_LINE1", "RSE EN_ADDRESS_LINE2",
                "RSE EN_ADDRESS_LINE3", "RSE EN_ADDRESS_LINE4", "RSE EN_ADDRESS_LINE5", "RSE CN_ADDRESS_LINE1", "RSE CN_ADDRESS_LINE2", "RSE CN_ADDRESS_LINE3",
                "RSE CN_ADDRESS_LINE4", "RSE CN_ADDRESS_LINE5", "RSE Contact No", "RSE Fax No", "RGE Certification No", "RGE Chinese Name", "RGE English Name",
                "RGE EN_ADDRESS_LINE1" , "RGE EN_ADDRESS_LINE2", "RGE EN_ADDRESS_LINE3", "RGE EN_ADDRESS_LINE4", "RGE EN_ADDRESS_LINE5", "RGE CN_ADDRESS_LINE1",
                "RGE CN_ADDRESS_LINE2", "RGE CN_ADDRESS_LINE3", "RGE CN_ADDRESS_LINE4", "RGE CN_ADDRESS_LINE5", "RGE Contact No", "RGE Fax No",
                "PRC Certification No", "PRC Chinese Name", "PRC English Name", "PRC EN_ADDRESS_LINE1", "PRC EN_ADDRESS_LINE2", "PRC EN_ADDRESS_LINE3",
                "PRC EN_ADDRESS_LINE4", "PRC EN_ADDRESS_LINE5", "PRC CN_ADDRESS_LINE1", "PRC CN_ADDRESS_LINE2", "PRC CN_ADDRESS_LINE3", "PRC CN_ADDRESS_LINE4",
                "PRC CN_ADDRESS_LINE5", "PRC Contact No", "PRC Fax No", "PRC AS Chinese Name", "PRC AS English Name"
            };
            List<List<object>> data = new List<List<object>>();
            data = getExportValidationData(uuid);
            return exportCSVFile("ValidationDataExport", headerList, data);
        }
        public FileStreamResult exportValidationDataExcel(string uuid) // B_SV_VALIDATION: UUID
        {
            List<string> headerList = new List<string>()
            {
                "Submission No.", "Form Code", "Received Date", "Expirty Date","Validation Result", "Remarks", "SPO Endorsement Date",
                "Letter Reason", "Letter Remarks", "Signboard Address", "Location of Signboard","Depscription", "Facade", "Type",
                "Bottom fixing at Floor", "Top fixing at Floor", "Display Area", "Projection", "Heigh of Signboard", "Clearance above ground",
                "LED/TV", "Building Portion", "RVD No.", "RV Block ID", "BCIS Block ID", "BCIS District", "BD ref. (4+2)",
                "Owner Chinese Name", "Owner English Name", "Owner Address", "Owner Email", "Owner Contact No", "Owner Fax NO", "PAW Chinese Name",
                "PAW English Name", "PAW Address", "PAW Email", "PAW Contact No", "PAW Fax NO", "IO Chinese Name", "IO English Name", "IO Address",
                "IO Email", "IO Contact No", "IO Fax NO", "IO PBP Name", "IO PBP Contact No", "IO PRC Name", "IO PRC Contact No", "AP Certification No",
                "AP Chinese Name", "AP English Name", "AP EN_ADDRESS_LINE1", "AP EN_ADDRESS_LINE2", "AP EN_ADDRESS_LINE3", "AP EN_ADDRESS_LINE4",
                "AP EN_ADDRESS_LINE5", "AP CN_ADDRESS_LINE1", "AP CN_ADDRESS_LINE2", "AP CN_ADDRESS_LINE3", "AP CN_ADDRESS_LINE4", "AP CN_ADDRESS_LINE5",
                "AP Contact No", "AP Fax No", "RSE Certification No", "RSE Chinese Name", "RSE English Name", "RSE EN_ADDRESS_LINE1", "RSE EN_ADDRESS_LINE2",
                "RSE EN_ADDRESS_LINE3", "RSE EN_ADDRESS_LINE4", "RSE EN_ADDRESS_LINE5", "RSE CN_ADDRESS_LINE1", "RSE CN_ADDRESS_LINE2", "RSE CN_ADDRESS_LINE3",
                "RSE CN_ADDRESS_LINE4", "RSE CN_ADDRESS_LINE5", "RSE Contact No", "RSE Fax No", "RGE Certification No", "RGE Chinese Name", "RGE English Name",
                "RGE EN_ADDRESS_LINE1" , "RGE EN_ADDRESS_LINE2", "RGE EN_ADDRESS_LINE3", "RGE EN_ADDRESS_LINE4", "RGE EN_ADDRESS_LINE5", "RGE CN_ADDRESS_LINE1",
                "RGE CN_ADDRESS_LINE2", "RGE CN_ADDRESS_LINE3", "RGE CN_ADDRESS_LINE4", "RGE CN_ADDRESS_LINE5", "RGE Contact No", "RGE Fax No",
                "PRC Certification No", "PRC Chinese Name", "PRC English Name", "PRC EN_ADDRESS_LINE1", "PRC EN_ADDRESS_LINE2", "PRC EN_ADDRESS_LINE3",
                "PRC EN_ADDRESS_LINE4", "PRC EN_ADDRESS_LINE5", "PRC CN_ADDRESS_LINE1", "PRC CN_ADDRESS_LINE2", "PRC CN_ADDRESS_LINE3", "PRC CN_ADDRESS_LINE4",
                "PRC CN_ADDRESS_LINE5", "PRC Contact No", "PRC Fax No", "PRC AS Chinese Name", "PRC AS English Name"
            };
            List<List<object>> data = new List<List<object>>();
            data = getExportValidationData(uuid);
            return exportExcelFile("ValidationDataExport", headerList, data);
        }

        public List<List<object>> getExportValidationData(string id)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = "select svr.reference_no, svr.form_code, svr.received_date, svv.signboard_expiry_date, "
                + " decode(svv.validation_result, 'A', :validation_result_accept, 'R', :validation_result_refuse, 'C', :validation_result_conditional_accept, svv.validation_result) as validation_result, "
                + " svv.remarks, "
                + " svv.spo_endorsement_date, svv.letter_reason, svv.letter_remarks, sba.full_address, "

                + " svs.location_of_signboard, svs.description, "
                + " svs.facade, svs.type, svs.btm_floor, svs.top_floor, svs.a_m2, svs.p_m, svs.h_m, svs.h2_m, svs.led, svs.building_portion, "
                + " svs.rvd_no, sba.rv_block_id, sba.bcis_block_id, sba.bcis_district, sba.file_reference_no, "

                + " owner.name_chinese as ownerCnaame, "
                + " owner.name_english as ownerEname, ownera.full_address as owneraddress, owner.email as owneremail, owner.contact_no as ownercontact, "
                + " owner.fax_no as ownerfaxno, paw.name_chinese as pawCnaame, paw.name_english as pawEname, pawa.full_address as pawaddress, "
                + " paw.email as pawemail,paw.contact_no as pawcontact, paw.fax_no as pawfaxno, io.name_chinese as ioCnaame, io.name_english as ioEname, "
                + " ioa.full_address as ioaddress, io.email as ioemail, io.contact_no as iocontact, io.fax_no as iofaxno, io.pbp_name, io.pbp_contact_no, "
                + " io.prc_name, io.prc_contact_no, ap.certification_no as apcert, ap.chinese_name as apCname, ap.english_name as apEname, "

                + " add_ap.EN_ADDRESS_LINE1 AS AP_EN_ADD_1, add_ap.EN_ADDRESS_LINE2 AS AP_EN_ADD_2, add_ap.EN_ADDRESS_LINE3 AS AP_EN_ADD_3, add_ap.EN_ADDRESS_LINE4 as AP_EN_ADD_4, add_ap.EN_ADDRESS_LINE5 AS AP_EN_ADD_5, "
                + " add_ap.CN_ADDRESS_LINE1 AS AP_CN_ADD_1, add_ap.CN_ADDRESS_LINE2 AS AP_CN_ADD_2, add_ap.CN_ADDRESS_LINE3 AS AP_CN_ADD_3, add_ap.CN_ADDRESS_LINE4 AS AP_CN_ADD_4, add_ap.CN_ADDRESS_LINE5 AS AP_CN_ADD_5, "

                + " ap.contact_no as apcontact, ap.fax_no as apfax, "

                + " rse.certification_no as rsecert , rse.chinese_name as rseCname,rse.english_name as rseEname, "

                + " add_rse.EN_ADDRESS_LINE1 AS RSE_EN_ADD_1, add_rse.EN_ADDRESS_LINE2 AS RSE_EN_ADD_2, add_rse.EN_ADDRESS_LINE3 AS RSE_EN_ADD_3, add_rse.EN_ADDRESS_LINE4 as RSE_EN_ADD_4, add_rse.EN_ADDRESS_LINE5 AS RSE_EN_ADD_5, "
                + " add_rse.CN_ADDRESS_LINE1 AS RSE_CN_ADD_1, add_rse.CN_ADDRESS_LINE2 AS RSE_CN_ADD_2, add_rse.CN_ADDRESS_LINE3 AS RSE_CN_ADD_3, add_rse.CN_ADDRESS_LINE4 AS RSE_CN_ADD_4, add_rse.CN_ADDRESS_LINE5 AS RSE_CN_ADD_5, "

                + " rse.contact_no as rsecontact, rse.fax_no as rsefax, "


                + " rge.certification_no as rgecert, rge.chinese_name as rgeCname, rge.english_name as rgeEname, "

                + " add_rge.EN_ADDRESS_LINE1 AS RGE_EN_ADD_1, add_rge.EN_ADDRESS_LINE2 AS RGE_EN_ADD_2, add_rge.EN_ADDRESS_LINE3 AS RGE_EN_ADD_3, add_rge.EN_ADDRESS_LINE4 as RGE_EN_ADD_4, add_rge.EN_ADDRESS_LINE5 AS RGE_EN_ADD_5, "
                + " add_rge.CN_ADDRESS_LINE1 AS RGE_CN_ADD_1, add_rge.CN_ADDRESS_LINE2 AS RGE_CN_ADD_2, add_rge.CN_ADDRESS_LINE3 AS RGE_CN_ADD_3, add_rge.CN_ADDRESS_LINE4 AS RGE_CN_ADD_4, add_rge.CN_ADDRESS_LINE5 AS RGE_CN_ADD_5, "

                + " rge.contact_no as rgecontact, rge.fax_no as rgefax, "

                + " prc.certification_no as prccert, prc.chinese_name as prcCname, prc.english_name as prcEname, "

                + " add_prc.EN_ADDRESS_LINE1 AS PRC_EN_ADD_1, add_prc.EN_ADDRESS_LINE2 AS PRC_EN_ADD_2, add_prc.EN_ADDRESS_LINE3 AS PRC_EN_ADD_3, add_prc.EN_ADDRESS_LINE4 as PRC_EN_ADD_4, add_prc.EN_ADDRESS_LINE5 AS PRC_EN_ADD_5, "
                + " add_prc.CN_ADDRESS_LINE1 AS PRC_CN_ADD_1, add_prc.CN_ADDRESS_LINE2 AS PRC_CN_ADD_2, add_prc.CN_ADDRESS_LINE3 AS PRC_CN_ADD_3, add_prc.CN_ADDRESS_LINE4 AS PRC_CN_ADD_4, add_prc.CN_ADDRESS_LINE5 AS PRC_CN_ADD_5, "

                + " prc.contact_no as prccontact, prc.fax_no as prcfax, prc.as_chinese_name, prc.as_english_name "

                + " from b_sv_record svr "
                + " LEFT JOIN b_sv_validation svv ON svr.uuid = svv.sv_record_id "
                + " LEFT JOIN b_sv_signboard svs ON svs.uuid = svr.sv_signboard_id "
                + " LEFT JOIN b_sv_address sba ON sba.uuid = svs.location_address_id "
                + " LEFT JOIN b_sv_person_contact owner ON owner.uuid = svs.owner_id "
                + " LEFT JOIN b_sv_address ownera ON ownera.uuid = owner.sv_address_id "
                + " LEFT JOIN b_sv_person_contact paw ON paw.uuid = svr.paw_id "
                + " LEFT JOIN b_sv_address pawa ON pawa.uuid = paw.sv_address_id "
                + " LEFT JOIN b_sv_person_contact io ON io.uuid = svr.oi_id "
                + " LEFT JOIN b_sv_address ioa ON ioa.uuid = io.sv_address_id "
                + " LEFT JOIN b_sv_appointed_professional ap ON ap.sv_record_id = svr.uuid "
                + " LEFT JOIN b_sv_appointed_professional rse ON rse.sv_record_id = svr.uuid "
                + " LEFT JOIN b_sv_appointed_professional rge ON rge.sv_record_id = svr.uuid "
                + " LEFT JOIN b_sv_appointed_professional prc ON prc.sv_record_id = svr.uuid "
                + " LEFT JOIN B_CRM_PBP_PRC add_ap ON add_ap.CERTIFICATION_NO = ap.CERTIFICATION_NO "
                + " LEFT JOIN B_CRM_PBP_PRC add_rse ON add_rse.CERTIFICATION_NO = rse.CERTIFICATION_NO "
                + " LEFT JOIN B_CRM_PBP_PRC add_rge ON add_rge.CERTIFICATION_NO = rge.CERTIFICATION_NO "
                + " LEFT JOIN B_CRM_PBP_PRC add_prc ON add_prc.CERTIFICATION_NO = prc.CERTIFICATION_NO "

                + " where svv.uuid in ( : uuid ) "
                + " and ap.identify_flag =:ap "
                + " and rse.identify_flag =:rse "
                + " and rge.identify_flag =:rge "
                + " and prc.identify_flag =:prc "
                + " order by svr.reference_no ";

            queryParameters.Add("validation_result_accept", SignboardConstant.RECOMMEND_ACK_STR);
            queryParameters.Add("validation_result_refuse", SignboardConstant.RECOMMEND_REF_STR);
            queryParameters.Add("validation_result_conditional_accept",SignboardConstant.RECOMMEND_COND_STR);
            queryParameters.Add("uuid", id);
            queryParameters.Add("ap", SignboardConstant.PBP_CODE_AP);
            queryParameters.Add("rse", SignboardConstant.PBP_CODE_RSE);
            queryParameters.Add("rge", SignboardConstant.PBP_CODE_RGE);
            queryParameters.Add("prc", SignboardConstant.PRC_CODE);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryString, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }

        public B_S_LETTER_TEMPLATE getLetterTemplate(string uuid, string letterUuid)
        {
            List<List<object>> selectedLetterList = new List<List<object>>();
            List<object> letterTemplate = new List<object>();
            B_S_LETTER_TEMPLATE query = new B_S_LETTER_TEMPLATE();
            if (SignboardConstant.FORM_CODE_AUDIT_FORM.Equals(letterUuid))
            {
                string formCode = "";
                string letterType = SignboardConstant.LETTER_TYPE_ADUIT_FORM_CODE;
                string result = SignboardConstant.RECOMMENDATION_ACCEPT;
                LetterTemplateDAOService ss = new LetterTemplateDAOService();
                selectedLetterList = ss.getLetterTemplateListWithPara(formCode, letterType, result);

                if(selectedLetterList != null && selectedLetterList.Count() > 0)
                {
                    letterTemplate = selectedLetterList.FirstOrDefault();
                    letterUuid = (string) letterTemplate[0];
                }
            }
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                query = db.B_S_LETTER_TEMPLATE.Find(letterUuid);
            }

            return query;
        }

        public DataEntryPrintModel GetPrintModel(string id)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                SvRecordDAOService svRecordDAOService = new SvRecordDAOService();
                SvAppointedProfessionalDAOService svAppointedProfessionalDAOService = new SvAppointedProfessionalDAOService();

                //B_SV_RECORD svRecord = svRecordDAOService.getSVRecordBySvSubmissionUUID(id);
                B_SV_RECORD svRecord = db.B_SV_RECORD.Find(id);

                B_SV_SIGNBOARD svSignboard = db.B_SV_SIGNBOARD.Where
                        (o => o.UUID == svRecord.SV_SIGNBOARD_ID).Include(o => o.B_SV_PERSON_CONTACT)
                        .Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                B_SV_PERSON_CONTACT owner = db.B_SV_PERSON_CONTACT.Where
                    (o => o.UUID == svSignboard.OWNER_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                B_SV_PERSON_CONTACT paw = db.B_SV_PERSON_CONTACT.Where
                    (o => o.UUID == svRecord.PAW_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                B_SV_PERSON_CONTACT oi = db.B_SV_PERSON_CONTACT.Where
                    (o => o.UUID == svRecord.OI_ID).Include(o => o.B_SV_ADDRESS).FirstOrDefault();

                B_SV_ADDRESS ownerAddress = (from svaddre in db.B_SV_ADDRESS
                                             join svpc in db.B_SV_PERSON_CONTACT on svaddre.UUID equals svpc.SV_ADDRESS_ID
                                             join svs in db.B_SV_SIGNBOARD on svpc.UUID equals svSignboard.OWNER_ID
                                             select svaddre).FirstOrDefault();

                B_SV_APPOINTED_PROFESSIONAL ap = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID, SignboardConstant.PBP_CODE_AP);

                B_SV_APPOINTED_PROFESSIONAL rge = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID, SignboardConstant.PBP_CODE_RGE);

                B_SV_APPOINTED_PROFESSIONAL rse = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID, SignboardConstant.PBP_CODE_RSE);

                B_SV_APPOINTED_PROFESSIONAL prc = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID, SignboardConstant.PRC_CODE);

                B_SV_APPOINTED_PROFESSIONAL ri = svAppointedProfessionalDAOService.getSvAppointedProfessional(svRecord.UUID, SignboardConstant.PBP_CODE_RI);

                B_CRM_PBP_PRC ap_detail = new B_CRM_PBP_PRC();
                if (ap.CERTIFICATION_NO != null)
                {
                    ap_detail = db.B_CRM_PBP_PRC.Where(x => x.CERTIFICATION_NO == ap.CERTIFICATION_NO).FirstOrDefault();

                }

                string RD_NAME_E = "";
                string RD_NAME_C = "";
                string RD_CONTACT = "";
                DateTime Today = DateTime.Now;


                if (SignboardConstant.FORM_CODE_SC01.Equals(svRecord.FORM_CODE) || SignboardConstant.FORM_CODE_SC01C.Equals(svRecord.FORM_CODE))
                {
                    RD_NAME_E = prc.ENGLISH_NAME;
                    RD_NAME_C = prc.CHINESE_NAME;
                    RD_CONTACT = prc.CONTACT_NO;
                }
                else
                {
                    RD_NAME_E = rge.ENGLISH_NAME;
                    RD_NAME_C = rge.CHINESE_NAME;
                    RD_CONTACT = rge.CONTACT_NO;
                    //putFaxOraddr(RD_FAX_OR_ADDR_E,RD_FAX_OR_ADDR_C,rge)
                }
                string PAW_FAX_OR_ADDR = "";
                if (!string.IsNullOrWhiteSpace(paw.FAX_NO))
                {
                    PAW_FAX_OR_ADDR = paw.FAX_NO;
                }
                else
                {
                    PAW_FAX_OR_ADDR = paw.B_SV_ADDRESS.FULL_ADDRESS;
                }
                DataEntryPrintModel model = new DataEntryPrintModel()
                {
                    //svRecord svSignboard SVaddress
                    FLOOR = svSignboard.B_SV_ADDRESS != null ? svSignboard.B_SV_ADDRESS.FLOOR : "",
                    FLAT = svSignboard.B_SV_ADDRESS != null ? svSignboard.B_SV_ADDRESS.FLOOR : "",
                    STREET = svSignboard.B_SV_ADDRESS != null ? svSignboard.B_SV_ADDRESS.STREET : "",
                    BLOCK = svSignboard.B_SV_ADDRESS != null ? svSignboard.B_SV_ADDRESS.BLOCK : "",
                    STREET_NO = svSignboard.B_SV_ADDRESS != null ? svSignboard.B_SV_ADDRESS.STREET_NO : "",
                    BUILDINGNAME = svSignboard.B_SV_ADDRESS != null ? svSignboard.B_SV_ADDRESS.BUILDINGNAME : "",
                    DISTRICT = svSignboard.B_SV_ADDRESS != null ? svSignboard.B_SV_ADDRESS.DISTRICT : "",
                    REGION = svSignboard.B_SV_ADDRESS != null ? svSignboard.B_SV_ADDRESS.REGION : "",
                    FULL_ADDRESS = svSignboard.B_SV_ADDRESS != null ? svSignboard.B_SV_ADDRESS.FULL_ADDRESS : "",
                    SUBMISSION_NO = svRecord != null ? svRecord.REFERENCE_NO : "",
                    PAW_NAME_E = paw != null ? paw.NAME_ENGLISH : "",
                    PAW_NAME_C = paw != null ? paw.NAME_CHINESE : "",
                    PAW_CONTACT = paw != null ? paw.CONTACT_NO : "",
                    PAW_FAX_OR_ADDR = PAW_FAX_OR_ADDR != null ? PAW_FAX_OR_ADDR : "",
                    PBP_NAME_E = paw != null ? paw.NAME_ENGLISH : "",
                    PBP_NAME_C = paw != null ? paw.NAME_CHINESE : "",
                    PBP_NAME = paw != null ? paw.NAME_ENGLISH : "",
                    PRC_NAME_E = prc != null ? prc.ENGLISH_NAME : "",
                    PRC_NAME_C = prc != null ? prc.CHINESE_NAME : "",
                    PRC_NAME = prc != null ? prc.ENGLISH_NAME : "",
                    PRC_CONTACT = prc != null ? prc.CONTACT_NO : "",
                    PRC_FAX_OR_ADDR_E = prc != null ? prc.FAX_NO : "",
                    PRC_FAX_OR_ADDR_C = prc != null ? prc.FAX_NO : "",
                    AP_NAME_C = ap != null ? ap.CHINESE_NAME : "",
                    AP_NAME_E = ap != null ? ap.ENGLISH_NAME : "",
                    AP_CONTACT = ap != null ? ap.CONTACT_NO : "",
                    AP_FAX_OR_ADDR_C = ap != null ? ap.CONTACT_NO : "",
                    AP_FAX_OR_ADDR_E = ap != null ? ap.CONTACT_NO : "",
                    RSE_NAME_E = rse != null ? rse.ENGLISH_NAME : "",
                    RSE_NAME_C = rse != null ? rse.CHINESE_NAME : "",
                    RSE_CONTACT = rse != null ? rse.CONTACT_NO : "",
                    RSE_FAX_OR_ADDR_C = rse != null ? rse.FAX_NO : "",
                    RI_NAME_C = ri != null ? ri.CHINESE_NAME : "",
                    RI_NAME_E = ri != null ? ri.ENGLISH_NAME : "",
                    RD_NAME_C = RD_NAME_C != null ? RD_NAME_C : "",
                    RD_NAME_E = RD_NAME_E != null ? RD_NAME_E : "",
                    RD_CONTACT = RD_CONTACT != null ? RD_CONTACT : "",
                    APPOINTED_PERSON_NAME_C = ap != null ? ap.ENGLISH_NAME : "",
                    APPOINTED_PERSON_NAME_E = ap != null ? ap.CHINESE_NAME : "",
                    APPOINTED_PERSON_COMPANY_NAME_C = ap_detail != null ? ap_detail.CHINESE_NAME : "",
                    APPOINTED_PERSON_COMPANY_NAME_E = ap_detail != null ? ((ap_detail.SURNAME != null || ap_detail.GIVEN_NAME != null) ? ap_detail.SURNAME + " " + ap_detail.GIVEN_NAME : "") : "",
                    APPOINTED_PERSON_ADDRESS_ROOM_FLAT_BLOCK_E = ap_detail != null ? ap_detail.EN_ADDRESS_LINE1 : "",
                    APPOINTED_PERSON_BUILDINGNAME_E = ap_detail != null ? ap_detail.EN_ADDRESS_LINE2 : "",
                    APPOINTED_PERSON_STREET_E = ap_detail != null ? ap_detail.EN_ADDRESS_LINE3 : "",
                    APPOINTED_PERSON_DISTRICT_E = ap_detail != null ? ap_detail.EN_ADDRESS_LINE4 : "",
                    APPOINTED_PERSON_REGION_E = ap_detail != null ? ap_detail.EN_ADDRESS_LINE5 : "",

                    APPOINTED_PERSON_ADDRESS_ROOM_FLAT_BLOCK_C = ap_detail != null ? ap_detail.CN_ADDRESS_LINE1 : "",
                    APPOINTED_PERSON_BUILDINGNAME_C = ap_detail != null ? ap_detail.CN_ADDRESS_LINE2 : "",
                    APPOINTED_PERSON_STREET_C = ap_detail != null ? ap_detail.CN_ADDRESS_LINE3 : "",
                    APPOINTED_PERSON_DISTRICT_C = ap_detail != null ? ap_detail.CN_ADDRESS_LINE4 : "",
                    APPOINTED_PERSON_REGION_C = ap_detail != null ? ap_detail.CN_ADDRESS_LINE5 : "",

                    //Signboard
                    SIGNBOARD_OWNER_NAME_E = owner.NAME_ENGLISH != null ? owner.NAME_ENGLISH : "",
                    SIGNBOARD_OWNER_NAME_C = owner.NAME_CHINESE != null ? owner.NAME_CHINESE : "",
                    LOCATION_OF_SIGNBOARD = svSignboard.LOCATION_OF_SIGNBOARD != null ? svSignboard.LOCATION_OF_SIGNBOARD : "",
                    SIGNBOARD_DESCRIPTION = svSignboard.DESCRIPTION != null ? svSignboard.DESCRIPTION : "",
                    NOTIFY_DATE = svRecord.ACK_LETTERISS_DATE.ToString() != null ? svRecord.ACK_LETTERISS_DATE.ToString() : "",
                    NOTIFY_DATE_C = DateUtil.getChineseFormatDate(svRecord.ACK_LETTERISS_DATE).ToString() != null ? DateUtil.getChineseFormatDate(svRecord.ACK_LETTERISS_DATE).ToString() : "",
                    RECEIVED_DATE = svRecord.RECEIVED_DATE.ToString() != null ? svRecord.RECEIVED_DATE.ToString() : "",
                    RECEIVED_DATE_C = DateUtil.getChineseFormatDate(svRecord.RECEIVED_DATE).ToString() != null ? DateUtil.getChineseFormatDate(svRecord.RECEIVED_DATE).ToString() : "",
                    RDATE_Y = svRecord.RECEIVED_DATE.Value.Year.ToString(),
                    RDATE_M = svRecord.RECEIVED_DATE.Value.Month.ToString(),
                    RDATE_M_STR = svRecord.RECEIVED_DATE.Value.ToString("MMM", new CultureInfo("zh-cn")),
                    RDATE_D = svRecord.RECEIVED_DATE.Value.Day.ToString("dd"),
                    TODAY_Y = Today.Year.ToString(),
                    TODAY_M = Today.Month.ToString(),
                    TODAY_M_STR = Today.ToString("MMM", new CultureInfo("zh-cn")),
                    TODAY_D = Today.ToString("dd"),
                    FORM_CODE = svRecord.FORM_CODE,
                    LANGCODE = svRecord.LANGUAGE_CODE


                };
                return model;
            }
        }

        public XWPFDocument PrintWord(string svRecordUuid, string languageCode, B_S_LETTER_TEMPLATE letter)
        {
            try
            {
                LetterTemplateService lms = new LetterTemplateService();
                //ValidationPrintModel model = getValidationPrintModel(DisplayModel);
                DataEntryPrintModel model = GetPrintModel(svRecordUuid);
                string filePath = lms.getFilePathByLetterType(letter.LETTER_TYPE) + letter.FILE_NAME;
                CT_SectPr sectPr = new CT_SectPr();
                using (FileStream fs = File.OpenRead(filePath))
                {
                    XWPFDocument doc = new XWPFDocument(fs);
                    sectPr = doc.Document.body.sectPr;
                }
                if (languageCode == null)
                {
                    languageCode = model.LANGCODE;
                }
                    if (languageCode == SignboardConstant.LANG_CHINESE)
                {
                    return BaseCommonService.GetWordDocument(filePath, model, sectPr, SignboardConstant.LANG_CHINESE);
                }
                else
                {
                    return BaseCommonService.GetWordDocument(filePath, model, sectPr, SignboardConstant.LANG_ENGLISH);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}