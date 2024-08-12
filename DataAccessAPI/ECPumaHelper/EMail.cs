using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace DataAccessAPI.ECPumaHelper
{
    /// <summary>
    /// Class to send SMTP Emails
    /// </summary>
    public class EMail
    {
        # region Private Variables

        private static string mailServer;
        private static string emailSender;

        private static string[] emailRecieversOfExceptions;
        private static string emailRecieverOfExceptions;

        private static string[] emailRecievers;
        private static string emailReciever;

        private static string emailExceptionSubject;
        private static string emailExceptionBody;

        private static bool isEmailToBeSent;
        private static bool isExceptionToBeSent;

        #endregion

        # region Constructor

        /// <summary>
        /// In constructor; get config.data
        /// </summary>
        static EMail()
        {
            AppSettingsReader appdata = new AppSettingsReader();

            mailServer = (string)appdata.GetValue("MailServer", typeof(string));
            emailSender = (string)appdata.GetValue("EmailSender", typeof(string));

            emailRecieverOfExceptions = (string)appdata.GetValue("EmailRecieversOfExceptions", typeof(string));
            emailRecieversOfExceptions = emailRecieverOfExceptions.Split(new Char[] { ';' });

            emailReciever = (string)appdata.GetValue("EmailRecievers", typeof(string));
            emailRecievers = emailReciever.Split(new Char[] { ';' });

            emailExceptionSubject = (string)appdata.GetValue("EmailExceptionSubject", typeof(string));
            emailExceptionBody = (string)appdata.GetValue("EmailExceptionBody", typeof(string));

            isEmailToBeSent = (bool)appdata.GetValue("IsEmailToBeSent", typeof(bool));
            isExceptionToBeSent = (bool)appdata.GetValue("IsExceptionToBeSent", typeof(bool));
        }

        #endregion

        # region Email

        /// <summary>
        /// Sends email to configured addresses. Uses sender defined in web.config
        /// </summary>
        /// <param name="recievers">recievers of email in string collection</param>
        /// <param name="mailSubject">MailSubject</param>
        /// <param name="mailBody">Mail body</param>
        public static void SendMail(List<string> recievers, string mailSubject, string mailBody)
        {
            if (isEmailToBeSent)
            {
                foreach (string reciever in recievers)
                {
                    MailAddress from = new MailAddress(emailSender);
                    MailAddress to = new MailAddress(reciever);
                    MailMessage message = new MailMessage(emailSender, reciever);

                    message.Subject = mailSubject;
                    message.Body = mailBody;

                    SmtpClient client = new SmtpClient(mailServer);
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;

                    client.Send(message);
                }
            }
        }

        /// <summary>
        /// Sends email to configured addresses. Sender must be specified
        /// </summary>
        /// <param name="recievers">recievers of email in string collection</param>
        /// <param name="sender">Sender of email</param>
        /// <param name="mailSubject">Mail subject</param>
        /// <param name="mailBody">Mail body</param>
        public static void SendMail(List<string> recievers, string sender, string mailSubject, string mailBody)
        {
            foreach (string reciever in recievers)
            {
                MailAddress from = new MailAddress(sender);
                MailAddress to = new MailAddress(reciever);
                MailMessage message = new MailMessage(sender, reciever);

                message.Subject = mailSubject;
                message.Body = mailBody;

                SmtpClient client = new SmtpClient(mailServer);
                client.Credentials = CredentialCache.DefaultNetworkCredentials;

                client.Send(message);
            }
        }

        #endregion

        #region EMail With Attachments

        /// <summary>
        /// Sends email to configured addresses
        /// </summary>
        /// <param name="recievers">recievers of email separated by semicolon</param>
        /// <param name="sender">Sender of email</param>
        /// <param name="mailSubject">MailSubject</param>
        /// <param name="mailBody">Mail body</param>
        /// <param name="attachmentFileName">Full path to attachment file</param>
        public static void SendMailWithAttachments(List<string> recievers, string mailSubject, string mailBody, string attachmentFileName)
        {
            if (isEmailToBeSent)
            {
                Attachment attachment = null;

                if (File.Exists(attachmentFileName))
                {
                    attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Octet);

                    ContentDisposition disposition = attachment.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(attachmentFileName);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachmentFileName);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(attachmentFileName);
                }

                foreach (string reciever in recievers)
                {
                    MailAddress from = new MailAddress(emailSender);
                    MailAddress to = new MailAddress(reciever);
                    MailMessage message = new MailMessage(emailSender, reciever);

                    message.Subject = mailSubject;
                    message.Body = mailBody;

                    if (attachment != null)
                    {
                        message.Attachments.Add(attachment);
                    }

                    SmtpClient client = new SmtpClient(mailServer);
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;

                    client.Send(message);
                }

                if (attachment != null)
                {
                    attachment.Dispose();
                }
            }
        }

        #endregion

        #region EMails Exception

        /// <summary>
        /// Sends exception info to collection of recievers defined in web.config
        /// </summary>
        /// <param name="mailSubject">Mail subject supplied with timestamp</param>
        /// <param name="mailBody">Mailbody containing assembly, function and stacktrace info</param>
        public static void SendException(string assembly, string function, string exception)
        {
            if (isExceptionToBeSent)
            {
                foreach (string reciever in emailRecieversOfExceptions)
                {
                    MailAddress from = new MailAddress(emailSender);
                    MailAddress to = new MailAddress(reciever);
                    MailMessage message = new MailMessage(emailSender, reciever);

                    message.Subject = string.Format(emailExceptionSubject, DateTime.Now.ToString());


                    //  Feilen inntraff i assembly: {0}{3}{3}Funksjon: {1}{3}{3}StackTrace: {2}{3}{3}"
                    message.Body = string.Format(emailExceptionBody, assembly, function, exception, Environment.NewLine);

                    SmtpClient client = new SmtpClient(mailServer);
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;

                    client.Send(message);
                }
            }
        }

        #endregion
    }
}