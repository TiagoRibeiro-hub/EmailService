namespace EmailService;
public static class ServiceInjection
{
    public static void AddEmailServices(this IServiceCollection services, EmailSettings emailSettings)
    {
        // Email
        services.AddTransient<IEmailSender>(_ => new EmailSender(emailSettings));

        // Files
        services.Configure<FormOptions>(config =>
        {
            config.ValueLengthLimit = int.MaxValue;
            config.MultipartBodyLengthLimit = int.MaxValue;
            config.MemoryBufferThreshold = int.MaxValue;
        });
    }
}
