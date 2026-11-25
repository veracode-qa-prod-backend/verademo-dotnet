using System.Data.Common;
using System.Data.SqlClient;
using System.Net;

namespace Verademo.Commands
{
    public class IgnoreCommand : BlabberCommandBase, IBlabberCommand
    {
        private readonly string username;

        public IgnoreCommand(DbConnection connect, string username)
        {
            this.connect = connect;
            this.username = username;
        }

        public void Execute(string blabberUsername)
        {
            var sqlQuery = "DELETE FROM listeners WHERE blabber=@blabber AND listener=@username";

            logger.Info(sqlQuery);

            var action = connect.CreateCommand();
            action.CommandText = sqlQuery;
            action.Parameters.Add(new SqlParameter { ParameterName = "@blabber", Value = blabberUsername });
            action.Parameters.Add(new SqlParameter { ParameterName = "@username", Value = username });
            action.ExecuteNonQuery();

            sqlQuery = "SELECT blab_name FROM users WHERE username = @blabberUsername";

            var sqlStatement = connect.CreateCommand();
            sqlStatement.CommandText = sqlQuery;
            sqlStatement.Parameters.Add(new SqlParameter { ParameterName = "@blabberUsername", Value = blabberUsername });
            logger.Info(WebUtility.UrlEncode(sqlQuery));
            var blabName = sqlStatement.ExecuteScalar();

            /* START BAD CODE */
            var ignoringEvent = username + " is now ignoring " + blabberUsername + "(" + blabName + ")";
            sqlQuery = "INSERT INTO users_history (blabber, event) VALUES (@username, @ignoringEvent)";

            sqlStatement.CommandText = sqlQuery;
            sqlStatement.Parameters.Clear();
            sqlStatement.Parameters.Add(new SqlParameter { ParameterName = "@username", Value = username });
            sqlStatement.Parameters.Add(new SqlParameter { ParameterName = "@ignoringEvent", Value = ignoringEvent });

            /* END BAD CODE */

            logger.Info(WebUtility.UrlEncode(sqlQuery));
            sqlStatement.ExecuteNonQuery();
        }
    }
}