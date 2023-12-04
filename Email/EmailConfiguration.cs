using System.Collections.Generic;

namespace TopSecretSanta.Email;

public sealed class EmailConfiguration
{
    public string From => "secret.santa@company.com";
    public string SmtpServer => "mail-server.com";
    public string Username => "secret.santa@company.com";
    public string Password => "sup3r-strong-smtp-Password";
    public int Port => 25;
    public bool UseSsl => false;
    public IEnumerable<string> CarbonCopies => new List<string>();
}
