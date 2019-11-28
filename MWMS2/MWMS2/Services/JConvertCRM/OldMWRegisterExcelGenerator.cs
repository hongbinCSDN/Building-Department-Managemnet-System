

using MWMS2.Entity;
using MWMS2.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;

public class OldMWRegisterExcelGenerator : OldMWRegisterHTMLGenerator{
	

	public static string FILE_XLS =".xls";
	
	
	
	
	public static List<FileInfo> generatorMWWebSite(
			String lang,
			String templateFilePath, string templateName,	
			String filePath, string fileNameStart,
			List<Object[]> registerData, 
			String title,
			List<C_S_SYSTEM_VALUE> noteList,
			List<C_S_SYSTEM_VALUE> remarksList){

        List<FileInfo> resultList = new List<FileInfo>();
		
		try{
			String fileName = changeFileNameForExcelDataExport(fileNameStart)+FILE_XLS;
			//String fileName = fileNameStart+FILE_XLS;
			HSSFWorkbook wb =  new  HSSFWorkbook();
			HSSFSheet sheet = wb.CreateSheet(fileNameStart) as HSSFSheet;
	
			createHeader( wb, sheet,  title, templateName);
			createMWIList(wb, sheet, registerData, lang);
			
			for (int j = 0; j < 15; j++) {
	            sheet.AutoSizeColumn((short)j, true);
			}
			
			createNotes(wb, sheet, noteList, lang);
			createRemarks( wb,  sheet, remarksList,  lang);
            
            FileStream outFile = new FileStream(filePath+fileName, FileMode.CreateNew);
		 	wb.Write(outFile);
            outFile.Close();
            FileInfo desc = new FileInfo(filePath+fileName);
			resultList.Add(desc);
			
		}catch(Exception e){
            Console.WriteLine(e);//			System.out.println(e);
		}
		return resultList;
	}
	
	


	
	public static List<FileInfo> generatorMWCompanyWebSite(
			String lang,
			String templateFilePath, string templateName,	
			String filePath, string fileNameStart,
            List<OldMWCompanyObjectForHTML> registerData,
			String title,
            List<C_S_SYSTEM_VALUE> noteList,
            List<C_S_SYSTEM_VALUE> remarksList){

        List<FileInfo> resultList = new List<FileInfo>();
		
		try{
			String fileName = changeFileNameForExcelDataExport(fileNameStart)+FILE_XLS;
			//String fileName = fileNameStart+FILE_XLS;
			HSSFWorkbook wb =  new  HSSFWorkbook();
			HSSFSheet sheet = wb.CreateSheet(fileNameStart) as HSSFSheet;
	
			createHeader( wb, sheet,  title, templateName);
			createMWCompanyWebSite(wb, sheet, registerData, lang, templateName);
			
			for (int j = 0; j < 15; j++) {
	            sheet.AutoSizeColumn((short)j, true);
			}
			
			createNotes(wb, sheet, noteList, lang);
			createRemarks( wb,  sheet, remarksList,  lang);

            FileStream outFile = new FileStream(filePath + fileName, FileMode.CreateNew);
            //FileOutputStream out = new FileOutputStream(filePath+fileName);
			wb.Write(outFile);
            outFile.Close();
			FileInfo desc = new FileInfo(filePath+fileName);
			resultList.Add(desc);
			
		}catch(Exception e){
            Console.WriteLine(e);////System.out.println(e);
		}
		return resultList;
	}

