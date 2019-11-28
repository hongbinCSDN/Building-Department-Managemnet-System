using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

public class OldRegisterHTMLGenerator {
	
	public  static string BLANK ="-";
	public  static string FILE_HTML =".html";

	public  static string PARA_PEOPLE ="{PEOPLE}";
	public  static string PARA_PAGE ="{PAGE}";
	public  static string PARA_TITLE ="{TITLE}";
	public  static string PARA_BS_ITEM ="{BS_ITEM}";
	public  static string PARA_NOTE ="{Note}";
	public  static string PARA_YEAR ="{YEAR}";
	public  static string PARA_TODAY ="{TODAY}";

	
	public  static int MAX_RECORD = 200;
	
	public  static string ENGLISH_REGISTER_TEMPLATE = "e_prof.html";
	public  static string CHINESE_REGISTER_TEMPLATE = "c_prof.html";
	public  static string SIMPLIFIED_CHINESE_REGISTER_TEMPLATE = "s_prof.html"; 
	
	public  static string ENGLISH_REGISTER_API_TEMPLATE = "e_prof_api.html";
	public  static string CHINESE_REGISTER_API_TEMPLATE = "c_prof_api.html";
	public  static string SIMPLIFIED_REGISTER_API_TEMPLATE = "s_prof_api.html";
	
	public  static string ENGLISH_REGISTER_APII_TEMPLATE = "e_prof_apii.html";
	public  static string CHINESE_REGISTER_APII_TEMPLATE = "c_prof_apii.html";
	public  static string SIMPLIFIED_CHINESE_REGISTER_APII_TEMPLATE = "s_prof_apii.html";
	
	public  static string ENGLISH_REGISTER_APIII_TEMPLATE = "e_prof_apiii.html";
	public  static string CHINESE_REGISTER_APIII_TEMPLATE = "c_prof_apiii.html";
	public  static string SIMPLIFIED_CHINESE_REGISTER_APIII_TEMPLATE = "s_prof_apiii.html";
	
	public  static string ENGLISH_RI_TEMPLATE = "e_ri.html";
	public  static string CHINESE_RI_TEMPLATE = "c_ri.html";
	public  static string ENGLISH_QP_RI_TEMPLATE = "e_qp_ri.html";
	public  static string CHINESE_QP_RI_TEMPLATE = "c_qp_ri.html"; 
	public  static string SIMPLIFIED_CHINESE_RI_TEMPLATE = "s_ri.html"; 
	
	public  static string ENGLISH_COMP_TEMPLATE = "e_Comp.html";
	public  static string CHINESE_COMP_TEMPLATE = "c_Comp.html";
	public  static string ENGLISH_QP_COMP_TEMPLATE = "e_qp_Comp.html";
	public  static string CHINESE_QP_COMP_TEMPLATE = "c_qp_Comp.html";
	public  static string SIMPLIFIED_CHINESE_COMP_TEMPLATE = "s_Comp.html";
	
	public  static string ENGLISH_COMP_NOBS_TEMPLATE = "e_CompNoBS.html";
	public  static string CHINESE_COMP_NOBS_TEMPLATE = "c_CompNoBS.html";
	public  static string SIMPLIFIED_CHINESE_COMP_NOBS_TEMPLATE = "s_CompNoBS.html";	
	
	public  static string LANG_ENG ="ENG";
	public  static string LANG_CHI ="CHI";
	public  static string LANG_SCH ="SCH";
	
	public  static string ENGLISH_QP_REGISTER_TEMPLATE = "e_qp_prof.html";
	public  static string CHINESE_QP_REGISTER_TEMPLATE = "c_qp_prof.html";
	
