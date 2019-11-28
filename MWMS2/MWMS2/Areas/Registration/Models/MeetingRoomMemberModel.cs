using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Registration.Models
{
    public class MeetingRoomMemberModel
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public string sName { get; set; }
        public string gName { get; set; }
        public string HKID { get; set; }
        public string PassportNo { get; set; }
        public string Rank { get; set; }
        public string Post { get; set; }
        public string Career { get; set; }
        public string Role { get; set; }
        public string Absent { get; set; }
    }
}