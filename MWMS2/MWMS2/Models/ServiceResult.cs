using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Models
{
    public class ServiceResult
    {
        public const string RESULT_SUCCESS = "SUCCESS";
        public const string RESULT_FAILURE = "FAILURE";
        public string Result { get; set; }
        public List<string> Message { get; set; } = new List<string>();
        public Dictionary<string, List<string>> ErrorMessages { get; set; } = new Dictionary<string, List<string>>();
        public object Data { get; set; }
    }
}