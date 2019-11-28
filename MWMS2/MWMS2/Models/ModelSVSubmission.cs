using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Models
{
    public class ModelSVSubmission
    {
        public string UUID { get; set; }
        public string SubmissionNo { get; set; }
        public string DSN_NO { get; set; }
        public string Form_Code { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Received_Date { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }
        public string Batch_Number { get; set; }
     
    }
}