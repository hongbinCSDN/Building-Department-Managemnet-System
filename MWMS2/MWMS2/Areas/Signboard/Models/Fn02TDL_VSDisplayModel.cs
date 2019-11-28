using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
	public class Fn02TDL_VSDisplayModel : DisplayGrid
	{
        public bool ShowPrivacy { get; set; }

        public B_SV_RECORD B_SV_RECORD { get; set; }
		public B_SV_VALIDATION B_SV_VALIDATION { get; set; }
        public B_SV_APPOINTED_PROFESSIONAL AP { get; set; }
        public B_SV_APPOINTED_PROFESSIONAL RSE { get; set; }
        public B_SV_APPOINTED_PROFESSIONAL RGE { get; set; }
        public B_SV_APPOINTED_PROFESSIONAL PRC { get; set; }
		public List<B_SV_SCANNED_DOCUMENT> DocList { get; set; }
		public List<B_SV_PHOTO_LIBRARY> PhotoList { get; set; }
		public List<string> SignboardRelationList { get; set; }
		public List<ProcessHandlingDisplayModel> ProcessHandlings { get; set; }
        public List<B_SV_RECORD_ITEM> B_SV_RECORD_ITEM { get; set; }

        public string To { get; set; } // display string
		public string Po { get; set; }
		public string Spo { get; set; }

        public string svValidationUuid { get; set; }

        public string Status { get; set; }

        public string RecommendAck = SignboardConstant.VALIDATION_RESULT_ACKNOWLEDGED;
        public string RecommendRef = SignboardConstant.VALIDATION_RESULT_REFUSED;
        public string RecommendCond = SignboardConstant.VALIDATION_RESULT_CONDITIONAL;
        public string RecommendWith = SignboardConstant.VALIDATION_RESULT_WITHDRAW;

        public string recommendAckDisplay = SignboardConstant.RECOMMEND_ACK_STR;
		public string recommendRefDisplay = SignboardConstant.RECOMMEND_REF_STR;
		public string recommendCondDisplay = SignboardConstant.RECOMMEND_COND_STR;
		public string recommendWithDisplay = SignboardConstant.RECOMMEND_WITH_STR;

        public string WF_MAP_VALIDATION_TO = SignboardConstant.WF_MAP_VALIDATION_TO;
        public string WF_MAP_VALIDATION_PO = SignboardConstant.WF_MAP_VALIDATION_PO;
        public string WF_MAP_VALIDATION_SPO = SignboardConstant.WF_MAP_VALIDATION_SPO;

        public string SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU;
    }
}