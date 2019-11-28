using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Web;
using MWMS2_RPT.App_Start;
using CrystalDecisions.CrystalReports.Engine;
using Oracle.ManagedDataAccess.Client;
using CrystalDecisions.Shared;
using System.IO;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Linq;

namespace MWMS2_RPT
{
    public partial class _Default : System.Web.UI.Page
    {
        public static string[] CRM0037Title =
        {
                "file_ref","reg_no","title","surname","given_name","chn_name","e_home1","e_home2","e_home3","e_home4","e_home5",
                "c_home1","c_home2","c_home3","c_home4","c_home5","c_o","c_o chn_c_o",
                "e_office1","e_office2","e_office3","e_office4","e_office5","c_office1","c_office2","c_office3","c_office4","c_office5",
                "emrg_no1","emrg_no2","emrg_no3","tel1","tel2","tel3",
                "fax1","fax2","email","pnap","qualification","category_code","expiry_date","form_used","period_of_validity","app_status",
                "date_of_registration","date_of_gazette","date_of_approval","retention_application_submitted","retention_commenced","restoration_application_submitted",
                "restoration_commenced","removed_from_register","extended_date_of_expiry","eng_authority_name","chn_authority_name",
                "eng_authority_title","chn_authority_title","letter_file_ref","Interested_in_Providing_Services_of_QP",
                "Interested_in_Providing_Services_in_Fire_Safety","issue_date_of_QpCard","expiry_date_of_QpCard","serial_no_of_QpCard return_date_of_QpCard"
        };


        private Dictionary<string, Object> getRequstParameters()
        {
            NameValueCollection pColl = Request.Params;

            Dictionary<string, Object> myDictionary = new Dictionary<string, Object>();
            if (pColl.Count != 0)
            {
                for (int i = 0; i < pColl.Count; i++)
                {
                    myDictionary.Add(pColl.GetKey(i) + "", pColl.GetValues(i)[0]);
                }
            }
            pColl = Request.QueryString;
            if (pColl.Count != 0)
            {
                for (int i = 0; i < pColl.Count; i++)
                {
                    if (myDictionary.ContainsKey(pColl.GetKey(i))) continue;
                    myDictionary.Add(pColl.GetKey(i) + "", pColl.GetValues(i)[0]);
                }
            }

            return myDictionary;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            string passType = checkParam();
            Dictionary<string, Object> myDictionary = new Dictionary<string, Object>();
            //edit for 19-08-2019
            //myDictionary = getRequstParameters();
            myDictionary = getDictionary(passType);

            if (myDictionary.Count == 0)
            {
                string url = HttpContext.Current.Request.Url.Query;
                if (!String.IsNullOrEmpty(url))
                {
                    //string p = url.Substring(1, url.Length - 1);
                    string p = url.Substring(1, url.Length - 1).Replace("amp%3b", "").Replace("+", " ").Replace("%2f", "/");
                    myDictionary = getMyDictionary(p);
                    LoadCrystalReports(myDictionary);
                }
                else
                {
                    Response.Write("Report ID Can not be empty!");
                }
            }
            else
            {
                LoadCrystalReports(myDictionary);
            }
        }

        private string getRptPath(string rptId)
        {
            string path = null;
            if (rptId != null)
            {
                switch (rptId)
                {
                    case "CRM0001":
                        path = "Reports/10-day_pledge_CGC.rpt";
                        break;
                    case "CRM0002":
                        path = "Reports/admin_rpt_committee_type.rpt";
                        break;
                    case "CRM0003":
                        path = "Reports/admin_rpt_panel_type.rpt";
                        break;
                    case "CRM0004":
                        path = "Reports/annual_gazette_CHN_CGC.rpt";
                        break;
                    case "CRM0005":
                        path = "Reports/RPT_ExpiryContractorCGC.rpt";
                        break;
                    case "CRM0006":
                        path = "Reports/RPT_applicant_info_CMW.rpt";
                        break;
                    case "CRM0007":
                        path = "Reports/application_count_CMW.rpt";
                        break;
                    case "CRM0008":
                        path = "Reports/application_count_IP.rpt";
                        break;
                    case "CRM0009":
                        path = "Reports/attendance_month.rpt";
                        break;
                    case "CRM0010":
                        path = "Reports/audit_log.rpt";
                        break;
                    case "CRM0011":
                        path = "Reports/c_mwc.rpt";
                        break;
                    case "CRM0012":
                        path = "Reports/c_mwc(w).rpt";
                        break;
                    case "CRM0013":
                        path = "Reports/c_qp_mwc.rpt";
                        break;
                    case "CRM0014":
                        path = "Reports/RPT_caseInHand.rpt";
                        break;
                    case "CRM0015":
                        path = "Reports/RPT_EMwcW.rpt";
                        break;
                    case "CRM0016":
                        path = "Reports/RPT_EQpMwcW.rpt";
                        break;
                    case "CRM0017":
                        path = "Reports/RPT_Expiry_registers_IP.rpt";
                        break;
                    case "CRM0018":
                        path = "Reports/RPT_countExpiryDateQpCardAll.rpt";
                        break;
                    case "CRM0019":
                        path = "Reports/RPT_countIR.rpt";
                        break;
                    case "CRM0020":
                        path = "Reports/RPT_countIssueDateQpCardAll.rpt";
                        break;
                    case "CRM0021":
                        path = "Reports/RPT_CountIssuedQpCardAll.rpt";
                        break;
                    case "CRM0022":
                        path = "Reports/RPT_countQP.rpt";
                        break;
                    case "CRM0023":
                        path = "Reports/RPT_countQpAll.rpt";
                        break;
                    case "CRM0024":
                        path = "Reports/RPT_countQpAllHistory.rpt";
                        break;
                    case "CRM0025":
                        path = "Reports/RPT_countQpAllHistory_xls.rpt";
                        break;
                    case "CRM0026":
                        path = "Reports/RPT_countQPXls.rpt";
                        break;
                    case "CRM0027":
                        path = "Reports/RPT_CountReturnDateQpCardAll.rpt";
                        break;
                    case "CRM0028":
                        path = "Reports/RPT_countRI.rpt";
                        break;
                    case "CRM0029":
                        path = "Reports/annual_gazette_ENG_IP.rpt";
                        break;
                    case "CRM0030":
                        path = "Reports/RPT_CheckPRS.rpt";
                        break;
                    case "CRM0031":
                        path = "Reports/RPT_CheckPRSXls.rpt";
                        break;
                    case "CRM0032":
                        path = "Reports/RPT_conviction_CGC1.rpt";
                        break;
                    case "CRM0033":
                        path = "Reports/RPT_convictionIP.rpt";
                        break;
                    case "CRM0034":
                        path = "Reports/RPT_ConvictionIP2.rpt";
                        break;
                    case "CRM0035":
                        path = "Reports/RPT_ConvictionIP3.rpt";
                        break;
                    case "CRM0036":
                        path = "Reports/RPT_FastTrack_CGC.rpt";
                        break;
                    case "CRM0037":
                        path = "Reports/RPT_Export_APRSERGE_information_IP.rpt";
                        break;
                    case "CRM0038":
                        path = "Reports/interested_QP_statistic.rpt";
                        break;
                    case "CRM0039":
                        path = "Reports/interested_QP_statisticXls.rpt";
                        break;
                    case "CRM0040":
                        path = "Reports/MMD0003a_1_IP.rpt";
                        break;
                    case "CRM0041":
                        path = "Reports/MMD0003a_4_IP.rpt";
                        break;
                    case "CRM0042":
                        path = "Reports/MMD0003b_1_CGC.rpt";
                        break;
                    case "CRM0043":
                        path = "Reports/MMD0003b_4_CGC.rpt";
                        break;
                    case "CRM0044":
                        path = "Reports/MMD0010a_IP.rpt";
                        break;
                    case "CRM0045":
                        path = "Reports/MMD0010b_CGC.rpt";
                        break;
                    case "CRM0046":
                        path = "Reports/MWCP_ExpiryDate.rpt";
                        break;
                    case "CRM0047":
                        path = "Reports/MWCP_ExpiryDate_xls.rpt";
                        break;
                    case "CRM0048":
                        path = "Reports/no_of_conviction_CGC.rpt";
                        break;
                    case "CRM0049":
                        path = "Reports/no_of_conviction_CGC_xls.rpt";
                        break;
                    case "CRM0050":
                        path = "Reports/no_of_reg_CGC.rpt";
                        break;
                    case "CRM0051":
                        path = "Reports/no_of_reg_CGC_xls.rpt";
                        break;
                    case "CRM0052":
                        path = "Reports/no_of_reg_CMW.rpt";
                        break;
                    case "CRM0053":
                        path = "Reports/no_of_reg_CMW_xls.rpt";
                        break;
                    case "CRM0054":
                        path = "Reports/CGC_process_monitor.rpt";
                        break;
                    case "CRM0055":
                        path = "Reports/applicants_mon_CGC_I_A.rpt";
                        break;
                    case "CRM0056":
                        path = "Reports/attendance_period.rpt";
                        break;
                    case "CRM0057":
                        path = "Reports/application_count_CGC.rpt";
                        break;
                    case "CRM0059":
                        path = "Reports/StatusAppControlForm_IP.rpt";
                        break;
                    case "CRM0060":
                        path = "Reports/StatusAppControlForm_IP_RI.rpt";
                        break;
                    case "CRM0061":
                        path = "Reports/no_of_reg_IP.rpt";
                        break;
                    case "CRM0062":
                        path = "Reports/RPT0010b_CGC.rpt";
                        break;
                    case "CRM0063":
                        path = "Reports/RPT0010b_a2_CGC.rpt";
                        break;
                    case "CRM0064":
                        path = "Reports/annual_gazette_ENG_CGC.rpt";
                        break;
                    case "CRM0065":
                        path = "Reports/annual_gazette_CHN_IP.rpt";
                        break;
                    case "CRM0066":
                        path = "Reports/RPT0007b_CGC.rpt";
                        break;
                    case "CRM0068":
                        path = "Reports/RPT0004_CGC.rpt";
                        break;
                    case "CRM0069":
                        path = "Reports/RPT0008_CGC_IP.rpt";
                        break;
                    case "CRM0070":
                        path = "Reports/RPT0008_IP.rpt";
                        break;
                    case "CRM0071":
                        path = "Reports/RPT0007a_IP.rpt";
                        break;
                    case "CRM0073":
                        path = "Reports/annual_gazette_CHN_MWC.rpt";
                        break;
                    case "CRM0074":
                        path = "Reports/process_monitor_MWC.rpt";
                        break;
                    case "CRM0076":
                        path = "Reports/RPT0004_CMW.rpt";
                        break;
                    case "CRM0078":
                        path = "Reports/RPT0010a_a3_IP.rpt";
                        break;
                    case "CRM0079":
                        path = "Reports/RPT0002_IMW_NEW.rpt";
                        break;
                    case "CRM0080":
                        path = "Reports/RPT0004_IMW.rpt";
                        break;
                    case "CRM0085":
                        path = "Reports/prog_mwc_reg.rpt";
                        break;
                    case "CRM0087":
                        path = "Reports/StatusAppControlForm_IMW.rpt";
                        break;
                    case "CRM0088":
                        path = "Reports/RPT0002_CGC_IP.rpt";
                        break;
                    case "CRM0089":
                        path = "Reports/RPT_countFSAll.rpt";
                        break;
                    case "CRM0090":
                        path = "Reports/RPT_countMBISAll.rpt";
                        break;
                    case "CRM0091":
                        path = "Reports/CheckMultiReg.rpt";
                        break;
                    case "CRM0092":
                        path = "Reports/RPT0002_IMW.rpt";
                        break;
                    case "CRM0093":
                        path = "Reports/RPT0002_CMW.rpt";
                        break;
                    case "CRM0094":
                        path = "Reports/applicant_info.rpt";
                        break;
                    case "CRM0095":
                        path = "Reports/Summary_Report_for_Application_Status.rpt";
                        break;

                }
            }

            return path;
        }

        private Dictionary<string, Object> getMyDictionary(string p)
        {
            Dictionary<string, Object> myDictionary = new Dictionary<string, Object>();
            SortedList table = getParam(p);
            if (table != null)
            {
                foreach (DictionaryEntry De in table)
                {
                    myDictionary.Add(De.Key + "", De.Value);
                }
            }
            return myDictionary;
        }

        private String checkParam()
        {
            string type = "";
            string Re = "";
            Re += "数据传送方式：";
            if (Request.RequestType.ToUpper() == "POST")
            {
                type = "POST";
                Re += type + "<br/>参数分别是：<br/>";
                SortedList table = Param();
                if (table != null)
                {
                    foreach (DictionaryEntry De in table)
                    {
                        Re += "参数名：" + De.Key + " 值：" + De.Value + "<br/>";
                    }
                }
                else
                { Re = "你没有传递任何参数过来！"; }
            }
            else
            {
                type = "GET";
                Re += type + "<br/>参数分别是：<br/>";
                NameValueCollection nvc = GETInput();
                if (nvc.Count != 0)
                {
                    for (int i = 0; i < nvc.Count; i++)
                    {
                        Re += "参数名：" + nvc.GetKey(i) + "值：" + nvc.GetValues(i)[0] + "<br/>";
                    }
                }
                else
                { Re = "你没有传递任何参数过来！"; }
            }
            //Response.Write(Re);
            return type;
        }

        private SortedList getParam(string param)
        {
            SortedList SortList = new SortedList();

            int index = param.IndexOf("&");
            string[] Arr = { };
            if (index != -1) //参数传递不只一项
            {
                Arr = param.Split('&');
                for (int i = 0; i < Arr.Length; i++)
                {
                    int equalindex = Arr[i].IndexOf('=');
                    string paramN = Arr[i].Substring(0, equalindex);
                    string paramV = Arr[i].Substring(equalindex + 1);
                    if (!SortList.ContainsKey(paramN))
                    //避免用户传递相同参数
                    { SortList.Add(paramN, paramV); }
                    else //如果有相同的，一直删除取最后一个值为准
                    {
                        SortList.Remove(paramN); SortList.Add(paramN, paramV);
                    }
                }
            }
            else //参数少于或等于1项
            {
                int equalindex = param.IndexOf('=');
                if (equalindex != -1)
                { //参数是1项
                    string paramN = param.Substring(0, equalindex);
                    string paramV = param.Substring(equalindex + 1);
                    SortList.Add(paramN, paramV);
                }
                else //没有传递参数过来
                { SortList = null; }
            }
            return SortList;
        }

        private SortedList Param()
        {
            string POSTStr = PostInput();
            SortedList SortList = new SortedList();

            int index = POSTStr.IndexOf("&");
            string[] Arr = { };
            if (index != -1) //参数传递不只一项
            {
                Arr = POSTStr.Split('&');
                for (int i = 0; i < Arr.Length; i++)
                {
                    int equalindex = Arr[i].IndexOf('=');
                    string paramN = Arr[i].Substring(0, equalindex);
                    string paramV = Arr[i].Substring(equalindex + 1);
                    if (!SortList.ContainsKey(paramN))
                    //避免用户传递相同参数
                    { SortList.Add(paramN, paramV); }
                    else //如果有相同的，一直删除取最后一个值为准
                    {
                        SortList.Remove(paramN); SortList.Add(paramN, paramV);
                    }
                }
            }
            else //参数少于或等于1项
            {
                int equalindex = POSTStr.IndexOf('=');
                if (equalindex != -1)
                { //参数是1项
                    string paramN = POSTStr.Substring(0, equalindex);
                    string paramV = POSTStr.Substring(equalindex + 1);
                    SortList.Add(paramN, paramV);
                }
                else //没有传递参数过来
                { SortList = null; }
            }
            return SortList;
        }

        private string PostInput()
        {
            try
            {

                System.IO.Stream s = Request.InputStream;
                int count = 0;
                byte[] buffer = new byte[1024];
                StringBuilder builder = new StringBuilder();
                while ((count = s.Read(buffer, 0, 1024)) > 0)
                {
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
                }
                s.Flush();
                s.Close();
                s.Dispose();
                return builder.ToString();
            }
            catch (Exception ex)
            { throw ex; }
        }

        private NameValueCollection GETInput()
        { return Request.QueryString; }

        private Dictionary<string, Object> getDictionary(string type)
        {
            type = "GET";
            Dictionary<string, Object> myDictionary = new Dictionary<string, Object>();

            if (type == "POST")
            {
                SortedList table = Param();
                if (table != null)
                {
                    foreach (DictionaryEntry De in table)
                    {
                        myDictionary.Add(De.Key + "", De.Value);
                    }
                }

            }
            else
            {
                NameValueCollection nvc = GETInput();
                if (nvc.Count != 0)
                {
                    for (int i = 0; i < nvc.Count; i++)
                    {
                        //myDictionary.Add(nvc.GetKey(i) + "", nvc.GetValues(i)[0]);
                        myDictionary.Add(nvc.GetKey(i).Replace("amp;", "") + "", nvc.GetValues(i)[0]);
                    }
                }
            }

            return myDictionary;
        }

        private Boolean checkSubRptId(string rptId)
        {
            Boolean checkSubRpt = false;
            switch (rptId)
            {
                case "CRM0069":
                    checkSubRpt = true;
                    break;
            }
            return checkSubRpt;
        }

        //andy
        private Boolean checkSpecRptId(string rptId)
        {
            Boolean checkSpecRpt = false;
            switch (rptId)
            {
                //case "CRM0004":
                //    checkSpecRpt = true;
                //    break;
                //case "CRM0006":
                //    checkSpecRpt = true;
                //    break;
                //case "CRM0009":
                //    checkSpecRpt = true;
                //    break;
                //case "CRM0022":
                //    checkSpecRpt = true;
                //    break;
                //case "CRM0029":
                //    checkSpecRpt = true;
                //    break;
                //case "CRM0032":
                //    checkSpecRpt = true;
                //    break;
                //case "CRM0046":
                //    checkSpecRpt = true;
                //    break;
                //case "CRM0055":
                //    checkSpecRpt = true;
                //    break;
                //case "CRM0056":
                //    checkSpecRpt = true;
                //    break;
                //case "CRM0059":
                //    checkSpecRpt = true;
                //    break;
                case "CRM0060":
                    checkSpecRpt = true;
                    break;
                    //case "CRM0062":
                    //    checkSpecRpt = true;
                    //    break;
                    //case "CRM0063":
                    //    checkSpecRpt = true;
                    //    break;
                    //case "CRM0064":
                    //    checkSpecRpt = true;
                    //    break;
                    //case "CRM0065":
                    //    checkSpecRpt = true;
                    //    break;
                    //case "CRM0066":
                    //    checkSpecRpt = true;
                    //    break;
                    //case "CRM0071":
                    //    checkSpecRpt = true;
                    //    break;
            }

            return checkSpecRpt;
        }

        private bool CheckListSqlRptId(string rptId)
        {
            bool checkListSqlRpt = false;
            switch (rptId)
            {
                case "CRM0069":
                    checkListSqlRpt = true;
                    break;
                case "CRM0088":
                    checkListSqlRpt = true;
                    break;
            }
            return checkListSqlRpt;
        }

        private bool CheckEmptyRecord(string rptId)
        {
            bool checkEmptyRecord = false;
            switch (rptId)
            {
                case "CRM0064":
                case "CRM0004":
                case "CRM0029":
                case "CRM0065":
                case "CRM0073":
                    checkEmptyRecord = true;
                    break;
            }
            return checkEmptyRecord;
        }

        private Boolean checkExcelRptId(string rptId)
        {
            Boolean checkExcelRpt = false;
            switch (rptId)
            {
                case "CRM0037":
                    checkExcelRpt = true;
                    break;
            }

            return checkExcelRpt;
        }

        private string getExcelFileName(string rptId)
        {
            string fileName = null;
            switch (rptId)
            {
                case "CRM0037":
                    fileName = "appln_info_IP";
                    break;
            }
            return fileName;
        }

        private OracleParameter[] getCRM0004ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string as_at_date = null;
            string gaz_fr_date = null;
            string gaz_to_date = null;
            string reg_type = null;
            int index = 10;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "as_at_date":
                        as_at_date = item.Value.ToString();
                        break;
                    case "gaz_fr_date":
                        gaz_fr_date = item.Value.ToString();
                        break;
                    case "gaz_to_date":
                        gaz_to_date = item.Value.ToString();
                        break;
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":as_at_date", as_at_date);
            paramList[1] = new OracleParameter(":as_at_date", as_at_date);
            paramList[2] = new OracleParameter(":as_at_date", as_at_date);
            paramList[3] = new OracleParameter(":reg_type", reg_type);
            paramList[4] = new OracleParameter(":reg_type", reg_type);
            paramList[5] = new OracleParameter(":gaz_fr_date", gaz_fr_date);
            paramList[6] = new OracleParameter(":gaz_to_date", gaz_to_date);
            paramList[7] = new OracleParameter(":as_at_date", as_at_date);
            paramList[8] = new OracleParameter(":as_at_date", as_at_date);
            paramList[9] = new OracleParameter(":as_at_date", as_at_date);

            return paramList;
        }

        private OracleParameter[] getCRM0006ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string c_appln_id = null;
            string appln_id = null;
            string reg_type = null;
            string file_ref_no = null;
            int index = 31;  // paramIndex
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "c_appln_id":
                        c_appln_id = item.Value.ToString();
                        break;
                    case "appln_id":
                        appln_id = item.Value.ToString();
                        break;
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                    case "file_ref_no":
                        file_ref_no = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":c_appln_id", c_appln_id);
            paramList[1] = new OracleParameter(":file_ref_no", file_ref_no);
            paramList[2] = new OracleParameter(":appln_id", appln_id);

            paramList[3] = new OracleParameter(":c_appln_id", c_appln_id);
            paramList[4] = new OracleParameter(":file_ref_no", file_ref_no);
            paramList[5] = new OracleParameter(":appln_id", appln_id);

            paramList[6] = new OracleParameter(":c_appln_id", c_appln_id);
            paramList[7] = new OracleParameter(":file_ref_no", file_ref_no);
            paramList[8] = new OracleParameter(":appln_id", appln_id);

            paramList[9] = new OracleParameter(":c_appln_id", c_appln_id);
            paramList[10] = new OracleParameter(":file_ref_no", file_ref_no);
            paramList[11] = new OracleParameter(":reg_type", reg_type);
            paramList[12] = new OracleParameter(":appln_id", appln_id);

            paramList[13] = new OracleParameter(":c_appln_id", c_appln_id);
            paramList[14] = new OracleParameter(":file_ref_no", file_ref_no);
            paramList[15] = new OracleParameter(":reg_type", reg_type);
            paramList[16] = new OracleParameter(":appln_id", appln_id);

            paramList[17] = new OracleParameter(":c_appln_id", c_appln_id);
            paramList[18] = new OracleParameter(":file_ref_no", file_ref_no);
            paramList[19] = new OracleParameter(":appln_id", appln_id);
            paramList[20] = new OracleParameter(":reg_type", reg_type);
            paramList[21] = new OracleParameter(":appln_id", appln_id);

            paramList[22] = new OracleParameter(":c_appln_id", c_appln_id);
            paramList[23] = new OracleParameter(":file_ref_no", file_ref_no);
            paramList[24] = new OracleParameter(":appln_id", appln_id);
            paramList[25] = new OracleParameter(":reg_type", reg_type);
            paramList[26] = new OracleParameter(":appln_id", appln_id);

            paramList[27] = new OracleParameter(":c_appln_id", c_appln_id);
            paramList[28] = new OracleParameter(":file_ref_no", file_ref_no);
            paramList[29] = new OracleParameter(":reg_type", reg_type);
            paramList[30] = new OracleParameter(":appln_id", appln_id);

            return paramList;
        }

        private OracleParameter[] getCRM0009ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string year = null;
            string month = null;
            string reg_type = null;
            int index = 17;  // paramIndex
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "AttendancYear":
                        year = item.Value.ToString();
                        break;
                    case "AttendancMonth":
                        month = item.Value.ToString();
                        break;
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":AttendancYear", year);
            paramList[1] = new OracleParameter(":AttendancMonth", month);
            paramList[2] = new OracleParameter(":reg_type", reg_type);

            paramList[3] = new OracleParameter(":AttendancYear", year);
            paramList[4] = new OracleParameter(":AttendancMonth", month);
            paramList[5] = new OracleParameter(":reg_type", reg_type);

            paramList[6] = new OracleParameter(":AttendancYear", year);
            paramList[7] = new OracleParameter(":AttendancMonth", month);
            paramList[8] = new OracleParameter(":reg_type", reg_type);

            paramList[9] = new OracleParameter(":AttendancMonth", month);
            paramList[10] = new OracleParameter(":AttendancYear", year);

            paramList[11] = new OracleParameter(":AttendancYear", year);
            paramList[12] = new OracleParameter(":reg_type", reg_type);

            paramList[13] = new OracleParameter(":AttendancYear", year);
            paramList[14] = new OracleParameter(":AttendancMonth", month);
            paramList[15] = new OracleParameter(":reg_type", reg_type);
            paramList[16] = new OracleParameter(":reg_type", reg_type);

            return paramList;
        }

        private OracleParameter[] getCRM0022ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string today = null;
            string willingness_qp = null;
            int index = 23;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "today":
                        today = item.Value.ToString();
                        break;
                    case "willingness_qp":
                        willingness_qp = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":willingness_qp", willingness_qp);
            paramList[1] = new OracleParameter(":today", today);
            paramList[2] = new OracleParameter(":today", today);
            paramList[3] = new OracleParameter(":today", today);
            paramList[4] = new OracleParameter(":today", today);

            paramList[5] = new OracleParameter(":today", today);
            paramList[6] = new OracleParameter(":today", today);
            paramList[7] = new OracleParameter(":today", today);
            paramList[8] = new OracleParameter(":today", today);
            paramList[9] = new OracleParameter(":today", today);

            paramList[10] = new OracleParameter(":today", today);
            paramList[11] = new OracleParameter(":today", today);
            paramList[12] = new OracleParameter(":today", today);
            paramList[13] = new OracleParameter(":today", today);
            paramList[14] = new OracleParameter(":today", today);
            paramList[15] = new OracleParameter(":today", today);

            paramList[16] = new OracleParameter(":today", today);
            paramList[17] = new OracleParameter(":today", today);
            paramList[18] = new OracleParameter(":today", today);
            paramList[19] = new OracleParameter(":today", today);
            paramList[20] = new OracleParameter(":today", today);
            paramList[21] = new OracleParameter(":today", today);
            paramList[22] = new OracleParameter(":today", today);


            return paramList;
        }

        private OracleParameter[] getCRM0029ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string as_at_date = null;
            string gaz_fr_date = null;
            string gaz_to_date = null;
            string reg_type = null;
            string cat_gp = null;
            int index = 13;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "as_at_date":
                        as_at_date = item.Value.ToString();
                        break;
                    case "cat_gp":
                        cat_gp = item.Value.ToString();
                        break;
                    case "gaz_fr_date":
                        gaz_fr_date = item.Value.ToString();
                        break;
                    case "gaz_to_date":
                        gaz_to_date = item.Value.ToString();
                        break;
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":as_at_date", as_at_date);
            paramList[1] = new OracleParameter(":as_at_date", as_at_date);
            paramList[2] = new OracleParameter(":as_at_date", as_at_date);
            paramList[3] = new OracleParameter(":gaz_fr_date", gaz_fr_date);
            paramList[4] = new OracleParameter(":gaz_to_date", gaz_to_date);

            paramList[5] = new OracleParameter(":reg_type", reg_type);
            paramList[6] = new OracleParameter(":reg_type", reg_type);
            paramList[7] = new OracleParameter(":cat_gp", cat_gp);
            paramList[8] = new OracleParameter(":gaz_fr_date", gaz_fr_date);
            paramList[9] = new OracleParameter(":gaz_to_date", gaz_to_date);
            paramList[10] = new OracleParameter(":as_at_date", as_at_date);
            paramList[11] = new OracleParameter(":as_at_date", as_at_date);
            paramList[12] = new OracleParameter(":as_at_date", as_at_date);

            return paramList;
        }

        private OracleParameter[] getCRM0032ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string today = null;
            string reg_type = null;
            int index = 2;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "today":
                        today = item.Value.ToString();
                        break;
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                }
            }
            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":today", today);
            paramList[1] = new OracleParameter(":reg_type", reg_type);

            return paramList;
        }

        private OracleParameter[] getCRM0046ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string report_type = null;
            string inputFirstApplicationDate = null;
            string inputOutstandingDate = null;
            string inputResultDate = null;
            int index = 12;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "report_type":
                        report_type = item.Value.ToString();
                        break;
                    case "inputFirstApplicationDate":
                        inputFirstApplicationDate = item.Value.ToString();
                        break;
                    case "inputOutstandingDate":
                        inputOutstandingDate = item.Value.ToString();
                        break;
                    case "inputResultDate":
                        inputResultDate = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":report_type", report_type);
            paramList[1] = new OracleParameter(":report_type", report_type);
            paramList[2] = new OracleParameter(":inputFirstApplicationDate", inputFirstApplicationDate);
            paramList[3] = new OracleParameter(":report_type", report_type);
            paramList[4] = new OracleParameter(":inputFirstApplicationDate", inputFirstApplicationDate);
            paramList[5] = new OracleParameter(":report_type", report_type);
            paramList[6] = new OracleParameter(":inputOutstandingDate", inputOutstandingDate);
            paramList[7] = new OracleParameter(":report_type", report_type);
            paramList[8] = new OracleParameter(":inputResultDate", inputResultDate);
            paramList[9] = new OracleParameter(":report_type", report_type);
            paramList[10] = new OracleParameter(":inputResultDate", inputResultDate);
            paramList[11] = new OracleParameter(":report_type", report_type);

            return paramList;
        }

        private OracleParameter[] getCRM0055ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string as_year = null;
            string reg_type = null;
            int index = 44;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "as_year":
                        as_year = item.Value.ToString();
                        break;
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":as_year", as_year);
            paramList[1] = new OracleParameter(":reg_type", reg_type);
            paramList[2] = new OracleParameter(":as_year", as_year);
            paramList[3] = new OracleParameter(":reg_type", reg_type);
            paramList[4] = new OracleParameter(":as_year", as_year);
            paramList[5] = new OracleParameter(":reg_type", reg_type);
            paramList[6] = new OracleParameter(":as_year", as_year);
            paramList[7] = new OracleParameter(":reg_type", reg_type);
            paramList[8] = new OracleParameter(":as_year", as_year);
            paramList[9] = new OracleParameter(":reg_type", reg_type);
            paramList[10] = new OracleParameter(":as_year", as_year);
            paramList[11] = new OracleParameter(":reg_type", reg_type);
            paramList[12] = new OracleParameter(":as_year", as_year);
            paramList[13] = new OracleParameter(":reg_type", reg_type);
            paramList[14] = new OracleParameter(":as_year", as_year);
            paramList[15] = new OracleParameter(":reg_type", reg_type);
            paramList[16] = new OracleParameter(":as_year", as_year);
            paramList[17] = new OracleParameter(":reg_type", reg_type);
            paramList[18] = new OracleParameter(":as_year", as_year);
            paramList[19] = new OracleParameter(":reg_type", reg_type);
            paramList[20] = new OracleParameter(":as_year", as_year);
            paramList[21] = new OracleParameter(":reg_type", reg_type);
            paramList[22] = new OracleParameter(":as_year", as_year);
            paramList[23] = new OracleParameter(":reg_type", reg_type);
            paramList[24] = new OracleParameter(":as_year", as_year);
            paramList[25] = new OracleParameter(":reg_type", reg_type);
            paramList[26] = new OracleParameter(":as_year", as_year);
            paramList[27] = new OracleParameter(":reg_type", reg_type);
            paramList[28] = new OracleParameter(":as_year", as_year);
            paramList[29] = new OracleParameter(":reg_type", reg_type);
            paramList[30] = new OracleParameter(":as_year", as_year);
            paramList[31] = new OracleParameter(":reg_type", reg_type);
            paramList[32] = new OracleParameter(":as_year", as_year);
            paramList[33] = new OracleParameter(":reg_type", reg_type);
            paramList[34] = new OracleParameter(":as_year", as_year);
            paramList[35] = new OracleParameter(":reg_type", reg_type);
            paramList[36] = new OracleParameter(":as_year", as_year);
            paramList[37] = new OracleParameter(":reg_type", reg_type);
            paramList[38] = new OracleParameter(":as_year", as_year);
            paramList[39] = new OracleParameter(":reg_type", reg_type);
            paramList[40] = new OracleParameter(":as_year", as_year);
            paramList[41] = new OracleParameter(":reg_type", reg_type);
            paramList[42] = new OracleParameter(":as_year", as_year);
            paramList[43] = new OracleParameter(":reg_type", reg_type);

            return paramList;
        }

        private OracleParameter[] getCRM0056ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string fr_date = null;
            string reg_type = null;
            string to_date = null;
            int index = 14;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "fr_date":
                        fr_date = item.Value.ToString();
                        break;
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                    case "to_date":
                        to_date = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":fr_date", fr_date);
            paramList[1] = new OracleParameter(":to_date", to_date);

            paramList[2] = new OracleParameter(":fr_date", fr_date);
            paramList[3] = new OracleParameter(":reg_type", reg_type);
            paramList[4] = new OracleParameter(":fr_date", fr_date);
            paramList[5] = new OracleParameter(":to_date", to_date);
            paramList[6] = new OracleParameter(":reg_type", reg_type);
            paramList[7] = new OracleParameter(":reg_type", reg_type);
            paramList[8] = new OracleParameter(":fr_date", fr_date);
            paramList[9] = new OracleParameter(":reg_type", reg_type);
            paramList[10] = new OracleParameter(":fr_date", fr_date);
            paramList[11] = new OracleParameter(":to_date", to_date);
            paramList[12] = new OracleParameter(":reg_type", reg_type);
            paramList[13] = new OracleParameter(":reg_type", reg_type);

            return paramList;
        }

        private OracleParameter[] getCRM0059ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string cat_gp = null;
            string reg_type = null;
            int index = 4;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                    case "CategoryGroup":
                        cat_gp = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];
            paramList[0] = new OracleParameter(":cat_gp", cat_gp);
            paramList[1] = new OracleParameter(":cat_gp", cat_gp);
            paramList[2] = new OracleParameter(":reg_type", reg_type);
            paramList[3] = new OracleParameter(":cat_gp", cat_gp);

            return paramList;
        }

        private OracleParameter[] getCRM0060ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string cat_gp = null;
            string reg_type = null;
            int index = 4;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                    case "cat_gp":
                        cat_gp = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];
            paramList[0] = new OracleParameter(":cat_gp", cat_gp);
            paramList[1] = new OracleParameter(":cat_gp", cat_gp);
            paramList[2] = new OracleParameter(":reg_type", reg_type);
            paramList[3] = new OracleParameter(":cat_gp", cat_gp);

            return paramList;
        }

        private OracleParameter[] getCRM0062ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string gza_fr_date = null;
            string reg_type = null;
            string gza_to_date = null;
            string ctr_code = null;
            int index = 4;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                    case "AGFrDate":
                        gza_fr_date = item.Value.ToString();
                        break;
                    case "AGToDate":
                        gza_to_date = item.Value.ToString();
                        break;
                    case "AnnualGazetteCtrUUID":
                        ctr_code = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];
            paramList[0] = new OracleParameter(":reg_type", reg_type);
            paramList[1] = new OracleParameter(":AGFrDate", gza_fr_date);
            paramList[2] = new OracleParameter(":AGToDate", gza_to_date);
            paramList[3] = new OracleParameter(":AnnualGazetteCtrUUID", ctr_code);

            return paramList;
        }

        private OracleParameter[] getCRM0063ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string acting = null;
            string auth_uuid = null;
            string gaz_date = null;
            string reg_type = null;
            int index = 5;  //10 p

            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                    case "acting":
                        acting = item.Value.ToString();
                        break;
                    case "gaz_date":
                        gaz_date = item.Value.ToString();
                        break;
                    case "auth_uuid":
                        auth_uuid = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];
            //paramList[0] = new OracleParameter(":acting", acting);
            //paramList[1] = new OracleParameter(":gaz_date", gaz_date == null ? DateTime.Now.ToString() : gaz_date);
            //paramList[2] = new OracleParameter(":reg_type", reg_type);
            //paramList[3] = new OracleParameter(":auth_uuid", auth_uuid);
            paramList[0] = new OracleParameter(":acting", acting);
            paramList[1] = new OracleParameter(":acting", acting);
            paramList[2] = new OracleParameter(":gaz_date", gaz_date == null ? DateTime.Now.ToString() : gaz_date);
            paramList[3] = new OracleParameter(":reg_type", reg_type);
            paramList[4] = new OracleParameter(":auth_uuid", auth_uuid);

            return paramList;
        }

        private OracleParameter[] getCRM0064ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string as_at_date = null;
            string gaz_fr_date = null;
            string gaz_to_date = null;
            string reg_type = null;
            int index = 10;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "as_at_date":
                        as_at_date = item.Value.ToString();
                        break;
                    case "gaz_fr_date":
                        gaz_fr_date = item.Value.ToString();
                        break;
                    case "gaz_to_date":
                        gaz_to_date = item.Value.ToString();
                        break;
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":as_at_date", as_at_date);
            paramList[1] = new OracleParameter(":as_at_date", as_at_date);
            paramList[2] = new OracleParameter(":as_at_date", as_at_date);
            paramList[3] = new OracleParameter(":reg_type", reg_type);
            paramList[4] = new OracleParameter(":reg_type", reg_type);
            paramList[5] = new OracleParameter(":gaz_fr_date", gaz_fr_date);
            paramList[6] = new OracleParameter(":gaz_to_date", gaz_to_date);
            paramList[7] = new OracleParameter(":as_at_date", as_at_date);
            paramList[8] = new OracleParameter(":as_at_date", as_at_date);
            paramList[9] = new OracleParameter(":as_at_date", as_at_date);

            return paramList;
        }

        private OracleParameter[] getCRM0065ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string as_at_date = null;
            string gaz_fr_date = null;
            string gaz_to_date = null;
            string reg_type = null;
            string cat_gp = null;
            int index = 13;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "as_at_date":
                        as_at_date = item.Value.ToString();
                        break;
                    case "cat_gp":
                        cat_gp = item.Value.ToString();
                        break;
                    case "gaz_fr_date":
                        gaz_fr_date = item.Value.ToString();
                        break;
                    case "gaz_to_date":
                        gaz_to_date = item.Value.ToString();
                        break;
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":as_at_date", as_at_date);
            paramList[1] = new OracleParameter(":as_at_date", as_at_date);
            paramList[2] = new OracleParameter(":as_at_date", as_at_date);
            paramList[3] = new OracleParameter(":gaz_fr_date", gaz_fr_date);
            paramList[4] = new OracleParameter(":gaz_to_date", gaz_to_date);

            paramList[5] = new OracleParameter(":reg_type", reg_type);
            paramList[6] = new OracleParameter(":reg_type", reg_type);
            paramList[7] = new OracleParameter(":cat_gp", cat_gp);
            paramList[8] = new OracleParameter(":gaz_fr_date", gaz_fr_date);
            paramList[9] = new OracleParameter(":gaz_to_date", gaz_to_date);
            paramList[10] = new OracleParameter(":as_at_date", as_at_date);
            paramList[11] = new OracleParameter(":as_at_date", as_at_date);
            paramList[12] = new OracleParameter(":as_at_date", as_at_date);

            return paramList;
        }

        private OracleParameter[] getCRM0066ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string expiry_fr_date = null;
            string expiry_to_date = null;
            string order_name = null;
            string reg_type = null;
            string cat_group = null;
            int index = 7;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "ExpiryFrDate":
                        expiry_fr_date = item.Value.ToString();
                        break;
                    case "ExpiryToDate":
                        expiry_to_date = item.Value.ToString();
                        break;
                    case "CategoryGroup":
                        cat_group = item.Value.ToString();
                        break;
                    case "order":
                        order_name = item.Value.ToString();
                        break;
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":ExpiryFrDate", expiry_fr_date);
            paramList[1] = new OracleParameter(":ExpiryToDate", expiry_to_date);
            paramList[2] = new OracleParameter(":reg_type", reg_type);
            paramList[3] = new OracleParameter(":ExpiryFrDate", expiry_fr_date);
            paramList[4] = new OracleParameter(":ExpiryToDate", expiry_to_date);

            paramList[5] = new OracleParameter(":CategoryGroup", cat_group);
            paramList[6] = new OracleParameter(":order_name", order_name);

            return paramList;
        }

        private OracleParameter[] getCRM0071ParamList(Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            string expiry_fr_date = null;
            string expiry_to_date = null;
            string order_name = null;
            string reg_type = null;
            string cat_group = null;
            int index = 8;  //10 p
            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                switch (item.Key)
                {
                    case "Expired_fr_date":
                        expiry_fr_date = item.Value.ToString();
                        break;
                    case "Expired_to_date":
                        expiry_to_date = item.Value.ToString();
                        break;
                    case "ExpiredCtrUUID":
                        cat_group = item.Value.ToString();
                        break;
                    case "order_name":
                        order_name = item.Value.ToString();
                        break;
                    case "reg_type":
                        reg_type = item.Value.ToString();
                        break;
                }
            }

            paramList = new OracleParameter[index];

            paramList[0] = new OracleParameter(":expiry_fr_date", expiry_fr_date);
            paramList[1] = new OracleParameter(":expiry_to_date", expiry_to_date);
            paramList[2] = new OracleParameter(":reg_type", reg_type);
            paramList[3] = new OracleParameter(":reg_type", reg_type);
            paramList[4] = new OracleParameter(":cat_group", cat_group);

            paramList[5] = new OracleParameter(":expiry_fr_date", expiry_fr_date);
            paramList[6] = new OracleParameter(":expiry_to_date", expiry_to_date);


            paramList[7] = new OracleParameter(":order_name", order_name);

            return paramList;
        }

        private OracleParameter[] getSpecParamList(string rptId, Dictionary<string, Object> myDictionary)
        {
            OracleParameter[] paramList = null;
            switch (rptId)
            {
                case "CRM0004":
                    paramList = getCRM0004ParamList(myDictionary);
                    break;
                case "CRM0006":
                    paramList = getCRM0006ParamList(myDictionary);
                    break;
                case "CRM0009":
                    paramList = getCRM0009ParamList(myDictionary);
                    break;
                case "CRM0022":
                    paramList = getCRM0022ParamList(myDictionary);
                    break;
                case "CRM0029":
                    paramList = getCRM0029ParamList(myDictionary);
                    break;
                case "CRM0032":
                    paramList = getCRM0032ParamList(myDictionary);
                    break;
                case "CRM0046":
                    paramList = getCRM0046ParamList(myDictionary);
                    break;
                case "CRM0055":
                    paramList = getCRM0055ParamList(myDictionary);
                    break;
                case "CRM0056":
                    paramList = getCRM0056ParamList(myDictionary);
                    break;
                case "CRM0059":
                    paramList = getCRM0059ParamList(myDictionary);
                    break;
                case "CRM0060":
                    paramList = getCRM0060ParamList(myDictionary);
                    break;
                case "CRM0062":
                    paramList = getCRM0062ParamList(myDictionary);
                    break;
                case "CRM0063":
                    paramList = getCRM0063ParamList(myDictionary);
                    break;
                //case "CRM0064":
                //    paramList = getCRM0064ParamList(myDictionary);
                //    break;
                case "CRM0065":
                    paramList = getCRM0065ParamList(myDictionary);
                    break;
                case "CRM0066":
                    paramList = getCRM0066ParamList(myDictionary);
                    break;
                case "CRM0071":
                    paramList = getCRM0071ParamList(myDictionary);
                    break;
            }

            return paramList;
        }


        private void LoadCrystalReports(Dictionary<string, Object> myDictionary)
        {
            CrystalReportViewer1.HasToggleGroupTreeButton = false;
            int myFOpts = (int)(
            //CrystalDecisions.Shared.ViewerExportFormats.RptFormat |
            CrystalDecisions.Shared.ViewerExportFormats.PdfFormat |
            //CrystalDecisions.Shared.ViewerExportFormats.RptrFormat |
            CrystalDecisions.Shared.ViewerExportFormats.XLSXFormat |
            //CrystalDecisions.Shared.ViewerExportFormats.CsvFormat |
            //CrystalDecisions.Shared.ViewerExportFormats.EditableRtfFormat |
            //CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat |
            //CrystalDecisions.Shared.ViewerExportFormats.RtfFormat |
            //CrystalDecisions.Shared.ViewerExportFormats.WordFormat |
            //CrystalDecisions.Shared.ViewerExportFormats.XmlFormat |
            CrystalDecisions.Shared.ViewerExportFormats.ExcelFormat |
            CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

            CrystalReportViewer1.AllowedExportFormats = myFOpts;

            //CrystalReportViewer1.DisplayToolbar = false;
            ConnDb conn = new ConnDb();
            ReportDocument rd = new ReportDocument();

            DataSet dtTmp = new DataSet();
            DataSet dtSub = new DataSet();

            string sql = null;
            string rptPath = null;
            string reportName = "";
            List<string> sqlList = new List<string>();
            string rptId = null;
            Dictionary<string, Object> specDictionary = new Dictionary<string, Object>();

            int index = 0;
            OracleParameter[] paramList = null;

            if (myDictionary.Keys.Contains("REPORT_NAME"))
                reportName = myDictionary["REPORT_NAME"].ToString();


            if (myDictionary.Count > 1)
            {
                paramList = new OracleParameter[myDictionary.Count - 1];
            }

            Object sValue = null;
            if (myDictionary.TryGetValue("rptId", out sValue))
            {
                rptId = sValue.ToString();
                rptPath = getRptPath(rptId);
                if (CheckListSqlRptId(rptId))
                    sqlList = getCorListSqlByRptId(rptId, myDictionary);
                else
                    sql = getCorSqlByRptId(rptId, myDictionary);     //根据ID获取对应的sql
                //sql = getCorSqlByRptId(rptId, myDictionary)
            }
            else
            {
                if (myDictionary.TryGetValue("REPORT_NAME", out sValue))
                {
                    rptId = sValue.ToString();
                    rptPath = getRptPath(rptId);
                    sql = getCorSqlByRptId(rptId);     //根据ID获取对应的sql
                }

            }

            foreach (KeyValuePair<string, object> item in myDictionary)
            {
                if (item.Key != "rptId")
                {
                    //item.Key.Replace("amp;", "");
                    if (rptId != null && checkSpecRptId(rptId))
                    {
                        specDictionary.Add(item.Key, item.Value.ToString());
                    }
                    else
                    {
                        if (index < myDictionary.Count - 1)
                        {
                            paramList[index] = new OracleParameter(":" + item.Key, item.Value.ToString());
                            index++;
                        }
                    }
                }
            }

            if (rptId != null && checkSpecRptId(rptId))
            {
                paramList = getSpecParamList(rptId, specDictionary);
            }

            /*
            if(rptId != null && checkSubRptId(rptId))
            {
                string subSql = getSubReportSql(rptId);
                string subRptId = getSubReportId(rptId);
                OracleParameter[] subReportParamList = null;

                if (myDictionary.Count > 1)
                {
                    subReportParamList = new OracleParameter[myDictionary.Count - 1];
                }
                subReportParamList = getSpecParamList(subRptId, specDictionary);
                dtSub = conn.ExecuteDataSet(subSql, subReportParamList);
                rd.Subreports["RPT0008_IP.rpt"].SetDataSource(dtSub);
            }
            */
            if (myDictionary.Count > 1)
            {
                if (CheckListSqlRptId(rptId))
                    dtTmp = conn.queryListSql(sqlList);
                else
                    dtTmp = conn.ExecuteDataSet(sql, paramList);
            }
            else
            {
                if (CheckListSqlRptId(rptId))
                    dtTmp = conn.queryListSql(sqlList);
                else
                    dtTmp = conn.query(sql);
                //dtTmp = conn.query(sql); 
            }
            if (dtTmp.Tables[0].Rows.Count < 1 && CheckEmptyRecord(rptId))
            {
                dtTmp = conn.query(getCorSqlByRptId(rptId + "E", myDictionary));
            }

            //dtTmp.Tables[0].TableName = "DataTable1";
            //dtTmp.Tables[1].TableName = "DAtaTable2";

            DataTable dataTable = dtTmp.Tables[0];
            string reportPath = Server.MapPath(rptPath);
            rd.Load(reportPath);
            if (rptId == "CRM0009" && dataTable.Rows.Count == 0)
            {
                DataRow dr = dataTable.NewRow();
                dr["ENAME"] = "No Data";
                dr["P"] = "0";
                dr["A"] = "0";
                dr["MONTH"] = myDictionary["AttendancMonth"];
                dr["YEAR"] = myDictionary["AttendancYear"];
                dataTable.Rows.Add(dr);
            }
            if (CheckListSqlRptId(rptId))
            {
                if (rptId == "CRM0088")
                {
                    //rd.SetDataSource(dtTmp);
                    rd.Subreports[0].SetDataSource(dtTmp.Tables[0]);
                    rd.Subreports[1].SetDataSource(dtTmp.Tables[1]);
                    rd.Subreports[2].SetDataSource(dtTmp.Tables[2]);
                    rd.Subreports[3].SetDataSource(dtTmp.Tables[3]);
                }
                if (rptId == "CRM0069")
                {

                    rd.SetDataSource(dtTmp.Tables[1]);
                    rd.Subreports[0].SetDataSource(dtTmp.Tables[0]);

                    //rd.SetDataSource(dtTmp);

                }
            }
            else
            {
                if (rptId == "CRM0024" && dataTable.Rows.Count == 0)
                {
                    DataRow dr = dataTable.NewRow();
                    dr["COUNT_TYPE"] = "No Data";
                    dr["AS_AT_TODAY"] = Convert.ToDateTime(myDictionary["QpAsAT"]).ToString("MM/yyyy");
                    dr["WILLINGNESS_QP"] = myDictionary["CountForWillingnessQp"];
                    dr["YES"] = "0";
                    dr["NO"] = "0";
                    dr["NO_INDICATION"] = "0";
                    dataTable.Rows.Add(dr);
                }
                if (rptId == "CRM0032" && dataTable.Rows.Count == 0)
                {
                    DataRow dr = dataTable.NewRow();
                    dr["HDR2"] = "No Data";
                    dr["UP_TO_TODAY"] = DateTime.Now.ToString("dd/MM/yyyy");
                    dataTable.Rows.Add(dr);
                }
                rd.SetDataSource(dataTable);
            }
            //rd.Subreports[0].SetDataSource(dt);
            //rd.FileName = rptId;
            CrystalReportViewer1.ReportSource = rd;
            CrystalReportViewer1.ID = reportName;
            if (checkExcelRptId(rptId))
            {
                NPOIExcel myhelper = new NPOIExcel();

                string fileName = getExcelFileName(rptId);
                byte[] data = myhelper.DataTable2Excel(dataTable, "sheet", fileName, CRM0037Title);
            }
        }

        public class NPOIExcel
        {
            //最大数据条数
            readonly int EXCEL03_MaxRow = 65535;
            /// <summary>
            /// 将DataTable转换为excel2003格式。
            /// </summary>
            /// <param name="dt"></param>
            /// <returns></returns>
            public byte[] DataTable2Excel(DataTable dt, string sheetName, string fileName, string[] titleList)
            {

                IWorkbook book = new HSSFWorkbook();
                if (dt.Rows.Count < EXCEL03_MaxRow)
                    DataWrite2Sheet(dt, 0, dt.Rows.Count - 1, book, sheetName, titleList);
                else
                {
                    int page = dt.Rows.Count / EXCEL03_MaxRow;
                    for (int i = 0; i < page; i++)
                    {
                        int start = i * EXCEL03_MaxRow;
                        int end = (i * EXCEL03_MaxRow) + EXCEL03_MaxRow - 1;
                        DataWrite2Sheet(dt, start, end, book, sheetName + i.ToString(), titleList);
                    }
                    int lastPageItemCount = dt.Rows.Count % EXCEL03_MaxRow;
                    DataWrite2Sheet(dt, dt.Rows.Count - lastPageItemCount, lastPageItemCount, book, sheetName + page.ToString(), titleList);
                }
                MemoryStream ms = new MemoryStream();

                HttpResponse httpResponse = HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.Buffer = true;
                httpResponse.Charset = Encoding.UTF8.BodyName;
                httpResponse.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
                httpResponse.ContentEncoding = Encoding.UTF8;
                httpResponse.ContentType = "application/vnd.ms-excel; charset=UTF-8";
                book.Write(httpResponse.OutputStream);
                httpResponse.End();
                book.Write(ms);
                return ms.ToArray();
            }

            private void DataWrite2Sheet(DataTable dt, int startRow, int endRow, IWorkbook book, string sheetName, string[] titleList)
            {
                ISheet sheet = book.CreateSheet(sheetName);
                IRow header = sheet.CreateRow(0);
                //title
                for (int i = 0; i < titleList.Length; i++)
                {
                    sheet.SetColumnWidth(i, 4500);
                    ICell cell = header.CreateCell(i);
                    string val = titleList[i].ToString();
                    cell.SetCellValue(val);
                }
                int rowIndex = 1;
                //contents
                for (int i = startRow; i <= endRow; i++)
                {
                    DataRow dtRow = dt.Rows[i];
                    IRow excelRow = sheet.CreateRow(rowIndex++);
                    for (int j = 0; j < dtRow.ItemArray.Length; j++)
                    {
                        excelRow.CreateCell(j).SetCellValue(dtRow[j].ToString());
                    }
                }
            }

        }

        private string getSubReportId(string rptId)
        {
            string subReportId = null;
            switch (rptId)
            {
                case "CRM0069":
                    //sub report CRM0070
                    subReportId = "CRM0070";
                    break;
            }
            return subReportId;
        }

        private string getSubReportSql(string rptId)
        {
            string subReportSql = null;
            switch (rptId)
            {
                case "CRM0069":
                    //sub report CRM0070
                    //subReportSql = getRPT0008IP();
                    break;
            }
            return subReportSql;
        }

        // if need to add report,should add in this method
        private string getCorSqlByRptId(string rptId, Dictionary<string, Object> myDictionary = null)
        {
            string sql = null;
            switch (rptId)
            {
                case "CRM0001":
                    sql = get10DayPledgeCGCSql();
                    break;
                case "CRM0002":
                    sql = getAdminRptCommitteeTypeSql();
                    break;
                case "CRM0003":
                    sql = getAdminRptPanelTypeSql();
                    break;
                case "CRM0004":
                    sql = getAnnualGazetteCHNCGCSql(myDictionary);
                    break;
                case "CRM0005":
                    sql = getExpiryContractorCGCSql();
                    break;
                case "CRM0006":
                    sql = getApplicantInfoCMWOrgSql();
                    break;
                case "CRM0007":
                    sql = getApplicationCountCMWSql();
                    break;
                case "CRM0008":
                    sql = getApplicationCountIPSql(myDictionary);
                    break;
                case "CRM0009":
                    sql = getAttendanceMonthSql();
                    break;
                case "CRM0010":
                    sql = getAuditLog(myDictionary);
                    break;
                case "CRM0011":
                    sql = getCMwcSql();
                    break;
                case "CRM0012":
                    sql = getCMwcWSql();
                    break;
                case "CRM0013":
                    sql = getCQpMwcSql();
                    break;
                case "CRM0014":
                    sql = getCaseInHandSql(myDictionary);
                    break;
                case "CRM0015":
                    sql = getEMwcWSql();
                    break;
                case "CRM0016":
                    sql = getEQpMwcWSql();
                    break;
                case "CRM0017":
                    sql = getExpiryRegistersIPSql();
                    break;
                case "CRM0018":
                    sql = getCountExpiryDateQpSql();
                    break;
                case "CRM0019":
                    sql = getCountIrSql();
                    break;
                case "CRM0020":
                    sql = getCountIssueDateQpCardAll();
                    break;
                case "CRM0021":
                    sql = getCountIssuedQpCardAll();
                    break;
                case "CRM0022":
                    sql = getCountQpSql();
                    break;
                case "CRM0023":
                    sql = getCountQpAllSql();
                    break;
                case "CRM0024":
                    sql = getCountQpAllHistorySql(myDictionary);
                    break;
                case "CRM0025":
                    sql = getCountQpAllHistoryXlsSql();
                    break;
                case "CRM0026":
                    sql = getCountQpXlsSql();
                    break;
                case "CRM0027":
                    sql = getCountReturnDateQpCardAll();
                    break;
                case "CRM0028":
                    sql = getCountRISql(myDictionary);
                    break;
                case "CRM0029":
                    sql = getAnnualGazetteENGIPSql(myDictionary);
                    break;
                case "CRM0030":
                    sql = getCheckPRSSql(myDictionary);
                    break;
                case "CRM0031":
                    sql = getCheckPRSXlsSql();
                    break;
                case "CRM0032":
                    sql = getConvictionCGCSql(myDictionary);
                    break;
                case "CRM0033":
                    sql = getConvictionIP(myDictionary);
                    break;
                case "CRM0034":
                    sql = getConvictionIP2();
                    break;
                case "CRM0035":
                    sql = getConvictionIP3();
                    break;
                case "CRM0036":
                    sql = getFastTrackCGCSql();
                    break;
                case "CRM0037":
                    sql = getExportAPRSERGEInformationIPSql();
                    break;
                case "CRM0038":
                    sql = getInterestedQpStatisticSql();
                    break;
                case "CRM0039":
                    sql = getInterestedQpStatisticXlsSql();
                    break;
                case "CRM0040":
                    sql = getMMD0003a1IPSql();
                    break;
                case "CRM0041":
                    sql = getMMD0003a4IPSql();
                    break;
                case "CRM0042":
                    sql = getMMD0003b1CGCSql();
                    break;
                case "CRM0043":
                    sql = getMMD0003b4CGCSql();
                    break;
                case "CRM0044":
                    sql = getMMD0010aIPSql();
                    break;
                case "CRM0045":
                    sql = getMMD0010bCGCSql();
                    break;
                case "CRM0046":
                    sql = getMWCPExpiryDateSql();
                    break;
                case "CRM0047":
                    sql = getMWCPExpiryDateXlsSql();
                    break;
                case "CRM0048":
                    sql = getNoOfConvictionCGCSql(myDictionary);
                    break;
                case "CRM0049":
                    sql = getNoOfConvictionCGCXlsSql();
                    break;
                case "CRM0050":
                    sql = getNoOfRegCGCSql(myDictionary);
                    break;
                case "CRM0051":
                    sql = getNoOfRegCGCXlsSql();
                    break;
                case "CRM0052":
                    sql = getNoOfRegCMWSql();
                    break;
                case "CRM0053":
                    sql = getNoOfRegCMWXlsSql();
                    break;
                case "CRM0054":
                    sql = getCGCProcessMonitor(myDictionary);
                    break;
                case "CRM0055":
                    sql = getApplicantsMonCGCIA();
                    break;
                case "CRM0056":
                    sql = getAttendancePeriod();
                    break;
                case "CRM0057":
                    sql = getApplicationCountCGC(myDictionary);
                    break;
                case "CRM0059":
                    sql = getStatusAppControlFormIP(myDictionary);
                    break;
                case "CRM0060":
                    sql = getStatusAppControlFormIP_RI();
                    break;
                case "CRM0061":
                    sql = getNoOfRegIP(myDictionary);
                    break;
                case "CRM0062":
                    sql = getRPT0010bCGC(myDictionary);
                    break;
                case "CRM0063":
                    sql = getRPT0010bA2CGC(myDictionary);
                    break;
                case "CRM0064":
                    sql = getAnnualGazetteENGCGC(myDictionary);
                    break;
                case "CRM0065":
                    sql = getAnnualGazetteCHNIPSql(myDictionary);
                    break;
                case "CRM0066":
                    sql = getRPT0007bCGC(myDictionary);
                    break;
                case "CRM0068":
                    sql = getRPT0004CGC(myDictionary);
                    break;
                //case "CRM0069":
                //    sql = getRPT0008CGCIP(myDictionary);
                //    break;
                case "CRM0070":
                    sql = getRPT0008IP(myDictionary);
                    break;
                case "CRM0071":
                    sql = getRPT0007aIP(myDictionary);
                    break;
                case "CRM0073":
                    sql = getAnnualGazetteChnMWC(myDictionary);
                    break;
                case "CRM0074":
                    sql = getProcessMonitorMWC();
                    break;
                case "CRM0076":
                    sql = getRPT0004MWC(myDictionary);
                    break;
                case "CRM0078":
                    sql = getRPT0010bA2CGC(myDictionary);
                    break;
                case "CRM0079":
                    sql = getRPT0002IMWSql();
                    break;
                case "CRM0080":
                    sql = getRPT0004_IMW(myDictionary);
                    break;
                case "CRM0085":
                    sql = getProgMWCReg(myDictionary);
                    break;
                case "CRM0087":
                    sql = getStatusAppControlFormIMW();
                    break;
                //case "CRM0088":
                //    sql = getRPT002_CGC_IP();
                //    break;
                case "CRM0089":
                    sql = getCountForFSSsql(myDictionary);
                    break;
                case "CRM0090":
                    sql = getCountForMBISsql(myDictionary);
                    break;
                case "CRM0091":
                    sql = getCheckMultiReg();
                    break;
                case "CRM0092":
                    sql = getRPT0002IMW();
                    break;
                case "CRM0093":
                    sql = getRPT0002_CMW();
                    break;
                case "CRM0094":
                    sql = getRPT0006CGC();
                    break;
                case "CRM0095":
                    sql = getSummryReportForApplicationStatusIMWSql();
                    break;
                case "CRM0064E":
                    sql = getAnnualGazetteENGCGCEmptySql(myDictionary);
                    break;
                case "CRM0004E":
                    sql = getAnnualGazetteCHNCGCEmtpySql(myDictionary);
                    break;
                case "CRM0029E":
                    sql = getAnnualGazetteENGIPEmptySql(myDictionary);
                    break;
                case "CRM0065E":
                    sql = getAnnualGazetteCHNIPEmptySql(myDictionary);
                    break;
                case "CRM0073E":
                    sql = getAnnualGazetteChnMWCEmptySql(myDictionary);
                    break;

            }
            return sql;
        }

        public List<string> getCorListSqlByRptId(string rptId, Dictionary<string, Object> myDictionary = null)
        {
            List<string> sql = new List<string>();
            switch (rptId)
            {
                case "CRM0069":
                    sql = getRPT0008CGCIP(myDictionary);
                    break;
                case "CRM0088":
                    sql = getRPT002_CGC_IP(myDictionary);
                    break;
            }
            return sql;
        }


        //get report SQL
        //CRM0001
        private string get10DayPledgeCGCSql()
        {
            string sql = "SELECT C_APPL.ENGLISH_COMPANY_NAME AS ENAME," +
                " C_APPL.FILE_REFERENCE_NO AS FILE_REF," +
                "C_PMON.NATURE AS NATURE," +
                "to_char(C_PMON.RECEIVED_DATE,'dd/mm/yyyy'） AS RECEIVED_D," +
                "C_PMON.VETTING_OFFICER AS VO," +
                "to_char(C_PMON.PLEDGE_DUE_10_DAYS_DATE,'dd/mm/yyyy'） AS PLEDGE_DUE_10_DAYS_D," +
                "C_PMON.INITIAL_REPLY AS INIT_REPLY," +
                "C_PMON.REMARKS AS REMARKS," +
                "to_char(C_PMON.PLEDGE_INITIAL_DATE,'dd/mm/yyyy'） AS PLEDGE_INITIAL_D," +
                "C_PMON.NO_WORKING_DAYS_DATE AS NO_WORKING_DAYS," +
                "C_PMON.ASSISTANT AS ASSISTANT," +
                "to_char(to_date(:today,'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_AT_TODAY " +
                "FROM C_COMP_APPLICATION C_APPL " +
                "INNER JOIN C_COMP_PROCESS_MONITOR C_PMON ON C_APPL.UUID = C_PMON.MASTER_ID " +
                "WHERE C_PMON.MONITOR_TYPE = 'UPM_10DAYS' and C_APPL.REGISTRATION_TYPE =:reg_type " +
                "ORDER BY FILE_REF ASC,ENAME ASC ";
            return sql;
        }

        //CRM0002
        private string getAdminRptCommitteeTypeSql()
        {



            string sql = "select  c_mem.post as \"post title\"," +
     "appln.surname || ' ' || appln.given_name_on_id as \"name in english\"," +
     "appln.gender as gender," +
     "appln.chinese_name as \"name in chinese\"," +
     "to_char(c_pnl_mem.expiry_date,'dd/mm/yyyy') as \"expiry date of present term\"," +
     "to_char(c_mem.created_date,'dd/mm/yyyy') as \"start date of present term\"," +
     "s_society.english_description as \"nominated/elected by\"," +
     "to_char(( select min(a.created_date) from c_committee_member a where a.applicant_id = applicant_id ),'dd/mm/yyyy') as \"start date of 1st term\"," +
     "s_committee_type.english_description as s_committee_type_eng," +
     "s_pnl_role.english_description as \"position held\"" +
"from " +
     "c_committee_member c_mem " +
     "inner join c_committee_panel_member c_pnl_mem on c_mem.uuid = c_pnl_mem.member_id " +
     "inner join c_committee_member_institute c_mem_int on c_mem.uuid = c_mem_int.member_id " +
     "inner join c_applicant appln on c_mem.applicant_id = appln.uuid " +
     "inner join c_member_category mem_cat on c_mem.uuid = mem_cat.member_id " +
     "inner join c_s_system_value s_committee_type on mem_cat.committee_type_id = s_committee_type.uuid " +
     "inner join c_s_system_value s_society on c_mem_int.society_id = s_society.uuid " +
     "inner join c_s_system_value s_pnl_role on c_pnl_mem.panel_role_id = s_pnl_role.uuid " +
"where " +
     "s_committee_type.code = :committee_type_code " +
  "and to_char(c_pnl_mem.expiry_date,'yyyymmdd') >= to_char(sysdate, 'yyyymmdd') ";
            return sql;
        }

        //CRM0003
        private string getAdminRptPanelTypeSql()
        {
            string sql = "SELECT  C_MEM.POST AS C_MEM_POST," +
     "APPLN.SURNAME || ' ' || APPLN.GIVEN_NAME_ON_ID AS ENAME," +
     "APPLN.GENDER AS GENDER," +
     "APPLN.CHINESE_NAME AS CNAME," +
     "to_char(C_PNL_MEM.EXPIRY_DATE,'dd/mm/yyyy') AS C_MEM_EXP_D," +
     "to_char(C_MEM.CREATED_DATE,'dd/mm/yyyy') AS C_MEM_CREATED_D," +
     "S_SOCIETY.ENGLISH_DESCRIPTION AS INT_ENAME," +
     "to_char(( SELECT MIN(A.CREATED_DATE) FROM C_COMMITTEE_MEMBER A WHERE A.APPLICANT_ID = APPLICANT_ID ),'dd/mm/yyyy') AS FIRST_D," +
     "S_PNL_TYPE.ENGLISH_DESCRIPTION AS PNL_TYPE_ENG," +
     "S_PNL_ROLE.ENGLISH_DESCRIPTION AS PNL_ROLE_ENAME " +
"FROM " +
     "C_COMMITTEE_MEMBER C_MEM " +
     "INNER JOIN C_COMMITTEE_PANEL_MEMBER C_PNL_MEM ON C_MEM.UUID = C_PNL_MEM.MEMBER_ID " +
     "INNER JOIN C_COMMITTEE_PANEL C_PNL ON C_PNL_MEM.COMMITTEE_PANEL_ID = C_PNL.UUID " +
     "INNER JOIN C_S_SYSTEM_VALUE S_PNL_ROLE ON C_PNL_MEM.PANEL_ROLE_ID = S_PNL_ROLE.UUID " +
     "INNER JOIN C_S_SYSTEM_VALUE S_PNL_TYPE ON C_PNL.PANEL_TYPE_ID = S_PNL_TYPE.UUID " +
     "INNER JOIN C_COMMITTEE_MEMBER_INSTITUTE C_MEM_INT ON C_MEM.UUID = C_MEM_INT.MEMBER_ID " +
     "INNER JOIN C_APPLICANT APPLN ON C_MEM.APPLICANT_ID = APPLN.UUID " +
     "INNER JOIN C_S_SYSTEM_VALUE S_SOCIETY ON C_MEM_INT.SOCIETY_ID = S_SOCIETY.UUID " +
"WHERE " +
     "S_PNL_TYPE.CODE =:panel_type_code " +
  "AND to_char(C_PNL_MEM.EXPIRY_DATE,'yyyymmdd') >= to_char(sysdate, 'yyyymmdd') ";
            return sql;
        }

        //CRM0004
        private string getAnnualGazetteCHNCGCSql(Dictionary<string, Object> myDictionary)
        {

            string header1 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                        myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None).ToList<string>()[0]
                        : myDictionary["HeaderChnDesc"].ToString();
            string header2 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                        myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None).ToList<string>()[1]
                        : "";
            string sql = "SELECT " +
                "upper(C_APPL.ENGLISH_COMPANY_NAME) AS ENAME," +
                "C_APPL.CHINESE_COMPANY_NAME AS CNAME," +
                "to_char(C_APPL.EXPIRY_DATE,'dd.mm.yyyy') AS EXP_D," +
                "C_APPL.CERTIFICATION_NO AS CERT_NO," +
                "S_CAT.CHINESE_DESCRIPTION AS CHN_SUB," +
                "S_CAT.ENGLISH_DESCRIPTION AS ENG_SUB," +
                "S_CAT.CODE AS CAT_CODE," +
                "(select MIN(CHINESE_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_NAME," +
                "(select MIN(ENGLISH_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_ENAME," +
                "CASE WHEN to_date(:asAtDate2, 'dd/mm/yyyy') > C_APPL.expiry_date " +
                "AND add_months(C_APPL.retention_application_date, 24) > trunc(to_date(:asAtDate2, 'dd/mm/yyyy')) THEN '@' ELSE '' END AS FLAG," +
                "'" + header1 + "' AS HEADER1," +
                "'" + header2 + "'  AS HEADER2," +
                "to_date(:asAtDate2,'dd/mm/yyyy') AS AS_AT_DATE " +
                "FROM C_S_CATEGORY_CODE S_CAT " +
                "INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE S_CAT.REGISTRATION_TYPE =:reg_type " +
                "AND S_CAT_GP.REGISTRATION_TYPE =:reg_type " +
                "AND S_CAT_GP.Code = :Chn_cat_gp " +
                "AND C_APPL.GAZETTE_DATE >= to_date(:ReportChnGazDateFrom, 'dd/mm/yyyy') " +
                "AND C_APPL.GAZETTE_DATE <= to_date(:ReportChnGazDateTo, 'dd/mm/yyyy') " +
                "AND C_APPL.CERTIFICATION_NO IS NOT NULL AND(" +
                "(C_APPL.EXPIRY_DATE IS NOT NULL and C_APPL.EXPIRY_DATE >= to_date(:asAtDate2, 'dd/mm/yyyy')) or ((C_APPL.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and " +
                "(C_APPL.EXPIRY_DATE < to_date(:asAtDate2, 'dd/mm/yyyy'))) ) )  AND((C_APPL.REMOVAL_DATE IS NULL) OR " +
                "(C_APPL.REMOVAL_DATE > to_date(:asAtDate2, 'dd/mm/yyyy'))) " +
                "AND S_CAT_GP.CODE IN('GB','SC') " +
                "ORDER BY ENG_SUB ASC,ENAME ASC ";
            return sql;
        }

        public string getAnnualGazetteCHNCGCEmtpySql(Dictionary<string, Object> myDictionary)
        {
            string header1 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                        myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None).ToList<string>()[0]
                        : myDictionary["HeaderChnDesc"].ToString();
            string header2 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None).ToList<string>()[1]
                            : "";
            string sql = "SELECT " +
                "null ENAME," +
                "null AS CNAME," +
                "null AS EXP_D," +
                "null AS CERT_NO," +
                "null AS CHN_SUB," +
                "null AS ENG_SUB," +
                "null AS CAT_CODE," +
                "null as AUTH_NAME," +
                "null as AUTH_ENAME," +
                "null AS FLAG," +
                "'" + header1 + "' AS HEADER1," +
                "'" + header2 + "'  AS HEADER2," +
                "to_date('" + myDictionary["asAtDate2"].ToString() + "','dd/mm/yyyy') AS AS_AT_DATE from dual ";
            return sql;
        }
        //CRM0005
        private string getExpiryContractorCGCSql()
        {
            string sql = "SELECT SYS_VAL1.CODE AS CAT_GP_CODE,S_CAT.CODE AS CAT_CODE,C_APPL.UUID AS MASTER_ID," +
                "CASE " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '01' THEN '01-January' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '02' THEN '02-February' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '03' THEN '03-March' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '04' THEN '04-April' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '05' THEN '05-May' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '06' THEN '06-June' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '07' THEN '07-July' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '08' THEN '08-August' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '09' THEN '09-September' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '10' THEN '10-October' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '11' THEN '11-November' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_APPL.EXPIRY_DATE, 'MM') = '12' THEN '12-December' || ' ' || to_char(C_APPL.EXPIRY_DATE, 'YYYY') " +
                "END AS EXP_MY, " +
                "to_char(C_APPL.EXPIRY_DATE, 'YYYY') AS EXP_Y, " +
                " '(as at ' || " +
                "CASE WHEN substr(:today,0,2) = '01' THEN '1' " +
                "WHEN substr(:today,0,2) = '02' THEN '2' " +
                "WHEN substr(:today,0,2) = '03' THEN '3' " +
                "WHEN substr(:today,0,2) = '04' THEN '4' " +
                "WHEN substr(:today,0,2) = '05' THEN '5' " +
                "WHEN substr(:today,0,2) = '06' THEN '6' " +
                "WHEN substr(:today,0,2) = '07' THEN '7' " +
                "WHEN substr(:today,0,2) = '08' THEN '8' " +
                "WHEN substr(:today,0,2) = '09' THEN '9' " +
                "ELSE substr(:today,0,2) END || ' ' || " +
                "CASE " +
                "WHEN substr(:today,3,2) = '01' THEN 'January' " +
                "WHEN substr(:today,3,2) = '02' THEN 'February' " +
                "WHEN substr(:today,3,2) = '03' THEN 'March' " +
                "WHEN substr(:today,3,2) = '04' THEN 'April' " +
                "WHEN substr(:today,3,2) = '05' THEN 'May' " +
                "WHEN substr(:today,3,2) = '06' THEN 'June' " +
                "WHEN substr(:today,3,2) = '07' THEN 'July' " +
                "WHEN substr(:today,3,2) = '08' THEN 'August' " +
                "WHEN substr(:today,3,2) = '09' THEN 'September' " +
                "WHEN substr(:today,3,2) = '10' THEN 'October' " +
                "WHEN substr(:today,3,2) = '11' THEN 'November' " +
                "ELSE 'December' END || ' ' || substr(:today, 5, 7) || ' )' AS AS_AT_TODAY " +
                "FROM C_COMP_APPLICATION C_APPL INNER JOIN C_S_CATEGORY_CODE S_CAT ON C_APPL.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE SYS_VAL1 ON S_CAT.CATEGORY_GROUP_ID = SYS_VAL1.UUID " +
                "INNER JOIN C_S_SYSTEM_TYPE SYS_TYPE1 ON SYS_VAL1.SYSTEM_TYPE_ID = SYS_TYPE1.UUID " +
                "WHERE SYS_VAL1.REGISTRATION_TYPE = :reg_type AND SYS_TYPE1.TYPE = 'CATEGORY_GROUP' " +
                "AND to_char(C_APPL.EXPIRY_DATE,'yyyymmdd') >= to_char(sysdate, 'yyyymmdd') " +
                "and(C_APPL.REMOVAL_DATE is null or to_char(C_APPL.REMOVAL_DATE, 'yyyymmdd') > to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd')) " +
                "ORDER BY EXP_Y DESC, EXP_MY ";
            return sql;
        }

        //CRM0006
        public string getApplicantInfoCMWOrgSql()
        {
            string sql = "SELECT '1I'AS TYPE," +
                " 'IMW'AS REG_TYPE," +
                " 'Registration History (RMWC (Individual))' as TYPE_DESC," +
                " ''AS MW_CLASS_CODE," +
                "''AS MW_TYPE_CODE," +
                "''AS SRC_START_TIME," +
                "I_APPL.FILE_REFERENCE_NO AS FILE_REF," +
                "''AS SRC_INTERV_NO," +
                "c_propercase(APPLN.SURNAME)|| ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS C_ENAME," +
                "C_MWI_CONCAT_MW_ITEM(I_APPL.UUID) AS ROLE_CODE," +
                "TO_CHAR(I_PMON.RESULT_ACCEPT_DATE, 'DD/MM/YYYY') AS ACCEPT_D," +
                "TO_CHAR(I_CERT.REMOVAL_DATE, 'DD/MM/YYYY') AS REMOVAL_D," +
                "CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID," +
                "c_propercase(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME," +
                "''AS SRC_RESULT," +
                "''AS SRC_RESULT_DT," +
                "(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = :c_appln_id)  AS THIS_FILE_REF," +
                ":file_ref_no AS FILE_REF_NO " +
                "FROM " +
                "C_IND_APPLICATION I_APPL " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "INNER JOIN C_APPLICANT APPLN ON I_APPL.APPLICANT_ID = APPLN.UUID " +
                "LEFT JOIN C_IND_PROCESS_MONITOR I_PMON ON I_APPL.UUID = I_PMON.MASTER_ID " +
                "WHERE I_APPL.REGISTRATION_TYPE = 'IMW' " +
                "and APPLN.UUID = :appln_id " +
                "and(I_CERT.REMOVAL_DATE IS NOT NULL OR I_PMON.RESULT_ACCEPT_DATE IS NOT NULL) " +

                "UNION " +

                "SELECT '3I'AS TYPE," +
                "'IMW'AS REG_TYPE, 'Proposed (RMWC (Individual))' as TYPE_DESC, ''AS MW_CLASS_CODE, ''AS MW_TYPE_CODE," +
                "''AS SRC_START_TIME, I_APPL.FILE_REFERENCE_NO AS FILE_REF, ''AS SRC_INTERV_NO," +
                "c_propercase(APPLN.SURNAME)|| ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS C_ENAME," +
                "C_MWI_CONCAT_MW_ITEM(I_APPL.UUID)AS ROLE_CODE,TO_CHAR(I_PMON.RESULT_ACCEPT_DATE, 'DD/MM/YYYY') AS ACCEPT_D," +
                "TO_CHAR(I_CERT.REMOVAL_DATE, 'DD/MM/YYYY') AS REMOVAL_D," +
                "CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID," +
                "c_propercase(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME," +
                "''AS SRC_RESULT,''AS SRC_RESULT_DT," +
                "(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = :c_appln_id)  AS THIS_FILE_REF," +
                ":file_ref_no AS FILE_REF_NO " +
                "FROM " +
                "C_IND_APPLICATION I_APPL " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "INNER JOIN C_APPLICANT APPLN ON I_APPL.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_STATUS ON I_CERT.APPLICATION_STATUS_ID = S_STATUS.UUID " +
                "LEFT JOIN C_IND_PROCESS_MONITOR I_PMON ON I_APPL.UUID = I_PMON.MASTER_ID " +
                "WHERE " +
                "I_APPL.REGISTRATION_TYPE = 'IMW' " +
                "AND APPLN.UUID = :appln_id " +
                "AND I_PMON.RESULT_ACCEPT_DATE IS NULL " +
                "AND I_CERT.REMOVAL_DATE IS NULL " +
                "AND I_PMON.RESULT_REFUSE_DATE IS NULL " +
                "AND S_STATUS.CODE <> '10' " +

                "UNION " +

                "SELECT '2' AS TYPE, 'IMW' AS REG_TYPE,'Interview History' as TYPE_DESC," +
                "'' AS MW_CLASS_CODE, '' AS MW_TYPE_CODE,'' AS SRC_START_TIME,I_APPL.FILE_REFERENCE_NO AS FILE_REF,I_CAND.INTERVIEW_NUMBER AS SRC_INTERV_NO," +
                "c_propercase(APPLN.SURNAME)||' '|| c_propercase(APPLN.GIVEN_NAME_ON_ID) AS C_ENAME,C_MWI_CONCAT_MW_ITEM(I_APPL.UUID)AS ROLE_CODE," +
                "'' AS ACCEPT_D,'' AS REMOVAL_D," +
                "CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID," +
                "c_propercase(APPLN.SURNAME)||' '|| c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME," +
                "S_INTRV_RESULT.CODE AS SRC_RESULT," +
                "TO_CHAR(I_CAND.RESULT_DATE, 'DD/MM/YYYY') AS SRC_RESULT_DT," +
                "(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = :c_appln_id)  AS THIS_FILE_REF," +
                ":file_ref_no AS FILE_REF_NO " +
                "FROM " +
                "C_INTERVIEW_CANDIDATES I_CAND LEFT JOIN C_IND_CERTIFICATE I_CERT ON I_CAND.CANDIDATE_NUMBER = I_CERT.CANDIDATE_NUMBER " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_INTRV_RESULT ON S_INTRV_RESULT.UUID = I_CAND.RESULT_ID " +
                "INNER JOIN C_IND_APPLICATION I_APPL ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "INNER JOIN C_APPLICANT APPLN ON I_APPL.APPLICANT_ID = APPLN.UUID " +
                "WHERE I_APPL.REGISTRATION_TYPE = 'IMW' and APPLN.UUID = :appln_id " +

                "UNION " +

                "SELECT '1'AS TYPE, 'CMW' AS REG_TYPE, 'Registration History (RMWC (Company))'as TYPE_DESC," +
                "Case when MW_CLASS.CODE='Class 1' then 'Class I,II '||'&'||' III' when MW_CLASS.CODE='Class 2' then 'Class II '||'&'||' III' " +
                "when MW_CLASS.CODE='Class 3' then 'Class III only' end AS MW_CLASS_CODE," +
                "C_APPLN_INFO_CONCAT_MWTYPE(C_MW.COMPANY_APPLICANTS_ID, MW_CLASS.UUID) AS MW_TYPE_CODE," +
                "'' AS SRC_START_TIME,C_APPL.FILE_REFERENCE_NO AS FILE_REF,'' AS SRC_INTERV_NO," +
                "c_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME,S_APPLN_ROLE.CODE AS ROLE_CODE,TO_CHAR (C_APPLN.ACCEPT_DATE, 'DD/MM/YYYY') AS ACCEPT_D," +
                " TO_CHAR (C_APPLN.REMOVAL_DATE, 'DD/MM/YYYY') AS REMOVAL_D,CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID," +
                "c_propercase(APPLN.SURNAME)||' '|| c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME," +
                "'' AS SRC_RESULT,'' AS SRC_RESULT_DT," +
                "(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = :c_appln_id)  AS THIS_FILE_REF," +
                ":file_ref_no AS FILE_REF_NO " +
                "FROM " +
                "C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID " +
                "INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_APPLN_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_APPLN_ROLE.UUID " +
                "INNER JOIN C_COMP_APPLICANT_MW_ITEM C_MW ON C_APPLN.UUID = C_MW.COMPANY_APPLICANTS_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE MW_CLASS ON C_MW.ITEM_CLASS_ID = MW_CLASS.UUID " +
                "WHERE " +
                "C_APPL.REGISTRATION_TYPE = :reg_type and APPLN.UUID=  :appln_id " +
                "and (C_APPLN.ACCEPT_DATE IS NOT NULL OR C_APPLN.REMOVAL_DATE IS NOT NULL) " +
                "and  C_APPLN_INFO_CONCAT_MWTYPE(C_MW.COMPANY_APPLICANTS_ID, MW_CLASS.UUID) is not null " +

                "UNION " +

                " SELECT '3'AS TYPE,'CMW' AS REG_TYPE,'Proposed (RMWC (Company))'as TYPE_DESC," +
                "Case when MW_CLASS.CODE='Class 1' then 'Class I,II '||'&'||' III' when MW_CLASS.CODE='Class 2' then 'Class II '||'&'||' III' " +
                "when MW_CLASS.CODE='Class 3' then 'Class III only' end AS MW_CLASS_CODE,C_APPLN_INFO_CONCAT_MWTYPE(C_MW.COMPANY_APPLICANTS_ID, MW_CLASS.UUID) AS MW_TYPE_CODE," +
                "''AS SRC_START_TIME,C_APPL.FILE_REFERENCE_NO AS FILE_REF,''AS SRC_INTERV_NO," +
                "c_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME," +
                "S_APPLN_ROLE.CODE AS ROLE_CODE,TO_CHAR(C_APPLN.ACCEPT_DATE,'DD/MM/YYYY')AS ACCEPT_D,TO_CHAR(C_APPLN.REMOVAL_DATE,'DD/MM/YYYY')AS REMOVAL_D," +
                "CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID," +
                "c_propercase(APPLN.SURNAME)||' '|| c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME," +
                "''AS SRC_RESULT,''AS SRC_RESULT_DT," +
                "(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = :c_appln_id)  AS THIS_FILE_REF," +
                ":file_ref_no AS FILE_REF_NO " +
                "FROM " +
                "C_COMP_APPLICATION C_APPL " +
                "INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID " +
                "INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_APPLN_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_APPLN_ROLE.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_STATUS ON C_APPLN.APPLICANT_STATUS_ID = S_STATUS.UUID " +
                "INNER JOIN C_COMP_APPLICANT_MW_ITEM C_MW ON C_APPLN.UUID = C_MW.COMPANY_APPLICANTS_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE MW_CLASS ON C_MW.ITEM_CLASS_ID = MW_CLASS.UUID " +
                "WHERE " +
                "C_APPL.REGISTRATION_TYPE = :reg_type " +
                "and APPLN.UUID = :appln_id " +
                "and C_APPLN.ACCEPT_DATE IS NULL " +
                "and C_APPLN.REMOVAL_DATE IS NULL and C_APPLN.INTERVIEW_WITHDRAWN_DATE IS NULL " +
                "AND C_APPLN.INTERVIEW_REFUSAL_DATE IS NULL AND S_STATUS.CODE <> '10' " +
                "and  C_APPLN_INFO_CONCAT_MWTYPE(C_MW.COMPANY_APPLICANTS_ID, MW_CLASS.UUID) is not null " +

                "UNION " +

                "SELECT '1'AS TYPE,'CMW' AS REG_TYPE,'Registration History (RMWC (Company))'as TYPE_DESC," +
                "Case when MW_CLASS.CODE='Class 1' then 'Class I,II '||'&'||' III' when MW_CLASS.CODE='Class 2' then 'Class II '||'&'||' III'" +
                "when MW_CLASS.CODE='Class 3' then 'Class III only' end AS MW_CLASS_CODE,C_APPLN_INFO_CONCAT_MWTYPE(C_MW.COMPANY_APPLICANTS_ID, MW_CLASS.UUID) AS MW_TYPE_CODE," +
                "'' AS SRC_START_TIME,C_APPL.FILE_REFERENCE_NO AS FILE_REF,'' AS SRC_INTERV_NO," +
                "c_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME," +
                "S_APPLN_ROLE.CODE AS ROLE_CODE,TO_CHAR(C_APPLN.ACCEPT_DATE, 'DD/MM/YYYY') AS ACCEPT_D, TO_CHAR (C_APPLN.REMOVAL_DATE, 'DD/MM/YYYY') AS REMOVAL_D," +
                "CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID," +
                "c_propercase(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME," +
                "'' AS SRC_RESULT,'' AS SRC_RESULT_DT," +
                "(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = :c_appln_id)  AS THIS_FILE_REF," +
                ":file_ref_no AS FILE_REF_NO " +
                "FROM " +
                "C_COMP_APPLICANT_INFO C_APPLN " +
                "INNER JOIN C_COMP_APPLICANT_INFO_HISTORY C_APPLN_HIST ON C_APPLN.UUID = C_APPLN_HIST.COMPANY_APPLICANTS_ID " +
                "INNER JOIN C_COMP_APPLICATION C_APPL ON C_APPLN.MASTER_ID = C_APPL.UUID " +
                "INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_APPLN_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_APPLN_ROLE.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_STATUS ON C_APPLN.APPLICANT_STATUS_ID = S_STATUS.UUID " +
                "INNER JOIN C_COMP_APPLICANT_MW_ITEM C_MW ON C_APPLN.UUID = C_MW.COMPANY_APPLICANTS_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE MW_CLASS ON C_MW.ITEM_CLASS_ID = MW_CLASS.UUID " +
                "INNER JOIN (SELECT MAX(A.CREATED_DATE) AS CREATED_D, A.COMPANY_APPLICANTS_ID AS C_APPLN_ID, A.ACCEPT_DATE AS ACCEPT_D " +
                "FROM C_COMP_APPLICANT_INFO_HISTORY A WHERE  A.COMPANY_APPLICANTS_ID = :appln_id " +
                " AND A.REMOVAL_DATE IS NOT NULL or A.ACCEPT_DATE IS NOT NULL " +
                "GROUP BY A.COMPANY_APPLICANTS_ID,A.ACCEPT_DATE) C_DT ON C_DT.C_APPLN_ID = C_APPLN_HIST.COMPANY_APPLICANTS_ID AND C_DT.ACCEPT_D = C_APPLN_HIST.ACCEPT_DATE " +
                "WHERE C_APPL.REGISTRATION_TYPE = :reg_type " +
                "and APPLN.UUID = :appln_id " +
                "and (C_APPLN_HIST.ACCEPT_DATE IS NOT NULL OR C_APPLN_HIST.REMOVAL_DATE IS NOT NULL) " +
                "AND C_DT.CREATED_D = C_APPLN_HIST.CREATED_DATE and  C_APPLN_INFO_CONCAT_MWTYPE(C_MW.COMPANY_APPLICANTS_ID, MW_CLASS.UUID) is not null " +
                //--UNION 4
                "UNION " +
                "SELECT '3' AS TYPE,'CMW' AS REG_TYPE,'Proposed (RMWC (Company))'as TYPE_DESC," +
                "Case when MW_CLASS.CODE='Class 1' then 'Class I,II '||'&'||' III' when MW_CLASS.CODE='Class 2' then 'Class II '||'&'||' III' " +
                "when MW_CLASS.CODE='Class 3' then 'Class III only' end AS MW_CLASS_CODE,C_APPLN_INFO_CONCAT_MWTYPE(C_MW.COMPANY_APPLICANTS_ID, MW_CLASS.UUID) AS MW_TYPE_CODE," +
                " '' AS SRC_START_TIME, C_APPL.FILE_REFERENCE_NO AS FILE_REF," +
                "'' AS SRC_INTERV_NO, c_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME,S_APPLN_ROLE.CODE AS ROLE_CODE," +
                "TO_CHAR (C_APPLN.ACCEPT_DATE, 'DD/MM/YYYY') AS ACCEPT_D,TO_CHAR (C_APPLN.REMOVAL_DATE, 'DD/MM/YYYY') AS REMOVAL_D," +
                "CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID," +
                " c_propercase(APPLN.SURNAME)||' '|| c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME," +
                "'' AS SRC_RESULT,'' AS SRC_RESULT_DT," +
                "(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = :c_appln_id)  AS THIS_FILE_REF," +
                ":file_ref_no AS FILE_REF_NO " +
                "FROM " +
                "C_COMP_APPLICANT_INFO C_APPLN " +
                " INNER JOIN C_COMP_APPLICANT_INFO_HISTORY C_APPLN_HIST ON C_APPLN.UUID = C_APPLN_HIST.COMPANY_APPLICANTS_ID " +
                "INNER JOIN C_COMP_APPLICATION C_APPL ON C_APPLN.MASTER_ID = C_APPL.UUID " +
                "INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_APPLN_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_APPLN_ROLE.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_STATUS ON C_APPLN.APPLICANT_STATUS_ID = S_STATUS.UUID " +
                "INNER JOIN C_COMP_APPLICANT_MW_ITEM C_MW ON C_APPLN.UUID = C_MW.COMPANY_APPLICANTS_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE MW_CLASS ON C_MW.ITEM_CLASS_ID = MW_CLASS.UUID " +
                "INNER JOIN (SELECT MAX(A.CREATED_DATE) AS CREATED_D, A.COMPANY_APPLICANTS_ID AS C_APPLN_ID,A.ACCEPT_DATE AS ACCEPT_D " +
                "FROM " +
                " C_COMP_APPLICANT_INFO_HISTORY A " +
                "WHERE  A.COMPANY_APPLICANTS_ID = :appln_id AND A.REMOVAL_DATE IS NOT NULL or A.ACCEPT_DATE IS NOT NULL " +
                "GROUP BY A.COMPANY_APPLICANTS_ID,A.ACCEPT_DATE) C_DT ON C_DT.C_APPLN_ID = C_APPLN_HIST.COMPANY_APPLICANTS_ID AND C_DT.ACCEPT_D = C_APPLN_HIST.ACCEPT_DATE " +
                "WHERE " +
                "C_APPL.REGISTRATION_TYPE = :reg_type " +
                "and APPLN.UUID = :appln_id " +
                "and C_APPLN_HIST.ACCEPT_DATE IS NULL " +
                "and C_APPLN_HIST.REMOVAL_DATE IS NULL and C_APPLN.INTERVIEW_WITHDRAWN_DATE IS NULL AND C_APPLN.INTERVIEW_REFUSAL_DATE IS NULL " +
                "AND C_DT.CREATED_D = C_APPLN_HIST.CREATED_DATE and  C_APPLN_INFO_CONCAT_MWTYPE(C_MW.COMPANY_APPLICANTS_ID, MW_CLASS.UUID) is not null " +
                // --UNION 5
                "UNION " +
                "SELECT '2' AS TYPE,'CMW' AS REG_TYPE,'Interview History'as TYPE_DESC," +
                "Case when MW_CLASS.CODE='Class 1' then 'Class I,II '||'&'||' III' when MW_CLASS.CODE='Class 2' then 'Class II '||'&'||' III' " +
                "when MW_CLASS.CODE='Class 3' then 'Class III only' end AS MW_CLASS_CODE,C_APPLN_INFO_CONCAT_MWTYPE(C_MW.COMPANY_APPLICANTS_ID, MW_CLASS.UUID) AS MW_TYPE_CODE," +
                "TO_CHAR(I_CAND.START_DATE,'DD/MM/YYYY') AS SRC_START_TIME,C_APPL.FILE_REFERENCE_NO AS FILE_REF," +
                "I_CAND.INTERVIEW_NUMBER AS SRC_INTERV_NO,c_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME," +
                "S_APPLN_ROLE.CODE AS ROLE_CODE,TO_CHAR(C_APPLN.ACCEPT_DATE,'DD/MM/YYYY')AS ACCEPT_D,TO_CHAR(C_APPLN.REMOVAL_DATE,'DD/MM/YYYY')AS REMOVAL_D," +
                "CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID," +
                "c_propercase(APPLN.SURNAME)||' '|| c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME," +
                "S_INTRV_RESULT.CODE AS SRC_RESULT,TO_CHAR(I_CAND.RESULT_DATE,'DD/MM/YYYY')AS SRC_RESULT_DT," +
                "(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = :c_appln_id)  AS THIS_FILE_REF," +
                ":file_ref_no AS FILE_REF_NO " +
                "FROM " +
                "C_INTERVIEW_CANDIDATES I_CAND inner JOIN C_COMP_APPLICANT_INFO C_APPLN ON I_CAND.CANDIDATE_NUMBER = C_APPLN.CANDIDATE_NUMBER " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_INTRV_RESULT ON S_INTRV_RESULT.UUID = I_CAND.RESULT_ID " +
                "INNER JOIN C_COMP_APPLICATION C_APPL ON C_APPL.UUID = C_APPLN.MASTER_ID " +
                "INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_APPLN_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_APPLN_ROLE.UUID " +
                "INNER JOIN C_COMP_APPLICANT_MW_ITEM C_MW ON C_APPLN.UUID = C_MW.COMPANY_APPLICANTS_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE MW_CLASS ON C_MW.ITEM_CLASS_ID = MW_CLASS.UUID " +
                "WHERE " +
                "C_APPL.REGISTRATION_TYPE = :reg_type " +
                "and APPLN.UUID = :appln_id " +
                "and  C_APPLN_INFO_CONCAT_MWTYPE(C_MW.COMPANY_APPLICANTS_ID, MW_CLASS.UUID) is not null " +
                "order by TYPE, MW_CLASS_CODE, ACCEPT_D desc, SRC_START_TIME, SRC_INTERV_NO ";

            return sql;
        }

        //CRM0007
        private string getApplicationCountCMWSql()
        {
            //            string sql = "SELECT " +
            //"sum(CASE WHEN status_code = 'APPLY' THEN mwc_c1 ELSE 0 END) AS rece1," +
            //"sum(CASE WHEN status_code = 'APPLY' THEN mwcp_c1 ELSE 0 END) AS rece2," +
            //"sum(CASE WHEN status_code = 'APPLY' THEN mwc_c2 ELSE 0 END) AS rece3," +
            //"sum(CASE WHEN status_code = 'APPLY' THEN mwcp_c2 ELSE 0 END) AS rece4," +
            //"sum(CASE WHEN status_code = 'APPLY' OR status_code = ' ' THEN mwc_c3 ELSE 0 END) AS rece5," +
            //"sum(CASE WHEN status_code = 'APPLY' OR status_code = ' ' THEN mwcp_c3 ELSE 0 END) AS rece6," +
            //"sum(CASE WHEN status_code = 'APPLY' THEN mwcw ELSE 0 END) AS rece7," +
            //"sum(CASE WHEN status = 'Active' AND status_code = 'APPROVED' THEN mwc_c1 ELSE 0 END) AS allo1," +
            //"sum(CASE WHEN status = 'Active' AND status_code = 'APPROVED' THEN mwcp_c1 ELSE 0 END) AS allo2," +
            //"sum(CASE WHEN status = 'Active' AND status_code = 'APPROVED' THEN mwc_c2 ELSE 0 END) AS allo3," +
            //"sum(CASE WHEN status = 'Active' AND status_code = 'APPROVED' THEN mwcp_c2 ELSE 0 END) AS allo4," +
            //"sum(CASE WHEN status = 'Active' AND(status_code = 'APPROVED' OR status_code = ' ') THEN mwc_c3 ELSE 0 END) AS allo5," +
            //"sum(CASE WHEN status = 'Active' AND(status_code = 'APPROVED' OR status_code = ' ') THEN mwcp_c3 ELSE 0 END) AS allo6," +
            //"sum(CASE WHEN status = 'Active' AND status_code = 'APPROVED' THEN mwcw ELSE 0 END) AS allo7," +
            //"sum(CASE WHEN status IN('Inactive', 'Irregular', 'Removed', 'Business closed') AND status_code = 'APPLY' THEN mwc_c1 ELSE 0 END) AS refu1," +
            //"sum(CASE WHEN status IN('Inactive', 'Irregular', 'Removed', 'Business closed') AND status_code = 'APPLY' THEN mwcp_c1 ELSE 0 END) AS refu2," +
            //"sum(CASE WHEN status IN('Inactive', 'Irregular', 'Removed', 'Business closed') AND status_code = 'APPLY' THEN mwc_c2 ELSE 0 END) AS refu3," +
            //"sum(CASE WHEN status IN('Inactive', 'Irregular', 'Removed', 'Business closed') AND status_code = 'APPLY' THEN mwcp_c2 ELSE 0 END) AS refu4," +
            //"sum(CASE WHEN status IN('Inactive', 'Irregular', 'Removed', 'Business closed') AND(status_code = 'APPLY' OR status_code = ' ') THEN mwc_c3 ELSE 0 END) AS refu5," +
            //"sum(CASE WHEN status IN('Inactive', 'Irregular', 'Removed', 'Business closed') AND(status_code = 'APPLY' OR status_code = ' ') THEN mwcp_c3 ELSE 0 END) AS refu6," +
            //"sum(CASE WHEN status IN('Inactive', 'Irregular', 'Removed', 'Business closed') AND status_code = 'APPLY' THEN mwcw ELSE 0 END) AS refu7," +
            //"sum(CASE WHEN status = 'Withdrawn' AND status_code = 'APPLY' THEN mwc_c1 ELSE 0 END) AS with1," +
            //"sum(CASE WHEN status = 'Withdrawn' AND status_code = 'APPLY' THEN mwcp_c1 ELSE 0 END) AS with2," +
            //"sum(CASE WHEN status = 'Withdrawn' AND status_code = 'APPLY' THEN mwc_c2 ELSE 0 END) AS with3," +
            //"sum(CASE WHEN status = 'Withdrawn' AND status_code = 'APPLY' THEN mwcp_c2 ELSE 0 END) AS with4," +
            //"sum(CASE WHEN status = 'Withdrawn' AND(status_code = 'APPLY' OR status_code = ' ') THEN mwc_c3 ELSE 0 END) AS with5," +
            //"sum(CASE WHEN status = 'Withdrawn' AND(status_code = 'APPLY' OR status_code = ' ') THEN mwcp_c3 ELSE 0 END) AS with6," +
            //"sum(CASE WHEN status = 'Withdrawn' AND status_code = 'APPLY' THEN mwcw ELSE 0 END) AS with7," +
            //"sum(CASE WHEN status IN('Application in progress', 'Document missing', 'Certificate prepared') AND status_code = 'APPLY' THEN mwc_c1 ELSE 0 END) AS proc1," +
            //"sum(CASE WHEN status IN('Application in progress', 'Document missing', 'Certificate prepared') AND status_code = 'APPLY' THEN mwcp_c1 ELSE 0 END) AS proc2," +
            //"sum(CASE WHEN status IN('Application in progress', 'Document missing', 'Certificate prepared') AND status_code = 'APPLY' THEN mwc_c2 ELSE 0 END) AS proc3," +
            //"sum(CASE WHEN status IN('Application in progress', 'Document missing', 'Certificate prepared') AND status_code = 'APPLY' THEN mwcp_c2 ELSE 0 END) AS proc4," +
            //"sum(CASE WHEN status IN('Application in progress', 'Document missing', 'Certificate prepared') AND(status_code = 'APPLY' OR status_code = ' ') THEN mwc_c3 ELSE 0 END) AS proc5," +
            //"sum(CASE WHEN status IN('Application in progress', 'Document missing', 'Certificate prepared') AND(status_code = 'APPLY' OR status_code = ' ') THEN mwcp_c3 ELSE 0 END) AS proc6," +
            //"sum(CASE WHEN status IN('Application in progress', 'Document missing', 'Certificate prepared') AND status_code = 'APPLY' THEN mwcw ELSE 0 END) AS proc7 " +
            //"FROM( " +

            //" SELECT all_status.status AS status, all_status.status_code, mwc_c1.count AS mwc_c1, mwcp_c1.count AS mwcp_c1," +
            //"mwc_c2.count AS mwc_c2, mwcp_c2.count AS mwcp_c2, mwc_c3.count AS mwc_c3," +
            //"mwcp_c3.count AS mwcp_c3, mwcw.count AS mwcw " +
            //"FROM( " +

            //"( " +
            //   " SELECT * FROM( " +
            //        "(SELECT sval.ENGLISH_DESCRIPTION AS status FROM C_S_SYSTEM_VALUE sval, C_S_SYSTEM_TYPE stype " +
            //       " WHERE sval.SYSTEM_TYPE_ID = stype.UUID " +
            //"AND stype.TYPE = 'APPLICANT_STATUS' " +
            //"AND sval.IS_ACTIVE = 'Y') stat " +
            //"LEFT JOIN ( " +
            //    "SELECT 'APPLY' AS status_code FROM DUAL " +
            //    "UNION ALL " +
            //    "SELECT 'APPROVED' FROM DUAL " +
            //    "UNION ALL " +
            //    "SELECT ' ' FROM DUAL) approved ON 1 = 1 " +
            //    ") " +
            //") all_status " +
            //"LEFT JOIN " +
            //"( " +
            //    "SELECT status, STATUS_CODE, count(DISTINCT uuid) AS count FROM( " +
            //        "SELECT capp.uuid, sstatus.ENGLISH_DESCRIPTION AS status, cinfod.STATUS_CODE, min(sclass.CODE) AS class " +
            //        "FROM " +
            //        "C_COMP_APPLICANT_INFO_MASTER cinfom," +
            //        "C_COMP_APPLICANT_INFO_DETAIL cinfod," +
            //        "C_COMP_APPLICANT_INFO cinfo," +
            //        "C_COMP_APPLICATION capp," +
            //        "C_S_SYSTEM_VALUE sclass," +
            //        "C_S_SYSTEM_VALUE sstatus," +
            //        "C_S_SYSTEM_VALUE srole," +
            //        "C_S_CATEGORY_CODE scat " +
            //        "WHERE cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID " +
            //        "AND cinfod.ITEM_CLASS_ID = sclass.UUID " +
            //        "AND cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID " +
            //        "AND capp.UUID = cinfo.MASTER_ID " +
            //        "AND scat.UUID = capp.CATEGORY_ID " +
            //        "AND sstatus.UUID = capp.APPLICATION_STATUS_ID " +
            //        "AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
            //        "AND scat.CODE = 'MWC' " +
            //        "AND srole.CODE = 'AS' " +
            //        "AND sstatus.IS_ACTIVE = 'Y' " +

            //        "GROUP BY capp.uuid, cinfod.STATUS_CODE," +
            //        "sstatus.ENGLISH_DESCRIPTION " +

            //    ") WHERE class = 'Class 1' " +
            //    "GROUP BY status, STATUS_CODE " +
            //") mwc_c1 ON all_status.status = mwc_c1.status AND all_status.status_code = mwc_c1.status_code " +

            //"LEFT JOIN " +

            //"( SELECT status, STATUS_CODE, count(DISTINCT uuid) AS count FROM ( " +
            //        "SELECT capp.uuid, sstatus.ENGLISH_DESCRIPTION AS status, cinfod.STATUS_CODE, min(sclass.CODE) AS class " +
            //        "FROM " +

            //        "C_COMP_APPLICANT_INFO_MASTER cinfom," +
            //        "C_COMP_APPLICANT_INFO_DETAIL cinfod," +
            //        "C_COMP_APPLICANT_INFO cinfo," +
            //        "C_COMP_APPLICATION capp," +
            //        "C_S_SYSTEM_VALUE sclass," +
            //        "C_S_SYSTEM_VALUE sstatus," +
            //        "C_S_SYSTEM_VALUE srole," +
            //        "C_S_CATEGORY_CODE scat " +
            //        "WHERE cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID " +
            //        "AND cinfod.ITEM_CLASS_ID = sclass.UUID " +
            //        "AND cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID " +
            //        "AND capp.UUID = cinfo.MASTER_ID " +
            //        "AND scat.UUID = capp.CATEGORY_ID " +
            //        "AND sstatus.UUID = capp.APPLICATION_STATUS_ID " +
            //        "AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
            //        "AND scat.CODE = 'MWC(P)' " +
            //        "AND srole.CODE = 'AS' " +
            //        "AND sstatus.IS_ACTIVE = 'Y' " +

            //        "GROUP BY capp.uuid, cinfod.STATUS_CODE, " +
            //        "sstatus.ENGLISH_DESCRIPTION " +

            //    ") WHERE class = 'Class 1' " +
            //    "GROUP BY status, STATUS_CODE " +
            //") mwcp_c1 ON all_status.status = mwcp_c1.status AND all_status.status_code = mwcp_c1.status_code " +


            //"LEFT JOIN " +

            //"( " +
            //    "SELECT status, STATUS_CODE, count(DISTINCT uuid) AS count FROM ( " +
            //        "SELECT capp.uuid, sstatus.ENGLISH_DESCRIPTION AS status, cinfod.STATUS_CODE, min(sclass.CODE) AS class " +
            //        "FROM " +

            //        "C_COMP_APPLICANT_INFO_MASTER cinfom, " +
            //        "C_COMP_APPLICANT_INFO_DETAIL cinfod, " +
            //        "C_COMP_APPLICANT_INFO cinfo, " +
            //        "C_COMP_APPLICATION capp, " +
            //        "C_S_SYSTEM_VALUE sclass, " +
            //        "C_S_SYSTEM_VALUE sstatus, " +
            //        "C_S_SYSTEM_VALUE srole, " +
            //        "C_S_CATEGORY_CODE scat " +
            //        "WHERE cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID " +
            //        "AND cinfod.ITEM_CLASS_ID = sclass.UUID " +
            //        "AND cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID " +
            //        "AND capp.UUID = cinfo.MASTER_ID " +
            //        "AND scat.UUID = capp.CATEGORY_ID " +
            //        "AND sstatus.UUID = capp.APPLICATION_STATUS_ID " +
            //        "AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
            //        "AND scat.CODE = 'MWC' " +
            //        "AND srole.CODE = 'AS' " +
            //        "AND sstatus.IS_ACTIVE = 'Y' " +


            //        "GROUP BY capp.uuid, cinfod.STATUS_CODE, " +
            //        "sstatus.ENGLISH_DESCRIPTION " +

            //    ") WHERE class = 'Class 2' " +
            //    "GROUP BY status, STATUS_CODE " +
            //") mwc_c2 ON all_status.status = mwc_c2.status AND all_status.status_code = mwc_c2.status_code " +


            //"LEFT JOIN " +

            //"( " +
            //    "SELECT status, STATUS_CODE, count(DISTINCT uuid) AS count FROM ( " +
            //        "SELECT capp.uuid, sstatus.ENGLISH_DESCRIPTION AS status, cinfod.STATUS_CODE, min(sclass.CODE) AS class " +
            //        "FROM " +

            //        "C_COMP_APPLICANT_INFO_MASTER cinfom, " +
            //        "C_COMP_APPLICANT_INFO_DETAIL cinfod, " +
            //        "C_COMP_APPLICANT_INFO cinfo, " +
            //        "C_COMP_APPLICATION capp," +
            //        "C_S_SYSTEM_VALUE sclass," +
            //        "C_S_SYSTEM_VALUE sstatus," +
            //        "C_S_SYSTEM_VALUE srole," +
            //        "C_S_CATEGORY_CODE scat " +
            //        "WHERE cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID " +
            //        "AND cinfod.ITEM_CLASS_ID = sclass.UUID " +
            //        "AND cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID " +
            //        "AND capp.UUID = cinfo.MASTER_ID " +
            //        "AND scat.UUID = capp.CATEGORY_ID " +
            //        "AND sstatus.UUID = capp.APPLICATION_STATUS_ID " +
            //        "AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
            //        "AND scat.CODE = 'MWC(P)' " +
            //        "AND srole.CODE = 'AS' " +
            //        "AND sstatus.IS_ACTIVE = 'Y' " +


            //        "GROUP BY capp.uuid, cinfod.STATUS_CODE, " +
            //        "sstatus.ENGLISH_DESCRIPTION " +

            //    ") WHERE class = 'Class 2' " +
            //    "GROUP BY status, STATUS_CODE " +
            //") mwcp_c2 ON all_status.status = mwcp_c2.status AND all_status.status_code = mwcp_c2.status_code " +



            //"LEFT JOIN " +

            //"( " +
            //    "SELECT status, STATUS_CODE, count(DISTINCT uuid) AS count FROM ( " +
            //        "SELECT capp.uuid, sstatus.ENGLISH_DESCRIPTION AS status, decode(cinfod.STATUS_CODE, NULL, ' ', cinfod.STATUS_CODE) AS status_code, min(sclass.CODE) AS class " +
            //        "FROM " +

            //        "C_COMP_APPLICATION capp " +

            //        "LEFT JOIN C_COMP_APPLICANT_INFO cinfo ON capp.UUID = cinfo.MASTER_ID " +
            //        "LEFT JOIN C_COMP_APPLICANT_INFO_MASTER cinfom ON cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID " +
            //        "LEFT JOIN C_COMP_APPLICANT_INFO_DETAIL cinfod ON cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID " +
            //        "LEFT JOIN C_S_SYSTEM_VALUE sclass ON cinfod.ITEM_CLASS_ID = sclass.UUID " +
            //        "LEFT JOIN C_S_SYSTEM_VALUE sstatus ON sstatus.UUID = capp.APPLICATION_STATUS_ID " +
            //        "LEFT JOIN C_S_SYSTEM_VALUE srole ON srole.UUID = cinfo.APPLICANT_ROLE_ID " +
            //        "LEFT JOIN C_S_CATEGORY_CODE scat ON scat.UUID = capp.CATEGORY_ID " +
            //        "WHERE scat.CODE = 'MWC' " +
            //        "AND (srole.CODE = 'AS' OR srole.CODE IS NULL) " +
            //        //"--AND sstatus.IS_ACTIVE = 'Y' "+
            //        "GROUP BY capp.uuid, decode(cinfod.STATUS_CODE, NULL, ' ', cinfod.STATUS_CODE), sstatus.ENGLISH_DESCRIPTION " +
            //    ") WHERE class = 'Class 3' OR class IS NULL " +
            //    "GROUP BY status, STATUS_CODE " +
            //") mwc_c3 ON all_status.status = mwc_c3.status AND all_status.status_code = mwc_c3.status_code " +
            //"LEFT JOIN " +
            //"( " +
            //    "SELECT status, STATUS_CODE, count(DISTINCT uuid) AS count FROM ( " +
            //        "SELECT capp.uuid, sstatus.ENGLISH_DESCRIPTION AS status, decode(cinfod.STATUS_CODE, NULL, ' ', cinfod.STATUS_CODE) AS status_code, min(sclass.CODE) AS class " +
            //        "FROM " +
            //        "C_COMP_APPLICATION capp " +
            //        "LEFT JOIN C_COMP_APPLICANT_INFO cinfo ON capp.UUID = cinfo.MASTER_ID " +
            //        "LEFT JOIN C_COMP_APPLICANT_INFO_MASTER cinfom ON cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID " +

            //        "LEFT JOIN C_COMP_APPLICANT_INFO_DETAIL cinfod ON cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID " +

            //        "LEFT JOIN C_S_SYSTEM_VALUE sclass ON cinfod.ITEM_CLASS_ID = sclass.UUID " +

            //        "LEFT JOIN C_S_SYSTEM_VALUE sstatus ON sstatus.UUID = capp.APPLICATION_STATUS_ID " +

            //        "LEFT JOIN C_S_SYSTEM_VALUE srole ON srole.UUID = cinfo.APPLICANT_ROLE_ID " +

            //        "LEFT JOIN C_S_CATEGORY_CODE scat ON scat.UUID = capp.CATEGORY_ID " +

            //        "WHERE " +
            //        "scat.CODE = 'MWC(P)' " +

            //        " AND (srole.CODE = 'AS' OR srole.CODE IS NULL) " +
            //        //--AND sstatus.IS_ACTIVE = 'Y'

            //        "GROUP BY capp.uuid, decode(cinfod.STATUS_CODE, NULL, ' ', cinfod.STATUS_CODE), sstatus.ENGLISH_DESCRIPTION " +
            //    ") WHERE class = 'Class 3' OR class IS NULL " +
            //    "GROUP BY status, STATUS_CODE " +
            //") mwcp_c3 ON all_status.status = mwcp_c3.status AND all_status.status_code = mwcp_c3.status_code " +
            //"LEFT JOIN " +
            //"( " +
            //    "SELECT status, count(DISTINCT uuid) AS count FROM ( " +
            //        "SELECT iapp.uuid, sstatus.ENGLISH_DESCRIPTION AS status FROM " +

            //        "C_IND_CERTIFICATE icer," +
            //        "C_IND_APPLICATION iapp," +
            //        "C_S_SYSTEM_VALUE sstatus," +
            //        "C_S_CATEGORY_CODE scat " +

            //        "WHERE iapp.UUID = icer.MASTER_ID " +

            //        "AND scat.UUID = icer.CATEGORY_ID " +

            //        "AND sstatus.UUID = icer.APPLICATION_STATUS_ID " +

            //        "AND scat.CODE = 'MWC(W)' " +

            //        "AND sstatus.IS_ACTIVE = 'Y' " +

            //        "GROUP BY iapp.uuid, " +
            //        "sstatus.ENGLISH_DESCRIPTION " +

            //    ")  GROUP BY status " +
            //") mwcw ON all_status.status = mwcw.status ))";

            string sql = @"SELECT Sum(CASE
             WHEN status_code = 'APPLY' THEN mwc_c1
             ELSE 0
           END) AS rece1,
       Sum(CASE
             WHEN status_code = 'APPLY' THEN mwcp_c1
             ELSE 0
           END) AS rece2,
       Sum(CASE
             WHEN status_code = 'APPLY' THEN mwc_c2
             ELSE 0
           END) AS rece3,
       Sum(CASE
             WHEN status_code = 'APPLY' THEN mwcp_c2
             ELSE 0
           END) AS rece4,
       Sum(CASE
             WHEN status_code = 'APPLY'
                   OR status_code = ' ' THEN mwc_c3
             ELSE 0
           END) AS rece5,
       Sum(CASE
             WHEN status_code = 'APPLY'
                   OR status_code = ' ' THEN mwcp_c3
             ELSE 0
           END) AS rece6,
       Sum(CASE
             WHEN status_code = 'APPLY' THEN mwcw
             ELSE 0
           END) AS rece7,
       Sum(CASE
             WHEN status = 'Active'
                  AND status_code = 'APPROVED' THEN mwc_c1
             ELSE 0
           END) AS allo1,
       Sum(CASE
             WHEN status = 'Active'
                  AND status_code = 'APPROVED' THEN mwcp_c1
             ELSE 0
           END) AS allo2,
       Sum(CASE
             WHEN status = 'Active'
                  AND status_code = 'APPROVED' THEN mwc_c2
             ELSE 0
           END) AS allo3,
       Sum(CASE
             WHEN status = 'Active'
                  AND status_code = 'APPROVED' THEN mwcp_c2
             ELSE 0
           END) AS allo4,
       Sum(CASE
             WHEN status = 'Active'
                  AND ( status_code = 'APPROVED'
                         OR status_code = ' ' ) THEN mwc_c3
             ELSE 0
           END) AS allo5,
       Sum(CASE
             WHEN status = 'Active'
                  AND ( status_code = 'APPROVED'
                         OR status_code = ' ' ) THEN mwcp_c3
             ELSE 0
           END) AS allo6,
       Sum(CASE
             WHEN status = 'Active'
                  AND status_code = 'APPROVED' THEN mwcw
             ELSE 0
           END) AS allo7,
       Sum(CASE
             WHEN status IN( 'Inactive', 'Irregular', 'Removed', 'Business closed' )
                  AND status_code = 'APPLY' THEN mwc_c1
             ELSE 0
           END) AS refu1,
       Sum(CASE
             WHEN status IN( 'Inactive', 'Irregular', 'Removed', 'Business closed' )
                  AND status_code = 'APPLY' THEN mwcp_c1
             ELSE 0
           END) AS refu2,
       Sum(CASE
             WHEN status IN( 'Inactive', 'Irregular', 'Removed', 'Business closed' )
                  AND status_code = 'APPLY' THEN mwc_c2
             ELSE 0
           END) AS refu3,
       Sum(CASE
             WHEN status IN( 'Inactive', 'Irregular', 'Removed', 'Business closed' )
                  AND status_code = 'APPLY' THEN mwcp_c2
             ELSE 0
           END) AS refu4,
       Sum(CASE
             WHEN status IN( 'Inactive', 'Irregular', 'Removed', 'Business closed' )
                  AND ( status_code = 'APPLY'
                         OR status_code = ' ' ) THEN mwc_c3
             ELSE 0
           END) AS refu5,
       Sum(CASE
             WHEN status IN( 'Inactive', 'Irregular', 'Removed', 'Business closed' )
                  AND ( status_code = 'APPLY'
                         OR status_code = ' ' ) THEN mwcp_c3
             ELSE 0
           END) AS refu6,
       Sum(CASE
             WHEN status IN( 'Inactive', 'Irregular', 'Removed', 'Business closed' )
                  AND status_code = 'APPLY' THEN mwcw
             ELSE 0
           END) AS refu7,
       Sum(CASE
             WHEN status = 'Withdrawn'
                  AND status_code = 'APPLY' THEN mwc_c1
             ELSE 0
           END) AS with1,
       Sum(CASE
             WHEN status = 'Withdrawn'
                  AND status_code = 'APPLY' THEN mwcp_c1
             ELSE 0
           END) AS with2,
       Sum(CASE
             WHEN status = 'Withdrawn'
                  AND status_code = 'APPLY' THEN mwc_c2
             ELSE 0
           END) AS with3,
       Sum(CASE
             WHEN status = 'Withdrawn'
                  AND status_code = 'APPLY' THEN mwcp_c2
             ELSE 0
           END) AS with4,
       Sum(CASE
             WHEN status = 'Withdrawn'
                  AND ( status_code = 'APPLY'
                         OR status_code = ' ' ) THEN mwc_c3
             ELSE 0
           END) AS with5,
       Sum(CASE
             WHEN status = 'Withdrawn'
                  AND ( status_code = 'APPLY'
                         OR status_code = ' ' ) THEN mwcp_c3
             ELSE 0
           END) AS with6,
       Sum(CASE
             WHEN status = 'Withdrawn'
                  AND status_code = 'APPLY' THEN mwcw
             ELSE 0
           END) AS with7,
       Sum(CASE
             WHEN status IN( 'Application in progress', 'Document missing', 'Certificate prepared' )
                  AND status_code = 'APPLY' THEN mwc_c1
             ELSE 0
           END) AS proc1,
       Sum(CASE
             WHEN status IN( 'Application in progress', 'Document missing', 'Certificate prepared' )
                  AND status_code = 'APPLY' THEN mwcp_c1
             ELSE 0
           END) AS proc2,
       Sum(CASE
             WHEN status IN( 'Application in progress', 'Document missing', 'Certificate prepared' )
                  AND status_code = 'APPLY' THEN mwc_c2
             ELSE 0
           END) AS proc3,
       Sum(CASE
             WHEN status IN( 'Application in progress', 'Document missing', 'Certificate prepared' )
                  AND status_code = 'APPLY' THEN mwcp_c2
             ELSE 0
           END) AS proc4,
       Sum(CASE
             WHEN status IN( 'Application in progress', 'Document missing', 'Certificate prepared' )
                  AND ( status_code = 'APPLY'
                         OR status_code = ' ' ) THEN mwc_c3
             ELSE 0
           END) AS proc5,
       Sum(CASE
             WHEN status IN( 'Application in progress', 'Document missing', 'Certificate prepared' )
                  AND ( status_code = 'APPLY'
                         OR status_code = ' ' ) THEN mwcp_c3
             ELSE 0
           END) AS proc6,
       Sum(CASE
             WHEN status IN( 'Application in progress', 'Document missing', 'Certificate prepared' )
                  AND status_code = 'APPLY' THEN mwcw
             ELSE 0
           END) AS proc7
FROM   (SELECT all_status.status       AS status,
               all_status.status_code,
               mwc_c1.count            AS mwc_c1,
               mwcp_c1.count           AS mwcp_c1,
               mwc_c2.count            AS mwc_c2,
               mwcp_c2.count           AS mwcp_c2,
               mwc_c3.count            AS mwc_c3,
               mwcp_c3.count           AS mwcp_c3,
               mwcw.count              AS mwcw,
               mwcp_c3.applicationDate AS appDate
        FROM   ( (SELECT *
                FROM   ( (SELECT sval.ENGLISH_DESCRIPTION AS status
                        FROM   C_S_SYSTEM_VALUE sval,
                               C_S_SYSTEM_TYPE stype
                        WHERE  sval.SYSTEM_TYPE_ID = stype.UUID
                               AND stype.TYPE = 'APPLICANT_STATUS'
                               AND sval.IS_ACTIVE = 'Y') stat
                         LEFT JOIN (SELECT 'APPLY' AS status_code
                                    FROM   DUAL
                                    UNION ALL
                                    SELECT 'APPROVED'
                                    FROM   DUAL
                                    UNION ALL
                                    SELECT ' '
                                    FROM   DUAL) approved
                                ON 1 = 1 )) all_status
                 LEFT JOIN (SELECT status,
                                   STATUS_CODE,
                                   Count(DISTINCT uuid) AS count
                            FROM   (SELECT capp.uuid,
                                           sstatus.ENGLISH_DESCRIPTION AS status,
                                           cinfod.STATUS_CODE,
                                           Min(sclass.CODE)            AS class
                                    FROM   C_COMP_APPLICANT_INFO_MASTER cinfom,
                                           C_COMP_APPLICANT_INFO_DETAIL cinfod,
                                           C_COMP_APPLICANT_INFO cinfo,
                                           C_COMP_APPLICATION capp,
                                           C_S_SYSTEM_VALUE sclass,
                                           C_S_SYSTEM_VALUE sstatus,
                                           C_S_SYSTEM_VALUE srole,
                                           C_S_CATEGORY_CODE scat
                                    WHERE  cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID
                                           AND cinfod.ITEM_CLASS_ID = sclass.UUID
                                           AND cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID
                                           AND capp.UUID = cinfo.MASTER_ID
                                           AND scat.UUID = capp.CATEGORY_ID
                                           AND sstatus.UUID = capp.APPLICATION_STATUS_ID
                                           AND srole.UUID = cinfo.APPLICANT_ROLE_ID
                                           AND scat.CODE = 'MWC'
                                           AND srole.CODE = 'AS'
                                           AND sstatus.IS_ACTIVE = 'Y'
                                    GROUP  BY capp.uuid,
                                              cinfod.STATUS_CODE,
                                              sstatus.ENGLISH_DESCRIPTION)
                            WHERE  class = 'Class 1'
                            GROUP  BY status,
                                      STATUS_CODE) mwc_c1
                        ON all_status.status = mwc_c1.status
                           AND all_status.status_code = mwc_c1.status_code
                 LEFT JOIN (SELECT status,
                                   STATUS_CODE,
                                   Count(DISTINCT uuid) AS count
                            FROM   (SELECT capp.uuid,
                                           sstatus.ENGLISH_DESCRIPTION AS status,
                                           cinfod.STATUS_CODE,
                                           Min(sclass.CODE)            AS class
                                    FROM   C_COMP_APPLICANT_INFO_MASTER cinfom,
                                           C_COMP_APPLICANT_INFO_DETAIL cinfod,
                                           C_COMP_APPLICANT_INFO cinfo,
                                           C_COMP_APPLICATION capp,
                                           C_S_SYSTEM_VALUE sclass,
                                           C_S_SYSTEM_VALUE sstatus,
                                           C_S_SYSTEM_VALUE srole,
                                           C_S_CATEGORY_CODE scat
                                    WHERE  cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID
                                           AND cinfod.ITEM_CLASS_ID = sclass.UUID
                                           AND cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID
                                           AND capp.UUID = cinfo.MASTER_ID
                                           AND scat.UUID = capp.CATEGORY_ID
                                           AND sstatus.UUID = capp.APPLICATION_STATUS_ID
                                           AND srole.UUID = cinfo.APPLICANT_ROLE_ID
                                           AND scat.CODE = 'MWC(P)'
                                           AND srole.CODE = 'AS'
                                           AND sstatus.IS_ACTIVE = 'Y'
                                    GROUP  BY capp.uuid,
                                              cinfod.STATUS_CODE,
                                              sstatus.ENGLISH_DESCRIPTION)
                            WHERE  class = 'Class 1'
                            GROUP  BY status,
                                      STATUS_CODE) mwcp_c1
                        ON all_status.status = mwcp_c1.status
                           AND all_status.status_code = mwcp_c1.status_code
                 LEFT JOIN (SELECT status,
                                   STATUS_CODE,
                                   Count(DISTINCT uuid) AS count
                            FROM   (SELECT capp.uuid,
                                           sstatus.ENGLISH_DESCRIPTION AS status,
                                           cinfod.STATUS_CODE,
                                           Min(sclass.CODE)            AS class
                                    FROM   C_COMP_APPLICANT_INFO_MASTER cinfom,
                                           C_COMP_APPLICANT_INFO_DETAIL cinfod,
                                           C_COMP_APPLICANT_INFO cinfo,
                                           C_COMP_APPLICATION capp,
                                           C_S_SYSTEM_VALUE sclass,
                                           C_S_SYSTEM_VALUE sstatus,
                                           C_S_SYSTEM_VALUE srole,
                                           C_S_CATEGORY_CODE scat
                                    WHERE  cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID
                                           AND cinfod.ITEM_CLASS_ID = sclass.UUID
                                           AND cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID
                                           AND capp.UUID = cinfo.MASTER_ID
                                           AND scat.UUID = capp.CATEGORY_ID
                                           AND sstatus.UUID = capp.APPLICATION_STATUS_ID
                                           AND srole.UUID = cinfo.APPLICANT_ROLE_ID
                                           AND scat.CODE = 'MWC'
                                           AND srole.CODE = 'AS'
                                           AND sstatus.IS_ACTIVE = 'Y'
                                    GROUP  BY capp.uuid,
                                              cinfod.STATUS_CODE,
                                              sstatus.ENGLISH_DESCRIPTION)
                            WHERE  class = 'Class 2'
                            GROUP  BY status,
                                      STATUS_CODE) mwc_c2
                        ON all_status.status = mwc_c2.status
                           AND all_status.status_code = mwc_c2.status_code
                 LEFT JOIN (SELECT status,
                                   STATUS_CODE,
                                   Count(DISTINCT uuid) AS count
                            FROM   (SELECT capp.uuid,
                                           sstatus.ENGLISH_DESCRIPTION AS status,
                                           cinfod.STATUS_CODE,
                                           Min(sclass.CODE)            AS class
                                    FROM   C_COMP_APPLICANT_INFO_MASTER cinfom,
                                           C_COMP_APPLICANT_INFO_DETAIL cinfod,
                                           C_COMP_APPLICANT_INFO cinfo,
                                           C_COMP_APPLICATION capp,
                                           C_S_SYSTEM_VALUE sclass,
                                           C_S_SYSTEM_VALUE sstatus,
                                           C_S_SYSTEM_VALUE srole,
                                           C_S_CATEGORY_CODE scat
                                    WHERE  cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID
                                           AND cinfod.ITEM_CLASS_ID = sclass.UUID
                                           AND cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID
                                           AND capp.UUID = cinfo.MASTER_ID
                                           AND scat.UUID = capp.CATEGORY_ID
                                           AND sstatus.UUID = capp.APPLICATION_STATUS_ID
                                           AND srole.UUID = cinfo.APPLICANT_ROLE_ID
                                           AND scat.CODE = 'MWC(P)'
                                           AND srole.CODE = 'AS'
                                           AND sstatus.IS_ACTIVE = 'Y'
                                    GROUP  BY capp.uuid,
                                              cinfod.STATUS_CODE,
                                              sstatus.ENGLISH_DESCRIPTION)
                            WHERE  class = 'Class 2'
                            GROUP  BY status,
                                      STATUS_CODE) mwcp_c2
                        ON all_status.status = mwcp_c2.status
                           AND all_status.status_code = mwcp_c2.status_code
                 LEFT JOIN (SELECT status,
                                   STATUS_CODE,
                                   Count(DISTINCT uuid) AS count
                            FROM   (SELECT capp.uuid,
                                           sstatus.ENGLISH_DESCRIPTION                    AS status,
                                           Decode(cinfod.STATUS_CODE, NULL, ' ',
                                                                      cinfod.STATUS_CODE) AS status_code,
                                           Min(sclass.CODE)                               AS class
                                    FROM   C_COMP_APPLICATION capp
                                           LEFT JOIN C_COMP_APPLICANT_INFO cinfo
                                                  ON capp.UUID = cinfo.MASTER_ID
                                           LEFT JOIN C_COMP_APPLICANT_INFO_MASTER cinfom
                                                  ON cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID
                                           LEFT JOIN C_COMP_APPLICANT_INFO_DETAIL cinfod
                                                  ON cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID
                                           LEFT JOIN C_S_SYSTEM_VALUE sclass
                                                  ON cinfod.ITEM_CLASS_ID = sclass.UUID
                                           LEFT JOIN C_S_SYSTEM_VALUE sstatus
                                                  ON sstatus.UUID = capp.APPLICATION_STATUS_ID
                                           LEFT JOIN C_S_SYSTEM_VALUE srole
                                                  ON srole.UUID = cinfo.APPLICANT_ROLE_ID
                                           LEFT JOIN C_S_CATEGORY_CODE scat
                                                  ON scat.UUID = capp.CATEGORY_ID
                                    WHERE  scat.CODE = 'MWC'
                                           AND ( srole.CODE = 'AS'
                                                  OR srole.CODE IS NULL )
                                    GROUP  BY capp.uuid,
                                              Decode(cinfod.STATUS_CODE, NULL, ' ',
                                                                         cinfod.STATUS_CODE),
                                              sstatus.ENGLISH_DESCRIPTION)
                            WHERE  class = 'Class 3'
                                    OR class IS NULL
                            GROUP  BY status,
                                      STATUS_CODE) mwc_c3
                        ON all_status.status = mwc_c3.status
                           AND all_status.status_code = mwc_c3.status_code
                 LEFT JOIN (SELECT status,
                                   STATUS_CODE,
                                   applicationDate,
                                   Count(DISTINCT uuid) AS count
                            FROM   (SELECT capp.uuid,
                                           capp.application_date                          AS applicationDate,
                                           sstatus.ENGLISH_DESCRIPTION                    AS status,
                                           Decode(cinfod.STATUS_CODE, NULL, ' ',
                                                                      cinfod.STATUS_CODE) AS status_code,
                                           Min(sclass.CODE)                               AS class
                                    FROM   C_COMP_APPLICATION capp
                                           LEFT JOIN C_COMP_APPLICANT_INFO cinfo
                                                  ON capp.UUID = cinfo.MASTER_ID
                                           LEFT JOIN C_COMP_APPLICANT_INFO_MASTER cinfom
                                                  ON cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID
                                           LEFT JOIN C_COMP_APPLICANT_INFO_DETAIL cinfod
                                                  ON cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID
                                           LEFT JOIN C_S_SYSTEM_VALUE sclass
                                                  ON cinfod.ITEM_CLASS_ID = sclass.UUID
                                           LEFT JOIN C_S_SYSTEM_VALUE sstatus
                                                  ON sstatus.UUID = capp.APPLICATION_STATUS_ID
                                           LEFT JOIN C_S_SYSTEM_VALUE srole
                                                  ON srole.UUID = cinfo.APPLICANT_ROLE_ID
                                           LEFT JOIN C_S_CATEGORY_CODE scat
                                                  ON scat.UUID = capp.CATEGORY_ID
                                    WHERE  scat.CODE = 'MWC(P)'
                                           AND ( srole.CODE = 'AS'
                                                  OR srole.CODE IS NULL )
                                    GROUP  BY capp.uuid,
                                              capp.application_date,
                                              Decode(cinfod.STATUS_CODE, NULL, ' ',
                                                                         cinfod.STATUS_CODE),
                                              sstatus.ENGLISH_DESCRIPTION)
                            WHERE  class = 'Class 3'
                                    OR class IS NULL
                            GROUP  BY status,
                                      applicationDate,
                                      STATUS_CODE) mwcp_c3
                        ON all_status.status = mwcp_c3.status
                           AND all_status.status_code = mwcp_c3.status_code
                 LEFT JOIN (SELECT status,
                                   Count(DISTINCT uuid) AS count
                            FROM   (SELECT iapp.uuid,
                                           sstatus.ENGLISH_DESCRIPTION AS status
                                    FROM   C_IND_CERTIFICATE icer,
                                           C_IND_APPLICATION iapp,
                                           C_S_SYSTEM_VALUE sstatus,
                                           C_S_CATEGORY_CODE scat
                                    WHERE  iapp.UUID = icer.MASTER_ID
                                           AND scat.UUID = icer.CATEGORY_ID
                                           AND sstatus.UUID = icer.APPLICATION_STATUS_ID
                                           AND scat.CODE = 'MWC(W)'
                                           AND sstatus.IS_ACTIVE = 'Y'
                                    GROUP  BY iapp.uuid,
                                              sstatus.ENGLISH_DESCRIPTION)
                            GROUP  BY status) mwcw
                        ON all_status.status = mwcw.status )
        WHERE  To_date(To_char(mwcp_c3.applicationDate, 'dd/MM/yyyy'), 'dd/MM/yyyy') BETWEEN To_date(:from_date, 'dd/MM/yyyy') AND To_date(:to_date, 'dd/MM/yyyy'))
";

            return sql;
        }

        //CRM0008
        private string getApplicationCountIPSql(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT status, sum(AP_A) AS AP_A, sum(AP_E) AS AP_E, sum(AP_S) AS AP_S, sum(AP_I) AS AP_I, sum(AP_II) AS AP_II, sum(AP_III) AS AP_III, sum(RSE) AS RSE, sum(RGE) AS RGE, sum(RI_A) AS RI_A, sum(RI_E) AS RI_E, sum(RI_S) AS RI_S FROM ( " +
                        "SELECT scat.code, " +
                        "CASE WHEN ipm.RESULT_ACCEPT_DATE IS NOT NULL THEN 'ACCEPTED' " +

                             "WHEN ipm.RESULT_DEFER_DATE IS NOT NULL THEN 'DEFER' " +

                             "WHEN ipm.RESULT_REFUSE_DATE IS NOT NULL THEN 'REFUSED' END AS status, " +
                        "CASE WHEN scat.CODE = 'AP(A)' THEN count(*) ELSE 0 END AS AP_A, " +
                        "CASE WHEN scat.CODE = 'AP(E)' THEN count(*) ELSE 0 END AS AP_E, " +
                        "CASE WHEN scat.CODE = 'AP(S)' THEN count(*) ELSE 0 END AS AP_S, " +
                        "CASE WHEN scat.CODE = 'API' THEN count(*) ELSE 0 END AS AP_I, " +
                        "CASE WHEN scat.CODE = 'APII' THEN count(*) ELSE 0 END AS AP_II, " +
                        "CASE WHEN scat.CODE = 'APIII' THEN count(*) ELSE 0 END AS AP_III, " +
                        "CASE WHEN scat.CODE = 'RSE' THEN count(*) ELSE 0 END AS RSE, " +
                        "CASE WHEN scat.CODE = 'RGE' THEN count(*) ELSE 0 END AS RGE, " +
                        "CASE WHEN scat.CODE = 'RI(A)' THEN count(*) ELSE 0 END AS RI_A, " +
                        "CASE WHEN scat.CODE = 'RI(E)' THEN count(*) ELSE 0 END AS RI_E, " +
                        "CASE WHEN scat.CODE = 'RI(S)' THEN count(*) ELSE 0 END AS RI_S " +
                        "FROM C_IND_CERTIFICATE icer, C_IND_PROCESS_MONITOR ipm, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp " +
                        "WHERE icer.CATEGORY_ID = scat.UUID " +
                        "AND scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
                        "AND ipm.CATEGORY_ID = icer.CATEGORY_ID ");
            if (!string.IsNullOrEmpty(myDictionary["from_date"].ToString()))
                sql.Append(" And icer.application_date >= to_date(:from_date,'dd/mm/yyyy') ");
            if (!string.IsNullOrEmpty(myDictionary["to_date"].ToString()))
                sql.Append(" And icer.application_date <= to_date(:to_date,'dd/mm/yyyy') ");

            sql.Append(" AND ipm.MASTER_ID = icer.MASTER_ID " +
                        "and scatgrp.code in ('AP', 'RSE', 'RGE', 'RI') " +
                        "AND scat.ACTIVE = 'Y' " +
                                    "GROUP BY scat.code, " +
                        "CASE WHEN ipm.RESULT_ACCEPT_DATE IS NOT NULL THEN 'ACCEPTED' " +

                             "WHEN ipm.RESULT_DEFER_DATE IS NOT NULL THEN 'DEFER' " +

                             "WHEN ipm.RESULT_REFUSE_DATE IS NOT NULL THEN 'REFUSED' END " +

                        "UNION ALL " +

                        "SELECT '' AS code, 'ACCEPTED' AS status, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 FROM DUAL " +
                        "UNION ALL " +
                        "SELECT '' AS code, 'DEFER' AS status, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 FROM DUAL " +
                        "UNION ALL " +
                        "SELECT '' AS code, 'REFUSED' AS status, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 FROM dual " +

                        ") GROUP BY status ORDER BY 1 ");


            //            string sql = "SELECT status, sum(AP_A) AS AP_A, sum(AP_E) AS AP_E, sum(AP_S) AS AP_S, sum(AP_I) AS AP_I, sum(AP_II) AS AP_II, sum(AP_III) AS AP_III, sum(RSE) AS RSE, sum(RGE) AS RGE, sum(RI_A) AS RI_A, sum(RI_E) AS RI_E, sum(RI_S) AS RI_S FROM ( " +

            //"SELECT scat.code, " +
            //"CASE WHEN ipm.RESULT_ACCEPT_DATE IS NOT NULL THEN 'ACCEPTED' " +

            //     "WHEN ipm.RESULT_DEFER_DATE IS NOT NULL THEN 'DEFER' " +

            //     "WHEN ipm.RESULT_REFUSE_DATE IS NOT NULL THEN 'REFUSED' END AS status, " +
            //"CASE WHEN scat.CODE = 'AP(A)' THEN count(*) ELSE 0 END AS AP_A, " +
            //"CASE WHEN scat.CODE = 'AP(E)' THEN count(*) ELSE 0 END AS AP_E, " +
            //"CASE WHEN scat.CODE = 'AP(S)' THEN count(*) ELSE 0 END AS AP_S, " +
            //"CASE WHEN scat.CODE = 'API' THEN count(*) ELSE 0 END AS AP_I, " +
            //"CASE WHEN scat.CODE = 'APII' THEN count(*) ELSE 0 END AS AP_II, " +
            //"CASE WHEN scat.CODE = 'APIII' THEN count(*) ELSE 0 END AS AP_III, " +
            //"CASE WHEN scat.CODE = 'RSE' THEN count(*) ELSE 0 END AS RSE, " +
            //"CASE WHEN scat.CODE = 'RGE' THEN count(*) ELSE 0 END AS RGE, " +
            //"CASE WHEN scat.CODE = 'RI(A)' THEN count(*) ELSE 0 END AS RI_A, " +
            //"CASE WHEN scat.CODE = 'RI(E)' THEN count(*) ELSE 0 END AS RI_E, " +
            //"CASE WHEN scat.CODE = 'RI(S)' THEN count(*) ELSE 0 END AS RI_S " +
            //"FROM C_IND_CERTIFICATE icer, C_IND_PROCESS_MONITOR ipm, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp " +
            //"WHERE icer.CATEGORY_ID = scat.UUID " +
            //"AND scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
            //"AND ipm.CATEGORY_ID = icer.CATEGORY_ID " +
            //"AND ipm.MASTER_ID = icer.MASTER_ID " +
            //"and scatgrp.code in ('AP', 'RSE', 'RGE', 'RI') " +
            //"AND scat.ACTIVE = 'Y' " +
            //            "GROUP BY scat.code, " +
            //"CASE WHEN ipm.RESULT_ACCEPT_DATE IS NOT NULL THEN 'ACCEPTED' " +

            //     "WHEN ipm.RESULT_DEFER_DATE IS NOT NULL THEN 'DEFER' " +

            //     "WHEN ipm.RESULT_REFUSE_DATE IS NOT NULL THEN 'REFUSED' END " +

            //"UNION ALL " +

            //"SELECT '' AS code, 'ACCEPTED' AS status, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 FROM DUAL " +
            //"UNION ALL " +
            //"SELECT '' AS code, 'DEFER' AS status, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 FROM DUAL " +
            //"UNION ALL " +
            //"SELECT '' AS code, 'REFUSED' AS status, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 FROM dual " +

            //") GROUP BY status ORDER BY 1";
            return sql.ToString();
        }

        //CRM0009
        public string getAttendanceMonthSql()
        {
            string sql = "SELECT " +
                "A.APPLN_UUID AS UUID," +
                "c_propercase(A.ENAME) AS ENAME," +
                "CASE WHEN B.COMM_TYPE_CODE IS NULL THEN (SELECT CODE from C_s_system_value a, C_meeting b where b.committee_type_id =a.uuid and b.uuid=C_min_meeting_uuid(:AttendancYear,:AttendancMonth,:reg_type) ) " +
                "ELSE B.COMM_TYPE_CODE END AS COMM_TYPE_CODE," +

                "CASE WHEN B.INTRV_D IS NULL THEN (SELECT to_char(INTERVIEW_DATE, 'DD/MM/YYYY') from C_INTERVIEW_SCHEDULE where meeting_id=C_min_meeting_uuid(:AttendancYear,:AttendancMonth,:reg_type)) " +
                "ELSE B.INTRV_D END AS INTRV_D," +

                "CASE WHEN B.MEETING IS NULL THEN (SELECT  MEETING_GROUP || MEETING_NO ||'/'||substr(year,3,2) from C_meeting where uuid=C_min_meeting_uuid(:AttendancYear,:AttendancMonth,:reg_type)) " +
                "ELSE B.MEETING END AS MEETING," +

                "(CASE WHEN B.IS_CANCEL ='Y' THEN null WHEN B.IS_ABSENT='N' THEN '1' WHEN B.IS_ABSENT='Y' THEN '0' END)AS ABSENT," +
                "(CASE WHEN B.IS_CANCEL ='Y' THEN 0 WHEN B.IS_ABSENT='N' THEN 1 else 0 end) as P," +
                "(CASE WHEN B.IS_CANCEL ='Y' THEN 0 WHEN B.IS_ABSENT='Y' THEN 1 else 0 end) as A," +
                ":AttendancMonth AS MONTH, " +
                ":AttendancYear AS YEAR " +

                "FROM " +
                   "(SELECT APPLICANT_ID AS APPLN_UUID, C_MEM.UUID AS MEMBER_ID,APPLN.SURNAME||' '||APPLN.GIVEN_NAME_ON_ID AS ENAME,C_PNL.PANEL_TYPE_ID AS PANEL_TYPE_ID " +
                    "FROM C_COMMITTEE_PANEL_MEMBER C_PNL_MEM " +
                    "INNER JOIN C_COMMITTEE_MEMBER C_MEM ON C_PNL_MEM.MEMBER_ID = C_MEM.UUID " +
                    "INNER JOIN C_APPLICANT APPLN ON C_MEM.APPLICANT_ID = APPLN.UUID " +
                    "INNER JOIN C_COMMITTEE_PANEL C_PNL ON C_PNL_MEM.COMMITTEE_PANEL_ID = C_PNL.UUID " +
                    "INNER JOIN C_S_SYSTEM_VALUE S_PNL ON C_PNL.PANEL_TYPE_ID = S_PNL.UUID " +
                    "WHERE C_PNL.YEAR = :AttendancYear AND S_PNL.REGISTRATION_TYPE = :reg_type ) A " +
                 "LEFT JOIN " +
                    "(SELECT M_MEM.IS_ABSENT AS IS_ABSENT, to_char(I_SCH.INTERVIEW_DATE,'DD/MM/YYYY')AS INTRV_D,M_MEM.MEMBER_ID AS MEMBER_ID," +
                    "M.YEAR AS M_YEAR,M.MEETING_GROUP||M.MEETING_NO||'/'||substr(M.YEAR,3,2)AS MEETING," +
                    "S_COMM_TYPE.CODE AS COMM_TYPE_CODE,S_PNL_TYPE.UUID AS PANEL_TYPE_ID,I_SCH.IS_CANCEL AS IS_CANCEL " +
                    "FROM " +
                    "C_MEETING_MEMBER M_MEM " +
                    "INNER JOIN C_MEETING M ON M_MEM.MEETING_ID = M.UUID " +
                    "INNER JOIN C_INTERVIEW_SCHEDULE I_SCH ON M.UUID = I_SCH.MEETING_ID " +
                    "INNER JOIN C_S_SYSTEM_VALUE S_COMM_TYPE ON M.COMMITTEE_TYPE_ID = S_COMM_TYPE.UUID " +
                    "INNER JOIN C_S_SYSTEM_VALUE S_PNL_TYPE ON S_COMM_TYPE.PARENT_ID = S_PNL_TYPE.UUID " +
                    "WHERE M_MEM.committee_role_id in " +
                    "( select sv.uuid from C_S_SYSTEM_VALUE sv, C_S_SYSTEM_TYPE st where sv.system_type_id=st.uuid and st.type ='COMMITTEE_ROLE' and sv.code not in ('1','2')) AND " +
                    "to_char(I_SCH.INTERVIEW_DATE,'yyyy') = :AttendancYear " +
                    "AND to_char(I_SCH.INTERVIEW_DATE,'mm') = :AttendancMonth " +
                    //"AND S_COMM_TYPE.REGISTRATION_TYPE = :reg_type " +
                    "AND S_COMM_TYPE.REGISTRATION_TYPE = :reg_type AND S_PNL_TYPE.REGISTRATION_TYPE = :reg_type) B ON A.MEMBER_ID=B.MEMBER_ID and A.PANEL_TYPE_ID = B.PANEL_TYPE_ID " +
                    "ORDER BY ENAME, COMM_TYPE_CODE, MEETING";
            return sql;
        }

        //CRM0010
        private string getAuditLog(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT  LOG.ACTION AS ACTION," +
                         "to_char(LOG.LOG_DATE,'dd/mm/yyyy hh24:mi:ss') AS LOG_DATE, " +
                         "c_propercase(LOG.USERNAME) AS USERNAME," +
                         "to_char(to_date(:fr_date,'dd/mm/yyyy'),'dd/mm/yyyy') AS FR_DATE," +
                         "to_char(to_date(:to_date,'dd/mm/yyyy'),'dd/mm/yyyy') AS TO_DATE " +
                         "FROM " +
                         "C_S_LOG LOG " +
                         "WHERE 1 =1 ");
            if (!string.IsNullOrEmpty(myDictionary["fr_date"].ToString()))
                sql.Append(" And LOG.LOG_DATE >= to_date(:fr_date,'dd/mm/yyyy')");
            if (!string.IsNullOrEmpty(myDictionary["to_date"].ToString()))
                sql.Append(" And LOG.LOG_DATE <= to_date(:to_date,'dd/mm/yyyy')");

            sql.Append(" ORDER BY USERNAME ASC,LOG_DATE DESC ");

            //string sql = "SELECT  LOG.ACTION AS ACTION," +
            //             "to_char(LOG.LOG_DATE,'dd/mm/yyyy hh24:mi:ss') AS LOG_DATE, " +
            //             "c_propercase(LOG.USERNAME) AS USERNAME," +
            //             "to_char(to_date(:fr_date,'ddmmyyyy'),'dd/mm/yyyy') AS FR_DATE," +
            //             "to_char(to_date(:to_date,'ddmmyyyy'),'dd/mm/yyyy') AS TO_DATE " +
            //             "FROM " +
            //             "C_S_LOG LOG " +
            //             "WHERE " +
            //             "LOG.LOG_DATE >= to_date(:fr_date,'ddmmyyyy') " +
            //             "AND LOG.LOG_DATE <= to_date(:to_date, 'ddmmyyyy') " +
            //             //"AND USERNAME='fanny' " +
            //             "ORDER BY USERNAME ASC,LOG_DATE DESC ";
            return sql.ToString();
        }

        //CRM0011
        public string getCMwcSql()
        {
            string sql = "SELECT " +
"C.UUID AS MASTER_ID," +
"C.FILE_REFERENCE_NO," +
"C.CERTIFICATION_NO, C.ENGLISH_COMPANY_NAME , C.CHINESE_COMPANY_NAME," +
"to_char(C.EXPIRY_DATE, 'dd/MM/yyyy') AS EXPIRY_DATE, C.BS_TELEPHONE_NO1 as TELEPHONE_NO," +
"GET_MW_COMP_CLASS_TYPE(C.UUID, 1) AS COMPANY_TYPE_ONE," +
"GET_MW_COMP_CLASS_TYPE(C.UUID, 2) AS COMPANY_TYPE_TWO," +
"GET_MW_COMP_CLASS_TYPE(C.UUID, 3) AS COMPANY_TYPE_THREE," +
"APP_INFO.UUID AS COMP_APPLICANT_INFO_ID," +
"APP.SURNAME , APP.GIVEN_NAME_ON_ID, APP.CHINESE_NAME," +
"GET_MW_COMP_AS_CLASS_TYPE(APP_INFO.UUID, 3) AS AS_TYPE_ONE," +
"GET_MW_COMP_AS_CLASS_TYPE(APP_INFO.UUID, 2) AS AS_TYPE_TWO," +
"GET_MW_COMP_AS_CLASS_TYPE(APP_INFO.UUID, 1) AS AS_TYPE_THREE," +
 "(SELECT REGION_CHI.ENGLISH_DESCRIPTION FROM C_S_SYSTEM_VALUE REGION_CHI WHERE REGION_CHI.UUID = C.BS_REGION_CODE_ID ) " +
 "AS REGION_ENG," +
 "(SELECT REGION_CHI.CHINESE_DESCRIPTION FROM C_S_SYSTEM_VALUE REGION_CHI WHERE REGION_CHI.UUID = C.BS_REGION_CODE_ID ) " +
 "AS REGION_CHI, C.BS_EMAIL_ADDRESS AS EMAIL_ADDRESS, C.BS_FAX_NO1," +
"case when C.CHINESE_COMPANY_NAME is not null then CAST(C.CHINESE_COMPANY_NAME AS VARCHAR2(100)) || ' ' || C.ENGLISH_COMPANY_NAME else C.ENGLISH_COMPANY_NAME end as cn_company_name," +
"to_char(to_date(:as_date,'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_DATE," +
":title AS TITLE " +
"FROM C_COMP_APPLICATION C, C_COMP_APPLICANT_INFO APP_INFO, C_APPLICANT APP, " +
"C_S_SYSTEM_VALUE S_ROLE, C_S_SYSTEM_VALUE S_STATUS, C_S_CATEGORY_CODE CAT " +
"WHERE " +
"C.UUID = APP_INFO.MASTER_ID " +
"AND APP_INFO.APPLICANT_ID = APP.UUID " +
"AND APP_INFO.APPLICANT_ROLE_ID = S_ROLE.UUID " +
"AND APP_INFO.APPLICANT_STATUS_ID = S_STATUS.UUID " +
"AND C.CATEGORY_ID = CAT.UUID " +
"AND C.REGISTRATION_TYPE = 'CMW' " +
"AND S_ROLE.CODE LIKE 'A%' " +
//"AND CAT.CODE = '$P!{category}' "+
"AND S_STATUS.CODE = '1' " +
"AND APP_INFO.accept_date IS NOT NULL " +
"AND((APP_INFO.REMOVAL_DATE IS NULL) OR(APP_INFO.REMOVAL_DATE < CURRENT_DATE) ) " +
"AND C.CERTIFICATION_NO IS NOT NULL AND( " +
"(C.EXPIRY_DATE IS NOT NULL and  C.EXPIRY_DATE >= CURRENT_DATE) or ((C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') " +
"and (C.EXPIRY_DATE < CURRENT_DATE)) ) )  AND((C.REMOVAL_DATE IS NULL) OR " +
"(C.REMOVAL_DATE > CURRENT_DATE)) " +
"ORDER BY upper(C.ENGLISH_COMPANY_NAME), C.CERTIFICATION_NO, C.UUID, APP.SURNAME , APP.GIVEN_NAME_ON_ID, APP_INFO.UUID";
            return sql;
        }

        //CRM0012
        public string getCMwcWSql()
        {
            string sql = "SELECT " +
                "C.CERTIFICATION_NO," +
                " APP.SURNAME," +
                " APP.GIVEN_NAME_ON_ID," +
                " APP.CHINESE_NAME," +
                "to_char(C.EXPIRY_DATE, 'dd/MM/yyyy') AS EXPIRY_DATE," +
                " A.bs_telephone_no1 as TELEPHONE_NO1," +
                "GET_MW_INDIVIDUAL_ITEMS(A.UUID) AS ITEMS," +
                "(SELECT REGION_CHI.ENGLISH_DESCRIPTION FROM C_S_SYSTEM_VALUE  REGION_CHI WHERE REGION_CHI.UUID = A.BS_REGION_CODE_ID ) AS REGION_ENG," +
                "(SELECT REGION_CHI.CHINESE_DESCRIPTION FROM C_S_SYSTEM_VALUE REGION_CHI WHERE REGION_CHI.UUID = A.BS_REGION_CODE_ID ) AS REGION_CHI," +
                "A.BS_EMAIL AS EMAIL," +
                "A.BS_FAX_NO1," +
                "case when CHINESE_NAME is not null then CAST(CHINESE_NAME AS VARCHAR2(100)) || ' ' || SURNAME || ' ' || GIVEN_NAME_ON_ID else SURNAME || ' ' || GIVEN_NAME_ON_ID end as cn_name," +
                "to_char(to_date(:as_date,'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_DATE," +
                ":title AS TITLE " +
        "FROM C_IND_CERTIFICATE C, C_APPLICANT APP, C_IND_APPLICATION A, C_S_SYSTEM_VALUE SV " +
        "WHERE C.MASTER_ID = A.UUID " +
        "AND A.APPLICANT_ID = APP.UUID " +
        "AND A.REGISTRATION_TYPE = 'IMW' " +
        "AND SV.UUID = C.application_status_id " +
        "AND SV.CODE = '1' " +
        "AND C.CERTIFICATION_NO IS NOT NULL " +
        "AND ((C.EXPIRY_DATE IS NOT NULL and C.EXPIRY_DATE >= CURRENT_DATE) or (C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(C.EXPIRY_DATE < CURRENT_DATE))) " +
        "AND ((C.REMOVAL_DATE IS NULL) OR(C.REMOVAL_DATE > CURRENT_DATE)) " +
        "ORDER BY upper(APP.SURNAME) || ' ' || upper(APP.GIVEN_NAME_ON_ID) ,C.CERTIFICATION_NO, C.UUID";
            return sql;
        }

        //CRM0013
        public string getCQpMwcSql()
        {
            string sql = "SELECT " +
"C.UUID AS MASTER_ID,C.FILE_REFERENCE_NO,C.CERTIFICATION_NO, C.ENGLISH_COMPANY_NAME , C.CHINESE_COMPANY_NAME," +
"to_char(C.EXPIRY_DATE, 'dd/MM/yyyy') as EXPIRY_DATE,  C.BS_TELEPHONE_NO1 as TELEPHONE_NO," +
"GET_MW_COMP_CLASS_TYPE(C.UUID, 1) AS COMPANY_TYPE_ONE," +
"GET_MW_COMP_CLASS_TYPE(C.UUID, 2) AS COMPANY_TYPE_TWO," +
"GET_MW_COMP_CLASS_TYPE(C.UUID, 3) AS COMPANY_TYPE_THREE," +
"APP_INFO.UUID AS COMP_APPLICANT_INFO_ID," +
"APP.SURNAME , APP.GIVEN_NAME_ON_ID, APP.CHINESE_NAME," +
"GET_MW_COMP_AS_CLASS_TYPE(APP_INFO.UUID, 3) AS AS_TYPE_ONE," +
"GET_MW_COMP_AS_CLASS_TYPE(APP_INFO.UUID, 2) AS AS_TYPE_TWO," +
"GET_MW_COMP_AS_CLASS_TYPE(APP_INFO.UUID, 1) AS AS_TYPE_THREE," +
 "(SELECT REGION_CHI.ENGLISH_DESCRIPTION FROM C_S_SYSTEM_VALUE REGION_CHI WHERE REGION_CHI.UUID = C.BS_REGION_CODE_ID ) AS REGION_ENG," +
 "(SELECT REGION_CHI.CHINESE_DESCRIPTION FROM C_S_SYSTEM_VALUE REGION_CHI WHERE REGION_CHI.UUID = C.BS_REGION_CODE_ID )  AS REGION_CHI," +
 " C.BS_EMAIL_ADDRESS AS EMAIL_ADDRESS, C.BS_FAX_NO1," +
" case when C.CHINESE_COMPANY_NAME is not null then CAST(C.CHINESE_COMPANY_NAME AS VARCHAR2(100)) || ' ' || C.ENGLISH_COMPANY_NAME else C.ENGLISH_COMPANY_NAME end as cn_company_name," +
"to_char(to_date(:as_date,'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_DATE,  " +
":title AS TITLE " +
"FROM C_COMP_APPLICATION C," +
"C_COMP_APPLICANT_INFO APP_INFO, C_APPLICANT APP,C_S_SYSTEM_VALUE S_ROLE, C_S_SYSTEM_VALUE S_STATUS " +
"WHERE " +
"C.UUID = APP_INFO.MASTER_ID " +
"AND APP_INFO.APPLICANT_ID = APP.UUID " +
"AND APP_INFO.APPLICANT_ROLE_ID = S_ROLE.UUID " +
"AND APP_INFO.APPLICANT_STATUS_ID = S_STATUS.UUID " +
"AND C.REGISTRATION_TYPE = 'CMW' " +
"AND S_ROLE.CODE LIKE 'A%' " +
"AND S_STATUS.CODE = '1' " +
"AND APP_INFO.accept_date IS NOT NULL " +
"AND((APP_INFO.REMOVAL_DATE IS NULL) OR(APP_INFO.REMOVAL_DATE < CURRENT_DATE) ) " +
"AND C.CERTIFICATION_NO IS NOT NULL AND(  " +
"(C.EXPIRY_DATE IS NOT NULL and  C.EXPIRY_DATE >= CURRENT_DATE) or " +
"((C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and " +
"(C.EXPIRY_DATE < CURRENT_DATE)) ) )  AND((C.REMOVAL_DATE IS NULL) OR " +
"(C.REMOVAL_DATE > CURRENT_DATE)) and APP_INFO.uuid in ( " +
"select cinfom.COMPANY_APPLICANTS_ID from C_COMP_APPLICANT_INFO_MASTER cinfom " +
"inner join C_COMP_APPLICANT_INFO_DETAIL cinfod on cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.uuid " +
"inner join C_s_system_value mwcode on cinfod.ITEM_TYPE_ID = mwcode.uuid " +
"where mwcode.code in('Type A')) " +
"ORDER BY upper(C.ENGLISH_COMPANY_NAME), C.CERTIFICATION_NO, C.UUID, APP.SURNAME , APP.GIVEN_NAME_ON_ID, APP_INFO.UUID";
            return sql;
        }

        //CRM0014
        public string getCaseInHandSql(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT main.uuid, REGISTRATION_TYPE, UPPER(VETTING_OFFICER) as VETTING_OFFICER, FILE_REFERENCE_NO," +
                "to_char(RECE_DATE, 'dd/MM/yyyy') AS RECEIVED_DATE," +
                "to_char(DUE_DATE, 'dd/MM/yyyy') AS DUE_DATE, code," +
                "to_char(INTERVIEW_DATE, 'dd/MM/yyyy') AS INTERVIEW_DATE," +
                "category, cat_grp FROM( " +
                "SELECT * FROM( " +
                "SELECT capp.UUID, capp.REGISTRATION_TYPE, cpm.VETTING_OFFICER, capp.FILE_REFERENCE_NO," +
                "cpm.RECEIVED_DATE AS RECE_DATE, CASE WHEN cpm.MONITOR_TYPE = 'UPM_10DAYS' THEN cpm.PLEDGE_DUE_10_DAYS_DATE " +
                "WHEN cpm.MONITOR_TYPE = 'FaskTrack' THEN cpm.FAST_TRACK_DUE_28_DAYS_DATE " +
                "ELSE cpm.DUE_DATE END AS DUE_DATE," +
                "sform.CODE, cpm.INTERVIEW_DATE, scat.CODE AS category, scat_grp.CODE AS cat_grp " +
                "FROM C_COMP_PROCESS_MONITOR cpm " +
                "LEFT JOIN C_COMP_APPLICATION capp ON cpm.MASTER_ID = capp.uuid " +
                "LEFT JOIN C_S_CATEGORY_CODE scat ON scat.UUID = capp.CATEGORY_ID " +
                "LEFT JOIN C_S_SYSTEM_VALUE sform ON sform.UUID = cpm.APPLICATION_FORM_ID " +
                "LEFT JOIN C_S_SYSTEM_VALUE scat_grp ON scat.CATEGORY_GROUP_ID = scat_grp.UUID " +
                "UNION ALL " +
                " SELECT iapp.UUID, iapp.REGISTRATION_TYPE, ipm.VETTING_OFFICER, iapp.FILE_REFERENCE_NO," +
                "ipm.RECEIVED_DATE, ipm.DUE_DATE, sform.CODE, ipm.INTERVIEW_DATE,scat.CODE AS category, scat_grp.CODE AS cat_grp " +
                "FROM C_IND_PROCESS_MONITOR ipm " +
                "LEFT JOIN C_IND_APPLICATION iapp ON ipm.MASTER_ID = iapp.UUID " +
                "LEFT JOIN C_S_SYSTEM_VALUE sform ON sform.UUID = ipm.APPLICATION_FORM_ID " +
                "LEFT JOIN C_S_CATEGORY_CODE scat ON scat.UUID = ipm.CATEGORY_ID " +
                "LEFT JOIN C_S_SYSTEM_VALUE scat_grp ON scat.CATEGORY_GROUP_ID = scat_grp.UUID )) main Where 1 = 1 ");
            foreach (var item in myDictionary)
            {
                switch (item.Key)
                {
                    case "FileRef":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And FILE_REFERENCE_NO = :FileRef ");
                        break;
                    case "VettingOfficer":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And VETTING_OFFICER = :VettingOfficer ");
                        break;
                    case "DateFromReceived":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And RECE_DATE >= to_date(:DateFromReceived,'dd/MM/yyyy') ");
                        break;
                    case "DateToReceived":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And RECE_DATE <= to_date(:DateToReceived,'dd/MM/yyyy') ");
                        break;
                    case "DateFromDue":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And DUE_DATE >= to_date(:DateFromDue,'dd/MM/yyyy') ");
                        break;
                    case "DateToDue":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And DUE_DATE <= to_date(:DateToDue,'dd/MM/yyyy') ");
                        break;
                    case "TypeOfRegisters":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And category = :TypeOfRegisters ");
                        break;
                    case "application_type":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And code = :application_type ");
                        break;

                }
            }



            sql.Append(" ORDER BY UPPER(VETTING_OFFICER), RECE_DATE ");
            return sql.ToString();
        }

        //CRM0015
        public string getEMwcWSql()
        {
            string sql = "SELECT C.CERTIFICATION_NO, APP.SURNAME, APP.GIVEN_NAME_ON_ID, APP.CHINESE_NAME," +
                "to_char(C.EXPIRY_DATE, 'dd/MM/yyyy') AS EXPIRY_DATE, A.bs_telephone_no1 as TELEPHONE_NO1," +
                "GET_MW_INDIVIDUAL_ITEMS(A.UUID) AS ITEMS," +
                "(SELECT REGION_CHI.ENGLISH_DESCRIPTION FROM C_S_SYSTEM_VALUE  REGION_CHI WHERE REGION_CHI.UUID = A.BS_REGION_CODE_ID ) AS REGION_ENG," +
                "(SELECT REGION_CHI.CHINESE_DESCRIPTION FROM C_S_SYSTEM_VALUE REGION_CHI " +
                "WHERE REGION_CHI.UUID = A.BS_REGION_CODE_ID ) AS REGION_CHI,A.BS_EMAIL AS EMAIL, A.BS_FAX_NO1," +
                "to_char(to_date(:as_date, 'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_DATE," +
                ":title AS TITLE " +
                "FROM  C_IND_CERTIFICATE C, C_APPLICANT APP, C_IND_APPLICATION A, C_S_SYSTEM_VALUE SV " +
                "WHERE  C.MASTER_ID = A.UUID AND A.APPLICANT_ID = APP.UUID " +
                "AND A.REGISTRATION_TYPE = 'IMW' AND SV.UUID = C.application_status_id " +
                "AND SV.CODE = '1' AND C.CERTIFICATION_NO IS NOT NULL AND " +
                "((C.EXPIRY_DATE IS NOT NULL and C.EXPIRY_DATE >= CURRENT_DATE) or " +
                "(C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(C.EXPIRY_DATE < CURRENT_DATE))) " +
                "AND ((C.REMOVAL_DATE IS NULL) OR(C.REMOVAL_DATE > CURRENT_DATE)) " +
                "ORDER BY upper(APP.SURNAME) || ' ' || upper(APP.GIVEN_NAME_ON_ID) ,  C.CERTIFICATION_NO, C.UUID";
            return sql;
        }

        //CRM0016
        public string getEQpMwcWSql()
        {
            string sql = "SELECT C.CERTIFICATION_NO, APP.SURNAME, APP.GIVEN_NAME_ON_ID, APP.CHINESE_NAME," +
                "to_char(C.EXPIRY_DATE, 'dd/MM/yyyy') as EXPIRY_DATE, A.bs_telephone_no1 as TELEPHONE_NO1," +
                "CAST('3.6' AS VARCHAR(10)) AS ITEMS," +
                "(SELECT REGION_CHI.ENGLISH_DESCRIPTION FROM C_S_SYSTEM_VALUE  REGION_CHI WHERE REGION_CHI.UUID = A.BS_REGION_CODE_ID ) AS REGION_ENG," +
                "(SELECT REGION_CHI.CHINESE_DESCRIPTION FROM C_S_SYSTEM_VALUE REGION_CHI " +
                "WHERE REGION_CHI.UUID = A.BS_REGION_CODE_ID ) AS REGION_CHI," +
                "A.BS_EMAIL AS EMAIL, A.BS_FAX_NO1," +
                "case when CHINESE_NAME is not null then CAST(CHINESE_NAME AS VARCHAR2(100)) || ' ' || SURNAME || ' ' || GIVEN_NAME_ON_ID else SURNAME || ' ' || GIVEN_NAME_ON_ID end as cn_name," +
                "to_char(to_date(:as_date, 'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_DATE," +
                ":title AS TITLE " +
                "FROM C_IND_CERTIFICATE C, C_APPLICANT APP, C_IND_APPLICATION A, C_S_SYSTEM_VALUE SV " +
                "WHERE C.MASTER_ID = A.UUID AND A.APPLICANT_ID = APP.UUID AND A.REGISTRATION_TYPE = 'IMW' AND SV.UUID = C.application_status_id " +
                "AND SV.CODE = '1' AND C.CERTIFICATION_NO IS NOT NULL AND ((C.EXPIRY_DATE IS NOT NULL and C.EXPIRY_DATE >= CURRENT_DATE) or " +
                "(C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(C.EXPIRY_DATE < CURRENT_DATE))) " +
                "AND ((C.REMOVAL_DATE IS NULL) OR(C.REMOVAL_DATE > CURRENT_DATE)) AND A.UUID IN(" +
                "SELECT distinct iitem.MASTER_ID FROM C_ind_application_mw_item iitem, C_s_system_value sitem " +
                "WHERE iitem.ITEM_DETAILS_ID = sitem.UUID " +
                "AND sitem.code in ('Item 3.6')) " +
                "ORDER BY upper(APP.SURNAME) || ' ' || upper(APP.GIVEN_NAME_ON_ID) ,  C.CERTIFICATION_NO, C.UUID";
            return sql;
        }

        //CRM0017
        public string getExpiryRegistersIPSql()
        {
            string sql = "SELECT S_CAT_GP.CODE AS CAT_GP_CODE,S_CAT.CODE AS CAT_CODE,I_CERT.UUID AS MASTER_ID," +
                "CASE WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '01' THEN '01-January' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '02' THEN '02-February' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '03' THEN '03-March' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '04' THEN '04-April' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '05' THEN '05-May' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY')  " +
                "WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '06' THEN '06-June' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '07' THEN '07-July' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '08' THEN '08-August' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '09' THEN '09-September' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '10' THEN '10-October' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '11' THEN '11-November' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.EXPIRY_DATE, 'MM') = '12' THEN '12-December' || ' ' || to_char(I_CERT.EXPIRY_DATE, 'YYYY') " +
                "END AS EXP_MY,to_char(I_CERT.EXPIRY_DATE, 'YYYY') AS EXP_Y,to_char (I_CERT.EXPIRY_DATE, 'MM') AS EXP_M," +
                " '(as at ' || " +
                "CASE WHEN substr(:today,0,2) = '01' THEN '1' " +
                "WHEN substr(:today,0,2) = '02' THEN '2' " +
                "WHEN substr(:today,0,2) = '03' THEN '3' " +
                "WHEN substr(:today,0,2) = '04' THEN '4' " +
                "WHEN substr(:today,0,2) = '05' THEN '5' " +
                "WHEN substr(:today,0,2) = '06' THEN '6' " +
                "WHEN substr(:today,0,2) = '07' THEN '7' " +
                "WHEN substr(:today,0,2) = '08' THEN '8' " +
                "WHEN substr(:today,0,2) = '09' THEN '9' " +
                "ELSE substr(:today,0,2) END || ' ' || " +
                "CASE " +
                "WHEN substr(:today,3,2) = '01' THEN 'January' " +
                "WHEN substr(:today,3,2) = '02' THEN 'February' " +
                "WHEN substr(:today,3,2) = '03' THEN 'March' " +
                "WHEN substr(:today,3,2) = '04' THEN 'April' " +
                "WHEN substr(:today,3,2) = '05' THEN 'May' " +
                "WHEN substr(:today,3,2) = '06' THEN 'June' " +
                "WHEN substr(:today,3,2) = '07' THEN 'July' " +
                "WHEN substr(:today,3,2) = '08' THEN 'August' " +
                "WHEN substr(:today,3,2) = '09' THEN 'September' " +
                "WHEN substr(:today,3,2) = '10' THEN 'October' " +
                "WHEN substr(:today,3,2) = '11' THEN 'November' " +
                "ELSE 'December' END || ' ' || substr(:today, 5, 7) || ' )' AS AS_AT_TODAY " +
                "FROM C_IND_APPLICATION I_APPL " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_APP_FORM ON I_CERT.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE S_APP_FORM.REGISTRATION_TYPE = :reg_type " +
                "AND S_CAT_GP.REGISTRATION_TYPE = :reg_type " +
                "and to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE, 'YYYYMMDD') " +
                "and(I_CERT.REMOVAL_DATE is null or(to_char(I_CERT.REMOVAL_DATE, 'YYYYMMDD') > to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD'))) " +
                "ORDER BY to_NUMBER(EXP_Y), to_NUMBER(EXP_M)";
            return sql;
        }

        //CRM0018
        public string getCountExpiryDateQpSql()
        {
            string sql = "select rownum,a.* from (" +
                "SELECT S_CAT_GP.CODE AS CAT_GP_CODE, S_CAT.CODE AS CAT_CODE, I_CERT.UUID AS MASTER_ID," +
                "CASE " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '01' THEN '01-January' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '02' THEN '02-February' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '03' THEN '03-March' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '04' THEN '04-April' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '05' THEN '05-May' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '06' THEN '06-June' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '07' THEN '07-July' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '08' THEN '08-August' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '09' THEN '09-September' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '10' THEN '10-October' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '11' THEN '11-November' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_EXPIRY_DATE, 'MM') = '12' THEN '12-December' || ' ' || to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') END AS EXP_MY," +
                "to_char(I_CERT.CARD_EXPIRY_DATE, 'YYYY') AS EXP_Y,to_char (I_CERT.CARD_EXPIRY_DATE, 'MM') AS EXP_M," +
                "'(as at ' || " +
                "CASE WHEN substr(:today,0,2) = '01' THEN '1' " +
                "WHEN substr(:today,0,2) = '02' THEN '2' " +
                "WHEN substr(:today,0,2) = '03' THEN '3' " +
                "WHEN substr(:today,0,2) = '04' THEN '4' " +
                "WHEN substr(:today,0,2) = '05' THEN '5' " +
                "WHEN substr(:today,0,2) = '06' THEN '6' " +
                "WHEN substr(:today,0,2) = '07' THEN '7' " +
                "WHEN substr(:today,0,2) = '08' THEN '8' " +
                "WHEN substr(:today,0,2) = '09' THEN '9' " +
                "ELSE substr(:today,0,2) END || ' ' || " +
                "CASE " +
                "WHEN substr(:today,3,2) = '01' THEN 'January' " +
                "WHEN substr(:today,3,2) = '02' THEN 'February' " +
                "WHEN substr(:today,3,2) = '03' THEN 'March' " +
                "WHEN substr(:today,3,2) = '04' THEN 'April' " +
                "WHEN substr(:today,3,2) = '05' THEN 'May' " +
                "WHEN substr(:today,3,2) = '06' THEN 'June' " +
                "WHEN substr(:today,3,2) = '07' THEN 'July' " +
                "WHEN substr(:today,3,2) = '08' THEN 'August' " +
                "WHEN substr(:today,3,2) = '09' THEN 'September' " +
                "WHEN substr(:today,3,2) = '10' THEN 'October' " +
                "WHEN substr(:today,3,2) = '11' THEN 'November' " +
                "ELSE 'December' END || ' ' || substr(:today, 5, 7) || ' )' AS AS_AT_TODAY " +
                "FROM C_IND_APPLICATION I_APPL " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE 1 = 1 AND I_CERT.CARD_SERIAL_NO is not null AND I_CERT.CARD_EXPIRY_DATE is not null " +
                "UNION ALL( SELECT S_CAT_GP.CODE AS CAT_GP_CODE, S_CAT.CODE AS CAT_CODE, C_INFO.UUID AS MASTER_ID," +
                "CASE WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '01' THEN '01-January' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '02' THEN '02-February' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '03' THEN '03-March' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '04' THEN '04-April' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '05' THEN '05-May' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY')  " +
                "WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '06' THEN '06-June' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '07' THEN '07-July' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '08' THEN '08-August' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '09' THEN '09-September' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '10' THEN '10-October' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '11' THEN '11-November' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_EXPIRY_DATE, 'MM') = '12' THEN '12-December' || ' ' || to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') END AS EXP_MY, " +
                "to_char(C_INFO.CARD_EXPIRY_DATE, 'YYYY') AS EXP_Y, " +
                "to_char (C_INFO.CARD_EXPIRY_DATE, 'MM') AS EXP_M," +
                "'(as at ' || " +
                "CASE WHEN substr(:today,0,2) = '01' THEN '1' " +
                "WHEN substr(:today,0,2) = '02' THEN '2' " +
                "WHEN substr(:today,0,2) = '03' THEN '3' " +
                "WHEN substr(:today,0,2) = '04' THEN '4' " +
                "WHEN substr(:today,0,2) = '05' THEN '5' " +
                "WHEN substr(:today,0,2) = '06' THEN '6' " +
                "WHEN substr(:today,0,2) = '07' THEN '7' " +
                "WHEN substr(:today,0,2) = '08' THEN '8' " +
                "WHEN substr(:today,0,2) = '09' THEN '9' " +
                "ELSE substr(:today,0,2) END || ' ' || " +
                "CASE " +
                "WHEN substr(:today,3,2) = '01' THEN 'January' " +
                "WHEN substr(:today,3,2) = '02' THEN 'February' " +
                "WHEN substr(:today,3,2) = '03' THEN 'March' " +
                "WHEN substr(:today,3,2) = '04' THEN 'April' " +
                "WHEN substr(:today,3,2) = '05' THEN 'May' " +
                "WHEN substr(:today,3,2) = '06' THEN 'June' " +
                "WHEN substr(:today,3,2) = '07' THEN 'July' " +
                "WHEN substr(:today,3,2) = '08' THEN 'August' " +
                "WHEN substr(:today,3,2) = '09' THEN 'September' " +
                "WHEN substr(:today,3,2) = '10' THEN 'October' " +
                "WHEN substr(:today,3,2) = '11' THEN 'November' " +
                "ELSE 'December' END || ' ' || substr(:today, 5, 7) || ' )' AS AS_AT_TODAY " +
                "FROM " +
                "C_COMP_APPLICATION C_APPL " +
                "INNER JOIN C_COMP_APPLICANT_INFO C_INFO ON C_APPL.UUID = C_INFO.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON C_APPL.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE 1 = 1 " +
                "AND C_INFO.CARD_SERIAL_NO is not null " +
                "AND C_INFO.CARD_EXPIRY_DATE is not null)) a WHERE 1 = 1 ORDER BY to_NUMBER(EXP_Y), to_NUMBER(EXP_M),ROWNUM";
            return sql;
        }

        //CRM0019
        public string getCountIrSql()
        {
            string sql = "select * from(" +
                "select prb_code, qcode, code, english_description, count(*) as count," +
                ":date_type_txt AS DATE_TYPE_TXT,:search_date_from AS SEARCH_DATE_FROM,:search_date_to AS SEARCH_DATE_TO,:active AS ACTIVE " +
                "from(" +
                "select distinct iapp.uuid, sqcode.code as qcode, scatd.code, prb.code as prb_code, scatd.english_description " +
                "from C_ind_qualification iq " +
                "left join C_ind_qualification_detail iqd on iqd.ind_qualification_id = iq.uuid " +
                "left join C_s_system_value prb on prb.uuid = iq.PRB_ID " +
                "left join C_ind_application iapp on iapp.uuid = iq.master_id " +
                "left join C_ind_certificate icer on iapp.uuid = icer.master_id " +
                "left join C_s_category_code scat on icer.CATEGORY_ID = scat.UUID " +
                "left join C_s_category_code sqcode on sqcode.uuid = iq.CATEGORY_ID " +
                "left join C_s_category_code_detail scatd on scatd.uuid = iqd.S_CATEGORY_CODE_DETAIL_ID where scat.code = 'IR' ) " +
                "group by prb_code, qcode, code, english_description " +
                "order by prb_code, qcode, english_description) " +
                "union all " +
                "select '', '', '', scat.code, count(*)," +
                ":date_type_txt AS DATE_TYPE_TXT,:search_date_from AS SEARCH_DATE_FROM,:search_date_to AS SEARCH_DATE_TO,:active AS ACTIVE " +
                "from C_ind_certificate icer, C_s_category_code scat " +
                "where icer.CATEGORY_ID = scat.uuid " +
                "and code = 'IR' " +
                "group by scat.code";
            return sql;
        }

        //CRM0020
        public string getCountIssueDateQpCardAll()
        {
            string sql = "select * from ( " +
                "SELECT S_CAT_GP.CODE AS CAT_GP_CODE, S_CAT.CODE AS CAT_CODE, I_CERT.UUID AS MASTER_ID," +
                "CASE " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '01' THEN '01-January' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '02' THEN '02-February' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '03' THEN '03-March' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '04' THEN '04-April' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '05' THEN '05-May' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '06' THEN '06-June' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '07' THEN '07-July' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '08' THEN '08-August' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '09' THEN '09-September' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '10' THEN '10-October' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '11' THEN '11-November' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '12' THEN '12-December' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') END AS ISSUE_MY, " +
                "to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') AS ISSUE_Y,to_char (I_CERT.CARD_ISSUE_DATE, 'MM') AS ISSUE_M," +
                "'(as at ' || " +
                "CASE WHEN substr(:today,0,2) = '01' THEN '1' " +
                "WHEN substr(:today,0,2) = '02' THEN '2' " +
                "WHEN substr(:today,0,2) = '03' THEN '3' " +
                "WHEN substr(:today,0,2) = '04' THEN '4' " +
                "WHEN substr(:today,0,2) = '05' THEN '5' " +
                "WHEN substr(:today,0,2) = '06' THEN '6' " +
                "WHEN substr(:today,0,2) = '07' THEN '7' " +
                "WHEN substr(:today,0,2) = '08' THEN '8' " +
                "WHEN substr(:today,0,2) = '09' THEN '9' " +
                "ELSE substr(:today,0,2) END || ' ' || " +
                "CASE " +
                "WHEN substr(:today,3,2) = '01' THEN 'January' " +
                "WHEN substr(:today,3,2) = '02' THEN 'February' " +
                "WHEN substr(:today,3,2) = '03' THEN 'March' " +
                "WHEN substr(:today,3,2) = '04' THEN 'April' " +
                "WHEN substr(:today,3,2) = '05' THEN 'May' " +
                "WHEN substr(:today,3,2) = '06' THEN 'June' " +
                "WHEN substr(:today,3,2) = '07' THEN 'July' " +
                "WHEN substr(:today,3,2) = '08' THEN 'August' " +
                "WHEN substr(:today,3,2) = '09' THEN 'September' " +
                "WHEN substr(:today,3,2) = '10' THEN 'October' " +
                "WHEN substr(:today,3,2) = '11' THEN 'November' " +
                "ELSE 'December' END || ' ' || substr(:today, 5, 7) || ' )' AS AS_AT_TODAY " +
                "FROM C_IND_APPLICATION I_APPL " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE 1 = 1 " +
                "AND I_CERT.CARD_SERIAL_NO is not null " +
                "AND I_CERT.CARD_ISSUE_DATE is not null " +
                "UNION ALL( " +
                "SELECT S_CAT_GP.CODE AS CAT_GP_CODE, S_CAT.CODE AS CAT_CODE, C_INFO.UUID AS MASTER_ID," +
                "CASE WHEN to_char (C_INFO.CARD_ISSUE_DATE, 'MM') = '01' THEN '01-January' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '02' THEN '02-February' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '03' THEN '03-March' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '04' THEN '04-April' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '05' THEN '05-May' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '06' THEN '06-June' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '07' THEN '07-July' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '08' THEN '08-August' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '09' THEN '09-September' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '10' THEN '10-October' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '11' THEN '11-November' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '12' THEN '12-December' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') END AS ISSUE_MY, " +
                "to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') AS ISSUE_Y, " +
                "to_char (C_INFO.CARD_ISSUE_DATE, 'MM') AS ISSUE_M," +
                "'(as at ' || " +
                "CASE WHEN substr(:today,0,2) = '01' THEN '1' " +
                "WHEN substr(:today,0,2) = '02' THEN '2' " +
                "WHEN substr(:today,0,2) = '03' THEN '3' " +
                "WHEN substr(:today,0,2) = '04' THEN '4' " +
                "WHEN substr(:today,0,2) = '05' THEN '5' " +
                "WHEN substr(:today,0,2) = '06' THEN '6' " +
                "WHEN substr(:today,0,2) = '07' THEN '7' " +
                "WHEN substr(:today,0,2) = '08' THEN '8' " +
                "WHEN substr(:today,0,2) = '09' THEN '9' " +
                "ELSE substr(:today,0,2) END || ' ' || " +
                "CASE " +
                "WHEN substr(:today,3,2) = '01' THEN 'January' " +
                "WHEN substr(:today,3,2) = '02' THEN 'February' " +
                "WHEN substr(:today,3,2) = '03' THEN 'March' " +
                "WHEN substr(:today,3,2) = '04' THEN 'April' " +
                "WHEN substr(:today,3,2) = '05' THEN 'May' " +
                "WHEN substr(:today,3,2) = '06' THEN 'June' " +
                "WHEN substr(:today,3,2) = '07' THEN 'July' " +
                "WHEN substr(:today,3,2) = '08' THEN 'August' " +
                "WHEN substr(:today,3,2) = '09' THEN 'September' " +
                "WHEN substr(:today,3,2) = '10' THEN 'October' " +
                "WHEN substr(:today,3,2) = '11' THEN 'November' " +
                "ELSE 'December' END || ' ' || substr(:today, 5, 7) || ' )' AS AS_AT_TODAY " +
                "FROM C_COMP_APPLICATION C_APPL " +
                "INNER JOIN C_COMP_APPLICANT_INFO C_INFO ON C_APPL.UUID = C_INFO.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON C_APPL.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE 1 = 1 " +
                "AND C_INFO.CARD_SERIAL_NO is not null " +
                " AND C_INFO.CARD_ISSUE_DATE is not null))  " +
                "WHERE 1 = 1 " +
                "ORDER BY to_NUMBER(ISSUE_Y), to_NUMBER(ISSUE_M)";
            return sql;
        }

        //CRM0021
        public string getCountIssuedQpCardAll()
        {
            string sql = "select * from ( " +
                "SELECT S_CAT_GP.CODE AS CAT_GP_CODE, S_CAT.CODE AS CAT_CODE, I_CERT.UUID AS MASTER_ID," +
                "CASE " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '01' THEN '01-January' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '02' THEN '02-February' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '03' THEN '03-March' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '04' THEN '04-April' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '05' THEN '05-May' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '06' THEN '06-June' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '07' THEN '07-July' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '08' THEN '08-August' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '09' THEN '09-September' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '10' THEN '10-October' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '11' THEN '11-November' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_ISSUE_DATE, 'MM') = '12' THEN '12-December' || ' ' || to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') END AS ISSUE_MY, " +
                "to_char(I_CERT.CARD_ISSUE_DATE, 'YYYY') AS ISSUE_Y,to_char (I_CERT.CARD_ISSUE_DATE, 'MM') AS ISSUE_M," +
                "'(as at ' || " +
                "CASE WHEN substr(:today,0,2) = '01' THEN '1' " +
                "WHEN substr(:today,0,2) = '02' THEN '2' " +
                "WHEN substr(:today,0,2) = '03' THEN '3' " +
                "WHEN substr(:today,0,2) = '04' THEN '4' " +
                "WHEN substr(:today,0,2) = '05' THEN '5' " +
                "WHEN substr(:today,0,2) = '06' THEN '6' " +
                "WHEN substr(:today,0,2) = '07' THEN '7' " +
                "WHEN substr(:today,0,2) = '08' THEN '8' " +
                "WHEN substr(:today,0,2) = '09' THEN '9' " +
                "ELSE substr(:today,0,2) END || ' ' || " +
                "CASE " +
                "WHEN substr(:today,3,2) = '01' THEN 'January' " +
                "WHEN substr(:today,3,2) = '02' THEN 'February' " +
                "WHEN substr(:today,3,2) = '03' THEN 'March' " +
                "WHEN substr(:today,3,2) = '04' THEN 'April' " +
                "WHEN substr(:today,3,2) = '05' THEN 'May' " +
                "WHEN substr(:today,3,2) = '06' THEN 'June' " +
                "WHEN substr(:today,3,2) = '07' THEN 'July' " +
                "WHEN substr(:today,3,2) = '08' THEN 'August' " +
                "WHEN substr(:today,3,2) = '09' THEN 'September' " +
                "WHEN substr(:today,3,2) = '10' THEN 'October' " +
                "WHEN substr(:today,3,2) = '11' THEN 'November' " +
                "ELSE 'December' END || ' ' || substr(:today, 5, 7) || ' )' AS AS_AT_TODAY " +
                "FROM C_IND_APPLICATION I_APPL " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE 1 = 1 " +
                "AND I_CERT.CARD_SERIAL_NO is not null " +
                "AND I_CERT.CARD_ISSUE_DATE is not null " +
                "UNION ALL( " +
                "SELECT S_CAT_GP.CODE AS CAT_GP_CODE, S_CAT.CODE AS CAT_CODE, C_INFO.UUID AS MASTER_ID," +
                "CASE WHEN to_char (C_INFO.CARD_ISSUE_DATE, 'MM') = '01' THEN '01-January' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '02' THEN '02-February' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '03' THEN '03-March' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '04' THEN '04-April' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '05' THEN '05-May' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '06' THEN '06-June' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '07' THEN '07-July' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '08' THEN '08-August' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '09' THEN '09-September' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '10' THEN '10-October' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '11' THEN '11-November' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_ISSUE_DATE, 'MM') = '12' THEN '12-December' || ' ' || to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') END AS ISSUE_MY, " +
                "to_char(C_INFO.CARD_ISSUE_DATE, 'YYYY') AS ISSUE_Y, " +
                "to_char (C_INFO.CARD_ISSUE_DATE, 'MM') AS ISSUE_M," +
                "'(as at ' || " +
                "CASE WHEN substr(:today,0,2) = '01' THEN '1' " +
                "WHEN substr(:today,0,2) = '02' THEN '2' " +
                "WHEN substr(:today,0,2) = '03' THEN '3' " +
                "WHEN substr(:today,0,2) = '04' THEN '4' " +
                "WHEN substr(:today,0,2) = '05' THEN '5' " +
                "WHEN substr(:today,0,2) = '06' THEN '6' " +
                "WHEN substr(:today,0,2) = '07' THEN '7' " +
                "WHEN substr(:today,0,2) = '08' THEN '8' " +
                "WHEN substr(:today,0,2) = '09' THEN '9' " +
                "ELSE substr(:today,0,2) END || ' ' || " +
                "CASE " +
                "WHEN substr(:today,3,2) = '01' THEN 'January' " +
                "WHEN substr(:today,3,2) = '02' THEN 'February' " +
                "WHEN substr(:today,3,2) = '03' THEN 'March' " +
                "WHEN substr(:today,3,2) = '04' THEN 'April' " +
                "WHEN substr(:today,3,2) = '05' THEN 'May' " +
                "WHEN substr(:today,3,2) = '06' THEN 'June' " +
                "WHEN substr(:today,3,2) = '07' THEN 'July' " +
                "WHEN substr(:today,3,2) = '08' THEN 'August' " +
                "WHEN substr(:today,3,2) = '09' THEN 'September' " +
                "WHEN substr(:today,3,2) = '10' THEN 'October' " +
                "WHEN substr(:today,3,2) = '11' THEN 'November' " +
                "ELSE 'December' END || ' ' || substr(:today, 5, 7) || ' )' AS AS_AT_TODAY " +
                "FROM C_COMP_APPLICATION C_APPL " +
                "INNER JOIN C_COMP_APPLICANT_INFO C_INFO ON C_APPL.UUID = C_INFO.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON C_APPL.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE 1 = 1 " +
                "AND C_INFO.CARD_SERIAL_NO is not null " +
                " AND C_INFO.CARD_ISSUE_DATE is not null))  " +
                "WHERE 1 = 1 " +
                "ORDER BY to_NUMBER(ISSUE_Y), to_NUMBER(ISSUE_M)";
            return sql;
        }

        //CRM0022
        public string getCountQpSql()
        {
            string sql = "select code, sum(count) as count, sum(seq) as seq,:CountForWillingnessQp AS WILLINGNESS_QP," +
                "'As at :' || " +
                "CASE WHEN substr(:today,0,2) = '01' THEN '1' " +
                "WHEN substr(:today,0,2) = '02' THEN '2' " +
                "WHEN substr(:today,0,2) = '03' THEN '3' " +
                "WHEN substr(:today,0,2) = '04' THEN '4' " +
                "WHEN substr(:today,0,2) = '05' THEN '5' " +
                "WHEN substr(:today,0,2) = '06' THEN '6' " +
                "WHEN substr(:today,0,2) = '07' THEN '7' " +
                "WHEN substr(:today,0,2) = '08' THEN '8' " +
                "WHEN substr(:today,0,2) = '09' THEN '9' " +
                "ELSE substr(:today,0,2) END || ' ' || " +
                "CASE " +
                "WHEN substr(:today,3,2) = '01' THEN 'January' " +
                "WHEN substr(:today,3,2) = '02' THEN 'February' " +
                "WHEN substr(:today,3,2) = '03' THEN 'March' " +
                "WHEN substr(:today,3,2) = '04' THEN 'April' " +
                "WHEN substr(:today,3,2) = '05' THEN 'May' " +
                "WHEN substr(:today,3,2) = '06' THEN 'June' " +
                "WHEN substr(:today,3,2) = '07' THEN 'July' " +
                "WHEN substr(:today,3,2) = '08' THEN 'August' " +
                "WHEN substr(:today,3,2) = '09' THEN 'September' " +
                "WHEN substr(:today,3,2) = '10' THEN 'October' " +
                "WHEN substr(:today,3,2) = '11' THEN 'November' " +
                "ELSE 'December' END || ' ' || substr(:today, 5, 7) || ' ' AS AS_AT_TODAY " +
                " from " +
                "(" +
                "select scatgrp.code, count(*) as count, 0 as seq " +
                "from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status " +
                "where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
                "and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id " +
                "and (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL))" +
                "and scatgrp.code in ('AP', 'RSE', 'RI') " +
                "group by scatgrp.code " +

                "union all " +
                "select 'RMWC(Individual) - Item 3.6', count(*) as count, 0 as seq " +
                "from C_ind_application iapp, C_ind_certificate c, C_s_system_value status " +
                "where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id " +
                "and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
                "and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem " +
                "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6')) " +
                "group by iapp.REGISTRATION_TYPE " +

                "union all " +

                "select 'RMWC(Company) - Type A', count(*) as count, 0 as seq FROM( " +
                "SELECT DISTINCT c.FILE_REFERENCE_NO " +
                "from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info,C_s_system_value s_status, C_applicant app " +
                "where c.REGISTRATION_TYPE = 'CMW' " +
                "and status.uuid = c.application_status_id and c.uuid=app_info.master_id and app_info.applicant_role_id=s_role.uuid " +
                "and app_info.applicant_status_id= s_status.uuid and app_info.applicant_id=app.uuid and s_role.code like 'A%' " +
                "and s_status.code= '1' and app_info.accept_date is not null " +
                "and ( (app_info.removal_date is null) or  (app_info.removal_date < current_date) ) " +
                "and c.certification_no is not null " +
                "and app_info.uuid in (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw " +
                "inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in('Type A')) ) " +

                "UNION ALL " +
                "SELECT 'No. of Person' AS code, count, 0 FROM ( (SELECT count(DISTINCT hkid) AS count FROM ( " +
                "select hkid || passport_no AS hkid " +
                "from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status " +
                "where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' " +
                "and status.uuid = c.application_status_id and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
                "and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem " +
                "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6')) " +
                "group by iapp.REGISTRATION_TYPE, hkid || passport_no " +
                "UNION ALL " +
                "select hkid || passport_no AS hkid " +
                "from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status " +
                "where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID " +
                "AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id " +
                "and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI')))) " +
                "UNION ALL " +
                "SELECT 'No. of Company' AS code, count, 0 FROM ( (SELECT count(DISTINCT br_no) AS count FROM ( " +
                "select c.BR_NO from C_comp_application c, C_s_system_value status, C_S_CATEGORY_CODE scat " +
                "where c.REGISTRATION_TYPE = 'CGC' and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID " +
                "AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL group by c.REGISTRATION_TYPE, c.br_no " +
                "UNION ALL " +
                "select c.BR_NO " +
                "from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app " +
                "where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id " +
                "and c.uuid=app_info.master_id and app_info.applicant_role_id=s_role.uuid and app_info.applicant_status_id= s_status.uuid " +
                "and app_info.applicant_id=app.uuid and s_role.code like 'A%' and s_status.code= '1' and app_info.accept_date is not null " +
                "and ( (app_info.removal_date is null) or  (app_info.removal_date < current_date)) and c.certification_no is not null " +
                "and app_info.uuid in ( select cmw.company_applicants_id from C_comp_applicant_mw_item cmw " +
                "inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid " +
                "where mwcode.code in('Type A')) group by c.REGISTRATION_TYPE, c.br_no )) comp_yes) " +

                "union all " +
                "select 'RGBC', count(*) as count, 0 as seq " +
                "from C_comp_application c, C_S_SYSTEM_VALUE status, C_S_CATEGORY_CODE scat " +
                "where c.REGISTRATION_TYPE = 'CGC' " +
                "AND c.APPLICATION_STATUS_ID = status.UUID " +
                "AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL " +
                "group by c.REGISTRATION_TYPE " +

                //total
                "union all  SELECT 'Total:',SUM(COUNT),11 as seq FROM( " +
                "select count(*) as count " +
                "from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status " +
                "where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
                "and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id " +
                "and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
                "and scatgrp.code in ('AP', 'RSE', 'RI') " +
                "group by scatgrp.code  " +
                "union all " +
                "select count(*) as count " +
                "from C_ind_application iapp, C_ind_certificate c, C_s_system_value status " +
                "where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id " +
                "and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
                "and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem " +
                "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6')) " +
                "group by iapp.REGISTRATION_TYPE " +
                "union all " +
                "select count(*) as count FROM( " +
                "SELECT DISTINCT c.FILE_REFERENCE_NO " +
                "from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app " +
                "where c.REGISTRATION_TYPE = 'CMW' " +
                "and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid " +
                "and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' " +
                "and s_status.code = '1' and app_info.accept_date is not null " +
                "and((app_info.removal_date is null) or(app_info.removal_date < current_date)) " +
                "and c.certification_no is not null " +
                "and app_info.uuid in (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw " +
                "inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) ) " +
                "union all " +
                "select count(*) as count " +
                "from C_comp_application c, C_S_SYSTEM_VALUE status, C_S_CATEGORY_CODE scat " +
                "where c.REGISTRATION_TYPE = 'CGC' AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL " +
                "group by c.REGISTRATION_TYPE) " +
                //end

                "union all select 'AP', 0, 1 as seq from dual " +
                "union all select 'RSE', 0, 2 as seq from dual " +
                "union all select 'RI', 0, 3 as seq from dual " +
                "union all select 'RGBC', 0, 4 as seq from dual " +
                "union all select 'RMWC(Company) - Type A', 0, 7 as seq from dual " +
                "union all select 'RMWC(Individual) - Item 3.6', 0, 10 as seq from dual " +
                "union all select 'No. of Person', 0, 12 as seq from DUAL " +
                "union all select 'No. of Company', 0, 13 as seq from dual ) " +
                "group by code order by seq";
            return sql;
        }

        //CRM0023
        public string getCountQpAllSql()
        {
            string sql = "select code, sum(yes_cnt) as yes_cnt, sum(no_cnt) as no_cnt, sum(no_ind_cnt) as no_ind_cnt, " +
                "sum(seq) as seq,to_char(to_date(:today,'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_AT_TODAY,:CountForWillingnessQp AS WILLINGNESS_QP from " +
                "( select ip_yes.code, ip_yes.count as yes_cnt, ip_no.count as no_cnt, ip_no_ind.count as no_ind_cnt, 0 AS seq from( (SELECT code, sum(count) AS count, 0 AS seq FROM( " +
                "select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status " +
                "where c.CATEGORY_ID = scat.UUID " +
                "and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
                "and scatgrp.code in ('AP', 'RSE', 'RI') and willingness_qp = 'Y' group by scatgrp.code " +
                "UNION ALL SELECT 'AP', 0, 0 FROM DUAL " +
                "UNION ALL SELECT 'RSE', 0, 0 FROM DUAL " +
                "UNION ALL SELECT 'RI', 0, 0 FROM DUAL) GROUP BY code ) ip_yes " +
                "LEFT JOIN (select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status " +
                "where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id " +
                "and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and willingness_qp = 'N' " +
                "group by scatgrp.code) ip_no on ip_yes.code = ip_no.code " +
                "LEFT JOIN (select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status " +
                "where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id " +
                "and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and(willingness_qp = 'I' or willingness_qp is null) " +
                "group by scatgrp.code) ip_no_ind on ip_yes.code = ip_no_ind.code) " +
                "union all " +
                "select cgc_yes.code, cgc_yes.count, cgc_no.count, cgc_no_ind.count, 0 AS seq from((" +
                "SELECT code, sum(count) AS count, 0 AS seq from(select 'RGBC' AS code, count(*) as count, 0 as seq FROM(SELECT DISTINCT c.FILE_REFERENCE_NO " +
                "from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID " +
                "AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and willingness_qp = 'Y') " +
                "UNION ALL " +
                "SELECT 'RGBC', 0, 0 FROM DUAL) GROUP BY code) cgc_yes LEFT JOIN (select 'RGBC' AS code, count(*) as count, 0 as seq FROM(" +
                " SELECT DISTINCT c.FILE_REFERENCE_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' " +
                "AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and willingness_qp = 'N')) cgc_no " +
                "on cgc_yes.code = cgc_no.code LEFT JOIN (select 'RGBC' AS code, count(*) as count, 0 as seq " +
                "FROM( SELECT DISTINCT c.FILE_REFERENCE_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat " +
                "where c.REGISTRATION_TYPE = 'CGC' AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC'" +
                "and c.CERTIFICATION_NO IS NOT NULL and(willingness_qp = 'I' or willingness_qp is null) )) cgc_no_ind on cgc_yes.code = cgc_no_ind.code) " +
                "union all " +
                "select imw_yes.code, imw_yes.count, imw_no.count, imw_no_ind.count, 0 AS seq from(( SELECT code, sum(count) AS count, 0 AS seq from( " +
                "select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status " +
                "where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id " +
                "and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem " +
                "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and willingness_qp = 'Y' group by iapp.REGISTRATION_TYPE " +
                "UNION ALL " +
                "SELECT 'RMWC(Individual) - Item 3.6', 0, 0 FROM DUAL) GROUP BY code) imw_yes " +
                "LEFT JOIN (select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status " +
                "where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
                "and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem " +
                "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and willingness_qp = 'N' " +
                "group by iapp.REGISTRATION_TYPE) imw_no on imw_yes.code = imw_no.code " +
                "LEFT JOIN (select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status " +
                "where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
                "and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6')) " +
                "and(willingness_qp = 'I' or willingness_qp is null) group by iapp.REGISTRATION_TYPE) imw_no_ind on imw_yes.code = imw_no_ind.code) " +
                "union all select cmw_yes.code, cmw_yes.count, cmw_no.count, cmw_no_ind.count, 0 AS seq from((SELECT code, sum(count) AS count, 0 AS seq FROM( " +
                "select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO " +
                "from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app " +
                "where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid " +
                "and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null " +
                "and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in ( select cmw.company_applicants_id " +
                "from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and willingness_qp = 'Y') " +
                "UNION ALL SELECT 'RMWC(Company) - Type A', 0, 0 FROM DUAL) GROUP BY code) cmw_yes " +
                "LEFT JOIN (select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO " +
                "from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app " +
                "where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id " +
                "and app_info.applicant_role_id = s_role.uuid and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' " +
                "and app_info.accept_date is not null and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in " +
                "(select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) " +
                "and willingness_qp = 'N')) cmw_no on cmw_yes.code = cmw_no.code " +
                "LEFT JOIN (select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO " +
                "from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app " +
                "where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid " +
                "and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null " +
                "and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in " +
                "( select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) " +
                "and(willingness_qp = 'I' or willingness_qp is null) )) cmw_no_ind on cmw_yes.code = cmw_no_ind.code) " +
                "UNION ALL SELECT 'No. of Person' AS code, ind_yes.count, ind_no.count, ind_no_ind.count, 0 FROM((SELECT count(DISTINCT hkid) AS count FROM( select hkid || passport_no AS hkid " +
                "from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID " +
                "and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
                "and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem where sitem.uuid = iitem.ITEM_DETAILS_ID " +
                "and sitem.code in ('Item 3.6')) and willingness_qp = 'Y'group by iapp.REGISTRATION_TYPE, hkid || passport_no " +
                "UNION ALL " +
                "select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status " +
                "where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id " +
                "and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and willingness_qp = 'Y')) ind_yes " +
                "LEFT JOIN ( SELECT count(DISTINCT hkid) AS count FROM( select hkid || passport_no AS hkid from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status " +
                "where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id " +
                "and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem " +
                "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and willingness_qp = 'N' " +
                "group by iapp.REGISTRATION_TYPE, hkid || passport_no " +
                "UNION ALL " +
                "select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status " +
                "where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID " +
                "AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
                "and scatgrp.code in ('AP', 'RSE', 'RI') and willingness_qp = 'N'))ind_no ON 1 = 1 " +
                "LEFT JOIN( SELECT count(DISTINCT hkid) AS count FROM ( select hkid || passport_no AS hkid from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status " +
                "where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id " +
                "and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem " +
                "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6')) and(willingness_qp = 'I' OR willingness_qp IS NULL) group by iapp.REGISTRATION_TYPE, hkid || passport_no " +
                "UNION ALL select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status " +
                "where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID AND app.UUID = iapp.APPLICANT_ID " +
                "and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') " +
                "and(willingness_qp = 'I' OR willingness_qp IS NULL)) )ind_no_ind ON 1 = 1) " +
                "UNION ALL SELECT 'No. of Company' AS code, comp_yes.count, comp_no.count, comp_no_ind.count, 0 FROM( (SELECT count(DISTINCT br_no) AS count FROM(select c.BR_NO " +
                "from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID " +
                "AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and willingness_qp = 'Y' " +
                "group by c.REGISTRATION_TYPE, c.br_no " +
                "UNION ALL " +
                "select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app " +
                "where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id " +
                "and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid " +
                "and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null and((app_info.removal_date is null) or(app_info.removal_date < current_date)) " +
                "and c.certification_no is not null and app_info.uuid in ( select cmw.company_applicants_id from C_comp_applicant_mw_item cmw " +
                "inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and willingness_qp = 'Y' group by c.REGISTRATION_TYPE, c.br_no)) comp_yes " +
                "LEFT JOIN ( SELECT count(DISTINCT br_no) AS count FROM( select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' " +
                "and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and willingness_qp = 'N' " +
                "group by c.REGISTRATION_TYPE, c.br_no " +
                "UNION ALL select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app " +
                "where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid " +
                "and app_info.applicant_status_id = s_status.uuid " +
                "and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null " +
                "and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in " +
                "(select cmw.company_applicants_id from C_comp_applicant_mw_item cmw " +
                "inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and willingness_qp = 'N' " +
                "group by c.REGISTRATION_TYPE, c.br_no))comp_no ON 1 = 1 " +
                "LEFT JOIN(SELECT count(DISTINCT br_no) AS count FROM (select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' " +
                "and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and (willingness_qp = 'I' OR willingness_qp IS NULL) " +
                "group by c.REGISTRATION_TYPE, c.br_no " +
                "UNION ALL " +
                "select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app " +
                "where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid " +
                "and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null " +
                "and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in " +
                "(select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in('Type A')) " +
                "and(willingness_qp = 'I' OR willingness_qp IS NULL) group by c.REGISTRATION_TYPE, c.br_no))comp_no_ind ON 1 = 1) " +
                "union all select 'AP', 0, 0, 0, 1 as seq from dual " +
                "union all select 'RSE', 0, 0, 0, 2 as seq from dual " +
                "union all select 'RI', 0, 0, 0, 3 as seq from dual" +
                " union all select 'RGBC', 0, 0, 0, 4 as seq from dual " +
                "union all select 'RMWC(Company) - Type A', 0, 0, 0, 7 as seq from dual " +
                "union all select 'RMWC(Individual) - Item 3.6', 0, 0, 0, 10 as seq from dual " +
                "union all select 'No. of Person', 0, 0, 0, 12 as seq from DUAL " +
                "union all select 'No. of Company', 0, 0, 0, 13 as seq from dual) " +
                "group by code order by seq";
            return sql;
        }

        //CRM0024
        public string getCountQpAllHistorySql(Dictionary<string, Object> myDictionary)
        {
            if (string.IsNullOrEmpty(myDictionary["CountForWillingnessQp"].ToString()))
                myDictionary["CountForWillingnessQp"] = "A";

            string sql = @"SELECT rownum, qp.*,:CountForWillingnessQp AS WILLINGNESS_QP,to_char(count_date,'dd/mm/yyyy')  AS AS_AT_TODAY 
                from C_QP_COUNT qp where TO_CHAR(qp.count_date,'yyyy/MM') = :QpAsAT";

            //string sql = "SELECT rownum, qp.*,:willingness_qp AS WILLINGNESS_QP,:as_at_today AS AS_AT_TODAY from C_QP_COUNT qp where to_date(count_date) = to_date(:as_at_today , 'dd/MM/yyyy')";
            return sql;
        }

        //CRM0025
        public string getCountQpAllHistoryXlsSql()
        {
            string sql = "SELECT rownum, qp.*,:willingness_qp AS WILLINGNESS_QP,:qp_as_at AS AS_AT_TODAY from C_QP_COUNT qp where to_date(count_date) = to_date(:qp_as_at , 'dd/MM/yyyy')";
            return sql;
        }

        //CRM0026
        public string getCountQpXlsSql()
        {
            string sql = "select code, sum(count) as count, sum(seq) as seq,:willingness_qp AS WILLINGNESS_QP," +
                "'(as at ' || " +
                "CASE WHEN substr(:today,0,2) = '01' THEN '1' " +
                "WHEN substr(:today,0,2) = '02' THEN '2' " +
                "WHEN substr(:today,0,2) = '03' THEN '3' " +
                "WHEN substr(:today,0,2) = '04' THEN '4' " +
                "WHEN substr(:today,0,2) = '05' THEN '5' " +
                "WHEN substr(:today,0,2) = '06' THEN '6' " +
                "WHEN substr(:today,0,2) = '07' THEN '7' " +
                "WHEN substr(:today,0,2) = '08' THEN '8' " +
                "WHEN substr(:today,0,2) = '09' THEN '9' " +
                "ELSE substr(:today,0,2) END || ' ' || " +
                "CASE " +
                "WHEN substr(:today,3,2) = '01' THEN 'January' " +
                "WHEN substr(:today,3,2) = '02' THEN 'February' " +
                "WHEN substr(:today,3,2) = '03' THEN 'March' " +
                "WHEN substr(:today,3,2) = '04' THEN 'April' " +
                "WHEN substr(:today,3,2) = '05' THEN 'May' " +
                "WHEN substr(:today,3,2) = '06' THEN 'June' " +
                "WHEN substr(:today,3,2) = '07' THEN 'July' " +
                "WHEN substr(:today,3,2) = '08' THEN 'August' " +
                "WHEN substr(:today,3,2) = '09' THEN 'September' " +
                "WHEN substr(:today,3,2) = '10' THEN 'October' " +
                "WHEN substr(:today,3,2) = '11' THEN 'November' " +
                "ELSE 'December' END || ' ' || substr(:today, 5, 7) || ' )' AS AS_AT_TODAY " +
                " from " +
                "(" +
                "select scatgrp.code, count(*) as count, 0 as seq " +
                "from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status " +
                "where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
                "and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id " +
                "and (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL))" +
                "and scatgrp.code in ('AP', 'RSE', 'RI') " +
                "group by scatgrp.code " +

                "union all " +
                "select 'RMWC(Individual) - Item 3.6', count(*) as count, 0 as seq " +
                "from C_ind_application iapp, C_ind_certificate c, C_s_system_value status " +
                "where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id " +
                "and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
                "and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem " +
                "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6')) " +
                "group by iapp.REGISTRATION_TYPE " +

                "union all " +

                "select 'RMWC(Company) - Type A', count(*) as count, 0 as seq FROM( " +
                "SELECT DISTINCT c.FILE_REFERENCE_NO " +
                "from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info,C_s_system_value s_status, C_applicant app " +
                "where c.REGISTRATION_TYPE = 'CMW' " +
                "and status.uuid = c.application_status_id and c.uuid=app_info.master_id and app_info.applicant_role_id=s_role.uuid " +
                "and app_info.applicant_status_id= s_status.uuid and app_info.applicant_id=app.uuid and s_role.code like 'A%' " +
                "and s_status.code= '1' and app_info.accept_date is not null " +
                "and ( (app_info.removal_date is null) or  (app_info.removal_date < current_date) ) " +
                "and c.certification_no is not null " +
                "and app_info.uuid in (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw " +
                "inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in('Type A')) ) " +

                "UNION ALL " +
                "SELECT 'No. of Person' AS code, count, 0 FROM ( (SELECT count(DISTINCT hkid) AS count FROM ( " +
                "select hkid || passport_no AS hkid " +
                "from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status " +
                "where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' " +
                "and status.uuid = c.application_status_id and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
                "and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem " +
                "where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6')) " +
                "group by iapp.REGISTRATION_TYPE, hkid || passport_no " +
                "UNION ALL " +
                "select hkid || passport_no AS hkid " +
                "from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status " +
                "where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID " +
                "AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id " +
                "and  (status.code = '1' or (c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI')))) " +
                "UNION ALL " +
                "SELECT 'No. of Company' AS code, count, 0 FROM ( (SELECT count(DISTINCT br_no) AS count FROM ( " +
                "select c.BR_NO from C_comp_application c, C_s_system_value status, C_S_CATEGORY_CODE scat " +
                "where c.REGISTRATION_TYPE = 'CGC' and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID " +
                "AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL group by c.REGISTRATION_TYPE, c.br_no " +
                "UNION ALL " +
                "select c.BR_NO " +
                "from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app " +
                "where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id " +
                "and c.uuid=app_info.master_id and app_info.applicant_role_id=s_role.uuid and app_info.applicant_status_id= s_status.uuid " +
                "and app_info.applicant_id=app.uuid and s_role.code like 'A%' and s_status.code= '1' and app_info.accept_date is not null " +
                "and ( (app_info.removal_date is null) or  (app_info.removal_date < current_date)) and c.certification_no is not null " +
                "and app_info.uuid in ( select cmw.company_applicants_id from C_comp_applicant_mw_item cmw " +
                "inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid " +
                "where mwcode.code in('Type A')) group by c.REGISTRATION_TYPE, c.br_no )) comp_yes) " +

                "union all " +
                "select 'RGBC', count(*) as count, 0 as seq " +
                "from C_comp_application c, C_S_SYSTEM_VALUE status, C_S_CATEGORY_CODE scat " +
                "where c.REGISTRATION_TYPE = 'CGC' " +
                "AND c.APPLICATION_STATUS_ID = status.UUID " +
                "AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL " +
                "group by c.REGISTRATION_TYPE " +
                "union all select 'AP', 0, 1 as seq from dual " +
                "union all select 'RSE', 0, 2 as seq from dual " +
                "union all select 'RI', 0, 3 as seq from dual " +
                "union all select 'RGBC', 0, 4 as seq from dual " +
                "union all select 'RMWC(Company) - Type A', 0, 7 as seq from dual " +
                "union all select 'RMWC(Individual) - Item 3.6', 0, 10 as seq from dual " +
                "union all select 'No. of Person', 0, 12 as seq from DUAL " +
                "union all select 'No. of Company', 0, 13 as seq from dual ) " +
                "group by code order by seq";
            return sql;
        }

        //CRM0027
        public string getCountReturnDateQpCardAll()
        {
            string sql = "select * from ( " +
                "SELECT S_CAT_GP.CODE AS CAT_GP_CODE,S_CAT.CODE AS CAT_CODE, I_CERT.UUID AS MASTER_ID," +
                "CASE WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '01' THEN '01-January' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '02' THEN '02-February' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '03' THEN '03-March' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '04' THEN '04-April' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '05' THEN '05-May' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '06' THEN '06-June' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '07' THEN '07-July' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '08' THEN '08-August' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '09' THEN '09-September' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '10' THEN '10-October' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '11' THEN '11-November' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(I_CERT.CARD_RETURN_DATE, 'MM') = '12' THEN '12-December' || ' ' || to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') END AS RET_MY, " +
                "to_char(I_CERT.CARD_RETURN_DATE, 'YYYY') AS RET_Y,to_char (I_CERT.CARD_RETURN_DATE, 'MM') AS RET_M," +
                ":today AS AS_AT_TODAY " +
                "FROM C_IND_APPLICATION I_APPL " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE 1 = 1 AND I_CERT.CARD_SERIAL_NO is not null AND I_CERT.CARD_RETURN_DATE is not null " +
                "UNION ALL( " +
                "SELECT S_CAT_GP.CODE AS CAT_GP_CODE,S_CAT.CODE AS CAT_CODE, C_INFO.UUID AS MASTER_ID,CASE " +
                "WHEN to_char (C_INFO.CARD_RETURN_DATE, 'MM') = '01' THEN '01-January' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_RETURN_DATE, 'MM') = '02' THEN '02-February' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_RETURN_DATE, 'MM') = '03' THEN '03-March' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_RETURN_DATE, 'MM') = '04' THEN '04-April' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_RETURN_DATE, 'MM') = '05' THEN '05-May' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_RETURN_DATE, 'MM') = '06' THEN '06-June' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_RETURN_DATE, 'MM') = '07' THEN '07-July' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_RETURN_DATE, 'MM') = '08' THEN '08-August' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_RETURN_DATE, 'MM') = '09' THEN '09-September' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_RETURN_DATE, 'MM') = '10' THEN '10-October' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_RETURN_DATE, 'MM') = '11' THEN '11-November' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "WHEN to_char(C_INFO.CARD_RETURN_DATE, 'MM') = '12' THEN '12-December' || ' ' || to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') " +
                "END AS RET_MY, " +
                "to_char(C_INFO.CARD_RETURN_DATE, 'YYYY') AS RET_Y, " +
                "to_char (C_INFO.CARD_RETURN_DATE, 'MM') AS RET_M," +
                ":today AS AS_AT_TODAY " +
                "FROM C_COMP_APPLICATION C_APPL " +
                "INNER JOIN C_COMP_APPLICANT_INFO C_INFO ON C_APPL.UUID = C_INFO.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON C_APPL.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE 1 = 1 AND C_INFO.CARD_SERIAL_NO is not null " +
                "AND C_INFO.CARD_RETURN_DATE is not null)) WHERE 1 = 1 " +
                "ORDER BY to_NUMBER(RET_Y), to_NUMBER(RET_M) ";
            return sql;
        }

        //CRM0028
        public string getCountRISql(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT prb_code, qcode, english_description, sum(ri_counter) AS count, sum(COUNTER_AP) AS COUNTER_AP, sum(COUNTER_RSE) AS COUNTER_RSE," +
                "to_char(to_date(:ir_search_date_from,'dd/mm/yyyy'),'dd/mm/yyyy') AS DATE_TYPE_TXT,:status_txt AS STATUS_TXT,to_char(to_date(:ir_search_date_to,'dd/mm/yyyy'),'dd/mm/yyyy') AS TXT_TO " +
                "FROM (" +
                "select * from(select prb_code, qcode, english_description, count(cer_uuid) AS ri_counter,SUM(CASE WHEN other_cat = 'AP(A)' THEN 1 " +
                "WHEN OTHER_CAT = 'AP(E)' THEN 1 " +
                "WHEN OTHER_CAT = 'AP(S)' THEN 1 WHEN OTHER_CAT = 'API' THEN 1 " +
                "WHEN OTHER_CAT = 'APII' THEN 1 " +
                "WHEN OTHER_CAT = 'APIII' THEN 1 ELSE 0 END) AS COUNTER_AP, " +
                "SUM(CASE WHEN other_cat = 'RSE' THEN 1 ELSE 0 END) AS COUNTER_RSE " +
                "FROM( " +
                "select distinct icer.uuid AS cer_uuid, scat.code as qcode, " +
                "CASE WHEN prb.code IS NULL THEN " +
                "CASE WHEN scat.code = 'RI(E)' THEN 'ERB' " +
                "WHEN scat.code = 'RI(S)' THEN 'SRB' " +
                "WHEN scat.code = 'RI(A)' THEN 'ARB' " +
                "ELSE " +
                "prb.code END " +
                "ELSE prb.code END as prb_code,scatd.english_description,other_cer.UUID AS other_cer,other_cer.CODE AS other_cat from C_ind_certificate icer " +
                "inner join C_s_category_code scat on icer.CATEGORY_ID = scat.UUID " +
                "inner JOIN C_S_SYSTEM_VALUE SCATGROUP ON scat.CATEGORY_GROUP_ID = SCATGROUP.UUID " +
                "left join C_ind_qualification iq on icer.master_id = iq.master_id AND icer.CATEGORY_ID = iq.CATEGORY_ID " +
                "left join C_ind_qualification_detail iqd on iqd.ind_qualification_id = iq.uuid " +
                "left join C_s_category_code_detail scatd on scatd.uuid = iqd.S_CATEGORY_CODE_DETAIL_ID " +
                "left join C_s_system_value prb on prb.uuid = iq.PRB_ID " +
                "left join(SELECT icer2.UUID, icer2.MASTER_ID, scat2.CODE FROM C_ind_certificate icer2 " +
                "inner join C_s_category_code scat2 on icer2.CATEGORY_ID = scat2.UUID AND scat2.code NOT IN('RI(A)', 'RI(E)', 'RI(S)') " +
                "inner JOIN C_S_SYSTEM_VALUE SCATGROUP2 ON scat2.CATEGORY_GROUP_ID = SCATGROUP2.UUID AND SCATGROUP2.CODE != 'RI' " +
                "Where ((icer2.expiry_date is not null and icer2.expiry_date >= current_date) " +
            "OR(icer2.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') and icer2.expiry_date < current_date)) " +
            "AND(icer2.removal_date is null or icer2.removal_date > current_date)) " +
            "other_cer ON icer.MASTER_ID = other_cer.master_id AND icer.uuid NOT IN other_cer.uuid " +
            "where SCATGROUP.code = 'RI' AND((icer.expiry_date is not null and icer.expiry_date >= current_date) " +
            "OR(icer.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') and icer.expiry_date < current_date)) " +
            "AND(icer.removal_date is null or icer.removal_date > current_date)) " +
            "group by prb_code, qcode, english_description, other_cat) " +
            "union ALL select '', '', SCATGROUP.code, count(*), 0, 0 " +
            "from C_ind_application iapp, C_ind_certificate icer, C_s_category_code scat, C_S_SYSTEM_VALUE SCATGROUP " +
            "where icer.MASTER_ID = iapp.UUID AND icer.CATEGORY_ID = scat.uuid and scat.CATEGORY_GROUP_ID = SCATGROUP.uuid and SCATGROUP.code = 'RI' ");

            if (!string.IsNullOrEmpty(myDictionary["date_type"].ToString()))
            {
                if (!string.IsNullOrEmpty(myDictionary["ir_search_date_from"].ToString()))
                    sql.Append(" And icer." + myDictionary["date_type"].ToString() + " >= to_date(:ir_search_date_from,'dd/mm/yyyy') ");
                if (!string.IsNullOrEmpty(myDictionary["ir_search_date_to"].ToString()))
                    sql.Append(" And icer." + myDictionary["date_type"].ToString() + " <= to_date(:ir_search_date_to,'dd/mm/yyyy') ");
            }

            string status_txt = "";
            if (myDictionary.Keys.Contains("Active") || myDictionary.Keys.Contains("Application in progress") ||
               myDictionary.Keys.Contains("Certificate prepared") || myDictionary.Keys.Contains("Removed") ||
               myDictionary.Keys.Contains("Inactive") || myDictionary.Keys.Contains("Withdrawn"))
            {
                sql.Append(" And icer.application_status_id in ( ");
                if (myDictionary.Keys.Contains("Active"))
                {
                    sql.Append("'" + myDictionary["Active"].ToString() + "',");
                    status_txt += " Active,";
                }
                if (myDictionary.Keys.Contains("Application in progress"))
                {
                    sql.Append("'" + myDictionary["Application in progress"].ToString() + "',");
                    status_txt += " Application in progress,";
                }
                if (myDictionary.Keys.Contains("Certificate prepared"))
                {
                    sql.Append("'" + myDictionary["Certificate prepared"].ToString() + "',");
                    status_txt += " Certificate prepared,";
                }
                if (myDictionary.Keys.Contains("Removed"))
                {
                    sql.Append("'" + myDictionary["Removed"].ToString() + "',");
                    status_txt += " Removed,";
                }
                if (myDictionary.Keys.Contains("Inactive"))
                {
                    sql.Append("'" + myDictionary["Inactive"].ToString() + "',");
                    status_txt += " Inactive,";
                }
                if (myDictionary.Keys.Contains("Withdrawn"))
                {
                    sql.Append("'" + myDictionary["Withdrawn"].ToString() + "',");
                    status_txt += " Withdrawn,";
                }
                if (!string.IsNullOrEmpty(status_txt))
                    myDictionary["status_txt"] = status_txt.Substring(0, status_txt.Length - 1);
                sql.Remove(sql.Length - 1, 1);
                sql.Append(" ) ");
            }


            //if (!string.IsNullOrEmpty(myDictionary["date_type"].ToString()))
            //{
            //    switch (myDictionary["date_type"].ToString())
            //    {
            //        case "application_date":
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_from"].ToString()))
            //                sql.Append(" And icer.application_date >= to_date(:ir_search_date_from,'dd/mm/yyyy') ");
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_to"].ToString()))
            //                sql.Append(" And icer.application_date <= to_date(:ir_search_date_to,'dd/mm/yyyy') ");
            //            break;
            //        case "registration_date":
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_from"].ToString()))
            //                sql.Append(" And icer.registration_date >= to_date(:ir_search_date_from,'dd/mm/yyyy') ");
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_to"].ToString()))
            //                sql.Append(" And icer.registration_date <= to_date(:ir_search_date_to,'dd/mm/yyyy') ");
            //            break;
            //        case "gazette_date":
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_from"].ToString()))
            //                sql.Append(" And icer.gazette_date >= to_date(:ir_search_date_from,'dd/mm/yyyy') ");
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_to"].ToString()))
            //                sql.Append(" And icer.gazette_date <= to_date(:ir_search_date_to,'dd/mm/yyyy') ");
            //            break;
            //        case "expiry_date":
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_from"].ToString()))
            //                sql.Append(" And icer.expiry_date >= to_date(:ir_search_date_from,'dd/mm/yyyy') ");
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_to"].ToString()))
            //                sql.Append(" And icer.expiry_date <= to_date(:ir_search_date_to,'dd/mm/yyyy') ");
            //            break;
            //        case "removal_date":
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_from"].ToString()))
            //                sql.Append(" And icer.removal_date >= to_date(:ir_search_date_from,'dd/mm/yyyy') ");
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_to"].ToString()))
            //                sql.Append(" And icer.removal_date <= to_date(:ir_search_date_to,'dd/mm/yyyy') ");
            //            break;
            //        case "retention_date":
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_from"].ToString()))
            //                sql.Append(" And icer.retention_date >= to_date(:ir_search_date_from,'dd/mm/yyyy') ");
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_to"].ToString()))
            //                sql.Append(" And icer.retention_date <= to_date(:ir_search_date_to,'dd/mm/yyyy') ");
            //            break;
            //        case "restore_date":
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_from"].ToString()))
            //                sql.Append(" And icer.restore_date >= to_date(:ir_search_date_from,'dd/mm/yyyy') ");
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_to"].ToString()))
            //                sql.Append(" And icer.restore_date <= to_date(:ir_search_date_to,'dd/mm/yyyy') ");
            //            break;
            //        case "approval_date":
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_from"].ToString()))
            //                sql.Append(" And icer.approval_date >= to_date(:ir_search_date_from,'dd/mm/yyyy') ");
            //            if (!string.IsNullOrEmpty(myDictionary["ir_search_date_to"].ToString()))
            //                sql.Append(" And icer.approval_date <= to_date(:ir_search_date_to,'dd/mm/yyyy') ");
            //            break;
            //    }

            //}

            sql.Append(
            "AND((icer.expiry_date is not null and icer.expiry_date >= current_date) " +
            "OR(icer.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') and icer.expiry_date < current_date)) " +
            "AND(icer.removal_date is null or icer.removal_date > current_date) " +
            "group by SCATGROUP.code " +
            "UNION ALL " +
            "SELECT 'ARB', 'RI(A)', '', 0, 0, 0 FROM dual " +
            "UNION ALL " +
            "SELECT 'ERB', 'RI(E)', '', 0, 0, 0 FROM dual " +
            "UNION ALL " +
            "SELECT 'ERB', 'RI(E)', 'Building (RPE-B)', 0, 0, 0 FROM dual " +
            "UNION ALL " +
            "SELECT 'ERB', 'RI(E)', 'Building Services (Building)(RPE-BS)', 0, 0, 0 FROM dual " +
            "UNION ALL " +
            "SELECT 'ERB', 'RI(E)', 'Civil (RPE-C)', 0, 0, 0 FROM dual " +
            "UNION ALL " +
            "SELECT 'ERB', 'RI(E)', 'Materials (Building)(RPE-M)',0, 0, 0 FROM dual " +
            "UNION ALL " +
            "SELECT 'ERB', 'RI(E)', 'Structural (RPE-S)', 0, 0, 0 FROM dual " +
            "UNION ALL " +
            "SELECT 'SRB', 'RI(S)', '', 0, 0, 0 FROM dual " +
            "UNION ALL " +
            "SELECT 'SRB', 'RI(S)', 'Building Surveying (RPS-B)', 0, 0, 0 FROM dual " +
            "UNION ALL " +
            "SELECT 'SRB', 'RI(S)', 'Quantity Surveying (RPS-Q)', 0, 0, 0 FROM dual) " +
            "GROUP BY prb_code, qcode, english_description " +
            "ORDER BY 1,2,3 ");


            //string sql = "SELECT prb_code, qcode, english_description, sum(ri_counter) AS count, sum(COUNTER_AP) AS COUNTER_AP, sum(COUNTER_RSE) AS COUNTER_RSE," +
            //"to_char(to_date(:date_type_txt,'ddmmyyyy'),'dd/mm/yyyy') AS DATE_TYPE_TXT,:status_txt AS STATUS_TXT,to_char(to_date(:txt_to,'ddmmyyyy'),'dd/mm/yyyy') AS TXT_TO " +
            //"FROM (" +
            //"select * from(select prb_code, qcode, english_description, count(cer_uuid) AS ri_counter,SUM(CASE WHEN other_cat = 'AP(A)' THEN 1 " +
            //"WHEN OTHER_CAT = 'AP(E)' THEN 1 " +
            //"WHEN OTHER_CAT = 'AP(S)' THEN 1 WHEN OTHER_CAT = 'API' THEN 1 " +
            //"WHEN OTHER_CAT = 'APII' THEN 1 " +
            //"WHEN OTHER_CAT = 'APIII' THEN 1 ELSE 0 END) AS COUNTER_AP, " +
            //"SUM(CASE WHEN other_cat = 'RSE' THEN 1 ELSE 0 END) AS COUNTER_RSE " +
            //"FROM( " +
            //"select distinct icer.uuid AS cer_uuid, scat.code as qcode, " +
            //"CASE WHEN prb.code IS NULL THEN " +
            //"CASE WHEN scat.code = 'RI(E)' THEN 'ERB' " +
            //"WHEN scat.code = 'RI(S)' THEN 'SRB' " +
            //"WHEN scat.code = 'RI(A)' THEN 'ARB' " +
            //"ELSE " +
            //"prb.code END " +
            //"ELSE prb.code END as prb_code,scatd.english_description,other_cer.UUID AS other_cer,other_cer.CODE AS other_cat from C_ind_certificate icer " +
            //"inner join C_s_category_code scat on icer.CATEGORY_ID = scat.UUID " +
            //"inner JOIN C_S_SYSTEM_VALUE SCATGROUP ON scat.CATEGORY_GROUP_ID = SCATGROUP.UUID " +
            //"left join C_ind_qualification iq on icer.master_id = iq.master_id AND icer.CATEGORY_ID = iq.CATEGORY_ID " +
            //"left join C_ind_qualification_detail iqd on iqd.ind_qualification_id = iq.uuid " +
            //"left join C_s_category_code_detail scatd on scatd.uuid = iqd.S_CATEGORY_CODE_DETAIL_ID " +
            //"left join C_s_system_value prb on prb.uuid = iq.PRB_ID " +
            //"left join(SELECT icer2.UUID, icer2.MASTER_ID, scat2.CODE FROM C_ind_certificate icer2 " +
            //"inner join C_s_category_code scat2 on icer2.CATEGORY_ID = scat2.UUID AND scat2.code NOT IN('RI(A)', 'RI(E)', 'RI(S)') " +
            //"inner JOIN C_S_SYSTEM_VALUE SCATGROUP2 ON scat2.CATEGORY_GROUP_ID = SCATGROUP2.UUID AND SCATGROUP2.CODE != 'RI' " +
            //"where((icer2.expiry_date is not null and icer2.expiry_date >= current_date) " +
            //"OR(icer2.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') and icer2.expiry_date < current_date)) " +
            //"AND(icer2.removal_date is null or icer2.removal_date > current_date)) " +
            //"other_cer ON icer.MASTER_ID = other_cer.master_id AND icer.uuid NOT IN other_cer.uuid " +
            //"where SCATGROUP.code = 'RI' AND((icer.expiry_date is not null and icer.expiry_date >= current_date) " +
            //"OR(icer.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') and icer.expiry_date < current_date)) " +
            //"AND(icer.removal_date is null or icer.removal_date > current_date)) " +
            //"group by prb_code, qcode, english_description, other_cat) " +
            //"union ALL select '', '', SCATGROUP.code, count(*), 0, 0 " +
            //"from C_ind_application iapp, C_ind_certificate icer, C_s_category_code scat, C_S_SYSTEM_VALUE SCATGROUP " +
            //"where icer.MASTER_ID = iapp.UUID AND icer.CATEGORY_ID = scat.uuid and scat.CATEGORY_GROUP_ID = SCATGROUP.uuid and SCATGROUP.code = 'RI' " +
            //"AND((icer.expiry_date is not null and icer.expiry_date >= current_date) " +
            //"OR(icer.retention_application_date > to_date('31/08/2004', 'dd/MM/yyyy') and icer.expiry_date < current_date)) " +
            //"AND(icer.removal_date is null or icer.removal_date > current_date) " +
            //"group by SCATGROUP.code " +
            //"UNION ALL " +
            //"SELECT 'ARB', 'RI(A)', '', 0, 0, 0 FROM dual " +
            //"UNION ALL " +
            //"SELECT 'ERB', 'RI(E)', '', 0, 0, 0 FROM dual " +
            //"UNION ALL " +
            //"SELECT 'ERB', 'RI(E)', 'Building (RPE-B)', 0, 0, 0 FROM dual " +
            //"UNION ALL " +
            //"SELECT 'ERB', 'RI(E)', 'Building Services (Building)(RPE-BS)', 0, 0, 0 FROM dual " +
            //"UNION ALL " +
            //"SELECT 'ERB', 'RI(E)', 'Civil (RPE-C)', 0, 0, 0 FROM dual " +
            //"UNION ALL " +
            //"SELECT 'ERB', 'RI(E)', 'Materials (Building)(RPE-M)',0, 0, 0 FROM dual " +
            //"UNION ALL " +
            //"SELECT 'ERB', 'RI(E)', 'Structural (RPE-S)', 0, 0, 0 FROM dual " +
            //"UNION ALL " +
            //"SELECT 'SRB', 'RI(S)', '', 0, 0, 0 FROM dual " +
            //"UNION ALL " +
            //"SELECT 'SRB', 'RI(S)', 'Building Surveying (RPS-B)', 0, 0, 0 FROM dual " +
            //"UNION ALL " +
            //"SELECT 'SRB', 'RI(S)', 'Quantity Surveying (RPS-Q)', 0, 0, 0 FROM dual) " +
            //"GROUP BY prb_code, qcode, english_description " +
            //"ORDER BY 1,2,3";
            return sql.ToString();
        }

        //CRM0029
        private string getAnnualGazetteENGIPSql(Dictionary<string, Object> myDictionnary)
        {
            string headerDesc = "";
            if (myDictionnary.Keys.Contains("PAHeaderDesc"))
            {
                headerDesc = myDictionnary["PAHeaderDesc"].ToString().Replace("'", "''");
                headerDesc = headerDesc.Replace("[DD][MM][YYYY]", " ");
            }
            else if (myDictionnary.Keys.Contains("MWIHeaderDesc"))
            {
                headerDesc = myDictionnary["MWIHeaderDesc"].ToString().Replace("'", "''");
                headerDesc = headerDesc.Replace("[DD][MM][YYYY]", " ");
            }
            else
            {
                headerDesc = myDictionnary["HeaderDesc"].ToString().Replace("'", "''");
                headerDesc = headerDesc.Replace("[DD][MM][YYYY]", " ");
            }


            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT " +
                "upper(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME, " +
                "APPLN.CHINESE_NAME AS CNAME," +
                "I_CERT.REGISTRATION_DATE AS REG_D," +
                "I_CERT.CERTIFICATION_NO AS CERT_NO," +
                "S_CAT.CODE AS CAT_CODE," +
                "S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION AS ENG_SUB," +
                "S_CAT.CHINESE_SUB_TITLE_DESCRIPTION AS CHN_SUB," +
                "(select MIN(ENGLISH_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_NAME, " +
                "CASE WHEN to_date(:asAtDate, 'dd/mm/yyyy') > I_CERT.expiry_date " +
                "AND add_months(I_CERT.retention_application_date, 24) > trunc(to_date(:asAtDate, 'dd/mm/yyyy')) THEN '@' ELSE '' END AS FLAG," +
                "to_date('" + myDictionnary["asAtDate"].ToString() + "','dd/mm/yyyy')  AS AS_AT_DATE," +
                //"INITCAP(to_char(to_date(:asAtDate, 'dd/mm/yyyy'),'dd mon yyyy','nls_date_language=american')) AS AS_AT_DATE," +
                "'" + headerDesc + "' AS HEADER," +
                "to_date(:ReportGazDateFrom,'dd/mm/yyyy') AS GAZ_FR_DATE," +
                "to_date(:ReportGazDateTo,'dd/mm/yyyy') AS GAZ_TO_DATE " +
                "FROM C_APPLICANT APPLN " +
                "INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE S_CAT.REGISTRATION_TYPE = :reg_type " +
                "AND S_CAT_GP.REGISTRATION_TYPE = :reg_type " +
                " AND I_CERT.CERTIFICATION_NO IS NOT NULL " +
                "AND" +
                "((I_CERT.EXPIRY_DATE IS NOT NULL and I_CERT.EXPIRY_DATE >= to_date(:asAtDate, 'dd/mm/yyyy')) or " +
                "(I_CERT.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(I_CERT.EXPIRY_DATE < to_date(:asAtDate, 'dd/mm/yyyy'))) )  " +
                "AND " +
                "((I_CERT.REMOVAL_DATE IS NULL) OR(I_CERT.REMOVAL_DATE > to_date(:asAtDate, 'dd/mm/yyyy')))");

            if (!string.IsNullOrEmpty(myDictionnary["cat_gp"].ToString()))
                sql.Append(" And S_CAT_GP.CODE IN(:cat_gp) ");
            if (!string.IsNullOrEmpty(myDictionnary["ReportGazDateFrom"].ToString()))
                sql.Append(" AND I_CERT.GAZETTE_DATE >= to_date(:ReportGazDateFrom, 'dd/mm/yyyy') ");
            if (!string.IsNullOrEmpty(myDictionnary["ReportGazDateTo"].ToString()))
                sql.Append(" AND I_CERT.GAZETTE_DATE <= to_date(:ReportGazDateTo, 'dd/mm/yyyy') ");
            sql.Append(" ORDER BY ENG_SUB ASC, ENAME ASC");

            //string sql = "SELECT " +
            //    "upper(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME, " +
            //    "APPLN.CHINESE_NAME AS CNAME," +
            //    "I_CERT.REGISTRATION_DATE AS REG_D," +
            //    "I_CERT.CERTIFICATION_NO AS CERT_NO," +
            //    "S_CAT.CODE AS CAT_CODE," +
            //    "S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION AS ENG_SUB," +
            //    "S_CAT.CHINESE_SUB_TITLE_DESCRIPTION AS CHN_SUB," +
            //    "(select MIN(ENGLISH_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_NAME, " +
            //    "CASE WHEN to_date(:as_at_date, 'dd/mm/yyyy') > I_CERT.expiry_date " +
            //    "AND add_months(I_CERT.retention_application_date, 24) > trunc(to_date(:as_at_date, 'dd/mm/yyyy')) THEN '@' ELSE '' END AS FLAG," +
            //    "INITCAP(to_char(to_date(:as_at_date, 'dd/mm/yyyy'),'dd mon yyyy','nls_date_language=american')) AS AS_AT_DATE," +
            //    "to_date(:gaz_fr_date,'dd/mm/yyyy') AS GAZ_FR_DATE," +
            //    "to_date(:gaz_to_date,'dd/mm/yyyy') AS GAZ_TO_DATE " +
            //    "FROM C_APPLICANT APPLN " +
            //    "INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID " +
            //    "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
            //    "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
            //    "WHERE S_CAT.REGISTRATION_TYPE = :reg_type " +
            //    "AND S_CAT_GP.REGISTRATION_TYPE = :reg_type " +
            //    "AND S_CAT_GP.CODE IN(:cat_gp) " +
            //    "AND I_CERT.GAZETTE_DATE >= to_date(:gaz_fr_date, 'dd/mm/yyyy') " +
            //    "AND I_CERT.GAZETTE_DATE <= to_date(:gaz_to_date, 'dd/mm/yyyy') " +
            //    "AND I_CERT.CERTIFICATION_NO IS NOT NULL " +
            //    "AND " +
            //    "((I_CERT.EXPIRY_DATE IS NOT NULL and I_CERT.EXPIRY_DATE >= to_date(:as_at_date, 'dd/mm/yyyy')) or " +
            //    "(I_CERT.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(I_CERT.EXPIRY_DATE < to_date(:as_at_date, 'dd/mm/yyyy'))) )  " +
            //    "AND " +
            //    "((I_CERT.REMOVAL_DATE IS NULL) OR(I_CERT.REMOVAL_DATE > to_date(:as_at_date, 'dd/mm/yyyy'))) " +
            //    "ORDER BY ENG_SUB ASC, ENAME ASC";
            return sql.ToString();
        }
        //CRM0029 Empty
        public string getAnnualGazetteENGIPEmptySql(Dictionary<string, Object> myDictionnary)
        {
            string headerDesc = "";
            if (myDictionnary.Keys.Contains("PAHeaderDesc"))
            {
                headerDesc = myDictionnary["PAHeaderDesc"].ToString().Replace("'", "''");
                headerDesc = headerDesc.Replace("[DD][MM][YYYY]", " ");
            }
            else if (myDictionnary.Keys.Contains("MWIHeaderDesc"))
            {
                headerDesc = myDictionnary["MWIHeaderDesc"].ToString().Replace("'", "''");
                headerDesc = headerDesc.Replace("[DD][MM][YYYY]", " ");
            }
            else
            {
                headerDesc = myDictionnary["HeaderDesc"].ToString().Replace("'", "''");
                headerDesc = headerDesc.Replace("[DD][MM][YYYY]", " ");
            }


            string sql = "SELECT " +
                "null AS ENAME, " +
                "null AS CNAME," +
                "null AS REG_D," +
                "null AS CERT_NO," +
                "null AS CAT_CODE," +
                "null AS ENG_SUB," +
                "null AS CHN_SUB," +
                "null as AUTH_NAME, " +
                "null AS FLAG," +
                "to_date('" + myDictionnary["asAtDate"].ToString() + "','dd/mm/yyyy')  AS AS_AT_DATE," +
                "'" + headerDesc + "' AS HEADER," +
                "null AS GAZ_FR_DATE," +
                "null AS GAZ_TO_DATE  from dual";
            return sql;
        }
        //CRM0030
        public string getCheckPRSSql(Dictionary<string, Object> myDictionary)
        {
            string categoryStr = null;
            string prbStr = null;
            foreach (var item in myDictionary)
            {
                switch (item.Key)
                {
                    case "checkBoxCatPRSForm_0":
                    case "checkBoxCatPRSForm_1":
                    case "checkBoxCatPRSForm_2":
                    case "checkBoxCatPRSForm_3":
                    case "checkBoxCatPRSForm_4":
                    case "checkBoxCatPRSForm_5":
                    case "checkBoxCatPRSForm_6":
                    case "checkBoxCatPRSForm_7":
                        categoryStr += "'" + item.Value + "',";
                        break;
                    case "checkBoxprbPRSForm_0":
                    case "checkBoxprbPRSForm_1":
                    case "checkBoxprbPRSForm_2":
                    case "checkBoxprbPRSForm_3":
                    case "checkBoxprbPRSForm_4":
                        prbStr += "'" + item.Value + "',";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(categoryStr))
                categoryStr = categoryStr.Substring(0, categoryStr.Length - 1);
            if (!string.IsNullOrEmpty(prbStr))
                prbStr = prbStr.Substring(0, prbStr.Length - 1);

            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT DISTINCT I_APPL.FILE_REFERENCE_NO AS FILE_REF," +
                "c_propercase(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME," +
                "C_CONCAT_REG_NO(I_APPL.FILE_REFERENCE_NO, '8a824790242974be01242974be530004___8a824790242974be01242974be530009','8a82479024294eb70124294eb8420002') AS REG_NO," +
                ":today AS AS_AT_TODAY,:PrsHeader AS COL1_TXT,:Short_name AS SHORT_NAME,'Engineer Registration Board___ERB (Geotechnical)___ERB (Civil / Structural)' AS ERB_NAME " +
                "FROM " +
                "C_IND_APPLICATION I_APPL INNER JOIN C_IND_QUALIFICATION I_QUALI ON I_APPL.UUID = I_QUALI.MASTER_ID " +
                "INNER JOIN C_APPLICANT APPLN ON I_APPL.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "WHERE " +
                "to_char(I_CERT.EXPIRY_DATE, 'yyyymmdd') >= to_char(sysdate, 'yyyymmdd') " +
                "and(I_CERT.REMOVAL_DATE IS null " +
                "or(to_char(I_CERT.REMOVAL_DATE, 'yyyymmdd') > to_char(I_CERT.EXPIRY_DATE, 'yyyymmdd'))) " +
                "and I_CERT.CATEGORY_ID IN( Select distinct  CATEGORY_ID from  C_IND_CERTIFICATE I_CERT inner join c_s_Category_Code sc on I_CERT.Category_Id = sc.uuid Where sc.code in ( " + categoryStr + " )) " +
                "and I_QUALI.REGISTRATION_NUMBER IS NOT null " +
                "and I_QUALI.PRB_ID IN( Select uuid from C_S_SYSTEM_VALUE Where Code  in ( " + prbStr + " ) ) " +
                "ORDER BY ENAME ASC "
                );




            //string sql = "SELECT DISTINCT I_APPL.FILE_REFERENCE_NO AS FILE_REF," +
            //    "c_propercase(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME," +
            //    "C_CONCAT_REG_NO(I_APPL.FILE_REFERENCE_NO, '8a824790242974be01242974be530004___8a824790242974be01242974be530009','8a82479024294eb70124294eb8420002') AS REG_NO," +
            //    ":today AS AS_AT_TODAY,:col1_txt AS COL1_TXT,:Short_name AS SHORT_NAME,'Engineer Registration Board___ERB (Geotechnical)___ERB (Civil / Structural)' AS ERB_NAME " +
            //    "FROM " +
            //    "C_IND_APPLICATION I_APPL INNER JOIN C_IND_QUALIFICATION I_QUALI ON I_APPL.UUID = I_QUALI.MASTER_ID " +
            //    "INNER JOIN C_APPLICANT APPLN ON I_APPL.APPLICANT_ID = APPLN.UUID " +
            //    "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
            //    "WHERE " +
            //    "to_char(I_CERT.EXPIRY_DATE, 'yyyymmdd') >= to_char(sysdate, 'yyyymmdd') " +
            //    "and(I_CERT.REMOVAL_DATE IS null " +
            //    "or(to_char(I_CERT.REMOVAL_DATE, 'yyyymmdd') > to_char(I_CERT.EXPIRY_DATE, 'yyyymmdd'))) " +
            //    "and I_CERT.CATEGORY_ID IN('8a824790242974be01242974be530004', '8a824790242974be01242974be530009') " +
            //    "and I_QUALI.REGISTRATION_NUMBER IS NOT null " +
            //    "and I_QUALI.PRB_ID IN('8a82479024294eb70124294eb8420002') " +
            //    "ORDER BY ENAME ASC";
            return sql.ToString();
        }

        //CRM0031
        public string getCheckPRSXlsSql()
        {
            string sql = "SELECT DISTINCT I_APPL.FILE_REFERENCE_NO AS FILE_REF," +
                "c_propercase(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME," +
                "C_CONCAT_REG_NO(I_APPL.FILE_REFERENCE_NO, '8a824790242974be01242974be530004___8a824790242974be01242974be530009','8a82479024294eb70124294eb8420002') AS REG_NO," +
                ":short_name AS SHORT_NAME,:today AS AS_AT_TODAY,:col1_txt AS COL1_TXT,'Engineer Registration Board___ERB (Geotechnical)___ERB (Civil / Structural)' AS ERB_NAME " +
                "FROM " +
                "C_IND_APPLICATION I_APPL INNER JOIN C_IND_QUALIFICATION I_QUALI ON I_APPL.UUID = I_QUALI.MASTER_ID " +
                "INNER JOIN C_APPLICANT APPLN ON I_APPL.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "WHERE " +
                "to_char(I_CERT.EXPIRY_DATE, 'yyyymmdd') >= to_char(sysdate, 'yyyymmdd') " +
                "and(I_CERT.REMOVAL_DATE IS null " +
                "or(to_char(I_CERT.REMOVAL_DATE, 'yyyymmdd') > to_char(I_CERT.EXPIRY_DATE, 'yyyymmdd'))) " +
                "and I_CERT.CATEGORY_ID IN('8a824790242974be01242974be530004', '8a824790242974be01242974be530009') " +
                "and I_QUALI.REGISTRATION_NUMBER IS NOT null " +
                "and I_QUALI.PRB_ID IN('8a82479024294eb70124294eb8420002') " +
                "ORDER BY ENAME ASC";
            return sql;
        }

        //CRM0032
        public string getConvictionCGCSql(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT " +
                "c_propercase(C_CVT.ENGLISH_NAME) || ' ' || C_CVT.CHINESE_NAME AS NAME," +
                "c_propercase(C_CVT.ENGLISH_NAME) AS ENG_COMP_NAME," +
                "C_CVT.CHINESE_NAME AS CHI_COMP_NAME," +
                "C_CVT.PROPRI_NAME AS PROPRI_NAME," +
                "C_CVT.SITE_DESCRIPTION AS SITE_DESC," +
                "C_CVT.REMARKS AS REMARKS," +
                "C_CVT.REFERENCE AS REFERENCE," +
                "CASE WHEN S_CVT.CODE = '6'THEN 's.27(3) PHMSO' ELSE C_CVT.CR_SECTION END AS SECTION," +
                "CASE WHEN C_CVT.CR_ACCIDENT = 'Y' THEN 'Yes' WHEN C_CVT.CR_ACCIDENT = 'N' THEN 'No'END AS ACCIDENT," +
                "CASE WHEN C_CVT.CR_FATAL = 'Y' THEN 'Yes' WHEN C_CVT.CR_FATAL = 'N' THEN 'No'END AS FATAL," +
                "C_CVT.CR_FINE AS FINE," +
                "to_char(C_CVT.CR_OFFENCE_DATE, 'DD/MM/YYYY') AS OFFENCE_D," +
                "to_char(C_CVT.CR_JUDGE_DATE, 'DD/MM/YYYY') AS JUDGE_D," +
                "to_char(C_CVT.DA_DECISION_DATE, 'DD/MM/YYYY') AS DA_DECISION_D," +
                "to_char(C_CVT.MISC_RECEIVING_DATE, 'DD/MM/YYYY') AS MISC_RECEIVING_D," +
                "to_char(C_CVT.SRR_SUSPENSION_FROM_DATE, 'DD/MM/YYYY') AS SUS_FR_D," +
                "to_char(C_CVT.SRR_SUSPENSION_TO_DATE, 'DD/MM/YYYY') AS SUS_TO_D," +
                "to_char(C_CVT.SRR_EFFECTIVE_DATE, 'DD/MM/YYYY') AS EFF_D," +
                "to_char(C_CVT.SRR_APPROVAL_DATE, 'DD/MM/YYYY') AS APPR_D," +
                "C_CVT.DA_DETAILS AS DA_DETAILS," +
                "C_CVT.MISC_DETAILS AS MISC_DETAILS," +
                "S_CVT.ENGLISH_DESCRIPTION AS S_CVT_DESC," +
                "C_CVT.SRR_CATEGORY AS SRR_CAT," +
                "C_CVT.SRR_SUSPENSION_DETAILS AS SRR_SUS_DETAILS," +
                "C_CVT.SRR_ACTION AS ACTION," +

                "CASE " +
                "WHEN C_CVT.RECORD_TYPE = 'S' THEN 'Suspension and Removal Records' " +
                "WHEN C_CVT.RECORD_TYPE = 'D' THEN 'Disciplinary Records' " +
                "WHEN C_CVT.RECORD_TYPE = 'M' THEN 'Miscellaneous' END AS HDR1," +

                "CASE " +
                "WHEN S_CVT.CODE = '6' THEN 'Convictions under s.27(3) of PHMSO' " +
                "WHEN S_CVT.CODE = '5'THEN 'Convictions under Labour Department' " +
                "WHEN S_CVT.CODE = '12'THEN 'Convictions under EPD' " +
                "ELSE 'Other Convictions (if any)' END AS HDR2," +

                "S_CVT.CODE AS CVT_CODE,C_CVT.RECORD_TYPE AS RECORD_TYPE," +

                "CASE " +
                "WHEN C_CVT.RECORD_TYPE = 'S' THEN 'LAYOUT1' " +
                "WHEN C_CVT.RECORD_TYPE = 'D' THEN 'LAYOUT2' " +
                "WHEN C_CVT.RECORD_TYPE = 'M' THEN 'LAYOUT5' " +
                "WHEN C_CVT.RECORD_TYPE NOT IN('S','D','M') AND S_CVT.CODE IN('5', '6') THEN 'LAYOUT3' " +
                "WHEN C_CVT.RECORD_TYPE NOT IN('S','D','M') AND S_CVT.CODE NOT IN('5','6') THEN 'LAYOUT4' END AS LAYOUT," +
                ":today AS UP_TO_TODAY " +

                "FROM C_COMP_CONVICTION C_CVT " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_CVT ON S_CVT.UUID = C_CVT.CONVICTION_SOURCE_ID " +
                "inner JOIN C_S_SYSTEM_TYPE SYS_TYPE ON S_CVT.SYSTEM_TYPE_ID = SYS_TYPE.UUID " +
                "WHERE SYS_TYPE.TYPE = 'CONVICTION_SOURCE' AND C_CVT.REGISTRATION_TYPE = :reg_type ");

            foreach (var item in myDictionary)
            {
                switch (item.Key)
                {
                    case "InfoSheetBRNo":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And C_CVT.BR_NO like '%:InfoSheetBRNo%' ");
                        break;
                    case "InfoSheetCompanyName":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            //string tempRegexpHql = item.Value.ToString().Replace(" ", "[[:space:],.-]*");
                            //tempRegexpHql = "'[[:space:],.-]*" + tempRegexpHql + "[[:space:],.-]*'";
                            //tempRegexpHql = tempRegexpHql.Replace("(", "\\(");
                            //tempRegexpHql = tempRegexpHql.Replace(")", "\\)");
                            //if (myDictionary["InfoSheetKeyword"].ToString() == "true")
                            //{
                            //    sql.Append(" And (REGEXP_LIKE(UPPER (C_CVT.ENGLISH_NAME)," + tempRegexpHql + ") or REGEXP_LIKE(C_CVT.CHINESE_NAME," + tempRegexpHql + ")) ");
                            //}
                            //else
                            //{
                            //    sql.Append(" And (REGEXP_LIKE(UPPER (C_CVT.ENGLISH_NAME)," + tempRegexpHql + ") or REGEXP_LIKE(C_CVT.CHINESE_NAME," + tempRegexpHql + ")) ");
                            //}
                            sql.Append(" And ( ( C_CVT.ENGLISH_NAME like '%" + item.Value.ToString() + "%' ) or ( C_CVT.CHINESE_NAME like '%" + item.Value.ToString() + "%' )  )");
                        }
                        break;
                    case "InfoSheetOrdinanceOrRegulation":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And C_CVT.CR_SECTION like '%" + item.Value.ToString() + "%' ");
                        break;
                    case "InfoSheetSuspensionReason":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And C_CVT.SRR_SUSPENSION_DETAILS like '%" + item.Value.ToString() + "%' ");
                        break;
                    case "InfoSheetODateFrom":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And C_CVT.CR_OFFENCE_DATE >= to_date(:InfoSheetODateFrom,'dd/mm/yyyy') ");
                        break;
                    case "InfoSheetODateTo":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And C_CVT.CR_OFFENCE_DATE <= to_date(:InfoSheetODateTo,'dd/mm/yyyy') ");
                        break;
                    case "InfoSheetJDateFrom":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(@" And (
                                         ( C_CVT.CR_JUDGE_DATE >= to_date(:InfoSheetJDateFrom,'dd/mm/yyyy') And C_CVT.RECORD_TYPE = 'C' ) or
                                         ( C_CVT.SRR_APPROVAL_DATE >= to_date(:InfoSheetJDateFrom,'dd/mm/yyyy') And C_CVT.RECORD_TYPE = 'S' ) or
                                         ( C_CVT.SRR_EFFECTIVE_DATE >= to_date(:InfoSheetJDateFrom,'dd/mm/yyyy') And C_CVT.RECORD_TYPE = 'S' ) or
                                         ( C_CVT.DA_DECISION_DATE >= to_date(:InfoSheetJDateFrom,'dd/mm/yyyy') And C_CVT.RECORD_TYPE = 'D' ) or
                                         ( C_CVT.MISC_RECEIVING_DATE >= to_date(:InfoSheetJDateFrom,'dd/mm/yyyy') And C_CVT.RECORD_TYPE = 'M' )   
                                            )
                                          ");
                        break;
                    case "InfoSheetJDateTo":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(@" And (
                                         ( C_CVT.CR_JUDGE_DATE <= to_date(:InfoSheetJDateTo,'dd/mm/yyyy') And C_CVT.RECORD_TYPE = 'C' ) or
                                         ( C_CVT.SRR_APPROVAL_DATE <= to_date(:InfoSheetJDateTo,'dd/mm/yyyy') And C_CVT.RECORD_TYPE = 'S' ) or
                                         ( C_CVT.SRR_EFFECTIVE_DATE <= to_date(:InfoSheetJDateTo,'dd/mm/yyyy') And C_CVT.RECORD_TYPE = 'S' ) or
                                         ( C_CVT.DA_DECISION_DATE <= to_date(:InfoSheetJDateTo,'dd/mm/yyyy') And C_CVT.RECORD_TYPE = 'D' ) or
                                         ( C_CVT.MISC_RECEIVING_DATE <= to_date(:InfoSheetJDateTo,'dd/mm/yyyy') And C_CVT.RECORD_TYPE = 'M' )   
                                            )
                                        ");
                        break;
                    case "InfoSheetCnvSource":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And C_CVT.CONVICTION_SOURCE_ID = :InfoSheetCnvSource");
                        break;
                    case "InfoSheetPrint":
                        if (item.Value.ToString() == "false")
                            sql.Append(@" And ( (C_CVT.CR_REPORT = 'N' ) or (C_CVT.CR_REPORT is null ) ) 
                                          And ( (C_CVT.SRR_REPORT = 'N' ) or (C_CVT.SRR_REPORT is null ) )  
                                          And ( (C_CVT.DA_REPORT = 'N' ) or (C_CVT.DA_REPORT is null ) )
                                          And ( (C_CVT.MISC_REPORT = 'N' ) or (C_CVT.MISC_REPORT is null ) ) 
                                        ");
                        break;
                }
            }

            sql.Append(" ORDER BY  NAME, HDR1,HDR2 , OFFENCE_D DESC");


            //string sql = "SELECT " +
            //    "c_propercase(C_CVT.ENGLISH_NAME) || ' ' || C_CVT.CHINESE_NAME AS NAME," +
            //    "c_propercase(C_CVT.ENGLISH_NAME) AS ENG_COMP_NAME," +
            //    "C_CVT.CHINESE_NAME AS CHI_COMP_NAME," +
            //    "C_CVT.PROPRI_NAME AS PROPRI_NAME," +
            //    "C_CVT.SITE_DESCRIPTION AS SITE_DESC," +
            //    "C_CVT.REMARKS AS REMARKS," +
            //    "C_CVT.REFERENCE AS REFERENCE," +
            //    "CASE WHEN S_CVT.CODE = '6'THEN 's.27(3) PHMSO' ELSE C_CVT.CR_SECTION END AS SECTION," +
            //    "CASE WHEN C_CVT.CR_ACCIDENT = 'Y' THEN 'Yes' WHEN C_CVT.CR_ACCIDENT = 'N' THEN 'No'END AS ACCIDENT," +
            //    "CASE WHEN C_CVT.CR_FATAL = 'Y' THEN 'Yes' WHEN C_CVT.CR_FATAL = 'N' THEN 'No'END AS FATAL," +
            //    "C_CVT.CR_FINE AS FINE," +
            //    "to_char(C_CVT.CR_OFFENCE_DATE, 'DD/MM/YYYY') AS OFFENCE_D," +
            //    "to_char(C_CVT.CR_JUDGE_DATE, 'DD/MM/YYYY') AS JUDGE_D," +
            //    "to_char(C_CVT.DA_DECISION_DATE, 'DD/MM/YYYY') AS DA_DECISION_D," +
            //    "to_char(C_CVT.MISC_RECEIVING_DATE, 'DD/MM/YYYY') AS MISC_RECEIVING_D," +
            //    "to_char(C_CVT.SRR_SUSPENSION_FROM_DATE, 'DD/MM/YYYY') AS SUS_FR_D," +
            //    "to_char(C_CVT.SRR_SUSPENSION_TO_DATE, 'DD/MM/YYYY') AS SUS_TO_D," +
            //    "to_char(C_CVT.SRR_EFFECTIVE_DATE, 'DD/MM/YYYY') AS EFF_D," +
            //    "to_char(C_CVT.SRR_APPROVAL_DATE, 'DD/MM/YYYY') AS APPR_D," +
            //    "C_CVT.DA_DETAILS AS DA_DETAILS," +
            //    "C_CVT.MISC_DETAILS AS MISC_DETAILS," +
            //    "S_CVT.ENGLISH_DESCRIPTION AS S_CVT_DESC," +
            //    "C_CVT.SRR_CATEGORY AS SRR_CAT," +
            //    "C_CVT.SRR_SUSPENSION_DETAILS AS SRR_SUS_DETAILS," +
            //    "C_CVT.SRR_ACTION AS ACTION," +

            //    "CASE " +
            //    "WHEN C_CVT.RECORD_TYPE = 'S' THEN 'Suspension and Removal Records' " +
            //    "WHEN C_CVT.RECORD_TYPE = 'D' THEN 'Disciplinary Records' " +
            //    "WHEN C_CVT.RECORD_TYPE = 'M' THEN 'Miscellaneous' END AS HDR1," +

            //    "CASE " +
            //    "WHEN S_CVT.CODE = '6' THEN 'Convictions under s.27(3) of PHMSO' " +
            //    "WHEN S_CVT.CODE = '5'THEN 'Convictions under Labour Department' " +
            //    "WHEN S_CVT.CODE = '12'THEN 'Convictions under EPD' " +
            //    "ELSE 'Other Convictions (if any)' END AS HDR2," +

            //    "S_CVT.CODE AS CVT_CODE,C_CVT.RECORD_TYPE AS RECORD_TYPE," +

            //    "CASE " +
            //    "WHEN C_CVT.RECORD_TYPE = 'S' THEN 'LAYOUT1' " +
            //    "WHEN C_CVT.RECORD_TYPE = 'D' THEN 'LAYOUT2' " +
            //    "WHEN C_CVT.RECORD_TYPE = 'M' THEN 'LAYOUT5' " +
            //    "WHEN C_CVT.RECORD_TYPE NOT IN('S','D','M') AND S_CVT.CODE IN('5', '6') THEN 'LAYOUT3' " +
            //    "WHEN C_CVT.RECORD_TYPE NOT IN('S','D','M') AND S_CVT.CODE NOT IN('5','6') THEN 'LAYOUT4' END AS LAYOUT," +
            //    ":today AS UP_TO_TODAY " +

            //    "FROM C_COMP_CONVICTION C_CVT " +
            //    "LEFT JOIN C_S_SYSTEM_VALUE S_CVT ON S_CVT.UUID = C_CVT.CONVICTION_SOURCE_ID " +
            //    "inner JOIN C_S_SYSTEM_TYPE SYS_TYPE ON S_CVT.SYSTEM_TYPE_ID = SYS_TYPE.UUID " +
            //    "WHERE SYS_TYPE.TYPE = 'CONVICTION_SOURCE' AND C_CVT.REGISTRATION_TYPE = :reg_type " +
            //    "ORDER BY  NAME, HDR1,HDR2 , OFFENCE_D DESC";
            return sql.ToString();
        }

        //CRM0033
        public string getConvictionIP(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT " +
                "c_propercase(I_CVT.SURNAME) || ' ' || c_propercase(I_CVT.GIVEN_NAME) || ' ' || I_CVT.CHINESE_NAME AS NAME," +
                "c_propercase(I_CVT.ENGLISH_COMPANY_NAME) || ' ' || I_CVT.CHINESE_COMPANY_NAME AS C_NAME," +
                "I_CVT.SITE_DESCRIPTION  AS SITE_DESC," +
                "I_CVT.CR_SECTION AS SECTION," +
                "I_CVT.SRR_ACTION AS ACTION," +
                "I_CVT.SRR_DETAILS AS SRR_DETAILS," +
                "I_CVT.REMARKS AS REMARKS," +
                "CASE WHEN I_CVT.CR_ACCIDENT = 'Y' THEN 'Yes' WHEN I_CVT.CR_ACCIDENT = 'N' THEN 'No' END AS ACCIDENT," +
                "CASE WHEN I_CVT.CR_FATAL = 'Y' THEN 'Yes' WHEN I_CVT.CR_FATAL = 'N'THEN 'No' END AS FATAL," +
                "I_CVT.CR_FINE AS FINE,I_CVT.DA_DETAILS AS DA_DETAILS,I_CVT.MISC_DETAILS AS MISC_DETAILS," +
                "S_CVT.ENGLISH_DESCRIPTION AS S_CVT_DESC,I_CVT.SRR_CATEGORY AS SRR_CAT," +
                "to_char(I_CVT.CR_OFFENCE_DATE, 'DD/MM/YYYY')AS OFFENCE_D,to_char(I_CVT.CR_JUDGEMENT_DATE, 'DD/MM/YYYY')AS JUDGE_D," +
                "to_char(I_CVT.DA_DECISION_DATE, 'DD/MM/YYYY')AS DA_DECISION_D," +
                "to_char(I_CVT.MISC_RECEIVING_DATE, 'DD/MM/YYYY')AS MISC_RECEIVING_D," +
                "to_char(I_CVT.SRR_SUSPENSION_FROM_DATE, 'DD/MM/YYYY')AS SUS_FR_D," +
                "to_char(I_CVT.SRR_EFFECT_DATE, 'DD/MM/YYYY')AS EFF_D," +
                "to_char(I_CVT.SRR_APPROVAL_DATE, 'DD/MM/YYYY')AS APPR_D," +
                "CASE WHEN I_CVT.RECORD_TYPE = 'S' THEN 'Suspension and Removal Records' WHEN I_CVT.RECORD_TYPE = 'D' THEN 'Disciplinary Records'" +
                "WHEN I_CVT.RECORD_TYPE = 'M' THEN 'Miscellaneous' END AS HDR1," +
                "CASE WHEN S_CVT.CODE = '6' THEN 'Convictions under s.27(3) of PHMSO' " +
                "WHEN S_CVT.CODE = '5'THEN 'Convictions under Labour Department'ELSE 'Other Convictions (if any)' END AS HDR2," +
                "S_CVT.CODE AS CVT_CODE," +
                "I_CVT.RECORD_TYPE AS RECORD_TYPE," +
                "CASE WHEN I_CVT.RECORD_TYPE = 'S' AND I_CVT.SRR_REPORT = 'Y' THEN 'LAYOUT1' " +
                "WHEN I_CVT.RECORD_TYPE = 'D' AND I_CVT.DA_REPORT = 'Y' THEN 'LAYOUT2' " +
                "WHEN I_CVT.RECORD_TYPE = 'C' AND S_CVT.CODE IN('5', '6') THEN 'LAYOUT3' " +
                "WHEN I_CVT.RECORD_TYPE = 'C' AND S_CVT.CODE NOT IN('5','6') THEN 'LAYOUT4' " +
                "WHEN I_CVT.RECORD_TYPE = 'M' AND I_CVT.SRR_REPORT = 'Y'  THEN 'LAYOUT5' " +
                "END AS LAYOUT," +
                ":today AS UP_TO_TODAY " +
                "FROM C_IND_CONVICTION I_CVT " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_CVT ON S_CVT.UUID = I_CVT.CONVICTION_SOURCE_ID " +
                "LEFT JOIN C_S_SYSTEM_TYPE SYS_TYPE ON S_CVT.SYSTEM_TYPE_ID = SYS_TYPE.UUID AND SYS_TYPE.TYPE = 'CONVICTION_SOURCE' " +
                "WHERE I_CVT.REGISTRATION_TYPE = 'IP' "
                //"AND S_CVT.CODE <> '5'AND RECORD_TYPE<>'C'  "
                );

            if (myDictionary.Keys.Contains("Surname"))
            {
                if (!string.IsNullOrEmpty(myDictionary["Surname"].ToString()))
                    sql.Append(" And upper(I_CVT.SURNAME) like '%" + myDictionary["Surname"].ToString().Trim().ToUpper() + "%' ");
                if (!string.IsNullOrEmpty(myDictionary["GivenName"].ToString()))
                    sql.Append(" And upper(I_CVT.GIVEN_NAME) like '%" + myDictionary["GivenName"].ToString().Trim().ToUpper() + "%' ");
            }

            if (myDictionary.Keys.Contains("InfoSurname"))
            {
                if (!string.IsNullOrEmpty(myDictionary["InfoSurname"].ToString()))
                    sql.Append(" And upper(I_CVT.SURNAME) like '%" + myDictionary["InfoSurname"].ToString().Trim().ToUpper() + "%' ");
                if (!string.IsNullOrEmpty(myDictionary["InfoGivenName"].ToString()))
                    sql.Append(" And upper(I_CVT.GIVEN_NAME) like '%" + myDictionary["InfoGivenName"].ToString().Trim().ToUpper() + "%' ");
            }


            if (!string.IsNullOrEmpty(myDictionary["offence_fr_date"].ToString()))
                sql.Append(" And I_CVT.CR_OFFENCE_DATE >= to_date(:offence_fr_date,'dd/mm/yyyy') ");
            if (!string.IsNullOrEmpty(myDictionary["offence_to_date"].ToString()))
                sql.Append(" And I_CVT.CR_OFFENCE_DATE <= to_date(:offence_to_date,'dd/mm/yyyy') ");
            if (!string.IsNullOrEmpty(myDictionary["InfoSheetJDateFrom"].ToString()))
                sql.Append(" And I_CVT.CR_JUDGEMENT_DATE >= to_date(:InfoSheetJDateFrom,'dd/mm/yyyy') ");
            if (!string.IsNullOrEmpty(myDictionary["InfoSheetJDateTo"].ToString()))
                sql.Append(" And I_CVT.CR_JUDGEMENT_DATE <= to_date(:InfoSheetJDateTo,'dd/mm/yyyy') ");

            sql.Append(" ORDER BY LAYOUT ");


            //string sql = "SELECT " +
            //    "c_propercase(I_CVT.SURNAME) || ' ' || c_propercase(I_CVT.GIVEN_NAME) || ' ' || I_CVT.CHINESE_NAME AS NAME," +
            //    "c_propercase(I_CVT.ENGLISH_COMPANY_NAME) || ' ' || I_CVT.CHINESE_COMPANY_NAME AS C_NAME," +
            //    "I_CVT.SITE_DESCRIPTION  AS SITE_DESC," +
            //    "I_CVT.CR_SECTION AS SECTION," +
            //    "I_CVT.SRR_ACTION AS ACTION," +
            //    "I_CVT.SRR_DETAILS AS SRR_DETAILS," +
            //    "I_CVT.REMARKS AS REMARKS," +
            //    "CASE WHEN I_CVT.CR_ACCIDENT = 'Y' THEN 'Yes' WHEN I_CVT.CR_ACCIDENT = 'N' THEN 'No' END AS ACCIDENT," +
            //    "CASE WHEN I_CVT.CR_FATAL = 'Y' THEN 'Yes' WHEN I_CVT.CR_FATAL = 'N'THEN 'No' END AS FATAL," +
            //    "I_CVT.CR_FINE AS FINE,I_CVT.DA_DETAILS AS DA_DETAILS,I_CVT.MISC_DETAILS AS MISC_DETAILS," +
            //    "S_CVT.ENGLISH_DESCRIPTION AS S_CVT_DESC,I_CVT.SRR_CATEGORY AS SRR_CAT," +
            //    "to_char(I_CVT.CR_OFFENCE_DATE, 'DD/MM/YYYY')AS OFFENCE_D,to_char(I_CVT.CR_JUDGEMENT_DATE, 'DD/MM/YYYY')AS JUDGE_D," +
            //    "to_char(I_CVT.DA_DECISION_DATE, 'DD/MM/YYYY')AS DA_DECISION_D," +
            //    "to_char(I_CVT.MISC_RECEIVING_DATE, 'DD/MM/YYYY')AS MISC_RECEIVING_D," +
            //    "to_char(I_CVT.SRR_SUSPENSION_FROM_DATE, 'DD/MM/YYYY')AS SUS_FR_D," +
            //    "to_char(I_CVT.SRR_EFFECT_DATE, 'DD/MM/YYYY')AS EFF_D," +
            //    "to_char(I_CVT.SRR_APPROVAL_DATE, 'DD/MM/YYYY')AS APPR_D," +
            //    "CASE WHEN I_CVT.RECORD_TYPE = 'S' THEN 'Suspension and Removal Records' WHEN I_CVT.RECORD_TYPE = 'D' THEN 'Disciplinary Records'" +
            //    "WHEN I_CVT.RECORD_TYPE = 'M' THEN 'Miscellaneous' END AS HDR1," +
            //    "CASE WHEN S_CVT.CODE = '6' THEN 'Convictions under s.27(3) of PHMSO' " +
            //    "WHEN S_CVT.CODE = '5'THEN 'Convictions under Labour Department'ELSE 'Other Convictions (if any)' END AS HDR2," +
            //    "S_CVT.CODE AS CVT_CODE," +
            //    "I_CVT.RECORD_TYPE AS RECORD_TYPE," +
            //    "CASE WHEN I_CVT.RECORD_TYPE = 'S' AND I_CVT.SRR_REPORT = 'Y' THEN 'LAYOUT1' " +
            //    "WHEN I_CVT.RECORD_TYPE = 'D' AND I_CVT.DA_REPORT = 'Y' THEN 'LAYOUT2' " +
            //    "WHEN I_CVT.RECORD_TYPE = 'C' AND S_CVT.CODE IN('5', '6') THEN 'LAYOUT3' " +
            //    "WHEN I_CVT.RECORD_TYPE = 'C' AND S_CVT.CODE NOT IN('5','6') THEN 'LAYOUT4' " +
            //    "WHEN I_CVT.RECORD_TYPE = 'M' AND I_CVT.SRR_REPORT = 'Y'  THEN 'LAYOUT5' " +
            //    "END AS LAYOUT," +
            //    ":today AS UP_TO_TODAY " +
            //    "FROM C_IND_CONVICTION I_CVT " +
            //    "LEFT JOIN C_S_SYSTEM_VALUE S_CVT ON S_CVT.UUID = I_CVT.CONVICTION_SOURCE_ID " +
            //    "LEFT JOIN C_S_SYSTEM_TYPE SYS_TYPE ON S_CVT.SYSTEM_TYPE_ID = SYS_TYPE.UUID AND SYS_TYPE.TYPE = 'CONVICTION_SOURCE' " +
            //    "WHERE I_CVT.REGISTRATION_TYPE = 'IP' AND S_CVT.CODE <> '5'AND RECORD_TYPE<>'C' ORDER BY LAYOUT";
            return sql.ToString();
        }

        //CRM0034
        public string getConvictionIP2()
        {
            string sql = "SELECT " +
                "c_propercase(I_CVT.SURNAME) || ' ' || c_propercase(I_CVT.GIVEN_NAME) || ' ' || I_CVT.CHINESE_NAME AS NAME," +
                "c_propercase(I_CVT.ENGLISH_COMPANY_NAME) || ' ' || I_CVT.CHINESE_COMPANY_NAME AS C_NAME," +
                "I_CVT.SITE_DESCRIPTION  AS SITE_DESC," +
                "I_CVT.CR_SECTION AS SECTION," +
                "I_CVT.SRR_ACTION AS ACTION," +
                "I_CVT.SRR_DETAILS AS SRR_DETAILS," +
                "I_CVT.REMARKS AS REMARKS," +
                "CASE WHEN I_CVT.CR_ACCIDENT = 'Y' THEN 'Yes' WHEN I_CVT.CR_ACCIDENT = 'N' THEN 'No' END AS ACCIDENT," +
                "CASE WHEN I_CVT.CR_FATAL = 'Y' THEN 'Yes' WHEN I_CVT.CR_FATAL = 'N'THEN 'No' END AS FATAL," +
                "I_CVT.CR_FINE AS FINE,I_CVT.DA_DETAILS AS DA_DETAILS,I_CVT.MISC_DETAILS AS MISC_DETAILS," +
                "S_CVT.ENGLISH_DESCRIPTION AS S_CVT_DESC,I_CVT.SRR_CATEGORY AS SRR_CAT," +
                "to_char(I_CVT.CR_OFFENCE_DATE, 'DD/MM/YYYY')AS OFFENCE_D,to_char(I_CVT.CR_JUDGEMENT_DATE, 'DD/MM/YYYY')AS JUDGE_D," +
                "to_char(I_CVT.DA_DECISION_DATE, 'DD/MM/YYYY')AS DA_DECISION_D," +
                "to_char(I_CVT.MISC_RECEIVING_DATE, 'DD/MM/YYYY')AS MISC_RECEIVING_D," +
                "to_char(I_CVT.SRR_SUSPENSION_FROM_DATE, 'DD/MM/YYYY')AS SUS_FR_D," +
                "to_char(I_CVT.SRR_EFFECT_DATE, 'DD/MM/YYYY')AS EFF_D," +
                "to_char(I_CVT.SRR_APPROVAL_DATE, 'DD/MM/YYYY')AS APPR_D," +
                "CASE WHEN I_CVT.RECORD_TYPE = 'S' THEN 'Suspension and Removal Records' WHEN I_CVT.RECORD_TYPE = 'D' THEN 'Disciplinary Records'" +
                "WHEN I_CVT.RECORD_TYPE = 'M' THEN 'Miscellaneous' END AS HDR1," +
                "CASE WHEN S_CVT.CODE = '6' THEN 'Convictions under s.27(3) of PHMSO' " +
                "WHEN S_CVT.CODE = '5'THEN 'Convictions under Labour Department'ELSE 'Other Convictions (if any)' END AS HDR2," +
                "S_CVT.CODE AS CVT_CODE," +
                "I_CVT.RECORD_TYPE AS RECORD_TYPE," +
                "CASE WHEN I_CVT.RECORD_TYPE = 'S' AND I_CVT.SRR_REPORT = 'Y' THEN 'LAYOUT1' " +
                "WHEN I_CVT.RECORD_TYPE = 'D' AND I_CVT.DA_REPORT = 'Y' THEN 'LAYOUT2' " +
                "WHEN I_CVT.RECORD_TYPE = 'C' AND S_CVT.CODE IN('5', '6') THEN 'LAYOUT3' " +
                "WHEN I_CVT.RECORD_TYPE = 'C' AND S_CVT.CODE NOT IN('5','6') THEN 'LAYOUT4' " +
                "WHEN I_CVT.RECORD_TYPE = 'M' AND I_CVT.SRR_REPORT = 'Y'  THEN 'LAYOUT5' " +
                "END AS LAYOUT," +
                ":today AS UP_TO_TODAY " +
                "FROM C_IND_CONVICTION I_CVT " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_CVT ON S_CVT.UUID = I_CVT.CONVICTION_SOURCE_ID " +
                "LEFT JOIN C_S_SYSTEM_TYPE SYS_TYPE ON S_CVT.SYSTEM_TYPE_ID = SYS_TYPE.UUID AND SYS_TYPE.TYPE = 'CONVICTION_SOURCE' " +
                "WHERE I_CVT.REGISTRATION_TYPE = 'IP' AND S_CVT.CODE <> '5'AND RECORD_TYPE<>'C' ORDER BY LAYOUT";
            return sql;
        }

        //CRM0035
        public string getConvictionIP3()
        {
            string sql = "( SELECT " +
                "c_propercase(I_CVT.SURNAME) || ' ' || c_propercase(I_CVT.GIVEN_NAME) || ' ' || I_CVT.CHINESE_NAME AS NAME," +
                "c_propercase(I_CVT.ENGLISH_COMPANY_NAME) || ' ' || I_CVT.CHINESE_COMPANY_NAME AS C_NAME," +
                "I_CVT.SITE_DESCRIPTION  AS SITE_DESC," +
                "I_CVT.CR_SECTION AS SECTION," +
                "I_CVT.SRR_ACTION AS ACTION," +
                "I_CVT.SRR_DETAILS AS SRR_DETAILS," +
                "I_CVT.REMARKS AS REMARKS," +
                "CASE WHEN I_CVT.CR_ACCIDENT = 'Y' THEN 'Yes' WHEN I_CVT.CR_ACCIDENT = 'N' THEN 'No' END AS ACCIDENT," +
                "CASE WHEN I_CVT.CR_FATAL = 'Y' THEN 'Yes' WHEN I_CVT.CR_FATAL = 'N'THEN 'No' END AS FATAL," +
                "I_CVT.CR_FINE AS FINE,I_CVT.DA_DETAILS AS DA_DETAILS,I_CVT.MISC_DETAILS AS MISC_DETAILS," +
                "S_CVT.ENGLISH_DESCRIPTION AS S_CVT_DESC,I_CVT.SRR_CATEGORY AS SRR_CAT," +
                "to_char(I_CVT.CR_OFFENCE_DATE, 'DD/MM/YYYY')AS OFFENCE_D,to_char(I_CVT.CR_JUDGEMENT_DATE, 'DD/MM/YYYY')AS JUDGE_D," +
                "to_char(I_CVT.DA_DECISION_DATE, 'DD/MM/YYYY')AS DA_DECISION_D," +
                "to_char(I_CVT.MISC_RECEIVING_DATE, 'DD/MM/YYYY')AS MISC_RECEIVING_D," +
                "to_char(I_CVT.SRR_SUSPENSION_FROM_DATE, 'DD/MM/YYYY')AS SUS_FR_D," +
                "to_char(I_CVT.SRR_EFFECT_DATE, 'DD/MM/YYYY')AS EFF_D," +
                "to_char(I_CVT.SRR_APPROVAL_DATE, 'DD/MM/YYYY')AS APPR_D," +
                "CASE WHEN I_CVT.RECORD_TYPE = 'S' THEN 'Suspension and Removal Records' WHEN I_CVT.RECORD_TYPE = 'D' THEN 'Disciplinary Records'" +
                "WHEN I_CVT.RECORD_TYPE = 'M' THEN 'Miscellaneous' END AS HDR1," +
                "CASE WHEN S_CVT.CODE = '6' THEN 'Convictions under s.27(3) of PHMSO' " +
                "WHEN S_CVT.CODE = '5'THEN 'Convictions under Labour Department'ELSE 'Other Convictions (if any)' END AS HDR2," +
                "S_CVT.CODE AS CVT_CODE," +
                "I_CVT.RECORD_TYPE AS RECORD_TYPE," +
                "CASE WHEN I_CVT.RECORD_TYPE = 'S' AND I_CVT.SRR_REPORT = 'Y' THEN 'LAYOUT1' " +
                "WHEN I_CVT.RECORD_TYPE = 'D' AND I_CVT.DA_REPORT = 'Y' THEN 'LAYOUT2' " +
                "WHEN I_CVT.RECORD_TYPE = 'C' AND S_CVT.CODE IN('5', '6') THEN 'LAYOUT3' " +
                "WHEN I_CVT.RECORD_TYPE = 'C' AND S_CVT.CODE NOT IN('5','6') THEN 'LAYOUT4' " +
                "WHEN I_CVT.RECORD_TYPE = 'M' AND I_CVT.SRR_REPORT = 'Y'  THEN 'LAYOUT5' " +
                "END AS LAYOUT," +
                ":today AS UP_TO_TODAY " +
                "FROM C_IND_CONVICTION I_CVT " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_CVT ON S_CVT.UUID = I_CVT.CONVICTION_SOURCE_ID " +
                "LEFT JOIN C_S_SYSTEM_TYPE SYS_TYPE ON S_CVT.SYSTEM_TYPE_ID = SYS_TYPE.UUID AND SYS_TYPE.TYPE = 'CONVICTION_SOURCE' " +
                "WHERE I_CVT.REGISTRATION_TYPE = 'IP' AND S_CVT.CODE <> '5'AND RECORD_TYPE<>'C' ) " +
                "UNION " +
                "(SELECT " +
                "c_propercase(I_CVT.SURNAME) || ' ' || c_propercase(I_CVT.GIVEN_NAME) || ' ' || I_CVT.CHINESE_NAME AS NAME," +
                "c_propercase(I_CVT.ENGLISH_COMPANY_NAME) || ' ' || I_CVT.CHINESE_COMPANY_NAME AS C_NAME," +
                "I_CVT.SITE_DESCRIPTION AS SITE_DESC," +
                "I_CVT.CR_SECTION AS SECTION,I_CVT.SRR_ACTION AS ACTION,I_CVT.SRR_DETAILS AS SRR_DETAILS,I_CVT.REMARKS AS REMARKS," +
                "CASE WHEN I_CVT.CR_ACCIDENT = 'Y' THEN 'Yes' WHEN I_CVT.CR_ACCIDENT = 'N' THEN 'No' END AS ACCIDENT," +
                "CASE WHEN I_CVT.CR_FATAL = 'Y' THEN 'Yes' WHEN I_CVT.CR_FATAL = 'N'THEN 'No' END AS FATAL," +
                "I_CVT.CR_FINE AS FINE,I_CVT.DA_DETAILS AS DA_DETAILS,I_CVT.MISC_DETAILS AS MISC_DETAILS," +
                "S_CVT.ENGLISH_DESCRIPTION AS S_CVT_DESC,I_CVT.SRR_CATEGORY AS SRR_CAT," +
                "to_char(I_CVT.CR_OFFENCE_DATE, 'DD/MM/YYYY')AS OFFENCE_D," +
                "to_char(I_CVT.CR_JUDGEMENT_DATE, 'DD/MM/YYYY')AS JUDGE_D," +
                "to_char(I_CVT.DA_DECISION_DATE, 'DD/MM/YYYY')AS DA_DECISION_D," +
                "to_char(I_CVT.MISC_RECEIVING_DATE, 'DD/MM/YYYY')AS MISC_RECEIVING_D," +
                "to_char(I_CVT.SRR_SUSPENSION_FROM_DATE, 'DD/MM/YYYY')AS SUS_FR_D," +
                "to_char(I_CVT.SRR_EFFECT_DATE, 'DD/MM/YYYY')AS EFF_D," +
                "to_char(I_CVT.SRR_APPROVAL_DATE, 'DD/MM/YYYY')AS APPR_D," +
                " CASE WHEN I_CVT.RECORD_TYPE = 'S' THEN 'Suspension and Removal Records' " +
                "WHEN I_CVT.RECORD_TYPE = 'D' THEN 'Disciplinary Records' " +
                "WHEN I_CVT.RECORD_TYPE = 'M' THEN 'Miscellaneous' END AS HDR1," +
                "CASE WHEN S_CVT.CODE = '6' THEN 'Convictions under s.27(3) of PHMSO' " +
                "WHEN S_CVT.CODE = '5'THEN 'Convictions under Labour Department'ELSE 'Other Convictions (if any)' END AS HDR2," +
                "S_CVT.CODE AS CVT_CODE,I_CVT.RECORD_TYPE AS RECORD_TYPE," +
                "CASE WHEN I_CVT.RECORD_TYPE = 'S' AND I_CVT.SRR_REPORT = 'Y' THEN 'LAYOUT1' " +
                "WHEN I_CVT.RECORD_TYPE = 'D' AND I_CVT.DA_REPORT = 'Y' THEN 'LAYOUT2' " +
                "WHEN I_CVT.RECORD_TYPE = 'C' AND I_CVT.CR_REPORT = 'Y' AND S_CVT.CODE IN('5', '6') THEN 'LAYOUT3' " +
                "WHEN I_CVT.RECORD_TYPE = 'C' AND I_CVT.CR_REPORT = 'Y' AND S_CVT.CODE NOT IN('5','6') THEN 'LAYOUT4' " +
                "WHEN I_CVT.RECORD_TYPE = 'M' AND I_CVT.SRR_REPORT = 'Y'  THEN 'LAYOUT5' END AS LAYOUT," +
                ":today AS UP_TO_TODAY " +
                "FROM " +
                "C_IND_CONVICTION I_CVT " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_CVT ON S_CVT.UUID = I_CVT.CONVICTION_SOURCE_ID " +
                "LEFT JOIN C_S_SYSTEM_TYPE SYS_TYPE ON S_CVT.SYSTEM_TYPE_ID = SYS_TYPE.UUID " +
                " AND SYS_TYPE.TYPE = 'CONVICTION_SOURCE' " +
                "WHERE I_CVT.REGISTRATION_TYPE = 'IP' AND S_CVT.CODE <> '5'AND RECORD_TYPE<>'C' )order by LAYOUT";
            return sql;
        }

        //CRM0036
        public string getFastTrackCGCSql()
        {
            string sql = "SELECT " +
                "(C_APPL.ENGLISH_COMPANY_NAME) AS ENAME," +
                "C_APPL.FILE_REFERENCE_NO AS FILE_REF," +
                "to_char(C_PMON.RECEIVED_DATE,'dd/mm/yyyy') AS RECEIVED_D," +
                "C_PMON.VETTING_OFFICER AS VO," +
                "C_PMON.REMARKS AS REMARKS," +
                "to_char(C_PMON.FAST_TRACK_DUE_28_DAYS_DATE,'dd/mm/yyyy') AS FAST_TRACK_DUE_28_D," +
                "to_char(C_PMON.CERTIFICATE_ISSUED_DATE,'dd/mm/yyyy') AS CERT_ISSUED_D," +
                "C_PMON.ASSISTANT AS ASSISTANT," +
                "C_PMON.FAST_TRCK AS FAST_TRCK," +
                "to_char(to_date(:today,'ddmmyyyy'),'dd/mm/yyyy') AS AS_AT_TODAY " +
                "FROM " +
                "C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_PROCESS_MONITOR C_PMON ON C_APPL.UUID = C_PMON.MASTER_ID " +
                "WHERE C_PMON.MONITOR_TYPE = 'FaskTrack' and C_APPL.REGISTRATION_TYPE = :reg_type " +
                "ORDER BY FILE_REF ASC,ENAME ASC";
            //string sql = "SELECT " +
            //    "(C_APPL.ENGLISH_COMPANY_NAME) AS ENAME," +
            //    "C_APPL.FILE_REFERENCE_NO AS FILE_REF," +
            //    "to_char(C_PMON.RECEIVED_DATE,'dd/mm/yyyy') AS RECEIVED_D," +
            //    "C_PMON.VETTING_OFFICER AS VO," +
            //    "C_PMON.REMARKS AS REMARKS," +
            //    "to_char(C_PMON.FAST_TRACK_DUE_28_DAYS_DATE,'dd/mm/yyyy') AS FAST_TRACK_DUE_28_D," +
            //    "to_char(C_PMON.CERTIFICATE_ISSUED_DATE,'dd/mm/yyyy') AS CERT_ISSUED_D," +
            //    "C_PMON.ASSISTANT AS ASSISTANT," +
            //    "C_PMON.FAST_TRCK AS FAST_TRCK," +
            //    "to_char(to_date(:as_today,'ddmmyyyy'),'dd/mm/yyyy') AS AS_AT_TODAY " +
            //    "FROM " +
            //    "C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_PROCESS_MONITOR C_PMON ON C_APPL.UUID = C_PMON.MASTER_ID " +
            //    "WHERE C_PMON.MONITOR_TYPE = 'FaskTrack' and C_APPL.REGISTRATION_TYPE = :reg_type " +
            //    "ORDER BY FILE_REF ASC,ENAME ASC";
            return sql;
        }

        //CRM0037
        public string getExportAPRSERGEInformationIPSql()
        {
            string sql = "SELECT I_APPL.FILE_REFERENCE_NO AS FILE_REF, I_CERT.CERTIFICATION_NO AS CERT_NO, S_TITLE.CODE AS TITLE,APPLN.SURNAME AS SURNAME,APPLN.GIVEN_NAME_ON_ID AS GIVEN_NAME, " +
                "APPLN.CHINESE_NAME AS CNAME," +
                "H_ADDR.ADDRESS_LINE1 AS H_ADDR1," +
                "H_ADDR.ADDRESS_LINE2 AS H_ADDR2," +
                "H_ADDR.ADDRESS_LINE3 AS H_ADDR3," +
                "H_ADDR.ADDRESS_LINE4 AS H_ADDR4," +
                "H_ADDR.ADDRESS_LINE5 AS H_ADDR5," +
                "H_CADDR.ADDRESS_LINE1 AS H_CADDR1,H_CADDR.ADDRESS_LINE2 AS H_CADDR2,H_CADDR.ADDRESS_LINE3 AS H_CADDR3,H_CADDR.ADDRESS_LINE4 AS H_CADDR4,H_CADDR.ADDRESS_LINE5 AS H_CADDR5," +
                "I_APPL.ENGLISH_CARE_OF AS C_O," +
                "I_APPL.CHINESE_CARE_OF AS C_C_O," +
                "O_ADDR.ADDRESS_LINE1 AS O_ADDR1,O_ADDR.ADDRESS_LINE2 AS O_ADDR2,O_ADDR.ADDRESS_LINE3 AS O_ADDR3,O_ADDR.ADDRESS_LINE4 AS O_ADDR4,O_ADDR.ADDRESS_LINE5 AS O_ADDR5," +
                "O_CADDR.ADDRESS_LINE1 AS O_CADDR1,O_CADDR.ADDRESS_LINE2 AS O_CADDR2,O_CADDR.ADDRESS_LINE3 AS O_CADDR3,O_CADDR.ADDRESS_LINE4 AS O_CADDR4,O_CADDR.ADDRESS_LINE5 AS O_CADDR5," +
                "I_APPL.EMERGENCY_NO1 AS EMRG_NO1,I_APPL.EMERGENCY_NO2 AS EMRG_NO2,I_APPL.EMERGENCY_NO3 AS EMRG_NO3," +
                "I_APPL.TELEPHONE_NO1 AS TEL_NO1,I_APPL.TELEPHONE_NO2 AS TEL_NO2,I_APPL.TELEPHONE_NO3 AS TEL_NO3," +
                "I_APPL.FAX_NO1 AS FAX_NO1,I_APPL.FAX_NO2 AS FAX_NO2,I_APPL.EMAIL AS EMAIL,S_PNRC.ENGLISH_DESCRIPTION AS PNAP,S_PRB.ENGLISH_DESCRIPTION AS PRB_DESC,S_CAT.CODE AS CAT_CODE," +
                "to_char(I_CERT.EXPIRY_DATE, 'yyyy-mm-dd') AS EXP_D,S_APP_FORM.CODE AS FORM_USED,S_POV.CODE AS POV_CODE,S_STATUS.ENGLISH_DESCRIPTION AS APP_STATUS," +
                //"to_char(I_CERT.APPLICATION_DATE, 'yyyy-mm-dd')AS APP_DT," +
                "to_char(I_CERT.REGISTRATION_DATE, 'yyyy-mm-dd') AS REG_D," +
                "to_char(I_CERT.GAZETTE_DATE, 'yyyy-mm-dd') AS GAZETTE_D," +
                "to_char(I_CERT.APPROVAL_DATE, 'yyyy-mm-dd') AS APPROVAL_D," +
                "to_char(I_CERT.RETENTION_APPLICATION_DATE, 'yyyy-mm-dd') AS RETENTION_APPL_D," +
                "to_char(I_CERT.RETENTION_DATE, 'yyyy-mm-dd') AS RETENTION_COMMENCED," +
                "to_char(I_CERT.RESTORATION_APPLICATION_DATE, 'yyyy-mm-dd') AS RESTORATION_APPL_D," +
                "to_char(I_CERT.RESTORE_DATE, 'yyyy-mm-dd') AS RESTORATION_COMMENCED," +
                "to_char(I_CERT.REMOVAL_DATE, 'yyyy-mm-dd') AS REMOVED_FROM_REGISTER," +
                "to_char(I_CERT.EXTENDED_DATE, 'yyyy-mm-dd') AS EXTENDED_D," +
                "AUTH.ENGLISH_NAME AS AUTH_NAME," +
                "AUTH.CHINESE_NAME AS AUTH_CNAME," +
                "AUTH.ENGLISH_RANK AS AUTH_RANK," +
                "AUTH.CHINESE_RANK AS AUTH_CRANK," +
                "CASE WHEN S_CAT.CODE = 'RSE' THEN I_APPL.FILE_REFERENCE_NO || '(SE)' " +
                "WHEN S_CAT.CODE = 'RGE' THEN I_APPL.FILE_REFERENCE_NO || '(GE)' ELSE I_APPL.FILE_REFERENCE_NO " +
                "END AS LETTER_FILE_REF " +
                "FROM C_APPLICANT APPLN " +
                "INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "INNER JOIN C_IND_QUALIFICATION QUALI ON I_APPL.UUID = QUALI.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "LEFT JOIN C_S_AUTHORITY AUTH ON AUTH.UUID = '8a8247902429b5ad012429b5adfc000d' " +
                "LEFT JOIN C_ADDRESS H_ADDR ON I_APPL.ENGLISH_HOME_ADDRESS_ID = H_ADDR.UUID " +
                "LEFT JOIN C_ADDRESS H_CADDR ON I_APPL.CHINESE_HOME_ADDRESS_ID = H_CADDR.UUID " +
                "LEFT JOIN C_ADDRESS O_ADDR ON I_APPL.ENGLISH_OFFICE_ADDRESS_ID = O_ADDR.UUID " +
                "LEFT JOIN C_ADDRESS O_CADDR ON I_APPL.CHINESE_OFFICE_ADDRESS_ID = O_CADDR.UUID " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_PNRC ON I_APPL.PRACTICE_NOTES_ID = S_PNRC.UUID " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_PRB ON QUALI.PRB_ID = S_PRB.UUID " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_APP_FORM ON I_CERT.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_STATUS ON I_CERT.APPLICATION_STATUS_ID = S_STATUS.UUID " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_POV ON I_CERT.PERIOD_OF_VALIDITY_ID = S_POV.UUID " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON APPLN.TITLE_ID = S_TITLE.UUID " +
                "WHERE S_CAT.REGISTRATION_TYPE = 'IP' " +
                "AND S_APP_FORM.REGISTRATION_TYPE = 'IP' AND (I_CERT.REMOVAL_DATE is null or to_char(I_CERT.REMOVAL_DATE, 'yyyymmdd') > to_char(sysdate, 'yyyymmdd')) ";
            return sql;
        }

        //CRM0038
        public string getInterestedQpStatisticSql()
        {
            string sql = "SELECT REG_TYPE, TOTAL,'Interested QP Statistic as at ' || CURRENT_DATE AS CURRENT_DATE FROM( " +
    "SELECT 'AP/RSE/RI' AS REG_TYPE, count(*) AS TOTAL " +
         "FROM " +
         "C_IND_CERTIFICATE C, C_S_CATEGORY_CODE CAT, C_S_SYSTEM_VALUE CATGRP,C_APPLICANT APP, C_IND_APPLICATION A ,C_S_SYSTEM_VALUE statusCode WHERE C.CATEGORY_ID = cat.UUID " +
         "AND C.MASTER_ID = A.UUID AND A.REGISTRATION_TYPE = 'IP' AND CATGRP.UUID = CAT.CATEGORY_GROUP_ID AND CATGRP.UUID in (SELECT  CATEGORY_GROUP_ID FROM C_S_CATEGORY_CODE SCC WHERE SCC.CODE IN('AP(A)', 'RI(A)', 'RSE')) " +
         "AND A.APPLICANT_ID = APP.UUID  " +
         "AND statusCode.UUID = C.application_status_id " +
         "AND(statusCode.CODE = '1' or(C.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
         "AND C.CERTIFICATION_NO IS NOT NULL " +
         "AND( " +
         "(C.EXPIRY_DATE IS NOT NULL and C.EXPIRY_DATE >= CURRENT_DATE) or " +
         "(C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(C.EXPIRY_DATE < CURRENT_DATE)) " +
         ") " +
         "AND((C.REMOVAL_DATE IS NULL) OR(C.REMOVAL_DATE > CURRENT_DATE))  " +
         "AND a.WILLINGNESS_QP = 'Y' " +

"UNION ALL " +

        "SELECT 'GBC', count(DISTINCT C.CERTIFICATION_NO) FROM C_S_CATEGORY_CODE CAT, C_COMP_APPLICATION C " +
        "WHERE CAT.UUID = C.CATEGORY_ID AND C.CATEGORY_ID = (SELECT  UUID FROM C_S_CATEGORY_CODE SCC WHERE SCC.CODE = 'GBC') " +
        "AND C.REGISTRATION_TYPE = 'CGC' AND C.CERTIFICATION_NO IS NOT NULL " +
        "AND((C.EXPIRY_DATE IS NOT NULL and  C.EXPIRY_DATE >= CURRENT_DATE) or " +
        "((C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(C.EXPIRY_DATE < CURRENT_DATE)) )) " +
        "AND((C.REMOVAL_DATE IS NULL) OR (C.REMOVAL_DATE > CURRENT_DATE)) AND C.WILLINGNESS_QP = 'Y' " +

"UNION ALL " +

        "SELECT 'MWC',count(DISTINCT C.CERTIFICATION_NO) FROM C_S_CATEGORY_CODE CAT, C_COMP_APPLICATION C, C_COMP_APPLICANT_INFO APP_INFO, C_APPLICANT APP,C_S_SYSTEM_VALUE S_ROLE, C_S_SYSTEM_VALUE S_STATUS " +
        "WHERE C.CATEGORY_ID = CAT.UUID AND C.UUID = APP_INFO.MASTER_ID AND APP_INFO.APPLICANT_ID = APP.UUID " +
        "AND APP_INFO.APPLICANT_ROLE_ID = S_ROLE.UUID " +
        "AND APP_INFO.APPLICANT_STATUS_ID = S_STATUS.UUID " +
        "AND C.REGISTRATION_TYPE = 'CMW' " +
        "AND S_ROLE.CODE LIKE 'A%' " +
        "AND S_STATUS.CODE = '1' " +
        "AND APP_INFO.accept_date IS NOT NULL " +
        "AND((APP_INFO.REMOVAL_DATE IS NULL) OR(APP_INFO.REMOVAL_DATE < CURRENT_DATE) )  " +
        "AND C.CERTIFICATION_NO IS NOT NULL AND( " +
        "(C.EXPIRY_DATE IS NOT NULL and  C.EXPIRY_DATE >= CURRENT_DATE) or " +
        "     ((C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and " +
        "       (C.EXPIRY_DATE < CURRENT_DATE)) ) )  AND((C.REMOVAL_DATE IS NULL) OR " +
        "(C.REMOVAL_DATE > CURRENT_DATE)) " +
        "and APP_INFO.uuid in ( " +
        "select cinfom.COMPANY_APPLICANTS_ID " +
        "from C_COMP_APPLICANT_INFO_MASTER cinfom " +
        "inner join C_COMP_APPLICANT_INFO_DETAIL cinfod on cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.uuid " +
        "inner join C_s_system_value mwcode on cinfod.ITEM_TYPE_ID = mwcode.uuid " +
        "where mwcode.code in('Type A')) " +
        "AND C.WILLINGNESS_QP = 'Y' " +

        "UNION ALL " +

        "SELECT  'MWC(W)',count(DISTINCT C.CERTIFICATION_NO) " +
        "FROM C_S_category_code cat, C_IND_CERTIFICATE C, C_APPLICANT APP, C_IND_APPLICATION A, C_S_SYSTEM_VALUE SV " +
        "WHERE  C.CATEGORY_ID = CAT.UUID " +
        "AND C.MASTER_ID = A.UUID " +
        "AND A.APPLICANT_ID = APP.UUID " +
        "AND A.REGISTRATION_TYPE = 'IMW' " +
        "AND SV.UUID = C.application_status_id " +
        "AND(SV.CODE = '1' or(C.RETENTION_APPLICATION_DATE IS NOT NULL))    " +
        "AND C.CERTIFICATION_NO IS NOT NULL " +
        "AND " +
        "((C.EXPIRY_DATE IS NOT NULL and C.EXPIRY_DATE >= CURRENT_DATE) or " +
        "(C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(C.EXPIRY_DATE < CURRENT_DATE)) " +
        ") AND " +
        "((C.REMOVAL_DATE IS NULL) OR(C.REMOVAL_DATE > CURRENT_DATE))   " +
         "AND A.UUID IN( " +
         "SELECT distinct iitem.MASTER_ID FROM C_ind_application_mw_item iitem, C_s_system_value sitem " +
         "WHERE iitem.ITEM_DETAILS_ID = sitem.UUID " +
         "AND sitem.code in ('Item 3.6'))   " +
         "AND A.WILLINGNESS_QP = 'Y' ) ";


            return sql;
        }

        //CRM0039
        public string getInterestedQpStatisticXlsSql()
        {
            string sql = "SELECT REG_TYPE, TOTAL,'Interested QP Statistic as at ' || CURRENT_DATE AS CURRENT_DATE FROM( " +
    "SELECT 'AP/RSE/RI' AS REG_TYPE, count(*) AS TOTAL " +
         "FROM " +
         "C_IND_CERTIFICATE C, C_S_CATEGORY_CODE CAT, C_S_SYSTEM_VALUE CATGRP,C_APPLICANT APP, C_IND_APPLICATION A ,C_S_SYSTEM_VALUE statusCode WHERE C.CATEGORY_ID = cat.UUID " +
         "AND C.MASTER_ID = A.UUID AND A.REGISTRATION_TYPE = 'IP' AND CATGRP.UUID = CAT.CATEGORY_GROUP_ID AND CATGRP.UUID in (SELECT  CATEGORY_GROUP_ID FROM C_S_CATEGORY_CODE SCC WHERE SCC.CODE IN('AP(A)', 'RI(A)', 'RSE')) " +
         "AND A.APPLICANT_ID = APP.UUID  " +
         "AND statusCode.UUID = C.application_status_id " +
         "AND(statusCode.CODE = '1' or(C.RETENTION_APPLICATION_DATE IS NOT NULL)) " +
         "AND C.CERTIFICATION_NO IS NOT NULL " +
         "AND( " +
         "(C.EXPIRY_DATE IS NOT NULL and C.EXPIRY_DATE >= CURRENT_DATE) or " +
         "(C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(C.EXPIRY_DATE < CURRENT_DATE)) " +
         ") " +
         "AND((C.REMOVAL_DATE IS NULL) OR(C.REMOVAL_DATE > CURRENT_DATE))  " +
         "AND a.WILLINGNESS_QP = 'Y' " +

"UNION ALL " +

        "SELECT 'GBC', count(DISTINCT C.CERTIFICATION_NO) FROM C_S_CATEGORY_CODE CAT, C_COMP_APPLICATION C " +
        "WHERE CAT.UUID = C.CATEGORY_ID AND C.CATEGORY_ID = (SELECT  UUID FROM C_S_CATEGORY_CODE SCC WHERE SCC.CODE = 'GBC') " +
        "AND C.REGISTRATION_TYPE = 'CGC' AND C.CERTIFICATION_NO IS NOT NULL " +
        "AND((C.EXPIRY_DATE IS NOT NULL and  C.EXPIRY_DATE >= CURRENT_DATE) or " +
        "((C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(C.EXPIRY_DATE < CURRENT_DATE)) )) " +
        "AND((C.REMOVAL_DATE IS NULL) OR (C.REMOVAL_DATE > CURRENT_DATE)) AND C.WILLINGNESS_QP = 'Y' " +

"UNION ALL " +

        "SELECT 'MWC',count(DISTINCT C.CERTIFICATION_NO) FROM C_S_CATEGORY_CODE CAT, C_COMP_APPLICATION C, C_COMP_APPLICANT_INFO APP_INFO, C_APPLICANT APP,C_S_SYSTEM_VALUE S_ROLE, C_S_SYSTEM_VALUE S_STATUS " +
        "WHERE C.CATEGORY_ID = CAT.UUID AND C.UUID = APP_INFO.MASTER_ID AND APP_INFO.APPLICANT_ID = APP.UUID " +
        "AND APP_INFO.APPLICANT_ROLE_ID = S_ROLE.UUID " +
        "AND APP_INFO.APPLICANT_STATUS_ID = S_STATUS.UUID " +
        "AND C.REGISTRATION_TYPE = 'CMW' " +
        "AND S_ROLE.CODE LIKE 'A%' " +
        "AND S_STATUS.CODE = '1' " +
        "AND APP_INFO.accept_date IS NOT NULL " +
        "AND((APP_INFO.REMOVAL_DATE IS NULL) OR(APP_INFO.REMOVAL_DATE < CURRENT_DATE) )  " +
        "AND C.CERTIFICATION_NO IS NOT NULL AND( " +
        "(C.EXPIRY_DATE IS NOT NULL and  C.EXPIRY_DATE >= CURRENT_DATE) or " +
        "     ((C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and " +
        "       (C.EXPIRY_DATE < CURRENT_DATE)) ) )  AND((C.REMOVAL_DATE IS NULL) OR " +
        "(C.REMOVAL_DATE > CURRENT_DATE)) " +
        "and APP_INFO.uuid in ( " +
        "select cinfom.COMPANY_APPLICANTS_ID " +
        "from C_COMP_APPLICANT_INFO_MASTER cinfom " +
        "inner join C_COMP_APPLICANT_INFO_DETAIL cinfod on cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.uuid " +
        "inner join C_s_system_value mwcode on cinfod.ITEM_TYPE_ID = mwcode.uuid " +
        "where mwcode.code in('Type A')) " +
        "AND C.WILLINGNESS_QP = 'Y' " +

        "UNION ALL " +

        "SELECT  'MWC(W)',count(DISTINCT C.CERTIFICATION_NO) " +
        "FROM C_S_category_code cat, C_IND_CERTIFICATE C, C_APPLICANT APP, C_IND_APPLICATION A, C_S_SYSTEM_VALUE SV " +
        "WHERE  C.CATEGORY_ID = CAT.UUID " +
        "AND C.MASTER_ID = A.UUID " +
        "AND A.APPLICANT_ID = APP.UUID " +
        "AND A.REGISTRATION_TYPE = 'IMW' " +
        "AND SV.UUID = C.application_status_id " +
        "AND(SV.CODE = '1' or(C.RETENTION_APPLICATION_DATE IS NOT NULL))    " +
        "AND C.CERTIFICATION_NO IS NOT NULL " +
        "AND " +
        "((C.EXPIRY_DATE IS NOT NULL and C.EXPIRY_DATE >= CURRENT_DATE) or " +
        "(C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(C.EXPIRY_DATE < CURRENT_DATE)) " +
        ") AND " +
        "((C.REMOVAL_DATE IS NULL) OR(C.REMOVAL_DATE > CURRENT_DATE))   " +
         "AND A.UUID IN( " +
         "SELECT distinct iitem.MASTER_ID FROM C_ind_application_mw_item iitem, C_s_system_value sitem " +
         "WHERE iitem.ITEM_DETAILS_ID = sitem.UUID " +
         "AND sitem.code in ('Item 3.6'))   " +
         "AND A.WILLINGNESS_QP = 'Y' ) ";


            return sql;
        }

        //CRM0040
        public string getMMD0003a1IPSql()
        {
            string sql = "SELECT RPT_INDIV_GAZETTE(:gaz_date, 'IP') as FILE_REF_STRING," +
                "CASE WHEN :acting = 1 then S_AUTH.ENGLISH_ACTION_NAME ELSE S_AUTH.ENGLISH_NAME END AS AUTH_ENAME," +
                "CASE WHEN :acting = 1 then S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_NAME END AS AUTH_CNAME," +
                "CASE WHEN :acting = 1 then S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_RANK END AS AUTH_ERANK," +
                "CASE WHEN :acting = 1 then S_AUTH.CHINESE_ACTION_RANK ELSE S_AUTH.CHINESE_RANK END AS AUTH_CRANK," +
                "S_AUTH.TELEPHONE_NO AS AUTH_TEL," +
                "S_AUTH.FAX_NO AS AUTH_FAX," +
                ":acting AS ACTING," +
                ":gaz_date AS GAZ_DATE " +
                "FROM C_S_AUTHORITY S_AUTH " +
                "WHERE S_AUTH.UUID='8a8247902429b5ad012429b5adec0003' ";

            return sql;
        }

        //CRM0041
        public string getMMD0003a4IPSql()
        {
            string sql = "SELECT RPT_INDIV_GAZETTE(:gaz_date, 'IP') as FILE_REF_STRING," +
                "CASE WHEN :acting = 1 then S_AUTH.ENGLISH_ACTION_NAME ELSE S_AUTH.ENGLISH_NAME END AS AUTH_ENAME," +
                "CASE WHEN :acting = 1 then S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_NAME END AS AUTH_CNAME," +
                "CASE WHEN :acting = 1 then S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_RANK END AS AUTH_ERANK," +
                "CASE WHEN :acting = 1 then S_AUTH.CHINESE_ACTION_RANK ELSE S_AUTH.CHINESE_RANK END AS AUTH_CRANK," +
                "S_AUTH.TELEPHONE_NO AS AUTH_TEL," +
                "S_AUTH.FAX_NO AS AUTH_FAX," +
                "case when :acting=1 then 'True' else 'False' end as ACTING," +
                ":gaz_date AS GAZ_DATE " +
                "FROM C_S_AUTHORITY S_AUTH " +
                "WHERE S_AUTH.UUID='8a8247902429b5ad012429b5adec0003' ";

            return sql;
        }

        //CRM0042
        public string getMMD0003b1CGCSql()
        {
            string sql = "SELECT RPT_INDIV_GAZETTE(:gaz_date, 'CGC') as FILE_REF_STRING," +
                "CASE WHEN :acting = 1 then S_AUTH.ENGLISH_ACTION_NAME ELSE S_AUTH.ENGLISH_NAME END AS AUTH_ENAME," +
                "CASE WHEN :acting = 1 then S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_NAME END AS AUTH_CNAME," +
                "CASE WHEN :acting = 1 then S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_RANK END AS AUTH_ERANK," +
                "CASE WHEN :acting = 1 then S_AUTH.CHINESE_ACTION_RANK ELSE S_AUTH.CHINESE_RANK END AS AUTH_CRANK," +
                "S_AUTH.TELEPHONE_NO AS AUTH_TEL," +
                "S_AUTH.FAX_NO AS AUTH_FAX," +
                "'GB' AS CAT_GP," +
                ":acting as ACTING," +
                ":gaz_date AS GAZ_DATE " +
                "FROM C_S_AUTHORITY S_AUTH " +
                "WHERE S_AUTH.UUID='8a8247902429b5ad012429b5adec0003' ";

            return sql;
        }

        //CRM0043
        public string getMMD0003b4CGCSql()
        {
            string sql = "SELECT RPT_INDIV_GAZETTE(:gaz_date, 'CGC') as FILE_REF_STRING," +
                "CASE WHEN :acting = 1 then S_AUTH.ENGLISH_ACTION_NAME ELSE S_AUTH.ENGLISH_NAME END AS AUTH_ENAME," +
                "CASE WHEN :acting = 1 then S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_NAME END AS AUTH_CNAME," +
                "CASE WHEN :acting = 1 then S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_RANK END AS AUTH_ERANK," +
                "CASE WHEN :acting = 1 then S_AUTH.CHINESE_ACTION_RANK ELSE S_AUTH.CHINESE_RANK END AS AUTH_CRANK," +
                "S_AUTH.TELEPHONE_NO AS AUTH_TEL," +
                "S_AUTH.FAX_NO AS AUTH_FAX," +
                "case when :acting=1 then 'True' else 'False' end as ACTING," +
                ":gaz_date AS GAZ_DATE " +
                "FROM C_S_AUTHORITY S_AUTH " +
                "WHERE S_AUTH.UUID='8a8247902429b5ad012429b5adec0003' ";


            return sql;
        }

        //CRM0044
        public string getMMD0010aIPSql()
        {
            string sql = "SELECT I_APPL.FILE_REFERENCE_NO AS FILE_REF,S_TITLE.CODE AS TITLE,APPLN.SURNAME AS SURNAME," +
                "APPLN.GIVEN_NAME_ON_ID AS GIVEN_NAME," +
     "APPLN.CHINESE_NAME AS CNAME," +
     "CASE WHEN APPLN.GENDER = 'M' THEN 'Sir'" +
     "WHEN APPLN.GENDER = 'F' THEN 'Madam' ELSE 'Sir/Madam' END SALUTATION," +
     "CASE WHEN I_APPL.POST_TO = 'H' THEN H_ADDR.ADDRESS_LINE1 ELSE  O_ADDR.ADDRESS_LINE1 END AS ADDR1," +
     "CASE WHEN I_APPL.POST_TO = 'H' THEN H_ADDR.ADDRESS_LINE2 ELSE  O_ADDR.ADDRESS_LINE2 END AS ADDR2," +
     "CASE WHEN I_APPL.POST_TO = 'H' THEN H_ADDR.ADDRESS_LINE3 ELSE  O_ADDR.ADDRESS_LINE3 END AS ADDR3," +
     "CASE WHEN I_APPL.POST_TO = 'H' THEN H_ADDR.ADDRESS_LINE4 ELSE  O_ADDR.ADDRESS_LINE4 END AS ADDR4," +
     "CASE WHEN I_APPL.POST_TO = 'H' THEN H_ADDR.ADDRESS_LINE5 ELSE  O_ADDR.ADDRESS_LINE5 END AS ADDR5," +
     "I_APPL.TELEPHONE_NO1 AS TEL_NO1," +
     "I_APPL.FAX_NO1 AS FAX_NO1," +
     "TO_CHAR(I_CERT.APPLICATION_DATE, 'YYYY-MM-DD') AS APP_DT," +
    "S_APP_FORM.CODE AS FORM_USED," +
     "CASE WHEN :acting = 'Y' THEN AUTH.ENGLISH_ACTION_NAME ELSE AUTH.ENGLISH_NAME END  AS AUTH_NAME," +
      " CASE WHEN :acting = 'Y' THEN AUTH.ENGLISH_ACTION_RANK ELSE AUTH.ENGLISH_RANK END  AS AUTH_RANK," +
     ":acting AS ACTING,     " +
    " AUTH.TELEPHONE_NO AS AUTH_TEL," +
     "AUTH.FAX_NO AS AUTH_FAX " +
"FROM " +
     "C_APPLICANT APPLN INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID " +
     "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
     "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
     "INNER JOIN C_S_SYSTEM_VALUE S_APP_FORM ON I_CERT.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
     "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
     "INNER JOIN C_S_AUTHORITY AUTH ON S_CAT_GP.UUID = AUTH.CATEGORY_GROUP_ID " +
     "LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON APPLN.TITLE_ID = S_TITLE.UUID " +
     "LEFT JOIN C_ADDRESS H_ADDR ON I_APPL.ENGLISH_HOME_ADDRESS_ID = H_ADDR.UUID " +
     "LEFT JOIN C_ADDRESS O_ADDR ON I_APPL.ENGLISH_OFFICE_ADDRESS_ID = O_ADDR.UUID " +
"WHERE " +
    "S_CAT.REGISTRATION_TYPE = 'IP' " +
     "AND S_APP_FORM.REGISTRATION_TYPE = 'IP' " +
    "AND S_APP_FORM.CODE = 'BA1' " +
    "AND I_CERT.APPLICATION_DATE = TO_DATE('23/04/2005', 'DD/MM/YYYY') " +
    "AND AUTH.UUID = '8a82479923c7240f0123c724ff840001' ";
            return sql;
        }

        //CRM0045
        public string getMMD0010bCGCSql()
        {
            string sql = "SELECT " +
     "C_APPL.FILE_REFERENCE_NO AS FILE_REF," +
     "C_APPL.ENGLISH_COMPANY_NAME AS ENAME," +
     "C_APPL.CHINESE_COMPANY_NAME AS CNAME," +
     "S_CAT.CODE AS CAT_CODE," +
     "S_CAT.ENGLISH_DESCRIPTION AS CAT_DESC," +
     "ADDR.ADDRESS_LINE1 AS ADDR1," +
     "ADDR.ADDRESS_LINE2 AS ADDR2," +
     "ADDR.ADDRESS_LINE3 AS ADDR3," +
     "ADDR.ADDRESS_LINE4 AS ADDR4," +
     "ADDR.ADDRESS_LINE5 AS ADDR5," +
     "C_APPL.TELEPHONE_NO1 AS TEL_NO1," +
     "C_APPL.FAX_NO1 AS FAX_NO1," +
     "C_APPL.APPLICATION_DATE AS APP_DT," +
     "S_APP_FORM.CODE AS FORM_USED," +
     "CASE WHEN :acting = 'Y' THEN AUTH.ENGLISH_ACTION_NAME ELSE AUTH.ENGLISH_NAME END  AS AUTH_NAME," +
     "CASE WHEN :acting = 'Y' THEN AUTH.ENGLISH_ACTION_RANK ELSE AUTH.ENGLISH_RANK END  AS AUTH_RANK," +
     "AUTH.TELEPHONE_NO AS AUTH_TEL, " +
     "AUTH.FAX_NO AS AUTH_FAX," +
     ":acting AS ACTING " +
"FROM " +
     "C_S_CATEGORY_CODE S_CAT " +
     "INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID " +
     "INNER JOIN C_S_SYSTEM_VALUE S_APP_FORM ON C_APPL.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
     "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
     "INNER JOIN C_S_AUTHORITY AUTH ON S_CAT_GP.UUID = AUTH.CATEGORY_GROUP_ID " +
     "LEFT JOIN C_ADDRESS ADDR ON C_APPL.ENGLISH_ADDRESS_ID = ADDR.UUID " +
"WHERE " +
     "S_CAT.REGISTRATION_TYPE = 'CGC' " +
     "AND S_APP_FORM.REGISTRATION_TYPE = 'CGC' " +
     "AND S_APP_FORM.CODE = 'BA2A' " +
     "AND C_APPL.APPLICATION_DATE = to_date('18/09/2008', 'DD/MM/YYYY') " +
     "AND AUTH.UUID = '8a82479923c78d7f0123c78e67470001'";
            return sql;
        }

        //CRM0046
        public string getMWCPExpiryDateSql()
        {

            string sql = "SELECT  BR_NO, FILE_REFERENCE_NO,  ENGLISH_COMPANY_NAME, CHINESE_COMPANY_NAME,CERTIFICATION_NO," +
               " to_char(RETENTION_APPLICATION_DATE,'dd/mm/yyyy') AS RETENTION_APPLICATION_DATE," +
               " to_char(APPLICATION_DATE,'dd/mm/yyyy') APPLICATION_DATE, " +
               "to_char(APPROVAL_DATE,'dd/mm/yyyy') APPROVAL_DATE , " +
               "to_char(GAZETTE_DATE,'dd/mm/yyyy') AS GAZETTE_DATE," +
               "to_char(EXPIRY_DATE,'dd/mm/yyyy') AS EXPIRY_DATE, " +
               "to_char(REMOVAL_DATE,'dd/mm/yyyy') AS REMOVAL_DATE," +
               "CODE, ENGLISH_DESCRIPTION, MWC_FILE_REFERENCE_NO, MWC_STATUS, CLASS1, CLASS2, CLASS3 " +
               "FROM( SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME," +
               "COMP.CERTIFICATION_NO, " +
               "COMP.RETENTION_APPLICATION_DATE," +
               "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE," +
               "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS," +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID,2) AS CLASS2," +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
               "FROM C_COMP_APPLICATION COMP " +
               "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
               "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
               "LEFT OUTER JOIN(" +
               "SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
               "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
               "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
               "WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND '1' = :report_type " +

               "UNION ALL " +
               "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME," +
               "COMP.CERTIFICATION_NO, " +
               "COMP.RETENTION_APPLICATION_DATE," +
               "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE," +
               "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS," +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2," +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
               "FROM C_COMP_APPLICATION COMP INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
               "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
               "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
               "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
               "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
               "WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP_MMC.FILE_REFERENCE_NO IS NOT NULL AND '2' = :report_type " +

               "UNION ALL " +

               "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME,COMP.CERTIFICATION_NO, " +
               "COMP.RETENTION_APPLICATION_DATE, " +
               "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
               "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS, " +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
               "FROM C_COMP_APPLICATION COMP " +
               "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
               "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
               "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
               "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
               "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
               "WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP_MMC.FILE_REFERENCE_NO IS NOT NULL " +
               "AND TO_DATE(COMP_MMC.FIRST_APPLICATION_DATE) > to_date(:inputFirstApplicationDate, 'dd/mm/yyyy') AND '3' = :report_type " +

               "UNION ALL " +

               "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME,COMP.CERTIFICATION_NO, " +
               "COMP.RETENTION_APPLICATION_DATE, " +
               "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
               "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS, " +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
               "FROM C_COMP_APPLICATION COMP " +
               "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
               "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
               "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
               "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
               "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID " +
               "AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' " +
               "AND COMP_MMC.FILE_REFERENCE_NO IS NOT NULL AND TO_DATE(COMP_MMC.FIRST_APPLICATION_DATE) <= to_date(:inputFirstApplicationDate, 'dd/mm/yyyy') AND '4' = :report_type " +

               "UNION ALL " +

               "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME, " +
               "COMP.CERTIFICATION_NO, " +
               "COMP.RETENTION_APPLICATION_DATE, " +
               "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE,COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
               "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS, " +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
               "FROM C_COMP_APPLICATION COMP " +
               "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
               "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
               "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
               "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
               "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID " +
               "AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
               "WHERE COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' " +
               "AND COMP.EXPIRY_DATE IS NOT NULL AND COMP.BR_NO IN(SELECT C.BR_NO FROM C_COMP_PROCESS_MONITOR R, C_COMP_APPLICATION C, (SELECT MAX(RECEIVED_DATE) AS RECEIVED_DATE, MASTER_ID  FROM C_COMP_PROCESS_MONITOR " +
               "WHERE RECEIVED_DATE IS NOT NULL GROUP BY MASTER_ID) A WHERE R.MASTER_ID = C.UUID AND R.MASTER_ID = A.MASTER_ID AND R.RECEIVED_DATE = A.RECEIVED_DATE AND " +
               "R.INTERVIEW_RESULT_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
               "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND ST.TYPE = 'INTERVIEW_RESULT' AND SV.CODE = 'R') AND R.RESULT_LETTER_DATE < to_date(:inputOutstandingDate, 'dd/mm/yyyy') " +
               "AND C.REGISTRATION_TYPE = 'CMW' AND C.CATEGORY_ID = (SELECT UUID FROM C_S_CATEGORY_CODE WHERE CODE = 'MWC') AND C.APPLICATION_FORM_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
               "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND SV.REGISTRATION_TYPE = 'CMW' AND SV.CODE = 'BA25' AND ST.TYPE = 'APPLICATION_FORM')) AND '5' = :report_type " +

               "UNION ALL " +

               "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO,  COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME, COMP.CERTIFICATION_NO, " +
               "COMP.RETENTION_APPLICATION_DATE, " +
               "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE , COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
               "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS,  " +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
               "FROM C_COMP_APPLICATION COMP " +
               "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
               "INNER JOIN C_S_SYSTEM_VALUE SV ON COMP.APPLICATION_STATUS_ID = SV.UUID " +
               "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
               "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
               "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
               "WHERE COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP.EXPIRY_DATE IS NOT NULL AND COMP.BR_NO IN( SELECT C.BR_NO FROM C_COMP_PROCESS_MONITOR R, C_COMP_APPLICATION C, (" +
               " SELECT MAX(RECEIVED_DATE) AS RECEIVED_DATE , MASTER_ID FROM C_COMP_PROCESS_MONITOR WHERE RECEIVED_DATE IS NOT NULL GROUP BY MASTER_ID) A WHERE R.MASTER_ID = C.UUID AND " +
               "R.MASTER_ID = A.MASTER_ID AND R.RECEIVED_DATE = A.RECEIVED_DATE AND R.INTERVIEW_RESULT_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
               "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND ST.TYPE = 'INTERVIEW_RESULT' AND SV.CODE = 'A') AND R.RESULT_LETTER_DATE < to_date(:inputResultDate, 'dd/mm/yyyy') AND C.REGISTRATION_TYPE = 'CMW' " +
               "AND C.CATEGORY_ID = (SELECT UUID FROM C_S_CATEGORY_CODE WHERE CODE = 'MWC') AND C.APPLICATION_FORM_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
               "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND SV.REGISTRATION_TYPE = 'CMW' AND SV.CODE = 'BA25' AND ST.TYPE = 'APPLICATION_FORM')) AND '6' = :report_type " +

               "UNION ALL " +

               "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO,  COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME,COMP.CERTIFICATION_NO, " +
               "COMP.RETENTION_APPLICATION_DATE," +
               "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE , COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE," +
               "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS," +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2," +
               "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 FROM C_COMP_APPLICATION COMP " +
               "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
               "INNER JOIN C_S_SYSTEM_VALUE SV ON COMP.APPLICATION_STATUS_ID = SV.UUID " +
               "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
               "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID " +
               "AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO WHERE COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP.EXPIRY_DATE IS NOT NULL AND COMP.BR_NO IN( " +
               "SELECT C.BR_NO FROM C_COMP_PROCESS_MONITOR R, C_COMP_APPLICATION C, (SELECT MAX(RECEIVED_DATE) AS RECEIVED_DATE , MASTER_ID " +
               "FROM C_COMP_PROCESS_MONITOR WHERE RECEIVED_DATE IS NOT NULL GROUP BY MASTER_ID) A WHERE R.MASTER_ID = C.UUID AND R.MASTER_ID = A.MASTER_ID AND R.RECEIVED_DATE = A.RECEIVED_DATE AND " +
               "R.INTERVIEW_RESULT_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND ST.TYPE = 'INTERVIEW_RESULT' AND SV.CODE = 'A') " +
               "AND R.RESULT_LETTER_DATE > to_date(:inputResultDate, 'dd/mm/yyyy') AND C.REGISTRATION_TYPE = 'CMW' AND C.CATEGORY_ID = (SELECT UUID FROM C_S_CATEGORY_CODE WHERE CODE = 'MWC') " +
               "AND C.APPLICATION_FORM_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND SV.REGISTRATION_TYPE = 'CMW' AND SV.CODE = 'BA25' AND ST.TYPE = 'APPLICATION_FORM')) " +
               "AND '7' = :report_type" +
               ") A ORDER BY FILE_REFERENCE_NO";

            //string sql = "SELECT  BR_NO, FILE_REFERENCE_NO,  ENGLISH_COMPANY_NAME, CHINESE_COMPANY_NAME,CERTIFICATION_NO," +
            //    " RETENTION_APPLICATION_DATE,APPLICATION_DATE, APPROVAL_DATE , GAZETTE_DATE,to_char(EXPIRY_DATE,'dd/mm/yyyy') AS EXPIRY_DATE, REMOVAL_DATE," +
            //    "CODE, ENGLISH_DESCRIPTION, MWC_FILE_REFERENCE_NO, MWC_STATUS, CLASS1, CLASS2, CLASS3 " +
            //    "FROM( SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME," +
            //    "COMP.CERTIFICATION_NO, " +
            //    "COMP.RETENTION_APPLICATION_DATE," +
            //    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE," +
            //    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS," +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID,2) AS CLASS2," +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
            //    "FROM C_COMP_APPLICATION COMP " +
            //    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
            //    "LEFT OUTER JOIN(" +
            //    "SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
            //    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
            //    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
            //    "WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND '1' = :report_type " +

            //    "UNION ALL " +
            //    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME," +
            //    "COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE," +
            //    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE," +
            //    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS," +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2," +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
            //    "FROM C_COMP_APPLICATION COMP INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
            //    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
            //    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
            //    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
            //    "WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP_MMC.FILE_REFERENCE_NO IS NOT NULL AND '2' = :report_type " +

            //    "UNION ALL " +

            //    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME,COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE, " +
            //    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
            //    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS, " +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
            //    "FROM C_COMP_APPLICATION COMP " +
            //    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
            //    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
            //    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
            //    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
            //    "WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP_MMC.FILE_REFERENCE_NO IS NOT NULL " +
            //    "AND TO_DATE(COMP_MMC.FIRST_APPLICATION_DATE) > to_date(:inputFirstApplicationDate, 'dd/mm/yyyy') AND '3' = :report_type " +

            //    "UNION ALL " +

            //    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME,COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE, " +
            //    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
            //    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS, " +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
            //    "FROM C_COMP_APPLICATION COMP " +
            //    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
            //    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
            //    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
            //    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID " +
            //    "AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' " +
            //    "AND COMP_MMC.FILE_REFERENCE_NO IS NOT NULL AND TO_DATE(COMP_MMC.FIRST_APPLICATION_DATE) <= to_date(:inputFirstApplicationDate, 'dd/mm/yyyy') AND '4' = :report_type " +

            //    "UNION ALL " +

            //    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME, " +
            //    "COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE, " +
            //    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE,COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
            //    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS, " +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
            //    "FROM C_COMP_APPLICATION COMP " +
            //    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
            //    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
            //    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
            //    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID " +
            //    "AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
            //    "WHERE COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' " +
            //    "AND COMP.EXPIRY_DATE IS NOT NULL AND COMP.BR_NO IN(SELECT C.BR_NO FROM C_COMP_PROCESS_MONITOR R, C_COMP_APPLICATION C, (SELECT MAX(RECEIVED_DATE) AS RECEIVED_DATE, MASTER_ID  FROM C_COMP_PROCESS_MONITOR " +
            //    "WHERE RECEIVED_DATE IS NOT NULL GROUP BY MASTER_ID) A WHERE R.MASTER_ID = C.UUID AND R.MASTER_ID = A.MASTER_ID AND R.RECEIVED_DATE = A.RECEIVED_DATE AND " +
            //    "R.INTERVIEW_RESULT_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
            //    "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND ST.TYPE = 'INTERVIEW_RESULT' AND SV.CODE = 'R') AND R.RESULT_LETTER_DATE < to_date(:inputOutstandingDate, 'dd/mm/yyyy') " +
            //    "AND C.REGISTRATION_TYPE = 'CMW' AND C.CATEGORY_ID = (SELECT UUID FROM C_S_CATEGORY_CODE WHERE CODE = 'MWC') AND C.APPLICATION_FORM_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
            //    "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND SV.REGISTRATION_TYPE = 'CMW' AND SV.CODE = 'BA25' AND ST.TYPE = 'APPLICATION_FORM')) AND '5' = :report_type " +

            //    "UNION ALL " +

            //    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO,  COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME, COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE, " +
            //    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE , COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
            //    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS,  " +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
            //    "FROM C_COMP_APPLICATION COMP " +
            //    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE SV ON COMP.APPLICATION_STATUS_ID = SV.UUID " +
            //    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
            //    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
            //    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
            //    "WHERE COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP.EXPIRY_DATE IS NOT NULL AND COMP.BR_NO IN( SELECT C.BR_NO FROM C_COMP_PROCESS_MONITOR R, C_COMP_APPLICATION C, (" +
            //    " SELECT MAX(RECEIVED_DATE) AS RECEIVED_DATE , MASTER_ID FROM C_COMP_PROCESS_MONITOR WHERE RECEIVED_DATE IS NOT NULL GROUP BY MASTER_ID) A WHERE R.MASTER_ID = C.UUID AND " +
            //    "R.MASTER_ID = A.MASTER_ID AND R.RECEIVED_DATE = A.RECEIVED_DATE AND R.INTERVIEW_RESULT_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
            //    "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND ST.TYPE = 'INTERVIEW_RESULT' AND SV.CODE = 'A') AND R.RESULT_LETTER_DATE < to_date(:inputResultDate, 'dd/mm/yyyy') AND C.REGISTRATION_TYPE = 'CMW' " +
            //    "AND C.CATEGORY_ID = (SELECT UUID FROM C_S_CATEGORY_CODE WHERE CODE = 'MWC') AND C.APPLICATION_FORM_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
            //    "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND SV.REGISTRATION_TYPE = 'CMW' AND SV.CODE = 'BA25' AND ST.TYPE = 'APPLICATION_FORM')) AND '6' = :report_type " +

            //    "UNION ALL " +

            //    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO,  COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME,COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE," +
            //    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE , COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE," +
            //    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS," +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2," +
            //    "C_GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 FROM C_COMP_APPLICATION COMP " +
            //    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE SV ON COMP.APPLICATION_STATUS_ID = SV.UUID " +
            //    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
            //    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID " +
            //    "AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO WHERE COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP.EXPIRY_DATE IS NOT NULL AND COMP.BR_NO IN( " +
            //    "SELECT C.BR_NO FROM C_COMP_PROCESS_MONITOR R, C_COMP_APPLICATION C, (SELECT MAX(RECEIVED_DATE) AS RECEIVED_DATE , MASTER_ID " +
            //    "FROM C_COMP_PROCESS_MONITOR WHERE RECEIVED_DATE IS NOT NULL GROUP BY MASTER_ID) A WHERE R.MASTER_ID = C.UUID AND R.MASTER_ID = A.MASTER_ID AND R.RECEIVED_DATE = A.RECEIVED_DATE AND " +
            //    "R.INTERVIEW_RESULT_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND ST.TYPE = 'INTERVIEW_RESULT' AND SV.CODE = 'A') " +
            //    "AND R.RESULT_LETTER_DATE > to_date(:inputResultDate, 'dd/mm/yyyy') AND C.REGISTRATION_TYPE = 'CMW' AND C.CATEGORY_ID = (SELECT UUID FROM C_S_CATEGORY_CODE WHERE CODE = 'MWC') " +
            //    "AND C.APPLICATION_FORM_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND SV.REGISTRATION_TYPE = 'CMW' AND SV.CODE = 'BA25' AND ST.TYPE = 'APPLICATION_FORM')) " +
            //    "AND '7' = :report_type" +
            //    ") A ORDER BY FILE_REFERENCE_NO";

            return sql;
        }

        //CRM0047
        public string getMWCPExpiryDateXlsSql()
        {
            string sql = "SELECT  BR_NO, FILE_REFERENCE_NO,  ENGLISH_COMPANY_NAME, CHINESE_COMPANY_NAME,CERTIFICATION_NO," +
    " RETENTION_APPLICATION_DATE,APPLICATION_DATE, APPROVAL_DATE , GAZETTE_DATE, EXPIRY_DATE, REMOVAL_DATE,CODE, ENGLISH_DESCRIPTION, MWC_FILE_REFERENCE_NO, MWC_STATUS, CLASS1, CLASS2, CLASS3 " +
    "FROM( SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME," +
    "COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE," +
    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE," +
    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS," +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2," +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
    "FROM C_COMP_APPLICATION COMP " +
    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
    "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
    "LEFT OUTER JOIN(" +
    "SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
    "WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND '1' = :report_type " +

    "UNION ALL " +
    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME," +
    "COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE," +
    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE," +
    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS," +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2," +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
    "FROM C_COMP_APPLICATION COMP INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
    "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
    "WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP_MMC.FILE_REFERENCE_NO IS NOT NULL AND '2' = '1' " +

    "UNION ALL " +

    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME,COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE, " +
    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS, " +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
    "FROM C_COMP_APPLICATION COMP " +
    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
    "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
    "WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP_MMC.FILE_REFERENCE_NO IS NOT NULL " +
    "AND TO_DATE(COMP_MMC.FIRST_APPLICATION_DATE) > to_date('01012012', 'dd/mm/yyyy') AND '3' = :report_type " +

    "UNION ALL " +

    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME,COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE, " +
    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS, " +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
    "FROM C_COMP_APPLICATION COMP " +
    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
    "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID " +
    "AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO WHERE COMP.EXPIRY_DATE IS NOT NULL AND COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' " +
    "AND COMP_MMC.FILE_REFERENCE_NO IS NOT NULL AND TO_DATE(COMP_MMC.FIRST_APPLICATION_DATE) <= to_date('01012012', 'dd/mm/yyyy') AND '4' = :report_type " +
    "UNION ALL " +
    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO, COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME, " +
    "COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE, " +
    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE, COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS, " +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
    "FROM C_COMP_APPLICATION COMP " +
    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
    "INNER JOIN C_S_SYSTEM_VALUE SV ON  COMP.APPLICATION_STATUS_ID = SV.UUID " +
    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID " +
    "AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
    "WHERE COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' " +
    "AND COMP.EXPIRY_DATE IS NOT NULL AND COMP.BR_NO IN(SELECT C.BR_NO FROM C_COMP_PROCESS_MONITOR R, C_COMP_APPLICATION C, (SELECT MAX(RECEIVED_DATE) AS RECEIVED_DATE, MASTER_ID  FROM C_COMP_PROCESS_MONITOR " +
    "WHERE RECEIVED_DATE IS NOT NULL GROUP BY MASTER_ID) A WHERE R.MASTER_ID = C.UUID AND R.MASTER_ID = A.MASTER_ID AND R.RECEIVED_DATE = A.RECEIVED_DATE AND " +
    "R.INTERVIEW_RESULT_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
    "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND ST.TYPE = 'INTERVIEW_RESULT' AND SV.CODE = 'R') AND R.RESULT_LETTER_DATE < to_date('01012012', 'dd/mm/yyyy') " +
    "AND C.REGISTRATION_TYPE = 'CMW' AND C.CATEGORY_ID = (SELECT UUID FROM C_S_CATEGORY_CODE WHERE CODE = 'MWC') AND C.APPLICATION_FORM_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
    "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND SV.REGISTRATION_TYPE = 'CMW' AND SV.CODE = 'BA25' AND ST.TYPE = 'APPLICATION_FORM')) AND '5' = :report_type " +

    "UNION ALL " +

    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO,  COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME, COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE, " +
    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE , COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE, " +
    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS,  " +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2, " +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 " +
    "FROM C_COMP_APPLICATION COMP " +
    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
    "INNER JOIN C_S_SYSTEM_VALUE SV ON COMP.APPLICATION_STATUS_ID = SV.UUID " +
    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV " +
    "WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO " +
    "WHERE COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP.EXPIRY_DATE IS NOT NULL AND COMP.BR_NO IN( SELECT C.BR_NO FROM C_COMP_PROCESS_MONITOR R, C_COMP_APPLICATION C, (" +
    " SELECT MAX(RECEIVED_DATE) AS RECEIVED_DATE , MASTER_ID FROM C_COMP_PROCESS_MONITOR WHERE RECEIVED_DATE IS NOT NULL GROUP BY MASTER_ID) A WHERE R.MASTER_ID = C.UUID AND " +
    "R.MASTER_ID = A.MASTER_ID AND R.RECEIVED_DATE = A.RECEIVED_DATE AND R.INTERVIEW_RESULT_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
    "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND ST.TYPE = 'INTERVIEW_RESULT' AND SV.CODE = 'A') AND R.RESULT_LETTER_DATE < to_date('01012012', 'dd/mm/yyyy') AND C.REGISTRATION_TYPE = 'CMW' " +
    "AND C.CATEGORY_ID = (SELECT UUID FROM C_S_CATEGORY_CODE WHERE CODE = 'MWC') AND C.APPLICATION_FORM_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST " +
    "WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND SV.REGISTRATION_TYPE = 'CMW' AND SV.CODE = 'BA25' AND ST.TYPE = 'APPLICATION_FORM')) AND '6' = :report_type " +

    "UNION ALL " +
    "SELECT COMP.BR_NO, COMP.FILE_REFERENCE_NO,  COMP.ENGLISH_COMPANY_NAME, COMP.CHINESE_COMPANY_NAME,COMP.CERTIFICATION_NO, COMP.RETENTION_APPLICATION_DATE," +
    "COMP.APPLICATION_DATE, COMP.APPROVAL_DATE , COMP.GAZETTE_DATE, COMP.EXPIRY_DATE, COMP.REMOVAL_DATE," +
    "SV.CODE, SV.ENGLISH_DESCRIPTION, COMP_MMC.FILE_REFERENCE_NO AS MWC_FILE_REFERENCE_NO, COMP_MMC.ENGLISH_DESCRIPTION AS MWC_STATUS," +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 1) AS CLASS1, GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 2) AS CLASS2," +
    "GET_MW_COMP_APPLY_CLASS_TYPE(COMP_MMC.UUID, 3) AS CLASS3 FROM C_COMP_APPLICATION COMP " +
    "INNER JOIN C_S_CATEGORY_CODE CAT ON COMP.CATEGORY_ID = CAT.UUID " +
    "INNER JOIN C_S_SYSTEM_VALUE SV ON COMP.APPLICATION_STATUS_ID = SV.UUID " +
    "LEFT OUTER JOIN(SELECT COMP_MWC.UUID, COMP_MWC.BR_NO, COMP_MWC.FILE_REFERENCE_NO, COMP_MWC_SV.ENGLISH_DESCRIPTION, COMP_MWC.FIRST_APPLICATION_DATE " +
    "FROM C_COMP_APPLICATION COMP_MWC, C_S_CATEGORY_CODE COMP_MWC_CAT, C_S_SYSTEM_VALUE COMP_MWC_SV WHERE COMP_MWC.CATEGORY_ID = COMP_MWC_CAT.UUID  AND  COMP_MWC.APPLICATION_STATUS_ID = COMP_MWC_SV.UUID " +
    "AND COMP_MWC_CAT.CODE = 'MWC') COMP_MMC ON COMP.BR_NO = COMP_MMC.BR_NO WHERE COMP.REGISTRATION_TYPE = 'CMW' AND CAT.CODE = 'MWC(P)' AND COMP.EXPIRY_DATE IS NOT NULL AND COMP.BR_NO IN( " +
    "SELECT C.BR_NO FROM C_COMP_PROCESS_MONITOR R, C_COMP_APPLICATION C, (SELECT MAX(RECEIVED_DATE) AS RECEIVED_DATE , MASTER_ID " +
    "FROM C_COMP_PROCESS_MONITOR WHERE RECEIVED_DATE IS NOT NULL GROUP BY MASTER_ID) A WHERE R.MASTER_ID = C.UUID AND R.MASTER_ID = A.MASTER_ID AND R.RECEIVED_DATE = A.RECEIVED_DATE AND " +
    "R.INTERVIEW_RESULT_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND ST.TYPE = 'INTERVIEW_RESULT' AND SV.CODE = 'A') " +
    "AND R.RESULT_LETTER_DATE > to_date('01012012', 'dd/mm/yyyy') AND C.REGISTRATION_TYPE = 'CMW' AND C.CATEGORY_ID = (SELECT UUID FROM C_S_CATEGORY_CODE WHERE CODE = 'MWC') " +
    "AND C.APPLICATION_FORM_ID = (SELECT SV.UUID FROM C_S_SYSTEM_VALUE SV, C_S_SYSTEM_TYPE ST WHERE SV.SYSTEM_TYPE_ID = ST.UUID AND SV.REGISTRATION_TYPE = 'CMW' AND SV.CODE = 'BA25' AND ST.TYPE = 'APPLICATION_FORM')) " +
    "AND '7' = :report_type) A ORDER BY FILE_REFERENCE_NO";

            return sql;
        }

        //CRM0048
        public string getNoOfConvictionCGCSql(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT C_CVT.BR_NO BR_NO,c_propercase(C_CVT.ENGLISH_NAME) AS ENAME,to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY') AS OFFENCE_Y," +
                "CASE WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '01' THEN '01-Jan' " +
                "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '02' THEN '02-Feb' " +
                "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '03' THEN '03-Mar' " +
                "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '04' THEN '04-April' " +
                "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '05' THEN '05-May' " +
                "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '06' THEN '06-June' " +
                "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '07' THEN '07-July' " +
                "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '08' THEN '08-Aug' " +
                "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '09' THEN '09-Sept' " +
                "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '10' THEN '10-Oct' " +
                "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '11' THEN '11-Nov' " +
                "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '12' THEN '12-Dec' END OFFENCE_M," +
                "C_OFFENCEDT (to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as OFFENCEDT_5, " +
                "C_CONVICTNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as CONVICTNUM_5," +
                "C_BNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as BNUM_5, " +
                "C_NBNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as NBNUM_5, " +
                "C_OFFENCEDT(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as OFFENCEDT_6, " +
                "C_CONVICTNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as CONVICTNUM_6, " +
                "C_BNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as BNUM_6, " +
                "C_NBNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as NBNUM_6," +
                "to_char(to_date(:today,'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_AT_TODAY " +
                "FROM C_COMP_CONVICTION C_CVT " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CVT ON S_CVT.UUID = C_CVT.CONVICTION_SOURCE_ID " +
                "and S_CVT.CODE in ('5', '6') and C_CVT.CR_OFFENCE_DATE is not null " +
                "AND C_CVT.REGISTRATION_TYPE = 'CGC'  AND C_CVT.ENGLISH_NAME = 'HEAD FAME COMPANY LIMITED' " +
                "LEFT JOIN " +
                "(SELECT S_NB.CODE FROM C_S_SYSTEM_VALUE S_NB " +
                "INNER JOIN C_S_SYSTEM_TYPE S_NB_TYPE ON S_NB.SYSTEM_TYPE_ID = S_NB_TYPE.UUID WHERE S_NB_TYPE.TYPE = 'NB_CODE') B ON C_CVT.CR_SECTION = B.CODE " +
                "Where 1=1 ");
            foreach (var item in myDictionary)
            {
                switch (item.Key)
                {
                    case "InfoSheetODateFrom":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And C_CVT.Cr_Offence_Date >= to_date(:InfoSheetODateFrom,'dd/mm/yyyy') ");
                        break;

                    case "InfoSheetODateTo":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And C_CVT.Cr_Offence_Date <= to_date(:InfoSheetODateTo,'dd/mm/yyyy')  ");
                        break;
                    case "InfoSheetJDateFrom":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And C_CVT.Cr_Judge_Date >= to_date(:InfoSheetJDateFrom,'dd/mm/yyyy') ");
                        break;
                    case "InfoSheetJDateTo":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And C_CVT.Cr_Judge_Date <= to_date(:InfoSheetJDateTo,'dd/mm/yyyy') ");
                        break;
                    case "InfoSheetCompanyName":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                            sql.Append(" And C_CVT.English_Name like '%" + item.Value.ToString() + "%' ");
                        break;
                }
            }
            sql.Append(" GROUP BY C_CVT.BR_NO,C_CVT.ENGLISH_NAME,to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'),to_char(C_CVT.CR_OFFENCE_DATE, 'MM') ORDER BY BR_NO, ENAME, OFFENCE_Y, OFFENCE_M ");


            //string sql = "SELECT C_CVT.BR_NO BR_NO,c_propercase(C_CVT.ENGLISH_NAME) AS ENAME,to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY') AS OFFENCE_Y," +
            //    "CASE WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '01' THEN '01-Jan' " +
            //    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '02' THEN '02-Feb' " +
            //    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '03' THEN '03-Mar' " +
            //    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '04' THEN '04-April' " +
            //    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '05' THEN '05-May' " +
            //    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '06' THEN '06-June' " +
            //    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '07' THEN '07-July' " +
            //    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '08' THEN '08-Aug' " +
            //    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '09' THEN '09-Sept' " +
            //    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '10' THEN '10-Oct' " +
            //    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '11' THEN '11-Nov' " +
            //    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '12' THEN '12-Dec' END OFFENCE_M," +
            //    "C_OFFENCEDT (to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as OFFENCEDT_5, " +
            //    "C_CONVICTNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as CONVICTNUM_5," +
            //    "C_BNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as BNUM_5, " +
            //    "C_NBNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as NBNUM_5, " +
            //    "C_OFFENCEDT(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as OFFENCEDT_6, " +
            //    "C_CONVICTNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as CONVICTNUM_6, " +
            //    "C_BNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as BNUM_6, " +
            //    "C_NBNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as NBNUM_6," +
            //    "to_char(to_date(:today,'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_AT_TODAY " +
            //    "FROM C_COMP_CONVICTION C_CVT " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_CVT ON S_CVT.UUID = C_CVT.CONVICTION_SOURCE_ID " +
            //    "and S_CVT.CODE in ('5', '6') and C_CVT.CR_OFFENCE_DATE is not null " +
            //    "AND C_CVT.REGISTRATION_TYPE = 'CGC' AND C_CVT.ENGLISH_NAME = 'HEAD FAME COMPANY LIMITED' " +
            //    "LEFT JOIN " +
            //    "(SELECT S_NB.CODE FROM C_S_SYSTEM_VALUE S_NB " +
            //    "INNER JOIN C_S_SYSTEM_TYPE S_NB_TYPE ON S_NB.SYSTEM_TYPE_ID = S_NB_TYPE.UUID WHERE S_NB_TYPE.TYPE = 'NB_CODE') B ON C_CVT.CR_SECTION = B.CODE " +
            //    "GROUP BY C_CVT.BR_NO,C_CVT.ENGLISH_NAME,to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'),to_char(C_CVT.CR_OFFENCE_DATE, 'MM') " +
            //    "ORDER BY BR_NO, ENAME, OFFENCE_Y, OFFENCE_M ";
            return sql.ToString();
        }

        //CRM0049
        public string getNoOfConvictionCGCXlsSql()
        {
            string sql = "SELECT C_CVT.BR_NO BR_NO,c_propercase(C_CVT.ENGLISH_NAME) AS ENAME,to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY') AS OFFENCE_Y," +
    "CASE WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '01' THEN '01-Jan' " +
    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '02' THEN '02-Feb' " +
    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '03' THEN '03-Mar' " +
    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '04' THEN '04-April' " +
    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '05' THEN '05-May' " +
    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '06' THEN '06-June' " +
    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '07' THEN '07-July' " +
    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '08' THEN '08-Aug' " +
    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '09' THEN '09-Sept' " +
    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '10' THEN '10-Oct' " +
    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '11' THEN '11-Nov' " +
    "WHEN to_char(C_CVT.CR_OFFENCE_DATE,'MM') = '12' THEN '12-Dec' END OFFENCE_M," +
    "OFFENCEDT (to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as OFFENCEDT_5, " +
    "CONVICTNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as CONVICTNUM_5," +
    "BNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as BNUM_5, " +
    "NBNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '5', 'CGC') as NBNUM_5, " +
    "OFFENCEDT(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as OFFENCEDT_6, " +
    "CONVICTNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as CONVICTNUM_6, " +
    "BNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as BNUM_6, " +
    "NBNUM(to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'), to_char(C_CVT.CR_OFFENCE_DATE, 'MM'), '6', 'CGC') as NBNUM_6," +
    ":today AS AS_AT_TODAY " +
    "FROM C_COMP_CONVICTION C_CVT " +
    "INNER JOIN C_S_SYSTEM_VALUE S_CVT ON S_CVT.UUID = C_CVT.CONVICTION_SOURCE_ID " +
    "and S_CVT.CODE in ('5', '6') and C_CVT.CR_OFFENCE_DATE is not null " +
    "AND C_CVT.REGISTRATION_TYPE = 'CGC' AND C_CVT.ENGLISH_NAME = 'HEAD FAME COMPANY LIMITED' " +
    "LEFT JOIN " +
    "(SELECT S_NB.CODE FROM C_S_SYSTEM_VALUE S_NB " +
    "INNER JOIN C_S_SYSTEM_TYPE S_NB_TYPE ON S_NB.SYSTEM_TYPE_ID = S_NB_TYPE.UUID WHERE S_NB_TYPE.TYPE = 'NB_CODE') B ON C_CVT.CR_SECTION = B.CODE " +
    "GROUP BY C_CVT.BR_NO,C_CVT.ENGLISH_NAME,to_char(C_CVT.CR_OFFENCE_DATE, 'YYYY'),to_char(C_CVT.CR_OFFENCE_DATE, 'MM') " +
    "ORDER BY BR_NO, ENAME, OFFENCE_Y, OFFENCE_M ";
            return sql;
        }

        //CRM0050
        public string getNoOfRegCGCSql(Dictionary<string, object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            //sql.Append(
            //    @"SELECT slist.code, slist.status, app_as.AS_count, app_td.TD_count, app_oo.OO_count, app_comp.COMP_count, app_comp2.COMP2_count,:TypeOfCategorys AS CATEGORY FROM(
            //    (SELECT t1.code, t2.status FROM(
            //    (SELECT scat.code, scat.english_description AS cat_desc FROM C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp WHERE scatgrp.UUID = scat.CATEGORY_GROUP_ID AND scatgrp.REGISTRATION_TYPE = 'CGC') t1 
            //    LEFT JOIN(SELECT svalue.english_description AS status FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue WHERE stype.UUID = svalue.SYSTEM_TYPE_ID AND stype.TYPE = 'APPLICANT_STATUS') t2 ON 1 = 1)) slist 
            //    LEFT JOIN ( SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS COMP_count 
            //    FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE status, C_S_SYSTEM_VALUE scatgrp, C_S_SYSTEM_VALUE comp_type WHERE capp.CATEGORY_ID = scat.UUID 
            //    AND capp.APPLICATION_STATUS_ID = status.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID 
            //    AND comp_type.UUID = capp.COMPANY_TYPE_ID AND comp_type.CODE = '1' GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION ) app_comp ON slist.status = app_comp.status AND slist.code = app_comp.code 
            //    LEFT JOIN (SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS COMP2_count FROM C_comp_application capp, 
            //    C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE status, C_S_SYSTEM_VALUE scatgrp, C_S_SYSTEM_VALUE comp_type WHERE capp.CATEGORY_ID = scat.UUID 
            //    AND capp.APPLICATION_STATUS_ID = status.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND comp_type.UUID = capp.COMPANY_TYPE_ID AND comp_type.CODE IN ('2', '3') 
            //    GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_comp2 ON slist.status = app_comp2.status AND slist.code = app_comp2.code 
            //    LEFT JOIN(SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS AS_count FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, 
            //    C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE status WHERE capp.CATEGORY_ID = scat.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND cinfo.MASTER_ID = capp.UUID 
            //    AND cinfo.APPLICANT_ROLE_ID = srole.UUID AND cinfo.APPLICANT_STATUS_ID = status.UUID 
            //    AND srole.CODE = 'AS' GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_as ON slist.status = app_as.status AND slist.code = app_as.code 
            //    LEFT JOIN(SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS TD_count 
            //    FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, 
            //    C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE status WHERE capp.CATEGORY_ID = scat.UUID 
            //    AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND cinfo.MASTER_ID = capp.UUID AND cinfo.APPLICANT_ROLE_ID = srole.UUID 
            //    AND cinfo.APPLICANT_STATUS_ID = status.UUID AND srole.CODE = 'TD' 
            //    GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_td ON slist.status = app_td.status AND slist.code = app_td.code 
            //    LEFT JOIN (SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS OO_count 
            //    FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE status 
            //    WHERE capp.CATEGORY_ID = scat.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND cinfo.MASTER_ID = capp.UUID 
            //    AND cinfo.APPLICANT_ROLE_ID = srole.UUID AND cinfo.APPLICANT_STATUS_ID = status.UUID AND srole.CODE = 'OO' 
            //    GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_oo ON slist.status = app_oo.status AND slist.code = app_oo.code) ORDER BY slist.code, slist.status");

            sql.Append(@"SELECT res.*,
                               :TypeOfCategorys AS CATEGORY
                        FROM   (SELECT h.status,
                                       Sum(h.AS_count)   AS AS_count,
                                       Sum(h.td_count)   AS TD_count,
                                       Sum(h.oo_count)   AS OO_count,
                                       Sum(h.comp_count) AS COMP_count,
                                       Sum(h.comp2_count)AS COMP2_count
                                FROM   (SELECT slist.code,
                                               slist.status,
                                               app_as.AS_count,
                                               app_td.TD_count,
                                               app_oo.OO_count,
                                               app_comp.COMP_count,
                                               app_comp2.COMP2_count,
                                               :TypeOfCategorys AS CATEGORY
                                        FROM   ( (SELECT t1.code,
                                                       t2.status
                                                FROM   ( (SELECT scat.code,
                                                               scat.english_description AS cat_desc
                                                        FROM   C_S_CATEGORY_CODE scat,
                                                               C_S_SYSTEM_VALUE scatgrp
                                                        WHERE  scatgrp.UUID = scat.CATEGORY_GROUP_ID
                                                               AND scatgrp.REGISTRATION_TYPE = 'CGC') t1
                                                         LEFT JOIN(SELECT svalue.english_description AS status
                                                                   FROM   C_S_SYSTEM_TYPE stype,
                                                                          C_S_SYSTEM_VALUE svalue
                                                                   WHERE  stype.UUID = svalue.SYSTEM_TYPE_ID
                                                                          AND stype.TYPE = 'APPLICANT_STATUS') t2
                                                                ON 1 = 1)) slist
                                                 LEFT JOIN (SELECT scat.CODE,
                                                                   status.ENGLISH_DESCRIPTION AS status,
                                                                   Count(*)                   AS COMP_count
                                                            FROM   C_comp_application capp,
                                                                   C_S_CATEGORY_CODE scat,
                                                                   C_S_SYSTEM_VALUE status,
                                                                   C_S_SYSTEM_VALUE scatgrp,
                                                                   C_S_SYSTEM_VALUE comp_type
                                                            WHERE  capp.CATEGORY_ID = scat.UUID
                                                                   AND capp.APPLICATION_STATUS_ID = status.UUID
                                                                   AND scatgrp.UUID = scat.CATEGORY_GROUP_ID
                                                                   AND comp_type.UUID = capp.COMPANY_TYPE_ID
                                                                   AND comp_type.CODE = '1'
                                                            GROUP  BY scat.CODE,
                                                                      status.ENGLISH_DESCRIPTION) app_comp
                                                        ON slist.status = app_comp.status
                                                           AND slist.code = app_comp.code
                                                 LEFT JOIN (SELECT scat.CODE,
                                                                   status.ENGLISH_DESCRIPTION AS status,
                                                                   Count(*)                   AS COMP2_count
                                                            FROM   C_comp_application capp,
                                                                   C_S_CATEGORY_CODE scat,
                                                                   C_S_SYSTEM_VALUE status,
                                                                   C_S_SYSTEM_VALUE scatgrp,
                                                                   C_S_SYSTEM_VALUE comp_type
                                                            WHERE  capp.CATEGORY_ID = scat.UUID
                                                                   AND capp.APPLICATION_STATUS_ID = status.UUID
                                                                   AND scatgrp.UUID = scat.CATEGORY_GROUP_ID
                                                                   AND comp_type.UUID = capp.COMPANY_TYPE_ID
                                                                   AND comp_type.CODE IN ( '2', '3' )
                                                            GROUP  BY scat.CODE,
                                                                      status.ENGLISH_DESCRIPTION) app_comp2
                                                        ON slist.status = app_comp2.status
                                                           AND slist.code = app_comp2.code
                                                 LEFT JOIN(SELECT scat.CODE,
                                                                  status.ENGLISH_DESCRIPTION AS status,
                                                                  Count(*)                   AS AS_count
                                                           FROM   C_comp_application capp,
                                                                  C_S_CATEGORY_CODE scat,
                                                                  C_S_SYSTEM_VALUE scatgrp,
                                                                  C_COMP_APPLICANT_INFO cinfo,
                                                                  C_S_SYSTEM_VALUE srole,
                                                                  C_S_SYSTEM_VALUE status
                                                           WHERE  capp.CATEGORY_ID = scat.UUID
                                                                  AND scatgrp.UUID = scat.CATEGORY_GROUP_ID
                                                                  AND cinfo.MASTER_ID = capp.UUID
                                                                  AND cinfo.APPLICANT_ROLE_ID = srole.UUID
                                                                  AND cinfo.APPLICANT_STATUS_ID = status.UUID
                                                                  AND srole.CODE = 'AS'
                                                           GROUP  BY scat.CODE,
                                                                     status.ENGLISH_DESCRIPTION) app_as
                                                        ON slist.status = app_as.status
                                                           AND slist.code = app_as.code
                                                 LEFT JOIN(SELECT scat.CODE,
                                                                  status.ENGLISH_DESCRIPTION AS status,
                                                                  Count(*)                   AS TD_count
                                                           FROM   C_comp_application capp,
                                                                  C_S_CATEGORY_CODE scat,
                                                                  C_S_SYSTEM_VALUE scatgrp,
                                                                  C_COMP_APPLICANT_INFO cinfo,
                                                                  C_S_SYSTEM_VALUE srole,
                                                                  C_S_SYSTEM_VALUE status
                                                           WHERE  capp.CATEGORY_ID = scat.UUID
                                                                  AND scatgrp.UUID = scat.CATEGORY_GROUP_ID
                                                                  AND cinfo.MASTER_ID = capp.UUID
                                                                  AND cinfo.APPLICANT_ROLE_ID = srole.UUID
                                                                  AND cinfo.APPLICANT_STATUS_ID = status.UUID
                                                                  AND srole.CODE = 'TD'
                                                           GROUP  BY scat.CODE,
                                                                     status.ENGLISH_DESCRIPTION) app_td
                                                        ON slist.status = app_td.status
                                                           AND slist.code = app_td.code
                                                 LEFT JOIN (SELECT scat.CODE,
                                                                   status.ENGLISH_DESCRIPTION AS status,
                                                                   Count(*)                   AS OO_count
                                                            FROM   C_comp_application capp,
                                                                   C_S_CATEGORY_CODE scat,
                                                                   C_S_SYSTEM_VALUE scatgrp,
                                                                   C_COMP_APPLICANT_INFO cinfo,
                                                                   C_S_SYSTEM_VALUE srole,
                                                                   C_S_SYSTEM_VALUE status
                                                            WHERE  capp.CATEGORY_ID = scat.UUID
                                                                   AND scatgrp.UUID = scat.CATEGORY_GROUP_ID
                                                                   AND cinfo.MASTER_ID = capp.UUID
                                                                   AND cinfo.APPLICANT_ROLE_ID = srole.UUID
                                                                   AND cinfo.APPLICANT_STATUS_ID = status.UUID
                                                                   AND srole.CODE = 'OO'
                                                            GROUP  BY scat.CODE,
                                                                      status.ENGLISH_DESCRIPTION) app_oo
                                                        ON slist.status = app_oo.status
                                                           AND slist.code = app_oo.code)
                                        ORDER  BY slist.code,
                                                  slist.status) h
                                GROUP  BY h.status) res 
                        ");

            //string sql = "SELECT slist.code, slist.status, app_as.AS_count, app_td.TD_count, app_oo.OO_count, app_comp.COMP_count, app_comp2.COMP2_count,:category AS CATEGORY FROM(" +
            //    "(SELECT t1.code, t2.status FROM(" +
            //    "(SELECT scat.code, scat.english_description AS cat_desc FROM C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp WHERE scatgrp.UUID = scat.CATEGORY_GROUP_ID AND scatgrp.REGISTRATION_TYPE = 'CGC') t1 " +
            //    "LEFT JOIN(SELECT svalue.english_description AS status FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue WHERE stype.UUID = svalue.SYSTEM_TYPE_ID AND stype.TYPE = 'APPLICANT_STATUS') t2 ON 1 = 1)) slist " +
            //    "LEFT JOIN ( SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS COMP_count " +
            //    "FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE status, C_S_SYSTEM_VALUE scatgrp, C_S_SYSTEM_VALUE comp_type WHERE capp.CATEGORY_ID = scat.UUID " +
            //    "AND capp.APPLICATION_STATUS_ID = status.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
            //    "AND comp_type.UUID = capp.COMPANY_TYPE_ID AND comp_type.CODE = '1' GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION ) app_comp ON slist.status = app_comp.status AND slist.code = app_comp.code " +
            //    "LEFT JOIN (SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS COMP2_count FROM C_comp_application capp," +
            //    "C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE status, C_S_SYSTEM_VALUE scatgrp, C_S_SYSTEM_VALUE comp_type WHERE capp.CATEGORY_ID = scat.UUID " +
            //    "AND capp.APPLICATION_STATUS_ID = status.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND comp_type.UUID = capp.COMPANY_TYPE_ID AND comp_type.CODE IN ('2', '3') " +
            //    "GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_comp2 ON slist.status = app_comp2.status AND slist.code = app_comp2.code " +
            //    "LEFT JOIN(SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS AS_count FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, " +
            //    "C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE status WHERE capp.CATEGORY_ID = scat.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND cinfo.MASTER_ID = capp.UUID " +
            //    "AND cinfo.APPLICANT_ROLE_ID = srole.UUID AND cinfo.APPLICANT_STATUS_ID = status.UUID " +
            //    "AND srole.CODE = 'AS' GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_as ON slist.status = app_as.status AND slist.code = app_as.code " +
            //    "LEFT JOIN(SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS TD_count " +
            //    "FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, " +
            //    "C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE status WHERE capp.CATEGORY_ID = scat.UUID " +
            //    "AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND cinfo.MASTER_ID = capp.UUID AND cinfo.APPLICANT_ROLE_ID = srole.UUID " +
            //    "AND cinfo.APPLICANT_STATUS_ID = status.UUID AND srole.CODE = 'TD' " +
            //    "GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_td ON slist.status = app_td.status AND slist.code = app_td.code " +
            //    "LEFT JOIN (SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS OO_count " +
            //    "FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE status " +
            //    "WHERE capp.CATEGORY_ID = scat.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND cinfo.MASTER_ID = capp.UUID " +
            //    "AND cinfo.APPLICANT_ROLE_ID = srole.UUID AND cinfo.APPLICANT_STATUS_ID = status.UUID AND srole.CODE = 'OO' " +
            //    "GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_oo ON slist.status = app_oo.status AND slist.code = app_oo.code) ORDER BY slist.code, slist.status";

            return sql.ToString();
        }

        //CRM0051
        public string getNoOfRegCGCXlsSql()
        {
            string sql = "SELECT slist.code, slist.status, app_as.AS_count, app_td.TD_count, app_oo.OO_count, app_comp.COMP_count, app_comp2.COMP2_count,:category AS CATEGORY FROM(" +
                "(SELECT t1.code, t2.status FROM(" +
                "(SELECT scat.code, scat.english_description AS cat_desc FROM C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp WHERE scatgrp.UUID = scat.CATEGORY_GROUP_ID AND scatgrp.REGISTRATION_TYPE = 'CGC') t1 " +
                "LEFT JOIN(SELECT svalue.english_description AS status FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue WHERE stype.UUID = svalue.SYSTEM_TYPE_ID AND stype.TYPE = 'APPLICANT_STATUS') t2 ON 1 = 1)) slist " +
                "LEFT JOIN ( SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS COMP_count " +
                "FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE status, C_S_SYSTEM_VALUE scatgrp, C_S_SYSTEM_VALUE comp_type WHERE capp.CATEGORY_ID = scat.UUID " +
                "AND capp.APPLICATION_STATUS_ID = status.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
                "AND comp_type.UUID = capp.COMPANY_TYPE_ID AND comp_type.CODE = '1' GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION ) app_comp ON slist.status = app_comp.status AND slist.code = app_comp.code " +
                "LEFT JOIN (SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS COMP2_count FROM C_comp_application capp," +
                "C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE status, C_S_SYSTEM_VALUE scatgrp, C_S_SYSTEM_VALUE comp_type WHERE capp.CATEGORY_ID = scat.UUID " +
                "AND capp.APPLICATION_STATUS_ID = status.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND comp_type.UUID = capp.COMPANY_TYPE_ID AND comp_type.CODE IN ('2', '3') " +
                "GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_comp2 ON slist.status = app_comp2.status AND slist.code = app_comp2.code " +
                "LEFT JOIN(SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS AS_count FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, " +
                "C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE status WHERE capp.CATEGORY_ID = scat.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND cinfo.MASTER_ID = capp.UUID " +
                "AND cinfo.APPLICANT_ROLE_ID = srole.UUID AND cinfo.APPLICANT_STATUS_ID = status.UUID " +
                "AND srole.CODE = 'AS' GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_as ON slist.status = app_as.status AND slist.code = app_as.code " +
                "LEFT JOIN(SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS TD_count " +
                "FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, " +
                "C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE status WHERE capp.CATEGORY_ID = scat.UUID " +
                "AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND cinfo.MASTER_ID = capp.UUID AND cinfo.APPLICANT_ROLE_ID = srole.UUID " +
                "AND cinfo.APPLICANT_STATUS_ID = status.UUID AND srole.CODE = 'TD' " +
                "GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_td ON slist.status = app_td.status AND slist.code = app_td.code " +
                "LEFT JOIN (SELECT scat.CODE, status.ENGLISH_DESCRIPTION AS status, count(*) AS OO_count " +
                "FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE status " +
                "WHERE capp.CATEGORY_ID = scat.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND cinfo.MASTER_ID = capp.UUID " +
                "AND cinfo.APPLICANT_ROLE_ID = srole.UUID AND cinfo.APPLICANT_STATUS_ID = status.UUID AND srole.CODE = 'OO' " +
                "GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION) app_oo ON slist.status = app_oo.status AND slist.code = app_oo.code) ORDER BY slist.code, slist.status";

            return sql;
        }

        //CRM0052
        public string getNoOfRegCMWSql()
        {
            string sql = "SELECT '3-AS-1' AS row_type, TYPE, sum(class_1) AS class_1, sum(class_2) AS class_2, sum(class_3) AS class_3, 0 as comp2_cnt,:TypeOfCategorys AS CATEGORY FROM (" +
"SELECT TYPE, class," +
"CASE WHEN class='Class 1' THEN sum(count) END AS Class_1," +
"CASE WHEN class='Class 2' THEN sum(count) END AS Class_2," +
"CASE WHEN class='Class 3' THEN sum(count) END AS Class_3 " +
"FROM (" +
"SELECT TYPE, class, count(*) AS count FROM (" +

"SELECT capp.uuid, app.uuid," +
"stype.CODE AS type, srole.ENGLISH_DESCRIPTION AS role," +
"sstatus.ENGLISH_DESCRIPTION AS status, min(sclass.CODE) AS class FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod," +
"C_COMP_APPLICANT_INFO cinfo," +
"C_COMP_APPLICATION capp," +
"C_APPLICANT app," +
"C_S_SYSTEM_VALUE sclass," +
"C_S_SYSTEM_VALUE stype," +
"C_S_SYSTEM_VALUE srole," +
"C_S_SYSTEM_VALUE sstatus," +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'AS' " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"AND capp.REGISTRATION_TYPE = 'CMW' " +
"AND scat.CODE = '' " +

"GROUP BY capp.uuid, app.uuid, " +
"stype.CODE,  srole.ENGLISH_DESCRIPTION, " +
"sstatus.ENGLISH_DESCRIPTION " +
") a " +
"GROUP BY TYPE, class " +

") " +
"GROUP BY TYPE, class " +
"UNION ALL " +
"SELECT distinct code, '', 0, 0, 0 FROM C_S_SYSTEM_VALUE WHERE CODE in ('Type A', 'Type B', 'Type C', 'Type D', 'Type E', 'Type F', 'Type G') " +
") " +
"GROUP BY TYPE " +

"UNION ALL " +

"SELECT '3-AS-2' AS row_type, status, sum(class1) AS class1, sum(class2) AS class2, sum(class3) AS class3, 0,:TypeOfCategorys AS CATEGORY FROM ( " +
"SELECT status, " +
"CASE WHEN class = 'Class 1' THEN count(DISTINCT app_uuid) ELSE 0 END AS class1, " +
"CASE WHEN class = 'Class 2' THEN count(DISTINCT app_uuid) ELSE 0 END AS class2, " +
"CASE WHEN class = 'Class 3' THEN count(DISTINCT app_uuid) ELSE 0 END AS class3 " +
"FROM ( " +

"SELECT capp.uuid AS cpp_uuid, cinfo.UUID  AS app_uuid, " +

"srole.ENGLISH_DESCRIPTION AS role," +
"sstatus.ENGLISH_DESCRIPTION AS status, " +
"min(sclass.CODE) AS class " +

"FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod, " +
"C_COMP_APPLICANT_INFO cinfo, " +
"C_COMP_APPLICATION capp, " +
"C_APPLICANT app, " +
"C_S_SYSTEM_VALUE sclass, " +
"C_S_SYSTEM_VALUE srole, " +
"C_S_SYSTEM_VALUE stype, " +
"C_S_SYSTEM_VALUE sstatus, " +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'AS' " +
"AND (sstatus.code NOT IN ('1', '4') OR ((sstatus.code IN ('1', '4')) " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
")) " +
"AND scat.CODE = '' " +

"GROUP BY capp.uuid, cinfo.uuid, " +
"srole.ENGLISH_DESCRIPTION, " +
"sstatus.ENGLISH_DESCRIPTION " +

") " +
"GROUP BY status, class " +



"UNION ALL " +


"SELECT t2.status, 0, 0, 0  FROM ( " +
"( " +
"SELECT scat.code, scat.english_description AS cat_desc, scatgrp.code AS catgrp " +
"FROM C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp " +
"WHERE scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
"AND scatgrp.REGISTRATION_TYPE = 'CMW' " +
"AND scat.CODE = '' " +
") t1 " +
"LEFT JOIN " +
"( " +
"SELECT svalue.english_description AS status FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue " +
"WHERE stype.UUID = svalue.SYSTEM_TYPE_ID " +
"AND stype.TYPE = 'APPLICANT_STATUS' " +

") t2 ON 1=1 " +
") " +

") " +
"GROUP BY 1, status " +


"UNION ALL " +



"SELECT '4-TD-1' AS row_type, TYPE, sum(class_1) AS class_1, sum(class_2) AS class_2, sum(class_3) AS class_3, 0,:TypeOfCategorys AS CATEGORY FROM ( " +
"SELECT TYPE, class, " +
"CASE WHEN class='Class 1' THEN sum(count) END AS Class_1, " +
"CASE WHEN class='Class 2' THEN sum(count) END AS Class_2, " +
"CASE WHEN class='Class 3' THEN sum(count) END AS Class_3 " +
"FROM ( " +
"SELECT TYPE, class, count(*) AS count FROM ( " +

"SELECT capp.uuid, app.uuid, " +
"stype.CODE AS type, srole.ENGLISH_DESCRIPTION AS role, " +
"sstatus.ENGLISH_DESCRIPTION AS status, min(sclass.CODE) AS class FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod, " +
"C_COMP_APPLICANT_INFO cinfo, " +
"C_COMP_APPLICATION capp, " +
"C_APPLICANT app, " +
"C_S_SYSTEM_VALUE sclass, " +
"C_S_SYSTEM_VALUE stype, " +
"C_S_SYSTEM_VALUE srole, " +
"C_S_SYSTEM_VALUE sstatus, " +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'TD' " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"AND capp.REGISTRATION_TYPE = 'CMW' " +
"AND scat.CODE = '' " +

"GROUP BY capp.uuid, app.uuid, " +
"stype.CODE,  srole.ENGLISH_DESCRIPTION, " +
"sstatus.ENGLISH_DESCRIPTION " +
") a " +
"GROUP BY TYPE, class " +
") " +
"GROUP BY TYPE, class " +
"UNION ALL " +
"SELECT distinct code, '', 0, 0, 0 FROM C_S_SYSTEM_VALUE WHERE CODE in ('Type A', 'Type B', 'Type C', 'Type D', 'Type E', 'Type F', 'Type G') " +
") " +
"GROUP BY TYPE " +


"UNION ALL " +


"SELECT '4-TD-2' AS row_type, status, sum(class1) AS class1, sum(class2) AS class2, sum(class3) AS class3, 0,:TypeOfCategorys AS CATEGORY FROM ( " +
"SELECT status, " +
"CASE WHEN class = 'Class 1' THEN count(DISTINCT app_uuid) ELSE 0 END AS class1, " +
"CASE WHEN class = 'Class 2' THEN count(DISTINCT app_uuid) ELSE 0 END AS class2, " +
"CASE WHEN class = 'Class 3' THEN count(DISTINCT app_uuid) ELSE 0 END AS class3 " +
"FROM ( " +
"SELECT capp.uuid AS cpp_uuid, cinfo.UUID  AS app_uuid, " +

"srole.ENGLISH_DESCRIPTION AS role," +
"sstatus.ENGLISH_DESCRIPTION AS status, " +
"min(sclass.CODE) AS class " +

"FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod, " +
"C_COMP_APPLICANT_INFO cinfo, " +
"C_COMP_APPLICATION capp, " +
"C_APPLICANT app, " +
"C_S_SYSTEM_VALUE sclass, " +
"C_S_SYSTEM_VALUE srole, " +
"C_S_SYSTEM_VALUE stype, " +
"C_S_SYSTEM_VALUE sstatus, " +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'TD' " +
"AND (sstatus.code NOT IN ('1', '4') OR ((sstatus.code IN ('1', '4')) " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
")) " +
"AND scat.CODE = '' " +

"GROUP BY capp.uuid, cinfo.uuid, " +
"srole.ENGLISH_DESCRIPTION, " +
"sstatus.ENGLISH_DESCRIPTION " +
") " +
"GROUP BY status, class " +



"UNION ALL " +


"SELECT t2.status, 0, 0, 0  FROM ( " +
"( " +
"SELECT scat.code, scat.english_description AS cat_desc, scatgrp.code AS catgrp " +
"FROM C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp " +
"WHERE scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
"AND scatgrp.REGISTRATION_TYPE = 'CMW' " +
"AND scat.CODE = '' " +
") t1 " +
"LEFT JOIN " +
"( " +
"SELECT svalue.english_description AS status FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue " +
"WHERE stype.UUID = svalue.SYSTEM_TYPE_ID " +
"AND stype.TYPE = 'APPLICANT_STATUS' " +

") t2 ON 1=1 " +
") " +

") " +
"GROUP BY 1, status " +


"UNION ALL " +

"SELECT '1-ALL' AS row_type, slist.status, comp_cnt.count AS comp_cnt, as_cnt.count AS as_cnt, td_cnt.count AS td_cnt, comp2_cnt.count AS comp2_cnt,:TypeOfCategorys AS CATEGORY FROM ( " +

"( " +
"SELECT t2.status FROM ( " +
"( " +
"SELECT scat.code, scat.english_description AS cat_desc, scatgrp.code AS catgrp " +
"FROM C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp " +
"WHERE scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
"AND scatgrp.REGISTRATION_TYPE = 'CMW' " +
"AND scat.CODE = '' " +
") t1 " +
"LEFT JOIN " +
"( " +
"SELECT svalue.english_description AS status FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue " +
"WHERE stype.UUID = svalue.SYSTEM_TYPE_ID " +
"AND stype.TYPE = 'APPLICANT_STATUS' " +

") t2 ON 1=1 " +
") " +
") slist " +

"LEFT JOIN " +

"( " +
"SELECT status.ENGLISH_DESCRIPTION AS status, count(*) AS count " +
"FROM C_COMP_APPLICATION capp, C_S_SYSTEM_VALUE status, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE comp_type " +
"WHERE capp.APPLICATION_STATUS_ID = status.uuid " +
"AND capp.CATEGORY_ID = scat.UUID " +
"AND comp_type.UUID = capp.COMPANY_TYPE_ID " +
"AND comp_type.CODE = '1' " +
"AND scat.code = '' " +

"GROUP BY status.ENGLISH_DESCRIPTION " +
") comp_cnt ON slist.status = comp_cnt.status " +
"LEFT JOIN " +

"( " +
"SELECT status.ENGLISH_DESCRIPTION AS status, count(*) AS count " +
"FROM C_COMP_APPLICATION capp, C_S_SYSTEM_VALUE status, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE comp_type " +
"WHERE capp.APPLICATION_STATUS_ID = status.uuid " +
"AND capp.CATEGORY_ID = scat.UUID " +
"AND comp_type.UUID = capp.COMPANY_TYPE_ID " +
"AND comp_type.CODE IN ('2', '3') " +
"AND scat.code = '' " +

"GROUP BY status.ENGLISH_DESCRIPTION " +
") comp2_cnt ON slist.status = comp2_cnt.status " +
"LEFT JOIN " +

"( " +


"SELECT " +
"sstatus.ENGLISH_DESCRIPTION AS status, count(DISTINCT cinfo.UUID) AS count " +

"FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod, " +
"C_COMP_APPLICANT_INFO cinfo, " +
"C_COMP_APPLICATION capp, " +
"C_APPLICANT app, " +
"C_S_SYSTEM_VALUE sclass, " +
"C_S_SYSTEM_VALUE srole, " +
"C_S_SYSTEM_VALUE stype, " +
"C_S_SYSTEM_VALUE sstatus, " +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'AS' " +
"AND (sstatus.code NOT IN ('1', '4') OR ((sstatus.code IN ('1', '4')) " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
")) " +
"AND scat.CODE = '' " +

"GROUP BY sstatus.ENGLISH_DESCRIPTION " +


") as_cnt ON slist.status = as_cnt.status " +

"LEFT JOIN " +

"( " +


"SELECT " +
"sstatus.ENGLISH_DESCRIPTION AS status, count(DISTINCT cinfo.UUID) AS count " +

"FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod, " +
"C_COMP_APPLICANT_INFO cinfo, " +
"C_COMP_APPLICATION capp, " +
"C_APPLICANT app, " +
"C_S_SYSTEM_VALUE sclass, " +
"C_S_SYSTEM_VALUE srole, " +
"C_S_SYSTEM_VALUE stype, " +
"C_S_SYSTEM_VALUE sstatus, " +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'TD' " +
"AND (sstatus.code NOT IN ('1', '4') OR ((sstatus.code IN ('1', '4')) " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
")) " +
"AND scat.CODE = '' " +

"GROUP BY sstatus.ENGLISH_DESCRIPTION " +

") td_cnt ON slist.status = td_cnt.status " +
") " +


"ORDER BY 1, 2 ";

            return sql;
        }

        //CRM0053
        public string getNoOfRegCMWXlsSql()
        {
            string sql = "SELECT '3-AS-1' AS row_type, TYPE, sum(class_1) AS class_1, sum(class_2) AS class_2, sum(class_3) AS class_3, 0 as comp2_cnt,:category AS CATEGORY FROM (" +
"SELECT TYPE, class," +
"CASE WHEN class='Class 1' THEN sum(count) END AS Class_1," +
"CASE WHEN class='Class 2' THEN sum(count) END AS Class_2," +
"CASE WHEN class='Class 3' THEN sum(count) END AS Class_3 " +
"FROM (" +
"SELECT TYPE, class, count(*) AS count FROM (" +

"SELECT capp.uuid, app.uuid," +
"stype.CODE AS type, srole.ENGLISH_DESCRIPTION AS role," +
"sstatus.ENGLISH_DESCRIPTION AS status, min(sclass.CODE) AS class FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod," +
"C_COMP_APPLICANT_INFO cinfo," +
"C_COMP_APPLICATION capp," +
"C_APPLICANT app," +
"C_S_SYSTEM_VALUE sclass," +
"C_S_SYSTEM_VALUE stype," +
"C_S_SYSTEM_VALUE srole," +
"C_S_SYSTEM_VALUE sstatus," +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'AS' " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"AND capp.REGISTRATION_TYPE = 'CMW' " +
"AND scat.CODE = '' " +

"GROUP BY capp.uuid, app.uuid, " +
"stype.CODE,  srole.ENGLISH_DESCRIPTION, " +
"sstatus.ENGLISH_DESCRIPTION " +
") a " +
"GROUP BY TYPE, class " +

") " +
"GROUP BY TYPE, class " +
"UNION ALL " +
"SELECT distinct code, '', 0, 0, 0 FROM C_S_SYSTEM_VALUE WHERE CODE in ('Type A', 'Type B', 'Type C', 'Type D', 'Type E', 'Type F', 'Type G') " +
") " +
"GROUP BY TYPE " +

"UNION ALL " +

"SELECT '3-AS-2' AS row_type, status, sum(class1) AS class1, sum(class2) AS class2, sum(class3) AS class3, 0,:category AS CATEGORY FROM ( " +
"SELECT status, " +
"CASE WHEN class = 'Class 1' THEN count(DISTINCT app_uuid) ELSE 0 END AS class1, " +
"CASE WHEN class = 'Class 2' THEN count(DISTINCT app_uuid) ELSE 0 END AS class2, " +
"CASE WHEN class = 'Class 3' THEN count(DISTINCT app_uuid) ELSE 0 END AS class3 " +
"FROM ( " +

"SELECT capp.uuid AS cpp_uuid, cinfo.UUID  AS app_uuid, " +

"srole.ENGLISH_DESCRIPTION AS role," +
"sstatus.ENGLISH_DESCRIPTION AS status, " +
"min(sclass.CODE) AS class " +

"FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod, " +
"C_COMP_APPLICANT_INFO cinfo, " +
"C_COMP_APPLICATION capp, " +
"C_APPLICANT app, " +
"C_S_SYSTEM_VALUE sclass, " +
"C_S_SYSTEM_VALUE srole, " +
"C_S_SYSTEM_VALUE stype, " +
"C_S_SYSTEM_VALUE sstatus, " +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'AS' " +
"AND (sstatus.code NOT IN ('1', '4') OR ((sstatus.code IN ('1', '4')) " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
")) " +
"AND scat.CODE = '' " +

"GROUP BY capp.uuid, cinfo.uuid, " +
"srole.ENGLISH_DESCRIPTION, " +
"sstatus.ENGLISH_DESCRIPTION " +

") " +
"GROUP BY status, class " +



"UNION ALL " +


"SELECT t2.status, 0, 0, 0  FROM ( " +
"( " +
"SELECT scat.code, scat.english_description AS cat_desc, scatgrp.code AS catgrp " +
"FROM C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp " +
"WHERE scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
"AND scatgrp.REGISTRATION_TYPE = 'CMW' " +
"AND scat.CODE = '' " +
") t1 " +
"LEFT JOIN " +
"( " +
"SELECT svalue.english_description AS status FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue " +
"WHERE stype.UUID = svalue.SYSTEM_TYPE_ID " +
"AND stype.TYPE = 'APPLICANT_STATUS' " +

") t2 ON 1=1 " +
") " +

") " +
"GROUP BY 1, status " +


"UNION ALL " +



"SELECT '4-TD-1' AS row_type, TYPE, sum(class_1) AS class_1, sum(class_2) AS class_2, sum(class_3) AS class_3, 0,:category AS CATEGORY FROM ( " +
"SELECT TYPE, class, " +
"CASE WHEN class='Class 1' THEN sum(count) END AS Class_1, " +
"CASE WHEN class='Class 2' THEN sum(count) END AS Class_2, " +
"CASE WHEN class='Class 3' THEN sum(count) END AS Class_3 " +
"FROM ( " +
"SELECT TYPE, class, count(*) AS count FROM ( " +

"SELECT capp.uuid, app.uuid, " +
"stype.CODE AS type, srole.ENGLISH_DESCRIPTION AS role, " +
"sstatus.ENGLISH_DESCRIPTION AS status, min(sclass.CODE) AS class FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod, " +
"C_COMP_APPLICANT_INFO cinfo, " +
"C_COMP_APPLICATION capp, " +
"C_APPLICANT app, " +
"C_S_SYSTEM_VALUE sclass, " +
"C_S_SYSTEM_VALUE stype, " +
"C_S_SYSTEM_VALUE srole, " +
"C_S_SYSTEM_VALUE sstatus, " +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'TD' " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"AND capp.REGISTRATION_TYPE = 'CMW' " +
"AND scat.CODE = '' " +

"GROUP BY capp.uuid, app.uuid, " +
"stype.CODE,  srole.ENGLISH_DESCRIPTION, " +
"sstatus.ENGLISH_DESCRIPTION " +
") a " +
"GROUP BY TYPE, class " +
") " +
"GROUP BY TYPE, class " +
"UNION ALL " +
"SELECT distinct code, '', 0, 0, 0 FROM C_S_SYSTEM_VALUE WHERE CODE in ('Type A', 'Type B', 'Type C', 'Type D', 'Type E', 'Type F', 'Type G') " +
") " +
"GROUP BY TYPE " +


"UNION ALL " +


"SELECT '4-TD-2' AS row_type, status, sum(class1) AS class1, sum(class2) AS class2, sum(class3) AS class3, 0,:category AS CATEGORY FROM ( " +
"SELECT status, " +
"CASE WHEN class = 'Class 1' THEN count(DISTINCT app_uuid) ELSE 0 END AS class1, " +
"CASE WHEN class = 'Class 2' THEN count(DISTINCT app_uuid) ELSE 0 END AS class2, " +
"CASE WHEN class = 'Class 3' THEN count(DISTINCT app_uuid) ELSE 0 END AS class3 " +
"FROM ( " +
"SELECT capp.uuid AS cpp_uuid, cinfo.UUID  AS app_uuid, " +

"srole.ENGLISH_DESCRIPTION AS role," +
"sstatus.ENGLISH_DESCRIPTION AS status, " +
"min(sclass.CODE) AS class " +

"FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod, " +
"C_COMP_APPLICANT_INFO cinfo, " +
"C_COMP_APPLICATION capp, " +
"C_APPLICANT app, " +
"C_S_SYSTEM_VALUE sclass, " +
"C_S_SYSTEM_VALUE srole, " +
"C_S_SYSTEM_VALUE stype, " +
"C_S_SYSTEM_VALUE sstatus, " +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'TD' " +
"AND (sstatus.code NOT IN ('1', '4') OR ((sstatus.code IN ('1', '4')) " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
")) " +
"AND scat.CODE = '' " +

"GROUP BY capp.uuid, cinfo.uuid, " +
"srole.ENGLISH_DESCRIPTION, " +
"sstatus.ENGLISH_DESCRIPTION " +
") " +
"GROUP BY status, class " +



"UNION ALL " +


"SELECT t2.status, 0, 0, 0  FROM ( " +
"( " +
"SELECT scat.code, scat.english_description AS cat_desc, scatgrp.code AS catgrp " +
"FROM C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp " +
"WHERE scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
"AND scatgrp.REGISTRATION_TYPE = 'CMW' " +
"AND scat.CODE = '' " +
") t1 " +
"LEFT JOIN " +
"( " +
"SELECT svalue.english_description AS status FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue " +
"WHERE stype.UUID = svalue.SYSTEM_TYPE_ID " +
"AND stype.TYPE = 'APPLICANT_STATUS' " +

") t2 ON 1=1 " +
") " +

") " +
"GROUP BY 1, status " +


"UNION ALL " +

"SELECT '1-ALL' AS row_type, slist.status, comp_cnt.count AS comp_cnt, as_cnt.count AS as_cnt, td_cnt.count AS td_cnt, comp2_cnt.count AS comp2_cnt,:category AS CATEGORY FROM ( " +

"( " +
"SELECT t2.status FROM ( " +
"( " +
"SELECT scat.code, scat.english_description AS cat_desc, scatgrp.code AS catgrp " +
"FROM C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp " +
"WHERE scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
"AND scatgrp.REGISTRATION_TYPE = 'CMW' " +
"AND scat.CODE = '' " +
") t1 " +
"LEFT JOIN " +
"( " +
"SELECT svalue.english_description AS status FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue " +
"WHERE stype.UUID = svalue.SYSTEM_TYPE_ID " +
"AND stype.TYPE = 'APPLICANT_STATUS' " +

") t2 ON 1=1 " +
") " +
") slist " +

"LEFT JOIN " +

"( " +
"SELECT status.ENGLISH_DESCRIPTION AS status, count(*) AS count " +
"FROM C_COMP_APPLICATION capp, C_S_SYSTEM_VALUE status, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE comp_type " +
"WHERE capp.APPLICATION_STATUS_ID = status.uuid " +
"AND capp.CATEGORY_ID = scat.UUID " +
"AND comp_type.UUID = capp.COMPANY_TYPE_ID " +
"AND comp_type.CODE = '1' " +
"AND scat.code = '' " +

"GROUP BY status.ENGLISH_DESCRIPTION " +
") comp_cnt ON slist.status = comp_cnt.status " +
"LEFT JOIN " +

"( " +
"SELECT status.ENGLISH_DESCRIPTION AS status, count(*) AS count " +
"FROM C_COMP_APPLICATION capp, C_S_SYSTEM_VALUE status, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE comp_type " +
"WHERE capp.APPLICATION_STATUS_ID = status.uuid " +
"AND capp.CATEGORY_ID = scat.UUID " +
"AND comp_type.UUID = capp.COMPANY_TYPE_ID " +
"AND comp_type.CODE IN ('2', '3') " +
"AND scat.code = '' " +

"GROUP BY status.ENGLISH_DESCRIPTION " +
") comp2_cnt ON slist.status = comp2_cnt.status " +
"LEFT JOIN " +

"( " +


"SELECT " +
"sstatus.ENGLISH_DESCRIPTION AS status, count(DISTINCT cinfo.UUID) AS count " +

"FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod, " +
"C_COMP_APPLICANT_INFO cinfo, " +
"C_COMP_APPLICATION capp, " +
"C_APPLICANT app, " +
"C_S_SYSTEM_VALUE sclass, " +
"C_S_SYSTEM_VALUE srole, " +
"C_S_SYSTEM_VALUE stype, " +
"C_S_SYSTEM_VALUE sstatus, " +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'AS' " +
"AND (sstatus.code NOT IN ('1', '4') OR ((sstatus.code IN ('1', '4')) " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
")) " +
"AND scat.CODE = '' " +

"GROUP BY sstatus.ENGLISH_DESCRIPTION " +


") as_cnt ON slist.status = as_cnt.status " +

"LEFT JOIN " +

"( " +


"SELECT " +
"sstatus.ENGLISH_DESCRIPTION AS status, count(DISTINCT cinfo.UUID) AS count " +

"FROM " +
"C_COMP_APPLICANT_MW_ITEM cinfod, " +
"C_COMP_APPLICANT_INFO cinfo, " +
"C_COMP_APPLICATION capp, " +
"C_APPLICANT app, " +
"C_S_SYSTEM_VALUE sclass, " +
"C_S_SYSTEM_VALUE srole, " +
"C_S_SYSTEM_VALUE stype, " +
"C_S_SYSTEM_VALUE sstatus, " +
"C_S_CATEGORY_CODE scat " +
"WHERE cinfod.ITEM_CLASS_ID = sclass.UUID " +
"AND cinfod.ITEM_TYPE_ID = stype.UUID " +
"AND cinfod.COMPANY_APPLICANTS_ID = cinfo.UUID " +
"AND app.UUID = cinfo.APPLICANT_ID " +
"AND capp.UUID = cinfo.MASTER_ID " +
"AND srole.UUID = cinfo.APPLICANT_ROLE_ID " +
"AND sstatus.UUID = cinfo.APPLICANT_STATUS_ID " +
"AND scat.UUID = capp.CATEGORY_ID " +
"AND srole.CODE = 'TD' " +
"AND (sstatus.code NOT IN ('1', '4') OR ((sstatus.code IN ('1', '4')) " +
"AND cinfo.ACCEPT_DATE IS NOT NULL " +
"AND capp.CERTIFICATION_NO IS NOT NULL " +
"AND to_char(capp.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd') " +
"and (cinfo.REMOVAL_DATE IS NULL or (cinfo.REMOVAL_DATE IS NOT NULL and to_char(cinfo.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
"and (capp.REMOVAL_DATE IS NULL or (to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char(capp.EXPIRY_DATE, 'yyyymmdd') and to_char(capp.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd'))) " +
")) " +
"AND scat.CODE = '' " +

"GROUP BY sstatus.ENGLISH_DESCRIPTION " +

") td_cnt ON slist.status = td_cnt.status " +
") " +


"ORDER BY 1, 2 ";

            return sql;
        }

        //CRM0054
        public string getCGCProcessMonitor(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT " +
                "(C_APPL.ENGLISH_COMPANY_NAME) AS ENAME, " +
                "C_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
                "S_APP_FORM.CODE AS FORM_CODE," +
                "C_PMON.NATURE AS NATURE, " +
                "(APPLN.SURNAME)AS SURNAME, " +
                "(APPLN.GIVEN_NAME_ON_ID)AS GIVEN_NAME, " +
                "S_ROLE.CODE AS POST_ROLE, " +
                "C_PMON.RECEIVED_DATE AS RECEIVED_D, " +
                "C_PMON.VETTING_OFFICER AS VO, " +
                "C_PMON.PLEDGE_INITIAL_DATE AS PLEDGE_DUE_10_DAYS_D, " +
                "C_PMON.ALTERNATIVE_LETTER_DATE AS ALT_LETTER_D, " +
                "C_PMON.SUBSEQUENT_DATE AS SUB_D, " +
                "C_PMON.PRELIMINARY_LETTER_DATE AS PRELIM_LETTER_D, " +
                "C_PMON.CRC_NAME AS CRC, " +
                "C_PMON.INTERVIEW_DATE AS INTRV_D, " +
                "C_PMON.SECRETARY AS SECRETARY," +
                "C_PMON.RESULT_LETTER_DATE AS RESULT_LETTER_D," +
                "S_INTRV_RESULT.CODE AS INTRV_RESULT," +
                "C_PMON.INITIAL_REPLY AS INIT_REPLY," +
                "C_PMON.INTERVIEW AS INTRV," +
                "C_PMON.RESULT_LETTER AS RESULT_LETTER," +
                " C_PMON.CERTIFICATE_ISSUED_DATE AS CERT_ISSUED_D," +
                "C_PMON.REMARKS AS REMARKS," +
                ":today AS AS_TODAY " +
                "FROM C_COMP_APPLICATION C_APPL " +
                "INNER JOIN C_COMP_PROCESS_MONITOR C_PMON ON C_APPL.UUID = C_PMON.MASTER_ID " +
                "INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID " +
                "AND C_APPLN.UUID = C_PMON.COMPANY_APPLICANTS_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_ROLE.UUID " +
                "INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_APP_FORM ON C_PMON.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_INTRV_RESULT ON C_PMON.INTERVIEW_RESULT_ID = S_INTRV_RESULT.UUID " +
                "WHERE C_PMON.MONITOR_TYPE = 'UPM' and C_APPL.REGISTRATION_TYPE =:reg_type ");
            if (myDictionary["pmon_type"].ToString() == "C")
                sql.Append(" And (C_PMON.RESULT_LETTER_DATE is not null or C_PMON.CERTIFICATE_ISSUED_DATE is not null or C_PMON.WITHDRAW_DATE is not null )");
            if (myDictionary["pmon_type"].ToString() == "I")
                sql.Append("　And (C_PMON.RESULT_LETTER_DATE is  null or C_PMON.CERTIFICATE_ISSUED_DATE is  null or C_PMON.WITHDRAW_DATE is  null )");

            //string sql = "SELECT " +
            //    "(C_APPL.ENGLISH_COMPANY_NAME) AS ENAME, " +
            //    "C_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
            //    "S_APP_FORM.CODE AS FORM_CODE," +
            //    "C_PMON.NATURE AS NATURE, " +
            //    "(APPLN.SURNAME)AS SURNAME, " +
            //    "(APPLN.GIVEN_NAME_ON_ID)AS GIVEN_NAME, " +
            //    "S_ROLE.CODE AS POST_ROLE, " +
            //    "C_PMON.RECEIVED_DATE AS RECEIVED_D, " +
            //    "C_PMON.VETTING_OFFICER AS VO, " +
            //    "C_PMON.PLEDGE_INITIAL_DATE AS PLEDGE_DUE_10_DAYS_D, " +
            //    "C_PMON.ALTERNATIVE_LETTER_DATE AS ALT_LETTER_D, " +
            //    "C_PMON.SUBSEQUENT_DATE AS SUB_D, " +
            //    "C_PMON.PRELIMINARY_LETTER_DATE AS PRELIM_LETTER_D, " +
            //    "C_PMON.CRC_NAME AS CRC, " +
            //    "C_PMON.INTERVIEW_DATE AS INTRV_D, " +
            //    "C_PMON.SECRETARY AS SECRETARY," +
            //    "C_PMON.RESULT_LETTER_DATE AS RESULT_LETTER_D," +
            //    "S_INTRV_RESULT.CODE AS INTRV_RESULT," +
            //    "C_PMON.INITIAL_REPLY AS INIT_REPLY," +
            //    "C_PMON.INTERVIEW AS INTRV," +
            //    "C_PMON.RESULT_LETTER AS RESULT_LETTER," +
            //    " C_PMON.CERTIFICATE_ISSUED_DATE AS CERT_ISSUED_D," +
            //    "C_PMON.REMARKS AS REMARKS," +
            //    ":today AS AS_TODAY " +
            //    "FROM C_COMP_APPLICATION C_APPL " +
            //    "INNER JOIN C_COMP_PROCESS_MONITOR C_PMON ON C_APPL.UUID = C_PMON.MASTER_ID " +
            //    "INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID " +
            //    "AND C_APPLN.UUID = C_PMON.COMPANY_APPLICANTS_ID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_ROLE.UUID " +
            //    "INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_APP_FORM ON C_PMON.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_INTRV_RESULT ON C_PMON.INTERVIEW_RESULT_ID = S_INTRV_RESULT.UUID " +
            //    "WHERE C_PMON.MONITOR_TYPE = 'UPM' and C_APPL.REGISTRATION_TYPE =:reg_type " +
            //    "order by FILE_REF, ENAME, SURNAME, GIVEN_NAME";
            return sql.ToString();
        }

        //CRM0055
        public string getApplicantsMonCGCIA()
        {
            string sql = "SELECT " +
                "S_CAT.CODE AS CAT_CODE, " +
                "M2.L_MONTH, " +
                "M2.MONTH, " +
                "C_RPT_ASTDOO_NO_INTRV('I', S_CAT.UUID, M2.MONTH, :MonitorReportsYear, :reg_type) AS NO_INTRV, " +
                "C_RPT_AS_TD ('I', 'AS', S_CAT.UUID, 'A', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTD_AS_A, " +
                "C_RPT_AS_TD ('I', 'AS', S_CAT.UUID, 'D', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTD_AS_D, " +
                "C_RPT_AS_TD ('I', 'AS', S_CAT.UUID, 'R', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTD_AS_R, " +
                "C_RPT_AS_TD ('I', 'AS', S_CAT.UUID, 'I', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTD_AS_I, " +
                "C_RPT_AS_TD ('I', 'TD', S_CAT.UUID, 'A', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTD_TD_A, " +
                "C_RPT_AS_TD ('I', 'TD', S_CAT.UUID, 'D', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTD_TD_D, " +
                "C_RPT_AS_TD ('I', 'TD', S_CAT.UUID, 'R', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTD_TD_R, " +
                "C_RPT_AS_TD ('I', 'TD', S_CAT.UUID, 'I', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTD_TD_I, " +
                "C_RPT_ASTDOO ('I', 'AS', S_CAT.UUID, 'A', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_AS_A, " +
                "C_RPT_ASTDOO ('I', 'AS', S_CAT.UUID, 'D', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_AS_D, " +
                "C_RPT_ASTDOO ('I', 'AS', S_CAT.UUID, 'R', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_AS_R, " +
                "C_RPT_ASTDOO ('I', 'AS', S_CAT.UUID, 'I', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_AS_I, " +
                "C_RPT_ASTDOO ('I', 'TD', S_CAT.UUID, 'A', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_TD_A, " +
                "C_RPT_ASTDOO ('I', 'TD', S_CAT.UUID, 'D', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_TD_D, " +
                "C_RPT_ASTDOO ('I', 'TD', S_CAT.UUID, 'R', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_TD_R, " +
                "C_RPT_ASTDOO ('I', 'TD', S_CAT.UUID, 'I', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_TD_I, " +
                "C_RPT_ASTDOO ('I', 'OO', S_CAT.UUID, 'A', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_OO_A, " +
                "C_RPT_ASTDOO ('I', 'OO', S_CAT.UUID, 'D', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_OO_D, " +
                "C_RPT_ASTDOO ('I', 'OO', S_CAT.UUID, 'R', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_OO_R, " +
                "C_RPT_ASTDOO ('I', 'OO', S_CAT.UUID, 'I', M2.MONTH, :MonitorReportsYear, :reg_type) AS I_ASTDOO_OO_I," +
                ":MonitorReportsYear AS AS_YEAR " +
                "FROM " +
                "C_S_SYSTEM_VALUE CAT_GP " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON CAT_GP.UUID = S_CAT.CATEGORY_GROUP_ID " +
                "LEFT JOIN (" +
                "select '01' as MONTH, 'I' AS APPLY_STATUS,'01-Jan' as L_MONTH FROM DUAL " +
                "union select '02' as MONTH, 'I' AS APPLY_STATUS,'02-Feb' as L_MONTH FROM DUAL " +
                "union select '03' as MONTH, 'I' AS APPLY_STATUS,'03-Mar' as L_MONTH FROM DUAL " +
                "union select '04' as MONTH, 'I' AS APPLY_STATUS,'04-Apr' as L_MONTH FROM DUAL " +
                "union select '05' as MONTH, 'I' AS APPLY_STATUS,'05-May' as L_MONTH FROM DUAL " +
                "union select '06' as MONTH, 'I' AS APPLY_STATUS,'06-Jun' as L_MONTH FROM DUAL " +
                "union select '07' as MONTH, 'I' AS APPLY_STATUS,'07-Jul' as L_MONTH FROM DUAL " +
                "union select '08' as MONTH, 'I' AS APPLY_STATUS,'08-Aug' as L_MONTH FROM DUAL " +
                "union select '09' as MONTH, 'I' AS APPLY_STATUS,'09-Sep' as L_MONTH FROM DUAL " +
                "union select '10' as MONTH, 'I' AS APPLY_STATUS,'10-Oct' as L_MONTH FROM DUAL " +
                "union select '11' as MONTH, 'I' AS APPLY_STATUS,'11-Nov' as L_MONTH FROM DUAL " +
                "union select '12' as MONTH, 'I' AS APPLY_STATUS,'12-Dec' as L_MONTH FROM DUAL) M2 ON S_CAT.UUID<> M2.MONTH " +
                "WHERE CAT_GP.REGISTRATION_TYPE = :reg_type " +
                "ORDER by S_CAT.CODE, M2.L_MONTH";
            return sql;
        }

        //CRM0056
        public string getAttendancePeriod()
        {
            string sql = "SELECT " +
                "A.APPLN_UUID, " +
                "c_propercase(A.ENAME) AS ENAME," +
                "count(B.IS_ABSENT)AS ABSENT," +
                "sum(CASE WHEN B.IS_ABSENT = 'N' THEN 1 else 0 end)as P," +
                " sum(CASE WHEN B.IS_ABSENT = 'Y' THEN 1 else 0 end) as A," +
                "summary.SUMP," +
                "summary.SUMA," +
                "to_char(to_date(:AttendancFromDate,'dd/mm/yyyy'),'dd/mm/yyyy') AS FR_DATE," +
                "to_char(to_date(:AttendancFromTo,'dd/mm/yyyy'),'dd/mm/yyyy') AS TO_DATE " +
                "FROM " +
                "(SELECT C_MEM.APPLICANT_ID AS APPLN_UUID, C_MEM.UUID AS MEMBER_ID,APPLN.SURNAME || ' ' || APPLN.GIVEN_NAME_ON_ID AS ENAME," +
                "C_PNL.PANEL_TYPE_ID AS PANEL_TYPE_ID " +
                "FROM C_COMMITTEE_PANEL_MEMBER C_PNL_MEM " +
                "INNER JOIN C_COMMITTEE_MEMBER C_MEM ON C_PNL_MEM.MEMBER_ID = C_MEM.UUID " +
                "INNER JOIN C_APPLICANT APPLN ON C_MEM.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_COMMITTEE_PANEL C_PNL ON C_PNL_MEM.COMMITTEE_PANEL_ID = C_PNL.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_PNL ON C_PNL.PANEL_TYPE_ID = S_PNL.UUID " +
                "WHERE C_PNL.YEAR = substr(to_char(to_date(:AttendancFromDate,'dd/mm/yyyy'),'dd/mm/yyyy'), 7, 4) " +
                "and S_PNL.REGISTRATION_TYPE = :reg_type ) A " +
                "LEFT JOIN (SELECT " +
                "M_MEM.IS_ABSENT AS IS_ABSENT, " +
                "to_char(I_SCH.INTERVIEW_DATE, 'DD/MM/YYYY')AS INTRV_D, " +
                "M_MEM.MEMBER_ID AS MEMBER_ID, " +
                "M.YEAR AS M_YEAR, " +
                "M.MEETING_GROUP || M.MEETING_NO || '/' || substr(M.YEAR, 3, 2)AS MEETING, " +
                "S_COMM_TYPE.CODE AS COMM_TYPE_CODE, " +
                "S_PNL_TYPE.UUID AS PANEL_TYPE_ID " +
                "FROM " +
                "C_MEETING_MEMBER M_MEM INNER JOIN C_MEETING M ON M_MEM.MEETING_ID = M.UUID " +
                "INNER JOIN C_INTERVIEW_SCHEDULE I_SCH ON M.UUID = I_SCH.MEETING_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_COMM_TYPE ON M.COMMITTEE_TYPE_ID = S_COMM_TYPE.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_PNL_TYPE ON S_COMM_TYPE.PARENT_ID = S_PNL_TYPE.UUID " +
                "WHERE " +
                "M_MEM.committee_role_id in " +
                "(select sv.uuid from C_S_SYSTEM_VALUE sv, C_S_SYSTEM_TYPE st " +
                " where sv.system_type_id = st.uuid and " +
                "st.type = 'COMMITTEE_ROLE' and sv.code not in ('1', '2')) AND " +
                "I_SCH.INTERVIEW_DATE >= to_date(:AttendancFromDate, 'DD/MM/YYYY') " +
                "AND I_SCH.INTERVIEW_DATE <= to_date(:AttendancFromTo, 'DD/MM/YYYY') " +
                "AND I_SCH.IS_CANCEL = 'N' " +
                " AND S_COMM_TYPE.REGISTRATION_TYPE = :reg_type " +
                "AND S_PNL_TYPE.REGISTRATION_TYPE = :reg_type " +
                ") B ON A.MEMBER_ID = B.MEMBER_ID and A.PANEL_TYPE_ID = B.PANEL_TYPE_ID " +
                "INNER JOIN(SELECT " +
                "sum(CASE WHEN B.IS_ABSENT= 'N' THEN 1 else 0 end)as sumP,sum(CASE WHEN B.IS_ABSENT = 'Y' THEN 1 else 0 end) as sumA " +
                "FROM " +
                "(SELECT " +
                "C_MEM.APPLICANT_ID AS APPLN_UUID, " +
                "C_MEM.UUID AS MEMBER_ID, " +
                "APPLN.SURNAME || ' ' || APPLN.GIVEN_NAME_ON_ID AS ENAME, " +
                "C_PNL.PANEL_TYPE_ID AS PANEL_TYPE_ID FROM C_COMMITTEE_PANEL_MEMBER C_PNL_MEM " +
                "INNER JOIN C_COMMITTEE_MEMBER C_MEM ON C_PNL_MEM.MEMBER_ID = C_MEM.UUID " +
                "INNER JOIN C_APPLICANT APPLN ON C_MEM.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_COMMITTEE_PANEL C_PNL ON C_PNL_MEM.COMMITTEE_PANEL_ID = C_PNL.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_PNL ON C_PNL.PANEL_TYPE_ID = S_PNL.UUID " +
                "WHERE C_PNL.YEAR = substr(:AttendancFromDate, 7, 4) and S_PNL.REGISTRATION_TYPE = :reg_type  ) A " +
                "LEFT JOIN " +
                "(SELECT M_MEM.IS_ABSENT AS IS_ABSENT, to_char(I_SCH.INTERVIEW_DATE, 'DD/MM/YYYY')AS INTRV_D, M_MEM.MEMBER_ID AS MEMBER_ID," +
                "M.YEAR AS M_YEAR, M.MEETING_GROUP || M.MEETING_NO || '/' || substr(M.YEAR, 3, 2)AS MEETING, S_COMM_TYPE.CODE AS COMM_TYPE_CODE," +
                "S_PNL_TYPE.UUID AS PANEL_TYPE_ID " +
                "FROM C_MEETING_MEMBER M_MEM " +
                "INNER JOIN C_MEETING M ON M_MEM.MEETING_ID = M.UUID " +
                "INNER JOIN C_INTERVIEW_SCHEDULE I_SCH ON M.UUID = I_SCH.MEETING_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_COMM_TYPE ON M.COMMITTEE_TYPE_ID = S_COMM_TYPE.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_PNL_TYPE ON S_COMM_TYPE.PARENT_ID = S_PNL_TYPE.UUID " +
                "WHERE M_MEM.committee_role_id in (select sv.uuid from C_S_SYSTEM_VALUE sv, C_S_SYSTEM_TYPE st " +
                "where sv.system_type_id = st.uuid and st.type = 'COMMITTEE_ROLE' and sv.code not in ('1', '2')) AND " +
                "I_SCH.INTERVIEW_DATE >= to_date(:AttendancFromDate, 'DD/MM/YYYY') AND I_SCH.INTERVIEW_DATE <= to_date(:AttendancFromTo, 'DD/MM/YYYY') " +
                "AND I_SCH.IS_CANCEL = 'N' " +
                "AND S_COMM_TYPE.REGISTRATION_TYPE = :reg_type " +
                "AND S_PNL_TYPE.REGISTRATION_TYPE = :reg_type " +
                ") B ON A.MEMBER_ID = B.MEMBER_ID and A.PANEL_TYPE_ID = B.PANEL_TYPE_ID ) summary ON 1 = 1 " +
                "GROUP BY A.APPLN_UUID, A.ENAME, summary.SUMP,  summary.SUMA " +
                "ORDER BY A.ENAME";
            return sql;
        }

        //CRM0057
        public string getApplicationCountCGC(Dictionary<string, object> myDictionary)
        {
            string query1 = "";
            string query2 = "";
            if (!string.IsNullOrEmpty(myDictionary["from_date"].ToString()))
            {
                query1 = " And capp.application_date >= to_date('" + myDictionary["from_date"].ToString() + "', 'DD/MM/YYYY')";
            }
            if (!string.IsNullOrEmpty(myDictionary["from_date"].ToString()))
            {
                query2 = " And capp.application_date <= to_date('" + myDictionary["to_date"].ToString() + "', 'DD/MM/YYYY')";
            }

            string sql = "SELECT slist.code, decode(slist.status, ' ', 'null', slist.status) as status, app_as.AS_count, app_td.TD_count, app_oo.OO_count, app_comp.COMP_count, app_comp2.COMP2_count,:category AS CATEGORY FROM(" +
                "( SELECT t1.code, t2.status, t2.ordering FROM((SELECT scat.code, scat.english_description || ' ' AS cat_desc " +
                "FROM C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp " +
                "WHERE scatgrp.UUID = scat.CATEGORY_GROUP_ID AND scatgrp.REGISTRATION_TYPE = 'CGC' ) t1 " +
                "LEFT JOIN( SELECT svalue.english_description || ' ' AS status, ordering FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue " +
                "WHERE stype.UUID = svalue.SYSTEM_TYPE_ID AND stype.TYPE = 'INTERVIEW_RESULT' ) t2 ON 1 = 1 ) ) slist " +
                "LEFT JOIN( SELECT scat.CODE, status.ENGLISH_DESCRIPTION || ' ' AS status, count(*) AS COMP_count " +
                "FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE status, C_S_SYSTEM_VALUE scatgrp, C_S_SYSTEM_VALUE comp_type, " +
                "C_COMP_PROCESS_MONITOR cpm WHERE capp.CATEGORY_ID = scat.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND comp_type.UUID = capp.COMPANY_TYPE_ID " +
                "AND cpm.MASTER_ID = capp.UUID AND cpm.INTERVIEW_RESULT_ID = status.UUID AND comp_type.CODE = '1' " +
                query1 +
                query2 +
                "GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION ) app_comp ON slist.status = app_comp.status AND slist.code = app_comp.code " +
                "LEFT JOIN ( SELECT scat.CODE, status.ENGLISH_DESCRIPTION || ' ' AS status, count(*) AS COMP2_count " +
                "FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE status, " +
                "C_S_SYSTEM_VALUE scatgrp, C_S_SYSTEM_VALUE comp_type, C_COMP_PROCESS_MONITOR cpm " +
                "WHERE capp.CATEGORY_ID = scat.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND comp_type.UUID = capp.COMPANY_TYPE_ID " +
                "AND cpm.MASTER_ID = capp.UUID AND cpm.INTERVIEW_RESULT_ID = status.UUID AND comp_type.CODE IN('2', '3') " +
                query1 +
                query2 +
                "GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION ) app_comp2 ON slist.status = app_comp2.status AND slist.code = app_comp2.code " +
                "LEFT JOIN( SELECT scat.CODE, status.ENGLISH_DESCRIPTION || ' ' AS status, count(*) AS AS_count " +
                "FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, " +
                "C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE status, C_COMP_PROCESS_MONITOR cpm " +
                "WHERE capp.CATEGORY_ID = scat.UUID " +
                "AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND cinfo.MASTER_ID = capp.UUID AND cinfo.APPLICANT_ROLE_ID = srole.UUID AND cpm.MASTER_ID = capp.UUID " +
                "AND cpm.INTERVIEW_RESULT_ID = status.UUID AND cpm.COMPANY_APPLICANTS_ID = cinfo.UUID AND srole.CODE = 'AS' " +
                "GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION ) app_as ON slist.status = app_as.status AND slist.code = app_as.code " +
                "LEFT JOIN ( SELECT scat.CODE, status.ENGLISH_DESCRIPTION || ' ' AS status, count(*) AS TD_count " +
                "FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, " +
                "C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole,C_S_SYSTEM_VALUE status, C_COMP_PROCESS_MONITOR cpm " +
                "WHERE capp.CATEGORY_ID = scat.UUID AND scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
                "AND cinfo.MASTER_ID = capp.UUID AND cinfo.APPLICANT_ROLE_ID = srole.UUID AND cpm.MASTER_ID = capp.UUID AND cpm.INTERVIEW_RESULT_ID = status.UUID " +
                "AND cpm.COMPANY_APPLICANTS_ID = cinfo.UUID AND srole.CODE = 'TD' " +
                query1 +
                query2 +
                "GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION ) app_td ON slist.status = app_td.status AND slist.code = app_td.code " +
                "LEFT JOIN( SELECT scat.CODE, status.ENGLISH_DESCRIPTION || ' ' AS status, count(*) AS OO_count " +
                "FROM C_comp_application capp, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, " +
                "C_COMP_APPLICANT_INFO cinfo, C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE status, C_COMP_PROCESS_MONITOR cpm WHERE capp.CATEGORY_ID = scat.UUID " +
                "AND scatgrp.UUID = scat.CATEGORY_GROUP_ID AND cinfo.MASTER_ID = capp.UUID AND cinfo.APPLICANT_ROLE_ID = srole.UUID AND cpm.MASTER_ID = capp.UUID " +
                "AND cpm.INTERVIEW_RESULT_ID = status.UUID AND cpm.COMPANY_APPLICANTS_ID = cinfo.UUID AND srole.CODE = 'OO' " +
                query1 +
                query2 +
                "GROUP BY scat.CODE, status.ENGLISH_DESCRIPTION ) app_oo ON slist.status = app_oo.status AND slist.code = app_oo.code) WHERE slist.code = 'GBC' " +
                "ORDER BY slist.code, slist.ordering, slist.status";
            return sql;
        }


        //CRM0059
        public string getStatusAppControlFormIP(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT S_TITLE.ENGLISH_DESCRIPTION AS TITLE,c_propercase(APPLN.SURNAME) AS SURNAME,c_propercase(APPLN.GIVEN_NAME_ON_ID) AS GIVEN_NAME," +
                "I_PMON.AUDIT_TEXT AS AUDIT_TEXT," +
                "I_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
                "I_PMON.RECEIVED_DATE AS RECEIVED_D," +
                "I_PMON.SUPPLE_DOCUMENT_DATE AS SUP_DOC_D," +
                "CASE WHEN :CategoryGroup = 'RI' THEN I_PMON.DUE_DATE " +
                "ELSE " +
                "CASE WHEN I_PMON.SUPPLE_DOCUMENT_DATE IS NOT NULL THEN " +
                "C_GET_IND_PROC_DUEDATE(I_PMON.SUPPLE_DOCUMENT_DATE + 120) " +
                "ELSE " +
                "C_GET_IND_PROC_DUEDATE(I_PMON.RECEIVED_DATE + 120) END " +
                "END as FOUR_MTHS, " +
                "I_PMON.WITHDRAWAL_DATE AS WITHDRAW_D, " +
                "I_PMON.REFERENCE_ASK_DATE AS REF_ASK_D, " +
                "I_PMON.REGISTRATION_ASK_DATE AS REG_ASK_D, " +
                "I_PMON.REGISTRATION_REPLY_DATE AS REG_REPLY_D, " +
                "I_PMON.INTERVIEW_NOTIFY_DATE AS INTRV_NOTIFY_D, " +
                "I_PMON.INTERVIEW_DATE AS INTRV_D, " +
                "I_PMON.RESULT_ACCEPT_DATE AS RESULT_ACCEPT_D, " +
                "I_PMON.RESULT_DEFER_DATE AS RESULT_DEFER_D, " +
                "I_PMON.RESULT_REFUSE_DATE AS RESULT_REFUSE_D, " +
                "I_PMON.GAZETTE_DATE AS GAZETTE_D, " +
                "(I_PMON.REFERENCE_ASK_DATE - I_PMON.RECEIVED_DATE)  AS INI_REPLY, " +
                "(I_PMON.REGISTRATION_REPLY_DATE - I_PMON.RECEIVED_DATE)  AS SI_56, " +
                "(I_PMON.INTERVIEW_DATE - I_PMON.RECEIVED_DATE)  AS INTRV," +
                " CASE WHEN( ((I_PMON.RESULT_ACCEPT_DATE IS NOT NULL) AND(I_PMON.RESULT_DEFER_DATE IS NOT NULL) ) OR " +
                "((I_PMON.RESULT_ACCEPT_DATE IS NOT NULL) AND(I_PMON.RESULT_REFUSE_DATE IS NOT NULL) ) OR " +
                "((I_PMON.RESULT_DEFER_DATE IS NOT NULL) AND(I_PMON.RESULT_REFUSE_DATE IS NOT NULL) ) ) " +
                "THEN '*' ELSE  '' || (coalesce(I_PMON.RESULT_ACCEPT_DATE, I_PMON.RESULT_DEFER_DATE, I_PMON.RESULT_REFUSE_DATE) - I_PMON.RECEIVED_DATE) END AS RESULT_L," +
                "(I_PMON.SUPPLE_DOCUMENT_DATE - I_PMON.RECEIVED_DATE)  AS SUP_INFO,(I_PMON.GAZETTE_DATE - I_PMON.RECEIVED_DATE)  AS GAZETTE, " +
                "S_CAT_GP.CODE AS CAT_GP_CODE, " +
                "S_CAT_GP.ENGLISH_DESCRIPTION AS CAT_GP_ENG, " +
                "CASE when CAT_CODE.CODE = 'API' or CAT_CODE.CODE = 'AP(A)' OR CAT_CODE.CODE = 'RI(A)' then 'A' " +
                "when CAT_CODE.CODE = 'APII' or CAT_CODE.CODE = 'AP(E)' OR CAT_CODE.CODE = 'RI(E)' then 'E' " +
                " when CAT_CODE.CODE = 'APIII' or CAT_CODE.CODE = 'AP(S)' OR CAT_CODE.CODE = 'RI(S)' then 'S' ELSE 'E' END AS SHORT_CODE, " +
                "CASE WHEN :CategoryGroup = 'RI' THEN " +
                "TRUNC(I_PMON.DUE_DATE - I_PMON.RECEIVED_DATE) ELSE " +
                "CASE WHEN I_PMON.SUPPLE_DOCUMENT_DATE IS NOT NULL THEN TRUNC(C_GET_IND_PROC_DUEDATE(I_PMON.SUPPLE_DOCUMENT_DATE + 120) - I_PMON.RECEIVED_DATE) " +
                "ELSE TRUNC(C_GET_IND_PROC_DUEDATE(I_PMON.RECEIVED_DATE + 120) - I_PMON.RECEIVED_DATE) END " +
                "END as PROC_DATE, " +
                "(SELECT listagg(scatd.ENGLISH_DESCRIPTION) FROM C_IND_QUALIFICATION iq, C_IND_QUALIFICATION_DETAIL iqd, C_S_CATEGORY_CODE_DETAIL scatd " +
                "WHERE iqd.IND_QUALIFICATION_ID = iq.UUID AND iqd.S_CATEGORY_CODE_DETAIL_ID = scatd.UUID AND iq.MASTER_ID = I_APPL.UUID) AS disciplines " +
                "FROM C_APPLICANT APPLN " +
                "INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID " +
                "LEFT JOIN C_IND_PROCESS_MONITOR I_PMON ON I_APPL.UUID = I_PMON.MASTER_ID " +
                " LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON APPLN.TITLE_ID = S_TITLE.UUID " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_CAT_GP ON I_PMON.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                " LEFT JOIN C_S_CATEGORY_CODE CAT_CODE ON I_PMON.CATEGORY_ID = CAT_CODE.UUID " +
                "AND S_CAT_GP.UUID = CAT_CODE.CATEGORY_GROUP_ID " +
                "WHERE " +
                "S_CAT_GP.REGISTRATION_TYPE = :reg_type ");
            if (!string.IsNullOrEmpty(myDictionary["CategoryGroup"].ToString()))
                sql.Append(" And S_CAT_GP.CODE =:CategoryGroup ");
            //sql.Append("And rownum < 2  order by I_PMON.RECEIVED_DATE,c_propercase(APPLN.SURNAME), c_propercase(APPLN.GIVEN_NAME_ON_ID) ");
            sql.Append("order by I_PMON.RECEIVED_DATE,c_propercase(APPLN.SURNAME), c_propercase(APPLN.GIVEN_NAME_ON_ID) ");

            //string sql = "SELECT S_TITLE.ENGLISH_DESCRIPTION AS TITLE,c_propercase(APPLN.SURNAME) AS SURNAME,c_propercase(APPLN.GIVEN_NAME_ON_ID) AS GIVEN_NAME," +
            //    "I_PMON.AUDIT_TEXT AS AUDIT_TEXT," +
            //    "I_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
            //    "I_PMON.RECEIVED_DATE AS RECEIVED_D," +
            //    "I_PMON.SUPPLE_DOCUMENT_DATE AS SUP_DOC_D," +
            //    "CASE WHEN :CategoryGroup = 'RI' THEN I_PMON.DUE_DATE " +
            //    "ELSE " +
            //    "CASE WHEN I_PMON.SUPPLE_DOCUMENT_DATE IS NOT NULL THEN " +
            //    "C_GET_IND_PROC_DUEDATE(I_PMON.SUPPLE_DOCUMENT_DATE + 120) " +
            //    "ELSE " +
            //    "C_GET_IND_PROC_DUEDATE(I_PMON.RECEIVED_DATE + 120) END " +
            //    "END as FOUR_MTHS, " +
            //    "I_PMON.WITHDRAWAL_DATE AS WITHDRAW_D, " +
            //    "I_PMON.REFERENCE_ASK_DATE AS REF_ASK_D, " +
            //    "I_PMON.REGISTRATION_ASK_DATE AS REG_ASK_D, " +
            //    "I_PMON.REGISTRATION_REPLY_DATE AS REG_REPLY_D, " +
            //    "I_PMON.INTERVIEW_NOTIFY_DATE AS INTRV_NOTIFY_D, " +
            //    "I_PMON.INTERVIEW_DATE AS INTRV_D, " +
            //    "I_PMON.RESULT_ACCEPT_DATE AS RESULT_ACCEPT_D, " +
            //    "I_PMON.RESULT_DEFER_DATE AS RESULT_DEFER_D, " +
            //    "I_PMON.RESULT_REFUSE_DATE AS RESULT_REFUSE_D, " +
            //    "I_PMON.GAZETTE_DATE AS GAZETTE_D, " +
            //    "(I_PMON.REFERENCE_ASK_DATE - I_PMON.RECEIVED_DATE)  AS INI_REPLY, " +
            //    "(I_PMON.REGISTRATION_REPLY_DATE - I_PMON.RECEIVED_DATE)  AS SI_56, " +
            //    "(I_PMON.INTERVIEW_DATE - I_PMON.RECEIVED_DATE)  AS INTRV," +
            //    " CASE WHEN( ((I_PMON.RESULT_ACCEPT_DATE IS NOT NULL) AND(I_PMON.RESULT_DEFER_DATE IS NOT NULL) ) OR " +
            //    "((I_PMON.RESULT_ACCEPT_DATE IS NOT NULL) AND(I_PMON.RESULT_REFUSE_DATE IS NOT NULL) ) OR " +
            //    "((I_PMON.RESULT_DEFER_DATE IS NOT NULL) AND(I_PMON.RESULT_REFUSE_DATE IS NOT NULL) ) ) " +
            //    "THEN '*' ELSE  '' || (coalesce(I_PMON.RESULT_ACCEPT_DATE, I_PMON.RESULT_DEFER_DATE, I_PMON.RESULT_REFUSE_DATE) - I_PMON.RECEIVED_DATE) END AS RESULT_L," +
            //    "(I_PMON.SUPPLE_DOCUMENT_DATE - I_PMON.RECEIVED_DATE)  AS SUP_INFO,(I_PMON.GAZETTE_DATE - I_PMON.RECEIVED_DATE)  AS GAZETTE, " +
            //    "S_CAT_GP.CODE AS CAT_GP_CODE, " +
            //    "S_CAT_GP.ENGLISH_DESCRIPTION AS CAT_GP_ENG, " +
            //    "CASE when CAT_CODE.CODE = 'API' or CAT_CODE.CODE = 'AP(A)' OR CAT_CODE.CODE = 'RI(A)' then 'A' " +
            //    "when CAT_CODE.CODE = 'APII' or CAT_CODE.CODE = 'AP(E)' OR CAT_CODE.CODE = 'RI(E)' then 'E' " +
            //    " when CAT_CODE.CODE = 'APIII' or CAT_CODE.CODE = 'AP(S)' OR CAT_CODE.CODE = 'RI(S)' then 'S' ELSE 'E' END AS SHORT_CODE, " +
            //    "CASE WHEN :CategoryGroup = 'RI' THEN " +
            //    "TRUNC(I_PMON.DUE_DATE - I_PMON.RECEIVED_DATE) ELSE " +
            //    "CASE WHEN I_PMON.SUPPLE_DOCUMENT_DATE IS NOT NULL THEN TRUNC(C_GET_IND_PROC_DUEDATE(I_PMON.SUPPLE_DOCUMENT_DATE + 120) - I_PMON.RECEIVED_DATE) " +
            //    "ELSE TRUNC(C_GET_IND_PROC_DUEDATE(I_PMON.RECEIVED_DATE + 120) - I_PMON.RECEIVED_DATE) END " +
            //    "END as PROC_DATE, " +
            //    "(SELECT scatd.ENGLISH_DESCRIPTION FROM C_IND_QUALIFICATION iq, C_IND_QUALIFICATION_DETAIL iqd, C_S_CATEGORY_CODE_DETAIL scatd " +
            //    "WHERE iqd.IND_QUALIFICATION_ID = iq.UUID AND iqd.S_CATEGORY_CODE_DETAIL_ID = scatd.UUID AND iq.MASTER_ID = I_APPL.UUID) AS disciplines " +
            //    "FROM C_APPLICANT APPLN " +
            //    "INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID " +
            //    "LEFT JOIN C_IND_PROCESS_MONITOR I_PMON ON I_APPL.UUID = I_PMON.MASTER_ID " +
            //    " LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON APPLN.TITLE_ID = S_TITLE.UUID " +
            //    "LEFT JOIN C_S_SYSTEM_VALUE S_CAT_GP ON I_PMON.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
            //    " LEFT JOIN C_S_CATEGORY_CODE CAT_CODE ON I_PMON.CATEGORY_ID = CAT_CODE.UUID " +
            //    "AND S_CAT_GP.UUID = CAT_CODE.CATEGORY_GROUP_ID " +
            //    "WHERE " +
            //    "S_CAT_GP.REGISTRATION_TYPE = :reg_type " +
            //    "AND S_CAT_GP.CODE =:CategoryGroup " +
            //    "order by I_PMON.RECEIVED_DATE,c_propercase(APPLN.SURNAME), c_propercase(APPLN.GIVEN_NAME_ON_ID)";
            return sql.ToString();
        }

        //CRM0060
        public string getStatusAppControlFormIP_RI()
        {
            string sql = "SELECT S_TITLE.ENGLISH_DESCRIPTION AS TITLE,c_propercase(APPLN.SURNAME) AS SURNAME,c_propercase(APPLN.GIVEN_NAME_ON_ID) AS GIVEN_NAME," +
                "I_PMON.AUDIT_TEXT AS AUDIT_TEXT," +
                "I_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
                "I_PMON.RECEIVED_DATE AS RECEIVED_D," +
                "I_PMON.SUPPLE_DOCUMENT_DATE AS SUP_DOC_D," +
                "CASE WHEN :cat_gp = 'RI' THEN I_PMON.DUE_DATE " +
                "ELSE " +
                "CASE WHEN I_PMON.SUPPLE_DOCUMENT_DATE IS NOT NULL THEN " +
                "GET_IND_PROC_DUEDATE(I_PMON.SUPPLE_DOCUMENT_DATE + 120) " +
                "ELSE " +
                "GET_IND_PROC_DUEDATE(I_PMON.RECEIVED_DATE + 120) END " +
                "END as FOUR_MTHS, " +
                "I_PMON.WITHDRAWAL_DATE AS WITHDRAW_D, " +
                "I_PMON.REFERENCE_ASK_DATE AS REF_ASK_D, " +
                "I_PMON.REGISTRATION_ASK_DATE AS REG_ASK_D, " +
                "I_PMON.REGISTRATION_REPLY_DATE AS REG_REPLY_D, " +
                "I_PMON.INTERVIEW_NOTIFY_DATE AS INTRV_NOTIFY_D, " +
                "I_PMON.INTERVIEW_DATE AS INTRV_D, " +
                "I_PMON.RESULT_ACCEPT_DATE AS RESULT_ACCEPT_D, " +
                "I_PMON.RESULT_DEFER_DATE AS RESULT_DEFER_D, " +
                "I_PMON.RESULT_REFUSE_DATE AS RESULT_REFUSE_D, " +
                "I_PMON.GAZETTE_DATE AS GAZETTE_D, " +
                "(I_PMON.REFERENCE_ASK_DATE - I_PMON.RECEIVED_DATE)  AS INI_REPLY, " +
                "(I_PMON.REGISTRATION_REPLY_DATE - I_PMON.RECEIVED_DATE)  AS SI_56, " +
                "(I_PMON.INTERVIEW_DATE - I_PMON.RECEIVED_DATE)  AS INTRV," +
                " CASE WHEN( ((I_PMON.RESULT_ACCEPT_DATE IS NOT NULL) AND(I_PMON.RESULT_DEFER_DATE IS NOT NULL) ) OR " +
                "((I_PMON.RESULT_ACCEPT_DATE IS NOT NULL) AND(I_PMON.RESULT_REFUSE_DATE IS NOT NULL) ) OR " +
                "((I_PMON.RESULT_DEFER_DATE IS NOT NULL) AND(I_PMON.RESULT_REFUSE_DATE IS NOT NULL) ) ) " +
                "THEN '*' ELSE  '' || (coalesce(I_PMON.RESULT_ACCEPT_DATE, I_PMON.RESULT_DEFER_DATE, I_PMON.RESULT_REFUSE_DATE) - I_PMON.RECEIVED_DATE) END AS RESULT_L," +
                "(I_PMON.SUPPLE_DOCUMENT_DATE - I_PMON.RECEIVED_DATE)  AS SUP_INFO,(I_PMON.GAZETTE_DATE - I_PMON.RECEIVED_DATE)  AS GAZETTE, " +
                "S_CAT_GP.CODE AS CAT_GP_CODE, " +
                "S_CAT_GP.ENGLISH_DESCRIPTION AS CAT_GP_ENG, " +
                "CASE when CAT_CODE.CODE = 'API' or CAT_CODE.CODE = 'AP(A)' OR CAT_CODE.CODE = 'RI(A)' then 'A' " +
                "when CAT_CODE.CODE = 'APII' or CAT_CODE.CODE = 'AP(E)' OR CAT_CODE.CODE = 'RI(E)' then 'E' " +
                " when CAT_CODE.CODE = 'APIII' or CAT_CODE.CODE = 'AP(S)' OR CAT_CODE.CODE = 'RI(S)' then 'S' ELSE 'E' END AS SHORT_CODE, " +
                "CASE WHEN :cat_gp = 'RI' THEN " +
                "TRUNC(I_PMON.DUE_DATE - I_PMON.RECEIVED_DATE) ELSE " +
                "CASE WHEN I_PMON.SUPPLE_DOCUMENT_DATE IS NOT NULL THEN TRUNC(GET_IND_PROC_DUEDATE(I_PMON.SUPPLE_DOCUMENT_DATE + 120) - I_PMON.RECEIVED_DATE) " +
                "ELSE TRUNC(GET_IND_PROC_DUEDATE(I_PMON.RECEIVED_DATE + 120) - I_PMON.RECEIVED_DATE) END " +
                "END as PROC_DATE, " +
                "(SELECT scatd.ENGLISH_DESCRIPTION FROM C_IND_QUALIFICATION iq, C_IND_QUALIFICATION_DETAIL iqd, C_S_CATEGORY_CODE_DETAIL scatd " +
                "WHERE iqd.IND_QUALIFICATION_ID = iq.UUID AND iqd.S_CATEGORY_CODE_DETAIL_ID = scatd.UUID AND iq.MASTER_ID = I_APPL.UUID) AS disciplines " +
                "FROM C_APPLICANT APPLN " +
                "INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID " +
                "LEFT JOIN C_IND_PROCESS_MONITOR I_PMON ON I_APPL.UUID = I_PMON.MASTER_ID " +
                " LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON APPLN.TITLE_ID = S_TITLE.UUID " +
                "LEFT JOIN C_S_SYSTEM_VALUE S_CAT_GP ON I_PMON.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                " LEFT JOIN C_S_CATEGORY_CODE CAT_CODE ON I_PMON.CATEGORY_ID = CAT_CODE.UUID " +
                "AND S_CAT_GP.UUID = CAT_CODE.CATEGORY_GROUP_ID " +
                "WHERE " +
                "S_CAT_GP.REGISTRATION_TYPE = :reg_type AND S_CAT_GP.CODE =:cat_gp " +
                "order by I_PMON.RECEIVED_DATE,c_propercase(APPLN.SURNAME), c_propercase(APPLN.GIVEN_NAME_ON_ID)";
            return sql;
        }

        //CRM0061
        private string getNoOfRegIP(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sqlWhere = new StringBuilder("");
            string DateFrom = (from d in myDictionary where d.Key == "DateFrom" select d.Value).FirstOrDefault().ToString();
            string DateTo = (from d in myDictionary where d.Key == "DateTo" select d.Value).FirstOrDefault().ToString();
            foreach (var item in myDictionary)
            {
                if (item.Key == "date_type")
                {
                    switch (item.Value)
                    {
                        case "application_date":
                            if (!string.IsNullOrEmpty(DateFrom))
                                sqlWhere.Append(" And icer.application_date >= to_date(:DateFrom,'dd/MM/yyyy') ");
                            if (!string.IsNullOrEmpty(DateTo))
                                sqlWhere.Append(" And icer.application_date <= to_date(:DateTo,'dd/MM/yyyy') ");
                            break;
                        case "registration_date":
                            if (!string.IsNullOrEmpty(DateFrom))
                                sqlWhere.Append(" And icer.registration_date >= to_date(:DateFrom,'dd/MM/yyyy') ");
                            if (!string.IsNullOrEmpty(DateTo))
                                sqlWhere.Append(" And icer.registration_date <= to_date(:DateTo,'dd/MM/yyyy') ");
                            break;
                        case "gazette_date":
                            if (!string.IsNullOrEmpty(DateFrom))
                                sqlWhere.Append(" And icer.gazette_date >= to_date(:DateFrom,'dd/MM/yyyy') ");
                            if (!string.IsNullOrEmpty(DateTo))
                                sqlWhere.Append(" And icer.gazette_date <= to_date(:DateTo,'dd/MM/yyyy') ");
                            break;
                        case "expiry_date":
                            if (!string.IsNullOrEmpty(DateFrom))
                                sqlWhere.Append(" And icer.expiry_date >= to_date(:DateFrom,'dd/MM/yyyy') ");
                            if (!string.IsNullOrEmpty(DateTo))
                                sqlWhere.Append(" And icer.expiry_date <= to_date(:DateTo,'dd/MM/yyyy') ");
                            break;
                        case "removal_date":
                            if (!string.IsNullOrEmpty(DateFrom))
                                sqlWhere.Append(" And icer.removal_date >= to_date(:DateFrom,'dd/MM/yyyy') ");
                            if (!string.IsNullOrEmpty(DateTo))
                                sqlWhere.Append(" And icer.removal_date <= to_date(:DateTo,'dd/MM/yyyy') ");
                            break;
                        case "retention_date":
                            if (!string.IsNullOrEmpty(DateFrom))
                                sqlWhere.Append(" And icer.retention_date >= to_date(:DateFrom,'dd/MM/yyyy') ");
                            if (!string.IsNullOrEmpty(DateTo))
                                sqlWhere.Append(" And icer.retention_date <= to_date(:DateTo,'dd/MM/yyyy') ");
                            break;
                        case "restore_date":
                            if (!string.IsNullOrEmpty(DateFrom))
                                sqlWhere.Append(" And icer.restore_date >= to_date(:DateFrom,'dd/MM/yyyy') ");
                            if (!string.IsNullOrEmpty(DateTo))
                                sqlWhere.Append(" And icer.restore_date <= to_date(:DateTo,'dd/MM/yyyy') ");
                            break;
                        case "approval_date":
                            if (!string.IsNullOrEmpty(DateFrom))
                                sqlWhere.Append(" And icer.approval_date >= to_date(:DateFrom,'dd/MM/yyyy') ");
                            if (!string.IsNullOrEmpty(DateTo))
                                sqlWhere.Append(" And icer.approval_date <= to_date(:DateTo,'dd/MM/yyyy') ");
                            break;
                    }
                }
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT status, sum(AP_A) AS AP_A, sum(AP_E) AS AP_E, sum(AP_S) AS AP_S, sum(AP_I) AS AP_I, 
                        sum(AP_II) AS AP_II, sum(AP_III) AS AP_III, sum(RSE) AS RSE, sum(RGE) AS RGE, sum(RI_A) AS RI_A, sum(RI_E) AS RI_E, sum(RI_S) AS RI_S 
                        FROM ( 
                        SELECT scat.code, status.ENGLISH_DESCRIPTION AS status, 
                        CASE WHEN scat.CODE = 'AP(A)' THEN count(*) ELSE 0 END AS AP_A, 
                        CASE WHEN scat.CODE = 'AP(E)' THEN count(*) ELSE 0 END AS AP_E,  
                        CASE WHEN scat.CODE = 'AP(S)' THEN count(*) ELSE 0 END AS AP_S, 
                        CASE WHEN scat.CODE = 'API' THEN count(*) ELSE 0 END AS AP_I, 
                        CASE WHEN scat.CODE = 'APII' THEN count(*) ELSE 0 END AS AP_II, 
                        CASE WHEN scat.CODE = 'APIII' THEN count(*) ELSE 0 END AS AP_III, 
                        CASE WHEN scat.CODE = 'RSE' THEN count(*) ELSE 0 END AS RSE, 
                        CASE WHEN scat.CODE = 'RGE' THEN count(*) ELSE 0 END AS RGE, 
                        CASE WHEN scat.CODE = 'RI(A)' THEN count(*) ELSE 0 END AS RI_A, 
                        CASE WHEN scat.CODE = 'RI(E)' THEN count(*) ELSE 0 END AS RI_E, 
                        CASE WHEN scat.CODE = 'RI(S)' THEN count(*) ELSE 0 END AS RI_S 
                        FROM C_IND_CERTIFICATE icer, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, C_S_SYSTEM_VALUE status 
                        WHERE icer.CATEGORY_ID = scat.UUID 
                        AND scat.CATEGORY_GROUP_ID = scatgrp.UUID 
                        AND icer.APPLICATION_STATUS_ID = status.UUID 
                        and scatgrp.code in ('AP', 'RSE', 'RGE', 'RI') 
                        " + sqlWhere.ToString() + @"
                        GROUP BY scat.code, status.ENGLISH_DESCRIPTION  
                        UNION ALL 
                        SELECT '' AS code, svalue.english_description AS status, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue 
                        WHERE stype.UUID = svalue.SYSTEM_TYPE_ID AND stype.TYPE = 'APPLICANT_STATUS') 
                        GROUP BY status ");


            //string sql = "SELECT status, sum(AP_A) AS AP_A, sum(AP_E) AS AP_E, sum(AP_S) AS AP_S, sum(AP_I) AS AP_I, sum(AP_II) AS AP_II, sum(AP_III) AS AP_III, sum(RSE) AS RSE, sum(RGE) AS RGE, sum(RI_A) AS RI_A, sum(RI_E) AS RI_E, sum(RI_S) AS RI_S FROM ( " +
            //            "SELECT scat.code, status.ENGLISH_DESCRIPTION AS status, " +
            //            "CASE WHEN scat.CODE = 'AP(A)' THEN count(*) ELSE 0 END AS AP_A, " +
            //            "CASE WHEN scat.CODE = 'AP(E)' THEN count(*) ELSE 0 END AS AP_E, " +
            //            "CASE WHEN scat.CODE = 'AP(S)' THEN count(*) ELSE 0 END AS AP_S, " +
            //            "CASE WHEN scat.CODE = 'API' THEN count(*) ELSE 0 END AS AP_I, " +
            //            "CASE WHEN scat.CODE = 'APII' THEN count(*) ELSE 0 END AS AP_II, " +
            //            "CASE WHEN scat.CODE = 'APIII' THEN count(*) ELSE 0 END AS AP_III, " +
            //            "CASE WHEN scat.CODE = 'RSE' THEN count(*) ELSE 0 END AS RSE, " +
            //            "CASE WHEN scat.CODE = 'RGE' THEN count(*) ELSE 0 END AS RGE, " +
            //            "CASE WHEN scat.CODE = 'RI(A)' THEN count(*) ELSE 0 END AS RI_A, " +
            //            "CASE WHEN scat.CODE = 'RI(E)' THEN count(*) ELSE 0 END AS RI_E, " +
            //            "CASE WHEN scat.CODE = 'RI(S)' THEN count(*) ELSE 0 END AS RI_S " +
            //            "FROM C_IND_CERTIFICATE icer, C_S_CATEGORY_CODE scat, C_S_SYSTEM_VALUE scatgrp, C_S_SYSTEM_VALUE status " +
            //            "WHERE icer.CATEGORY_ID = scat.UUID " +
            //            "AND scat.CATEGORY_GROUP_ID = scatgrp.UUID " +
            //            "AND icer.APPLICATION_STATUS_ID = status.UUID " +
            //            "and scatgrp.code in ('AP', 'RSE', 'RGE', 'RI') " +
            //            "GROUP BY scat.code, status.ENGLISH_DESCRIPTION " +

            //            "UNION ALL " +

            //            "SELECT '' AS code, svalue.english_description AS status, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 FROM C_S_SYSTEM_TYPE stype, C_S_SYSTEM_VALUE svalue " +
            //            "WHERE stype.UUID = svalue.SYSTEM_TYPE_ID AND stype.TYPE = 'APPLICANT_STATUS') " +
            //            "GROUP BY status ";
            return sql.ToString();
        }

        //CRM0062
        public string getRPT0010bCGC(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT " +
                "c_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME, " +
                "to_char(C_APPL.EXPIRY_DATE,'dd/mm/yyyy') AS EXP_D," +
                "C_APPL.CERTIFICATION_NO AS CERT_NO," +
                "S_CAT.ENGLISH_DESCRIPTION AS ENG_SUB " +
                "FROM " +
                "C_S_CATEGORY_CODE S_CAT " +
                "INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID " +
                "WHERE S_CAT.REGISTRATION_TYPE = :reg_type ");

            if (!string.IsNullOrEmpty(myDictionary["AGFrDate"].ToString()))
                sql.Append(" AND GAZETTE_DATE >= to_date(:AGFrDate, 'DD/MM/YYYY') ");
            if (!string.IsNullOrEmpty(myDictionary["AGToDate"].ToString()))
                sql.Append(" AND GAZETTE_DATE <= to_date(:AGToDate, 'DD/MM/YYYY') ");
            if (!string.IsNullOrEmpty(myDictionary["AnnualGazetteCtrUUID"].ToString()))
                sql.Append(" And S_CAT.CODE = :AnnualGazetteCtrUUID ");
            sql.Append(" ORDER BY ENG_SUB ASC, C_ENAME ASC, CERT_NO ASC,EXP_D ASC ");



            //string sql = "SELECT " +
            //    "c_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME, " +
            //    "C_APPL.EXPIRY_DATE AS EXP_D," +
            //    "C_APPL.CERTIFICATION_NO AS CERT_NO," +
            //    "S_CAT.ENGLISH_DESCRIPTION AS ENG_SUB " +
            //    "FROM " +
            //    "C_S_CATEGORY_CODE S_CAT " +
            //    "INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID " +
            //    "WHERE S_CAT.REGISTRATION_TYPE = :reg_type" +
            //    " AND GAZETTE_DATE >= to_date(:AGFrDate, 'DD/MM/YYYY') " +
            //    "AND GAZETTE_DATE <= to_date(:AGToDate, 'DD/MM/YYYY') " +
            //    "AND S_CAT.CODE = :AnnualGazetteCtrUUID " +
            //    "ORDER BY ENG_SUB ASC, C_ENAME ASC, CERT_NO ASC,EXP_D ASC";
            return sql.ToString();
        }

        //CRM0063
        public string getRPT0010bA2CGC(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT " +
                "CASE WHEN :acting = 1 then S_AUTH.ENGLISH_ACTION_NAME || ' ' || S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_NAME || ' ' || S_AUTH.ENGLISH_RANK END AS AUTH_ENAME, " +
                "CASE WHEN :acting = 1 then S_AUTH.CHINESE_ACTION_RANK || ' ' || S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_RANK || ' ' || S_AUTH.CHINESE_NAME END AS AUTH_CNAME, " +
                "to_date(:Gaz_date,'dd/mm/yyyy') AS GAZ_DATE, " +
                ":reg_type AS REC_TYPE " +
                "FROM C_S_AUTHORITY S_AUTH Where 1=1 ");
            if (!string.IsNullOrEmpty(myDictionary["NameOfSignture"].ToString()))
                sql.Append(" And S_AUTH.UUID = :NameOfSignture ");

            //string sql = "SELECT " +
            //    "CASE WHEN :acting = 1 then S_AUTH.ENGLISH_ACTION_NAME || ' ' || S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_NAME || ' ' || S_AUTH.ENGLISH_RANK END AS AUTH_ENAME, " +
            //    "CASE WHEN :acting = 1 then S_AUTH.CHINESE_ACTION_RANK || ' ' || S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_RANK || ' ' || S_AUTH.CHINESE_NAME END AS AUTH_CNAME, " +
            //    "to_date(:Gaz_date,'dd/mm/yyyy') AS GAZ_DATE, " +
            //    ":rec_type AS REC_TYPE " +
            //    "FROM C_S_AUTHORITY S_AUTH " +
            //    " WHERE S_AUTH.UUID = :auth_uuid";
            return sql.ToString();
        }

        //CRM0064
        private string getAnnualGazetteENGCGC(Dictionary<string, Object> myDictionary)
        {
            myDictionary["HeaderDesc"] = myDictionary["HeaderDesc"].ToString().Replace("[DD][MM][YYYY]", " ");
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT " +
                "upper(C_APPL.ENGLISH_COMPANY_NAME) AS ENAME," +
                "C_APPL.CHINESE_COMPANY_NAME AS CNAME," +
                "to_char(C_APPL.EXPIRY_DATE,'dd.mm.yyyy') AS EXP_D," +
                "C_APPL.CERTIFICATION_NO AS CERT_NO," +
                "S_CAT.CHINESE_DESCRIPTION AS CHN_SUB," +
                "CASE WHEN S_CAT.CODE = 'MWC' THEN UPPER(S_CAT.ENGLISH_DESCRIPTION) ELSE S_CAT.ENGLISH_DESCRIPTION END AS ENG_SUB," +
                "S_CAT.CODE AS CAT_CODE," +
                "(select MIN(CHINESE_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_NAME," +
                "CASE WHEN to_date(:asAtDate, 'dd/mm/yyyy') > C_APPL.expiry_date " +
                "AND add_months(C_APPL.retention_application_date, 24) > trunc(to_date(:asAtDate, 'dd/mm/yyyy')) THEN '@' ELSE '' END AS FLAG," +
                "'" + myDictionary["HeaderDesc"].ToString() + "' AS HEADER," +
                "to_date(:asAtDate,'dd/mm/yyyy')  AS AS_AT_DATE " +
                "FROM C_S_CATEGORY_CODE S_CAT " +
                "INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE S_CAT.REGISTRATION_TYPE =:reg_type " +
                "AND S_CAT_GP.REGISTRATION_TYPE =:reg_type " +
                "AND S_CAT_GP.Code = :cat_gp2 " +
                "AND C_APPL.GAZETTE_DATE >= to_date(:ReportGazDateFrom, 'dd/mm/yyyy') " +
                "AND C_APPL.GAZETTE_DATE <= to_date(:ReportGazDateTo, 'dd/mm/yyyy') " +
                "AND C_APPL.CERTIFICATION_NO IS NOT NULL AND(" +
                "(C_APPL.EXPIRY_DATE IS NOT NULL and C_APPL.EXPIRY_DATE >= to_date(:asAtDate, 'dd/mm/yyyy')) or ((C_APPL.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and " +
                "(C_APPL.EXPIRY_DATE < to_date(:asAtDate, 'dd/mm/yyyy'))) ) )  AND((C_APPL.REMOVAL_DATE IS NULL) OR " +
                "(C_APPL.REMOVAL_DATE > to_date(:asAtDate, 'dd/mm/yyyy'))) " +
                "AND S_CAT_GP.CODE IN('GB','SC') " +
                "ORDER BY ENG_SUB ASC,ENAME ASC ");

            //string sql = "SELECT " +
            //    "upper(C_APPL.ENGLISH_COMPANY_NAME) AS ENAME," +
            //    "C_APPL.CHINESE_COMPANY_NAME AS CNAME," +
            //    "to_char(C_APPL.EXPIRY_DATE,'dd.mm.yyyy') AS EXP_D," +
            //    "C_APPL.CERTIFICATION_NO AS CERT_NO," +
            //    "S_CAT.CHINESE_DESCRIPTION AS CHN_SUB," +
            //    "CASE WHEN S_CAT.CODE = 'MWC' THEN UPPER(S_CAT.ENGLISH_DESCRIPTION) ELSE S_CAT.ENGLISH_DESCRIPTION END AS ENG_SUB," +
            //    "S_CAT.CODE AS CAT_CODE," +
            //    "(select MIN(CHINESE_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_NAME," +
            //    "CASE WHEN to_date(:as_at_date, 'dd/mm/yyyy') > C_APPL.expiry_date " +
            //    "AND add_months(C_APPL.retention_application_date, 24) > trunc(to_date(:as_at_date, 'dd/mm/yyyy')) THEN '@' ELSE '' END AS FLAG," +
            //    "to_date(:as_at_date,'dd/mm/yyyy') AS AS_AT_DATE " +
            //    "FROM C_S_CATEGORY_CODE S_CAT " +
            //    "INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
            //    "WHERE S_CAT.REGISTRATION_TYPE =:reg_type " +
            //    "AND S_CAT_GP.REGISTRATION_TYPE =:reg_type " +
            //    "AND C_APPL.GAZETTE_DATE >= to_date(:gaz_fr_date, 'dd/mm/yyyy') " +
            //    "AND C_APPL.GAZETTE_DATE <= to_date(:gaz_to_date, 'dd/mm/yyyy') " +
            //    "AND C_APPL.CERTIFICATION_NO IS NOT NULL AND(" +
            //    "(C_APPL.EXPIRY_DATE IS NOT NULL and C_APPL.EXPIRY_DATE >= to_date(:as_at_date, 'dd/mm/yyyy')) or ((C_APPL.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and " +
            //    "(C_APPL.EXPIRY_DATE < to_date(:as_at_date, 'dd/mm/yyyy'))) ) )  AND((C_APPL.REMOVAL_DATE IS NULL) OR " +
            //    "(C_APPL.REMOVAL_DATE > to_date(:as_at_date, 'dd/mm/yyyy'))) " +
            //    "AND S_CAT_GP.CODE IN('GB','SC') " +
            //    "ORDER BY ENG_SUB ASC,ENAME ASC ";
            return sql.ToString();
        }
        //CRM0064 Empty
        private string getAnnualGazetteENGCGCEmptySql(Dictionary<string, Object> myDictionary)
        {

            string sql = @"SELECT " +
                 "null AS ENAME," +
                "null AS CNAME," +
                "null AS EXP_D," +
                "null AS CERT_NO," +
                "null AS CHN_SUB," +
                "null AS ENG_SUB," +
                "null AS CAT_CODE," +
                "null as AUTH_NAME," +
                "null AS FLAG," +
                "'" + myDictionary["HeaderDesc"].ToString() + "' AS HEADER," +
                "to_date('" + myDictionary["asAtDate"].ToString() + "','dd/mm/yyyy')  AS AS_AT_DATE from dual";
            return sql;
        }
        //CRM0065
        private string getAnnualGazetteCHNIPSql(Dictionary<string, Object> myDictionary)
        {
            string header1 = "";
            string header2 = "";
            if (myDictionary.Keys.Contains("PAHeaderChnDesc"))
            {
                header1 = myDictionary["PAHeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["PAHeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[0]
                            : myDictionary["PAHeaderChnDesc"].ToString();
                header2 = myDictionary["PAHeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["PAHeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[1]
                            : "";
            }
            else if (myDictionary.Keys.Contains("MWIHeaderChnDesc"))
            {
                header1 = myDictionary["MWIHeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["MWIHeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[0]
                            : myDictionary["MWIHeaderChnDesc"].ToString();
                header2 = myDictionary["MWIHeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["MWIHeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[1]
                            : "";
            }
            else
            {
                header1 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[0]
                            : myDictionary["HeaderChnDesc"].ToString();
                header2 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[1]
                            : "";
            }



            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT " +
                "upper(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME, " +
                "APPLN.CHINESE_NAME AS CNAME," +
                "I_CERT.REGISTRATION_DATE AS REG_D," +
                "I_CERT.CERTIFICATION_NO AS CERT_NO," +
                "S_CAT.CODE AS CAT_CODE," +
                "S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION AS ENG_SUB," +
                "S_CAT.CHINESE_SUB_TITLE_DESCRIPTION AS CHN_SUB," +
                "(select MIN(ENGLISH_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_NAME," +
                "(select MIN(ENGLISH_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_ENAME, " +
                "CASE WHEN to_date(:asAtDate2, 'dd/mm/yyyy') > I_CERT.expiry_date " +
                "AND add_months(I_CERT.retention_application_date, 24) > trunc(to_date(:asAtDate2, 'dd/mm/yyyy')) THEN '@' ELSE '' END AS FLAG," +
                "to_date(:asAtDate2, 'dd/mm/yyyy') AS AS_AT_DATE," +
                "'" + header1 + "' AS HEADER1," +
                "'" + header2 + "' AS HEADER2," +
                "to_date(:ReportChnGazDateFrom,'dd/mm/yyyy') AS GAZ_FR_DATE," +
                "to_date(:ReportChnGazDateTo,'dd/mm/yyyy') AS GAZ_TO_DATE " +
                "FROM C_APPLICANT APPLN " +
                "INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID " +
                "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE S_CAT.REGISTRATION_TYPE = :reg_type " +
                "AND S_CAT_GP.REGISTRATION_TYPE = :reg_type " +
                "AND I_CERT.CERTIFICATION_NO IS NOT NULL " +
                "AND " +
                "((I_CERT.EXPIRY_DATE IS NOT NULL and I_CERT.EXPIRY_DATE >= to_date(:asAtDate2, 'dd/mm/yyyy')) or " +
                "(I_CERT.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(I_CERT.EXPIRY_DATE < to_date(:asAtDate2, 'dd/mm/yyyy'))) )  " +
                "AND " +
                "((I_CERT.REMOVAL_DATE IS NULL) OR(I_CERT.REMOVAL_DATE > to_date(:asAtDate2, 'dd/mm/yyyy')))");
            if (!string.IsNullOrEmpty(myDictionary["ReportChnGazDateFrom"].ToString()))
                sql.Append(" AND I_CERT.GAZETTE_DATE >= to_date(:ReportChnGazDateFrom, 'dd/mm/yyyy') ");
            if (!string.IsNullOrEmpty(myDictionary["ReportChnGazDateTo"].ToString()))
                sql.Append(" AND I_CERT.GAZETTE_DATE <= to_date(:ReportChnGazDateTo, 'dd/mm/yyyy') ");
            if (!string.IsNullOrEmpty(myDictionary["Chn_cat_gp"].ToString()))
                sql.Append(" AND S_CAT_GP.CODE IN(:Chn_cat_gp)  ");

            sql.Append(" ORDER BY ENG_SUB ASC, ENAME ASC");
            //string sql = "SELECT " +
            //    "upper(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME, " +
            //    "APPLN.CHINESE_NAME AS CNAME," +
            //    "I_CERT.REGISTRATION_DATE AS REG_D," +
            //    "I_CERT.CERTIFICATION_NO AS CERT_NO," +
            //    "S_CAT.CODE AS CAT_CODE," +
            //    "S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION AS ENG_SUB," +
            //    "S_CAT.CHINESE_SUB_TITLE_DESCRIPTION AS CHN_SUB," +
            //    "(select MIN(ENGLISH_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_NAME," +
            //    "(select MIN(ENGLISH_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_ENAME, " +
            //    "CASE WHEN to_date(:as_at_date, 'dd/mm/yyyy') > I_CERT.expiry_date " +
            //    "AND add_months(I_CERT.retention_application_date, 24) > trunc(to_date(:as_at_date, 'dd/mm/yyyy')) THEN '@' ELSE '' END AS FLAG," +
            //    "to_date(:as_at_date, 'dd/mm/yyyy') AS AS_AT_DATE," +
            //    "to_date(:gaz_fr_date,'dd/mm/yyyy') AS GAZ_FR_DATE," +
            //    "to_date(:gaz_to_date,'dd/mm/yyyy') AS GAZ_TO_DATE " +
            //    "FROM C_APPLICANT APPLN " +
            //    "INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID " +
            //    "INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
            //    "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
            //    "WHERE S_CAT.REGISTRATION_TYPE = :reg_type " +
            //    "AND S_CAT_GP.REGISTRATION_TYPE = :reg_type " +
            //    "AND S_CAT_GP.CODE IN(:cat_gp) " +
            //    "AND I_CERT.GAZETTE_DATE >= to_date(:gaz_fr_date, 'dd/mm/yyyy') " +
            //    "AND I_CERT.GAZETTE_DATE <= to_date(:gaz_to_date, 'dd/mm/yyyy') " +
            //    "AND I_CERT.CERTIFICATION_NO IS NOT NULL " +
            //    "AND " +
            //    "((I_CERT.EXPIRY_DATE IS NOT NULL and I_CERT.EXPIRY_DATE >= to_date(:as_at_date, 'dd/mm/yyyy')) or " +
            //    "(I_CERT.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(I_CERT.EXPIRY_DATE < to_date(:as_at_date, 'dd/mm/yyyy'))) )  " +
            //    "AND " +
            //    "((I_CERT.REMOVAL_DATE IS NULL) OR(I_CERT.REMOVAL_DATE > to_date(:as_at_date, 'dd/mm/yyyy'))) " +
            //    "ORDER BY ENG_SUB ASC, ENAME ASC";
            return sql.ToString();
        }

        public string getAnnualGazetteCHNIPEmptySql(Dictionary<string, Object> myDictionary)
        {
            string header1 = "";
            string header2 = "";
            if (myDictionary.Keys.Contains("PAHeaderChnDesc"))
            {
                header1 = myDictionary["PAHeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["PAHeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[0]
                            : myDictionary["PAHeaderChnDesc"].ToString();
                header2 = myDictionary["PAHeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["PAHeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[1]
                            : "";
            }
            else if (myDictionary.Keys.Contains("MWIHeaderChnDesc"))
            {
                header1 = myDictionary["MWIHeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["MWIHeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[0]
                            : myDictionary["MWIHeaderChnDesc"].ToString();
                header2 = myDictionary["MWIHeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["MWIHeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[1]
                            : "";
            }
            else
            {
                header1 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[0]
                            : myDictionary["HeaderChnDesc"].ToString();
                header2 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[1]
                            : "";
            }
            string sql = "SELECT " +
                "null AS ENAME, " +
                "null AS CNAME," +
                "null AS REG_D," +
                "null AS CERT_NO," +
                "null AS CAT_CODE," +
                "null AS ENG_SUB," +
                "null AS CHN_SUB," +
                "null as AUTH_NAME," +
                "null as AUTH_ENAME, " +
                "null AS FLAG," +
                "to_date('" + myDictionary["asAtDate2"].ToString() + "', 'dd/mm/yyyy') AS AS_AT_DATE," +
                "'" + header1 + "' AS HEADER1," +
                "'" + header2 + "' AS HEADER2," +
                "null AS GAZ_FR_DATE," +
                "null AS GAZ_TO_DATE " +
                "FROM dual";
            return sql;
        }
        //CRM0066
        private string getRPT0007bCGC(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(" SELECT " +
                "c_propercase(COMP_APPL.ENGLISH_COMPANY_NAME) AS ENAME, " +
                "to_char(COMP_APPL.EXPIRY_DATE,'dd/mm/yyyy') AS EXP_D, " +
                "COMP_APPL.CERTIFICATION_NO AS CERT_NO," +
                " COMP_APPL.BR_NO AS BR_NO," +
                "COMP_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
                "S_CAT_GP.ENGLISH_DESCRIPTION AS CAT_GP_ENG_DESC, " +
                "S_CAT_CODE.ENGLISH_DESCRIPTION AS CAT_CODE_ENG_DESC," +
                "to_date(:ExpiryFrDate,'dd/mm/yyyy') AS EXPIRY_FR_DATE," +
                "to_date(:ExpiryToDate,'dd/mm/yyyy') AS EXPIRY_TO_DATE  " +
                "FROM C_S_CATEGORY_CODE S_CAT_CODE " +
                "INNER JOIN C_COMP_APPLICATION COMP_APPL ON S_CAT_CODE.UUID = COMP_APPL.CATEGORY_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT_CODE.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE S_CAT_CODE.REGISTRATION_TYPE = :reg_type ");
            if (!string.IsNullOrEmpty(myDictionary["ExpiryFrDate"].ToString()))
                sql.Append(" AND EXPIRY_DATE >= to_date(:ExpiryFrDate, 'DD/MM/YYYY') ");

            if (!string.IsNullOrEmpty(myDictionary["ExpiryToDate"].ToString()))
                sql.Append(" AND EXPIRY_DATE <= to_date(:ExpiryToDate, 'DD/MM/YYYY') ");

            sql.Append("  ORDER BY CAT_CODE_ENG_DESC," + myDictionary["order_name"].ToString());  //:order_name


            //string sql = "SELECT " +
            //    "c_propercase(COMP_APPL.ENGLISH_COMPANY_NAME) AS ENAME, " +
            //    "to_char(COMP_APPL.EXPIRY_DATE,'dd/mm/yyyy') AS EXP_D, " +
            //    "COMP_APPL.CERTIFICATION_NO AS CERT_NO," +
            //    " COMP_APPL.BR_NO AS BR_NO," +
            //    "COMP_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
            //    "S_CAT_GP.ENGLISH_DESCRIPTION AS CAT_GP_ENG_DESC, " +
            //    "S_CAT_CODE.ENGLISH_DESCRIPTION AS CAT_CODE_ENG_DESC," +
            //    "to_date(:ExpiryFrDate,'dd/mm/yyyy') AS EXPIRY_FR_DATE," +
            //    "to_date(:ExpiryToDate,'dd/mm/yyyy') AS EXPIRY_TO_DATE  " +
            //    "FROM C_S_CATEGORY_CODE S_CAT_CODE " +
            //    "INNER JOIN C_COMP_APPLICATION COMP_APPL ON S_CAT_CODE.UUID = COMP_APPL.CATEGORY_ID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT_CODE.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
            //    "WHERE S_CAT_CODE.REGISTRATION_TYPE = :reg_type " +
            //    "AND EXPIRY_DATE >= to_date(:ExpiryFrDate, 'DD/MM/YYYY') " +
            //    "AND EXPIRY_DATE <= to_date(:ExpiryToDate, 'DD/MM/YYYY') " +
            //    "AND S_CAT_GP.CODE = :CategoryGroup " +
            //    "ORDER BY CAT_CODE_ENG_DESC, :order_name ";
            return sql.ToString();
        }

        //CRM0068
        private string getRPT0004CGC(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT " +
                "c_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS COMP_ENAME," +
                "to_char(C_APPL.EXPIRY_DATE,'dd/mm/yyyy') AS EXP_D," +
                "C_APPL.FILE_REFERENCE_NO AS FILE_REF," +
                "to_char(C_APPL.REGISTRATION_DATE,'dd/mm/yyyy') AS REG_D, " +
                "c_propercase(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID)AS APPLN_NAME," +
                "S_VAL1.CODE AS S_VAL1_CODE," +
                "S_VAL1.ENGLISH_DESCRIPTION AS ROLE_NAME " +
                "FROM C_COMP_APPLICATION C_APPL " +
                "INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID " +
                "INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_VAL1 ON C_APPLN.APPLICANT_ROLE_ID = S_VAL1.UUID " +
                "INNER JOIN C_S_SYSTEM_TYPE S_TYPE ON S_VAL1.SYSTEM_TYPE_ID = S_TYPE.UUID " +
                "WHERE " +
                "S_TYPE.TYPE = 'APPLICANT_ROLE' " +
                "AND S_VAL1.REGISTRATION_TYPE = :reg_type "
                //+"AND UPPER(C_APPL.ENGLISH_COMPANY_NAME) LIKE 'LEE HING REFRIGERATION SERVICE CO., LTD.' "
                );
            if (!string.IsNullOrEmpty(myDictionary["RegisteredContractorSummaryComp"].ToString()))
                sql.Append(" And C_APPL.ENGLISH_COMPANY_NAME like '%" + myDictionary["RegisteredContractorSummaryComp"].ToString() + "%'");
            sql.Append(" ORDER BY COMP_ENAME ASC, S_VAL1_CODE ASC,APPLN_NAME ASC ");

            //string sql = "SELECT " +
            //    "c_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS COMP_ENAME," +
            //    "to_char(C_APPL.EXPIRY_DATE,'dd/mm/yyyy') AS EXP_D," +
            //    "C_APPL.FILE_REFERENCE_NO AS FILE_REF," +
            //    "to_char(C_APPL.REGISTRATION_DATE,'dd/mm/yyyy') AS REG_D, " +
            //    "c_propercase(APPLN.SURNAME) || ' ' || c_propercase(APPLN.GIVEN_NAME_ON_ID)AS APPLN_NAME," +
            //    "S_VAL1.CODE AS S_VAL1_CODE," +
            //    "S_VAL1.ENGLISH_DESCRIPTION AS ROLE_NAME " +
            //    "FROM C_COMP_APPLICATION C_APPL " +
            //    "INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID " +
            //    "INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_VAL1 ON C_APPLN.APPLICANT_ROLE_ID = S_VAL1.UUID " +
            //    "INNER JOIN C_S_SYSTEM_TYPE S_TYPE ON S_VAL1.SYSTEM_TYPE_ID = S_TYPE.UUID " +
            //    "WHERE " +
            //    "S_TYPE.TYPE = 'APPLICANT_ROLE' " +
            //    "AND S_VAL1.REGISTRATION_TYPE = :reg_type " +
            //    "AND UPPER(C_APPL.ENGLISH_COMPANY_NAME) LIKE 'LEE HING REFRIGERATION SERVICE CO., LTD.' " +
            //    "ORDER BY COMP_ENAME ASC, S_VAL1_CODE ASC,APPLN_NAME ASC ";
            return sql.ToString();
        }

        //CRM0069
        private List<string> getRPT0008CGCIP(Dictionary<string, Object> myDictionary)
        {
            List<string> strSql = new List<string>();
            string regType1 = "";
            string regType2 = "";
            if (myDictionary["reg_type"].ToString() == "IP" || myDictionary["reg_type"].ToString() == "CGC")
            {
                regType1 = "IP";
                regType2 = "CGC";
            }
            if (myDictionary["reg_type"].ToString() == "CMW")
            {
                regType1 = "IMW";
                regType2 = "CMW";
            }
            if (myDictionary["reg_type"].ToString() == "IMW")
            {
                regType1 = "IMW";
                regType2 = "IMW";
            }

            StringBuilder sql1 = new StringBuilder();
            sql1.Append(" SELECT " +
                     "S_CAT_GP.CODE AS CAT_GP_CODE, " +
                     "S_APP_FORM.CODE AS FORM_CODE, " +
                     "I_CERT_HIST.MASTER_ID AS MASTER_ID, " +
                     "to_char(I_CERT_HIST.APPLICATION_DATE, 'MM') as AS_MONTH,  " +
                     "to_char(I_CERT_HIST.APPLICATION_DATE, 'YYYY') as AS_YEAR " +
                     "FROM C_IND_CERTIFICATE I_CERT " +
                     "INNER JOIN C_IND_CERTIFICATE_HISTORY I_CERT_HIST ON I_CERT.UUID = I_CERT_HIST.CERTIFICATE_ID " +
                     "INNER JOIN C_S_SYSTEM_VALUE S_APP_FORM ON I_CERT_HIST.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
                     "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT_HIST.CATEGORY_ID = S_CAT.UUID " +
                     "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                     "WHERE S_APP_FORM.REGISTRATION_TYPE = '" + regType1 + "' " +
                     " AND S_CAT_GP.REGISTRATION_TYPE = '" + regType1 + "'");
            if (!string.IsNullOrEmpty(myDictionary["ReceivedFrDate"].ToString()))
                sql1.Append(" AND I_CERT_HIST.APPLICATION_DATE >= to_date('" + myDictionary["ReceivedFrDate"].ToString() + "', 'DD/MM/YYYY') ");
            if (!string.IsNullOrEmpty(myDictionary["ReceivedToDate"].ToString()))
                sql1.Append(" AND I_CERT_HIST.APPLICATION_DATE <= to_date('" + myDictionary["ReceivedToDate"].ToString() + "', 'DD/MM/YYYY') ");
            sql1.Append(" GROUP BY S_CAT_GP.CODE, S_APP_FORM.CODE, I_CERT_HIST.MASTER_ID, to_char(I_CERT_HIST.APPLICATION_DATE, 'MM'), to_char(I_CERT_HIST.APPLICATION_DATE, 'YYYY') ");

            StringBuilder sql2 = new StringBuilder();
            sql2.Append("  SELECT S_CAT.CODE AS CAT_CODE, " +
                "S_APP_FORM.CODE AS FORM_CODE, " +
                "C_APPL_HIST.MASTER_ID AS MASTER_ID, " +
                "to_char(C_APPL_HIST.APPLICATION_DATE, 'MM') as AS_MONTH,  " +
                "to_char(C_APPL_HIST.APPLICATION_DATE, 'YYYY') as AS_YEAR " +
                "FROM C_COMP_APPLICATION C_APPL " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON C_APPL.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_COMP_APPLICATION_HISTORY C_APPL_HIST ON C_APPL.UUID = C_APPL_HIST.MASTER_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_APP_FORM ON C_APPL_HIST.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE S_APP_FORM.REGISTRATION_TYPE = '" + regType2 + "' " +
                "AND S_CAT_GP.REGISTRATION_TYPE = '" + regType2 + "'");
            if (!string.IsNullOrEmpty(myDictionary["ReceivedFrDate"].ToString()))
                sql2.Append(" AND C_APPL_HIST.APPLICATION_DATE >= to_date('" + myDictionary["ReceivedFrDate"].ToString() + "', 'DD/MM/YYYY') ");
            if (!string.IsNullOrEmpty(myDictionary["ReceivedToDate"].ToString()))
                sql2.Append(" And C_APPL_HIST.APPLICATION_DATE <= to_date('" + myDictionary["ReceivedToDate"].ToString() + "', 'DD/MM/YYYY') ");
            sql2.Append(" GROUP BY S_CAT.CODE, S_APP_FORM.CODE, C_APPL_HIST.MASTER_ID, to_char(C_APPL_HIST.APPLICATION_DATE, 'MM'), to_char(C_APPL_HIST.APPLICATION_DATE, 'YYYY') ");


            //string sql = "SELECT S_CAT.CODE AS CAT_CODE, " +
            //    "S_APP_FORM.CODE AS FORM_CODE, " +
            //    "C_APPL_HIST.MASTER_ID AS MASTER_ID, " +
            //    "to_char(C_APPL_HIST.APPLICATION_DATE, 'MM') as AS_MONTH,  " +
            //    "to_char(C_APPL_HIST.APPLICATION_DATE, 'YYYY') as AS_YEAR " +
            //    "FROM C_COMP_APPLICATION C_APPL " +
            //    "INNER JOIN C_S_CATEGORY_CODE S_CAT ON C_APPL.CATEGORY_ID = S_CAT.UUID " +
            //    "INNER JOIN C_COMP_APPLICATION_HISTORY C_APPL_HIST ON C_APPL.UUID = C_APPL_HIST.MASTER_ID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_APP_FORM ON C_APPL_HIST.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
            //    "WHERE S_APP_FORM.REGISTRATION_TYPE = 'CGC' " +
            //    "AND S_CAT_GP.REGISTRATION_TYPE = 'CGC' " +
            //    "AND C_APPL_HIST.APPLICATION_DATE >= to_date(:ReceivedFrDate, 'DD/MM/YYYY') " +
            //    "AND C_APPL_HIST.APPLICATION_DATE <= to_date(:ReceivedToDate, 'DD/MM/YYYY') " +
            //    "GROUP BY S_CAT.CODE, S_APP_FORM.CODE, C_APPL_HIST.MASTER_ID, to_char(C_APPL_HIST.APPLICATION_DATE, 'MM'), to_char(C_APPL_HIST.APPLICATION_DATE, 'YYYY')";
            //return sql.ToString();

            strSql.Add(sql1.ToString());
            strSql.Add(sql2.ToString());

            return strSql;
        }

        //CRM0070
        private string getRPT0008IP(Dictionary<string, Object> myDictionary)
        {

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT " +
                "S_CAT_GP.CODE AS CAT_GP_CODE, " +
                "S_APP_FORM.CODE AS FORM_CODE, " +
                "I_CERT_HIST.MASTER_ID AS MASTER_ID, " +
                "to_char(I_CERT_HIST.APPLICATION_DATE, 'MM') as AS_MONTH,  " +
                "to_char(I_CERT_HIST.APPLICATION_DATE, 'YYYY') as AS_YEAR " +
                "FROM C_IND_CERTIFICATE I_CERT " +
                "INNER JOIN C_IND_CERTIFICATE_HISTORY I_CERT_HIST ON I_CERT.UUID = I_CERT_HIST.CERTIFICATE_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_APP_FORM ON I_CERT_HIST.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT_HIST.CATEGORY_ID = S_CAT.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
                "WHERE S_APP_FORM.REGISTRATION_TYPE = 'IP' " +
                "AND S_CAT_GP.REGISTRATION_TYPE = 'IP' ");
            if (!string.IsNullOrEmpty(myDictionary["ReceivedFrDate"].ToString()))
                sql.Append(" AND I_CERT_HIST.APPLICATION_DATE >= to_date(:ReceivedFrDate, 'DD/MM/YYYY') ");
            if (!string.IsNullOrEmpty(myDictionary["ReceivedToDate"].ToString()))
                sql.Append(" AND I_CERT_HIST.APPLICATION_DATE <= to_date(:ReceivedToDate, 'DD/MM/YYYY') ");
            sql.Append(" GROUP BY S_CAT_GP.CODE, S_APP_FORM.CODE, I_CERT_HIST.MASTER_ID, to_char(I_CERT_HIST.APPLICATION_DATE, 'MM'), to_char(I_CERT_HIST.APPLICATION_DATE, 'YYYY') ");
            //string sql = "SELECT " +
            //    "S_CAT_GP.CODE AS CAT_GP_CODE, " +
            //    "S_APP_FORM.CODE AS FORM_CODE, " +
            //    "I_CERT_HIST.MASTER_ID AS MASTER_ID, " +
            //    "to_char(I_CERT_HIST.APPLICATION_DATE, 'MM') as AS_MONTH,  " +
            //    "to_char(I_CERT_HIST.APPLICATION_DATE, 'YYYY') as AS_YEAR " +
            //    "FROM C_IND_CERTIFICATE I_CERT " +
            //    "INNER JOIN C_IND_CERTIFICATE_HISTORY I_CERT_HIST ON I_CERT.UUID = I_CERT_HIST.CERTIFICATE_ID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_APP_FORM ON I_CERT_HIST.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
            //    "INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT_HIST.CATEGORY_ID = S_CAT.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +
            //    "WHERE S_APP_FORM.REGISTRATION_TYPE = 'IP' " +
            //    "AND S_CAT_GP.REGISTRATION_TYPE = 'IP' " +
            //    "AND I_CERT_HIST.APPLICATION_DATE >= to_date(:ReceivedFrDate, 'DD/MM/YYYY') " +
            //    "AND I_CERT_HIST.APPLICATION_DATE <= to_date(:ReceivedToDate, 'DD/MM/YYYY') " +
            //    "GROUP BY S_CAT_GP.CODE, S_APP_FORM.CODE, I_CERT_HIST.MASTER_ID, to_char(I_CERT_HIST.APPLICATION_DATE, 'MM'), to_char(I_CERT_HIST.APPLICATION_DATE, 'YYYY')";
            return sql.ToString();
        }
        //CRM0071
        private string getRPT0007aIP(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            string expiredFrDate = null;
            string expiredToDate = null;
            if (myDictionary.Keys.Contains("ExpiryFrDate"))
            {
                expiredFrDate = myDictionary["ExpiryFrDate"].ToString();
                expiredToDate = myDictionary["ExpiryToDate"].ToString();
            }
            else
            {
                expiredFrDate = myDictionary["Expired_fr_date"].ToString();
                expiredToDate = myDictionary["Expired_to_date"].ToString();
            }


            sql.Append(" SELECT " +
                "c_propercase(APPLN.SURNAME) AS SURNAME, " +
                "c_propercase(APPLN.GIVEN_NAME_ON_ID) AS GIVEN_NAME," +
                "IND_APPL.FILE_REFERENCE_NO AS FILE_REF," +
                "to_char(IND_CERT.EXPIRY_DATE,'dd/mm/yyyy') AS EXP_D, " +
                "IND_CERT.CERTIFICATION_NO AS CERT_NO," +
                "S_CAT_CODE.ENGLISH_DESCRIPTION AS ENG_DESC," +
                "S_CAT_GP.ENGLISH_DESCRIPTION AS CAT_GP_ENG, " +
                "to_date('" + expiredFrDate + "','dd/mm/yyyy') AS EXPIRY_FR_DATE," +
                "to_date('" + expiredToDate + "','dd/mm/yyyy') AS EXPIRY_TO_DATE  " +

                "FROM C_APPLICANT APPLN " +
                "INNER JOIN C_IND_APPLICATION IND_APPL ON APPLN.UUID = IND_APPL.APPLICANT_ID " +
                "INNER JOIN C_IND_CERTIFICATE IND_CERT ON IND_APPL.UUID = IND_CERT.MASTER_ID " +
                "INNER JOIN C_S_CATEGORY_CODE S_CAT_CODE ON IND_CERT.CATEGORY_ID = S_CAT_CODE.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT_CODE.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +

                "WHERE S_CAT_CODE.REGISTRATION_TYPE = :reg_type  and (IND_CERT.REMOVAL_DATE IS null or (to_char(IND_CERT.REMOVAL_DATE,'yyyymmdd') > to_char(IND_CERT.EXPIRY_DATE,'yyyymmdd'))) ");
            if (myDictionary.Keys.Contains("ExpiryFrDate"))
            {
                if (!string.IsNullOrEmpty(myDictionary["ExpiryFrDate"].ToString()))
                    sql.Append(" AND EXPIRY_DATE >= to_date(:ExpiryFrDate, 'DD/MM/YYYY') ");
                if (!string.IsNullOrEmpty(myDictionary["ExpiryToDate"].ToString()))
                    sql.Append(" AND EXPIRY_DATE <= to_date(:ExpiryToDate, 'DD/MM/YYYY') ");
                if (!string.IsNullOrEmpty(myDictionary["MWCategoryGroup"].ToString()))
                    sql.Append(" And S_CAT_GP.UUID = :MWCategoryGroup ");
            }
            else
            {
                if (!string.IsNullOrEmpty(myDictionary["Expired_fr_date"].ToString()))
                    sql.Append(" AND EXPIRY_DATE >= to_date(:Expired_fr_date, 'DD/MM/YYYY') ");
                if (!string.IsNullOrEmpty(myDictionary["Expired_to_date"].ToString()))
                    sql.Append(" AND EXPIRY_DATE <= to_date(:Expired_to_date, 'DD/MM/YYYY') ");
                if (!string.IsNullOrEmpty(myDictionary["ExpiredCtrUUID"].ToString()))
                    sql.Append(" And S_CAT_GP.UUID = :ExpiredCtrUUID ");
            }




            sql.Append(" ORDER BY ENG_DESC ASC ");

            if (!string.IsNullOrEmpty(myDictionary["IpOrderBy"].ToString()))
            {
                if (myDictionary["IpOrderBy"].ToString() == "SURNAME")
                    sql.Append(" ,APPLN.SURNAME ASC ");
                if (myDictionary["IpOrderBy"].ToString() == "EXPDSURNAME")
                    sql.Append(" ,IND_CERT.EXPIRY_DATE ASC, APPLN.SURNAME ASC ");
                else
                    sql.Append(" ,IND_APPL.FILE_REFERENCE_NO ASC ");
            }


            //string sql = "SELECT " +
            //    "c_propercase(APPLN.SURNAME) AS SURNAME, " +
            //    "c_propercase(APPLN.GIVEN_NAME_ON_ID) AS GIVEN_NAME," +
            //    "IND_APPL.FILE_REFERENCE_NO AS FILE_REF," +
            //    "to_char(IND_CERT.EXPIRY_DATE,'dd/mm/yyyy') AS EXP_D, " +
            //    "IND_CERT.CERTIFICATION_NO AS CERT_NO," +
            //    "S_CAT_CODE.ENGLISH_DESCRIPTION AS ENG_DESC," +
            //    "S_CAT_GP.ENGLISH_DESCRIPTION AS CAT_GP_ENG, " +
            //    "to_date(:expiry_fr_date,'dd/mm/yyyy') AS EXPIRY_FR_DATE," +
            //    "to_date(:expiry_to_date,'dd/mm/yyyy') AS EXPIRY_TO_DATE  " +

            //    "FROM C_APPLICANT APPLN " +
            //    "INNER JOIN C_IND_APPLICATION IND_APPL ON APPLN.UUID = IND_APPL.APPLICANT_ID " +
            //    "INNER JOIN C_IND_CERTIFICATE IND_CERT ON IND_APPL.UUID = IND_CERT.MASTER_ID " +
            //    "INNER JOIN C_S_CATEGORY_CODE S_CAT_CODE ON IND_CERT.CATEGORY_ID = S_CAT_CODE.UUID " +
            //    "INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT_CODE.CATEGORY_GROUP_ID = S_CAT_GP.UUID " +

            //    "WHERE S_CAT_CODE.REGISTRATION_TYPE = :reg_type AND S_CAT_GP.REGISTRATION_TYPE = :reg_type AND S_CAT_GP.CODE = :cat_group " +
            //    "AND EXPIRY_DATE >= to_date(:expiry_fr_date, 'DD/MM/YYYY') " +
            //    "AND EXPIRY_DATE <= to_date(:expiry_to_date, 'DD/MM/YYYY') " +
            //    "and (IND_CERT.REMOVAL_DATE IS null or (to_char(IND_CERT.REMOVAL_DATE,'yyyymmdd') > to_char(IND_CERT.EXPIRY_DATE,'yyyymmdd'))) " +
            //    "ORDER BY ENG_DESC ASC, :order_name ASC";
            return sql.ToString();
        }

        //CRM0073
        private string getAnnualGazetteChnMWC(Dictionary<string, Object> myDictionary)
        {
            string header1 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[0]
                            : myDictionary["HeaderChnDesc"].ToString();
            string header2 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[1]
                            : "";
            string sql = @"SELECT
                         upper(C_APPL.ENGLISH_COMPANY_NAME) AS ENAME,
                         C_APPL.CHINESE_COMPANY_NAME AS CNAME,
                         C_APPL.EXPIRY_DATE AS EXP_D,
                         C_APPL.CERTIFICATION_NO AS CERT_NO,
                         S_CAT.CHINESE_DESCRIPTION AS CHN_SUB,
                         S_CAT.ENGLISH_DESCRIPTION AS ENG_SUB,
                         S_CAT.CODE AS CAT_CODE,
                     (select MIN(CHINESE_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_NAME,
                    (select MIN(ENGLISH_NAME) from C_S_AUTHORITY where ENGLISH_RANK = 'Director of Buildings' ) as AUTH_ENAME,
                     CASE 
                                     WHEN to_date(:asAtDate2,'dd/mm/yyyy') > C_APPL.expiry_date 
                                          AND add_months(C_APPL.retention_application_date, 24) >
                                            trunc(to_date(:asAtDate2,'dd/mm/yyyy')) THEN 
                                     '@' 
                                     ELSE '' 
                                   END                    AS FLAG
                        ,to_date(:asAtDate2, 'dd/mm/yyyy') AS AS_AT_TIME 
                        ,'" + header1 + "' AS HEADER1, '" + header2 + @"' AS HEADER2 
                    FROM
                         C_S_CATEGORY_CODE S_CAT INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID
                         INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID
                    WHERE
                         S_CAT.REGISTRATION_TYPE = :reg_type
                     AND S_CAT_GP.REGISTRATION_TYPE = :reg_type
                     AND C_APPL.GAZETTE_DATE >= to_date(:ReportChnGazDateFrom,'dd/mm/yyyy')
                     AND C_APPL.GAZETTE_DATE <= to_date(:ReportChnGazDateTo,'dd/mm/yyyy')
 	                    AND C_APPL.CERTIFICATION_NO IS NOT NULL AND (  
		                     (C_APPL.EXPIRY_DATE IS NOT NULL and C_APPL.EXPIRY_DATE >= to_date(:asAtDate2,'dd/mm/yyyy')) or      
			                     ((C_APPL.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and  
			                       (C_APPL.EXPIRY_DATE < to_date(:asAtDate2,'dd/mm/yyyy')) ) ) )  AND ((C_APPL.REMOVAL_DATE IS NULL) OR      
		                       (C_APPL.REMOVAL_DATE > to_date(:asAtDate2,'dd/mm/yyyy')))
                     AND S_CAT_GP.CODE IN (:Chn_cat_gp)
                    ORDER BY
                         ENG_SUB ASC,
                         ENAME ASC";
            return sql;
        }

        public string getAnnualGazetteChnMWCEmptySql(Dictionary<string, Object> myDictionary)
        {
            string header1 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                           myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[0]
                           : myDictionary["HeaderChnDesc"].ToString();
            string header2 = myDictionary["HeaderChnDesc"].ToString().Contains("[YYYY]年[MM]月[DD]日") ?
                            myDictionary["HeaderChnDesc"].ToString().Split(new string[] { "[YYYY]年[MM]月[DD]日" }, StringSplitOptions.None)[1]
                            : "";
            string @sql = @"SELECT
                         null AS ENAME,
                         null AS CNAME,
                         null AS EXP_D,
                         null AS CERT_NO,
                         null AS CHN_SUB,
                         null AS ENG_SUB,
                         null AS CAT_CODE,
                         null as AUTH_NAME,
                         null as AUTH_ENAME,
                         null AS FLAG
                        ,to_date(' " + myDictionary["asAtDate2"].ToString() + @"', 'dd/mm/yyyy') AS AS_AT_TIME
                        ,'" + header1 + "' AS HEADER1, '" + header2 + "' AS HEADER2 FROM dual";
            return sql;
        }

        //public string getAnnualGazetteChnMWCEmpty()
        //{
        //    string sql = @"SELECT
        //                 null AS ENAME,
        //                 null AS CNAME,
        //                 null AS EXP_D,
        //                 null AS CERT_NO,
        //                 null AS CHN_SUB,
        //                 null AS ENG_SUB,
        //                 null AS CAT_CODE,
        //                 null as AUTH_NAME,
        //                 null as AUTH_ENAME,
        //                 null                   AS FLAG
        //            FROM Dual";
        //}

        //CRM0076
        private string getRPT0004MWC(Dictionary<string, Object> myDictionary)
        {

            string sql = @"SELECT 
C_propercase(A.COMP_ENAME) AS COMP_ENAME,
A.EXP_D,
A.FILE_REF,
A.REG_D,
C_propercase(A.APPLN_NAME) AS APPLN_NAME,
A.ROLE_CODE,
A.ROLE_NAME,
Case 
when B.MW_CLASS_CODE='Class 1' then 'Class I, II & III' || ': '
when B.MW_CLASS_CODE='Class 2' then 'Class II & III' || ': '
when B.MW_CLASS_CODE='Class 3' then 'Class III only' || ': ' end AS MW_CLASS_CODE,

B.MW_TYPE_CODE
FROM
(SELECT
     C_APPLN.UUID AS APPLN_ID,
     C_APPL.ENGLISH_COMPANY_NAME AS COMP_ENAME,
     C_APPL.EXPIRY_DATE AS EXP_D,
     C_APPL.FILE_REFERENCE_NO AS FILE_REF,
     C_APPL.REGISTRATION_DATE AS REG_D,
     (APPLN.SURNAME || ' ' || APPLN.GIVEN_NAME_ON_ID)AS APPLN_NAME,
     S_APPLN_ROLE.CODE AS ROLE_CODE,
     S_APPLN_ROLE.ENGLISH_DESCRIPTION AS ROLE_NAME

FROM
     C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
     INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_APPLN_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_APPLN_ROLE.UUID

WHERE
 S_APPLN_ROLE.REGISTRATION_TYPE = :reg_type
 AND C_APPL.ENGLISH_COMPANY_NAME LIKE '%" + myDictionary["RegisteredContractorSummaryComp"].ToString() + @"%'
) A

LEFT JOIN

(
SELECT 
     C_MW.COMPANY_APPLICANTS_ID AS APPLN_ID,
     MW_CLASS.UUID AS CLASS_ID,
     MW_CLASS.CODE AS MW_CLASS_CODE,
     C_RPT0004_CONCAT_MWTYPE(C_MW.COMPANY_APPLICANTS_ID, MW_CLASS.UUID) AS MW_TYPE_CODE
FROM
     C_COMP_APPLICANT_MW_ITEM C_MW INNER JOIN C_S_SYSTEM_VALUE MW_CLASS ON C_MW.ITEM_CLASS_ID = MW_CLASS.UUID
     INNER JOIN C_S_SYSTEM_VALUE MW_TYPE ON C_MW.ITEM_TYPE_ID = MW_TYPE.UUID
GROUP BY C_MW.COMPANY_APPLICANTS_ID,MW_CLASS.UUID, MW_CLASS.CODE
ORDER BY APPLN_ID ,MW_CLASS_CODE


) B ON B.APPLN_ID = A.APPLN_ID

ORDER BY
     COMP_ENAME ASC,
     ROLE_CODE ASC,
     APPLN_NAME ASC,
     MW_CLASS_CODE,
     MW_TYPE_CODE";
            return sql;
        }

        //CRM0074
        public string getProcessMonitorMWC()
        {
            string sql = "SELECT " +
                "(C_APPL.ENGLISH_COMPANY_NAME) AS ENAME, " +
                "C_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
                "S_APP_FORM.CODE AS FORM_CODE," +
                "C_PMON.NATURE AS NATURE, " +

                "case  when(select count(i.uuid) " +
                "from C_COMP_APPLICANT_MW_ITEM i, C_S_SYSTEM_VALUE  v  " +
                "where i.item_class_id = v.uuid and v.code = 'Class 1' and i.company_applicants_id = C_PMON.COMPANY_APPLICANTS_ID) > 0 then 'Y' else 'N' END AS CLASSS1," +

                "(APPLN.SURNAME)AS SURNAME, " +
                "(APPLN.GIVEN_NAME_ON_ID)AS GIVEN_NAME, " +
                "S_ROLE.CODE AS POST_ROLE, " +
                "to_char(C_PMON.RECEIVED_DATE,'dd/mm/yyyy')  AS RECEIVED_D," +

                "to_char(C_PMON.WITHDRAW_DATE,'dd/mm/yyyy')  AS WITHDRAW_DATE, " +

                "C_PMON.VETTING_OFFICER AS VO, " +
                "to_char(C_PMON.PLEDGE_INITIAL_DATE,'dd/mm/yyyy')  AS PLEDGE_DUE_10_DAYS_D, " +
                "to_char(C_PMON.ALTERNATIVE_LETTER_DATE,'dd/mm/yyyy') AS ALT_LETTER_D, " +
                "to_char(C_PMON.SUBSEQUENT_DATE,'dd/mm/yyyy') AS SUB_D, " +
                "to_char(C_PMON.PRELIMINARY_LETTER_DATE,'dd/mm/yyyy') AS PRELIM_LETTER_D, " +
                "C_PMON.CRC_NAME AS CRC, " +
                "to_char(C_PMON.INTERVIEW_DATE,'dd/mm/yyyy') AS INTRV_D, " +
                "C_PMON.SECRETARY AS SECRETARY," +
                "to_char(C_PMON.RESULT_LETTER_DATE,'dd/mm/yyyy') AS RESULT_LETTER_D," +
                "S_INTRV_RESULT.CODE AS INTRV_RESULT," +
                "C_PMON.INITIAL_REPLY AS INIT_REPLY," +
                "C_PMON.INTERVIEW AS INTRV," +
                "C_PMON.RESULT_LETTER AS RESULT_LETTER," +
                "to_char(C_PMON.CERTIFICATE_ISSUED_DATE,'dd/mm/yyyy') AS CERT_ISSUED_D," +
                "C_PMON.REMARKS AS REMARKS," +
                "to_char(to_date(:today,'dd/mm/yyyy'),'dd/mm/yyyy')  AS AS_TODAY " +

                "FROM C_COMP_APPLICATION C_APPL " +
                "INNER JOIN C_COMP_PROCESS_MONITOR C_PMON ON C_APPL.UUID = C_PMON.MASTER_ID " +
                "INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID " +
                "AND C_APPLN.UUID = C_PMON.COMPANY_APPLICANTS_ID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_ROLE.UUID " +
                "INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_APP_FORM ON C_PMON.APPLICATION_FORM_ID = S_APP_FORM.UUID " +
                "INNER JOIN C_S_SYSTEM_VALUE S_INTRV_RESULT ON C_PMON.INTERVIEW_RESULT_ID = S_INTRV_RESULT.UUID " +
                "WHERE C_PMON.MONITOR_TYPE = 'UPM' and C_APPL.REGISTRATION_TYPE =:reg_type " +
                "order by FILE_REF, ENAME, SURNAME, GIVEN_NAME";
            return sql;
        }
        //CRM0079
        private string getRPT0002IMWSql()
        {
            string sql = @"SELECT
layout.description AS S_MW_I_CAPA_M_DESCRIPTION,
nvl(datac.S_MW_I_CAPA_MAIN_IND_CAPA_ID, layout.description) AS S_MW_I_CAPA_MAIN_IND_CAPA_ID,
nvl(datac.TOTAL,0) as TOTAL,
nvl(datac.TOTAL2,0)as TOTAL2,
C_RPT0002_IMW_TOTAL3_NEW (:reg_type) AS TOTAL3
from(
SELECT distinct s.description FROM C_S_Mw_Ind_Capa_Main s 
inner join C_S_Mw_Ind_Capa v on v.UUID = s.S_MW_IND_CAPA_ID) layout left outer join
(    
SELECT 
    S_Mw_I_C_Main.description AS S_MW_I_CAPA_M_DESCRIPTION,
    S_Mw_I_C_Main.S_MW_IND_CAPA_ID AS S_MW_I_CAPA_MAIN_IND_CAPA_ID,
    COUNT (*) AS TOTAL,
    C_RPT0002_IMW_TOTAL2_NEW (:reg_type, S_Mw_I_C_Main.S_MW_IND_CAPA_ID)as TOTAL2,
    C_RPT0002_IMW_TOTAL3_NEW (:reg_type) AS TOTAL3      
FROM
     C_IND_CERTIFICATE I_CERT 
     INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
     INNER JOIN C_MW_IND_CAPA_FINAL MW_I_C_Final ON  I_APPL.UUID = MW_I_C_Final.MASTER_ID     
     INNER JOIN C_S_Mw_Ind_Capa S_MW_I_Capa ON  MW_I_C_Final.MW_IND_CAPA_DISPLAY_ID = S_MW_I_Capa.UUID
     INNER JOIN C_S_Mw_Ind_Capa_Main S_Mw_I_C_Main ON S_MW_I_Capa.UUID = S_MW_I_C_Main.S_MW_IND_CAPA_ID
WHERE 1=1
     and I_APPL.REGISTRATION_TYPE = :reg_type
     AND I_CERT.CERTIFICATION_NO IS NOT NULL
     and to_char(I_CERT.EXPIRY_DATE, 'yyyymmdd') >= to_char(sysdate, 'yyyymmdd')
     and (I_CERT.REMOVAL_DATE IS NULL or (to_char(I_CERT.REMOVAL_DATE, 'yyyymmdd') > to_char(I_CERT.EXPIRY_DATE)))
GROUP BY 
     S_Mw_I_C_Main.description,S_Mw_I_C_Main.S_MW_IND_CAPA_ID
     )
     datac on layout.description = datac.S_MW_I_CAPA_M_DESCRIPTION order by layout.description";
            return sql;
        }
        //CRM0080
        private string getRPT0004_IMW(Dictionary<string, Object> myDictionary)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@" SELECT
             C_propercase(APPLN.SURNAME) || ' ' || C_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME,
             TO_CHAR(I_CERT.EXPIRY_DATE, 'dd/mm/yyyy') AS EXP_D,
             I_APPL.FILE_REFERENCE_NO AS FILE_REF,
             TO_CHAR(I_CERT.REGISTRATION_DATE, 'dd/mm/yyyy') AS REG_D,
             MAX(C_RPT0004_CONCAT_MW_ITEM ( I_APPL.UUID, TO_CHAR(I_CERT.REGISTRATION_DATE, 'YYYYMMDD'))) AS MW_ITEM

        FROM
             C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
             INNER JOIN C_APPLICANT APPLN ON I_APPL.APPLICANT_ID = APPLN.UUID
        WHERE
                  I_APPL.REGISTRATION_TYPE = :reg_type
              ");

            if (!string.IsNullOrEmpty(myDictionary["Surname"].ToString()))
            {
                sql.Append(" And APPLN.SURNAME like '%" + myDictionary["Surname"].ToString() + "%' ");
            }
            if (!string.IsNullOrEmpty(myDictionary["GivenName"].ToString()))
            {
                sql.Append(" And APPLN.GIVEN_NAME_ON_ID like '%" + myDictionary["GivenName"].ToString() + "%' ");
            }

            sql.Append(@"    group by 
            C_propercase(APPLN.SURNAME) || ' ' || C_propercase(APPLN.GIVEN_NAME_ON_ID),
            TO_CHAR(I_CERT.EXPIRY_DATE, 'dd/mm/yyyy'),
            I_APPL.FILE_REFERENCE_NO,
            TO_CHAR(I_CERT.REGISTRATION_DATE, 'dd/mm/yyyy')");

            return sql.ToString();
        }

        //CRM0085
        private string getProgMWCReg(Dictionary<string, object> myDictionary)
        {
            if (string.IsNullOrEmpty(myDictionary["PMdate"].ToString()))
                myDictionary["PMdate"] = DateTime.Now.ToString("dd/MM/yyyy");
            string sql = @"SELECT * FROM (
                            (SELECT
                            SUM(CASE WHEN cat_code = 'MWC' AND instr(min_class, 'Class 1') > 0 THEN 1 ELSE 0 END) AS comp_mwc_1,
                            SUM(CASE WHEN cat_code = 'MWC' AND instr(min_class, 'Class 2') > 0 THEN 1 ELSE 0 END) AS comp_mwc_2,
                            SUM(CASE WHEN cat_code = 'MWC' AND instr(min_class, 'Class 3') > 0 THEN 1 ELSE 0 END) AS comp_mwc_3,
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class IS NULL THEN 1 ELSE 0 END) AS comp_mwc_0,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND instr(min_class, 'Class 1') > 0 THEN 1 ELSE 0 END) AS comp_mwcp_1,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND instr(min_class, 'Class 2') > 0 THEN 1 ELSE 0 END) AS comp_mwcp_2,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND instr(min_class, 'Class 3') > 0 THEN 1 ELSE 0 END) AS comp_mwcp_3,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class IS NULL THEN 1 ELSE 0 END) AS comp_mwcp_0,
                            SUM(CASE WHEN cat_code = 'MWC(W)' THEN 1 ELSE 0 END) AS comp_mwcw,
                            :PMdate AS AS_AT_DATE
                            FROM (
                            SELECT DISTINCT
                            scat.CODE AS cat_code,
                            capp.UUID,
                            capp.ENGLISH_COMPANY_NAME,
                            (SELECT
                            listagg(min(DISTINCT b.code))
                            FROM C_COMP_APPLICANT_INFO_MASTER a, C_S_SYSTEM_VALUE b, C_COMP_APPLICANT_INFO c,
                            C_S_SYSTEM_VALUE d, C_COMP_APPLICANT_INFO_DETAIL e, C_S_SYSTEM_VALUE f
                            WHERE e.ITEM_CLASS_ID = b.UUID
                            AND a.COMPANY_APPLICANTS_ID = c.UUID
                            AND e.COMPANY_APPLICANTS_MASTER_ID = a.UUID
                            AND e.ITEM_TYPE_ID = d.UUID
                            AND c.APPLICANT_ROLE_ID = f.UUID
                            AND c.master_id = capp.UUID
                            AND e.STATUS_CODE = 'APPLY'
                            AND f.CODE LIKE 'A%'
                            GROUP BY c.UUID
                            ) AS min_class
                            FROM C_COMP_APPLICATION capp
                            INNER JOIN C_S_CATEGORY_CODE scat ON capp.CATEGORY_ID = scat.UUID
                            WHERE
                            capp.REGISTRATION_TYPE= 'CMW'
                            AND scat.CODE IN('MWC', 'MWC(P)') 
And capp.application_date <= to_date(:PMdate,'dd/mm/yyyy')
And capp.registration_date <= to_date(:PMdate,'dd/mm/yyyy')


                            UNION ALL

                            SELECT scat.code, iapp.FILE_REFERENCE_NO,
                            app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID AS name,
                            '1' AS count
                            FROM C_IND_APPLICATION iapp,
                            C_APPLICANT app,
                            C_IND_CERTIFICATE icer,
                            C_S_CATEGORY_CODE scat
                            WHERE scat.UUID = icer.CATEGORY_ID
                            AND iapp.UUID = icer.MASTER_ID
                            AND iapp.APPLICANT_ID = app.UUID
                            AND iapp.REGISTRATION_TYPE = 'IMW'
                            AND scat.CODE = 'MWC(W)' 
AND icer.application_date <= to_date(:PMdate,'dd/mm/yyyy')
And icer.registration_date <= to_date(:PMdate,'dd/mm/yyyy')

                            )
                            )

                            LEFT JOIN

                            (SELECT
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class = 'Class 1' THEN 1 ELSE 0 END) AS as_mwc_1,
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class = 'Class 2' THEN 1 ELSE 0 END) AS as_mwc_2,
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class = 'Class 3' THEN 1 ELSE 0 END) AS as_mwc_3,
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class IS NULL THEN 1 ELSE 0 END) AS as_mwc_0,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class = 'Class 1' THEN 1 ELSE 0 END) AS as_mwcp_1,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class = 'Class 2' THEN 1 ELSE 0 END) AS as_mwcp_2,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class = 'Class 3' THEN 1 ELSE 0 END) AS as_mwcp_3,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class IS NULL THEN 1 ELSE 0 END) AS as_mwcp_0
                            FROM (

                            SELECT DISTINCT
                            scat.CODE AS cat_code,
                            cinfo.UUID,
                            capp.ENGLISH_COMPANY_NAME,
                            (SELECT DISTINCT min(b.code)
                            FROM C_COMP_APPLICANT_INFO_MASTER a, C_S_SYSTEM_VALUE b, C_COMP_APPLICANT_INFO c,
                            C_S_SYSTEM_VALUE d, C_COMP_APPLICANT_INFO_DETAIL e, C_S_SYSTEM_VALUE f
                            WHERE e.ITEM_CLASS_ID = b.UUID
                            AND a.COMPANY_APPLICANTS_ID = c.UUID
                            AND e.COMPANY_APPLICANTS_MASTER_ID = a.UUID
                            AND e.ITEM_TYPE_ID = d.UUID
                            AND c.UUID = cinfo.UUID
                            AND e.STATUS_CODE = 'APPLY'
                            AND c.APPLICANT_ROLE_ID = f.UUID
                            AND f.CODE LIKE 'A%'
                            ) AS min_class
                            FROM C_COMP_APPLICATION capp
                            INNER JOIN C_COMP_APPLICANT_INFO cinfo ON cinfo.MASTER_ID = capp.UUID
                            INNER JOIN C_S_CATEGORY_CODE scat ON capp.CATEGORY_ID = scat.UUID
                            INNER JOIN C_S_SYSTEM_VALUE srole ON cinfo.APPLICANT_ROLE_ID = srole.UUID
                            LEFT JOIN C_COMP_APPLICANT_INFO_MASTER cinfom ON cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID
                            LEFT JOIN C_COMP_APPLICANT_INFO_DETAIL cinfod ON cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID
                            WHERE
                            capp.REGISTRATION_TYPE= 'CMW'
                            AND scat.CODE IN('MWC', 'MWC(P)')
                            AND srole.CODE LIKE 'A%' 
And capp.application_date <= to_date(:PMdate,'dd/mm/yyyy')
And capp.registration_date <= to_date(:PMdate,'dd/mm/yyyy')
And cinfom.application_date <= to_date(:PMdate,'dd/mm/yyyy')
And cinfom.approved_date <= to_date(:PMdate,'dd/mm/yyyy')


                            )
                            ) ON 1=1

                            LEFT JOIN

                            (SELECT
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class = 'Class 1' THEN 1 ELSE 0 END) AS comp2_mwc_1,
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class = 'Class 2' THEN 1 ELSE 0 END) AS comp2_mwc_2,
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class = 'Class 3' THEN 1 ELSE 0 END) AS comp2_mwc_3,
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class IS NULL THEN 1 ELSE 0 END) AS comp2_mwc_0,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class = 'Class 1' THEN 1 ELSE 0 END) AS comp2_mwcp_1,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class = 'Class 2' THEN 1 ELSE 0 END) AS comp2_mwcp_2,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class = 'Class 3' THEN 1 ELSE 0 END) AS comp2_mwcp_3,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class IS NULL THEN 1 ELSE 0 END) AS comp2_mwcp_0,
                            SUM(CASE WHEN cat_code = 'MWC(W)' THEN 1 ELSE 0 END) AS comp2_mwcw
                            FROM (
                            SELECT DISTINCT
                            scat.CODE AS cat_code,
                            capp.UUID,
                            capp.ENGLISH_COMPANY_NAME,
                            (SELECT DISTINCT min(b.code)
                            FROM C_COMP_APPLICANT_INFO_MASTER a, C_S_SYSTEM_VALUE b, C_COMP_APPLICANT_INFO c,
                            C_S_SYSTEM_VALUE d, C_COMP_APPLICANT_INFO_DETAIL e, C_S_SYSTEM_VALUE f
                            WHERE e.ITEM_CLASS_ID = b.UUID
                            AND a.COMPANY_APPLICANTS_ID = c.UUID
                            AND e.COMPANY_APPLICANTS_MASTER_ID = a.UUID
                            AND e.ITEM_TYPE_ID = d.UUID
                            AND c.MASTER_ID = capp.UUID
                            AND e.STATUS_CODE = 'APPROVED'
                            AND c.APPLICANT_ROLE_ID = f.UUID
                            AND f.CODE LIKE 'A%'
                            ) AS min_class
                            FROM C_COMP_APPLICATION capp
                            INNER JOIN C_COMP_APPLICANT_INFO cinfo ON cinfo.MASTER_ID = capp.UUID
                            INNER JOIN C_S_CATEGORY_CODE scat ON capp.CATEGORY_ID = scat.UUID
                            INNER JOIN C_S_SYSTEM_VALUE status ON capp.APPLICATION_STATUS_ID = status.UUID
                            INNER JOIN C_S_SYSTEM_VALUE status2 ON cinfo.APPLICANT_STATUS_ID = status2.UUID
                            LEFT JOIN C_COMP_APPLICANT_INFO_MASTER cinfom ON cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID
                            LEFT JOIN C_COMP_APPLICANT_INFO_DETAIL cinfod ON cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID
                            WHERE
                            capp.REGISTRATION_TYPE= 'CMW'
                            AND scat.CODE IN('MWC', 'MWC(P)')
                            AND status.CODE <> '9'
                            AND status2.CODE <> '9'
                            AND cinfom.APPROVED_DATE IS NOT NULL 
And capp.application_date <= to_date(:PMdate,'dd/mm/yyyy')
And capp.registration_date <= to_date(:PMdate,'dd/mm/yyyy')
And cinfom.application_date <= to_date(:PMdate,'dd/mm/yyyy')
And cinfom.approved_date <= to_date(:PMdate,'dd/mm/yyyy')


                            UNION ALL

                            SELECT scat.code, iapp.FILE_REFERENCE_NO,
                            app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID AS name,
                            '1' AS count
                            FROM C_IND_APPLICATION iapp,
                            C_APPLICANT app,
                            C_IND_CERTIFICATE icer,
                            C_S_CATEGORY_CODE scat,
                            C_S_SYSTEM_VALUE status
                            WHERE scat.UUID = icer.CATEGORY_ID
                            AND iapp.UUID = icer.MASTER_ID
                            AND iapp.APPLICANT_ID = app.UUID
                            AND iapp.REGISTRATION_TYPE = 'IMW'
                            AND scat.CODE = 'MWC(W)'
                            AND icer.APPLICATION_STATUS_ID = status.UUID
                            AND status.CODE = '1' 
AND icer.application_date <= to_date(:PMdate,'dd/mm/yyyy')
And icer.registration_date <= to_date(:PMdate,'dd/mm/yyyy')

                            )
                            ) ON 1=1

                            LEFT JOIN (


                            (SELECT
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class = 'Class 1' THEN 1 ELSE 0 END) AS as2_mwc_1,
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class = 'Class 2' THEN 1 ELSE 0 END) AS as2_mwc_2,
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class = 'Class 3' THEN 1 ELSE 0 END) AS as2_mwc_3,
                            SUM(CASE WHEN cat_code = 'MWC' AND min_class IS NULL THEN 1 ELSE 0 END) AS as2_mwc_0,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class = 'Class 1' THEN 1 ELSE 0 END) AS as2_mwcp_1,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class = 'Class 2' THEN 1 ELSE 0 END) AS as2_mwcp_2,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class = 'Class 3' THEN 1 ELSE 0 END) AS as2_mwcp_3,
                            SUM(CASE WHEN cat_code = 'MWC(P)' AND min_class IS NULL THEN 1 ELSE 0 END) AS as2_mwcp_0
                            FROM (
                            SELECT DISTINCT
                            scat.CODE AS cat_code,
                            cinfo.UUID,
                            capp.ENGLISH_COMPANY_NAME,
                            (SELECT DISTINCT min(b.code)
                            FROM C_COMP_APPLICANT_INFO_MASTER a, C_S_SYSTEM_VALUE b, C_COMP_APPLICANT_INFO c,
                            C_S_SYSTEM_VALUE d, C_COMP_APPLICANT_INFO_DETAIL e, C_S_SYSTEM_VALUE f
                            WHERE e.ITEM_CLASS_ID = b.UUID
                            AND a.COMPANY_APPLICANTS_ID = c.UUID
                            AND e.COMPANY_APPLICANTS_MASTER_ID = a.UUID
                            AND e.ITEM_TYPE_ID = d.UUID
                            AND c.UUID = cinfo.UUID
                            AND e.STATUS_CODE = 'APPROVED'
                            AND c.APPLICANT_ROLE_ID = f.UUID
                            AND f.CODE LIKE 'A%'
                            ) AS min_class
                            FROM C_COMP_APPLICATION capp
                            INNER JOIN C_COMP_APPLICANT_INFO cinfo ON cinfo.MASTER_ID = capp.UUID
                            INNER JOIN C_S_CATEGORY_CODE scat ON capp.CATEGORY_ID = scat.UUID
                            INNER JOIN C_S_SYSTEM_VALUE status ON capp.APPLICATION_STATUS_ID = status.UUID
                            INNER JOIN C_S_SYSTEM_VALUE status2 ON cinfo.APPLICANT_STATUS_ID = status2.UUID
                            INNER JOIN C_S_SYSTEM_VALUE srole ON cinfo.APPLICANT_ROLE_ID = srole.UUID
                            LEFT JOIN C_COMP_APPLICANT_INFO_MASTER cinfom ON cinfom.COMPANY_APPLICANTS_ID = cinfo.UUID
                            LEFT JOIN C_COMP_APPLICANT_INFO_DETAIL cinfod ON cinfod.COMPANY_APPLICANTS_MASTER_ID = cinfom.UUID
                            WHERE
                            capp.REGISTRATION_TYPE= 'CMW'
                            AND scat.CODE IN('MWC', 'MWC(P)')
                            AND status.CODE <> '9'
                            AND status2.CODE <> '9'
                            AND srole.code LIKE 'A%'
                            AND cinfom.APPROVED_DATE IS NOT NULL 
And capp.application_date <= to_date(:PMdate,'dd/mm/yyyy')
And capp.registration_date <= to_date(:PMdate,'dd/mm/yyyy')
And cinfom.application_date <= to_date(:PMdate,'dd/mm/yyyy')
And cinfom.approved_date <= to_date(:PMdate,'dd/mm/yyyy')

                            )AS2_MWCP_1
                            )
                            ) ON 1=1

                            )";
            return sql;
        }

        //CRM0087
        private string getStatusAppControlFormIMW()
        {
            string sql = @"SELECT  
                S_TITLE.ENGLISH_DESCRIPTION AS TITLE, 
                C_propercase(APPLN.SURNAME) AS SURNAME, 
                C_propercase(APPLN.GIVEN_NAME_ON_ID) AS GIVEN_NAME, 
                I_PMON.AUDIT_TEXT AS AUDIT_TEXT, 
                I_APPL.FILE_REFERENCE_NO AS FILE_REF,  
                I_PMON.RECEIVED_DATE AS RECEIVED_D, 
                CASE WHEN I_PMON.DUE_DATE IS NOT NULL THEN I_PMON.DUE_DATE ELSE ADD_MONTHS(I_PMON.RECEIVED_DATE, 3) - 1 END as FOUR_MTHS, 
                I_PMON.SUPPLE_DOCUMENT_DATE AS SUP_DOC_D, 
                I_PMON.REFERENCE_ASK_DATE AS REF_ASK_D, 
                I_PMON.VETTING_OFFICER AS VETTING_OFFICER, 
                I_PMON.REGISTRATION_ASK_DATE AS REG_ASK_D,  
                I_PMON.REGISTRATION_REPLY_DATE AS REG_REPLY_D,  
                I_PMON.INTERVIEW_NOTIFY_DATE AS INTRV_NOTIFY_D, 
                I_PMON.INTERVIEW_DATE AS INTRV_D, 
                I_PMON.RESULT_ACCEPT_DATE AS RESULT_ACCEPT_D, 
                I_PMON.RESULT_DEFER_DATE AS RESULT_DEFER_D, 
                I_PMON.RESULT_REFUSE_DATE AS RESULT_REFUSE_D, 
                I_PMON.GAZETTE_DATE AS GAZETTE_D, 
                I_PMON.WITHDRAWAL_DATE AS WITHDRAWAL_D, 
                TRUNC(I_PMON.REFERENCE_ASK_DATE - I_PMON.RECEIVED_DATE)  AS INI_REPLY, 
                TRUNC(I_PMON.REGISTRATION_REPLY_DATE - I_PMON.RECEIVED_DATE)  AS SI_56, 
                TRUNC(I_PMON.INTERVIEW_DATE - I_PMON.RECEIVED_DATE)  AS INTRV, 
                TRUNC(I_PMON.RESULT_ACCEPT_DATE - I_PMON.RECEIVED_DATE)  AS RESULT_L, 
                TRUNC(I_PMON.SUPPLE_DOCUMENT_DATE - I_PMON.RECEIVED_DATE)  AS SUP_INFO, 
                TRUNC(I_PMON.GAZETTE_DATE - I_PMON.RECEIVED_DATE)  AS GAZETTE, 
                S_CAT_GP.CODE AS CAT_GP_CODE, 
                S_CAT_GP.ENGLISH_DESCRIPTION AS CAT_GP_ENG, 

                CASE  
                when CAT_CODE.CODE = 'API' or CAT_CODE.CODE = 'AP(A)' then 'A'  
                when CAT_CODE.CODE = 'APII' or CAT_CODE.CODE = 'AP(E)' then 'E'  
                when CAT_CODE.CODE = 'APIII' or CAT_CODE.CODE = 'AP(S)' then 'S' ELSE 'E' END AS SHORT_CODE 

                FROM C_APPLICANT APPLN  
                INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID  
                LEFT JOIN C_IND_PROCESS_MONITOR I_PMON ON I_APPL.UUID = I_PMON.MASTER_ID  
                LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON APPLN.TITLE_ID = S_TITLE.UUID  
                LEFT JOIN C_S_SYSTEM_VALUE S_CAT_GP ON I_PMON.CATEGORY_GROUP_ID = S_CAT_GP.UUID  
                LEFT JOIN C_S_CATEGORY_CODE CAT_CODE ON I_PMON.CATEGORY_ID = CAT_CODE.UUID AND S_CAT_GP.UUID = CAT_CODE.CATEGORY_GROUP_ID  

                WHERE  
                S_CAT_GP.REGISTRATION_TYPE = :reg_type AND S_CAT_GP.CODE=:CategoryGroup  
                order by I_PMON.RECEIVED_DATE,C_propercase(APPLN.SURNAME),C_propercase(APPLN.GIVEN_NAME_ON_ID) ";
            return sql;
        }

        //CRM0088
        private List<string> getRPT002_CGC_IP(Dictionary<string, Object> myDictionary)
        {
            string reg_type = "'" + myDictionary["reg_type"].ToString() + "'";

            List<string> strSql = new List<string>();
            string sql1 = @" SELECT
                         S_VAL1.CODE AS CAT_GP_CODE,
                         S_CAT.CODE AS CAT_CODE,
                         COUNT(*) AS TOTAL,
                         I_CERT.CATEGORY_ID AS CAT_ID,
                         S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION AS CAT_ENG,
                    S_VAL1.ENGLISH_DESCRIPTION AS CAT_GP_ENG,
                    CASE WHEN
                    (SELECT
                         COUNT(*)
                    FROM
                         C_IND_CERTIFICATE I_CERT2

                    WHERE
                    I_CERT2.CERTIFICATION_NO IS NOT NULL AND to_char(SYSDATE, 'YYYYMMDD') > to_char(I_CERT2.EXPIRY_DATE, 'YYYYMMDD')
                    and to_char(I_CERT2.RETENTION_APPLICATION_DATE, 'YYYYMMDD') > '20040831' and to_char(I_CERT2.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                    and(I_CERT2.REMOVAL_DATE is null or to_char(I_CERT2.REMOVAL_DATE, 'YYYYMMDD') > to_char(SYSDATE, 'YYYYMMDD'))
                    AND I_CERT2.CATEGORY_ID = I_CERT.CATEGORY_ID
                    GROUP BY I_CERT2.CATEGORY_ID) IS NULL THEN 0 ELSE
                    (SELECT
                         COUNT(*)
                    FROM
                         C_IND_CERTIFICATE I_CERT2

                    WHERE
                    I_CERT2.CERTIFICATION_NO IS NOT NULL AND to_char(SYSDATE, 'YYYYMMDD') > to_char(I_CERT2.EXPIRY_DATE, 'YYYYMMDD')
                    and to_char(I_CERT2.RETENTION_APPLICATION_DATE, 'YYYYMMDD') > '20040831' and to_char(I_CERT2.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                    and(I_CERT2.REMOVAL_DATE is null or to_char(I_CERT2.REMOVAL_DATE, 'YYYYMMDD') > to_char(SYSDATE, 'YYYYMMDD'))
                    AND I_CERT2.CATEGORY_ID = I_CERT.CATEGORY_ID
                    GROUP BY I_CERT2.CATEGORY_ID) END AS TOTAL2

                    FROM
                         C_S_CATEGORY_CODE S_CAT INNER JOIN C_S_SYSTEM_VALUE S_VAL1 ON S_CAT.CATEGORY_GROUP_ID = S_VAL1.UUID
                         INNER JOIN C_S_SYSTEM_TYPE S_TYPE1 ON S_VAL1.SYSTEM_TYPE_ID = S_TYPE1.UUID
                         INNER JOIN C_IND_CERTIFICATE I_CERT ON S_CAT.UUID = I_CERT.CATEGORY_ID

                    WHERE
                         S_TYPE1.TYPE = 'CATEGORY_GROUP'
                     AND S_VAL1.REGISTRATION_TYPE = " + reg_type + @"
                    AND I_CERT.CERTIFICATION_NO IS NOT NULL
                     and to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD') >= to_char(SYSDATE, 'YYYYMMDD') and(I_CERT.REMOVAL_DATE IS NULL
                    or(to_char(I_CERT.REMOVAL_DATE, 'YYYYMMDD') > to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD')))
                    GROUP BY S_VAL1.CODE, I_CERT.CATEGORY_ID, S_CAT.CODE, S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION, S_VAL1.ENGLISH_DESCRIPTION
                    order by CAT_GP_CODE, CAT_ENG, CAT_CODE
                   ";
            string sql2 = @" select
                    Z.NUM, sum (COUNT) as SUM_COUNT
                    from
                    (
                    SELECT 
                    '2' AS NUM, COUNT (*) AS COUNT
                    FROM
                    (SELECT A.FILE_REF, COUNT(*)
                    FROM
                    (SELECT
                        I_CERT.CATEGORY_ID AS CAT_ID,
                        I_APPL.FILE_REFERENCE_NO AS FILE_REF
                    FROM
                        C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                    WHERE

                        to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                        and (I_CERT.REMOVAL_DATE IS NULL or (to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD')
                        AND to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                        AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                    GROUP BY
                        I_CERT.CATEGORY_ID,
                        I_APPL.FILE_REFERENCE_NO
                    ) A
                    GROUP BY A.FILE_REF
                    HAVING COUNT(*) = 2
                    ) B

                    UNION

                    SELECT 
                    '2', COUNT (*)
                    FROM
                    (SELECT A.FILE_REF, COUNT(*)
                    FROM
                    (SELECT
                        I_CERT.CATEGORY_ID AS CAT_ID,
                        I_APPL.FILE_REFERENCE_NO AS FILE_REF
                    FROM
                        C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                    WHERE
                    to_char(I_CERT.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                    and to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                    and (I_CERT.REMOVAL_DATE is null or to_char(I_CERT.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))
                    AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                    GROUP BY
                        I_CERT.CATEGORY_ID,
                        I_APPL.FILE_REFERENCE_NO
                    ) A
                    GROUP BY A.FILE_REF
                    HAVING COUNT(*) = 2
                    ) B

                    UNION


                    SELECT 
                    '3', COUNT (*)
                    FROM
                    (SELECT A.FILE_REF, COUNT(*)
                    FROM
                    (SELECT
                        I_CERT.CATEGORY_ID AS CAT_ID,
                        I_APPL.FILE_REFERENCE_NO AS FILE_REF
                    FROM
                        C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                    WHERE

                        to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                        and (I_CERT.REMOVAL_DATE IS NULL or (to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD')
                        AND to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                        AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                    GROUP BY
                        I_CERT.CATEGORY_ID,
                        I_APPL.FILE_REFERENCE_NO
                    ) A
                    GROUP BY A.FILE_REF
                    HAVING COUNT(*) = 3
                    ) B

                    UNION

                    SELECT 
                    '3', COUNT (*)
                    FROM
                    (SELECT A.FILE_REF, COUNT(*)
                    FROM
                    (SELECT
                        I_CERT.CATEGORY_ID AS CAT_ID,
                        I_APPL.FILE_REFERENCE_NO AS FILE_REF
                    FROM
                        C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                    WHERE
                    to_char(I_CERT.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                    and to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                    and (I_CERT.REMOVAL_DATE is null or to_char(I_CERT.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))
                    AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                    GROUP BY
                        I_CERT.CATEGORY_ID,
                        I_APPL.FILE_REFERENCE_NO
                    ) A
                    GROUP BY A.FILE_REF
                    HAVING COUNT(*) = 3
                    ) B

                    UNION

                    SELECT 
                    '4', COUNT (*)
                    FROM
                    (SELECT A.FILE_REF, COUNT(*)
                    FROM
                    (SELECT
                        I_CERT.CATEGORY_ID AS CAT_ID,
                        I_APPL.FILE_REFERENCE_NO AS FILE_REF
                    FROM
                        C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                    WHERE

                        to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                        and (I_CERT.REMOVAL_DATE IS NULL or (to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD')
                        AND to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                        AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                    GROUP BY
                        I_CERT.CATEGORY_ID,
                        I_APPL.FILE_REFERENCE_NO
                    ) A
                    GROUP BY A.FILE_REF
                    HAVING COUNT(*) = 4
                    ) B

                    UNION

                    SELECT 
                    '4', COUNT (*)
                    FROM
                    (SELECT A.FILE_REF, COUNT(*)
                    FROM
                    (SELECT
                        I_CERT.CATEGORY_ID AS CAT_ID,
                        I_APPL.FILE_REFERENCE_NO AS FILE_REF
                    FROM
                        C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                    WHERE
                    to_char(I_CERT.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                    and to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                    and (I_CERT.REMOVAL_DATE is null or to_char(I_CERT.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))
                    AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                    GROUP BY
                        I_CERT.CATEGORY_ID,
                        I_APPL.FILE_REFERENCE_NO
                    ) A
                    GROUP BY A.FILE_REF
                    HAVING COUNT(*) = 4
                    ) B


                    UNION

                    SELECT 
                    '5', COUNT (*)
                    FROM
                    (SELECT A.FILE_REF, COUNT(*)
                    FROM
                    (SELECT
                        I_CERT.CATEGORY_ID AS CAT_ID,
                        I_APPL.FILE_REFERENCE_NO AS FILE_REF
                    FROM
                        C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                    WHERE

                        to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                        and (I_CERT.REMOVAL_DATE IS NULL or (to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD')
                        AND to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                        AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                    GROUP BY
                        I_CERT.CATEGORY_ID,
                        I_APPL.FILE_REFERENCE_NO
                    ) A
                    GROUP BY A.FILE_REF
                    HAVING COUNT(*) = 5
                    ) B

                    UNION

                    SELECT 
                    '5', COUNT (*)
                    FROM
                    (SELECT A.FILE_REF, COUNT(*)
                    FROM
                    (SELECT
                        I_CERT.CATEGORY_ID AS CAT_ID,
                        I_APPL.FILE_REFERENCE_NO AS FILE_REF
                    FROM
                        C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                    WHERE
                    to_char(I_CERT.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                    and to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                    and (I_CERT.REMOVAL_DATE is null or to_char(I_CERT.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))
                    AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                    GROUP BY
                        I_CERT.CATEGORY_ID,
                        I_APPL.FILE_REFERENCE_NO
                    ) A
                    GROUP BY A.FILE_REF
                    HAVING COUNT(*) = 5
                    ) B



                    UNION

                    SELECT 
                    'AP_RSE', COUNT (DISTINCT I_APPL.APPLICANT_ID)
                    FROM
                        C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                    WHERE

                        to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                        and (I_CERT.REMOVAL_DATE IS NULL or (to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD')))
                        AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"

                    UNION

                    SELECT 
                    'AP_RSE', COUNT (DISTINCT I_APPL.APPLICANT_ID)
                    FROM
                        C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                    WHERE
                    to_char(I_CERT.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                    and to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                    and (I_CERT.REMOVAL_DATE is null or to_char(I_CERT.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))
                    AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                    )Z
                    group by Z.NUM
                    ORDER BY Z.NUM ";
            string sql3 = @"SELECT 
                             A.CAT_ID,
                             A.CAT_CODE,
                             A.CAT_ENG,
                             A.TOTAL,
                             A.ROLE_CODE,
                             B.TOTAL2,
                             A.ENG_ROLE
                        FROM

                        (SELECT 
                             C_APPL.CATEGORY_ID AS CAT_ID,
                             S_CAT.CODE AS CAT_CODE,
                             S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION AS CAT_ENG,
                             COUNT(DISTINCT C_APPLN.APPLICANT_ID) AS TOTAL,
                             'AS' AS ROLE_ID,
                             'AS' AS ROLE_CODE,
                             'Authorized Signatory (AS)' AS ENG_ROLE
                        FROM
                             C_S_CATEGORY_CODE S_CAT INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID
                             INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
                             INNER JOIN C_S_SYSTEM_VALUE S_VAL_A_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_VAL_A_ROLE.UUID
                        WHERE
                             C_APPLN.ACCEPT_DATE IS NOT NULL
                         AND C_APPL.CERTIFICATION_NO IS NOT NULL
                         AND to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                             and (C_APPLN.REMOVAL_DATE IS null
                             or (C_APPLN.REMOVAL_DATE IS NOT null
                             and to_char(C_APPLN.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                             and (C_APPL.REMOVAL_DATE IS null
                             or (to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD')
                             and to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                             and S_VAL_A_ROLE.CODE like 'A%'
                             AND C_APPL.REGISTRATION_TYPE = " + reg_type + @"
                        GROUP BY
                             C_APPL.CATEGORY_ID,
                             S_CAT.CODE,
                             S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION) A

                        LEFT JOIN 
                        (SELECT 
                             C_APPL.CATEGORY_ID AS CAT_ID,
                             COUNT(DISTINCT C_APPLN.APPLICANT_ID) AS TOTAL2
                        FROM
                             C_S_CATEGORY_CODE S_CAT INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID
                             INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
                             INNER JOIN C_S_SYSTEM_VALUE S_VAL_STATUS ON C_APPLN.APPLICANT_STATUS_ID = S_VAL_STATUS.UUID
                             INNER JOIN C_S_SYSTEM_VALUE S_VAL_A_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_VAL_A_ROLE.UUID
                        WHERE
                             S_VAL_STATUS.CODE = '1'
                             and S_VAL_A_ROLE.CODE LIKE 'A%'
                             AND C_APPL.REGISTRATION_TYPE = " + reg_type + @"
                             AND C_APPL.CERTIFICATION_NO IS NOT NULL
                             AND C_APPLN.ACCEPT_DATE IS NOT NULL
                             and (to_char(SYSDATE,'YYYYMMDD') > to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD')
                             and to_char(C_APPL.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831')
                             and (C_APPL.REMOVAL_DATE IS null
                             or to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD'))

                        GROUP BY
                             C_APPL.CATEGORY_ID) B 
                        ON   B.CAT_ID = A.CAT_ID   


                        --UNION 1.2 O-----
                        UNION
                        SELECT 

                             A.CAT_ID,
                             A.CAT_CODE,
                             A.CAT_ENG,
                             A.TOTAL,
                             A.ROLE_CODE,
                             B.TOTAL2,
                             A.ENG_ROLE
                        FROM

                        (SELECT 
                             C_APPL.CATEGORY_ID AS CAT_ID,
                             S_CAT.CODE AS CAT_CODE,
                             S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION AS CAT_ENG,
                             COUNT(DISTINCT C_APPLN.APPLICANT_ID) AS TOTAL,
                             'OO' AS ROLE_ID,
                             'OO' AS ROLE_CODE,
                             'Other Officer (OO)' AS ENG_ROLE
                        FROM
                             C_S_CATEGORY_CODE S_CAT INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID
                             INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
                             INNER JOIN C_S_SYSTEM_VALUE S_VAL_A_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_VAL_A_ROLE.UUID
                        WHERE
                             C_APPLN.ACCEPT_DATE IS NOT NULL
                         AND C_APPL.CERTIFICATION_NO IS NOT NULL
                         AND to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                             and (C_APPLN.REMOVAL_DATE IS null
                             or (C_APPLN.REMOVAL_DATE IS NOT null
                             and to_char(C_APPLN.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                             and (C_APPL.REMOVAL_DATE IS null
                             or (to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD')
                             and to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                             and S_VAL_A_ROLE.CODE like 'O%'
                             AND C_APPL.REGISTRATION_TYPE = " + reg_type + @"
                        GROUP BY
                             C_APPL.CATEGORY_ID,
                             S_CAT.CODE,
                             S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION) A

                        LEFT JOIN 
                        (SELECT 
                             C_APPL.CATEGORY_ID AS CAT_ID,
                             COUNT(DISTINCT C_APPLN.APPLICANT_ID) AS TOTAL2
                        FROM
                             C_S_CATEGORY_CODE S_CAT INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID
                             INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
                             INNER JOIN C_S_SYSTEM_VALUE S_VAL_STATUS ON C_APPLN.APPLICANT_STATUS_ID = S_VAL_STATUS.UUID
                             INNER JOIN C_S_SYSTEM_VALUE S_VAL_A_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_VAL_A_ROLE.UUID
                        WHERE
                             S_VAL_STATUS.CODE = '1'
                             and S_VAL_A_ROLE.CODE LIKE 'O%'
                             AND C_APPL.REGISTRATION_TYPE = " + reg_type + @"
                             AND C_APPL.CERTIFICATION_NO IS NOT NULL
                             AND C_APPLN.ACCEPT_DATE IS NOT NULL
                             and (to_char(SYSDATE,'YYYYMMDD') > to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD')
                             and to_char(C_APPL.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831')
                             and (C_APPL.REMOVAL_DATE IS null
                             or to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD'))

                        GROUP BY
                             C_APPL.CATEGORY_ID) B 
                        ON   B.CAT_ID = A.CAT_ID   


                        --UNION 1.3 T-----
                        UNION

                        SELECT 

                             A.CAT_ID,
                             A.CAT_CODE,
                             A.CAT_ENG,
                             A.TOTAL,
                             A.ROLE_CODE,
                             B.TOTAL2,
                             A.ENG_ROLE
                        FROM

                        (SELECT 
                             C_APPL.CATEGORY_ID AS CAT_ID,
                             S_CAT.CODE AS CAT_CODE,
                             S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION AS CAT_ENG,
                             COUNT(DISTINCT C_APPLN.APPLICANT_ID) AS TOTAL,
                             'TD' AS ROLE_ID,
                             'TD' AS ROLE_CODE,
                             'Technical Director (TD)' AS ENG_ROLE
                        FROM
                             C_S_CATEGORY_CODE S_CAT INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID
                             INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
                             INNER JOIN C_S_SYSTEM_VALUE S_VAL_A_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_VAL_A_ROLE.UUID
                        WHERE
                             C_APPLN.ACCEPT_DATE IS NOT NULL
                         AND C_APPL.CERTIFICATION_NO IS NOT NULL
                         AND to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                             and (C_APPLN.REMOVAL_DATE IS null
                             or (C_APPLN.REMOVAL_DATE IS NOT null
                             and to_char(C_APPLN.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                             and (C_APPL.REMOVAL_DATE IS null
                             or (to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD')
                             and to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                             and S_VAL_A_ROLE.CODE like 'T%'
                             AND C_APPL.REGISTRATION_TYPE = " + reg_type + @"
                        GROUP BY
                             C_APPL.CATEGORY_ID,
                             S_CAT.CODE,
                             S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION) A

                        LEFT JOIN 
                        (SELECT 
                             C_APPL.CATEGORY_ID AS CAT_ID,
                             COUNT(DISTINCT C_APPLN.APPLICANT_ID) AS TOTAL2
                        FROM
                             C_S_CATEGORY_CODE S_CAT INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID
                             INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
                             INNER JOIN C_S_SYSTEM_VALUE S_VAL_STATUS ON C_APPLN.APPLICANT_STATUS_ID = S_VAL_STATUS.UUID
                             INNER JOIN C_S_SYSTEM_VALUE S_VAL_A_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_VAL_A_ROLE.UUID
                        WHERE
                             S_VAL_STATUS.CODE = '1'
                             and S_VAL_A_ROLE.CODE LIKE 'T%'
                             AND C_APPL.REGISTRATION_TYPE = " + reg_type + @"
                             AND C_APPL.CERTIFICATION_NO IS NOT NULL
                             AND C_APPLN.ACCEPT_DATE IS NOT NULL
                             and (to_char(SYSDATE,'YYYYMMDD') > to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD')
                             and to_char(C_APPL.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831')
                             and (C_APPL.REMOVAL_DATE IS null
                             or to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD'))

                        GROUP BY
                             C_APPL.CATEGORY_ID) B 
                        ON   B.CAT_ID = A.CAT_ID   

                        --------------------
                        --UNION 2

                        UNION

                        SELECT 
                             A.CAT_ID,
                             A.CAT_CODE,
                             A.CAT_ENG,
                             A.TOTAL,
                             'A1' ROLE_CODE,
                             B.TOTAL2,
                             'Contractors' ENG_ROLE
                        FROM

                        (SELECT
                             C_APPL.CATEGORY_ID CAT_ID,
                             S_CAT.CODE CAT_CODE,
                             S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION CAT_ENG,
                             COUNT(*)AS TOTAL 
                        FROM
                             C_S_CATEGORY_CODE S_CAT INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID
                        WHERE
                             C_APPL.CERTIFICATION_NO IS NOT NULL 
                             AND to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                             and (C_APPL.REMOVAL_DATE IS null or (to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD')
                             and to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                             AND C_APPL.REGISTRATION_TYPE = " + reg_type + @"
                        GROUP BY
                             C_APPL.CATEGORY_ID,
                             S_CAT.CODE,
                             S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION ) A


                        LEFT JOIN
                        (SELECT 
                             C_APPL.CATEGORY_ID CAT_ID,
                             count(*) TOTAL2
                        FROM
                             C_S_CATEGORY_CODE S_CAT INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID
                        WHERE
                             C_APPL.CERTIFICATION_NO IS NOT NULL 
                             AND (to_char(SYSDATE, 'YYYYMMDD') > to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD') 
                          and to_char(C_APPL.RETENTION_APPLICATION_DATE, 'YYYYMMDD') > '20040831' )
                          and (C_APPL.REMOVAL_DATE is null or to_char( C_APPL.REMOVAL_DATE, 'YYYYMMDD') > to_char(SYSDATE, 'YYYYMMDD'))
                        GROUP BY
                             C_APPL.CATEGORY_ID) B

                        ON A.CAT_ID=B.CAT_ID




                        -----------------------
                        --UNION 3

                        UNION

                        SELECT 

                             A.CAT_ID,
                             A.CAT_CODE,
                             A.CAT_ENG,
                             A.TOTAL,
                             'Z1' ROLE_CODE,
                             B.TOTAL2,
                             'Corporations' ENG_ROLE
                        FROM

                        (SELECT
                             C_APPL.CATEGORY_ID CAT_ID,
                             S_CAT.CODE CAT_CODE,
                             S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION CAT_ENG,
                             COUNT(*)AS TOTAL
                        FROM
                             C_S_CATEGORY_CODE S_CAT INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID
                             INNER JOIN C_S_SYSTEM_VALUE S_VAL_C_TYPE ON C_APPL.COMPANY_TYPE_ID = S_VAL_C_TYPE.UUID
                        WHERE
                             S_VAL_C_TYPE.CODE='1'
                             AND C_APPL.CERTIFICATION_NO IS NOT NULL
                             AND C_APPL.REGISTRATION_TYPE = " + reg_type + @"
                             AND to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                             and (C_APPL.REMOVAL_DATE IS null or (to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD')
                             and to_char(C_APPL.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))

                        GROUP BY
                             C_APPL.CATEGORY_ID,
                             S_CAT.CODE,
                             S_CAT.ENGLISH_SUB_TITLE_DESCRIPTION ) A

                        LEFT JOIN
                        (SELECT 
                             C_APPL.CATEGORY_ID CAT_ID,
                             count(*) TOTAL2
                        FROM
                             C_S_CATEGORY_CODE S_CAT INNER JOIN C_COMP_APPLICATION C_APPL ON S_CAT.UUID = C_APPL.CATEGORY_ID
                             INNER JOIN C_S_SYSTEM_VALUE S_VAL_C_TYPE ON C_APPL.COMPANY_TYPE_ID = S_VAL_C_TYPE.UUID
                        WHERE
                             S_VAL_C_TYPE.CODE='1'
                             AND C_APPL.CERTIFICATION_NO IS NOT NULL
                             AND (to_char( SYSDATE, 'YYYYMMDD') > to_char(C_APPL.EXPIRY_DATE,'YYYYMMDD') 
                             and to_char(  C_APPL.RETENTION_APPLICATION_DATE, 'YYYYMMDD') > '20040831' )
                          and (C_APPL.REMOVAL_DATE is null or to_char( C_APPL.REMOVAL_DATE, 'YYYYMMDD') > to_char(SYSDATE, 'YYYYMMDD'))
                        GROUP BY
                             C_APPL.CATEGORY_ID) B
                        ON A.CAT_ID=B.CAT_ID

                        ORDER BY ROLE_CODE, CAT_CODE";
            string sql4 = @"SELECT 
                            '2' AS NUM,'Person holding 2 registrations' as head, B.FILE_REF, (SELECT CODE FROM C_S_CATEGORY_CODE WHERE UUID = I_CERT2.CATEGORY_ID) AS CAT_CODE2,'' AS TEMP
                            FROM
                            (SELECT A.FILE_REF,A.MASTER_ID, COUNT(*)
                            FROM
                            (SELECT
                                 I_CERT.CATEGORY_ID AS CAT_ID,
                                 I_APPL.FILE_REFERENCE_NO AS FILE_REF,
                                 I_APPL.UUID AS MASTER_ID
                            FROM
                                 C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                            WHERE
                                 to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                                 and (I_CERT.REMOVAL_DATE IS NULL or (to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD')
                                 AND to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                                 AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                            GROUP BY
                                 I_CERT.CATEGORY_ID,
                                 I_APPL.FILE_REFERENCE_NO,
                                 I_APPL.UUID
                            ) A
                            GROUP BY A.FILE_REF, A.MASTER_ID
                            HAVING COUNT(*) = 2
                            ) B left join C_IND_CERTIFICATE I_CERT2 ON B.MASTER_ID = I_CERT2.MASTER_ID
                            WHERE
                                 to_char(I_CERT2.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                                 and (I_CERT2.REMOVAL_DATE IS NULL or (to_char(I_CERT2.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT2.EXPIRY_DATE,'YYYYMMDD')
                                 AND to_char(I_CERT2.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))

                            UNION

                            SELECT 
                            '2' AS NUM,'Person holding 2 registrations' as head, B.FILE_REF, (SELECT CODE FROM C_S_CATEGORY_CODE WHERE UUID = I_CERT2.CATEGORY_ID) AS CAT_CODE2,'*' AS TEMP
                            FROM
                            (SELECT A.FILE_REF,A.MASTER_ID, COUNT(*)
                            FROM
                            (SELECT
                                 I_CERT.CATEGORY_ID AS CAT_ID,
                                 I_APPL.FILE_REFERENCE_NO AS FILE_REF,
                                 I_APPL.UUID AS MASTER_ID
                            FROM
                                 C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                            WHERE
                                 to_char(I_CERT.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                                 and to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                                 and (I_CERT.REMOVAL_DATE is null or to_char(I_CERT.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))
                                 AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                            GROUP BY
                                 I_CERT.CATEGORY_ID,
                                 I_APPL.FILE_REFERENCE_NO,
                                 I_APPL.UUID
                            ) A
                            GROUP BY A.FILE_REF, A.MASTER_ID
                            HAVING COUNT(*) = 2
                            ) B left join C_IND_CERTIFICATE I_CERT2 ON B.MASTER_ID = I_CERT2.MASTER_ID
                            WHERE
                                 to_char(I_CERT2.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                                 and to_char(I_CERT2.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                                 and (I_CERT2.REMOVAL_DATE is null or to_char(I_CERT2.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))

                            UNION
                            -- COUNT = 3

                            SELECT 
                            '3' AS NUM,'Person holding 3 registrations' as head, B.FILE_REF, (SELECT CODE FROM C_S_CATEGORY_CODE WHERE UUID = I_CERT2.CATEGORY_ID) AS CAT_CODE2,'' AS TEMP
                            FROM
                            (SELECT A.FILE_REF,A.MASTER_ID, COUNT(*)
                            FROM
                            (SELECT
                                 I_CERT.CATEGORY_ID AS CAT_ID,
                                 I_APPL.FILE_REFERENCE_NO AS FILE_REF,
                                 I_APPL.UUID AS MASTER_ID
                            FROM
                                 C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                            WHERE
                                 to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                                 and (I_CERT.REMOVAL_DATE IS NULL or (to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD')
                                 AND to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                                 AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                            GROUP BY
                                 I_CERT.CATEGORY_ID,
                                 I_APPL.FILE_REFERENCE_NO,
                                 I_APPL.UUID
                            ) A
                            GROUP BY A.FILE_REF, A.MASTER_ID
                            HAVING COUNT(*) = 3
                            ) B left join C_IND_CERTIFICATE I_CERT2 ON B.MASTER_ID = I_CERT2.MASTER_ID
                            WHERE
                                 to_char(I_CERT2.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                                 and (I_CERT2.REMOVAL_DATE IS NULL or (to_char(I_CERT2.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT2.EXPIRY_DATE,'YYYYMMDD')
                                 AND to_char(I_CERT2.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))

                            UNION

                            SELECT 
                            '3' AS NUM,'Person holding 3 registrations' as head, B.FILE_REF, (SELECT CODE FROM C_S_CATEGORY_CODE WHERE UUID = I_CERT2.CATEGORY_ID) AS CAT_CODE2,'*' AS TEMP
                            FROM
                            (SELECT A.FILE_REF,A.MASTER_ID, COUNT(*)
                            FROM
                            (SELECT
                                 I_CERT.CATEGORY_ID AS CAT_ID,
                                 I_APPL.FILE_REFERENCE_NO AS FILE_REF,
                                 I_APPL.UUID AS MASTER_ID
                            FROM
                                 C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                            WHERE
                                 to_char(I_CERT.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                                 and to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                                 and (I_CERT.REMOVAL_DATE is null or to_char(I_CERT.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))
                                 AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                            GROUP BY
                                 I_CERT.CATEGORY_ID,
                                 I_APPL.FILE_REFERENCE_NO,
                                 I_APPL.UUID
                            ) A
                            GROUP BY A.FILE_REF, A.MASTER_ID
                            HAVING COUNT(*) = 3
                            ) B left join C_IND_CERTIFICATE I_CERT2 ON B.MASTER_ID = I_CERT2.MASTER_ID
                            WHERE
                                 to_char(I_CERT2.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                                 and to_char(I_CERT2.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                                 and (I_CERT2.REMOVAL_DATE is null or to_char(I_CERT2.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))

                            UNION

                            -- COUNT = 4

                            SELECT 
                            '4' AS NUM,'Person holding 4 registrations' as head, B.FILE_REF, (SELECT CODE FROM C_S_CATEGORY_CODE WHERE UUID = I_CERT2.CATEGORY_ID) AS CAT_CODE2,'' AS TEMP
                            FROM
                            (SELECT A.FILE_REF,A.MASTER_ID, COUNT(*)
                            FROM
                            (SELECT
                                 I_CERT.CATEGORY_ID AS CAT_ID,
                                 I_APPL.FILE_REFERENCE_NO AS FILE_REF,
                                 I_APPL.UUID AS MASTER_ID
                            FROM
                                 C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                            WHERE
                                 to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                                 and (I_CERT.REMOVAL_DATE IS NULL or (to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD')
                                 AND to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                                 AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                            GROUP BY
                                 I_CERT.CATEGORY_ID,
                                 I_APPL.FILE_REFERENCE_NO,
                                 I_APPL.UUID
                            ) A
                            GROUP BY A.FILE_REF, A.MASTER_ID
                            HAVING COUNT(*) = 4
                            ) B left join C_IND_CERTIFICATE I_CERT2 ON B.MASTER_ID = I_CERT2.MASTER_ID
                            WHERE
                                 to_char(I_CERT2.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                                 and (I_CERT2.REMOVAL_DATE IS NULL or (to_char(I_CERT2.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT2.EXPIRY_DATE,'YYYYMMDD')
                                 AND to_char(I_CERT2.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))

                            UNION

                            SELECT 
                            '4' AS NUM,'Person holding 4 registrations' as head, B.FILE_REF, (SELECT CODE FROM C_S_CATEGORY_CODE WHERE UUID = I_CERT2.CATEGORY_ID) AS CAT_CODE2,'*' AS TEMP
                            FROM
                            (SELECT A.FILE_REF,A.MASTER_ID, COUNT(*)
                            FROM
                            (SELECT
                                 I_CERT.CATEGORY_ID AS CAT_ID,
                                 I_APPL.FILE_REFERENCE_NO AS FILE_REF,
                                 I_APPL.UUID AS MASTER_ID
                            FROM
                                 C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                            WHERE
                                 to_char(I_CERT.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                                 and to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                                 and (I_CERT.REMOVAL_DATE is null or to_char(I_CERT.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))
                                 AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                            GROUP BY
                                 I_CERT.CATEGORY_ID,
                                 I_APPL.FILE_REFERENCE_NO,
                                 I_APPL.UUID
                            ) A
                            GROUP BY A.FILE_REF, A.MASTER_ID
                            HAVING COUNT(*) = 4
                            ) B left join C_IND_CERTIFICATE I_CERT2 ON B.MASTER_ID = I_CERT2.MASTER_ID
                            WHERE
                                 to_char(I_CERT2.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                                 and to_char(I_CERT2.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                                 and (I_CERT2.REMOVAL_DATE is null or to_char(I_CERT2.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))


                            UNION
                            -- COUNT = 5

                            SELECT 
                            '5' AS NUM,'Person holding 5 registrations' as head, B.FILE_REF, (SELECT CODE FROM C_S_CATEGORY_CODE WHERE UUID = I_CERT2.CATEGORY_ID) AS CAT_CODE2,'' AS TEMP
                            FROM
                            (SELECT A.FILE_REF,A.MASTER_ID, COUNT(*)
                            FROM
                            (SELECT
                                 I_CERT.CATEGORY_ID AS CAT_ID,
                                 I_APPL.FILE_REFERENCE_NO AS FILE_REF,
                                 I_APPL.UUID AS MASTER_ID
                            FROM
                                 C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                            WHERE
                                 to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                                 and (I_CERT.REMOVAL_DATE IS NULL or (to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT.EXPIRY_DATE,'YYYYMMDD')
                                 AND to_char(I_CERT.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))
                                 AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                            GROUP BY
                                 I_CERT.CATEGORY_ID,
                                 I_APPL.FILE_REFERENCE_NO,
                                 I_APPL.UUID
                            ) A
                            GROUP BY A.FILE_REF, A.MASTER_ID
                            HAVING COUNT(*) = 5
                            ) B left join C_IND_CERTIFICATE I_CERT2 ON B.MASTER_ID = I_CERT2.MASTER_ID
                            WHERE
                                 to_char(I_CERT2.EXPIRY_DATE,'YYYYMMDD') >= to_char(SYSDATE,'YYYYMMDD')
                                 and (I_CERT2.REMOVAL_DATE IS NULL or (to_char(I_CERT2.REMOVAL_DATE,'YYYYMMDD') > to_char(I_CERT2.EXPIRY_DATE,'YYYYMMDD')
                                 AND to_char(I_CERT2.REMOVAL_DATE,'YYYYMMDD') > to_char(SYSDATE,'YYYYMMDD')))

                            UNION

                            SELECT 
                            '5' AS NUM,'Person holding 5 registrations' as head, B.FILE_REF, (SELECT CODE FROM C_S_CATEGORY_CODE WHERE UUID = I_CERT2.CATEGORY_ID) AS CAT_CODE2,'*' AS TEMP
                            FROM
                            (SELECT A.FILE_REF,A.MASTER_ID, COUNT(*)
                            FROM
                            (SELECT
                                 I_CERT.CATEGORY_ID AS CAT_ID,
                                 I_APPL.FILE_REFERENCE_NO AS FILE_REF,
                                 I_APPL.UUID AS MASTER_ID
                            FROM
                                 C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
                            WHERE
                                 to_char(I_CERT.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                                 and to_char(I_CERT.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                                 and (I_CERT.REMOVAL_DATE is null or to_char(I_CERT.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))
                                 AND I_APPL.REGISTRATION_TYPE = " + reg_type + @"
                            GROUP BY
                                 I_CERT.CATEGORY_ID,
                                 I_APPL.FILE_REFERENCE_NO,
                                 I_APPL.UUID
                            ) A
                            GROUP BY A.FILE_REF, A.MASTER_ID
                            HAVING COUNT(*) = 5
                            ) B left join C_IND_CERTIFICATE I_CERT2 ON B.MASTER_ID = I_CERT2.MASTER_ID
                            WHERE
                                 to_char(I_CERT2.RETENTION_APPLICATION_DATE,'YYYYMMDD') > '20040831' 
                                 and to_char(I_CERT2.EXPIRY_DATE, 'YYYYMMDD') < to_char(SYSDATE, 'YYYYMMDD')
                                 and (I_CERT2.REMOVAL_DATE is null or to_char(I_CERT2.REMOVAL_DATE, 'YYYYMMDD') >to_char( SYSDATE, 'YYYYMMDD'))";
            strSql.Add(sql1);
            strSql.Add(sql2);
            strSql.Add(sql3);
            strSql.Add(sql4);
            return strSql;
        }



        //CRM0089
        private string getCountForFSSsql(Dictionary<string, Object> myDictionary)
        {
            string INTERESTED_FSS = "";
            if (string.IsNullOrEmpty(myDictionary["INTERESTED_FSS"].ToString()))
            {
                INTERESTED_FSS = "'Y','N','I'";
                myDictionary["INTERESTED_FSS"] = "A";
            }
            else if (myDictionary["INTERESTED_FSS"].ToString() == "Y")
            {
                INTERESTED_FSS = "'Y'";
            }
            else if (myDictionary["INTERESTED_FSS"].ToString() == "N")
            {
                INTERESTED_FSS = "'N'";
            }
            else
                INTERESTED_FSS = "'I'";
            //if (myDictionary["INTERESTED_FSS"].ToString() == "Y")
            //{
            //    INTERESTED_FSS = "'Y'";
            //}
            //else
            //{
            //    INTERESTED_FSS = "'N','I'";
            //}
            string sql = @"select code, sum(yes_cnt) as yes_cnt, sum(no_cnt) as no_cnt, sum(no_ind_cnt) as no_ind_cnt,  
                sum(seq) as seq,to_char(to_date(:today,'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_AT_TODAY,:INTERESTED_FSS AS WILLINGNESS_QP from  
                ( select ip_yes.code, ip_yes.count as yes_cnt, ip_no.count as no_cnt, ip_no_ind.count as no_ind_cnt, 0 AS seq from( (SELECT code, sum(count) AS count, 0 AS seq FROM(  
                select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status  
                where c.CATEGORY_ID = scat.UUID  
                and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
                and scatgrp.code in ('AP', 'RSE', 'RI') and INTERESTED_FSS in( " + INTERESTED_FSS + @" )  group by scatgrp.code  
                UNION ALL SELECT 'AP', 0, 0 FROM DUAL
                UNION ALL SELECT 'RSE', 0, 0 FROM DUAL
                UNION ALL SELECT 'RI', 0, 0 FROM DUAL) GROUP BY code ) ip_yes
                LEFT JOIN(select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status
                where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id
                and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and INTERESTED_FSS in (" + INTERESTED_FSS + @")
                group by scatgrp.code) ip_no on ip_yes.code = ip_no.code
                LEFT JOIN(select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status
               where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id
                and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and(INTERESTED_FSS in (" + INTERESTED_FSS + @")  or INTERESTED_FSS is null)
                group by scatgrp.code) ip_no_ind on ip_yes.code = ip_no_ind.code)  
                union all
                select cgc_yes.code, cgc_yes.count, cgc_no.count, cgc_no_ind.count, 0 AS seq from((
                SELECT code, sum(count) AS count, 0 AS seq from(select 'RGBC' AS code, count(*) as count, 0 as seq FROM(SELECT DISTINCT c.FILE_REFERENCE_NO
                from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID
                AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and INTERESTED_FSS in (" + INTERESTED_FSS + @"))
                UNION ALL
                SELECT 'RGBC', 0, 0 FROM DUAL) GROUP BY code) cgc_yes LEFT JOIN(select 'RGBC' AS code, count(*) as count, 0 as seq FROM(
                 SELECT DISTINCT c.FILE_REFERENCE_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'
                AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and INTERESTED_FSS in (" + INTERESTED_FSS + @"))) cgc_no
              on cgc_yes.code = cgc_no.code LEFT JOIN(select 'RGBC' AS code, count(*) as count, 0 as seq
                FROM(SELECT DISTINCT c.FILE_REFERENCE_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat
                where c.REGISTRATION_TYPE = 'CGC' AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC'
                and c.CERTIFICATION_NO IS NOT NULL and(INTERESTED_FSS in (" + INTERESTED_FSS + @")  or INTERESTED_FSS is null))) cgc_no_ind on cgc_yes.code = cgc_no_ind.code)  
                union all
                select imw_yes.code, imw_yes.count, imw_no.count, imw_no_ind.count, 0 AS seq from((SELECT code, sum(count) AS count, 0 AS seq from(
                select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status
                where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id
                and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem
                where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and INTERESTED_FSS in (" + INTERESTED_FSS + @")  group by iapp.REGISTRATION_TYPE
                UNION ALL
                SELECT 'RMWC(Individual) - Item 3.6', 0, 0 FROM DUAL) GROUP BY code) imw_yes
                LEFT JOIN(select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status
                where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))
                and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem
                where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and INTERESTED_FSS in(" + INTERESTED_FSS + @")
                group by iapp.REGISTRATION_TYPE) imw_no on imw_yes.code = imw_no.code
                LEFT JOIN(select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status
               where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))
                and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6'))  
                and(INTERESTED_FSS in (" + INTERESTED_FSS + @")  or INTERESTED_FSS is null) group by iapp.REGISTRATION_TYPE) imw_no_ind on imw_yes.code = imw_no_ind.code)  
                union all select cmw_yes.code, cmw_yes.count, cmw_no.count, cmw_no_ind.count, 0 AS seq from((SELECT code, sum(count) AS count, 0 AS seq FROM(
                select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM(SELECT DISTINCT c.FILE_REFERENCE_NO
                from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app
                where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid
                and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null
                and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in (select cmw.company_applicants_id
                from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and INTERESTED_FSS in (" + INTERESTED_FSS + @"))
                UNION ALL SELECT 'RMWC(Company) - Type A', 0, 0 FROM DUAL) GROUP BY code) cmw_yes
                LEFT JOIN(select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM(SELECT DISTINCT c.FILE_REFERENCE_NO
                from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app
                where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id
                and app_info.applicant_role_id = s_role.uuid and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1'
                and app_info.accept_date is not null and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in
                (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A'))
                and INTERESTED_FSS in (" + INTERESTED_FSS + @"))) cmw_no on cmw_yes.code = cmw_no.code
                LEFT JOIN(select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM(SELECT DISTINCT c.FILE_REFERENCE_NO

               from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app

               where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid

               and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null

               and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in
               (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A'))
                and(INTERESTED_FSS in (" + INTERESTED_FSS + @")  or INTERESTED_FSS is null) )) cmw_no_ind on cmw_yes.code = cmw_no_ind.code)  
                UNION ALL SELECT 'No. of Person' AS code, ind_yes.count, ind_no.count, ind_no_ind.count, 0 FROM((SELECT count(DISTINCT hkid) AS count FROM(select hkid || passport_no AS hkid
                from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID
                and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))
                and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem where sitem.uuid = iitem.ITEM_DETAILS_ID
                and sitem.code in ('Item 3.6')) and INTERESTED_FSS in (" + INTERESTED_FSS + @") group by iapp.REGISTRATION_TYPE, hkid || passport_no
                UNION ALL
                select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status
                where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id
                and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and INTERESTED_FSS in (" + INTERESTED_FSS + @"))) ind_yes
              LEFT JOIN(SELECT count(DISTINCT hkid) AS count FROM(select hkid || passport_no AS hkid from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status

              where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id

              and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem

              where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and INTERESTED_FSS in (" + INTERESTED_FSS + @")

              group by iapp.REGISTRATION_TYPE, hkid || passport_no

              UNION ALL

              select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status

              where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID

              AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))

              and scatgrp.code in ('AP', 'RSE', 'RI') and INTERESTED_FSS in (" + INTERESTED_FSS + @")))ind_no ON 1 = 1
                LEFT JOIN(SELECT count(DISTINCT hkid) AS count FROM (select hkid || passport_no AS hkid from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status
                where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id
                and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem
                where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6')) and(INTERESTED_FSS in (" + INTERESTED_FSS + @")  OR INTERESTED_FSS IS NULL) group by iapp.REGISTRATION_TYPE, hkid || passport_no
                UNION ALL select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status
                where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID AND app.UUID = iapp.APPLICANT_ID
                and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI')
                and(INTERESTED_FSS in (" + INTERESTED_FSS + @")  OR INTERESTED_FSS IS NULL)) )ind_no_ind ON 1 = 1)  
                UNION ALL SELECT 'No. of Company' AS code, comp_yes.count, comp_no.count, comp_no_ind.count, 0 FROM((SELECT count(DISTINCT br_no) AS count FROM(select c.BR_NO
                from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID
                AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and INTERESTED_FSS in (" + INTERESTED_FSS + @")
                group by c.REGISTRATION_TYPE, c.br_no
                UNION ALL
                select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app
                where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id
                and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid
                and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null and((app_info.removal_date is null) or(app_info.removal_date < current_date))
                and c.certification_no is not null and app_info.uuid in (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw

               inner
                join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and INTERESTED_FSS in (" + INTERESTED_FSS + @")  group by c.REGISTRATION_TYPE, c.br_no)) comp_yes
                LEFT JOIN(SELECT count(DISTINCT br_no) AS count FROM(select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'
                                                                                   
                and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and INTERESTED_FSS in (" + INTERESTED_FSS + @")
                                                                                   
                group by c.REGISTRATION_TYPE, c.br_no
                                                                                   
                UNION ALL select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app
                                                                                   
                where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid
                                                                                   
                and app_info.applicant_status_id = s_status.uuid
                                                                                   
                and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null
                                                                                   
                and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in
                (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw
                                                                                   
                inner
                                            join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and INTERESTED_FSS in (" + INTERESTED_FSS + @")
                                                                                   
                                                                                                   group by c.REGISTRATION_TYPE, c.br_no))comp_no ON 1 = 1
                LEFT JOIN(SELECT count(DISTINCT br_no) AS count FROM (select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'
                and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and (INTERESTED_FSS in ( " + INTERESTED_FSS + @" )  OR INTERESTED_FSS IS NULL)  
                group by c.REGISTRATION_TYPE, c.br_no
                UNION ALL
                select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app
                where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid
                and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null
                and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
                (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in('Type A'))  
                and(INTERESTED_FSS in (" + INTERESTED_FSS + @")  OR INTERESTED_FSS IS NULL) group by c.REGISTRATION_TYPE, c.br_no))comp_no_ind ON 1 = 1)  
                union all select 'AP', 0, 0, 0, 1 as seq from dual
                union all
                select 'RSE', 0, 0, 0, 2 as seq from dual
                union all
               select 'RI', 0, 0, 0, 3 as seq from dual
               union all
               select 'RGBC', 0, 0, 0, 4 as seq from dual
               union all
                                                                                                            select 'RMWC(Company) - Type A', 0, 0, 0, 7 as seq from dual
                                                                                                union all
                                                                                                                                                            select 'RMWC(Individual) - Item 3.6', 0, 0, 0, 10 as seq from dual
                                                                                                                                                    union all
                                                                                                                                                                                                                                                                     select 'No. of Person', 0, 0, 0, 12 as seq from DUAL
                                                                                                                                                                                                                                                           union all
                                                                                                                                                                                                                                                                                                                select 'No. of Company', 0, 0, 0, 13 as seq from dual)  
                group by code order by seq";

            //string sql = @"select code, sum(yes_cnt) as yes_cnt, sum(no_cnt) as no_cnt, sum(no_ind_cnt) as no_ind_cnt,  
            //    sum(seq) as seq,to_char(to_date(:today,'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_AT_TODAY,'CountForINTERESTED_FSS' AS WILLINGNESS_QP from  
            //    ( select ip_yes.code, ip_yes.count as yes_cnt, ip_no.count as no_cnt, ip_no_ind.count as no_ind_cnt, 0 AS seq from( (SELECT code, sum(count) AS count, 0 AS seq FROM(  
            //    select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID  
            //    and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
            //    and scatgrp.code in ('AP', 'RSE', 'RI') and INTERESTED_FSS = 'Y' group by scatgrp.code  
            //    UNION ALL SELECT 'AP', 0, 0 FROM DUAL  
            //    UNION ALL SELECT 'RSE', 0, 0 FROM DUAL  
            //    UNION ALL SELECT 'RI', 0, 0 FROM DUAL) GROUP BY code ) ip_yes  
            //    LEFT JOIN (select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and INTERESTED_FSS = 'N'  
            //    group by scatgrp.code) ip_no on ip_yes.code = ip_no.code  
            //    LEFT JOIN (select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and(INTERESTED_FSS = 'I' or INTERESTED_FSS is null)  
            //    group by scatgrp.code) ip_no_ind on ip_yes.code = ip_no_ind.code)  
            //    union all  
            //    select cgc_yes.code, cgc_yes.count, cgc_no.count, cgc_no_ind.count, 0 AS seq from(( 
            //    SELECT code, sum(count) AS count, 0 AS seq from(select 'RGBC' AS code, count(*) as count, 0 as seq FROM(SELECT DISTINCT c.FILE_REFERENCE_NO  
            //    from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID  
            //    AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and INTERESTED_FSS = 'Y')  
            //    UNION ALL  
            //    SELECT 'RGBC', 0, 0 FROM DUAL) GROUP BY code) cgc_yes LEFT JOIN (select 'RGBC' AS code, count(*) as count, 0 as seq FROM( 
            //     SELECT DISTINCT c.FILE_REFERENCE_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'  
            //    AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and INTERESTED_FSS = 'N')) cgc_no  
            //    on cgc_yes.code = cgc_no.code LEFT JOIN (select 'RGBC' AS code, count(*) as count, 0 as seq  
            //    FROM( SELECT DISTINCT c.FILE_REFERENCE_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat  
            //    where c.REGISTRATION_TYPE = 'CGC' AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC' 
            //    and c.CERTIFICATION_NO IS NOT NULL and(INTERESTED_FSS = 'I' or INTERESTED_FSS is null) )) cgc_no_ind on cgc_yes.code = cgc_no_ind.code)  
            //    union all  
            //    select imw_yes.code, imw_yes.count, imw_no.count, imw_no_ind.count, 0 AS seq from(( SELECT code, sum(count) AS count, 0 AS seq from(  
            //    select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status  
            //    where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
            //    where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and INTERESTED_FSS = 'Y' group by iapp.REGISTRATION_TYPE  
            //    UNION ALL  
            //    SELECT 'RMWC(Individual) - Item 3.6', 0, 0 FROM DUAL) GROUP BY code) imw_yes  
            //    LEFT JOIN (select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status  
            //    where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
            //    and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
            //    where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and INTERESTED_FSS = 'N'  
            //    group by iapp.REGISTRATION_TYPE) imw_no on imw_yes.code = imw_no.code  
            //    LEFT JOIN (select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status  
            //    where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
            //    and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6'))  
            //    and(INTERESTED_FSS = 'I' or INTERESTED_FSS is null) group by iapp.REGISTRATION_TYPE) imw_no_ind on imw_yes.code = imw_no_ind.code)  
            //    union all select cmw_yes.code, cmw_yes.count, cmw_no.count, cmw_no_ind.count, 0 AS seq from((SELECT code, sum(count) AS count, 0 AS seq FROM(  
            //    select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO  
            //    from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
            //    and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
            //    and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in ( select cmw.company_applicants_id  
            //    from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and INTERESTED_FSS = 'Y')  
            //    UNION ALL SELECT 'RMWC(Company) - Type A', 0, 0 FROM DUAL) GROUP BY code) cmw_yes  
            //    LEFT JOIN (select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO  
            //    from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id  
            //    and app_info.applicant_role_id = s_role.uuid and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1'  
            //    and app_info.accept_date is not null and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
            //    (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A'))  
            //    and INTERESTED_FSS = 'N')) cmw_no on cmw_yes.code = cmw_no.code  
            //    LEFT JOIN (select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO  
            //    from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
            //    and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
            //    and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
            //    ( select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A'))  
            //    and(INTERESTED_FSS = 'I' or INTERESTED_FSS is null) )) cmw_no_ind on cmw_yes.code = cmw_no_ind.code)  
            //    UNION ALL SELECT 'No. of Person' AS code, ind_yes.count, ind_no.count, ind_no_ind.count, 0 FROM((SELECT count(DISTINCT hkid) AS count FROM( select hkid || passport_no AS hkid  
            //    from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID  
            //    and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
            //    and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem where sitem.uuid = iitem.ITEM_DETAILS_ID  
            //    and sitem.code in ('Item 3.6')) and INTERESTED_FSS = 'Y'group by iapp.REGISTRATION_TYPE, hkid || passport_no  
            //    UNION ALL  
            //    select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and INTERESTED_FSS = 'Y')) ind_yes  
            //    LEFT JOIN ( SELECT count(DISTINCT hkid) AS count FROM( select hkid || passport_no AS hkid from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status  
            //    where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
            //    where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and INTERESTED_FSS = 'N'  
            //    group by iapp.REGISTRATION_TYPE, hkid || passport_no  
            //    UNION ALL  
            //    select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID  
            //    AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
            //    and scatgrp.code in ('AP', 'RSE', 'RI') and INTERESTED_FSS = 'N'))ind_no ON 1 = 1  
            //    LEFT JOIN( SELECT count(DISTINCT hkid) AS count FROM ( select hkid || passport_no AS hkid from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status  
            //    where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
            //    where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6')) and(INTERESTED_FSS = 'I' OR INTERESTED_FSS IS NULL) group by iapp.REGISTRATION_TYPE, hkid || passport_no  
            //    UNION ALL select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID AND app.UUID = iapp.APPLICANT_ID  
            //    and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI')  
            //    and(INTERESTED_FSS = 'I' OR INTERESTED_FSS IS NULL)) )ind_no_ind ON 1 = 1)  
            //    UNION ALL SELECT 'No. of Company' AS code, comp_yes.count, comp_no.count, comp_no_ind.count, 0 FROM( (SELECT count(DISTINCT br_no) AS count FROM(select c.BR_NO  
            //    from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID  
            //    AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and INTERESTED_FSS = 'Y'  
            //    group by c.REGISTRATION_TYPE, c.br_no  
            //    UNION ALL  
            //    select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id  
            //    and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid  
            //    and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null and((app_info.removal_date is null) or(app_info.removal_date < current_date))  
            //    and c.certification_no is not null and app_info.uuid in ( select cmw.company_applicants_id from C_comp_applicant_mw_item cmw  
            //    inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and INTERESTED_FSS = 'Y' group by c.REGISTRATION_TYPE, c.br_no)) comp_yes  
            //    LEFT JOIN ( SELECT count(DISTINCT br_no) AS count FROM( select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'  
            //    and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and INTERESTED_FSS = 'N'  
            //    group by c.REGISTRATION_TYPE, c.br_no  
            //    UNION ALL select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
            //    and app_info.applicant_status_id = s_status.uuid  
            //    and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
            //    and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
            //    (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw  
            //    inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and INTERESTED_FSS = 'N'  
            //    group by c.REGISTRATION_TYPE, c.br_no))comp_no ON 1 = 1  
            //    LEFT JOIN(SELECT count(DISTINCT br_no) AS count FROM (select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'  
            //    and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and (INTERESTED_FSS = 'I' OR INTERESTED_FSS IS NULL)  
            //    group by c.REGISTRATION_TYPE, c.br_no  
            //    UNION ALL  
            //    select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
            //    and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
            //    and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
            //    (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in('Type A'))  
            //    and(INTERESTED_FSS = 'I' OR INTERESTED_FSS IS NULL) group by c.REGISTRATION_TYPE, c.br_no))comp_no_ind ON 1 = 1)  
            //    union all select 'AP', 0, 0, 0, 1 as seq from dual  
            //    union all select 'RSE', 0, 0, 0, 2 as seq from dual  
            //    union all select 'RI', 0, 0, 0, 3 as seq from dual 
            //     union all select 'RGBC', 0, 0, 0, 4 as seq from dual  
            //    union all select 'RMWC(Company) - Type A', 0, 0, 0, 7 as seq from dual  
            //    union all select 'RMWC(Individual) - Item 3.6', 0, 0, 0, 10 as seq from dual  
            //    union all select 'No. of Person', 0, 0, 0, 12 as seq from DUAL  
            //    union all select 'No. of Company', 0, 0, 0, 13 as seq from dual)  
            //    group by code order by seq";
            return sql;
        }

        //CRM0090
        private string getCountForMBISsql(Dictionary<string, Object> myDictionary)
        {
            string SERVICE_IN_MWIS = "";
            if (myDictionary["SERVICE_IN_MWIS"].ToString() == "Y")
            {
                SERVICE_IN_MWIS = "'Y'";
            }
            else
            {
                SERVICE_IN_MWIS = "'N','I'";
            }

            string sql = @"select code, sum(yes_cnt) as yes_cnt, sum(no_cnt) as no_cnt, sum(no_ind_cnt) as no_ind_cnt,  
                sum(seq) as seq,to_char(to_date(:today,'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_AT_TODAY,:SERVICE_IN_MWIS AS WILLINGNESS_QP from  
                ( select ip_yes.code, ip_yes.count as yes_cnt, ip_no.count as no_cnt, ip_no_ind.count as no_ind_cnt, 0 AS seq from( (SELECT code, sum(count) AS count, 0 AS seq FROM(  
                select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status  
                where c.CATEGORY_ID = scat.UUID  
                and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
                and scatgrp.code in ('AP', 'RSE', 'RI') and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ) group by scatgrp.code  
                UNION ALL SELECT 'AP', 0, 0 FROM DUAL  
                UNION ALL SELECT 'RSE', 0, 0 FROM DUAL  
                UNION ALL SELECT 'RI', 0, 0 FROM DUAL) GROUP BY code ) ip_yes  
                LEFT JOIN (select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status  
                where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id  
                and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" )  
                group by scatgrp.code) ip_no on ip_yes.code = ip_no.code  
                LEFT JOIN (select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status  
                where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id  
                and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and(SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ) or SERVICE_IN_MWIS is null)  
                group by scatgrp.code) ip_no_ind on ip_yes.code = ip_no_ind.code)  
                union all  
                select cgc_yes.code, cgc_yes.count, cgc_no.count, cgc_no_ind.count, 0 AS seq from(( 
                SELECT code, sum(count) AS count, 0 AS seq from(select 'RGBC' AS code, count(*) as count, 0 as seq FROM(SELECT DISTINCT c.FILE_REFERENCE_NO  
                from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID  
                AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ))  
                UNION ALL  
                SELECT 'RGBC', 0, 0 FROM DUAL) GROUP BY code) cgc_yes LEFT JOIN (select 'RGBC' AS code, count(*) as count, 0 as seq FROM( 
                 SELECT DISTINCT c.FILE_REFERENCE_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'  
                AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ))) cgc_no  
                on cgc_yes.code = cgc_no.code LEFT JOIN (select 'RGBC' AS code, count(*) as count, 0 as seq  
                FROM( SELECT DISTINCT c.FILE_REFERENCE_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat  
                where c.REGISTRATION_TYPE = 'CGC' AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC' 
                and c.CERTIFICATION_NO IS NOT NULL and(SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ) or SERVICE_IN_MWIS is null) )) cgc_no_ind on cgc_yes.code = cgc_no_ind.code)  
                union all  
                select imw_yes.code, imw_yes.count, imw_no.count, imw_no_ind.count, 0 AS seq from(( SELECT code, sum(count) AS count, 0 AS seq from(  
                select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status  
                where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id  
                and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
                where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ) group by iapp.REGISTRATION_TYPE  
                UNION ALL  
                SELECT 'RMWC(Individual) - Item 3.6', 0, 0 FROM DUAL) GROUP BY code) imw_yes  
                LEFT JOIN (select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status  
                where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
                and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
                where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" )  
                group by iapp.REGISTRATION_TYPE) imw_no on imw_yes.code = imw_no.code  
                LEFT JOIN (select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status  
                where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
                and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6'))  
                and(SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ) or SERVICE_IN_MWIS is null) group by iapp.REGISTRATION_TYPE) imw_no_ind on imw_yes.code = imw_no_ind.code)  
                union all select cmw_yes.code, cmw_yes.count, cmw_no.count, cmw_no_ind.count, 0 AS seq from((SELECT code, sum(count) AS count, 0 AS seq FROM(  
                select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO  
                from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
                where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
                and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
                and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in ( select cmw.company_applicants_id  
                from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ))  
                UNION ALL SELECT 'RMWC(Company) - Type A', 0, 0 FROM DUAL) GROUP BY code) cmw_yes  
                LEFT JOIN (select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO  
                from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
                where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id  
                and app_info.applicant_role_id = s_role.uuid and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1'  
                and app_info.accept_date is not null and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
                (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A'))  
                and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ))) cmw_no on cmw_yes.code = cmw_no.code  
                LEFT JOIN (select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO  
                from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
                where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
                and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
                and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
                ( select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A'))  
                and(SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ) or SERVICE_IN_MWIS is null) )) cmw_no_ind on cmw_yes.code = cmw_no_ind.code)  
                UNION ALL SELECT 'No. of Person' AS code, ind_yes.count, ind_no.count, ind_no_ind.count, 0 FROM((SELECT count(DISTINCT hkid) AS count FROM( select hkid || passport_no AS hkid  
                from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID  
                and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
                and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem where sitem.uuid = iitem.ITEM_DETAILS_ID  
                and sitem.code in ('Item 3.6')) and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" )group by iapp.REGISTRATION_TYPE, hkid || passport_no  
                UNION ALL  
                select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status  
                where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id  
                and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ))) ind_yes  
                LEFT JOIN ( SELECT count(DISTINCT hkid) AS count FROM( select hkid || passport_no AS hkid from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status  
                where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id  
                and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
                where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" )  
                group by iapp.REGISTRATION_TYPE, hkid || passport_no  
                UNION ALL  
                select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status  
                where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID  
                AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
                and scatgrp.code in ('AP', 'RSE', 'RI') and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" )))ind_no ON 1 = 1  
                LEFT JOIN( SELECT count(DISTINCT hkid) AS count FROM ( select hkid || passport_no AS hkid from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status  
                where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id  
                and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
                where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6')) and(SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ) OR SERVICE_IN_MWIS IS NULL) group by iapp.REGISTRATION_TYPE, hkid || passport_no  
                UNION ALL select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status  
                where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID AND app.UUID = iapp.APPLICANT_ID  
                and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI')  
                and(SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ) OR SERVICE_IN_MWIS IS NULL)) )ind_no_ind ON 1 = 1)  
                UNION ALL SELECT 'No. of Company' AS code, comp_yes.count, comp_no.count, comp_no_ind.count, 0 FROM( (SELECT count(DISTINCT br_no) AS count FROM(select c.BR_NO  
                from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID  
                AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" )  
                group by c.REGISTRATION_TYPE, c.br_no  
                UNION ALL  
                select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
                where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id  
                and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid  
                and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null and((app_info.removal_date is null) or(app_info.removal_date < current_date))  
                and c.certification_no is not null and app_info.uuid in ( select cmw.company_applicants_id from C_comp_applicant_mw_item cmw  
                inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ) group by c.REGISTRATION_TYPE, c.br_no)) comp_yes  
                LEFT JOIN ( SELECT count(DISTINCT br_no) AS count FROM( select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'  
                and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" )  
                group by c.REGISTRATION_TYPE, c.br_no  
                UNION ALL select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
                where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
                and app_info.applicant_status_id = s_status.uuid  
                and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
                and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
                (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw  
                inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" )  
                group by c.REGISTRATION_TYPE, c.br_no))comp_no ON 1 = 1  
                LEFT JOIN(SELECT count(DISTINCT br_no) AS count FROM (select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'  
                and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and (SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ) OR SERVICE_IN_MWIS IS NULL)  
                group by c.REGISTRATION_TYPE, c.br_no  
                UNION ALL  
                select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
                where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
                and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
                and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
                (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in('Type A'))  
                and(SERVICE_IN_MWIS in ( " + SERVICE_IN_MWIS + @" ) OR SERVICE_IN_MWIS IS NULL) group by c.REGISTRATION_TYPE, c.br_no))comp_no_ind ON 1 = 1)  
                union all select 'AP', 0, 0, 0, 1 as seq from dual  
                union all select 'RSE', 0, 0, 0, 2 as seq from dual  
                union all select 'RI', 0, 0, 0, 3 as seq from dual 
                 union all select 'RGBC', 0, 0, 0, 4 as seq from dual  
                union all select 'RMWC(Company) - Type A', 0, 0, 0, 7 as seq from dual  
                union all select 'RMWC(Individual) - Item 3.6', 0, 0, 0, 10 as seq from dual  
                union all select 'No. of Person', 0, 0, 0, 12 as seq from DUAL  
                union all select 'No. of Company', 0, 0, 0, 13 as seq from dual)  
                group by code order by seq";

            //string sql = @"select code, sum(yes_cnt) as yes_cnt, sum(no_cnt) as no_cnt, sum(no_ind_cnt) as no_ind_cnt,  
            //    sum(seq) as seq,to_char(to_date(:today,'dd/mm/yyyy'),'dd/mm/yyyy') AS AS_AT_TODAY,'CountForSERVICE_IN_MWIS' AS WILLINGNESS_QP from  
            //    ( select ip_yes.code, ip_yes.count as yes_cnt, ip_no.count as no_cnt, ip_no_ind.count as no_ind_cnt, 0 AS seq from( (SELECT code, sum(count) AS count, 0 AS seq FROM(  
            //    select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID  
            //    and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
            //    and scatgrp.code in ('AP', 'RSE', 'RI') and SERVICE_IN_MWIS = 'Y' group by scatgrp.code  
            //    UNION ALL SELECT 'AP', 0, 0 FROM DUAL  
            //    UNION ALL SELECT 'RSE', 0, 0 FROM DUAL  
            //    UNION ALL SELECT 'RI', 0, 0 FROM DUAL) GROUP BY code ) ip_yes  
            //    LEFT JOIN (select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and SERVICE_IN_MWIS = 'N'  
            //    group by scatgrp.code) ip_no on ip_yes.code = ip_no.code  
            //    LEFT JOIN (select scatgrp.code, count(*) as count, 0 as seq from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and(SERVICE_IN_MWIS = 'I' or SERVICE_IN_MWIS is null)  
            //    group by scatgrp.code) ip_no_ind on ip_yes.code = ip_no_ind.code)  
            //    union all  
            //    select cgc_yes.code, cgc_yes.count, cgc_no.count, cgc_no_ind.count, 0 AS seq from(( 
            //    SELECT code, sum(count) AS count, 0 AS seq from(select 'RGBC' AS code, count(*) as count, 0 as seq FROM(SELECT DISTINCT c.FILE_REFERENCE_NO  
            //    from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID  
            //    AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and SERVICE_IN_MWIS = 'Y')  
            //    UNION ALL  
            //    SELECT 'RGBC', 0, 0 FROM DUAL) GROUP BY code) cgc_yes LEFT JOIN (select 'RGBC' AS code, count(*) as count, 0 as seq FROM( 
            //     SELECT DISTINCT c.FILE_REFERENCE_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'  
            //    AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and SERVICE_IN_MWIS = 'N')) cgc_no  
            //    on cgc_yes.code = cgc_no.code LEFT JOIN (select 'RGBC' AS code, count(*) as count, 0 as seq  
            //    FROM( SELECT DISTINCT c.FILE_REFERENCE_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat  
            //    where c.REGISTRATION_TYPE = 'CGC' AND c.APPLICATION_STATUS_ID = status.UUID AND scat.UUID = c.CATEGORY_ID AND scat.CODE = 'GBC' 
            //    and c.CERTIFICATION_NO IS NOT NULL and(SERVICE_IN_MWIS = 'I' or SERVICE_IN_MWIS is null) )) cgc_no_ind on cgc_yes.code = cgc_no_ind.code)  
            //    union all  
            //    select imw_yes.code, imw_yes.count, imw_no.count, imw_no_ind.count, 0 AS seq from(( SELECT code, sum(count) AS count, 0 AS seq from(  
            //    select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status  
            //    where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
            //    where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and SERVICE_IN_MWIS = 'Y' group by iapp.REGISTRATION_TYPE  
            //    UNION ALL  
            //    SELECT 'RMWC(Individual) - Item 3.6', 0, 0 FROM DUAL) GROUP BY code) imw_yes  
            //    LEFT JOIN (select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status  
            //    where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
            //    and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
            //    where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and SERVICE_IN_MWIS = 'N'  
            //    group by iapp.REGISTRATION_TYPE) imw_no on imw_yes.code = imw_no.code  
            //    LEFT JOIN (select 'RMWC(Individual) - Item 3.6' AS code, count(*) as count, 0 as seq from C_ind_application iapp, C_ind_certificate c, C_s_system_value status  
            //    where iapp.uuid = c.MASTER_ID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
            //    and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6'))  
            //    and(SERVICE_IN_MWIS = 'I' or SERVICE_IN_MWIS is null) group by iapp.REGISTRATION_TYPE) imw_no_ind on imw_yes.code = imw_no_ind.code)  
            //    union all select cmw_yes.code, cmw_yes.count, cmw_no.count, cmw_no_ind.count, 0 AS seq from((SELECT code, sum(count) AS count, 0 AS seq FROM(  
            //    select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO  
            //    from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
            //    and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
            //    and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in ( select cmw.company_applicants_id  
            //    from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and SERVICE_IN_MWIS = 'Y')  
            //    UNION ALL SELECT 'RMWC(Company) - Type A', 0, 0 FROM DUAL) GROUP BY code) cmw_yes  
            //    LEFT JOIN (select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO  
            //    from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id  
            //    and app_info.applicant_role_id = s_role.uuid and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1'  
            //    and app_info.accept_date is not null and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
            //    (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A'))  
            //    and SERVICE_IN_MWIS = 'N')) cmw_no on cmw_yes.code = cmw_no.code  
            //    LEFT JOIN (select 'RMWC(Company) - Type A' AS code, count(*) as count, 0 as seq FROM( SELECT DISTINCT c.FILE_REFERENCE_NO  
            //    from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
            //    and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
            //    and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
            //    ( select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A'))  
            //    and(SERVICE_IN_MWIS = 'I' or SERVICE_IN_MWIS is null) )) cmw_no_ind on cmw_yes.code = cmw_no_ind.code)  
            //    UNION ALL SELECT 'No. of Person' AS code, ind_yes.count, ind_no.count, ind_no_ind.count, 0 FROM((SELECT count(DISTINCT hkid) AS count FROM( select hkid || passport_no AS hkid  
            //    from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID  
            //    and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
            //    and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem where sitem.uuid = iitem.ITEM_DETAILS_ID  
            //    and sitem.code in ('Item 3.6')) and SERVICE_IN_MWIS = 'Y'group by iapp.REGISTRATION_TYPE, hkid || passport_no  
            //    UNION ALL  
            //    select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI') and SERVICE_IN_MWIS = 'Y')) ind_yes  
            //    LEFT JOIN ( SELECT count(DISTINCT hkid) AS count FROM( select hkid || passport_no AS hkid from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status  
            //    where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
            //    where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in ('Item 3.6')) and SERVICE_IN_MWIS = 'N'  
            //    group by iapp.REGISTRATION_TYPE, hkid || passport_no  
            //    UNION ALL  
            //    select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID  
            //    AND app.UUID = iapp.APPLICANT_ID and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL))  
            //    and scatgrp.code in ('AP', 'RSE', 'RI') and SERVICE_IN_MWIS = 'N'))ind_no ON 1 = 1  
            //    LEFT JOIN( SELECT count(DISTINCT hkid) AS count FROM ( select hkid || passport_no AS hkid from C_ind_application iapp, C_ind_certificate c, C_APPLICANT app, C_s_system_value status  
            //    where iapp.uuid = c.MASTER_ID AND iapp.APPLICANT_ID = app.UUID and iapp.REGISTRATION_TYPE = 'IMW' and status.uuid = c.application_status_id  
            //    and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and iapp.uuid in (select master_id from C_ind_application_mw_item iitem, C_s_system_value sitem  
            //    where sitem.uuid = iitem.ITEM_DETAILS_ID and sitem.code in('Item 3.6')) and(SERVICE_IN_MWIS = 'I' OR SERVICE_IN_MWIS IS NULL) group by iapp.REGISTRATION_TYPE, hkid || passport_no  
            //    UNION ALL select hkid || passport_no AS hkid from C_ind_certificate c, C_s_category_code scat, C_s_system_value scatgrp, C_ind_application iapp, C_APPLICANT app, C_s_system_value status  
            //    where c.CATEGORY_ID = scat.UUID and scat.CATEGORY_GROUP_ID = scatgrp.UUID and iapp.uuid = c.MASTER_ID AND app.UUID = iapp.APPLICANT_ID  
            //    and status.uuid = c.application_status_id and(status.code = '1' or(c.RETENTION_APPLICATION_DATE IS NOT NULL)) and scatgrp.code in ('AP', 'RSE', 'RI')  
            //    and(SERVICE_IN_MWIS = 'I' OR SERVICE_IN_MWIS IS NULL)) )ind_no_ind ON 1 = 1)  
            //    UNION ALL SELECT 'No. of Company' AS code, comp_yes.count, comp_no.count, comp_no_ind.count, 0 FROM( (SELECT count(DISTINCT br_no) AS count FROM(select c.BR_NO  
            //    from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC' and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID  
            //    AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and SERVICE_IN_MWIS = 'Y'  
            //    group by c.REGISTRATION_TYPE, c.br_no  
            //    UNION ALL  
            //    select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id  
            //    and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid  
            //    and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null and((app_info.removal_date is null) or(app_info.removal_date < current_date))  
            //    and c.certification_no is not null and app_info.uuid in ( select cmw.company_applicants_id from C_comp_applicant_mw_item cmw  
            //    inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and SERVICE_IN_MWIS = 'Y' group by c.REGISTRATION_TYPE, c.br_no)) comp_yes  
            //    LEFT JOIN ( SELECT count(DISTINCT br_no) AS count FROM( select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'  
            //    and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and SERVICE_IN_MWIS = 'N'  
            //    group by c.REGISTRATION_TYPE, c.br_no  
            //    UNION ALL select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
            //    and app_info.applicant_status_id = s_status.uuid  
            //    and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
            //    and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
            //    (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw  
            //    inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in ('Type A')) and SERVICE_IN_MWIS = 'N'  
            //    group by c.REGISTRATION_TYPE, c.br_no))comp_no ON 1 = 1  
            //    LEFT JOIN(SELECT count(DISTINCT br_no) AS count FROM (select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_category_code scat where c.REGISTRATION_TYPE = 'CGC'  
            //    and status.uuid = c.application_status_id AND c.CATEGORY_ID = scat.UUID AND scat.CODE = 'GBC' and c.CERTIFICATION_NO IS NOT NULL and (SERVICE_IN_MWIS = 'I' OR SERVICE_IN_MWIS IS NULL)  
            //    group by c.REGISTRATION_TYPE, c.br_no  
            //    UNION ALL  
            //    select c.BR_NO from C_comp_application c, C_s_system_value status, C_s_system_value s_role, C_comp_applicant_info app_info, C_s_system_value s_status, C_applicant app  
            //    where c.REGISTRATION_TYPE = 'CMW' and status.uuid = c.application_status_id and c.uuid = app_info.master_id and app_info.applicant_role_id = s_role.uuid  
            //    and app_info.applicant_status_id = s_status.uuid and app_info.applicant_id = app.uuid and s_role.code like 'A%' and s_status.code = '1' and app_info.accept_date is not null  
            //    and((app_info.removal_date is null) or(app_info.removal_date < current_date)) and c.certification_no is not null and app_info.uuid in  
            //    (select cmw.company_applicants_id from C_comp_applicant_mw_item cmw inner join C_s_system_value mwcode on cmw.item_type_id = mwcode.uuid where mwcode.code in('Type A'))  
            //    and(SERVICE_IN_MWIS = 'I' OR SERVICE_IN_MWIS IS NULL) group by c.REGISTRATION_TYPE, c.br_no))comp_no_ind ON 1 = 1)  
            //    union all select 'AP', 0, 0, 0, 1 as seq from dual  
            //    union all select 'RSE', 0, 0, 0, 2 as seq from dual  
            //    union all select 'RI', 0, 0, 0, 3 as seq from dual 
            //     union all select 'RGBC', 0, 0, 0, 4 as seq from dual  
            //    union all select 'RMWC(Company) - Type A', 0, 0, 0, 7 as seq from dual  
            //    union all select 'RMWC(Individual) - Item 3.6', 0, 0, 0, 10 as seq from dual  
            //    union all select 'No. of Person', 0, 0, 0, 12 as seq from DUAL  
            //    union all select 'No. of Company', 0, 0, 0, 13 as seq from dual)  
            //    group by code order by seq";

            return sql;
        }

        //CRM0091
        private string getCheckMultiReg()
        {
            string sql = @"(
                    SELECT 'COMP' AS TYPE, cappl.br_no, cappl.english_company_name, cappl.file_reference_no, '' AS hkid, '' AS name, '' as role, '' as code, 1 as seq, to_char(cappl.GAZETTE_DATE, 'dd/mm/yyyy') AS GAZETTE_DATE
                    FROM C_comp_application cappl
                    WHERE cappl.BR_NO || cappl.category_id IN (
                    SELECT capp.br_no || capp.category_id FROM C_comp_application capp, C_S_SYSTEM_VALUE status
                    where capp.REGISTRATION_TYPE = 'CGC'
                    and capp.APPLICATION_STATUS_ID = status.uuid
                    and status.code = '1'
                    and ((capp.EXPIRY_DATE IS NOT NULL and capp.EXPIRY_DATE >= CURRENT_DATE ) or
                    (capp.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and (capp.EXPIRY_DATE < CURRENT_DATE)))
                    GROUP BY capp.BR_NO, capp.CATEGORY_ID
                    HAVING count(*) > 1)

                    union all

                    SELECT 'CMW' AS TYPE, cappl.br_no, cappl.english_company_name, cappl.file_reference_no, '' AS hkid, '' AS name, '' as role, '' as code, 2 as seq, to_char(cappl.GAZETTE_DATE, 'dd/mm/yyyy') AS GAZETTE_DATE
                    FROM C_comp_application cappl
                    WHERE cappl.BR_NO || cappl.category_id IN (
                    SELECT capp.br_no || capp.category_id FROM C_comp_application capp, C_S_SYSTEM_VALUE status
                    where capp.REGISTRATION_TYPE = 'CMW'
                    and capp.APPLICATION_STATUS_ID = status.uuid
                    and status.code = '1'
                    and ((capp.EXPIRY_DATE IS NOT NULL and capp.EXPIRY_DATE >= CURRENT_DATE ) or
                    (capp.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and (capp.EXPIRY_DATE < CURRENT_DATE)))
                    GROUP BY capp.BR_NO, capp.CATEGORY_ID
                    HAVING count(*) > 1)

                    )UNION ALL(


                    SELECT 'MWC_MWCP' AS TYPE, cappl.br_no, cappl.english_company_name, cappl.file_reference_no, '' AS hkid, '' AS name, '' as role, '' as code, 3 as seq, to_char(cappl.GAZETTE_DATE, 'dd/mm/yyyy') AS GAZETTE_DATE
                    FROM C_comp_application cappl, C_S_CATEGORY_CODE scat
                    WHERE cappl.BR_NO IN (
                    SELECT capp.BR_NO FROM C_COMP_APPLICATION capp, C_S_SYSTEM_VALUE status, C_S_CATEGORY_CODE scat
                    WHERE capp.APPLICATION_STATUS_ID = status.UUID
                    AND capp.CATEGORY_ID = scat.UUID
                    AND scat.CODE = 'MWC'
                    AND status.CODE = '1'
                    AND ((capp.EXPIRY_DATE IS NOT NULL and capp.EXPIRY_DATE >= CURRENT_DATE ) or
                    (capp.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and (capp.EXPIRY_DATE < CURRENT_DATE)))

                    INTERSECT

                    SELECT capp.BR_NO FROM C_COMP_APPLICATION capp, C_S_SYSTEM_VALUE status, C_S_CATEGORY_CODE scat
                    WHERE capp.APPLICATION_STATUS_ID = status.UUID
                    AND capp.CATEGORY_ID = scat.UUID
                    AND scat.CODE = 'MWC(P)'
                    AND status.CODE = '1'
                    AND ((capp.EXPIRY_DATE IS NOT NULL and capp.EXPIRY_DATE >= CURRENT_DATE ) or
                    (capp.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and (capp.EXPIRY_DATE < CURRENT_DATE)))
                    )
                    AND cappl.CATEGORY_ID = scat.UUID
                    AND scat.CODE IN ('MWC', 'MWC(P)')


                    )UNION ALL(


                    SELECT 'IPERSON_IP' AS TYPE, '' AS br_no, '' AS english_company_name, iapp.file_reference_no,
                    C_DECRYPT(hkid, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') || C_DECRYPT(passport_no, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') AS hkid, surname || ' ' || given_name_on_id as name, '' as role, scat.code, 4 as seq, to_char(icer.GAZETTE_DATE, 'dd/mm/yyyy') AS GAZETTE_DATE
                    FROM C_IND_APPLICATION iapp, C_APPLICANT app, C_IND_CERTIFICATE icer, C_s_category_code scat
                    WHERE iapp.APPLICANT_ID = app.UUID
                    and iapp.REGISTRATION_TYPE = 'IP'
                    and iapp.uuid = icer.master_id
                    and scat.uuid = icer.category_id

                    AND
                    hkid || passport_no IN
                    (
                    SELECT hkid || passport_no AS hkid
                    FROM C_ind_application iapp, C_APPLICANT app, C_IND_CERTIFICATE icer, C_S_SYSTEM_VALUE status
                    WHERE iapp.APPLICANT_ID = app.uuid
                    and iapp.REGISTRATION_TYPE = 'IP'
                    AND icer.APPLICATION_STATUS_ID = status.UUID
                    and iapp.uuid = icer.master_id
                    AND status.CODE = '1'
                    GROUP BY hkid || passport_no, iapp.REGISTRATION_TYPE
                    HAVING count(DISTINCT iapp.uuid) > 1

                    UNION ALL

                    select hkid || passport_no
                    from C_IND_CERTIFICATE icer, C_IND_APPLICATION iapp, C_APPLICANT app
                    where iapp.APPLICANT_ID = app.UUID
                    and iapp.uuid = icer.master_id
                    and iapp.REGISTRATION_TYPE = 'IP'
                    AND iapp.UUID IN (
                    SELECT icer.MASTER_ID
                    from C_IND_CERTIFICATE icer, C_S_SYSTEM_VALUE status
                    where status.uuid = icer.application_status_id
                    and status.code = '1'
                    GROUP by icer.MASTER_ID, icer.CATEGORY_ID
                    HAVING count(*) > 1
                    )
                    group by hkid || passport_no
                    having count(*) > 1
                    )

                    )UNION ALL(


                    SELECT 'IPERSON_IMW' AS TYPE, '' AS br_no, '' AS english_company_name, iapp.file_reference_no,
                    C_DECRYPT(hkid, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') || C_DECRYPT(passport_no, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') AS hkid, surname || ' ' || given_name_on_id as name, '' as role, scat.code, 5 as seq, to_char(icer.GAZETTE_DATE, 'dd/mm/yyyy') AS GAZETTE_DATE
                    FROM C_IND_APPLICATION iapp, C_APPLICANT app, C_IND_CERTIFICATE icer, C_s_category_code scat
                    WHERE iapp.APPLICANT_ID = app.UUID
                    and iapp.REGISTRATION_TYPE = 'IMW'
                    and iapp.uuid = icer.master_id
                    and scat.uuid = icer.category_id
                    AND
                    hkid || passport_no IN
                    (
                    SELECT hkid || passport_no AS hkid FROM C_ind_application iapp, C_APPLICANT app, C_IND_CERTIFICATE icer, C_S_SYSTEM_VALUE status
                    WHERE iapp.APPLICANT_ID = app.uuid
                    AND icer.APPLICATION_STATUS_ID = status.UUID
                    AND status.CODE = '1'
                    and iapp.REGISTRATION_TYPE = 'IMW'
                    AND icer.MASTER_ID = iapp.UUID
                    GROUP BY hkid || passport_no, iapp.REGISTRATION_TYPE
                    HAVING count(DISTINCT iapp.uuid) > 1

                    UNION ALL

                    select hkid || passport_no
                    from C_IND_CERTIFICATE icer, C_IND_APPLICATION iapp, C_APPLICANT app
                    where iapp.APPLICANT_ID = app.UUID
                    and iapp.uuid = icer.master_id
                    and iapp.REGISTRATION_TYPE = 'IMW'
                    AND iapp.UUID IN (
                    SELECT icer.MASTER_ID from C_IND_CERTIFICATE icer, C_S_SYSTEM_VALUE status
                    where status.uuid = icer.application_status_id
                    and status.code = '1'
                    GROUP by icer.MASTER_ID, icer.CATEGORY_ID
                    HAVING count(*) > 1
                    )
                    group by hkid || passport_no
                    having count(*) > 1
                    )

                    )UNION ALL(


                    SELECT 'MWI_MWC_AS' AS TYPE, '' AS br_no, '' AS english_company_name, imw.file_reference_no, imw.hkid, imw.name, '' as role, '' AS code, 6 AS seq, GAZETTE_DATE
                    FROM (
                    SELECT iapp.FILE_REFERENCE_NO, C_DECRYPT(hkid, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') || C_DECRYPT(passport_no, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') AS hkid, surname || ' ' || given_name_on_id as name, to_char(icer.GAZETTE_DATE, 'dd/mm/yyyy') AS GAZETTE_DATE
                    FROM C_IND_APPLICATION iapp, C_APPLICANT app, C_IND_CERTIFICATE icer, C_S_SYSTEM_VALUE status
                    WHERE iapp.APPLICANT_ID = app.UUID
                    AND icer.MASTER_ID = iapp.UUID
                    AND icer.APPLICATION_STATUS_ID = status.UUID
                    AND iapp.REGISTRATION_TYPE = 'IMW'
                    AND status.CODE = '1'
                    ) imw
                    INNER JOIN
                    (
                    SELECT capp.FILE_REFERENCE_NO, C_DECRYPT(hkid, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') || C_DECRYPT(passport_no, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') AS hkid
                    FROM C_COMP_APPLICATION capp, C_COMP_APPLICANT_INFO cinfo, C_APPLICANT app,
                    C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE comp_type, C_S_SYSTEM_VALUE status, C_S_SYSTEM_VALUE app_status
                    WHERE capp.UUID = cinfo.MASTER_ID
                    AND app.UUID = cinfo.APPLICANT_ID
                    AND srole.UUID = cinfo.APPLICANT_ROLE_ID
                    AND comp_type.UUID = capp.COMPANY_TYPE_ID
                    AND app_status.UUID = cinfo.APPLICANT_STATUS_ID
                    AND status.UUID = capp.APPLICATION_STATUS_ID
                    AND ((srole.CODE = 'AS' AND comp_type.CODE IN ('2', '3')) OR (srole.CODE = 'TD' AND comp_type.CODE = '1'))
                    AND status.CODE = '1'
                    AND app_status.CODE = '1'
                    AND capp.REGISTRATION_TYPE = 'CMW'
                    AND ((capp.EXPIRY_DATE IS NOT NULL and capp.EXPIRY_DATE >= CURRENT_DATE ) or
                    (capp.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and (capp.EXPIRY_DATE < CURRENT_DATE)))
                    ) cmw ON imw.hkid = cmw.hkid

                    UNION

                    SELECT 'MWI_MWC_AS' AS TYPE, '' AS br_no, '' AS english_company_name, cmw.file_reference_no, cmw.hkid, cmw.name, '' as role, '' AS code, 6 AS seq, GAZETTE_DATE
                    FROM (
                    SELECT iapp.FILE_REFERENCE_NO, C_DECRYPT(hkid, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') || C_DECRYPT(passport_no, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') AS hkid
                    FROM C_IND_APPLICATION iapp, C_APPLICANT app, C_IND_CERTIFICATE icer, C_S_SYSTEM_VALUE status
                    WHERE iapp.APPLICANT_ID = app.UUID
                    AND icer.MASTER_ID = iapp.UUID
                    AND icer.APPLICATION_STATUS_ID = status.UUID
                    AND iapp.REGISTRATION_TYPE = 'IMW'
                    AND status.CODE = '1'
                    ) imw
                    INNER JOIN
                    (
                    SELECT capp.FILE_REFERENCE_NO, C_DECRYPT(hkid, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') || C_DECRYPT(passport_no, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') AS hkid, surname || ' ' || given_name_on_id as name, to_char(capp.GAZETTE_DATE, 'dd/mm/yyyy') AS GAZETTE_DATE
                    FROM C_COMP_APPLICATION capp, C_COMP_APPLICANT_INFO cinfo, C_APPLICANT app,
                    C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE comp_type, C_S_SYSTEM_VALUE status, C_S_SYSTEM_VALUE app_status
                    WHERE capp.UUID = cinfo.MASTER_ID
                    AND app.UUID = cinfo.APPLICANT_ID
                    AND srole.UUID = cinfo.APPLICANT_ROLE_ID
                    AND comp_type.UUID = capp.COMPANY_TYPE_ID
                    AND app_status.UUID = cinfo.APPLICANT_STATUS_ID
                    AND status.UUID = capp.APPLICATION_STATUS_ID
                    AND ((srole.CODE = 'AS' AND comp_type.CODE IN ('2', '3')) OR (srole.CODE = 'TD' AND comp_type.CODE = '1'))
                    AND status.CODE = '1'
                    AND app_status.CODE = '1'
                    AND capp.REGISTRATION_TYPE = 'CMW'
                    AND ((capp.EXPIRY_DATE IS NOT NULL and capp.EXPIRY_DATE >= CURRENT_DATE ) or
                    (capp.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and (capp.EXPIRY_DATE < CURRENT_DATE)))
                    ) cmw ON imw.hkid = cmw.hkid

                    )UNION ALL(

                    SELECT 'MWI_GBC_AS' AS TYPE, '' AS br_no, '' AS english_company_name, imw.file_reference_no, imw.hkid, imw.name, '' as role, '' AS code, 7 AS seq, GAZETTE_DATE
                    FROM (
                    SELECT iapp.FILE_REFERENCE_NO, C_DECRYPT(hkid, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') || C_DECRYPT(passport_no, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') AS hkid, surname || ' ' || given_name_on_id as name, to_char(icer.GAZETTE_DATE, 'dd/mm/yyyy') AS GAZETTE_DATE
                    FROM C_IND_APPLICATION iapp, C_APPLICANT app, C_IND_CERTIFICATE icer, C_S_SYSTEM_VALUE status
                    WHERE iapp.APPLICANT_ID = app.UUID
                    AND icer.MASTER_ID = iapp.UUID
                    AND icer.APPLICATION_STATUS_ID = status.UUID
                    AND iapp.REGISTRATION_TYPE = 'IMW'
                    AND status.CODE = '1'
                    ) imw
                    INNER JOIN
                    (
                    SELECT capp.FILE_REFERENCE_NO, C_DECRYPT(hkid, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') || C_DECRYPT(passport_no, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') AS hkid, surname || ' ' || given_name_on_id as name
                    FROM C_COMP_APPLICATION capp, C_COMP_APPLICANT_INFO cinfo, C_APPLICANT app,
                    C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE comp_type, C_S_SYSTEM_VALUE status, C_S_SYSTEM_VALUE app_status
                    WHERE capp.UUID = cinfo.MASTER_ID
                    AND app.UUID = cinfo.APPLICANT_ID
                    AND srole.UUID = cinfo.APPLICANT_ROLE_ID
                    AND comp_type.UUID = capp.COMPANY_TYPE_ID
                    AND app_status.UUID = cinfo.APPLICANT_STATUS_ID
                    AND status.UUID = capp.APPLICATION_STATUS_ID
                    AND ((srole.CODE = 'AS' AND comp_type.CODE IN ('2', '3')) OR (srole.CODE = 'TD' AND comp_type.CODE = '1'))
                    AND status.CODE = '1'
                    AND app_status.CODE = '1'
                    AND capp.REGISTRATION_TYPE = 'CGC'
                    AND ((capp.EXPIRY_DATE IS NOT NULL and capp.EXPIRY_DATE >= CURRENT_DATE ) or
                    (capp.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and (capp.EXPIRY_DATE < CURRENT_DATE)))
                    ) gbc ON imw.hkid = gbc.hkid

                    UNION

                    SELECT 'MWI_GBC_AS' AS TYPE, '' AS br_no, '' AS english_company_name, gbc.file_reference_no, gbc.hkid, gbc.name, '' as role, '' AS code, 7 AS seq, GAZETTE_DATE
                    FROM (
                    SELECT iapp.FILE_REFERENCE_NO, C_DECRYPT(hkid, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') || C_DECRYPT(passport_no, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') AS hkid
                    FROM C_IND_APPLICATION iapp, C_APPLICANT app, C_IND_CERTIFICATE icer, C_S_SYSTEM_VALUE status
                    WHERE iapp.APPLICANT_ID = app.UUID
                    AND icer.MASTER_ID = iapp.UUID
                    AND icer.APPLICATION_STATUS_ID = status.UUID
                    AND iapp.REGISTRATION_TYPE = 'IMW'
                    AND status.CODE = '1'
                    ) imw
                    INNER JOIN
                    (
                    SELECT capp.FILE_REFERENCE_NO, C_DECRYPT(hkid, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') || C_DECRYPT(passport_no, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') AS hkid, surname || ' ' || given_name_on_id as name, to_char(capp.GAZETTE_DATE, 'dd/mm/yyyy') AS GAZETTE_DATE
                    FROM C_COMP_APPLICATION capp, C_COMP_APPLICANT_INFO cinfo, C_APPLICANT app,
                    C_S_SYSTEM_VALUE srole, C_S_SYSTEM_VALUE comp_type, C_S_SYSTEM_VALUE status, C_S_SYSTEM_VALUE app_status
                    WHERE capp.UUID = cinfo.MASTER_ID
                    AND app.UUID = cinfo.APPLICANT_ID
                    AND srole.UUID = cinfo.APPLICANT_ROLE_ID
                    AND comp_type.UUID = capp.COMPANY_TYPE_ID
                    AND app_status.UUID = cinfo.APPLICANT_STATUS_ID
                    AND status.UUID = capp.APPLICATION_STATUS_ID
                    AND ((srole.CODE = 'AS' AND comp_type.CODE IN ('2', '3')) OR (srole.CODE = 'TD' AND comp_type.CODE = '1'))
                    AND status.CODE = '1'
                    AND app_status.CODE = '1'
                    AND capp.REGISTRATION_TYPE = 'CGC'
                    AND ((capp.EXPIRY_DATE IS NOT NULL and capp.EXPIRY_DATE >= CURRENT_DATE ) or
                    (capp.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and (capp.EXPIRY_DATE < CURRENT_DATE)))
                    ) gbc ON imw.hkid = gbc.hkid

                    )

                    order by seq, type, hkid, br_no, file_reference_no";

            return sql;
        }
        //CRM0092
        private string getRPT0002IMW()
        {
            string sql = @"select 
layout.code  AS S_MW_DETAILS_CODE,
nvl(datac.S_MW_DETAILS_UUID, layout.code) AS S_MW_DETAILS_UUID,
nvl(datac.TOTAL,0) as TOTAL,
nvl(datac.TOTAL2,0)as TOTAL2,
  C_RPT0002_IMW_TOTAL3 (:reg_type, 'Class 3') AS TOTAL3
from 
(SELECT distinct s.code FROM C_s_system_value s where code like 'Item 3.%') layout left outer join
(    SELECT
     S_MW_DETAILS.CODE AS S_MW_DETAILS_CODE,
     S_MW_DETAILS.UUID AS S_MW_DETAILS_UUID,
     COUNT (*) AS TOTAL,
     C_RPT0002_IMW_TOTAL2 (:reg_type, 'Class 3', S_MW_DETAILS.UUID)as TOTAL2,
     C_RPT0002_IMW_TOTAL3 (:reg_type, 'Class 3') AS TOTAL3

FROM
     C_IND_CERTIFICATE I_CERT INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID
     INNER JOIN C_IND_APPLICATION_MW_ITEM I_MW_ITEM ON I_APPL.UUID = I_MW_ITEM.MASTER_ID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_CLASS ON I_MW_ITEM.ITEM_CLASS_ID = S_MW_CLASS.UUID
     right JOIN C_S_SYSTEM_VALUE S_MW_DETAILS ON I_MW_ITEM.ITEM_DETAILS_ID = S_MW_DETAILS.UUID
WHERE
     I_APPL.REGISTRATION_TYPE = :reg_type
     AND S_MW_CLASS.CODE = 'Class 3'
     AND I_CERT.CERTIFICATION_NO IS NOT NULL
     and to_char(I_CERT.EXPIRY_DATE, 'yyyymmdd') >= to_char(sysdate, 'yyyymmdd')
     and (I_CERT.REMOVAL_DATE IS NULL
     or (to_char(I_CERT.REMOVAL_DATE, 'yyyymmdd') > to_char(I_CERT.EXPIRY_DATE)))

GROUP BY 
     S_MW_DETAILS.CODE, S_MW_DETAILS.UUID
     )
          datac on layout.code = datac.S_MW_DETAILS_CODE
  order by  CAST(substr(layout.code,8) as int)";
            return sql;
        }
        //CRM0093
        private string getRPT0002_CMW()
        {
            string sql = @"SELECT
MW_TYPE_CODE,
ROLE_CODE,
ENG_ROLE,
CLASS1,
CLASS1_T2,
CLASS2,
CLASS2_T2,
CLASS3,
CLASS3_T2,
CASE WHEN ROLE_CODE = 'A1' THEN C_RPT0002_CMW_CORPORATIONS('Class 1')
WHEN ROLE_CODE = 'Z1' THEN C_RPT0002_CMW_CORPORATIONS('Class 1')
WHEN ROLE_CODE IN ('OO','AS','TD') THEN C_RPT0002_CMW_ASTDOO(ROLE_CODE,'Class 1')
END AS H1,
CASE WHEN ROLE_CODE = 'A1' THEN C_RPT0002_CMW_CORPORATIONS('Class 2')
WHEN ROLE_CODE = 'Z1' THEN C_RPT0002_CMW_CORPORATIONS('Class 2')
WHEN ROLE_CODE IN ('OO','AS','TD') THEN C_RPT0002_CMW_ASTDOO(ROLE_CODE,'Class 2')
END AS H2,
CASE WHEN ROLE_CODE = 'A1' THEN C_RPT0002_CMW_CORPORATIONS('Class 3')
WHEN ROLE_CODE = 'Z1' THEN C_RPT0002_CMW_CORPORATIONS('Class 3')
WHEN ROLE_CODE IN ('OO','AS','TD') THEN C_RPT0002_CMW_ASTDOO(ROLE_CODE,'Class 3')
END AS H3

FROM
(
--UNION 1

(SELECT
A2.MW_TYPE_CODE,
A2.ROLE_CODE,
A2.ENG_ROLE,
CASE WHEN SUM(A1.CLASS1) IS NULL THEN 0 ELSE SUM(A1.CLASS1) END AS CLASS1,
CASE WHEN SUM(A11.CLASS1_T2) IS NULL THEN 0 ELSE SUM(A11.CLASS1_T2) END AS CLASS1_T2,
CASE WHEN SUM(A1.CLASS2) IS NULL THEN 0 ELSE SUM(A1.CLASS2) END AS CLASS2,
CASE WHEN SUM(A11.CLASS2_T2) IS NULL THEN 0 ELSE SUM(A11.CLASS2_T2) END AS CLASS2_T2,
CASE WHEN SUM(A1.CLASS3) IS NULL THEN 0 ELSE SUM(A1.CLASS3) END AS CLASS3,
CASE WHEN SUM(A11.CLASS3_T2) IS NULL THEN 0 ELSE SUM(A11.CLASS3_T2) END AS CLASS3_T2
FROM
(SELECT
     C_APPLN.APPLICANT_ROLE_ID AS ROLE_ID,
     S_VAL_A_ROLE.CODE AS ROLE_CODE,
     S_MW_TYPE.UUID AS MW_TYPE_ID,
     S_MW_TYPE.CODE AS MW_TYPE_CODE,
     S_MW_CLASS.UUID AS MW_CLASS_ID,
     S_MW_CLASS.CODE AS MW_CLASS_CODE,
     CASE WHEN S_MW_CLASS.CODE = 'Class 1' Then COUNT(*) End AS CLASS1,
     CASE WHEN S_MW_CLASS.CODE = 'Class 2' Then COUNT(*) End AS CLASS2,
     CASE WHEN S_MW_CLASS.CODE = 'Class 3' Then COUNT(*) End AS CLASS3
FROM
     C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
     INNER JOIN C_S_SYSTEM_VALUE S_VAL_A_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_VAL_A_ROLE.UUID
     INNER JOIN C_COMP_APPLICANT_MW_ITEM C_APPLN_MW ON C_APPLN.UUID = C_APPLN_MW.COMPANY_APPLICANTS_ID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_TYPE ON C_APPLN_MW.ITEM_TYPE_ID = S_MW_TYPE.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_CLASS ON C_APPLN_MW.ITEM_CLASS_ID = S_MW_CLASS.UUID
WHERE
     C_APPLN.ACCEPT_DATE is not null
     AND C_APPL.CERTIFICATION_NO is not null
     AND to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd')
     and (C_APPLN.REMOVAL_DATE IS null
     or (C_APPLN.REMOVAL_DATE IS NOT null
     and to_char(C_APPLN.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd')))
     and (C_APPL.REMOVAL_DATE IS null
     or (to_char(C_APPL.REMOVAL_DATE, 'yyyymmdd') > to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd')
     and to_char(C_APPL.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd')))
     and S_VAL_A_ROLE.CODE = 'AS'
     AND C_APPL.REGISTRATION_TYPE = 'CMW'

GROUP BY
     C_APPLN.APPLICANT_ROLE_ID,
     S_VAL_A_ROLE.CODE,
     S_MW_TYPE.UUID,
     S_MW_TYPE.CODE,
     S_MW_CLASS.UUID,
     S_MW_CLASS.CODE ) A1

LEFT JOIN
(SELECT
     C_APPLN.APPLICANT_ROLE_ID AS ROLE_ID,
     S_MW_TYPE.UUID AS MW_TYPE_ID,
     S_MW_CLASS.UUID AS MW_CLASS_ID,
     S_MW_CLASS.CODE AS MW_CLASS_CODE,
     CASE WHEN S_MW_CLASS.CODE = 'Class 1' Then COUNT(*) End AS CLASS1_T2,
     CASE WHEN S_MW_CLASS.CODE = 'Class 2' Then COUNT(*) End AS CLASS2_T2,
     CASE WHEN S_MW_CLASS.CODE = 'Class 3' Then COUNT(*) End AS CLASS3_T2

FROM
     C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
     INNER JOIN C_S_SYSTEM_VALUE S_VAL_A_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_VAL_A_ROLE.UUID
     INNER JOIN C_COMP_APPLICANT_MW_ITEM C_APPLN_MW ON C_APPLN.UUID = C_APPLN_MW.COMPANY_APPLICANTS_ID
     INNER JOIN C_S_SYSTEM_VALUE S_VAL_STATUS ON C_APPLN.APPLICANT_STATUS_ID = S_VAL_STATUS.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_TYPE ON C_APPLN_MW.ITEM_TYPE_ID = S_MW_TYPE.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_CLASS ON C_APPLN_MW.ITEM_CLASS_ID = S_MW_CLASS.UUID
WHERE
     S_VAL_STATUS.CODE = '1'
     AND C_APPL.CERTIFICATION_NO is not null
     AND C_APPLN.ACCEPT_DATE IS NOT NULL
     and (to_char (sysdate, 'yyyymmdd') > to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd')
     and  to_char(C_APPL.RETENTION_APPLICATION_DATE, 'yyyymmdd') > '20040831' )
     and (C_APPL.REMOVAL_DATE is null or to_char( C_APPL.REMOVAL_DATE, 'yyyymmdd') > to_char( sysdate, 'yyyymmdd'))

GROUP BY
     C_APPLN.APPLICANT_ROLE_ID,
     S_MW_TYPE.UUID,
     S_MW_CLASS.UUID,
     S_MW_CLASS.CODE) A11

ON   A11.ROLE_ID = A1.ROLE_ID AND A11.MW_TYPE_ID = A1.MW_TYPE_ID AND A11.MW_CLASS_ID = A1.MW_CLASS_ID

RIGHT JOIN

(SELECT
     DISTINCT S_VAL1.CODE AS MW_TYPE_CODE, 'AS' AS ROLE_CODE,
     (SELECT MAX(S_VAL1.ENGLISH_DESCRIPTION)
     FROM
     C_S_SYSTEM_VALUE S_VAL1, C_S_SYSTEM_TYPE S_TYPE1
     WHERE S_TYPE1.TYPE='APPLICANT_ROLE'
     AND S_TYPE1.UUID = S_VAL1.SYSTEM_TYPE_ID
     AND S_VAL1.REGISTRATION_TYPE = 'CMW'
     AND S_VAL1.CODE = 'AS') AS ENG_ROLE
     FROM
     C_S_SYSTEM_VALUE S_VAL1, C_S_SYSTEM_TYPE S_TYPE1
     WHERE S_TYPE1.TYPE='MINOR_WORKS_TYPE'
     AND S_TYPE1.UUID = S_VAL1.SYSTEM_TYPE_ID

) A2
ON A2.MW_TYPE_CODE= A1.MW_TYPE_CODE AND A2.ROLE_CODE=A1.ROLE_CODE


GROUP BY
A2.MW_TYPE_CODE,
A2.ROLE_CODE,
A2.ENG_ROLE

)


UNION


--UNION 2

(SELECT
A2.MW_TYPE_CODE,
A2.ROLE_CODE,
A2.ENG_ROLE,
CASE WHEN SUM(A1.CLASS1) IS NULL THEN 0 ELSE SUM(A1.CLASS1) END AS CLASS1,
CASE WHEN SUM(A11.CLASS1_T2) IS NULL THEN 0 ELSE SUM(A11.CLASS1_T2) END AS CLASS1_T2,
CASE WHEN SUM(A1.CLASS2) IS NULL THEN 0 ELSE SUM(A1.CLASS2) END AS CLASS2,
CASE WHEN SUM(A11.CLASS2_T2) IS NULL THEN 0 ELSE SUM(A11.CLASS2_T2) END AS CLASS2_T2,
CASE WHEN SUM(A1.CLASS3) IS NULL THEN 0 ELSE SUM(A1.CLASS3) END AS CLASS3,
CASE WHEN SUM(A11.CLASS3_T2) IS NULL THEN 0 ELSE SUM(A11.CLASS3_T2) END AS CLASS3_T2
FROM
(SELECT
     C_APPLN.APPLICANT_ROLE_ID AS ROLE_ID,
     S_VAL_A_ROLE.CODE AS ROLE_CODE,
     S_MW_TYPE.UUID AS MW_TYPE_ID,
     S_MW_TYPE.CODE AS MW_TYPE_CODE,
     S_MW_CLASS.UUID AS MW_CLASS_ID,
     S_MW_CLASS.CODE AS MW_CLASS_CODE,
     CASE WHEN S_MW_CLASS.CODE = 'Class 1' Then COUNT(*) End AS CLASS1,
     CASE WHEN S_MW_CLASS.CODE = 'Class 2' Then COUNT(*) End AS CLASS2,
     CASE WHEN S_MW_CLASS.CODE = 'Class 3' Then COUNT(*) End AS CLASS3
FROM
     C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
     INNER JOIN C_S_SYSTEM_VALUE S_VAL_A_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_VAL_A_ROLE.UUID
     INNER JOIN C_COMP_APPLICANT_MW_ITEM C_APPLN_MW ON C_APPLN.UUID = C_APPLN_MW.COMPANY_APPLICANTS_ID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_TYPE ON C_APPLN_MW.ITEM_TYPE_ID = S_MW_TYPE.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_CLASS ON C_APPLN_MW.ITEM_CLASS_ID = S_MW_CLASS.UUID
WHERE
     C_APPLN.ACCEPT_DATE is not null
     AND C_APPL.CERTIFICATION_NO is not null
     AND to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd') >= to_char (sysdate, 'yyyymmdd')
     and (C_APPLN.REMOVAL_DATE IS null
     or (C_APPLN.REMOVAL_DATE IS NOT null
     and to_char(C_APPLN.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd')))
     and (C_APPL.REMOVAL_DATE IS null
     or (to_char(C_APPL.REMOVAL_DATE, 'yyyymmdd') > to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd')
     and to_char(C_APPL.REMOVAL_DATE, 'yyyymmdd') > to_char (sysdate, 'yyyymmdd')))
     and S_VAL_A_ROLE.CODE = 'TD'
     AND C_APPL.REGISTRATION_TYPE = 'CMW'

GROUP BY
     C_APPLN.APPLICANT_ROLE_ID,
     S_VAL_A_ROLE.CODE,
     S_MW_TYPE.UUID,
     S_MW_TYPE.CODE,
     S_MW_CLASS.UUID,
     S_MW_CLASS.CODE ) A1

LEFT JOIN
(SELECT
     C_APPLN.APPLICANT_ROLE_ID AS ROLE_ID,
     S_MW_TYPE.UUID AS MW_TYPE_ID,
     S_MW_CLASS.UUID AS MW_CLASS_ID,
     S_MW_CLASS.CODE AS MW_CLASS_CODE,
     CASE WHEN S_MW_CLASS.CODE = 'Class 1' Then COUNT(*) End AS CLASS1_T2,
     CASE WHEN S_MW_CLASS.CODE = 'Class 2' Then COUNT(*) End AS CLASS2_T2,
     CASE WHEN S_MW_CLASS.CODE = 'Class 3' Then COUNT(*) End AS CLASS3_T2

FROM
     C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
     INNER JOIN C_S_SYSTEM_VALUE S_VAL_A_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_VAL_A_ROLE.UUID
     INNER JOIN C_COMP_APPLICANT_MW_ITEM C_APPLN_MW ON C_APPLN.UUID = C_APPLN_MW.COMPANY_APPLICANTS_ID
     INNER JOIN C_S_SYSTEM_VALUE S_VAL_STATUS ON C_APPLN.APPLICANT_STATUS_ID = S_VAL_STATUS.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_TYPE ON C_APPLN_MW.ITEM_TYPE_ID = S_MW_TYPE.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_CLASS ON C_APPLN_MW.ITEM_CLASS_ID = S_MW_CLASS.UUID
WHERE
     S_VAL_STATUS.CODE = '1'
     AND C_APPL.CERTIFICATION_NO is not null
     AND C_APPLN.ACCEPT_DATE IS NOT NULL
     and (to_char(sysdate, 'yyyymmdd') > to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd')
     and  to_char(C_APPL.RETENTION_APPLICATION_DATE, 'yyyymmdd') > '20040831' )
     and (C_APPL.REMOVAL_DATE is null or to_char( C_APPL.REMOVAL_DATE, 'yyyymmdd') > to_char( sysdate, 'yyyymmdd'))

GROUP BY
     C_APPLN.APPLICANT_ROLE_ID,
     S_MW_TYPE.UUID,
     S_MW_CLASS.UUID,
     S_MW_CLASS.CODE) A11

ON   A11.ROLE_ID = A1.ROLE_ID AND A11.MW_TYPE_ID = A1.MW_TYPE_ID AND A11.MW_CLASS_ID = A1.MW_CLASS_ID

RIGHT JOIN

(SELECT
     DISTINCT S_VAL1.CODE AS MW_TYPE_CODE, 'TD' AS ROLE_CODE,
     (SELECT MAX(S_VAL1.ENGLISH_DESCRIPTION)
     FROM
     C_S_SYSTEM_VALUE S_VAL1, C_S_SYSTEM_TYPE S_TYPE1
     WHERE S_TYPE1.TYPE='APPLICANT_ROLE'
     AND S_TYPE1.UUID = S_VAL1.SYSTEM_TYPE_ID
     AND S_VAL1.REGISTRATION_TYPE = 'CMW'
     AND S_VAL1.CODE = 'TD') AS ENG_ROLE
     FROM
     C_S_SYSTEM_VALUE S_VAL1, C_S_SYSTEM_TYPE S_TYPE1
     WHERE S_TYPE1.TYPE='MINOR_WORKS_TYPE'
     AND S_TYPE1.UUID = S_VAL1.SYSTEM_TYPE_ID

) A2
ON A2.MW_TYPE_CODE= A1.MW_TYPE_CODE AND A2.ROLE_CODE=A1.ROLE_CODE


GROUP BY
A2.MW_TYPE_CODE,
A2.ROLE_CODE,
A2.ENG_ROLE

)

UNION
--UNION 4

(SELECT
A2.MW_TYPE_CODE,
A2.ROLE_CODE,
A2.ENG_ROLE,
CASE WHEN SUM(A1.CLASS1) IS NULL THEN 0 ELSE SUM(A1.CLASS1) END AS CLASS1,
CASE WHEN SUM(A11.CLASS1_T2) IS NULL THEN 0 ELSE SUM(A11.CLASS1_T2) END AS CLASS1_T2,
CASE WHEN SUM(A1.CLASS2) IS NULL THEN 0 ELSE SUM(A1.CLASS2) END AS CLASS2,
CASE WHEN SUM(A11.CLASS2_T2) IS NULL THEN 0 ELSE SUM(A11.CLASS2_T2) END AS CLASS2_T2,
CASE WHEN SUM(A1.CLASS3) IS NULL THEN 0 ELSE SUM(A1.CLASS3) END AS CLASS3,
CASE WHEN SUM(A11.CLASS3_T2) IS NULL THEN 0 ELSE SUM(A11.CLASS3_T2) END AS CLASS3_T2


FROM
(SELECT
     'A1' AS ROLE_ID,
     'A1' AS ROLE_CODE,
     S_MW_TYPE.UUID AS MW_TYPE_ID,
     S_MW_TYPE.CODE AS MW_TYPE_CODE,
     S_MW_CLASS.UUID AS MW_CLASS_ID,
     S_MW_CLASS.CODE AS MW_CLASS_CODE,
     CASE WHEN S_MW_CLASS.CODE = 'Class 1' Then COUNT(distinct C_APPL.uuid) End AS CLASS1,
     CASE WHEN S_MW_CLASS.CODE = 'Class 2' Then COUNT(distinct C_APPL.uuid) End AS CLASS2,
     CASE WHEN S_MW_CLASS.CODE = 'Class 3' Then COUNT(distinct C_APPL.uuid) End AS CLASS3
FROM
     C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
     INNER JOIN C_COMP_APPLICANT_MW_ITEM C_APPLN_MW ON C_APPLN.UUID = C_APPLN_MW.COMPANY_APPLICANTS_ID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_TYPE ON C_APPLN_MW.ITEM_TYPE_ID = S_MW_TYPE.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_CLASS ON C_APPLN_MW.ITEM_CLASS_ID = S_MW_CLASS.UUID
      INNER JOIN C_S_SYSTEM_VALUE S_APP_STATUS ON C_APPL.APPLICATION_STATUS_ID = S_APP_STATUS.UUID

WHERE
     C_APPL.CERTIFICATION_NO is not null
     AND to_char(C_APPL.EXPIRY_DATE,'yyyymmdd') >= to_char (sysdate, 'yyyymmdd')
     and (C_APPL.REMOVAL_DATE IS null or (to_char(C_APPL.REMOVAL_DATE, 'yyyymmdd') > to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd')
     and to_char(C_APPL.REMOVAL_DATE,'yyyymmdd') > to_char (sysdate, 'yyyymmdd')))
     AND C_APPL.REGISTRATION_TYPE = 'CMW'
     AND S_APP_STATUS.CODE IN ('1', '4')

GROUP BY

     S_MW_TYPE.UUID,
     S_MW_TYPE.CODE,
     S_MW_CLASS.UUID,
     S_MW_CLASS.CODE ) A1

LEFT JOIN

(SELECT
     'A1' AS ROLE_ID,
     S_MW_TYPE.UUID AS MW_TYPE_ID,
     S_MW_CLASS.UUID AS MW_CLASS_ID,
     S_MW_CLASS.CODE AS MW_CLASS_CODE,
     CASE WHEN S_MW_CLASS.CODE = 'Class 1' Then COUNT(*) End AS CLASS1_T2,
     CASE WHEN S_MW_CLASS.CODE = 'Class 2' Then COUNT(*) End AS CLASS2_T2,
     CASE WHEN S_MW_CLASS.CODE = 'Class 3' Then COUNT(*) End AS CLASS3_T2
FROM
     C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
     INNER JOIN C_COMP_APPLICANT_MW_ITEM C_APPLN_MW ON C_APPLN.UUID = C_APPLN_MW.COMPANY_APPLICANTS_ID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_TYPE ON C_APPLN_MW.ITEM_TYPE_ID = S_MW_TYPE.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_MW_CLASS ON C_APPLN_MW.ITEM_CLASS_ID = S_MW_CLASS.UUID
WHERE
     C_APPL.CERTIFICATION_NO is not null
     AND (to_char( sysdate, 'yyyymmdd') > to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd')
  and to_char( C_APPL.RETENTION_APPLICATION_DATE, 'yyyymmdd') > '20040831' )
  and (C_APPL.REMOVAL_DATE is null or to_char( C_APPL.REMOVAL_DATE, 'yyyymmdd') > to_char(sysdate, 'yyyymmdd'))
GROUP BY
     S_MW_TYPE.UUID,
     S_MW_CLASS.UUID,
     S_MW_CLASS.CODE ) A11

ON   A11.ROLE_ID = A1.ROLE_ID AND A11.MW_TYPE_ID = A1.MW_TYPE_ID AND A11.MW_CLASS_ID = A1.MW_CLASS_ID

LEFT JOIN

(SELECT
     DISTINCT S_VAL1.CODE AS MW_TYPE_CODE, 'A1' AS ROLE_CODE,
     'Contractors' AS ENG_ROLE
     FROM
     C_S_SYSTEM_VALUE S_VAL1, C_S_SYSTEM_TYPE S_TYPE1
     WHERE S_TYPE1.TYPE='MINOR_WORKS_TYPE'
     AND S_TYPE1.UUID = S_VAL1.SYSTEM_TYPE_ID

) A2
ON A2.MW_TYPE_CODE= A1.MW_TYPE_CODE AND A2.ROLE_CODE=A1.ROLE_CODE

GROUP BY
A2.MW_TYPE_CODE,
A2.ROLE_CODE,
A2.ENG_ROLE

)


) Z11


ORDER BY Z11.ROLE_CODE,
Z11.MW_TYPE_CODE";

            return sql;
        }
        //CRM0094
        private string getRPT0006CGC()
        {
            string sql = @"select
  TYPE,
  TYPE_DESC,
   CAT_GP_CODE,
   sub_title,
   SRC_START_TIME,
     FILE_REF,
     SRC_INTERV_NO,
     C_ENAME,
     ROLE_CODE,
   ACCEPT_D,
     REMOVAL_D,
       C_DECRYPT(HKID, '22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3') AS HKID,
    ENAME,
      SRC_RESULT,
      SRC_RESULT_DT,
THIS_FILE_REF
from (

--union 1

SELECT
     '1'AS TYPE,
     'Registration History'as TYPE_DESC,
     S_CAT_GP.CODE AS CAT_GP_CODE,
     'Registration History' || '(' || S_CAT_GP.CODE || ')' as sub_title,
     '' AS SRC_START_TIME,
     C_APPL.FILE_REFERENCE_NO AS FILE_REF,
     '' AS SRC_INTERV_NO,
     C_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME,
     S_APPLN_ROLE.CODE AS ROLE_CODE,
     TO_CHAR (C_APPLN.ACCEPT_DATE, 'DD/MM/YYYY') AS ACCEPT_D,
     TO_CHAR (C_APPLN.REMOVAL_DATE, 'DD/MM/YYYY') AS REMOVAL_D,
     CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID,
     C_propercase(APPLN.SURNAME)||' '|| C_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME,
     '' AS SRC_RESULT,
     '' AS SRC_RESULT_DT,(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = '$P!{c_appln_id}')  AS THIS_FILE_REF
FROM
     C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
     INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_APPLN_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_APPLN_ROLE.UUID
     INNER JOIN C_S_CATEGORY_CODE S_CAT ON C_APPL.CATEGORY_ID = S_CAT.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID
WHERE C_APPL.REGISTRATION_TYPE = :reg_type
 and APPLN.UUID=  :appln_id
 and (C_APPLN.ACCEPT_DATE IS NOT NULL OR C_APPLN.REMOVAL_DATE IS NOT NULL)

 --- union 2
UNION

 SELECT
     '3'AS TYPE,
     'Proposed'as TYPE_DESC,
     S_CAT_GP.CODE AS CAT_GP_CODE,
     'Proposed' || '(' || S_CAT_GP.CODE || ')' as sub_title,
     ''AS SRC_START_TIME,
     C_APPL.FILE_REFERENCE_NO AS FILE_REF,
     ''AS SRC_INTERV_NO,
     C_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME,
     S_APPLN_ROLE.CODE AS ROLE_CODE,
     TO_CHAR(C_APPLN.ACCEPT_DATE,'DD/MM/YYYY')AS ACCEPT_D,
     TO_CHAR(C_APPLN.REMOVAL_DATE,'DD/MM/YYYY')AS REMOVAL_D,
     CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID,
     C_propercase(APPLN.SURNAME)||' '|| C_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME,
     ''AS SRC_RESULT,
     ''AS SRC_RESULT_DT,(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = '$P!{c_appln_id}')  AS THIS_FILE_REF
FROM
     C_COMP_APPLICATION C_APPL INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID
     INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_APPLN_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_APPLN_ROLE.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_STATUS ON C_APPLN.APPLICANT_STATUS_ID = S_STATUS.UUID
     INNER JOIN C_S_CATEGORY_CODE S_CAT ON C_APPL.CATEGORY_ID = S_CAT.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID
WHERE
     C_APPL.REGISTRATION_TYPE = :reg_type
     and APPLN.UUID = :appln_id
     and C_APPLN.ACCEPT_DATE IS NULL
     and C_APPLN.REMOVAL_DATE IS NULL
     and C_APPLN.INTERVIEW_WITHDRAWN_DATE IS NULL
 AND C_APPLN.INTERVIEW_REFUSAL_DATE IS NULL
 AND S_STATUS.CODE <> '10'

 -- UNION 3
UNION
----------------------------------------------------------------
SELECT
     '1'AS TYPE,
     'Registration History'as TYPE_DESC,
     S_CAT_GP.CODE AS CAT_GP_CODE,
     'Registration History' || '(' || S_CAT_GP.CODE || ')' as sub_title,
     '' AS SRC_START_TIME,
     C_APPL.FILE_REFERENCE_NO AS FILE_REF,
     '' AS SRC_INTERV_NO,
     C_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME,
       C_APPLN_HIST.ROLE_NAME AS ROLE_CODE,
     TO_CHAR (C_APPLN_HIST.ACCEPT_DATE, 'DD/MM/YYYY') AS ACCEPT_D,
     TO_CHAR (C_APPLN_HIST.REMOVAL_DATE, 'DD/MM/YYYY') AS REMOVAL_D,
     CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID,
     C_propercase(APPLN.SURNAME)||' '|| C_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME,
     '' AS SRC_RESULT,
     '' AS SRC_RESULT_DT,(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = '$P!{c_appln_id}')  AS THIS_FILE_REF
FROM
     C_COMP_APPLICANT_INFO C_APPLN INNER JOIN C_COMP_APPLICANT_INFO_HISTORY C_APPLN_HIST ON C_APPLN.UUID = C_APPLN_HIST.COMPANY_APPLICANTS_ID
     INNER JOIN C_COMP_APPLICATION C_APPL ON C_APPLN.MASTER_ID = C_APPL.UUID
     INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_APPLN_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_APPLN_ROLE.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_STATUS ON C_APPLN.APPLICANT_STATUS_ID = S_STATUS.UUID
     INNER JOIN C_S_CATEGORY_CODE S_CAT ON C_APPL.CATEGORY_ID = S_CAT.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID
     INNER JOIN 
     (SELECT
     MAX(A.CREATED_DATE) AS CREATED_D,
     A.COMPANY_APPLICANTS_ID AS C_APPLN_ID,
     A.ACCEPT_DATE AS ACCEPT_D
     FROM
     C_COMP_APPLICANT_INFO_HISTORY A
      WHERE  A.REMOVAL_DATE IS NOT NULL or A.ACCEPT_DATE IS NOT NULL
     GROUP BY 
     A.COMPANY_APPLICANTS_ID,
     A.ACCEPT_DATE) C_DT ON C_DT.C_APPLN_ID = C_APPLN_HIST.COMPANY_APPLICANTS_ID
     AND C_DT.ACCEPT_D = C_APPLN_HIST.ACCEPT_DATE

WHERE C_APPL.REGISTRATION_TYPE = :reg_type
 and APPLN.UUID=  :appln_id
 and (C_APPLN_HIST.ACCEPT_DATE IS NOT NULL OR C_APPLN_HIST.REMOVAL_DATE IS NOT NULL)
 AND C_DT.CREATED_D = C_APPLN_HIST.CREATED_DATE


 --UNION 4
UNION

SELECT
     '3'AS TYPE,
     'Proposed'as TYPE_DESC,
     S_CAT_GP.CODE AS CAT_GP_CODE,
     'Proposed' || '(' || S_CAT_GP.CODE || ')' as sub_title,
     '' AS SRC_START_TIME,
     C_APPL.FILE_REFERENCE_NO AS FILE_REF,
     '' AS SRC_INTERV_NO,
     C_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME,
     S_APPLN_ROLE.CODE AS ROLE_CODE,
     TO_CHAR (C_APPLN.ACCEPT_DATE, 'DD/MM/YYYY') AS ACCEPT_D,
     TO_CHAR (C_APPLN.REMOVAL_DATE, 'DD/MM/YYYY') AS REMOVAL_D,
     CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID,
     C_propercase(APPLN.SURNAME)||' '|| C_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME,
     '' AS SRC_RESULT,
     '' AS SRC_RESULT_DT,(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = '$P!{c_appln_id}')  AS THIS_FILE_REF
FROM
     C_COMP_APPLICANT_INFO C_APPLN INNER JOIN C_COMP_APPLICANT_INFO_HISTORY C_APPLN_HIST ON C_APPLN.UUID = C_APPLN_HIST.COMPANY_APPLICANTS_ID
     INNER JOIN C_COMP_APPLICATION C_APPL ON C_APPLN.MASTER_ID = C_APPL.UUID
     INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_APPLN_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_APPLN_ROLE.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_STATUS ON C_APPLN.APPLICANT_STATUS_ID = S_STATUS.UUID
     INNER JOIN C_S_CATEGORY_CODE S_CAT ON C_APPL.CATEGORY_ID = S_CAT.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID
     INNER JOIN 
     (SELECT
     MAX(A.CREATED_DATE) AS CREATED_D,
     A.COMPANY_APPLICANTS_ID AS C_APPLN_ID,
     A.ACCEPT_DATE AS ACCEPT_D
     FROM
     C_COMP_APPLICANT_INFO_HISTORY A
     WHERE  A.REMOVAL_DATE IS NOT NULL or A.ACCEPT_DATE IS NOT NULL
     GROUP BY 
     A.COMPANY_APPLICANTS_ID,
     A.ACCEPT_DATE) C_DT ON C_DT.C_APPLN_ID = C_APPLN_HIST.COMPANY_APPLICANTS_ID
     AND C_DT.ACCEPT_D = C_APPLN_HIST.ACCEPT_DATE
WHERE
     C_APPL.REGISTRATION_TYPE = :reg_type
     and APPLN.UUID = :appln_id
     and C_APPLN_HIST.ACCEPT_DATE IS NULL
     and C_APPLN_HIST.REMOVAL_DATE IS NULL
     and C_APPLN.INTERVIEW_WITHDRAWN_DATE IS NULL
 AND C_APPLN.INTERVIEW_REFUSAL_DATE IS NULL
 AND C_DT.CREATED_D = C_APPLN_HIST.CREATED_DATE


 --UNION 5

 UNION
 SELECT
     '2'AS TYPE,
     'Interview History'as TYPE_DESC,
     '' AS CAT_GP_CODE,
     'Interview History' as sub_title,
     TO_CHAR(I_CAND.START_DATE,'DD/MM/YYYY') AS SRC_START_TIME,
     C_APPL.FILE_REFERENCE_NO AS FILE_REF,
     I_CAND.INTERVIEW_NUMBER AS SRC_INTERV_NO,
     C_propercase(C_APPL.ENGLISH_COMPANY_NAME) AS C_ENAME,
     S_APPLN_ROLE.CODE AS ROLE_CODE,
     TO_CHAR(C_APPLN.ACCEPT_DATE,'DD/MM/YYYY')AS ACCEPT_D,
     TO_CHAR(C_APPLN.REMOVAL_DATE,'DD/MM/YYYY')AS REMOVAL_D,
     CASE WHEN APPLN.HKID IS NULL THEN APPLN.PASSPORT_NO ELSE APPLN.HKID END AS HKID,
     C_propercase(APPLN.SURNAME)||' '|| C_propercase(APPLN.GIVEN_NAME_ON_ID) AS ENAME,
     S_INTRV_RESULT.CODE AS SRC_RESULT,
     TO_CHAR(I_CAND.RESULT_DATE,'DD/MM/YYYY')AS SRC_RESULT_DT,(select  A.FILE_REFERENCE_NO from C_COMP_APPLICATION A inner join C_COMP_APPLICANT_INFO B on A.UUID = B.MASTER_ID WHERE B.UUID = '$P!{c_appln_id}')  AS THIS_FILE_REF
FROM
     C_INTERVIEW_CANDIDATES I_CAND INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON I_CAND.CANDIDATE_NUMBER = C_APPLN.CANDIDATE_NUMBER
     LEFT JOIN C_S_SYSTEM_VALUE S_INTRV_RESULT ON S_INTRV_RESULT.UUID = I_CAND.RESULT_ID
     INNER JOIN C_COMP_APPLICATION C_APPL ON C_APPL.UUID = C_APPLN.MASTER_ID
     INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID
     INNER JOIN C_S_SYSTEM_VALUE S_APPLN_ROLE ON C_APPLN.APPLICANT_ROLE_ID = S_APPLN_ROLE.UUID
     
WHERE
     C_APPL.REGISTRATION_TYPE = :reg_type
     and APPLN.UUID = :appln_id
) a
order by TYPE, CAT_GP_CODE,
to_date(SRC_START_TIME,'DD/MM/YYYY'),
 to_date(ACCEPT_D,'DD/MM/YYYY') desc ,SRC_START_TIME
, SRC_INTERV_NO,ROLE_CODE";
            return sql;
        }

        //CRM0095
        public string getSummryReportForApplicationStatusIMWSql()
        {
            return @"select Status,VFORM from (                                        
  select tb1.ordering,tb1.Status,tb1.VForm from (                     
        select 
        1 as ordering,
        'Active' as Status,
        count(*) as VForm
        from C_ind_application indApp
        inner join C_ind_certificate indCert on indCert.MASTER_ID = indApp.UUID
        inner join C_s_system_value sv on sv.UUID = indCert.APPLICATION_STATUS_ID
        where 1=1
        and indApp.REGISTRATION_TYPE='IMW'
        and sv.CHINESE_DESCRIPTION='Active'                                            
      Union                                                
        select 
        2 as ordering,
        'Application in progress' as Status,
        count(*) as VForm
        from C_ind_application indApp
        inner join C_ind_certificate indCert on indCert.MASTER_ID = indApp.UUID
        inner join C_s_system_value sv on sv.UUID = indCert.APPLICATION_STATUS_ID
        where 1=1
        and indApp.REGISTRATION_TYPE='IMW'
        and sv.CHINESE_DESCRIPTION='Application in progress'                                            
      Union                                                
        select 
        3 as ordering,
        'Certificate prepared' as Status,
        count(*) as VForm
        from C_ind_application indApp
        inner join C_ind_certificate indCert on indCert.MASTER_ID = indApp.UUID
        inner join C_s_system_value sv on sv.UUID = indCert.APPLICATION_STATUS_ID
        where 1=1
        and indApp.REGISTRATION_TYPE='IMW'
        and sv.CHINESE_DESCRIPTION='Certificate prepared'                                         
      Union                                                
        select 
        4 as ordering,
        'Removed' as Status,
        count(*) as VForm
        from C_ind_application indApp
        inner join C_ind_certificate indCert on indCert.MASTER_ID = indApp.UUID
        inner join C_s_system_value sv on sv.UUID = indCert.APPLICATION_STATUS_ID
        where 1=1
        and indApp.REGISTRATION_TYPE='IMW'
        and sv.CHINESE_DESCRIPTION='Removed'                                           
     Union                                                 
        select 
        5 as ordering,
        'Inactive' as Status,
        count(*) as VForm
        from C_ind_application indApp
        inner join C_ind_certificate indCert on indCert.MASTER_ID = indApp.UUID
        inner join C_s_system_value sv on sv.UUID = indCert.APPLICATION_STATUS_ID
        where 1=1
        and indApp.REGISTRATION_TYPE='IMW'
        and sv.CHINESE_DESCRIPTION='Inactive'                                          
      Union                                                
        select 
        6 as ordering,
        'Withdrawn' as Status,
        count(*) as VForm
        from C_ind_application indApp
        inner join C_ind_certificate indCert on indCert.MASTER_ID = indApp.UUID
        inner join C_s_system_value sv on sv.UUID = indCert.APPLICATION_STATUS_ID
        where 1=1
        and indApp.REGISTRATION_TYPE='IMW'
        and sv.CHINESE_DESCRIPTION='Withdrawn'                                                                                   
  ) tb1                                                    
  order by tb1.ordering							          
)     ";
        }

    }
}