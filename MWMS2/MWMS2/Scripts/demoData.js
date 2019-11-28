

var Menu_PORTAL = {code:"PORTAL", text:"PORTAL", head: true, child:[
	{code:"PRT01", text: "CRM", icon:"edit"}
	, {code:"PRT02", text: "SMM", icon:"edit"}
	, {code:"PRT03", text: "PEM", icon:"edit"}
]};

var Menu_COMMON_FUNCTION = {code:"COMMON_FUNCTION", text:"COMMON_FUNCTION", head: true, child:[
	{code:"CMN01", text: "Common Search", icon:"search"}
	, {code:"CMN02", text: "User Setting", icon:"user"}
	, {code:"CMN03", text: "Logout", icon:"sign-out-alt"}
]};

//register serach = 4x application
var Menu_CRM = {code:"CRM", text: "CRM", head: true, child:[
    {
        code: "CRM01", text: "Search                                                       ", child: [
           /* { link: "/Registration/Fn01Search_GCA/Index", text: "Search General Contractor Application" }
            , { link: "/Registration/Fn01Search_PA/Index", text: "Search Professional Application" }
            , { link: "/Registration/Fn01Search_CNV/Index", text: "Search Conviction" }
            ,*/

		//  {code:"CRM0100", text: "Register Search          "}
		 {code:"CRM0101", text: "Search General Contractor Application                        "}
		, {code:"CRM0102", text: "Search Professional Application                              "}
		///, {code:"CRM0103", text: "Search Conviction                                    "}
		///, {code:"CRM0103", text: "Search Company Conviction                                    "}
		, {code:"CRM0104", text: "Search  Conviction                                 "}
		, {code:"CRM0105", text: "Search Deferral                           "}
		//, {code:"CRM0105", text: "Search General Contractor Deferral                           "}
		//, {code:"CRM0106", text: "Search Professional Deferral                                 "}
		, {code:"CRM0107", text: "Search MW Company Application                                "}
		, {code:"CRM0108", text: "Search MW Individual Application                             "}
		//, {code:"CRM0109", text: "Search MW Company Deferral                                   "}
		//, {code:"CRM0110", text: "Search MW Individual Deferral                                "}
		, {code:"CRM0111", text: "Search Registered Inspector Application                      "}
		, {code:"CRM0112", text: "Search Qualified Person                                      "}
		, {code:"CRM0113", text: "Cases In Hand Report                                         "}
		, {code:"CRM0114", text: "Class, Type, Item Count Report                               "}
		, {code:"CRM0115", text: "Registered Company Report                                    "}
		, {code:"CRM0116", text: "Registered Person Report                                     "}
	]}
	, {code:"CRM02", text: "General Contractor Application                             ", child:[
		  {code:"CRM0201", text: "Company Application                                      "}
		//  {code:"CRM0201", text: "New Company Application                                      "}
		//, {code:"CRM0202", text: "Edit Company Application                                     "}
		//, {code:"CRM0203", text: "Conviction Records                                           "}
		, {code:"CRM0204", text: "Synchronize Address                                          "}
		, {code:"CRM0205", text: "Update Application Status                                    "}
		, {code:"CRM0206", text: "Process Monitor                                              "}
		, {code:"CRM0207", text: "Process Monitor on 10-Day Pledge                             "}
		, {code:"CRM0208", text: "Monitor on Fast-Track Processing of Form BA2A                "}
		, {code:"CRM0209", text: "Export Data                                                  "}
		, {code:"CRM0210", text: "Reports                                                      "}
		//, {code:"CRM0211", text: "Leave Form (PNRC 59)                                         "}
		, {code:"CRM0212", text: "Generate Candidate No                                        "}
		, {code:"CRM0213", text: "Meeting & Room Arrangement                                   "}
		, {code:"CRM0214", text: "Interview Candidates                                         "}
		, {code:"CRM0215", text: "Interview Result                                             "}
		, {code:"CRM0216", text: "Export Letter                                                "}
	]}
	, {code:"CRM03", text: "Professional Application                                   ", child:[
		  {code:"CRM0301", text: "Professional Application                                 "}
		 // {code:"CRM0301", text: "New Professional Application                                 "}
		//, {code:"CRM0302", text: "Edit Professional Application                                "}
		//, {code:"CRM0303", text: "Conviction Records                                           "}
		, {code:"CRM0304", text: "Synchronize Address                                          "}
		, {code:"CRM0305", text: "Update Application Status                                    "}
		, {code:"CRM0306", text: "Process Monitor                                              "}
		, {code:"CRM0307", text: "Export Data                                                  "}
		, {code:"CRM0308", text: "Reports                                                      "}
		//, {code:"CRM0309", text: "Leave Form (BA21)                                            "}
		, {code:"CRM0310", text: "Generate Candidate No                                        "}
		, {code:"CRM0311", text: "Meeting & Room Arrangement                                   "}
		, {code:"CRM0312", text: "Interview Candidates                                         "}
        , { code: "CRM0313", text: "Interview Result                                             " }
        , { code: "CRM0314", text: "Batch Upload                                            " }
	]}
	, {code:"CRM04", text: "MW Company Application                                     ", child:[
		  {code:"CRM0401", text: "MW Company Application                                   "}
		//  {code:"CRM0401", text: "New MW Company Application                                   "}
		//, {code:"CRM0402", text: "Edit MW Company Application                                  "}
		//, {code:"CRM0403", text: "Conviction Records                                           "}
		, {code:"CRM0404", text: "Synchronize Address                                          "}
		, {code:"CRM0405", text: "Update Application Status                                    "}
		, {code:"CRM0406", text: "Process Monitor                                              "}
		//, {code:"CRM0407", text: "Process Monitor on 10-Day Pledge                             "}
		//, {code:"CRM0408", text: "Monitor on Fast-Track Processing of Form BA25A               "}
		, {code:"CRM0409", text: "Export Data                                                  "}
		, {code:"CRM0410", text: "Reports                                                      "}
		//, {code:"CRM0411", text: "Leave Form                                                   "}
		, {code:"CRM0412", text: "Generate Candidate No                                        "}
		, {code:"CRM0413", text: "Meeting & Room Arrangement                                   "}
		, {code:"CRM0414", text: "Interview Candidates                                         "}
		, {code:"CRM0415", text: "Interview Result                                             "}
		, {code:"CRM0416", text: "Export Letter                                                "}
	]}
	, {code:"CRM05", text: "MW Individual Application                                  ", child:[
		  {code:"CRM0501", text: "MW Individual Application                                "}
		//  {code:"CRM0501", text: "New MW Individual Application                                "}
		//, {code:"CRM0502", text: "Edit MW Individual Application                               "}
		//, {code:"CRM0503", text: "Conviction Records                                           "}
		, {code:"CRM0504", text: "Synchronize Address                                          "}
		, {code:"CRM0505", text: "Update Application Status                                    "}
		, {code:"CRM0506", text: "Process Monitor                                              "}
		, {code:"CRM0507", text: "Export Data                                                  "}
		, {code:"CRM0508", text: "Reports                                                      "}
		//, {code:"CRM0509", text: "Leave Form                                                   "}
		, {code:"CRM0510", text: "Generate Candidate No                                        "}
		, {code:"CRM0511", text: "Meeting & Room Arrangement                                   "}
		, {code:"CRM0512", text: "Interview Candidates                                         "}
		, {code:"CRM0513", text: "Interview Result                                             "}
	]}
	, {code:"CRM06", text: "Committee                                      ", child:[
		  {code:"CRM0601", text: "Members Particular                                           "}
		, {code:"CRM0602", text: "Committee Panels                                             "}
		, {code:"CRM0603", text: "Committees                                                   "}
		, {code:"CRM0604", text: "Committee Groups                                             "}
	]}
	

		//, {code:"CRM0503", text: "Conviction Records                                           "}
	///, {code:"CRM07", text: "List Management                                            ", child:[
	///	  {code:"CRM0701", text: "Applicant Role                                               "}
	///	, {code:"CRM0702", text: "Applicant Status                                             "}
	///	, {code:"CRM0703", text: "Application Form                                             "}
	///	, {code:"CRM0704", text: "Authority Name                                               "}
	///	, {code:"CRM0705", text: "Buildings Safety & Code                                      "}
	///	, {code:"CRM0706", text: "Buildings Safety & Code Item                                 "}
	///	, {code:"CRM0707", text: "Company Type                                                 "}
	///	, {code:"CRM0708", text: "Panel Type                                                   "}
	///	, {code:"CRM0709", text: "Committee Type                                               "}
	///	, {code:"CRM0710", text: "Conviction Source                                            "}
	///	, {code:"CRM0711", text: "Category Code                                                "}
	///	, {code:"CRM0712", text: "Category Group                                               "}
	///	, {code:"CRM0713", text: "HTML Notes                                                   "}
	///	, {code:"CRM0714", text: "Interview Result                                             "}
	///	, {code:"CRM0715", text: "Panel Role                                                   "}
	///	, {code:"CRM0716", text: "Committee Role                                               "}
	///	, {code:"CRM0717", text: "Non-building Works Related                                   "}
	///	, {code:"CRM0718", text: "Period of Validity                                           "}
	///	, {code:"CRM0719", text: "Practice Notes                                               "}
	///	, {code:"CRM0720", text: "Professional Registration Board                              "}
	///	, {code:"CRM0721", text: "Room Details                                                 "}
	///	, {code:"CRM0722", text: "Society Name                                                 "}
	///	, {code:"CRM0723", text: "Title                                                        "}
	///	, {code:"CRM0724", text: "Public Holiday Management                                    "}
	///	, {code:"CRM0725", text: "Minor Works Class                                            "}
	///	, {code:"CRM0726", text: "Minor Works Type                                             "}
	///	, {code:"CRM0727", text: "Minor Works Item                                             "}
	///	, {code:"CRM0728", text: "Vetting Officer                                              "}
	///	, {code:"CRM0729", text: "Secretary                                                    "}
	///]}
	, {code:"CRM1001", text: "Conviction                                             "/*, child:[
		  {code:"CRM0801", text: "Conviction                                                 "}
    ]*/
    }

    //, {
    //    code: "CRM09", text: "Quick Search                                             ", child: [
        
    //    ]
    //}
   

]};

