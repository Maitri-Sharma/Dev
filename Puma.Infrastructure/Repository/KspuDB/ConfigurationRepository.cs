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

namespace Puma.Infrastructure.Repository.KspuDB
{
    public class ConfigurationRepository : IConfigurationRepository
    {

        public string Connctionstring;

        private readonly ILogger<ConfigurationRepository> _logger;
        IServiceScopeFactory _services;
        public Dictionary<string, string> lstConfig = null;
        public ConfigurationRepository(ILogger<ConfigurationRepository> logger, IServiceScopeFactory services)
        {
            _logger = logger;
            _services = services;

        }




        public Task<string> GetConfigValue(string configKey)
        {
            //First check if list has that key or not, if list has key then directly return it
            if (lstConfig?.Any() == true && (lstConfig.ContainsKey(configKey.ToLower())))
            {

                return Task.FromResult(lstConfig[configKey.ToLower()]);
            }
            else
            {
                string result;
                try
                {
                    //if connection string is not there get connection string
                    if (string.IsNullOrWhiteSpace(Connctionstring))
                    {
                        using (var scope = _services.CreateScope())
                        {
                            var db = scope.ServiceProvider.GetRequiredService<KspuDBContext>();

                            Connctionstring = db.Database.GetConnectionString();

                        }
                    }

                    ////If Currrent reol table name fetched from database once for day then no need to fetch again and again
                    //if (string.Compare(Constants.CurrentReolTableName.Trim(), configKey.Trim(),true) ==0)
                    //{
                    //    if (!string.IsNullOrWhiteSpace(currentReolTableName) && (currentReolTableName_LastFetchedTime.Date == DateTime.Today.Date))
                    //    {
                    //        return Task.FromResult(currentReolTableName);
                    //    }
                    //}

                    _logger.LogDebug("Preparing the data for GetConfigValue");

                    NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                    npgsqlParameters[0] = new NpgsqlParameter("p_configkey", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                    npgsqlParameters[0].Value = configKey;

                    using (PGDBHelper dbhelper = new PGDBHelper(Connctionstring))
                    {
                        result = dbhelper.ExecuteScalar<string>("kspu_db.getconfigvalue", CommandType.StoredProcedure, npgsqlParameters);
                    }

                    //result = "kspu_gdb.norway_reol20220903";
                    _logger.LogInformation("GetConfigValue is: {0}", result);

                    _logger.LogDebug("Exiting from GetConfigValue");

                    ////If Currrent reol table name fetched from database once for day then set in variable and when was last fetched
                    //if (string.Compare(Constants.CurrentReolTableName.Trim(), configKey.Trim(), true) == 0)
                    //{
                    //    currentReolTableName = result;
                    //    currentReolTableName_LastFetchedTime = DateTime.Today;
                    //}

                    //Once data is fetched add it to list
                    if (lstConfig == null)
                    {
                        lstConfig = new Dictionary<string, string>();
                    }
                    lstConfig.Add(configKey.ToLower(), result);

                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error in GetConfigValue: " + exception.Message);
                    result = exception.Message;
                }

                return Task.FromResult(result);
            }
        }

        public async Task SetConfigValue(string configKey, string configValue)
        {
            await Task.Run(() => { });

            try
            {
                _logger.LogDebug("Preparing the data for SetConfigValue");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[3];
                int result;

                #region Parameter assignement

                npgsqlParameters[0] = new NpgsqlParameter("p_configkey", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[0].Value = configKey;

                npgsqlParameters[1] = new NpgsqlParameter("p_configvalue", NpgsqlTypes.NpgsqlDbType.Varchar);
                npgsqlParameters[1].Value = configValue;

                npgsqlParameters[2] = new NpgsqlParameter("rows_affected", NpgsqlTypes.NpgsqlDbType.Integer);
                npgsqlParameters[2].Direction = ParameterDirection.Output;

                #endregion

                using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(Connctionstring))
                {
                    dbhelper.ExecuteNonQuery("kspu_db.setconfigvalue", CommandType.StoredProcedure, npgsqlParameters);
                    result = Convert.ToInt32(npgsqlParameters[2].Value);
                    _logger.LogInformation(string.Format("SetConfigValue is: {0}", result));
                }

                _logger.LogDebug("Exiting from SetConfigValue");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in SetConfigValue: " + exception.Message);
            }
        }
    }
}
