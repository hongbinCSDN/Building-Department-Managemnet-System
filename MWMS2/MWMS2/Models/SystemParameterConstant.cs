using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Models
{
    public static class SystemParameterConstant
    {
        //public const string StateId = "ST";

        public const string UserName = "Admin";
        //public const string WFUserUUID = "8a85934933b6427e0133b648cd94002a";//1007592


        public const string SubmissionDraftStatus = "SCU_DATA_ENTRY";
        public const string SVRecordDraftStatus = "DATA_ENTRY_DRAFT";
        public const string AppointedProfRSE = "RSE";
        public const string AppointedProfPRC = "PRC";
        public const string AppointedProfAP  = "AP";
        public const string AppointedProfRGE = "RGE";


        public static readonly string[] StatusLevel = { "ACTIVE", "INACTIVE" };
        public static readonly string[] SecurityLevel = { "SCU Internal","BD Staff" };
        public static readonly Dictionary<string, int> RankWF =
            new Dictionary<string, int>() {
                {"SPO", 1},
                {"PO", 2},
                {"TO", 3},
                {"TC",4 },
                { "others",5 },

};

    }
}