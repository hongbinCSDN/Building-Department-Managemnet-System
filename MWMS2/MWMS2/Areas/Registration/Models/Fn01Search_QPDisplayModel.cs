using MWMS2.Entity;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Registration.Models
{

    public class Fn01Search_QPDisplayModel
    {
        public C_COMP_APPLICATION C_COMP_APPLICATION { get; set; }
        public List<C_BUILDING_SAFETY_INFO> C_BUILDING_SAFETY_INFOs { get; set; }

    }
}