	public  static string FILENAME_ORIGINAL_C_API_="c_api_";
	public  static string FILENAME_ORIGINAL_C_APII_="c_apii_";
	public  static string FILENAME_ORIGINAL_C_APIII_="c_apiii_";
	public  static string FILENAME_ORIGINAL_C_GBC_="c_gbc_";
	public  static string FILENAME_ORIGINAL_C_MWC_P_="c_mwc(p)_";
	public  static string FILENAME_ORIGINAL_C_MWC_W_="c_mwc(w)_";
	public  static string FILENAME_ORIGINAL_C_MWC_="c_mwc_";
	public  static string FILENAME_ORIGINAL_C_RGE_="c_rge_";
	public  static string FILENAME_ORIGINAL_C_RI_A_="c_ri(a)_";
	public  static string FILENAME_ORIGINAL_C_RI_E_="c_ri(e)_";
	public  static string FILENAME_ORIGINAL_C_RI_S_="c_ri(s)_";
	public  static string FILENAME_ORIGINAL_C_RSC_D_="c_rsc_d_";
	public  static string FILENAME_ORIGINAL_C_RSC_F_="c_rsc_f_";
	public  static string FILENAME_ORIGINAL_C_RSC_GI_="c_rsc_gi_";
	public  static string FILENAME_ORIGINAL_C_RSC_SF_="c_rsc_sf_";
	public  static string FILENAME_ORIGINAL_C_RSC_V_="c_rsc_v_";
	public  static string FILENAME_ORIGINAL_C_RSE_="c_rse_";
	public  static string FILENAME_ORIGINAL_E_API_="e_api_";
	public  static string FILENAME_ORIGINAL_E_APII_="e_apii_";
	public  static string FILENAME_ORIGINAL_E_APIII_="e_apiii_";
	public  static string FILENAME_ORIGINAL_E_GBC_="e_gbc_";
	public  static string FILENAME_ORIGINAL_E_MWC_P_="e_mwc(p)_";
	public  static string FILENAME_ORIGINAL_E_MWC_W_="e_mwc(w)_";
	public  static string FILENAME_ORIGINAL_E_MWC_="e_mwc_";
	public  static string FILENAME_ORIGINAL_E_RGE_="e_rge_";
	public  static string FILENAME_ORIGINAL_E_RI_A_="e_ri(a)_";
	public  static string FILENAME_ORIGINAL_E_RI_E_="e_ri(e)_";
	public  static string FILENAME_ORIGINAL_E_RI_S_="e_ri(s)_";
	public  static string FILENAME_ORIGINAL_E_RSC_D_="e_rsc_d_";
	public  static string FILENAME_ORIGINAL_E_RSC_F_="e_rsc_f_";
	public  static string FILENAME_ORIGINAL_E_RSC_GI_="e_rsc_gi_";
	public  static string FILENAME_ORIGINAL_E_RSC_SF_="e_rsc_sf_";
	public  static string FILENAME_ORIGINAL_E_RSC_V_="e_rsc_v_";
	public  static string FILENAME_ORIGINAL_E_RSE_="e_rse_";
	
