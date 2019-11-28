using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MWMS2_RPT
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0001&as_at_today=23052019&reg_type=CGC");
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0002&committee_type_code=GBC");
        }

        protected void LinkButton3_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0003&panel_type_code=CRC");
        }

        protected void LinkButton4_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0004&as_at_date=08042019&gaz_fr_date=08022019&gaz_to_date=08042019&reg_type=CGC");
           // Response.Redirect("Default.aspx?rptId=CRM0004&gaz_fr_date=08022019&gaz_to_date=08042019&as_at_date=08042019&as_at_today=08042019");
        }

        protected void LinkButton5_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0005&today=08042019");
        }

        protected void LinkButton6_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0006&appln_id=8a82479024713c510124713c5614129d&c_appln_id=8a82479424a4d6ec0124a5a5850500b3&file_ref_no=test123&reg_type=CMW");
        }

        protected void LinkButton7_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0007");
        }

        protected void LinkButton8_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0008");
        }

        protected void LinkButton9_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0009&month=07&reg_type=CGC&year=2006");
        }

        protected void LinkButton10_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0010&fr_date=02042019&to_date=29052019");
        }

        protected void LinkButton11_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0011&as_date=12042019&title=bcisrpt");
        }

        protected void LinkButton12_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0012&as_date=31052019&title=");
        }

        protected void LinkButton13_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0013&as_date=31052019&title=bcisrpt");
        }

        protected void LinkButton14_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0014");
        }

        protected void LinkButton15_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0015&as_date=28052019&title=CRM");
        }

        protected void LinkButton16_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0016&as_date=03062019&title=Bcisreport");
        }

        protected void LinkButton17_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0017&today=12042019");
        }

        protected void LinkButton18_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0018&today=12042019");
        }

        protected void LinkButton19_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0019&date_type_txt=ASLDateTypeTxt&search_date_from=20190416&search_date_to=20190501&active=Y");
        }

        protected void LinkButton20_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0020&today=16042019");
        }

        protected void LinkButton21_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0021&today=16042019");
        }

        protected void LinkButton22_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0022&willingness_qp=Y&today=16042019");
        }

        protected void LinkButton23_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0023&today=16042019&willingness_qp=A");
        }

        protected void LinkButton24_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0024&willingness_qp=Y&as_at_today=29032019");
        }

        protected void LinkButton25_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0025&willingness_qp=Y&qp_as_at=29032019");
        }
    
        protected void LinkButton26_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0026&today=16042019&willingness_qp=Y");
        }

        protected void LinkButton27_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0027&today=16042019");
        }

        protected void LinkButton28_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0028&date_type_txt=01052018&status_txt=All&txt_to=24052019"); 
        }

        protected void LinkButton29_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0029&as_at_date=08042019&gaz_fr_date=07032006&gaz_to_date=31122019&reg_type=IP&cat_gp=AP");
        }

        protected void LinkButton30_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0030&short_name=aaaa&today=20190418&col1_txt=bcis");
        }

        protected void LinkButton31_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0031&short_name=aaaa&today=20190418&col1_txt=bcis");
        }

        protected void LinkButton32_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0032&today=20190418&reg_type=CGC");
        }

        protected void LinkButton33_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0033&today=20190418");
        }

        protected void LinkButton34_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0034&today=20190418");
        }

        protected void LinkButton35_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0035&today=20190418");
        }

        protected void LinkButton36_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0036&as_today=18042019&reg_type=CGC");
        }

        protected void LinkButton37_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0037");
        }

        protected void LinkButton38_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0038");
        }

        protected void LinkButton39_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0039");
        }

        protected void LinkButton40_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0040&gaz_date=12042019&acting=0");
        }

        protected void LinkButton41_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0041&gaz_date=12042019&acting=0");
        }

        protected void LinkButton42_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0042&gaz_date=12042019&acting=0");
        }

        protected void LinkButton43_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0043&gaz_date=12042019&acting=0");
        }

        protected void LinkButton44_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0044&acting=N");
        }

        protected void LinkButton45_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0045&acting=N");
        }

        protected void LinkButton46_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0046&report_type=1&inputFirstApplicationDate=01012012&inputOutstandingDate=01012012&inputResultDate=01012012");
        }

        protected void LinkButton47_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0047&report_type=1");
        }

        protected void LinkButton48_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0048&as_today=10062019");
        }

        protected void LinkButton49_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0049&today=20190423");
        }

        protected void LinkButton50_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0050&category=GBC");
        }

        protected void LinkButton51_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0051&category=GBC");
        }

        protected void LinkButton52_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0052&category=");
        }

        protected void LinkButton53_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0053&category=");
        }

        protected void LinkButton54_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0054&as_today=12062019&reg_type=CGC");
        }

        protected void LinkButton55_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0055&reg_type=CGC&as_year=2008");
        }

        protected void LinkButton56_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0056&fr_date=31012006&reg_type=CGC&to_date=19062019");
        }

        protected void LinkButton59_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0059&cat_gp=AP&reg_type=IP");
        }

        protected void LinkButton57_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0057&category=GBC");
        }

        protected void LinkButton58_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0058");
        }

        protected void LinkButton60_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0060&cat_gp=AP&reg_type=IP");
        }

        protected void LinkButton61_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0061");
        }

        protected void LinkButton62_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0062&ctr_code=GBC&gaz_fr_date=31011990&gaz_to_date=31122019&reg_type=CGC");
        }

        protected void LinkButton63_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0063&acting=0&auth_uuid=8a8247902429b5ad012429b5adec0003&gaz_date=31122019&reg_type=CGC");
        }

        protected void LinkButton64_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0064&as_at_date=01062019&gaz_fr_date=01011990&gaz_to_date=24062019&reg_type=CGC");
        }

        protected void LinkButton65_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0065&as_at_date=01052019&gaz_fr_date=07032006&gaz_to_date=31122019&reg_type=IP&cat_gp=AP");
        }

        protected void LinkButton66_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0066&cat_group=GB&expiry_fr_date=01011990&expiry_to_date=01122019&reg_type=CGC&order_name=EXP_D");
        }

        protected void LinkButton67_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0067");
        }

        protected void LinkButton68_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0068&rec_type=CGC");
        }

        protected void LinkButton69_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0069&received_fr_date=01011990&received_to_date=31122019");
        }

        protected void LinkButton70_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0070&received_fr_date=01011990&received_to_date=31122019");
        }

        protected void LinkButton71_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx?rptId=CRM0071&cat_group=GB&expiry_fr_date=01011990&expiry_to_date=01122019&reg_type=IP&order_name=EXP_D");
        }
    }
    
}