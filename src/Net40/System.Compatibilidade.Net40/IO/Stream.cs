using System.Threading.Tasks;

namespace System.IO
{
    public static class StreamExtensions
    {
        public static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
            return TaskUtil.CompletedTask();
        }
    }
 
}