var Menu_SMM = {
    code: "SMM", text: "SMM", head: true, child: [
        {
               code: "SMM01", text: "SCU Registry                                                ", child: [
                  { code: "SMM0101", text:   "Receipt from R&D                                           " }
                , { code: "SMM0102", text: "Receive Application                                          " }
                , { code: "SMM0103", text: "Scan Document                                                " }
                , { code: "SMM0104", text: "Data Entry                                                   " }
                , { code: "SMM0105", text: "Print Barcode Sticker                                        " }
                , { code: "SMM0106", text: "Mail Merge                                                   " }    
            ]}
        , {
               code: "SMM02", text: "To Do List                                                         ", child: [
                  { code: "SMM0201", text: "To Do List                                                    " }
                , { code: "SMM0202", text: "Job Assignment                                                " }
                , { code: "SMM0203", text: "Subordinate Task Search                                       " }
                , { code: "SMM0204", text: "Validation Search                                             " }
                , { code: "SMM0205", text: "Signboard Search                                             " }
            ]}
        , {
            code: "SMM03", text: "Search                                              ", child: [
                  { code: "SMM0301", text: "Validation Application                                        " }
                , { code: "SMM0302", text: "Audit Application                                             " }             
            ]}
     //  , {
     //      code: "SMM04", text: "G.C.                                                            ", child: [
     //          { code: "SMM0401", text: "Gov. Contractor Action Record                             " }            
     //      ]}
     //  , {
     //      code: "SMM05", text: "Order/DSRN                                                      ", child: [
     //          { code: "SMM0501", text: "Order/ DSRN Management                                          " }
     //      ]}
     //  , {
     //      code: "SMM06", text: "Report to SCU                                                   ", child: [
     //          { code: "SMM0601", text: "Report to SCU                                                   " }
     //      ]}
     //  , {
     //      code: "SMM07", text: "Enquiry To SCU                                                  ", child: [
     //          { code: "SMM0701", text: "Enquiry To SCU                                                  " }
     //      ]}
        , {
            code: "SMM08", text: "Reports                                                         ", child: [
                  { code: "SMM0801", text: "Monthly Records                                               " }
                , { code: "SMM0802", text: "Monthly Statistics                                            " }
                , { code: "SMM0803", text: "Submission Progress Report                                    " }
                , { code: "SMM0804", text: "PBP and PRC Job List                                          " }
                , { code: "SMM0805", text: "Un-process Submission Report                                  " }
                , { code: "SMM0806", text: "Government Contractor Report                                  " }
                , { code: "SMM0807", text: "Subordinate Task                                              " }
                , { code: "SMM0808", text: "S24 Order Report                                              " }
                , { code: "SMM0809", text: "Data Export                                                   " }              
            ]}
       // , {
       //      code: "SMM09", text: "R & D                                                      ", child: [
       //           { code: "SMM0901", text: "Document to SCU                                               " }
       //         , { code: "SMM0902", text: "Proceed Delivery                                              " }
       //         , { code: "SMM0903", text: "Outstanding Document                                          " }
       //     ]}
        //, {
        //    code: "SMM10", text: "Maintanence                                                     ", child: [
        //        { code:   "SMM1001", text: "New Item                                                      " }
        //        , { code: "SMM1002", text: "Bulk Update                                                   " }
        //        , { code: "SMM1003", text: "RFID                                                          " }
        //        , { code: "SMM1004", text: "Transfer to RRM                                               " }
        //     ]}
      /// , {
      ///     code: "SMM11", text: "Admin                                                      ", child: [
      ///           { code: "SMM1101", text: "User Management                                              " }
      ///         , { code: "SMM1102", text: "Role Management                                              " }
      ///         , { code: "SMM1103", text: "System Parameter Management                                  " }
      ///         , { code: "SMM1104", text: "SCU Team Management                                          " }
      ///         , { code: "SMM1105", text: "Update Signboard Application                                 " }
      ///         , { code: "SMM1106", text: "Update Job Assignment                                        " }
      ///         , { code: "SMM1107", text: "Validation Option                                            " }
      ///         , { code: "SMM1108", text: "Reason for Refuse                                            " }
      ///         , { code: "SMM1109", text: "Types of Signboard                                           " }
      ///     ]
      /// }
    ]
};


var Menu_PEM = {
    code: "PEM", text: "PEM", head: true, child: [
        {
            code: "PEM01", text: "Letter Module                                               ", child: [
                  { code: "PEM0101", text: "Ack"                                                            }
                , { code: "PEM0102", text: "Statistics                                                    " }
                , { code: "PEM0103", text: "Pre-commencement Site Audit List                             " }
                , { code: "PEM0104", text: "Search                                                        " }
             //  , { code: "PEM0105", text: "DW letter                                                     " }
                , { code: "PEM0106", text: "AL                                                            " }
                , { code: "PEM0107", text: "MW list                                                       " }
                , { code: "PEM0108", text: "Signboard                                                     " }
                , { code: "PEM0109", text: "Order                                                         " }
                , { code: "PEM0110", text: "Manually Pick Audit                                           " }
                , { code: "PEM0111", text: "Other                                                         " }
                , { code: "PEM0112", text: "Ack Submission                                                " }
                , { code: "PEM0113", text: "Audit List Management                                  " }
                , { code: "PEM0114", text: "Direct Return Submission                                 " }
                , { code: "PEM0115", text: "Modification                                 " }
            ]
        }
        , {
            code: "PEM02", text: "MWU Registry                                                         ", child: [
                  { code: "PEM0201", text: "Receipt                                                       " }
                , { code: "PEM0202", text: "Scan and Dispatch                                             " }
                , { code: "PEM0203", text: "Scan and Assign Document                                      " }
                , { code: "PEM0204", text: "Dispatch                                                      " }
                , { code: "PEM0205", text: "General Entry                                                 " }
               // , { code: "PEM0206", text: "First Entry                                                   " }
             //   , { code: "PEM0207", text: "Second Entry                                                  " }
              //  , { code: "PEM0208", text: "Outgoing and Incoming Record                                  " }
                , { code: "PEM0209", text: "Incoming                                                      " }
                , { code: "PEM0210", text: "Outgoing                                                      " }
                , { code: "PEM0211", text: "Document Compiling                                            " }
                , { code: "PEM0212", text: "Print Existing Barcode                                        " }
                , { code: "PEM0213", text: "MWU Receive Counter                                           " }
                , { code: "PEM0214", text: "DSN Mapping                                                   " }
            ]
        }
        , {
            code: "PEM03", text: "Task                                                            ", child: [
                  { code: "PEM0301", text: "To Do List                                                    " }
                , { code: "PEM0302", text: "Subordinates Task Search                                      " }
                , { code: "PEM0303", text: "Submission Search                                             " }
                , { code: "PEM0304", text: "Address Search                                                " }
                , { code: "PEM0305", text: "General Search                                                " }

            ]
        }
        , {
            code: "PEM0401", text: "Verification                                                        "
        }
        //, {
        //    code: "PEM04", text: "Verification                                                        ", child: [
        //        { code: "PEM0401", text: "Form Checking                                                  " }
        //    ]
        //}
        , {
            code: "PEM0501", text: "Ackn - Checklist Summary                                                     "/*, child: [
                { code: "PEM0501", text: "                                             " }
            ]*/
        }
        , {
            code: "PEM06", text: "Audit                                                   ", child: [
                  { code: "PEM0601", text: "Audit Selection Case                                         " }
                , { code: "PEM0602", text: "Audit Control                                                " }
            ]
        }
        , {
            code: "PEM0701", text: "ICC                                                 "
        }
        , {
            code: "PEM0801", text: "Doc.Sorting                                                         "
        }
        /*, {
            code: "PEM08", text: "Doc.Sorting                                                         ", child: [
                { code: "PEM0801", text: "Doc.Sorting                                          " }
            ]
        }*/
        , {
            code: "PEM09", text: "Reports                                                   ", child: [
                { code: "PEM0901", text: "Submission Records                                                " }
                , { code: "PEM0902", text: "Types of Submission Record                                      " }
                , { code: "PEM0903", text: "AFC Progress Report                                             " }
                , { code: "PEM0904", text: "PBP and PRC Job List                                            " }
                , { code: "PEM0905", text: "Enq. and Complaint Progress report                           " }
                , { code: "PEM0906", text: "Document Time Record Sheet                                      " }
                , { code: "PEM0907", text: "Quasi Signboard Registration                                    " }
                , { code: "PEM0908", text: "Outstanding Document List                                       " }
                , { code: "PEM0909", text: "Submission Location Report                                      " }
                , { code: "PEM0910", text: "Error Report                                                    " }
                , { code: "PEM0911", text: "MW Submissions w/o Cert. of Completion report         " }
                , { code: "PEM0912", text: "eSubmission Report                                              " }
                , { code: "PEM0913", text: "eSubmission Summary Report                                      " }
            ]
        }
        , {
            code: "PEM10", text: "R&D                                                     ", child: [
                //{ code: "PEM1001", text: "R&D                                  " },
                { code: "PEM1002", text: "Document to MWU" },
                { code: "PEM1003", text: "Proceed Delivery" },
                //{ code: "PEM1004", text: "Outstanding Document" },
                //{ code: "PEM1005", text: "Back to main Page" },
                //{ code: "PEM1006", text: "Document From MWU" },
                //{ code: "PEM1007", text: "Outstanding Document" },

                //{ code: "PEM1008", text: "Back to main Page" },
            ]
        }
       //// , {
       ////     code: "PEM11", text: "Admin                                                      ", child: [
       ////         { code: "PEM1101", text: "User Management                                       " }
       ////            , { code: "PEM1102", text: "Role Management                                    " }
       ////            , { code: "PEM1103", text: "System Parameter Management                                      " }
       ////            , { code: "PEM1104", text: "Letter Template Management                                    " }
       ////            , { code: "PEM1105", text: "Organization Escalation Chart                                       " }
       ////            , { code: "PEM1106", text: "Handling Outstanding Documentser Management                                       " }
       ////            , { code: "PEM1107", text: "Process Status                                 " }
       ////            , { code: "PEM1108", text: "Import MW05(3.6) Record                                     " }
       ////            , { code: "PEM1109", text: "Edit DSN Nature                                    " }
       ////            , { code: "PEM1110", text: "Import Records                                     " }
       ////            , { code: "PEM1111", text: "Upload PDF History                                    " }
       ////            , { code: "PEM1112", text: "Pending Transfer                                     " }
       ////            , { code: "PEM1113", text: "Update Minor Work Record                                  " }
       ////            , { code: "PEM1114", text: "MW Number Mapping                               " }
       ////            , { code: "PEM1115", text: "Update Submission Record                             " }
       ////     ]
       //// }
        , {
            code: "PEM1201", text: "Elementary Checking                                                            "
        }
    ]
};





var All_MENU = {};
function loadMenuData(menu, parent) {
    
	if(menu == null){
	}if(menu.code == null) {
		//alert("Error, Menu item need to set code attribute");
		return "ERROR";
	} else if(All_MENU[menu.code] != null) {
		//alert("Error, Duplicate menu code : " + menu.code);
		return "ERROR";
	} else {
		All_MENU[menu.code] = menu;
		if(parent != null) menu.parent = parent;
		if(menu.child != null) {
			for(var i = 0; i < menu.child.length; i++) {
				var errorMessage = loadMenuData(menu.child[i], menu);
				if(errorMessage == "ERROR") return errorMessage;
			}
		}
    }
}

