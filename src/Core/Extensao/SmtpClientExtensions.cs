
namespace System.Net.Mail;

public static class SmtpClientExtensions
{
    public static void SendAsync(this SmtpClient smtpClient, MailMessage mailMessage)
    {
        Task.Factory.StartNew(() => smtpClient.Send(mailMessage));
    }
}
