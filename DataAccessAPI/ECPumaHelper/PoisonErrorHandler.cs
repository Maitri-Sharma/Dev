using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Dispatcher;
using Puma.Shared;
using static Puma.Shared.PumaEnum;
using log4net.Core;

namespace DataAccessAPI.ECPumaHelper
{
    /// <summary>
    /// Feilhåndtering for netMsmqBinding.
    /// Mer info: http://msdn2.microsoft.com/en-us/library/ms751472.aspx
    /// </summary>
    class PoisonErrorHandler : IErrorHandler 
    {
        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, Exception e)
        {
            throw new NotImplementedException();
        }

        public void Error(string message)
        {
            throw new NotImplementedException();
        }
        #region IErrorHandler Members

        /// <summary>
        /// Should Handle poison message exception by moving the offending message out of the way for regular processing to go on.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool HandleError(Exception error)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
            // no-op -we are not interested in this.
        }

        #endregion
    }
}
