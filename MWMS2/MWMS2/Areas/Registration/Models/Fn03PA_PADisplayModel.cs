//using MWMS2.Dao.Registration;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn03PA_PADisplayModel : DisplayGrid
    {

        public C_IND_APPLICATION C_IND_APPLICATION { get; set; }
        public C_IND_CERTIFICATE C_IND_CERTIFICATE { get; set; }
        public C_IND_QUALIFICATION C_IND_QUALIFICATION { get; set; }




    }
}