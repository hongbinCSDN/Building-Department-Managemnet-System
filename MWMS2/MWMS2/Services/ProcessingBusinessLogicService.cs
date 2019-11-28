using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Constant;

namespace MWMS2.Services
{
    public class ProcessingBusinessLogicService
    {
        // Stand alone form
        public List<String> autoGenMwFormCodeList()
        {
            List<String> formList = new List<string>();
            formList.Add(ProcessingConstant.FORM_MW01);
            formList.Add(ProcessingConstant.FORM_MW03);
            formList.Add(ProcessingConstant.FORM_MW05);
            formList.Add(ProcessingConstant.FORM_MW06);
            formList.Add(ProcessingConstant.FORM_MW32);
            return formList;
        }

        public string validFormNoForNewMwSubmission(string formNo)
        {
            string errorMsg = "";



            return errorMsg;
        }

    }
}