using MWMS2.Areas.Admin.Models;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;
namespace MWMS2.Services
{
    public class ProcessingBookMarkSerivce
    {

        String SearchBM_q = ""
          + "\r\n" + "\t" + "Select UUID,  Street, Street_No,Building,Floor,Unit from P_S_ADDRESS_LIST where 1=1"
             ;
        private string SearchBM_whereQ(PEMBookMarkSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.Street))
            {
                whereQ += "\r\n\t" + "AND UPPER(STREET) LIKE :Street";
                model.QueryParameters.Add("Street", "%" + model.Street.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.StreetNo))
            {
                whereQ += "\r\n\t" + "AND UPPER(Street_No) LIKE :StreetNO";
                model.QueryParameters.Add("StreetNO", "%" + model.StreetNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Building))
            {
                whereQ += "\r\n\t" + "AND UPPER(BUILDING) LIKE :Building";
                model.QueryParameters.Add("Building", "%" + model.Building.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Floor))
            {
                whereQ += "\r\n\t" + "AND UPPER(FLOOR) LIKE :Floor";
                model.QueryParameters.Add("Floor", "%" + model.Floor.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Unit))
            {
                whereQ += "\r\n\t" + "AND UPPER(UNIT) LIKE :Unit";
                model.QueryParameters.Add("Unit", "%" + model.Unit.Trim().ToUpper() + "%");
            }

            return whereQ;
        }
        public PEMBookMarkSearchModel SearchBM(PEMBookMarkSearchModel model)
        {
            model.Query = SearchBM_q;

            model.QueryWhere = SearchBM_whereQ(model);

            model.Search();
            return model;
        }

        public string Excel(PEMBookMarkSearchModel model)
        {
            model.Query = SearchBM_q;

            model.QueryWhere = SearchBM_whereQ(model);

            return model.Export("Book Mark");
        }

        public PEMBookMarkSearchModel EditBMAddress(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var q = db.P_S_ADDRESS_LIST.Where(x => x.UUID == uuid).FirstOrDefault();

                if (q == null)
                {
                    return new PEMBookMarkSearchModel();
                }
                else
                return new PEMBookMarkSearchModel()
                {
                    Unit = q.UNIT,
                    Street = q.STREET,
                    StreetNo = q.STREET_NO,
                    Floor  =q.FLOOR,
                    Building = q.BUILDING
                };

            }

        }
        public ServiceResult SaveBMAddress(PEMBookMarkSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_S_ADDRESS_LIST l = new P_S_ADDRESS_LIST();
                if (!string.IsNullOrEmpty(model.UUID))
                {
                    l = db.P_S_ADDRESS_LIST.Where(x => x.UUID == model.UUID).FirstOrDefault();
                }
                
                l.UNIT = model.Unit;
                l.STREET = model.Street;
                l.STREET_NO = model.StreetNo;
                l.FLOOR = model.Floor;
                l.BUILDING = model.Building;

                if (string.IsNullOrEmpty(model.UUID))
                {
                    db.P_S_ADDRESS_LIST.Add(l);

                }
                db.SaveChanges();

            }



                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }
    }
}