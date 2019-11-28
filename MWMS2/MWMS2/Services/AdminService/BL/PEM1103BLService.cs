using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.AdminService.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.AdminService.BL
{
    public class PEM1103BLService
    {
        private PEM1103DAOService _DA;
        protected PEM1103DAOService DA
        {
            get
            {
                return _DA ?? (_DA = new PEM1103DAOService());
            }
        }

        #region Number Prefix
        public PEM1103MWNumberPrefixModel GetMWNumberPrefix()
        {
            return DA.GetMWNumberPrefix(new PEM1103MWNumberPrefixModel());
        }
        public ServiceResult SaveMWNumberPrefix(PEM1103MWNumberPrefixModel model)
        {
            return DA.SaveMWNumberPrefix(model);
        }
        #endregion

        #region Import MW Item

        public PEM1103ImportMWItemModel SearchImportMWItem(PEM1103ImportMWItemModel model)
        {
            return DA.SearchImportMWItem(model);
        }

        public PEM1103ImportMWItemModel SearchDetailImportMWItem(string uuid)
        {
            return DA.SearchDetailImportMWItem(uuid);
        }

        public ServiceResult UpdateImportMwItemDescription(PEM1103ImportMWItemModel model)
        {
            return DA.UpdateImportMwItemDescription(model);
        }

        #endregion

        #region Audit Percentage

        public PEM1103AuditPercentageModel SearchAuditPercentage()
        {
            return DA.SearchAuditPercentage();
        }

        public ServiceResult SaveAuditPercentage(PEM1103AuditPercentageModel model)
        {
            return DA.SaveAuditPercentage(model);
        }

        #endregion

        #region Number Validated Structrues

       
        public PEM1103NumValidatedStructuresModel SearchNumberValidatedStructrues()
        {
            return DA.SearchNumberValidatedStructrues();
        }

        public ServiceResult UpdateNumberValidatedStructrues(PEM1103NumValidatedStructuresModel model)
        {
            return DA.UpdateNumberValidatedStructrues(model);
        }

        #endregion

        #region Day Back
        public PEM1103DayBackModel SearchDayBack()
        {
            return DA.SearchDayBack();
        }

        public ServiceResult UpdateDayBack(PEM1103DayBackModel model)
        {
            return DA.UpdateDayBack(model);
        }

        #endregion

        #region Ack Letter Template Signature
        public PEM1103AckLetterTemplateSignatureModel SearchAckLetterTemplateSignature(string letterType)
        {
            return DA.SearchAckLetterTemplateSignature(letterType);
        }

        public ServiceResult UpdateSearchAckLetterTemplateSignature(PEM1103AckLetterTemplateSignatureModel model, string letterType)
        {
            return DA.UpdateSearchAckLetterTemplateSignature(model, letterType);
        }

        #endregion

        #region Public Holiday

        public PEM1103PublicHolidayModel InitPublicHolidayModel(string year)
        {
            PEM1103PublicHolidayModel model = new PEM1103PublicHolidayModel();
            if (string.IsNullOrEmpty(year))
                model.Year = DateTime.Now.Year.ToString();
            else
                model.Year = year;
            var result = DA.GetPublicHolidayData(model);
            if (result.holidays.Count == 0)
            {
                result.holidays = new List<Holiday>();
                result.holidays.Add(new Holiday());

            }
            //model.holidays = new List<Holiday>();
            //for (int i = model.holidays.Count; i < 30; i++)
            //{
            //    model.holidays.Add(new Holiday() { Date = null, HolidayName_Desc = null });
            //}
            return model;
        }

        public ServiceResult SavePublicHolidays(PEM1103PublicHolidayModel model)
        {
            return DA.SavePublicHolidays(model);
        }

        public ServiceResult DeleteHoliday(string UUID)
        {
            return DA.DeleteHoliday(UUID);
        }

        #endregion

        #region Number of daily Direct Return Over Counter
        public PEM1103NoOfDirectReturnModel SearchNoOfDailyDirectReturnData(PEM1103NoOfDirectReturnModel model)
        {
            Dictionary<string, int> weekDic = new Dictionary<string, int>();
            weekDic.Add("Monday", 2);
            weekDic.Add("Tuesday", 3);
            weekDic.Add("Wednesday", 4);
            weekDic.Add("Thursday", 5);
            weekDic.Add("Friday", 6);
            weekDic.Add("Saturday", 7);
            weekDic.Add("Sunday", 1);
            if (string.IsNullOrEmpty(model.Month))
                model.Month = DateTime.Now.Month.ToString();
            if (string.IsNullOrEmpty(model.Year))
                model.Year = DateTime.Now.Year.ToString();
            model.calendarModels = new List<CalendarModel>();
            int monthDay = DateTime.DaysInMonth(Convert.ToInt32(model.Year), Convert.ToInt32(model.Month));
            for(int i=1;i <= monthDay; i++)
            {
                model.calendarModels.Add(new CalendarModel()
                {
                    Date = new DateTime(Convert.ToInt32(model.Year), Convert.ToInt32(model.Month), i).ToString()
                    ,
                    Number = i
                    ,
                    Week = weekDic[new DateTime(Convert.ToInt32(model.Year), Convert.ToInt32(model.Month), i).DayOfWeek.ToString()]
                    ,
                    Counter = 0
                });
            }
            return DA.SearchNoOfDailyDirectReturnData(model);
        }

        public ServiceResult SaveNoOfDirectReturn(PEM1103NoOfDirectReturnModel model)
        {
            return DA.SaveNoOfDirectReturn(model);  
        }

        #endregion

        #region To Details
        public PEM1103ToDetailsModel GetToDetails()
        {
            return new PEM1103ToDetailsModel() { pEM1103ToDetailsListModels = DA.GetToDetails() };
        }

        public ServiceResult DeleteToDetails(string UUID)
        {
            return DA.DeleteToDetails(UUID);
        }

        public ServiceResult SaveToDetails(PEM1103ToDetailsModel model)
        {
            return DA.SaveToDetails(model);
        }

        #endregion


    }
}