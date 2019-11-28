using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;
namespace MWMS2.Areas.Registration.Models
{
    public class Fn05MWIA_IRSearchModel: DisplayGrid
    {
        public string Year { get; set; }
        public string Date { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public string Surname { get; set; }
        public string GiveName { get; set; }
        public string FileRef { get; set; }
        public string InterviewAssessmentNo { get; set; }
        public string HKID { get; set; }
        public string PassportNo { get; set; }
        


    }
}