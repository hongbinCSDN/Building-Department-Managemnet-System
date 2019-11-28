using MWMS2.Entity;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{

    public class CRM_AFDisplayModel
    {


        public C_S_SYSTEM_TYPE C_S_SYSTEM_TYPE { get; set; }
        public C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE { get; set; }

        public bool IsActive
        {
            get { return C_S_SYSTEM_VALUE != null && "Y".Equals(C_S_SYSTEM_VALUE.IS_ACTIVE); }
            set { C_S_SYSTEM_VALUE.IS_ACTIVE = value ? "Y" : "N"; }
        }

        public string RegType
        {
            get
            {
                string tmp = "";
                if (C_S_SYSTEM_VALUE != null)
                {

                    switch (C_S_SYSTEM_VALUE.REGISTRATION_TYPE)
                    {
                        case "CGC":
                            tmp = " General Contractor";
                            break;
     
                        case "CMW":
                            tmp = " MW Company";
                            break;

                        case "IP":
                            tmp = " Professional";
                            break;

                        case "IMW":
                            tmp = " MW Individual";
                            break;

                        default:
                            tmp = " ";
                            break;
                    }
                }
                return tmp;     
            }
            set { C_S_SYSTEM_VALUE.REGISTRATION_TYPE = value; }
           
        }
    }
}