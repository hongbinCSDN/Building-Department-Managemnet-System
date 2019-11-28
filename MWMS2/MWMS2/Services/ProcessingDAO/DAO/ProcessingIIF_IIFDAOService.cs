using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;
using MWMS2.Areas.MWProcessing.Models;
using System.Data.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using MWMS2.Constant;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingIIF_IIFDAOService
    {
        private const string pImport36_searchq = @"select item.*,'View' As RESULT from P_IMPORT_36 item";

        public Fn05IIF_IIFModel Search(Fn05IIF_IIFModel model)
        {
            model.Query = pImport36_searchq;
            model.Search();
            return model;
        }

        public ServiceResult AddImport36(Fn05IIF_IIFModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {

                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    model.Import36.STATUS = ProcessingConstant.MW_UPLOAD_STATUS_UPLOADED;
                    model.Import36.CREATED_NAME = SessionUtil.LoginPost.BD_PORTAL_LOGIN;

                    db.P_IMPORT_36.Add(model.Import36);
                    try
                    {
                        int affRow = db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error:" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }
            }
        }


        public ServiceResult AddImport36Item(Fn05IIF_IIFModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {

                    for (int i = 0; i < model.Import36ItemList.Count(); i++)
                    {
                        db.P_IMPORT_36_ITEM.Add(model.Import36ItemList[i]);
                    }
                    try
                    {
                        int affRow = db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error:" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }
            }
        }
    }
}