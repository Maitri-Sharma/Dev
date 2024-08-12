//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Puma.Infrastructure.AppConfig
//{
//    /// <summary>
//    /// ConfigSetting
//    /// </summary>
//    public static class ConfigSetting
//    {
//        /// <summary>
//        /// The client identifier
//        /// </summary>
//        private static string _clientId = string.Empty;

//        /// <summary>
//        /// The client secret
//        /// </summary>
//        private static string _clientSecret = string.Empty;

//        /// <summary>
//        /// Gets the get client identifier.
//        /// </summary>
//        /// <value>
//        /// The get client identifier.
//        /// </value>
//        public static string GetClientId
//        {
//            get
//            {
//                if (string.IsNullOrWhiteSpace(_clientId))
//                    _clientId = new AppSettings().OEBSClientId;

//                return _clientId;
//            }
//        }

//        /// <summary>
//        /// Gets the get client secret.
//        /// </summary>
//        /// <value>
//        /// The get client secret.
//        /// </value>
//        public static string GetClientSecret
//        {
//            get
//            {
//                if (string.IsNullOrWhiteSpace(_clientSecret))
//                    _clientSecret = new AppSettings().OEBSClientSecret;

//                return _clientSecret;
//            }
//        }
//    }
//}