	public  static string FILENAME_NEW_C_API_="c_ap_architects";
	public  static string FILENAME_NEW_C_APII_="c_ap_engineers";
	public  static string FILENAME_NEW_C_APIII_="c_ap_surveyors";
	public  static string FILENAME_NEW_C_GBC_="c_gbc";
	public  static string FILENAME_NEW_C_MWC_P_="c_mwc_provisional";
	public  static string FILENAME_NEW_C_MWC_W_="c_mwc_individual";
	public  static string FILENAME_NEW_C_MWC_="c_mwc_company";
	public  static string FILENAME_NEW_C_RGE_="c_rge";
	public  static string FILENAME_NEW_C_RI_A_="c_ri_architects";
	public  static string FILENAME_NEW_C_RI_E_="c_ri_engineers";
	public  static string FILENAME_NEW_C_RI_S_="c_ri_surveyors";
	public  static string FILENAME_NEW_C_RSC_D_="c_rsc_demolition_works";
	public  static string FILENAME_NEW_C_RSC_F_="c_rsc_foundation_works";
	public  static string FILENAME_NEW_C_RSC_GI_="c_rsc_investigation_field_works";
	public  static string FILENAME_NEW_C_RSC_SF_="c_rsc_site_formation_works";
	public  static string FILENAME_NEW_C_RSC_V_="c_rsc_ventilation_works";
	public  static string FILENAME_NEW_C_RSE_="c_rse";
	public  static string FILENAME_NEW_E_API_="e_ap_architects";
	public  static string FILENAME_NEW_E_APII_="e_ap_engineers";
	public  static string FILENAME_NEW_E_APIII_="e_ap_surveyors";
	public  static string FILENAME_NEW_E_GBC_="e_gbc";
	public  static string FILENAME_NEW_E_MWC_P_="e_mwc_provisional";
	public  static string FILENAME_NEW_E_MWC_W_="e_mwc_individual";
	public  static string FILENAME_NEW_E_MWC_="e_mwc_company";
	public  static string FILENAME_NEW_E_RGE_="e_rge";
	public  static string FILENAME_NEW_E_RI_A_="e_ri_architects";
	public  static string FILENAME_NEW_E_RI_E_="e_ri_engineers";
	public  static string FILENAME_NEW_E_RI_S_="e_ri_surveyors";
	public  static string FILENAME_NEW_E_RSC_D_="e_rsc_demolition_works";
	public  static string FILENAME_NEW_E_RSC_F_="e_rsc_foundation_works";
	public  static string FILENAME_NEW_E_RSC_GI_="e_rsc_investigation_field_works";
	public  static string FILENAME_NEW_E_RSC_SF_="e_rsc_site_formation_works";
	public  static string FILENAME_NEW_E_RSC_V_="e_rsc_ventilation_works";
	public  static string FILENAME_NEW_E_RSE_="e_rse";


