
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn06CMM_CMModel : ApiController
    {

        public string selectedUUID { set; get; }
        public C_COMMITTEE_MEMBER C_COMMITTEE_MEMBER { get; set; }
        public C_COMMITTEE_MEMBER_INSTITUTE C_COMMITTEE_MEMBER_INSTITUTE { get; set; }


    }
}