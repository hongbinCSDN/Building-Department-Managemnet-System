
using MWMS2.Entity;
using MWMS2.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;

public class OldRegisterExcelGenerator : OldRegisterHTMLGenerator {
	
	public static String BLANK ="-";
	public static String FILE_XLS =".xls";

	public static String PARA_PEOPLE ="{PEOPLE}";
	public static String PARA_PAGE ="{PAGE}";
	public static String PARA_TITLE ="{TITLE}";
	public static String PARA_BS_ITEM ="{BS_ITEM}";
	public static String PARA_NOTE ="{Note}";
	public static String PARA_YEAR ="{YEAR}";
	public static String PARA_TODAY ="{TODAY}";
	
	
		

	public static List<FileInfo> generatorWebSite(
			String langCode, String templateFilePath, String templateName, 
			bool isComp, bool isShowBuildingSafetly,	
			String filePath, String fileNameStart,
			String title, 
			List<String[]> registerList, 
			List<C_S_SYSTEM_VALUE> bsitem, 
			List<C_S_HTML_NOTES> notes, 
			String catCode, 
			List<C_S_SYSTEM_VALUE> extraRemarksList,
			List<C_S_SYSTEM_VALUE> extraNoteList){

		List<FileInfo> resultList = new List<FileInfo>();
		try{		
		bool isQP = fileNameStart.IndexOf("qp_") != -1;
		
		
		
		
		
		//String fileName = fileNameStart+FILE_XLS;
		
		String fileName = changeFileNameForExcelDataExport(fileNameStart)+FILE_XLS;
		
		
		HSSFWorkbook wb =  new  HSSFWorkbook();
		HSSFSheet sheet = wb.CreateSheet(fileNameStart) as HSSFSheet;
		
		bool showFSS = true;
		bool showMBIS = false;
		if (catCode.Equals("SC(D)") || catCode.Equals("SC(F)") || catCode.Equals("SC(GI)") ||
			catCode.Equals("SC(SF)") || catCode.Equals("SC(V)") || catCode.Equals("RGE"))
		{
			showFSS = false;
		}
		

		if (catCode.Equals("RI(A)") || catCode.Equals("RI(E)") || catCode.Equals("RI(S)"))
		{
			showMBIS = true;
		}
		
		createHeader( wb, sheet,  title, templateName, showFSS,showMBIS );
		createRegistorList(wb, sheet, registerList, isComp, isShowBuildingSafetly, catCode, isQP, showFSS, showMBIS);
		
		for (int j = 0; j < 10; j++) {
            sheet.AutoSizeColumn((short)j, true);
		}
		
		createNotes(wb, sheet, notes, langCode, extraNoteList);
		
		createRemarks(wb, sheet, templateName, bsitem, langCode, extraRemarksList);
		
		
		
		/**
		createNotes(sheet, registerList, isComp, isShowBuildingSafetly, catCode, isQP);
		
		
		String sPARA_NOTE = getNotes(notes, langCode);
		String sPARA_YEAR = getYear();
		String sPARA_TODAY = getToday(langCode);
		bool isQP = fileNameStart.indexOf("qp_") != -1;
		if(!isQP){
			sPARA_BS_ITEM = getBSItem(bsitem, langCode);
		}
		
		String fileName = fileNameStart+FILE_XLS;
		
		sPARA_PEOPLE = getRegistor( start ,end , registerList,isComp, isShowBuildingSafetly, catCode, isQP);
		sPARA_PAGE =getLink(curPage, numberOfFile, fileNameStart );

		FileUtil.readReplace(desc, PARA_PAGE, sPARA_PAGE);
		FileUtil.readReplace(desc, PARA_TITLE, sPARA_TITLE);
		FileUtil.readReplace(desc, PARA_BS_ITEM, sPARA_BS_ITEM);
		FileUtil.readReplace(desc, PARA_NOTE, sPARA_NOTE);
		FileUtil.readReplace(desc, PARA_YEAR, sPARA_YEAR);
		FileUtil.readReplace(desc, PARA_TODAY, sPARA_TODAY);
		FileUtil.readReplace(desc, PARA_PEOPLE, sPARA_PEOPLE);
		
		
		**/
		if(File.Exists(filePath + fileName))
            {
                File.Delete(filePath + fileName);
            }
        FileStream outFile = new FileStream(filePath + fileName, FileMode.CreateNew);
            wb.Write(outFile);
            outFile.Close();
		FileInfo desc = new FileInfo(filePath+fileName);
		resultList.Add(desc);
		}catch(Exception e){
            Console.WriteLine(e);
			//System.out.println(e);
		}
		return resultList;
	}


