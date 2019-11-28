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

namespace MWMS2.Services
{
    public class RegistrationMWCAService
    {

        String SearchTemp = ""
               + "\r\n" + "\t" + " SELECT                                                               "
               + "\r\n" + "\t" + " T1.*, T2.ENGLISH_DESCRIPTION                                         "
               + "\r\n" + "\t" + " FROM C_COMP_APPLICATION T1                                           "
               + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE T2 ON T1.APPLICATION_STATUS_ID = T2.UUID  "
               + "\r\n" + "\t" + " WHERE T1.REGISTRATION_TYPE in ('CMW' , 'CGC') Order by T1.file_reference_no  ";
        
        public string ExportTemp(Dictionary<string, string>[] Columns, FormCollection post)
        {
            DisplayGrid dlr = new DisplayGrid() { Query = SearchTemp, Columns = Columns, Parameters = post };
            //dlr.Sort = "indCert.certification_No";
            //dlr.SortType = 2;
            return dlr.Export("ExportData");
        }
        

        public void SearchPM(Fn04MCA_PMSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchTemp;  //need to change query after all
            model.Search();
        }

        public void SearchGCN(Fn04MCA_GCNSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchTemp;  //need to change query after all
            model.Search();
        }

        public void SearchMRA(Fn04MCA_MRASearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchTemp;  //need to change query after all
            model.Search();
        }

        public void SearchIC(InterviewCandidatesSearchModel model)
        {
            //model.Query = SearchCA_q;
            RegistrationCommonService registrationCommonService = new RegistrationCommonService();
            model = registrationCommonService.SearchInterviewCandidatesForCompany(model);
        }

        public void SearchIR(InterviewResultSearchModel model)
        {
            //model.Query = SearchCA_q;
            RegistrationCommonService registrationCommonService = new RegistrationCommonService();
            model = registrationCommonService.SearchInterviewResultForCompany(model);
        }
    }
}