using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using Zebra.Sdk.Printer.Discovery;

namespace MWMS2PrintAgent
{

    class Program
    {
      
      //  public static string m_DSN;
        static string RegisterMyProtocol(string myAppPath)  //myAppPath = full path to your application
        {
            string result = "";
            string appName = "MWMS2PrinterAgent";
            Console.WriteLine("reg Path : " + myAppPath);
            using (RegistryKey key2 = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\ProtocolExecute\mwms2printeragent"))
            {

                Console.WriteLine(@"Software\Microsoft\Internet Explorer\ProtocolExecute\mwms2printeragent");
                key2.SetValue("WarnOnOpen",0x00000000, RegistryValueKind.DWord);
                key2.Close();

            }
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(appName);  //open myApp protocol's subkey
            Registry.CurrentUser.CreateSubKey(@"SOFTWARE\IDG");
            if (key == null)  //if the protocol is not registered yet...we register it
            {

                Console.WriteLine("register key ");
                result = "Installed By ASL";
                key = Registry.ClassesRoot.CreateSubKey(appName);
                key.SetValue(string.Empty, "URL: " + appName + " Protocol");
                key.SetValue("URL Protocol", string.Empty);

                key = key.CreateSubKey(@"shell\open\command");
                key.SetValue(string.Empty, myAppPath + " " + "%1");
                //%1 represents the argument - this tells windows to open this program with an argument / parameter
            }

            key.Close();
            Console.WriteLine("register finished ");
            return result;
        }


        private static byte[] GetConfigLabel(PrinterLanguage printerLanguage,string m_DSN)
        {
            byte[] configLabel = null;
            if (printerLanguage == PrinterLanguage.ZPL)
            {
                configLabel = Encoding.UTF8.GetBytes("^XA^FO15,30^BY1,2,10^BCN,40,Y,N,N^A0,32,25^FD" + m_DSN + "^FS^XZ");


                // configLabel = Encoding.UTF8.GetBytes("^XA^FO335,50^BY1,2,10^BCN,40,Y,N,N^A0,32,25^FD"+m_DSN+"^FS^XZ");
            }
            else if (printerLanguage == PrinterLanguage.CPCL)
            {
                string cpclConfigLabel = "! 0 200 200 406 1\r\n" + "ON-FEED IGNORE\r\n" + "BOX 20 20 380 380 8\r\n" + "T 0 6 137 177 TEST\r\n" + "PRINT\r\n";
                configLabel = Encoding.UTF8.GetBytes(cpclConfigLabel);
            }
            return configLabel;
        }
        public static void PrintOut(object s )
        {  // File.AppendAllText(@"C:\Temp\logForPrintBarcode.txt", s.ToString()+ "\r\n");
        }

        static void Main_Testing()
        {
            Console.WriteLine("123");
            string[] args = Environment.GetCommandLineArgs();
            PrintOut("System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count");
            PrintOut(System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count);
            PrintOut("UsbDiscoverer.GetZebraUsbPrinters()");
            PrintOut(UsbDiscoverer.GetZebraUsbPrinters().Count);

            //foreach (var item in System.Drawing.Printing.PrinterSettings.InstalledPrinters)

            foreach (var item in UsbDiscoverer.GetZebraUsbPrinters())
                
            {
                PrintOut("Printer:"+item.ToString());
                try
                {

                    Connection printerConnection = item.GetConnection(); 
                   // printerConnection = new DriverPrinterConnection(item.ToString());
                    printerConnection.Open();
                    // await Task.Delay(1500);
                  //  PrinterLanguage printerLanguage = ZebraPrinterFactory.GetInstance(printerConnection).PrinterControlLanguage;
                    //await Task.Delay(1500);
                    printerConnection.Write(GetConfigLabel(PrinterLanguage.ZPL, "TESTING"));

                    PrintOut("Finished");
                }
                catch (Exception e)
                {
                    PrintOut(e);
                }

            }
            
        }

         static void Main()
        {
            Console.WriteLine("start ");
            string[] args = Environment.GetCommandLineArgs();

            PrintOut(args[0]);
            PrintOut(args[0]);

            /**
            File.AppendAllText(@"C:\Temp\logForPrintBarcode.txt",
                System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count + "\r\n");
            foreach (var item in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                File.AppendAllText(@"C:\Temp\logForPrintBarcode.txt", item.ToString() + "\r\n");
            }
            **/
            if (args.Length == 2)
            {

                try
                {
                    string m_DSN = args[1].Replace("mwms2printeragent:", string.Empty);
                    PrintOut(args[0]);
                    PrintOut(DateTime.Now.ToString() + " DSN:" + m_DSN);
                    if (m_DSN.Contains("_"))
                    {
                        string[] dsn = m_DSN.Split('_');
                        foreach (var item in UsbDiscoverer.GetZebraUsbPrinters())
                        {
                            Connection printerConnection = null;
                            try
                            {
                                printerConnection = item.GetConnection();
                                // printerConnection = new DriverPrinterConnection(item.ToString());
                                printerConnection.Open();
                                // await Task.Delay(1500);
                             //   PrinterLanguage printerLanguage = ZebraPrinterFactory.GetInstance(printerConnection).PrinterControlLanguage;
                                //await Task.Delay(1500);
                                int count = 0;
                                int.TryParse(dsn[1], out count);
                                for (int i = 0; i < count; i++)
                                {
                                    printerConnection.Write(GetConfigLabel(PrinterLanguage.ZPL, dsn[0]));
                                    System.Threading.Thread.Sleep(500);
                                }
                                if (dsn.Length != 2)
                                {
                                    int.TryParse(dsn[3], out count);
                                    for (int i = 0; i < count; i++)
                                    {
                                        printerConnection.Write(GetConfigLabel(PrinterLanguage.ZPL, dsn[2]));
                                        System.Threading.Thread.Sleep(500);
                                    }
                                }
                               
                            
                       
                                PrintOut("Finished");
                            }
                            catch (Exception e)
                            {
                                PrintOut("Error: " + e);
                            }
                            finally
                            {
                                try
                                {
                                    if (printerConnection != null)
                                    {
                                        printerConnection.Close();
                                    }
                                }
                                catch (ConnectionException)
                                {
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in UsbDiscoverer.GetZebraUsbPrinters())
                        {
                            Connection printerConnection = null;
                            try
                            {
                                printerConnection = item.GetConnection();
                                // printerConnection = new DriverPrinterConnection(item.ToString());
                                printerConnection.Open();
                                // await Task.Delay(1500);
                               // PrinterLanguage printerLanguage = ZebraPrinterFactory.GetInstance(printerConnection).PrinterControlLanguage;
                                //await Task.Delay(1500);
                                printerConnection.Write(GetConfigLabel(PrinterLanguage.ZPL, m_DSN));
                                PrintOut("Finished");
                            }
                            catch (Exception e)
                            {
                                PrintOut("Error: " + e);
                            }
                            finally
                            {
                                try
                                {
                                    if (printerConnection != null)
                                    {
                                        printerConnection.Close();
                                    }
                                }
                                catch (ConnectionException)
                                {
                                }
                            }
                        }
                    }
         
                }
                catch (Exception e)
                {
                    PrintOut("Error: " + e);
                }

            }
            else
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +".exe"; 
                
                string result = RegisterMyProtocol(path+appName);
                //string result = RegisterMyProtocol(args[0]);

                foreach (var item in UsbDiscoverer.GetZebraUsbPrinters())
                {
                    Connection printerConnection = null;
                    try
                    {
                        printerConnection = item.GetConnection();
                        // printerConnection = new DriverPrinterConnection(item.ToString());
                        printerConnection.Open();
                        // await Task.Delay(1500);
                      //  PrinterLanguage printerLanguage = ZebraPrinterFactory.GetInstance(printerConnection).PrinterControlLanguage;
                        //await Task.Delay(1500);
                        printerConnection.Write(GetConfigLabel(PrinterLanguage.ZPL, "Installed By ASL"));
                        //printerConnection.Write(GetConfigLabel(printerLanguage, "By"));
                        //printerConnection.Write(GetConfigLabel(printerLanguage, "ASL"));

                        PrintOut("Finished");
                    }
                    catch (Exception e)
                    {
                        PrintOut("Error: " + e);
                    }
                    finally
                    {
                        try
                        {
                            if (printerConnection != null)
                            {
                                printerConnection.Close();
                            }
                        }
                        catch (ConnectionException)
                        {
                        }
                    }
                    PrintOut("Installed Result: " + result);
                }
            }
        }

    }
}
