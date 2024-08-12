using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouteUpdateConsoleApplication.Helper
{
   public class RouteGeneration
    {
        public static void RunRouteGeneration()
        {
            try
            {
                CommonTask.GenerateLog("RunRouteGeneration started");
                //CommonTask.RunBatFilefromParticularLocation(Config.RouteGenerationBatFilePath);
                CommonTask.RunEXEfromParticularPath(Config.RouteGenerationExeFilePath);
                //CommonTask.StartStopIISInstance("Stop","N");
                CommonTask.SendMail(Config.UpdateReceiverEmails, "Route Generation Process has been completed and Puma is up. Please check and confirm if everything looks fine to proceed further");
                CommonTask.GenerateLog("RunRouteGeneration finished");
            }
            catch (Exception ex)
            {
                CommonTask.GenerateLog("RunRouteGeneration failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                CommonTask.SendMail(Config.AppTeamMail, "RunRouteGeneration failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);

            }
        }
    }
}
