using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Controllers
{
    public class ACCController : Controller
    {
        /*
         * 
SELECT table_name FROM all_tables where OWNER = 'MWMS2'  AND TABLE_NAME LIKE 'P%'
MINUS

SELECT TABLE_NAME FROM all_constraints WHERE OWNER = 'MWMS2' AND CONSTRAINT_TYPE = 'P' AND TABLE_NAME LIKE 'P%'
*/
        // GET: ACC
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ACC()
        {
            return View();
        }
    }
}