using MWMS2.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Web;

namespace MWMS2
{
    public class SqlCommandInterceptor : DbCommandInterceptor
    {

        public void printSQL(string function, DbCommand command)
        {
            AuditLogService.logDebug(function+" SQL: " + command.CommandText);

            for (var i = 0; i < command.Parameters.Count; i++)
            {
                AuditLogService.logDebug("ParameterName: [" + command.Parameters[i].ParameterName +
                                                "] Value:["+ command.Parameters[i].Value+"]");
                
            }
        }

        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            printSQL("NonQueryExecuting", command);
            base.NonQueryExecuting(command, interceptionContext);

        }

        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            printSQL("NonQueryExecuted", command);

            base.NonQueryExecuted(command, interceptionContext);
            if (interceptionContext.Exception != null)
            {
                AuditLogService.logDebug("ErrorMessage: " + interceptionContext.Exception);
                AuditLogService.logDebug("ErrorSql: " + command.CommandText);

                for (var i = 0; i < command.Parameters.Count; i++)
                {
                    AuditLogService.logDebug("ErrorParameterName: " + command.Parameters[i].ParameterName);
                    AuditLogService.logDebug("ErrorParameterValue: " + command.Parameters[i].Value);
                }
            }
        }

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            printSQL("ReaderExecuting", command);

            base.ReaderExecuting(command, interceptionContext);
        }

        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            printSQL("ReaderExecuted", command);

            base.ReaderExecuted(command, interceptionContext);
            if (interceptionContext.Exception != null)
            {
                AuditLogService.logDebug("ErrorMessage: " + interceptionContext.Exception);
                AuditLogService.logDebug("ErrorSql: " + command.CommandText);

                for (var i = 0; i < command.Parameters.Count; i++)
                {
                    AuditLogService.logDebug("ErrorParameterName: " + command.Parameters[i].ParameterName);
                    AuditLogService.logDebug("ErrorParameterValue: " + command.Parameters[i].Value);
                }
            }
        }
    }
}