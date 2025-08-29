namespace Snebur
{
    public class SqlCommandExecutionException : Exception
    {
        public string SqlCommand { get; }
        public SqlCommandExecutionException(string sqlCommand, Exception innerException)
            : base($"Error executing SQL command: {sqlCommand}\r\n{innerException.GetAllExceptionMessages()}", innerException)
        {
            this.SqlCommand = sqlCommand;
        }
    }
}
