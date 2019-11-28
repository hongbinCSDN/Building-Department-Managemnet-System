using MWMS2.Constant;
using MWMS2.Entity;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MWMS2.Utility
{
    public class AuthUtil
    {

        private static bool isCheckSearchLevel(string registrationType, string typecode)
        {
            if (SessionUtil.SearchLevelRights.Where(o => o.REGISTRATION_TYPE == registrationType && o.CODE == typecode).FirstOrDefault() == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private static SYS_ROLE_FUNC getSysRoleFunc(string functionCode)
        {
            return SessionUtil.AccessableFunctions.Where(o => o.SYS_FUNC.CODE == functionCode).FirstOrDefault();
        }

        private static SYS_ROLE_FUNC Current_SYS_ROLE_FUNC
        {
            get
            {
                if (SessionUtil.AccessableFunctions == null) return null;
                if (SessionUtil.CurrentFunction == null) return null;
                if (string.IsNullOrWhiteSpace(SessionUtil.CurrentFunction.CODE)) return null;
                return getSysRoleFunc(SessionUtil.CurrentFunction.CODE);
            }
        }
        public static bool CanCreate
        {
            get { SYS_ROLE_FUNC current = Current_SYS_ROLE_FUNC; if (current == null) return false; return "Y".Equals(current.SYS_FUNC.ABLE_CREATE) && "Y".Equals(current.CAN_CREATE); }
        }
        public static bool CanDelete
        {
            get { SYS_ROLE_FUNC current = Current_SYS_ROLE_FUNC; if (current == null) return false; return "Y".Equals(current.SYS_FUNC.ABLE_DELETE) && "Y".Equals(current.CAN_DELETE); }
        }
        public static bool CanEdit
        {
            get { SYS_ROLE_FUNC current = Current_SYS_ROLE_FUNC; if (current == null) return false; return "Y".Equals(current.SYS_FUNC.ABLE_EDIT) && "Y".Equals(current.CAN_EDIT); }
        }
        public static bool CanList
        {
            get { SYS_ROLE_FUNC current = Current_SYS_ROLE_FUNC; if (current == null) return false; return "Y".Equals(current.SYS_FUNC.ABLE_LIST) && "Y".Equals(current.CAN_LIST); }
        }
        public static bool CanViewDetails
        {
            get { SYS_ROLE_FUNC current = Current_SYS_ROLE_FUNC; if (current == null) return false; return "Y".Equals(current.SYS_FUNC.ABLE_VIEW_DETAILS) && "Y".Equals(current.CAN_VIEW_DETAILS); }
        }
        public static bool CanViewMBI
        {
            get {
                var a = SessionUtil.LoginPostRoleList.Where(x => x.SYS_ROLE_ID == "3").ToList();
                return a.Count()> 0 ? true : false;
            }
        }
        public static bool CanViewUserManagement
        {
            get {
                List<string> list = new List<string>() { "10060", "10050", "10070", "10080", "1000" };
                return SessionUtil.AccessableFunctions.Select(x => x.SYS_FUNC.PARENT_ID).Intersect(list).Any();

            }

        }

        public static bool FunctionCanCreate(string functionCode)
        {
            SYS_ROLE_FUNC current = getSysRoleFunc(functionCode); if (current == null) return false; return "Y".Equals(current.SYS_FUNC.ABLE_CREATE) && "Y".Equals(current.CAN_CREATE);
        }
        public static bool FunctionCanDelete(string functionCode)
        {
            SYS_ROLE_FUNC current = getSysRoleFunc(functionCode); if (current == null) return false; return "Y".Equals(current.SYS_FUNC.ABLE_DELETE) && "Y".Equals(current.CAN_DELETE);
        }
        public static bool FunctionCanEdit(string functionCode)
        {
            SYS_ROLE_FUNC current = getSysRoleFunc(functionCode); if (current == null) return false; return "Y".Equals(current.SYS_FUNC.ABLE_EDIT) && "Y".Equals(current.CAN_EDIT);
        }
        public static bool FunctionCanList(string functionCode)
        {
            SYS_ROLE_FUNC current = getSysRoleFunc(functionCode); if (current == null) return false; return "Y".Equals(current.SYS_FUNC.ABLE_LIST) && "Y".Equals(current.CAN_LIST);
        }
        public static bool FunctionCanViewDetails(string functionCode)
        {
            SYS_ROLE_FUNC current = getSysRoleFunc(functionCode); if (current == null) return false; return "Y".Equals(current.SYS_FUNC.ABLE_VIEW_DETAILS) && "Y".Equals(current.CAN_VIEW_DETAILS);
        }
        //Auth - SearchLevel - CGC
        public static bool CGC_Show_ContractorName
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_CGA, RegistrationConstant.CGC_ContractorName); }
        }

        public static bool CGC_Show_AddrContact
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_CGA, RegistrationConstant.CGC_AddrContact); }
        }

        public static bool CGC_Show_BRNo
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_CGA, RegistrationConstant.CGC_BRNo); }
        }

        public static bool CGC_Show_LeaveRecords
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_CGA, RegistrationConstant.CGC_LeaveRecords); }
        }

        public static bool CGC_Show_ASTDOO
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_CGA, RegistrationConstant.CGC_ASTDOO); }
        }

        public static bool CGC_Show_AppliStatus
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_CGA, RegistrationConstant.CGC_AppliStatus); }
        }

        public static bool CGC_Show_HKIDPASSPORT
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_CGA, RegistrationConstant.CGC_HKIDPASSPORT); }
        }

        public static bool CGC_Show_NA
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_CGA, RegistrationConstant.CGC_NA); }
        }

        public static bool CGC_Show_SpecimenSignature
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_CGA, RegistrationConstant.CGC_SpecimenSignature); }
        }

        public static bool CGC_Show_NonSpecimenSignature
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_CGA, RegistrationConstant.CGC_NonSpecimenSignature); }
        }


        //Auth - SearchLevel - CMW
        public static bool CMW_Show_ContractorName
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWCA, RegistrationConstant.CMW_ContractorName); }
        }

        public static bool CMW_Show_AddrContact
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWCA, RegistrationConstant.CMW_AddrContact); }
        }

        public static bool CMW_Show_BRNo
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWCA, RegistrationConstant.CMW_BRNo); }
        }

        public static bool CMW_Show_LeaveRecords
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWCA, RegistrationConstant.CMW_LeaveRecords); }
        }

        public static bool CMW_Show_ASTDOO
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWCA, RegistrationConstant.CMW_ASTDOO); }
        }

        public static bool CMW_Show_AppliStatus
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWCA, RegistrationConstant.CMW_AppliStatus); }
        }

        public static bool CMW_Show_HKIDPASSPORT
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWCA, RegistrationConstant.CMW_HKIDPASSPORT); }
        }

        public static bool CMW_Show_NA
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWCA, RegistrationConstant.CMW_NA); }
        }

        public static bool CMW_Show_SpecimenSignature
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWCA, RegistrationConstant.CMW_SpecimenSignature); }
        }

        public static bool CMW_Show_NonSpecimenSignature
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWCA, RegistrationConstant.CMW_NonSpecimenSignature); }
        }


        //Auth - SearchLevel - IMW
        public static bool IMW_Show_Name_RegNo
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWIA, RegistrationConstant.IMW_Name_RegNo); }
        }

        public static bool IMW_Show_Contact
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWIA, RegistrationConstant.IMW_Contact); }
        }

        public static bool IMW_Show_HKIDPASSPORT
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWIA, RegistrationConstant.IMW_HKIDPASSPORT); }
        }

        public static bool IMW_Show_SpecimenSignature
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWIA, RegistrationConstant.IMW_SpecimenSignature); }
        }

        public static bool IMW_Show_LeaveRecords
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWIA, RegistrationConstant.IMW_LeaveRecords); }
        }

        public static bool IMW_Show_CorrAddress
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWIA, RegistrationConstant.IMW_CorrAddress); }
        }

        public static bool IMW_Show_AppliStatus
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWIA, RegistrationConstant.IMW_AppliStatus); }
        }

        public static bool IMW_Show_OfficeAddress
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_MWIA, RegistrationConstant.IMW_OfficeAddress); }
        }


        //Auth - SearchLevel - IP
        public static bool IP_Show_Name_RegNo
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_IP, RegistrationConstant.IP_Name_RegNo); }
        }

        public static bool IP_Show_Contact
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_IP, RegistrationConstant.IP_Contact); }
        }

        public static bool IP_Show_HKIDPASSPORT
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_IP, RegistrationConstant.IP_HKIDPASSPORT); }
        }

        public static bool IP_Show_SpecimenSignature
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_IP, RegistrationConstant.IP_SpecimenSignature); }
        }

        public static bool IP_Show_LeaveRecords
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_IP, RegistrationConstant.IP_LeaveRecords); }
        }

        public static bool IP_Show_CorrAddress
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_IP, RegistrationConstant.IP_CorrAddress); }
        }

        public static bool IP_Show_AppliStatus
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_IP, RegistrationConstant.IP_AppliStatus); }
        }

        public static bool IP_Show_OfficeAddress
        {
            get { return isCheckSearchLevel(RegistrationConstant.REGISTRATION_TYPE_IP, RegistrationConstant.IP_OfficeAddress); }
        }





        /*  o.CAN_CREATE
              o.CAN_DELETE

              o.CAN_EDIT
              o.CAN_LIST
              o.CAN_VIEW_DETAILS*/
    }
}