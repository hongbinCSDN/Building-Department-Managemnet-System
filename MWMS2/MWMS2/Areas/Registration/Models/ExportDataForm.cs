using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Registration.Models
{
    public class ExportDataForm
    {
        public bool SaveSuccess { get; set; }
        public string ErrMsg { get; set; }
        public string RegistrationType { get; set; }
        public string UserAccessRight { get; set; }

        public string SearchTitle { get; set; }

        public string MenuLink { get; set; }

        public string CategoryUUID { get; set; }
        public string ApplicantRoleUUID { get; set; }
        public string PnrcUUID { get; set; }
        public string[] ArrCategoryUUID { get; set; }

        public bool SelectONE { get; set; }
        public bool SelectTWO { get; set; }
        public bool SelectTHREE { get; set; }
        public bool SelectFour { get; set; }
        public bool SelectFive { get; set; }

        public string RegisterType { get; set; }
        public string OutputType { get; set; }

        public List<FileInfo> WebSiteFile { get; set; }




























        public String getCategoryUUID()
        {
            return CategoryUUID.Trim();
        }

        public void setCategoryUUID(String categoryUUID)
        {
            this.CategoryUUID = categoryUUID;
        }

        public String getApplicantRoleUUID()
        {
            return ApplicantRoleUUID.Trim();
        }

        public void setApplicantRoleUUID(String applicantRoleUUID)
        {
            this.ApplicantRoleUUID = applicantRoleUUID;
        }

        public String getPnrcUUID()
        {
            return PnrcUUID.Trim();
        }

        public void setPnrcUUID(String pnrcUUID)
        {
            this.PnrcUUID = pnrcUUID;
        }

        public bool isSaveSuccess()
        {
            return SaveSuccess;
        }

        public void setSaveSuccess(bool saveSuccess)
        {
            this.SaveSuccess = saveSuccess;
        }

        public String getErrMsg()
        {
            return ErrMsg;
        }

        public void setErrMsg(String errMsg)
        {
            this.ErrMsg = errMsg;
        }

        public String getRegistrationType()
        {
            return RegistrationType.Trim();
        }

        public void setRegistrationType(String registrationType)
        {
            this.RegistrationType = registrationType;
        }

        public String getUserAccessRight()
        {
            return UserAccessRight.Trim();
        }

        public void setUserAccessRight(String userAccessRight)
        {
            this.UserAccessRight = userAccessRight;
        }

        public String getSearchTitle()
        {
            return SearchTitle.Trim();
        }

        public void setSearchTitle(String searchTitle)
        {
            this.SearchTitle = searchTitle;
        }

        public String getMenuLink()
        {
            return MenuLink.Trim();
        }

        public void setMenuLink(String menuLink)
        {
            this.MenuLink = menuLink;
        }

        public bool isSelectONE()
        {
            return SelectONE;
        }

        public void setSelectONE(bool selectONE)
        {
            this.SelectONE = selectONE;
        }

        public bool isSelectTWO()
        {
            return SelectTWO;
        }

        public void setSelectTWO(bool selectTWO)
        {
            this.SelectTWO = selectTWO;
        }

        public bool isSelectTHREE()
        {
            return SelectTHREE;
        }

        public void setSelectTHREE(bool selectTHREE)
        {
            this.SelectTHREE = selectTHREE;
        }

        public bool isSelectFour()
        {
            return SelectFour;
        }

        public void setSelectFour(bool selectFour)
        {
            this.SelectFour = selectFour;
        }

        public bool isSelectFive()
        {
            return SelectFive;
        }

        public void setSelectFive(bool selectFive)
        {
            this.SelectFive = selectFive;
        }

        public String getRegisterType()
        {
            return RegisterType;
        }

        public void setRegisterType(String registerType)
        {
            this.RegisterType = registerType;
        }

        public String getOutputType()
        {
            return OutputType;
        }

        public void setOutputType(String outputType)
        {
            this.OutputType = outputType;
        }

        public List<FileInfo> getWebSiteFile()
        {
            return WebSiteFile;
        }

        public void setWebSiteFile(List<FileInfo> webSiteFile)
        {
            this.WebSiteFile = webSiteFile;
        }

        public String[] getArrCategoryUUID()
        {
            return ArrCategoryUUID;
        }

        public void setArrCategoryUUID(String[] arrCategoryUUID)
        {
            this.ArrCategoryUUID = arrCategoryUUID;
        }













    }
}