using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MWMS2Interface.Constant
{
    class ApplicationConstant
    {
   
        public static string BCIS_IP => ConfigurationManager.AppSettings["BCIS_IP"];
        public static string BCIS_ACCOUNT => ConfigurationManager.AppSettings["BCIS_ACCOUNT"];
        public static string BCIS_PW => ConfigurationManager.AppSettings["BCIS_PW"];



        public static string BRAVO_IP => ConfigurationManager.AppSettings["BRAVO_IP"];
        public static string BRAVO_ACCOUNT => ConfigurationManager.AppSettings["BRAVO_ACCOUNT"];
        public static string BRAVO_PW => ConfigurationManager.AppSettings["BRAVO_PW"];



        public static string DRMS_IP => ConfigurationManager.AppSettings["DRMS_IP"];
        public static string DRMS_ACCOUNT => ConfigurationManager.AppSettings["DRMS_ACCOUNT"];
        public static string DRMS_PW => ConfigurationManager.AppSettings["DRMS_PW"];



        public static string FPIS_IP => ConfigurationManager.AppSettings["FPIS_IP"];
        public static string FPIS_ACCOUNT => ConfigurationManager.AppSettings["FPIS_ACCOUNT"];
        public static string FPIS_PW => ConfigurationManager.AppSettings["FPIS_PW"];
    }
}