function getRootMenu(code) {
	var v = All_MENU[code];
	if(v == null) return null;
	if(v.parent == null) return v;
	else return getRootMenu(v.parent.code);
}



/*
var current_menu = Menu_PEM;
var urlParas = getUrlParameters();
if(urlParas['pageCode'] == null) urlParas['pageCode'] = 'PRT01';
if(current_menu == null) current_menu = Menu_CRM;
if (urlParas['pageCode'] == 'PRT01' || urlParas['pageCode'].includes('CRM')) { current_menu = Menu_CRM; }
else if (urlParas['pageCode'] == 'PRT02' || urlParas['pageCode'].includes('SMM')) { current_menu = Menu_SMM; }
else if (urlParas['pageCode'] == 'PRT03' || urlParas['pageCode'].includes('PEM')) {
    current_menu = Menu_PEM;

}*/

//loadMenuData(Menu_PORTAL);
//loadMenuData(Menu_COMMON_FUNCTION);
loadMenuData(Menu_CRM);
loadMenuData(Menu_PEM);
loadMenuData(Menu_SMM);

//#region exportExcelFunction
function exportExcel(tableName) {

    var tab_text = "<table border='2px'><tr bgcolor='#87AFC6'>";
    var textRange; var j = 0;
    tab = document.getElementById(tableName); // id of table

    for (j = 0; j < tab.rows.length; j++) {
        tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";
        //tab_text=tab_text+"</tr>";
    }

    tab_text = tab_text + "</table>";
    tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, "");//remove if u want links in your table
    tab_text = tab_text.replace(/<img[^>]*>/gi, ""); // remove if u want images in your table
    tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, ""); // reomves input params

    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");

    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))      // If Internet Explorer
    {
        txtArea1.document.open("txt/html", "replace");
        txtArea1.document.write(tab_text);
        txtArea1.document.close();
        txtArea1.focus();
        sa = txtArea1.document.execCommand("SaveAs", true, "111.xls");
    }
    else                 //other browser not tested on IE 11
        sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text));

    return (sa);
}

//#endregion

function loadContentPage(code){
	window.location = '?pageCode=' + code;
	return;
    //if (selectedPage == null) return;      
    $.ajax({
        method: "GET"
        , url: code + ".htm"
        , success: function (rs) {
            attr("content", "html", rs);
            $('.resultTable').DataTable({
                "paging": true
                , "ordering": false
                , "info": false
                , "searching": false
                , "bLengthChange": false
            });
			/*var h3s = $("#content h3");
			if(h3s.length > 0 ){
				h3s.get(0).ondblclick = function(){
					$('#sidebar').toggleClass('active');
					if($("#sidebar" ).hasClass( "active" )) {
						//document.getElementById("topMenuContainer").style.display="none";
						document.getElementById("sidebar").style.display="none";
					} else {
						//document.getElementById("topMenuContainer").style.display="";
						document.getElementById("sidebar").style.display="";
					}
				}
			};*/


            loadMainMenu(leftMenuItem);
        }
        , error: function (ajaxContext) {
            //var msg = "";
            //var idxMenu = selectedPage;
            //while(true) {
            //	msg = idxMenu.text + (msg == "" ? msg : "->" + msg);
            //	if(idxMenu.parent != null) idxMenu = idxMenu.parent; else break;
            //}
            attr("content", "html", "<b>cannot load file : " + code + ".htm   </b>");


        }
    });




  /////   
  /////   
  /////   
  /////    alert(page);
  /////    selectedPage.code = page;
  /////   
  /////   //
  /////   //}
  /////   
  /////    $.ajax({
  /////        method: "GET"
  /////        , url: selectedPage.code + ".htm"
  /////        , success: function (rs) {
  /////            attr("content", "html", rs);
  /////            $('.resultTable').DataTable({
  /////                "paging": true
  /////                , "ordering": false
  /////                , "info": false
  /////                , "searching": false
  /////                , "bLengthChange": false
  /////            });
  /////            var h3s = $("#content h3");
  /////            if (h3s.length > 0) {
  /////                h3s.get(0).ondblclick = function () {
  /////                    $('#sidebar').toggleClass('active');
  /////                    if ($("#sidebar").hasClass("active")) {
  /////                        //document.getElementById("topMenuContainer").style.display="none";
  /////                        document.getElementById("sidebar").style.display = "none";
  /////                    } else {
  /////                        //document.getElementById("topMenuContainer").style.display="";
  /////                        document.getElementById("sidebar").style.display = "";
  /////                    }
  /////                }
  /////            };
  /////        }
  /////        , error: function (ajaxContext) {
  /////            var msg = "";
  /////            var idxMenu = selectedPage;
  /////            while (true) {
  /////                msg = idxMenu.text + (msg == "" ? msg : "->" + msg);
  /////                if (idxMenu.parent != null) idxMenu = idxMenu.parent; else break;
  /////            }
  /////            attr("content", "html", "<b>cannot load file : " + selectedPage.code + ".htm, page : (" + msg + ")  </b>");
  /////   
  /////   
  /////        }
  /////    });
  /////   
  /////   
  /////    //$.ajax({
  /////    //    method: "GET"
  /////    //    , url: selectedPage.code + ".htm"
  /////    //    , success: function (rs) {
  /////    //        attr("content", "html", rs);
  /////    //        $('.resultTable').DataTable();
  /////    //    }
  /////    //    , error: function (ajaxContext) {
  /////    //        var msg = "";
  /////    //        var idxMenu = selectedPage;
  /////    //        while (true) {
  /////    //            msg = idxMenu.text + (msg == "" ? msg : "->" + msg);
  /////    //            if (idxMenu.parent != null) idxMenu = idxMenu.parent; else break;
  /////    //        }
  /////    //        attr("content", "html", "<b>cannot load file : " + selectedPage.code + ".htm, page : (" + msg + ")  </b>");
  /////   
  /////   
  /////    //    }
  /////    //});
  /////   
  /////   
  /////   
  /////   
  /////   
}

//#endregion


function ToDoListTab2(table) {
    if (table == "AL") {
        $('#WL').parents('div.dataTables_wrapper').first().hide();
        $('#AL').parents('div.dataTables_wrapper').first().show();
    }
    else {
        $('#WL').parents('div.dataTables_wrapper').first().show();
        $('#AL').parents('div.dataTables_wrapper').first().hide();
    }



    if (!$('#' + table).hasClass("hide")) return;
    $('#' + table).removeClass("hide");
    dataTableize($('#' + table));
    window.scrollTo(0, document.body.scrollHeight);
}

function  ToDoListTab(evt, tabName) {
    var i, x, tablinks;
    x = document.getElementsByClassName("tab");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablink");
    for (i = 0; i < x.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" w3-border-red", "");
    }
    document.getElementById(tabName).style.display = "block";
    evt.currentTarget.firstElementChild.className += " w3-border-red";

  
}



//#region PEM0101
function PEM0114Edit()
{
    document.getElementById('DSN').value = "D00000001";
    document.getElementById('FT').value = "xxxx";
    document.getElementById('name').value = "xx";
   
    document.getElementById('date').value = " 11/8/2018";    
    document.getElementById('HS1').value = "SO2";
    document.getElementById('HS2').value = "BS1";
}
var DSNNumber = 2;
function PEM0114Save()
{

    $('#PEM0114Table tr:last').after('<tr> <td>D00000' + DSNNumber+'</td>        <td>xx</td>        <td>xx            </td>        <td>            MWC xxx/xxx                    </td>        <td>            11/8/2018            </td>        <td>            a,b,k            </td>        <td>            SO2            </td>        <td>            BS1          </td>  <td><input type="button" value="Edit" onclick="PEM0114Edit()" /></td></tr>');
    DSNNumber++;
}

function PEM0115MainSearch()
{
    $('#PEM0115Table').removeClass('hide');
}
function PEM0115Assign()
{
    document.getElementById('HS').value = "BS1";
}
function PEM0115Edit() {
    loadContentPage('PEM0115Main');
}
function PEM0115Remove()
{
    $("#PEm0115Mapping1").on('click', '.btnRemove', function () {
        $(this).closest('tr').remove();
    });

}
function PEM0115MappingAdd()
{
    $('#PEm0115Mapping1 tr:first').after('<tr>            <td>MW Submission No.</td><td>MW0000000' + DSNNumber +'</td><td>  <input type="button" value="remove"class="btnRemove" onclick="PEM0115Remove()"  /></td></tr>');
    DSNNumber++;
}
function PEM0115MappingSearch()
{
    $('#PEm0115Mapping1').removeClass('hide');
    $('#PEm0115Mapping2').removeClass('hide');
}
function PEM0115Submit() {

    $('#PEM0115Table tr:last').after('<tr> <td>MW xxx/2018(Mod) </td>               <td>            11/8/2018            </td>            <td>            BS1          </td>  <td><input type="button" value="Edit" onclick="PEM0115Edit()" /></td></tr>');
    DSNNumber++;
}
function PEM0114Assign()
{
    document.getElementById('HS1').value = "SO2";
    document.getElementById('HS2').value = "BS1";
}
function onSearchByDate() {
    var dsn = document.getElementById('mwAckLetterVo.base.dsn').value
    document.getElementById('mwAckLetterVo.base.mwNo.label').textContent = "MW0000001";
    document.getElementById('mwAckLetterVo.base.mwNo').value = "MW0000001";

    document.getElementById('mwAckLetterVo.base.formNo.label').textContent = "MW01";

    if (dsn == "1") {

        var element = document.getElementById("additionTr1");
        element.classList.remove("hide");
        var element = document.getElementById("additionTr2");
        element.classList.remove("hide");
        var element = document.getElementById("additionTr3");
        element.classList.remove("hide");
    }
    else  {

        var element = document.getElementById('ackTableColumnName');
        element.classList.remove('hide');


        var element = document.getElementById('ackTableTbody');
        element.classList.remove('hide');



    }

    var element = document.getElementById("PEMHideDiv");
    element.classList.remove("hide");

}

function onSave()
{//$('#PEM0101Table').DataTable().row.add([
    $('#PEMHideDiv').DataTable().row.add([

        "<input type='button' value = 'Edit' onclick=" + "loadContentPage('PEM0104a')>",
        'D0000748875',
        'MW181100002',
        'MW01 ',
        '06/11/2018 ',
        '06/11/2018',
        '',
        '',
        '',
        'N',
        'N',
        'N',
        'N',
        "<input type='button' class='inputButton' onclick=" + "window.open('/img/PEM0101.pdf')" + " value = 'print'>",
        
    ]).draw(false);



}

