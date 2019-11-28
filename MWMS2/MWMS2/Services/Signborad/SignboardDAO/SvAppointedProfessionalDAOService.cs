using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SvAppointedProfessionalDAOService
    {
        public B_SV_APPOINTED_PROFESSIONAL getSvAppointedProfessional(string svRecordUUID, string identifyFlag)
        {
            List<B_SV_APPOINTED_PROFESSIONAL> svAppointedProfessionalList = null;

            B_SV_APPOINTED_PROFESSIONAL result = new B_SV_APPOINTED_PROFESSIONAL();

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                svAppointedProfessionalList = (from svap in db.B_SV_APPOINTED_PROFESSIONAL
                                                where svap.SV_RECORD_ID == svRecordUUID
                                                && svap.IDENTIFY_FLAG == identifyFlag
                                               select svap).ToList();
            }
            if(svAppointedProfessionalList.Count != 0)
            {
                result = svAppointedProfessionalList[0];
            }
            return result;
        }
    }
}