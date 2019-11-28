using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
namespace MWMS2Interface.Services
{
    class Common
    {
             public string appendDoubleQuote(String str)
        {
            str = String.IsNullOrEmpty(str) ? "\"" + "\"" : "\"" + str.Trim() + "\"";
            return str;
        }
        public string appendCSVLine(String dataContent, string appendStr)
        {
            appendStr = appendDoubleQuote(appendStr);

            dataContent += String.IsNullOrEmpty(dataContent) ? appendStr : "," + appendStr;
            return dataContent;
        }
        public string appendNewLine(String dataContent)
        {
            dataContent += Environment.NewLine;
            return dataContent;
        }

        public void exportCSVFile(string fileName, string ext, List<string> Columns,
               List<List<object>> Data)
        {

            var sb = new StringBuilder();
            String eachLine = "";
            for (int i = 0; i < Columns.Count; i++)
            {
                eachLine = appendCSVLine(eachLine, Columns[i]);
            }
            eachLine = appendNewLine(eachLine);
            sb.Append(eachLine);

            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    List<object> eachRow = Data[i];
                    eachLine = "";
                    for (int j = 0; j < eachRow.Count; j++)
                    {

                        object dataObejct = eachRow[j];
                        String dataValue = "";
                        if (dataObejct is DateTime)
                        {
                            dataValue = ((DateTime)dataObejct).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            dataValue = dataObejct.ToString();

                        }
                        eachLine = appendCSVLine(eachLine, dataValue);
                    }
                    eachLine = appendNewLine(eachLine);
                    sb.Append(eachLine);
                }
            }
            var byteArray = Encoding.UTF8.GetBytes(sb.ToString());

           // File.WriteAllBytes(fileName, byteArray);
            // ZipFile.CreateFromDirectory(fileName, "C:\Users\user\Desktop\Document\Test");

            using (var zf = ZipFile.Open(fileName+".zip", ZipArchiveMode.Create))
            {
                var ze = zf.CreateEntry(fileName+".csv");
                using (var zs = ze.Open())
                {
                    using (var writ = new StreamWriter(zs, Encoding.UTF8))
                    {
                        writ.BaseStream.Write(byteArray,0,byteArray.Length);
                    }
                }
            }

        
        }

    }
}
