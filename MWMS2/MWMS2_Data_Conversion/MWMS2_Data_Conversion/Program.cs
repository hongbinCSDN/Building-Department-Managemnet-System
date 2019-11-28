using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWMS2_Data_Conversion
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //string odbcConnectString = @"Driver={Oracle in OraDB18Home2};Server=orcl2;Persist Security Info=False;Trusted_Connection=Yes;UID=MWMS2;PWD=123456;";
            // string conString2 = "Dsn=test2;Uid=MWMS2;Pwd=123456;";

            //using(OdbcConnection connection = new OdbcConnection(odbcConnectString))
            //{
            //    connection.Open();
            //    Console.WriteLine("ConnectionString = {0}\n", odbcConnectString);
            //    Console.WriteLine("State = {0}", connection.State);
            //    Console.WriteLine("DataSource = {0}", connection.DataSource);
            //    Console.WriteLine("ServerVersion = {0}", connection.ServerVersion);
            //}
            Conversion_BL BL = new Conversion_BL();
            DataSet ds = BL.SelectDemo();
            if(ds != null)
            {
                Console.WriteLine(ds.Tables[0]);
            }

        }
    }
}