//#endregion

//#region PEM0103

var PEM103TableCount = 1;
function searchSiteAudit() {

    $('#PEM103AuditList').DataTable().row.add([
        '<a href=javascript:' + 'loadContentPage("PEM0103a")' + '>' + 'D000000000' + PEM103TableCount++ + '</a>',
        'MW160603649',
        '22/06/2016 ',
        'G/F, IND..',
        '22/06/2016',
        '22/06/2016',
        'TOMW4',
        'Not yet Inspected ',
        ''
    ]).draw(false);



   
}

function PEM0103a() {
    confirm("Are you sure to remove from Pre-comm Audit List?");
  
    loadContentPage('PEM0103');

}


//#endregion

//#region PEM104
function PEM0104a() {
    confirm("Are you sure to delete this DSN and its Data?");

    loadContentPage('PEM0104');

}
function PEM0107a() {
    confirm("Are you sure to delete this DSN and its Data?");

    loadContentPage('PEM0107');

}
function PEM0108a() {
    confirm("Are you sure to delete this DSN and its Data?");

    loadContentPage('PEM0108');

}
function PEM0109a() {
    confirm("Are you sure to delete this DSN and its Data?");

    loadContentPage('PEM0109');


}


function PEM0112a() {
    confirm("Are you sure to delete this DSN and its Data?");

    loadContentPage('PEM0112');

}
function PEM_Search(DivName) {

    document.getElementById(DivName).style.display = "block";
}
function PEM_ResetSerach(DivName) {


    document.getElementById(DivName).style.display = "none";
}

//#endregion


//#region PEM105

function PEM0105a()
{

    confirm('Are you sure to remove this DSN from the DW Letter List ? ');
    loadContentPage('PEM0105');

}

function searchDWLetter() {




       
       
       
        var table = document.getElementById('PEM105DWLetterList');

        //alert(table.rows.length);
   // table.deleteRow(1);
       for (var i = 1; i < 5; i++) {
       
                   table.deleteRow(i);
       
               
           }
        }
    







//#endregion PEM105




//#region PEM106

function PEM0106a() {

    confirm('Are you sure to remove this DSN from the Advisory Letter List?  ');
    loadContentPage('PEM0106');

}

function onSearch() {


        document.getElementById('searchPo').value = "LEE LI";
        document.getElementById('searchPoContact').value = "3162 3115";

        var table = document.getElementById('PEM106ALetterList');

      
                    table.deleteRow(1);

                }
    




function addToList() {


    var ToPost = document.getElementById('searchPoPost').value;

    var table = document.getElementById("PEM106ALetterList");
   

    var rowCount = table.rows.length;
    var row = table.insertRow(rowCount);
    row.className = "sRow2";

    //if (rowCount != 1) { table.deleteRow(1); }

    var td1 = document.createElement("td"); //td1
    td1.className = "sCell";
    td1.innerHTML = ToPost;
    row.appendChild(td1);


    var td2 = document.createElement("td"); //td1
    td2.className = "sCell";
    td2.innerHTML = "<a href>D0000740185</a>";
    row.appendChild(td2);



    var td3 = document.createElement("td"); //td1
    td3.className = "sCell";
    td3.innerHTML = "22/06/2016 ";
    row.appendChild(td3);

    var td4 = document.createElement("td"); //td1
    td4.className = "sCell";
    td4.innerHTML = "22/06/2016 ";
    row.appendChild(td4);

    var td5 = document.createElement("td"); //td1
    td5.className = "sCell";
    td5.innerHTML = "MW161100001";
    row.appendChild(td5);

    var td6 = document.createElement("td"); //td1
    td6.className = "sCell";
    td6.innerHTML = "MW01";
    row.appendChild(td6);

    var td7 = document.createElement("td"); //td1
    td7.className = "sCell";
    td7.innerHTML = "";
    row.appendChild(td7);

    var td8 = document.createElement("td"); //td1
    td8.className = "sCell";
    td8.innerHTML = "";
    row.appendChild(td8);

    var td9 = document.createElement("td"); //td1
    td9.className = "sCell";
    td9.innerHTML = "";
    row.appendChild(td9);







}
//#endregion 

//#region PEM107
function PEM107search() {

    var displayMwNo = document.getElementById('displayMwNo').value;


    var table = document.getElementById('PEM107Table');

    //alert(table.rows.length);

    for (var i = 0; i < 4; i++) {
        for (var r = 2; r < table.rows.length; r++) {

            // alert(table.rows[r].cells[0].innerHTML);
            if (table.rows[r].cells[0].innerHTML == displayMwNo) {
                //  alert('nodel');


            }
            else {
                // alert('del');
                //alert('*' + table.rows[r].cells[8].innerHTML + '*');
                table.deleteRow(r);

            }
            //for (var c = 0, m = table.rows[r].cells.length; c < m; c++) {
            // alert(table.rows[r].cells[8].innerHTML);
            //}
        }

    }




}
//#endregion 


//#region PEM108
function PEM108search() {

    var displayMwNo = document.getElementById('displayMwNo').value;


    var table = document.getElementById('PEM108Table');

    //alert(table.rows.length);

    for (var i = 0; i < 3; i++) {
        for (var r = 1; r < table.rows.length; r++) {

            //  alert(table.rows[r].cells[0].innerHTML);
            if (table.rows[r].cells[0].innerHTML == displayMwNo) {
                //  alert('nodel');


            }
            else {
                // alert('del');
                //alert('*' + table.rows[r].cells[8].innerHTML + '*');
                table.deleteRow(r);

            }
            //for (var c = 0, m = table.rows[r].cells.length; c < m; c++) {
            // alert(table.rows[r].cells[8].innerHTML);
            //}
        }

    }




}
//#endregion 

//#region PEM109
function PEM109search() {

    var displayMwNo = document.getElementById('displayMwNo').value;


    var table = document.getElementById('PEM109Table');

    //alert(table.rows.length);

    for (var i = 0; i < 4; i++) {
        for (var r = 1; r < table.rows.length; r++) {

            //  alert(table.rows[r].cells[0].innerHTML);
            if (table.rows[r].cells[0].innerHTML == displayMwNo) {
                //  alert('nodel');


            }
            else {
                // alert('del');
                //alert('*' + table.rows[r].cells[8].innerHTML + '*');
                table.deleteRow(r);

            }
            //for (var c = 0, m = table.rows[r].cells.length; c < m; c++) {
            // alert(table.rows[r].cells[8].innerHTML);
            //}
        }

    }




}


function SearchPEM1101() {
    var table = document.getElementById('PEM1101Table'); if (table.rows[1].cells[1].innerHTML == 'Tony Kwok&nbsp;') { table.deleteRow(1); }
}

//#endregion 
//#region PEM0112 
var PEM0112TableCount = 1;
function searchACKSubmission() {
    $('#PEM0112Table').DataTable().row.add([
        '<a href=javascript:' + 'loadContentPage("PEM0112a")' + '>' + 'Edit' + '</a>',
        'D000000000' + PEM0112TableCount++,
        ' MW181000011' + PEM0112TableCount++,
        'MW01',
        '23/10/2018',
        '23/10/2018',
        '',
        '',
        '',
        'Y',
        'N',
        'N',
        'N',
        '<input type="checkbox"></input>',

    ]).draw(false);



}
//#endregion 
//#region PEM202
var PEM202TableCount = 1;
function scanAndDispatch() {

    var newDsn = document.getElementById('newDsn').value;



	
	
        $('#PEM202Table').DataTable().row.add( [
            '<a href=javascript:' + 'loadContentPage("PEM0202a")'   +'>'+'D000000000' + PEM202TableCount++ +'</a>',
            '22/11/2013',
            '11:33',
            'Enq101200008',
            'New'
        ] ).draw( false );
	
	




}


function PEM0202aTable() {
    
    $('#PEM0202aTable').DataTable().row.add([
        'D000000000' + PEM202TableCount++,
        'Letter&nbsp;',
        '1&nbsp;',
        '31/12/2010&nbsp;',
        '<img style="cursor: pointer;" src="images/ico_view.gif" onclick="openFile(' + 'D0000000001' + ')">',
        '<input class="inputButton" type="button" id="" name="" value="Delete" style="width:120px;" onclick="deleteDsn(' + 'D0000000001' + ')">',
    ]).draw(false);
   
}
     //#endregion 
    
    //#region PEM203
    
    
    var PEM203TableCount = 1;
function PEM203search() {

        $('#PEM0203Table').DataTable().row.add( [
            '<a href=javascript:' + 'loadContentPage("PEM0203a")' + '>' + 'D000000000' + PEM203TableCount++ + '</a>',
            '22/11/2013',
            '11:33',
            'Enq101200008','MW03',
            'New'
        ] ).draw( false );
	
	
	


}
//#endregion 


//#region PEM205
//function search(tableName, item) {

//    var T = '#' + tableName;

//    $(T).DataTable().row.add([
//        '<a href=javascript:' + 'loadContentPage("PEM0205a")' + '>' + 'D000000000' + PEM203TableCount++ + '</a>',
//        '22/11/2013',
//        '11:33',
//        '',
//        '	Enquiry',
//        '	Scanned',
       
//    ]).draw(false);



//}
//#endregion 
//#region PEM206
function searchPEM0206() {

    $('#PEM0206Table').DataTable().row.add([
        '<a href=javascript:' + 'loadContentPage("PEM0205a")' + '>' + 'D000000000' + PEM203TableCount++ + '</a>',
        '22/11/2013',
        '11:33',
        '',
        '	Enquiry',
        '	Scanned',

    ]).draw(false);


}
//#endregion 

//#region PEM206
function searchPEM0207() {


    $('#PEM0207Table').DataTable().row.add([
        '<a href=javascript:' + 'loadContentPage("PEM0205a")' + '>' + 'D000000000' + PEM203TableCount++ + '</a>',
        '22/11/2013',
        '11:33',
        '',
        '	Enquiry',
        '	Scanned',

    ]).draw(false);


}
//#endregion 


//#region PEM206
function searchPEM0208() {


    $('#PEM0208Table').DataTable().row.add([
        '22/10/2013',
        '00:00',
        'D000000000' + PEM203TableCount++ ,
        'MW120200551 ',
        '',
        '',
       'Sent',
      

    ]).draw(false);


}
//#endregion 

//#region PEM209
function generateIncomingBarcodeByType() {

    var element = document.getElementById("PEMHideDiv");
    element.classList.remove("hide");

    $('#PEMHideDivPEM0210').css("display", "none");
    $('#PEMHideDiv').css("display", "");
}




//endregion




