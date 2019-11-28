using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;
namespace MWMS2.Areas.Registration.Models
{
    public class Fn04MCA_ICSearchModel: DisplayGrid
    {
        public string Year { get; set; }
        public string InterviewDate { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }

        public string UUID { get; set; }
        public string MeetingNo { get; set; }



    }
}