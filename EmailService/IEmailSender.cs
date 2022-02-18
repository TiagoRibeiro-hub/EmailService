namespace EmailService;
public interface IEmailSender
{
    Task SendEmailAsync(Message message);
    Task SendEmailWithFilesAsync(Message message);
}

