using System.Text;

namespace System
{
    public static class ExceptionExtensions
    {
        public static string GetAllExceptionMessages(this Exception exception)
        {
            var sb = new StringBuilder();
            Exception? currentException = exception;
            while (currentException != null)
            {
                sb.AppendLine(currentException.Message);
                currentException = currentException.InnerException;
            }
            return sb.ToString().TrimEnd();
        }
    }
}
