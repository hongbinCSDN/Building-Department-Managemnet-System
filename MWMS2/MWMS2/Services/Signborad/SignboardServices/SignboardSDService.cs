using MWMS2.Areas.Signboard.Models;
using MWMS2.Entity;
using MWMS2.Services.Signborad.SignboardDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardSDService
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
    }
}