using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RouteUpdateConsoleApplication.Helper
{
    public class RouteRecreation
    {
        public static void RunRouteRecreation()
        {
            try
            {
                CommonTask.GenerateLog("RunRouteReceation started");
                CommonTask.SendMail(Config.UpdateReceiverEmails, "We are taking Puma down. Route Recreation will start after that.");
                //CommonTask.StartStopIISInstance("Start", "N");
                CommonTask.RunEXEfromParticularPath(Config.RouteRecreationExePath);
                CheckandCopyWorstCaseFiles();
                CommonTask.SendMail(Config.UpdateReceiverEmails, "Route Recreation is completed successfully. Please find worst case reports on this location: " + Config.WorstCaseSharedPath);
                CommonTask.GenerateLog("RunRouteReceation finished");
            }
            catch (Exception ex)
            {
                CommonTask.GenerateLog("RunRouteReceation failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                CommonTask.SendMail(Config.AppTeamMail, "RunRouteReceation failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
            }
        }

        public static void CheckandCopyWorstCaseFiles()
        {
            try
            {
                string FileSourcePath = Config.WorstCaseReportsPath;

                string[] files = System.IO.Directory.GetFiles(FileSourcePath);
                foreach (string s in files)
                {
                    if (s.Contains("WorstCase") || s.Contains("summary") || s.Contains("gjenskaping") || s.Contains("verifisering"))
                    {
                        if ((File.GetCreationTime(s).Date == DateTime.Today) || (File.GetCreationTime(s).Date == DateTime.Now.AddDays(-1).Date))
                        {
                            string TargetPath = Config.WorstCaseSharedPath;
                            string fileName = System.IO.Path.GetFileName(s);
                            TargetPath = System.IO.Path.Combine(TargetPath, fileName);
                            System.IO.File.Copy(s, TargetPath, true);
                        }
                        else
                        {
                            try
                            {
                                CommonTask.RunEXEfromParticularPath(Config.LogWriterExeFilePath);
                                CommonTask.GenerateLog("Log Writer exe completed successfully");
                                CheckandCopyWorstCaseFiles();
                            }
                            catch (Exception ex)
                            {
                                CommonTask.GenerateLog("Log Writer has error " + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                                CommonTask.SendMail(Config.AppTeamMail, "Log Writer has error " + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                CommonTask.GenerateLog("CheckandCopyWorstCaseFiles Failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                CommonTask.SendMail(Config.PumaTeamMail, "CheckandCopyWorstCaseFiles Failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
            }
        }
    }
}
