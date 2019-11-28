using MWMS2.Areas.Signboard.Models;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Services.Signborad.SignboardDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardMMBLService
    {

        public Fn01SCUR_SDDisplayModel SetASThumbnail(Fn01SCUR_SDDisplayModel model)
        {


           
            SignboardSDDaoService ss = new SignboardSDDaoService();
            var DSNASSetToYes = model.TargetDSNUUID;
            model = ss.ViewSD(model.SubmissionNo);

            foreach (var item in model.B_SV_SCANNED_DOCUMENT)
            {

                item.AS_THUMBNAIL = "N";
                if (item.UUID == DSNASSetToYes)
                {
                    item.AS_THUMBNAIL = "Y";
                }
            }

            ss.SaveSD(model);
            return model;
         }
        public Fn01SCUR_MMSearchModel mailMergeSearch(Fn01SCUR_MMSearchModel model)
        {
            SignboardMMDAOService ss = new SignboardMMDAOService();
            model = ss.mailMergeSearch(model);
            return model;
        }
        public FileStreamResult exportToXLSX(String uuid)
        {
            SignboardDataExportService ss = new SignboardDataExportService();
            FileStreamResult a = ss.exportQPExcelData(uuid);
            return a;
        }
        public FileStreamResult exportToCSV(String uuid)
        {
            SignboardDataExportService ss = new SignboardDataExportService();
            FileStreamResult a = ss.exportQPCSVData(uuid);
            return a;
        }
        public ServiceResult UpdateLetterStatus(String uuid,String statusID,String mergeBy)
        {
            SignboardMMDAOService ss = new SignboardMMDAOService();
            ss.Update_B_SV_VALIDATION_Status(uuid, statusID, mergeBy);
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }
    }
}