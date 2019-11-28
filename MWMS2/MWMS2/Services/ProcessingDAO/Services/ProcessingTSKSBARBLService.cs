using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Services.ProcessingDAO.DAO;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingTSKSBARBLService
    {
        private ProcessingTSKSBARDAOService _DA;
        protected ProcessingTSKSBARDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new ProcessingTSKSBARDAOService());
            }
        }

        private ProcessingMwItemDAOService _MWItemDA;
        protected ProcessingMwItemDAOService MWItemDA
        {
            get
            {
                return _MWItemDA ?? (_MWItemDA = new ProcessingMwItemDAOService());
            }
        }

        private string Search_whereq(Fn03TSK_SBARModel model)
        {
            StringBuilder whereq = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(model.ReceiveDateFrom))
            {
                whereq.Append(@" and to_date(to_char(F.RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') >= to_date(:receiveDtFrom,'dd/MM/yyyy') ");
                model.QueryParameters.Add("receiveDtFrom", model.ReceiveDateFrom.Trim());
            }

            if (!string.IsNullOrWhiteSpace(model.ReceiveDateTo))
            {
                whereq.Append(@" and to_date(to_char(F.RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')  <= to_date(:receiveDtTo,'dd/mm/yyyy') ");
                model.QueryParameters.Add("receiveDtTo", model.ReceiveDateTo.Trim());
            }

            model = GetTypeOfMwFormAry(model);
            if (model.TypeOfMwFormAry.Count > 0)
            {
                whereq.Append(@" and D.S_FORM_TYPE_CODE in (:formNoList) ");
                model.QueryParameters.Add("formNoList", model.TypeOfMwFormAry);
            }

            if (!string.IsNullOrWhiteSpace(model.RegNo))
            {
                whereq.Append(@" and (AP.CERTIFICATION_NO = :pbpOrPrcNo or PRC.PRC.CERTIFICATION_NO = :pbpOrPrcNo) ");
                model.QueryParameters.Add("pbpOrPrcNo", model.RegNo.Trim());
            }

            model = GetTypeOfMwItemCondition(model);
            if (model.TypeOfMwItemAry.Count > 0)
            {
                whereq.Append(@" and mwri.MW_ITEM_CODE in (:typeOfMwItemAry) ");
                model.QueryParameters.Add("typeOfMwItemAry", model.TypeOfMwItemAry);
            }

            whereq.Append(" " + GetIrregularitiesCondition(model));

            model.QueryParameters.Add("mwRecordStatusCode", ProcessingConstant.MW_SECOND_COMPLETE);
            model.QueryParameters.Add("mwAckResult", ProcessingConstant.MW_ACKN_STATUS_COMPLETE);
            model.QueryParameters.Add("auditRelated", ProcessingConstant.FLAG_Y);
            // model.QueryParameters.Add("taskNameList",
            //    new ArrayList<String>(Arrays.asList(
            //            BpmConstant.WF_ACTIVITY_ACKNOWLEDGEMENT_SPO,
            //            BpmConstant.WF_ACTIVITY_VERIFICATION_SPO,
            //            BpmConstant.WF_ACTIVITY_ACKNOWLEDGEMENT_PO,
            //            BpmConstant.WF_ACTIVITY_VERIFICATION_TO)
            //        ));
            model.QueryParameters.Add("taskNameList", "");

            model.Sort = " REF_NO.REFERENCE_NO ";
            return whereq.ToString();
        }
        private string GetIrregularitiesCondition(Fn03TSK_SBARModel model)
        {
            List<string> criteriaList = new List<string>();
            foreach (var item in model.Checkbox_Irregularities)
            {
                if (item.IsChecked)
                {
                    continue;
                }
                switch (item.Code)
                {
                    case ProcessingConstant.REQUIRE_SITE_RECT:
                        criteriaList.Append(@" (SIC.GROUNDS_OF_REFUSAL = 'Y' or SIC.GROUNDS_OF_CONDITIONAL = 'Y') ");
                        break;
                    case ProcessingConstant.RECOMMENDATION_NOT_IN_ACKNOWLEDGEMENT_AND_NOT_REQUIRE_SITR_RECTIFICATION:
                        criteriaList.Append(@" (SIC.GROUNDS_OF_REFUSAL = 'N' or SIC.GROUNDS_OF_CONDITIONAL = 'N') ");
                        break;
                    case ProcessingConstant.RECOMMENDATION_MARK_AS_ACKNOWLEDGEMENT:
                        criteriaList.Append(@" (SIC.RECOMMEDATION_APPLICATION = 'O') ");
                        break;
                    case ProcessingConstant.WITHDRAWN_WITHOUT_IRREGULARITIES:
                        criteriaList.Append(@" (SIC.GROUNDS_OF_REFUSAL = 'W') ");
                        break;
                }
            }

            string criteriaStr = "";
            for (int i = 0; i < criteriaList.Count; i++)
            {
                if (i == 0)
                {
                    criteriaStr += " and ( " + criteriaList[i];
                }
                else
                {
                    criteriaStr += " or " + criteriaList[i];
                }

                if (i == criteriaList.Count - 1)
                {
                    criteriaStr += ") ";
                }
            }
            return criteriaStr;
        }
        private Fn03TSK_SBARModel GetTypeOfMwItemCondition(Fn03TSK_SBARModel model)
        {
            model.TypeOfMwItemAry = new List<string>();
            foreach (var item in model.Checkbox_ItemNo_TypeofMWs_Class1.MWItemNos)
            {
                if (item.Value.IsChecked)
                {
                    model.TypeOfMwItemAry.Add(item.Value.Code);
                }
            }
            foreach (var item in model.Checkbox_ItemNo_TypeofMWs_Class2.MWItemNos)
            {
                if (item.Value.IsChecked)
                {
                    model.TypeOfMwItemAry.Add(item.Value.Code);
                }
            }
            foreach (var item in model.Checkbox_ItemNo_TypeofMWs_Class3.MWItemNos)
            {
                if (item.Value.IsChecked)
                {
                    model.TypeOfMwItemAry.Add(item.Value.Code);
                }
            }
            return model;
        }
        private Fn03TSK_SBARModel GetTypeOfMwFormAry(Fn03TSK_SBARModel model)
        {
            model.TypeOfMwFormAry = new List<string>();
            foreach (var item in model.Checkbox_TypeofMwforms)
            {
                if (item.IsChecked)
                {
                    model.TypeOfMwFormAry.Add(item.Code);
                }
            }
            return model;
        }
        public Fn03TSK_SBARModel Search(Fn03TSK_SBARModel model)
        {

            model.QueryWhere = Search_whereq(model);
            return DA.Search(model);
        }


        public MwClassListModel GetItemNosAndType(string classType)
        {
            Dictionary<string, CheckboxModel> mwItemNos = new Dictionary<string, CheckboxModel>();
            Dictionary<string, CheckboxModel> mwItemTypes = new Dictionary<string, CheckboxModel>();
            foreach (var item in MWItemDA.GetMWItemNoAndType(classType))
            {
                if (!mwItemTypes.ContainsKey(item.MWItemType))
                {
                    mwItemTypes.Add(item.MWItemType, new CheckboxModel()
                    {
                        Code = item.MWItemType
                        ,
                        Description = item.MWItemType
                        ,
                        Group = classType + "_" + item.MWItemType
                        ,
                        UUID = item.MWItemTypeUUID
                    });
                }

                if (!mwItemNos.ContainsKey(item.MWItemNo))
                {
                    mwItemNos.Add(item.MWItemNo, new CheckboxModel()
                    {
                        Code = item.MWItemNo
                    ,
                        Description = item.MWItemNo
                    ,
                        Group = classType + "_" + item.MWItemType
                    ,
                        UUID = item.MWItemUUID
                    });
                }
                else
                {
                    mwItemNos[item.MWItemNo].Group += "|" + classType + "_" + item.MWItemType;
                }
            }

            return new MwClassListModel()
            {
                MWItemTypes = mwItemTypes.OrderBy(m => m.Key).ToDictionary(m => m.Key, o => o.Value)
                ,
                MWItemNos = mwItemNos
            };
        }

        public List<CheckboxModel> GetCheckboxTypeofMwforms()
        {
            return new List<CheckboxModel>()
            {
                new CheckboxModel()
                {
                    Code=ProcessingConstant.FORM_MW01
                    ,Description=ProcessingConstant.FORM_MW01
                }
                ,new CheckboxModel()
                {
                    Code=ProcessingConstant.FORM_MW03
                    ,Description=ProcessingConstant.FORM_MW03
                }
                ,new CheckboxModel()
                {
                    Code=ProcessingConstant.FORM_MW05
                    ,Description=ProcessingConstant.FORM_MW05
                }
                ,new CheckboxModel()
                {
                    Code=ProcessingConstant.FORM_MW02
                    ,Description=ProcessingConstant.FORM_MW02
                }
                ,new CheckboxModel()
                {
                    Code=ProcessingConstant.FORM_MW04
                    ,Description=ProcessingConstant.FORM_MW04
                }
                ,new CheckboxModel()
                {
                    Code=ProcessingConstant.FORM_MW11
                    ,Description=ProcessingConstant.FORM_MW11
                }
                ,new CheckboxModel()
                {
                    Code=ProcessingConstant.FORM_MW12
                    ,Description=ProcessingConstant.FORM_MW12
                }
            };
        }

        public List<CheckboxModel> GetCheckboxIrregularities()
        {
            return new List<CheckboxModel>()
            {
                new CheckboxModel()
                {
                    Code=ProcessingConstant.REQUIRE_SITE_RECT
                    ,Description=ProcessingConstant.DISPLAY_REQUIRE_SITE_RECT
                }
                ,new CheckboxModel()
                {
                    Code=ProcessingConstant.RECOMMENDATION_NOT_IN_ACKNOWLEDGEMENT_AND_NOT_REQUIRE_SITR_RECTIFICATION
                    ,Description=ProcessingConstant.DISPLAY_RECOMMENDATION_NOT_IN_ACKNOWLEDGEMENT_AND_NOT_REQUIRE_SITR_RECTIFICATION
                }
                ,new CheckboxModel()
                {
                    Code=ProcessingConstant.RECOMMENDATION_MARK_AS_ACKNOWLEDGEMENT
                    ,Description=ProcessingConstant.DISPLAY_RECOMMENDATION_MARK_AS_ACKNOWLEDGEMENT
                }
                ,new CheckboxModel()
                {
                    Code=ProcessingConstant.WITHDRAWN_WITHOUT_IRREGULARITIES
                    ,Description=ProcessingConstant.DISPLAY_WITHDRAWN_WITHOUT_IRREGULARITIES
                }
            };
        }

        public string Excel(Fn03TSK_SBARModel model)
        {
            return DA.Excel(model);
        }
    }
}