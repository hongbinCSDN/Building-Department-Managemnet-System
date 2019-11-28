using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
    public class CommentDisplayModel : DisplayGrid
    {
        public string Uuid { get; set; }    // comment record id
        public string RecordType { get; set; }
        public string RecordId { get; set; }    // audit record id
        //public string FromUser { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public DateTime? ModifiedDate { get; set; }
        public string CommentArea { get; set; }

        public List<B_SV_COMMENT> CommentList { get; set; }

        public string EditMode { get; set; }
        public string ErrMsg { get; set; }
        public string JavascriptToRun { get; set; }

    }
}