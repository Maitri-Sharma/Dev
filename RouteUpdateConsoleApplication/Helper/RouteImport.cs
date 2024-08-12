using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Threading;


namespace RouteUpdateConsoleApplication.Helper
{
    public class RouteImport
    {
        public static void RunRouteImport()
        {
            try
            {
                CommonTask.SendMail(Config.UpdateReceiverEmails, "Route Import is started in Puma");
                CommonTask.GenerateLog("RunRouteImport Started  - ");
                //ConvertCSVToText();
                CommonTask.CopyFile(Config.RouteExtractPath, Config.RouteImportPath);
                CommonTask.RunEXEfromParticularPath(Config.RouteImportExePath);
                CopyLogFileToSharedPath();
                CommonTask.SendMail(Config.UpdateReceiverEmails, "Route Import Completeted successfully. Please find logs on this location: " + Config.RouteImportLogsSharedPath);
                CommonTask.GenerateLog("RunRouteImport Finished  - ");
                //Thread.Sleep(3600000);
            }
            catch (Exception exception)
            {
                CommonTask.GenerateLog("RunRouteImport Failed  - " + "\n" + "Message: " + exception.Message + "\n" + "StackTrace: " + exception.StackTrace);
                CommonTask.SendMail(Config.AppTeamMail, "RunRouteImport Failed - " + "<br>" + "Message: " + exception.Message + "<br>" + "StackTrace: " + exception.StackTrace);
                throw;
            }
        }

        private static void ConvertCSVToText()
        {
            try
            {
                CommonTask.GenerateLog("ConvertCSVToText Started  - ");
                string csvPath = Config.RouteExtractPath;
                string[] files = System.IO.Directory.GetFiles(csvPath);
                foreach (string p in files)
                {
                    string csvContentStr = File.ReadAllText(p);
                    string targettxt = Config.RouteImportPath;
                    string fileName = System.IO.Path.GetFileName(p);
                    fileName = fileName.Replace(".csv", ".txt");
                    if (fileName.Contains("Rutpunker"))
                    {
                        fileName = "Rute_Rutpunker.txt";
                    }
                    targettxt = System.IO.Path.Combine(targettxt, fileName);
                    File.WriteAllText(targettxt, csvContentStr);
                }
               
                UpdateConfig(csvPath);
                //CommonTask.GenerateLog("Copied File Successfully");
                CommonTask.GenerateLog("ConvertCSVToText Finished  - ");
            }
            catch (Exception ex)
            {
                CommonTask.GenerateLog("ConvertCSVToText Failed" + ex.Message);
                //CommonTask.GenerateLog("ConvertCSVToText Error  - " + ex.Message);
            }
        }

        public static void UpdateConfig(string filePath)
        {
            try
            {
                CommonTask.GenerateLog("UpdateConfig Started");
                string updatedstring = null;
                string a = "Rute_Antall", b = "Rute_RutePunkter", c = "Rute_AntallKommune", d = "Rute_BoksAnlegg";
                if ((Directory.EnumerateFiles(filePath).Any(f => f.IndexOf(a, StringComparison.OrdinalIgnoreCase) > 0)))
                {
                    //contains = Directory.EnumerateFiles(sourcePath).Any(f => f.Contains(a)); 
                    //|| Directory.EnumerateFiles(sourcePath).Any(g => g.Contains(b)) || Directory.EnumerateFiles(sourcePath).Any(h => h.Contains(c)) || Directory.EnumerateFiles(sourcePath).Any(i => i.Contains(d));
                    updatedstring = updatedstring + a + ";";
                }
                if ((Directory.EnumerateFiles(filePath).Any(f => f.IndexOf(b, StringComparison.OrdinalIgnoreCase) > 0)))
                {
                    updatedstring = updatedstring + b + ";";

                }
                if ((Directory.EnumerateFiles(filePath).Any(f => f.IndexOf(c, StringComparison.OrdinalIgnoreCase) > 0)))
                {
                    updatedstring = updatedstring + c + ";";
                }
                if ((Directory.EnumerateFiles(filePath).Any(f => f.IndexOf(d, StringComparison.OrdinalIgnoreCase) > 0)))
                {
                    updatedstring = updatedstring + d + ";";
                }
                updatedstring = updatedstring.Remove(updatedstring.Length - 1, 1);

                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = Config.RouteImportConfigPath;
                System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                var configValue = config.AppSettings.Settings["Import.DataSources"].Value;

                config.AppSettings.Settings["Import.DataSources"].Value = updatedstring;
                config.Save();
                CommonTask.GenerateLog("UpdateConfig Finished");
            }
            catch (Exception ex)
            {
                CommonTask.GenerateLog("UpdateConfig Failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                CommonTask.SendMail(Config.AppTeamMail, "UpdateConfig Failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                throw;
            }
        }
        public static void CopyLogFileToSharedPath()
        {
            try
            {
                CommonTask.GenerateLog("CopyLogFileToSharedPath Started  - ");
                string fileName, destFile;
                string sourcePath = Config.RouteImportLogs;
                string targetPath = Config.RouteImportLogsSharedPath;

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
                        //DateTime LastSaved;

                        System.DateTime LastSaved = System.IO.File.GetCreationTime(s);

                        if (LastSaved.Date == DateTime.Now.Date)
                        {
                            System.IO.File.Copy(s, destFile, true);
                        }
                    }
                    //CommonTask.SendMail("maitri.sharma@posten.no", "Route Import Completed by Automated Service");
                    CommonTask.GenerateLog("CopyLogFileToSharedPath Finished");
                }
                else
                {
                   CommonTask.GenerateLog("CopyLogFileToSharedPath failed: Source path does not exist. Source Path: " + sourcePath);
                   CommonTask.SendMail(Config.AppTeamMail, "CopyLogFileToSharedPath failed: Source path does not exist. Source Path: " + sourcePath);
                   System.Environment.Exit(0);
                }

            }
            catch (Exception ex)
            {
                CommonTask.GenerateLog("CopyLogFileToSharedPath Failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                CommonTask.SendMail(Config.AppTeamMail, "CopyLogFileToSharedPath Failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                throw;
            }
        }
    }
}


    
