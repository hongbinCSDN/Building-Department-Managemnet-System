using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PScan
{

    public class DocServerException : Exception
    {
        public static DocServerException ConnectionProblem
        {
            get
            {
                JObject r = new JObject();
                r.Add(DATA_KEY.RESULT, RESULT.CONNECTION_ERROR);
                r.Add(DATA_KEY.MESSAGE, "Connection Error.");
                return Create(r);
            }
        }
        public JObject Details { get; }
        private DocServerException(JObject jObject) { Details = jObject; }
        public static DocServerException Create(JObject jObject) { return new DocServerException(jObject); }
    }

    public class LocalFileExistException : Exception
    {
        public string[] Filepath { get; }
        private LocalFileExistException(string[] filepath) { this.Filepath = filepath; }
        public static LocalFileExistException Create(List<string> filepath) { return new LocalFileExistException(filepath.ToArray()); }
    }
}
