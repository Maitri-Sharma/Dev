using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Configuration;

namespace RouteUpdateConsoleApplication.Helper
{
    public class Config
    {
        //Route Import
        public static string RouteExtractPath
        {
            get { return ConfigurationManager.AppSettings["RouteExtractPath"]; }

        }
        public static string RouteImportDirectory
        {
            get { return ConfigurationManager.AppSettings["RouteImportDirectory"]; }

        }
        public static string RouteImportPath
        {
            get { return ConfigurationManager.AppSettings["RouteImportPath"]; }

        }
        public static string RouteImportConfigPath
        {
            get { return ConfigurationManager.AppSettings["RouteImportConfigPath"]; }

        }
        public static string RouteImportExePath
        {
            get { return ConfigurationManager.AppSettings["RouteImportExePath"]; }

        }
        public static string RouteImportBatFilePath
        {
            get { return ConfigurationManager.AppSettings["RouteImportBatFilePath"]; }

        }
        public static string RouteImportLogs
        {
            get { return ConfigurationManager.AppSettings["RouteImportLogs"]; }

        }
        public static string RouteImportLogsSharedPath
        {
            get { return ConfigurationManager.AppSettings["RouteImportLogsSharedPath"]; }

        }
        public static string PIBImportFilePath
        {
            get { return ConfigurationManager.AppSettings["PIBImportFilePath"]; }

        }
        public static string BackupBoksanlegg
        {
            get { return ConfigurationManager.AppSettings["BackupBoksanlegg"]; }

        }

        //Route Generation
        public static string RouteGenerationBatFilePath
        {
            get { return ConfigurationManager.AppSettings["RouteGenerationBatFilePath"]; }

        }
        public static string RouteGenerationExeFilePath
        {
            get { return ConfigurationManager.AppSettings["RouteGenerationExeFilePath"]; }

        }

        //Route Recreation
        public static string RouteRecreationExePath
        {
            get { return ConfigurationManager.AppSettings["RouteRecreationExePath"]; }

        }
        public static string RouteRecreationBatFilePath
        {
            get { return ConfigurationManager.AppSettings["RouteRecreationBatFilePath"]; }

        }
        public static string WorstCaseReportsPath
        {
            get { return ConfigurationManager.AppSettings["WorstCaseReportsPath"]; }

        }
        public static string LogWriterBatFilePath
        {
            get { return ConfigurationManager.AppSettings["LogWriterBatFilePath"]; }

        }
        public static string LogWriterExeFilePath
        {
            get { return ConfigurationManager.AppSettings["LogWriterExeFilePath"]; }

        }
        public static string WorstCaseSharedPath
        {
            get { return ConfigurationManager.AppSettings["WorstCaseSharedPath"]; }

        }

        //Common Paths
        public static string LogFilePath
        {
            get { return ConfigurationManager.AppSettings["LogFilePath"]; }

        }
        public static string CmdPath
        {
            get { return ConfigurationManager.AppSettings["CmdPath"]; }

        }
        public static string IISBatFilePath
        {
            get { return ConfigurationManager.AppSettings["IISBatFilePath"]; }

        }

        //ReportServicePath
        public static string ReportServiceStartBatFilePath
        {
            get { return ConfigurationManager.AppSettings["ReportServiceStartBatFilePath"]; }

        }
        public static string ReportServiceStopBatFilePath
        {
            get { return ConfigurationManager.AppSettings["ReportServiceStopBatFilePath"]; }

        }
        public static string TempReports
        {
            get { return ConfigurationManager.AppSettings["TempReports"]; }

        }
        public static string OldReports
        {
            get { return ConfigurationManager.AppSettings["OldReports"]; }

        }

        //Route Update Mail
        public static string MailSubject
        {
            get { return ConfigurationManager.AppSettings["MailSubject"]; }

        }
        public static string DBTeamEmail
        {
            get { return ConfigurationManager.AppSettings["DBTeamEmail"]; }

        }
        public static string DBBackupRequestBody
        {
            get { return ConfigurationManager.AppSettings["DBBackupRequestBody"]; }

        }

        public static string UpdateReceiverEmails
        {
            get { return ConfigurationManager.AppSettings["UpdateReceiverEmails"]; }

        }
        public static string PumaTeamMail
        {
            get { return ConfigurationManager.AppSettings["PumaTeamMail"]; }

        }
        public static string AppTeamMail
        {
            get { return ConfigurationManager.AppSettings["AppTeamMail"]; }

        }


        //DRM File Extract
        public static string DRMXML
        {
            get { return ConfigurationManager.AppSettings["DRMXML"]; }
        }
        public static string MDMUrl
        {
            get { return ConfigurationManager.AppSettings["MDMUrl"]; }
        }
        public static string PMSUrl
        {
            get { return ConfigurationManager.AppSettings["PMSUrl"]; }
        }


    }
}
