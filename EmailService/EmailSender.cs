namespace EmailService;
public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    public EmailSender(EmailSettings emailSettings)
    {
        _emailSettings = emailSettings;
    }

    //  Send Email Async
    public async Task SendEmailAsync(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        await SendAsync(emailMessage);
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(MailboxAddress.Parse(_emailSettings.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;
        var bodybuilder = new BodyBuilder { HtmlBody = message.Content };
        emailMessage.Body = bodybuilder.ToMessageBody();

        return emailMessage;
    }

    //  Send Email With Files Async
    public async Task SendEmailWithFilesAsync(Message message)
    {
        var emailMessage = CreateEmailMessageWithFiles(message);
        await SendAsync(emailMessage);
    }

    private MimeMessage CreateEmailMessageWithFiles(Message message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(MailboxAddress.Parse(_emailSettings.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;

        var bodybuilder = new BodyBuilder { HtmlBody = message.Content };

        if (message.Attachments.Count >= 1)
        {
            byte[] fileBytes;
            foreach (var item in message.Attachments)
            {
                using (var ms = new MemoryStream())
                {
                    item.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                bodybuilder.Attachments.Add(item.FileName, fileBytes, ContentType.Parse(item.ContentType));
            }
        }

        emailMessage.Body = bodybuilder.ToMessageBody();

        return emailMessage;
    }

    // Send it
    private async Task SendAsync(MimeMessage emailMessage)
    {
        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, _emailSettings.SSL);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);

                await client.SendAsync(emailMessage);
            }
            catch (Exception)
            {

                throw new Exception("EmailService ()=> SendAsync");
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }

    }

}