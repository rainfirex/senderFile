using System;
using System.Net.Mail;

namespace senderFile.Modules
{
    class SendMail
    {
        SmtpClient smtpClient;

        private MailMessage _message;


        public static void sendOne(string from, string fromName, string to, string subject, string htmlBody, string smtpHost, int port, string user, string password, string attach = "", bool isSSL = false)
        {
            MailAddress _from = new MailAddress(from, fromName);
            MailAddress _to = new MailAddress(to);
            MailMessage message = new MailMessage(_from, _to);
            message.IsBodyHtml = true;
            message.Subject = subject;
            message.Body = htmlBody;
            if (!String.IsNullOrEmpty(attach))
                message.Attachments.Add(new Attachment(attach));

            SmtpClient smtpClient = new SmtpClient(smtpHost, port);
            smtpClient.Credentials = new System.Net.NetworkCredential(user, password);
            smtpClient.EnableSsl = isSSL;
            smtpClient.Send(message);
        }

        public SendMail(string smtpHost, int port, string user, string password, bool ssl, int timeout)
        {
            smtpClient = new SmtpClient(smtpHost, port);
            smtpClient.EnableSsl = ssl;
            smtpClient.Timeout = timeout;
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = user,
                Password = password
            };
        }

        public SendMail InitAddress(string from, string fromName, string to)
        {
            _message = new MailMessage(new MailAddress(from, fromName), new MailAddress(to));
            return this;
        }

        public SendMail AddText(string subject, string htmlBody)
        {
            _message.IsBodyHtml = true;
            _message.Subject = subject;
            _message.Body = htmlBody;
            return this;
        }

        public SendMail AddFile(string attachPath = "")
        {
            if (!String.IsNullOrEmpty(attachPath))
                _message.Attachments.Add(new Attachment(attachPath));

            return this;
        }

        public void send()
        {
            smtpClient.Send(_message);
        }
    }
}
