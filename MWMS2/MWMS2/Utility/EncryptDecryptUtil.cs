using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Utility
{
    public class EncryptDecryptUtil
    {
        public static string ENCRYPTED_KEY = "22A7907EF0B997F0821ADE5538A6A5249F94AEB41D95D3756629D53AFD44DFD3";
                                             
        public string retrieveHKIDKey() {
            return ENCRYPTED_KEY;
        }

        static public string getDecryptSQL(string field)
        {
            return " C_DECRYPT( " + field + " , '" + ENCRYPTED_KEY + "' ) ";

        }

        // Andy: Common function get decrypted HKID
        static public string getDecryptHKID(string field)
        {
            if (string.IsNullOrWhiteSpace(field)) return "";
            String result = "";
            String query = "select " + getDecryptSQL("'"+field +"'") + " from dual";

            // setup connection
            EntitiesRegistration db = new EntitiesRegistration();
            DbConnection conn = db.Database.Connection;
            DbCommand comm = conn.CreateCommand();
            comm.CommandText = query;
            conn.Open();
            DbDataReader dr = comm.ExecuteReader(CommandBehavior.CloseConnection);

            dr.Read();
            result = dr.GetString(0);
            dr.Close();

            conn.Close();
            return result;
        }

        static public string getEncryptSQL(string field)
        {
            return " C_ENCRYPT( " + field + " , '" + ENCRYPTED_KEY + "' ) ";
        }

     
        static public string getEncrypt(string field)
        {
            if (string.IsNullOrWhiteSpace(field)) return "";
            String result = "";
            String query = "select " + getEncryptSQL("'" + field + "'") + " from dual";
             
            // setup connection
            EntitiesRegistration db = new EntitiesRegistration();
            DbConnection conn = db.Database.Connection;
            DbCommand comm = conn.CreateCommand();
            comm.CommandText = query;
            conn.Open();
            DbDataReader dr = comm.ExecuteReader(CommandBehavior.CloseConnection);

            dr.Read();
            result = dr.GetString(0);
            dr.Close();

            conn.Close();
            return result;
        }
    }
}
