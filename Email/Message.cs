using System.Collections.Generic;
using MimeKit;

namespace TopSecretSanta.Email;
public sealed class Message
{
    public IEnumerable<MailboxAddress>? To { get; set; }
    public string? Subject { get; set; }
    public string? Content { get; set; }
    public AttachedFile? AttachedFile {get; set;}
}

public class AttachedFile
{
    public string? Name { get; set; }
    public byte[]? Content { get; set; }
}