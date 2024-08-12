#region Namespaces
using DataAccessAPI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.CustomException;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
#endregion

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        #region Variables
        private readonly ILogger<ConfigController> _logger;
        private static DateTime _CurrentReolTableName_CacheValidUntil = DateTime.MinValue;
        private static string _CurrentReolTableName_Cached = null;
        #endregion

        #region Properties

        public string CurrentReolTableName
        {
            get
            {
                // Cache current reol table name for one minute so that we don't get performance bottlenecks when fetching many individual reols and instantiating a new ReolTableController object for each reol.
                if ((DateTime.Now < _CurrentReolTableName_CacheValidUntil))
                    return _CurrentReolTableName_Cached;
                _CurrentReolTableName_Cached = GetConfigValue("CurrentReolTableName");
                _CurrentReolTableName_CacheValidUntil = DateTime.Now.AddSeconds(60);
                return _CurrentReolTableName_Cached;
            }
            set
            {
                SetConfigValue("CurrentReolTableName", value);
            }
        }

        public string PreviousReolTableName
        {
            get
            {
                try
                {
                    return GetConfigValue("PreviousReolTableName");
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                SetConfigValue("PreviousReolTableName", value);
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Paramaterized Constructor
        /// </summary>
        /// <param name="logger">Instance of Microsoft.Extensions.Logging</param>
        public ConfigController(ILogger<ConfigController> logger)
        {
            _logger = logger;
        }

        #endregion

        #region private Methods
        private Dictionary<string, string> _criteriaTextLookUp = null;


        /// <summary>
        /// Gets the setting.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="logError">if set to <c>true</c> [log error].</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Feil ved lesing av konfigurasjonsfil. Konfigurasjonsnøkkel " + key + " er ikke angitt.
        /// or
        /// error
        /// </exception>
        private string GetSetting(string key, bool logError = true, string defaultValue = "")
        {
            try
            {

                if (defaultValue == null || defaultValue == "")
                    throw new PumaException("Feil ved lesing av konfigurasjonsfil. Konfigurasjonsnøkkel " + key + " er ikke angitt.");
                else
                    return defaultValue;

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "error" + exception.Message);
                throw new PumaException("error" + exception.Message);
            }
        }

        #endregion
        #region Public Methods
        /// <summary>
        /// Get the Config value based on config key
        /// </summary>
        /// <param name="configKey">Config key to fetch Config value from Config table</param>
        /// <returns>Config Value</returns>
        [HttpGet("GetConfigValue", Name = nameof(GetConfigValue))]
        public string GetConfigValue(string configKey)
        {
            string result;
            try
            {
                _logger.BeginScope("Inside into GetConfigValue");

                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];
                npgsqlParameters[0] = new NpgsqlParameter("p_configkey", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                npgsqlParameters[0].Value = configKey;

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    result = dbhelper.ExecuteScalar<string>("kspu_db.getconfigvalue", CommandType.StoredProcedure, npgsqlParameters);
                }

                _logger.LogInformation("GetConfigValue is: {0}", result);

                _logger.LogDebug("Exiting from GetConfigValue");

            }
            catch (Exception exception)
            {
                _logger.LogError(exception,"Error in GetConfigValue: " + exception.Message);
                result = exception.Message;
            }

            return result;
        }

        /// <summary>
        /// Update the config value of table on the basis of config key
        /// </summary>
        /// <param name="configKey">Key of config table</param>
        /// <param name="configValue">Value of config table</param>
        [HttpPost("SetConfigValue", Name = nameof(SetConfigValue))]
        public void SetConfigValue(string configKey, string configValue)
        {
            try
            {
                _logger.BeginScope("Inside into SetConfigValue");

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

                using (PGDBHelper dbhelper = new PGDBHelper(ConfigSettings.GetConnectionString))
                {
                    dbhelper.ExecuteNonQuery("kspu_db.setconfigvalue", CommandType.StoredProcedure, npgsqlParameters);
                    result = Convert.ToInt32(npgsqlParameters[2].Value);
                    _logger.LogInformation(string.Format("SetConfigValue is {0}", result));
                }

                _logger.LogDebug("Exiting from SetConfigValue");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,"Error in SetConfigValue: " + exception.Message);
            }
        }

        //public void BuildSettingsCache()
        //{
        //    Dictionary<string, string> settings = new Dictionary<string, string>();
        //    XmlDocument configDoc = new XmlDocument();
        //    string dllPath = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(Config)).CodeBase);
        //    string configFileName = Path.Combine(dllPath, "KSPUConfig.xml");
        //    try
        //    {
        //        configDoc.Load(configFileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Could not load configuration file " + configFileName + Constants.vbCrLf + ex.Message, ex);
        //    }
        //    foreach (XmlNode settingNode in configDoc.SelectNodes("config/setting"))
        //        settings.Item[settingNode.Attributes("key").Value] = settingNode.Attributes("value").Value;
        //    _CachedSettings = settings;
        //}
        [HttpGet("GetDemografiCriteriaText", Name = nameof(GetDemografiCriteriaText))]
        public string GetDemografiCriteriaText(string criteria)
        {
            _logger.BeginScope("Inside into GetDemografiCriteriaText");
            if (_criteriaTextLookUp != null)
            {
                if (_criteriaTextLookUp.ContainsKey(criteria))
                    return criteria;
                else
                    return criteria;
            }

            var tempLoopUp = new Dictionary<string, string>();
            string guiConfig = GetSetting("demografi_GUI");

            foreach (string guiConfigPart in guiConfig.Split("|"))
            {
                string keyValueString = "default";
                if (keyValueString != "default")
                {
                    foreach (string keyValuePart in keyValueString.Split("|"))
                    {
                        string[] keyValuePartArray = keyValuePart.Split(";");
                        if (!tempLoopUp.ContainsKey(keyValuePartArray[0]))
                            tempLoopUp.Add(keyValuePartArray[0], keyValuePartArray[1]);
                    }
                }
            }

            _criteriaTextLookUp = tempLoopUp;

            return GetDemografiCriteriaText(criteria);
        }


        #endregion


    }
}