	protected static void createNotes(HSSFWorkbook wb, HSSFSheet sheetFile,
										List<C_S_HTML_NOTES> notes, String lang, 
										List<C_S_SYSTEM_VALUE> extraNoteList){
		int row = sheetFile.LastRowNum+2;
		int col=0;
		HSSFCellStyle noWrapCellStyle = wb.CreateCellStyle() as HSSFCellStyle;
        noWrapCellStyle.WrapText = false;
	
		
		HSSFRow dataRow = sheetFile.CreateRow(row++) as HSSFRow;
		/*if(lang.Equals(LANG_ENG)){
			setText(dataRow.CreateCell(col++) as HSSFCell, noWrapCellStyle , "Note:");
		}else{
			setText(dataRow.CreateCell(col++) as HSSFCell, noWrapCellStyle ,"註:");
		}*/
		/*for(int i = 0; i <  notes.Count; i++){
			C_S_HTML_NOTES sHtmlNotes =  notes.get(i);

			dataRow = sheetFile.CreateRow(row++);
			
			
			if(lang.Equals(LANG_ENG)){
				setText(dataRow.CreateCell(0), noWrapCellStyle ,
						stringUtil.getDisplay(sHtmlNotes.getCode())+". "+
						stringUtil.getDisplay(sHtmlNotes.getEnglishDescription()));
				
				//sheetFile.AddMergedRegion(new Region(row-1 , (short)(0) , row-1,(short) (4) ));
			}else{
				setText(dataRow.CreateCell(0), noWrapCellStyle ,
						stringUtil.getDisplay(sHtmlNotes.getCode())+". "+
						stringUtil.getDisplay(sHtmlNotes.getChineseDescription()));
				//sheetFile.AddMergedRegion(new Region(row-1 , (short)(0) , row-1,(short) (4) ));
			}
		}*/
		for(int i = 0; i <  extraNoteList.Count; i++){
			C_S_SYSTEM_VALUE sSystemValue =  extraNoteList[i];
			dataRow = sheetFile.CreateRow(row++) as HSSFRow;
			if(lang.Equals(LANG_ENG)){
				setText(dataRow.CreateCell(0) as HSSFCell, noWrapCellStyle,
							stringUtil.getDisplay(sSystemValue.ENGLISH_DESCRIPTION));
			}else{
				setText(dataRow.CreateCell(0) as HSSFCell, noWrapCellStyle,
						stringUtil.getDisplay(sSystemValue.CHINESE_DESCRIPTION));
			}
		}
	}
	
	
	protected static void createRemarks(HSSFWorkbook wb, HSSFSheet sheetFile, String fileTemplate,
											List<C_S_SYSTEM_VALUE> bsitem, String lang,
											List<C_S_SYSTEM_VALUE> extraRemarksList){
	
		int row = sheetFile.LastRowNum+2;
		int col=0;
		
		HSSFCellStyle noWrapCellStyle = wb.CreateCellStyle() as HSSFCellStyle;
        noWrapCellStyle.WrapText = false;
		
		
		HSSFRow dataRow =  sheetFile.CreateRow(row++) as HSSFRow;
		/*if(lang.Equals(LANG_ENG)){
			setText(dataRow.CreateCell(0), noWrapCellStyle, "Remark:");
		}else{
			setText(dataRow.CreateCell(0), noWrapCellStyle, "備註:");
		}*/
		for(int i = 0; i <  extraRemarksList.Count; i++){
            C_S_SYSTEM_VALUE sSystemValue = extraRemarksList[i];
			dataRow = sheetFile.CreateRow(row++) as HSSFRow;
			if(lang.Equals(LANG_ENG)){
				setText(dataRow.CreateCell(0) as HSSFCell, noWrapCellStyle, 
						stringUtil.getDisplay(sSystemValue.ENGLISH_DESCRIPTION));

			}else{
				setText(dataRow.CreateCell(0) as HSSFCell, noWrapCellStyle, 
						stringUtil.getDisplay(sSystemValue.CHINESE_DESCRIPTION));

			}
		}
		
		
		
		/*HSSFRow dataRow = null;
		
		if(ENGLISH_QP_RI_TEMPLATE.Equals(fileTemplate) ||
		   CHINESE_QP_RI_TEMPLATE.Equals(fileTemplate)){
			bsitem.clear();
		}
		
		if ((ENGLISH_COMP_NOBS_TEMPLATE.Equals(fileTemplate)) ||
			(CHINESE_COMP_NOBS_TEMPLATE.Equals(fileTemplate)) ||
			(ENGLISH_RI_TEMPLATE.Equals(fileTemplate)) ||
			(CHINESE_RI_TEMPLATE.Equals(fileTemplate))
		){
			
		}else{
			dataRow = sheetFile.CreateRow(row++);
			
			if(lang.Equals(LANG_ENG)){
				setText(dataRow.CreateCell(col++) as HSSFCell, noWrapCellStyle, "Remark:");
			}else{
				setText(dataRow.CreateCell(col++) as HSSFCell,noWrapCellStyle, "備註:");
			}
			for(int i = 0; i <  bsitem.Count; i++){
				int j =0;
				dataRow = sheetFile.CreateRow(row++);
				col=0;
				C_S_SYSTEM_VALUE sSystemValue =  bsitem.get(i);
				if(lang.Equals(LANG_ENG)){
					setText(dataRow.CreateCell(col),  noWrapCellStyle, 
							"'"+stringUtil.getDisplay(sSystemValue.getCode())+"' "+
							stringUtil.getDisplay(sSystemValue.getEnglishDescription()));
				}else{
					setText(dataRow.CreateCell(col),  noWrapCellStyle, 
							"'"+stringUtil.getDisplay(sSystemValue.getCode())+"' "+
							stringUtil.getDisplay(sSystemValue.getChineseDescription()));
				}
			}
		}
		
		row++;
		
		
		for(int i = 0; i <  extraRemarksList.Count; i++){
			C_S_SYSTEM_VALUE sSystemValue =  extraRemarksList.get(i);
			col=0;
			dataRow = sheetFile.CreateRow(row++);
			setText(dataRow.CreateCell(col),  noWrapCellStyle,  sSystemValue.getEnglishDescription());
			//sheetFile.AddMergedRegion(new Region(row-1 , (short)(0) , row-1,(short) (4) ));

		}*/
		
	}
	
		
	

	
	
