using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using MWMS2.Entity;
using MWMS2.PrintBarcodeServiceReference;




namespace MWMS2.Controllers
{
    public class CommonFunction
    {
        public DateTime? StringToDateTime(string time)
        {
            DateTime? dt = null;
            if (time!=null)
             dt = DateTime.ParseExact(time, "dd/MM/yyyy", CultureInfo.InvariantCulture);


            return dt;
      
               
        }
        public bool PrintBarcodeLabel(string DSN)
        {
            try
            {
                var PrintClient = new PrinterWebSerSoapClient();
                PrintClient.PrintBarcodeLabel(DSN);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public string DateTimeToString(DateTime? dt)
        {
            string time = "";
            if(dt!=null)
             time = ((DateTime)dt).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


            return time;


        }
       




    }
}