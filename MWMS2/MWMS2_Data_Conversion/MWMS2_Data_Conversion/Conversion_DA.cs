using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWMS2_Data_Conversion
{
    public class Conversion_DA
    {
        private ConnDb _Db;
        protected ConnDb Db
        {
            get
            {
                return _Db ?? (_Db = new ConnDb());
            }
        }
        public DataSet SelectDemo()
        {
            string sql = "Select * from Sys_Func Where rownum < 10";
            return Db.ExecuteQuerySql(sql);
        }
    }
}
