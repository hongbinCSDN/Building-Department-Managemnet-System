using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{

    public class SysEditModel : DisplayGrid
    {
        public string SYS_ROLE_ID { get; set; }
    }
}