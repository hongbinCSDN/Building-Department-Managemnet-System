

using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class OldMWRegisterHTMLGenerator : OldRegisterHTMLGenerator {
	
	public static string ENGLISH_MWI_TEMPLATE = "e_MWC(W).html";
	public static string CHINESE_MWI_TEMPLATE = "c_MWC(W).html";
	public static string ENGLISH_QP_MWI_TEMPLATE = "e_qp_MWC(W).html";
	public static string CHINESE_QP_MWI_TEMPLATE = "c_qp_MWC(W).html";
	public static string SIMPLIFIED_CHINESE_MWI_TEMPLATE = "s_MWC(W).html";
	
	public static string ENGLISH_MWC_TEMPLATE = "e_MWC.html";
	public static string CHINESE_MWC_TEMPLATE = "c_MWC.html";
	public static string ENGLISH_QP_MWC_TEMPLATE = "e_qp_MWC.html";
	public static string CHINESE_QP_MWC_TEMPLATE = "c_qp_MWC.html";
	public static string SIMPLIFIED_CHINESE_MWC_TEMPLATE = "s_MWC.html";
	
	public static string ENGLISH_MWC_P_TEMPLATE = "e_MWC(P).html";
	public static string CHINESE_MWC_P_TEMPLATE = "c_MWC(P).html";
	public static string SIMPLIFIED_CHINESE_MWC_P_TEMPLATE = "s_MWC(P).html";
	
	public static string EXPIRY_DATE_ENG = "31/12/2012";
	public static string EXPIRY_DATE_CHI = "31/12/2012";
	public static string EXPIRY_DATE = "31/12/2012"; 
	
	public static int MAX_RECORD_IND = 200;
	
	public static int MAX_RECORD_COMP = 100;
	
	public static List<FileInfo> generatorMWWebSite(
			String lang,
			String templateFilePath, string templateName,	
			String filePath, string fileNameStart,
            List<object[]> registerData, 
			String title){

        List<FileInfo> result = new List<FileInfo>();
        //ZHConverter converter = ZHConverter.getInstance(ZHConverter.SIMPLIFIED);

        FileInfo src = new FileInfo(templateFilePath+templateName);
		int numberOfFile = 0;
		
		if((registerData.Count % MAX_RECORD_IND) ==0 ){
			numberOfFile=(registerData.Count / MAX_RECORD_IND);
		}else{
			numberOfFile = (registerData.Count / MAX_RECORD_IND) + 1;
		}
		if(registerData.Count == 0){
			numberOfFile=1;
		}
	
		String sPARA_TITLE ="{TITLE}";
		String sPARA_PEOPLE ="{PEOPLE}";
		String sPARA_PAGE = "";
		String sPARA_YEAR = getYear();
		String sPARA_TODAY = getToday(lang);
	
		for(int curPage=1;  curPage <= numberOfFile ; curPage++){
			String fileName = fileNameStart+curPage+FILE_HTML;
			int start = (curPage-1) * MAX_RECORD_IND;
			int end = (curPage) * MAX_RECORD_IND;
			sPARA_PEOPLE = getMWIRegistor(lang, start ,end , registerData);
			sPARA_PAGE =getLink(curPage, numberOfFile, fileNameStart );
            FileInfo desc = new FileInfo(filePath+fileName);
			if(!stringUtil.isBlank(title)){
				sPARA_TITLE = title;
			}else{
				if(lang.Equals(OldRegisterHTMLGenerator.LANG_ENG)){
					sPARA_TITLE = "Registered Minor Works Contractors (Individual)";
				}else if(lang.Equals(OldRegisterHTMLGenerator.LANG_CHI)){
					sPARA_TITLE = "註冊小型工程承建商(個人)";
				}
			}
			if(lang.Equals(OldRegisterHTMLGenerator.LANG_SCH)){
				sPARA_TITLE = (sPARA_TITLE);
			}
			 
			try{
				FileUtil.copy(src, desc);
				FileUtil.readReplace(desc, PARA_PAGE, sPARA_PAGE);
				FileUtil.readReplace(desc, PARA_YEAR, sPARA_YEAR);
				FileUtil.readReplace(desc, PARA_TODAY, sPARA_TODAY);
				FileUtil.readReplace(desc, PARA_PEOPLE, sPARA_PEOPLE);
				FileUtil.readReplace(desc, PARA_TITLE, sPARA_TITLE);
			}catch(Exception e){
				//e.printStackTrace();
			}
			result.Add(desc);
		}
		return result;
	}
    
	protected static string getMWIRegistor(String lang, int start, int end, List<Object[]> registerList){
		
		int stop = end;
		if(stop > registerList.Count){
			stop = registerList.Count;
		}
		StringBuilder sb = new StringBuilder();
		//ZHConverter converter = ZHConverter.getInstance(ZHConverter.SIMPLIFIED);
		
		for(int i = start; i < stop; i++){
			int j =0;
			Object[] register = registerList[i];
			String cerNo = stringUtil.getDisplay((String)register[j++]);
			String surName = stringUtil.getDisplay((String)register[j++]);
			String giveName =stringUtil.getDisplay((String)register[j++]);
			String chineseName = stringUtil.getDisplay((String)register[j++]);
			String simplifiedChineseName = (chineseName);
			String expiryDate = OldDateUtil.getDateDisplayFormat((DateTime)register[j++]);
			String telPhone = stringUtil.getDisplay((String)register[j++]);
			String items = stringUtil.getDisplay((String)register[j++]);
			String nameDisplay = (surName +  " "+ giveName).ToUpper();
						
			String regionEng = stringUtil.getDisplay((String)register[j++]);
			String regionChi = stringUtil.getDisplay((String)register[j++]);
			String regionSChi = (regionChi);
			String email = stringUtil.getDisplay((String)register[j++]);
			String bsFaxNo = stringUtil.getDisplay((String)register[j++]);
			
			String regionDisplay = regionEng;
			
			if(lang.Equals(LANG_CHI)){
				nameDisplay = chineseName + " " + nameDisplay;
				regionDisplay = regionChi;
			}else if(lang.Equals(LANG_SCH)){
				nameDisplay = simplifiedChineseName + " " + nameDisplay;
				regionDisplay = regionSChi;
			}
			
			sb.Append("<TR>");
			sb.Append("<TD> " +getRegistorString(nameDisplay)+"</TD>");
			sb.Append("<TD>" +getRegistorString(items)+"</TD> ");
			sb.Append("<TD>" +getRegistorString(cerNo)+"</TD> ");
			sb.Append("<TD>" +getRegistorString(expiryDate)+"</TD> ");
			sb.Append("<TD  bgColor='yellow'>" +getRegistorString(telPhone)+"</TD> ");
			
			sb.Append("<TD>" +getRegistorString(regionDisplay)+"</TD> ");
			sb.Append("<TD>" +getRegistorString(email)+"</TD> ");
			sb.Append("<TD>" +getRegistorString(bsFaxNo)+"</TD> ");
			
			sb.Append("</TR>");
		}
		return sb.ToString();
		
	}
	
    
	public static List<FileInfo> generatorMWCompanyWebSite(
			String lang,
			String templateFilePath, string templateName,	
			String filePath, string fileNameStart,
            List<OldMWCompanyObjectForHTML> registerData, string title){

        List<FileInfo> result = new List<FileInfo>();
        //ZHConverter converter = ZHConverter.getInstance(ZHConverter.SIMPLIFIED);

        FileInfo src = new FileInfo(templateFilePath+templateName);
		int numberOfFile = 0;
		
		if((registerData.Count % MAX_RECORD_COMP) ==0 ){
			numberOfFile=(registerData.Count / MAX_RECORD_COMP);
		}else{
			numberOfFile = (registerData.Count / MAX_RECORD_COMP) + 1;
		}
		
		if(registerData.Count == 0){
			numberOfFile=1;
		}
	
		String sPARA_TITLE ="{TITLE}";
		String sPARA_PEOPLE ="{PEOPLE}";
		String sPARA_PAGE = "";
		String sPARA_YEAR = getYear();
		String sPARA_TODAY = getToday(lang);
	
		for(int curPage=1;  curPage <= numberOfFile ; curPage++){
			String fileName = fileNameStart+curPage+FILE_HTML;
			int start = (curPage-1) * MAX_RECORD_COMP;
			int end = (curPage) * MAX_RECORD_COMP;
			sPARA_PEOPLE = getMWCRegistor(lang, start ,end , registerData, templateName);
			if(!stringUtil.isBlank(title)){
				sPARA_TITLE = title;
			}else{
				if(templateName.Equals(ENGLISH_MWC_P_TEMPLATE)){
					sPARA_TITLE = "Registered Minor Works Contractors (Provisional)";
				}else if(templateName.Equals(CHINESE_MWC_P_TEMPLATE)){
					sPARA_TITLE = "臨時註冊小型工程承建商";
				}else if(templateName.Equals(SIMPLIFIED_CHINESE_MWC_P_TEMPLATE)){
					sPARA_TITLE = "临时注册小型工程承建商";
				}else if(templateName.Equals(ENGLISH_MWC_TEMPLATE)){
					sPARA_TITLE = "Registered Minor Works Contractors (Company)";
				}else if(templateName.Equals(CHINESE_MWC_TEMPLATE)){
					sPARA_TITLE = "註冊小型工程承建商(公司)";
				}else if(templateName.Equals(SIMPLIFIED_CHINESE_MWC_TEMPLATE)){
					sPARA_TITLE = "注册小型工程承建商(公司)";
				}
				
			}
			if(lang.Equals(LANG_SCH)){
				sPARA_TITLE = (sPARA_TITLE);
			}
				
			sPARA_PAGE =getLink(curPage, numberOfFile, fileNameStart);
			FileInfo desc = new FileInfo(filePath+fileName);
			try{
				FileUtil.copy(src, desc);
				FileUtil.readReplace(desc, PARA_PAGE, sPARA_PAGE);
				FileUtil.readReplace(desc, PARA_YEAR, sPARA_YEAR);
				FileUtil.readReplace(desc, PARA_TODAY, sPARA_TODAY);
				FileUtil.readReplace(desc, PARA_PEOPLE, sPARA_PEOPLE);
				FileUtil.readReplace(desc, PARA_TITLE, sPARA_TITLE);
			}catch(Exception e){
				//e.printStackTrace();
			}
			result.Add(desc);
		}
		return result;
	}
	
	private static string getTypeDisplay(bool chi, String[] data){
		
		String result = data[1];
		if("".Equals(data[1])){
			result = "-";
		}
		
		if(chi){
			return "類型: "+result;
		}else{
			return "Type: "+result;
		}
		
	}
	private static string getClassDisplay(bool chi, String[] data){
		
		String result ="";
		if(data[0].Equals("1")){
			result = "I, II, III";
		}else if(data[0].Equals("2")){
			result = "II, III";
		}else if(data[0].Equals("3")){
			result = "III";
		}else{
			result = "-";
		}
		if(chi){
			return "級別: "+result;
		}else{
			return "Class: "+result;
		}
		
	}
	
	protected static string getMWCRegistor(String lang, int start, int end, List<OldMWCompanyObjectForHTML> registerData, string templateName){
		
		int stop = end;
		if(stop > registerData.Count){
			stop = registerData.Count;
		}
		StringBuilder sb = new StringBuilder();
		//ZHConverter converter = ZHConverter.getInstance(ZHConverter.SIMPLIFIED);
		
		for(int i = start; i < stop; i++){
			int j =0;
            OldMWCompanyObjectForHTML register = registerData[i];
			if(lang.Equals(LANG_CHI) || lang.Equals(LANG_SCH)){
				register.setDisplayChi(true);
			}else{
				register.setDisplayChi(false);
			}
			
			String companyClass = "";
			String companyType = "";
			if(!register.getTypeOne().Equals("")){
				companyClass += "I, II, III <br><br>";
				companyType += register.getTypeOne()+ "<br><br>";
			}
			if(!register.getTypeTwo().Equals("")){
				companyClass += "II, III<br><br>";
				companyType += register.getTypeTwo()+ "<br><br>";
			}
			if(!register.getTypeThree().Equals("")){
				companyClass += "III";
				companyType += register.getTypeThree();
			}
			
			String star = "";
			if(templateName.Equals(ENGLISH_MWC_P_TEMPLATE) || templateName.Equals(CHINESE_MWC_P_TEMPLATE)|| 
					templateName.Equals(SIMPLIFIED_CHINESE_MWC_P_TEMPLATE)){
					star = register.getStar();
			};
			
			
			int iNumberOfAS = register.getNumberOfAS();
			int ROWSPAN = register.getNumberOfLine();		
			OldMWCompanyauthorizedSignatoryObject firstAS = register.getASObject(0);
			int asline = firstAS.getLineNumber();
			
			
			String asWidth =" width='100' ";
			
			sb.Append("<TR>");
			sb.Append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getDisplayName())+"</TD>");
			sb.Append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(companyClass)+"</TD>");
			sb.Append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(companyType)+"</TD>");
			sb.Append("<TD ROWSPAN ="+asline*2  +asWidth+" > " +getASRegistorString(firstAS.getDisplayName())+"</TD>");
			String regStr = getRegistorString(getClassDisplay(firstAS.isDisplayChi(), firstAS.getTypeLine(1)));
			regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
			sb.Append("<TD > " + regStr + "</TD>");
			sb.Append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getRegistrationNumber())+"</TD>");

			
			if(templateName.Equals(ENGLISH_MWC_P_TEMPLATE) || templateName.Equals(CHINESE_MWC_P_TEMPLATE)|| 
					templateName.Equals(SIMPLIFIED_CHINESE_MWC_P_TEMPLATE) ||
					getRegistorString(register.getRegistrationNumber()).StartsWith("MWC(P)")		
            ){
				if(!register.getExpiryDate().Equals(EXPIRY_DATE)){
					if(register.isDisplayChi()){
						sb.Append("<TD ROWSPAN ="+ROWSPAN+"> " + getRegistorStarString(EXPIRY_DATE_CHI,star)  +"</TD>");
					}else{
						sb.Append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorStarString(EXPIRY_DATE_CHI,star)+"</TD>");
					}
				}else{
					sb.Append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorStarString(register.getExpiryDate(),star)+"</TD>");
				}
			}else{
				sb.Append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorStarString(register.getExpiryDate(),star)+"</TD>");
			}
			sb.Append("<TD bgColor='yellow' ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getTelephoneNumber())+"</TD>");
			
			sb.Append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getCompRegion())+"</TD>");
			sb.Append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getEmailAddress())+"</TD>");
			sb.Append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getBsFaxNumber())+"</TD>");
			
			sb.Append("</TR>");
			
			sb.Append("<TR>");
			regStr = getRegistorString(getTypeDisplay(firstAS.isDisplayChi(), firstAS.getTypeLine(1)));
			regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
			sb.Append("<TD>" + regStr + "</TD>");
			sb.Append("</TR>");
			
			for(int eachASLineCounter = 1 ; eachASLineCounter < asline; eachASLineCounter++){
				
				regStr = getRegistorString(getClassDisplay(firstAS.isDisplayChi(), firstAS.getTypeLine(eachASLineCounter+1)));
				regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
				
				sb.Append("<TR>");
				sb.Append("<TD>" + regStr + "</TD>");
				sb.Append("</TR>");
				
				regStr = getRegistorString(getTypeDisplay(firstAS.isDisplayChi(), firstAS.getTypeLine(eachASLineCounter+1)));
				regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
				
				sb.Append("<TR>");
				sb.Append("<TD>" + regStr + "</TD>");
				sb.Append("</TR>");
			}
			
			for(int asCounter = 1 ; asCounter < iNumberOfAS; asCounter++){
				OldMWCompanyauthorizedSignatoryObject asObject = register.getASObject(asCounter);
				
				int asObjectLine = asObject.getLineNumber();
				
				regStr = getRegistorString(getClassDisplay(asObject.isDisplayChi(), asObject.getTypeLine(1)));
				regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
				
				sb.Append("<TR>");
				sb.Append("<TD ROWSPAN ="+asObjectLine*2  +asWidth+"> " +getASRegistorString(asObject.getDisplayName())+"</TD>");
				sb.Append("<TD > " + regStr + "</TD>");
				sb.Append("</TR>");
				sb.Append("<TR>");
				
				regStr = getRegistorString(getTypeDisplay(asObject.isDisplayChi(), asObject.getTypeLine(1)));
				regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
				
				sb.Append("<TD>" + regStr + "</TD>");
				sb.Append("</TR>");
				
				for(int eachASLineCounter = 1 ; eachASLineCounter < asObjectLine; eachASLineCounter++){
					
					regStr = getRegistorString(getClassDisplay(asObject.isDisplayChi(), asObject.getTypeLine(eachASLineCounter+1)));
					regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
					
					sb.Append("<TR>");
					sb.Append("<TD>" + regStr + "</TD>");
					sb.Append("</TR>");
					
					regStr = getRegistorString(getTypeDisplay(asObject.isDisplayChi(), asObject.getTypeLine(eachASLineCounter+1)));
					regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
					
					sb.Append("<TR>");
					sb.Append("<TD>" + regStr + "</TD>");
					sb.Append("</TR>");
				}
			
				//System.out.println(start+" -----------------------------------------");
				//System.out.println(register);
				//System.out.println("HTML -----------------------------------------");
				//System.out.println(sb);
				//System.out.println("--------------------------------------------");
			}
			
			
		}
		return sb.ToString();
		
	}
    
	private static string getASRegistorString(String item){
		if(item == null){
			return "<a href='#note'>#</a>";
		}
		if(item.Equals("")){
			return "<a href='#note'>#</a>";
		}else {
			return item;
		}
			
	}
    
	
	private static string getRegistorStarString(String name , string star ){
		String resultString;
		resultString =  getRegistorString(name);
		if(!stringUtil.isBlank(star)){
			resultString = resultString+ "<a href='#notestar'>*</a>";
		}
	    return resultString;
	}
	

}
