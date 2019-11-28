using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Controllers
{
    public class CommentController : Controller
    {
        private ProcessingCommentBLService blService;
        protected ProcessingCommentBLService BL
        {
            get { return blService ?? (blService = new ProcessingCommentBLService()); }
        }

        // GET: MWProcessing/Comment
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult AddComment(CommentModel model)
        {
            BL.GetCommentModel(model);
            return View(model);
        }

        public ActionResult Create(P_MW_COMMENT model)
        {
            return Json(BL.AddP_MW_COMMENT(model));
        }

        public ActionResult Update(P_MW_COMMENT model)
        {
            return Json(BL.Update(model));
        }

        public ActionResult RollbackAndComment(CommentModel model)
        {
            return Json(BL.RollbackAndComment(model));
        }
    }
}