//#region PEM213
function searchPEM0213() {


    $('#PEM0213Table').DataTable().row.add([
        '22/10/2013',
        '00:00',
        'D000000000' + PEM203TableCount++,
        'MW120200551 ',
        'MW01',
        
        "<button onclick=" + "loadContentPage('PEM0101')" + ">Ack. Letter!</button>",

    ]).draw(false);
    PEM203TableCount++;
    $('#PEM0213Table').DataTable().row.add([
        '22/10/2013',
        '00:00',
        'D000000000' + PEM203TableCount++,
        'MW 001/2018(MOD)',
        'MW01',

        "<button onclick=" + "loadContentPage('PEM0115Receive')" + ">Modification!</button>",

    ]).draw(false);

}
//#endregion 

//#region PEM213
function searchPEM0214() {


    $('#PEM0214Table').DataTable().row.add([
        'D000000000' + PEM203TableCount++,
        '22/10/2013',
        '00:00',
        'Delivered',
           '<a href="">Mapping</a> ' ,



    ]).draw(false);


}
//#endregion 

function searchPEM0301b() {


    $('#PEM0301b1').css("display", "");


}

function searchPEM0301b2() {

    $('#PEM0301b2').css("display", "");

}

function searchPEM0301d() {


    $('#PEM0301d1').css("display", "");


}

function searchPEM0301d2() {

    $('#PEM0301d2').css("display", "");

}
function searchPEM0303() {


    $('#PEM0303Table').DataTable().row.add([
        '<a href=javascript:' + 'loadContentPage("PEM0304a")' + '>' + 'MW00000000' + PEM103TableCount++ + '</a>',
        '2.6',
        '1/10/2017',
        '',
        '',



    ]).draw(false);


}


function searchPEM0304() {


    $('#PEM0304Table').DataTable().row.add([
        '<a href=javascript:' + 'loadContentPage("PEM0304a")' + '>' + 'MW00000000' + PEM103TableCount++ + '</a>',
        'Completed ',
        '30/6/2016',
        '24/5/2016 ',
        '3/6/2016 ',
        '',



    ]).draw(false);

    PEMRemoveHideSearch();
} 



function searchPEM0305() {


    $('#PEM0305Table').DataTable().row.add([
        '1-268169162  ',
        '<a href=javascript:' + 'loadContentPage("PEM0305a")' + '>' + 'Enq10120000' + PEM103TableCount++ + '</a>',
        '31/12/2010  ',
        '30/1/2011  ',
        ' 	-2829  ',
        '3/1/2011  ',
        ' 	-2849',
        '',
        'Telephone',
        'Endorsed ',



    ]).draw(false);

 
}


function searchPEM0401() {


    $('#PEM0401Table').DataTable().row.add([
       
        '<a href=javascript:' + 'loadContentPage("PEM0301c")' + '>' + 'VS170700000' + PEM103TableCount++ + '</a>',
        'MW06-01',
        ' 	 1, 3.45 ',
        '  18/07/2017 ',
        'Open',
        ' ',
    


    ]).draw(false);


}



function searchPEM0501() {


    $('#PEM0501Table').DataTable().row.add([

        '<a href=javascript:' + 'loadContentPage("PEM0501a")' + '>' + 'MW18090000' + PEM103TableCount++ + '</a>',
        'MW01 ',
        ' 	 	20/09/2018  ',
        ' Open',



    ]).draw(false);


}


function searchPEM0601() {


    $('#PEM0601Table').DataTable().row.add([

        '<a href=javascript:' + 'loadContentPage("PEM0601a")' + '>' + 'MW170300000' + PEM103TableCount++ + '</a>',
        'MW01 ',
        '3.27 ',
        'In Progress ',
        ' 	  	7/7/2016  ',
        ' 	  	12/7/2016  ',
        ' ',



    ]).draw(false);


}
//#endregion 
function searchReport()
{
    $("#PEM0601aTable").css("display", "none");
    $("#PEM0601aTable2").css("display", "");

    var element = document.getElementById("PEM0601aTable2");
    element.classList.remove("hide");
}


function addCase()
{

    $("#PEM0601aTable3").css("display", "none");


}
function PEM1001ChangeMenu(menuIndex)
{
    if (menuIndex == '1') { window.location = "/index.htm?pageCode=PEM1002"; }
    else { window.location = "/index.htm?pageCode=PEM1006";}
    

}
function PEM0301ChangeLan()
{
    var radios = document.getElementsByName('mwAckLetterVo.base.language');

    for (var i = 0, length = radios.length; i < length; i++) {
        if (radios[i].checked) {
            // do whatever you want with the checked radio

            if (radios[i].value != "ENG") {

                var div = document.getElementById('Eng');
                div.style.display = "none";
                var div2 = document.getElementById('Chn');
                div2.style.display = "";
            }
            else {
                var div = document.getElementById('Eng');
                div.style.display = "";
                var div2 = document.getElementById('Chn');
                div2.style.display = "none"; }
           
            // only one radio can be logically checked, don't check the rest
            break;
        }
    }
}
var IrregularitiesNumber = 2;
function PEM0114AddIrregularities()
{
    //var select = " <td colspan='3'> <select style='max-width:600px;'>"
    //    + "  <option> a. 在完工證明書上沒有填寫正確的小型工程呈交編號；</option>"
    //    + "<option> b. 沒有填寫展開工程的日期；</option>                    "
    //    + "<option>c. 沒有填寫完成工程的日期；</option>                     "
    //    + "<option> d. 沒有填寫簽署的日期；</option>                        "
    //    + "<option> e. 沒有使用適當的指明表格；</option>                    "
    //    + "<option>  f. 安排進行小型工程的人/訂明建築專業人士/訂明註冊承建商在呈交的指明表格上的簽署不是正本/並未簽署；   </option>"
    //    + "<option>g.  沒有填寫有效的小型工程項目編號；</option>"
    //    + "  <option>h. 沒有呈交所需的訂明圖則及詳圖/照片/圖則或工程描述；</option>"
    //    + "   <option>       i. 呈交的文件的規格及方式不當，例如散裝照片；           </option>"
    //    + "    <option>                j. 沒有填寫由他人代為豎設招牌的人士的詳情；</option>"
    //    + "      <option> k. 沒有填妥訂明建築專業人士/訂明註冊承建商的資料，例如英文/中文名稱、註冊證明書編號、註冊屆滿日期等；  </option>"
    //    + "     <option>             l. 根據本署記錄，訂明建築專業人士/訂明註冊承建商的有關註冊已經失效/訂明註冊承建商並未就該小型工程項目註冊；      </option>"
    //    + "   <option>       m. 訂明建築專業人士/訂明註冊承建商在呈交的指明表格上的簽署與本署記錄不吻合；   </option>"
    //    + "    <option>      n. 其他:         請盡快改正並重新一併呈交。   </option>  </select>   <input type='button' onclick='PEM0114AddIrregularities()' value='Add' />";


     







    $('#PEM0114Table tr:last').after('<tr><th>Irregularities ' + IrregularitiesNumber+ ' :</th>' + select+'</tr>');
    IrregularitiesNumber++;
}

function onDownload() {
    var radios = document.getElementsByName('mwAckLetterVo.base.language');
    
    for (var i = 0, length = radios.length; i < length; i++) {
        if (radios[i].checked) {
            // do whatever you want with the checked radio

            if (radios[i].value != "CHT") {
            
                document.getElementById('my_iframe').src = 'img/PEM0301DREng.doc';

            }
            else {
                         document.open('img/PEM0301DRChi.doc');
            }

            // only one radio can be logically checked, don't check the rest
            break;
        }
    }}

function PEMRemoveHideSearch() {

    var element = document.getElementById("PEMHideDiv");
    element.classList.remove("hide");
    var element = document.getElementById("PEMHideDiv2");
    element.classList.remove("hide");
    var element = document.getElementById("PEMHideDiv3");
    element.classList.remove("hide");
    var element = document.getElementById("PEMHideDiv4");
    element.classList.remove("hide");
    var element = document.getElementById("PEMHideDiv5");
    element.classList.remove("hide");
    var element = document.getElementById("PEMHideDiv6");
    element.classList.remove("hide");
  
}

function assignNewDsn()
{
    $('#PEM1002Table').DataTable().row.add([' 12 / 11 / 2018', '15:38', 'D0000748938']).draw(false);
 //   $('#PEM1002Table tr:last').after('<tr class="sRow1">  <td > 12 / 11 / 2018</td >          <td class="sCell">15:38&nbsp;</td>           <td class="sCell">D0000748938&nbsp;</td>              </tr >');
}


function DelHideClass() {

    var element = document.getElementById("PEMHideDiv");
    element.classList.remove("hide");

    var element = document.getElementById("PEMHideDiv2");
    element.classList.remove("hide");
    var element = document.getElementById("PEMHideDiv3");
    element.classList.remove("hide");
    var element = document.getElementById("PEMHideDiv4");
    element.classList.remove("hide");
    var element = document.getElementById("PEMHideDiv5");
    element.classList.remove("hide");
}



function confirmSubmit(tableName) {
    var table = document.getElementById(tableName);
    //alert(table.rows[2].cells[5].innerHTML.);
    //table.rows[2].cells[5].innerHTML = "Confirmed";
    /* declare an checkbox array */
    var chkArray = [];

    /* look for all checkboes that have a class 'chk' attached to it and check if it was checked */
    $(".inputCheckbox:checked").each(function () {
        //alert($(this).val());
        var selected = $(this).val();
        selected++;
        table.rows[selected].cells[5].innerHTML = "Confirmed";
    });


}

function viewRecordDetail() { alert('No Record Detail.');}


function PEM1109Search()
{
    $("#PEM1109Table").css("display", "none");
    $("#PEM1109Table2").css("display", "");

}

function removeAction() {
    $('.inputCheckbox:checkbox:checked').parents("tr").remove();

}

function onImportFile()
{
    $('#PEM1108Row').css("display", "");

    $('#PEM1110Row').css("display", "");

}

function onClickDelete()
{
    confirm("Confirm to delete file : 'template.xls'?");
    $('#PEMresultTalbe tbody').on("click", "td", function () {
        $(this).closest("tr").remove();
    });

}

