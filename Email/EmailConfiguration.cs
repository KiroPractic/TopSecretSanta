using System.Collections.Generic;

namespace TopSecretSanta.Email;
public sealed class EmailConfiguration
{
    public string From => "secret.santa@company.com";
    public IEnumerable<string> CarbonCopies => new List<string>();
    public string SmtpServer => "mail-server.com";
    public int Port => 25;
    public bool UseSsl => false;
}
