using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace RouteUpdateConsoleApplication.Helper
{
    public class CommonTask
    {
        public static void RunEXEfromParticularPath(string exePath)
        {
            try
            {
                GenerateLog("RunEXEfromParticularPath started");
                Process p = new Process();
                p.StartInfo.FileName = exePath;
                p.StartInfo.Verb = "runas";
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.WaitForExit();
                if (p.ExitCode != 0)
                {
                    SendMail(Config.AppTeamMail, "RunEXEfromParticularPath not finished successfully. Exe Path: " + exePath);
                    p.Kill();
                }
                else
                    GenerateLog("RunEXEfromParticularPath finished" + exePath);
            }
            catch (Exception ex)
            {
                GenerateLog("RunEXEfromParticularPath failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                SendMail(Config.AppTeamMail, "RunEXEfromParticularPath Failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                throw;
            }

        }

        public static void GenerateLog(string logMessage)
        {
            try
            {
                StreamWriter log;
                FileStream fileStream = null;
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo;

                string logFilePath = Config.LogFilePath;
                logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
                logFileInfo = new FileInfo(logFilePath);
                logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                if (!logFileInfo.Exists)
                {
                    fileStream = logFileInfo.Create();
                }
                else
                {
                    fileStream = new FileStream(logFilePath, FileMode.Append);
                }
                log = new StreamWriter(fileStream);
                log.WriteLine(logMessage);
                log.Close();
            }
            catch (Exception ex)
            {
                SendMail(Config.AppTeamMail, "GenerateLog failed" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
            }
        }

        public static void CopyFile(string sourcePath, string targetPath)
        {
            try
            {
                GenerateLog("CopyFile Started");
                string fileName, destFile;
                System.IO.Directory.CreateDirectory(targetPath);

                if (System.IO.Directory.Exists(sourcePath))
                {
                    string[] files = System.IO.Directory.GetFiles(sourcePath);

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(targetPath, fileName);
                        //DateTime dt = File.GetLastWriteTime(destFile);
                            System.IO.File.Copy(s, destFile, true);
                            GenerateLog("CopyFile finished from " + sourcePath);
                    }
                    RouteImport.UpdateConfig(sourcePath);
                }
                else
                {
                    //Console.WriteLine("Source path does not exist!");
                    GenerateLog("CopyFile failed: Source path does not exist. Source Path: " + sourcePath);
                    SendMail(Config.AppTeamMail, "CopyFile failed: Source path does not exist. Source Path: " + sourcePath);
                    System.Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                GenerateLog("CopyFile failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                SendMail(Config.AppTeamMail, "CopyFile failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                throw;
            }
        }
        public static void SendMail(string toEmailAddress, string mailBody)
        {
            try
            {
            string[] MultiEmails = toEmailAddress.Split(',');
            
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@posten.no");
            foreach (string ToEmail in MultiEmails)
            {
                mailMessage.To.Add(new MailAddress(ToEmail)); //adding multiple email addresses
            }
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = mailBody;
            mailMessage.Subject = Config.MailSubject;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Port = 25; //also tried 25 and 587,465
            smtpClient.Host = "scan.posten.no"; //also tried smtp-mail.outlook.com
            smtpClient.EnableSsl = false;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                GenerateLog("SendMail Failed -" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                SendMail(Config.PumaTeamMail, "SendMail Failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                throw;
            }
        }
        
        public static void CheckLogFile(string filePath)
        {
            var reader = new StreamReader(filePath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.ToLower().Contains("failed") || line.ToLower().Contains("error") || line.ToLower().Contains("exception"))
                {
                    System.Environment.Exit(0);
                }
            }
        }
    }

}