function onChangeFile()
{


    $('#PEM1110Row').css("display", "");
}
function transfer()
{
    $('.inputCheckbox:checkbox:checked').closest("td").prev('td').text("4(1)/5 ");
    
}
function getMwRecordDetail()
{


}
function addRevised1Item()
{
    $('#PEM0301cEditBody tr:last').after($('<tr><td><input type=text/></td><td><input type=text/></td><td><input type=text/></td></tr>'));
  

}
function addItem() {
   
    $('tr#PEM1201Row td:first').append($('<br/><input type="text" style="width: 80px;" id="hidden_field" name="hidden_field" value=""/> <input type="text" style="width: 80px;" id="hidden_field" name="hidden_field" value=""/> <input type="text" style="width: 80px;" id="hidden_field" name="hidden_field" value=""/> <input type="text" style="width: 80px;" id="hidden_field" name="hidden_field" value=""/> <input type="text" style="width: 80px;" id="hidden_field" name="hidden_field" value=""/>'));
  // alert("123");PEMHideDiv
    $('tr#PEM1201row td:first').append($('<br/><input type="text" style="width: 80px;" id="hidden_field" name="hidden_field" value=""/><input type="text" style="width: 80px;" id="hidden_field" name="hidden_field" value=""/><input type="text" style="width: 80px;" id="hidden_field" name="hidden_field" value=""/><input type="text" style="width: 80px;" id="hidden_field" name="hidden_field" value=""/><input type="text" style="width: 80px;" id="hidden_field" name="hidden_field" value=""/>'));
   //$(this).closest("td").prev('td').text(
   //    '<input type="text" style="width: 80px;>',
   //    '<input type="text" style="width: 80px;>',
   //    '<input type="text" style="width: 80px;>',
   //    '<input type="text" style="width: 80px;>',
   //
   //    '<input type="text" style="width: 80px;>',
   //
   //);

    $('#itemTableBody tr:last').after('   <tr class="sRow2" id="itemTr0"><td class="sCell"><input type="button" value="Delete" class="inputButton hideSpan" id="deleteItemBtn0" name="deleteItemBtn"></td><td class="sCell"><select name="dataEntryObject.svItemResult[0].mwItemCode" style="width: 100px;"><option value="">-</option><option value="1.20">1.20</option><option value="1.21">1.21</option><option value="1.22">1.22</option><option value="1.23">1.23</option><option value="1.24">1.24</option><option value="2.18">2.18</option><option value="2.19">2.19</option><option value="2.20">2.20</option><option value="2.21">2.21</option><option value="2.22">2.22</option><option value="2.23">2.23</option><option value="2.24">2.24</option><option value="2.25">2.25</option><option value="2.26">2.26</option><option value="2.27">2.27</option><option value="3.16">3.16</option><option value="3.17">3.17</option><option value="3.18">3.18</option><option value="3.19">3.19</option><option value="3.20">3.20</option><option value="3.21">3.21</option><option value="3.22">3.22</option></select></td><td class="sCell"><textarea class="inputTextarea" rows="3" name="dataEntryObject.svItemResult[0].locationDescription" style="width: 300px;"></textarea></td><td class="sCell"><textarea class="inputTextarea" rows="3" name="dataEntryObject.svItemResult[0].revision" style="width: 300px;"></textarea></td></tr>');
  
}


function copySignboardAddressToOwner() {

    document.getElementById('dataEntryObject.signboard.owner.svAddress.buildingname').value = '兆輝大廈';
    document.getElementById('dataEntryObject.signboard.owner.svAddress.street').value = "石排灣道";

}

function copySignboardAddressToOI()
{

    document.getElementById('dataEntryObject.oi.svAddress.buildingname').value = '兆輝大廈';
    document.getElementById('dataEntryObject.oi.svAddress.street').value =          "石排灣道";


}
function copySignboardAddressToPAW() 
{
    document.getElementById('dataEntryObject.paw.svAddress.buildingname').value = '兆輝大廈';
    document.getElementById('dataEntryObject.paw.svAddress.street').value="石排灣道";



}

function onClickImport()
{
    confirm("Confirm to import file : 'template.xls'?");
    var table = $('#PEMresultTalbe').DataTable();
 
    $('#PEMresultTalbe tbody').on("click", "td", function () {
        $(this).closest("td").empty();
    });
}

function searchPEM1111() {
    var element = document.getElementById("PEM1111Table");
    element.classList.remove("hide");

	//if(domId("PEM1111Table").style.display == "") return ;
   // $("#PEM1111Table").css("display", "");
	dataTableize("#PEM1111Table");
    $('#PEM1111Table').DataTable().row.add([

        '29/08/2018 11:33:16',
        'admin ',
        ' 	1 / 0',
        "<button onclick="+"exportExcel('PEM1111Table')"+">Details</button>",
       

    ]).draw(false);


}
function PEM1112search() {

    var element = document.getElementById("PEM1112Table");
    element.classList.remove("hide");

}
function PEM1114Search() {
	//if(domId("PEM1114Table").style.display == "") return ;
 //   $("#PEM1114Table").css("display", "");
	//dataTableize("#PEM1114Table");


    var element = document.getElementById("PEM1114Table");
    element.classList.remove("hide");
}



function PEM1001Search() {

    loadContentPage('PEM1002')
}

//function print() { window.print(); }
function printData() { window.print(); }
function printReport() { window.print();}
function maximizeItem() { $('#PEM0301cTable').css("display", ""); }


function saveFolderType() { alert('save successfully');  }

function save() { loadContentPage('PEM1103'); }
function cancel() { loadContentPage('PEM1103'); }


function onAdd() {
    $('#PEM1103iRow').css("display", "");
    $('#PEM1103iRow1').css("display", "");
    $('#PEM1103iRow2').css("display", "");
    $('#PEM1103iRow3').css("display", "");
}



function onCancelAdd() {
    $('#PEM1103iRow').css("display",  "none");
    $('#PEM1103iRow1').css("display", "none");
    $('#PEM1103iRow2').css("display", "none");
    $('#PEM1103iRow3').css("display", "none");
}



function onDelete()
{
    confirm('Are you sure continue to delete? ');
    for (var i = 0; i < 4; i++) {
        $('#PEM1103iFirstRow').remove();
    }

}
//-----------------CRM-----------------
//#region CRM






//#region CRM01
//function popupWind() { alert('no image find'); }
/*
function searchForm() {

   //var element = document.getElementById("dgList");
   //element.classList.remove("hide");


    var element = document.getElementById("CRMHideDiv");
    element.classList.remove("hide");
  
    $('#CRMHideDiv').css("display", "");
    $('#CRMHideDiv').css("visibility", "");
}*/
/*
function searchForm() {
    history.pushState(null, null, "?doSearch=true");
	if(!$('#CRMHideDiv').hasClass("hide")) return;
    $('#CRMHideDiv').removeClass("hide");
    dataTableize($('#CRMHideDiv'));
   window.scrollTo(0,document.body.scrollHeight);
}
*/





//#endregion


//#region CRM0113

function printCaseInHandReport() {
    var win = window.open("/CRM0113a.htm", '_blank');
    win.focus();
}


//#endregion

//#region CRM0114

function printNumberOfRegistrationReport() {
    var win = window.open("/CRM0114a.htm", '_blank');
    win.focus();
}


//#endregion


//#region CRM0201 companyInfo companyApplicant

function setTab(divID) {

    if (divID == 'memberInfo') {
        $('#memberInfo').css("display", "");
        $('#committeeType').css("display", "none");
        $('#institutes').css("display", "none");
    }
    else if (divID == 'committeeType') {
        $('#memberInfo').css("display", "none");
        $('#committeeType').css("display", "");
        
        $('#CRM0601Table').removeClass('hide');
        $('#CRM0601Table2').removeClass('hide');
        $('#institutes').css("display", "none");
    }
    else if (divID == 'institutes') {
        $('#memberInfo').css("display", "none");
        $('#committeeType').css("display", "none");
        $('#institutes').css("display", ""); }


    if (divID == "companyInfo") {
 
        document.getElementById('td2').className = "untab";
        document.getElementById('td1').className = "tab";
        document.getElementById('td1').style.backgroundColor = "yellow";
        document.getElementById('td2').style.backgroundColor = "#AAA"; if (document.getElementById('td3') != null) {
            document.getElementById('td3').style.backgroundColor = "#AAA";
            document.getElementById('td4').style.backgroundColor = "#AAA";
        }

        $('#compInfo').css("display", "");
        $('#companyApplicant').css("display", "");



        $('#CRM0402aResultTable').removeClass('hide');
        $('#companyApplicant').css("display", "none");

        $('#compInfo').css("display", "");

        $('#compStatus').css("display", "none");
        $('#compBS').css("display", "none");

    }
    else
        if (divID == "companyApplicant") {
    
         
                document.getElementById('td2').className = "tab";
                document.getElementById('td1').className = "untab";
                document.getElementById('td1').style.backgroundColor = "#AAA";
            document.getElementById('td2').style.backgroundColor = "yellow";
            if (document.getElementById('td3') != null) {
                document.getElementById('td3').style.backgroundColor = "#AAA";
                document.getElementById('td4').style.backgroundColor = "#AAA";
            }
            $('#CRM0402aResultComTable').removeClass('hide');   
            $('#companyApplicant').css("display", "");
            $('#CRM0201aResultTable').removeClass('hide');


            $('#compInfo').css("display", "none");
          
            $('#compAppls').css("display", "");

            $('#compStatus').css("display", "none");
            $('#compBS').css("display", "none");
            if (!$('#CRM0202aApplsTable').hasClass("hide")) return;
            $('#CRM0202aApplsTable').removeClass("hide");
            

        }
        else if (divID == "companyStatus") {
        
            document.getElementById('td1').style.backgroundColor = "#AAA";
            document.getElementById('td2').style.backgroundColor = "#AAA";
            document.getElementById('td3').style.backgroundColor = "yellow";
            document.getElementById('td4').style.backgroundColor = "#AAA";
          
            $('#compInfo').css("display", "none");

            $('#compAppls').css("display", "none");

            $('#compStatus').css("display", "");
            $('#compBS').css("display", "none");
        }
        else if (divID == "companyBuildingSafty") {
         
            document.getElementById('td1').style.backgroundColor = "#AAA";
            document.getElementById('td2').style.backgroundColor = "#AAA";
            document.getElementById('td3').style.backgroundColor = "#AAA";
            document.getElementById('td4').style.backgroundColor = "yellow";

            $('#compInfo').css("display", "none");

            $('#compAppls').css("display", "none");

            $('#compStatus').css("display", "none");
            $('#compBS').css("display", "");
            if (!$('#CRM0202aTable').hasClass("hide")) return;
            $('#CRM0202aTable').removeClass("hide");
        }

    if (divID == 'individualMwItem')
    {
        document.getElementById('td1').style.backgroundColor = "#AAA";
        document.getElementById('td2').style.backgroundColor = "yellow";
        document.getElementById('td3').style.backgroundColor = "#AAA";
        document.getElementById('td4').style.backgroundColor = "#AAA";

        $('#CRM0502ResultTable').removeClass('hide');
        $('#indInfo').css("display", "none");
        $('#indMW').css("display", "");
        $('#uclmwITEM_pnlGrid').css("display", "");
        $('#indBS').css("display", "none");
        $('#indQual').css("display", "none");
    }
    if (divID == 'individualBuildingSafty')
    {
        document.getElementById('td1').style.backgroundColor = "#AAA";
        document.getElementById('td2').style.backgroundColor = "#AAA";
        document.getElementById('td3').style.backgroundColor = "#AAA";
        document.getElementById('td4').style.backgroundColor = "#yellow";
        $('#indQual').css("display", "none");
      
        $('#indInfo').css("display", "none");
        $('#indCert').css("display", "none");
        $('#indMW').css("display", "none");
        $('#indBS').css("display", "");
    }
    

    if (divID == 'individualQuali') {
        document.getElementById('td1').style.backgroundColor = "#AAA";
        document.getElementById('td2').style.backgroundColor = "#AAA";
        document.getElementById('td3').style.backgroundColor = "yellow";
        document.getElementById('td4').style.backgroundColor = "#AAA";
        $('#indQual').css("display", "");
        $('#CRM0302QuTable').removeClass("hide");
        $('#indInfo').css("display", "none");
        $('#indCert').css("display", "none");
        $('#indMW').css("display", "none");
        $('#indBS').css("display", "none"); }
    if (divID == 'professionalQuali' ) {
        document.getElementById('td1').className = "untab";
        document.getElementById('td2').className = "tab";
        document.getElementById('td1').style.backgroundColor = "#AAA";
        document.getElementById('td2').style.backgroundColor = "yellow";
        document.getElementById('td3').style.backgroundColor = "#AAA";
        document.getElementById('td4').style.backgroundColor = "#AAA";
        $('#indQual').css("display", "");
        $('#CRM0302QuTable').removeClass("hide");
        $('#indInfo').css("display", "none");
        $('#indCert').css("display", "none");
        $('#indMW').css("display", "none");
        $('#indBS').css("display", "none");
    }
    else if (divID == 'professionalInfo' || divID == "individualInfo") {
        document.getElementById('td1').className = "tab";
        document.getElementById('td2').className = "untab";
        document.getElementById('td1').style.backgroundColor = "yellow";
        document.getElementById('td2').style.backgroundColor = "#AAA";
        document.getElementById('td3').style.backgroundColor = "#AAA";
        document.getElementById('td4').style.backgroundColor = "#AAA";
        $('#indQual').css("display", "none");
        $('#indInfo').css("display", "");
        $('#indCert').css("display", "none");
    } else if (divID == 'professionalCert') {

        document.getElementById('td1').style.backgroundColor = "#AAA";
        document.getElementById('td2').style.backgroundColor = "#AAA";
        document.getElementById('td3').style.backgroundColor = "yellow";
        document.getElementById('td4').style.backgroundColor = "#AAA";
       
        $('#CRM0302CertTable').removeClass("hide");
        $('#indQual').css("display", "none");
        $('#indInfo').css("display", "none");
        $('#indCert').css("display", "");
        $('#indBS').css("display", "none");
    } else if (divID == 'professionalBuildingSafty')
    {
        document.getElementById('td1').style.backgroundColor = "#AAA";
        document.getElementById('td2').style.backgroundColor = "#AAA";
        document.getElementById('td3').style.backgroundColor = "#AAA";
        document.getElementById('td4').style.backgroundColor = "yellow";
        $('#indQual').css("display", "none");
        $('#indInfo').css("display", "none");
        $('#indCert').css("display", "none");
        
        $('#indBS').css("display", "");
        $('#CRM0302aResultTable').removeClass('hide');
    }
  //else if (divID == 'individualMwItem')
  //{
  //   
  //    $('#indInfo').css("display", "");
  //    $('#indMW').css("display", "");
  //    $('#uclmwITEM_pnlGrid').css("display", "");
  //   
  //}
       
}
function saveQualification() {
    $('#uclQual_pnlGrid').css("display", "");
    $('#uclQual_pnlForm').css("display", "none"); }
