using MWMS2.App_Start;
using MWMS2.Constant;
using MWMS2.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Utility
{
    public class ValidationUtil
    {
        public enum VAR_PROP { VALUE, LABEL };
        public static ServiceResult ValidateForm(ModelStateDictionary m, ViewDataDictionary v)
        {
            return m.IsValid
                ? new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS }
                : new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, ErrorMessages = v.ModelState.Where(o => o.Value.Errors.Count > 0).ToDictionary(o => o.Key, o => o.Value.Errors.Select(o2 => (string.IsNullOrWhiteSpace(o2.ErrorMessage) ? o2.Exception.ToString() : o2.ErrorMessage)).ToList()) };
        }
        private static Dictionary<VAR_PROP, object> GetPropDetails(object obj, ref string propName)
        {
            Dictionary<VAR_PROP, object> r = new Dictionary<VAR_PROP, object>();
            r.Add(VAR_PROP.VALUE, null);
            r.Add(VAR_PROP.LABEL, null);
            if (string.IsNullOrWhiteSpace(propName)) return r;
            propName = propName.Trim();
            string[] nameParts = propName.Split('.');
            if (nameParts.Length == 1)
            {
                r[VAR_PROP.VALUE] = obj.GetType().GetProperty(propName).GetValue(obj, null);
                DisplayAttribute da = obj.GetType().GetProperty(propName).GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                if (da != null) r[VAR_PROP.LABEL] = da.Name == null ? propName : da.Name;
            }
            else
            {
                foreach (string part in nameParts)
                {
                    if (obj == null) { return r; }
                    Type type = obj.GetType();
                    PropertyInfo info = type.GetProperty(part);
                    if (info == null) { return r; }
                    obj = info.GetValue(obj, null);
                    MemberInfo property = type.GetProperty(part);
                    DisplayAttribute displayAttribute = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                    if (displayAttribute == null)
                    {
                        MetadataTypeAttribute metadataType = (MetadataTypeAttribute)type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();
                        if (metadataType != null)
                        {
                            property = metadataType.MetadataClassType.GetProperty(part);
                            if (property != null) displayAttribute = property.GetCustomAttributes().Where(o => o.GetType().Name.Equals("DisplayAttribute")).FirstOrDefault() as DisplayAttribute;
                        }
                    }
                    r[VAR_PROP.VALUE] = obj;
                    r[VAR_PROP.LABEL] = displayAttribute != null ? displayAttribute.Name : part;
                }
            }
            return r;
        }

        public static ValidationResult Validate_Length(object model, string propName)
        {
            Dictionary<VAR_PROP, object> v = GetPropDetails(model, ref propName);
            if (v[VAR_PROP.VALUE] == null) return null;
            if (!GlobalConfig.SYSTEM_COLUMN_META_LIST.ContainsKey(propName)) return null;
            if (GlobalConfig.SYSTEM_COLUMN_META_LIST[propName] >= v[VAR_PROP.VALUE].ToString().Length) return null;
            return new ValidationResult(
                " The maximum length for " +
                v[VAR_PROP.LABEL] + " is " +
                GlobalConfig.SYSTEM_COLUMN_META_LIST[propName] +
                " characters.", new List<string> { propName });
        }

        public static ValidationResult Validate_Email(object model, string propName)
        {
            Dictionary<VAR_PROP, object> v = GetPropDetails(model, ref propName);
            if (v[VAR_PROP.VALUE] == null) return null;
            if (!GlobalConfig.SYSTEM_COLUMN_META_LIST.ContainsKey(propName)) return null;
           
            try
            {
                string txt = v[VAR_PROP.VALUE].ToString().ToLower();
                var addr = new System.Net.Mail.MailAddress(txt);
                if (addr.Address.Equals(txt)) return null;
            }
            catch { }
            return new ValidationResult("Invalid Email Address", new List<string> { propName });
        }


        public static ValidationResult Validate_FormNo(string value, string propName, String DocType)
        {
            if ("Enquiry".Equals(DocType) || "Complaint".Equals(DocType))
            {
                // No form no for Enquiry 
                return null;
            }
            else if ("Modification".Equals(DocType))
            {
                // No form no for Enquiry 
                return null;
            }
            else if ("MWForm".Equals(DocType))
            {
                String[] validFormNos = ProcessingConstant.validMWFormNos;
                List<String> validFormNosList = new List<String>(validFormNos);

                if (validFormNosList.Contains(value))
                {
                    // Valid
                    return null;
                }
                else
                {
                    // InValid
                    return new ValidationResult(" Invalid Form No.", new List<string> { propName });
                }
            }

            return null;
        }


        public static ValidationResult Validate_isNumber(object model, string fieldKey, string displayName)
        {/**
            Dictionary<VAR_PROP, object> v = GetPropDetails(model, fieldKey);
            if (v[VAR_PROP.VALUE] == null) return null;

            v[VAR_PROP.VALUE].ToString


            if (!GlobalConfig.SYSTEM_COLUMN_META_LIST.ContainsKey(fieldKey)) return null;
            if (GlobalConfig.SYSTEM_COLUMN_META_LIST[fieldKey] >= v[VAR_PROP.VALUE].ToString().Length) return null;
            return new ValidationResult(
                " The maximum length for " +
                v[VAR_PROP.LABEL] + " is " +
                GlobalConfig.SYSTEM_COLUMN_META_LIST[fieldKey] +
                " characters.", new List<string> { fieldKey });
    **/
            return null;
        }


        public static ValidationResult Validate_Mandatory(object model, string propName)
        {
            Dictionary<VAR_PROP, object> v = GetPropDetails(model, ref propName);
            if (v[VAR_PROP.VALUE] != null)
            {
                if (v[VAR_PROP.VALUE].GetType().Name.Equals("DateTime") && v[VAR_PROP.VALUE] != null) return null;
                else if (!string.IsNullOrWhiteSpace(v[VAR_PROP.VALUE].ToString())) return null;
            }
            return new ValidationResult("Please enter " + v[VAR_PROP.LABEL] + ".", new List<string> { propName });
        }

        public static ValidationResult Validate_Mandatory(string value, string message, string propName)
        {
            if(string.IsNullOrWhiteSpace(value))
                return new ValidationResult(message, new List<string> { propName });
            else return null;
        }

        public static ValidationResult Validate_IsEqual(object model, string propName1, string propName2)
        {
            Dictionary<VAR_PROP, object> v1 = GetPropDetails(model, ref propName1);
            Dictionary<VAR_PROP, object> v2 = GetPropDetails(model, ref propName2);
            if (v1[VAR_PROP.VALUE] == null && v2[VAR_PROP.VALUE] == null) return null;
            //if (v1[VAR_PROP.VALUE] == null && v2[VAR_PROP.VALUE] != null) return null;
            //if (v1[VAR_PROP.VALUE] != null && v2[VAR_PROP.VALUE] == null) return null;
            return
               (v1[VAR_PROP.VALUE] == null) || (v2[VAR_PROP.VALUE] == null) ||
                (!v1[VAR_PROP.VALUE].ToString().Equals(v2[VAR_PROP.VALUE].ToString()))
                ? new ValidationResult(v2[VAR_PROP.LABEL] + " must match with " + v1[VAR_PROP.LABEL] + ".", new List<string> { propName2 }) : null;
        }

        public static ValidationResult Validate_Mandatory(object model, params string[] propNames)
        {
            if (propNames == null || propNames.Length == 0) return null;
            for (int i = 0; i < propNames.Length; i++) {
                ValidationResult vrs = Validate_Mandatory(model, propNames[i]);
                if (vrs == null) return null;
            }
            string msg = "Please enter either ";
            for (int i = 0; i < propNames.Length; i++)
            {
                Dictionary<VAR_PROP, object> v = GetPropDetails(model, ref propNames[i]);
                msg += v[VAR_PROP.LABEL];
                if (i == propNames.Length -2) msg += " or ";
                else if (i < propNames.Length - 2) msg += ", ";
                else msg += ".";
            }
            return new ValidationResult(msg, new List<string> { propNames[0] });
        }


        public static IEnumerable<ValidationResult> Validate_Properties_Length(object model, object o)
        {
            if (o != null)
            {
                for (int i = 0; i < o.GetType().GetProperties().Length; i++)
                {
                    if (o.GetType().GetProperties()[i].CanRead)
                    {
                        if ("System.String".Equals(o.GetType().GetProperties()[i].PropertyType.FullName))
                        {
                            string p = o.GetType().Name + "." + o.GetType().GetProperties()[i].Name;
                            yield return Validate_Length(model, p);
                        }
                    }
                }
            }
        }
    }
}