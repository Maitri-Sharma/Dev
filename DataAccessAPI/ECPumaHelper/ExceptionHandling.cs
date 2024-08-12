using System;
using System.Configuration;
using System.ServiceModel;
//using ErgoGroup.MS.Framework.Logging;
//using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;

namespace DataAccessAPI.ECPumaHelper
{
    /// <summary>
    /// General purpose exception handler. Returns ValidationFault messages
    /// </summary>
    public class ExceptionHandling
    {
        #region Private variables

        private static string generalApplicationErrorText = "Det har oppstått en applikasjonsfeil. Vennligst kontakt systemansvarlig for detaljer.";
        private static string communicationErrorText = "Det har oppstått en kommunikasjonsfeil. Vennligst kontakt systemansvarlig for detaljer.";
        private static string formatExceptionErrorText = "Det har oppstått en formatfeil. Vennligst kontakt systemansvarlig for detaljer.";
        private static string mqExceptionErrorText = "Det har oppstått en feil i forbindelse med lagring til kø. Vennligst kontakt systemansvarlig for detaljer.";
        private static string invalidOperationExceptionText = "Det har oppstått en applikasjonsfeil. Vennligst kontakt systemansvarlig for detaljer.";
        private static string argumentNullExceptionText = "Det har oppstått en applikasjonsfeil. Vennligst kontakt systemansvarlig for detaljer.";

        private static AppSettingsReader reader = null;
        private static bool isExceptionDetailToBeSentToClient = false;

        #endregion

        #region Constructor

        /// <summary>
        /// In constructor; get config data
        /// </summary>
        static ExceptionHandling()
        {
            reader = new AppSettingsReader();
            isExceptionDetailToBeSentToClient = (bool)reader.GetValue("IsExceptionDetailToBeSentToClient", typeof(bool));
        }

        #endregion

        #region HandleException

        /// <summary>
        /// Checks for type of exception and handle each according to different handlers
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="function"></param>
        /// <param name="exception"></param>
        public static ValidationFault HandleException(string assembly, string function, Exception exception)
        {
            try
            {
                string exceptionName = exception.GetType().Name;
                ValidationFault fault;

                switch (exceptionName)
                {
                    case "ApplicationException":
                        fault = ApplicationExceptionHandling(assembly, function, (ApplicationException)exception);
                        break;

                    case "ArgumentNullException":
                        fault = ArgumentNullExceptionHandling(assembly, function, (ArgumentNullException)exception);
                        break;

                    case "TimeoutException":
                        fault = TimeoutExceptionHandling(assembly, function, (TimeoutException)exception);
                        break;

                    case "CommunicationException":
                        fault = CommunicationExceptionHandling(assembly, function, (CommunicationException)exception);
                        break;

                    case "FormatException":
                        fault = FormatExceptionHandling(assembly, function, (FormatException)exception);
                        break;

                    case "InvalidOperationException":
                        fault = InvalidOperationExceptionHandling(assembly, function, (InvalidOperationException)exception);
                        break;

                    case "MQException":
                        fault = MQExceptionHandling(assembly, function, (IBM.WMQ.MQException)exception);
                        break;

                    case "NotSupportedException":
                        fault = NotSupportedExceptionHandling(assembly, function, (NotSupportedException) exception);
                        break;

                    default:
                        fault = GeneralExceptionHandling(assembly, function, (Exception)exception);
                        break;
                }

                return fault;
            }
            catch (Exception ex)
            {
                FileLogging.LogException(ex);
                EMail.SendException(assembly, function, CreateExceptionMessageDetails(ex));

                string returnMessage = generalApplicationErrorText;

                ValidationFault whenEveryThingElseFailsFault = CreateValidationFault(assembly, function, "GeneralException", returnMessage);
                return whenEveryThingElseFailsFault;
            }
        }

        #endregion

        #region NotSupportedExceptionHandling

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="function"></param>
        /// <param name="notSupportedException"></param>
        /// <returns></returns>
        private static ValidationFault NotSupportedExceptionHandling(string assembly, string function, NotSupportedException notSupportedException)
        {
            FileLogging.LogException(notSupportedException);
            EMail.SendException(assembly, function, CreateExceptionMessageDetails(notSupportedException));

            string returnMessage = notSupportedException.Message;

            if (isExceptionDetailToBeSentToClient)
            {
                returnMessage = notSupportedException.Message;
            }

            return CreateValidationFault(assembly, function, "NotSupportedExceptionHandling", notSupportedException.Message);
        }

        #endregion

        #region ArgumentNullExceptionHandling (sender ikke epost)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="function"></param>
        /// <param name="argumentNullException"></param>
        private static ValidationFault ArgumentNullExceptionHandling(string assembly, string function, ArgumentNullException argumentNullException)
        {
            FileLogging.LogException(argumentNullException);

            string returnMessage = argumentNullExceptionText;

            if (isExceptionDetailToBeSentToClient)
            {
                returnMessage = argumentNullException.Message;
            }

            return CreateValidationFault(assembly, function, "ArgumentNullException", returnMessage);
        }

        #endregion

