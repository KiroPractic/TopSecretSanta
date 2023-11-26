namespace TopSecretSanta.Email;
public interface IEmailSender
{
    void SendEmail(Message message);
}