	protected static String getRegistorString(String item){
		if(item == null){
			return BLANK;
		}
		if(item.Equals("")){
			return BLANK;
		}else {
			return item;
		}
			
	}
	
	private static String getASRegistorString(String item){
		if(item == null){
			return "#";
		}
		if(item.Equals("")){
			return "#";
		}else {
			item = item.Replace("<BR> ", "\n");
			item = item.Replace("<BR>", "\n");
			return item;
		}
	}
	
	
	protected static void createRegistorList(HSSFWorkbook wb, HSSFSheet sheetFile, List<String[]> registerList, 
			bool isComp, bool isbs, String catCode, bool isQP, bool showFSS, bool showMBIS){
		
		int row = sheetFile.LastRowNum+1;

        try {
            HSSFCellStyle wrapCellStyle = wb.CreateCellStyle() as HSSFCellStyle;
            wrapCellStyle.WrapText = true;
            wrapCellStyle.VerticalAlignment = VerticalAlignment.Center;//.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);


            for (int i = 0; i < registerList.Count; i++)
            {
                String[] register = registerList[i];
                HSSFRow dataRow = sheetFile.CreateRow(row++) as HSSFRow;
                int col = 0;
                int j = 0;
                int sdv = 0;
                if (isComp)
                {
                    String englishName = register[j++];
                    String cerNo = register[j++];
                    String expiryDate = register[j++];
                    String bsCodeList = register[j++];
                    String telNo = register[j++];
                    String asNameList = register[j++];
                    String flag = register[j++];
                    String interestedFSS = register[j++];
                    String mbisRI = register[j++];
                    String region = "";
                    String email = "";
                    String fax = "";

                    // Checking added at 2015.09.04 
                    // because 'region', 'email', 'fax' 
                    // should be removed in function 'exportRegistersData' in ExportDataManager.java at 2014
                    // So, prevent out of index exception by checking the register.Length.
                    if (register.Length >= 10)
                    {
                        region = register[j++];
                        email = register[j++];
                        fax = register[j++];
                    }
                    sdv = cerNo.IndexOf("SC(V)");

                    if (getRegistorString(bsCodeList).Equals(BLANK))
                    {
                        if (sdv != 0)
                        {
                            telNo = "";
                        }
                        region = "";
                        email = "";
                        fax = "";

                    }
                    if (isbs)
                    {
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(englishName));
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getASRegistorString(asNameList));


                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(cerNo));
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(expiryDate) + flag);
                        if (!isQP)
                        {
                            setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(bsCodeList));
                        }

