using MailKit.Net.Smtp;
using MimeKit;
using System.Linq;


namespace TopSecretSanta.Email;
public sealed class EmailSender : IEmailSender
{
    private readonly EmailConfiguration configuration;

    public EmailSender()
    {
        configuration = new EmailConfiguration();
    }

    public void SendEmail(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        Send(emailMessage);
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(configuration.From, configuration.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Cc.AddRange(configuration.CarbonCopies.Select(_ => new MailboxAddress(_, _)));
        emailMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = message.Content;
        if (message.AttachedFile != null)
        {
            bodyBuilder.Attachments.Add(message.AttachedFile.Name, message.AttachedFile.Content);
        }
        emailMessage.Body = bodyBuilder.ToMessageBody();

        return emailMessage;
    }

    private void Send(MimeMessage mailMessage)
    {
        using (var client = new SmtpClient())
        {
            try
            {
                client.Connect(configuration.SmtpServer, configuration.Port, configuration.UseSsl);
                client.Send(mailMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
