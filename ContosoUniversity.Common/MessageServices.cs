using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ContosoUniversity.Common
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor, IOptions<SMSOptions> smsOptionsAccessor)
        {
            Options = optionsAccessor.Value;
            SmsOptions = smsOptionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; }
        public SMSOptions SmsOptions { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // plug in your email service here to send an email
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("no-reply@contoso.com", "Consoto University"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // plug in your sms service here to send a text message
            // your account SID from twilio.com/console
            var accountSid = SmsOptions.SMSAccountIdentification;
            // your auth token from twilio.com/console
            var authToken = SmsOptions.SMSAccountPassword;

            TwilioClient.Init(accountSid, authToken);

            var msg = MessageResource.Create(
                to: new Twilio.Types.PhoneNumber(number),
                from: new Twilio.Types.PhoneNumber(SmsOptions.SMSAccountFrom),
                body: message);

            return Task.FromResult(0);
        }
    }
}
