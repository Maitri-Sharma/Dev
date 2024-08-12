using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.SQLDbHelper;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using Azure.Identity;
using Puma.DataLayer.BusinessEntity;

namespace Puma.Infrastructure.Repository.KspuDB
{
    public class AppSettingRepository : IAppSettingRepository
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<AppSettingRepository> _logger;

        public Dictionary<string, string> lstAppSettings = null;


        public AppSettingRepository(IConfiguration configuration, ILogger<AppSettingRepository> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetAppSettingValue(string appSettingKey, bool isFromAzureKeyVault = false)
        {
            _logger.LogDebug("Get Appsetting value for Key" + appSettingKey);
            //Check if Key Exists then get from List
            if (lstAppSettings?.Any() == true && (lstAppSettings.ContainsKey(appSettingKey.ToLower())))
            {

                return lstAppSettings[appSettingKey.ToLower()];
            }
            //If key is not there in DB and if not from azure key vault then return from Appsetting.json
            else
            {
                string result;
                if (!isFromAzureKeyVault)
                {
                    result = _configuration.GetSection(appSettingKey).Value;
                }
                //return value from azurekeyvalut
                else
                {
                    result = await GetkeyValutName(_configuration.GetSection(appSettingKey).Value);
                }

                if (lstAppSettings == null)
                {
                    lstAppSettings = new Dictionary<string, string>();
                }
                lstAppSettings.Add(appSettingKey.ToLower(), result);


                return result;
            }

        }


        public async Task<string> GetkeyValutName(string secretName)
        {
            SecretClientOptions options = new SecretClientOptions()
            {
                Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                     }
            };
            string keyvalutName = await GetAppSettingValue(AppSetting.KeyVaultName);
            string ManagedIdentityClientId = await GetAppSettingValue(AppSetting.ManagedIdentityClientId);

            var secretClient = new SecretClient(new Uri(string.Format("https://{0}.vault.azure.net/", keyvalutName)), new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = ManagedIdentityClientId }), options);

            KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);

            return secret.Value;
        }

        public async Task<string> GetConnectionString()
        {
            await Task.Run(() => { });
            string connectionstring = _configuration.GetSection("ConnectionStrings").GetSection("Puma_DB").Value;


            //connectionstring = connectionstring.Replace("{userid}", "puma_dev_admin");
            //connectionstring = connectionstring.Replace("{pswrd}", "z$mCX#IMsJ8@ykS2t8Ukz)xNAU!_h[x[");

            connectionstring = connectionstring.Replace("{userid}", await GetAppSettingValue(AppSetting.DBUserName, true), StringComparison.OrdinalIgnoreCase);
            connectionstring = connectionstring.Replace("{pswrd}", await GetAppSettingValue(AppSetting.DBPassword, true), StringComparison.OrdinalIgnoreCase);
            connectionstring = connectionstring.Replace("{dbserver}", await GetAppSettingValue(AppSetting.DBServerName, false), StringComparison.OrdinalIgnoreCase);
            //read connection string from app setting
            return connectionstring;
        }
    }
}
