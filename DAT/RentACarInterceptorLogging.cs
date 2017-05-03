using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Reflection;
using RentACar.Logging;
using System.Data.Common;

namespace RentACar.DAT
{
    /*  Entity Framework will call this class every time it is going to send a query to the database, to do logging. 
     *  This interceptor class must derive from the DbCommandInterceptor class. In it you write method overrides that are automatically called
     *  when query is about to be executed. In these methods you can examine or log the query that is being sent to the database, 
     *  and you can change the query before it's sent to the database or return something to Entity Framework yourself without even passing the query to the database. */
    public class RentACarInterceptorLogging : DbCommandInterceptor
    {
        // Creating field that is type of interface and initializes it with class constructor is possible
        // only if that class implements that interface. 
        private ILogger _logger = new Logger();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        // For successful queries or commands, this code writes an Information log with latency information. For exceptions, it creates an Error log.         

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            base.ScalarExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }

        public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            _stopwatch.Stop();

            if(interceptionContext.Exception != null)
            {
                _logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
            }
            else
            {
                _logger.TraceApi("SQL Database", "CompanyInterceptor.ScalarExecuted", _stopwatch.Elapsed, "Command: {0}: ", command.CommandText);
            }

            base.ScalarExecuted(command, interceptionContext);
        }

        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            base.NonQueryExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }

        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            _stopwatch.Stop();

            if (interceptionContext.Exception != null)
            {
                _logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
            }
            else
            {
                _logger.TraceApi("SQl Database", "CompanyInterceptor.NonQueryExecuted", _stopwatch.Elapsed, "Command: {0}", command.CommandText);
            }
            base.NonQueryExecuted(command, interceptionContext);
        }

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            base.ReaderExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }

        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            _stopwatch.Stop();

            if (interceptionContext.Exception != null)
            {
                _logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
            }
            else
            {
                _logger.TraceApi("SQL Database", "CompanyInterceptor.ReaderExecuted", _stopwatch.Elapsed, "Command: {0}", command.CommandText);
            }

            base.ReaderExecuted(command, interceptionContext);
        }
    }
}