        #region TimeoutExceptionHandling  (sender epost)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="function"></param>
        /// <param name="timeoutException"></param>
        /// <returns></returns>
        private static ValidationFault TimeoutExceptionHandling(string assembly, string function, TimeoutException timeoutException)
        {
            FileLogging.LogException(timeoutException);
            EMail.SendException(assembly, function, CreateExceptionMessageDetails(timeoutException));

            string returnMessage = timeoutException.Message;

            if (isExceptionDetailToBeSentToClient)
            {
                returnMessage = timeoutException.Message;
            }

            return CreateValidationFault(assembly, function, "TimeoutException", timeoutException.Message);

        }

        #endregion

        #region ApplicationExceptionHandling (sender ikke epost)

        /// <summary>
        /// Returns by default always applicationException.Message
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="function"></param>
        /// <param name="aEx"></param>
        private static ValidationFault ApplicationExceptionHandling(string assembly, string function, ApplicationException applicationException)
        {
            FileLogging.LogException(applicationException);

            string returnMessage = applicationException.Message;

            if (isExceptionDetailToBeSentToClient)
            {
                returnMessage = applicationException.Message;
            }

            return CreateValidationFault(assembly, function, "ApplicationException", returnMessage);
        }

        #endregion

        #region CommunicationExceptionHandling (sender epost)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="function"></param>
        /// <param name="cEx"></param>
        private static ValidationFault CommunicationExceptionHandling(string assembly, string function, CommunicationException communicationException)
        {
            FileLogging.LogException(communicationException);
            EMail.SendException(assembly, function, CreateExceptionMessageDetails(communicationException));

            string returnMessage = communicationErrorText;

            if (isExceptionDetailToBeSentToClient)
            {
                returnMessage = communicationException.Message;
            }

            return CreateValidationFault(assembly, function, "CommunicationException", returnMessage);
        }

        #endregion

        #region MQExceptionHandling (sender epost)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="function"></param>
        /// <param name="mqException"></param>
        /// <returns></returns>
        private static ValidationFault MQExceptionHandling(string assembly, string function, IBM.WMQ.MQException mqException)
        {
            FileLogging.LogException(mqException);
            EMail.SendException(assembly, function, CreateExceptionMessageDetails(mqException));

            string returnMessage = mqExceptionErrorText;

            if (isExceptionDetailToBeSentToClient)
            {
                returnMessage = CreateExceptionMessageDetails(mqException);
                string temp = string.Format("{2}Reason: {0}, ReasonCode: {1}", mqException.Reason.ToString(), mqException.ReasonCode.ToString(), Environment.NewLine);
                returnMessage += temp;
            }

            return CreateValidationFault(assembly, function, "MQException", returnMessage);
        }

        #endregion

        #region InvalidOperationExceptionHandling (sender ikke epost)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="function"></param>
        /// <param name="invalidOperationException"></param>
        private static ValidationFault InvalidOperationExceptionHandling(string assembly, string function, InvalidOperationException invalidOperationException)
        {
            FileLogging.LogException(invalidOperationException);


            EMail.SendException(assembly, function, CreateExceptionMessageDetails(invalidOperationException));

            string returnMessage = invalidOperationExceptionText;

            if (isExceptionDetailToBeSentToClient)
            {
                returnMessage = invalidOperationException.Message;
            }

            return CreateValidationFault(assembly, function, "InvalidOperationException", returnMessage);
        }

        #endregion

        #region FormatExceptionHandling (sender ikke epost)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="function"></param>
        /// <param name="formatException"></param>
        /// <returns></returns>
        private static ValidationFault FormatExceptionHandling(string assembly, string function, FormatException formatException)
        {
            FileLogging.LogException(formatException);

            string returnMessage = formatExceptionErrorText;

            if (isExceptionDetailToBeSentToClient)
            {
                returnMessage = formatException.Message;
            }

            return CreateValidationFault(assembly, function, "FormatException", returnMessage);
        }

        #endregion

        #region GeneralExceptionHandling (sender epost)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="function"></param>
        /// <param name="ex"></param>
        private static ValidationFault GeneralExceptionHandling(string assembly, string function, Exception exception)
        {
            FileLogging.LogException(exception);
            EMail.SendException(assembly, function, CreateExceptionMessageDetails(exception));

            string returnMessageDetails = exception.Message;
            EMail.SendException(assembly, function, returnMessageDetails);

            string returnMessage = generalApplicationErrorText;

            if (isExceptionDetailToBeSentToClient)
            {
                returnMessage = returnMessageDetails;
            }

            return CreateValidationFault(assembly, function, "GeneralException", returnMessage);
        }

        #endregion

        #region Local Utilities

        #region CreateExceptionMessageDetails

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string CreateExceptionMessageDetails(Exception exception)
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

        #region CreateValidationFault

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="function"></param>
        /// <param name="tag"></param>
        /// <param name="exceptionMessage"></param>
        /// <returns></returns>
        private static ValidationFault CreateValidationFault(string assembly, string function, string tag, string exceptionMessage)
        {
            ValidationFault validationFault = new ValidationFault();

            ValidationDetail validationDetail = new ValidationDetail(exceptionMessage, function, assembly);
            validationFault.Details.Add(validationDetail);
            return validationFault;
        }

        #endregion

        #endregion
    }
}
