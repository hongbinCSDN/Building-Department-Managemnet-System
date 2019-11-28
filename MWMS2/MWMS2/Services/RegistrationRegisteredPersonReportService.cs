using MWMS2.Areas.Registration.Models;
using MWMS2.Entity;
using MWMS2.Utility;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class RegistrationRegisteredPersonReportService
    {
        public FileStreamResult exportRegisteredPersonReport(Fn01Search_RPRSearchModel model)
        {
            string wherestr = "";
            wherestr += processRegisteredPersonReportWhereClause(model.rc_applicationType1, model.rc_subType1);
            wherestr += processRegisteredPersonReportWhereClause(model.rc_applicationType2, model.rc_subType2);
            wherestr += processRegisteredPersonReportWhereClause(model.rc_applicationType3, model.rc_subType3);
            wherestr += processRegisteredPersonReportWhereClause(model.rc_applicationType4, model.rc_subType4);
            wherestr += processRegisteredPersonReportWhereClause(model.rc_applicationType5, model.rc_subType5);
            wherestr += processRegisteredPersonReportWhereClause(model.rc_applicationType6, model.rc_subType6);
            wherestr += processRegisteredPersonReportWhereClause(model.rc_applicationType7, model.rc_subType7);

            if (wherestr.Length > 1)
            {
                wherestr = wherestr.Substring(1);
            }
            string rpt_key = SessionUtil.LoginPost.CODE + DateTime.Now.ToString("YYYYMMddmmss");// "admin_1559292200";
            CallSQL(rpt_key, wherestr,model.Logic);
            return GetRegisteredPersonWorkbook("RegisteredPersonReport",model,rpt_key);
            //DeleteSQL(rpt_key);
            //querystr = "delete from report_rpr where rpt_key = :rpt_key ";
            //query = session.createsqlquery(querystr);
            //query.setparameter("rpt_key", rpt_key);
            //query.executeupdate();
        }
        public void CallSQL(string rpt_key,string wherestr,string Logic)
        {
            string querystr = "call c_registered_person_report('" + rpt_key + "', '" + wherestr + "', '" + Logic + "')";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, querystr, null);
                    conn.Close();
                }
            }
        }
        public void DeleteSQL()
        {
            string querystr = "delete from c_report_rpr where rpt_key = :rpt_key ";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, querystr, null);
                    conn.Close();
                }
            }
        }
        public FileStreamResult GetRegisteredPersonWorkbook(string fileName, Fn01Search_RPRSearchModel model, string rpt_key)
        {
            List<List<string>> header = new List<List<string>>();
            List<List<string>> orderBy = new List<List<string>>();
            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Sheet1");
            printHeader(model.rc_applicationType1, model.rc_subType1, header);
            printHeader(model.rc_applicationType2, model.rc_subType2, header);
            printHeader(model.rc_applicationType3, model.rc_subType3, header);
            printHeader(model.rc_applicationType4, model.rc_subType4, header);
            printHeader(model.rc_applicationType5, model.rc_subType5, header);
            printHeader(model.rc_applicationType6, model.rc_subType6, header);
            printHeader(model.rc_applicationType7, model.rc_subType7, header);
            if (model.rc_orderBy != "")
            {
                printHeader(model.rc_orderBy, model.rc_suborderBy, orderBy);
            }
            List<List<string>> distinctHeader = new List<List<string>>(); 
            for(int k = 0; k < header.Count(); k++)
            {
                distinctHeader = header.Distinct().ToList();
            }

            sheet.Cells[1, 1].LoadFromText("Name");
            sheet.Cells[1, 1].Style.Font.Bold = true;
            sheet.Cells[1, 1].Style.Font.Size = 14;
            sheet.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

            sheet.Cells[1, 2].LoadFromText("HKID/Passport No.");
            sheet.Cells[1, 2].Style.Font.Bold = true;
            sheet.Cells[1, 2].Style.Font.Size = 14;
            sheet.Cells[1, 2].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

            bool _hasMw = false;
            int pre_head = 3;

            foreach (string title in distinctHeader.ElementAt(0))
            {
                if (title.IndexOf("MWC") != -1)
                {
                    _hasMw = true;
                    break;
                }
            }
            if (_hasMw)
            {
                sheet.Cells[1, 3].LoadFromText("District Area");
                sheet.Cells[1, 3].Style.Font.Bold = true;
                sheet.Cells[1, 3].Style.Font.Size = 14;
                sheet.Cells[1, 3].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                pre_head++;
            }
            for (int j = 0; j < distinctHeader.ElementAt(0).Count(); j++)
            {
                sheet.Cells[1, j + pre_head].LoadFromText(distinctHeader.ElementAt(0).ElementAt(j).ToString());
                sheet.Cells[1, j + pre_head].Style.Font.Bold = true;
                sheet.Cells[1, j + pre_head].Style.Font.Size = 14;
                sheet.Cells[1, j + pre_head].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }

            string case_clause = "";
            string select_clause = "";
            string allAnd_clause = "";

            for (int j = 0; j < distinctHeader.ElementAt(1).Count(); j++)
            {
                if (j != 0)
                {
                    case_clause += ", ";
                    select_clause += ", ";
                }
                case_clause += distinctHeader.ElementAt(1).ElementAt(j);
                select_clause += distinctHeader.ElementAt(2).ElementAt(j);
            }
            //select_clause = select_clause.Replace("Max(", "LISTAGG(");

            for (int j = 0; j < distinctHeader.ElementAt(3).Count(); j++)
            {
                if (j > 0)
                {
                    allAnd_clause += " " + model.Logic + " ";
                }
                allAnd_clause += distinctHeader.ElementAt(3).ElementAt(j);
            }
            if (allAnd_clause.Length > 0)
            {
                allAnd_clause = "HAVING " + allAnd_clause;
            }
            List<List<object>> lst = SQL(_hasMw, select_clause,case_clause, allAnd_clause, orderBy,rpt_key);
            int i = 1;
            for (int j = 0; j < lst.Count(); j++)
            {

                for (int h = 0; h < lst.ElementAt(j).Count(); h++)
                {
                    if (h == 2)
                    {
                        if (lst.ElementAt(j)[h] != null)
                        {
                            string[] areaList = lst.ElementAt(j)[h].ToString().Split(',');
                            if (areaList.Count() > 0)
                            {
                                lst.ElementAt(j)[h] = areaList[0];
                            }
                        }

                    }
                    sheet.Cells[j + 2, h + 1].LoadFromText(lst.ElementAt(j)[h].ToString());
                    //sheet.GetRow .LoadFromText(lst.ElementAt(j)[h].ToString());
                }
            }
            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";
            return fsr;
        }
        public List<List<object>> SQL(bool _hasMw, string select_clause, string case_clause,string allAnd_clause,List<List<string>> orderBy,string rpt_key)
        {
            List<List<object>> lst = null;
            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();
            string TypeOfRIE = "";
            if (select_clause.IndexOf("Max(RI_E)") >0)
            {
                select_clause = select_clause.Replace("Max(RI_E)", "Max(RI_E),ESTATUS ,DISCIPLINES ");
                TypeOfRIE = ",ESTATUS ,DISCIPLINES";
            }
            QueryParameters.Add(":rpt_key", rpt_key);
            string queryStr =""
                            +"\r\n" + "\t" + "SELECT name, hkid " 
                            +"\r\n" + "\t" + (_hasMw ? ", LISTAGG(area)" : "") 
                            +"\r\n" + "\t" + (string.IsNullOrWhiteSpace(select_clause) ? select_clause : ", " + select_clause)
                           // +"\r\n" + "\t" + TypeOfRIE
                            +"\r\n" + "\t" + " FROM ( " 
                            +"\r\n" + "\t" +"SELECT DISTINCT name, " 
                            +"\r\n" + "\t" +EncryptDecryptUtil.getDecryptSQL("hkid") 
                            +"\r\n" + "\t" + " as HKID," 
                            +"\r\n" + "\t" +" area, line_no, rpt_key, " 
                            +"\r\n" + "\t" +case_clause + " " 
                            +"\r\n" + "\t" +"FROM C_REPORT_RPR R" 
                            +"\r\n" + "\t" +") WHERE rpt_key = :rpt_key " 
                            +"\r\n" + "\t" +"GROUP BY name, hkid "
                            +"\r\n" + "\t" + TypeOfRIE
                            + "\r\n" + "\t"+ allAnd_clause;

            if (orderBy != null)
            {
                if (orderBy.ElementAt(2).Count() > 0)
                {
                    string orderCol = orderBy.ElementAt(2).ElementAt(0).ToString();
                    if (queryStr.IndexOf(orderCol) != -1)
                    {
                        queryStr += " order by " + orderCol;
                    }
                }
            }
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, QueryParameters);
                    lst = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return lst;
        }
        private void printHeader(string c1, string c2, List<List<string>> header)
        {
            if (string.IsNullOrWhiteSpace(c1) || c1.Equals(""))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(c2))
            {
                c2 = "";
            }
            List<string> column = null;
            List<string> case_clause = null;
            List<string> select_clause = null;
            List<string> allAnd_clause = null;

            if (header.Count > 0)
            {
                column = header.ElementAt(0);
                case_clause = header.ElementAt(1);
                select_clause = header.ElementAt(2);
                allAnd_clause = header.ElementAt(3);
            }
            else
            {
                column = new List<string>();
                case_clause = new List<string>();
                select_clause = new List<string>();
                allAnd_clause = new List<string>();
            }



            if (c1.Equals("AP"))
            {
                c2 = c2.Replace("(", "AP(");
                string[] itemList = c2.Split('\\','|');
                string andOr = itemList[0];
                if (c2.IndexOf("AP") != -1)
                {
                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string colStr = item.Replace(")", "").Replace("(", "_");
                        //colStr = colStr == "on" ? "on1" : colStr;
                        if (colStr == "on") continue;
                        column.Add(item);
                        case_clause.Add("CASE WHEN category = '" + item + "' THEN file_reference_no END AS " + colStr);
                        select_clause.Add("Max(" + colStr + ")");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(" + colStr + ") IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(" + colStr + ") IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    column.Add("AP(A)");
                    column.Add("AP(E)");
                    column.Add("AP(S)");
                    case_clause.Add("CASE WHEN category = 'AP(A)' THEN file_reference_no END AS AP_A");
                    case_clause.Add("CASE WHEN category = 'AP(E)' THEN file_reference_no END AS AP_E");
                    case_clause.Add("CASE WHEN category = 'AP(S)' THEN file_reference_no END AS AP_S");
                    select_clause.Add("Max(AP_A)");
                    select_clause.Add("Max(AP_E)");
                    select_clause.Add("Max(AP_S)");

                    allAnd_clause.Add(" (Max(AP_A) IS NOT NULL " + andOr + " Max(AP_E) IS NOT NULL " + andOr + " Max(AP_S) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("RSE"))
            {
                column.Add("RSE");
                case_clause.Add("CASE WHEN category = 'RSE' THEN file_reference_no END AS RSE");
                select_clause.Add("Max(RSE)");
                allAnd_clause.Add(" Max(RSE) IS NOT NULL ");
            }
            else if (c1.Equals("RGE"))
            {
                column.Add("RGE");
                case_clause.Add("CASE WHEN category = 'RGE' THEN file_reference_no END AS RGE");
                select_clause.Add("Max(RGE)");
                allAnd_clause.Add(" Max(RGE) IS NOT NULL ");
            }
            else if (c1.Equals("RI"))
            {
                c2 = c2.Replace("(", "RI(");
                string[] itemList = c2.Split('\\', '|');
                string andOr = itemList[0];
                if (c2.IndexOf("RI") != -1)
                {
                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string colStr = item.Replace(")", "").Replace("(", "_");
                        //colStr = colStr == "on" ? "on2" : colStr;
                        if (colStr == "on") continue;

                        column.Add(item);
                        //from UR
                        if ("RI_E" == colStr)
                        {
                            column.Add("Status");
                            column.Add("Disciplines/Divisions");
                            case_clause.Add("CASE WHEN category = '" + item + "' THEN file_reference_no END AS " + colStr + ",ESTATUS, DISCIPLINES ");
                        } else
                        {
                            case_clause.Add("CASE WHEN category = '" + item + "' THEN file_reference_no END AS " + colStr + " "); //STATUS, DISCIPLINES
                        }
                        select_clause.Add("Max(" + colStr + ")");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(" + colStr + ") IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(" + colStr + ") IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    column.Add("RI(A)");
                    column.Add("RI(E)");
                    column.Add("RI(S)");
                    case_clause.Add("CASE WHEN category = 'RI(A)' THEN file_reference_no END AS RI_A");
                    case_clause.Add("CASE WHEN category = 'RI(E)' THEN file_reference_no END AS RI_E");
                    case_clause.Add("CASE WHEN category = 'RI(S)' THEN file_reference_no END AS RI_S");
                    select_clause.Add("Max(RI_A)");
                    select_clause.Add("Max(RI_E)");
                    select_clause.Add("Max(RI_S)");

                    allAnd_clause.Add(" (Max(RI_A) IS NOT NULL " + andOr + " Max(RI_E) IS NOT NULL " + andOr + " Max(RI_S) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("RGBC - AS"))
            {
                column.Add("GBC - AS");
                case_clause.Add("CASE WHEN category = 'GBC' and R.role = 'AS' THEN file_reference_no END AS GBC_AS");
                select_clause.Add("Max(GBC_AS)");
                allAnd_clause.Add(" Max(GBC_AS) IS NOT NULL ");
            }
            else if (c1.Equals("RGBC - TD"))
            {
                column.Add("GBC - TD");
                case_clause.Add("CASE WHEN category = 'GBC' and R.role = 'TD' THEN file_reference_no END AS GBC_TD");
                select_clause.Add("Max(GBC_TD)");
                allAnd_clause.Add(" Max(GBC_TD) IS NOT NULL ");
            }
            else if (c1.Equals("SC - AS"))
            {
                c2 = c2.Replace("(", "SC(");
                string[] itemList = c2.Split('\\', '|');
                string andOr = itemList[0];
                if (c2.IndexOf("SC") != -1)
                {
                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string colStr = item.Replace(")", "").Replace("(", "");
                        //colStr = colStr == "on" ? "on3" : colStr;
                        if (colStr == "on") continue;

                        column.Add(item + " - AS");
                        case_clause.Add("CASE WHEN category = '" + item + "' and R.role = 'AS' THEN file_reference_no END AS " + colStr + "_AS");
                        select_clause.Add("Max(" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    column.Add("SC(D) - AS");
                    column.Add("SC(F) - AS");
                    column.Add("SC(GI) - AS");
                    column.Add("SC(SF) - AS");
                    column.Add("SC(V) - AS");
                    case_clause.Add("CASE WHEN category = 'SC(D)' and R.role = 'AS' THEN file_reference_no END AS SCD_AS");
                    case_clause.Add("CASE WHEN category = 'SC(F)' and R.role = 'AS' THEN file_reference_no END AS SCF_AS");
                    case_clause.Add("CASE WHEN category = 'SC(GI)' and R.role = 'AS' THEN file_reference_no END AS SCGI_AS");
                    case_clause.Add("CASE WHEN category = 'SC(SF)' and R.role = 'AS' THEN file_reference_no END AS SCSF_AS");
                    case_clause.Add("CASE WHEN category = 'SC(V)' and R.role = 'AS' THEN file_reference_no END AS SCV_AS");
                    select_clause.Add("Max(SCD_AS)");
                    select_clause.Add("Max(SCF_AS)");
                    select_clause.Add("Max(SCGI_AS)");
                    select_clause.Add("Max(SCSF_AS)");
                    select_clause.Add("Max(SCV_AS)");

                    allAnd_clause.Add(" (Max(SCD_AS) IS NOT NULL " + andOr + " Max(SCF_AS) IS NOT NULL " + andOr + " Max(SCGI_AS) IS NOT NULL " + andOr + " Max(SCSF_AS) IS NOT NULL " + andOr + " Max(SCV_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("SC - TD"))
            {
                c2 = c2.Replace("(", "SC(");
                string[] itemList = c2.Split('\\', '|');
                string andOr = itemList[0];
                if (c2.IndexOf("SC") != -1)
                {
                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string colStr = item.Replace(")", "").Replace("(", "");
                        //colStr = colStr == "on" ? "on4" : colStr;
                        if (colStr == "on") continue;

                        column.Add(item + " - TD");
                        case_clause.Add("CASE WHEN category = '" + item + "' and R.role = 'TD' THEN file_reference_no END AS " + colStr + "_TD");
                        select_clause.Add("Max(" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    column.Add("SC(D) - TD");
                    column.Add("SC(F) - TD");
                    column.Add("SC(GI) - TD");
                    column.Add("SC(SF) - TD");
                    column.Add("SC(V) - TD");
                    case_clause.Add("CASE WHEN category = 'SC(D)' and R.role = 'TD' THEN file_reference_no END AS SCD_TD");
                    case_clause.Add("CASE WHEN category = 'SC(F)' and R.role = 'TD' THEN file_reference_no END AS SCF_TD");
                    case_clause.Add("CASE WHEN category = 'SC(GI)' and R.role = 'TD' THEN file_reference_no END AS SCGI_TD");
                    case_clause.Add("CASE WHEN category = 'SC(SF)' and R.role = 'TD' THEN file_reference_no END AS SCSF_TD");
                    case_clause.Add("CASE WHEN category = 'SC(V)' and R.role = 'TD' THEN file_reference_no END AS SCV_TD");
                    select_clause.Add("Max(SCD_TD)");
                    select_clause.Add("Max(SCF_TD)");
                    select_clause.Add("Max(SCGI_TD)");
                    select_clause.Add("Max(SCSF_TD)");
                    select_clause.Add("Max(SCV_TD)");

                    allAnd_clause.Add(" (Max(SCD_TD) IS NOT NULL " + andOr + " Max(SCF_TD) IS NOT NULL " + andOr + " Max(SCGI_TD) IS NOT NULL " + andOr + " Max(SCSF_TD) IS NOT NULL " + andOr + " Max(SCV_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - AS - Class I, II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                       // colStr = colStr == "on" ? "on5" : colStr;
                        if (colStr == "on") continue;

                        column.Add("MWC(P) - Class I, II, III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 1' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_I_" + colStr + "_AS");
                        select_clause.Add("Max(MWCP_I_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_I_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_I_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class I - (A) - AS");
                    column.Add("MWC(P) - Class I - (B) - AS");
                    column.Add("MWC(P) - Class I - (C) - AS");
                    column.Add("MWC(P) - Class I - (D) - AS");
                    column.Add("MWC(P) - Class I - (E) - AS");
                    column.Add("MWC(P) - Class I - (F) - AS");
                    column.Add("MWC(P) - Class I - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type A' THEN file_reference_no END AS MWCP_I_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type B' THEN file_reference_no END AS MWCP_I_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type C' THEN file_reference_no END AS MWCP_I_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type D' THEN file_reference_no END AS MWCP_I_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type E' THEN file_reference_no END AS MWCP_I_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type F' THEN file_reference_no END AS MWCP_I_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type G' THEN file_reference_no END AS MWCP_I_G_AS");
                    select_clause.Add("Max(MWCP_I_A_AS)");
                    select_clause.Add("Max(MWCP_I_B_AS)");
                    select_clause.Add("Max(MWCP_I_C_AS)");
                    select_clause.Add("Max(MWCP_I_D_AS)");
                    select_clause.Add("Max(MWCP_I_E_AS)");
                    select_clause.Add("Max(MWCP_I_F_AS)");
                    select_clause.Add("Max(MWCP_I_G_AS)");

                    allAnd_clause.Add(" (Max(MWCP_I_A_AS) IS NOT NULL " + andOr + " Max(MWCP_I_B_AS) IS NOT NULL " + andOr + " Max(MWCP_I_C_AS) IS NOT NULL " + andOr + " Max(MWCP_I_D_AS) IS NOT NULL " + andOr + " Max(MWCP_I_E_AS) IS NOT NULL " + andOr + " Max(MWCP_I_F_AS) IS NOT NULL " + andOr + " Max(MWCP_I_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - AS - Class II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        //colStr = colStr == "on" ? "on6" : colStr;

                        if (colStr == "on") continue;
                        column.Add("MWC(P) - Class II, III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 2' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_II_" + colStr + "_AS");
                        select_clause.Add("Max(MWCP_II_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_II_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_II_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class II - (A) - AS");
                    column.Add("MWC(P) - Class II - (B) - AS");
                    column.Add("MWC(P) - Class II - (C) - AS");
                    column.Add("MWC(P) - Class II - (D) - AS");
                    column.Add("MWC(P) - Class II - (E) - AS");
                    column.Add("MWC(P) - Class II - (F) - AS");
                    column.Add("MWC(P) - Class II - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type A' THEN file_reference_no END AS MWCP_II_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type B' THEN file_reference_no END AS MWCP_II_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type C' THEN file_reference_no END AS MWCP_II_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type D' THEN file_reference_no END AS MWCP_II_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type E' THEN file_reference_no END AS MWCP_II_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type F' THEN file_reference_no END AS MWCP_II_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type G' THEN file_reference_no END AS MWCP_II_G_AS");
                    select_clause.Add("Max(MWCP_II_A_AS)");
                    select_clause.Add("Max(MWCP_II_B_AS)");
                    select_clause.Add("Max(MWCP_II_C_AS)");
                    select_clause.Add("Max(MWCP_II_D_AS)");
                    select_clause.Add("Max(MWCP_II_E_AS)");
                    select_clause.Add("Max(MWCP_II_F_AS)");
                    select_clause.Add("Max(MWCP_II_G_AS)");

                    allAnd_clause.Add(" (Max(MWCP_II_A_AS) IS NOT NULL " + andOr + " Max(MWCP_II_B_AS) IS NOT NULL " + andOr + " Max(MWCP_II_C_AS) IS NOT NULL " + andOr + " Max(MWCP_II_D_AS) IS NOT NULL " + andOr + " Max(MWCP_II_E_AS) IS NOT NULL " + andOr + " Max(MWCP_II_F_AS) IS NOT NULL " + andOr + " Max(MWCP_II_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - AS - Class III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        //colStr = colStr == "on" ? "on7" : colStr;

                        if (colStr == "on") continue;
                        column.Add("MWC(P) - Class III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 3' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_III_" + colStr + "_AS");
                        select_clause.Add("Max(MWCP_III_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_III_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_III_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class III - (A) - AS");
                    column.Add("MWC(P) - Class III - (B) - AS");
                    column.Add("MWC(P) - Class III - (C) - AS");
                    column.Add("MWC(P) - Class III - (D) - AS");
                    column.Add("MWC(P) - Class III - (E) - AS");
                    column.Add("MWC(P) - Class III - (F) - AS");
                    column.Add("MWC(P) - Class III - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type A' THEN file_reference_no END AS MWCP_III_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type B' THEN file_reference_no END AS MWCP_III_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type C' THEN file_reference_no END AS MWCP_III_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type D' THEN file_reference_no END AS MWCP_III_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type E' THEN file_reference_no END AS MWCP_III_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type F' THEN file_reference_no END AS MWCP_III_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type G' THEN file_reference_no END AS MWCP_III_G_AS");
                    select_clause.Add("Max(MWCP_III_A_AS)");
                    select_clause.Add("Max(MWCP_III_B_AS)");
                    select_clause.Add("Max(MWCP_III_C_AS)");
                    select_clause.Add("Max(MWCP_III_D_AS)");
                    select_clause.Add("Max(MWCP_III_E_AS)");
                    select_clause.Add("Max(MWCP_III_F_AS)");
                    select_clause.Add("Max(MWCP_III_G_AS)");

                    allAnd_clause.Add(" (Max(MWCP_III_A_AS) IS NOT NULL " + andOr + " Max(MWCP_III_B_AS) IS NOT NULL " + andOr + " Max(MWCP_III_C_AS) IS NOT NULL " + andOr + " Max(MWCP_III_D_AS) IS NOT NULL " + andOr + " Max(MWCP_III_E_AS) IS NOT NULL " + andOr + " Max(MWCP_III_F_AS) IS NOT NULL " + andOr + " Max(MWCP_III_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - TD - Class I, II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        //colStr = colStr == "on" ? "on8" : colStr;
                        if (colStr == "on") continue;

                        column.Add("MWC(P) - Class I, II, III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 1' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_I_" + colStr + "_TD");
                        select_clause.Add("Max(MWCP_I_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_I_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_I_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class I, II, III - (A) - TD");
                    column.Add("MWC(P) - Class I, II, III - (B) - TD");
                    column.Add("MWC(P) - Class I, II, III - (C) - TD");
                    column.Add("MWC(P) - Class I, II, III - (D) - TD");
                    column.Add("MWC(P) - Class I, II, III - (E) - TD");
                    column.Add("MWC(P) - Class I, II, III - (F) - TD");
                    column.Add("MWC(P) - Class I, II, III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type A' THEN file_reference_no END AS MWCP_I_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type B' THEN file_reference_no END AS MWCP_I_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type C' THEN file_reference_no END AS MWCP_I_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type D' THEN file_reference_no END AS MWCP_I_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type E' THEN file_reference_no END AS MWCP_I_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type F' THEN file_reference_no END AS MWCP_I_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type G' THEN file_reference_no END AS MWCP_I_G_TD");
                    select_clause.Add("Max(MWCP_I_A_TD)");
                    select_clause.Add("Max(MWCP_I_B_TD)");
                    select_clause.Add("Max(MWCP_I_C_TD)");
                    select_clause.Add("Max(MWCP_I_D_TD)");
                    select_clause.Add("Max(MWCP_I_E_TD)");
                    select_clause.Add("Max(MWCP_I_F_TD)");
                    select_clause.Add("Max(MWCP_I_G_TD)");

                    allAnd_clause.Add(" (Max(MWCP_I_A_TD) IS NOT NULL " + andOr + " Max(MWCP_I_B_TD) IS NOT NULL " + andOr + " Max(MWCP_I_C_TD) IS NOT NULL " + andOr + " Max(MWCP_I_D_TD) IS NOT NULL " + andOr + " Max(MWCP_I_E_TD) IS NOT NULL " + andOr + " Max(MWCP_I_F_TD) IS NOT NULL " + andOr + " Max(MWCP_I_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - TD - Class II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        //colStr = colStr == "on" ? "on9" : colStr;
                        if (colStr == "on") continue;

                        column.Add("MWC(P) - Class II, III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 2' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_II_" + colStr + "_TD");
                        select_clause.Add("Max(MWCP_II_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_II_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_II_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class II, III - (A) - TD");
                    column.Add("MWC(P) - Class II, III - (B) - TD");
                    column.Add("MWC(P) - Class II, III - (C) - TD");
                    column.Add("MWC(P) - Class II, III - (D) - TD");
                    column.Add("MWC(P) - Class II, III - (E) - TD");
                    column.Add("MWC(P) - Class II, III - (F) - TD");
                    column.Add("MWC(P) - Class II, III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type A' THEN file_reference_no END AS MWCP_II_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type B' THEN file_reference_no END AS MWCP_II_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type C' THEN file_reference_no END AS MWCP_II_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type D' THEN file_reference_no END AS MWCP_II_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type E' THEN file_reference_no END AS MWCP_II_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type F' THEN file_reference_no END AS MWCP_II_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type G' THEN file_reference_no END AS MWCP_II_G_TD");
                    select_clause.Add("Max(MWCP_II_A_TD)");
                    select_clause.Add("Max(MWCP_II_B_TD)");
                    select_clause.Add("Max(MWCP_II_C_TD)");
                    select_clause.Add("Max(MWCP_II_D_TD)");
                    select_clause.Add("Max(MWCP_II_E_TD)");
                    select_clause.Add("Max(MWCP_II_F_TD)");
                    select_clause.Add("Max(MWCP_II_G_TD)");

                    allAnd_clause.Add(" (Max(MWCP_II_A_TD) IS NOT NULL " + andOr + " Max(MWCP_II_B_TD) IS NOT NULL " + andOr + " Max(MWCP_II_C_TD) IS NOT NULL " + andOr + " Max(MWCP_II_D_TD) IS NOT NULL " + andOr + " Max(MWCP_II_E_TD) IS NOT NULL " + andOr + " Max(MWCP_II_F_TD) IS NOT NULL " + andOr + " Max(MWCP_II_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - TD - Class III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        //colStr = colStr == "on" ? "on10" : colStr;
                        if (colStr == "on") continue;

                        column.Add("MWC(P) - Class III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 3' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_III_" + colStr + "_TD");
                        select_clause.Add("Max(MWCP_III_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_III_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_III_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class III - (A) - TD");
                    column.Add("MWC(P) - Class III - (B) - TD");
                    column.Add("MWC(P) - Class III - (C) - TD");
                    column.Add("MWC(P) - Class III - (D) - TD");
                    column.Add("MWC(P) - Class III - (E) - TD");
                    column.Add("MWC(P) - Class III - (F) - TD");
                    column.Add("MWC(P) - Class III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type A' THEN file_reference_no END AS MWCP_III_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type B' THEN file_reference_no END AS MWCP_III_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type C' THEN file_reference_no END AS MWCP_III_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type D' THEN file_reference_no END AS MWCP_III_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type E' THEN file_reference_no END AS MWCP_III_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type F' THEN file_reference_no END AS MWCP_III_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type G' THEN file_reference_no END AS MWCP_III_G_TD");
                    select_clause.Add("Max(MWCP_III_A_TD)");
                    select_clause.Add("Max(MWCP_III_B_TD)");
                    select_clause.Add("Max(MWCP_III_C_TD)");
                    select_clause.Add("Max(MWCP_III_D_TD)");
                    select_clause.Add("Max(MWCP_III_E_TD)");
                    select_clause.Add("Max(MWCP_III_F_TD)");
                    select_clause.Add("Max(MWCP_III_G_TD)");

                    allAnd_clause.Add(" (Max(MWCP_III_A_TD) IS NOT NULL " + andOr + " Max(MWCP_III_B_TD) IS NOT NULL " + andOr + " Max(MWCP_III_C_TD) IS NOT NULL " + andOr + " Max(MWCP_III_D_TD) IS NOT NULL " + andOr + " Max(MWCP_III_E_TD) IS NOT NULL " + andOr + " Max(MWCP_III_F_TD) IS NOT NULL " + andOr + " Max(MWCP_III_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - AS - Class I, II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        //colStr = colStr == "on" ? "on11" : colStr;
                        if (colStr == "on") continue;

                        column.Add("MWC - Class I, II, III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 1' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_I_" + colStr + "_AS");
                        select_clause.Add("Max(MWC_I_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_I_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_I_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class I, II, III - (A) - AS");
                    column.Add("MWC - Class I, II, III - (B) - AS");
                    column.Add("MWC - Class I, II, III - (C) - AS");
                    column.Add("MWC - Class I, II, III - (D) - AS");
                    column.Add("MWC - Class I, II, III - (E) - AS");
                    column.Add("MWC - Class I, II, III - (F) - AS");
                    column.Add("MWC - Class I, II, III - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type A' THEN file_reference_no END AS MWC_I_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type B' THEN file_reference_no END AS MWC_I_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type C' THEN file_reference_no END AS MWC_I_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type D' THEN file_reference_no END AS MWC_I_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type E' THEN file_reference_no END AS MWC_I_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type F' THEN file_reference_no END AS MWC_I_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 1' and mtype = 'Type G' THEN file_reference_no END AS MWC_I_G_AS");
                    select_clause.Add("Max(MWC_I_A_AS)");
                    select_clause.Add("Max(MWC_I_B_AS)");
                    select_clause.Add("Max(MWC_I_C_AS)");
                    select_clause.Add("Max(MWC_I_D_AS)");
                    select_clause.Add("Max(MWC_I_E_AS)");
                    select_clause.Add("Max(MWC_I_F_AS)");
                    select_clause.Add("Max(MWC_I_G_AS)");

                    allAnd_clause.Add(" (Max(MWC_I_A_AS) IS NOT NULL " + andOr + " Max(MWC_I_B_AS) IS NOT NULL " + andOr + " Max(MWC_I_C_AS) IS NOT NULL " + andOr + " Max(MWC_I_D_AS) IS NOT NULL " + andOr + " Max(MWC_I_E_AS) IS NOT NULL " + andOr + " Max(MWC_I_F_AS) IS NOT NULL " + andOr + " Max(MWC_I_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - AS - Class II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        //colStr = colStr == "on" ? "on12" : colStr;
                        if (colStr == "on") continue;

                        column.Add("MWC - Class II, III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 2' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_II_" + colStr + "_AS");
                        select_clause.Add("Max(MWC_II_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_II_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_II_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class II, III - (A) - AS");
                    column.Add("MWC - Class II, III - (B) - AS");
                    column.Add("MWC - Class II, III - (C) - AS");
                    column.Add("MWC - Class II, III - (D) - AS");
                    column.Add("MWC - Class II, III - (E) - AS");
                    column.Add("MWC - Class II, III - (F) - AS");
                    column.Add("MWC - Class II, III - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type A' THEN file_reference_no END AS MWC_II_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type B' THEN file_reference_no END AS MWC_II_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type C' THEN file_reference_no END AS MWC_II_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type D' THEN file_reference_no END AS MWC_II_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type E' THEN file_reference_no END AS MWC_II_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type F' THEN file_reference_no END AS MWC_II_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 2' and mtype = 'Type G' THEN file_reference_no END AS MWC_II_G_AS");
                    select_clause.Add("Max(MWC_II_A_AS)");
                    select_clause.Add("Max(MWC_II_B_AS)");
                    select_clause.Add("Max(MWC_II_C_AS)");
                    select_clause.Add("Max(MWC_II_D_AS)");
                    select_clause.Add("Max(MWC_II_E_AS)");
                    select_clause.Add("Max(MWC_II_F_AS)");
                    select_clause.Add("Max(MWC_II_G_AS)");

                    allAnd_clause.Add(" (Max(MWC_II_A_AS) IS NOT NULL " + andOr + " Max(MWC_II_B_AS) IS NOT NULL " + andOr + " Max(MWC_II_C_AS) IS NOT NULL " + andOr + " Max(MWC_II_D_AS) IS NOT NULL " + andOr + " Max(MWC_II_E_AS) IS NOT NULL " + andOr + " Max(MWC_II_F_AS) IS NOT NULL " + andOr + " Max(MWC_II_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - AS - Class III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        //colStr = colStr == "on" ? "on13" : colStr;
                        if (colStr == "on") continue;

                        column.Add("MWC - Class III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 3' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_III_" + colStr + "_AS");
                        select_clause.Add("Max(MWC_III_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_III_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_III_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class III - (A) - AS");
                    column.Add("MWC - Class III - (B) - AS");
                    column.Add("MWC - Class III - (C) - AS");
                    column.Add("MWC - Class III - (D) - AS");
                    column.Add("MWC - Class III - (E) - AS");
                    column.Add("MWC - Class III - (F) - AS");
                    column.Add("MWC - Class III - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type A' THEN file_reference_no END AS MWC_III_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type B' THEN file_reference_no END AS MWC_III_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type C' THEN file_reference_no END AS MWC_III_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type D' THEN file_reference_no END AS MWC_III_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type E' THEN file_reference_no END AS MWC_III_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type F' THEN file_reference_no END AS MWC_III_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'AS' and Class = 'Class 3' and mtype = 'Type G' THEN file_reference_no END AS MWC_III_G_AS");
                    select_clause.Add("Max(MWC_III_A_AS)");
                    select_clause.Add("Max(MWC_III_B_AS)");
                    select_clause.Add("Max(MWC_III_C_AS)");
                    select_clause.Add("Max(MWC_III_D_AS)");
                    select_clause.Add("Max(MWC_III_E_AS)");
                    select_clause.Add("Max(MWC_III_F_AS)");
                    select_clause.Add("Max(MWC_III_G_AS)");

                    allAnd_clause.Add(" (Max(MWC_III_A_AS) IS NOT NULL " + andOr + " Max(MWC_III_B_AS) IS NOT NULL " + andOr + " Max(MWC_III_C_AS) IS NOT NULL " + andOr + " Max(MWC_III_D_AS) IS NOT NULL " + andOr + " Max(MWC_III_E_AS) IS NOT NULL " + andOr + " Max(MWC_III_F_AS) IS NOT NULL " + andOr + " Max(MWC_III_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - TD - Class I, II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        //colStr = colStr == "on" ? "on14" : colStr;
                        if (colStr == "on") continue;

                        column.Add("MWC - Class I, II, III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 1' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_I_" + colStr + "_TD");
                        select_clause.Add("Max(MWC_I_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_I_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_I_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class I, II, III - (A) - TD");
                    column.Add("MWC - Class I, II, III - (B) - TD");
                    column.Add("MWC - Class I, II, III - (C) - TD");
                    column.Add("MWC - Class I, II, III - (D) - TD");
                    column.Add("MWC - Class I, II, III - (E) - TD");
                    column.Add("MWC - Class I, II, III - (F) - TD");
                    column.Add("MWC - Class I, II, III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type A' THEN file_reference_no END AS MWC_I_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type B' THEN file_reference_no END AS MWC_I_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type C' THEN file_reference_no END AS MWC_I_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type D' THEN file_reference_no END AS MWC_I_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type E' THEN file_reference_no END AS MWC_I_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type F' THEN file_reference_no END AS MWC_I_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 1' and mtype = 'Type G' THEN file_reference_no END AS MWC_I_G_TD");
                    select_clause.Add("Max(MWC_I_A_TD)");
                    select_clause.Add("Max(MWC_I_B_TD)");
                    select_clause.Add("Max(MWC_I_C_TD)");
                    select_clause.Add("Max(MWC_I_D_TD)");
                    select_clause.Add("Max(MWC_I_E_TD)");
                    select_clause.Add("Max(MWC_I_F_TD)");
                    select_clause.Add("Max(MWC_I_G_TD)");

                    allAnd_clause.Add(" (Max(MWC_I_A_TD) IS NOT NULL " + andOr + " Max(MWC_I_B_TD) IS NOT NULL " + andOr + " Max(MWC_I_C_TD) IS NOT NULL " + andOr + " Max(MWC_I_D_TD) IS NOT NULL " + andOr + " Max(MWC_I_E_TD) IS NOT NULL " + andOr + " Max(MWC_I_F_TD) IS NOT NULL " + andOr + " Max(MWC_I_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - TD - Class II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        //colStr = colStr == "on" ? "on15" : colStr;
                        if (colStr == "on") continue;

                        column.Add("MWC - Class II, III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 2' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_II_" + colStr + "_TD");
                        select_clause.Add("Max(MWC_II_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_II_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_II_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class II, III - (A) - TD");
                    column.Add("MWC - Class II, III - (B) - TD");
                    column.Add("MWC - Class II, III - (C) - TD");
                    column.Add("MWC - Class II, III - (D) - TD");
                    column.Add("MWC - Class II, III - (E) - TD");
                    column.Add("MWC - Class II, III - (F) - TD");
                    column.Add("MWC - Class II, III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type A' THEN file_reference_no END AS MWC_II_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type B' THEN file_reference_no END AS MWC_II_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type C' THEN file_reference_no END AS MWC_II_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type D' THEN file_reference_no END AS MWC_II_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type E' THEN file_reference_no END AS MWC_II_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type F' THEN file_reference_no END AS MWC_II_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 2' and mtype = 'Type G' THEN file_reference_no END AS MWC_II_G_TD");
                    select_clause.Add("Max(MWC_II_A_TD)");
                    select_clause.Add("Max(MWC_II_B_TD)");
                    select_clause.Add("Max(MWC_II_C_TD)");
                    select_clause.Add("Max(MWC_II_D_TD)");
                    select_clause.Add("Max(MWC_II_E_TD)");
                    select_clause.Add("Max(MWC_II_F_TD)");
                    select_clause.Add("Max(MWC_II_G_TD)");

                    allAnd_clause.Add(" (Max(MWC_II_A_TD) IS NOT NULL " + andOr + " Max(MWC_II_B_TD) IS NOT NULL " + andOr + " Max(MWC_II_C_TD) IS NOT NULL " + andOr + " Max(MWC_II_D_TD) IS NOT NULL " + andOr + " Max(MWC_II_E_TD) IS NOT NULL " + andOr + " Max(MWC_II_F_TD) IS NOT NULL " + andOr + " Max(MWC_II_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - TD - Class III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        //colStr = colStr == "on" ? "on16" : colStr;
                        if (colStr == "on") continue;

                        column.Add("MWC - Class III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 3' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_III_" + colStr + "_TD");
                        select_clause.Add("Max(MWC_III_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_III_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_III_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class III - (A) - TD");
                    column.Add("MWC - Class III - (B) - TD");
                    column.Add("MWC - Class III - (C) - TD");
                    column.Add("MWC - Class III - (D) - TD");
                    column.Add("MWC - Class III - (E) - TD");
                    column.Add("MWC - Class III - (F) - TD");
                    column.Add("MWC - Class III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type A' THEN file_reference_no END AS MWC_III_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type B' THEN file_reference_no END AS MWC_III_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type C' THEN file_reference_no END AS MWC_III_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type D' THEN file_reference_no END AS MWC_III_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type E' THEN file_reference_no END AS MWC_III_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type F' THEN file_reference_no END AS MWC_III_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and R.role = 'TD' and Class = 'Class 3' and mtype = 'Type G' THEN file_reference_no END AS MWC_III_G_TD");
                    select_clause.Add("Max(MWC_III_A_TD)");
                    select_clause.Add("Max(MWC_III_B_TD)");
                    select_clause.Add("Max(MWC_III_C_TD)");
                    select_clause.Add("Max(MWC_III_D_TD)");
                    select_clause.Add("Max(MWC_III_E_TD)");
                    select_clause.Add("Max(MWC_III_F_TD)");
                    select_clause.Add("Max(MWC_III_G_TD)");

                    allAnd_clause.Add(" (Max(MWC_III_A_TD) IS NOT NULL " + andOr + " Max(MWC_III_B_TD) IS NOT NULL " + andOr + " Max(MWC_III_C_TD) IS NOT NULL " + andOr + " Max(MWC_III_D_TD) IS NOT NULL " + andOr + " Max(MWC_III_E_TD) IS NOT NULL " + andOr + " Max(MWC_III_F_TD) IS NOT NULL " + andOr + " Max(MWC_III_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(W)"))
            {
                if (c2.IndexOf("Item") != -1)
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string sitem = item.Replace(" ", "_").Replace(".", "_");
                        column.Add("MWC(W) - " + item);
                        case_clause.Add("CASE WHEN category = 'MWC(W)' AND instr(item, '" + item + ",') > 0 THEN file_reference_no END AS MWC_W_" + sitem);
                        select_clause.Add("Max(MWC_W_" + sitem + ")");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_W_" + sitem + ") IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_W_" + sitem + ") IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\','|');
                    string andOr = itemList[0];
                    //if(c2.Equals("") || c2.Equals("All") || null == c2){
                    column.Add("MWC(W) - Item 3.1");
                    column.Add("MWC(W) - Item 3.2");
                    column.Add("MWC(W) - Item 3.3");
                    column.Add("MWC(W) - Item 3.4");
                    column.Add("MWC(W) - Item 3.5");
                    column.Add("MWC(W) - Item 3.6");
                    column.Add("MWC(W) - Item 3.7");
                    column.Add("MWC(W) - Item 3.8");
                    column.Add("MWC(W) - Item 3.9");
                    column.Add("MWC(W) - Item 3.10");
                    column.Add("MWC(W) - Item 3.11");
                    column.Add("MWC(W) - Item 3.12");
                    column.Add("MWC(W) - Item 3.13");
                    column.Add("MWC(W) - Item 3.14");
                    column.Add("MWC(W) - Item 3.15");
                    column.Add("MWC(W) - Item 3.16");
                    column.Add("MWC(W) - Item 3.17");
                    column.Add("MWC(W) - Item 3.18");
                    column.Add("MWC(W) - Item 3.19");
                    column.Add("MWC(W) - Item 3.20");
                    column.Add("MWC(W) - Item 3.21");
                    column.Add("MWC(W) - Item 3.22");
                    column.Add("MWC(W) - Item 3.23");
                    column.Add("MWC(W) - Item 3.24");
                    column.Add("MWC(W) - Item 3.25");
                    column.Add("MWC(W) - Item 3.26");
                    column.Add("MWC(W) - Item 3.27");
                    column.Add("MWC(W) - Item 3.28");
                    column.Add("MWC(W) - Item 3.29");
                    column.Add("MWC(W) - Item 3.30");
                    column.Add("MWC(W) - Item 3.31");
                    column.Add("MWC(W) - Item 3.32");
                    column.Add("MWC(W) - Item 3.33");
                    column.Add("MWC(W) - Item 3.34");
                    column.Add("MWC(W) - Item 3.35");
                    column.Add("MWC(W) - Item 3.36");
                    column.Add("MWC(W) - Item 3.37");
                    column.Add("MWC(W) - Item 3.38");
                    column.Add("MWC(W) - Item 3.39");
                    column.Add("MWC(W) - Item 3.40");
                    column.Add("MWC(W) - Item 3.41");
                    column.Add("MWC(W) - Item 3.42");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.1,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_1");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.2,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_2");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.3,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_3");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.4,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_4");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.5,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_5");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.6,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_6");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.7,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_7");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.8,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_8");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.9,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_9");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.10,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_10");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.11,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_11");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.12,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_12");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.13,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_13");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.14,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_14");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.15,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_15");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.16,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_16");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.17,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_17");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.18,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_18");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.19,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_19");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.20,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_20");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.21,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_21");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.22,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_22");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.23,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_23");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.24,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_24");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.25,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_25");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.26,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_26");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.27,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_27");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.28,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_28");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.29,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_29");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.30,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_30");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.31,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_31");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.32,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_32");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.33,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_33");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.34,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_34");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.35,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_35");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.36,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_36");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.37,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_37");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.38,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_38");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.39,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_39");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.40,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_40");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.41,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_41");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.42,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_42");
                    select_clause.Add("Max(MWCW_ITEM_3_1)");
                    select_clause.Add("Max(MWCW_ITEM_3_2)");
                    select_clause.Add("Max(MWCW_ITEM_3_3)");
                    select_clause.Add("Max(MWCW_ITEM_3_4)");
                    select_clause.Add("Max(MWCW_ITEM_3_5)");
                    select_clause.Add("Max(MWCW_ITEM_3_6)");
                    select_clause.Add("Max(MWCW_ITEM_3_7)");
                    select_clause.Add("Max(MWCW_ITEM_3_8)");
                    select_clause.Add("Max(MWCW_ITEM_3_9)");
                    select_clause.Add("Max(MWCW_ITEM_3_10)");
                    select_clause.Add("Max(MWCW_ITEM_3_11)");
                    select_clause.Add("Max(MWCW_ITEM_3_12)");
                    select_clause.Add("Max(MWCW_ITEM_3_13)");
                    select_clause.Add("Max(MWCW_ITEM_3_14)");
                    select_clause.Add("Max(MWCW_ITEM_3_15)");
                    select_clause.Add("Max(MWCW_ITEM_3_16)");
                    select_clause.Add("Max(MWCW_ITEM_3_17)");
                    select_clause.Add("Max(MWCW_ITEM_3_18)");
                    select_clause.Add("Max(MWCW_ITEM_3_19)");
                    select_clause.Add("Max(MWCW_ITEM_3_20)");
                    select_clause.Add("Max(MWCW_ITEM_3_21)");
                    select_clause.Add("Max(MWCW_ITEM_3_22)");
                    select_clause.Add("Max(MWCW_ITEM_3_23)");
                    select_clause.Add("Max(MWCW_ITEM_3_24)");
                    select_clause.Add("Max(MWCW_ITEM_3_25)");
                    select_clause.Add("Max(MWCW_ITEM_3_26)");
                    select_clause.Add("Max(MWCW_ITEM_3_27)");
                    select_clause.Add("Max(MWCW_ITEM_3_28)");
                    select_clause.Add("Max(MWCW_ITEM_3_29)");
                    select_clause.Add("Max(MWCW_ITEM_3_30)");
                    select_clause.Add("Max(MWCW_ITEM_3_31)");
                    select_clause.Add("Max(MWCW_ITEM_3_32)");
                    select_clause.Add("Max(MWCW_ITEM_3_33)");
                    select_clause.Add("Max(MWCW_ITEM_3_34)");
                    select_clause.Add("Max(MWCW_ITEM_3_35)");
                    select_clause.Add("Max(MWCW_ITEM_3_36)");
                    select_clause.Add("Max(MWCW_ITEM_3_37)");
                    select_clause.Add("Max(MWCW_ITEM_3_38)");
                    select_clause.Add("Max(MWCW_ITEM_3_39)");
                    select_clause.Add("Max(MWCW_ITEM_3_40)");
                    select_clause.Add("Max(MWCW_ITEM_3_41)");
                    select_clause.Add("Max(MWCW_ITEM_3_42)");
                    allAnd_clause.Add(" (Max(MWCW_ITEM_3_1) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_2) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_3) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_4) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_5) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_6) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_7) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_8) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_9) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_10) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_11) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_12) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_13) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_14) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_15) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_16) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_17) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_18) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_19) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_20) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_21) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_22) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_23) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_24) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_25) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_26) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_27) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_28) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_29) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_30) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_31) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_32) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_33) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_34) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_35) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_36) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_37) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_38) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_39) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_40) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_41) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_42) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("name"))
            {
                select_clause.Add("name");
            }
            else if (c1.Equals("hkid"))
            {
                select_clause.Add("hkid");
            }

            header.Add(column);
            header.Add(case_clause);
            header.Add(select_clause);
            header.Add(allAnd_clause);
        }

        public string processRegisteredPersonReportWhereClause(string c1, string c2)
        {
            string _where = "";
            if (string.IsNullOrWhiteSpace(c1) || c1.Equals("")){ return "";}
            if (string.IsNullOrWhiteSpace(c2))
            {
                c2 = "";
            }


            if (/*c1 == */"AP".Equals(c1))
            {
                c2 = c2.Replace("(", "AP(");
                if (c2.IndexOf("AP") != -1)
                {
                    _where += c2;
                }
                else
                {
                    _where += "AP(A)AP(E)AP(S)";
                }
            }
            else if (/*c1 == */"RSE".Equals(c1))
            {
                _where += "RSE";
            }
            else if (/*c1 == */"RGE".Equals(c1))
            {
                _where += "RGE";
            }
            else if (/*c1 == */"RI".Equals(c1))
            {
                c2 = c2.Replace("(", "RI(");
                if (c2.IndexOf("RI") != -1)
                {
                    _where += c2;
                }
                else
                {
                    _where += "RI(A)RI(E)RI(S)";
                }
            }
            else if (/*c1 == */"RGBC - AS".Equals(c1))
            {
                _where += "GBC AS";
            }
            else if (c1 == "RGBC - TD")
            {
                _where += "GBC TD";
            }
            else if (c1 == "SC - AS")
            {
                c2 = c2.Replace("(", "SC(");
                if (c2.IndexOf("SC") != -1)
                {
                    _where += c2 + " AS";
                }
                else
                {
                    _where += "SC(D)SC(F)SC(GI)SC(SF)SC(V) AS";
                }
            }
            else if (/*c1 == */"SC - TD".Equals(c1))
            {
                c2 = c2.Replace("(", "SC(");
                if (c2.IndexOf("SC") != -1)
                {
                    _where += c2 + " TD";
                }
                else
                {
                    _where += "SC(D)SC(F)SC(GI)SC(SF)SC(V) TD";
                }
            }
            else if (/*c1 == */"MWC - AS - All Classes".Equals(c1))
            {
                _where += "MWC AS";
            }
            else if (/*c1 == */"MWC - AS - Class I, II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 1 AS " + c2;
                }
                else
                {
                    _where += "MWC Class 1 AS ";
                }
            }
            else if (/*c1 == */"MWC - AS - Class II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 2 AS " + c2;
                }
                else
                {
                    _where += "MWC Class 2 AS ";
                }
            }
            else if (/*c1 == */"MWC - AS - Class III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 3 AS " + c2;
                }
                else
                {
                    _where += "MWC Class 3 AS ";
                }
            }
            else if (/*c1 == */"MWC - TD - Class I, II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 1 TD " + c2;
                }
                else
                {
                    _where += "MWC Class 1 TD ";
                }
            }
            else if (/*c1 == */"MWC - TD - Class II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 2 TD " + c2;
                }
                else
                {
                    _where += "MWC Class 2 TD ";
                }
            }
            else if (/*c1 == */"MWC - TD - Class III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 3 TD " + c2;
                }
                else
                {
                    _where += "MWC Class 3 TD ";
                }
            }
            else if (/*c1 == */"MWC(P) - AS - All Classes".Equals(c1))
            {
                _where += "MWC(P) AS";
            }
            else if (/*c1 == */"MWC(P) - AS - Class I, II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 1 AS " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 1 AS ";
                }
            }
            else if (/*c1 == */"MWC(P) - AS - Class II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 2 AS " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 2 AS ";
                }
            }
            else if (/*c1 == */"MWC(P) - AS - Class III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 3 AS " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 3 AS ";
                }
            }
            else if (/*c1 == */"MWC(P) - TD - Class I, II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 1 TD " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 1 TD ";
                }
            }
            else if (/*c1 == */"MWC(P) - TD - Class II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 2 TD " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 2 TD ";
                }
            }
            else if (/*c1 == */"MWC(P) - TD - Class III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 3 TD " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 3 TD ";
                }
            }
            else
            {
                if (c2.IndexOf("Item") != -1)
                {
                    _where += "MWC(W) " + c2;
                }
                else
                {
                    _where += "MWC(W)";
                }
            }

            if (_where != "")
            {
                _where = "," + _where;
            }
            return _where;
        }
    }
}