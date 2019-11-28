using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingEformDataEntryService : ProcessingEformService
    {
        public int[] getOrderings(string formCode)
        {
            if (formCode.Equals(ProcessingConstant.FORM_01))
            {
                return new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_02))
            {
                return new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_03))
            {
                return new int[] { 0, 1 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_04))
            {
                return new int[] { 0, 1, 2 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_05))
            {
                return new int[] { 0, 1 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_06))
            {
                return new int[] { 0, 1, 2, 3 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_07))
            {
                return new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_08))
            {
                return new int[] { 0, 1, 2 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_09))
            {
                return new int[] { 0, 1 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_10))
            {
                return new int[] { 0, 1 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_11))
            {
                return new int[] { 0, 1, 2, 3 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_12))
            {
                return new int[] { 0 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_31))
            {
                return new int[] { 0 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_32))
            {
                return new int[] { 0 };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_33))
            {
                return new int[] { 0 };
            }

            return null;
        }

        public string[] getFormParts(string formCode)
        {
            string A = ProcessingConstant.PART_A;
            string B = ProcessingConstant.PART_B;
            string C = ProcessingConstant.PART_C;
            string D = ProcessingConstant.PART_D;
            string E = ProcessingConstant.PART_E;
            string F = ProcessingConstant.PART_F;

            if (formCode.Equals(ProcessingConstant.FORM_01))
            {
                return new string[] { A, A, A, A, B, C, D, E };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_02))
            {
                return new string[] { A, B, C, D, E, F, F, D };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_03))
            {
                return new string[] { A, B };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_04))
            {
                return new string[] { A, B, C };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_05))
            {
                return new string[] { A, B };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_06))
            {
                return new string[] { A, A, B, C };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_07))
            {
                return new string[] { A, A, A, A, B, C, D, E };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_08))
            {
                return new string[] { A, A, B };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_09))
            {
                return new string[] { A, B };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_10))
            {
                return new string[] { A, B };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_11))
            {
                return new string[] { A, A, A, B };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_12))
            {
                return new string[] { B };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_31))
            {
                return new string[] { A };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_32))
            {
                return new string[] { B };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_33))
            {
                return new string[] { A };
            }
            return null;
        }

        public string[] getIdentifyFlags(string formCode)
        {
            string AP = ProcessingConstant.AP;
            string PRC = ProcessingConstant.PRC;
            string RSE = ProcessingConstant.RSE;
            string RGE = ProcessingConstant.RGE;
            string OTHER = ProcessingConstant.OTHER;

            if (formCode.Equals(ProcessingConstant.FORM_01))
            {
                return new string[] { AP, RSE, RGE, PRC, AP, RSE, RGE, PRC };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_02))
            {
                return new string[] { AP, RSE, RGE, PRC, AP, PRC, PRC, PRC };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_03))
            {
                return new string[] { PRC, PRC };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_04))
            {
                return new string[] { PRC, PRC, OTHER };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_05))
            {
                return new string[] { PRC, PRC };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_06))
            {
                return new string[] { AP, PRC, AP, PRC };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_07))
            {
                return new string[] { OTHER, RSE, RGE, PRC, RSE, RGE, PRC, AP };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_08))
            {
                return new string[] { AP, AP, AP };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_09))
            {
                return new string[] { OTHER, OTHER };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_10))
            {
                return new string[] { PRC, AP };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_11))
            {
                return new string[] { AP, RSE, RGE, PRC };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_12))
            {
                return new string[] { PRC };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_31))
            {
                return new string[] { OTHER };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_32))
            {
                return new string[] { PRC };
            }
            else if (formCode.Equals(ProcessingConstant.FORM_33))
            {
                return new string[] { OTHER };
            }
            return null;
        }

        #region MW Form
        public P_MW_FORM setMwForm(P_MW_RECORD record, P_MW_FORM form)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);

                if (ProcessingConstant.FORM_04.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW04.Find(master.FORMCONTENTID);
                    if (detail.SOCHINAME != null)
                    {
                        form.INVOLVE_SIGNBOARD = ProcessingConstant.FLAG_Y;
                    }
                    else
                    {
                        form.INVOLVE_SIGNBOARD = ProcessingConstant.FLAG_N;
                    }
                }
                else if (ProcessingConstant.FORM_06.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW06.Find(master.FORMCONTENTID);
                    // A1
                    var mwitemas = db.P_EFMWU_TBL_MW06_ITEMAS.Where(x => x.MW6ID == detail.ID).FirstOrDefault();
                    if (mwitemas != null)
                    {
                        form.FORM06_A_1_INVOLVE = ProcessingConstant.FLAG_Y;
                        form.FORM06_A_5_COMPLETED_MENTION = ProcessingConstant.FLAG_Y;
                    }
                    else
                    {
                        form.FORM06_A_1_INVOLVE = ProcessingConstant.FLAG_N;
                        form.FORM06_A_5_COMPLETED_MENTION = ProcessingConstant.FLAG_N;
                    }
                    // A4
                    if (getTrueFalseFromOption(detail.APOPTION.ToString()))
                    {
                        form.FORM06_A_4_AP = ProcessingConstant.FLAG_Y;
                        form.FORM06_A_4_RI = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RSE = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RGBC = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RMWC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.RSEOPTION.ToString()))
                    {
                        form.FORM06_A_4_AP = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RI = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RSE = ProcessingConstant.FLAG_Y;
                        form.FORM06_A_4_RGBC = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RMWC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.RIOPTION.ToString()))
                    {
                        form.FORM06_A_4_AP = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RI = ProcessingConstant.FLAG_Y;
                        form.FORM06_A_4_RSE = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RGBC = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RMWC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.GBCOPTION.ToString()))
                    {
                        form.FORM06_A_4_AP = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RI = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RSE = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RGBC = ProcessingConstant.FLAG_Y;
                        form.FORM06_A_4_RMWC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.MWCOPTION.ToString()))
                    {
                        form.FORM06_A_4_AP = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RI = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RSE = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RGBC = ProcessingConstant.FLAG_N;
                        form.FORM06_A_4_RMWC = ProcessingConstant.FLAG_Y;
                    }
                    // A5
                    if (getTrueFalseFromOption(detail.ARRCORRADDOPTION.ToString()))
                    {
                        form.FORM06_A_5_IDENTICAL = ProcessingConstant.FLAG_Y;
                    }
                    else
                    {
                        form.FORM06_A_5_IDENTICAL = ProcessingConstant.FLAG_N;
                    }
                    if (detail.ARCCHINAME == null)
                    {
                        form.FORM06_A_5_IDENTICAL = ProcessingConstant.FLAG_Y;
                    }
                    else
                    {
                        form.FORM06_A_5_IDENTICAL = ProcessingConstant.FLAG_N;
                    }
                    // B1
                    if (getTrueFalseFromOption(detail.APCFMAPOPTION.ToString()))
                    {
                        form.FORM06_B_1_AP = ProcessingConstant.FLAG_Y;
                        form.FORM06_B_1_RI = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RSE = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RGBC = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RMWC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.APCFMRSEOPTION.ToString()))
                    {
                        form.FORM06_B_1_AP = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RI = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RSE = ProcessingConstant.FLAG_Y;
                        form.FORM06_B_1_RGBC = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RMWC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.APCFMRIOPTION.ToString()))
                    {
                        form.FORM06_B_1_AP = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RI = ProcessingConstant.FLAG_Y;
                        form.FORM06_B_1_RSE = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RGBC = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RMWC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.APCFMGBCOPTION.ToString()))
                    {
                        form.FORM06_B_1_AP = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RI = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RSE = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RGBC = ProcessingConstant.FLAG_Y;
                        form.FORM06_B_1_RMWC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.APCFMMWCOPTION.ToString()))
                    {
                        form.FORM06_B_1_AP = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RI = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RSE = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RGBC = ProcessingConstant.FLAG_N;
                        form.FORM06_B_1_RMWC = ProcessingConstant.FLAG_Y;
                    }
                }
                else if (ProcessingConstant.FORM_07.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW07.Find(master.FORMCONTENTID);
                    // s27
                    if (detail.APCHINAME != null)
                    {
                        form.FORM07_A_S27 = ProcessingConstant.FLAG_Y;
                    }
                    else
                    {
                        form.FORM07_A_S27 = ProcessingConstant.FLAG_N;
                    }
                }
                else if (ProcessingConstant.FORM_09.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW09.Find(master.FORMCONTENTID);
                    // s27
                    if (getTrueFalseFromOption(detail.APOPTION.ToString()))
                    {
                        form.FORM09_A_AP = ProcessingConstant.FLAG_Y;
                        form.FORM09_A_RI = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.RIOPTION.ToString()))
                    {
                        form.FORM09_A_AP = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RI = ProcessingConstant.FLAG_Y;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.RSEOPTION.ToString()))
                    {
                        form.FORM09_A_AP = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RI = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_Y;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.RGEOPTION.ToString()))
                    {
                        form.FORM09_A_AP = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RI = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_Y;
                    }
                    // reason
                    if (getTrueFalseFromOption(detail.ILLOPTION.ToString()))
                    {
                        form.FORM09_REASON_ILL = ProcessingConstant.FLAG_Y;
                        form.FORM09_REASON_ABSENCE = ProcessingConstant.FLAG_N;

                    }
                    else if (getTrueFalseFromOption(detail.TAOPTION.ToString()))
                    {
                        form.FORM09_REASON_ILL = ProcessingConstant.FLAG_N;
                        form.FORM09_REASON_ABSENCE = ProcessingConstant.FLAG_Y;
                    }
                    // unknown period
                    if (getTrueFalseFromOption(detail.PERIODUNKNOWNOPTION.ToString()))
                    {
                        form.FORM09_FURTHER_NOTICE = ProcessingConstant.FLAG_Y;
                    }
                    else
                    {
                        form.FORM09_FURTHER_NOTICE = ProcessingConstant.FLAG_N;
                    }

                }
                else if (ProcessingConstant.FORM_10.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW10.Find(master.FORMCONTENTID);
                    form.FORM10_B_DELIVERY_DATE = detail.DELIVERDATE.ToLongDateString();
                }
                else if (ProcessingConstant.FORM_11.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW11.Find(master.FORMCONTENTID);
                    // part E: prc
                    if (getTrueFalseFromOption(detail.SAMEDATEOPTION.ToString()))
                    {
                        form.FORM11_E_SAME_DATE = ProcessingConstant.FLAG_Y;
                        form.FORM11_E_NEW_DATE = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.STARTDATEOPTION.ToString()))
                    {
                        form.FORM11_E_SAME_DATE = ProcessingConstant.FLAG_N;
                        form.FORM11_E_NEW_DATE = ProcessingConstant.FLAG_Y;
                    }

                    // so
                    if (detail.SOCHINAME != null)
                    {
                        form.INVOLVE_SIGNBOARD = ProcessingConstant.FLAG_Y;
                    }

                    // oi
                    if (getTrueFalseFromOption(detail.COMOPTION.ToString()))
                    {

                    }
                }
                else if (ProcessingConstant.FORM_31.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW31.Find(master.FORMCONTENTID);
                    if (getTrueFalseFromOption(detail.APOPTION.ToString()))
                    {
                        form.FORM09_A_AP = ProcessingConstant.FLAG_Y;
                        form.FORM09_A_RI = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.RIOPTION.ToString()))
                    {
                        form.FORM09_A_AP = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RI = ProcessingConstant.FLAG_Y;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.RSEOPTION.ToString()))
                    {
                        form.FORM09_A_AP = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RI = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_Y;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.RGEOPTION.ToString()))
                    {
                        form.FORM09_A_AP = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RI = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_Y;
                    }
                }
                else if (ProcessingConstant.FORM_32.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW32.Find(master.FORMCONTENTID);
                    form.FORM32_A_NO_SIGNBOARD = detail.SIGNBOARDERECTEDNO.ToString();
                }
                else if (ProcessingConstant.FORM_33.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW33.Find(master.FORMCONTENTID);
                    // ap
                    if (getTrueFalseFromOption(detail.APOPTION.ToString()))
                    {
                        form.FORM33_AP = ProcessingConstant.FLAG_Y;
                        form.FORM33_RI = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_N;
                        form.FORM33_PRC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.RIOPTION.ToString()))
                    {
                        form.FORM33_AP = ProcessingConstant.FLAG_N;
                        form.FORM33_RI = ProcessingConstant.FLAG_Y;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_N;
                        form.FORM33_PRC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.RSEOPTION.ToString()))
                    {
                        form.FORM33_AP = ProcessingConstant.FLAG_N;
                        form.FORM33_RI = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_Y;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_N;
                        form.FORM33_PRC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.RGEOPTION.ToString()))
                    {
                        form.FORM33_AP = ProcessingConstant.FLAG_N;
                        form.FORM33_RI = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_Y;
                        form.FORM33_PRC = ProcessingConstant.FLAG_N;
                    }
                    else if (getTrueFalseFromOption(detail.PRCOPTION.ToString()))
                    {
                        form.FORM33_AP = ProcessingConstant.FLAG_N;
                        form.FORM33_RI = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RSE = ProcessingConstant.FLAG_N;
                        form.FORM09_A_RGE = ProcessingConstant.FLAG_N;
                        form.FORM33_PRC = ProcessingConstant.FLAG_Y;
                    }

                    // documents
                    form.FORM33_DEC_1 = ProcessingConstant.FLAG_N;
                    form.FORM33_DEC_2 = ProcessingConstant.FLAG_N;
                    form.FORM33_DEC_3 = ProcessingConstant.FLAG_N;
                    form.FORM33_DEC_4 = ProcessingConstant.FLAG_N;
                    form.FORM33_DEC_5 = ProcessingConstant.FLAG_N;
                    form.FORM33_DEC_6 = ProcessingConstant.FLAG_N;
                    form.FORM33_DEC_7 = ProcessingConstant.FLAG_N;
                    form.FORM33_DEC_8 = ProcessingConstant.FLAG_N;
                    form.FORM33_DEC_9 = ProcessingConstant.FLAG_N;
                    form.FORM33_DEC_10 = ProcessingConstant.FLAG_N;
                    form.FORM33_DEC_11 = ProcessingConstant.FLAG_N;
                    form.FORM33_DEC_12 = ProcessingConstant.FLAG_N;
                    if (getTrueFalseFromOption(detail.PHOTOPREMISEBEFOREOPTION.ToString()))
                    {
                        form.FORM33_DEC_1 = ProcessingConstant.FLAG_Y;
                    }
                    else if (getTrueFalseFromOption(detail.PHOTOALLCOMPLETEDOPTION.ToString()))
                    {
                        form.FORM33_DEC_2 = ProcessingConstant.FLAG_Y;
                    }
                    else if (getTrueFalseFromOption(detail.REVISEDPLANOPTION.ToString()))
                    {
                        form.FORM33_DEC_3 = ProcessingConstant.FLAG_Y;
                    }
                    else if (getTrueFalseFromOption(detail.STRUCCALOPTION.ToString()))
                    {
                        form.FORM33_DEC_4 = ProcessingConstant.FLAG_Y;
                    }
                    else if (getTrueFalseFromOption(detail.STRUCAPPRREPORTOPTION.ToString()))
                    {
                        form.FORM33_DEC_5 = ProcessingConstant.FLAG_Y;
                    }
                    else if (getTrueFalseFromOption(detail.GAROPTION.ToString()))
                    {
                        form.FORM33_DEC_6 = ProcessingConstant.FLAG_Y;
                    }
                    else if (getTrueFalseFromOption(detail.DEMOPROPOSALOPTION.ToString()))
                    {
                        form.FORM33_DEC_7 = ProcessingConstant.FLAG_Y;
                    }
                    else if (getTrueFalseFromOption(detail.TEMPSAFETYOPTION.ToString()))
                    {
                        form.FORM33_DEC_8 = ProcessingConstant.FLAG_Y;
                    }
                    else if (getTrueFalseFromOption(detail.CATALOGOPTION.ToString()))
                    {
                        form.FORM33_DEC_9 = ProcessingConstant.FLAG_Y;
                    }
                    else if (getTrueFalseFromOption(detail.MATERIALINVOPTION.ToString()))
                    {
                        form.FORM33_DEC_10 = ProcessingConstant.FLAG_Y;
                    }
                    else if (getTrueFalseFromOption(detail.SUPERVISIONPLANOPTION.ToString()))
                    {
                        form.FORM33_DEC_11 = ProcessingConstant.FLAG_Y;
                    }
                    else if (getTrueFalseFromOption(detail.OTHEROPTION.ToString()))
                    {
                        form.FORM33_DEC_12 = ProcessingConstant.FLAG_Y;
                    }
                }

                return form;
            }
        }

        public List<P_MW_FORM_09> createBlankMw09(P_MW_RECORD record, List<P_MW_FORM_09> forms)
        {
            int total = 27;
            for (int i = 0; i < total; i++)
            {
                P_MW_FORM_09 form = new P_MW_FORM_09();
                form.ORDERING = i;
                forms.Add(form);
            }
            return forms;
        }

        public List<P_MW_FORM_09> setMW09Form(P_MW_RECORD record, List<P_MW_FORM_09> forms)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);
                var detail = db.P_EFMWU_TBL_MW09.Find(master.FORMCONTENTID);
                var mwnums = db.P_EFMWU_TBL_MW09_ITEM.Where(x => x.MW9ID == detail.ID).ToList();
                for (int i = 0; i < mwnums.Count(); i++)
                {
                    forms[i].MW_NUMBER = mwnums[i].MWSUBMISSIONID;
                }
                return forms;
            }
        }
        #endregion

        #region Record (master)/P_MW_RECORD
        public P_MW_RECORD setMwRecord(P_MW_RECORD record)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);

                if (ProcessingConstant.FORM_01.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW01.Find(master.FORMCONTENTID);

                    record.LOCATION_OF_MINOR_WORK = detail.LOCATION;

                    if (record.COMMENCEMENT_DATE == null)
                    {
                        record.COMMENCEMENT_DATE = detail.COMMDATE;
                    }
                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_02.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW02.Find(master.FORMCONTENTID);

                    if (record.COMPLETION_DATE == null)
                    {
                        record.COMPLETION_DATE = detail.COMMENDATE;
                    }
                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_03.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW03.Find(master.FORMCONTENTID);

                    record.LOCATION_OF_MINOR_WORK = detail.MWLOCATION;

                    if (record.COMMENCEMENT_DATE == null)
                    {
                        record.COMMENCEMENT_DATE = detail.COMMENDATE;
                    }
                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_04.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW05.Find(master.FORMCONTENTID);

                    if (record.COMPLETION_DATE == null)
                    {
                        record.COMPLETION_DATE = detail.COMPLETIONDATE;
                    }
                    if (record.COMMENCEMENT_DATE == null)
                    {
                        record.COMMENCEMENT_DATE = detail.COMMENDATE;
                    }
                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_05.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW05.Find(master.FORMCONTENTID);

                    record.LOCATION_OF_MINOR_WORK = detail.MWLOCATION;

                    if (record.COMMENCEMENT_DATE == null)
                    {
                        record.COMMENCEMENT_DATE = detail.COMMENDATE;
                    }
                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_06.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW06.Find(master.FORMCONTENTID);

                    record.LOCATION_OF_MINOR_WORK = detail.MWLOCATION;

                    if (record.COMPLETION_DATE == null)
                    {
                        record.COMPLETION_DATE = detail.COMPLETEDATE;
                    }
                    if (record.COMMENCEMENT_DATE == null)
                    {
                        record.COMMENCEMENT_DATE = detail.COMMENDATE;
                    }
                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_07.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW07.Find(master.FORMCONTENTID);

                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_08.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW08.Find(master.FORMCONTENTID);

                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_09.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW09.Find(master.FORMCONTENTID);

                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_10.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW10.Find(master.FORMCONTENTID);

                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_11.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW11.Find(master.FORMCONTENTID);

                    if (record.COMMENCEMENT_DATE == null)
                    {
                        record.COMMENCEMENT_DATE = detail.COMMENDATE;
                    }
                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_12.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW12.Find(master.FORMCONTENTID);

                    if (record.COMMENCEMENT_DATE == null)
                    {
                        record.COMMENCEMENT_DATE = detail.COMMENDATE;
                    }
                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_31.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW31.Find(master.FORMCONTENTID);

                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_32.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW32.Find(master.FORMCONTENTID);

                    record.LOCATION_OF_MINOR_WORK = detail.MWLOCATION;
                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }
                else if (ProcessingConstant.FORM_33.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW33.Find(master.FORMCONTENTID);

                    if (record.LANGUAGE_CODE == null)
                    {
                        record.LANGUAGE_CODE = ProcessingConstant.LANG_ENGLISH;
                    }
                    if (record.FILEREF_FOUR == null)
                    {
                        record.FILEREF_FOUR = master.FOURPLUSTWO.Split('/')[0];
                    }
                    if (record.FILEREF_TWO == null)
                    {
                        record.FILEREF_TWO = master.FOURPLUSTWO.Split('/')[1];
                    }
                    if (record.FIRST_RECEIVED_DATE == null)
                    {
                        record.FIRST_RECEIVED_DATE = DateTime.Now;
                    }
                }

                return record;
            }
        }
        #endregion

        #region Address/P_MW_ADDRESS
        public P_MW_ADDRESS setMwAddress(P_MW_RECORD record, P_MW_ADDRESS address)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);

                if (ProcessingConstant.FORM_01.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW01.Find(master.FORMCONTENTID);

                    address.FLAT = detail.FLATROOM;
                    address.DISPLAY_FLAT = detail.FLATROOM;
                    address.FLOOR = detail.FLOOR;
                    address.DISPLAY_FLOOR = detail.FLOOR;
                    address.BUILDING_NAME = detail.BUILDING;
                    address.DISPLAY_BUILDINGNAME = detail.BUILDING;
                    address.STREET_NO = detail.STREETNO;
                    address.DISPLAY_STREET_NO = detail.STREETNO;
                    address.STREE_NAME = detail.STREETROADVILLAGE;
                    address.DISPLAY_STREET = detail.STREETROADVILLAGE;
                    address.DISTRICT = detail.DISTRICT;
                    address.DISPLAY_DISTRICT = detail.DISTRICT;
                    address.REGION = detail.DISTRICTOPTION;
                    address.DISPLAY_REGION = address.REGION;
                    if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                    {
                        address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                    else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                    {
                        address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                }
                else if (ProcessingConstant.FORM_03.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW03.Find(master.FORMCONTENTID);

                    address.FLAT = detail.FLAT;
                    address.DISPLAY_FLAT = detail.FLAT;
                    address.FLOOR = detail.FLOOR;
                    address.DISPLAY_FLOOR = detail.FLOOR;
                    address.BUILDING_NAME = detail.BUILDING;
                    address.DISPLAY_BUILDINGNAME = detail.BUILDING;
                    address.STREET_NO = detail.STREETNO;
                    address.DISPLAY_STREET_NO = detail.STREETNO;
                    address.STREE_NAME = detail.STREETROADVILLAGE;
                    address.DISPLAY_STREET = detail.STREETROADVILLAGE;
                    address.DISTRICT = detail.DISTRICT;
                    address.DISPLAY_DISTRICT = detail.DISTRICT;
                    address.REGION = detail.DISTRICTOPTION;
                    address.DISPLAY_REGION = address.REGION;
                    if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                    {
                        address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                    else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                    {
                        address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                }
                else if (ProcessingConstant.FORM_05.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW05.Find(master.FORMCONTENTID);

                    address.STREE_NAME = detail.STREETROADVILLAGE;
                    address.DISPLAY_STREET = detail.STREETROADVILLAGE;
                    address.STREET_NO = detail.STREETNO;
                    address.DISPLAY_STREET_NO = detail.STREETNO;
                    address.BUILDING_NAME = detail.BUILDING;
                    address.DISPLAY_BUILDINGNAME = detail.BUILDING;
                    address.FLOOR = detail.FLOOR;
                    address.DISPLAY_FLOOR = detail.FLOOR;
                    address.FLAT = detail.FLAT;
                    address.DISPLAY_FLAT = detail.FLAT;
                    address.DISTRICT = detail.DISTRICT;
                    address.DISPLAY_DISTRICT = detail.DISTRICT;
                    address.REGION = detail.DISTRICTOPTION;
                    address.DISPLAY_REGION = detail.DISTRICTOPTION;
                    if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                    {
                        address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                    else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                    {
                        address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                }
                else if (ProcessingConstant.FORM_06.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW06.Find(master.FORMCONTENTID);

                    address.STREE_NAME = detail.STREETROADVILLAGE;
                    address.DISPLAY_STREET = address.STREE_NAME;
                    address.STREET_NO = detail.STREETNO;
                    address.DISPLAY_STREET_NO = address.STREET_NO;
                    address.BUILDING_NAME = detail.BUILDING;
                    address.DISPLAY_BUILDINGNAME = address.BUILDING_NAME;
                    address.FLOOR = detail.FLOOR;
                    address.DISPLAY_FLOOR = address.FLOOR;
                    address.FLAT = detail.FLAT;
                    address.DISPLAY_FLAT = address.FLAT;
                    address.DISTRICT = detail.DISTRICT;
                    address.DISPLAY_DISTRICT = address.DISTRICT;
                    address.REGION = detail.DISTRICTOPTION;
                    address.DISPLAY_REGION = address.REGION;
                    if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                    {
                        address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                    else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                    {
                        address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                }
                else if (ProcessingConstant.FORM_32.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW32.Find(master.FORMCONTENTID);

                    address.STREE_NAME = detail.STREETROADVILLAGE;
                    address.DISPLAY_STREET = address.STREE_NAME;
                    address.STREET_NO = detail.STREETNO;
                    address.DISPLAY_STREET_NO = address.STREET_NO;
                    address.BUILDING_NAME = detail.BUILDING;
                    address.DISPLAY_BUILDINGNAME = address.BUILDING_NAME;
                    address.FLOOR = detail.FLOOR;
                    address.DISPLAY_FLOOR = address.FLOOR;
                    address.FLAT = detail.FLAT;
                    address.DISPLAY_FLAT = address.FLAT;
                    address.DISTRICT = detail.DISTRICT;
                    address.DISPLAY_DISTRICT = address.DISTRICT;
                    address.REGION = detail.DISTRICTOPTION;
                    address.DISPLAY_REGION = address.REGION;
                    if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                    {
                        address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                    else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                    {
                        address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                }

                return address;
            }
        }
        #endregion

        #region OI/Owners's Corporations/P_MW_ADDRESS/P_MW_PERSON_CONTACT (record: OWNER_ID)
        public P_MW_PERSON_CONTACT setOIInfo(P_MW_RECORD record, P_MW_PERSON_CONTACT person)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);

                if (ProcessingConstant.FORM_01.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW01.Find(master.FORMCONTENTID);

                    person.NAME_ENGLISH = detail.COMNAME;
                    person.FAX_NO = detail.COMFAX;
                    person.EMAIL = detail.COMEMAIL;
                    person.CONTACT_NO = detail.COMTEL;
                }
                else if (ProcessingConstant.FORM_03.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW03.Find(master.FORMCONTENTID);

                    person.NAME_ENGLISH = detail.COMNAME;
                    person.FAX_NO = detail.COMFAX;
                    person.EMAIL = detail.COMEMAIL;
                    person.CONTACT_NO = detail.COMTEL;
                }
                //else if (ProcessingConstant.FORM_11.Equals(record.S_FORM_TYPE_CODE))
                //{
                //    var detail = db.P_EFMWU_TBL_MW11.Find(master.FORMCONTENTID);

                //    person.NAME_ENGLISH = detail.COMNAME;
                //    person.FAX_NO = detail.COMFAX;
                //    person.EMAIL = detail.COMEMAIL;
                //    person.CONTACT_NO = detail.COMTEL;
                //}
                //else if (ProcessingConstant.FORM_12.Equals(record.S_FORM_TYPE_CODE))
                //{
                //    var detail = db.P_EFMWU_TBL_MW12.Find(master.FORMCONTENTID);

                //    person.NAME_ENGLISH = detail.COMNAME;
                //    person.FAX_NO = detail.COMFAX;
                //    person.EMAIL = detail.COMEMAIL;
                //    person.CONTACT_NO = detail.COMTEL;
                //}

                return person;
            }
        }

        public P_MW_ADDRESS setOIAddress(P_MW_RECORD record, P_MW_ADDRESS address)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);

                if (record.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01))
                {
                    var detail = db.P_EFMWU_TBL_MW01.Find(master.FORMCONTENTID);

                    address.FLAT = detail.COMROOM;
                    address.DISPLAY_FLAT = detail.COMROOM;
                    address.FLOOR = detail.COMFLOOR;
                    address.DISPLAY_FLOOR = detail.COMFLOOR;
                    address.BUILDING_NAME = detail.COMBUILDING;
                    address.DISPLAY_BUILDINGNAME = detail.COMBUILDING;
                    address.STREET_NO = detail.COMSTREET;
                    address.DISPLAY_STREET_NO = detail.COMSTREET;
                    address.STREE_NAME = detail.COMSRV;
                    address.DISPLAY_STREET = detail.COMSRV;
                    address.DISTRICT = detail.DISTRICT;
                    address.DISPLAY_DISTRICT = detail.DISTRICT;
                    address.REGION = detail.COMDISTRICTOPTION; // HK/KLN/NT
                    address.DISPLAY_REGION = address.REGION;
                    if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                    {
                        address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                    else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                    {
                        address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                }
                else if (record.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_03))
                {
                    var detail = db.P_EFMWU_TBL_MW03.Find(master.FORMCONTENTID);

                    address.STREE_NAME = detail.COMSRV;
                    address.DISPLAY_STREET = detail.COMSRV;
                    address.STREET_NO = detail.COMSTREETNO;
                    address.DISPLAY_STREET_NO = detail.COMSTREETNO;
                    address.BUILDING_NAME = detail.COMBUILDING;
                    address.DISPLAY_BUILDINGNAME = detail.COMBUILDING;
                    address.FLOOR = detail.COMFLOOR;
                    address.DISPLAY_FLOOR = detail.COMFLOOR;
                    address.FLAT = detail.COMROOM;
                    address.DISPLAY_FLAT = detail.COMROOM;
                    address.DISTRICT = detail.COMDISTRICT;
                    address.DISPLAY_DISTRICT = detail.COMDISTRICT;
                    address.REGION = detail.COMDISTRICTOPTION;
                    address.DISPLAY_REGION = detail.COMDISTRICTOPTION;
                    if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                    {
                        address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                    else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                    {
                        address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                    }
                }
                //else if(record.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_11))
                //{
                //    var detail = db.P_EFMWU_TBL_MW11.Find(master.FORMCONTENTID);

                //    address.STREE_NAME = detail.COMSRV;
                //    address.DISPLAY_STREET = address.STREE_NAME;
                //    address.STREET_NO = detail.COMSTREETNO;
                //    address.DISPLAY_STREET_NO = address.STREET_NO;
                //    address.BUILDING_NAME = detail.COMBUILDING;
                //    address.DISPLAY_BUILDINGNAME = address.BUILDING_NAME;
                //    address.FLOOR = detail.COMFLOOR;
                //    address.DISPLAY_FLOOR = address.FLOOR;
                //    address.FLAT = detail.COMROOM;
                //    address.DISPLAY_FLAT = address.FLAT;
                //    address.DISTRICT = detail.COMDISTRICT;
                //    address.DISPLAY_DISTRICT = address.DISTRICT;
                //    address.REGION = detail.COMDISTRICTOPTION;
                //    address.DISPLAY_REGION = address.REGION;
                //    if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                //    {
                //        address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                //    }
                //    else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                //    {
                //        address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                //    }
                //}
                //else if(record.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_12))
                //{
                //    var detail = db.P_EFMWU_TBL_MW12.Find(master.FORMCONTENTID);

                //    address.STREE_NAME = detail.COMSRV;
                //    address.DISPLAY_STREET = address.STREE_NAME;
                //    address.STREET_NO = detail.COMSTREETNO;
                //    address.DISPLAY_STREET_NO = address.STREET_NO;
                //    address.BUILDING_NAME = detail.COMBUILDING;
                //    address.DISPLAY_BUILDINGNAME = address.BUILDING_NAME;
                //    address.FLOOR = detail.COMFLOOR;
                //    address.DISPLAY_FLOOR = address.FLOOR;
                //    address.FLAT = detail.COMROOM;
                //    address.DISPLAY_FLAT = address.FLAT;
                //    address.DISTRICT = detail.COMDISTRICT;
                //    address.DISPLAY_DISTRICT = address.DISTRICT;
                //    address.REGION = detail.COMDISTRICTOPTION;
                //    address.DISPLAY_REGION = address.REGION;
                //    if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                //    {
                //        address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                //    }
                //    else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                //    {
                //        address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                //    }
                //}

                return address;
            }
        }
        #endregion

        #region SO/Signboard Owner/Signboard Performer/P_MW_ADDRESS/P_MW_PERSON_CONTACT
        public P_MW_PERSON_CONTACT setSignboardOnwerInfo(P_MW_RECORD record, P_MW_PERSON_CONTACT person)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);

                if (ProcessingConstant.FORM_01.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW01.Find(master.FORMCONTENTID);

                    if (getTrueFalseFromOption(detail.SOCORRADDRESS.ToString())) // same as A1
                    {
                        person.ADDRESS_SAME_A1 = ProcessingConstant.FLAG_Y;
                    }
                    if (getTrueFalseFromOption(detail.SOHKIDOPTION.ToString()))
                    {
                        person.ID_TYPE = "1";
                        person.ID_NUMBER = detail.SOHKIDNUM;
                    }
                    if (getTrueFalseFromOption(detail.SOPASSPORTOPTION.ToString()))
                    {
                        person.ID_TYPE = "2";
                        person.ID_ISSUE_COUNTRY = detail.SOPASSPORTISSUECNTRY;
                        person.ID_NUMBER = detail.SOPASSPORT;
                    }
                    if (getTrueFalseFromOption(detail.SOBRNOPTION.ToString()))
                    {
                        person.ID_TYPE = "3";
                        person.ID_NUMBER = detail.SOBRN;
                    }
                    if (getTrueFalseFromOption(detail.SOOTHEROPTION.ToString()))
                    {
                        person.ID_TYPE = "4";
                        person.OTHER_ID_TYPE = detail.SOOTHERDESC;
                        person.ID_NUMBER = detail.SOOTHERNO;
                    }
                    //person.LAST_NAME = detail.SOENGNAME1;
                    //person.FIRST_NAME = detail.SOENGNAME2;
                    person.NAME_ENGLISH = detail.SOENGNAME1 + SPACE + detail.SOENGNAME2;
                    person.NAME_CHINESE = detail.SOCHINAME;
                    person.FAX_NO = detail.SOFAX;
                    person.CONTACT_NO = detail.SOTEL;
                    person.EMAIL = detail.SOEMAIL;
                    person.SIGNATURE_DATE = detail.SOSIGNDATE;
                }
                else if ((ProcessingConstant.FORM_02.Equals(record.S_FORM_TYPE_CODE)))
                {
                    var detail = db.P_EFMWU_TBL_MW02.Find(master.FORMCONTENTID);
                    if (!getTrueFalseFromOption(detail.SOOPTION.ToString()) && getTrueFalseFromOption(detail.SOOPTIONN.ToString())) // !provided == true && not provided == true
                    {
                        person.NAME_CHINESE = detail.SOCHINAME;
                        person.NAME_ENGLISH = detail.SOENGNAME1 + SPACE + detail.SOENGNAME2;
                        if (getTrueFalseFromOption(detail.SOHKIDOPTION.ToString()))
                        {
                            person.ID_TYPE = "1";
                            person.ID_NUMBER = detail.SOHKIDNUM;
                        }
                        if (getTrueFalseFromOption(detail.SOPNOPTION.ToString()))
                        {
                            person.ID_TYPE = "2";
                            person.ID_ISSUE_COUNTRY = detail.SOPASSPORTISSUECNTRY;
                            person.ID_NUMBER = detail.SOPN;
                        }
                        if (getTrueFalseFromOption(detail.SOBRNOPTION.ToString()))
                        {
                            person.ID_TYPE = "3";
                            person.ID_NUMBER = detail.SOBRN;
                        }
                        if (getTrueFalseFromOption(detail.SOOTHEROPTION.ToString()))
                        {
                            person.ID_TYPE = "4";
                            person.OTHER_ID_TYPE = detail.SOOTHERDESC;
                            person.ID_NUMBER = detail.SOOTHERNO;
                        }
                        person.FAX_NO = detail.SOFAX;
                        person.CONTACT_NO = detail.SOTEL;
                        person.EMAIL = detail.SOEMAIL;
                        person.SIGNATURE_DATE = detail.SOSIGNDATE;
                    }
                }
                else if (ProcessingConstant.FORM_03.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW03.Find(master.FORMCONTENTID);

                    person.NAME_CHINESE = detail.SOCHINAME;
                    person.NAME_ENGLISH = detail.SOENGNAME1 + SPACE + detail.SOENGNAME2;
                    if (getTrueFalseFromOption(detail.SOHKIDOPTION.ToString()))
                    {
                        person.ID_TYPE = "1";
                        person.ID_NUMBER = detail.SOID;
                    }
                    if (getTrueFalseFromOption(detail.SOPNOPTION.ToString()))
                    {
                        person.ID_TYPE = "2";
                        person.ID_ISSUE_COUNTRY = detail.SOPASSPORTISSUECNTRY;
                        person.ID_NUMBER = detail.SOPN;
                    }
                    if (getTrueFalseFromOption(detail.SOBRNOPTION.ToString()))
                    {
                        person.ID_TYPE = "3";
                        person.ID_NUMBER = detail.SOBRN;
                    }
                    if (getTrueFalseFromOption(detail.SOOTHEROPTION.ToString()))
                    {
                        person.ID_TYPE = "4";
                        person.OTHER_ID_TYPE = detail.SOOTHERDESC;
                        person.ID_NUMBER = detail.SOOTHERNO;
                    }
                    if (getTrueFalseFromOption(detail.SOCORRADD.ToString())) // same as A1
                    {
                        person.ADDRESS_SAME_A1 = ProcessingConstant.FLAG_Y;
                    }
                    person.FAX_NO = detail.SOFAX;
                    person.CONTACT_NO = detail.SOTEL;
                    person.EMAIL = detail.SOEMAIL;
                    person.SIGNATURE_DATE = detail.SOSIGNDATE;
                }
                else if (ProcessingConstant.FORM_04.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW04.Find(master.FORMCONTENTID);
                    if (!getTrueFalseFromOption(detail.SOOPTION.ToString()) && getTrueFalseFromOption(detail.SOOPTIONN.ToString())) // !provided == true && not provided == true
                    {
                        person.NAME_CHINESE = detail.SOCHINAME;
                        person.NAME_ENGLISH = detail.SOENGNAME1 + SPACE + detail.SOENGNAME2;
                        if (getTrueFalseFromOption(detail.SOHKIDOPTION.ToString()))
                        {
                            person.ID_TYPE = "1";
                            person.ID_NUMBER = detail.SOHKIDNUM;
                        }
                        if (getTrueFalseFromOption(detail.SOPNOPTION.ToString()))
                        {
                            person.ID_TYPE = "2";
                            person.ID_ISSUE_COUNTRY = detail.SOPASSPORTISSUECNTRY;
                            person.ID_NUMBER = detail.SOPN;
                        }
                        if (getTrueFalseFromOption(detail.SOBRNOPTION.ToString()))
                        {
                            person.ID_TYPE = "3";
                            person.ID_NUMBER = detail.SOBRN;
                        }
                        if (getTrueFalseFromOption(detail.SOOTHEROPTION.ToString()))
                        {
                            person.ID_TYPE = "4";
                            person.OTHER_ID_TYPE = detail.SOOTHERDESC;
                            person.ID_NUMBER = detail.SOOTHERNO;
                        }
                        person.FAX_NO = detail.SOFAX;
                        person.CONTACT_NO = detail.SOTEL;
                        person.EMAIL = detail.SOEMAIL;
                        person.SIGNATURE_DATE = detail.SOSIGNDATE;
                    }
                }
                else if (ProcessingConstant.FORM_05.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW05.Find(master.FORMCONTENTID);

                    person.NAME_CHINESE = detail.SOCHINAME;
                    person.NAME_ENGLISH = detail.SOENGNAME1 + SPACE + detail.SOENGNAME2;
                    if (getTrueFalseFromOption(detail.SOHKIDOPTION.ToString()))
                    {
                        person.ID_TYPE = "1";
                        person.ID_NUMBER = detail.SOHKIDNUM;
                    }
                    if (getTrueFalseFromOption(detail.SOPNOPTION.ToString()))
                    {
                        person.ID_TYPE = "2";
                        person.ID_ISSUE_COUNTRY = detail.SOPASSPORTISSUECNTRY;
                        person.ID_NUMBER = detail.SOPN;
                    }
                    if (getTrueFalseFromOption(detail.SOBRNOPTION.ToString()))
                    {
                        person.ID_TYPE = "3";
                        person.ID_NUMBER = detail.SOBRN;
                    }
                    if (getTrueFalseFromOption(detail.SOOTHEROPTION.ToString()))
                    {
                        person.ID_TYPE = "4";
                        person.OTHER_ID_TYPE = detail.SOOTHERDESC;
                        person.ID_NUMBER = detail.SOOTHERNO;
                    }
                    person.FAX_NO = detail.SOFAX;
                    person.CONTACT_NO = detail.SOTEL;
                    person.EMAIL = detail.SOEMAIL;
                    person.SIGNATURE_DATE = detail.SOSIGNDATE;
                }
                else if (ProcessingConstant.FORM_11.Equals(record.S_FORM_TYPE_CODE))
                {

                    var detail = db.P_EFMWU_TBL_MW11.Find(master.FORMCONTENTID);
                    if (!getTrueFalseFromOption(detail.SOOPTION.ToString()) && getTrueFalseFromOption(detail.SOOPTIONN.ToString())) // !provided == true && not provided == true
                    {
                        person.NAME_CHINESE = detail.SOCHINAME;
                        person.NAME_ENGLISH = detail.SOENGNAME1 + SPACE + detail.SOENGNAME2;
                        if (getTrueFalseFromOption(detail.SOHKIDOPTION.ToString()))
                        {
                            person.ID_TYPE = "1";
                            person.ID_NUMBER = detail.SOHKIDNUM;
                        }
                        if (getTrueFalseFromOption(detail.SOPNOPTION.ToString()))
                        {
                            person.ID_TYPE = "2";
                            person.ID_ISSUE_COUNTRY = detail.SOPASSPORTISSUECNTRY;
                            person.ID_NUMBER = detail.SOPN;
                        }
                        if (getTrueFalseFromOption(detail.SOBRNOPTION.ToString()))
                        {
                            person.ID_TYPE = "3";
                            person.ID_NUMBER = detail.SOBRN;
                        }
                        if (getTrueFalseFromOption(detail.SOOTHEROPTION.ToString()))
                        {
                            person.ID_TYPE = "4";
                            person.OTHER_ID_TYPE = detail.SOOTHERDESC;
                            person.ID_NUMBER = detail.SOOTHERNO;
                        }
                        person.FAX_NO = detail.SOFAX;
                        person.CONTACT_NO = detail.SOTEL;
                        person.EMAIL = detail.SOEMAIL;
                        person.SIGNATURE_DATE = detail.SOSIGNDATE;
                    }
                }
                else if (ProcessingConstant.FORM_12.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW12.Find(master.FORMCONTENTID);
                    if (!getTrueFalseFromOption(detail.SOOPTION.ToString()) && getTrueFalseFromOption(detail.SOOPTIONN.ToString())) // !provided == true && not provided == true
                    {
                        person.NAME_CHINESE = detail.SOCHINAME;
                        person.NAME_ENGLISH = detail.SOENGNAME1 + SPACE + detail.SOENGNAME2;
                        if (getTrueFalseFromOption(detail.SOHKIDOPTION.ToString()))
                        {
                            person.ID_TYPE = "1";
                            person.ID_NUMBER = detail.SOHKIDNUM;
                        }
                        if (getTrueFalseFromOption(detail.SOPNOPTION.ToString()))
                        {
                            person.ID_TYPE = "2";
                            person.ID_ISSUE_COUNTRY = detail.SOPASSPORTISSUECNTRY;
                            person.ID_NUMBER = detail.SOPN;
                        }
                        if (getTrueFalseFromOption(detail.SOBRNOPTION.ToString()))
                        {
                            person.ID_TYPE = "3";
                            person.ID_NUMBER = detail.SOBRN;
                        }
                        if (getTrueFalseFromOption(detail.SOOTHEROPTION.ToString()))
                        {
                            person.ID_TYPE = "4";
                            person.OTHER_ID_TYPE = detail.SOOTHERDESC;
                            person.ID_NUMBER = detail.SOOTHERNO;
                        }
                        person.FAX_NO = detail.SOFAX;
                        person.CONTACT_NO = detail.SOTEL;
                        person.EMAIL = detail.SOEMAIL;
                        person.SIGNATURE_DATE = detail.SOSIGNDATE;
                    }
                }
                else if (ProcessingConstant.FORM_32.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW32.Find(master.FORMCONTENTID);
                    person.NAME_CHINESE = detail.SOCHINAME;
                    person.NAME_ENGLISH = detail.SOENGNAME1 + SPACE + detail.SOENGNAME2;
                }
                return person;
            }
        }

        public P_MW_ADDRESS setSignboardOwnerAddress(P_MW_RECORD record, P_MW_ADDRESS address)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);

                if (ProcessingConstant.FORM_01.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW01.Find(master.FORMCONTENTID);

                    if (!getTrueFalseFromOption(detail.SOCORRADDRESS.ToString())) // not same as A1
                    {
                        address.FLAT = detail.SOROOM;
                        address.DISPLAY_FLAT = detail.SOROOM;
                        address.FLOOR = detail.SOFLOOR;
                        address.DISPLAY_FLOOR = detail.SOFLOOR;
                        address.BUILDING_NAME = detail.SOBUILDING;
                        address.DISPLAY_BUILDINGNAME = detail.SOBUILDING;
                        address.STREET_NO = detail.SOSTREETNUM;
                        address.DISPLAY_STREET_NO = detail.SOSTREETNUM;
                        address.STREE_NAME = detail.SOSRV;
                        address.DISPLAY_STREET = detail.SOSRV;
                        address.DISTRICT = detail.SODISTRICT;
                        address.DISPLAY_DISTRICT = detail.SODISTRICT;
                        address.REGION = detail.SODISTRICTNO;
                        address.DISPLAY_REGION = address.REGION;
                        if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                        {
                            address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                        else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                        {
                            address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                    }
                }
                else if (ProcessingConstant.FORM_02.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW02.Find(master.FORMCONTENTID);
                    if (!getTrueFalseFromOption(detail.SOOPTION.ToString()) && getTrueFalseFromOption(detail.SOOPTIONN.ToString())) // !provided == true && not provided == true
                    {
                        address.FLAT = detail.SOROOM;
                        address.DISPLAY_FLAT = detail.SOROOM;
                        address.FLOOR = detail.SOFLOOR;
                        address.DISPLAY_FLOOR = detail.SOFLOOR;
                        address.BUILDING_NAME = detail.SOBUILDING;
                        address.DISPLAY_BUILDINGNAME = detail.SOBUILDING;
                        address.STREET_NO = detail.SOSTREETNO;
                        address.DISPLAY_STREET_NO = detail.SOSTREETNO;
                        address.STREE_NAME = detail.SOSRVADDRESS;
                        address.DISPLAY_STREET = detail.SOSRVADDRESS;
                        address.DISTRICT = detail.SODISTRICT;
                        address.DISPLAY_DISTRICT = detail.SODISTRICT;
                        address.REGION = detail.SODISTRICTOPTION;
                        address.DISPLAY_REGION = address.REGION;
                        if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                        {
                            address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                        else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                        {
                            address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                    }
                }
                else if (ProcessingConstant.FORM_03.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW03.Find(master.FORMCONTENTID);

                    if (!getTrueFalseFromOption(detail.SOCORRADD.ToString())) // not same as A1
                    {
                        address.STREE_NAME = detail.SOCORRSRV;
                        address.DISPLAY_STREET = detail.SOCORRSRV;
                        address.STREET_NO = detail.SOCORRSTREETNO;
                        address.DISPLAY_STREET_NO = detail.SOCORRSTREETNO;
                        address.BUILDING_NAME = detail.SOCORRBUILDING;
                        address.DISPLAY_BUILDINGNAME = detail.SOCORRBUILDING;
                        address.FLOOR = detail.SOCORRFLOOR;
                        address.DISPLAY_FLOOR = detail.SOCORRFLOOR;
                        address.FLAT = detail.SOCORRROOM;
                        address.DISPLAY_FLAT = detail.SOCORRROOM;
                        address.DISTRICT = detail.SOCORRDISTRICT;
                        address.DISPLAY_DISTRICT = detail.SOCORRDISTRICT;
                        address.REGION = detail.SOCORRDISTRICTOPTION;
                        address.DISPLAY_REGION = address.REGION;
                        if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                        {
                            address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                        else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                        {
                            address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                    }
                }
                else if (ProcessingConstant.FORM_04.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW04.Find(master.FORMCONTENTID);
                    if (!getTrueFalseFromOption(detail.SOOPTION.ToString()) && getTrueFalseFromOption(detail.SOOPTIONN.ToString())) // !provided == true && not provided == true
                    {
                        address.FLAT = detail.SOCORRROOM;
                        address.DISPLAY_FLAT = detail.SOCORRROOM;
                        address.FLOOR = detail.SOCORRFLOOR;
                        address.DISPLAY_FLOOR = detail.SOCORRFLOOR;
                        address.BUILDING_NAME = detail.SOCORRBUILDING;
                        address.DISPLAY_BUILDINGNAME = detail.SOCORRBUILDING;
                        address.STREET_NO = detail.SOCORRSTREETNO;
                        address.DISPLAY_STREET_NO = detail.SOCORRSTREETNO;
                        address.STREE_NAME = detail.SOCORRSRV;
                        address.DISPLAY_STREET = detail.SOCORRSRV;
                        address.DISTRICT = detail.SOCORRDISTRICT;
                        address.DISPLAY_DISTRICT = detail.SOCORRDISTRICT;
                        address.REGION = detail.SOCORRDISTRICTOPTION;
                        address.DISPLAY_REGION = address.REGION;
                        if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                        {
                            address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                        else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                        {
                            address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                    }
                }
                else if (ProcessingConstant.FORM_05.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW05.Find(master.FORMCONTENTID);
                    if (!getTrueFalseFromOption(detail.SOCORRADDOPTION.ToString())) // not same as A1
                    {
                        address.STREE_NAME = detail.SOCORRSRV;
                        address.DISPLAY_STREET = address.STREE_NAME;
                        address.STREET_NO = detail.SOCORRSTREETNO;
                        address.DISPLAY_STREET_NO = address.STREET_NO;
                        address.BUILDING_NAME = detail.SOCORRBUILDING;
                        address.DISPLAY_BUILDINGNAME = address.BUILDING_NAME;
                        address.FLOOR = detail.SOCORRFLOOR;
                        address.DISPLAY_FLOOR = address.FLOOR;
                        address.FLAT = detail.SOCORRROOM;
                        address.DISPLAY_FLAT = address.FLAT;
                        address.DISTRICT = detail.SOCORRDISTRICT;
                        address.DISPLAY_DISTRICT = address.DISTRICT;
                        address.REGION = detail.SOCORRDISTRICTOPTION;
                        address.DISPLAY_REGION = address.REGION;
                        if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                        {
                            address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                        else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                        {
                            address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                    }
                }
                else if (ProcessingConstant.FORM_11.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW11.Find(master.FORMCONTENTID);
                    if (!getTrueFalseFromOption(detail.SOOPTION.ToString()) && getTrueFalseFromOption(detail.SOOPTIONN.ToString())) // !provided == true && not provided == true
                    {
                        address.FLAT = detail.SOCORRROOM;
                        address.DISPLAY_FLAT = address.FLAT;
                        address.FLOOR = detail.SOCORRFLOOR;
                        address.DISPLAY_FLOOR = address.FLOOR;
                        address.BUILDING_NAME = detail.SOCORRBUILDING;
                        address.DISPLAY_BUILDINGNAME = address.BUILDING_NAME;
                        address.STREET_NO = detail.SOCORRSTREETNO;
                        address.DISPLAY_STREET_NO = address.STREET_NO;
                        address.STREE_NAME = detail.SOCORRSRV;
                        address.DISPLAY_STREET = address.STREE_NAME;
                        address.DISTRICT = detail.SOCORRDISTRICT;
                        address.DISPLAY_DISTRICT = address.DISTRICT;
                        address.REGION = detail.SOCORRDISTRICTOPTION;
                        address.DISPLAY_REGION = address.REGION;
                        if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                        {
                            address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                        else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                        {
                            address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                    }
                }
                else if (ProcessingConstant.FORM_12.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW12.Find(master.FORMCONTENTID);
                    if (!getTrueFalseFromOption(detail.SOOPTION.ToString()) && getTrueFalseFromOption(detail.SOOPTIONN.ToString())) // !provided == true && not provided == true
                    {
                        address.FLAT = detail.SOCORRROOM;
                        address.DISPLAY_FLAT = address.FLAT;
                        address.FLOOR = detail.SOCORRFLOOR;
                        address.DISPLAY_FLOOR = address.FLOOR;
                        address.BUILDING_NAME = detail.SOCORRBUILDING;
                        address.DISPLAY_BUILDINGNAME = address.BUILDING_NAME;
                        address.STREET_NO = detail.SOCORRSTREETNO;
                        address.DISPLAY_STREET_NO = address.STREET_NO;
                        address.STREE_NAME = detail.SOCORRSRV;
                        address.DISPLAY_STREET = address.STREE_NAME;
                        address.DISTRICT = detail.SOCORRDISTRICT;
                        address.DISPLAY_DISTRICT = address.DISTRICT;
                        address.REGION = detail.SOCORRDISTRICTOPTION;
                        address.DISPLAY_REGION = address.REGION;
                        if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                        {
                            address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                        else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                        {
                            address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                    }
                }

                return address;
            }
        }
        #endregion

        #region PAW/Owner/P_MW_ADDRESS/P_MW_PERSON_CONTACT
        public P_MW_PERSON_CONTACT setOwnerInfo(P_MW_RECORD record, P_MW_PERSON_CONTACT person) // paw
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);

                if (ProcessingConstant.FORM_01.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW01.Find(master.FORMCONTENTID);

                    if (getTrueFalseFromOption(detail.HKIDOPTION.ToString()))
                    {
                        person.ID_TYPE = "1";
                        person.ID_NUMBER = detail.HKIDNUM;
                    }
                    if (getTrueFalseFromOption(detail.PASSPORTOPTION.ToString()))
                    {
                        person.ID_TYPE = "2";
                        person.ID_ISSUE_COUNTRY = detail.PASSPORTISSUECNTRY;
                        person.ID_NUMBER = detail.PASSPORT;
                    }
                    if (getTrueFalseFromOption(detail.BRNOPTION.ToString()))
                    {
                        person.ID_TYPE = "3";
                        person.ID_NUMBER = detail.BRN;
                    }
                    if (getTrueFalseFromOption(detail.OTHEROPTION.ToString()))
                    {
                        person.ID_TYPE = "4";
                        person.OTHER_ID_TYPE = detail.OTHERDESC;
                        person.ID_NUMBER = detail.OTHERNO;
                    }
                    person.NAME_CHINESE = detail.MWCHINAME;
                    person.NAME_ENGLISH = detail.MWENGNAME1 + SPACE + detail.MWENGNAME2;
                    person.FAX_NO = detail.ARRFAX;
                    person.CONTACT_NO = detail.ARRTEL;
                    person.EMAIL = detail.EMAIL;
                    if (getTrueFalseFromOption(detail.CORRADDOPTION.ToString())) // same as A1
                    {
                        person.ADDRESS_SAME_A1 = ProcessingConstant.FLAG_Y;
                    }
                    else
                    {
                        person.ADDRESS_SAME_A1 = ProcessingConstant.FLAG_N;
                    }
                    person.SIGNATURE_DATE = detail.ARRSIGNDATE;
                }
                else if (ProcessingConstant.FORM_03.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW03.Find(master.FORMCONTENTID);

                    person.NAME_CHINESE = detail.MWCHINAME;
                    person.NAME_ENGLISH = detail.MWENGNAME1 + SPACE + detail.MWENGNAME2;
                    if (getTrueFalseFromOption(detail.MWHKIDOPTION.ToString()))
                    {
                        person.ID_TYPE = "1";
                        person.ID_NUMBER = detail.MWID;
                    }
                    if (getTrueFalseFromOption(detail.MWPNOPTION.ToString()))
                    {
                        person.ID_TYPE = "2";
                        person.ID_ISSUE_COUNTRY = detail.MWPASSPORTISSUECNTRY;
                        person.ID_NUMBER = detail.MWPN;
                    }
                    if (getTrueFalseFromOption(detail.MWBRNOPTION.ToString()))
                    {
                        person.ID_TYPE = "3";
                        person.ID_NUMBER = detail.MWBRN;
                    }
                    if (getTrueFalseFromOption(detail.MWOTHEROPTION.ToString()))
                    {
                        person.ID_TYPE = "4";
                        person.OTHER_ID_TYPE = detail.MWOTHERDESC;
                        person.ID_NUMBER = detail.MWOTHERNO;
                    }
                    if (getTrueFalseFromOption(detail.MWCORRADD.ToString())) // same as A1
                    {
                        person.ADDRESS_SAME_A1 = ProcessingConstant.FLAG_Y;
                    }
                    else
                    {
                        person.ADDRESS_SAME_A1 = ProcessingConstant.FLAG_N;
                    }
                    person.FAX_NO = detail.MWFAX;
                    person.EMAIL = detail.MWEMAIL;
                    person.CONTACT_NO = detail.MWTEL;
                    person.SIGNATURE_DATE = detail.MWSIGNDATE;
                }
                else if (ProcessingConstant.FORM_05.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW05.Find(master.FORMCONTENTID);

                    person.NAME_CHINESE = detail.MWCHINAME;
                    person.NAME_ENGLISH = detail.MWENGNAME1 + SPACE + detail.MWENGNAME2;
                    if (getTrueFalseFromOption(detail.MWHKIDOPTION.ToString()))
                    {
                        person.ID_TYPE = "1";
                        person.ID_NUMBER = detail.MWHKIDNUM;
                    }
                    if (getTrueFalseFromOption(detail.MWPNOPTION.ToString()))
                    {
                        person.ID_TYPE = "2";
                        person.ID_ISSUE_COUNTRY = detail.MWPASSPORTISSUECNTRY;
                        person.ID_NUMBER = detail.MWPN;
                    }
                    if (getTrueFalseFromOption(detail.MWBRNOPTION.ToString()))
                    {
                        person.ID_TYPE = "3";
                        person.ID_NUMBER = detail.MWBRN;
                    }
                    if (getTrueFalseFromOption(detail.MWOTHEROPTION.ToString()))
                    {
                        person.ID_TYPE = "4";
                        person.OTHER_ID_TYPE = detail.MWOTHERDESC;
                        person.ID_NUMBER = detail.MWOTHERNO;
                    }
                    if (getTrueFalseFromOption(detail.MWCORRADDOPTION.ToString())) // same as A1
                    {
                        person.ADDRESS_SAME_A1 = ProcessingConstant.FLAG_Y;
                    }
                    else
                    {
                        person.ADDRESS_SAME_A1 = ProcessingConstant.FLAG_N;
                    }
                    person.EMAIL = detail.EMAIL;
                    person.FAX_NO = detail.MWFAX;
                    person.CONTACT_NO = detail.MWTEL;
                    person.SIGNATURE_DATE = detail.MWSIGNDATE;
                }
                else if (ProcessingConstant.FORM_06.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW06.Find(master.FORMCONTENTID);

                    person.NAME_CHINESE = detail.ARRCHINAME;
                    person.NAME_ENGLISH = detail.ARRENGNAME1 + SPACE + detail.ARRENGNAME2;
                    if (getTrueFalseFromOption(detail.ARRHKIDOPTION.ToString()))
                    {
                        person.ID_TYPE = "1";
                        person.ID_NUMBER = detail.ARRHKIDNUM;
                    }
                    if (getTrueFalseFromOption(detail.ARRPNOPTION.ToString()))
                    {
                        person.ID_TYPE = "2";
                        person.ID_ISSUE_COUNTRY = detail.ARRPASSPORTISSUECNTRY;
                        person.ID_NUMBER = detail.ARRPN;
                    }
                    if (getTrueFalseFromOption(detail.ARRBRNOPTION.ToString()))
                    {
                        person.ID_TYPE = "3";
                        person.ID_NUMBER = detail.ARRBRN;
                    }
                    if (getTrueFalseFromOption(detail.ARROTHEROPTION.ToString()))
                    {
                        person.ID_TYPE = "4";
                        person.OTHER_ID_TYPE = detail.ARROTHERDESC;
                        person.ID_NUMBER = detail.ARROTHERNO;
                    }
                    if (getTrueFalseFromOption(detail.ARRCORRADDOPTION.ToString())) // same as A1
                    {
                        person.ADDRESS_SAME_A1 = ProcessingConstant.FLAG_Y;
                    }
                    else
                    {
                        person.ADDRESS_SAME_A1 = ProcessingConstant.FLAG_N;
                    }
                    person.EMAIL = detail.EMAIL;
                    person.FAX_NO = detail.ARRFAX;
                    person.CONTACT_NO = detail.ARRTEL;
                    person.SIGNATURE_DATE = detail.ARRSIGNDATE;

                }
                else if (ProcessingConstant.FORM_32.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW32.Find(master.FORMCONTENTID);
                    person.NAME_CHINESE = detail.ARRCHINAME;
                    person.NAME_ENGLISH = detail.ARRENGNAME1 + SPACE + detail.ARRENGNAME2;
                }

                return person;
            }
        }

        public P_MW_ADDRESS setOwnerAddress(P_MW_RECORD record, P_MW_ADDRESS address) // paw
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);

                if (record.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_01))
                {
                    var detail = db.P_EFMWU_TBL_MW01.Find(master.FORMCONTENTID);

                    if (!getTrueFalseFromOption(detail.CORRADDOPTION.ToString())) // not same as A1
                    {
                        address.FLAT = detail.CORRROOM;
                        address.DISPLAY_FLOOR = detail.CORRROOM;
                        address.FLOOR = detail.CORRFLOOR;
                        address.DISPLAY_FLOOR = detail.CORRFLOOR;
                        address.BUILDING_NAME = detail.CORRBUILDING;
                        address.DISPLAY_BUILDINGNAME = detail.CORRBUILDING;
                        address.STREET_NO = detail.CORRSTREETNO;
                        address.DISPLAY_STREET_NO = detail.CORRSTREETNO;
                        address.STREE_NAME = detail.CORRADDSTREET;
                        address.DISPLAY_STREET = detail.CORRADDSTREET;
                        address.DISTRICT = detail.CORRDISTRICT;
                        address.DISPLAY_DISTRICT = detail.CORRDISTRICT;
                        address.REGION = detail.CORRDISTRICTOPTION;
                        address.DISPLAY_REGION = address.REGION;
                        if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                        {
                            address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                        else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                        {
                            address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                    }
                }
                else if (record.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_03))
                {
                    var detail = db.P_EFMWU_TBL_MW03.Find(master.FORMCONTENTID);

                    if (!getTrueFalseFromOption(detail.MWCORRADD.ToString()))
                    {
                        address.STREE_NAME = detail.MWCORRSRV;
                        address.DISPLAY_STREET = detail.MWCORRSRV;
                        address.STREET_NO = detail.MWCORRSTREET;
                        address.DISPLAY_STREET_NO = detail.MWCORRSTREET;
                        address.BUILDING_NAME = detail.MWCORRBUILDING;
                        address.DISPLAY_BUILDINGNAME = detail.MWCORRBUILDING;
                        address.FLOOR = detail.MWCORRFLOOR;
                        address.DISPLAY_FLOOR = detail.MWCORRFLOOR;
                        address.FLAT = detail.MWCORRROOM;
                        address.DISPLAY_FLAT = detail.MWCORRROOM;
                        address.DISTRICT = detail.MWCORRDISTRICT;
                        address.DISPLAY_DISTRICT = detail.MWCORRDISTRICT;
                        address.REGION = detail.MWCORRDISTRICTOPTION;
                        address.DISPLAY_REGION = detail.MWCORRDISTRICTOPTION;
                        if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                        {
                            address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                        else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                        {
                            address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                    }
                }
                else if (record.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_05))
                {
                    var detail = db.P_EFMWU_TBL_MW05.Find(master.FORMCONTENTID);

                    if (!getTrueFalseFromOption(detail.MWCORRADDOPTION.ToString())) // not same as A1
                    {
                        address.FLAT = detail.MWCORRROOM;
                        address.DISPLAY_FLOOR = detail.MWCORRROOM;
                        address.FLOOR = detail.MWCORRFLOOR;
                        address.DISPLAY_FLOOR = detail.MWCORRFLOOR;
                        address.BUILDING_NAME = detail.MWCORRBUILDING;
                        address.DISPLAY_BUILDINGNAME = detail.MWCORRBUILDING;
                        address.STREET_NO = detail.MWCORRSTREETNO;
                        address.DISPLAY_STREET_NO = detail.MWCORRSTREETNO;
                        address.STREE_NAME = detail.MWCORRSRV;
                        address.DISPLAY_STREET = detail.MWCORRSRV;
                        address.DISTRICT = detail.MWCORRDISTRICT;
                        address.DISPLAY_DISTRICT = detail.MWCORRDISTRICT;
                        address.REGION = detail.MWCORRDISTRICTOPTION;
                        address.DISPLAY_REGION = address.REGION;
                        if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                        {
                            address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                        else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                        {
                            address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                    }
                }
                else if (record.S_FORM_TYPE_CODE.Equals(ProcessingConstant.FORM_06))
                {
                    var detail = db.P_EFMWU_TBL_MW06.Find(master.FORMCONTENTID);

                    if (!getTrueFalseFromOption(detail.ARRCORRADDOPTION.ToString())) // not same as A1
                    {
                        address.FLAT = detail.ARRCORRROOM;
                        address.DISPLAY_FLOOR = address.FLAT;
                        address.FLOOR = detail.ARRCORRFLOOR;
                        address.DISPLAY_FLOOR = address.FLOOR;
                        address.BUILDING_NAME = detail.ARRCORRBUILDING;
                        address.DISPLAY_BUILDINGNAME = address.BUILDING_NAME;
                        address.STREET_NO = detail.ARRCORRSTREETNO;
                        address.DISPLAY_STREET_NO = address.STREET_NO;
                        address.STREE_NAME = detail.ARRCORRSRV;
                        address.DISPLAY_STREET = address.STREE_NAME;
                        address.DISTRICT = detail.ARRCORRDISTRICT;
                        address.DISPLAY_DISTRICT = address.DISTRICT;
                        address.REGION = detail.ARRCORRDISTRICTOPTION;
                        address.DISPLAY_REGION = address.REGION;
                        if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_ENGLISH))
                        {
                            address.ENGLISH_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }
                        else if (record.LANGUAGE_CODE.Equals(ProcessingConstant.LANG_CHINESE))
                        {
                            address.CHINESE_DISPLAY = pc.getAddressDisplayFormat(address, record.LANGUAGE_CODE);
                        }

                    }
                }

                return address;
            }
        }
        #endregion

        #region MW items/P_MW_RECORD_ITEM
        public List<P_MW_RECORD_ITEM> saveMwItems(P_MW_RECORD record, List<P_MW_RECORD_ITEM> items)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);

                if (ProcessingConstant.FORM_01.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW01.Find(master.FORMCONTENTID);
                    var mwitems = db.P_EFMWU_TBL_MW01_ITEM.Where(x => x.MW1ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    foreach (var mwitem in mwitems)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREFNO;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                }
                else if (ProcessingConstant.FORM_02.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW02.Find(master.FORMCONTENTID);
                    var mwitems1 = db.P_EFMWU_TBL_MW02_ITEM1.Where(x => x.MW2ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    foreach (var mwitem in mwitems1)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWDESCDIFF;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                    var mwitems2 = db.P_EFMWU_TBL_MW02_ITEM2.Where(x => x.MW2ID == detail.ID).ToList();
                    foreach (var mwitem in mwitems2)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWDESCDIFF;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                    var mwitems3 = db.P_EFMWU_TBL_MW02_ITEM3.Where(x => x.MW2ID == detail.ID).ToList();
                    foreach (var mwitem in mwitems3)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREFNO;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                }
                else if (ProcessingConstant.FORM_03.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW03.Find(master.FORMCONTENTID);
                    var mwitems = db.P_EFMWU_TBL_MW03_ITEM.Where(x => x.MW3ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    foreach (var mwitem in mwitems)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREFNO;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }

                }
                else if (ProcessingConstant.FORM_04.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW04.Find(master.FORMCONTENTID);
                    var mwitems2 = db.P_EFMWU_TBL_MW04_ITEM2.Where(x => x.MW4ID == detail.ID).ToList();
                    foreach (var mwitem in mwitems2)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWDIFF;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                    var mwitems3 = db.P_EFMWU_TBL_MW04_ITEM3.Where(x => x.MW4ID == detail.ID).ToList();
                    foreach (var mwitem in mwitems3)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREF;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                }
                else if (ProcessingConstant.FORM_05.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW05.Find(master.FORMCONTENTID);
                    var mwitems = db.P_EFMWU_TBL_MW05_ITEM.Where(x => x.MW5ID == detail.ID).OrderBy(x => x.MWITEM).ToList();

                    foreach (var mwitem in mwitems)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREFNO;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                }
                else if (ProcessingConstant.FORM_06.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW06.Find(master.FORMCONTENTID);
                    var mwitems = db.P_EFMWU_TBL_MW06_ITEM.Where(x => x.MW6ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    foreach (var mwitem in mwitems)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREFNO;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                    var mwitemsas = db.P_EFMWU_TBL_MW06_ITEMAS.Where(x => x.MW6ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    foreach (var mwitem in mwitemsas)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREFNO;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }

                }
                else if (ProcessingConstant.FORM_10.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW10.Find(master.FORMCONTENTID);
                    var mwitems = db.P_EFMWU_TBL_MW10_ITEM.Where(x => x.MW10ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    foreach (var mwitem in mwitems)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREFNO;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                }
                else if (ProcessingConstant.FORM_11.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW11.Find(master.FORMCONTENTID);
                    var mwitems = db.P_EFMWU_TBL_MW11_ITEM.Where(x => x.MW11ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    foreach (var mwitem in mwitems)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREFNO;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                }
                else if (ProcessingConstant.FORM_12.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW12.Find(master.FORMCONTENTID);
                    var mwitems = db.P_EFMWU_TBL_MW12_ITEM.Where(x => x.MW12ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    foreach (var mwitem in mwitems)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREFNO;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                }
                else if (ProcessingConstant.FORM_31.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW31.Find(master.FORMCONTENTID);
                    var mwitems = db.P_EFMWU_TBL_MW31_ITEM.Where(x => x.MW31ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    foreach (var mwitem in mwitems)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREFNO;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                }
                else if (ProcessingConstant.FORM_32.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW32.Find(master.FORMCONTENTID);
                    var mwitems = db.P_EFMWU_TBL_MW32_ITEM.Where(x => x.MW32ID == detail.ID).OrderBy(x => x.MWITEM).ToList();
                    foreach (var mwitem in mwitems)
                    {
                        P_MW_RECORD_ITEM item = new P_MW_RECORD_ITEM();
                        item.MW_RECORD_ID = record.UUID;
                        item.MW_ITEM_CODE = mwitem.MWITEM;
                        item.LOCATION_DESCRIPTION = mwitem.MWDESC;
                        item.RELEVANT_REFERENCE = mwitem.MWREFNO;
                        item.CLASS_CODE = getClassCode(mwitem.MWITEM);

                        items.Add(item);
                    }
                }

                return items;
            }
        }
        #endregion

        #region PBP/PRC/Appointed Professional/P_MW_APPOINTED_PROFESSIONL
        public List<P_MW_APPOINTED_PROFESSIONAL> createBlankAP(P_MW_RECORD record, List<P_MW_APPOINTED_PROFESSIONAL> aps)
        {
            int[] orderings = getOrderings(record.S_FORM_TYPE_CODE);
            string[] formParts = getFormParts(record.S_FORM_TYPE_CODE);
            string[] identifyFlags = getIdentifyFlags(record.S_FORM_TYPE_CODE);

            for (int i = 0; i < orderings.Count(); i++)
            {
                P_MW_APPOINTED_PROFESSIONAL ap = new P_MW_APPOINTED_PROFESSIONAL();
                ap.ORDERING = orderings[i];
                ap.FORM_PART = formParts[i];
                ap.IDENTIFY_FLAG = identifyFlags[i];
                //ap.MW_RECORD_ID = record.UUID;
                aps.Add(ap);
            }
            return aps;
        }

        public List<P_MW_APPOINTED_PROFESSIONAL> setAppointedProfessional(P_MW_RECORD record, List<P_MW_APPOINTED_PROFESSIONAL> aps)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var master = getEfssRecordByDsn(record.MW_DSN);
                //Start modify by dive 20191011
                if (master == null) { master = new P_EFSS_FORM_MASTER(); }
                //End modify by dive 20191011
                if (ProcessingConstant.FORM_01.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW01.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW01(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0)
                        {
                            ap.CHINESE_NAME = detail.APRICHINAME;
                            ap.ENGLISH_NAME = detail.APRIENGNAME1 + SPACE + detail.APRIENGNAME2;
                            ap.CERTIFICATION_NO = detail.APRICERTNUM1 + SEPARATOR + detail.APRICERTNUM2;
                            //if (getTrueFalseFromOption(detail.AUTHPERSONOPTION.ToString()))
                            //{
                            //    ap.IDENTIFY_FLAG = ProcessingConstant.AP;
                            //} elsew
                            if (getTrueFalseFromOption(detail.REGINSPECOPTION.ToString()))
                            {
                                ap.IDENTIFY_FLAG = ProcessingConstant.RI;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_A;
                        }
                        else if (ap.ORDERING == 1)
                        {
                            if (getTrueFalseFromOption(detail.RSEOPTION.ToString()))
                            {
                                ap.CHINESE_NAME = detail.RSECHINAME;
                                ap.ENGLISH_NAME = detail.RSEENGNAME;
                                ap.CERTIFICATION_NO = detail.RSECERTNUM1 + SEPARATOR + detail.RSECERTNUM2;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RSE;
                        }
                        else if (ap.ORDERING == 2)
                        {
                            if (getTrueFalseFromOption(detail.RGEOPTION.ToString()))
                            {
                                ap.CHINESE_NAME = detail.RGECHINAME;
                                ap.ENGLISH_NAME = detail.RGEENGNAME;
                                ap.CERTIFICATION_NO = detail.RGECERTNUM1 + SEPARATOR + detail.RGECERTNUM2;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RGE;
                        }
                        else if (ap.ORDERING == 3)
                        {
                            ap.CHINESE_NAME = detail.APTPASCHINAME;
                            ap.ENGLISH_NAME = detail.APTPASENGNAME1 + SPACE + detail.APTPASENGNAME2;
                            ap.CERTIFICATION_NO = detail.APTPCERTNUM1 + SEPARATOR + detail.APTPCERTNUM2;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                        else if (ap.ORDERING == 4)
                        {
                            ap.CHINESE_NAME = detail.ARCFMCHINAME;
                            ap.ENGLISH_NAME = detail.ARCFMENGNAME1 + SPACE + detail.ARCFMENGNAME2;
                            ap.COMMENCED_DATE = detail.COMMDATE;
                            ap.FAX_NO = detail.ARCFMFAX;
                            ap.CONTACT_NO = detail.ARCFMTEL;
                            ap.CERTIFICATION_NO = detail.ARCFMCRNUM1 + SEPARATOR + detail.ARCFMCRNUM2;
                            ap.EXPIRY_DATE = detail.ARCFMREGEXPIREDATE;
                            ap.ENDORSEMENT_DATE = detail.APCFMSIGNDATE;
                            //if (getTrueFalseFromOption(detail.AUTHPERSONOPTION.ToString()))
                            //{
                            //    ap.IDENTIFY_FLAG = ProcessingConstant.AP;
                            //} else
                            if (getTrueFalseFromOption(detail.REGINSPECOPTION.ToString()))
                            {
                                ap.IDENTIFY_FLAG = ProcessingConstant.RI;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_B;
                        }
                        else if (ap.ORDERING == 5)
                        {
                            if (getTrueFalseFromOption(detail.RSEOPTION.ToString()))
                            {
                                ap.CHINESE_NAME = detail.RSECFMCHINAME;
                                ap.ENGLISH_NAME = detail.RSECFMENGNAME;
                                ap.CONTACT_NO = detail.RSECFMTEL;
                                ap.CERTIFICATION_NO = detail.RSECFMCRNUM1 + SEPARATOR + detail.RSECFMCRNUM2;
                                ap.EXPIRY_DATE = detail.RSECFMREGEXPIREDATE;
                                ap.ENDORSEMENT_DATE = detail.RSECFMSIGNDATE;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_C;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RSE;
                        }
                        else if (ap.ORDERING == 6)
                        {
                            if (getTrueFalseFromOption(detail.RGEOPTION.ToString()))
                            {
                                ap.CHINESE_NAME = detail.RGECHINAME;
                                ap.ENGLISH_NAME = detail.RGECFMENGNAME;
                                ap.CONTACT_NO = detail.RGECFMTEL;
                                ap.CERTIFICATION_NO = detail.RGECFMCRNUM1 + SEPARATOR + detail.RGECFMCRNUM2;
                                ap.EXPIRY_DATE = detail.RGECFMREGEXPIREDATE;
                                ap.ENDORSEMENT_DATE = detail.RGECFMSIGNDATE;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_D;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RGE;
                        }
                        else if (ap.ORDERING == 7)
                        {
                            ap.CHINESE_NAME = detail.RCCFMCHINAME;
                            ap.ENGLISH_NAME = detail.RCCFMENGNAME1 + SPACE + detail.RCCFMENGNAME2;
                            ap.CERTIFICATION_NO = detail.RCCFMCRNUM1 + SEPARATOR + detail.RCCFMCRNUM2;
                            ap.CHINESE_COMPANY_NAME = detail.ASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.ASENGNAME1 + SPACE + detail.ASENGNAME2;
                            ap.EXPIRY_DATE = detail.RCCFMREGEXPIREDATE;
                            ap.FAX_NO = detail.RCCFMFAX;
                            ap.CONTACT_NO = detail.RCCFMTEL;

                            ap.FORM_PART = ProcessingConstant.PART_E;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }

                    }
                }
                else if (ProcessingConstant.FORM_02.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW02.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW02(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // ap
                        {
                            ap.CHINESE_NAME = detail.APCHINAME;
                            ap.ENGLISH_NAME = detail.APENGNAME;
                            ap.COMPLETION_DATE = detail.COMMENDATE;
                            ap.CERTIFICATION_NO = detail.APCRNUM1 + SPACE + detail.APCRNUM2;
                            ap.EXPIRY_DATE = detail.APCREXPIREDATE;
                            ap.ENDORSEMENT_DATE = detail.APSIGNDATE;
                            ap.FAX_NO = detail.APFAX;
                            ap.CONTACT_NO = detail.APTEL;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                        }
                        else if (ap.ORDERING == 1) // rse
                        {
                            ap.CHINESE_NAME = detail.RSECHINAME;
                            ap.ENGLISH_NAME = detail.RSEENGNAME;
                            ap.CERTIFICATION_NO = detail.RSECRNUM1 + SEPARATOR + detail.RSECRNUM2;
                            ap.EXPIRY_DATE = detail.RSECREXPIREDATE;
                            ap.ENDORSEMENT_DATE = detail.RSESIGNDATE;
                            ap.CONTACT_NO = detail.RSETEL;

                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RSE;
                        }
                        else if (ap.ORDERING == 2) // rge
                        {
                            ap.CHINESE_NAME = detail.RGECHINAME;
                            ap.ENGLISH_NAME = detail.RGEENGNAME;
                            ap.CERTIFICATION_NO = detail.RGECRNUM1 + SEPARATOR + detail.RGECRNUM2;
                            ap.EXPIRY_DATE = detail.RGECREXPIREDATE;
                            ap.ENDORSEMENT_DATE = detail.RGESIGNDATE;
                            ap.CONTACT_NO = detail.RGETEL;

                            ap.FORM_PART = ProcessingConstant.PART_C;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RGE;
                        }
                        else if (ap.ORDERING == 3) // prc
                        {
                            ap.CHINESE_NAME = detail.RCCHINAME;
                            ap.ENGLISH_NAME = detail.RCENGNAME1 + SPACE + detail.RCENGNAME2;
                            ap.CERTIFICATION_NO = detail.RCCRNUM1 + SEPARATOR + detail.RCCRNUM2;
                            ap.CHINESE_COMPANY_NAME = detail.ASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.ASEHINAME;
                            ap.EXPIRY_DATE = detail.RCCREXPIREDDATE;
                            ap.ENDORSEMENT_DATE = detail.RCSIGNDATE;
                            ap.FAX_NO = detail.RCFAX;
                            ap.CONTACT_NO = detail.RCTEL;

                            ap.FORM_PART = ProcessingConstant.PART_D;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                        else if (ap.ORDERING == 4)
                        {
                            ap.FORM_PART = ProcessingConstant.PART_E;
                            ap.IDENTIFY_FLAG = aps[0].IDENTIFY_FLAG;
                        }
                        else if (ap.ORDERING == 5)
                        {
                            ap.FORM_PART = ProcessingConstant.PART_F;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                        else if (ap.ORDERING == 6) // prc
                        {
                            ap.CHINESE_NAME = detail.PRCCHINAME;
                            ap.ENGLISH_NAME = detail.PRCENGNAME1 + SPACE + detail.PRCENGNAME2;
                            ap.ENDORSEMENT_DATE = detail.PRCSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_F;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                        else if (ap.ORDERING == 7)
                        {
                            ap.FORM_PART = ProcessingConstant.PART_D;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                    }
                }
                else if (ProcessingConstant.FORM_03.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW03.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW03(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // prc
                        {
                            ap.CHINESE_NAME = detail.ARCCHINAME;
                            ap.ENGLISH_NAME = detail.ARCENGNAME1 + SPACE + detail.ARCENGNAME2;
                            //ap.CERTIFICATION_NO = 

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                        else if (ap.ORDERING == 1) // prc
                        {
                            ap.CHINESE_NAME = detail.RCCHINAME;
                            ap.ENGLISH_NAME = detail.RCENGNAME1 + SPACE + detail.RCENGNAME2;
                            ap.COMMENCED_DATE = detail.COMMENDATE;
                            ap.CERTIFICATION_NO = detail.RCCRNUM1 + SEPARATOR + detail.RCCRNUM2;
                            ap.CHINESE_COMPANY_NAME = detail.ASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.ASENGNAME1 + SPACE + detail.ASENGNAME2;
                            ap.EXPIRY_DATE = detail.RCCREXPIREDDATE;
                            ap.MW_NO = detail.RCMWSUBMISSIONID;
                            ap.FAX_NO = detail.RCFAX;
                            ap.CONTACT_NO = detail.RCTEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.ENDORSEMENT_DATE = detail.RCSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }

                    }
                }
                else if (ProcessingConstant.FORM_04.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW04.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW04(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // prc
                        {
                            ap.CHINESE_NAME = detail.RCCHINAME;
                            ap.ENGLISH_NAME = detail.RCENGNAME1 + SPACE + detail.RCENGNAME2;
                            ap.COMPLETION_DATE = detail.COMPLETDATE;
                            ap.CERTIFICATION_NO = detail.RCCRNUM1 + SPACE + detail.RCCRNUM2;
                            ap.CHINESE_COMPANY_NAME = detail.ASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.ASENGNAME;
                            ap.EXPIRY_DATE = detail.RCCREXPIREDDATE;
                            ap.FAX_NO = detail.RCFAX;
                            ap.CONTACT_NO = detail.RCTEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.ENDORSEMENT_DATE = detail.RCSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                        else if (ap.ORDERING == 1)
                        {
                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;

                        }// prc
                        else if (ap.ORDERING == 2) // other
                        {
                            ap.CHINESE_NAME = detail.ARCCHINAME;
                            ap.ENGLISH_NAME = detail.ARCENGNAME1 + SPACE + detail.ARCENGNAME2;
                            ap.ENDORSEMENT_DATE = detail.ARCSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_C;
                            ap.IDENTIFY_FLAG = ProcessingConstant.OTHER;
                        }
                    }
                }
                else if (ProcessingConstant.FORM_05.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW05.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW05(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // prc
                        {
                            ap.CHINESE_NAME = detail.ARCCHINAME;
                            ap.ENGLISH_NAME = detail.ARCENGNAME1 + SPACE + detail.ARCENGNAME2;
                            ap.CERTIFICATION_NO = detail.ARCCRNUM1 + SEPARATOR + detail.ARCCRNUM2;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                        else if (ap.ORDERING == 1) // prc
                        {
                            ap.CHINESE_NAME = detail.RCCHINAME;
                            ap.ENGLISH_NAME = detail.RCENGNAME1 + SPACE + detail.RCENGNAME2;
                            ap.COMMENCED_DATE = detail.COMMENDATE;
                            ap.COMPLETION_DATE = detail.COMPLETIONDATE;
                            ap.CERTIFICATION_NO = detail.RCCRNUM1 + SEPARATOR + detail.RCCRNUM2;
                            ap.CHINESE_COMPANY_NAME = detail.ASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.ASENGNAME1 + SPACE + detail.ASENGNAME2;
                            ap.EXPIRY_DATE = detail.RCCREXPIREDDATE;
                            ap.MW_NO = detail.PMWSUBMISSIONID;
                            ap.FAX_NO = detail.RCFAX;
                            ap.CONTACT_NO = detail.RCTEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.ENDORSEMENT_DATE = detail.RCSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                    }
                }
                else if (ProcessingConstant.FORM_06.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW06.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW06(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // ap
                        {
                            ap.CHINESE_NAME = detail.APCHINAME;
                            ap.ENGLISH_NAME = detail.APENGNAME1 + SPACE + detail.APENGNAME2;
                            ap.CERTIFICATION_NO = detail.APCRNUM1 + SEPARATOR + detail.APCRNUM2;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.AP;

                        }
                        else if (ap.ORDERING == 1) // prc
                        {
                            ap.CHINESE_NAME = detail.ARCCHINAME;
                            ap.ENGLISH_NAME = detail.ARCENGNAME1 + SPACE + detail.ARCENGNAME2;
                            ap.CERTIFICATION_NO = detail.ARCCRNUM1 + SEPARATOR + detail.ARCCRNUM2;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                        else if (ap.ORDERING == 2) // ap
                        {
                            ap.CHINESE_NAME = detail.APCFMCHINAME;
                            ap.ENGLISH_NAME = detail.APCFMENGNAME;
                            ap.COMPLETION_DATE = detail.INSPECTIONDATE;
                            ap.CHINESE_COMPANY_NAME = detail.APCFMASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.APCFMASENGNAME1 + SPACE + detail.APCFMASENGNAME2;
                            ap.MW_NO = detail.APCFMSUBMISSIONID;
                            ap.FAX_NO = detail.APCFMFAX;
                            ap.CONTACT_NO = detail.APCFMTEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.EXPIRY_DATE = detail.APCFMCREXPIREDATE;
                            ap.CERTIFICATION_NO = detail.APCFMCRNUM1 + SEPARATOR + detail.APCFMCRNUM2;
                            ap.ENDORSEMENT_DATE = detail.APCFMSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.AP;
                        }
                        else if (ap.ORDERING == 3) // prc
                        {
                            ap.CHINESE_NAME = detail.RCCFMCHINAME;
                            ap.ENGLISH_NAME = detail.RCCFMENGNAME1 + SPACE + detail.RCCFMENGNAME2;
                            ap.COMMENCED_DATE = detail.COMMENDATE;
                            ap.COMPLETION_DATE = detail.COMPLETEDATE;
                            ap.CERTIFICATION_NO = detail.RCCFMCRNUM1 + SEPARATOR + detail.RCCFMCRNUM2;
                            ap.CHINESE_COMPANY_NAME = detail.RCCFMASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.RCCFMASENGNAME1 + SPACE + detail.RCCFMASENGNAME2;
                            ap.EXPIRY_DATE = detail.RCCFMCREXPIREDDATE;
                            ap.FAX_NO = detail.RCCFMFAX;
                            ap.CONTACT_NO = detail.RCCFMTEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.ENDORSEMENT_DATE = detail.RCCFMSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_C;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                    }
                }
                else if (ProcessingConstant.FORM_07.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW07.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW07(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // other
                        {
                            ap.CHINESE_NAME = detail.APCHINAME;
                            ap.ENGLISH_NAME = detail.APENGNAME1 + SPACE + detail.APENGNAME2;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.OTHER;
                        }
                        else if (ap.ORDERING == 1) // rse
                        {
                            if (getTrueFalseFromOption(detail.RSEOPTION.ToString()))
                            {
                                ap.CHINESE_NAME = detail.RSECHINAME;
                                ap.ENGLISH_NAME = detail.RSEENGNAME;
                                ap.CERTIFICATION_NO = detail.RSECRNUM1 + SEPARATOR + detail.RSECRNUM2;
                                ap.IS_CHECKED = true;
                            }
                            else
                            {
                                ap.IS_CHECKED = false;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RSE;
                        }
                        else if (ap.ORDERING == 2) // rge
                        {
                            if (getTrueFalseFromOption(detail.RGEOPTION.ToString()))
                            {
                                ap.CHINESE_NAME = detail.RGECHINAME;
                                ap.ENGLISH_NAME = detail.RGEENGNAME;
                                ap.CERTIFICATION_NO = detail.RGECRNUM1 + SEPARATOR + detail.RGECRNUM2;
                                ap.IS_CHECKED = true;
                            }
                            else
                            {
                                ap.IS_CHECKED = false;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RGE;
                        }
                        else if (ap.ORDERING == 3) // prc
                        {
                            if (getTrueFalseFromOption(detail.RCOPTION.ToString()))
                            {
                                ap.CHINESE_NAME = detail.RCCHINAME;
                                ap.ENGLISH_NAME = detail.RCENGNAME1 + SPACE + detail.RCENGNAME2;
                                ap.CERTIFICATION_NO = detail.RCCRNUM1 + SEPARATOR + detail.RCCRNUM2;
                                ap.FAX_NO = detail.APFAX;
                                ap.CONTACT_NO = detail.APTEL;
                                ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                                ap.EFFECT_FROM_DATE = detail.EFFECTDATE;
                                ap.ENDORSEMENT_DATE = detail.APSIGNDATE;
                                ap.IS_CHECKED = true;
                            }
                            else
                            {
                                ap.IS_CHECKED = false;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                        else if (ap.ORDERING == 4) // rse
                        {
                            if (getTrueFalseFromOption(detail.RSEOPTION.ToString()))
                            {
                                ap.CHINESE_NAME = detail.RSECFMCHINAME;
                                ap.ENGLISH_NAME = detail.RSECFMENGNAME;
                                ap.FAX_NO = detail.RSECFMFAX;
                                ap.CONTACT_NO = detail.RSECFMTEL;
                                ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                                ap.CERTIFICATION_NO = detail.RSECFMCRNUM1 + SEPARATOR + detail.RSECFMCRNUM2;
                                ap.EXPIRY_DATE = detail.RSECFMCREXPIREDDATE;
                                ap.ENDORSEMENT_DATE = detail.RSECFMSIGNDATE;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RSE;
                        }
                        else if (ap.ORDERING == 5) // rge
                        {
                            if (getTrueFalseFromOption(detail.RGEOPTION.ToString()))
                            {
                                ap.CHINESE_NAME = detail.RGECFMCHINAME;
                                ap.ENGLISH_NAME = detail.RGECFMENGNAME;
                                ap.FAX_NO = detail.RGECFMFAX;
                                ap.CONTACT_NO = detail.RGECFMTEL;
                                ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                                ap.CERTIFICATION_NO = detail.RGECFMCRNUM1 + SEPARATOR + detail.RGECFMCRNUM2;
                                ap.EXPIRY_DATE = detail.RGECFMCREXPIREDDATE;
                                ap.ENDORSEMENT_DATE = detail.RGECFMSIGNDATE;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_C;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RGE;
                        }
                        else if (ap.ORDERING == 6) // prc
                        {
                            if (getTrueFalseFromOption(detail.RCOPTION.ToString()))
                            {
                                ap.CHINESE_NAME = detail.RCCFMASCHINAME;
                                ap.ENGLISH_NAME = detail.RCCFMENGNAME1 + SPACE + detail.RCCFMENGNAME2;
                                ap.CERTIFICATION_NO = detail.RCCFMCRNUM1 + SEPARATOR + detail.RCCFMCRNUM2;
                                ap.CHINESE_COMPANY_NAME = detail.RCCFMASCHINAME;
                                ap.ENGLISH_COMPANY_NAME = detail.RCCFMASENGNAME1 + SPACE + detail.RCCFMASENGNAME2;
                                ap.FAX_NO = detail.RCCFMFAX;
                                ap.CONTACT_NO = detail.RCCFMTEL;
                                ap.EXPIRY_DATE = detail.RCCFMCREXPIREDDATE;
                                ap.ENDORSEMENT_DATE = detail.RCCFMSIGNDATE;
                            }

                            ap.FORM_PART = ProcessingConstant.PART_D;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                        else if (ap.ORDERING == 7) // ap
                        {
                            ap.CHINESE_NAME = detail.APRICHINAME;
                            ap.ENGLISH_NAME = detail.APRIENGNAME;
                            ap.FAX_NO = detail.APRIFAX;
                            ap.CONTACT_NO = detail.APRITEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.CERTIFICATION_NO = detail.APRICRNUM1 + SEPARATOR + detail.APRICRNUM2;
                            ap.EXPIRY_DATE = detail.APRICREXPIREDDATE;
                            ap.ENDORSEMENT_DATE = detail.APRISIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_E;
                        }
                    }
                }
                else if (ProcessingConstant.FORM_08.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW08.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW08(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // ap
                        {
                            ap.CHINESE_NAME = detail.ARRCHINAME;
                            ap.ENGLISH_NAME = detail.ARRENGNAME1 + SPACE + detail.ARRENGNAME2;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                        }
                        else if (ap.ORDERING == 1) // ap
                        {
                            ap.CHINESE_NAME = detail.NAPCHINAME;
                            ap.ENGLISH_NAME = detail.NAPENGNAME;
                            ap.CERTIFICATION_NO = detail.NAPCRNUM1 + SEPARATOR + detail.NAPCRNUM2;
                            ap.APPOINTMENT_DATE = detail.EFFECTDATE;
                            ap.ENDORSEMENT_DATE = detail.ARRSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.AP;
                        }
                        else if (ap.ORDERING == 2) // ap
                        {
                            ap.CHINESE_NAME = detail.APRICHINAME;
                            ap.ENGLISH_NAME = detail.APRIENGNAME;
                            ap.CERTIFICATION_NO = detail.APRICRNUM1 + SEPARATOR + detail.APRICRNUM2;
                            ap.EXPIRY_DATE = detail.APRICREXPIREDDATE;
                            ap.ENDORSEMENT_DATE = detail.APRISIGNDATE;
                            ap.FAX_NO = detail.APRIFAX;
                            ap.CONTACT_NO = detail.APRITEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;

                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.AP;
                        }
                    }
                }
                else if (ProcessingConstant.FORM_09.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW09.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW09(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // other
                        {
                            ap.CHINESE_NAME = detail.NOMINATORCHINAME;
                            ap.ENGLISH_NAME = detail.NOMINATORENGNAME1;
                            ap.CERTIFICATION_NO = detail.NOMINATORCRNUM1 + SEPARATOR + detail.NOMINATORCRNUM2;
                            ap.EFFECT_FROM_DATE = detail.EFFECTFROMDATE;
                            ap.EFFECT_TO_DATE = detail.EFFECTTODATE;
                            ap.EXPIRY_DATE = detail.NOMINATORCREXPIREDDATE;
                            ap.ENDORSEMENT_DATE = detail.NOMINATORSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.OTHER;
                        }
                        else if (ap.ORDERING == 1) // other
                        {
                            ap.CHINESE_NAME = detail.NOMINEECHINAME;
                            ap.ENGLISH_NAME = detail.NOMINEEENGNAME;
                            ap.CERTIFICATION_NO = detail.NOMINEECRNUM1 + SEPARATOR + detail.NOMINEECRNUM2;
                            ap.EXPIRY_DATE = detail.NOMINEECREXPIREDDATE;
                            ap.FAX_NO = detail.NOMINEEFAX;
                            ap.ENDORSEMENT_DATE = detail.NOMINEESIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.OTHER;
                        }
                    }
                }
                else if (ProcessingConstant.FORM_10.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW10.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW10(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // other
                        {
                            ap.CHINESE_NAME = detail.CARCCHINAME;
                            ap.ENGLISH_NAME = detail.CARCENGNAME1 + SPACE + detail.CARCENGNAME2;
                            ap.CLASS1_CEASE_DATE = detail.EFFECTDATE;
                            ap.CERTIFICATION_NO = detail.ACRCCRNUM1 + SEPARATOR + detail.ACRCCRNUM2;
                            ap.CHINESE_COMPANY_NAME = detail.ASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.ASENGNAME;
                            ap.ENDORSEMENT_DATE = detail.ACRCCREXPIREDDATE;
                            ap.FAX_NO = detail.ACRCFAX;
                            ap.CONTACT_NO = detail.ACRCTEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                        else if (ap.ORDERING == 1) // other
                        {
                            ap.CHINESE_NAME = detail.APRICHINAME;
                            ap.ENGLISH_NAME = detail.APRIENGNAME;
                            ap.CERTIFICATION_NO = detail.APRICRNUM1 + SEPARATOR + detail.APRICRNUM2;
                            ap.EXPIRY_DATE = detail.APRICREXPIREDDATE;
                            ap.COMMENCED_DATE = detail.DELIVERDATE;
                            ap.FAX_NO = detail.APRIFAX;
                            ap.CONTACT_NO = detail.APRITEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.ENDORSEMENT_DATE = detail.APRISIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.AP;
                        }
                    }
                }
                else if (ProcessingConstant.FORM_11.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW11.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW11(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // ap
                        {
                            ap.CHINESE_NAME = detail.AUTHCHINAME;
                            ap.ENGLISH_NAME = detail.AUTHENGNAME;
                            ap.COMMENCED_DATE = detail.COMMENDATE;
                            ap.FAX_NO = detail.AUTHFAX;
                            ap.CONTACT_NO = detail.AUTHTEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.CERTIFICATION_NO = detail.AUTHCRNUM1 + SEPARATOR + detail.AUTHCRNUM2;
                            ap.EXPIRY_DATE = detail.AUTHCREXPIREDDATE;
                            ap.ENDORSEMENT_DATE = detail.AUTHSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                        }
                        else if (ap.ORDERING == 1) // rse
                        {
                            ap.CHINESE_NAME = detail.RSECHINAME;
                            ap.ENGLISH_NAME = detail.RGEENGNAME;
                            ap.CERTIFICATION_NO = detail.RGECRNUM1 + SEPARATOR + detail.RGECRNUM2;
                            ap.CONTACT_NO = detail.RGETEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.EXPIRY_DATE = detail.RGECREXPIREDDATE;
                            ap.ENDORSEMENT_DATE = detail.RGESIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RSE;
                        }
                        else if (ap.ORDERING == 2) // rge
                        {
                            ap.CHINESE_NAME = detail.RGECHINAME;
                            ap.ENGLISH_NAME = detail.RGEENGNAME;
                            ap.CERTIFICATION_NO = detail.RGECRNUM1 + SEPARATOR + detail.RGECRNUM2;
                            ap.CONTACT_NO = detail.RGETEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.EXPIRY_DATE = detail.RGECREXPIREDDATE;
                            ap.ENDORSEMENT_DATE = detail.RGESIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.RGE;
                        }
                        else if (ap.ORDERING == 3) // prc
                        {
                            ap.CHINESE_NAME = detail.RCCHINAME;
                            ap.ENGLISH_NAME = detail.RCENGNAME1 + SPACE + detail.RCENGNAME2;
                            if (getTrueFalseFromOption(detail.STARTDATEOPTION.ToString()))
                            {
                                ap.COMMENCED_DATE = detail.STARTDATE;
                            }
                            ap.CERTIFICATION_NO = detail.RCCRNUM1 + SEPARATOR + detail.RCCRNUM2;
                            ap.CHINESE_COMPANY_NAME = detail.ASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.ASENGNAME1 + SPACE + detail.ASENGNAME2;
                            ap.FAX_NO = detail.RCFAX;
                            ap.CONTACT_NO = detail.RCTEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.EXPIRY_DATE = detail.RCEXPIREDDATE;
                            ap.ENDORSEMENT_DATE = detail.RCSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                    }
                }
                else if (ProcessingConstant.FORM_12.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW12.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW12(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // prc
                        {
                            ap.CHINESE_NAME = detail.RCCHINAME;
                            ap.ENGLISH_NAME = detail.RCENGNAME1 + SPACE + detail.RCENGNAME2;
                            ap.COMMENCED_DATE = detail.COMMENDATE;
                            ap.CERTIFICATION_NO = detail.RCCRNUM1 + SEPARATOR + detail.RCCRNUM2;
                            ap.CHINESE_COMPANY_NAME = detail.RCASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.RCASENGNAME1 + SPACE + detail.RCASENGNAME2;
                            ap.FAX_NO = detail.RCFAX;
                            ap.CONTACT_NO = detail.RCTEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            ap.EXPIRY_DATE = detail.RCCREXPIREDDATE;
                            ap.ENDORSEMENT_DATE = detail.RCSIGNDATE;

                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }
                    }
                }

                else if (ProcessingConstant.FORM_31.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW31.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW31(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // other
                        {
                            ap.CHINESE_NAME = detail.CARCCHINAME;
                            ap.ENGLISH_NAME = detail.CARCENGNAME;
                            ap.CLASS1_CEASE_DATE = detail.EFFECTDATE;
                            ap.CERTIFICATION_NO = detail.ACRCCRNUM1 + SEPARATOR + detail.ACRCCRNUM2;
                            ap.ENDORSEMENT_DATE = detail.ACRCCREXPIREDDATE;
                            ap.FAX_NO = detail.ACRCFAX;
                            ap.CONTACT_NO = detail.ACRCTEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.OTHER;
                        }

                    }
                }
                else if (ProcessingConstant.FORM_32.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW32.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW32(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // prc
                        {
                            ap.COMMENCED_DATE = detail.EXPECTEDCOMMENCEDATE;
                            ap.COMPLETION_DATE = detail.EXPECTEDCOMPLETEDATE;

                            ap.CHINESE_NAME = detail.RCCHINAME;
                            ap.ENGLISH_NAME = detail.RCENGNAME1 + SPACE + detail.RCENGNAME2;
                            ap.CERTIFICATION_NO = detail.RCCRNUM1 + SEPARATOR + detail.RCCRNUM2;
                            ap.CHINESE_COMPANY_NAME = detail.ASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.ASENGNAME1 + SPACE + detail.ASENGNAME2;
                            ap.EXPIRY_DATE = detail.RCCREXPIREDDATE;
                            ap.ENDORSEMENT_DATE = detail.RCSIGNDATE;
                            ap.FAX_NO = detail.RCFAX;
                            ap.CONTACT_NO = detail.RCTEL;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;

                            ap.FORM_PART = ProcessingConstant.PART_B;
                            ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                        }

                    }
                }
                else if (ProcessingConstant.FORM_33.Equals(record.S_FORM_TYPE_CODE))
                {
                    var detail = db.P_EFMWU_TBL_MW33.Find(master.FORMCONTENTID);
                    if (detail == null) { detail = new P_EFMWU_TBL_MW33(); }
                    foreach (var ap in aps)
                    {
                        if (ap.ORDERING == 0) // prc
                        {
                            ap.CHINESE_NAME = detail.APCHINAME;
                            ap.ENGLISH_NAME = detail.APENGNAME1 + SPACE + detail.APENGNAME2;
                            ap.CHINESE_COMPANY_NAME = detail.ASCHINAME;
                            ap.ENGLISH_COMPANY_NAME = detail.ASENGNAME1 + SPACE + detail.ASENGNAME2;
                            ap.CERTIFICATION_NO = detail.APCRNUM1 + SEPARATOR + detail.APCRNUM2;
                            ap.EXPIRY_DATE = detail.APSIGNEXPIREDDATE;
                            ap.CONTACT_NO = detail.APTEL;
                            ap.FAX_NO = detail.APFAX;
                            ap.ENDORSEMENT_DATE = detail.APSIGNDATE;
                            ap.RECEIVE_SMS = ProcessingConstant.FLAG_N;
                            //if (getTrueFalseFromOption(detail.APOPTION.ToString()))
                            //{
                            //    ap.IDENTIFY_FLAG = ProcessingConstant.AP;
                            //}
                            //else if(getTrueFalseFromOption(detail.RIOPTION.ToString()))
                            //{
                            //    ap.IDENTIFY_FLAG = ProcessingConstant.RI;
                            //}
                            //else if(getTrueFalseFromOption(detail.RSEOPTION.ToString()))
                            //{
                            //    ap.IDENTIFY_FLAG = ProcessingConstant.RSE;
                            //}
                            //else if(getTrueFalseFromOption(detail.RGEOPTION.ToString()))
                            //{
                            //    ap.IDENTIFY_FLAG = ProcessingConstant.RGE;
                            //}
                            //else if(getTrueFalseFromOption(detail.PRCOPTION.ToString()))
                            //{
                            //    ap.IDENTIFY_FLAG = ProcessingConstant.PRC;
                            //}

                            ap.FORM_PART = ProcessingConstant.PART_A;
                            ap.IDENTIFY_FLAG = ProcessingConstant.OTHER;
                        }
                    }
                }

                return aps;
            }
        }

        #endregion

        #region Upload documents/DSN/P_MW_SCANNED_DOCUMENT
        public void uploadScannedDocument(string formType, string dsnNo, EntitiesMWProcessing db)
        {
            // eform
            var master = getEfssRecordByDsn(dsnNo);
            string dsn = dsnNo;
            string dsnSub = getDsnSubByDsn(dsn, ProcessingConstant.DSN_DOCUMENT_TYPE_FORM);
            string fullPath = uploadScannedDocumentFullPath(dsnSub);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            fullPath += dsnSub + master.FILEEXTENSION;
            File.WriteAllBytes(@fullPath, master.FILECONTENT);

            P_MW_SCANNED_DOCUMENT sd = new P_MW_SCANNED_DOCUMENT();
            sd.DSN_ID = db.P_MW_DSN.Where(x => x.DSN == dsn).FirstOrDefault().UUID;
            sd.DSN_SUB = dsnSub;
            sd.FILE_PATH = fullPath;
            sd.DOCUMENT_TYPE = ProcessingConstant.DSN_DOCUMENT_TYPE_FORM;
            sd.SCAN_DATE = DateTime.Now;
            sd.DOC_TITLE = formType;
            sd.RELATIVE_FILE_PATH = uploadScannedDocumentRelativePath(dsnSub) + dsnSub + master.FILEEXTENSION;
            sd.FOLDER_TYPE = getFolderTypeByDocType(ProcessingConstant.DSN_DOCUMENT_TYPE_FORM);
            db.P_MW_SCANNED_DOCUMENT.Add(sd);
            db.SaveChanges();

            // other attachments
            List<P_EFSS_FORM_ATTACHMENTS> docs = db.P_EFSS_FORM_ATTACHMENTS.Where(x => x.RECVFORMID == master.ID).ToList();
            if (docs != null && docs.Count() > 0)
            {
                foreach (var doc in docs)
                {
                    string dsn_sub = getDsnSubByDsn(dsn, doc.DOCTYPE);
                    string full_path = uploadScannedDocumentFullPath(dsn_sub);
                    if (!Directory.Exists(full_path))
                    {
                        Directory.CreateDirectory(full_path);
                    }
                    full_path += dsn_sub + doc.FILEEXTENSION;
                    File.WriteAllBytes(@full_path, doc.FILECONTENT);

                    P_MW_SCANNED_DOCUMENT s = new P_MW_SCANNED_DOCUMENT();
                    s.DSN_ID = db.P_MW_DSN.Where(x => x.DSN == dsn).FirstOrDefault().UUID;
                    s.DSN_SUB = dsn_sub;
                    s.FILE_PATH = full_path;
                    s.DOCUMENT_TYPE = doc.DOCTYPE;
                    s.SCAN_DATE = DateTime.Now;
                    s.DOC_TITLE = formType;
                    s.RELATIVE_FILE_PATH = uploadScannedDocumentRelativePath(dsn_sub) + dsn_sub + doc.FILEEXTENSION;
                    s.FOLDER_TYPE = getFolderTypeByDocType(doc.DOCTYPE);
                    db.P_MW_SCANNED_DOCUMENT.Add(s);
                    db.SaveChanges();
                }
            }

        }
        #endregion
    }
}