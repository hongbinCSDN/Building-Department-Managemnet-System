using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SignboardSDDaoService
    {

        string Serach_ScannedDocument_q = ""
               + "\r\n" + "\t" + "SELECT reference_no, '' as remarks FROM                                                      "
               + "\r\n" + "\t" + "(                                                             "
               + "\r\n" + "\t" + " SELECT distinct reference_no from b_sv_submission                                           "
               + "\r\n" + "\t" + " UNION ALL SELECT distinct gc_number from b_sv_gc                                        "
               + "\r\n" + "\t" + " UNION ALL SELECT distinct order_no from b_sv_24_order                                         "
               + "\r\n" + "\t" + " UNION ALL SELECT distinct  complain_number from b_sv_complain                                              "
               + "\r\n" + "\t" + " UNION ALL SELECT distinct new_item_number from b_sv_signboard                                       "
               + "\r\n" + "\t" + ") where 1=1 ";
        public void SearchScannedDocument(Fn01SCUR_SDSearchModel model)
        {

            //model.Query = SearchCA_q;
            model.Query = Serach_ScannedDocument_q;  //need to change query after all
            model.QueryWhere = SearchSD_whereQ(model);
            model.Search();
        }
        public string Export_SD(Fn01SCUR_SDSearchModel model)
        {
            model.Query = Serach_ScannedDocument_q;
            model.QueryWhere = SearchSD_whereQ(model);
            return model.Export("ExportData");
        }


        private string SearchSD_whereQ(Fn01SCUR_SDSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.FILE_REFERCEN_NO))
            {
                whereQ += "\r\n\t" + "AND UPPER(REFERENCE_NO) LIKE :FILE_REFERCEN_NO";
                model.QueryParameters.Add("FILE_REFERCEN_NO", "%" + model.FILE_REFERCEN_NO.Trim().ToUpper() + "%");
            }


            return whereQ;
        }
        public int SubmissionCountbySD_ID(string SD_ID)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                return db.B_SV_SUBMISSION.Where(x => x.SV_SCANNED_DOCUMENT_ID == SD_ID).Count();
            }
              
        }
        public Fn01SCUR_SDDisplayModel ViewSD(string refNo)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
               List<B_SV_SCANNED_DOCUMENT> query = db.B_SV_SCANNED_DOCUMENT.Where(x => x.RECORD_ID == refNo).Include(x=>x.B_SV_SUBMISSION).OrderByDescending(x=>x.CREATED_DATE).ToList();



                // return db.B_SV_SUBMISSION.Where(x => x.SV_SCANNED_DOCUMENT_ID == SD_ID).Count();
             
                var temp = db.B_SV_SUBMISSION.Where(x => x.REFERENCE_NO == refNo).Count();
                return new Fn01SCUR_SDDisplayModel()
                {
                    SubmissionNo = refNo,
                
                    B_SV_SCANNED_DOCUMENT = query

                };
            }
        }
        public void SaveSD(Fn01SCUR_SDDisplayModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                foreach (var item in model.B_SV_SCANNED_DOCUMENT)
                {
                    db.B_SV_SCANNED_DOCUMENT.Attach(item);
                    db.Entry(item).State = EntityState.Modified;
                }

    
                db.SaveChanges();
            }
        }
        public void DeleteScannedDocument(string uuid)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                var query = db.B_SV_SCANNED_DOCUMENT.Where(x => x.UUID == uuid).FirstOrDefault();
                db.B_SV_SCANNED_DOCUMENT.Remove(query);
                db.SaveChanges();
          
            }

        }
        public ServiceResult postSubmittedDocs(Dictionary<string,string> list)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {

                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    foreach (var item in list)
                    {
                        var query = db.B_SV_SCANNED_DOCUMENT.Find(item.Key);
                        query.FOLDER_TYPE = item.Value;
                    }
                    try
                    {
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (DbEntityValidationException validationError)
                    {
                        var errorMessages = validationError.EntityValidationErrors
                        .SelectMany(validationResult => validationResult.ValidationErrors)
                        .Select(m => m.ErrorMessage);
                        Console.WriteLine("Error :" + validationError.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { validationError.Message } };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                    return new ServiceResult { Result = ServiceResult.RESULT_SUCCESS, Message = new List<string>() { "Save Successfully." } };
                }
                
               





            }

        }
        public void DeleteSD(Fn01SCUR_SDDisplayModel model)
        {
            DeleteScannedDocument(model.TargetDSNUUID);
        }

        public void CreateRecord(Fn01SCUR_SDDisplayModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                B_SV_SCANNED_DOCUMENT sd = new B_SV_SCANNED_DOCUMENT();
                SignboardCommonDAOService ss = new SignboardCommonDAOService();
                sd.DSN_NUMBER = ss.GetNumber();
                sd.RECORD_ID = model.SubmissionNo;
              //  sd.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
              //  sd.DOCUMENT_TYPE = SignboardConstant.SCAN_DOC_DOCUMENT_TYPE_FORM;
                //sd.FOLDER_TYPE = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU;

                db.B_SV_SCANNED_DOCUMENT.Add(sd);
                db.SaveChanges();
            }

        }
    }
}