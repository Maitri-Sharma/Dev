using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Xml.SerMessageTypeialization;
using System.Threading;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;

namespace DataAccessAPI.ECPumaHelper
{
    /// <summary>
    /// Class handling logging information for DataAccessAPI
    /// </summary>
    public class FileLogging
    {
        #region Private variables

        private static bool doLogging = false;
        private static bool doLoggingToQueue = false;
        private static bool doObjectLogging = false;

        private static bool doObjectLoggingRequestFromClient = false;
        private static bool doObjectLoggingResponseToClient = false;
        private static bool doObjectLoggingResponseFromGD = false;
        private static bool doObjectLoggingResponseToQueue = false;

        private static bool doTimeUsedLogging = false;
        private static bool doDeleteLogFiles = false;
        private static bool doDeleteObjectLogFiles = false;

        private static string fileLogPath = string.Empty;
        private static string fileLogExtension = string.Empty;
        private static string fileLogName = string.Empty;

        private static string objectLogPath = string.Empty;
        private static string objectLogExtension = string.Empty;

        private static int daysToKeepLogFiles = 0;
        private static int daysToKeepLogObjects = 0;

        private static int maxNumberOfRetries = 5;
        private static int numberOfMillisecondsToSleep = 1000;

        #endregion

        #region Constructor

        /// <summary>
        /// Get data from config file
        /// Rename loggfile if nescessary
        /// Delete loggfiles if old enough
        /// </summary>
        static FileLogging()
        {
            try
            {
                AppSettingsReader appdata = new AppSettingsReader();

                doLogging = Startup.StaticConfiguration.GetValue<bool>("FileLogging.DoLogging");

                doLoggingToQueue = Startup.StaticConfiguration.GetValue<bool>("FileLogging.DoLoggingToQueue");
                doObjectLogging = Startup.StaticConfiguration.GetValue<bool>("FileLogging.DoObjectLogging");

                doObjectLoggingRequestFromClient = Startup.StaticConfiguration.GetValue<bool>("FileLogging.DoObjectLogging.RequestFromClient");
                doObjectLoggingResponseToClient = Startup.StaticConfiguration.GetValue<bool>("FileLogging.DoObjectLogging.ResponseToClient");
                doObjectLoggingResponseFromGD = Startup.StaticConfiguration.GetValue<bool>("FileLogging.DoObjectLogging.ResponseFromGD");
                doObjectLoggingResponseToQueue = Startup.StaticConfiguration.GetValue<bool>("FileLogging.DoObjectLogging.ResponseToQueue");

                doTimeUsedLogging = Startup.StaticConfiguration.GetValue<bool>("FileLogging.DoTimeUsedLogging");

                doDeleteLogFiles = Startup.StaticConfiguration.GetValue<bool>("FileLogging.DoDeleteLogFiles");
                doDeleteObjectLogFiles = Startup.StaticConfiguration.GetValue<bool>("FileLogging.DoDeleteObjectLogFiles");

                fileLogPath = Startup.StaticConfiguration.GetValue<string>("FileLogging.FileLogPath");
                fileLogName = Startup.StaticConfiguration.GetValue<string>("FileLogging.FileLogName");
                fileLogExtension = Startup.StaticConfiguration.GetValue<string>("FileLogging.FileLogExtension");

                objectLogPath = Startup.StaticConfiguration.GetValue<string>("FileLogging.ObjectLogPath");
                objectLogExtension = Startup.StaticConfiguration.GetValue<string>("FileLogging.ObjectLogExtension");

                daysToKeepLogFiles = Startup.StaticConfiguration.GetValue<int>("FileLogging.DaysToKeepLogFiles");
                daysToKeepLogObjects = Startup.StaticConfiguration.GetValue<int>("FileLogging.DaysToKeepLogObjects");

                maxNumberOfRetries = Startup.StaticConfiguration.GetValue<int>("FileLogging.MaxNumberOfRetries");
                numberOfMillisecondsToSleep = Startup.StaticConfiguration.GetValue<int>("FileLogging.NumberOfMillisecondsToSleep");

                CreateDirectory(fileLogPath);
                CreateDirectory(objectLogPath);

            }
            catch (Exception ex)
            {
                WriteMessage(ex.Message);
            }
        }

        #endregion

        #region Public property

        /// <summary>
        /// 
        /// </summary>
        public enum MessageType
        {
            InfoMessage,
            ErrorMessage,
            ApplicationExceptionMessage,
            ExceptionMessage,
            TimeDurationMessage,
            MessageFromQueue
        }

