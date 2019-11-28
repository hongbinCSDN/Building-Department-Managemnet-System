using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWMS2_Data_Conversion
{
    public class ConnDb
    {
        string odbcConnectString = @"Driver={Oracle in OraDB18Home2};Server=orcl2;Persist Security Info=False;Trusted_Connection=Yes;UID=MWMS2;PWD=123456;";

        OdbcConnection connection;

        public ConnDb()
        {
            if(connection == null)
            {
                connection = new OdbcConnection(odbcConnectString);

                if(connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
            }
        }

        public void connClose()
        {
            if(connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }



        public DataSet ExecuteQuerySql(string sql)
        {
            DataSet ds = new DataSet();
            OdbcDataAdapter da = new OdbcDataAdapter(sql, connection);
            da.Fill(ds);
            connClose();
            return ds;
        }

        public DataSet ExecuteDataSet(string sql,OracleParameter[] parameterList)
        {
            DataSet dt = new DataSet();
            using(OdbcDataAdapter oda = new OdbcDataAdapter(sql, connection))
            {
                oda.SelectCommand = GetCommand(sql, parameterList);
                oda.Fill(dt);
            }
            return dt;
        }

        public OdbcCommand GetCommand(string sql, OracleParameter[] oraPra)
        {
            OdbcCommand oracmd = new OdbcCommand(sql, connection);
            if (null != oraPra)
            {
                foreach (var p in oraPra)
                {
                    if (sql.IndexOf(p.ParameterName) > 0)
                    {
                        if (p.Value == null) p.Value = DBNull.Value;

                        if (!oracmd.Parameters.Contains(p.ParameterName))
                            oracmd.Parameters.Add(p);
                    }

                }
            }
            return oracmd;
        }
    }
}
