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

namespace MWMS2.Utility
{
    public class ImageUtil
    {
        // private String crmSignatureRootPath = "\\\\10.5.17.39\\crm_prod_image\\crm"; 
        private String crmSignatureRootPath = "\\\\192.168.88.200\\dump\\crm_prod_image\\crm"; 

        // Andy: CRM return absolute file path of Signature  
        public String retreiveAbsoluteSignatureFilePath(String filePathNonRestricted)
        {
            String fullPath = "";
            if (String.IsNullOrEmpty(filePathNonRestricted)) { return ""; }
            else
            {
                fullPath = crmSignatureRootPath + "\\" + filePathNonRestricted;
            }
            return fullPath;
        }
    }
}
