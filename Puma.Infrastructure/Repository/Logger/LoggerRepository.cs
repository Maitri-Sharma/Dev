using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.Logger;
using Puma.Infrastructure.Repository.KspuDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Repository.Logger
{
    public class LoggerRepository : KsupDBGenericRepository<utvalglist>, ILoggerRepository
    {
        public readonly string connctionstring;

        /// <summary>
        /// AppSetting repository
        /// </summary>
        private readonly IAppSettingRepository _appSettingRepository;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appSettingRepository"></param>
        public LoggerRepository(KspuDBContext context, IAppSettingRepository appSettingRepository) : base(context)
        {
            connctionstring = _context.Database.GetConnectionString();
            _appSettingRepository = appSettingRepository ?? throw new ArgumentNullException(nameof(appSettingRepository));
        }

        /// <summary>
        /// Add debugs to DB
        /// </summary>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task LogDebug(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string fileName = "")
        {
            await AddLogsToDB(LogLevel.Debug.ToString(), message, fileName, methodName);


        }

        /// <summary>
        /// Add Information to DB
        /// </summary>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task LogInformation(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string fileName = "")
        {
            await AddLogsToDB(LogLevel.Information.ToString(), message, fileName, methodName);
        }

        /// <summary>
        /// Add warning to DB
        /// </summary>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task LogWarning(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string fileName = "")
        {
            await AddLogsToDB(LogLevel.Warning.ToString(), message, fileName, methodName);

        }

        /// <summary>
        /// Add Warning to DB
        /// </summary>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task LogError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string fileName = "")
        {
            await AddLogsToDB(LogLevel.Error.ToString(), message, fileName, methodName);
        }

        /// <summary>
        /// Add Error with exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="methodName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task LogError(string message, Exception ex, [CallerMemberName] string methodName = "", [CallerFilePath] string fileName = "")
        {
            //If statck trace found then save that data.
            if (!string.IsNullOrWhiteSpace(ex?.StackTrace))
                await AddLogsToDB(LogLevel.Error.ToString(), message, fileName, methodName, ex?.StackTrace);
            //Else save exception message
            else
                await AddLogsToDB(LogLevel.Error.ToString(), message, fileName, methodName, ex?.Message);
        }

        /// <summary>
        /// Add Logs to DB
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="fileName"></param>
        /// <param name="methodName"></param>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
        public async Task AddLogsToDB(string logLevel, string message, string fileName, string methodName, string stackTrace = "")
        {
            await Task.Run(() => { });

            object result;

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[6];

            npgsqlParameters[0] = new NpgsqlParameter("p_loglevel", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[0].Value = logLevel;

            npgsqlParameters[1] = new NpgsqlParameter("p_logdate", NpgsqlTypes.NpgsqlDbType.TimestampTz);
            npgsqlParameters[1].Value = System.DateTime.Now;

            npgsqlParameters[2] = new NpgsqlParameter("p_logfilename", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[2].Value = Path.GetFileName(fileName);

            npgsqlParameters[3] = new NpgsqlParameter("p_logmethodname", NpgsqlTypes.NpgsqlDbType.Varchar);
            npgsqlParameters[3].Value = methodName;

            npgsqlParameters[4] = new NpgsqlParameter("p_logmessage", NpgsqlTypes.NpgsqlDbType.Text);
            npgsqlParameters[4].Value = message;

            npgsqlParameters[5] = new NpgsqlParameter("p_errorstacktrace", NpgsqlTypes.NpgsqlDbType.Text);
            npgsqlParameters[5].Value = stackTrace;


            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {
                try
                {
                    result = dbhelper.ExecuteNonQuery("kspu_db.insertpumalogs", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch
                {
                }
            }


        }

        /// <summary>
        /// Delete Puma logs from DB
        /// </summary>
        /// <returns></returns>
        public async Task DeleteLogs()
        {
            await Task.Run(() => { });

            int pumaLogCleanUpDays = -1 * Convert.ToInt32(_appSettingRepository.GetAppSettingValue(AppSetting.PumaLogCleanUpDays).Result ?? "7");

            NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[1];

            npgsqlParameters[0] = new NpgsqlParameter("p_logdate", NpgsqlTypes.NpgsqlDbType.TimestampTz);
            npgsqlParameters[0].Value = System.DateTime.Now.AddDays(pumaLogCleanUpDays);



            using (SQLDbHelper.PGDBHelper dbhelper = new SQLDbHelper.PGDBHelper(connctionstring))
            {
                try
                {
                    dbhelper.ExecuteNonQuery("kspu_db.deletepumalogs", CommandType.StoredProcedure, npgsqlParameters);
                }
                catch
                {
                }
            }


        }
    }
}
