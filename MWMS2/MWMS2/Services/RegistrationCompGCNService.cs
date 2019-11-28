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
    public class RegistrationCompGCNService
    {
        String SearchGCN_CGC_q = ""
                         + "\r\n" + "\t" + "SELECT                                                               "
                         + "\r\n" + "\t" + "T1.*,T2.ENGLISH_DESCRIPTION                                          "
                         + "\r\n" + "\t" + "FROM C_COMP_APPLICATION T1                                           "
                         + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE T2 ON T1.APPLICATION_STATUS_ID = T2.UUID  "
                         + "\r\n" + "\t" + "WHERE 1=1                                                            "
                         + "\r\n" + "\t" + "AND T1.REGISTRATION_TYPE = 'CGC'                                     ";

        String SearchGCN_CMW_q = ""
                        + "\r\n" + "\t" + "SELECT                                                               "
                        + "\r\n" + "\t" + "T1.*,T2.ENGLISH_DESCRIPTION                                          "
                        + "\r\n" + "\t" + "FROM C_COMP_APPLICATION T1                                           "
                        + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE T2 ON T1.APPLICATION_STATUS_ID = T2.UUID  "
                        + "\r\n" + "\t" + "WHERE 1=1                                                            "
                        + "\r\n" + "\t" + "AND T1.REGISTRATION_TYPE = 'CMW'                                     ";


        public Fn02GCA_GCNSearchModel SearchGCNCGC(Fn02GCA_GCNSearchModel model)
        {
            model.Query = SearchGCN_CGC_q;  //need to change query after all
            model.QueryWhere = SearchGCN_whereQ(model);
            model.Search();
            return model;
        }
        public string ExportGCN(Fn02GCA_GCNSearchModel model)
        {
            model.Query = SearchGCN_CGC_q;
            model.QueryWhere = SearchGCN_whereQ(model);
            //DisplayGrid dlr = new DisplayGrid() { Query = SearchGCA_q, Columns = Columns, Parameters = post };
            return model.Export("ExportData");
        }
        public Fn02GCA_GCNSearchModel SearchGCNCMW(Fn02GCA_GCNSearchModel model)
        {
            model.Query = SearchGCN_CMW_q;  //need to change query after all
            model.QueryWhere = SearchGCN_whereQ(model);
            model.Search();
            return model;
        }

        public string Excel(Fn02GCA_GCNSearchModel model)
        {
            model.Query = SearchGCN_CMW_q;  //need to change query after all
            model.QueryWhere = SearchGCN_whereQ(model);
            return model.Export("ExportData");
        }

        private string SearchGCN_whereQ(Fn02GCA_GCNSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + "AND UPPER(FILE_REFERENCE_NO) LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ComName))
            {
                whereQ += "\r\n\t" + "AND UPPER(ENGLISH_COMPANY_NAME) LIKE :ComName";
                model.QueryParameters.Add("ComName", "%" + model.ComName.Trim().ToUpper() + "%");
            }
           
            return whereQ;
        }

        public DisplayGrid AjaxApplicantInfos(string FileRef)
        {
            DisplayGrid grid = new DisplayGrid();
            
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List<C_COMP_APPLICANT_INFO> l = db.C_COMP_APPLICANT_INFO.Where(o => o.C_COMP_APPLICATION.FILE_REFERENCE_NO == FileRef)
                    .Include(o => o.C_S_SYSTEM_VALUE)
                    .Include(o => o.C_S_SYSTEM_VALUE1)
                    .Include(O => O.C_APPLICANT)
                    .Include(o => o.C_APPLICANT.C_S_SYSTEM_VALUE)
                    .ToList();
                for(int i = 0;i < l.Count; i++)
                {
                    l[i].C_APPLICANT.Decrypt();
                }


                grid.Data = l.Select(o => new Dictionary<string, object>() {
                     { "V1", o.UUID }
                    ,{ "V2", o.CANDIDATE_NUMBER }
                    ,{ "V3", o.C_APPLICANT.HKID_PASSPORT_DISPLAY }
                    ,{ "V4", o.C_APPLICANT.C_S_SYSTEM_VALUE?.ENGLISH_DESCRIPTION }
                    ,{ "V5", o.C_APPLICANT.FULL_NAME_DISPLAY}
                    ,{ "V6", o.C_APPLICANT.GENDER }
                    ,{ "V7", o.C_S_SYSTEM_VALUE?.CODE }
                    ,{ "V8", o.C_S_SYSTEM_VALUE1?.ENGLISH_DESCRIPTION } //wrong
                    ,{ "V9", o.ACCEPT_DATE }
                    ,{ "V10", o.REMOVAL_DATE }
                    ,{ "V11", o.C_S_SYSTEM_VALUE?.REGISTRATION_TYPE }


                }).ToList();
                grid.Total = grid.Data.Count;
                return grid;

            }
        }//super useful

        public Fn02GCA_GCNDisplayModel ViewGCN(string fileRef)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICATION queryApp = (from ca in db.C_COMP_APPLICATION
                                               join cs in db.C_S_SYSTEM_VALUE on ca.APPLICATION_STATUS_ID equals cs.UUID
                                               where ca.FILE_REFERENCE_NO == fileRef
                                               select ca).FirstOrDefault();

                return new Fn02GCA_GCNDisplayModel
                {
                    C_COMP_APPLICATION = queryApp
                   

                };

            }

        }

        private readonly object Locker = new object();
        public ServiceResult GenNo(Fn02GCA_GCNSearchModel model)
        {
            lock (Locker)
            {
                if (model.C_COMP_APPLICANT_INFO_UUIDs == null)
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    for (int i = 0; i < model.C_COMP_APPLICANT_INFO_UUIDs.Count; i++)
                    {
                        C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO = db.C_COMP_APPLICANT_INFO.Find(model.C_COMP_APPLICANT_INFO_UUIDs[i]);
                        C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Where(o => o.C_S_SYSTEM_TYPE.TYPE == "CANDIDATE_NUMBER").FirstOrDefault();
                        Nullable<decimal> ordering = C_S_SYSTEM_VALUE.ORDERING;
                        C_COMP_APPLICANT_INFO.CANDIDATE_NUMBER = decimal.ToInt32(ordering.Value);
                        C_S_SYSTEM_VALUE.ORDERING = C_COMP_APPLICANT_INFO.CANDIDATE_NUMBER + 1;
                        db.SaveChanges();
                    }
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
        }
    }
}