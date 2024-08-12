using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace RouteUpdateConsoleApplication.Helper
{
    class PreAutoUpdate
    {
        public static void RunPreAutoUpdate()
        {
            try
            {
                Execute();
                CommonTask.SendMail(Config.UpdateReceiverEmails, "DRM file has been imported into PRS Plan");
                //DeleteOldReportsfromReportServer();
                //CommonTask.StartStopIISInstance("Start", "Y");
                //CommonTask.GenerateLog("RunPreAutoUpdate Finished");
            }
            catch (Exception ex)
            {
                CommonTask.GenerateLog("RunPreAutoUpdate Failed" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                CommonTask.SendMail(Config.AppTeamMail, "RunPreAutoUpdate Failed" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                throw ;
            }
        }

        private static void Execute()
        {
            try
            {
                HttpWebRequest request = CreateWebRequest();
                XmlDocument soapEnvelopeXml = new XmlDocument();


                soapEnvelopeXml.Load(Config.DRMXML);

                using (Stream stream = request.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }

                //System.Net.ServicePointManager.Expect100Continue = false;

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string soapResult = rd.ReadToEnd();
                        Console.WriteLine(soapResult);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonTask.GenerateLog("Execute DRM file Failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                CommonTask.SendMail(Config.AppTeamMail, "Execute DRM file Failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
            }
        }
        /// <summary>
        /// Create a soap webrequest to [Url]
        /// </summary>
        /// <returns></returns>
        private static HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Config.MDMUrl);
            //webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

    }
}
