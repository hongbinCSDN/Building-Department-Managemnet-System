using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn03TSK_SBARModel : DisplayGrid
    {
        public string RefNo { get; set; }
        public string ReceiveDateFrom { get; set; }
        public string ReceiveDateTo { get; set; }
        public string RegNo { get; set; }
        public List<string> TypeOfMwFormAry { get; set; }
        public List<string> TypeOfMwItemAry { get; set; }
        public List<CheckboxModel> Checkbox_TypeofMwforms { get; set; }
        public MwClassListModel Checkbox_ItemNo_TypeofMWs_Class1 { get; set; }
        public MwClassListModel Checkbox_ItemNo_TypeofMWs_Class2 { get; set; }
        public MwClassListModel Checkbox_ItemNo_TypeofMWs_Class3 { get; set; }
        public List<CheckboxModel> Checkbox_Irregularities { get; set; }
    }
    public class MwClassListModel
    {
        public Dictionary<string, CheckboxModel> MWItemTypes { get; set; }
        public Dictionary<string, CheckboxModel> MWItemNos { get; set; }
    }
}