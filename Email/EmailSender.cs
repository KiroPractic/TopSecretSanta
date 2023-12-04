using MailKit.Net.Smtp;
using MimeKit;
using System.Linq;

namespace TopSecretSanta.Email;
public sealed class EmailSender : IEmailSender
{
    private readonly EmailConfiguration _configuration = new();

    public void SendEmail(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        Send(emailMessage);
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_configuration.From, _configuration.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Cc.AddRange(_configuration.CarbonCopies.Select(_ => new MailboxAddress(_, _)));
        emailMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message.Content
        };
        if (message.AttachedFile != null)
        {
            bodyBuilder.Attachments.Add(message.AttachedFile.Name, message.AttachedFile.Content);
        }
        emailMessage.Body = bodyBuilder.ToMessageBody();

        return emailMessage;
    }

    private void Send(MimeMessage mailMessage)
    {
        using var client = new SmtpClient();
        try
        {
            client.Connect(_configuration.SmtpServer, _configuration.Port, _configuration.UseSsl);
            client.Authenticate(_configuration.Username, _configuration.Password);
            client.Send(mailMessage);
        }
        finally
        {
            client.Disconnect(true);
            client.Dispose();
        }
    }
}
