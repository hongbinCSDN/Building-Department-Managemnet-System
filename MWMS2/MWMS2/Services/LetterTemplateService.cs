using MWMS2.Constant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class LetterTemplateService : BaseCommonService
    {
        public bool checkFileExist(string filePath)
        {
            AuditLogService.logDebug("File Path" +filePath);
            return File.Exists(filePath);
        }

        #region SMM
        public string getFilePathByLetterType(string letterType)
        {
            string path = ApplicationConstant.SMMFilePath;
            if (SignboardConstant.LETTER_TYPE_ACKNOWLEDGEMENT_LETTER_CODE.Equals(letterType)) // AL
            {
                path += SignboardConstant.LETTER_MODULE_TEMPLATE_FILE_PATH;
            }
            else if (SignboardConstant.LETTER_TYPE_ADUIT_FORM_CODE.Equals(letterType)) // FM
            {
                path += SignboardConstant.LETTER_TEMPLATE_FILE_PATH;
            }
            else if (SignboardConstant.LETTER_TYPE_ADVISORYLETTER_CODE.Equals(letterType)) // ADL
            {
                path += SignboardConstant.LETTER_TEMPLATE_FILE_PATH;
            }
            else if (SignboardConstant.LETTER_TYPE_D_LETTER_CODE.Equals(letterType)) // DL
            {
                path += SignboardConstant.LETTER_TEMPLATE_FILE_PATH;
            }
            else if (SignboardConstant.LETTER_TYPE_IO_LETTER_CODE.Equals(letterType)) // IO
            {
                path += SignboardConstant.LETTER_TEMPLATE_FILE_PATH;
            }

            return path;
        }
        #endregion
    }
}