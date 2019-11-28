using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Models
{
    public class ModelSystemValue
    {
        public string UUID { get; set; }
        public string SYSTEM_TYPE_ID { get; set; }
        public string CODE { get; set; }
        public string DESCRIPTION { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N0}")]
        public decimal? ORDERING { get; set; }
        public string Region { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }

    }
}