	private static void createHeader(HSSFWorkbook wb, HSSFSheet sheetFile, string title,  string fileTemplate){


        int row = sheetFile.LastRowNum + 1;
        int col = 0;
		
		
		HSSFCellStyle csBOLD = wb.CreateCellStyle() as HSSFCellStyle;
		HSSFFont font = wb.CreateFont() as HSSFFont;
        font.Boldweight = (short)FontBoldWeight.Bold; // HSSFFont.BOLDWEIGHT_BOLD;
        csBOLD.SetFont(font);
        csBOLD.WrapText = true;
		HSSFRow headerRow = sheetFile.CreateRow(row++) as HSSFRow;
	
		String sPARA_TITLE= "";
		
		
		if(fileTemplate.Equals(ENGLISH_MWC_P_TEMPLATE)){
			sPARA_TITLE = "Registered Minor Works Contractors (Provisional)";
	
			setText(headerRow.CreateCell(col++) as HSSFCell , csBOLD, 
					stringUtil.isBlank(title)? sPARA_TITLE: title );
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,  "Company Name");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Class\n(See Note 2 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Type\n(See Note 3 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Name of Authorized Signatory with Registered Class/Type of Minor Works\n(See Remark 3 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row,(short) (col) ));
			col++;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Registration Number");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Expiry Date\n(See Remark 5 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Remark 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col+3) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Interested in Providing Services in Fire Safety\n(See Remark 5 below)");		
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Phone Number");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "District area");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Email address");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Fax no.");
			
		}else if(fileTemplate.Equals(CHINESE_MWC_P_TEMPLATE)){
			
			sPARA_TITLE = "臨時註冊小型工程承建商";
			
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, 
					stringUtil.isBlank(title)? sPARA_TITLE: title );
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,  "公司名稱");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "級別\n(見註釋 2)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "類型\n(見註釋 3)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "獲授權簽署人的姓名及其獲註冊的級別/類型的小型工程\n(見備註 3)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row,(short) (col) ));
			col++;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "註冊編號");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "屆滿日期\n(見備註 5)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"備註 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col+3) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "提供消防安全改善服務\n(見備註 5)");		
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "聯絡電話");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "傳真號碼");
			/*
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "公司名稱");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "級別 *註釋1");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "類型 *註釋2");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "獲授權簽署人的姓名及其獲註冊的級別/類型的小型工程");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col) ));
			col++;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "註冊號碼");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "屆滿日期");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "聯絡電話 *備註2(資料由承建商以自願性質提供)");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "傳真號碼");*/
	
		}
		
		if(fileTemplate.Equals(ENGLISH_MWC_TEMPLATE)){
			
			sPARA_TITLE = "Registered Minor Works Contractors (Company)";
			
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, 
					stringUtil.isBlank(title)? sPARA_TITLE: title );
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			col = 0;
			
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,  "Company Name");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Class\n(See Note 2 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Type\n(See Note 3 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Name of Authorized Signatory with Registered Class/Type of Minor Works\n(See Remark 3 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row,(short) (col) ));
			col++;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Registration Number");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Expiry Date\n(See Remark 4 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Remark 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col+3) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Interested in Providing Services in Fire Safety\n(See Remark 5 below)");		
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Phone Number");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "District area");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Email address");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Fax no.");
			
		}else if(fileTemplate.Equals(CHINESE_MWC_TEMPLATE)){
			
			sPARA_TITLE = "註冊小型工程承建商(公司)";
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, 
					stringUtil.isBlank(title)? sPARA_TITLE: title );
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,  "公司名稱");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "級別\n(見註釋 2)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "類型\n(見註釋 3)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "獲授權簽署人的姓名及其獲註冊的級別/類型的小型工程\n(見備註 3)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row,(short) (col) ));
			col++;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "註冊編號");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "屆滿日期\n(見備註 4)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"備註 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col+3) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "提供消防安全改善服務\n(見備註 5)");		
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "聯絡電話");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "傳真號碼");
		}else if(fileTemplate.Equals(ENGLISH_QP_MWC_TEMPLATE)){

			sPARA_TITLE = "Qualified Persons(Registered Minor Works Contractors (Provisional) & Registered Minor Works Contractors (Company))";
			
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, 
					stringUtil.isBlank(title)? sPARA_TITLE: title );
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,  "Company Name");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Class *Note1");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Type *Note2");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Name of Authorized Signatory with Registered Class/Type of Minor Works");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col) ));
			col++;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Registration Number");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Expiry Date");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Interested in Providing Services in Fire Safety\n(See Remark 5 below)");		
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Telephone Number *Remark2 (Provided by the contractors voluntarily)");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "District area");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Email address");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Fax no.");
			
		}else if(fileTemplate.Equals(CHINESE_QP_MWC_TEMPLATE)){
			
			sPARA_TITLE = "合資格人士(臨時註冊小型工程承建商及註冊小型工程承建商(公司))";
			
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, 
					stringUtil.isBlank(title)? sPARA_TITLE: title );
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "公司名稱");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "級別 *註釋1");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "類型 *註釋2");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "獲授權簽署人的姓名及其獲註冊的級別/類型的小型工程");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col) ));
			col++;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "註冊號碼");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "屆滿日期");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "提供消防安全改善服務\n(見備註 5)");		
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "聯絡電話 *備註2(資料由承建商以自願性質提供)");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "傳真號碼");
		}
		

		if(fileTemplate.Equals(ENGLISH_MWI_TEMPLATE)){
			
			sPARA_TITLE = "Registered Minor Works Contractors (Individual)";
			
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, 
					stringUtil.isBlank(title)? sPARA_TITLE: title );
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,  "Contractor Name");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Items of Class III Minor Works\n(See Note 5 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Registration Number");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Expiry Date\n(See Remark 4 below)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row,(short) (col-1) ));
		
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"Remark 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col+3) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Interested in Providing Services in Fire Safety\n(See Remark 5 below)");		
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Phone Number");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "District area");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Email address");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Fax no.");
			
		}else if(fileTemplate.Equals(CHINESE_MWI_TEMPLATE)){
			
			sPARA_TITLE = "註冊小型工程承建商(個人)" ;
			
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, 
					stringUtil.isBlank(title)? sPARA_TITLE: title );
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			col = 0;
			
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,  "承建商姓名");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "獲註冊第III級別小型工程項目\n(見註釋 5)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "註冊編號");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row ,(short) (col-1) ));
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "屆滿日期\n(見備註 4)");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row,(short) (col-1) ));
			
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,"備註 1");
			sheetFile.AddMergedRegion(new Region(row-1 , (short)(col-1) , row -1,(short) (col+3) ));
			col--;
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "提供消防安全改善服務\n(見備註 5)");		
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "聯絡電話");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "傳真號碼");
			/*setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "承建商姓名");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "獲註冊第III級別小型工程項目 *註釋1");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "註冊號碼");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "屆滿日期");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "聯絡電話 *備註2 (資料由承建商以自願性質提供)");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "傳真號碼");*/
			
		}else if(fileTemplate.Equals(ENGLISH_QP_MWI_TEMPLATE)){

			sPARA_TITLE = "Qualified Persons(Registered Minor Works Contractors (Individual)) *Remark1";
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, 
					stringUtil.isBlank(title)? sPARA_TITLE: title );
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD,  "Contractor Name");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Items of Class III Minor Works *Note 1");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Registration Number");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Expiry Date");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Interested in Providing Services in Fire Safety\n(See Remark 5 below)");		
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Phone Number *Remark 2 (Information provided by the contractors voluntarily)");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "District area");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Email address");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "Fax no.");
			
			
		}else if(fileTemplate.Equals(CHINESE_QP_MWI_TEMPLATE)){
			
			sPARA_TITLE = "合資格人士(註冊小型工程承建商(個人)) *備註1" ;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, 
					stringUtil.isBlank(title)? sPARA_TITLE: title );
			headerRow =  sheetFile.CreateRow(row++) as HSSFRow;
			col = 0;
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "承建商姓名");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "獲註冊第III級別小型工程項目 *註釋1");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "註冊號碼");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "屆滿日期");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "提供消防安全改善服務\n(見備註 5)");		
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "聯絡電話 *備註2 (資料由承建商以自願性質提供)");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "地區");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "電郵地址");
			setText(headerRow.CreateCell(col++) as HSSFCell, csBOLD, "傳真號碼");
		}
	}
	
	
	protected static void createNotes(HSSFWorkbook wb, HSSFSheet sheetFile,
			List<C_S_SYSTEM_VALUE> noteList, string lang){
		if(noteList == null || noteList.Count == 0){
			return;
		}
	
		HSSFCellStyle noWrapCellStyle = wb.CreateCellStyle() as HSSFCellStyle;
        noWrapCellStyle.WrapText = false;
			
		int row = sheetFile.LastRowNum+2;
		int col=0;
		
		HSSFRow dataRow = sheetFile.CreateRow(row++) as HSSFRow;
		//if(lang.Equals(LANG_ENG)){
			//setText(dataRow.CreateCell(col++),noWrapCellStyle, "Notes :");
		//}else{
			//setText(dataRow.CreateCell(col++), noWrapCellStyle,"註:");	
		//}
		for(int i = 0; i <  noteList.Count; i++){
            C_S_SYSTEM_VALUE sSystemValue =  noteList[i];
			dataRow = sheetFile.CreateRow(row++)  as HSSFRow;
			if(lang.Equals(LANG_ENG)){
				setText(dataRow.CreateCell(0) as HSSFCell, noWrapCellStyle,
							stringUtil.getDisplay(sSystemValue.ENGLISH_DESCRIPTION));
			}else{
				setText(dataRow.CreateCell(0) as HSSFCell, noWrapCellStyle,
						stringUtil.getDisplay(sSystemValue.CHINESE_DESCRIPTION));
			}
		}
	}

	protected static void createRemarks(HSSFWorkbook wb, HSSFSheet sheetFile, 
			List<C_S_SYSTEM_VALUE> remarksList, string lang){

        if (remarksList == null || remarksList.Count == 0)
        {
			return;
		}
		
		HSSFCellStyle noWrapCellStyle = wb.CreateCellStyle() as HSSFCellStyle;
        noWrapCellStyle.WrapText = false;

        int row = sheetFile.LastRowNum+2;
		
		HSSFRow dataRow =  sheetFile.CreateRow(row++) as HSSFRow;
		/*if(lang.Equals(LANG_ENG)){
			setText(dataRow.CreateCell(0), noWrapCellStyle, "Remark:");
		}else{
			setText(dataRow.CreateCell(0), noWrapCellStyle, "備註:");
		}*/
		for(int i = 0; i <  remarksList.Count; i++){
            C_S_SYSTEM_VALUE sSystemValue =  remarksList[i];
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

	
	
	
	protected static void createMWIList(HSSFWorkbook wb, HSSFSheet sheetFile,
			 List<Object[]> registerList,  string lang){
		
		int row = sheetFile.LastRowNum +1;

		HSSFCellStyle wrapCellStyle = wb.CreateCellStyle() as HSSFCellStyle;
		wrapCellStyle.WrapText = true;
        wrapCellStyle.VerticalAlignment = VerticalAlignment.Center;//wrapCellStyle.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);
    
        try {
            for (int i = 0; i < registerList.Count; i++)
            {
                int j = 0;
                Object[] register = registerList[i];

                String cerNo = "";
                String surName = "";
                String giveName = "";
                String chineseName = "";
                String simplifiedChineseName = "";
                String expiryDate = "";
                String telPhone = "";
                String items = "";
                String nameDisplay = "";

                if (!register[j].Equals(System.DBNull.Value))
                    cerNo = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;

                if (!register[j].Equals(System.DBNull.Value))
                    surName = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;

                if (!register[j].Equals(System.DBNull.Value))
                    giveName = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;
                if (!register[j].Equals(System.DBNull.Value))
                    chineseName = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;


                simplifiedChineseName = (chineseName);

                if (!register[j].Equals(System.DBNull.Value))
                    expiryDate = OldDateUtil.getDateDisplayFormat(((DateTime)register[j++]).ToString());
                else
                    j++;

                if (!register[j].Equals(System.DBNull.Value))
                    telPhone = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;

                if (!register[j].Equals(System.DBNull.Value))
                    items = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;

                nameDisplay = (surName + " " + giveName).ToUpper();


                String regionEng = "";
                String regionChi = "";
                String email = "";
                String bsFaxNo = "";
                String flag = "";
                String interestedFSS = "";
                String interestedFSSChi = "";
                String regionSChi = "";
                String regionDisplay = "";
                if (!register[j].Equals(System.DBNull.Value))
                    regionEng = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;

                if (!register[j].Equals(System.DBNull.Value))
                    regionChi = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;

               
                    regionSChi = (regionChi);
              
                if (!register[j].Equals(System.DBNull.Value))
                    email = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;
                if (!register[j].Equals(System.DBNull.Value))
                    bsFaxNo = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;
                if (!register[j].Equals(System.DBNull.Value))
                    flag = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;

                if (!register[j].Equals(System.DBNull.Value))
                    interestedFSS = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;

                if (!register[j].Equals(System.DBNull.Value))
                    interestedFSSChi = stringUtil.getDisplay((String)register[j++]);
                else
                    j++;

         
                regionDisplay = regionEng;
          
                if (lang.Equals(LANG_CHI))
                {
                    nameDisplay = chineseName + " " + nameDisplay;
                    regionDisplay = regionChi;
                }
                else if (lang.Equals(LANG_SCH))
                {
                    nameDisplay = simplifiedChineseName + " " + nameDisplay;
                    regionDisplay = regionSChi;
                }

                HSSFRow dataRow = sheetFile.CreateRow(row++) as HSSFRow;
                int col = 0;
                setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(nameDisplay));
                setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(items));

                setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(cerNo));
                setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(expiryDate + flag));
                setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(lang.Equals(LANG_CHI) ? interestedFSSChi : interestedFSS));
                setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(telPhone));
                setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(regionDisplay));
                setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(email));
                setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(bsFaxNo));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
     
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
    protected static void createMWCompanyWebSite( HSSFWorkbook wb, HSSFSheet sheetFile,
			List<OldMWCompanyObjectForHTML> registerDataList,  string lang, string templateName){
		
		HSSFCellStyle wrapCellStyle = wb.CreateCellStyle() as HSSFCellStyle;
        wrapCellStyle.WrapText = true;
        wrapCellStyle.VerticalAlignment = VerticalAlignment.Center;// HSSFCellStyle.VERTICAL_CENTER;


        int row = sheetFile.LastRowNum+1;
		
		for(int i = 0; i < registerDataList.Count; i++){
			int j =0;
			
			OldMWCompanyObjectForHTML register = registerDataList[i];
			if(lang.Equals(LANG_CHI) || lang.Equals(LANG_SCH)){
				register.setDisplayChi(true);
			}else{
				register.setDisplayChi(false);
			}
			
			String companyClass = "";
			String companyType = "";
			if(!register.getTypeOne().Equals("")){
				companyClass += "I, II, III \n";
				companyType += register.getTypeOne()+ "\n";
			}
			if(!register.getTypeTwo().Equals("")){
				companyClass += "II, III \n";
				companyType += register.getTypeTwo()+ "\n";
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
			int totalLine = register.getNumberOfLine();		
			
			HSSFRow dataRow = sheetFile.CreateRow(row) as HSSFRow;
			int col = 0;
			setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(register.getDisplayName()));
			setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(companyClass));
			setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(companyType));
			
			String regStr = "";
					
		
			
			int startRowNumber = row;
			
			for(int asCounter = 0 ; asCounter < iNumberOfAS; asCounter++){
				
				OldMWCompanyauthorizedSignatoryObject asObject = register.getASObject(asCounter);
				
				int asObjectLine = asObject.getLineNumber();
				
				dataRow = sheetFile.GetRow(startRowNumber) as HSSFRow;
				if(dataRow == null){ 
					dataRow = sheetFile.CreateRow(startRowNumber) as HSSFRow;	
				}
				setText(dataRow.CreateCell(col) as HSSFCell, wrapCellStyle, getASRegistorString(asObject.getDisplayName()));
			
				
				for(int eachASLineCounter = 0 ; eachASLineCounter < asObjectLine; eachASLineCounter++){

					int startAsLine = startRowNumber + eachASLineCounter*2;
					dataRow = sheetFile.GetRow(startAsLine) as HSSFRow;
					
					if(dataRow == null){ 
						dataRow = sheetFile.CreateRow(startAsLine) as HSSFRow;	
					}
					
					regStr = getRegistorString(getClassDisplay(asObject.isDisplayChi(), asObject.getTypeLine(eachASLineCounter+1)));
					regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
					setText(dataRow.CreateCell(col+1) as HSSFCell, wrapCellStyle, getASRegistorString(regStr));

					startAsLine++;
					dataRow = sheetFile.GetRow(startAsLine) as HSSFRow;
					if(dataRow == null){ 
						dataRow = sheetFile.CreateRow(startAsLine) as HSSFRow;	
					}
					regStr = getRegistorString(getTypeDisplay(asObject.isDisplayChi(), asObject.getTypeLine(eachASLineCounter+1)));
					regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
					setText(dataRow.CreateCell(col+1) as HSSFCell, wrapCellStyle, getASRegistorString(regStr));
				}
				startRowNumber = startRowNumber + asObjectLine*2;
			}
			col++;
			col++;
			dataRow = sheetFile.GetRow(row) as HSSFRow;
			setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(register.getRegistrationNumber()));
			
			if(templateName.Equals(ENGLISH_MWC_P_TEMPLATE) || templateName.Equals(CHINESE_MWC_P_TEMPLATE)|| templateName.Equals(SIMPLIFIED_CHINESE_MWC_P_TEMPLATE) ||
			   getRegistorString(register.getRegistrationNumber()).StartsWith("MWC(P)")		
            ){
				if(!register.getExpiryDate().Equals(EXPIRY_DATE)){
					if(register.isDisplayChi()){
						setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, EXPIRY_DATE_CHI+"^");
					}else{
						setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, EXPIRY_DATE_CHI+"^");
					}
				}else{
					setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, register.getExpiryDate()+"^");
				}
			}else{
				setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorStarString(register.getExpiryDate(),star));
			}
			
			setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(register.isDisplayChi() ? register.getInterestedFSSChi() : register.getInterestedFSS()));
			
			setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(register.getTelephoneNumber()));
			
			setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(register.getCompRegion()));
			setText(dataRow.CreateCell(col++) as HSSFCell,  wrapCellStyle, getRegistorString(register.getEmailAddress()));
			setText(dataRow.CreateCell(col++) as HSSFCell, wrapCellStyle, getRegistorString(register.getBsFaxNumber()));
			
	
			sheetFile.AddMergedRegion(new Region(row , (short)(0) , row+totalLine-1 ,(short) (0) ));
			sheetFile.AddMergedRegion(new Region(row , (short)(1) , row+totalLine-1 ,(short) (1) ));
			sheetFile.AddMergedRegion(new Region(row , (short)(2) , row+totalLine-1 ,(short) (2) ));
						
			int current =row;
			for(int asCounter = 0 ; asCounter < register.getNumberOfAS() ; asCounter++){
				OldMWCompanyauthorizedSignatoryObject asObject = register.getASObject(asCounter);
				
				int numberOfASLine = asObject.getLineNumber()*2;
				
				sheetFile.AddMergedRegion(new Region(current , (short)(3) , 
										current+ numberOfASLine-1 , (short) (3) ));
				
				current = current+asObject.getLineNumber()*2;
			}
			sheetFile.AddMergedRegion(new Region(row , (short)(5) , row+totalLine-1 ,(short) (5) ));
			sheetFile.AddMergedRegion(new Region(row , (short)(6) , row+totalLine-1 ,(short) (6) ));
			sheetFile.AddMergedRegion(new Region(row , (short)(7) , row+totalLine-1 ,(short) (7) ));
			sheetFile.AddMergedRegion(new Region(row , (short)(8) , row+totalLine-1 ,(short) (8) ));
			sheetFile.AddMergedRegion(new Region(row , (short)(9) , row+totalLine-1 ,(short) (9) ));
			sheetFile.AddMergedRegion(new Region(row , (short)(10) , row+totalLine-1 ,(short) (10) ));
			sheetFile.AddMergedRegion(new Region(row , (short)(11) , row+totalLine-1 ,(short) (11) ));
			
			row = totalLine+row;
		}
	}

	
	
	/**
	protected static string getMWCRegistor(String lang, int start, int end, ArrayList<MWCompanyObjectForHTML> registerData, string templateName){
		
		int stop = end;
		if(stop > registerData.Count){
			stop = registerData.Count;
		}
		StringBuffer sb = new StringBuffer();
		//ZHConverter converter = ZHConverter.getInstance(ZHConverter.SIMPLIFIED);
		
		for(int i = start; i < stop; i++){
			int j =0;
			MWCompanyObjectForHTML register = registerData[i];
			if(lang.Equals(LANG_CHI) || lang.Equals(LANG_SCH)){
				register.setDisplayChi(true);
			}else{
				register.setDisplayChi(false);
			}
			
			String companyClass = "";
			String companyType = "";
			if(!register.getTypeOne().Equals("")){
				companyClass += "I, II, III<br><br>";
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
			MWCompanyauthorizedSignatoryObject firstAS = register.getASObject(0);
			int asline = firstAS.getLineNumber();
			
			
			String asWidth =" width='100' ";
			
			sb.append("<TR>");
			sb.append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getDisplayName())+"</TD>");
			sb.append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(companyClass)+"</TD>");
			sb.append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(companyType)+"</TD>");
			sb.append("<TD ROWSPAN ="+asline*2  +asWidth+" > " +getASRegistorString(firstAS.getDisplayName())+"</TD>");
			String regStr = getRegistorString(getClassDisplay(firstAS.isDisplayChi(), firstAS.getTypeLine(1)));
			regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
			sb.append("<TD > " + regStr + "</TD>");
			sb.append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getRegistrationNumber())+"</TD>");

			
			if(templateName.Equals(ENGLISH_MWC_P_TEMPLATE) || templateName.Equals(CHINESE_MWC_P_TEMPLATE)|| 
					templateName.Equals(SIMPLIFIED_CHINESE_MWC_P_TEMPLATE) ||
					getRegistorString(register.getRegistrationNumber()).startsWith(ApplicationConstants.S_CATEGORY_CODE_MWC_P)		
			){
				if(!register.getExpiryDate().Equals(EXPIRY_DATE)){
					if(register.isDisplayChi()){
						sb.append("<TD ROWSPAN ="+ROWSPAN+"> " + getRegistorStarString(EXPIRY_DATE_CHI,star)  +"</TD>");
					}else{
						sb.append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorStarString(EXPIRY_DATE_CHI,star)+"</TD>");
					}
				}else{
					sb.append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorStarString(register.getExpiryDate(),star)+"</TD>");
				}
			}else{
				sb.append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorStarString(register.getExpiryDate(),star)+"</TD>");
			}
			sb.append("<TD bgColor='yellow' ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getTelephoneNumber())+"</TD>");
			
			sb.append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getCompRegion())+"</TD>");
			sb.append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getEmailAddress())+"</TD>");
			sb.append("<TD ROWSPAN ="+ROWSPAN+"> " +getRegistorString(register.getBsFaxNumber())+"</TD>");
			
			sb.append("</TR>");
			
			sb.append("<TR>");
			regStr = getRegistorString(getTypeDisplay(firstAS.isDisplayChi(), firstAS.getTypeLine(1)));
			regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
			sb.append("<TD>" + regStr + "</TD>");
			sb.append("</TR>");
			
			for(int eachASLineCounter = 1 ; eachASLineCounter < asline; eachASLineCounter++){
				
				regStr = getRegistorString(getClassDisplay(firstAS.isDisplayChi(), firstAS.getTypeLine(eachASLineCounter+1)));
				regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
				
				sb.append("<TR>");
				sb.append("<TD>" + regStr + "</TD>");
				sb.append("</TR>");
				
				regStr = getRegistorString(getTypeDisplay(firstAS.isDisplayChi(), firstAS.getTypeLine(eachASLineCounter+1)));
				regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
				
				sb.append("<TR>");
				sb.append("<TD>" + regStr + "</TD>");
				sb.append("</TR>");
			}
			
			for(int asCounter = 1 ; asCounter < iNumberOfAS; asCounter++){
				MWCompanyauthorizedSignatoryObject asObject = register.getASObject(asCounter);
				
				int asObjectLine = asObject.getLineNumber();
				
				regStr = getRegistorString(getClassDisplay(asObject.isDisplayChi(), asObject.getTypeLine(1)));
				regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
				
				sb.append("<TR>");
				sb.append("<TD ROWSPAN ="+asObjectLine*2  +asWidth+"> " +getASRegistorString(asObject.getDisplayName())+"</TD>");
				sb.append("<TD > " + regStr + "</TD>");
				sb.append("</TR>");
				sb.append("<TR>");
				
				regStr = getRegistorString(getTypeDisplay(asObject.isDisplayChi(), asObject.getTypeLine(1)));
				regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
				
				sb.append("<TD>" + regStr + "</TD>");
				sb.append("</TR>");
				
				for(int eachASLineCounter = 1 ; eachASLineCounter < asObjectLine; eachASLineCounter++){
					
					regStr = getRegistorString(getClassDisplay(asObject.isDisplayChi(), asObject.getTypeLine(eachASLineCounter+1)));
					regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
					
					sb.append("<TR>");
					sb.append("<TD>" + regStr + "</TD>");
					sb.append("</TR>");
					
					regStr = getRegistorString(getTypeDisplay(asObject.isDisplayChi(), asObject.getTypeLine(eachASLineCounter+1)));
					regStr = lang.Equals(LANG_SCH) ? (regStr) : regStr;
					
					sb.append("<TR>");
					sb.append("<TD>" + regStr + "</TD>");
					sb.append("</TR>");
				}
			
				//System.out.println(start+" -----------------------------------------");
				//System.out.println(register);
				//System.out.println("HTML -----------------------------------------");
				//System.out.println(sb);
				//System.out.println("--------------------------------------------");
			}
			
			
		}
		return sb.toString();
		
	}
	
	**/
	
	private static string getASRegistorString(String item){
		if(item == null){
			return "#";
		}
		if(item.Equals("")){
			return "#";
		}else {
			item= item.Replace("<BR> ", "\n");
			return item.Replace("<BR>", "\n");
			
		}
			
	}
	
	protected static string getRegistorString(String item){
		if(item == null){
			return BLANK;
		}
		if(item.Equals("")){
			return BLANK;
		}else {
			item= item.Replace("<BR> ", "\n");
			return  item.Replace("<BR>", "\n");
		}
			
	}
	
	private static string getRegistorStarString(String name , string star ){
		String resultString;
		resultString =  getRegistorString(name);
		if(!stringUtil.isBlank(star)){
			resultString = resultString+ "*";
		}
	    return resultString;
	}
	
	private static void setText(HSSFCell cell, HSSFCellStyle style, string value){
		cell.SetCellValue(value);
        cell.CellStyle = style;
    }
	
	/**

	private static void setText(HSSFCell cell, string value){
		cell.setCellValue(value);
		HSSFCellStyle cs = cell.getCellStyle();
		cs.WrapText = true;
		cell.setCellStyle(cs);
	}
	
	private static void setTextNoWrap(HSSFCell cell, string value){
		cell.setCellValue(value);
		HSSFCellStyle cs = cell.getCellStyle();
		cs.setWrapText(false);
		cell.setCellStyle(cs);
	}
	
	**/
	
}
