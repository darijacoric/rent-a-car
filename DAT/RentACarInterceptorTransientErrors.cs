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
    /*  Entity Framework will call this class every time it is going to send a query to the database, to simulate transient errors. 
     *  This interceptor class must derive from the DbCommandInterceptor class. In it you write method overrides that are automatically called
     *  when query is about to be executed. In these methods you can examine or log the query that is being sent to the database, 
     *  and you can change the query before it's sent to the database or return something to Entity Framework yourself without even passing the query to the database. */
    public class RentACarInterceptorTransientErrors : DbCommandInterceptor
    {
        private int _counter = 0;
        private ILogger _logger = new Logger();

        /* This code only overrides the ReaderExecuting method, which is called for queries that can return multiple rows of data. 
         * If you wanted to check connection resiliency for other types of queries, you could also override the NonQueryExecuting and ScalarExecuting methods, 
         * as the logging interceptor does. */
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            bool throwTransientErrors = false;

            /* When you run the Student page and enter "Throw" as the search string, this code creates a dummy SQL Database exception for error number 20 (sqlErrorNumber in CreateDummySqlException), 
             * a type known to be typically transient. */
            if (command.Parameters.Count > 0 && command.Parameters[0].Value.ToString() == "%Throw%")
            {
                /* The value you enter in the Search box will be in command.Parameters[0] and command.Parameters[1] (one is used for the first name and one for the last name).
                 *  When the value "%Throw%" is found, "Throw" is replaced in those parameters by "an" so that some students will be found and returned. */
                throwTransientErrors = true;
                command.Parameters[0].Value = "%an%";
                command.Parameters[1].Value = "%an%";
            }

            /* The code returns the exception to Entity Framework instead of running the query and passing back query results. 
             * The transient exception is returned four times, and then the code reverts to the normal procedure of passing the query to the database.  
             * Because everything is logged, you'll be able to see that Entity Framework tries to execute the query four times before finally succeeding,
             * and the only difference in the application is that it takes longer to render a page with query results. */
            if (throwTransientErrors && _counter < 4)
            {
                _logger.Information("Returning transient error for command: {0}", command.CommandText);
                _counter++;
                interceptionContext.Exception = CreateDummySqlException();
            }
        }

        private SqlException CreateDummySqlException()
        {
            // The instance of SQL Server you attempted to connect to does not support encryption
            
            var sqlErrorNumber = 20; // It will be displayed in output window 

            var sqlErrorCtor = typeof(SqlError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 7).Single();
            var sqlError = sqlErrorCtor.Invoke(new object[] { sqlErrorNumber, (byte)0, (byte)0, "", "", "", 1 });

            var errorCollection = Activator.CreateInstance(typeof(SqlErrorCollection), true);
            var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic);
            addMethod.Invoke(errorCollection, new[] { sqlError });

            var sqlExceptionCtor = typeof(SqlException).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 4).Single();
            var sqlException = (SqlException)sqlExceptionCtor.Invoke(new object[] { "Dummy", errorCollection, null, Guid.NewGuid() });

            return sqlException;
        }

        /* You've written the transient error simulation code in a way that lets you cause transient errors by entering a different value in the UI.  
         * As an alternative, you could write the interceptor code to always generate the sequence of transient exceptions without checking for a particular parameter value.
         *  You could then add the interceptor only when you want to generate transient errors. If you do this, however, don't add the interceptor until
         *   after database initialization has completed. In other words, do at least one database operation such as a query on one of your entity sets 
         *   before you start generating transient errors. The Entity Framework executes several queries during database initialization, 
         *   and they aren't executed in a transaction, so errors during initialization could cause the context to get into an inconsistent state. */
    }
}
 
 