        /// <summary>
        /// Exposes different types of logfiles
        /// </summary>
        public enum TypeOfObject
        {
            RequestFromClient,
            ResponseToClient,
            ResponseFromGD,
            ResponseToQueue
        }

        #endregion

        #region Public Logging methods

        #region LogStart

        /// <summary>
        /// Logs start of operation
        /// </summary>
        /// <param name="applicationName"></param>
        public static void LogStart(string applicationName, string IP_Address)
        {
            if (doLogging)
            {
                string separator = new string('-', 50);
                string message = string.Format("{0}\r\n{1} {2}\r\n{3} starter logging. Avsenders IP-adresse: {4}\r\n", separator, DateTime.Now.ToString(), MessageType.InfoMessage.ToString(), applicationName, IP_Address);

                WriteMessage(message);
            }
        }

        #endregion

        #region LogEnd

        /// <summary>
        /// Logs end of operation
        /// </summary>
        /// <param name="applicationName"></param>
        public static void LogEnd(string applicationName)
        {
            if (doLogging)
            {
                string message = string.Format("{0} {1}\r\n{2} avslutter logging...\r\n", DateTime.Now.ToString(), MessageType.InfoMessage.ToString(), applicationName);
                WriteMessage(message);
            }
        }

        #endregion

        #region LogException

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public static void LogException(Exception exception)
        {
            if (doLogging)
            {
                //string exceptionMessage = CreateExceptionString(exception);
                //string exceptionMessage = CreateExceptionMessageDetails(exception);
                string exceptionMessage = CreateExceptionLogString(exception);                          
                WriteMessage(exceptionMessage);
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="qEx"></param>
        //public static void LogException(IBM.WMQ.MQException qEx)
        //{
        //    string exceptionMessage = CreateExceptionLogString(qEx);
        //    WriteMessage(exceptionMessage);
        //}

        #endregion

        #region LogTimeUsed

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="whatToMeasure"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void LogTimeUsed(string whatToMeasure, DateTime start, DateTime end)
        {
            if (doLogging)
            {
                if (doTimeUsedLogging)
                {
                    char pad = '0';
                    TimeSpan timeUsed = end - start;

                    string timeUsedString = string.Format("Tidsforbruk: {0} dager {1}:{2}:{3}.{4} ",
                        timeUsed.Days.ToString(),
                        timeUsed.Hours.ToString().PadLeft(2, pad),
                        timeUsed.Minutes.ToString().PadLeft(2, pad),
                        timeUsed.Seconds.ToString().PadLeft(2, pad),
                        timeUsed.Milliseconds.ToString().PadLeft(3, pad));

                    string message = string.Format("{0} {1}\r\nMålt: {2}\r\n{3}\r\n",
                        DateTime.Now.ToString(),
                        MessageType.TimeDurationMessage.ToString(),
                        whatToMeasure,
                        timeUsedString);

                    WriteMessage(message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whatToMeasure"></param>
        /// <param name="timeSpent"></param>
        public static void LogTimeUsed(string whatToMeasure, TimeSpan timeSpent)
        {
            if (doTimeUsedLogging)
            {
                char pad = '0';

                string timeUsedString = string.Format("Tidsforbruk: {0} dager {1}:{2}:{3}.{4} ",
                    timeSpent.Days.ToString(),
                    timeSpent.Hours.ToString().PadLeft(2, pad),
                    timeSpent.Minutes.ToString().PadLeft(2, pad),
                    timeSpent.Seconds.ToString().PadLeft(2, pad),
                    timeSpent.Milliseconds.ToString().PadLeft(3, pad));

                string message = string.Format("{0} {1}\r\nMålt: {2}\r\n{3}\r\n",
                    DateTime.Now.ToString(),
                    MessageType.TimeDurationMessage.ToString(),
                    whatToMeasure,
                    timeUsedString);

                WriteMessage(message);
            }
        }

        #endregion

        #region LogObject

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="typeOfObject"></param>
        /// <param name="obj"></param>
        public static void LogObject(string application, TypeOfObject typeOfObject, object obj)
        {
            if (doLogging)
            {
                if (doObjectLogging)
                {
                    switch (typeOfObject)
                    {
                        case TypeOfObject.RequestFromClient:
                            if (! doObjectLoggingRequestFromClient)
                            { return; }
                            break;

                        case TypeOfObject.ResponseToClient:
                            if (!doObjectLoggingResponseToClient)
                            { return; }
                            break;

                        case TypeOfObject.ResponseFromGD:
                            if (!doObjectLoggingResponseFromGD)
                            { return; }
                            break;

                        case TypeOfObject.ResponseToQueue:
                            if (!doObjectLoggingResponseToQueue)
                            { return; }
                            break;
                    }

                    string requestObject = Serialize.ToUTF8String(obj);
                    string messageFileName = CreateLogObjectFileName(application, typeOfObject);

                    string logMessage = string.Format("{0} {1} har lengde: {2}. Filnavn: {3}", application, typeOfObject.ToString(), requestObject.Length.ToString(), messageFileName);
                    LogMessage(logMessage, MessageType.InfoMessage);

                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.LoadXml(requestObject);

                    doc.PreserveWhitespace = true;
                    doc.Save(messageFileName);
                }
            }
        }

        #endregion

        #region LogMessage

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void LogMessage(string message, MessageType type)
        {
            if (doLogging)
            {
                message = string.Format("{0} {1}\r\n{2}\r\n", DateTime.Now.ToString(), type.ToString(), message);
                WriteMessage(message);
            }
        }

        #endregion

        #region LogMessageFromQueue

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void LogMessageFromQueue(string message, MessageType type)
        {
            if (doLogging)
            {
                if (doLoggingToQueue)
                {
                    message = string.Format("{0} {1}\r\n{2}\r\n", DateTime.Now.ToString(), type.ToString(), message);
                    WriteMessage(message);
                }
            }
        }

        #endregion

        #endregion

        #region WriteMessage

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private static void WriteMessage(string message)
        {
            string logFileFullName = CreateFileLogName();

            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            int numberOfRetries = 0;

            try
            {
                if (!File.Exists(logFileFullName))
                {
                    LogFileMaintenance();
                }

                while (numberOfRetries < maxNumberOfRetries)
                {
                    try
                    {
                        WriteToStream(logFileFullName, message, ref fileStream, ref streamWriter);
                        numberOfRetries = int.MaxValue;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(numberOfMillisecondsToSleep);
                        numberOfRetries += 1;
                    }
                }
                   
            }
            catch (Exception ex)
            {
                DisposeStream(fileStream, streamWriter, ex);
            }
        }

        #endregion

        #region WriteToStream

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFileFullName"></param>
        /// <param name="message"></param>
        /// <param name="fileStream"></param>
        /// <param name="streamWriter"></param>
        private static void WriteToStream(string logFileFullName, string message, ref FileStream fileStream, ref StreamWriter streamWriter)
        {
            fileStream = new FileStream(logFileFullName, FileMode.Append, FileAccess.Write, FileShare.Write);
            streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(message);

            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        #endregion

        #region DisposeStream

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="streamWriter"></param>
        /// <param name="ex"></param>
        private static void DisposeStream(FileStream fileStream, StreamWriter streamWriter, Exception ex)
        {
            if (streamWriter != null)
            {
                streamWriter.Flush();
                streamWriter.Close();
            }

            if (fileStream != null)
            {
                fileStream.Close();
            }
        }

        #endregion

        #region Logfile Maintenance

        #region LogFileMaintenance

        /// <summary>
        /// Performs logfile maintenance
        /// </summary>
        private static void LogFileMaintenance()
        {
            BackgroundWorker bwDeleteLogFiles = new BackgroundWorker();
            bwDeleteLogFiles.WorkerSupportsCancellation = false;
            bwDeleteLogFiles.WorkerReportsProgress = false;
            bwDeleteLogFiles.DoWork+=new DoWorkEventHandler(bwDeleteLogFiles_DoWork);
            bwDeleteLogFiles.RunWorkerAsync();

            BackgroundWorker bwDeleteObjectLogFiles = new BackgroundWorker();
            bwDeleteObjectLogFiles.WorkerSupportsCancellation = false;
            bwDeleteObjectLogFiles.WorkerReportsProgress = false;
            bwDeleteObjectLogFiles.DoWork+=new DoWorkEventHandler(bwDeleteObjectLogFiles_DoWork);
            bwDeleteObjectLogFiles.RunWorkerAsync();
        }

        #endregion

        #region bwDeleteLogFiles_DoWork

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void bwDeleteLogFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            //LogMessage("Sletting av loggfiler påbegynnes", MessageType.InfoMessage);
            int numberOfDeletedLogFiles = DeleteLogFiles();
            //LogMessage(string.Format("Sletting av loggfiler avsluttet. Antall loggfiler slettet: {0}", numberOfDeletedLogFiles.ToString()), MessageType.InfoMessage);
        }

        #endregion

        #region bwDeleteObjectLogFiles_DoWork

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void bwDeleteObjectLogFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            //LogMessage("Sletting av objektloggfiler påbegynnes", MessageType.InfoMessage);
            int numberOfDeletedLogFiles = DeleteObjectLogFiles();
            //LogMessage(string.Format("Sletting av objektloggfiler avsluttet. Antall objektloggfiler slettet: {0}", numberOfDeletedLogFiles.ToString()), MessageType.InfoMessage);
        }

        #endregion

        #region DeleteLogFiles

        /// <summary>
        /// 
        /// </summary>
        public static int DeleteLogFiles()
        {
            int deletedFiles = 0;

            if (doDeleteLogFiles)
            {
                string[] logFiles = GetLogFiles(fileLogExtension);

                foreach (string logFile in logFiles)
                {
                    FileInfo logFileData = new FileInfo(logFile);

                    DateTime created = logFileData.CreationTime;

                    if (created.AddDays(daysToKeepLogFiles) < DateTime.Now)
                    {
                        try
                        {
                            File.Delete(logFile);
                            deletedFiles += 1;

                            //string message = string.Format("Slettet logfile: {0}", logFile);
                            //WriteMessage(message);
                        }
                        catch (Exception)
                        {
                            //DO nothing
                        }
                    }
                }
            }

            return deletedFiles;
        }

        #endregion

        #region DeleteObjectLogFiles

        /// <summary>
        /// Deletes Object logfiles if older than specified number of days
        /// </summary>
        public static int  DeleteObjectLogFiles()
        {
            int deletedFiles = 0;

            if (doDeleteObjectLogFiles)
            {
                string [] objectlogFiles = GetObjectLogFiles(objectLogExtension);

                foreach (string objectlogFile in objectlogFiles)
                {
                    FileInfo objectlogFileData = new FileInfo(objectlogFile);

                    DateTime created = objectlogFileData.CreationTime;

                    if (created.AddDays(daysToKeepLogObjects) < DateTime.Now)
                    {
                        try
                        {
                            File.Delete(objectlogFile);
                            deletedFiles += 1;

                            //string message = string.Format("Slettet objektlogg: {0}", objectlogFile);
                            //WriteMessage(message);
                        }
                        catch (Exception)
                        {
                            //Do nothing
                        }
                    }
                }
            }

            return deletedFiles;
        }

        #endregion

        #endregion

        #region Private Utilities

        #region CreateLogObjectFileName

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="typeOfObject"></param>
        /// <returns></returns>
        private static string CreateLogObjectFileName(string application, TypeOfObject typeOfObject)
        {
            char pad = '0';
            DateTime current = DateTime.Now;

            string fileName = string.Format("{0}_{1}_{2}_{3}_{4}_{5}.{6}",
                application,
                typeOfObject.ToString(),
                current.ToString("yyyyMMdd"),
                current.ToLongTimeString().Replace(":", string.Empty),
                current.Millisecond.ToString().PadLeft(3, pad),
                current.Ticks.ToString(),
                objectLogExtension
                );

            fileName = string.Format(@"{0}\{1}", objectLogPath, fileName);
            return fileName;
        }

        #endregion

        #region CreateFileLogName

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string CreateFileLogName()
        {
            return string.Format("{0}{1}_{2}.{3}", 
                fileLogPath, 
                fileLogName, 
                DateTime.Now.ToString("yyyyMMdd"), 
                fileLogExtension);
        }

        #endregion

        #region CreateExceptionString

        private static string CreateExceptionLogString(Exception exception)
        {
            string message = CreateExceptionMessageDetails(exception);
            return string.Format("{0}\r\n{1}\r\n", DateTime.Now.ToString(), message);
        }

        //private static string CreateExceptionLogString(IBM.WMQ.MQException qEx)
        //{
        //    string message = CreateQueueExceptionMessageDetails(qEx);
        //    return string.Format("{0}\r\n{1}\r\n", DateTime.Now.ToString(), message);
        //}



        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        //private static string CreateExceptionString(Exception exception)
        //{
        //    string exceptionType = exception.GetType().ToString();
        //    string innerExceptionMessage = "InnerException: Null\r\n";

        //    string exceptionMessage = string.Format("ExceptionType: {0}\r\nMessage: {1}\r\nStackTrace: {2}\r\n",
        //        exceptionType,
        //        exception.Message,
        //        exception.StackTrace);

        //    if (exception.InnerException != null)
        //    {
        //        string innerExceptionType = exception.InnerException.GetType().ToString();

        //        innerExceptionMessage = string.Format("InnerExceptionType: {0}\r\nMessage: {1}\r\nStackTrace: {2}\r\n",
        //            innerExceptionType,
        //            exception.InnerException.Message,
        //            exception.InnerException.StackTrace);
        //    }

        //    return string.Format("{0} {1}\r\n{2}\r\n{3}",
        //        DateTime.Now.ToString(),
        //        MessageType.ExceptionMessage.ToString(),
        //        exceptionMessage,
        //        innerExceptionMessage);
        //}

        #endregion

        #region CreateExceptionMessageDetails

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static string CreateExceptionMessageDetails(Exception exception)
        {
            string message = string.Format("Exceptiontype: {0}.\r\nMessage: {1}\r\nStacktrace: {2}\r\n{3}",
                exception.GetType().ToString(),
                exception.Message,
                exception.StackTrace,
                GetInnerExceptionDetails(exception.InnerException));

            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qEx"></param>
        /// <returns></returns>
        //private static string CreateQueueExceptionMessageDetails(IBM.WMQ.MQException qEx)
        //{
        //    string message = string.Format("Exceptiontype: {0}.\r\nMessage: {1}\r\nCompletionCode: {2}, ReasonCode: {3}\r\nStacktrace: {4}\r\n{5}",
        //        qEx.GetType().ToString(),
        //        qEx.Message, 
        //        qEx.CompletionCode.ToString(), 
        //        qEx.ReasonCode.ToString(),
        //        qEx.StackTrace,
        //        GetInnerExceptionDetails(qEx.InnerException));

        //    return message;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerException"></param>
        /// <returns></returns>
        private static string GetInnerExceptionDetails(Exception innerException)
        {
            string message = string.Empty;

            if (innerException != null)
            {
                message = string.Format("InnerExceptionType: {0}.\r\nInnerExceptionMessage: {1}\r\nInnerExceptionStacktrace: {2}",
                    innerException.GetType().ToString(),
                    innerException.Message,
                    innerException.StackTrace);
            }
            else
            {
                message = "Inner Exception: er lik NULL";
            }

            return message;
        }

        #endregion

        #region CreateLogFileArchiveName

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string CreateLogFileArchiveName()
        {
            char pad = '0';
            DateTime current = DateTime.Now;

            string fileLogFullName = CreateFileLogName();
            string directoryName = Path.GetDirectoryName(fileLogFullName);
            string fileName = Path.GetFileNameWithoutExtension(fileLogFullName);
            string fileExtension = Path.GetExtension(fileLogFullName);

            string archiveName = string.Format(@"{0}\{1}_{2}_{3}_{4}{5}",
                directoryName,
                fileName,
                current.ToShortDateString().Replace(".", string.Empty),
                current.ToLongTimeString().Replace(":", string.Empty),
                current.Millisecond.ToString().PadLeft(3, pad),
                fileExtension
                );

            return archiveName;
        }

        #endregion

        #region CreateDirectory

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        #endregion

        #endregion

        #region Public Debug Methods

        #region GetObjectLogFiles
      
        /// <summary>
        /// Returns collection of request / response logfilenames of specified type
        /// </summary>
        /// <returns></returns>
        public static string[] GetObjectLogFiles(string filter)
        {
            FileInfo objectLogFileData = new FileInfo(objectLogPath);
            string logPath = objectLogFileData.DirectoryName;

            string[]files = Directory.GetFiles(logPath, filter);
            //string message = string.Format("{0} filer hentet fra katalog '{1}' og filter '{2}'", files.Length.ToString(), logPath, filter);
            //FileLogging.LogMessage(message, MessageType.InfoMessage);

            return files;
        }

        #endregion

        #region GetLogFiles

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string[] GetLogFiles(string filter)
        {
            FileInfo logFileData = new FileInfo(fileLogPath);
            string logPath = logFileData.DirectoryName;
            filter = string.Format("*.{0}", filter);

            return Directory.GetFiles(logPath, filter);
        }

        #endregion

        #endregion

    }
}
