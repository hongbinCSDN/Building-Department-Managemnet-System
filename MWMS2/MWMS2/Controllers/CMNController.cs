using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Entity;

namespace MWMS2.Controllers
{
    public class CMNController : Controller
    {
       // private Entities1 db = new Entities1();
        // GET: CMN
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CMN01()
        {

            //return View(db.WL.ToList());
            return View();
        }
        ////public ActionResult GetSearch()  
        ////{
        ////    CMNSearch m_CMNSearch = new CMNSearch { CMNSearchKeyWord = "333" };
        ////    return View(m_CMNSearch);
        ////}   
        [HttpPost]
        public JavaScriptResult GetSearchKeyword(CMNSearch m_CMNSearch)
        {

            return JavaScript("alert('" + m_CMNSearch.CMNSearchKeyWord + "')");
        }
        //public ActionResult GetSearchKeyword(FormCollection post)
        //{
        //    System.Diagnostics.Debug.WriteLine("------------------------------------------");

        //    CMNSearch m_CMNSearch = new CMNSearch();
        //    m_CMNSearch.CMNSearchKeyWord = post["CMNSearchKeyWord"];


        //     System.Diagnostics.Debug.WriteLine("------------------------------------------");

        //    return View(m_CMNSearch);
        //}
        // public JavaScriptResult GetSearchKeyword(FormCollection post)
        // {
        //     System.Diagnostics.Debug.WriteLine("------------------------------------------");
        //
        //     CMNSearch m_CMNSearch = new CMNSearch
        //     {
        //         CMNSearchKeyWord = post["CMNSearchKeyWord"]
        //     };
        //
        //
        //     System.Diagnostics.Debug.WriteLine("------------------------------------------");
        //
        //     return JavaScript("alert('" + m_CMNSearch.CMNSearchKeyWord+"')");
        // }
    }
}