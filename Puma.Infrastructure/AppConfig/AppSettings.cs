//#region Namespace
//using System;
//using System.IO;
//using Azure.Core;
//using Azure.Identity;
//using Azure.Security.KeyVault.Secrets;
//using Microsoft.Extensions.Configuration;
//#endregion

//namespace Puma.Infrastructure.AppConfig
//{
//    public class AppSettings
//    {
//        #region Variables
//        private readonly string _connectionString = string.Empty;
//        private readonly string _ec_api_url = string.Empty;
//        private readonly string _ec_username = string.Empty;
//        private readonly string _ec_pwd = string.Empty;
//        private readonly string _managed_identity_name = string.Empty;
//        private readonly string _managed_identity_client_id = string.Empty;
//        private readonly string _key_vault_name = string.Empty;
//        private readonly bool _isToAuthenticate = false;
//        private readonly string _OEBSClientSecret = string.Empty;
//        private readonly string _OEBSClientId = string.Empty;
//        private readonly string _JWTSecret = string.Empty;
//        private readonly string _arcadminusername = string.Empty;
//        private readonly string _arcadminuserpwd = string.Empty;

//        #endregion

//        #region Properties
//        public string ConnectionString
//        {
//            get => _connectionString;
//        }

//        public string EC_API_URL
//        {
//            get => _ec_api_url;
//        }

//        public string EC_USERNAME
//        {
//            get => _ec_username;
//        }

//        public string EC_PWD
//        {
//            get => _ec_pwd;
//        }

//        public bool isToAuthenticate
//        {
//            get => _isToAuthenticate;

//        }

//        public string OEBSClientSecret
//        {
//            get => _OEBSClientSecret;
//        }

//        public string OEBSClientId
//        {
//            get => _OEBSClientId;
//        }

//        public string JWTSecret
//        {
//            get => _JWTSecret;
//        }
//        public string ArcAdminUserName
//        {
//            get => _arcadminusername;
//        }

//        public string ArcAdminUserPwd
//        {
//            get => _arcadminuserpwd;
//        }

//        #endregion


//        #region Constructor
//        /// <summary>
//        /// Default constructor
//        /// </summary>
//        public AppSettings()
//        {
//            if (string.IsNullOrWhiteSpace(_connectionString))
//            {
//                var configurationBuilder = new ConfigurationBuilder();
//                //Get appsetting file
//                string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

//                configurationBuilder.AddJsonFile(path, false);

//                var root = configurationBuilder.Build();

//                //read Key valut name string from app setting
//                _key_vault_name = root.GetSection("Key-Vault-Name").Value;

//                //read managed identity name string from app setting
//                _managed_identity_name = root.GetSection("Managed-Identity-Name").Value;

//                //read managed identity client ID string from app setting
//                _managed_identity_client_id = root.GetSection("Managed-Identity-Client-ID").Value;

//                string connectionstring = string.Empty;

//                connectionstring = root.GetSection("ConnectionStrings").GetSection("Puma_DB").Value;
//                connectionstring = connectionstring.Replace("{userid}", "puma_admin");
//                connectionstring = connectionstring.Replace("{pswrd}", "yEWnkgnwXDeNQ113:]q%>N*[oqDU6p1I");

//                //connectionstring = connectionstring.Replace("{userid}", GetkeyValutName(root.GetSection("Key-Vault-Secret-DBName").Value), StringComparison.OrdinalIgnoreCase);
//                //connectionstring = connectionstring.Replace("{pswrd}", GetkeyValutName(root.GetSection("Key-Vault-Secret-DBPswrd").Value), StringComparison.OrdinalIgnoreCase);
//                connectionstring = connectionstring.Replace("{dbserver}", root.GetSection("DBServer").Value, StringComparison.OrdinalIgnoreCase);
//                //read connection string from app setting
//                _connectionString = connectionstring;


//                _ec_api_url = root.GetSection("EC_API_URL").Value;
//                _ec_username = root.GetSection("EC_USERNAME").Value;
//              //  _ec_pwd = GetkeyValutName(root.GetSection("Key-Vault-Secret-ECPswrd").Value);
//                _isToAuthenticate = Convert.ToBoolean(root.GetSection("IsAuthenticationApplicable").Value);
//                //ecre_OEBSClientSt = GetkeyValutName(root.GetSection("Client_Secret").Value);
//                //_OEBSClientId = GetkeyValutName(root.GetSection("Client_Id").Value);
//                //_JWTSecret = GetkeyValutName(root.GetSection("JWTSecret").Value);
//                //_arcadminusername = GetkeyValutName(root.GetSection("Key-Vault-arcgisadminUserName").Value);
//                //_arcadminuserpwd = GetkeyValutName(root.GetSection("Key-Vault-arcgisadminUserPwd").Value);
//            }
//        }
//        #endregion

//        #region Private method
//        /// <summary>
//        /// This method is used to fetch secret value from keyvault
//        /// </summary>
//        /// <param name="secretName">Name of secret</param>
//        /// <returns>value of secret key</returns>
//        private string GetkeyValutName(string secretName)
//        {
//            SecretClientOptions options = new SecretClientOptions()
//            {
//                Retry =
//                    {
//                        Delay= TimeSpan.FromSeconds(2),
//                        MaxDelay = TimeSpan.FromSeconds(16),
//                        MaxRetries = 5,
//                        Mode = RetryMode.Exponential
//                     }
//            };

//            var secretClient = new SecretClient(new Uri(string.Format("https://{0}.vault.azure.net/", _key_vault_name)), new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = _managed_identity_client_id }), options);

//            KeyVaultSecret secret = secretClient.GetSecret(secretName);

//            return secret.Value;

//        }
//        #endregion
//    }
//}
