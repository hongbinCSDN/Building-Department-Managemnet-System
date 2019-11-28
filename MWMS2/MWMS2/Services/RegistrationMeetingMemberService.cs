using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class RegistrationMeetingMemberService
    {
        private string Query_MeetingMember =
            " SELECT CM.UUID, (TITLE.ENGLISH_DESCRIPTION||' '|| upper(APPLN.SURNAME) ||' '|| APPLN.GIVEN_NAME_ON_ID) AS NAME  " +
            " , SOC.ENGLISH_DESCRIPTION AS SOCI, ROLE.CODE AS ROLE, CM.POST AS POST, CM.RANK AS RANK  " +
            " FROM C_COMMITTEE_MEMBER CM " +
            " INNER JOIN C_APPLICANT APPLN ON CM.APPLICANT_ID = APPLN.UUID  " +
            " INNER JOIN C_MEETING_MEMBER MM ON CM.UUID=MM.MEMBER_ID  " +
            " LEFT OUTER JOIN C_COMMITTEE_MEMBER_INSTITUTE CMI ON CMI.MEMBER_ID = CM.UUID   " +
            " LEFT OUTER JOIN C_S_SYSTEM_VALUE TITLE ON APPLN.TITLE_ID = TITLE.UUID  " +
            " LEFT OUTER JOIN C_S_SYSTEM_VALUE SOC ON CMI.SOCIETY_ID = SOC.UUID   " +
            " LEFT OUTER JOIN C_S_SYSTEM_VALUE ROLE ON MM.COMMITTEE_ROLE_ID = ROLE.UUID   " +
            " WHERE MM.meeting_id = :meeting " +
            " ORDER BY APPLN.SURNAME, APPLN.GIVEN_NAME_ON_ID ";

        public List<List<object>> getMeetingMemberForExport(string meetingId)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    List<List<object>> data = new List<List<object>>();

                    string query = Query_MeetingMember;
                    Dictionary<string, object> queryParameters = new Dictionary<string, object>();
                    queryParameters.Add("meeting", meetingId);
                    DbDataReader dr = CommonUtil.GetDataReader(conn, query, queryParameters);
                    data = CommonUtil.convertToList(dr);

                    return data;
                }

            }
        }

    }
}