using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardCommentService
    {
        public CommentDisplayModel list(string recordType, string recordId, string editMode)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                List<B_SV_COMMENT> commentList = new List<B_SV_COMMENT>();
                commentList = db.B_SV_COMMENT.Where(x => x.RECORD_ID == recordId && x.RECORD_TYPE == recordType).OrderByDescending(x => x.MODIFIED_DATE).ToList();

                return new CommentDisplayModel
                {
                    CommentList = (commentList != null && commentList.Count() > 0) ? commentList : null,
                    EditMode = editMode,
                    ErrMsg = "",
                    RecordType = recordType,
                    RecordId = recordId,
                };
            }
        }

        //public CommentDisplayModel load(string recordType, string recordId, string editMode, string commentUuid)
         public CommentDisplayModel load(CommentDisplayModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                List<B_SV_COMMENT> commentList = new List<B_SV_COMMENT>();
                commentList = db.B_SV_COMMENT.Where(x => x.RECORD_ID == model.RecordId).Where(x => x.RECORD_TYPE == model.RecordType).OrderByDescending(x => x.MODIFIED_DATE).ToList();
                B_SV_COMMENT comment = db.B_SV_COMMENT.Find(model.Uuid); // comment uuid

                return new CommentDisplayModel
                {
                    CommentList = (commentList != null && commentList.Count() > 0) ? commentList : null,
                    EditMode = model.EditMode, // "edit"
                    ErrMsg = "",
                    RecordType = model.RecordType, // "AUDIT"
                    Uuid = comment != null ? comment.UUID : "",
                    CommentArea = comment != null ? comment.COMMENT_AREA : null,
                };
            }
            
        }

        public void post(CommentDisplayModel model)
        {
            using(EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // validation
                        string comment_area = model.CommentArea;
                        if(String.IsNullOrEmpty(comment_area) || (String.IsNullOrWhiteSpace(comment_area)))
                        {
                            // return ;
                        }
                        if(model.RecordType.Equals(SignboardConstant.RECORD_TYPE_VALIDATION))
                        {
                            B_SV_VALIDATION svValidation = db.B_SV_VALIDATION.Find(model.RecordId);
                            if(svValidation == null)
                            {
                                // return ;
                            }
                        }
                        if(model.RecordType.Equals(SignboardConstant.RECORD_TYPE_AUDIT))
                        {
                            B_SV_AUDIT_RECORD svAudit = db.B_SV_AUDIT_RECORD.Find(model.RecordId);
                            if(svAudit == null)
                            {
                                // return ;
                            }
                        }

                        B_SV_COMMENT comment = db.B_SV_COMMENT.Find(model.Uuid);

                        if(comment == null) // add
                        {
                            B_SV_COMMENT cm = new B_SV_COMMENT();
                            cm.FROM_USER = SessionUtil.LoginPost.BD_PORTAL_LOGIN;
                            cm.RECORD_ID = model.RecordId;
                            cm.RECORD_TYPE = model.RecordType;
                            cm.COMMENT_AREA = comment_area;
                            db.B_SV_COMMENT.Add(cm);
                        }
                        else // update
                        {
                            comment.COMMENT_AREA = comment_area;
                        }
                        
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                    }

                }
            }

        }
        public static bool IsNew(string uuid)
        {
            if (uuid == null)
            {
                return true;
            }
            else if (uuid.Equals(""))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string getClassCode(string itemNo)
        {
            itemNo = getDisplay(itemNo).Trim();

            if (itemNo.IndexOf("1.") != -1)
            {
                return SignboardConstant.CLASS_I;
            }
            if (itemNo.IndexOf("2.") != -1)
            {
                return SignboardConstant.CLASS_II;
            }
            if (itemNo.IndexOf("3.") != -1)
            {
                return SignboardConstant.CLASS_III;
            }
            return itemNo;
        }
        public static string getDisplay(string dbValue)
        {
            if (dbValue == null)
            {
                return "";
            }
            else
            {
                return dbValue.Trim();
            }
        }
    }
}