
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;

namespace MessageManager
{
    public class MessageSerivces : IMessageServices
    {
        private readonly IOptions<EmailConfiguration> config;

        public MessageSerivces(IOptions<EmailConfiguration> _config)
        {
            config = _config;
        }
        public void SendEmail( object Content)
        {
            string subject = config.Value.Subject;
            string to = config.Value.To;
            string from = config.Value.From;
            string smtpServer = config.Value.SmtpServer;
            int port = config.Value.Port;
            string password = config.Value.Password;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Test Message",from));
            message.To.Add(new MailboxAddress("test", to));
            message.Subject = subject;
           
            var _content = Content as Exception;
            if(_content == null)
            {
                return;
            }
            message.Body = new TextPart("plain")
            {
                Text = _content.StackTrace
            };

            using (var emailclient = new SmtpClient())
            {
                emailclient.Connect(smtpServer, port, false);
                emailclient.AuthenticationMechanisms.Remove("XOAUTH2");
               
                emailclient.Authenticate(from, password);
                emailclient.Send(message);
                emailclient.Disconnect(true);

            }
           
        }
    }
}