function qualificationFrom()
{
    $('#uclQual_pnlGrid').css("display", "none");
    $('#uclQual_pnlForm').css("display", "");
    
}
function itemInput()
{
   
    $('#uclmwITEM_pnlGrid').css("display", "none");
    $('#uclmwITEM_pnlGrid2').css("display", "");
}
function saveMWItem()
{
    $('#uclmwITEM_pnlGrid2').css("display", "none");
    $('#uclmwITEM_pnlGrid').css("display", "");
}
function certificateFrom()
{
    $('#uclQual_dgQual').removeClass('hide');
    $('#uclCert_pnlGrid').css("display", "none");
    $('#uclCert_pnlEdit').css("display", "");
}
function saveCertificate()
{
    $('#uclCert_pnlGrid').css("display", "");
    $('#uclCert_pnlEdit').css("display", "none");
}

function saveApplicant() {
    
    $('#uclApplicants_pnlGrid').css("display", ""); 
    $('#uclApplicants_pnlEdit').css("display", "none"); 
    $('#uclApplicants_pnlEditMWItem').css("display", "none"); 
    
    
}

function searchCompany() {
    $('#compMsg').css("display", "");
}

function qualificationFrom() {
    $('#uclQual_pnlForm').css("display", "");
  
    $('#uclQual_pnlGrid').css("display", "none");
}

function saveQualification() {
    $('#uclQual_pnlForm').css("display", "none");

    $('#uclQual_pnlGrid').css("display", "");
    $('#uclApplicants_pnlEditMWItem').css("display", "none");
    
}

function applicantFrom()
{
    
    $('#uclApplicants_pnlGrid').css("display", "none");

    $('#uclApplicants_pnlEdit').css("display", "");

    if (!$('#CRM0401bTable').hasClass("hide")) return;
    $('#CRM0401bTable').removeClass("hide");
  

    if (!$('#CRM0401bTable2').hasClass("hide")) return;
    $('#CRM0401bTable2').removeClass("hide");


 
}
function classInput()
{
    $('#uclApplicants_pnlEditMWItem').css("display", "");
    $('#uclApplicants_pnlEdit').css("display", "none");

}
//#endregion
function getCompanyByBr()
{
  
    $('#englishCompanyName').val("UNION 'LUEN HOP' REFRIGERATION CO LTD");



    $('#chineseCompanyName').val("聯合冷氣工程有限公司");
}

function getApplicantById()
{

    alert("HKID or Passport No. does not exists.");
}

function createForm() {





    selectedPage.code = 'CRM07new';

    $.ajax({
        method: "GET"
        , url: selectedPage.code + ".htm"
        , success: function (rs) {
            attr("content", "html", rs);
			dataTableize('.resultTable');
           
            var h3s = $("#content h3");
            if (h3s.length > 0) {
                h3s.get(0).ondblclick = function () {
                    $('#sidebar').toggleClass('active');
                    if ($("#sidebar").hasClass("active")) {
                        //document.getElementById("topMenuContainer").style.display="none";
                        document.getElementById("sidebar").style.display = "none";
                    } else {
                        //document.getElementById("topMenuContainer").style.display="";
                        document.getElementById("sidebar").style.display = "";
                    }
                }
            };
        }
        , error: function (ajaxContext) {
            var msg = "";
            var idxMenu = selectedPage;
            while (true) {
                msg = idxMenu.text + (msg == "" ? msg : "->" + msg);
                if (idxMenu.parent != null) idxMenu = idxMenu.parent; else break;
            }
            attr("content", "html", "<b>cannot load file : " + selectedPage.code + ".htm, page : (" + msg + ")  </b>");


        }
    });

    





}


function createForm2() {





    selectedPage.code = 'CRM07new2';

    $.ajax({
        method: "GET"
        , url: selectedPage.code + ".htm"
        , success: function (rs) {
            attr("content", "html", rs);
            $('.resultTable').DataTable({
                "paging": true
                , "ordering": false
                , "info": false
                , "searching": false
                , "bLengthChange": false
            });
            var h3s = $("#content h3");
            if (h3s.length > 0) {
                h3s.get(0).ondblclick = function () {
                    $('#sidebar').toggleClass('active');
                    if ($("#sidebar").hasClass("active")) {
                        //document.getElementById("topMenuContainer").style.display="none";
                        document.getElementById("sidebar").style.display = "none";
                    } else {
                        //document.getElementById("topMenuContainer").style.display="";
                        document.getElementById("sidebar").style.display = "";
                    }
                }
            };
        }
        , error: function (ajaxContext) {
            var msg = "";
            var idxMenu = selectedPage;
            while (true) {
                msg = idxMenu.text + (msg == "" ? msg : "->" + msg);
                if (idxMenu.parent != null) idxMenu = idxMenu.parent; else break;
            }
            attr("content", "html", "<b>cannot load file : " + selectedPage.code + ".htm, page : (" + msg + ")  </b>");


        }
    });







}
function showSearchForm(number)
{
    var sDivs = $("#searchContent div").not(".scrollTableDivLower").get();
    for (var i = 0; i < sDivs.length; i++) {
        if (i == number) {
            sDivs[i].style.display = "";
        } else {
            sDivs[i].style.display = "none";
        }
    }
}




//#region CRM0211
function NewLeave() {
    SetMode('L');


}
function NewCancel() {
    SetMode('C');
}

function cancelInput() {
    SetMode('');
}

function SetMode(mode) {
    if (!dateValid()) {
        return;
    }
    document.getElementById('leaveInformation').style.display = 'none';
    document.getElementById('CancelInformation').style.display = 'none';
    document.getElementById('buttonInformation').style.display = 'none';
    document.getElementById('mode').value = mode;

    if (mode == 'L') {
        document.getElementById('leaveInformation').style.display = '';
        document.getElementById('buttonInformation').style.display = '';

        document.getElementById('inputNature').value = 'L';
        document.getElementById('inputNature').checked = true;

        document.getElementById('inputStartDate').value = '';
        document.getElementById('inputEndDate').value = '';
        document.getElementById('upLoadLeaveImage').value = '';
        document.getElementById('inputRemarks').value = '';
    }
    if (mode == 'C') {
        document.getElementById('CancelInformation').style.display = '';
        document.getElementById('buttonInformation').style.display = '';
        document.getElementById('inputCancelStartDate').value = '';
        document.getElementById('inputCancelEndDate').value = '';
        document.getElementById('upLoadCancelLeaveImage').value = '';
        document.getElementById('inputCancelRemarks').value = '';
    }
}
function genSNBarcodeByNumber()
{
    $('#PEMHideDiv').css("display", "none");
    $('#PEMHideDivPEM0210').css("display", "");

}

//endregion

//#endregion



//#region SMM
function searchSMM() {

	if(!$('#SMMHideDiv').hasClass("hide")) return;
    $('#SMMHideDiv').removeClass("hide");
    dataTableize($('#SMMHideDiv'));
   window.scrollTo(0,document.body.scrollHeight);
}






function deleteScanDetail(a)
{
    var table = document.getElementById('SMM0103aTable');
    table.deleteRow(1); 
}



