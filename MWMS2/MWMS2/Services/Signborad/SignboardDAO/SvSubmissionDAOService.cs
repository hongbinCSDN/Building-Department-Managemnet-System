using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SvSubmissionDAOService
    {
        public B_SV_SUBMISSION getByUuid(string uuid)
        {
            B_SV_SUBMISSION result = new B_SV_SUBMISSION();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                result = (from svs in db.B_SV_SUBMISSION
                                    where svs.UUID == uuid
                                    select svs).FirstOrDefault();
            }
            return result;
        }
        public B_SV_SUBMISSION getSVRecordBySvSubmissionUUID(string svRecordUUID)
        {
            List<B_SV_SUBMISSION> svSubmissionList = null;
            B_SV_SUBMISSION result = new B_SV_SUBMISSION();

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                svSubmissionList = (from svs in db.B_SV_SUBMISSION
                                where svs.UUID == svRecordUUID
                                select svs).ToList();
            }
            if (svSubmissionList.Count != 0)
            {
                result = svSubmissionList[0];
            }
            return result;
        }
    }
}