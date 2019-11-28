using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{
    [MetadataType(typeof(DRMS_DOCUMENT_META_DATA_Extend))]
    public partial class DRMS_DOCUMENT_META_DATA
    {
        public string SubDocList { get; set; }
    }
    public class DRMS_DOCUMENT_META_DATA_Extend
    {
    }
}