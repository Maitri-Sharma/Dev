using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.CompilerServices;

namespace DataAccessAPI.Helper
{
    /// <summary>
    /// Class to send SMTP Emails
    /// </summary>
    public class EMail
    {
        #region Private Variables

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
        private static string emailSubject;
        private static string emailBodyNoMessages;
        private static string emailBodyNotFresh;
        private readonly IConfiguration _configuration;


        #endregion

        #region Constructor

        /// <summary>
        /// In constructor; get config.data
        /// </summary>
        public EMail(IConfiguration configuration)
        {
            string currentParameter = string.Empty;
            try
            {
                _configuration = configuration;
                currentParameter = "MailServer";
                mailServer = _configuration.GetValue<string>("MailServer");
                currentParameter = "EmailSender";
                emailSender = _configuration.GetValue<string>("EmailSender");
                currentParameter = "EmailRecieversOfExceptions";
                emailRecieverOfExceptions = _configuration.GetValue<string>("EmailRecieversOfExceptions");
                emailRecieversOfExceptions = emailRecieverOfExceptions.Split(new char[] { ';' });
                currentParameter = "EmailRecievers";
                emailReciever = _configuration.GetValue<string>("EmailRecievers");
                emailRecievers = emailReciever.Split(new char[] { ';' });
                currentParameter = "EmailExceptionSubject";
                emailExceptionSubject = _configuration.GetValue<string>("EmailExceptionSubject");
                currentParameter = "EmailExceptionBody";
                emailExceptionBody = _configuration.GetValue<string>("EmailExceptionBody");
                currentParameter = "EmailSubject";
                emailSubject = _configuration.GetValue<string>("EmailSubject");
                currentParameter = "EmailBodyNoMessages";
                emailBodyNoMessages = _configuration.GetValue<string>("EmailBodyNoMessages");
                currentParameter = "EmailBodyNotFresh";
                emailBodyNotFresh = _configuration.GetValue<string>("EmailBodyNotFresh");
                currentParameter = "IsEmailToBeSent";
                isEmailToBeSent = _configuration.GetValue<bool>("IsEmailToBeSent");
                currentParameter = "IsExceptionToBeSent";
                isExceptionToBeSent = _configuration.GetValue<bool>("IsExceptionToBeSent");
            }
            catch (Exception ex)
            {
                string error = string.Format("Restkapasitet Import.ErgoGroup.ToolBox.EMail.Constructor" + Microsoft.VisualBasic.Constants.vbCrLf + "Current parameter: {0}" + Microsoft.VisualBasic.Constants.vbCrLf + "Exception:" + Microsoft.VisualBasic.Constants.vbCrLf + "{1}", currentParameter, ex.Message);
                throw new ApplicationException(error);
            }
        }

        #endregion

        #region Email

        /// <summary>
        /// Sends email to configured addresses. Uses sender defined in web.config
        /// </summary>
        /// <param name="numberOfMessages">recievers of email in string collection</param>
        /// <param name="latestDate">MailSubject</param>
        public void SendMail(int numberOfMessages, DateTime latestDate)
        {
            if (isEmailToBeSent)
            {
                foreach (var reciever in emailRecievers)
                {
                    var from = new MailAddress(emailSender);
                    var to = new MailAddress(reciever);
                    var message = new MailMessage(emailSender, reciever);
                    message.Subject = emailSubject;
                    message.Body = string.Format(emailBodyNotFresh, numberOfMessages.ToString(), latestDate.ToString());
                    var client = new SmtpClient(mailServer);
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    client.Send(message);
                }
            }
        }

        /// <summary>
        /// Sends email to configured addresses. Sender must be specified
        /// </summary>
        public void SendMail(string msg)
        {
            foreach (var reciever in emailRecievers)
            {
                var from = new MailAddress(emailSender);
                var to = new MailAddress(reciever);
                var message = new MailMessage(emailSender, reciever);
                message.Subject = emailSubject;
                message.Body = msg;
                var client = new SmtpClient(mailServer);
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
                    var disposition = attachment.ContentDisposition;
                    disposition.CreationDate = File.GetCreationTime(attachmentFileName);
                    disposition.ModificationDate = File.GetLastWriteTime(attachmentFileName);
                    disposition.ReadDate = File.GetLastAccessTime(attachmentFileName);
                }

                foreach (var reciever in recievers)
                {
                    var from = new MailAddress(emailSender);
                    var to = new MailAddress(reciever);
                    var message = new MailMessage(emailSender, reciever);
                    message.Subject = mailSubject;
                    message.Body = mailBody;
                    if (attachment is object)
                    {
                        message.Attachments.Add(attachment);
                    }

                    var client = new SmtpClient(mailServer);
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    client.Send(message);
                }

                if (attachment is object)
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
        /// <param name="assembly">Mail subject supplied with timestamp</param>
        /// <param name="function">Mailbody containing assembly, function and stacktrace info</param>
        /// <param name="exception">Mailbody containing assembly, function and stacktrace info</param>
        public static void SendException(string assembly, string function, string exception)
        {
            if (isExceptionToBeSent)
            {
                foreach (var reciever in emailRecieversOfExceptions)
                {
                    var from = new MailAddress(emailSender);
                    var to = new MailAddress(reciever);
                    var message = new MailMessage(emailSender, reciever);
                    message.Subject = string.Format(emailExceptionSubject, DateTime.Now.ToString());
                    message.Body = string.Format(emailExceptionBody, assembly, function, exception, Environment.NewLine);
                    var client = new SmtpClient(mailServer);
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    client.Send(message);
                }
            }
        }

        #endregion
    }
}