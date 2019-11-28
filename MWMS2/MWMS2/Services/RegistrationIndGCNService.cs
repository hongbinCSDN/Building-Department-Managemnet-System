using MWMS2.Areas.Registration.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using MWMS2.Constant;
using System.Data.Entity;

namespace MWMS2.Services
{
    public class RegistrationIndGCNService
    {
        string SearchGCNIP_q = ""
                         + "\r\n" + "\t" + "select ind.*, app.SURNAME,app.GIVEN_NAME_ON_ID,"
                         + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " as hkid ,"
                         + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " as passport_no ,"
                         + "\r\n" + "\t" + " (" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " || '/' || " + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " ) as HKIDPASSPORT"
                         + "\r\n" + "\t" + " ,(app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID )as NAME,"
                         + "\r\n" + "\t" + " app.CHINESE_NAME, app.GENDER "
                         + "\r\n" + "\t" + "from C_Ind_Application ind, C_Applicant app                      "
                         + "\r\n" + "\t" + "where ind.APPLICANT_ID = app.UUID and ind.registration_Type = 'IP' ";

        string SearchGCNIMW_q = ""
                         + "\r\n" + "\t" + "select ind.*, app.SURNAME,app.GIVEN_NAME_ON_ID,"
                         + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " as hkid ,"
                         + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " as passport_no ,"
                         + "\r\n" + "\t" + " (" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " || '/' || " + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " ) as HKIDPASSPORT"
                         + "\r\n" + "\t" + " ,(app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID )as NAME,"
                         + "\r\n" + "\t" + " app.CHINESE_NAME, app.GENDER "
                         + "\r\n" + "\t" + "from C_Ind_Application ind, C_Applicant app                      "
                         + "\r\n" + "\t" + "where ind.APPLICANT_ID = app.UUID and ind.registration_Type = 'IMW' ";

        public Fn03PA_GCNSearchModel SearchGCNIP(Fn03PA_GCNSearchModel model)
        {
            model.Query = SearchGCNIP_q;
            model.QueryWhere = SearchGCN_whereQ(model);
            model.Search();
            return model;
        }
        public string ExportGCNIP(Fn03PA_GCNSearchModel model)
        {
            model.Query = SearchGCNIP_q;
            model.QueryWhere = SearchGCN_whereQ(model);

            return model.Export("ExportData");
        }
        public Fn03PA_GCNSearchModel SearchGCNIMW(Fn03PA_GCNSearchModel model)
        {
            model.Query = SearchGCNIP_q;
            model.QueryWhere = SearchGCN_whereQ(model);
            model.Search();
            return model;
        }

        public string Excel(Fn03PA_GCNSearchModel model)
        {
            model.Query = SearchGCNIP_q;
            model.QueryWhere = SearchGCN_whereQ(model);
            return model.Export("ExportData");
        }

        private string SearchGCN_whereQ(Fn03PA_GCNSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + "AND ind.FILE_REFERENCE_NO LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.SurName))
            {
                whereQ += "\r\n\t" + "AND UPPER(app.SURNAME) LIKE :SurnName";
                model.QueryParameters.Add("SurnName", "%" + model.SurName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + "AND UPPER(app.GIVEN_NAME_ON_ID) LIKE :GivenName";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + "AND app.CHINESE_NAME LIKE :ChiName";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }

            return whereQ;
        }

       
        public DisplayGrid AjaxApplicantInfos(string FileRef)
        {
            DisplayGrid grid = new DisplayGrid();
            
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List<C_IND_CERTIFICATE> l = db.C_IND_CERTIFICATE.Where(o => o.C_IND_APPLICATION.FILE_REFERENCE_NO == FileRef)
                   
                    .Include(O => O.C_S_CATEGORY_CODE)
                    .Include(O => O.C_IND_APPLICATION)
                    .ToList();

                grid.Data = l.Select(o => new Dictionary<string, object>() {
                     { "V1", o.UUID }
                    ,{ "V2", o.CANDIDATE_NUMBER }
                    ,{ "V3", o.C_S_CATEGORY_CODE.CODE }
                    ,{ "V4", o.C_S_CATEGORY_CODE.ENGLISH_DESCRIPTION}
                   


                }).ToList();
                grid.Total = grid.Data.Count;
                return grid;

            }
        }//super useful

        private readonly object Locker = new object();
        public ServiceResult GenNo(Fn03PA_GCNSearchModel model)
        {
            lock (Locker)
            {
                if (model.C_IND_CERTIFICATE_UUIDs == null)
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    for (int i = 0; i < model.C_IND_CERTIFICATE_UUIDs.Count; i++)
                    {
                        C_IND_CERTIFICATE C_IND_CERTIFICATE = db.C_IND_CERTIFICATE.Find(model.C_IND_CERTIFICATE_UUIDs[i]);
                        C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Where(o => o.C_S_SYSTEM_TYPE.TYPE == "CANDIDATE_NUMBER").FirstOrDefault();
                        Nullable<decimal> ordering = C_S_SYSTEM_VALUE.ORDERING;
                        C_IND_CERTIFICATE.CANDIDATE_NUMBER = decimal.ToInt32(ordering.Value);
                        C_S_SYSTEM_VALUE.ORDERING = C_IND_CERTIFICATE.CANDIDATE_NUMBER + 1;
                        db.SaveChanges();
                    }
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
        }

    }
}