function copyFromBaseDataEntry()
{

    document.getElementById('dataEntryObject.signboard.locationOfSignboard').value = "香港仔石排灣道81號兆輝大廈2/F外牆";

    document.getElementById('dataEntryObject.signboard.svAddress.street').value = "石排灣道";
}


function addVaItem()
{
    $('#vaTableBody tr:last').after('<tr><td class="sCell"><input type="button" value="Delete" class="inputButton hideSpan" name="deleteVaItemBtn"></td><td class="sCell"><select name="dataEntryObject.selectedVaItemsR[0]"><option value="">-</option><option value="1(a)">1(a)</option><option value="1(b)">1(b)</option><option value="1(c)">1(c)</option><option value="2(a)">2(a)</option><option value="2(b)">2(b)</option><option value="2(c)">2(c)</option><option value="3">3</option><option value="4(a)">4(a)</option><option value="4(b)">4(b)</option><option value="5">5</option><option value="6">6</option></select></td><td class="sCell"><textarea class="inputTextarea" rows="3" name="dataEntryObject.selectedVaDescription[0]" style="width: 300px;"></textarea></td><td class="sCell"><span></span><input type="hidden" name="dataEntryObject.selectedVaCorrItemsR[0]" value=""><input type="hidden" name="dataEntryObject.selectedVaCorrItemsR2[0]" value=""></td></tr>');

    $('#SMM0104aTable tr:last').after('<tr><td class="sCell"><input type="button" value="Delete" class="inputButton hideSpan" name="deleteVaItemBtn"></td><td class="sCell"><select name="dataEntryObject.selectedVaItemsR[0]"><option value="">-</option><option value="1(a)">1(a)</option><option value="1(b)">1(b)</option><option value="1(c)">1(c)</option><option value="2(a)">2(a)</option><option value="2(b)">2(b)</option><option value="2(c)">2(c)</option><option value="3">3</option><option value="4(a)">4(a)</option><option value="4(b)">4(b)</option><option value="5">5</option><option value="6">6</option></select></td><td class="sCell"><textarea class="inputTextarea" rows="3" name="dataEntryObject.selectedVaDescription[0]" style="width: 300px;"></textarea></td><td class="sCell"><span></span><input type="hidden" name="dataEntryObject.selectedVaCorrItemsR[0]" value=""><input type="hidden" name="dataEntryObject.selectedVaCorrItemsR2[0]" value=""></td></tr>');
    
   
}


function deleteRecord()
{
    $('#SMM1108Row').parents("tr").remove();
}

function SMM0102() {

    var a = document.getElementById('fileRefNo');
    a.value = "SC18110003";

}
function create()
{
    $('#SMM0103aTable').DataTable().row.add([

        "<button" + ">Delete</button>",
        '    D000000888',
        '  30/01/2018 ',
            '',
            '',
            '',
            '',
            '',
            "<button" + ">set</button>",
        "<button" + ">Print</button>",


    ]).draw(false);

    $('#SMM0103aTable').DataTable().order([1, 'asc']).draw();
    $('#SMM0103aTable').DataTable().page('last').draw(false);
}
function SMM0104Search()
{
    document.getElementById('SMM0104Table').deleteRow(2);


}
function SMM0101YesNo(a) {
    if (a.value == "No") a.value = "Yes";
    else a.value = "No";


}
//function SMM0104YesNO()
//{
//    var elem = document.getElementById("OutStandingbtn");
//    if (elem.value == "No") elem.value = "Yes";
//    else elem.value = "No";

//}

function SMM0301YesNO() {
    var elem = document.getElementById("OutStandingbtn");
    if (elem.value == "NOT OK") elem.value = "OK";
    else elem.value = "NOT OK";

}
function SMM0104Assign()
{
    $('#SMM0104Row').text('BT11-0001/18');

}
//function showToCheckbox() {
//    var elem = document.getElementById("SMM0106YesNoBtn");
//    if (elem.value == "No") elem.value = "Yes";
//    else elem.value = "No";}
//#region SMM0901
var SMM901TableCount = 1;
function searchSMM0901() {
 

    $('#SMM0901Table').DataTable().row.add([
        '22/10/2013',
        '00:00',
        'SC18100013 ',
        'D000000000' + SMM901TableCount++,
        ' 	SC01  ',
     

    ]).draw(false);


}
function deleteSubmittedDoc()
{

    

    $('#SMM0301aTable tbody').on("click", "th", function () {
            $(this).closest("tr").remove();
        });

}

function setAsThumbnail()
{


  

}
function printNumber() {

    loadContentPage('SMM0103a');
}
 
function WNCreate() {

    loadContentPage('WNCREATE');
}
 
function genAuditForm()
{
    window.open('/img/SMM0301.docx');

}
//function toggleOkBtn(a)
//{
//    if (a.value == "Not OK") a.value = "OK";
//    else a.value = "Not OK";


//}
function confirmSubmitToDoSMM0301a() { confirm('Are you sure to save the document?'); loadContentPage('SMM0301a'); }
function confirmSubmitToDo() { confirm( 'Are you sure to save the document?'); loadContentPage('SMM0201'); }
//#endregion 

//endregion
function exportData()
{

    window.open('/img/Validation.xlsx');

}

//#region Warning Letter
$("#WNSearchBtn").click(function () {
    alert("123");
    var url = $("#WNSearchBtn").data("url");
    $.get(url, function (result) {
        //do something with the result if you ever need to
    });
});

function WNSearch()
{


    $.ajax({
        type: "POST",
        url: "/WN/WN", // the URL of the controller action method
        data: HTMLElement, // optional data
        success: function (result) {
          
            if (!$('#WNDiv').hasClass("hide")) return;
            $('#WNDiv').removeClass("hide");
            dataTableize($('#WNDiv'));
            window.scrollTo(0, document.body.scrollHeight);
            // do something with result
        },
        error: function (req, status, error) {
            // do something with error   
        }
    });

   

}
function CRM1001Search()
{
	if(!$('#CRM1001SearchDIV').hasClass("hide")) return;
    $('#CRM1001SearchDIV').removeClass("hide");
    dataTableize($('#CRM1001SearchDIV'));
   window.scrollTo(0,document.body.scrollHeight);

}

function WNCreate()
{loadContentPage('WNCREATE');
}
function goCRM1001a()
{loadContentPage('CRM1001a');
}

function cancel1001aForm()
{loadContentPage('CRM1001');
}

function save1001aForm()
{loadContentPage('CRM1001');
}


function WNCreateAtt(ele) {

dom_(ele.parentNode,'br');
dom_(ele.parentNode, 'input', [{type:'file', onclick: {
parameters: {}
, callback: function() {
	WNCreateAtt(ele);
}	
}}]);
}





function reload() 
{
	window.location.reload(false);
}
//#endregion

var currentMenu = Menu_PEM;











/*



function getRegSearchData() {
	var comEngName = [
	"1 CLASS MINOR WORKS LIMITED          "
	, "A SHING ENGINEERING COMPANY LIMITED  "
	, "A SQUARE LIMITED 	                 "
	, "AD DESIGN AND ENGINEERING LIMITED    "
	, "ADVANCE LINK ENGINEERING LIMITED     "];

	var comChiName = [
	"?��??��?穀?��??��??��?線�?緹�?踝蕭?��??��?踝蕭?�緬"
	,"?��??��??��??��??��?線�?緹�?踝蕭?��??��?踝蕭?�緬"
	,"穩�?罷�?緘�?踝蕭?��??�緹?��??��?踝蕭?��??��?�?
	,"?��??�畿?��??��??��?線�?緹�?踝蕭?��??��?踝蕭?�緬"
	,"?��??��?衝�??��??��?線�?緹�?踝蕭?��??��?踝蕭?�緬"];

	var asName = [
	"CHAN Tam Man"
	, "CHEUNG Siu Wai"
	, "LEE Man Ming"
	, "WONG Tat Yui"
	, "SIU Chi Keung"
	];


	var expDate = [
	"11-Oct-2020"
	"11-Sep-2021"
	"11-Nov-2022"
	"11-Dec-2021"
	"11-Mar-2020"
	];

	var regNo = [
	"MWC 525/2011"
	, "MWC 424/2000"
	, "MWC 485/2012'
	, "MWC 325/2013"
	, "MWC 477/2014"
	];
	var bs = [
	"Yes"
	, "No"
	, "No"
	, "Yes"
	, "No"
	];
	var tel = [
	"2565 5678"
	, "3165 5678"
	, "6565 4668"
	, "9565 5633"
	, "2565 6869"
	];
	var demoData= [];
	for(var i = 0; i < 5; i++) {
		demoData.put(
			{
				"Chinese Company Name                              ".trim()      : comChiName[i]
				, "English Company Name                            ".trim()    : comEngName[i]
				, "Name of Authorized Signatory                    ".trim()    : asName[i]
				
				, "Chinese Name                                    ".trim()    : ""
				, "English name                                    ".trim()    : ""
																	.trim()
				, "Expiry Date                                     ".trim()    : expDate[i]
				, "Registration No.                                ".trim()    : regNo[i]
				, "Services in Building Safety                     ".trim()    : bs[i]
				, "Telephone No. for Service in Building Safety    ".trim()    : tel[i]
			}
		);
	}
	return demoData;
}



*/







function showTable(tableId) {
if(!$('#' + tableId).hasClass("hide")) return;
$('#' + tableId).removeClass("hide");
dataTableize($('#' + tableId));
window.scrollTo(0,document.body.scrollHeight);
}

function PEM0110Search() {
	$(".buttonPICK").click(function() {
		this.parentNode.innerHTML = "Yes";
	});
}

function CMN01AdvanceSearch()
{
    var radios = document.getElementsByName('searchType');

    for (var i = 0, length = radios.length; i < length; i++) {
        if (radios[i].checked) {
            // do whatever you want with the checked radio

            if (radios[i].value == 2) {
                window.location = '?pageCode=PEM0303';
            }
            else {
                window.location = '?pageCode=CM01_MWSRegister_AdvanceSEARCH';
            }
            // only one radio can be logically checked, don't check the rest
            break;
        }
    }
 
}
function onClickTab(a)
{
    if (a == "SDM") {
        loadContentPage('PEM0102SDM');

    }
    else if (a == "Incoming") { loadContentPage('PEM0102'); }
    else { loadContentPage('PEM0102b');}
}

function archiveFrozenDataList()
{
    $('#frozenDataTable').css("display", "");

}
function changePEM1103dItem(id)
{
    var a = document.getElementById('mwItemVersionCode').value;
    //alert(a);
    var table = document.getElementById('PEM1103dTable');

    if (a == 2) {
        for (var i = 0; i < table.rows.length; i++)
            table.rows[i].cells[2].innerHTML = "";
    }
    else { loadContentPage('PEM1103d');}
}


