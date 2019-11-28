using MWMS2.Services.ProcessingDAO.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using MWMS2.Utility;
using MWMS2.Constant;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingRDBLService
    {
        private ProcessingRDDAOService _DA;
        protected ProcessingRDDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new ProcessingRDDAOService());
            }
        }

        #region Document to MWU
        public Fn12RD_DtmModel SearchDtm(Fn12RD_DtmModel model)
        {
            return model;
        }
        #endregion

        public bool assignNewDSN(Fn12RD_DtmModel model)
        {
            try
            {
                P_S_MW_NUMBER sMwNumber = new P_S_MW_NUMBER();
                String newNo = ProcessingCommonService.SynchronizedSaveNewRecord(sMwNumber, ProcessingConstant.PREFIX_DSN);
                P_MW_DSN mwDSN = ProcessingCommonService.createDsnRecord(newNo, ProcessingConstant.DSN_TBC);
                DA.createOrUpdateMWDSN(mwDSN);
                mwDSN.CREATED_DATE = DateUtil.getNow();
                mwDSN.CREATED_BY = mwDSN.CREATED_DATE.ToString("HH:mm");
                mwDSN.DSN = mwDSN.DSN;
                model.DSN_LIST.Add(mwDSN);
                model.DSN = mwDSN.DSN;
            }
            catch (Exception e)
            {   model.errorMsg = e.Message;
                return false;
            }
            return true;
        }

        public bool loadProceedDelivery(Fn12RD_DtmModel model)
        {
            try
            {
                ProcessingMWDSNDAOService dsnDAO = new ProcessingMWDSNDAOService();
                List<P_MW_DSN> DSN_LIST = dsnDAO.GetDSNList(ProcessingConstant.DSN_TBC);
                DSN_LIST.AddRange(dsnDAO.GetDSNList(ProcessingConstant.DSN_RD_RE_SENT_TBC));
                model.DSN_LIST = DSN_LIST;
            }
            catch (Exception e)
            {
                model.errorMsg = e.Message;
                return false;
            }
            return true;
        }


        public bool rollBack(Fn12RD_DtmModel model)
        {
            try
            {
                ProcessingMWDSNDAOService dsnDAO = new ProcessingMWDSNDAOService();
                ProcessingSystemValueDAOService dao = new ProcessingSystemValueDAOService();

                List<P_MW_DSN> DSN_LIST = dsnDAO.GetDSNList(ProcessingConstant.DSN_RD_DELIVER_COUNTED);

                P_S_SYSTEM_VALUE sysValue = dao.GetSSystemValueByTypeAndCode(
                           ProcessingConstant.SYSTEM_TYPE_DSN_STATUS, ProcessingConstant.DSN_TBC);
                           for (int i = 0; i < DSN_LIST.Count(); i++)
                {   DSN_LIST[i].P_S_SYSTEM_VALUE = null;
                    DSN_LIST[i].SCANNED_STATUS_ID = sysValue.UUID;
                }

                List<P_MW_DSN> resentDSNList = dsnDAO.GetDSNList(ProcessingConstant.DSN_RD_RE_SENT_COUNTED);

                sysValue = dao.GetSSystemValueByTypeAndCode(
                           ProcessingConstant.SYSTEM_TYPE_DSN_STATUS, ProcessingConstant.DSN_RD_RE_SENT_TBC);

                for (int i = 0; i < resentDSNList.Count(); i++)
                {
                    resentDSNList[i].P_S_SYSTEM_VALUE = null;
                    resentDSNList[i].SCANNED_STATUS_ID = sysValue.UUID;
                }

                DSN_LIST.AddRange(resentDSNList);
                dsnDAO.createOrUpdateMWDSNList(DSN_LIST);
            }
            catch (Exception e)
            {
                model.errorMsg = e.Message;
                return false;
            }
            finally
            {
                model.DSN = "";
                loadProceedDelivery(model);
            }
            return true;
        }

        public bool confirmDelivery(Fn12RD_DtmModel model)
        {
            try
            {
                ProcessingMWDSNDAOService dsnDAO = new ProcessingMWDSNDAOService();
                ProcessingSystemValueDAOService dao = new ProcessingSystemValueDAOService();

                List<P_MW_DSN> DSN_LIST = dsnDAO.GetDSNList(ProcessingConstant.DSN_RD_DELIVER_COUNTED);

                P_S_SYSTEM_VALUE sysValue = dao.GetSSystemValueByTypeAndCode(
                           ProcessingConstant.SYSTEM_TYPE_DSN_STATUS, ProcessingConstant.DSN_RD_DELIVERED);
                for (int i = 0; i < DSN_LIST.Count(); i++)
                {
                    DSN_LIST[i].P_S_SYSTEM_VALUE = null;
                    DSN_LIST[i].SCANNED_STATUS_ID = sysValue.UUID;
                    DSN_LIST[i].RD_DELIVERED_DATE = DateTime.Now;
                }

                List<P_MW_DSN> resentDSNList = dsnDAO.GetDSNList(ProcessingConstant.DSN_RD_RE_SENT_COUNTED);

                sysValue = dao.GetSSystemValueByTypeAndCode(
                           ProcessingConstant.SYSTEM_TYPE_DSN_STATUS, ProcessingConstant.DSN_RD_RE_SENT);

                for (int i = 0; i < resentDSNList.Count(); i++)
                {
                    resentDSNList[i].P_S_SYSTEM_VALUE = null;
                    resentDSNList[i].SCANNED_STATUS_ID = sysValue.UUID;
                }

                DSN_LIST.AddRange(resentDSNList);
                dsnDAO.createOrUpdateMWDSNList(DSN_LIST);
            }
            catch (Exception e)
            {
                model.errorMsg = e.Message;
                return false;
            }
            finally
            {
                model.DSN = "";
                loadProceedDelivery(model);
            }
            return true;
        }

        public bool deliveryCounted(Fn12RD_DtmModel model)
        {
            try
            {
                ProcessingMWDSNDAOService dsnDAO = new ProcessingMWDSNDAOService();

                P_MW_DSN DSN = dsnDAO.GetP_MW_DSN(model.DSN);

                if(DSN == null)
                {
                    model.errorMsg = "Invalid Barcode";
                }
                else
                {
                    string dsnStatus = DSN.P_S_SYSTEM_VALUE.CODE;
                    string newStatus = "";
                    if (ProcessingConstant.DSN_TBC.Equals(dsnStatus)){
                        newStatus = ProcessingConstant.DSN_RD_DELIVER_COUNTED;
                    }
                    else if (ProcessingConstant.DSN_RD_RE_SENT_TBC.Equals(dsnStatus)){
                        newStatus = ProcessingConstant.DSN_RD_RE_SENT_COUNTED;
                    }
                    else
                    {
                        model.errorMsg = "Invalid Barcode";
                    }

                    if (!String.IsNullOrEmpty(newStatus))
                    {
                        ProcessingSystemValueDAOService dao = new ProcessingSystemValueDAOService();
                        P_S_SYSTEM_VALUE sysValue = dao.GetSSystemValueByTypeAndCode(
                               ProcessingConstant.SYSTEM_TYPE_DSN_STATUS, newStatus);
                        DSN.P_S_SYSTEM_VALUE = null;
                        DSN.SCANNED_STATUS_ID = sysValue.UUID;
                        DA.createOrUpdateMWDSN(DSN);
                    }

                    

                }


            }
            catch (Exception e)
            {
                model.errorMsg = e.Message;
                return false;
            }
            finally
            {
                model.DSN = "";
                loadProceedDelivery(model);
            }
            return true;
        }



        #region Proceed Delivery
        public Fn12RD_PdModel SearchPd(Fn12RD_PdModel model)
        {
            return model;
        }
        #endregion
    }
}