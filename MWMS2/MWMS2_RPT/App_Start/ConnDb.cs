using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;

namespace MWMS2_RPT.App_Start
{
    public class ConnDb
    {
        OracleConnection conn;//连接数据库的对象

        /// <summary>
        /// 构造函数,连接数据库，数据库连接字符在web.Config文件的AppSettings下的conStr
        /// </summary>
        public ConnDb()
        {
            if (conn == null)//判断连接是否为空
            {
              // string conString = @"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.88.200)(PORT=1521))(CONNECT_DATA = (SERVICE_NAME = orcl.bd.net))); Persist Security Info = True; User ID = MWMS2; Password = 123456; ";//连接数据库的字符串
                //string conString = System.Configuration.ConfigurationManager.AppSettings["conStr"];//连接数据库的字符串
                conn = new OracleConnection(ConfigurationManager.AppSettings["ConnectionString"]);

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();//打开数据库连接

                }
            }
        }

        /// <summary>
        /// 从数据库中查询数据的,返回为DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet query(string sql)
        {
            DataSet ds = new DataSet();//DataSet是表的集合

            OracleDataAdapter da = new OracleDataAdapter(sql, conn);//从数据库中查询
           /* using (OracleDataAdapter oda = new OracleDataAdapter(sql, conn))
            {
                oda.SelectCommand = command(sql);
                oda.Fill(ds);
            }*/
            da.Fill(ds);//将数据填充到DataSet

            connClose();//关闭连接

            return ds;//返回结果

        }

        public DataSet queryListSql(List<string> sqlList)
        {
            DataSet ds = new DataSet();
            
            int i = 1;
            foreach (var sql in sqlList)
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    using (OracleDataAdapter da = new OracleDataAdapter(sql, conn))
                    {
                        da.Fill(ds, "DataTable" + i);
                    }
                }
                i++;
            }
            connClose();
            return ds;
        }


        
        public DataSet ExecuteDataSet(string sql, OracleParameter[] parameterList)
        {   //查询语句的参数化  
            DataSet dt = new DataSet();
            using (OracleDataAdapter oda = new OracleDataAdapter(sql, conn))
            {
                    oda.SelectCommand = GetCommand(sql, parameterList);
                    oda.Fill(dt);
            }
            return dt;
        }

        public OracleCommand GetCommand(string sql, OracleParameter[] oraPra)
        {
            OracleCommand oracmd = new OracleCommand(sql, conn);
            oracmd.BindByName = true; 
            if (null != oraPra)
            {
                foreach (var p in oraPra)
                {
                    if (sql.IndexOf( p.ParameterName)>0) {
                        if (p.Value == null) p.Value = DBNull.Value;

                        if(!oracmd.Parameters.Contains(p.ParameterName))
                            oracmd.Parameters.Add(p);
                    }


                    
                }
            }
            return oracmd;
        }

        public OracleCommand command(string sql)
        {
            OracleCommand oracmd = new OracleCommand();
            oracmd.CommandText = sql;

            return oracmd;
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int update(string sql)
        {
            OracleCommand oc = new OracleCommand();//表示要对数据源执行的SQL语句或存储过程

            oc.CommandText = sql;//设置命令的文本

            oc.CommandType = CommandType.Text;//设置命令的类型

            oc.Connection = conn;//设置命令的连接

            int x = oc.ExecuteNonQuery();//执行SQL语句

            connClose();//关闭连接

            return x; //返回一个影响行数

        }


        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void connClose()
        {
            if (conn.State == ConnectionState.Open)
            {//判断数据库的连接状态，如果状态是打开的话就将它关闭

                conn.Close();
            }
        }

    }
}