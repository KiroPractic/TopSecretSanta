using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MimeKit;
using Newtonsoft.Json;
using QRCoder;
using TopSecretSanta;
using TopSecretSanta.Email;

IEmailSender emailSender = new EmailSender();
var currentYear = DateTime.Now.Year;

using var streamReader = new StreamReader("participants.json");
var json = streamReader.ReadToEnd();
var participants = JsonConvert.DeserializeObject<List<Participant>>(json)!;

var giftReceivers = participants.OrderBy(_ => Guid.NewGuid()).ToList();
var giftGivers = participants.OrderBy(_ => Guid.NewGuid()).ToList();

var giftPairings = new List<GiftingPairing>();

foreach (var giftGiver in giftGivers)
{
    var giftReceiver = giftReceivers.FirstOrDefault(_ => _.Id != giftGiver.Id);
    if (giftReceiver is null || giftReceivers.RemoveAll(_ => _.Id == giftReceiver.Id) != 1)
        throw new("Invalid operation result.");

    giftPairings.Add(new(giftGiver, giftReceiver));
}

var originalTemplateContent = File.ReadAllText("template.html")
    .Replace("{{year}}", $"{currentYear}")
    .Replace("{{giftBudgetWithCurrencySign}}", "15 EUR");
foreach (var giftPairing in giftPairings)
{
    var qrGenerator = new QRCodeGenerator();
    var qrCodeData = qrGenerator.CreateQrCode($"{giftPairing.GiftReceiver.Name}", QRCodeGenerator.ECCLevel.Q);
    var qrCode = new QRCode(qrCodeData);
    var qrCodeImage = qrCode.GetGraphic(5, Color.Black, Color.White, true);
    emailSender.SendEmail(
        new()
        {
            To = new List<MailboxAddress> { new(giftPairing.GiftGiver.Name, giftPairing.GiftGiver.Email) },
            Subject = $"Secret Santa {currentYear}.",
            Content = originalTemplateContent.Replace("{{genderSpecificSanta}}", giftPairing.GiftGiver.Gender == Gender.Female ? "tajna bakica mraz" : "tajni djedica mraz"),
            AttachedFile = new() { Name = "SecretQRCode.png", Content = BitmapUtilities.BitmapToBytes(qrCodeImage) }
        });
}

namespace TopSecretSanta
{
    public class Participant
    {
        public Guid Id = Guid.NewGuid();
        public string Name;
        public string Email;
        public Gender Gender;
    }

    public enum Gender
    {
        Female,
        Male
    }

    public class GiftingPairing
    {
        public GiftingPairing(Participant giftGiver, Participant giftReceiver)
        {
            GiftGiver = giftGiver;
            GiftReceiver = giftReceiver;
        }

        public Participant GiftGiver { get; set; }
        public Participant GiftReceiver { get; set; }
    }

    public class BitmapUtilities
    {
        public static Byte[] BitmapToBytes(Bitmap img)
        {
            using var stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }
}