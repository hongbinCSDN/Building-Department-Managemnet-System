using ExcelReader.BL;
using ExcelReader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ExcelReader.Controllers
{
    public class ExcelController : ApiController
    {

        private Excel_BL _BL;

        protected Excel_BL BL
        {
            get { return _BL ?? (_BL = new Excel_BL()); }
        }

        [Route("ExcelReader")]
        [HttpPost]
        public IHttpActionResult ExcelReader()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string name = request.Form["name"];
            int total = Convert.ToInt32(request.Form["total"]);
            int index = Convert.ToInt32(request.Form["index"]);
            var data = request.Files["data"];
            string Tpath = DateTime.Now.ToString("yyyy-MM-dd") + @"/import/";
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/") + Tpath +  index + "_MW." + name.Split('.')[1];
            string diPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(diPath)) { Directory.CreateDirectory(diPath); };
            try
            {
                data.SaveAs(filePath);            
            }
            catch (Exception ex)
            {

            }
            //byte[] fileData = null;
            //using(var binaryReader = new BinaryReader(data.InputStream))
            //{
            //    fileData = binaryReader.ReadBytes(data.ContentLength);
            //}
            //MemoryStream memoryStream = new MemoryStream();
            //memoryStream.Write(fileData, 0, fileData.Length);
            //var dt = BL.ExcelToDataTable(data.InputStream, name);
            return Ok("test");
        }

        [Route("ExcelReaderPath")]
        [HttpPost]
        public IHttpActionResult ExcelReaderPath(UploadModel model)
        {
            BL.ExcelToDataTable(model.filePath);
            return Ok("test");
        }


        [Route("GetSysFunc")]
        [HttpGet]
        public IHttpActionResult GetSysFunc()
        {
            return Ok(BL.GetSysFunc());
        }

    }
}
