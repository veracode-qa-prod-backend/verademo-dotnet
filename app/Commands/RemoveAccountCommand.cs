using System.Data.Common;
using System.Net;

namespace Verademo.Commands
{
    public class RemoveAccountCommand : BlabberCommandBase, IBlabberCommand
    {
        public RemoveAccountCommand(DbConnection connect)
        {
            this.connect = connect;
        }

        public void Execute(string blabberUsername)
        {
            var sqlQuery = "DELETE FROM listeners WHERE blabber=@Username OR listener=@Username;";
            logger.Info(sqlQuery);
            DbCommand action;
            action = connect.CreateCommand();
            action.CommandText = sqlQuery;

            var parameter = action.CreateParameter();
            parameter.ParameterName = "@Username";
            parameter.Value = blabberUsername;
            action.Parameters.Add(parameter);

            action.ExecuteNonQuery();

            sqlQuery = "SELECT blab_name FROM users WHERE username = @Username";
            var sqlStatement = connect.CreateCommand();
            sqlStatement.CommandText = sqlQuery;

            var selectParameter = sqlStatement.CreateParameter();
            selectParameter.ParameterName = "@Username";
            selectParameter.Value = blabberUsername;
            sqlStatement.Parameters.Add(selectParameter);

            logger.Info("SELECT blab_name FROM users WHERE username = " + WebUtility.UrlEncode(blabberUsername));
            var result = sqlStatement.ExecuteScalar();

            /* START BAD CODE */
            var removeEvent = "Removed account for blabber " + result;
            sqlQuery = "INSERT INTO users_history (blabber, event) VALUES (@Username, @Event)";
            sqlStatement = connect.CreateCommand();
            sqlStatement.CommandText = sqlQuery;

            var insertUsernameParam = sqlStatement.CreateParameter();
            insertUsernameParam.ParameterName = "@Username";
            insertUsernameParam.Value = blabberUsername;
            sqlStatement.Parameters.Add(insertUsernameParam);

            var insertEventParam = sqlStatement.CreateParameter();
            insertEventParam.ParameterName = "@Event";
            insertEventParam.Value = removeEvent;
            sqlStatement.Parameters.Add(insertEventParam);

            logger.Info("INSERT INTO users_history (blabber, event) VALUES (" + WebUtility.UrlEncode(blabberUsername) + ", " + WebUtility.UrlEncode(removeEvent) + ")");
            sqlStatement.ExecuteNonQuery();

            sqlQuery = "DELETE FROM users WHERE username = @Username";
            sqlStatement = connect.CreateCommand();
            sqlStatement.CommandText = sqlQuery;

            var deleteParameter = sqlStatement.CreateParameter();
            deleteParameter.ParameterName = "@Username";
            deleteParameter.Value = blabberUsername;
            sqlStatement.Parameters.Add(deleteParameter);

            logger.Info("DELETE FROM users WHERE username = " + WebUtility.UrlEncode(blabberUsername));
            sqlStatement.ExecuteNonQuery();
            /* END BAD CODE */
        }
    }
}