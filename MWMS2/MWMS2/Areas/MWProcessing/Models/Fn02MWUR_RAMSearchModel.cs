using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_RAMSearchModel : DisplayGrid
    {
        public string RefNo { get; set; }
        public string DSN { get; set; }
        public string File_Reference_Four { get; set; }
        public string File_Reference_Two { get; set; }
        public string ReceivedDateFrom { get; set; }
        public string ReceivedDateTo { get; set; }
        public string IsCaseTransfertoBravo { get; set; }
        public string BlockID { get; set; }
        public IEnumerable<SelectListItem> TransfertoBravoList
        {
            get
            {
                return (new List<SelectListItem>()
                {
                    new SelectListItem{Value = "",Text = "ALL"},
                    new SelectListItem{Value = "Y",Text = "Y"},
                    new SelectListItem{Value ="N",Text="N"}

                });
            }
        }
    }

    public class Fn02MWUR_RAMModel : DisplayGrid
    {
        public string MWRecordID { get; set; }
        public string RefNo { get; set; }
        public string IsCaseTransferToBravo { get; set; }
        public DateTime? CommencementDate { get; set; }
        public DateTime? CompletionCondintionalDate { get; set; }
        public string FileReference { get; set; }
        public DateTime? CompletionAcknowledgeDate { get; set; }
        public string LocationOfMinorWork { get; set; }
        public P_MW_FILEREF P_MW_FILEREF { get; set; }
        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTsNIC { get; set; }
        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTsIC { get; set; }
        public bool IsEnableTransfer { get; set; }
        public string TRANSFER_RRM { get; set; }
    }
}