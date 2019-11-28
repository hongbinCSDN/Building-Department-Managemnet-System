using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn09SC_SCModel : DisplayGrid
    {
        public string UUID { get; set; }
        public string RegistrationNo { get; set; }

        public string Surname { get; set; }
        public string Given_Name { get; set; }
        public string Eng_Full_Name { get; set; }
        public string Chinese_Name { get; set; }
        public string HKID { get; set; }
        public string File_Reference_No { get; set; }
        public string Eng_Comp_Name { get; set; }
        public string Chi_Comp_Name { get; set; }
        public DateTime? Expiry_Date { get; set; }
        public DateTime? Removal_Date { get; set; }
        public List<string> CourseNameList { get; set; }
        public List<string> CourseIssueDateList { get; set; }
        public List<string> CourseScoreList { get; set; }

        // Added on 22-11-2019
        public string SearchFileRef { get; set; }
        public string SearchChinName { get; set; }
        public string SearchEngName { get; set; }

        public string FormUuid { get; set; }
        public string FormType { get; set; }
        public string FormFileRef { get; set; }
        public List<W_WL> WL_List { get; set; }
        // Individual
        public C_IND_APPLICATION C_IND_APPLICATION { get; set; }
        public C_APPLICANT C_APPLICANT { get; set; }
        // Company
        public C_COMP_APPLICATION C_COMP_APPLICATION { get; set; }

        // Form (Detail page)
        // Individual
        // Company
        public int NumOfPeriod { get; set; }
        public List<List<W_WL>> WL_List_by_AL { get; set; }
        public List<C_APPLICANT> AS_List { get; set; }
    }

    public class UpdateScoreModel
    {
        public string UUID { get; set; }
        public string CourseName { get; set; }
        public DateTime CourseDate { get; set; }
        public decimal CourseScore { get; set; }
    }

    //public class Fn09SC_SCDetailModel :DisplayGrid
    //{
        
      
    //    public List<string> CourseNameList { get; set; }
    //    public List<string> CourseIssueDateList { get; set; }
    //    public List<string> CourseScoreList { get; set; }
    //}

    public class Fn09SC_SCCourseModel : DisplayGrid
    {
     
    } 

   
  
}