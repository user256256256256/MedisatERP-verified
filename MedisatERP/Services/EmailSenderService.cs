using MedisatERP.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSenderService : IEmailSender
{
    private readonly ExceptionHandlerService _exceptionHandlerService;
    private readonly ILogger<EmailSenderService> _logger;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;

    // Constructor to inject IConfiguration for accessing appsettings.json
    public EmailSenderService(
        ExceptionHandlerService exceptionHandlerService,
        ILogger<EmailSenderService> logger,
        IConfiguration configuration)
    {
        _exceptionHandlerService = exceptionHandlerService;
        _logger = logger;

        // Retrieve SMTP settings from the appsettings.json file via IConfiguration
        _smtpServer = configuration["EmailSettings:SmtpServer"];
        _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
        _smtpUser = configuration["EmailSettings:SmtpUser"];
        _smtpPassword = configuration["EmailSettings:SmtpPassword"];
        _fromEmail = configuration["EmailSettings:FromEmail"];
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            // Log start of method
            _logger.LogInformation("Starting to send email...");

            // Set up the email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),  // Use the email from the configuration
                Subject = subject,
                Body = message,
                IsBodyHtml = true,  // Set to true if the message is in HTML format
            };

            // Log email setup details
            _logger.LogInformation($"Email configured. Subject: {subject}, Recipient: {email}");

            // Add the recipient email
            mailMessage.To.Add(email);
            _logger.LogInformation($"Recipient {email} added to the email.");

            // Set up the SMTP client with your server details
            using (var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
                EnableSsl = true,  // Use SSL for secure communication
            })
            {
                _logger.LogInformation("SMTP client configured. Sending email...");

                // Send the email asynchronously
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully.");
            }
        }
        catch (Exception ex)
        {
            // Log exception and handle it via the ExceptionHandlerService
            _logger.LogError(ex, "Error occurred while sending email.");
            _exceptionHandlerService.HandleException(ex);

            // Rethrow the exception to let the calling method handle it
            throw;
        }
    }
}
