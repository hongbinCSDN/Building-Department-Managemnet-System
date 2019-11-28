using MWMS2.Entity;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{

    public class ParaModel
    {
        public List<SYS_FUNC> CommFunc { get; set; }
        public List<SYS_FUNC> CRMFunc { get; set; }
        public List<SYS_FUNC> PEMFunc { get; set; }
        public List<SYS_FUNC> SMMFunc { get; set; }
        public List<SYS_FUNC> WLMFunc { get; set; }
    }
}