    static Dictionary<string, string> FILENAME_EXCEL_MAP = new Dictionary<string, string>() {
        {FILENAME_ORIGINAL_C_API_,    FILENAME_NEW_C_API_           }         ,
         {FILENAME_ORIGINAL_C_APII_,    FILENAME_NEW_C_APII_           }      ,
         {FILENAME_ORIGINAL_C_APIII_,    FILENAME_NEW_C_APIII_     }          ,
         {FILENAME_ORIGINAL_C_GBC_,    FILENAME_NEW_C_GBC_         }          ,
         {FILENAME_ORIGINAL_C_MWC_P_,    FILENAME_NEW_C_MWC_P_     }          ,
         {FILENAME_ORIGINAL_C_MWC_W_,    FILENAME_NEW_C_MWC_W_     }          ,
         {FILENAME_ORIGINAL_C_MWC_,    FILENAME_NEW_C_MWC_         }          ,
         {FILENAME_ORIGINAL_C_RGE_,    FILENAME_NEW_C_RGE_         }          ,
         {FILENAME_ORIGINAL_C_RI_A_,    FILENAME_NEW_C_RI_A_           }      ,
         {FILENAME_ORIGINAL_C_RI_E_,    FILENAME_NEW_C_RI_E_           }      ,
         {FILENAME_ORIGINAL_C_RI_S_,    FILENAME_NEW_C_RI_S_           }      ,
         {FILENAME_ORIGINAL_C_RSC_D_,    FILENAME_NEW_C_RSC_D_     }          ,
         {FILENAME_ORIGINAL_C_RSC_F_,    FILENAME_NEW_C_RSC_F_     }          ,
         {FILENAME_ORIGINAL_C_RSC_GI_,    FILENAME_NEW_C_RSC_GI_       }      ,
         {FILENAME_ORIGINAL_C_RSC_SF_,    FILENAME_NEW_C_RSC_SF_       }      ,
         {FILENAME_ORIGINAL_C_RSC_V_,    FILENAME_NEW_C_RSC_V_     }          ,
         {FILENAME_ORIGINAL_C_RSE_,    FILENAME_NEW_C_RSE_         }          ,
         {FILENAME_ORIGINAL_E_API_,    FILENAME_NEW_E_API_         }          ,
         {FILENAME_ORIGINAL_E_APII_,    FILENAME_NEW_E_APII_           }      ,
         {FILENAME_ORIGINAL_E_APIII_,    FILENAME_NEW_E_APIII_     }          ,
         {FILENAME_ORIGINAL_E_GBC_,    FILENAME_NEW_E_GBC_         }          ,
         {FILENAME_ORIGINAL_E_MWC_P_,    FILENAME_NEW_E_MWC_P_     }          ,
         {FILENAME_ORIGINAL_E_MWC_W_,    FILENAME_NEW_E_MWC_W_     }          ,
         {FILENAME_ORIGINAL_E_MWC_,    FILENAME_NEW_E_MWC_         }          ,
         {FILENAME_ORIGINAL_E_RGE_,    FILENAME_NEW_E_RGE_         }          ,
         {FILENAME_ORIGINAL_E_RI_A_,    FILENAME_NEW_E_RI_A_           }      ,
         {FILENAME_ORIGINAL_E_RI_E_,    FILENAME_NEW_E_RI_E_           }      ,
         {FILENAME_ORIGINAL_E_RI_S_,    FILENAME_NEW_E_RI_S_           }      ,
         {FILENAME_ORIGINAL_E_RSC_D_,    FILENAME_NEW_E_RSC_D_     }          ,
         {FILENAME_ORIGINAL_E_RSC_F_,    FILENAME_NEW_E_RSC_F_     }          ,
         {FILENAME_ORIGINAL_E_RSC_GI_,    FILENAME_NEW_E_RSC_GI_       }      ,
         {FILENAME_ORIGINAL_E_RSC_SF_,    FILENAME_NEW_E_RSC_SF_       }      ,
         {FILENAME_ORIGINAL_E_RSC_V_,    FILENAME_NEW_E_RSC_V_     }          ,
         {FILENAME_ORIGINAL_E_RSE_,    FILENAME_NEW_E_RSE_           }
    };



    
    public static List<FileInfo> generatorWebSite(
			string lang, string templateFilePath, string templateName, bool isComp, bool isbs,	
			string filePath, string fileNameStart,
			string title, List<String[]> registerList,
            List<C_S_SYSTEM_VALUE> bsitem, List<C_S_HTML_NOTES> notes, string catCode){

        List<FileInfo> result = new List<FileInfo>();

        FileInfo src = new FileInfo(templateFilePath+templateName);
        try {

            int numberOfFile = (registerList.Count / MAX_RECORD) + 1;

            string sPARA_PEOPLE = "{PEOPLE}";
            string sPARA_PAGE = "";

            string sPARA_TITLE = title;
            string sPARA_BS_ITEM = "";
            string sPARA_NOTE = getNotes(notes, lang);
            string sPARA_YEAR = getYear();
            string sPARA_TODAY = getToday(lang);
            bool isQP = fileNameStart.IndexOf("qp_") != -1;
            if (!isQP)
            {
                sPARA_BS_ITEM = getBSItem(bsitem, lang);
            }

            for (int curPage = 1; curPage <= numberOfFile; curPage++)
            {

                string fileName = fileNameStart + curPage + FILE_HTML;
                int start = (curPage - 1) * MAX_RECORD;
                int end = (curPage) * MAX_RECORD;
                sPARA_PEOPLE = getRegistor(start, end, registerList, isComp, isbs, catCode, isQP);
                sPARA_PAGE = getLink(curPage, numberOfFile, fileNameStart);

                FileInfo desc = new FileInfo(filePath + fileName);
                try
                {
                    FileUtil.copy(src, desc);
                    FileUtil.readReplace(desc, PARA_PAGE, sPARA_PAGE);
                    FileUtil.readReplace(desc, PARA_TITLE, sPARA_TITLE);
                    FileUtil.readReplace(desc, PARA_BS_ITEM, sPARA_BS_ITEM);
                    FileUtil.readReplace(desc, PARA_NOTE, sPARA_NOTE);
                    FileUtil.readReplace(desc, PARA_YEAR, sPARA_YEAR);
                    FileUtil.readReplace(desc, PARA_TODAY, sPARA_TODAY);
                    FileUtil.readReplace(desc, PARA_PEOPLE, sPARA_PEOPLE);
                }
                catch (Exception e)
                {
                    //e.printStackTrace();
                }
                result.Add(desc);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
		return result;
	}
    
	protected static string getYear(){
        return DateTime.Now.Year.ToString();// DateUtil.getCurrentYear()+"";
	}
	
	protected static string getToday(string lang){
		if(lang.Equals(LANG_ENG)){
			return OldDateUtil.getEnglishFormatDate(DateTime.Now);
		}else{
			return OldDateUtil.getChineseFormatDate(DateTime.Now);
		}
	}

	protected static string getNotes( List<C_S_HTML_NOTES> notes, string lang){
        StringBuilder sb = new StringBuilder();
		for(int i = 0; i <  notes.Count; i++){
			int j =0;
            C_S_HTML_NOTES sHtmlNotes = notes[i];
			
			if(lang.Equals(LANG_ENG)){
				sb.Append(" "+stringUtil.getDisplay(sHtmlNotes.CODE)+". ");
				sb.Append(stringUtil.getDisplay(sHtmlNotes.ENGLISH_DESCRIPTION)+"<br>");
			}else{
				sb.Append(" "+ stringUtil.getDisplay(sHtmlNotes.CODE)+". ");
				sb.Append(stringUtil.getDisplay(sHtmlNotes.CHINESE_DESCRIPTION)+"<br>");
			}
		}
		return sb.ToString();
	}

    protected static string getBSItem(List<C_S_SYSTEM_VALUE> bsitem, string lang)
    {

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bsitem.Count; i++)
        {
            int j = 0;
            C_S_SYSTEM_VALUE sSystemValue = bsitem[i];

            if (lang.Equals(LANG_ENG))
            {
                sb.Append("'" + stringUtil.getDisplay(sSystemValue.CODE) + "'");
                sb.Append(" " + stringUtil.getDisplay(sSystemValue.ENGLISH_DESCRIPTION) + "<br>");
            }
            else
            {
                sb.Append("'" + stringUtil.getDisplay(sSystemValue.CODE) + "'");
                sb.Append(" " + stringUtil.getDisplay(sSystemValue.CHINESE_DESCRIPTION) + "<br>");
            }


        }
        return sb.ToString();
    }

    protected static string getLink(int currentPage, int numberOfFile, string filename){
		string result = "";
		for(int i = 1; i <= numberOfFile; i++){
			string pageLink =  "";
			if(currentPage==i){
				pageLink = i+" ";
			}else{
				pageLink = "<A href='"+filename+i+FILE_HTML+"'>"+ i+" </A>";
			}
			result = result+pageLink;
		}
		return result;
	}
	
	protected static string getRegistor(int start, int end, List<String[]> registerList, bool isComp, bool isbs, string catCode, bool isQP){
		
		int stop = end;
		
		if(stop > registerList.Count){
			stop = registerList.Count;
		}
        StringBuilder sb = new StringBuilder();
		
		for(int i = start; i < stop; i++){
			String[] register = registerList[i];
			int j =0;
			if(isComp){
				string englishName = register[j++];
				string cerNo = register[j++];
				string expiryDate = register[j++];
				string bsCodeList = register[j++];
				string telNo = register[j++];
				string asNameList = register[j++];
				string flag = 		register[j++];
				string region = register[j++];
				string email = register[j++];
				string fax = register[j++];
				if(getRegistorString(bsCodeList).Equals(BLANK)){
					telNo = "";
					region = "";
					email = "";
					fax = "";
				}
					
				if(isbs){
					sb.Append("<TR>");
					sb.Append("<TD width='400'> " + getRegistorString(englishName)+"</TD>");
					sb.Append("<TD width='200'>" +getASRegistorString(asNameList)+"&nbsp;</TD> ");
					sb.Append("<TD width='150'>" +getRegistorString(cerNo)+"</TD> ");
					sb.Append("<TD width='100' nowrap>" +getRegistorString(expiryDate)+"<a href='#note'>"+flag+"</a></TD> ");
					if(!isQP){
						sb.Append("<TD width='100'><A href='#remark'>" +getRegistorString(bsCodeList)+"</A></TD> ");
					}
					sb.Append("<TD width='100'>" +getRegistorString(region)+"</TD> ");
					sb.Append("<TD width='100'>" +getRegistorString(email)+"</TD> ");
					sb.Append("<TD width='100'>" +getRegistorString(fax)+"</TD> ");
					sb.Append("<TD width='100'>" +getRegistorString(telNo)+"</TD> ");

					sb.Append("</TR>");
				}else{
					sb.Append("<TR>");
					sb.Append("<TD> " +getRegistorString(englishName)+"&nbsp;</TD>");
					sb.Append("<TD>" +getRegistorString(asNameList)+"&nbsp;</TD> ");
					sb.Append("<TD width='150'>" +getRegistorString(cerNo)+"&nbsp;</TD> ");
					sb.Append("<TD  nowrap>" +getRegistorString(expiryDate)+" <a href='#note'>"+flag+"</a></TD> ");
					sb.Append("<TD width='100'>" +getRegistorString(region)+"</TD> ");
					sb.Append("<TD width='100'>" +getRegistorString(email)+"</TD> ");
					sb.Append("<TD width='100'>" +getRegistorString(fax)+"</TD> ");
					sb.Append("<TD width='100'>" +getRegistorString(telNo)+"</TD> ");
					sb.Append("</TR>");
				}
				
			}else{
				string englishName = register[j++];
				string cerNo = register[j++];
				string expiryDate = register[j++];
				string bsCodeList = register[j++];
				string telNo = register[j++];
				string asNameList = register[j++];
				string flag = 		register[j++];
				string region = register[j++];
				string email = register[j++];
				string fax = register[j++];
				if(getRegistorString(bsCodeList).Equals(BLANK)){
					telNo = "";
					region = "";
					email = "";
					fax = "";
				}
				sb.Append("<TR>");
				sb.Append("<TD width='400'> " +getRegistorString(englishName)+"&nbsp;</TD>");
				sb.Append("<TD width='150'>" +getRegistorString(cerNo)+"&nbsp;</TD> ");
				sb.Append("<TD width='100' nowrap>" +getRegistorString(expiryDate)+" &nbsp;<a href='#note'>"+flag+"</a></TD> ");
				
				if(!(catCode.Equals("RI(A)") || catCode.Equals("RI(E)") ||catCode.Equals("RI(S)")) && !isQP) {
					sb.Append("<TD width='100'><A href='#remark'>" +getRegistorString(bsCodeList)+"&nbsp;</A>&nbsp;</TD> ");
				} else {
					j++;
				}
				sb.Append("<TD width='100'>" +getRegistorString(region)+"&nbsp;</TD> ");
				sb.Append("<TD width='100'>" +getRegistorString(email)+"&nbsp;</TD> ");
				sb.Append("<TD width='100'>" +getRegistorString(fax)+"&nbsp;</TD> ");
				sb.Append("<TD width='100'>" +getRegistorString(telNo)+"&nbsp;</TD> ");

				sb.Append("</TR>");
			}
		}	
		return sb.ToString();
	}
	protected static string getRegistorString(string item){
		if(item == null){
			return BLANK;
		}
		if(item.Equals("")){
			return BLANK;
		}else {
			return item;
		}
			
	}
	
	private static string getASRegistorString(string item){
		if(item == null){
			return "<a href='#note'>#</a>";
		}
		if(item.Equals("")){
			return "<a href='#note'>#</a>";
		}else {
			return item;
		}
			
	}
	/***
	private static void getTitle(PrintWriter printWriter){
		printWriter.println("<TR>");
		printWriter.println("<TH width='400' bgColor='#ddffe7' rowSpan='2'><STRONG>Name</STRONG></TH>");
		printWriter.println("<TH width='150' bgColor='#ddffe7' rowSpan='2'><STRONG>Registration Number</STRONG></TH>");
		printWriter.println("<TH width='100' bgColor='#ddffe7' rowSpan='2'><STRONG>Expiry Date</STRONG></TH>");
		printWriter.println("<TH width='200' bgColor='#ddffe7' colSpan='2'><STRONG><A href='#remark'>Remark</A></STRONG></TH>");
		printWriter.println("</TR>");
		printWriter.println("<TR>");
		printWriter.println("<TH width='100' bgColor='#ddffe7'><STRONG><A href='#remark'>Service in Building Safety</A></STRONG></TH>");
		printWriter.println("<TH width='100' bgColor='#ddffe7'><STRONG>Phone Number</STRONG></TH>");
		printWriter.println("</TR>");
	}
	**/
	/**
	
	private static void getLinkTitle(PrintWriter printWriter,
			string title, int currentPage, int numberOfFile, string filename){
		
		printWriter.println("<TR>");
		printWriter.println("<TH style='WIDTH: 550px' align='left' width='532' colSpan='2' height='40'>");
		printWriter.println("<B><FONT face='Arial' size='2'>");
		printWriter.println(title);
		
		printWriter.println("</FONT></B> ");
		
		printWriter.println("</TH>");
		printWriter.println("<TH width='300' colSpan='3'>");
		printWriter.println("<FONT face='Arial' size='2'>| ");
		
		for(int i = 1; i <= numberOfFile; i++){
			string pageLink =  "";
			if(currentPage==1){
				pageLink = "Page "+i;
			}else{
				pageLink = "<A href='"+filename+i+FILE_HTML+"'>"+numberOfFile+"</A>";
			}
			printWriter.println(pageLink);
		}
		printWriter.println("|</FONT></TH> ");
		printWriter.println("</TR> ");

	}
	**/
	/**
	private  static string getHeader( string title){
		
		StringBuffer sb = new StringBuffer();
		
		sb.Append("<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.0 Transitional//EN'>");
		sb.Append("<HTML>");				
		sb.Append("<HEAD>");				
		sb.Append("<title>");
		sb.Append(title);
		sb.Append("</title>");
		sb.Append("<META http-equiv='Content-Type' content='text/html; charset=utf-8'>");
		sb.Append("");
		sb.Append("<meta content='JavaScript' name='vs_defaultClientScript'>");
		sb.Append("<meta content='http://schemas.microsoft.com/intellisense/ie3-2nav3-0' name='vs_targetSchema'>");
		sb.Append("<STYLE type='text/css'>A:link { COLOR: blue; TEXT-DECORATION: none }");
		sb.Append("A:active { COLOR: blue; TEXT-DECORATION: none }");
		sb.Append("A:visited { COLOR: blue; TEXT-DECORATION: none }");
		sb.Append("</STYLE>");
		sb.Append("</HEAD>");
		sb.Append("<body MS_POSITIONING='FlowLayout'>");
		sb.Append("<TABLE id='Table2' borderColor='#008080' cellPadding='2' width='850' border='1'> ");
			
		
		
		
		return sb.toString();
	}
	**/
	
	
	public static string changeFileNameForExcelDataExport(string fileName){
		string result = FILENAME_EXCEL_MAP[fileName];
		if(result == null || result.Equals("")){
			result=fileName;
		}
		return result;
	}
	
	
	
}