                        if (showFSS)
                        {
                            setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(interestedFSS));
                        }
                        if (register.Length >= 10)
                        {
                            setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(region));
                            setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(email));
                            setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(fax));
                        }
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(telNo));
                    }
                    else
                    {
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(englishName));
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getASRegistorString(asNameList));
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(cerNo));
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(expiryDate) + flag);

                        if (showFSS)
                        {
                            setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(interestedFSS));
                        }
                        if (register.Length >= 10)
                        {
                            setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(region));
                            setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(email));
                            setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(fax));
                        }
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(telNo));
                    }
                }
                else
                {
                    String englishName = register[j++];
                    String cerNo = register[j++];
                    String expiryDate = register[j++];
                    String bsCodeList = register[j++];
                    String telNo = register[j++];
                    String asNameList = register[j++];
                    String flag = register[j++];
                    String interestedFSS = register[j++];
                    String mbisRI = register[j++];
                    String region = "";
                    String email = "";
                    String fax = "";

                    if (register.Length >= 10)
                    {
                        region = register[j++];
                        email = register[j++];
                        fax = register[j++];
                    }
                    if (getRegistorString(bsCodeList).Equals(BLANK))
                    {
                        telNo = "";
                        region = "";
                        email = "";
                        fax = "";

                    }
                    setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(englishName));
                    setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(cerNo));
                    setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(expiryDate + flag));
                    if (!(catCode.Equals("RI(A)") || catCode.Equals("RI(E)") || catCode.Equals("RI(S)")) && !isQP)
                    {
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(bsCodeList));
                    }
                    else
                    {
                        j++;
                    }
                    if (showMBIS)
                    {
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(mbisRI));
                    }

                    if (showFSS)
                    {
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(interestedFSS));
                    }
                    if (register.Length >= 10)
                    {
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(region));
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(email));
                        setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(fax));
                    }
                    setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(telNo));
                }
            }


        } catch (Exception ex) {

            Console.WriteLine(ex.Message); }
			
		
	}
	
	
	private static void setText(HSSFCell cell, HSSFCellStyle style, String value){
		cell.SetCellValue(value);
		cell.CellStyle = style;
	}

	/**
	private static void setText(HSSFCell cell, String value){
		cell.setCellValue(value);
		HSSFCellStyle cs = cell.getCellStyle();
		cs.setWrapText(true);
		cell.setCellStyle(cs);
	}
	
	private static void setTextNoWrap(HSSFCell cell, String value){
		cell.setCellValue(value);
		HSSFCellStyle cs = cell.getCellStyle();
		cs.setWrapText(false);
		cell.setCellStyle(cs);
	}
	**/
	
	private static void createHeader(HSSFWorkbook wb, HSSFSheet sheetFile, String title, String fileTemplate, bool showFSS, bool showMBIS){
	
		int row = 0;
		int col = 0;

        HSSFCellStyle csBOLD = wb.CreateCellStyle() as HSSFCellStyle;
        HSSFFont font = wb.CreateFont() as HSSFFont;
        font.Boldweight = (short)FontBoldWeight.Bold; // HSSFFont.BOLDWEIGHT_BOLD;
        csBOLD.SetFont(font);
        csBOLD.WrapText = true;
        HSSFRow headerRow = sheetFile.CreateRow(row++) as HSSFRow;

		
		
		
		
		setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,title );

		headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
		if(	ENGLISH_REGISTER_TEMPLATE.Equals(fileTemplate) || 
			ENGLISH_REGISTER_API_TEMPLATE.Equals(fileTemplate) ||
			ENGLISH_REGISTER_APII_TEMPLATE.Equals(fileTemplate) ||
			ENGLISH_REGISTER_APIII_TEMPLATE.Equals(fileTemplate) 
		){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,  "Name");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Registration Number");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Expiry Date\n(See Remark 4 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Remark 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col + (showFSS ? 1 : 0)) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Service in Building Safety\n(See Remark 2 below)");
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Interested in Providing Services in Fire Safety\n(See Remark 5 below)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"District Area");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Email Address");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Fax No.");
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Phone Number");
		}
		
		
		if(	CHINESE_REGISTER_TEMPLATE.Equals(fileTemplate) || 
			CHINESE_REGISTER_API_TEMPLATE.Equals(fileTemplate) ||
			CHINESE_REGISTER_APII_TEMPLATE.Equals(fileTemplate) ||
			CHINESE_REGISTER_APIII_TEMPLATE.Equals(fileTemplate) 
		){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"姓 名");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"註冊編號");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"屆滿日期\n(見備註 4)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"備註 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col + (showFSS ? 1 : 0)) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"樓宇安全服務範圍\n(見備註 2)");
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"提供消防安全改善服務\n(見備註 5)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"傳真號碼");
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"聯絡電話");
		}
		
		if(	ENGLISH_COMP_TEMPLATE.Equals(fileTemplate)){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Company Name");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Name of Authorized Signatory\n(See Remark 3 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Registration Number");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Expiry Date\n(See Remark 4 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Remark 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col + (showFSS ? 1 : 0)) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Service in Building Safety\n(See Remark 2 below)");
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Interested in Providing Services in Fire Safety\n(See Remark 5 below)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"District Area");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Email Address");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Fax No.");
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Phone Number");
		}
		if(	CHINESE_COMP_TEMPLATE.Equals(fileTemplate)){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"公司名稱");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"獲授權簽署人的姓名\n(見備註 3)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"註冊編號");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"屆滿日期\n(見備註 4)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"備註 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col + (showFSS ? 1 : 0)) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"樓宇安全服務範圍\n(見備註 2)");
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"提供消防安全改善服務\n(見備註 5)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"傳真號碼");
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"聯絡電話");
		}
		
		if(	ENGLISH_COMP_NOBS_TEMPLATE.Equals(fileTemplate)){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Company Name");
			//sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Name of Authorized Signatory\n(See Note 3 below)");
			//sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Registration Number");
			//sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Expiry Date\n(See Remark 4 below)");
			//sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col) ));
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Interested in Providing Services in Fire Safety\n(See Remark 5 below)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"District Area");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Email Address");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Fax No.");
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Phone Number\n(See Remark 1 below)");
		}

		if(	CHINESE_COMP_NOBS_TEMPLATE.Equals(fileTemplate)){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"公司名稱");
			//sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"獲授權簽署人的姓名\n(見備註 3)");
			//sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"註冊編號");
			//sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"屆滿日期\n(見備註 4)");
			//sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col) ));
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"提供消防安全改善服務\n(見備註 5)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"傳真號碼");
			
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"聯絡電話\n(見備註 1)");
		}
		
		
		if(	ENGLISH_RI_TEMPLATE.Equals(fileTemplate) ||
			ENGLISH_QP_RI_TEMPLATE.Equals(fileTemplate) 
		){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,  "Name");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Registration Number");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Expiry Date\n(See Remark 4 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			
			
			if(showMBIS){
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Service in MBIS\n(See Remark 6 below)");		
				sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			}
			
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Remark 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col-1 + (showFSS ? 1 : 0) )));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Interested in Providing Services in Fire Safety\n(See Remark 5 below)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"District Area");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Email Address");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Fax No.");
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Phone Number");
			

			/*col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Name");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Registration Number");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Expiry Date\n(See Remark 4 below)");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Phone Number");*/
		}

		if(	CHINESE_RI_TEMPLATE.Equals(fileTemplate)||
			CHINESE_QP_RI_TEMPLATE.Equals(fileTemplate) ){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,  "姓 名");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"註冊編號");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"屆滿日期\n(見備註 4)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			
			
			if(showMBIS){
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"強制驗樓計劃服務\n(見備註 6)");
				sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			}
			
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"備註 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col-1 + (showFSS ? 1 : 0) )));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"提供消防安全改善服務\n(見備註 5)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"傳真號碼");
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"聯絡電話");
			
			/*col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"姓 名");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"註冊編號");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"屆滿日期");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"聯絡電話");*/
			
		}
		
		
		if(	ENGLISH_QP_COMP_TEMPLATE.Equals(fileTemplate)){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Company Name");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Name of Authorized Signatory (See Note 1 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Registration Number");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Expiry Date");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Remark");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col-1 + (showFSS ? 1 : 0))));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Interested in Providing Services in Fire Safety\n(See Remark 5 below)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"District Area");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Email Address");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Fax No.");
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Phone Number");
		}
		
		if(	CHINESE_QP_COMP_TEMPLATE.Equals(fileTemplate)){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"公司名稱");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"獲授權簽署人的姓名(見註 1)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"註冊編號");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"屆滿日期");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"備註");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col-1 + (showFSS ? 1 : 0)) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"提供消防安全改善服務\n(見備註 5)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"傳真號碼");
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"聯絡電話");
		}
		
		
		if(	ENGLISH_QP_REGISTER_TEMPLATE.Equals(fileTemplate)
		){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Name");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Registration Number");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Expiry Date");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Remark");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col-1 + (showFSS ? 1 : 0)) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Interested in Providing Services in Fire Safety\n(See Remark 5 below)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"District Area");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Email Address");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Fax No.");
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Phone Number");
		}
	
		if(	CHINESE_QP_REGISTER_TEMPLATE.Equals(fileTemplate)){
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"姓 名");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"註冊編號");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"屆滿日期");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"備註");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col-1 + (showFSS ? 1 : 0)) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			if (showFSS)
			{
				setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"提供消防安全改善服務\n(見備註 5)");		
			}
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"傳真號碼");
			*/
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"聯絡電話");
		}
	}
	
	

	
}
