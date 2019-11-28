using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01LM_ALDisplayModel : DisplayGrid
    {
        public P_MW_ACK_LETTER P_MW_ACK_LETTER { get; set; }
        public string DSN { get; set; }
        public string Address { get; set; }
        //public List<List<object>> Items { get; set; } // P_MW_DW_LETTER_ITEM, order by item_no
        // [0]: one P_MW_DW_LETTER_ITEM -> [0]: UUID, [1]: CHECKED, [2]: ITEM_NO, [3]: ITEM_TEXT (C or E)
        public List<P_MW_DW_LETTER_ITEM> Items { get; set; }
        public Dictionary<string, bool> ItemCheckList { get; set; }
        public Dictionary<string, string> ItemNoList { get; set; }
        public Dictionary<string, string> ItemTextList { get; set; }


        public string PbPName { get; set; }
        public string PbpFax { get; set; }
        public List<string> PbpAddr { get; set; }
        public string PbpAddrFull { get; set; }
        public string PrcName { get; set; }
        public List<string> PrcAddr { get; set; }
        public string PrcFax { get; set; }
        public string PrcAddrFull { get; set; }

        public DateTime? LetterDate { get; set; }
        public string LetterDateDisplay { get; set; }
        public string ReceivedDateDisplay { get; set; }
        public string Heading { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public string FirstPara { get; set; }
        public string SecondPara { get; set; }
        public string ThirdPara { get; set; }
        public string FourthPara { get; set; }
        public string FifthPara { get; set; }

        public string PoName { get; set; }
        public string PoPost { get; set; }
        public string PoContact { get; set; }

        // authority -> spo name, contact
        public string SpoName { get; set; }
        public string SpoPost { get; set; }
        public string SpoContact { get; set; }

        // footing -> paw name, contact
        public string PawName { get; set; }
        public string PawContact { get; set; }

        // radio buttons
        public string Language { get; set; }
        public string FileType { get; set; }

        // constants
        public string FLAG_Y = ProcessingConstant.FLAG_Y;
        public string FLAG_N = ProcessingConstant.FLAG_N;
        public string LANGUAGE_RADIO_CHINESE = ProcessingConstant.LANGUAGE_RADIO_CHINESE;
        public string LANGUAGE_RADIO_ENGLISH = ProcessingConstant.LANGUAGE_RADIO_ENGLISH;
        public string DOC_TYPE_PDF = ProcessingConstant.DOC_TYPE_PDF;
        public string DOC_TYPE_DOCX = ProcessingConstant.DOC_TYPE_DOCX;
    }

    public class Fn01LM_ALPrintModel
    {
        public string mwno { get; set; }
        public string pbpName { get; set; }
        public string pbpFax { get; set; }
        public string pbpAddr1 { get; set; }
        public string pbpAddr2 { get; set; }
        public string pbpAddr3 { get; set; }
        public string pbpAddr4 { get; set; }
        public string pbpAddr5 { get; set; }
        public string prcName { get; set; }
        public string prcFax { get; set; }
        public string prcAddr1 { get; set; }
        public string prcAddr2 { get; set; }
        public string prcAddr3 { get; set; }
        public string prcAddr4 { get; set; }
        public string prcAddr5 { get; set; }
        public string title { get; set; }
        public string address { get; set; }
        public string firstPara { get; set; }
        public string aDesc { get; set; }
        public string bDesc { get; set; }
        public string cDesc { get; set; }
        public string dDesc { get; set; }
        public string eDesc { get; set; }
        public string fDesc { get; set; }
        public string gDesc { get; set; }
        public string hDesc { get; set; }
        public string iDesc { get; set; }
        public string jDesc { get; set; }
        public string secondPara { get; set; }
        public string thirdPara { get; set; }
        public string fourthPara { get; set; }
        public string fifthPara { get; set; }
        public string spoPost { get; set; }
        public string spoName { get; set; }
        public string pawName { get; set; }
        public string pawContact { get; set; }
        public string letterDate { get; set; }
    }
}