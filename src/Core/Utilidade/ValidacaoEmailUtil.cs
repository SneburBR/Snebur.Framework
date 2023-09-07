using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Snebur.Utilidade
{
    public static class ValidacaoEmailUtil
    {
        private static readonly int[] PortasSmtp = new int[] { 587, 25 };
        public static bool IsExisteEmail(string email)
        {
            var dominio = email.Split('@')[1];
            var dnsRecords = DnsUtil.GetDnsRecords(dominio, QueryType.MX);
            if (dnsRecords?.Count() > 0)
            {
                foreach (var record in dnsRecords.OrderBy(x => x.Preference))
                {
                    foreach(var porta in PortasSmtp)
                    {
                        var isExisteConta = ValidacaoEmailUtil.IsEmailAccountValid(record.Record, email, porta);
                        if (isExisteConta)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool IsEmailAccountValid(string tcpClient, string emailAddress, int porta)
        {
            string CRLF = "\r\n";
            using (var tClient = new TcpClient(tcpClient, porta))
            {
                byte[] dataBuffer;
                string responseString;
                using (var netStream = tClient.GetStream())
                using (var reader = new StreamReader(netStream))
                {
                    responseString = reader.ReadLine();
                    /* Perform HELO to SMTP Server and get Response */
                    dataBuffer = BytesFromString("HELO Hi" + CRLF);
                    netStream.Write(dataBuffer, 0, dataBuffer.Length);
                    responseString = reader.ReadLine();
                    dataBuffer = BytesFromString("MAIL FROM:<YourGmailIDHere@gmail.com>" + CRLF);
                    netStream.Write(dataBuffer, 0, dataBuffer.Length);
                    responseString = reader.ReadLine();
                    dataBuffer = BytesFromString($"RCPT TO:<{emailAddress}>" + CRLF);
                    netStream.Write(dataBuffer, 0, dataBuffer.Length);
                    responseString = reader.ReadLine();

                    var responseCode = GetResponseCode(responseString);

                    if (responseCode == 550)
                    {
                        return false;
                    }

                    /* QUITE CONNECTION */
                    dataBuffer = BytesFromString("QUITE" + CRLF);
                    netStream.Write(dataBuffer, 0, dataBuffer.Length);
                    tClient.Close();
                    return true;
                }
            }
        }

        private static byte[] BytesFromString(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private static int GetResponseCode(string responseString)
        {
            int statusCode;
            try
            {
                if (responseString == null)
                {
                    return -1;
                }
                statusCode = Convert.ToInt32(responseString.Substring(0, 3));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving response code. {ex.Message}");
                statusCode = -1;
            }

            return statusCode;
        }

    }

}
