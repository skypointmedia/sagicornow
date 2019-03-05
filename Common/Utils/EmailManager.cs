using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using MimeKit;

namespace SagicorNow.Common.Utils
{
    public class EmailManager
    {
        public EmailManager()
        {
        }

    /// <summary>
    /// Sends the email using the given SMTP settings
    /// </summary>
    /// <returns>The email async.</returns>
    /// <param name="smtpOptions">Smtp options.</param>
    /// <param name="to">To.</param>
    /// <param name="from">From.</param>
    /// <param name="subject">Subject.</param>
    /// <param name="plainTextMessage">Plain text message.</param>
    /// <param name="htmlMessage">Html message.</param>
    /// <param name="replyTo">Reply to.</param>
        public EmailRespose SendEmail(
            SmtpOptions smtpOptions,
			List<string> to,
			string from,
			string subject,
			string plainTextMessage,
			string htmlMessage,
			string replyTo = null)
		{
            if (to.Count == 0)
			{
				throw new ArgumentException("no to address provided");
			}

			if (string.IsNullOrWhiteSpace(from))
			{
				throw new ArgumentException("no from address provided");
			}

			if (string.IsNullOrWhiteSpace(subject))
			{
				throw new ArgumentException("no subject provided");
			}

			var hasPlainText = !string.IsNullOrWhiteSpace(plainTextMessage);
			var hasHtml = !string.IsNullOrWhiteSpace(htmlMessage);
			if (!hasPlainText && !hasHtml)
			{
				throw new ArgumentException("no message provided");
			}

			var m = new MimeMessage();

			m.From.Add(new MailboxAddress("", from));
			if (!string.IsNullOrWhiteSpace(replyTo))
			{
				m.ReplyTo.Add(new MailboxAddress("", replyTo));
			}

            //set recipients
            foreach (var mailAddr in to)
            {
                m.To.Add(new MailboxAddress("", mailAddr));
            }			

            m.Subject = subject;

			BodyBuilder bodyBuilder = new BodyBuilder();
			if (hasPlainText)
			{
				bodyBuilder.TextBody = plainTextMessage;
			}

			if (hasHtml)
			{
				bodyBuilder.HtmlBody = htmlMessage;
			}

			m.Body = bodyBuilder.ToMessageBody();

			using (var client = new SmtpClient())
			{
				    client.Connect(
					smtpOptions.Server,
					smtpOptions.Port,
                    smtpOptions.UseSsl);

				// Note: since we don't have an OAuth2 token, disable
				// the XOAUTH2 authentication mechanism.
				client.AuthenticationMechanisms.Remove("XOAUTH2");

				// Note: only needed if the SMTP server requires authentication
				if (smtpOptions.RequiresAuthentication)
				{
					client.Authenticate(smtpOptions.User, smtpOptions.Password);
				}

				client.Send(m);
			    client.Disconnect(true);
			}

            return new EmailRespose() { ResponseMessage = "Email successfully sent." };
		}

    }


    public class EmailRespose
    {
        public string ResponseMessage { get; set; }
        public string ErrorCode { get; set; }

    }


	public class SmtpOptions
	{
        public string Server { get; set; } = "slicscex01.sagicorgroup.com"; //"casjam.sagicorgroup.com";
		public int Port { get; set; } = 25;
		public string User { get; set; } = "";
		public string Password { get; set; } = "";
        public bool UseSsl { get; set; } = false;
        public bool RequiresAuthentication { get; set; } = false;
		public string PreferredEncoding { get; set; } = string.Empty;
	}
}
