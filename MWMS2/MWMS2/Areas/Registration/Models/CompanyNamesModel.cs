using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{

    public class CompanyNamesModel : DisplayGrid
    {
        public string C_COMP_APPLICATION_UUID { get; set; }
        public string FILE_REFERENCE_NO { get; set; }
        public string CHINESE_COMPANY_NAME { get; set; }
        public string ENGLISH_COMPANY_NAME { get; set; }
    }
}