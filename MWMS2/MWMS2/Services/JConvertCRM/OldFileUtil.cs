
using System;
using System.IO;
using System.Text;

public class FileUtil
{

    public static void deleteFile(String filepath)
    {
        DirectoryInfo a = new DirectoryInfo(filepath);
        FileInfo[] fileList = a.GetFiles();// listFiles();
        for (int i = 0; i < fileList.Length; i++)
        {
            fileList[i].Delete();
        }
        a.Delete();
    }

    public static void copy(FileInfo src, FileInfo dst) //throws Exception
    {
        File.Copy(src.FullName, dst.FullName);
        /*InputStream in = new FileInputStream(src );
        OutputStream out = new FileOutputStream(dst);
        // Transfer bytes from in to out
        byte[] buf = new byte[1024];
        int len;
        while ((len = in.read(buf)) > 0) {
            out.write(buf, 0, len);
        }
        in.close();
        out.close();*/
    }

    public static void readReplace(FileInfo fname, String oldPattern, String replPattern)
    {//throws Exception {
        String line;
        StringBuilder sb = new StringBuilder();
        FileStream fis = new FileStream(fname.FullName, FileMode.OpenOrCreate);

        int amount = 4048 * 4;

        byte[] buf = new byte[amount];
        int len;
        while ((len = fis.Read(buf, 0, buf.Length)) > 0)
        {
            String line1 = Encoding.UTF8.GetString(buf); //new String(buf, "UTF-8");
            line1 = line1.Replace(oldPattern, replPattern);
            sb.Append(line1);
            buf = new byte[amount];
        }
        fis.Close();

        writeUtf8ToFile(fname, false, sb.ToString());
        /**
	    OutputStream out = new FileOutputStream(fname);
	    out.write(sb.toString().getBytes());
	    out.close();
    **/
        /**    
            BufferedWriter out=new BufferedWriter ( new FileWriter(fname));
            out.write(sb.toString());
            out.close();**/
    }
    private static void writeUtf8ToFile(FileInfo file,
             bool append, String data)
    {//throws IOException {

        byte[] utf8_bom = { (byte)0xEF, (byte)0xBB, (byte)0xBF };

        bool exists = file.Exists;//.isFile();
        FileStream outFile = new FileStream(file.FullName, FileMode.CreateNew);
        try
        {
            if (!exists)
            {
                // then this is a new file
                // write the BOM marker
                outFile.Write(utf8_bom, 0, utf8_bom.Length);
            }
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            outFile.Write(byteData, 0, byteData.Length);
            outFile.Close();
            /*Writer writer = new OutputStreamWriter(out, "UTF-8");
		      try {
		        writer.write(data);
		      } finally {
		        writer.close();
		      }
		    } finally {
		      out.close();
		    }*/

        }
        catch (Exception ex)
        {

        }
    }
}
