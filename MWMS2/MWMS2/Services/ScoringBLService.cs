using MWMS2.Areas.Registration.Models;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MWMS2.Services
{
    public class ScoringBLService
    {

        private ScoringDAOService _DA;
        protected ScoringDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new ScoringDAOService());
            }
        }


        public string  SearcgWarningLetter_WhereQ(Fn09SC_SCModel model)
        {
            //StringBuilder queryWhere = new StringBuilder(" Where 1=1");
            //if (!string.IsNullOrEmpty(model.Surname))
            //{
            //    queryWhere.Append(" And upper(appl.SURNAME) like :SURNAME");
            //    model.QueryParameters.Add("SURNAME", "%" + model.Surname.ToUpper().Trim() + "%");
            //}
            //if (!string.IsNullOrEmpty(model.Given_Name))
            //{
            //    queryWhere.Append(" And upper(appl.GIVEN_NAME_ON_ID) like :GIVEN_NAME_ON_ID");
            //    model.QueryParameters.Add("GIVEN_NAME_ON_ID", "%" + model.Given_Name.ToUpper().Trim() + "%");
            //}
            //if (!string.IsNullOrEmpty(model.Chinese_Name))
            //{
            //    queryWhere.Append(" And appl.CHINESE_NAME like :CHINESE_NAME");
            //    model.QueryParameters.Add("CHINESE_NAME", "%" + model.Chinese_Name + "%");
            //}
            //if (!string.IsNullOrEmpty(model.HKID))
            //{
            //    queryWhere.Append(" And case when appl.HKID is null then " + EncryptDecryptUtil.getDecryptSQL("appl.PASSPORT_NO") +
            //                  " when appl.PASSPORT_NO is null then " + EncryptDecryptUtil.getDecryptSQL("appl.HKID") + " end like :ID_NO");
            //    model.QueryParameters.Add("ID_NO", "%" + model.HKID + "%");
            //}
            //return queryWhere.ToString();
            string whereQ = "";
            if(!string.IsNullOrWhiteSpace(model.SearchFileRef))
            {
                whereQ += " \r\n\t and lower(list.FILE_REF) like :FILE_REF ";
                model.QueryParameters.Add("FILE_REF", "%" + model.SearchFileRef.Trim().ToLower() + "%");
            }
            if(!string.IsNullOrWhiteSpace(model.SearchEngName))
            {
                whereQ += " \r\n\t and lower(list.ENG_NAME) like :ENG_NAME ";
                model.QueryParameters.Add("ENG_NAME", "%" + model.SearchEngName.Trim().ToLower() + "%");
            }
            if(!string.IsNullOrWhiteSpace(model.SearchChinName))
            {
                whereQ += " \r\n\t and lower(list.CHIN_NAME) like :CHIN_NAME ";
                model.QueryParameters.Add("CHIN_NAME", "%" + model.SearchChinName.Trim().ToLower() + "%");
            }

            return whereQ;
        }

        public Fn09SC_SCModel SearchWarningLetterRecord(Fn09SC_SCModel model)
        {
            model.QueryWhere = SearcgWarningLetter_WhereQ(model);
            return DA.SearchWarningLetterRecord(model);
        }

        public Fn09SC_SCModel getScoringDetail(string uuid, string type, string fileRef)
        {
            Fn09SC_SCModel model = new Fn09SC_SCModel();
            model.FormUuid = uuid;
            model.FormType = type;
            model.FormFileRef = fileRef;

            return  DA.getScoringDetail(model);;
        }

        //public Fn09SC_SCModel GetAppCompInfo(Fn09SC_SCModel model)
        //{
        //    return DA.GetAppCompInfo(model);
        //}

        public Fn09SC_SCModel SearchCompanyInfo(Fn09SC_SCModel model)
        {
            return DA.SearchCompanyInfo(model);
        }
        public Fn09SC_SCModel SearchIndInfo(Fn09SC_SCModel model)
        {
            return DA.SearchIndInfo(model);
        }
        public Fn09SC_SCModel SearchOffenceList(Fn09SC_SCModel model)
        {
            return DA.SearchOffenceList(model);
        }

        public ServiceResult CalculateTotalScore(string uuid)
        {
            return DA.CalculateTotalScore(uuid);
        }

        public Fn09SC_SCModel SearchCourseList(Fn09SC_SCModel model)
        {
            return DA.SearchCourseList(model);
        }

        public ServiceResult UpdateCourseSource(UpdateScoreModel model)
        {
            return DA.UpdateCourseSource(model);
        }

        public ServiceResult DeleteCourse(string UUID)
        {
            return DA.DeleteCourse(UUID);
        }

        public ServiceResult AddNewCourse(Fn09SC_SCModel model)
        {
            return DA.AddNewCourse(model);
        }

        public string Excel(Fn09SC_SCModel model)
        {
            model.QueryWhere = SearcgWarningLetter_WhereQ(model);
            DA.SearchWarningLetterRecord(model);
            return model.Export("Scoring Search Report");

        }

        public bool containsNegativeScore(Fn09SC_SCModel model)
        {
            bool containsNegative = false;

            foreach(var item in model.CourseScoreList)
            {
                //if(item.Contains("-"))
                //{
                //    containsNegative = true; break;
                //}
                int temp = Int32.Parse(item);
                if(temp < 0)
                {
                    containsNegative = true; break;
                }
            }

            return containsNegative;
        }
    }
}